using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using TicketMaster.Data;
using TicketMaster.Models;
using TicketMaster.Models.DTOs;

[Authorize]
public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<HomeController> _logger;

    public HomeController(ApplicationDbContext context, IWebHostEnvironment env, ILogger<HomeController> logger)
    {
        _context = context;
        _env = env;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Index()
    {
        if (TempData["ImageUrl"] != null)
        {
            ViewBag.ImagePath = TempData["ImageUrl"].ToString();

            ViewBag.GeneratedCode = TempData["GeneratedCode"] != null
                ? TempData["GeneratedCode"].ToString()
                : null;
        }

        // Pass success or error messages to ViewBag
        if (TempData["SuccessMessage"] != null)
        {
            ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
        }
        else if (TempData["ErrorMessage"] != null)
        {
            ViewBag.ErrorMessage = TempData["ErrorMessage"].ToString();
        }

        var imageDetails = _context.InvitationsWithPoint
            .Where(i => i.InvitationType == "True" && i.IsActive)
            .Select(i => new ImagesDTO
            {
                Value = i.Issued,
                Path = i.ImagePath
            })
            .ToList() ?? new List<ImagesDTO>();


        ViewBag.IssuedCodes = _context.Issueds.Where(x => x.Status == "True").ToList();

        // Load active events for event selection
        ViewBag.Events = _context.Events
            .Where(e => e.IsActive && (e.MaxTickets - e.CreatedTicketsCount) > 0)
            .OrderByDescending(e => e.EventDate)
            .ToList();

        InvitationDTO invitationDTO = new InvitationDTO
        {
            Id = 0,
            InviterName = "",
            InvitationType = "",
            Issued = "",
            UniqCode = "",
            Images = imageDetails
        };

        return View(invitationDTO);
    }


    [HttpPost]
    public IActionResult Index(InvitationDTO model)
    {
        if (model == null)
        {
            TempData["ErrorMessage"] = "Invalid request data.";
            return RedirectToAction("Index");
        }

        if (string.IsNullOrEmpty(model.InvitationType))
        {
            TempData["ErrorMessage"] = "Invitation type is required.";
            return RedirectToAction("Index");
        }

        string code = GenerateRandomCode(model.Issued);

        string imagePath = GenerateAndSaveImage(model.InvitationType, code);

        if (string.IsNullOrEmpty(imagePath))
        {
            _logger.LogWarning("Failed to generate invitation image for InvitationType: {InvitationType}, Code: {Code}", model.InvitationType, code);
            TempData["ErrorMessage"] = "Failed to generate the invitation image. Please ensure application settings are configured and the template image exists.";
            return RedirectToAction("Index");
        }

        try
        {
            // Validate event capacity if event is selected
            if (model.EventId.HasValue)
            {
                var evt = _context.Events.Find(model.EventId.Value);
                if (evt == null)
                {
                    TempData["ErrorMessage"] = "Selected event not found.";
                    return RedirectToAction("Index");
                }

                int remainingTickets = evt.MaxTickets - evt.CreatedTicketsCount;
                if (remainingTickets <= 0)
                {
                    TempData["ErrorMessage"] = $"Cannot create ticket. Event '{evt.EventName}' has reached maximum capacity.";
                    return RedirectToAction("Index");
                }
            }

            Invitation invitation = new Invitation
            {
                InviterName = model.InviterName,
                InvitationType = model.InvitationType,
                Issued = model.Issued,
                UniqCode = code,
                ImagePath = imagePath,
                CreatedAt = DateTime.Now,
                EventId = model.EventId
            };

            _context.Invitations.Add(invitation);

            // Update event ticket count if event is selected
            if (model.EventId.HasValue)
            {
                var evt = _context.Events.Find(model.EventId.Value);
                if (evt != null)
                {
                    evt.CreatedTicketsCount += 1;
                    _context.Events.Update(evt);
                }
            }

            _context.SaveChanges();

            TempData["SuccessMessage"] = "Invitation saved successfully!";
            TempData["ImageUrl"] = imagePath;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while saving invitation. InviterName: {InviterName}, InvitationType: {InvitationType}, Issued: {Issued}",
                model.InviterName, model.InvitationType, model.Issued);
            TempData["ErrorMessage"] = $"An error occurred while saving the invitation: {ex.Message}";
        }

        TempData["GeneratedCode"] = code;

        return RedirectToAction("Index");
    }


    private string? GenerateAndSaveImage(string invitationType, string code)
    {
        var imageDetail = _context.InvitationsWithPoint
            .Where(i => i.Issued == invitationType)
            .Select(i => new
            {
                i.ImagePath,
                i.CoordinateX,
                i.CoordinateY
            })
            .FirstOrDefault();

        if (imageDetail == null)
        {
            _logger.LogWarning("No template image found for InvitationType: {InvitationType}", invitationType);
            return null;
        }

        try
        {
            var originalFilePath = Path.Combine(_env.WebRootPath, imageDetail.ImagePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

            var existingSettings = _context.ApplicationSettings.FirstOrDefault();

            if (existingSettings == null)
            {
                _logger.LogError("ApplicationSettings not found in database. Please configure application settings.");
                return null;
            }

            float fontSize = ParseFontSize(existingSettings.FrontSize);

            if (!System.IO.File.Exists(originalFilePath))
            {
                _logger.LogError("Template image file not found at path: {FilePath}", originalFilePath);
                return null;
            }

            // Define the folder to save new generated images
            var generateImageFolder = Path.Combine(_env.WebRootPath, "generate_image");

            // Create folder if it doesn't exist
            if (!Directory.Exists(generateImageFolder))
                Directory.CreateDirectory(generateImageFolder);

            // Create unique new filename with code appended
            string newFileName = Path.GetFileNameWithoutExtension(originalFilePath) + $"_{code}" + Path.GetExtension(originalFilePath);
            var newFilePath = Path.Combine(generateImageFolder, newFileName);

            using (var bitmap = new System.Drawing.Bitmap(originalFilePath))
            using (var graphics = System.Drawing.Graphics.FromImage(bitmap))
            {
                var font = new System.Drawing.Font(existingSettings.FontFamily, fontSize, System.Drawing.FontStyle.Bold);
                var brush = System.Drawing.Brushes.Black;

                int x = Math.Clamp(imageDetail.CoordinateX, 0, bitmap.Width - 1);
                int y = Math.Clamp(imageDetail.CoordinateY, 0, bitmap.Height - 1);

                graphics.DrawString(code, font, brush, new System.Drawing.Point(x, y));

                // Save new image in generate_image folder
                bitmap.Save(newFilePath);
            }

            // Return relative URL to access the new image via web
            string relativeUrl = "/generate_image/" + newFileName;
            return relativeUrl;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating and saving image for InvitationType: {InvitationType}, Code: {Code}", invitationType, code);
            return null;
        }
    }

    private float ParseFontSize(string sizeValue)
    {
        if (string.IsNullOrWhiteSpace(sizeValue))
            return 12f; // Default font size if empty or null

        // Remove "px" if present and convert to float
        if (sizeValue.EndsWith("px", StringComparison.OrdinalIgnoreCase))
            sizeValue = sizeValue.Replace("px", "").Trim();

        if (float.TryParse(sizeValue, out float result))
            return result;

        return 12f; // Fallback default if parsing fails
    }


    public IActionResult DownloadInvitationImage(string invitationType, string code)
    {
        var imageUrl = GenerateAndSaveImage(invitationType, code);
        if (imageUrl == null)
            return NotFound("Image not found or error occurred.");

        // Return the URL so the frontend can download or show the image
        return Json(new { ImageUrl = imageUrl });
    }

    [HttpGet]
    public IActionResult GetNextCode(string prefix)
    {
        if (string.IsNullOrEmpty(prefix))
            return Json(new { code = "" });

        var nextCode = GenerateRandomCode(prefix);
        return Json(new { code = nextCode });
    }

    private string GenerateRandomCode(string prefix, int numericLength = 6)
    {
        if (numericLength <= 0)
            throw new ArgumentException("Numeric length must be greater than zero.");

        var rng = new Random();

        string GenerateRandomDigits(int length)
        {
            const string digits = "0123456789";
            var buffer = new char[length];
            for (int i = 0; i < length; i++)
            {
                buffer[i] = digits[rng.Next(digits.Length)];
            }
            return new string(buffer);
        }

        string newCode;
        bool exists;

        do
        {
            var randomPart = GenerateRandomDigits(numericLength);
            newCode = $"{prefix}-{randomPart}";

            exists = _context.Invitations.Any(i => i.UniqCode == newCode);

        } while (exists);

        return newCode;
    }

    public IActionResult IssuedList()
    {
        var invitations = _context.Invitations
            .Where(i => !i.IsDeleted)
            .OrderByDescending(i => i.CreatedAt)
            .ToList();
        return View(invitations);
    }

    [HttpPost]
    public IActionResult DeleteInvitation(int id)
    {
        try
        {
            var invitation = _context.Invitations.FirstOrDefault(i => i.Id == id);
            if (invitation == null)
            {
                TempData["ErrorMessage"] = "Invitation not found.";
                return RedirectToAction("IssuedList");
            }

            // Soft delete - don't delete the image file
            invitation.IsDeleted = true;
            invitation.DeletedAt = DateTime.UtcNow;
            _context.SaveChanges();

            _logger.LogInformation("Invitation {Id} ({UniqCode}) soft deleted successfully", id, invitation.UniqCode);
            TempData["SuccessMessage"] = $"Invitation '{invitation.UniqCode}' deleted successfully.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting invitation {Id}", id);
            TempData["ErrorMessage"] = "An error occurred while deleting the invitation.";
        }

        return RedirectToAction("IssuedList");
    }

    public IActionResult DeletedInvitations()
    {
        var deletedInvitations = _context.Invitations
            .Where(i => i.IsDeleted)
            .OrderByDescending(i => i.DeletedAt)
            .ToList();
        return View(deletedInvitations);
    }

    [HttpPost]
    public IActionResult RestoreInvitation(int id)
    {
        try
        {
            var invitation = _context.Invitations.FirstOrDefault(i => i.Id == id);
            if (invitation == null)
            {
                TempData["ErrorMessage"] = "Invitation not found.";
                return RedirectToAction("DeletedInvitations");
            }

            invitation.IsDeleted = false;
            invitation.DeletedAt = null;
            _context.SaveChanges();

            _logger.LogInformation("Invitation {Id} ({UniqCode}) restored successfully", id, invitation.UniqCode);
            TempData["SuccessMessage"] = $"Invitation '{invitation.UniqCode}' restored successfully.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restoring invitation {Id}", id);
            TempData["ErrorMessage"] = "An error occurred while restoring the invitation.";
        }

        return RedirectToAction("DeletedInvitations");
    }

    [HttpPost]
    public IActionResult PermanentDeleteInvitation(int id)
    {
        try
        {
            var invitation = _context.Invitations.FirstOrDefault(i => i.Id == id);
            if (invitation == null)
            {
                TempData["ErrorMessage"] = "Invitation not found.";
                return RedirectToAction("DeletedInvitations");
            }

            // Permanent delete - remove from database but still don't delete the image file
            _context.Invitations.Remove(invitation);
            _context.SaveChanges();

            _logger.LogInformation("Invitation {Id} ({UniqCode}) permanently deleted", id, invitation.UniqCode);
            TempData["SuccessMessage"] = "Invitation permanently deleted.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error permanently deleting invitation {Id}", id);
            TempData["ErrorMessage"] = "An error occurred while permanently deleting the invitation.";
        }

        return RedirectToAction("DeletedInvitations");
    }

    public IActionResult SelectPoint()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> SelectPoint(IFormFile ImageUpload, int CoordinateX, int CoordinateY)
    {

        if (ImageUpload == null || ImageUpload.Length == 0)
        {
            ModelState.AddModelError("ImageUpload", "Please upload an image.");
            return View();
        }

        if (!ModelState.IsValid)
            return View();

        // Ensure uploads folder exists
        var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        // Generate unique filename to avoid conflicts
        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageUpload.FileName);
        var filePath = Path.Combine(uploadsFolder, fileName);

        // Save uploaded image to disk
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await ImageUpload.CopyToAsync(stream);
        }

        int imgeCount = _context.InvitationsWithPoint.Count();

        // Generate unique code for the invitation
        string issuedCode = $"INV-{imgeCount + 1:D4}";

        // Save the invitation data in database
        var InvitationWithPoint = new InvitationWithPoint
        {
            Issued = issuedCode,
            ImagePath = "/uploads/" + fileName,
            CoordinateX = CoordinateX,
            CoordinateY = CoordinateY,
            InvitationType = "True",
            IsActive = true
        };

        _context.InvitationsWithPoint.Add(InvitationWithPoint);
        await _context.SaveChangesAsync();

        ViewBag.Message = "Invitation saved successfully!";
        return View();
    }

    public IActionResult ManageDesing()
    {
        List<InvitationWithPoint> invitations = _context.InvitationsWithPoint.ToList();

        if (TempData["Message"] != null)
        {
            ViewBag.Message = TempData["Message"].ToString();
        }

        return View(invitations);
    }


    // GET: /Home/Details/5
    [HttpGet]
    public IActionResult Details(int id)
    {
        // Fetch the invitation by id
        var invitation = _context.Invitations
            .FirstOrDefault(i => i.Id == id);

        if (invitation == null)
        {
            TempData["ErrorMessage"] = "Invitation not found.";
            return RedirectToAction("Index");
        }

        // You can create a ViewModel or just pass the invitation for display
        return View(invitation);
    }

    // GET: /Home/Download/5
    [HttpGet]
    public IActionResult Download(int id)
    {
        var invitation = _context.Invitations.FirstOrDefault(i => i.Id == id);
        if (invitation == null)
        {
            TempData["ErrorMessage"] = "Invitation not found.";
            return RedirectToAction("Index");
        }

        string imagePath = invitation.ImagePath;
        if (string.IsNullOrEmpty(imagePath))
        {
            TempData["ErrorMessage"] = "No image available to download.";
            return RedirectToAction("Index");
        }

        var physicalPath = Path.Combine(_env.WebRootPath, imagePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        if (!System.IO.File.Exists(physicalPath))
        {
            TempData["ErrorMessage"] = "Image file not found.";
            return RedirectToAction("Index");
        }

        // Load image and draw watermark
        using (var image = new Bitmap(physicalPath))
        using (var graphics = Graphics.FromImage(image))
        {
            // Configure watermark text style
            var font = new Font("Arial", 80, FontStyle.Bold, GraphicsUnit.Pixel);
            var color = System.Drawing.Color.FromArgb(128, System.Drawing.Color.White); // semi-transparent red
            var brush = new SolidBrush(color);

            // Save to memory stream for returning
            using (var ms = new MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                string fileName = $"{invitation.UniqCode ?? "Invitation"}.png";
                return File(ms.ToArray(), "image/png", fileName);
            }
        }
    }


    [HttpGet]
    public IActionResult ExportExcel()
    {
        var invitations = _context.Invitations.ToList();

        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Invitations");

            // Header row
            worksheet.Cell(1, 1).Value = "ID";
            worksheet.Cell(1, 2).Value = "Inviter Name";
            worksheet.Cell(1, 3).Value = "Issued Code";
            worksheet.Cell(1, 4).Value = "Invitation Type";
            worksheet.Cell(1, 5).Value = "Unique Code";
            worksheet.Cell(1, 6).Value = "Create At";


            // Data rows
            for (int i = 0; i < invitations.Count; i++)
            {
                var inv = invitations[i];
                worksheet.Cell(i + 2, 1).Value = inv.Id;
                worksheet.Cell(i + 2, 2).Value = inv.InviterName;
                worksheet.Cell(i + 2, 3).Value = inv.Issued;
                worksheet.Cell(i + 2, 4).Value = inv.InvitationType;
                worksheet.Cell(i + 2, 5).Value = inv.UniqCode;
                worksheet.Cell(i + 2, 6).Value = inv.CreatedAt;
            }

            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                stream.Position = 0;
                string fileName = $"Invitations_{DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss")}.xlsx";
                return File(stream.ToArray(),
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            fileName);
            }
        }
    }

    public IActionResult DownloadSqliteBackup()
    {
        var originalFilePath = Path.Combine(Directory.GetCurrentDirectory(), "invitations.db");
        var tempFilePath = Path.Combine(Path.GetTempPath(), $"invitations_backup_{DateTime.Now:yyyyMMdd_HHmmss}.db");

        if (!System.IO.File.Exists(originalFilePath))
            return NotFound("Backup file not found.");

        try
        {
            // Copy the DB file to temp location
            System.IO.File.Copy(originalFilePath, tempFilePath, overwrite: true);

            var fileBytes = System.IO.File.ReadAllBytes(tempFilePath);
            var fileName = $"SqliteBackup_{DateTime.Now:yyyyMMdd_HHmmss}.db";

            // Optionally delete the temp file after reading (can also delete asynchronously later)
            System.IO.File.Delete(tempFilePath);

            return File(fileBytes, "application/x-sqlite3", fileName);
        }
        catch (Exception ex)
        {
            // Log the error
            return StatusCode(500, $"Error creating backup: {ex.Message}");
        }
    }

    public IActionResult SaveSettings()
    {
        var existingSettings = _context.ApplicationSettings.FirstOrDefault();

        var appSetting = existingSettings ?? new ApplicationSetting
        {
            ColorCode = "#000000",
            FrontSize = "14px",
            FontFamily = "Arial"
        };
        return View(appSetting);
    }

    [HttpPost]
    public IActionResult SaveSettings(ApplicationSetting model)
    {
        if (ModelState.IsValid)
        {

            // Check if settings already exist
            var existingSettings = _context.ApplicationSettings.FirstOrDefault();

            if (existingSettings != null)
            {
                // Update existing settings
                existingSettings.ColorCode = model.ColorCode;
                existingSettings.FrontSize = model.FrontSize;
                existingSettings.FontFamily = model.FontFamily;
            }
            else
            {
                // Create new settings
                var newSettings = new ApplicationSetting
                {
                    ColorCode = model.ColorCode,
                    FrontSize = model.FrontSize,
                    FontFamily = model.FontFamily
                };
                _context.ApplicationSettings.Add(newSettings);
            }

            // Save changes to the database
            _context.SaveChanges();

            TempData["Success"] = "Settings saved successfully!";
            return RedirectToAction("SaveSettings");
        }

        return View(model);
    }

    // Your existing controller methods - exactly as you have them working:

    public IActionResult CreateIssuer()
    {
        try
        {
            var issuedList = _context.Issueds
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            return View(issuedList);
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = "An error occurred while loading the records.";
            return View(new List<Issued>());
        }
    }

    [HttpPost]
    public IActionResult CreateIssuer(Issued model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                // Check if UserCode already exists
                var existingCode = _context.Issueds
                    .FirstOrDefault(x => x.UserCode == model.UserCode);

                if (existingCode != null)
                {
                    ViewBag.ErrorMessage = $"User code '{model.UserCode}' already exists. Please use a different code.";
                    return RedirectToAction("CreateIssuer");
                }

                model.CreatedAt = DateTime.UtcNow;
                _context.Issueds.Add(model);
                _context.SaveChanges();

                ViewBag.SuccessMessage = $"Successfully added '{model.Name}' with code '{model.UserCode}'.";
            }
            else
            {
                ViewBag.ErrorMessage = "Please check the form data and try again.";
            }
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = "An error occurred while creating the record. Please try again.";
        }

        return RedirectToAction("CreateIssuer");
    }

    [HttpPost]
    public IActionResult Update(Issued model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var existingRecord = _context.Issueds
                    .FirstOrDefault(x => x.UserCode == model.UserCode);

                if (existingRecord != null)
                {
                    existingRecord.Name = model.Name;
                    existingRecord.Status = model.Status;
                    // Don't update UserCode since it's the primary key
                    // Don't update CreatedAt to preserve original creation time

                    _context.Issueds.Update(existingRecord);
                    _context.SaveChanges();

                    ViewBag.SuccessMessage = $"Successfully updated '{model.Name}'.";
                }
                else
                {
                    ViewBag.ErrorMessage = "Record not found.";
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Please check the form data and try again.";
            }
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = "An error occurred while updating the record. Please try again.";
        }

        return RedirectToAction("CreateIssuer");
    }

    [HttpPost]
    public IActionResult Delete(string userCode)
    {
        try
        {
            var record = _context.Issueds
                .FirstOrDefault(x => x.UserCode == userCode);

            if (record != null)
            {
                _context.Issueds.Remove(record);
                _context.SaveChanges();

                ViewBag.SuccessMessage = $"Successfully deleted '{record.Name}'.";
            }
            else
            {
                ViewBag.ErrorMessage = "Record not found.";
            }
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = "An error occurred while deleting the record. Please try again.";
        }

        return RedirectToAction("CreateIssuer");
    }

    public IActionResult CheckUserCodeExists(string userCode, string? excludeUserCode = null)
    {
        try
        {
            var query = _context.Issueds.AsQueryable();

            // Check for the same user code
            query = query.Where(x => x.UserCode == userCode);

            // Optionally exclude a specific UserCode
            if (!string.IsNullOrEmpty(excludeUserCode))
            {
                query = query.Where(x => x.UserCode != excludeUserCode);
            }

            bool exists = query.Any();
            return Json(new { exists });
        }
        catch
        {
            return Json(new { exists = false });
        }
    }

    public IActionResult GetRecord(string userCode)
    {
        try
        {
            var record = _context.Issueds
                .FirstOrDefault(x => x.UserCode == userCode);

            if (record != null)
            {
                return Json(new
                {
                    success = true,
                    data = new
                    {
                        userCode = record.UserCode,
                        name = record.Name,
                        status = record.Status,
                        createdAt = record.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
                    }
                });
            }

            return Json(new { success = false, message = "Record not found." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "An error occurred." });
        }
    }

    // ==================== EVENT MANAGEMENT ACTIONS ====================

    public IActionResult ManageEvents()
    {
        try
        {
            var events = _context.Events
                .Select(e => new EventDTO
                {
                    Id = e.Id,
                    EventName = e.EventName,
                    EventDate = e.EventDate,
                    Description = e.Description,
                    MaxTickets = e.MaxTickets,
                    CreatedTicketsCount = e.Invitations.Count(i => !i.IsDeleted),
                    ImagePath = e.ImagePath,
                    IsActive = e.IsActive
                })
                .OrderByDescending(e => e.EventDate)
                .ToList();

            return View(events);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading events");
            TempData["ErrorMessage"] = "An error occurred while loading events.";
            return View(new List<EventDTO>());
        }
    }

    [HttpGet]
    public IActionResult CreateEvent()
    {
        return View();
    }

    [HttpPost]
    public IActionResult CreateEvent(Event model, IFormFile? eventImage)
    {
        if (ModelState.IsValid)
        {
            try
            {
                // Handle image upload
                if (eventImage != null && eventImage.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_env.WebRootPath, "event_images");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = $"{Guid.NewGuid()}_{eventImage.FileName}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        eventImage.CopyTo(fileStream);
                    }

                    model.ImagePath = "/event_images/" + uniqueFileName;
                }

                model.CreatedAt = DateTime.UtcNow;
                model.CreatedTicketsCount = 0;
                model.IsActive = true;

                _context.Events.Add(model);
                _context.SaveChanges();

                TempData["SuccessMessage"] = $"Event '{model.EventName}' created successfully!";
                return RedirectToAction("ManageEvents");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating event");
                TempData["ErrorMessage"] = "An error occurred while creating the event.";
            }
        }

        return View(model);
    }

    [HttpGet]
    public IActionResult CreateBulkTickets(int eventId)
    {
        var evt = _context.Events.Find(eventId);
        if (evt == null)
        {
            TempData["ErrorMessage"] = "Event not found.";
            return RedirectToAction("ManageEvents");
        }

        ViewBag.Event = evt;
        ViewBag.RemainingTickets = evt.MaxTickets - evt.CreatedTicketsCount;
        ViewBag.IssuedCodes = _context.Issueds.Where(x => x.Status == "True").ToList();

        var imageDetails = _context.InvitationsWithPoint
            .Where(i => i.InvitationType == "True" && i.IsActive)
            .Select(i => new ImagesDTO { Value = i.Issued, Path = i.ImagePath })
            .ToList();

        ViewBag.Images = imageDetails;

        return View(new BulkTicketCreateDTO { EventId = eventId });
    }

    [HttpPost]
    public async Task<IActionResult> CreateBulkTickets(BulkTicketCreateDTO model)
    {
        var evt = _context.Events.Find(model.EventId);
        if (evt == null)
        {
            TempData["ErrorMessage"] = "Event not found.";
            return RedirectToAction("ManageEvents");
        }

        // Validation: Check if quantity exceeds remaining tickets
        int remainingTickets = evt.MaxTickets - evt.CreatedTicketsCount;
        if (model.Quantity > remainingTickets)
        {
            TempData["ErrorMessage"] = $"Cannot create {model.Quantity} tickets. Only {remainingTickets} remaining.";
            return RedirectToAction("CreateBulkTickets", new { eventId = model.EventId });
        }

        if (model.Quantity <= 0)
        {
            TempData["ErrorMessage"] = "Quantity must be greater than zero.";
            return RedirectToAction("CreateBulkTickets", new { eventId = model.EventId });
        }

        var createdCodes = new List<string>();
        var failedCount = 0;

        // Use transaction for data integrity
        using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                for (int i = 0; i < model.Quantity; i++)
                {
                    string code = GenerateRandomCode(model.Issued);
                    string imagePath = GenerateAndSaveImage(model.InvitationType, code);

                    if (string.IsNullOrEmpty(imagePath))
                    {
                        failedCount++;
                        _logger.LogWarning($"Failed to generate image for ticket {i + 1}/{model.Quantity}");
                        continue;
                    }

                    var invitation = new Invitation
                    {
                        InviterName = model.InviterName,
                        InvitationType = model.InvitationType,
                        Issued = model.Issued,
                        UniqCode = code,
                        ImagePath = imagePath,
                        CreatedAt = DateTime.Now,
                        EventId = model.EventId
                    };

                    _context.Invitations.Add(invitation);
                    createdCodes.Add(code);
                }

                // Update event ticket count
                evt.CreatedTicketsCount += (model.Quantity - failedCount);
                _context.Events.Update(evt);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = $"Successfully created {model.Quantity - failedCount} tickets for event '{evt.EventName}'.";
                if (failedCount > 0)
                {
                    TempData["WarningMessage"] = $"{failedCount} tickets failed to generate.";
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error creating bulk tickets");
                TempData["ErrorMessage"] = "An error occurred while creating tickets.";
            }
        }

        return RedirectToAction("EventTickets", new { eventId = model.EventId });
    }

    public IActionResult EventTickets(int eventId)
    {
        try
        {
            var evt = _context.Events
                .FirstOrDefault(e => e.Id == eventId);

            if (evt == null)
            {
                TempData["ErrorMessage"] = "Event not found.";
                return RedirectToAction("ManageEvents");
            }

            var tickets = _context.Invitations
                .Where(i => i.EventId == eventId && !i.IsDeleted)
                .OrderByDescending(i => i.CreatedAt)
                .ToList();

            ViewBag.Event = evt;
            return View(tickets);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading event tickets");
            TempData["ErrorMessage"] = "An error occurred while loading tickets.";
            return RedirectToAction("ManageEvents");
        }
    }

    [HttpPost]
    public IActionResult ToggleEventStatus(int eventId)
    {
        try
        {
            var evt = _context.Events.Find(eventId);
            if (evt == null)
            {
                TempData["ErrorMessage"] = "Event not found.";
                return RedirectToAction("ManageEvents");
            }

            evt.IsActive = !evt.IsActive;
            _context.Events.Update(evt);
            _context.SaveChanges();

            TempData["SuccessMessage"] = $"Event '{evt.EventName}' is now {(evt.IsActive ? "Active" : "Inactive")}.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling event status");
            TempData["ErrorMessage"] = "An error occurred while updating event status.";
        }

        return RedirectToAction("ManageEvents");
    }

    [HttpPost]
    public IActionResult ToggleImageStatus(int imageId)
    {
        try
        {
            var image = _context.InvitationsWithPoint.Find(imageId);
            if (image == null)
            {
                TempData["AlertMessage"] = "Image not found.";
                TempData["AlertType"] = "danger";
                return RedirectToAction("ManageDesing");
            }

            image.IsActive = !image.IsActive;
            _context.InvitationsWithPoint.Update(image);
            _context.SaveChanges();

            TempData["AlertMessage"] = $"Image '{image.Issued}' is now {(image.IsActive ? "Active" : "Inactive")}.";
            TempData["AlertType"] = "success";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling image status");
            TempData["AlertMessage"] = "An error occurred while updating image status.";
            TempData["AlertType"] = "danger";
        }

        return RedirectToAction("ManageDesing");
    }
}
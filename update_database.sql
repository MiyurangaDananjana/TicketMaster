-- Manual Database Update Script
-- Run this if migrations are failing
-- This will add the IsDeleted and DeletedAt columns if they don't exist

-- Drop problematic index if it exists (won't fail if it doesn't)
DROP INDEX IF EXISTS IX_InvitationsWithPoint_IssuedUserCode;

-- Create a backup table structure to check columns
CREATE TABLE IF NOT EXISTS _column_check AS SELECT * FROM Invitations WHERE 0;

-- Try to add DeletedAt column (will fail silently if it already exists)
-- Unfortunately, SQLite doesn't have IF NOT EXISTS for ALTER TABLE
-- So we'll need to handle this differently

-- For SQLite, we need to check if column exists first
-- Run these commands one by one:

-- Step 1: Try adding DeletedAt (if this fails with "duplicate column", that's OK)
-- ALTER TABLE Invitations ADD COLUMN DeletedAt TEXT;

-- Step 2: Try adding IsDeleted (if this fails with "duplicate column", that's OK)
-- ALTER TABLE Invitations ADD COLUMN IsDeleted INTEGER NOT NULL DEFAULT 0;

-- Clean up
DROP TABLE IF EXISTS _column_check;


USE SUMISAN;
GO

-- Insert dummy PHOTOS (linked to existing WashingIds 25080101 and 25080102)
INSERT INTO PHOTOS (WashingId, FileName, FilePath, CreatedAt) VALUES
(25080101, '25080101_01.jpg', '/shared/photos/2025/25080101_01.jpg', GETDATE()),
(25080101, '25080101_02.jpg', '/shared/photos/2025/25080101_02.jpg', GETDATE()),
(25080101, '25080101_03.jpg', '/shared/photos/2025/25080101_03.jpg', GETDATE()),
(25080102, '25080102_01.jpg', '/shared/photos/2025/25080102_01.jpg', GETDATE()),
(25080102, '25080102_02.jpg', '/shared/photos/2025/25080102_02.jpg', GETDATE()),
(25080102, '25080102_03.jpg', '/shared/photos/2025/25080102_03.jpg', GETDATE());

-- Insert dummy PARAMETERS (configurable values like image path)
INSERT INTO PARAMETERS (Name, Value) VALUES
('ImagePath', '/shared/photos'),
('MaxPhotosPerWash', '99'),
('SupportedFileTypes', 'jpg,png'),
('DateFormat', 'yyyy-MM-dd HH:mm:ss');

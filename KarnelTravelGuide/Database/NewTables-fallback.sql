-- FALLBACK ONLY. Preferred way: EF migration (see README-MERGE.txt). Use ONE of the two, never both.
-- Creates Bookings, ContactMessages and UserAccounts in KarnelTravelGuideDB. Does not touch TourPackages or any data.
USE KarnelTravelGuideDB;
GO
IF OBJECT_ID(N'dbo.Bookings', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Bookings (
        Id nvarchar(20) NOT NULL CONSTRAINT PK_Bookings PRIMARY KEY,
        AccountEmail nvarchar(150) NOT NULL, CreatedAtUtc datetime2 NOT NULL,
        Name nvarchar(100) NOT NULL, Email nvarchar(150) NOT NULL, Phone nvarchar(30) NULL,
        Destination nvarchar(200) NULL, Kind nvarchar(50) NULL, Location nvarchar(150) NULL, FromPrice int NULL,
        Hotel nvarchar(200) NULL, HotelLocation nvarchar(150) NULL, Included nvarchar(2000) NULL, HotelPrice int NULL,
        TravelDate datetime2 NOT NULL, People int NOT NULL, Message nvarchar(1000) NULL,
        Status nvarchar(20) NOT NULL, AdminNote nvarchar(500) NULL, DecidedAtUtc datetime2 NULL);
    CREATE INDEX IX_Bookings_AccountEmail ON dbo.Bookings (AccountEmail);
    CREATE INDEX IX_Bookings_Status ON dbo.Bookings (Status);
END
GO
IF OBJECT_ID(N'dbo.ContactMessages', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ContactMessages (
        Id nvarchar(32) NOT NULL CONSTRAINT PK_ContactMessages PRIMARY KEY,
        CreatedAtUtc datetime2 NOT NULL, Name nvarchar(100) NOT NULL, Email nvarchar(150) NOT NULL, Phone nvarchar(30) NULL,
        Type nvarchar(20) NOT NULL, Rating int NULL, [Text] nvarchar(1000) NOT NULL, Destination nvarchar(200) NULL,
        TravelDate datetime2 NULL, People int NULL, IsRead bit NOT NULL);
    CREATE INDEX IX_ContactMessages_CreatedAtUtc ON dbo.ContactMessages (CreatedAtUtc);
END
GO
IF OBJECT_ID(N'dbo.UserAccounts', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.UserAccounts (
        Id nvarchar(32) NOT NULL CONSTRAINT PK_UserAccounts PRIMARY KEY,
        CreatedAtUtc datetime2 NOT NULL, Name nvarchar(100) NOT NULL, Email nvarchar(150) NOT NULL, Phone nvarchar(30) NULL,
        Salt nvarchar(64) NOT NULL, PasswordHash nvarchar(100) NOT NULL, Iterations int NOT NULL);
    CREATE UNIQUE INDEX IX_UserAccounts_Email ON dbo.UserAccounts (Email);
END
GO

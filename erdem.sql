-- ==============================================
-- ARAÇ KÝRALAMA SÝSTEMÝ - VERÝTABANI
-- ==============================================

USE master;
GO

-- Eski veritabanýný sil
IF EXISTS (SELECT * FROM sys.databases WHERE name = 'CarRentalSystem')
BEGIN
    ALTER DATABASE CarRentalSystem SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE CarRentalSystem;
END
GO

-- Yeni veritabaný oluþtur
CREATE DATABASE CarRentalSystem;
GO

USE CarRentalSystem;
GO

-- ==============================================
-- TABLOLAR
-- ==============================================

-- Araç Kategorileri
CREATE TABLE Categories (
    CategoryID INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName NVARCHAR(50) NOT NULL,
    DailyPrice DECIMAL(10,2) NOT NULL
);

-- Araçlar
CREATE TABLE Vehicles (
    VehicleID INT IDENTITY(1,1) PRIMARY KEY,
    Brand NVARCHAR(50) NOT NULL,
    Model NVARCHAR(50) NOT NULL,
    Year INT NOT NULL,
    Plate NVARCHAR(20) NOT NULL UNIQUE,
    CategoryID INT NOT NULL,
    Color NVARCHAR(30) NULL,
    IsAvailable BIT DEFAULT 1,
    FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID)
);

-- Müþteriler
CREATE TABLE Customers (
    CustomerID INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(100) NOT NULL,
    Phone NVARCHAR(20) NOT NULL,
    Email NVARCHAR(100) NULL,
    LicenseNumber NVARCHAR(30) NOT NULL,
    Address NVARCHAR(200) NULL
);

-- Kiralamalar
CREATE TABLE Rentals (
    RentalID INT IDENTITY(1,1) PRIMARY KEY,
    VehicleID INT NOT NULL,
    CustomerID INT NOT NULL,
    RentalDate DATETIME DEFAULT GETDATE(),
    ReturnDate DATETIME NULL,
    PlannedReturnDate DATETIME NOT NULL,
    DailyPrice DECIMAL(10,2) NOT NULL,
    TotalPrice AS (DATEDIFF(DAY, RentalDate, ISNULL(ReturnDate, PlannedReturnDate)) * DailyPrice) PERSISTED,
    Status NVARCHAR(20) DEFAULT 'Active',
    Notes NVARCHAR(300) NULL,
    FOREIGN KEY (VehicleID) REFERENCES Vehicles(VehicleID),
    FOREIGN KEY (CustomerID) REFERENCES Customers(CustomerID)
);
GO

-- ==============================================
-- BAÞLANGIÇ VERÝLERÝ
-- ==============================================

-- Kategoriler
INSERT INTO Categories (CategoryName, DailyPrice) VALUES
(N'Ekonomi', 300.00),
(N'Orta Segment', 500.00),
(N'Lüks', 1000.00),
(N'SUV', 800.00),
(N'Ticari', 600.00);

-- Araçlar
INSERT INTO Vehicles (Brand, Model, Year, Plate, CategoryID, Color, IsAvailable) VALUES
(N'Fiat', N'Egea', 2023, N'34 ABC 123', 1, N'Beyaz', 1),
(N'Renault', N'Clio', 2022, N'34 DEF 456', 1, N'Kýrmýzý', 1),
(N'Toyota', N'Corolla', 2023, N'06 GHI 789', 2, N'Gri', 1),
(N'Volkswagen', N'Passat', 2023, N'35 JKL 012', 2, N'Siyah', 1),
(N'BMW', N'5 Serisi', 2024, N'34 MNO 345', 3, N'Beyaz', 1),
(N'Mercedes', N'E-Class', 2024, N'06 PRS 678', 3, N'Gümüþ', 1),
(N'Honda', N'CR-V', 2023, N'34 TUV 901', 4, N'Mavi', 1),
(N'Nissan', N'Qashqai', 2022, N'35 WXY 234', 4, N'Beyaz', 0),
(N'Ford', N'Transit', 2023, N'34 ZAB 567', 5, N'Beyaz', 1),
(N'Hyundai', N'H100', 2022, N'06 CDE 890', 5, N'Gri', 1);

-- Müþteriler
INSERT INTO Customers (FullName, Phone, Email, LicenseNumber, Address) VALUES
(N'Ali Yýlmaz', N'0532-111-2233', N'ali@email.com', N'A12345678', N'Ýstanbul, Kadýköy'),
(N'Ayþe Demir', N'0533-222-3344', N'ayse@email.com', N'B87654321', N'Ankara, Çankaya'),
(N'Mehmet Kaya', N'0534-333-4455', N'mehmet@email.com', N'C11223344', N'Ýzmir, Karþýyaka'),
(N'Fatma Öz', N'0535-444-5566', N'fatma@email.com', N'D55667788', N'Antalya, Muratpaþa'),
(N'Ahmet Çelik', N'0536-555-6677', N'ahmet@email.com', N'E99887766', N'Bursa, Nilüfer');

-- Örnek Kiralamalar
-- Ýlk kiralama (Aktif)
DECLARE @Price1 DECIMAL(10,2) = (SELECT DailyPrice FROM Categories WHERE CategoryID = 1);
INSERT INTO Rentals (VehicleID, CustomerID, RentalDate, PlannedReturnDate, DailyPrice, Status)
VALUES (1, 1, GETDATE(), DATEADD(DAY, 5, GETDATE()), @Price1, 'Active');

-- Ýkinci kiralama (Tamamlanmýþ)
DECLARE @Price2 DECIMAL(10,2) = (SELECT DailyPrice FROM Categories WHERE CategoryID = 3);
INSERT INTO Rentals (VehicleID, CustomerID, RentalDate, ReturnDate, PlannedReturnDate, DailyPrice, Status)
VALUES (5, 2, DATEADD(DAY, -10, GETDATE()), DATEADD(DAY, -3, GETDATE()), DATEADD(DAY, -3, GETDATE()), @Price2, 'Completed');

-- Üçüncü kiralama (Aktif - SUV kiralanmýþ)
DECLARE @Price3 DECIMAL(10,2) = (SELECT DailyPrice FROM Categories WHERE CategoryID = 4);
INSERT INTO Rentals (VehicleID, CustomerID, RentalDate, PlannedReturnDate, DailyPrice, Status, Notes)
VALUES (8, 3, DATEADD(DAY, -2, GETDATE()), DATEADD(DAY, 4, GETDATE()), @Price3, 'Active', N'Havalimaný teslim');

-- Kiralanmýþ araçlarý güncelle
UPDATE Vehicles SET IsAvailable = 0 WHERE VehicleID IN (1, 8);
GO

PRINT '==============================================';
PRINT 'CarRentalSystem Veritabaný Hazýr!';
PRINT 'Kategoriler: 5 adet';
PRINT 'Araçlar: 10 adet';
PRINT 'Müþteriler: 5 adet';
PRINT 'Kiralamalar: 3 adet';
PRINT '==============================================';
GO
IF DB_ID(N'OrderDb') IS NULL
BEGIN
    CREATE DATABASE [OrderDb];
END
GO

USE [OrderDb];
GO

IF OBJECT_ID(N'dbo.Client', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Client]
    (
        [Id] VARCHAR(32) NOT NULL PRIMARY KEY,
        [Name] VARCHAR(100) NOT NULL,
        [Email] VARCHAR(100) NULL,
        [PhoneNumber] VARCHAR(20) NULL,
        [Address] VARCHAR(200) NULL,
        [CreatedAt] DATETIME2(7) NOT NULL
    );
END
GO

IF OBJECT_ID(N'dbo.[User]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[User]
    (
        [Id] VARCHAR(32) NOT NULL PRIMARY KEY,
        [Name] VARCHAR(100) NOT NULL,
        [Login] VARCHAR(50) NOT NULL UNIQUE,
        [PasswordHash] VARCHAR(255) NOT NULL,
        [CreatedAt] DATETIME2(7) NOT NULL
    );
END
GO

IF OBJECT_ID(N'dbo.Product', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Product]
    (
        [Id] VARCHAR(32) NOT NULL PRIMARY KEY,
        [Description] VARCHAR(100) NOT NULL,
        [SellValue] DECIMAL(18, 2) NOT NULL,
        [Stock] INT NOT NULL CONSTRAINT [DF_Product_Stock] DEFAULT (0),
        [CreatedAt] DATETIME2(7) NOT NULL
    );
END
GO

IF OBJECT_ID(N'dbo.[Order]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Order]
    (
        [Id] VARCHAR(32) NOT NULL PRIMARY KEY,
        [ClientId] VARCHAR(32) NOT NULL,
        [UserId] VARCHAR(32) NOT NULL,
        [CreatedAt] DATETIME2(7) NOT NULL,
        CONSTRAINT [FK_Order_Client] FOREIGN KEY ([ClientId]) REFERENCES [dbo].[Client]([Id]),
        CONSTRAINT [FK_Order_User] FOREIGN KEY ([UserId]) REFERENCES [dbo].[User]([Id])
    );
END
GO

IF OBJECT_ID(N'dbo.OrderItem', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[OrderItem]
    (
        [Id] VARCHAR(32) NOT NULL PRIMARY KEY,
        [OrderId] VARCHAR(32) NOT NULL,
        [ProductId] VARCHAR(32) NOT NULL,
        [SellValue] DECIMAL(18, 2) NOT NULL,
        [Quantity] INT NOT NULL,
        [TotalAmount] DECIMAL(18, 2) NOT NULL,
        [CreatedAt] DATETIME2(7) NOT NULL,
        CONSTRAINT [FK_OrderItem_Order] FOREIGN KEY ([OrderId]) REFERENCES [dbo].[Order]([Id]),
        CONSTRAINT [FK_OrderItem_Product] FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Product]([Id]),
        CONSTRAINT [CK_OrderItem_Quantity] CHECK ([Quantity] > 0)
    );
END
GO

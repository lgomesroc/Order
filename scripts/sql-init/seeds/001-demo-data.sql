USE [OrderDb];
GO
IF NOT EXISTS (SELECT 1 FROM [dbo].[User] WHERE [Login] = 'demo')
BEGIN
 INSERT INTO [dbo].[User] ([Id],[Name],[Login],[PasswordHash],[CreatedAt]) VALUES
 ('aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa','Usuário de Demonstração','demo','$2a$12$SJVB1Gpv6MMZF8BYc7gQcetoVob7/ThwE1qCnvQPoX8HwP1SycUSG',SYSUTCDATETIME());
END
GO
IF NOT EXISTS (SELECT 1 FROM [dbo].[Client] WHERE [Id] = '11111111111111111111111111111111')
BEGIN
 INSERT INTO [dbo].[Client] ([Id],[Name],[Email],[PhoneNumber],[Address],[CreatedAt]) VALUES
 ('11111111111111111111111111111111','Maria Oliveira','maria.oliveira@example.test','(21) 99999-1001','Rua das Flores, 100 - Rio de Janeiro/RJ',SYSUTCDATETIME()),
 ('12121212121212121212121212121212','Carlos Santos','carlos.santos@example.test','(21) 99999-1002','Av. Central, 250 - Rio de Janeiro/RJ',SYSUTCDATETIME());
END
GO
IF NOT EXISTS (SELECT 1 FROM [dbo].[Product] WHERE [Id] = '22222222222222222222222222222222')
BEGIN
 INSERT INTO [dbo].[Product] ([Id],[Description],[SellValue],[Stock],[CreatedAt]) VALUES
 ('22222222222222222222222222222222','Teclado mecânico',250.00,18,SYSUTCDATETIME()),
 ('23232323232323232323232323232323','Mouse sem fio',120.00,25,SYSUTCDATETIME());
END
GO
IF NOT EXISTS (SELECT 1 FROM [dbo].[Order] WHERE [Id] = '44444444444444444444444444444444')
BEGIN
 INSERT INTO [dbo].[Order] ([Id],[ClientId],[UserId],[CreatedAt]) VALUES
 ('44444444444444444444444444444444','11111111111111111111111111111111','aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa',SYSUTCDATETIME());
 INSERT INTO [dbo].[OrderItem] ([Id],[OrderId],[ProductId],[SellValue],[Quantity],[TotalAmount],[CreatedAt]) VALUES
 ('55555555555555555555555555555555','44444444444444444444444444444444','22222222222222222222222222222222',250.00,2,500.00,SYSUTCDATETIME());
 UPDATE [dbo].[Product] SET [Stock] = [Stock] - 2 WHERE [Id] = '22222222222222222222222222222222';
END
GO

CREATE PROCEDURE [dbo].[sp_AddInventory]
				 @ObjectName NVARCHAR(60),
				 @ObjectQuantity INT,
				 @ObjectStatus NVARCHAR(60),
				 @ObjectID INT OUT
				 AS
				 INSERT INTO [Inventory] ([Name], [Quantity], [Status])
				 VALUES (@ObjectName, @ObjectQuantity, @ObjectStatus)
				 SELECT @ObjectID = SCOPE_IDENTITY()
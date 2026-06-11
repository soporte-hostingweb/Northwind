USE Northwind;
GO

-- 1. Agregar las columnas primero
ALTER TABLE Usuarios ADD CustomerID NCHAR(5) NULL;
ALTER TABLE Usuarios ADD SupplierID INT NULL;
GO

-- 2. Agregar las restricciones de llave foránea una por una
ALTER TABLE Usuarios 
ADD CONSTRAINT FK_Usuarios_Customers 
FOREIGN KEY (CustomerID) REFERENCES Customers(CustomerID)
ON DELETE SET NULL;
GO

ALTER TABLE Usuarios 
ADD CONSTRAINT FK_Usuarios_Suppliers 
FOREIGN KEY (SupplierID) REFERENCES Suppliers(SupplierID)
ON DELETE SET NULL;
GO

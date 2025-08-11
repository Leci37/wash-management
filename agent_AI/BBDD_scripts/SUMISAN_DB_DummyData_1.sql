
USE SUMISAN;
GO

-- Insert dummy USERS (Basque names)
INSERT INTO USERS (UserName, PasswordHash, Role) VALUES 
('Ane Etxebarria', NULL, 'WarehouseUser'),
('Iker Mendizabal', NULL, 'WarehouseUser'),
('Maialen Garmendia', NULL, 'WarehouseUser'),
('Unai Arrieta', NULL, 'WarehouseUser'),
('Nahia Altuna', NULL, 'WarehouseUser'),
('Eneko Iturralde', NULL, 'WarehouseUser'),
('Irati Zabaleta', NULL, 'WarehouseUser'),
('Gaizka Olaizola', NULL, 'WarehouseUser'),
('Leire Otxoa', NULL, 'WarehouseUser'),
('Ander Arozena', NULL, 'WarehouseUser'),
('Oihana Aramburu', NULL, 'WarehouseUser'),
('Mikel Agirre', NULL, 'WarehouseUser');

-- Insert dummy MACHINES (Brands + Models)
INSERT INTO MACHINES (Id, Name) VALUES
(1, 'Steris AMSCO 5052'),
(2, 'Belimed WD290'),
(3, 'Miele PG 8591'),
(4, 'Getinge 8668');

-- Assume WASHING IDs 25080101 and 25080102 already exist in WASHINGS table.
-- Insert dummy PROTS
INSERT INTO PROTS (WashingId, ProtId, BatchNumber, BagNumber) VALUES
(25080101, 'PROT001', 'NL01', '01/01'),
(25080101, 'PROT002', 'NL01', '01/02'),
(25080101, 'PROT003', 'NL02', '01/03'),
(25080101, 'PROT007', 'NL01', '01/04'),
(25080101, 'PROT008', 'NL01', '01/05'),
(25080101, 'PROT009', 'NL02', '01/06'),
(25080102, 'PROT004', 'NL02', '01/01'),
(25080102, 'PROT005', 'NL03', '01/02'),
(25080102, 'PROT006', 'NL03', '01/03'),
(25080102, 'PROT010', 'NL02', '01/04'),
(25080102, 'PROT011', 'NL03', '01/05'),
(25080102, 'PROT012', 'NL03', '01/06'),
(25080102, 'PROT013', 'NL04', '01/07'),
(25080102, 'PROT014', 'NL04', '01/08'),
(25080102, 'PROT015', 'NL04', '01/09');

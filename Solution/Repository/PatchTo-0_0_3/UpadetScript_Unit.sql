ALTER TABLE "Unit" RENAME TO "Unit_temp";
CREATE TABLE [Unit] (
	[ID]	integer PRIMARY KEY AUTOINCREMENT NOT NULL,
	[Name]	nvarchar(20) NOT NULL COLLATE NOCASE,
	[DisplayNames]	nvarchar(450) NOT NULL COLLATE NOCASE

);
-- INSERT INTO "Unit" SELECT "ID","Name","DisplayName" FROM "Unit_temp";
DROP TABLE "Unit_temp";

INSERT INTO "Unit" VALUES(0,'None','en-US:None;hu-HU:Nincs;');
INSERT INTO "Unit" VALUES(101,'Pc','en-US:pc;hu-HU:db;');
INSERT INTO "Unit" VALUES(102,'Kg','en-US:kg;hu-HU:kg;');
INSERT INTO "Unit" VALUES(103,'Dkg','en-US:dkg;hu-HU:dkg;');
INSERT INTO "Unit" VALUES(104,'Gram','en-US:g;hu-HU:g;');
INSERT INTO "Unit" VALUES(105,'Liter','en-US:liter;hu-HU:liter;');

CREATE TRIGGER [fkd_TransactionItem_UnitID_Unit_ID] Before Delete ON [Unit] BEGIN SELECT RAISE(ROLLBACK, 'delete on table Unit violates foreign key constraint fkd_TransactionItem_UnitID_Unit_ID') WHERE (SELECT UnitID FROM TransactionItem WHERE UnitID = OLD.ID) IS NOT NULL;  END;

CREATE UNIQUE INDEX [Unit_UK_Unit_DisplayNames]
ON [Unit]
([DisplayNames] DESC);
CREATE UNIQUE INDEX [Unit_UK_Unit_Name]
ON [Unit]
([Name] DESC);

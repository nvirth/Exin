ALTER TABLE "Category" RENAME TO "Category_temp";
CREATE TABLE [Category] (
	[ID]	integer PRIMARY KEY AUTOINCREMENT NOT NULL,
	[Name]	nvarchar(50) NOT NULL COLLATE NOCASE,
	[DisplayNames]	nvarchar(450) NOT NULL COLLATE NOCASE

);
-- INSERT INTO "Category" SELECT "ID","Name","DisplayName" FROM "Category_temp";
DROP TABLE "Category_temp";

INSERT INTO "Category" VALUES(0,'None','en-US:None;hu-HU:Nincs;');
INSERT INTO "Category" VALUES(1,'FullExpenseSummary','en-US:Full expense summary (without category);hu-HU:Teljes kiadás összegzés (kategória nélkül);');
INSERT INTO "Category" VALUES(2,'FullIncomeSummary','en-US:Full income summary (without category);hu-HU:Teljes bevétel összegzés (kategória nélkül);');
INSERT INTO "Category" VALUES(101,'Food','en-US:Food;hu-HU:Kaja;');
INSERT INTO "Category" VALUES(102,'ConfectionTonic','en-US:Confection, tonic;hu-HU:Édesség, üdítő;');
INSERT INTO "Category" VALUES(103,'Booze','en-US:Booze;hu-HU:Pia;');
INSERT INTO "Category" VALUES(104,'Invoice','en-US:Invoice;hu-HU:Számla;');
INSERT INTO "Category" VALUES(105,'Train','en-US:Train;hu-HU:Vonat;');
INSERT INTO "Category" VALUES(106,'OtherPublicTransport','en-US:Other public transport;hu-HU:Más tömegközlekedés;');
INSERT INTO "Category" VALUES(107,'Cigarette','en-US:Cigarette;hu-HU:Cigi;');
INSERT INTO "Category" VALUES(108,'Others','en-US:Others;hu-HU:Egyéb;');

CREATE TRIGGER [fkd_SummaryItem_CategoryID_Category_ID] Before Delete ON [Category] BEGIN SELECT RAISE(ROLLBACK, 'delete on table Category violates foreign key constraint fkd_SummaryItem_CategoryID_Category_ID') WHERE (SELECT CategoryID FROM SummaryItem WHERE CategoryID = OLD.ID) IS NOT NULL;  END;
CREATE TRIGGER [fkd_TransactionItem_CategoryID_Category_ID] Before Delete ON [Category] BEGIN SELECT RAISE(ROLLBACK, 'delete on table Category violates foreign key constraint fkd_TransactionItem_CategoryID_Category_ID') WHERE (SELECT CategoryID FROM TransactionItem WHERE CategoryID = OLD.ID) IS NOT NULL;  END;

CREATE UNIQUE INDEX [Category_UK_Category_DisplayNames]
ON [Category]
([DisplayNames] DESC);
CREATE UNIQUE INDEX [Category_UK_Category_Name]
ON [Category]
([Name] DESC);
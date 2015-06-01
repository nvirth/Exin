CREATE TABLE [Category] (
	[ID]	integer PRIMARY KEY AUTOINCREMENT NOT NULL,
	[Name]	nvarchar(50) NOT NULL COLLATE NOCASE,
	[DisplayName]	nvarchar(50) NOT NULL COLLATE NOCASE

);
CREATE TABLE [SummaryItem] (
	[ID]	integer PRIMARY KEY AUTOINCREMENT NOT NULL,
	[Amount]	integer NOT NULL,
	[CategoryID]	integer NOT NULL,
	[Date]	datetime NOT NULL COLLATE NOCASE
,
    FOREIGN KEY ([CategoryID])
        REFERENCES [Category]([ID])
);
CREATE TABLE [TransactionItem] (
	[ID]	integer PRIMARY KEY AUTOINCREMENT NOT NULL,
	[Amount]	integer NOT NULL,
	[Quantity]	integer NOT NULL,
	[UnitID]	integer NOT NULL,
	[Title]	nvarchar(100) NOT NULL COLLATE NOCASE,
	[Comment]	nvarchar(200) COLLATE NOCASE,
	[Date]	datetime NOT NULL COLLATE NOCASE,
	[CategoryID]	integer NOT NULL,
	[IsExpenseItem]	bit NOT NULL,
	[IsIncomeItem]	bit NOT NULL
,
    FOREIGN KEY ([CategoryID])
        REFERENCES [Category]([ID]),
    FOREIGN KEY ([UnitID])
        REFERENCES [Unit]([ID])
);
CREATE TABLE [Unit] (
	[ID]	integer PRIMARY KEY AUTOINCREMENT NOT NULL,
	[Name]	nvarchar(20) NOT NULL COLLATE NOCASE,
	[DisplayName]	nvarchar(20) NOT NULL COLLATE NOCASE

);
CREATE TRIGGER [fkd_SummaryItem_CategoryID_Category_ID] Before Delete ON [Category] BEGIN SELECT RAISE(ROLLBACK, 'delete on table Category violates foreign key constraint fkd_SummaryItem_CategoryID_Category_ID') WHERE (SELECT CategoryID FROM SummaryItem WHERE CategoryID = OLD.ID) IS NOT NULL;  END;
CREATE TRIGGER [fkd_TransactionItem_CategoryID_Category_ID] Before Delete ON [Category] BEGIN SELECT RAISE(ROLLBACK, 'delete on table Category violates foreign key constraint fkd_TransactionItem_CategoryID_Category_ID') WHERE (SELECT CategoryID FROM TransactionItem WHERE CategoryID = OLD.ID) IS NOT NULL;  END;
CREATE TRIGGER [fkd_TransactionItem_UnitID_Unit_ID] Before Delete ON [Unit] BEGIN SELECT RAISE(ROLLBACK, 'delete on table Unit violates foreign key constraint fkd_TransactionItem_UnitID_Unit_ID') WHERE (SELECT UnitID FROM TransactionItem WHERE UnitID = OLD.ID) IS NOT NULL;  END;
CREATE TRIGGER [fki_SummaryItem_CategoryID_Category_ID] Before Insert ON [SummaryItem] BEGIN SELECT RAISE(ROLLBACK, 'insert on table SummaryItem violates foreign key constraint fki_SummaryItem_CategoryID_Category_ID') WHERE (SELECT ID FROM Category WHERE ID = NEW.CategoryID) IS NULL;  END;
CREATE TRIGGER [fki_TransactionItem_CategoryID_Category_ID] Before Insert ON [TransactionItem] BEGIN SELECT RAISE(ROLLBACK, 'insert on table TransactionItem violates foreign key constraint fki_TransactionItem_CategoryID_Category_ID') WHERE (SELECT ID FROM Category WHERE ID = NEW.CategoryID) IS NULL;  END;
CREATE TRIGGER [fki_TransactionItem_UnitID_Unit_ID] Before Insert ON [TransactionItem] BEGIN SELECT RAISE(ROLLBACK, 'insert on table TransactionItem violates foreign key constraint fki_TransactionItem_UnitID_Unit_ID') WHERE (SELECT ID FROM Unit WHERE ID = NEW.UnitID) IS NULL;  END;
CREATE TRIGGER [fku_SummaryItem_CategoryID_Category_ID] Before Update ON [SummaryItem] BEGIN SELECT RAISE(ROLLBACK, 'update on table SummaryItem violates foreign key constraint fku_SummaryItem_CategoryID_Category_ID') WHERE (SELECT ID FROM Category WHERE ID = NEW.CategoryID) IS NULL;  END;
CREATE TRIGGER [fku_TransactionItem_CategoryID_Category_ID] Before Update ON [TransactionItem] BEGIN SELECT RAISE(ROLLBACK, 'update on table TransactionItem violates foreign key constraint fku_TransactionItem_CategoryID_Category_ID') WHERE (SELECT ID FROM Category WHERE ID = NEW.CategoryID) IS NULL;  END;
CREATE TRIGGER [fku_TransactionItem_UnitID_Unit_ID] Before Update ON [TransactionItem] BEGIN SELECT RAISE(ROLLBACK, 'update on table TransactionItem violates foreign key constraint fku_TransactionItem_UnitID_Unit_ID') WHERE (SELECT ID FROM Unit WHERE ID = NEW.UnitID) IS NULL;  END;
CREATE UNIQUE INDEX [Category_UK_Category_DisplayName]
ON [Category]
([DisplayName] DESC);
CREATE UNIQUE INDEX [Category_UK_Category_Name]
ON [Category]
([Name] DESC);
CREATE UNIQUE INDEX [SummaryItem_UK_IX_SummaryItem_Date_CategoryID]
ON [SummaryItem]
([Date] DESC, [CategoryID] DESC);
CREATE INDEX [TransactionItem_IX_TransactionItem_Date]
ON [TransactionItem]
([Date] DESC);
CREATE INDEX [Unit_UK_Unit_DisplayName]
ON [Unit]
([DisplayName] DESC);
CREATE UNIQUE INDEX [Unit_UK_Unit_Name]
ON [Unit]
([Name] DESC);

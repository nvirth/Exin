CREATE TABLE [dbo].[TransactionItem](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Amount] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[UnitID] [int] NOT NULL,
	[Title] [nvarchar](100) NOT NULL,
	[Comment] [nvarchar](200) NULL,
	[Date] [date] NOT NULL,
	[CategoryID] [int] NOT NULL,
	[IsExpenseItem] [bit] NOT NULL,
	[IsIncomeItem] [bit] NOT NULL,
 CONSTRAINT [PK_TransactionItem] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[TransactionItem]   ADD  CONSTRAINT [FK_TransactionItem_Category] FOREIGN KEY([CategoryID])
REFERENCES [dbo].[Category] ([ID])
GO

ALTER TABLE [dbo].[TransactionItem] CHECK CONSTRAINT [FK_TransactionItem_Category]
GO
ALTER TABLE [dbo].[TransactionItem]   ADD  CONSTRAINT [FK_TransactionItem_Unit] FOREIGN KEY([UnitID])
REFERENCES [dbo].[Unit] ([ID])
GO

ALTER TABLE [dbo].[TransactionItem] CHECK CONSTRAINT [FK_TransactionItem_Unit]
GO
ALTER TABLE [dbo].[TransactionItem]   ADD  CONSTRAINT [CK_TransactionItem_Amount_bte_0] CHECK  (([Amount]>=(0)))
GO

ALTER TABLE [dbo].[TransactionItem] CHECK CONSTRAINT [CK_TransactionItem_Amount_bte_0]
GO
ALTER TABLE [dbo].[TransactionItem]   ADD  CONSTRAINT [CK_TransactionItem_IsExpenseItem_XOR_IsIncomeItem] CHECK  ((([IsExpenseItem]^[IsIncomeItem])=(1)))
GO

ALTER TABLE [dbo].[TransactionItem] CHECK CONSTRAINT [CK_TransactionItem_IsExpenseItem_XOR_IsIncomeItem]
GO
ALTER TABLE [dbo].[TransactionItem]   ADD  CONSTRAINT [CK_TransactionItem_Quantity_bte_0] CHECK  (([Quantity]>=(0)))
GO

ALTER TABLE [dbo].[TransactionItem] CHECK CONSTRAINT [CK_TransactionItem_Quantity_bte_0]
GO
/****** Object:  Index [IX_TransactionItem_Date]    Script Date: 2014.02.01. 11:22:38 ******/
CREATE NONCLUSTERED INDEX [IX_TransactionItem_Date] ON [dbo].[TransactionItem]
(
	[Date] ASC
)
INCLUDE ( 	[ID],
	[Amount],
	[Quantity],
	[UnitID],
	[Title],
	[Comment],
	[CategoryID],
	[IsExpenseItem],
	[IsIncomeItem]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
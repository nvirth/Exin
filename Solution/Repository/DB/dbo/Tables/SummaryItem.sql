﻿CREATE TABLE [dbo].[SummaryItem](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Amount] [int] NOT NULL,
	[CategoryID] [int] NOT NULL,
	[Date] [date] NOT NULL,
 CONSTRAINT [PK_SummaryItem] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[SummaryItem]  ADD  CONSTRAINT [FK_SummaryItem_Category] FOREIGN KEY([CategoryID])
REFERENCES [dbo].[Category] ([ID])
GO

ALTER TABLE [dbo].[SummaryItem] CHECK CONSTRAINT [FK_SummaryItem_Category]
GO
/****** Object:  Index [UK_IX_SummaryItem_Date_CategoryID]    Script Date: 2014.02.01. 11:22:38 ******/
CREATE UNIQUE NONCLUSTERED INDEX [UK_IX_SummaryItem_Date_CategoryID] ON [dbo].[SummaryItem]
(
	[Date] ASC,
	[CategoryID] ASC
)
INCLUDE ( 	[ID],
	[Amount]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
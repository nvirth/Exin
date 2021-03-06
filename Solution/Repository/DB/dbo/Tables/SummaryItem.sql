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
ALTER TABLE [dbo].[SummaryItem]  WITH CHECK ADD  CONSTRAINT [FK_SummaryItem_Category] FOREIGN KEY([CategoryID])
REFERENCES [dbo].[Category] ([ID])
GO

ALTER TABLE [dbo].[SummaryItem] CHECK CONSTRAINT [FK_SummaryItem_Category]
GO


GO
/****** Object:  Index [UK_IX_SummaryItem_Date_CategoryID]    Script Date: 2014.02.01. 11:22:38 ******/
/****** Object:  Index [UK_IX_SummaryItem_Date_CategoryID]    Script Date: 6/7/2015 9:39:53 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [UK_IX_SummaryItem_Date_CategoryID] ON [dbo].[SummaryItem]
(
	[Date] ASC,
	[CategoryID] ASC
)
INCLUDE ( 	[ID],
	[Amount]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
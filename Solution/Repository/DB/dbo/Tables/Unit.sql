CREATE TABLE [dbo].[Unit](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](20) NOT NULL,
	[DisplayNames] [nvarchar](450) NOT NULL,
 CONSTRAINT [PK_Unit] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_Unit_Name] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Index [UK_Unit_DisplayNames]    Script Date: 9/5/2015 6:41:52 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [UK_Unit_DisplayNames] ON [dbo].[Unit]
(
	[DisplayNames] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Expects localized display names, like: "en-US:liter;hu-HU:liter;". So, the maximum lenght of a DisplayName value is 20 char long; +7 chars come in per language => 27 char per lang. There are currently 2 languages in use (54 char); so 450 (which is the max index size) would be enough for 16 languages.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Unit', @level2type=N'COLUMN',@level2name=N'DisplayNames'
USE [master]
GO
/****** Object:  Database [Exin]    Script Date: 2014.01.02. 15:31:00 ******/
CREATE DATABASE [Exin]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'Exin', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\DATA\Exin_Primary.mdf' , SIZE = 4160KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'Exin_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\DATA\Exin_Primary.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [Exin] SET COMPATIBILITY_LEVEL = 110
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Exin].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [Exin] SET ANSI_NULL_DEFAULT ON 
GO
ALTER DATABASE [Exin] SET ANSI_NULLS ON 
GO
ALTER DATABASE [Exin] SET ANSI_PADDING ON 
GO
ALTER DATABASE [Exin] SET ANSI_WARNINGS ON 
GO
ALTER DATABASE [Exin] SET ARITHABORT ON 
GO
ALTER DATABASE [Exin] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [Exin] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [Exin] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [Exin] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [Exin] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [Exin] SET CURSOR_DEFAULT  LOCAL 
GO
ALTER DATABASE [Exin] SET CONCAT_NULL_YIELDS_NULL ON 
GO
ALTER DATABASE [Exin] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [Exin] SET QUOTED_IDENTIFIER ON 
GO
ALTER DATABASE [Exin] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [Exin] SET  DISABLE_BROKER 
GO
ALTER DATABASE [Exin] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [Exin] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [Exin] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [Exin] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [Exin] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [Exin] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [Exin] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [Exin] SET RECOVERY FULL 
GO
ALTER DATABASE [Exin] SET  MULTI_USER 
GO
ALTER DATABASE [Exin] SET PAGE_VERIFY NONE  
GO
ALTER DATABASE [Exin] SET DB_CHAINING OFF 
GO
ALTER DATABASE [Exin] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [Exin] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
EXEC sys.sp_db_vardecimal_storage_format N'Exin', N'ON'
GO
USE [Exin]
GO
/****** Object:  Table [dbo].[Category]    Script Date: 2014.01.02. 15:31:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Category](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Category] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_Category] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SummaryItem]    Script Date: 2014.01.02. 15:31:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SummaryItem](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SumIn] [int] NOT NULL,
	[SumOut] [int] NOT NULL,
	[CategoryID] [int] NULL,
	[Date] [date] NOT NULL,
 CONSTRAINT [PK_SummaryItem] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_Date_SumIn_SumOut_CategoryID] UNIQUE NONCLUSTERED 
(
	[SumIn] ASC,
	[SumOut] ASC,
	[CategoryID] ASC,
	[Date] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TransactionItem]    Script Date: 2014.01.02. 15:31:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TransactionItem](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Amount] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[UnitID] [int] NOT NULL,
	[Title] [nvarchar](100) NOT NULL,
	[Comment] [nvarchar](200) NULL,
	[Date] [date] NOT NULL,
	[CategoryID] [int] NULL,
	[IsExpenseItem] [bit] NOT NULL,
	[IsIncomeItem] [bit] NOT NULL,
 CONSTRAINT [PK_TransactionItem] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Unit]    Script Date: 2014.01.02. 15:31:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Unit](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](20) NOT NULL,
 CONSTRAINT [PK_Unit] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_Unit] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[SummaryItem]  WITH CHECK ADD  CONSTRAINT [FK_SummaryItem_Category] FOREIGN KEY([CategoryID])
REFERENCES [dbo].[Category] ([ID])
GO
ALTER TABLE [dbo].[SummaryItem] CHECK CONSTRAINT [FK_SummaryItem_Category]
GO
ALTER TABLE [dbo].[TransactionItem]  WITH CHECK ADD  CONSTRAINT [FK_TransactionItem_Category] FOREIGN KEY([CategoryID])
REFERENCES [dbo].[Category] ([ID])
GO
ALTER TABLE [dbo].[TransactionItem] CHECK CONSTRAINT [FK_TransactionItem_Category]
GO
ALTER TABLE [dbo].[TransactionItem]  WITH CHECK ADD  CONSTRAINT [FK_TransactionItem_Unit] FOREIGN KEY([UnitID])
REFERENCES [dbo].[Unit] ([ID])
GO
ALTER TABLE [dbo].[TransactionItem] CHECK CONSTRAINT [FK_TransactionItem_Unit]
GO
ALTER TABLE [dbo].[SummaryItem]  WITH CHECK ADD  CONSTRAINT [CK_SumIn_XOR_SumOut_XOR_CategoryID] CHECK  ((((case when [SumIn]=(0) then (0) else (1) end+case when [SumOut]=(0) then (0) else (1) end)+case when [CategoryID] IS NULL then (0) else (1) end)=(1)))
GO
ALTER TABLE [dbo].[SummaryItem] CHECK CONSTRAINT [CK_SumIn_XOR_SumOut_XOR_CategoryID]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'This unique key is for Date and CategoryID pairs ( 1 date - 1 category summary only). There must be SumIn and SumOut columns in it as well, because CategoryID is null for both' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SummaryItem', @level2type=N'CONSTRAINT',@level2name=N'UK_Date_SumIn_SumOut_CategoryID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'A SummaryItem can be IncomeSummary, ExpenseSummary or ExpenseCategorySummary at a time (only 1)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SummaryItem', @level2type=N'CONSTRAINT',@level2name=N'CK_SumIn_XOR_SumOut_XOR_CategoryID'
GO
USE [master]
GO
ALTER DATABASE [Exin] SET  READ_WRITE 
GO

USE [master]
GO
/****** Object:  Database [ExinDeveloper]    Script Date: 2014.02.01. 11:22:38 ******/
CREATE DATABASE [ExinDeveloper]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'ExinDeveloper', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\DATA\ExinDeveloper_Primary.mdf' , SIZE = 4160KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'ExinDeveloper_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\DATA\ExinDeveloper_Primary.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [ExinDeveloper] SET COMPATIBILITY_LEVEL = 110
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [ExinDeveloper].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [ExinDeveloper] SET ANSI_NULL_DEFAULT ON 
GO
ALTER DATABASE [ExinDeveloper] SET ANSI_NULLS ON 
GO
ALTER DATABASE [ExinDeveloper] SET ANSI_PADDING ON 
GO
ALTER DATABASE [ExinDeveloper] SET ANSI_WARNINGS ON 
GO
ALTER DATABASE [ExinDeveloper] SET ARITHABORT ON 
GO
ALTER DATABASE [ExinDeveloper] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [ExinDeveloper] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [ExinDeveloper] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [ExinDeveloper] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [ExinDeveloper] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [ExinDeveloper] SET CURSOR_DEFAULT  LOCAL 
GO
ALTER DATABASE [ExinDeveloper] SET CONCAT_NULL_YIELDS_NULL ON 
GO
ALTER DATABASE [ExinDeveloper] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [ExinDeveloper] SET QUOTED_IDENTIFIER ON 
GO
ALTER DATABASE [ExinDeveloper] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [ExinDeveloper] SET  DISABLE_BROKER 
GO
ALTER DATABASE [ExinDeveloper] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [ExinDeveloper] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [ExinDeveloper] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [ExinDeveloper] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [ExinDeveloper] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [ExinDeveloper] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [ExinDeveloper] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [ExinDeveloper] SET RECOVERY FULL 
GO
ALTER DATABASE [ExinDeveloper] SET  MULTI_USER 
GO
ALTER DATABASE [ExinDeveloper] SET PAGE_VERIFY NONE  
GO
ALTER DATABASE [ExinDeveloper] SET DB_CHAINING OFF 
GO
ALTER DATABASE [ExinDeveloper] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [ExinDeveloper] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
EXEC sys.sp_db_vardecimal_storage_format N'ExinDeveloper', N'ON'
GO
USE [ExinDeveloper]
GO
/****** Object:  StoredProcedure [dbo].[ELMAH_GetErrorsXml]    Script Date: 2014.02.01. 11:22:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ELMAH_GetErrorsXml]
(
    @Application NVARCHAR(60),
    @PageIndex INT = 0,
    @PageSize INT = 15,
    @TotalCount INT OUTPUT
)
AS 

    SET NOCOUNT ON

    DECLARE @FirstTimeUTC DATETIME
    DECLARE @FirstSequence INT
    DECLARE @StartRow INT
    DECLARE @StartRowIndex INT

    SELECT 
        @TotalCount = COUNT(1) 
    FROM 
        [ELMAH_Error]
    WHERE 
        [Application] = @Application

    -- Get the ID of the first error for the requested page

    SET @StartRowIndex = @PageIndex * @PageSize + 1

    IF @StartRowIndex <= @TotalCount
    BEGIN

        SET ROWCOUNT @StartRowIndex

        SELECT  
            @FirstTimeUTC = [TimeUtc],
            @FirstSequence = [Sequence]
        FROM 
            [ELMAH_Error]
        WHERE   
            [Application] = @Application
        ORDER BY 
            [TimeUtc] DESC, 
            [Sequence] DESC

    END
    ELSE
    BEGIN

        SET @PageSize = 0

    END

    -- Now set the row count to the requested page size and get
    -- all records below it for the pertaining application.

    SET ROWCOUNT @PageSize

    SELECT 
        errorId     = [ErrorId], 
        application = [Application],
        host        = [Host], 
        type        = [Type],
        source      = [Source],
        message     = [Message],
        [user]      = [User],
        statusCode  = [StatusCode], 
        time        = CONVERT(VARCHAR(50), [TimeUtc], 126) + 'Z'
    FROM 
        [ELMAH_Error] error
    WHERE
        [Application] = @Application
    AND
        [TimeUtc] <= @FirstTimeUTC
    AND 
        [Sequence] <= @FirstSequence
    ORDER BY
        [TimeUtc] DESC, 
        [Sequence] DESC
    FOR
        XML AUTO


GO
/****** Object:  StoredProcedure [dbo].[ELMAH_GetErrorXml]    Script Date: 2014.02.01. 11:22:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ELMAH_GetErrorXml]
(
    @Application NVARCHAR(60),
    @ErrorId UNIQUEIDENTIFIER
)
AS

    SET NOCOUNT ON

    SELECT 
        [AllXml]
    FROM 
        [ELMAH_Error]
    WHERE
        [ErrorId] = @ErrorId
    AND
        [Application] = @Application


GO
/****** Object:  StoredProcedure [dbo].[ELMAH_LogError]    Script Date: 2014.02.01. 11:22:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ELMAH_LogError]
(
    @ErrorId UNIQUEIDENTIFIER,
    @Application NVARCHAR(60),
    @Host NVARCHAR(30),
    @Type NVARCHAR(100),
    @Source NVARCHAR(60),
    @Message NVARCHAR(500),
    @User NVARCHAR(50),
    @AllXml NTEXT,
    @StatusCode INT,
    @TimeUtc DATETIME
)
AS

    SET NOCOUNT ON

    INSERT
    INTO
        [ELMAH_Error]
        (
            [ErrorId],
            [Application],
            [Host],
            [Type],
            [Source],
            [Message],
            [User],
            [AllXml],
            [StatusCode],
            [TimeUtc]
        )
    VALUES
        (
            @ErrorId,
            @Application,
            @Host,
            @Type,
            @Source,
            @Message,
            @User,
            @AllXml,
            @StatusCode,
            @TimeUtc
        )


GO
/****** Object:  Table [dbo].[Category]    Script Date: 2014.02.01. 11:22:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Category](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[DisplayName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Category] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_Category_DisplayName] UNIQUE NONCLUSTERED 
(
	[DisplayName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_Category_Name] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ELMAH_Error]    Script Date: 2014.02.01. 11:22:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ELMAH_Error](
	[ErrorId] [uniqueidentifier] NOT NULL,
	[Application] [nvarchar](60) NOT NULL,
	[Host] [nvarchar](50) NOT NULL,
	[Type] [nvarchar](100) NOT NULL,
	[Source] [nvarchar](60) NOT NULL,
	[Message] [nvarchar](500) NOT NULL,
	[User] [nvarchar](50) NOT NULL,
	[StatusCode] [int] NOT NULL,
	[TimeUtc] [datetime] NOT NULL,
	[Sequence] [int] IDENTITY(1,1) NOT NULL,
	[AllXml] [ntext] NOT NULL,
 CONSTRAINT [PK_ELMAH_Error] PRIMARY KEY NONCLUSTERED 
(
	[ErrorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SummaryItem]    Script Date: 2014.02.01. 11:22:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SummaryItem](
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
/****** Object:  Table [dbo].[TransactionItem]    Script Date: 2014.02.01. 11:22:38 ******/
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
	[CategoryID] [int] NOT NULL,
	[IsExpenseItem] [bit] NOT NULL,
	[IsIncomeItem] [bit] NOT NULL,
 CONSTRAINT [PK_TransactionItem] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Unit]    Script Date: 2014.02.01. 11:22:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Unit](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](20) NOT NULL,
	[DisplayName] [nvarchar](20) NOT NULL,
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
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_ELMAH_Error_App_Time_Seq]    Script Date: 2014.02.01. 11:22:38 ******/
CREATE NONCLUSTERED INDEX [IX_ELMAH_Error_App_Time_Seq] ON [dbo].[ELMAH_Error]
(
	[Application] ASC,
	[TimeUtc] DESC,
	[Sequence] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [UK_IX_SummaryItem_Date_CategoryID]    Script Date: 2014.02.01. 11:22:38 ******/
CREATE UNIQUE NONCLUSTERED INDEX [UK_IX_SummaryItem_Date_CategoryID] ON [dbo].[SummaryItem]
(
	[Date] ASC,
	[CategoryID] ASC
)
INCLUDE ( 	[ID],
	[Amount]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

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
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [UK_Unit_DisplayName]    Script Date: 2014.02.01. 11:22:38 ******/
CREATE NONCLUSTERED INDEX [UK_Unit_DisplayName] ON [dbo].[Unit]
(
	[DisplayName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ELMAH_Error] ADD  CONSTRAINT [DF_ELMAH_Error_ErrorId]  DEFAULT (newid()) FOR [ErrorId]
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
ALTER TABLE [dbo].[TransactionItem]  WITH CHECK ADD  CONSTRAINT [CK_TransactionItem_Amount_bte_0] CHECK  (([Amount]>=(0)))
GO
ALTER TABLE [dbo].[TransactionItem] CHECK CONSTRAINT [CK_TransactionItem_Amount_bte_0]
GO
ALTER TABLE [dbo].[TransactionItem]  WITH CHECK ADD  CONSTRAINT [CK_TransactionItem_IsExpenseItem_XOR_IsIncomeItem] CHECK  ((([IsExpenseItem]^[IsIncomeItem])=(1)))
GO
ALTER TABLE [dbo].[TransactionItem] CHECK CONSTRAINT [CK_TransactionItem_IsExpenseItem_XOR_IsIncomeItem]
GO
ALTER TABLE [dbo].[TransactionItem]  WITH CHECK ADD  CONSTRAINT [CK_TransactionItem_Quantity_bte_0] CHECK  (([Quantity]>=(0)))
GO
ALTER TABLE [dbo].[TransactionItem] CHECK CONSTRAINT [CK_TransactionItem_Quantity_bte_0]
GO
USE [master]
GO
ALTER DATABASE [ExinDeveloper] SET  READ_WRITE 
GO

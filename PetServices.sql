USE [master]
GO
/****** Object:  Database [PetServices]    Script Date: 12/24/2023 8:49:46 PM ******/
CREATE DATABASE [PetServices]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'PetServices', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\DATA\PetServices.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'PetServices_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\DATA\PetServices_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [PetServices] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [PetServices].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [PetServices] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [PetServices] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [PetServices] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [PetServices] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [PetServices] SET ARITHABORT OFF 
GO
ALTER DATABASE [PetServices] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [PetServices] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [PetServices] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [PetServices] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [PetServices] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [PetServices] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [PetServices] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [PetServices] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [PetServices] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [PetServices] SET  ENABLE_BROKER 
GO
ALTER DATABASE [PetServices] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [PetServices] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [PetServices] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [PetServices] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [PetServices] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [PetServices] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [PetServices] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [PetServices] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [PetServices] SET  MULTI_USER 
GO
ALTER DATABASE [PetServices] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [PetServices] SET DB_CHAINING OFF 
GO
ALTER DATABASE [PetServices] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [PetServices] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [PetServices] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [PetServices] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [PetServices] SET QUERY_STORE = ON
GO
ALTER DATABASE [PetServices] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [PetServices]
GO
/****** Object:  Table [dbo].[Accounts]    Script Date: 12/24/2023 8:49:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Accounts](
	[AccountID] [int] IDENTITY(1,1) NOT NULL,
	[Email] [varchar](100) NOT NULL,
	[Password] [varchar](100) NOT NULL,
	[Status] [bit] NOT NULL,
	[UserInfoID] [int] NULL,
	[PartnerInfoID] [int] NULL,
	[RoleID] [int] NULL,
	[OTPID] [int] NULL,
	[CreateDate] [date] NULL,
 CONSTRAINT [PK_Accounts] PRIMARY KEY CLUSTERED 
(
	[AccountID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Blogs]    Script Date: 12/24/2023 8:49:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Blogs](
	[BlogID] [int] IDENTITY(1,1) NOT NULL,
	[Content] [nvarchar](max) NULL,
	[Heading] [nvarchar](max) NULL,
	[PageTile] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[ImageURL] [nvarchar](max) NULL,
	[PublisheDate] [date] NULL,
	[Status] [bit] NULL,
	[TagID] [int] NULL,
 CONSTRAINT [PK_Blogs] PRIMARY KEY CLUSTERED 
(
	[BlogID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BookingRoomDetail]    Script Date: 12/24/2023 8:49:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BookingRoomDetail](
	[RoomID] [int] NOT NULL,
	[OrderID] [int] NOT NULL,
	[Price] [float] NULL,
	[Note] [nvarchar](max) NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[FeedbackStatus] [bit] NULL,
	[TotalPrice] [float] NULL,
 CONSTRAINT [PK_BookingRoomDetail] PRIMARY KEY CLUSTERED 
(
	[RoomID] ASC,
	[OrderID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BookingRoomServices]    Script Date: 12/24/2023 8:49:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BookingRoomServices](
	[OrderID] [int] NOT NULL,
	[RoomID] [int] NOT NULL,
	[ServiceID] [int] NOT NULL,
	[PriceService] [float] NULL,
 CONSTRAINT [PK_BookingRoomServices] PRIMARY KEY CLUSTERED 
(
	[OrderID] ASC,
	[RoomID] ASC,
	[ServiceID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BookingServicesDetail]    Script Date: 12/24/2023 8:49:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BookingServicesDetail](
	[ServiceID] [int] NOT NULL,
	[OrderID] [int] NOT NULL,
	[Price] [float] NULL,
	[Weight] [float] NULL,
	[PriceService] [float] NULL,
	[PetInfoID] [int] NULL,
	[PartnerInfoID] [int] NULL,
	[StartTime] [datetime] NULL,
	[EndTime] [datetime] NULL,
	[FeedbackPartnerStatus] [bit] NULL,
	[FeedbackStatus] [bit] NULL,
	[StatusOrderService] [nvarchar](200) NULL,
 CONSTRAINT [PK_BookingServicesDetail] PRIMARY KEY CLUSTERED 
(
	[ServiceID] ASC,
	[OrderID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Feedback]    Script Date: 12/24/2023 8:49:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Feedback](
	[FeedbackID] [int] IDENTITY(1,1) NOT NULL,
	[Content] [nvarchar](max) NULL,
	[NumberStart] [int] NULL,
	[ServiceID] [int] NULL,
	[RoomID] [int] NULL,
	[PartnerID] [int] NULL,
	[ProductID] [int] NULL,
	[UserID] [int] NULL,
	[OrderId] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderProductDetail]    Script Date: 12/24/2023 8:49:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderProductDetail](
	[Quantity] [int] NULL,
	[Price] [float] NULL,
	[ProductID] [int] NOT NULL,
	[OrderID] [int] NOT NULL,
	[FeedbackStatus] [bit] NULL,
	[StatusOrderProduct] [nvarchar](200) NULL,
 CONSTRAINT [PK_OrderProductDetail] PRIMARY KEY CLUSTERED 
(
	[ProductID] ASC,
	[OrderID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Orders]    Script Date: 12/24/2023 8:49:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Orders](
	[OrderID] [int] IDENTITY(1,1) NOT NULL,
	[OrderDate] [datetime] NULL,
	[OrderStatus] [nvarchar](500) NULL,
	[Province] [nvarchar](500) NULL,
	[District] [nvarchar](500) NULL,
	[Commune] [nvarchar](500) NULL,
	[Address] [nvarchar](500) NULL,
	[Phone] [varchar](10) NULL,
	[TypePay] [nvarchar](500) NULL,
	[FullName] [nvarchar](500) NULL,
	[UserInfoID] [int] NULL,
	[StatusPayment] [bit] NULL,
	[TotalPrice] [float] NULL,
 CONSTRAINT [PK_Orders] PRIMARY KEY CLUSTERED 
(
	[OrderID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderType]    Script Date: 12/24/2023 8:49:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderType](
	[OrderTypeID] [int] NOT NULL,
	[OrderProduct] [bit] NULL,
	[BookingRoom] [bit] NULL,
	[BookingService] [bit] NULL,
	[OrderID] [int] NULL,
 CONSTRAINT [PK_OrderType] PRIMARY KEY CLUSTERED 
(
	[OrderTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OTPS]    Script Date: 12/24/2023 8:49:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OTPS](
	[OTPID] [int] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](6) NULL,
 CONSTRAINT [PK_OTPS] PRIMARY KEY CLUSTERED 
(
	[OTPID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PartnerInfo]    Script Date: 12/24/2023 8:49:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PartnerInfo](
	[PartnerInfoID] [int] IDENTITY(1,1) NOT NULL,
	[FirstName] [nvarchar](100) NULL,
	[LastName] [nvarchar](100) NULL,
	[Phone] [varchar](10) NULL,
	[Province] [nvarchar](500) NULL,
	[District] [nvarchar](500) NULL,
	[Commune] [nvarchar](500) NULL,
	[Address] [nvarchar](1000) NULL,
	[Descriptions] [nvarchar](max) NULL,
	[CardNumber] [varchar](100) NULL,
	[ImagePartner] [varchar](max) NULL,
	[ImageCertificate] [varchar](max) NULL,
	[CardName] [varchar](100) NULL,
	[Lat] [nvarchar](500) NULL,
	[Lng] [nvarchar](500) NULL,
	[Dob] [date] NULL,
 CONSTRAINT [PK_PartnerInfo] PRIMARY KEY CLUSTERED 
(
	[PartnerInfoID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Payment]    Script Date: 12/24/2023 8:49:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Payment](
	[PaymentID] [int] IDENTITY(1,1) NOT NULL,
	[Salary] [float] NULL,
	[DateSalary] [datetime] NULL,
	[StatusSalary] [bit] NULL,
	[PartnerInfoID] [int] NULL,
 CONSTRAINT [PK_Payment] PRIMARY KEY CLUSTERED 
(
	[PaymentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PetInfo]    Script Date: 12/24/2023 8:49:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PetInfo](
	[PetInfoID] [int] IDENTITY(1,1) NOT NULL,
	[PetName] [nvarchar](500) NULL,
	[ImagePet] [varchar](max) NULL,
	[Species] [nvarchar](500) NULL,
	[Gender] [bit] NULL,
	[Descriptions] [nvarchar](max) NULL,
	[UserInfoID] [int] NULL,
	[Weight] [float] NULL,
	[Dob] [date] NULL,
 CONSTRAINT [PK_PetInfo] PRIMARY KEY CLUSTERED 
(
	[PetInfoID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Product]    Script Date: 12/24/2023 8:49:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Product](
	[ProductID] [int] IDENTITY(1,1) NOT NULL,
	[ProductName] [nvarchar](500) NULL,
	[Desciption] [nvarchar](max) NULL,
	[Picture] [varchar](max) NULL,
	[Status] [bit] NULL,
	[Price] [float] NULL,
	[CreateDate] [date] NULL,
	[UpdateDate] [date] NULL,
	[ProCategoriesID] [int] NULL,
	[Quantity] [int] NULL,
	[QuantitySold] [int] NULL,
 CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED 
(
	[ProductID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProductCategories]    Script Date: 12/24/2023 8:49:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductCategories](
	[ProCategoriesID] [int] IDENTITY(1,1) NOT NULL,
	[ProCategoriesName] [nvarchar](500) NULL,
	[Desciptions] [nvarchar](500) NULL,
	[Picture] [nvarchar](500) NULL,
	[Status] [bit] NULL,
 CONSTRAINT [PK_ProductCategories] PRIMARY KEY CLUSTERED 
(
	[ProCategoriesID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Reason]    Script Date: 12/24/2023 8:49:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Reason](
	[ReasonID] [int] IDENTITY(1,1) NOT NULL,
	[ReasonTitle] [nvarchar](max) NULL,
	[ReasonDescription] [nvarchar](max) NULL,
 CONSTRAINT [PK_Reason] PRIMARY KEY CLUSTERED 
(
	[ReasonID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ReasonOrders]    Script Date: 12/24/2023 8:49:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReasonOrders](
	[ReasonOrderID] [int] IDENTITY(1,1) NOT NULL,
	[ReasonOrderTitle] [nvarchar](max) NULL,
	[ReasonOrderDescription] [nvarchar](max) NULL,
	[OrderID] [int] NULL,
	[EmailReject] [varchar](100) NULL,
	[RejectTime] [datetime] NULL,
 CONSTRAINT [PK_ReasonOrders] PRIMARY KEY CLUSTERED 
(
	[ReasonOrderID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Roles]    Script Date: 12/24/2023 8:49:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Roles](
	[RoleID] [int] IDENTITY(1,1) NOT NULL,
	[RoleName] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED 
(
	[RoleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Room]    Script Date: 12/24/2023 8:49:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Room](
	[RoomID] [int] IDENTITY(1,1) NOT NULL,
	[RoomName] [nvarchar](500) NULL,
	[Desciptions] [nvarchar](max) NULL,
	[Status] [bit] NULL,
	[Picture] [varchar](max) NULL,
	[Price] [float] NULL,
	[RoomCategoriesID] [int] NULL,
	[Slot] [int] NULL,
 CONSTRAINT [PK_Room] PRIMARY KEY CLUSTERED 
(
	[RoomID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RoomCategories]    Script Date: 12/24/2023 8:49:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RoomCategories](
	[RoomCategoriesID] [int] IDENTITY(1,1) NOT NULL,
	[RoomCategoriesName] [nvarchar](500) NULL,
	[Desciptions] [nvarchar](max) NULL,
	[Picture] [nvarchar](500) NULL,
	[Status] [bit] NULL,
 CONSTRAINT [PK_RoomCategories] PRIMARY KEY CLUSTERED 
(
	[RoomCategoriesID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RoomServices]    Script Date: 12/24/2023 8:49:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RoomServices](
	[RoomID] [int] NOT NULL,
	[ServiceID] [int] NOT NULL,
 CONSTRAINT [PK_RoomServices] PRIMARY KEY CLUSTERED 
(
	[RoomID] ASC,
	[ServiceID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ServiceCategories]    Script Date: 12/24/2023 8:49:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ServiceCategories](
	[SerCategoriesID] [int] IDENTITY(1,1) NOT NULL,
	[SerCategoriesName] [nvarchar](500) NULL,
	[Desciptions] [nvarchar](max) NULL,
	[Picture] [nvarchar](500) NULL,
	[Status] [bit] NULL,
 CONSTRAINT [PK_ServiceCategories] PRIMARY KEY CLUSTERED 
(
	[SerCategoriesID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Services]    Script Date: 12/24/2023 8:49:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Services](
	[ServiceID] [int] IDENTITY(1,1) NOT NULL,
	[ServiceName] [nvarchar](500) NULL,
	[Desciptions] [nvarchar](max) NULL,
	[Status] [bit] NULL,
	[Time] [float] NULL,
	[Picture] [varchar](max) NULL,
	[Price] [float] NULL,
	[SerCategoriesID] [int] NULL,
 CONSTRAINT [PK_Services] PRIMARY KEY CLUSTERED 
(
	[ServiceID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Tags]    Script Date: 12/24/2023 8:49:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tags](
	[TagID] [int] IDENTITY(1,1) NOT NULL,
	[TagName] [nvarchar](max) NULL,
	[Status] [bit] NULL,
 CONSTRAINT [PK_Tags] PRIMARY KEY CLUSTERED 
(
	[TagID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserInfo]    Script Date: 12/24/2023 8:49:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserInfo](
	[UserInfoID] [int] IDENTITY(1,1) NOT NULL,
	[FirstName] [nvarchar](100) NULL,
	[LastName] [nvarchar](100) NULL,
	[Phone] [varchar](10) NULL,
	[Province] [nvarchar](500) NULL,
	[District] [nvarchar](500) NULL,
	[Commune] [nvarchar](500) NULL,
	[Address] [nvarchar](1000) NULL,
	[Descriptions] [nvarchar](max) NULL,
	[ImageUser] [varchar](max) NULL,
	[Dob] [date] NULL,
 CONSTRAINT [PK_UserInfo] PRIMARY KEY CLUSTERED 
(
	[UserInfoID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[Accounts]  WITH CHECK ADD  CONSTRAINT [FK_Accounts_OTPS] FOREIGN KEY([OTPID])
REFERENCES [dbo].[OTPS] ([OTPID])
GO
ALTER TABLE [dbo].[Accounts] CHECK CONSTRAINT [FK_Accounts_OTPS]
GO
ALTER TABLE [dbo].[Accounts]  WITH CHECK ADD  CONSTRAINT [FK_Accounts_PartnerInfo] FOREIGN KEY([PartnerInfoID])
REFERENCES [dbo].[PartnerInfo] ([PartnerInfoID])
GO
ALTER TABLE [dbo].[Accounts] CHECK CONSTRAINT [FK_Accounts_PartnerInfo]
GO
ALTER TABLE [dbo].[Accounts]  WITH CHECK ADD  CONSTRAINT [FK_Accounts_Roles] FOREIGN KEY([RoleID])
REFERENCES [dbo].[Roles] ([RoleID])
GO
ALTER TABLE [dbo].[Accounts] CHECK CONSTRAINT [FK_Accounts_Roles]
GO
ALTER TABLE [dbo].[Accounts]  WITH CHECK ADD  CONSTRAINT [FK_Accounts_UserInfo] FOREIGN KEY([UserInfoID])
REFERENCES [dbo].[UserInfo] ([UserInfoID])
GO
ALTER TABLE [dbo].[Accounts] CHECK CONSTRAINT [FK_Accounts_UserInfo]
GO
ALTER TABLE [dbo].[Blogs]  WITH CHECK ADD  CONSTRAINT [FK_Blogs_Tags] FOREIGN KEY([TagID])
REFERENCES [dbo].[Tags] ([TagID])
GO
ALTER TABLE [dbo].[Blogs] CHECK CONSTRAINT [FK_Blogs_Tags]
GO
ALTER TABLE [dbo].[BookingRoomDetail]  WITH CHECK ADD  CONSTRAINT [FK_BookingRoomDetail_Orders] FOREIGN KEY([OrderID])
REFERENCES [dbo].[Orders] ([OrderID])
GO
ALTER TABLE [dbo].[BookingRoomDetail] CHECK CONSTRAINT [FK_BookingRoomDetail_Orders]
GO
ALTER TABLE [dbo].[BookingRoomDetail]  WITH CHECK ADD  CONSTRAINT [FK_BookingRoomDetail_Room] FOREIGN KEY([RoomID])
REFERENCES [dbo].[Room] ([RoomID])
GO
ALTER TABLE [dbo].[BookingRoomDetail] CHECK CONSTRAINT [FK_BookingRoomDetail_Room]
GO
ALTER TABLE [dbo].[BookingRoomServices]  WITH CHECK ADD  CONSTRAINT [FK_BookingRoomServices_Orders] FOREIGN KEY([OrderID])
REFERENCES [dbo].[Orders] ([OrderID])
GO
ALTER TABLE [dbo].[BookingRoomServices] CHECK CONSTRAINT [FK_BookingRoomServices_Orders]
GO
ALTER TABLE [dbo].[BookingRoomServices]  WITH CHECK ADD  CONSTRAINT [FK_BookingRoomServices_Room] FOREIGN KEY([RoomID])
REFERENCES [dbo].[Room] ([RoomID])
GO
ALTER TABLE [dbo].[BookingRoomServices] CHECK CONSTRAINT [FK_BookingRoomServices_Room]
GO
ALTER TABLE [dbo].[BookingRoomServices]  WITH CHECK ADD  CONSTRAINT [FK_BookingRoomServices_Services] FOREIGN KEY([ServiceID])
REFERENCES [dbo].[Services] ([ServiceID])
GO
ALTER TABLE [dbo].[BookingRoomServices] CHECK CONSTRAINT [FK_BookingRoomServices_Services]
GO
ALTER TABLE [dbo].[BookingServicesDetail]  WITH CHECK ADD  CONSTRAINT [FK_BookingServicesDetail_Orders] FOREIGN KEY([OrderID])
REFERENCES [dbo].[Orders] ([OrderID])
GO
ALTER TABLE [dbo].[BookingServicesDetail] CHECK CONSTRAINT [FK_BookingServicesDetail_Orders]
GO
ALTER TABLE [dbo].[BookingServicesDetail]  WITH CHECK ADD  CONSTRAINT [FK_BookingServicesDetail_PartnerInfo] FOREIGN KEY([PartnerInfoID])
REFERENCES [dbo].[PartnerInfo] ([PartnerInfoID])
GO
ALTER TABLE [dbo].[BookingServicesDetail] CHECK CONSTRAINT [FK_BookingServicesDetail_PartnerInfo]
GO
ALTER TABLE [dbo].[BookingServicesDetail]  WITH CHECK ADD  CONSTRAINT [FK_BookingServicesDetail_PetInfo] FOREIGN KEY([PetInfoID])
REFERENCES [dbo].[PetInfo] ([PetInfoID])
GO
ALTER TABLE [dbo].[BookingServicesDetail] CHECK CONSTRAINT [FK_BookingServicesDetail_PetInfo]
GO
ALTER TABLE [dbo].[BookingServicesDetail]  WITH CHECK ADD  CONSTRAINT [FK_BookingServicesDetail_Services] FOREIGN KEY([ServiceID])
REFERENCES [dbo].[Services] ([ServiceID])
GO
ALTER TABLE [dbo].[BookingServicesDetail] CHECK CONSTRAINT [FK_BookingServicesDetail_Services]
GO
ALTER TABLE [dbo].[OrderProductDetail]  WITH CHECK ADD  CONSTRAINT [FK_OrderProductDetail_Orders] FOREIGN KEY([OrderID])
REFERENCES [dbo].[Orders] ([OrderID])
GO
ALTER TABLE [dbo].[OrderProductDetail] CHECK CONSTRAINT [FK_OrderProductDetail_Orders]
GO
ALTER TABLE [dbo].[OrderProductDetail]  WITH CHECK ADD  CONSTRAINT [FK_OrderProductDetail_Product] FOREIGN KEY([ProductID])
REFERENCES [dbo].[Product] ([ProductID])
GO
ALTER TABLE [dbo].[OrderProductDetail] CHECK CONSTRAINT [FK_OrderProductDetail_Product]
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [FK_Orders_UserInfo] FOREIGN KEY([UserInfoID])
REFERENCES [dbo].[UserInfo] ([UserInfoID])
GO
ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK_Orders_UserInfo]
GO
ALTER TABLE [dbo].[OrderType]  WITH CHECK ADD  CONSTRAINT [FK_OrderType_Orders] FOREIGN KEY([OrderID])
REFERENCES [dbo].[Orders] ([OrderID])
GO
ALTER TABLE [dbo].[OrderType] CHECK CONSTRAINT [FK_OrderType_Orders]
GO
ALTER TABLE [dbo].[Payment]  WITH CHECK ADD  CONSTRAINT [FK_Payment_PartnerInfo] FOREIGN KEY([PartnerInfoID])
REFERENCES [dbo].[PartnerInfo] ([PartnerInfoID])
GO
ALTER TABLE [dbo].[Payment] CHECK CONSTRAINT [FK_Payment_PartnerInfo]
GO
ALTER TABLE [dbo].[PetInfo]  WITH CHECK ADD  CONSTRAINT [FK_PetInfo_UserInfo] FOREIGN KEY([UserInfoID])
REFERENCES [dbo].[UserInfo] ([UserInfoID])
GO
ALTER TABLE [dbo].[PetInfo] CHECK CONSTRAINT [FK_PetInfo_UserInfo]
GO
ALTER TABLE [dbo].[Product]  WITH CHECK ADD  CONSTRAINT [FK_Product_ProductCategories] FOREIGN KEY([ProCategoriesID])
REFERENCES [dbo].[ProductCategories] ([ProCategoriesID])
GO
ALTER TABLE [dbo].[Product] CHECK CONSTRAINT [FK_Product_ProductCategories]
GO
ALTER TABLE [dbo].[ReasonOrders]  WITH CHECK ADD  CONSTRAINT [FK_ReasonOrders_Orders] FOREIGN KEY([OrderID])
REFERENCES [dbo].[Orders] ([OrderID])
GO
ALTER TABLE [dbo].[ReasonOrders] CHECK CONSTRAINT [FK_ReasonOrders_Orders]
GO
ALTER TABLE [dbo].[Room]  WITH CHECK ADD  CONSTRAINT [FK_Room_RoomCategories] FOREIGN KEY([RoomCategoriesID])
REFERENCES [dbo].[RoomCategories] ([RoomCategoriesID])
GO
ALTER TABLE [dbo].[Room] CHECK CONSTRAINT [FK_Room_RoomCategories]
GO
ALTER TABLE [dbo].[RoomServices]  WITH CHECK ADD  CONSTRAINT [FK_RoomServices_Room] FOREIGN KEY([RoomID])
REFERENCES [dbo].[Room] ([RoomID])
GO
ALTER TABLE [dbo].[RoomServices] CHECK CONSTRAINT [FK_RoomServices_Room]
GO
ALTER TABLE [dbo].[RoomServices]  WITH CHECK ADD  CONSTRAINT [FK_RoomServices_Services] FOREIGN KEY([ServiceID])
REFERENCES [dbo].[Services] ([ServiceID])
GO
ALTER TABLE [dbo].[RoomServices] CHECK CONSTRAINT [FK_RoomServices_Services]
GO
ALTER TABLE [dbo].[Services]  WITH CHECK ADD  CONSTRAINT [FK_Services_ServiceCategories] FOREIGN KEY([SerCategoriesID])
REFERENCES [dbo].[ServiceCategories] ([SerCategoriesID])
GO
ALTER TABLE [dbo].[Services] CHECK CONSTRAINT [FK_Services_ServiceCategories]
GO
USE [master]
GO
ALTER DATABASE [PetServices] SET  READ_WRITE 
GO

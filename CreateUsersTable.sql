USE [jersyMarketDB]
GO

/****** Object:  Table [dbo].[Users]    Script Date: 30/08/2026 1:07:45 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Users](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[Username] [nvarchar](450) NOT NULL,
	[Password] [nvarchar](max) NOT NULL,
	[RefreshToken] [nvarchar](max) NULL,
	[RefreshTokenExpiryTime] [datetime2](7) NOT NULL,
	[LastLogin] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO



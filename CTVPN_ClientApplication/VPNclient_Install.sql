USE [master]
GO

/* Create Database */
CREATE DATABASE [VPNClients]
GO

/* Create Database user */
--EXEC sp_addlogin '<user_name>','<password>'
--GO

USE [VPNClients]
GO

EXEC sp_grantdbaccess '<user_name>'
GO

EXEC sp_addrolemember 'db_owner', '<user_name>'
GO

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO

/* Create Stored Procedures */
--CREATE PROCEDURE spVPNClients_CreateClient
--	@guid nchar(108),	
--	@name varchar(50),
--	@connect bit,
--	@lastcheckin DateTime,
--	@user varchar(50),
--	@password varchar(50),
--	@cnxName varchar(50)

--AS
--BEGIN
--	SET NOCOUNT ON;

--	INSERT INTO VPNclients
--	VALUES (@guid, @name, @connect, @lastcheckin, @cnxName, @user, @password)

--END
--GO

--CREATE PROCEDURE spVPNclients_GetClients 

--AS
--BEGIN
--	SET NOCOUNT ON;
	
--	SELECT * FROM VPNclients

--END
--GO

/* Create Tables */
CREATE TABLE [dbo].[VPNClients] (
	[id] [int] IDENTITY (1, 1) NOT NULL ,
	[guid] [nchar] (108) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[name] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[connect] [bit] NOT NULL ,
	[lastcheckin] [datetime] NOT NULL ,
	[cnxName] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[cnxUser] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[cnxPassword] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL 
) ON [PRIMARY]
GO

--CREATE TABLE [dbo].[Log] (
--	[id] [int] IDENTITY (1, 1) NOT NULL ,
--	[message] [varchar] (2000) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL 
--) ON [PRIMARY]
--GO


/* Create ID Record in DB */
--DELETE FROM [dbo].[VPNclients]
--GO

/* Insert Identity in DB */
--this is how I distribute clients
SET IDENTITY_INSERT [dbo].[VPNclients] ON
INSERT INTO [dbo].[VPNclients] ([id], [guid], [name], [connect], [lastcheckin], [cnxName], [cnxUser], [cnxPassword])
VALUES (1, '<encryped_guid>', '<encryped_name>', 0, '2000/01/01', '<encryped_cnxName>', '<encryped_cnxUser>', '<encryped_cnxPassword>')
/*
Each client got it's own build and I've removed the individual identities here.
*/
SET IDENTITY_INSERT [dbo].[VPNClients] OFF

USE [master]
GO
USE master
GO

IF EXISTS (SELECT * FROM SYSDATABASES WHERE name = 'FEEDREADER')
DROP DATABASE FEEDREADER
GO

CREATE DATABASE FEEDREADER
GO

USE FEEDREADER
GO

CREATE TABLE UserInfo(
	UserInfoID int primary key identity(1,1),
	FirstName nvarchar(20) null,
	LastName nvarchar(20) null,
	UserName nvarchar(20),
	Email nvarchar(20),
	AspNetID nvarchar(128) null
)
GO

CREATE TABLE Feed(
	FeedID int primary key identity(1,1),
	Url nvarchar(128) not null,
	CategoryID int null,
	[Description] nvarchar(128) null,
	LastBuildDate date null,
	Subscribers int null,
	Title nvarchar(35) not null,
	SiteUrl nvarchar(128) null
)
GO

CREATE TABLE Category(
	CategoryID int primary key identity(1,1),
	Name nvarchar(30)
)
GO

CREATE TABLE UserInfoFeeds(
	UserInfoFeedsID int primary key identity(1,1),
	UserInfoID int,
	FeedID int
)
GO

ALTER TABLE Feed
	ADD CONSTRAINT FK_FeedCategory FOREIGN KEY (CategoryID)
	REFERENCES Category(CategoryID)
GO

ALTER TABLE UserInfoFeeds
	ADD CONSTRAINT FK_UserInfoFeedsFeed FOREIGN KEY (FeedID)
	REFERENCES Feed(FeedID)
GO

ALTER TABLE UserInfoFeeds
	ADD CONSTRAINT FK_UserInfoFeedsUserInfo FOREIGN KEY (UserInfoID)
	REFERENCES UserInfo(UserInfoID)
GO

ALTER TABLE UserInfoFeeds
	ADD CONSTRAINT UC_FeedIDUserInfoID UNIQUE (FeedID, UserInfoID)
GO

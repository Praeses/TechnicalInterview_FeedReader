USE FEEDREADER
GO

CREATE PROCEDURE FeedGetAllByUserId(
	@AspNetId nvarchar(128)
) AS
SELECT f.Url, f.FeedID, f.SiteUrl, f.Subscribers
FROM UserInfo u
	INNER JOIN UserInfoFeeds uf ON uf.UserInfoID = u.UserInfoID
	INNER JOIN Feed f ON f.FeedID = uf.FeedID
WHERE u.AspNetID = @AspNetId
GO

INSERT INTO Feed(Url, SiteUrl, Title, [Description], LastBuildDate, Subscribers)
VALUES ('http://feeds.guardian.co.uk/theguardian/world/rss', 'http://www.theguardian.com/world', 'World news | The Guardian', 
			'Latest World news news, comment and analysis from the Guardian, the world''s leading liberal voice', '11/23/2014 6:53:30 PM', 3),
      ('http://www.nytimes.com/services/xml/rss/nyt/GlobalHome.xml', 'http://www.nytimes.com/pages/international/index.html?partner=rss&emc=rss',
			'NYT > International Home', NULL, '11/23/2014 5:33:03 PM', 2),
      ('http://feeds.bbci.co.uk/news/world/rss.xml', 'http://www.bbc.co.uk/news/world/#sa-ns_mchannel=rss&ns_source=PublicRSS20-sa',
			'BBC News - World', 'The latest stories from the World section of the BBC News web site.', '11/23/2014 6:53:20 PM', 3),
	  ('http://www.mlssoccer.com/rss/en.xml', 'http://www.mlssoccer.com', 'MLSsoccer.com News', 'Latest Articles', NULL, 5),	  
	  ('http://feeds.hanselman.com/ScottHanselman', 'http://www.hanselman.com/blog/', 'Scott Hanselman''s Blog',
			'Scott Hanselman on Programming, User Experience, The Zen of Computers and Life in General', '11/19/2014 6:57:21 PM', 3),
	  ('http://syndication.thedailywtf.com/TheDailyWtf', 'http://thedailywtf.com/', 'The Daily WTF', 
			'Curious Perversions in Information Technology', NULL, 0)	  
GO

INSERT INTO UserInfoFeeds(UserInfoID, FeedID)
VALUES (1,1),(1,2),(1,4),(1,5),(2,4),(2,5),(3,1),(1,3),
		(3,3),(3,4),(3,5),(4,2),(5,3),(5,4),(6,4),(7,1)
GO

CREATE PROCEDURE FeedSearch(
	@AspNetID nvarchar(128),
	@SearchEntry nvarchar(60)
)
AS
BEGIN TRANSACTION

DECLARE @UserInfoID int
SET @UserInfoID = (SELECT UserInfoID
					FROM UserInfo u
					WHERE u.AspNetID = @AspNetID)

SELECT DISTINCT f.Title, f.[Description], f.Url, f.SiteUrl, f.Subscribers, f.FeedID
FROM Feed f
	LEFT JOIN UserInfoFeeds uif ON uif.FeedID = f.FeedID
	LEFT JOIN UserInfo u ON u.UserInfoID = uif.UserInfoID
	WHERE f.FeedID NOT IN (
		SELECT FeedID
		FROM UserInfoFeeds uif
		WHERE uif.UserInfoID = @UserInfoID)
		AND (f.Title LIKE '%' + @SearchEntry + '%'
			OR  f.[Description] LIKE '%' + @SearchEntry + '%'
			OR  f.Url LIKE '%' + @SearchEntry + '%'
			OR  f.SiteUrl LIKE '%' + @SearchEntry + '%')
ORDER BY f.Subscribers DESC

COMMIT TRANSACTION
GO

CREATE PROCEDURE FeedInsert(
	@Url nvarchar(128),
	@Description nvarchar(128),
	@LastBuildDate date,
	@Subscribers int,
	@Title nvarchar(35),
	@SiteUrl nvarchar(128)
) AS
BEGIN TRANSACTION
INSERT INTO Feed(Url, SiteUrl, Title, [Description], LastBuildDate, Subscribers)
VALUES (@Url, @SiteUrl, @Title, @Description, @LastBuildDate, @Subscribers)
SELECT SCOPE_IDENTITY();
COMMIT TRANSACTION
GO

CREATE PROCEDURE UpdateUserInfoFeeds(
	@AspNetId nvarchar(128),
	@FeedId int
) AS
BEGIN TRANSACTION
DECLARE @UserInfoID int
SET @UserInfoID = (SELECT UserInfoID
					FROM UserInfo u
					WHERE u.AspNetID = @AspNetID)

INSERT INTO UserInfoFeeds(UserInfoID, FeedID)
VALUES(@UserInfoID, @FeedId)

UPDATE Feed
SET Subscribers = Subscribers + 1
	WHERE FeedID = @FeedId

COMMIT TRANSACTION
GO

CREATE PROCEDURE RemoveUserInfoFeeds(
	@AspNetId nvarchar(128),
	@FeedId int
) AS
BEGIN TRANSACTION
DECLARE @UserInfoID int
SET @UserInfoID = (SELECT UserInfoID
					FROM UserInfo u
					WHERE u.AspNetID = @AspNetID)

DELETE FROM UserInfoFeeds
WHERE UserInfoID = @UserInfoID AND
		FeedID = @FeedID

UPDATE Feed
SET Subscribers = Subscribers - 1
	WHERE FeedID = @FeedId

COMMIT TRANSACTION
GO
-- ==============================================================
-- SIH1329: Database Views and Stored Procedures for INT402 (CO5)
-- Platform for Collecting 360-Degree Feedback on Government News
-- ==============================================================

-- 1. View: vw_NewsFeedbackSummary
IF OBJECT_ID('vw_NewsFeedbackSummary', 'V') IS NOT NULL
    DROP VIEW vw_NewsFeedbackSummary;
GO
CREATE VIEW vw_NewsFeedbackSummary AS
SELECT 
    n.Id AS NewsId,
    n.Title,
    c.Name AS CategoryName,
    n.State,
    n.Language,
    n.PublishedDate,
    COUNT(f.Id) AS TotalFeedbacks,
    ISNULL(ROUND(AVG(CAST(f.Rating AS FLOAT)), 2), 0) AS AverageRating,
    SUM(CASE WHEN f.UserSentiment = 1 OR f.DetectedSentiment = 1 THEN 1 ELSE 0 END) AS PositiveCount,
    SUM(CASE WHEN f.UserSentiment = 2 OR f.DetectedSentiment = 2 THEN 1 ELSE 0 END) AS NeutralCount,
    SUM(CASE WHEN f.UserSentiment = 3 OR f.DetectedSentiment = 3 THEN 1 ELSE 0 END) AS NegativeCount
FROM NewsStories n
LEFT JOIN Categories c ON n.CategoryId = c.Id
LEFT JOIN Feedbacks f ON n.Id = f.NewsStoryId AND f.IsDeleted = 0
WHERE n.IsPublished = 1
GROUP BY n.Id, n.Title, c.Name, n.State, n.Language, n.PublishedDate;
GO

-- 2. View: vw_RegionalFeedbackSummary
IF OBJECT_ID('vw_RegionalFeedbackSummary', 'V') IS NOT NULL
    DROP VIEW vw_RegionalFeedbackSummary;
GO
CREATE VIEW vw_RegionalFeedbackSummary AS
SELECT 
    ISNULL(n.State, 'National') AS State,
    COUNT(DISTINCT n.Id) AS NewsCount,
    COUNT(f.Id) AS FeedbackCount,
    ISNULL(ROUND(AVG(CAST(f.Rating AS FLOAT)), 2), 0) AS AverageRating
FROM NewsStories n
LEFT JOIN Feedbacks f ON n.Id = f.NewsStoryId AND f.IsDeleted = 0
WHERE n.IsPublished = 1
GROUP BY n.State;
GO

-- 3. View: vw_SentimentSummary
IF OBJECT_ID('vw_SentimentSummary', 'V') IS NOT NULL
    DROP VIEW vw_SentimentSummary;
GO
CREATE VIEW vw_SentimentSummary AS
SELECT 
    CASE 
        WHEN DetectedSentiment = 1 OR (DetectedSentiment IS NULL AND UserSentiment = 1) THEN 'Positive'
        WHEN DetectedSentiment = 3 OR (DetectedSentiment IS NULL AND UserSentiment = 3) THEN 'Negative'
        ELSE 'Neutral'
    END AS SentimentLabel,
    COUNT(*) AS Count
FROM Feedbacks
WHERE IsDeleted = 0
GROUP BY 
    CASE 
        WHEN DetectedSentiment = 1 OR (DetectedSentiment IS NULL AND UserSentiment = 1) THEN 'Positive'
        WHEN DetectedSentiment = 3 OR (DetectedSentiment IS NULL AND UserSentiment = 3) THEN 'Negative'
        ELSE 'Neutral'
    END;
GO

-- 4. Stored Procedure: sp_GetDashboardKPIs
IF OBJECT_ID('sp_GetDashboardKPIs', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetDashboardKPIs;
GO
CREATE PROCEDURE sp_GetDashboardKPIs
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        (SELECT COUNT(*) FROM AspNetUsers) AS TotalUsers,
        (SELECT COUNT(*) FROM NewsStories WHERE IsPublished = 1) AS TotalNews,
        (SELECT COUNT(*) FROM Feedbacks WHERE IsDeleted = 0) AS TotalFeedbacks,
        (SELECT ISNULL(ROUND(AVG(CAST(Rating AS FLOAT)), 1), 0) FROM Feedbacks WHERE IsDeleted = 0) AS AverageRating,
        (SELECT COUNT(DISTINCT State) FROM NewsStories WHERE State IS NOT NULL AND IsPublished = 1) AS TotalStates,
        (SELECT COUNT(DISTINCT Language) FROM NewsStories WHERE Language IS NOT NULL AND IsPublished = 1) AS TotalLanguages;
END;
GO

-- 5. Stored Procedure: sp_GetRegionalFeedbackAnalytics
IF OBJECT_ID('sp_GetRegionalFeedbackAnalytics', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetRegionalFeedbackAnalytics;
GO
CREATE PROCEDURE sp_GetRegionalFeedbackAnalytics
    @State NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        n.State,
        COUNT(DISTINCT n.Id) AS NewsCount,
        COUNT(f.Id) AS FeedbackCount,
        ISNULL(ROUND(AVG(CAST(f.Rating AS FLOAT)), 2), 0) AS AverageRating
    FROM NewsStories n
    LEFT JOIN Feedbacks f ON n.Id = f.NewsStoryId AND f.IsDeleted = 0
    WHERE n.IsPublished = 1
      AND (@State IS NULL OR n.State = @State)
      AND n.State IS NOT NULL
    GROUP BY n.State
    ORDER BY FeedbackCount DESC;
END;
GO

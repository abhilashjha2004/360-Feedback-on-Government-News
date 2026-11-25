using GovernmentFeedbackPlatform.Models;

namespace GovernmentFeedbackPlatform.Services
{
    public interface ISentimentService
    {
        SentimentType AnalyzeSentiment(string text);
    }
}

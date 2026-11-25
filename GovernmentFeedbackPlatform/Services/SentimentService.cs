using GovernmentFeedbackPlatform.Models;

namespace GovernmentFeedbackPlatform.Services
{
    public class SentimentService : ISentimentService
    {
        private static readonly HashSet<string> _positiveKeywords = new(StringComparer.OrdinalIgnoreCase)
        {
            "good", "great", "excellent", "success", "improvement", "benefit", "helpful",
            "progress", "development", "achievement", "welfare", "positive", "effective",
            "efficient", "transparent"
        };

        private static readonly HashSet<string> _negativeKeywords = new(StringComparer.OrdinalIgnoreCase)
        {
            "bad", "failed", "corrupt", "scam", "waste", "protest", "controversy", "poor",
            "inadequate", "inefficient", "delayed", "mismanaged", "negative", "disappointing",
            "ineffective"
        };

        public SentimentType AnalyzeSentiment(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return SentimentType.Neutral;

            // Split on non-word characters
            var words = text.Split(new char[] { ' ', '\t', '\n', '\r', ',', '.', '!', '?', ';', ':' },
                StringSplitOptions.RemoveEmptyEntries);

            int positive = 0;
            int negative = 0;

            foreach (var word in words)
            {
                var clean = word.Trim('\'', '"', '(', ')', '-');
                if (_positiveKeywords.Contains(clean)) positive++;
                if (_negativeKeywords.Contains(clean)) negative++;
            }

            if (positive > negative) return SentimentType.Positive;
            if (negative > positive) return SentimentType.Negative;
            return SentimentType.Neutral;
        }
    }
}

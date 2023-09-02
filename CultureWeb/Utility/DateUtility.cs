namespace CultureWeb.Utility
{
    public class DateUtility
    {
        public static string FormatTimeAgo(DateTime reviewDate)
        {
            TimeSpan timeSinceReview = DateTime.Now - reviewDate;

            if (timeSinceReview.TotalDays >= 1)
            {
                int days = (int)timeSinceReview.TotalDays;
                return $"{days} {(days > 1 ? "days" : "day")} ago";
            }
            else if (timeSinceReview.TotalHours >= 1)
            {
                int hours = (int)timeSinceReview.TotalHours;
                return $"{hours} {(hours > 1 ? "hours" : "hour")} ago";
            }
            else if (timeSinceReview.TotalMinutes >= 1)
            {
                int minutes = (int)timeSinceReview.TotalMinutes;
                return $"{minutes} {(minutes > 1 ? "minutes" : "minute")} ago";
            }
            else
            {
                return "Just now";
            }
        }
        public static string FormatTimeAgoKh(DateTime reviewDate)
        {
            TimeSpan timeSinceReview = DateTime.Now - reviewDate;

            if (timeSinceReview.TotalDays >= 1)
            {
                int days = (int)timeSinceReview.TotalDays;
                return $"{days} {(days > 1 ? "ថ្ងៃ" : "ថ្ងៃ")} មុន";
            }
            else if (timeSinceReview.TotalHours >= 1)
            {
                int hours = (int)timeSinceReview.TotalHours;
                return $"{hours} {(hours > 1 ? "ម៉ោង" : "ម៉ោង")} មុន";
            }
            else if (timeSinceReview.TotalMinutes >= 1)
            {
                int minutes = (int)timeSinceReview.TotalMinutes;
                return $"{minutes} {(minutes > 1 ? "នាទី" : "នាទី")} មុន";
            }
            else
            {
                return "ឥឡូវនេះ";
            }
        }
    }
}

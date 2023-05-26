namespace TelegramBotApi.Model1
{
    public class Models
    {
        public Results[] Results { get; set; }
        public Models()
        {
            Results = new Results[] { };
        }
    }
    public class Results
    {
        public string Poster_path { get; set; }
        public string Overview { get; set; }
        public string Release_date { get; set; }
        public string Title { get; set; }
        public string Vote_average { get; set; }
    }
    public class MovieActorModel
    {
        public results[] results { get; set; }
        public MovieActorModel()
        {
            results = new results[] { };
        }
    }
    public class results
    {
        public List<Known_for> Known_for { get; set; }
        public string Name { get; set; }
    }
    public class Known_for
    {
        public string title { get; set; }
        public string overview { get; set; }
        public double vote_average { get; set; }
    }
    public class Movie
    {
        public string Movie_name { get; set; }
        public int Movie_rate { get; set; }
        public string Movie_comment { get; set; }
    }
    public class Selected
    {
        public Selected_Array[] Selected_Array { get; set; }
        public Selected()
        {
            Selected_Array = new Selected_Array[] { };
        }
    }
    public class Selected_Array
    {
        public string Movie_name { get; set; }
    }
}

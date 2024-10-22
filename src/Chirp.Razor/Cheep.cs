namespace Chirp.Razor
{

    public class Cheep
    {
        public required int CheepID { get; set; }
        public required int AuthorID { get; set; }
        public required Author Author { get; set; }
        public required string Text { get; set; }
        public DateTime Timestamp { get; set; }
    }

}
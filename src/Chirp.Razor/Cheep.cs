namespace Chirp.Razor
{

    public class Cheep
    {
        public required int CheepId { get; set; }
        public required int AuthorId { get; set; }
        public required Author Author { get; set; }
        public required string Text { get; set; }
        public DateTime TimeStamp { get; set; }
    }

}
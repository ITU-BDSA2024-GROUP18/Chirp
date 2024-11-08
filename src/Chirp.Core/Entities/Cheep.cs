namespace Chirp.Core.Entities
{

    public class Cheep
    {
        public required int CheepId { get; set; }
        public required string Id { get; set; }
        public required Author Author { get; set; }
        public required string Text { get; set; }
        public DateTime TimeStamp { get; set; }
    }

}
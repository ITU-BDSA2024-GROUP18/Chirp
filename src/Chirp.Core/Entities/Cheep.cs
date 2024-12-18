namespace Chirp.Core.Entities
{
    /// <summary>
    /// Represents a Cheep posted by an author.
    /// </summary>
    public class Cheep
    {
        /// <summary>
        /// Unique identifier for the Cheep.
        /// </summary>
        public required int CheepId { get; set; }

        /// <summary>
        /// Identifier of the author who created the Cheep.
        /// </summary>
        public required string AuthorId { get; set; }

        /// <summary>
        /// Author entity associated with the Cheep.
        /// </summary>
        public required Author Author { get; set; }

        /// <summary>
        /// Content of the Cheep.
        /// </summary>
        public required string Text { get; set; }

        /// <summary>
        /// Timestamp of when the Cheep was posted.
        /// </summary>
        public DateTime TimeStamp { get; set; }
    }
}
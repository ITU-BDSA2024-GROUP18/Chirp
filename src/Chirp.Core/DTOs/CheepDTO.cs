namespace Chirp.Core.DTOs;

/// <summary>
/// Represents minimal data for a Cheep transfer.
/// </summary>
public class CheepDTO
{
    /// <summary>
    /// Content of the Cheep message.
    /// </summary>
    public required string Message { get; set; }

    /// <summary>
    /// Name of the author who posted the Cheep.
    /// </summary>
    public required string AuthorName { get; set; }

    /// <summary>
    /// Timestamp of when the Cheep was created.
    /// </summary>
    public required string Timestamp { get; set; }
}
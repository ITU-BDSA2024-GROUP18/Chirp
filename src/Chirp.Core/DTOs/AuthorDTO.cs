namespace Chirp.Core.DTOs;

/// <summary>
/// Represents minimal author data for transfer between layers.
/// </summary>
public class AuthorDTO
{
    /// <summary>
    /// Author's unique username.
    /// </summary>
    public required string Username { get; set; }

    /// <summary>
    /// Author's unique identifier (GUID).
    /// </summary>
    public required string Id { get; set; }
}
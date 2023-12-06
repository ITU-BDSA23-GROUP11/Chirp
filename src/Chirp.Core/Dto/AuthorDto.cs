namespace Chirp.Core.Dto;

public class AuthorDto
{
    public required Guid Id { get; init; }
    public string? Name { get; init; }
    public required string Username { get; init; }
    public required string AvatarUrl { get; init; }
}
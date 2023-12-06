namespace Chirp.Core.Dto;

public class AuthorDto
{
    public required Guid Id { get; init; }
    public string? Name { get; init; }
    public required string Login { get; init; }
    public required string AvatarUrl { get; init; }
}
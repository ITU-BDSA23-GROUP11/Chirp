namespace Chirp.Core.Dto;

public class AuthorDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Email { get; init; }
}
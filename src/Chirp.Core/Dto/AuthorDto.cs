namespace Chirp.Core.Dto;

public class AddAuthorDto
{
    public required string Name { get; init; }
    public required string Email { get; init; }
    //public required IEnumerable<AuthorDto> FollowedBy { get; init; }
}

public class AuthorDto
{
    public required string Name { get; init; }
    public required string Email { get; init; }
    //public required IEnumerable<AuthorDto> FollowedBy { get; init; }
}
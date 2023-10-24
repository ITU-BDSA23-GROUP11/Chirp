namespace Chirp.Core.Dto;

public class AuthorDto
{
    public required string Name;
    public required string Email;
    public ICollection<CheepDto>? Cheeps;
}
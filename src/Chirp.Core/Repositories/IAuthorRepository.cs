using Chirp.Core.Dto;

namespace Chirp.Core.Repositories;

public interface IAuthorRepository
{
    public void AddAuthor(AuthorDto authorDto);
}
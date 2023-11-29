using Chirp.Core.Dto;
using Chirp.Core.Repositories;
using Chirp.Infrastructure.Contexts;
using Chirp.Infrastructure.Models;

namespace Chirp.Infrastructure.Repositories;

public class AuthorRepository : IAuthorRepository
{
    private readonly ChirpDbContext _chirpDbContext;

    public AuthorRepository(ChirpDbContext chirpDbContext)
    {
        _chirpDbContext = chirpDbContext;
    }

    public void AddAuthor(AuthorDto authorDto)
    {
        var authorWithIdExists = _chirpDbContext.Authors.Any(a => a.AuthorId == authorDto.Id);
        
        if (!authorWithIdExists)
        {
            _chirpDbContext.Authors.Add(new Author
            {
                AuthorId = authorDto.Id,
                Name = authorDto.Name,
                Email = authorDto.Email
            });
            _chirpDbContext.SaveChanges();
        }
        
        
    }
}
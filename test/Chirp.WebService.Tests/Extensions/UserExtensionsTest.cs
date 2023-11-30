using System.Security.Claims;
using Chirp.Tests.Core;
using Chirp.WebService.Extensions;
using Moq;

namespace Chirp.WebService.Tests.Extensions;

public class UserExtensionsTest
{
    [Fact]
    public void TestGetUserFullNameAndEmail()
    {
        foreach (DataGenerator.FakeClaims claim in DataGenerator.GenerateUserClaims())
        {
            Mock<ClaimsPrincipal> mockClaims = new Mock<ClaimsPrincipal>();

            List<Claim> claims = new List<Claim>();

            if (claim.GivenName != null)
            {
                claims.Add(new Claim(ClaimTypes.GivenName, claim.GivenName));
            }

            if (claim.Surname != null)
            {
                claims.Add(new Claim(ClaimTypes.Surname, claim.Surname));
            }

            if (claim.Email != null)
            {
                claims.Add(new Claim("emails", claim.Email));
            }
            
            claims.Add(new Claim("http://schemas.microsoft.com/identity/claims/objectidentifier", claim.Id.ToString()));

            mockClaims.SetupGet(cp => cp.Claims).Returns(claims);

            ClaimsPrincipal claimsPrincipal = mockClaims.Object;

            Assert.Equal(claim.GivenName == null || claim.Surname == null ? "No Name Found" : $"{claim.GivenName} {claim.Surname}", claimsPrincipal.GetUserFullName());
            
            Assert.Equal(claim.Email ?? "No Email", claimsPrincipal.GetUserEmail());
            
            Assert.Equal(claim.Id, claimsPrincipal.GetUserId());
        }
    }
}
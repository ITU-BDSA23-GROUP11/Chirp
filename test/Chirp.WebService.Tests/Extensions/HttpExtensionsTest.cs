using Chirp.WebService.Extensions;
using Chirp.WebService.Tests.Utilities;
using Moq;

namespace Chirp.WebService.Tests.Extensions;

public class HttpExtensionsTest
{
    [Fact]
    public void TestGetPathUrl()
    {
        foreach (DataGenerator.FakeUrlOriginAndReferer urlOriginAndReferer in DataGenerator.GenerateUrlOriginAndReferer())
        {
            Mock<HttpRequest> mockHttpRequest = new Mock<HttpRequest>();
            mockHttpRequest.Setup(r => r.Headers["Origin"]).Returns(urlOriginAndReferer.Origin);
            mockHttpRequest.Setup(r => r.Headers["Referer"]).Returns(urlOriginAndReferer.Referer);
            
            Assert.Equal(urlOriginAndReferer.Path, mockHttpRequest.Object.GetPathUrl());
        }
    }
}
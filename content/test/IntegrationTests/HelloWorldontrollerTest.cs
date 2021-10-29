using CoreTemplate.API.Infrastructure.Models;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests
{
    public class HelloWorldontrollerTest : IntegrationTestFixture
    {
        private const string API_BASE_URI = "/HelloWorld";
        public HelloWorldontrollerTest(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }
        [Fact(DisplayName = "HelloWorld", Timeout = 30000)]
        public async Task HelloWorld_ShouldBe_Ok()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Post, API_BASE_URI);

            // Act
            var content = await SendAsync<ApiResponse<string>>(request);

            // Assert
            Assert.True(content.Success);
            Assert.NotNull(content.Result);
            Assert.Equal("hello world!", content.Result);
        }
    }
}

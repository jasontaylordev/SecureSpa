using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Shouldly;

namespace SecureSpa.IntegrationTests
{
    public class WeatherForecastControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public WeatherForecastControllerTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Get_GivenAuthenticatedUser_ReturnsSuccessResult()
        {
            // Arrange
            var client = _factory.CreateClient();

            var token = await GetAccessToken(client, "demouser@securespa", "Pass@word1");
            
            client.SetBearerToken(token);

            // Act
            var response = await client.GetAsync("/weatherforecast/");

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task Get_GivenAnonymousUser_ReturnsUnauthorizedResult()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/weatherforecast/");

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }

        private async Task<string> GetAccessToken(HttpClient client, string userName, string password)
        {
            var disco = await client.GetDiscoveryDocumentAsync();

            if (disco.IsError)
            {
                throw new Exception(disco.Error);
            }

            var response = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "SecureSpa.IntegrationTests",
                ClientSecret = "secret",

                Scope = "SecureSpaAPI openid profile",
                UserName = userName,
                Password = password
            });

            if (response.IsError)
            {
                throw new Exception(response.Error);
            }

            return response.AccessToken;
        }
    }
}

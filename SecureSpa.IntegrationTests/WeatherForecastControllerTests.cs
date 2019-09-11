using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using IdentityServer4;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

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
        public async Task Get_WhenAuthenticated_ReturnsSuccessResult()
        {
            // Arrange
            var client = _factory.CreateClient();

            var token = await GetAccessToken(client);
            
            client.SetBearerToken(token);

            // Act
            var response = await client.GetAsync("/weatherforecast/");

            // Assert
            response.EnsureSuccessStatusCode();
        }

        private async Task<string> GetAccessToken(HttpClient client)
        {
            var disco = await client.GetDiscoveryDocumentAsync();

            if (disco.IsError)
            {
                throw new Exception(disco.Error);
            }

            var response = await client.RequestTokenAsync(request: new TokenRequest
            {
                Address = disco.TokenEndpoint,
                GrantType = IdentityModel.OidcConstants.GrantTypes.ClientCredentials,
                ClientId = "SecureSpa",
                Parameters =
                {
                    { "username", "demouser"},
                    { "password", "Pass@word1"},
                    { "scope", IdentityServerConstants.LocalApi.ScopeName }
                }
            });

            //var response = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
            //{
            //    Address = disco.TokenEndpoint,
            //    ClientId = "SecureSpa",
            //    Scope = "SecureSpaAPI",

            //    UserName = "demouser@securespa",
            //    Password = "Pass@word1"
            //});

            if (response.IsError)
            {
                throw new Exception(response.Error);
            }

            return response.AccessToken;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel.Client;
using IdentityServer4;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace SecureSpa.IntegrationTests
{
    public class WeatherForecastControllerTestsV2 : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public WeatherForecastControllerTestsV2(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Get_WhenAuthenticated_ReturnsSuccessResult()
        {
            // Arrange
            var client = _factory.CreateClient();

            client.SetBearerToken(await GetAccessToken(client));

            // Act
            var response = await client.GetAsync("/weatherforecast/");

            // Assert
            response.EnsureSuccessStatusCode();
        }

        private async Task<string> GetAccessToken(HttpClient client)
        {
            var disco = await client.GetDiscoveryDocumentAsync();
            if (!string.IsNullOrEmpty(disco.Error))
            {
                throw new Exception(disco.Error);
            }

            var response = await client.RequestTokenAsync(new TokenRequest
            {
                Address = disco.TokenEndpoint,
                GrantType = IdentityModel.OidcConstants.GrantTypes.ClientCredentials,
                ClientId = "SecureSpa",

                Parameters =
                {
                    { "username", "demouser@northwind"},
                    { "password", "Pass@word1"},
                    { "scope", IdentityServerConstants.LocalApi.ScopeName }
                }
            });

            if (!string.IsNullOrEmpty(response.Error))
            {
                throw new Exception(response.Error);
            }

            return response.AccessToken;
        }
    }
}

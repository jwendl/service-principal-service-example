using Microsoft.Graph;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CreateServicePrincipal
{
    class Program
    {
        private static readonly string ClientSideClientId = "";
        private static readonly string ClientSideTenantId = "";
        private static readonly string ServerSideClientId = "";
        private static readonly string ServerSideTenantId = "";
        private static readonly string ServerSideClientSecret = "";

        static async Task Main(string[] args)
        {
            var publicApplicationClientBuilder = PublicClientApplicationBuilder
                .Create(ClientSideClientId)
                .WithTenantId(ClientSideTenantId)
                .WithDefaultRedirectUri()
                .Build();

            var scopes = new List<string>()
            {
                "api://serviceprincipalservice-server/access_as_user",
            };

            var authenticationResult = await publicApplicationClientBuilder.AcquireTokenWithDeviceCode(scopes, (dcr) =>
                {
                    Console.WriteLine(dcr.Message);
                    return Task.FromResult(0);
                })
                .ExecuteAsync();

            await CreateServicePrincipalWithSecurityGroupClaims(authenticationResult.AccessToken, "TestPrincipal");
        }

        private static async Task CreateServicePrincipalWithSecurityGroupClaims(string userAccessToken, string servicePrincipalName)
        {
            var delegateAuthenticationProvider = new DelegateAuthenticationProvider(
                async (requestMessage) =>
                {
                    var authenticationResult = await FetchAccessTokenOnBehalfOf(userAccessToken);
                    var graphAccessToken = authenticationResult.AccessToken;

                    // Append the access token to the request.
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", graphAccessToken);
                });

            var application = new Application()
            {
                DisplayName = servicePrincipalName,
                GroupMembershipClaims = "SecurityGroup",
            };

            var graphServiceClient = new GraphServiceClient(delegateAuthenticationProvider);
            var createdApplication = await graphServiceClient.Applications
                .Request()
                .AddAsync(application);
        }

        private static async Task<AuthenticationResult> FetchAccessTokenOnBehalfOf(string accessToken)
        {
            var confidentialClientApplication = ConfidentialClientApplicationBuilder
                .Create(ServerSideClientId)
                .WithClientSecret(ServerSideClientSecret)
                .WithTenantId(ServerSideTenantId)
                .Build();

            var scopes = new List<string>()
            {
                "https://graph.microsoft.com/Application.ReadWrite.All",
            };

            var userAssertion = new UserAssertion(accessToken);
            var graphAccessToken = await confidentialClientApplication.AcquireTokenOnBehalfOf(scopes, userAssertion)
                .ExecuteAsync();

            return graphAccessToken;
        }
    }
}

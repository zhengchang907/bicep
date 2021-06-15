// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Azure.Bicep.Types.Concrete;
using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;

namespace Bicep.Extensibility.Aad
{
    public class AadExtensibilityProvider : IExtensibilityProvider
    {
        private readonly IConfidentialClientApplication clientApp;

        private readonly IReadOnlyDictionary<string, ResourceType> types = new [] {
            new ResourceType(
                "aad://application@1.0",
                ScopeType.Unknown,
                TypeReference.For(new ObjectType(
                    "aad://application@1.0",
                    new Dictionary<string, ObjectProperty>
                    {
                        ["displayName"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.String)), ObjectPropertyFlags.Required, "The AAD app display name"),
                        ["appId"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.String)), ObjectPropertyFlags.ReadOnly, "The AAD app Id"),
                        // TODO add more fields
                    },
                    null))),
            new ResourceType(
                "aad://servicePrincipal@1.0",
                ScopeType.Unknown,
                TypeReference.For(new ObjectType(
                    "aad://servicePrincipal@1.0",
                    new Dictionary<string, ObjectProperty>
                    {
                        ["appId"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.String)), ObjectPropertyFlags.None, "The AAD app Id"),
                        // TODO add more fields
                    },
                    null))),
        }.ToImmutableDictionary(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);

        public AadExtensibilityProvider()
        {
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var contents = File.ReadAllText(Path.Combine(localAppData, "BICEP_POC_AAD_CREDS"));
            var credsObject = JObject.Parse(contents) ?? throw new InvalidOperationException($"Unable to load creds");
            var tenantId = credsObject["tenantId"]?.ToString() ?? throw new InvalidOperationException($"Unable to load creds.tenantId");
            var clientId = credsObject["clientId"]?.ToString() ?? throw new InvalidOperationException($"Unable to load creds.clientId");
            var clientSecret = credsObject["clientSecret"]?.ToString() ?? throw new InvalidOperationException($"Unable to load creds.clientSecret");

            clientApp = ConfidentialClientApplicationBuilder
                .Create(clientId)
                .WithClientSecret(clientSecret)
                .WithAuthority(new Uri($"https://login.microsoftonline.com/{tenantId}"))
                .Build();
        }

        public IEnumerable<string> ListAvailableResourceTypes()
            => types.Keys;

        public ResourceType LoadResourceType(string typeName)
            => types[typeName];

        public async Task<JToken> UpsertResource(string type, JToken body)
        {
            switch (type)
            {
                case "aad://application@1.0":
                    return await UpsertApplication(body);
                case "aad://servicePrincipal@1.0":
                default:
                    throw new NotImplementedException();
            }
        }

        public async Task<JToken> UpsertApplication(JToken body)
        {
            using var client = await GetAuthenticatedClient();

            var displayName = body["displayName"]?.ToString() ?? throw new ArgumentNullException($"Failed to find a displayName property");

            var listReq = new HttpRequestMessage(HttpMethod.Get, $"https://graph.microsoft.com/v1.0/applications?$search=\"displayName:{Uri.EscapeDataString(displayName)}\"");
            listReq.Headers.Add("ConsistencyLevel", "eventual");
            
            var listResp = await client.SendAsync(listReq);
            var data = JObject.Parse(await listResp.Content.ReadAsStringAsync());

            string appId;
            var values = (data["value"] as JArray) ?? throw new ArgumentNullException($"Failed to find a value property");
            if (!values.HasValues)
            {
                var postReq = new HttpRequestMessage(HttpMethod.Post, $"https://graph.microsoft.com/v1.0/applications");
                postReq.Content = new StringContent(body.ToString(), Encoding.UTF8, "application/json");
                var postResp = await client.SendAsync(postReq);
                postResp.EnsureSuccessStatusCode();

                var updatedBody = await postResp.Content.ReadAsStringAsync();
                return JObject.Parse(updatedBody);
            }
            else
            {
                appId = values.First!["id"]!.ToString();

                var putReq = new HttpRequestMessage(new HttpMethod("PATCH"), $"https://graph.microsoft.com/v1.0/applications/{appId}");
                putReq.Content = new StringContent(body.ToString(), Encoding.UTF8, "application/json");
                var putResp = await client.SendAsync(putReq);
                putResp.EnsureSuccessStatusCode();

                var getReq = new HttpRequestMessage(HttpMethod.Get, $"https://graph.microsoft.com/v1.0/applications/{appId}");
                var getResp = await client.SendAsync(getReq);
                getResp.EnsureSuccessStatusCode();

                var updatedBody = await getResp.Content.ReadAsStringAsync();
                return JObject.Parse(updatedBody);
            }
        }

        private async Task<HttpClient> GetAuthenticatedClient()
        {
            var result = await clientApp.AcquireTokenForClient(new [] { "https://graph.microsoft.com/.default" })
                .ExecuteAsync();

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);

            return client;
        }
    }
}

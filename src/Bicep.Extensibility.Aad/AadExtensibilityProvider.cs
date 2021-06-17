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

        // TODO: Consume data at https://raw.githubusercontent.com/microsoftgraph/msgraph-metadata/master/openapi/v1.0/openapi.yaml
        private readonly IReadOnlyDictionary<string, ResourceType> types = new [] {
            new ResourceType(
                "aad://application@1.0",
                ScopeType.Unknown,
                TypeReference.For(new ObjectType(
                    "aad://application@1.0",
                    new Dictionary<string, ObjectProperty>
                    {
                        // microsoft.graph.entity
                        ["id"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.String)), ObjectPropertyFlags.ReadOnly, null),
                        // microsoft.graph.directoryObject
                        ["deletedDateTime"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.String)), ObjectPropertyFlags.None, null),
                        // microsoft.graph.application
                        ["addIns"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.Array)), ObjectPropertyFlags.None, "Defines custom behavior that a consuming service can use to call an app in specific contexts. For example, applications that can render file streams may set the addIns property for its 'FileHandler' functionality. This will let services like Office 365 call the application in the context of a document the user is working on."),
                        ["api"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.Any)), ObjectPropertyFlags.None, "Specifies settings for an application that implements a web API."),
                        ["appId"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.String)), ObjectPropertyFlags.ReadOnly, "The unique identifier for the application that is assigned by Azure AD. Not nullable. Read-only."),
                        ["applicationTemplateId"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.String)), ObjectPropertyFlags.None, "Unique identifier of the applicationTemplate."),
                        ["appRoles"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.Array)), ObjectPropertyFlags.None, "The collection of roles assigned to the application. With app role assignments, these roles can be assigned to users, groups, or service principals associated with other applications. Not nullable."),
                        ["createdDateTime"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.String)), ObjectPropertyFlags.ReadOnly, "The date and time the application was registered. The DateTimeOffset type represents date and time information using ISO 8601 format and is always in UTC time. For example, midnight UTC on Jan 1, 2014 is 2014-01-01T00:00:00Z. Read-only."),
                        ["description"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.String)), ObjectPropertyFlags.None, null),
                        ["displayName"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.String)), ObjectPropertyFlags.Required, "The display name for the application."),
                        ["groupMembershipClaims"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.String)), ObjectPropertyFlags.None, "Configures the groups claim issued in a user or OAuth 2.0 access token that the application expects. To set this attribute, use one of the following string values: None, SecurityGroup (for security groups and Azure AD roles), All (this gets all security groups, distribution groups, and Azure AD directory roles that the signed-in user is a member of)."),
                        ["identifierUris"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.Array)), ObjectPropertyFlags.None, "The URIs that identify the application within its Azure AD tenant, or within a verified custom domain if the application is multi-tenant. For more information, see Application Objects and Service Principal Objects. The any operator is required for filter expressions on multi-valued properties. Not nullable."),
                        ["info"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.Any)), ObjectPropertyFlags.None, "Basic profile information of the application, such as it's marketing, support, terms of service, and privacy statement URLs. The terms of service and privacy statement are surfaced to users through the user consent experience. For more information, see How to: Add Terms of service and privacy statement for registered Azure AD apps."),
                        ["isDeviceOnlyAuthSupported"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.Bool)), ObjectPropertyFlags.None, "Specifies whether this application supports device authentication without a user. The default is false."),
                        ["isFallbackPublicClient"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.Bool)), ObjectPropertyFlags.None, "Specifies the fallback application type as public client, such as an installed application running on a mobile device. The default value is false which means the fallback application type is confidential client such as a web app. There are certain scenarios where Azure AD cannot determine the client application type. For example, the ROPC flow where the application is configured without specifying a redirect URI. In those cases Azure AD interprets the application type based on the value of this property."),
                        ["keyCredentials"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.Array)), ObjectPropertyFlags.None, "The collection of key credentials associated with the application. Not nullable."),
                        ["logo"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.String)), ObjectPropertyFlags.None, "The main logo for the application. Not nullable."),
                        ["notes"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.String)), ObjectPropertyFlags.None, "Notes relevant for the management of the application."),
                        ["oauth2RequirePostResponse"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.Bool)), ObjectPropertyFlags.None, null),
                        ["optionalClaims"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.Any)), ObjectPropertyFlags.None, "Application developers can configure optional claims in their Azure AD applications to specify the claims that are sent to their application by the Microsoft security token service. For more information, see How to: Provide optional claims to your app."),
                        ["parentalControlSettings"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.Any)), ObjectPropertyFlags.None, "Specifies parental control settings for an application."),
                        ["passwordCredentials"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.Array)), ObjectPropertyFlags.None, "The collection of password credentials associated with the application. Not nullable."),
                        ["publicClient"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.Any)), ObjectPropertyFlags.None, "Specifies settings for installed clients such as desktop or mobile devices."),
                        ["publisherDomain"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.String)), ObjectPropertyFlags.ReadOnly, "The verified publisher domain for the application. Read-only."),
                        ["requiredResourceAccess"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.Array)), ObjectPropertyFlags.None, "Specifies the resources that the application needs to access. This property also specifies the set of OAuth permission scopes and application roles that it needs for each of those resources. This configuration of access to the required resources drives the consent experience. Not nullable."),
                        ["signInAudience"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.String)), ObjectPropertyFlags.None, "Specifies the Microsoft accounts that are supported for the current application. Supported values are: AzureADMyOrg, AzureADMultipleOrgs, AzureADandPersonalMicrosoftAccount, PersonalMicrosoftAccount. See more in the table below."),
                        ["spa"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.Any)), ObjectPropertyFlags.None, "Specifies settings for a single-page application, including sign out URLs and redirect URIs for authorization codes and access tokens."),
                        ["tags"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.Array)), ObjectPropertyFlags.None, "Custom strings that can be used to categorize and identify the application. Not nullable."),
                        ["tokenEncryptionKeyId"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.String)), ObjectPropertyFlags.None, "Specifies the keyId of a public key from the keyCredentials collection. When configured, Azure AD encrypts all the tokens it emits by using the key this property points to. The application code that receives the encrypted token must use the matching private key to decrypt the token before it can be used for the signed-in user."),
                        ["web"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.Any)), ObjectPropertyFlags.None, "Specifies settings for a web application."),
                        ["createdOnBehalfOf"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.Any)), ObjectPropertyFlags.ReadOnly, "Read-only."),
                        ["extensionProperties"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.Array)), ObjectPropertyFlags.ReadOnly, "Read-only. Nullable."),
                        ["homeRealmDiscoveryPolicies"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.Array)), ObjectPropertyFlags.None, null),
                        ["owners"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.Array)), ObjectPropertyFlags.ReadOnly, "Directory objects that are owners of the application. Read-only. Nullable."),
                        ["tokenIssuancePolicies"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.Array)), ObjectPropertyFlags.None, null),
                        ["tokenLifetimePolicies"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.Array)), ObjectPropertyFlags.None, "The tokenLifetimePolicies assigned to this application."),
                    },
                    null))),
            new ResourceType(
                "aad://servicePrincipal@1.0",
                ScopeType.Unknown,
                TypeReference.For(new ObjectType(
                    "aad://servicePrincipal@1.0",
                    new Dictionary<string, ObjectProperty>
                    {
                        // microsoft.graph.entity
                        ["id"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.String)), ObjectPropertyFlags.ReadOnly, null),
                        // microsoft.graph.directoryObject
                        ["deletedDateTime"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.String)), ObjectPropertyFlags.None, null),
                        // microsoft.graph.servicePrincipal
                        ["accountEnabled"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.Bool)), ObjectPropertyFlags.None, "true if the service principal account is enabled; otherwise, false."),
                        ["addIns"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.Array)), ObjectPropertyFlags.None, "Defines custom behavior that a consuming service can use to call an app in specific contexts. For example, applications that can render file streams may set the addIns property for its ''FileHandler'' functionality. This will let services like Microsoft 365 call the application in the context of a document the user is working on."),
                        ["alternativeNames"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.Array)), ObjectPropertyFlags.None, "Used to retrieve service principals by subscription, identify resource group and full resource ids for managed identities."),
                        ["appDescription"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.String)), ObjectPropertyFlags.None, "The description exposed by the associated application."),
                        ["appDisplayName"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.String)), ObjectPropertyFlags.None, "The display name exposed by the associated application."),
                        ["appId"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.String)), ObjectPropertyFlags.Required, "The unique identifier for the associated application (its appId property)."),
                        ["applicationTemplateId"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.String)), ObjectPropertyFlags.ReadOnly, "Unique identifier of the applicationTemplate that the servicePrincipal was created from. Read-only."),
                        ["appOwnerOrganizationId"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.String)), ObjectPropertyFlags.None, "Contains the tenant id where the application is registered. This is applicable only to service principals backed by applications."),
                        ["appRoleAssignmentRequired"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.Bool)), ObjectPropertyFlags.None, "Specifies whether users or other service principals need to be granted an app role assignment for this service principal before users can sign in or apps can get tokens. The default value is false. Not nullable."),
                        ["appRoles"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.Array)), ObjectPropertyFlags.None, "The roles exposed by the application which this service principal represents. For more information see the appRoles property definition on the application entity. Not nullable."),
                        ["description"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.String)), ObjectPropertyFlags.None, "Free text field to provide an internal end-user facing description of the service principal. End-user portals such MyApps will display the application description in this field. The maximum allowed size is 1024 characters."),
                        ["displayName"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.String)), ObjectPropertyFlags.None, "The display name for the service principal."),
                        ["homepage"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.String)), ObjectPropertyFlags.None, "Home page or landing page of the application."),
                        ["info"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.Any)), ObjectPropertyFlags.None, "Basic profile information of the acquired application such as app''s marketing, support, terms of service and privacy statement URLs. The terms of service and privacy statement are surfaced to users through the user consent experience. For more info, see How to: Add Terms of service and privacy statement for registered Azure AD apps."),
                        ["keyCredentials"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.Array)), ObjectPropertyFlags.None, "The collection of key credentials associated with the service principal. Not nullable."),
                        ["loginUrl"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.String)), ObjectPropertyFlags.None, "Specifies the URL where the service provider redirects the user to Azure AD to authenticate. Azure AD uses the URL to launch the application from Microsoft 365 or the Azure AD My Apps. When blank, Azure AD performs IdP-initiated sign-on for applications configured with SAML-based single sign-on. The user launches the application from Microsoft 365, the Azure AD My Apps, or the Azure AD SSO URL."),
                        ["logoutUrl"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.String)), ObjectPropertyFlags.None, "Specifies the URL that will be used by Microsoft''s authorization service to logout an user using OpenId Connect front-channel, back-channel or SAML logout protocols."),
                        ["notes"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.String)), ObjectPropertyFlags.None, "Free text field to capture information about the service principal, typically used for operational purposes. Maximum allowed size is 1024 characters."),
                        ["notificationEmailAddresses"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.Array)), ObjectPropertyFlags.None, "Specifies the list of email addresses where Azure AD sends a notification when the active certificate is near the expiration date. This is only for the certificates used to sign the SAML token issued for Azure AD Gallery applications."),
                        ["oauth2PermissionScopes"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.Array)), ObjectPropertyFlags.None, "The delegated permissions exposed by the application. For more information see the oauth2PermissionScopes property on the application entity's api property. Not nullable."),
                        ["passwordCredentials"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.Array)), ObjectPropertyFlags.None, "The collection of password credentials associated with the service principal. Not nullable."),
                        ["preferredSingleSignOnMode"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.String)), ObjectPropertyFlags.None, "Specifies the single sign-on mode configured for this application. Azure AD uses the preferred single sign-on mode to launch the application from Microsoft 365 or the Azure AD My Apps. The supported values are password, saml, notSupported, and oidc."),
                        ["preferredTokenSigningKeyThumbprint"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.String)), ObjectPropertyFlags.None, "Reserved for internal use only. Do not write or otherwise rely on this property. May be removed in future versions."),
                        ["replyUrls"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.Array)), ObjectPropertyFlags.None, "The URLs that user tokens are sent to for sign in with the associated application, or the redirect URIs that OAuth 2.0 authorization codes and access tokens are sent to for the associated application. Not nullable."),
                        ["samlSingleSignOnSettings"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.Any)), ObjectPropertyFlags.None, "The collection for settings related to saml single sign-on."),
                        ["servicePrincipalNames"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.Array)), ObjectPropertyFlags.None, "Contains the list of identifiersUris, copied over from the associated application. Additional values can be added to hybrid applications. These values can be used to identify the permissions exposed by this app within Azure AD. For example,Client apps can specify a resource URI which is based on the values of this property to acquire an access token, which is the URI returned in the ''aud'' claim.The any operator is required for filter expressions on multi-valued properties. Not nullable."),
                        ["servicePrincipalType"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.String)), ObjectPropertyFlags.None, "Identifies if the service principal represents an application or a managed identity. This is set by Azure AD internally. For a service principal that represents an application this is set as Application. For a service principal that represent a managed identity this is set as ManagedIdentity."),
                        ["signInAudience"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.String)), ObjectPropertyFlags.None, "Specifies the Microsoft accounts that are supported for the current application. Read-only. Supported values are:AzureADMyOrg: Users with a Microsoft work or school account in my organization’s Azure AD tenant (single-tenant).AzureADMultipleOrgs: Users with a Microsoft work or school account in any organization’s Azure AD tenant (multi-tenant).AzureADandPersonalMicrosoftAccount: Users with a personal Microsoft account, or a work or school account in any organization’s Azure AD tenant.PersonalMicrosoftAccount: Users with a personal Microsoft account only."),
                        ["tags"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.Array)), ObjectPropertyFlags.None, "Custom strings that can be used to categorize and identify the service principal. Not nullable."),
                        ["tokenEncryptionKeyId"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.String)), ObjectPropertyFlags.None, "Specifies the keyId of a public key from the keyCredentials collection. When configured, Azure AD issues tokens for this application encrypted using the key specified by this property. The application code that receives the encrypted token must use the matching private key to decrypt the token before it can be used for the signed-in user."),
                        ["appRoleAssignedTo"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.Array)), ObjectPropertyFlags.None, "'App role assignments for this app or service, granted to users, groups, and other service principals.'"),
                        ["appRoleAssignments"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.Array)), ObjectPropertyFlags.None, "'App role assignment for another app or service, granted to this service principal.'"),
                        ["claimsMappingPolicies"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.Array)), ObjectPropertyFlags.None, "The claimsMappingPolicies assigned to this service principal."),
                        ["createdObjects"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.Array)), ObjectPropertyFlags.None, "Directory objects created by this service principal. Read-only. Nullable."),
                        ["delegatedPermissionClassifications"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.Array)), ObjectPropertyFlags.None, "The permission classifications for delegated permissions exposed by the app that this service principal represents."),
                        ["endpoints"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.Array)), ObjectPropertyFlags.None, "Endpoints available for discovery. Services like Sharepoint populate this property with a tenant specific SharePoint endpoints that other applications can discover and use in their experiences."),
                        ["homeRealmDiscoveryPolicies"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.Array)), ObjectPropertyFlags.None, "The homeRealmDiscoveryPolicies assigned to this service principal."),
                        ["memberOf"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.Array)), ObjectPropertyFlags.None, "'Roles that this service principal is a member of. HTTP Methods: GET Read-only. Nullable.'"),
                        ["oauth2PermissionGrants"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.Array)), ObjectPropertyFlags.None, "Delegated permission grants authorizing this service principal to access an API on behalf of a signed-in user. Read-only. Nullable."),
                        ["ownedObjects"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.Array)), ObjectPropertyFlags.None, "Directory objects that are owned by this service principal. Read-only. Nullable."),
                        ["owners"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.Array)), ObjectPropertyFlags.None, "Directory objects that are owners of this servicePrincipal. The owners are a set of non-admin users or servicePrincipals who are allowed to modify this object. Read-only. Nullable."),
                        ["tokenIssuancePolicies"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.Array)), ObjectPropertyFlags.None, "The tokenIssuancePolicies assigned to this service principal."),
                        ["tokenLifetimePolicies"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.Array)), ObjectPropertyFlags.None, "The tokenLifetimePolicies assigned to this service principal."),
                        ["transitiveMemberOf"] = new ObjectProperty(TypeReference.For(new BuiltInType(BuiltInTypeKind.Array)), ObjectPropertyFlags.None, "Represents an Azure Active Directory object. The directoryObject type is the base type for many other directory entity types."),
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
                    return await UpsertServicePrincipal(body);
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
                var appId = values.First!["id"]!.ToString();

                var patchReq = new HttpRequestMessage(new HttpMethod("PATCH"), $"https://graph.microsoft.com/v1.0/applications/{appId}");
                patchReq.Content = new StringContent(body.ToString(), Encoding.UTF8, "application/json");
                var patchResp = await client.SendAsync(patchReq);
                patchResp.EnsureSuccessStatusCode();

                var getReq = new HttpRequestMessage(HttpMethod.Get, $"https://graph.microsoft.com/v1.0/applications/{appId}");
                var getResp = await client.SendAsync(getReq);
                getResp.EnsureSuccessStatusCode();

                var updatedBody = await getResp.Content.ReadAsStringAsync();
                return JObject.Parse(updatedBody);
            }
        }

        public async Task<JToken> UpsertServicePrincipal(JToken body)
        {
            using var client = await GetAuthenticatedClient();

            var appId = body["appId"]?.ToString() ?? throw new ArgumentNullException($"Failed to find a appId property");

            var listReq = new HttpRequestMessage(HttpMethod.Get, $"https://graph.microsoft.com/v1.0/servicePrincipals?$search=\"appId:{Uri.EscapeDataString(appId)}\"");
            listReq.Headers.Add("ConsistencyLevel", "eventual");
            var listResp = await client.SendAsync(listReq);
            var data = JObject.Parse(await listResp.Content.ReadAsStringAsync());

            var values = (data["value"] as JArray) ?? throw new ArgumentNullException($"Failed to find a value property");
            if (!values.HasValues)
            {
                var postReq = new HttpRequestMessage(HttpMethod.Post, $"https://graph.microsoft.com/v1.0/servicePrincipals");
                postReq.Content = new StringContent(body.ToString(), Encoding.UTF8, "application/json");
                var postResp = await client.SendAsync(postReq);
                postResp.EnsureSuccessStatusCode();

                var updatedBody = await postResp.Content.ReadAsStringAsync();
                return JObject.Parse(updatedBody);
            }
            else
            {
                var spId = values.First!["id"]!.ToString();

                var patchReq = new HttpRequestMessage(new HttpMethod("PATCH"), $"https://graph.microsoft.com/v1.0/servicePrincipals/{spId}");
                patchReq.Content = new StringContent(body.ToString(), Encoding.UTF8, "application/json");
                var patchResp = await client.SendAsync(patchReq);
                patchResp.EnsureSuccessStatusCode();

                var getReq = new HttpRequestMessage(HttpMethod.Get, $"https://graph.microsoft.com/v1.0/servicePrincipals/{spId}");
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

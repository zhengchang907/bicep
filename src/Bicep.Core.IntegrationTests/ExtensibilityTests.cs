// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class ExtensibilityTests
    {
        [TestMethod]
        public void Basic_AAD()
        {
            var result = CompilationHelper.Compile(
                ("main.bicep", @"
resource myApp 'aad://application@1.0' = {
  displayName: 'Display name'
}

resource mySp 'aad://servicePrincipal@1.0' = {
  appId: myApp.appId
}

/*
resource myKey 'aad://servicePrincipal/key@1.0' = {
  keyCredential: {
    type: 'AsymmetricX509Cert'
    usage: 'Verify'
    key: keyValue
  }
  passwordCredential: null
  proof: //?
}
*/
"));

            result.Template.Should().DeepEqual(JToken.Parse(
@"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""metadata"": {
    ""_generator"": {
      ""name"": ""bicep"",
      ""version"": ""dev"",
      ""templateHash"": ""16469132788283103768""
    }
  },
  ""functions"": [],
  ""resources"": [
    {
      ""type"": ""Microsoft.CustomProviders/resourceProviders/resources"",
      ""name"": ""extensibilityProxy/myApp"",
      ""apiVersion"": ""2018-09-01-preview"",
      ""properties"": {
        ""proxyType"": ""/application@1.0"",
        ""proxyProperties"": {
          ""displayName"": ""Display name""
        }
      }
    },
    {
      ""type"": ""Microsoft.CustomProviders/resourceProviders/resources"",
      ""name"": ""extensibilityProxy/mySp"",
      ""apiVersion"": ""2018-09-01-preview"",
      ""properties"": {
        ""proxyType"": ""/servicePrincipal@1.0"",
        ""proxyProperties"": {
          ""appId"": ""[reference(resourceId('Microsoft.CustomProviders/resourceProviders/resources', 'extensibilityProxy', 'myApp')).proxyProperties]""
        }
      },
      ""dependsOn"": [
        ""[resourceId('Microsoft.CustomProviders/resourceProviders/resources', 'extensibilityProxy', 'myApp')]""
      ]
    }
  ]
}"
            ));

            result.Should().NotHaveAnyDiagnostics();
        }
    }
}



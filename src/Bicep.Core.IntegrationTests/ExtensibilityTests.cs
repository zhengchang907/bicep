// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

            result.Should().NotHaveAnyDiagnostics();
        }
    }
}



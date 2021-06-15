// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Extensibility.Aad;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Bicep.Extensibility.IntegrationTests
{
    [TestClass]
    public class AadExtensibilityProviderTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [TestMethod]
        public async Task UpsertTestAsync()
        {
            var test = new AadExtensibilityProvider();
            var updated = await test.UpsertResource("aad://application@1.0", new JObject {
                ["displayName"] = "Bicep Test App"
            });
        }
    }
}

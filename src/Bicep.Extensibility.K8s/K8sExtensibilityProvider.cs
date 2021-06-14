// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Azure.Bicep.Types.Concrete;
using Newtonsoft.Json.Linq;

namespace Bicep.Extensibility.K8s
{
    public class K8sExtensibilityProvider : IExtensibilityProvider
    {
        private readonly IReadOnlyDictionary<string, ResourceType> types = new ResourceType[] {
        }.ToImmutableDictionary(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);

        public IEnumerable<string> ListAvailableResourceTypes()
            => types.Keys;

        public ResourceType LoadResourceType(string typeName)
            => types[typeName];

        public Task<JToken> UpsertResource(string type, JToken body)
        {
            throw new NotImplementedException();
        }
    }
}

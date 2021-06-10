// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Azure.Bicep.Types.Concrete;

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
    }
}

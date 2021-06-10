// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Azure.Bicep.Types.Concrete;

namespace Bicep.Extensibility.Aad
{
    public class AadExtensibilityProvider : IExtensibilityProvider
    {
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

        public IEnumerable<string> ListAvailableResourceTypes()
            => types.Keys;

        public ResourceType LoadResourceType(string typeName)
            => types[typeName];
    }
}

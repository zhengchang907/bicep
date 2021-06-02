// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Extensions;
using Bicep.Core.Resources;

namespace Bicep.Core.TypeSystem.Extensibility
{
    public class AadResourceTypeProvider : IResourceTypeProvider
    {
        // TODO(extensibility): Ignore scopes for extensible resources rather than hacking it here
        private const ResourceScope AnyScope = 
            ResourceScope.Resource |
            ResourceScope.Module |
            ResourceScope.Tenant |
            ResourceScope.ManagementGroup |
            ResourceScope.Subscription |
            ResourceScope.ResourceGroup;

        private readonly ImmutableDictionary<ResourceTypeReference, ResourceType> Types = new []
        {
            new ResourceType(
                ResourceTypeReference.Parse("aad://application@1.0"),
                AnyScope,
                new ObjectType(
                    "aad://application@1.0",
                    TypeSymbolValidationFlags.Default,
                    new [] {
                        new TypeProperty("displayName", LanguageConstants.String, TypePropertyFlags.None, "The AAD app display name"),
                        new TypeProperty("appId", LanguageConstants.String, TypePropertyFlags.ReadOnly, "The AAD app Id"),
                    },
                    null)),
            new ResourceType(
                ResourceTypeReference.Parse("aad://servicePrincipal@1.0"),
                AnyScope,
                new ObjectType(
                    "aad://servicePrincipal@1.0",
                    TypeSymbolValidationFlags.Default,
                    new [] {
                        new TypeProperty("appId", LanguageConstants.String, TypePropertyFlags.None, "The AAD app Id"),
                    },
                    null)),
        }.ToImmutableDictionary(x => x.TypeReference, x => x, ResourceTypeReferenceComparer.Instance);

        public IEnumerable<ResourceTypeReference> GetAvailableTypes()
            => Types.Keys;

        public ResourceType GetType(ResourceTypeReference reference, ResourceTypeGenerationFlags flags)
        {
            if (flags.HasFlag(ResourceTypeGenerationFlags.ExistingResource) || flags.HasFlag(ResourceTypeGenerationFlags.NestedResource))
            {
                throw new NotImplementedException($"Flags are not currently supported for reference {reference.FormatName()}");
            }

            if (Types.TryGetValue(reference) is not {} resourceType)
            {
                throw new NotImplementedException($"Failed to find resource type for reference {reference.FormatName()}");
            }
            
            return resourceType;
        }

        public bool HasType(ResourceTypeReference typeReference)
            => Types.ContainsKey(typeReference);
    }
}
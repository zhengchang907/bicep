// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Resources;
using Bicep.Extensibility;

namespace Bicep.Core.TypeSystem.Extensibility
{
    public class ExtensibilityResourceTypeProvider : IResourceTypeProvider
    {
        private readonly ExtensibilityResourceTypeFactory typeFactory;
        private readonly IReadOnlyDictionary<ResourceTypeReference, Azure.Bicep.Types.Concrete.ResourceType> resourceTypes;

        // TODO(extensibility): Ignore scopes for extensible resources rather than hacking it here
        private const ResourceScope AnyScope = 
            ResourceScope.Resource |
            ResourceScope.Module |
            ResourceScope.Tenant |
            ResourceScope.ManagementGroup |
            ResourceScope.Subscription |
            ResourceScope.ResourceGroup;

        public ExtensibilityResourceTypeProvider(IExtensibilityTypeProvider typeProvider)
        {
            typeFactory = new ExtensibilityResourceTypeFactory();
            resourceTypes = typeProvider.ListAvailableResourceTypes()
                .ToImmutableDictionary(
                    x => ResourceTypeReference.Parse(x),
                    x => typeProvider.LoadResourceType(x),
                   ResourceTypeReferenceComparer.Instance);
        }

        public IEnumerable<ResourceTypeReference> GetAvailableTypes()
            => resourceTypes.Keys;

        public ResourceType GetType(ResourceTypeReference reference, ResourceTypeGenerationFlags flags)
        {
            // TODO(extensibility): Handle flags
            return typeFactory.GetResourceType(resourceTypes[reference]);
        }

        public bool HasType(ResourceTypeReference typeReference)
            => resourceTypes.ContainsKey(typeReference);
    }
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Resources;
using Bicep.Core.TypeSystem.Az;
using Bicep.Core.TypeSystem.Extensibility;
using Bicep.Extensibility.Aad;
using Bicep.Extensibility.K8s;

namespace Bicep.Core.TypeSystem
{
    public class CombinedResourceTypeProvider : IResourceTypeProvider
    {
        public readonly IReadOnlyDictionary<BicepExtension, IResourceTypeProvider> providers = new Dictionary<BicepExtension, IResourceTypeProvider>()
        {
            [BicepExtension.Az] = AzResourceTypeProvider.CreateWithAzTypes(),
            [BicepExtension.Aad] = new ExtensibilityResourceTypeProvider(new AadExtensibilityProvider()),
            [BicepExtension.K8s] = new ExtensibilityResourceTypeProvider(new K8sExtensibilityProvider()),            
        };

        public IEnumerable<ResourceTypeReference> GetAvailableTypes()
            => providers.Values.SelectMany(x => x.GetAvailableTypes());

        private IResourceTypeProvider GetProvider(BicepExtension bicepExtension)
            => providers[bicepExtension];

        public ResourceType GetType(ResourceTypeReference reference, ResourceTypeGenerationFlags flags)
            => providers[reference.Extension].GetType(reference, flags);

        public bool HasType(ResourceTypeReference reference)
            => providers[reference.Extension].HasType(reference);
    }
}

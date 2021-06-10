// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using Azure.Bicep.Types.Concrete;

namespace Bicep.Extensibility
{
    public interface IExtensibilityTypeProvider
    {
        ResourceType LoadResourceType(string typeName);

        IEnumerable<string> ListAvailableResourceTypes();
    }
}

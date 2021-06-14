// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Bicep.Extensibility
{
    public interface IExtensibilityProvider : IExtensibilityTypeProvider
    {
        Task<JToken> UpsertResource(string type, JToken body);
    }
}

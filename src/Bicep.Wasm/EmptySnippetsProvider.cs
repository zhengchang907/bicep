// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Resources;
using Bicep.Core.TypeSystem;
using Bicep.LanguageServer.Snippets;

namespace Bicep.Wasm
{
    public class EmptySnippetsProvider : ISnippetsProvider
    {
        public IEnumerable<Snippet> GetModuleBodyCompletionSnippets(TypeSymbol typeSymbol)
            => Enumerable.Empty<Snippet>();

        public IEnumerable<Snippet> GetNestedResourceDeclarationSnippets(ResourceTypeReference resourceTypeReference)
            => Enumerable.Empty<Snippet>();

        public IEnumerable<Snippet> GetObjectBodyCompletionSnippets(TypeSymbol typeSymbol)
            => Enumerable.Empty<Snippet>();

        public IEnumerable<Snippet> GetResourceBodyCompletionSnippets(ResourceType resourceType, bool isExistingResource, bool isResourceNested)
            => Enumerable.Empty<Snippet>();

        public IEnumerable<Snippet> GetTopLevelNamedDeclarationSnippets()
            => Enumerable.Empty<Snippet>();
    }
}

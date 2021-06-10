// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Bicep.Types.Concrete;

namespace Bicep.Extensibility
{
    public class TypeReference : ITypeReference
    {
        public static ITypeReference For(TypeBase type)
            => new TypeReference(type);

        private TypeReference(TypeBase type)
        {
            Type = type;
        }

        public TypeBase Type { get; }
    }
}

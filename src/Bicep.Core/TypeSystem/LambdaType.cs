// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.TypeSystem
{
    public class LambdaType : TypeSymbol
    {
        public LambdaType(ITypeReference argumentType, ITypeReference bodyType)
            : base(FormatTypeName(argumentType, bodyType))
        {
            ArgumentType = argumentType;
            BodyType = bodyType;
        }

        public override TypeKind TypeKind => TypeKind.Primitive;

        public ITypeReference ArgumentType { get; }

        public ITypeReference BodyType { get; }

        private static string FormatTypeName(ITypeReference argumentType, ITypeReference bodyType)
            => $"{argumentType.Type.FormatNameForCompoundTypes()} => {bodyType.Type.FormatNameForCompoundTypes()}";
    }
}

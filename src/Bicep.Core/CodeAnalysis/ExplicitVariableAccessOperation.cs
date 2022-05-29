// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.CodeAnalysis
{
    public record ExplicitVariableAccessOperation(
        string Name) : Operation
    {
        public override void Accept(IOperationVisitor visitor)
            => visitor.VisitExplicitVariableAccessOperation(this);
    }
}

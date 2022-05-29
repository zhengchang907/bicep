// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Semantics.Metadata;

namespace Bicep.Core.CodeAnalysis
{
    public record ResourceReferenceOperation(
        ResourceMetadata Metadata,
        ResourceIdOperation ResourceId,
        bool Full,
        bool ShouldIncludeApiVersion) : Operation
    {
        public override void Accept(IOperationVisitor visitor)
            => visitor.VisitResourceReferenceOperation(this);
    }
}

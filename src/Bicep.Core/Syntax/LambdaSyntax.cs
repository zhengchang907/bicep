// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    public class LambdaSyntax : ExpressionSyntax
    {
        public LambdaSyntax(LocalVariableSyntax variable, Token arrow, SyntaxBase body)
        {
            AssertTokenType(arrow, nameof(arrow), TokenType.Arrow);

            this.Variable = variable;
            this.Arrow = arrow;
            this.Body = body;
        }

        public LocalVariableSyntax Variable { get; }

        public Token Arrow { get; }

        public SyntaxBase Body { get; }

        public override TextSpan Span => TextSpan.Between(this.Variable, this.Body);

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitLambdaSyntax(this);
    }
}

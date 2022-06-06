// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.CodeAction;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;

namespace Bicep.LanguageServer.CodeFixes
{
    public class MultilineObjectsAndArraysCodeFixProvider : ICodeFixProvider
    {
        public IEnumerable<CodeFix> GetFixes(SemanticModel semanticModel, IReadOnlyList<SyntaxBase> matchingNodes)
        {
            var lastObjectOrArray = matchingNodes.Where(x => x is ArraySyntax or ObjectSyntax).LastOrDefault();
            if (lastObjectOrArray is null)
            {
                yield break;
            }

            var children = lastObjectOrArray switch {
                ObjectSyntax x => x.Children,
                ArraySyntax x => x.Children,
                _ => throw new NotImplementedException("blah"),
            };

            if (children.Any(x => x is Token { Type: TokenType.Comma }))
            {
                var updatedChildren = children
                    .Where(x => x is not Token { Type: TokenType.Comma } and not Token { Type: TokenType.NewLine })
                    .SelectMany(x => new [] { SyntaxFactory.NewlineToken, x })
                    .Concat(new [] { SyntaxFactory.NewlineToken });

                SyntaxBase newItem = lastObjectOrArray switch {
                    ObjectSyntax x => new ObjectSyntax(x.OpenBrace, updatedChildren, x.CloseBrace),
                    ArraySyntax x => new ArraySyntax(x.OpenBracket, updatedChildren, x.CloseBracket),
                    _ => throw new NotImplementedException("blah"),
                };

                var codeReplacement = new CodeReplacement(lastObjectOrArray.Span, newItem.ToText(indent: "  "));

                yield return new CodeFix(
                    "Convert to multi line",
                    false,
                    CodeFixKind.Refactor,
                    codeReplacement);
            }

            if (children.Any(x => x is Token { Type: TokenType.NewLine }))
            {
                var updatedChildren = children
                    .Where(x => x is not Token { Type: TokenType.Comma } and not Token { Type: TokenType.NewLine })
                    .SelectMany(x => new [] { x, SyntaxFactory.CommaToken });

                SyntaxBase newItem = lastObjectOrArray switch {
                    ObjectSyntax x => new ObjectSyntax(x.OpenBrace, updatedChildren, x.CloseBrace),
                    ArraySyntax x => new ArraySyntax(x.OpenBracket, updatedChildren, x.CloseBracket),
                    _ => throw new NotImplementedException("blah"),
                };

                var codeReplacement = new CodeReplacement(lastObjectOrArray.Span, newItem.ToText(indent: "  "));

                yield return new CodeFix(
                    "Convert to single line",
                    false,
                    CodeFixKind.Refactor,
                    codeReplacement);
            }
        }
    }
}

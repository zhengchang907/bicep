// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Azure.Deployments.Expression.Expressions;
using Bicep.Core.DataFlow;
using Bicep.Core.Extensions;
using Bicep.Core.CodeAnalysis;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Emit
{
    public class ExpressionConverter
    {
        private readonly EmitterContext context;

        private readonly ImmutableDictionary<LocalVariableSymbol, Operation> localReplacements;

        public ExpressionConverter(EmitterContext context)
            : this(context, ImmutableDictionary<LocalVariableSymbol, Operation>.Empty)
        {
        }

        private ExpressionConverter(EmitterContext context, ImmutableDictionary<LocalVariableSymbol, Operation> localReplacements)
        {
            this.context = context;
            this.localReplacements = localReplacements;
        }

        /// <summary>
        /// Converts the specified bicep expression tree into an ARM template expression tree.
        /// The returned tree may be rooted at either a function expression or jtoken expression.
        /// </summary>
        /// <param name="expression">The expression</param>
        public Operation ConvertExpressionOperation(SyntaxBase expression)
        {
            switch (expression)
            {
                case BooleanLiteralSyntax boolSyntax:
                    return new ConstantValueOperation(boolSyntax.Value);

                case IntegerLiteralSyntax integerSyntax:
                    var longValue = integerSyntax.Value switch {
                        <= long.MaxValue => (long)integerSyntax.Value,
                        _ => throw new InvalidOperationException($"Integer syntax hs value {integerSyntax.Value} which will overflow"),
                    };

                    return new ConstantValueOperation(longValue);

                case StringSyntax stringSyntax:
                    // using the throwing method to get semantic value of the string because
                    // error checking should have caught any errors by now
                    return ConvertString(stringSyntax);

                case NullLiteralSyntax _:
                    return new NullValueOperation();

                case ObjectSyntax @object:
                    return ConvertObject(@object);

                case ArraySyntax array:
                    return ConvertArray(array);

                case ParenthesizedExpressionSyntax parenthesized:
                    // template expressions do not have operators so parentheses are irrelevant
                    return ConvertExpressionOperation(parenthesized.Expression);

                case UnaryOperationSyntax unary:
                    return ConvertUnary(unary);

                case BinaryOperationSyntax binary:
                    return ConvertBinary(binary);

                case TernaryOperationSyntax ternary:
                    return new FunctionCallOperation(
                        "if",
                        ConvertExpressionOperation(ternary.ConditionExpression),
                        ConvertExpressionOperation(ternary.TrueExpression),
                        ConvertExpressionOperation(ternary.FalseExpression));

                case FunctionCallSyntaxBase functionCall:
                    return ConvertFunction(functionCall);

                case ArrayAccessSyntax arrayAccess:
                    return ConvertArrayAccess(arrayAccess);

                case ResourceAccessSyntax resourceAccess:
                    return ConvertResourceAccess(resourceAccess);

                case PropertyAccessSyntax propertyAccess:
                    return ConvertPropertyAccess(propertyAccess);

                case VariableAccessSyntax variableAccess:
                    return ConvertVariableAccess(variableAccess);

                default:
                    throw new NotImplementedException($"Cannot emit unexpected expression of type {expression.GetType().Name}");
            }
        }

        public LanguageExpression ConvertExpression(SyntaxBase syntax)
            => ConvertOperation(ConvertExpressionOperation(syntax));

        private Operation ConvertFunction(FunctionCallSyntaxBase functionCall)
        {
            var symbol = context.SemanticModel.GetSymbolInfo(functionCall);
            if (symbol is FunctionSymbol &&
                context.SemanticModel.TypeManager.GetMatchedFunctionOverload(functionCall) is {Evaluator: { }} functionOverload)
            {
                return ConvertExpressionOperation(functionOverload.Evaluator(functionCall, symbol, context.SemanticModel.GetTypeInfo(functionCall), context.FunctionVariables.GetValueOrDefault(functionCall)));
            }

            switch (functionCall)
            {
                case FunctionCallSyntax function:
                    return new FunctionCallOperation(
                        function.Name.IdentifierName,
                        function.Arguments.Select(a => ConvertExpressionOperation(a.Expression)).ToImmutableArray());

                case InstanceFunctionCallSyntax method:
                    var (baseSyntax, indexExpression) = SyntaxHelper.UnwrapArrayAccessSyntax(method.BaseExpression);
                    var baseSymbol = context.SemanticModel.GetSymbolInfo(baseSyntax);

                    switch (baseSymbol)
                    {
                        case INamespaceSymbol namespaceSymbol:
                            Debug.Assert(indexExpression is null, "Indexing into a namespace should have been blocked by type analysis");
                            return new FunctionCallOperation(
                                method.Name.IdentifierName,
                                method.Arguments.Select(a => ConvertExpressionOperation(a.Expression)).ToImmutableArray());
                        case { } _ when context.SemanticModel.ResourceMetadata.TryLookup(baseSyntax) is DeclaredResourceMetadata resource:
                            if (method.Name.IdentifierName.StartsWithOrdinalInsensitively("list"))
                            {
                                // Handle list<method_name>(...) method on resource symbol - e.g. stgAcc.listKeys()
                                var indexContext = TryGetReplacementContext(resource.NameSyntax, indexExpression, method);
                                var resourceIdOperation = new ResourceIdOperation(resource, indexContext);

                                var convertedArgs = method.Arguments.SelectArray(a => ConvertExpressionOperation(a.Expression));

                                var apiVersion = resource.TypeReference.ApiVersion ?? throw new InvalidOperationException($"Expected resource type {resource.TypeReference.FormatName()} to contain version");
                                var apiVersionOperation = new ConstantValueOperation(apiVersion);

                                var listArgs = convertedArgs.Length switch
                                {
                                    0 => new Operation[] { resourceIdOperation, apiVersionOperation, },
                                    _ => new Operation[] { resourceIdOperation, }.Concat(convertedArgs),
                                };

                                return new FunctionCallOperation(
                                    method.Name.IdentifierName,
                                    listArgs.ToImmutableArray());
                            }

                            if (LanguageConstants.IdentifierComparer.Equals(method.Name.IdentifierName, "getSecret"))
                            {
                                var indexContext = TryGetReplacementContext(resource.NameSyntax, indexExpression, method);
                                var resourceIdOperation = new ResourceIdOperation(resource, indexContext);

                                var convertedArgs = method.Arguments.SelectArray(a => ConvertExpressionOperation(a.Expression));

                                return new GetKeyVaultSecretOperation(
                                    resourceIdOperation,
                                    convertedArgs[0],
                                    convertedArgs.Length > 1 ? convertedArgs[1] : null);
                            }

                            break;
                    }
                    throw new InvalidOperationException($"Unrecognized base expression {baseSymbol?.Kind}");
                default:
                    throw new NotImplementedException($"Cannot emit unexpected expression of type {functionCall.GetType().Name}");
            }
        }

        public IndexReplacementContext? TryGetReplacementContext(DeclaredResourceMetadata resource, SyntaxBase? indexExpression, SyntaxBase newContext)
        {
            var movedSyntax = context.Settings.EnableSymbolicNames ? resource.Symbol.NameSyntax : resource.NameSyntax;

            return TryGetReplacementContext(movedSyntax, indexExpression, newContext);
        }

        public IndexReplacementContext? TryGetReplacementContext(SyntaxBase nameSyntax, SyntaxBase? indexExpression, SyntaxBase newContext)
        {
            var inaccessibleLocals = this.context.DataFlowAnalyzer.GetInaccessibleLocalsAfterSyntaxMove(nameSyntax, newContext);
            var inaccessibleLocalLoops = inaccessibleLocals.Select(local => GetEnclosingForExpression(local)).Distinct().ToList();

            switch (inaccessibleLocalLoops.Count)
            {
                case 0:
                    // moving the name expression does not produce any inaccessible locals (no locals means no loops)
                    // regardless if there is an index expression or not, we don't need to append replacements
                    if (indexExpression is null)
                    {
                        return null;
                    }

                    return new(this.localReplacements, ConvertExpressionOperation(indexExpression));

                case 1 when indexExpression is not null:
                    // TODO: Run data flow analysis on the array expression as well. (Will be needed for nested resource loops)
                    var @for = inaccessibleLocalLoops.Single();
                    var localReplacements = this.localReplacements;
                    var converter = new ExpressionConverter(this.context, localReplacements);
                    foreach (var local in inaccessibleLocals)
                    {
                        // Allow local variable symbol replacements to be overwritten, as there are scenarios where we recursively generate expressions for the same index symbol
                        var replacementValue = GetLoopVariable(local, @for, converter.ConvertExpressionOperation(indexExpression));
                        localReplacements = localReplacements.SetItem(local, replacementValue);
                    }

                    return new(localReplacements, converter.ConvertExpressionOperation(indexExpression));

                default:
                    throw new NotImplementedException("Mismatch between count of index expressions and inaccessible symbols during array access index replacement.");
            }
        }

        private ExpressionConverter GetConverter(IndexReplacementContext? replacementContext)
        {
            if (replacementContext is not null)
            {
                return new(this.context, replacementContext.LocalReplacements);
            }

            return this;
        }

        public ExpressionConverter CreateConverterForIndexReplacement(SyntaxBase nameSyntax, SyntaxBase? indexExpression, SyntaxBase newContext)
            => GetConverter(
                TryGetReplacementContext(nameSyntax, indexExpression, newContext));

        private Operation ConvertArrayAccess(ArrayAccessSyntax arrayAccess)
        {
            // if there is an array access on a resource/module reference, we have to generate differently
            // when constructing the reference() function call, the resource name expression needs to have its local
            // variable replaced with <loop array expression>[this array access' index expression]
            if (arrayAccess.BaseExpression is VariableAccessSyntax || arrayAccess.BaseExpression is ResourceAccessSyntax)
            {
                if (context.SemanticModel.ResourceMetadata.TryLookup(arrayAccess.BaseExpression) is DeclaredResourceMetadata resource &&
                    resource.Symbol.IsCollection)
                {
                    var indexContent = TryGetReplacementContext(resource, arrayAccess.IndexExpression, arrayAccess);
                    return GetResourceReference(resource, indexContent, full: true);
                }

                if (context.SemanticModel.GetSymbolInfo(arrayAccess.BaseExpression) is ModuleSymbol { IsCollection: true } moduleSymbol)
                {
                    var indexContent = TryGetReplacementContext(ExpressionConverter.GetModuleNameSyntax(moduleSymbol), arrayAccess.IndexExpression, arrayAccess);
                    return GetModuleOutputsReference(moduleSymbol, indexContent);
                }
            }

            return new ArrayAccessOperation(
                ConvertExpressionOperation(arrayAccess.BaseExpression),
                ConvertExpressionOperation(arrayAccess.IndexExpression));
        }

        public LanguageExpression ConvertOperation(Operation operation)
        {
            switch (operation)
            {
                case ConstantValueOperation op:
                    {
                        return op.Value switch
                        {
                            string value => new JTokenExpression(value),
                            long value when value <= int.MaxValue && value >= int.MinValue => new JTokenExpression((int)value),
                            long value => CreateFunction("json", new JTokenExpression(value.ToInvariantString())),
                            bool value => CreateFunction(value ? "true" : "false"),
                            _ => throw new NotImplementedException($"Cannot convert constant type {op.Value?.GetType()}"),
                        };
                    }
                case NullValueOperation op:
                    {
                        return CreateFunction("null");
                    }
                case PropertyAccessOperation op:
                    {
                        return AppendProperties(
                            ToFunctionExpression(ConvertOperation(op.Base)),
                            new JTokenExpression(op.PropertyName));
                    }
                case ArrayAccessOperation op:
                    {
                        return AppendProperties(
                            ToFunctionExpression(ConvertOperation(op.Base)),
                            ConvertOperation(op.Access));
                    }
                case ResourceIdOperation op:
                    {
                        return GetConverter(op.IndexContext).GetFullyQualifiedResourceId(op.Metadata);
                    }
                case ResourceNameOperation op:
                    {
                        return op.FullyQualified switch {
                            true => GetConverter(op.IndexContext).GetFullyQualifiedResourceName(op.Metadata),
                            false => GetConverter(op.IndexContext).GetUnqualifiedResourceName(op.Metadata),
                        };
                    }
                case ResourceTypeOperation op:
                    {
                        return new JTokenExpression(op.Metadata.TypeReference.FormatType());
                    }
                case ResourceApiVersionOperation op:
                    {
                        return op.Metadata.TypeReference.ApiVersion switch
                        {
                            { } apiVersion => new JTokenExpression(apiVersion),
                            _ => throw new NotImplementedException(""),
                        };
                    }
                case ResourceInfoOperation op:
                    {
                        return CreateFunction(
                            "resourceInfo",
                            GenerateSymbolicReference(op.Metadata.Symbol.Name, op.IndexContext));
                    }
                case ResourceReferenceOperation op:
                    {
                        return (op.Full, op.ShouldIncludeApiVersion) switch
                        {
                            (true, _) => CreateFunction(
                                "reference",
                                ConvertOperation(op.ResourceId),
                                new JTokenExpression(op.Metadata.TypeReference.ApiVersion!),
                                new JTokenExpression("full")),
                            (false, false) => CreateFunction(
                                "reference",
                                ConvertOperation(op.ResourceId)),
                            (false, true) => CreateFunction(
                                "reference",
                                ConvertOperation(op.ResourceId),
                                new JTokenExpression(op.Metadata.TypeReference.ApiVersion!)),
                        };
                    }
                case SymbolicResourceReferenceOperation op:
                    {
                        return (op.Full, op.Metadata.IsAzResource) switch
                        {
                            (true, true) => CreateFunction(
                                "reference",
                                GenerateSymbolicReference(op.Metadata.Symbol.Name, op.IndexContext),
                                new JTokenExpression(op.Metadata.TypeReference.ApiVersion!),
                                new JTokenExpression("full")),
                            _ => CreateFunction(
                                "reference",
                                GenerateSymbolicReference(op.Metadata.Symbol.Name, op.IndexContext)),
                        };
                    }
                case ModuleNameOperation op:
                    {
                        return GetConverter(op.IndexContext).GetModuleNameExpression(op.Symbol);
                    }
                case VariableAccessOperation op:
                    {
                        return CreateFunction(
                            "variables",
                            new JTokenExpression(op.Symbol.Name));
                    }
                case ExplicitVariableAccessOperation op:
                    {
                        return CreateFunction(
                            "variables",
                            new JTokenExpression(op.Name));
                    }
                case ParameterAccessOperation op:
                    {
                        return CreateFunction(
                            "parameters",
                            new JTokenExpression(op.Symbol.Name));
                    }
                case ModuleOutputOperation op:
                    {
                        var reference = ConvertOperation(new ModuleReferenceOperation(op.Symbol, op.IndexContext));

                        return AppendProperties(
                            ToFunctionExpression(reference),
                            new JTokenExpression("outputs"),
                            ConvertOperation(op.PropertyName),
                            new JTokenExpression("value"));
                    }
                case ModuleReferenceOperation op:
                    {
                        // See https://github.com/Azure/bicep/issues/6008 for more info
                        var shouldIncludeApiVersion = op.Symbol.DeclaringModule.HasCondition();

                        return (context.Settings.EnableSymbolicNames, shouldIncludeApiVersion) switch
                        {
                            (true, _) => CreateFunction(
                                "reference",
                                GenerateSymbolicReference(op.Symbol.Name, op.IndexContext)),
                            (false, false) => CreateFunction(
                                "reference",
                                GetConverter(op.IndexContext).GetFullyQualifiedResourceId(op.Symbol)),
                            (false, true) => CreateFunction(
                                "reference",
                                GetConverter(op.IndexContext).GetFullyQualifiedResourceId(op.Symbol),
                                new JTokenExpression(TemplateWriter.NestedDeploymentResourceApiVersion)),
                        };
                    }
                case FunctionCallOperation op:
                    {
                        return CreateFunction(
                            op.Name,
                            op.Parameters.Select(p => ConvertOperation(p)));
                    }
                default:
                    throw new NotImplementedException("");
            };
        }

        private Operation ConvertResourcePropertyAccess(ResourceMetadata resource, IndexReplacementContext? indexContext, string propertyName)
        {
            if (context.Settings.EnableSymbolicNames &&
                resource is DeclaredResourceMetadata declaredResource)
            {
                if (!resource.IsAzResource)
                {
                    // For an extensible resource, always generate a 'reference' statement.
                    // User-defined properties appear inside "properties", so use a non-full reference.
                    return new PropertyAccessOperation(
                        new SymbolicResourceReferenceOperation(declaredResource, indexContext, false),
                        propertyName);
                }

                if (context.Settings.EnableSymbolicNames)
                {
                    switch (propertyName)
                    {
                        case "id":
                        case "name":
                        case "type":
                        case "apiVersion":
                            return new PropertyAccessOperation(
                                new ResourceInfoOperation(declaredResource, indexContext),
                                propertyName);
                        case "properties":
                            return new SymbolicResourceReferenceOperation(declaredResource, indexContext, false);
                        default:
                            return new PropertyAccessOperation(
                                new SymbolicResourceReferenceOperation(declaredResource, indexContext, true),
                                propertyName);
                    }
                }
            }

            switch (propertyName)
            {
                case "id":
                    // the ID is dependent on the name expression which could involve locals in case of a resource collection
                    return new ResourceIdOperation(resource, indexContext);
                case "name":
                    // the name is dependent on the name expression which could involve locals in case of a resource collection

                    // Note that we don't want to return the fully-qualified resource name in the case of name property access.
                    // we should return whatever the user has set as the value of the 'name' property for a predictable user experience.
                    return new ResourceNameOperation(resource, indexContext, FullyQualified: false);
                case "type":
                    return new ResourceTypeOperation(resource);
                case "apiVersion":
                    return new ResourceApiVersionOperation(resource);
                case "properties":
                    var shouldIncludeApiVersion = resource.IsExistingResource ||
                        (resource is DeclaredResourceMetadata { Symbol.DeclaringResource: var declaringResource } && declaringResource.HasCondition());

                    return new ResourceReferenceOperation(
                        resource,
                        new ResourceIdOperation(resource, indexContext),
                        Full: false,
                        ShouldIncludeApiVersion: shouldIncludeApiVersion);
                default:
                    return new PropertyAccessOperation(
                        new ResourceReferenceOperation(
                            resource,
                            new ResourceIdOperation(resource, indexContext),
                            Full: true,
                            ShouldIncludeApiVersion: true),
                        propertyName);
            }
        }

        private Operation ConvertModuleOutput(ModuleSymbol moduleSymbol, IndexReplacementContext? indexContext, string propertyName)
        {
            return new ModuleOutputOperation(
                moduleSymbol,
                indexContext,
                new ConstantValueOperation(propertyName));
        }

        private Operation ConvertModulePropertyAccess(ModuleSymbol moduleSymbol, string propertyName, IndexReplacementContext? indexContext)
        {
            switch (propertyName)
            {
                case LanguageConstants.ModuleNamePropertyName:
                    // the name is dependent on the name expression which could involve locals in case of a resource collection
                    return new ModuleNameOperation(moduleSymbol, indexContext);
                default:
                    throw new NotImplementedException("Property access is only implemented for module name");
            }
        }

        private Operation ConvertPropertyAccess(PropertyAccessSyntax propertyAccess)
        {
            if (context.SemanticModel.ResourceMetadata.TryLookup(propertyAccess.BaseExpression) is DeclaredResourceMetadata resource)
            {
                // we are doing property access on a single resource
                var indexContext = TryGetReplacementContext(resource, null, propertyAccess);
                return ConvertResourcePropertyAccess(resource, indexContext, propertyAccess.PropertyName.IdentifierName);
            }

            if ((propertyAccess.BaseExpression is VariableAccessSyntax || propertyAccess.BaseExpression is ResourceAccessSyntax) &&
                context.SemanticModel.ResourceMetadata.TryLookup(propertyAccess.BaseExpression) is ResourceMetadata parameter)
            {
                // we are doing property access on a single resource
                // and we are dealing with special case properties
                return ConvertResourcePropertyAccess(parameter, null, propertyAccess.PropertyName.IdentifierName);
            }

            if (propertyAccess.BaseExpression is ArrayAccessSyntax propArrayAccess &&
                context.SemanticModel.ResourceMetadata.TryLookup(propArrayAccess.BaseExpression) is DeclaredResourceMetadata resourceCollection)
            {
                // we are doing property access on an array access of a resource collection
                var indexContext = TryGetReplacementContext(resourceCollection, propArrayAccess.IndexExpression, propertyAccess);
                return ConvertResourcePropertyAccess(resourceCollection, indexContext, propertyAccess.PropertyName.IdentifierName);
            }

            if (propertyAccess.BaseExpression is PropertyAccessSyntax &&
                context.SemanticModel.ResourceMetadata.TryLookup(propertyAccess.BaseExpression) is ModuleOutputResourceMetadata moduleOutput &&
                !moduleOutput.Module.IsCollection)
            {
                // we are doing property access on an output of a non-collection module.
                // and we are dealing with special case properties
                return this.ConvertResourcePropertyAccess(moduleOutput, null, propertyAccess.PropertyName.IdentifierName);
            }

            if (propertyAccess.BaseExpression is PropertyAccessSyntax moduleCollectionOutputProperty &&
                moduleCollectionOutputProperty.BaseExpression is PropertyAccessSyntax moduleCollectionOutputs &&
                moduleCollectionOutputs.BaseExpression is ArrayAccessSyntax moduleArrayAccess &&
                context.SemanticModel.ResourceMetadata.TryLookup(propertyAccess.BaseExpression) is ModuleOutputResourceMetadata moduleCollectionOutputMetadata &&
                moduleCollectionOutputMetadata.Module.IsCollection)
            {
                // we are doing property access on an output of an array of modules.
                // and we are dealing with special case properties
                var indexContext = TryGetReplacementContext(moduleCollectionOutputMetadata.NameSyntax, moduleArrayAccess.IndexExpression, propertyAccess);
                return ConvertResourcePropertyAccess(moduleCollectionOutputMetadata, indexContext, propertyAccess.PropertyName.IdentifierName);
            }

            if (context.SemanticModel.GetSymbolInfo(propertyAccess.BaseExpression) is ModuleSymbol moduleSymbol)
            {
                // we are doing property access on a single module
                var indexContext = TryGetReplacementContext(GetModuleNameSyntax(moduleSymbol), null, propertyAccess);
                return ConvertModulePropertyAccess(moduleSymbol, propertyAccess.PropertyName.IdentifierName, indexContext);
            }

            if (propertyAccess.BaseExpression is ArrayAccessSyntax modulePropArrayAccess &&
                context.SemanticModel.GetSymbolInfo(modulePropArrayAccess.BaseExpression) is ModuleSymbol moduleCollectionSymbol)
            {
                // we are doing property access on an array access of a module collection
                var indexContext = TryGetReplacementContext(GetModuleNameSyntax(moduleCollectionSymbol), modulePropArrayAccess.IndexExpression, propertyAccess);
                return ConvertModulePropertyAccess(moduleCollectionSymbol, propertyAccess.PropertyName.IdentifierName, indexContext);
            }

            if (propertyAccess.BaseExpression is PropertyAccessSyntax childPropertyAccess &&
                childPropertyAccess.PropertyName.NameEquals(LanguageConstants.ModuleOutputsPropertyName))
            {
                if (childPropertyAccess.BaseExpression is VariableAccessSyntax grandChildVariableAccess &&
                    context.SemanticModel.GetSymbolInfo(grandChildVariableAccess) is VariableSymbol variableSymbol &&
                    context.VariablesToInline.Contains(variableSymbol))
                {
                    // This is imprecise as we don't check that that variable being accessed is solely composed of modules. We'd end up generating incorrect code for:
                    // var foo = false ? mod1 : varWithOutputs
                    // var bar = foo.outputs.someProp
                    return new PropertyAccessOperation(
                        new PropertyAccessOperation(
                            ConvertVariableAccess(grandChildVariableAccess),
                            propertyAccess.PropertyName.IdentifierName),
                        "value");
                }

                if (context.SemanticModel.GetSymbolInfo(childPropertyAccess.BaseExpression) is ModuleSymbol outputModuleSymbol)
                {
                    var indexContext = TryGetReplacementContext(GetModuleNameSyntax(outputModuleSymbol), null, propertyAccess);
                    return ConvertModuleOutput(outputModuleSymbol, indexContext, propertyAccess.PropertyName.IdentifierName);
                }

                if (childPropertyAccess.BaseExpression is ArrayAccessSyntax outputModulePropArrayAccess &&
                    context.SemanticModel.GetSymbolInfo(outputModulePropArrayAccess.BaseExpression) is ModuleSymbol outputArrayModuleSymbol)
                {
                    var indexContext = TryGetReplacementContext(GetModuleNameSyntax(outputArrayModuleSymbol), outputModulePropArrayAccess.IndexExpression, propertyAccess);
                    return ConvertModuleOutput(outputArrayModuleSymbol, indexContext, propertyAccess.PropertyName.IdentifierName);
                }
            }

            return new PropertyAccessOperation(
                ConvertExpressionOperation(propertyAccess.BaseExpression),
                propertyAccess.PropertyName.IdentifierName);
        }

        public IEnumerable<LanguageExpression> GetResourceNameSegments(DeclaredResourceMetadata resource)
        {
            // TODO move this into az extension
            var typeReference = resource.TypeReference;
            var ancestors = this.context.SemanticModel.ResourceAncestors.GetAncestors(resource);
            var nameExpression = ConvertExpression(resource.NameSyntax);

            var typesAfterProvider = typeReference.TypeSegments.Skip(1).ToImmutableArray();

            if (ancestors.Length > 0)
            {
                var firstAncestorNameLength = typesAfterProvider.Length - ancestors.Length;

                var parentNames = ancestors.SelectMany((x, i) =>
                {
                    var expression = GetResourceNameAncestorSyntaxSegment(resource, i);
                    var nameExpression = this.ConvertExpression(expression);

                    if (i == 0 && firstAncestorNameLength > 1)
                    {
                        return Enumerable.Range(0, firstAncestorNameLength).Select(
                            (_, i) => AppendProperties(
                                CreateFunction("split", nameExpression, new JTokenExpression("/")),
                                new JTokenExpression(i)));
                    }

                    return nameExpression.AsEnumerable();
                });

                return parentNames.Concat(nameExpression.AsEnumerable());
            }

            if (typesAfterProvider.Length == 1)
            {
                return nameExpression.AsEnumerable();
            }

            return typesAfterProvider.Select(
                (type, i) => AppendProperties(
                    CreateFunction("split", nameExpression, new JTokenExpression("/")),
                    new JTokenExpression(i)));
        }        /// <summary>
        /// Returns a collection of name segment expressions for the specified resource. Local variable replacements
        /// are performed so the expressions are valid in the language/binding scope of the specified resource.
        /// </summary>
        /// <param name="resource">The resource</param>
        public IEnumerable<SyntaxBase> GetResourceNameSyntaxSegments(DeclaredResourceMetadata resource)
        {
            var ancestors = this.context.SemanticModel.ResourceAncestors.GetAncestors(resource);
            var nameExpression = resource.NameSyntax;

            return ancestors
                .Select((x, i) => GetResourceNameAncestorSyntaxSegment(resource, i))
                .Concat(nameExpression);
        }

        /// <summary>
        /// Calculates the expression that represents the parent name corresponding to the specified ancestor of the specified resource.
        /// The expressions returned are modified by performing the necessary local variable replacements.
        /// </summary>
        /// <param name="resource">The declared resource metadata</param>
        /// <param name="startingAncestorIndex">the index of the ancestor (0 means the ancestor closest to the root)</param>
        private SyntaxBase GetResourceNameAncestorSyntaxSegment(DeclaredResourceMetadata resource, int startingAncestorIndex)
        {
            var ancestors = this.context.SemanticModel.ResourceAncestors.GetAncestors(resource);
            if(startingAncestorIndex >= ancestors.Length)
            {
                // not enough ancestors
                throw new ArgumentException($"Resource type has {ancestors.Length} ancestor types but name expression was requested for ancestor type at index {startingAncestorIndex}.");
            }

            /*
             * Consider the following example:
             *
             * resource one 'MS.Example/ones@...' = [for (_, i) in range(0, ...) : {
             *   name: name_exp1(i)
             * }]
             *
             * resource two 'MS.Example/ones/twos@...' = [for (_, j) in range(0, ...) : {
             *   parent: one[index_exp2(j)]
             *   name: name_exp2(j)
             * }]
             *
             * resource three 'MS.Example/ones/twos/threes@...' = [for (_, k) in range(0, ...) : {
             *   parent: two[index_exp3(k)]
             *   name: name_exp3(k)
             * }]
             *
             * name_exp* and index_exp* are expressions represented here as functions
             *
             * The name segment expressions for "three" are the following:
             * 0. name_exp1(index_exp2(index_exp3(k)))
             * 1. name_exp2(index_exp3(k))
             * 2. name_exp3(k)
             *
             * (The formula can be generalized to more levels of nesting.)
             *
             * This function can be used to get 0 and 1 above by passing 0 or 1 respectively as the startingAncestorIndex.
             * The name segment 2 above must be obtained from the resource directly.
             *
             * Given that we don't have proper functions in our runtime AND that our expressions don't have side effects,
             * the formula is implemented via local variable replacement.
             */

            // the initial ancestor gives us the base expression
            SyntaxBase? rewritten = ancestors[startingAncestorIndex].Resource.NameSyntax;

            for(int i = startingAncestorIndex; i < ancestors.Length; i++)
            {
                var ancestor = ancestors[i];

                // local variable replacement will be done in context of the next ancestor
                // or the resource itself if we're on the last ancestor
                var newContext = i < ancestors.Length - 1 ? ancestors[i + 1].Resource : resource;

                var inaccessibleLocals = this.context.DataFlowAnalyzer.GetInaccessibleLocalsAfterSyntaxMove(rewritten, newContext.Symbol.NameSyntax);
                var inaccessibleLocalLoops = inaccessibleLocals.Select(local => GetEnclosingForExpression(local)).Distinct().ToList();

                switch (inaccessibleLocalLoops.Count)
                {
                    case 0 when i == startingAncestorIndex:
                        /*
                         * There are no local vars to replace. It is impossible for a local var to be introduced at the next level
                         * so we can just bail out with the result.
                         *
                         * This path is followed by non-loop resources.
                         *
                         * Case 0 is not possible for non-starting ancestor index because
                         * once we have a local variable replacement, it will propagate to the next levels
                         */
                        return ancestor.Resource.NameSyntax;

                    case 1 when ancestor.IndexExpression is not null:
                        if (LocalSymbolDependencyVisitor.GetLocalSymbolDependencies(this.context.SemanticModel, rewritten).SingleOrDefault(s => s.LocalKind == LocalKind.ForExpressionItemVariable) is { } loopItemSymbol)
                        {
                            // rewrite the straggler from previous iteration
                            // TODO: Nested loops will require DFA on the ForSyntax.Expression
                            rewritten = SymbolReplacer.Replace(this.context.SemanticModel, new Dictionary<Symbol, SyntaxBase> { [loopItemSymbol] = SyntaxFactory.CreateArrayAccess(GetEnclosingForExpression(loopItemSymbol).Expression, ancestor.IndexExpression) }, rewritten);
                        }

                        // TODO: Run data flow analysis on the array expression as well. (Will be needed for nested resource loops)
                        var @for = inaccessibleLocalLoops.Single();

                        var replacements = inaccessibleLocals.ToDictionary(local => (Symbol)local, local => local.LocalKind switch
                              {
                                  LocalKind.ForExpressionIndexVariable => ancestor.IndexExpression,
                                  LocalKind.ForExpressionItemVariable => SyntaxFactory.CreateArrayAccess(@for.Expression, ancestor.IndexExpression),
                                  _ => throw new NotImplementedException($"Unexpected local kind '{local.LocalKind}'.")
                              });

                        rewritten = SymbolReplacer.Replace(this.context.SemanticModel, replacements, rewritten);

                        break;

                    default:
                        throw new NotImplementedException("Mismatch between count of index expressions and inaccessible symbols during array access index expression rewriting.");
                }
            }

            return rewritten;
        }

        public LanguageExpression GetFullyQualifiedResourceName(ResourceMetadata resource)
        {
            switch (resource)
            {
                case DeclaredResourceMetadata declaredResource:
                    var nameValueSyntax = declaredResource.NameSyntax;

                    // For a nested resource we need to compute the name
                    var ancestors = this.context.SemanticModel.ResourceAncestors.GetAncestors(declaredResource);
                    if (ancestors.Length == 0)
                    {
                        return ConvertExpression(nameValueSyntax);
                    }

                    // Build an expression like '${parent.name}/${child.name}'
                    //
                    // This is a call to the `format` function with the first arg as a format string
                    // and the remaining args the actual name segments.
                    //
                    // args.Length = 1 (format string) + N (ancestor names) + 1 (resource name)

                    var nameSegments = GetResourceNameSegments(declaredResource);
                    // {0}/{1}/{2}....
                    var formatString = string.Join("/", nameSegments.Select((_, i) => $"{{{i}}}"));

                    return CreateFunction("format", new JTokenExpression(formatString).AsEnumerable().Concat(nameSegments));
                case ModuleOutputResourceMetadata:
                case ParameterResourceMetadata:
                    // TODO(antmarti): Can we come up with an expression to get the fully-qualified name here?
                    return GetUnqualifiedResourceName(resource);
                default:
                    throw new InvalidOperationException($"Unsupported resource metadata type: {resource}");
            }

        }

        public LanguageExpression GetUnqualifiedResourceName(ResourceMetadata resource)
        {
            switch (resource)
            {
                case DeclaredResourceMetadata declaredResource:
                    return ConvertExpression(declaredResource.NameSyntax);
                case ModuleOutputResourceMetadata:
                case ParameterResourceMetadata:
                    // create an expression like: `last(split(<resource id>, '/'))`
                    return CreateFunction(
                        "last",
                        CreateFunction(
                            "split",
                            GetFullyQualifiedResourceId(resource),
                            new JTokenExpression("/")));
                default:
                    throw new InvalidOperationException($"Unsupported resource metadata type: {resource}");
            }
        }

        private LanguageExpression GetModuleNameExpression(ModuleSymbol moduleSymbol)
        {
            SyntaxBase nameValueSyntax = GetModuleNameSyntax(moduleSymbol);
            return ConvertExpression(nameValueSyntax);
        }

        public static SyntaxBase GetModuleNameSyntax(ModuleSymbol moduleSymbol)
        {
            // this condition should have already been validated by the type checker
            return moduleSymbol.TryGetBodyPropertyValue(LanguageConstants.ModuleNamePropertyName) ?? throw new ArgumentException($"Expected module syntax body to contain property 'name'");
        }

        public LanguageExpression GetUnqualifiedResourceId(DeclaredResourceMetadata resource)
        {
            return ScopeHelper.FormatUnqualifiedResourceId(
                context,
                this,
                context.ResourceScopeData[resource],
                resource.TypeReference.FormatType(),
                GetResourceNameSegments(resource));
        }

        public LanguageExpression GetFullyQualifiedResourceId(ResourceMetadata resource)
        {
            return resource switch {
                ParameterResourceMetadata parameter => ConvertOperation(new ParameterAccessOperation(parameter.Symbol)),
                ModuleOutputResourceMetadata output => ConvertOperation(new ModuleOutputOperation(output.Module, null, new ConstantValueOperation(output.OutputName))),
                DeclaredResourceMetadata declared => ScopeHelper.FormatFullyQualifiedResourceId(
                    context,
                    this,
                    context.ResourceScopeData[declared],
                    resource.TypeReference.FormatType(),
                    GetResourceNameSegments(declared)),
                _ => throw new InvalidOperationException($"Unsupported resource metadata type: {resource}"),
            };
        }

        public LanguageExpression GetFullyQualifiedResourceId(ModuleSymbol moduleSymbol)
        {
            return ScopeHelper.FormatFullyQualifiedResourceId(
                context,
                this,
                context.ModuleScopeData[moduleSymbol],
                TemplateWriter.NestedDeploymentResourceType,
                GetModuleNameExpression(moduleSymbol).AsEnumerable());
        }

        private Operation GetModuleOutputsReference(ModuleSymbol moduleSymbol, IndexReplacementContext? indexContext)
        {
            return new PropertyAccessOperation(
                new ModuleReferenceOperation(
                    moduleSymbol,
                    indexContext),
                "outputs");
        }

        private Operation GetResourceReference(ResourceMetadata resource, IndexReplacementContext? indexContext, bool full)
        {
            return (context.Settings.EnableSymbolicNames, resource) switch {
                (true, DeclaredResourceMetadata declaredResource) => new SymbolicResourceReferenceOperation(declaredResource, indexContext, true),
                _ => new ResourceReferenceOperation(
                    resource,
                    new ResourceIdOperation(resource, indexContext),
                    Full: true,
                    ShouldIncludeApiVersion: true),
            };
        }

        private Operation GetLocalVariable(LocalVariableSymbol localVariableSymbol)
        {
            if (this.localReplacements.TryGetValue(localVariableSymbol, out var replacement))
            {
                // the current context has specified an expression to be used for this local variable symbol
                // to override the regular conversion
                return replacement;
            }

            var @for = GetEnclosingForExpression(localVariableSymbol);
            return GetLoopVariable(localVariableSymbol, @for, CreateCopyIndexFunction(@for));
        }

        private Operation GetLoopVariable(LocalVariableSymbol localVariableSymbol, ForSyntax @for, Operation indexOperation)
        {
            return localVariableSymbol.LocalKind switch
            {
                // this is the "item" variable of a for-expression
                // to emit this, we need to index the array expression by the copyIndex() function
                LocalKind.ForExpressionItemVariable => GetLoopItemVariable(@for, indexOperation),

                // this is the "index" variable of a for-expression inside a variable block
                // to emit this, we need to return a copyIndex(...) function
                LocalKind.ForExpressionIndexVariable => indexOperation,

                _ => throw new NotImplementedException($"Unexpected local variable kind '{localVariableSymbol.LocalKind}'."),
            };
        }

        private ForSyntax GetEnclosingForExpression(LocalVariableSymbol localVariable)
        {
            // we're following the symbol hierarchy rather than syntax hierarchy because
            // this guarantees a single hop in all cases
            var symbolParent = this.context.SemanticModel.GetSymbolParent(localVariable);
            if (symbolParent is not LocalScope localScope)
            {
                throw new NotImplementedException($"{nameof(LocalVariableSymbol)} has un unexpected parent of type '{symbolParent?.GetType().Name}'.");
            }

            if (localScope.DeclaringSyntax is ForSyntax @for)
            {
                return @for;
            }

            throw new NotImplementedException($"{nameof(LocalVariableSymbol)} was declared by an unexpected syntax type '{localScope.DeclaringSyntax?.GetType().Name}'.");
        }

        private string? GetCopyIndexName(ForSyntax @for)
        {
            return this.context.SemanticModel.Binder.GetParent(@for) switch
            {
                // copyIndex without name resolves to module/resource loop index in the runtime
                ResourceDeclarationSyntax => null,
                ModuleDeclarationSyntax => null,

                // variable copy index has the name of the variable
                VariableDeclarationSyntax variable when variable.Name.IsValid => variable.Name.IdentifierName,

                // output loops are only allowed at the top level and don't have names, either
                OutputDeclarationSyntax => null,

                // the property copy index has the name of the property
                ObjectPropertySyntax property when property.TryGetKeyText() is { } key && ReferenceEquals(property.Value, @for) => key,

                _ => throw new NotImplementedException("Unexpected for-expression grandparent.")
            };
        }

        private Operation CreateCopyIndexFunction(ForSyntax @for)
        {
            var copyIndexName = GetCopyIndexName(@for);
            return copyIndexName is null
                ? new FunctionCallOperation("copyIndex")
                : new FunctionCallOperation("copyIndex", new ConstantValueOperation(copyIndexName));
        }

        private Operation GetLoopItemVariable(ForSyntax @for, Operation indexOperation)
        {
            // loop item variable should be replaced with <array expression>[<index expression>]
            var forOperation = ConvertExpressionOperation(@for.Expression);

            return new ArrayAccessOperation(forOperation, indexOperation);
        }

        private Operation ConvertVariableAccess(VariableAccessSyntax variableAccessSyntax)
        {
            var name = variableAccessSyntax.Name.IdentifierName;

            if (variableAccessSyntax is ExplicitVariableAccessSyntax)
            {
                //just return a call to variables.
                return new ExplicitVariableAccessOperation(name);
            }

            var symbol = context.SemanticModel.GetSymbolInfo(variableAccessSyntax);

            switch (symbol)
            {
                case ParameterSymbol parameterSymbol when context.SemanticModel.ResourceMetadata.TryLookup(parameterSymbol.DeclaringSyntax) is {} resource:
                    // This is a reference to a pre-existing resource where the resource ID was passed in as a
                    // string. Generate a call to reference().
                    return GetResourceReference(resource, null, true);
                case ParameterSymbol parameterSymbol:
                    return new ParameterAccessOperation(parameterSymbol);

                case VariableSymbol variableSymbol:
                    if (context.VariablesToInline.Contains(variableSymbol))
                    {
                        // we've got a runtime dependency, so we have to inline the variable usage
                        return ConvertExpressionOperation(variableSymbol.DeclaringVariable.Value);
                    }

                    return new VariableAccessOperation(variableSymbol);

                case ResourceSymbol when context.SemanticModel.ResourceMetadata.TryLookup(variableAccessSyntax) is { } resource:
                    return GetResourceReference(resource, null, true);

                case ModuleSymbol moduleSymbol:
                    return GetModuleOutputsReference(moduleSymbol, null);

                case LocalVariableSymbol localVariableSymbol:
                    return GetLocalVariable(localVariableSymbol);

                default:
                    throw new NotImplementedException($"Encountered an unexpected symbol kind '{symbol?.Kind}' when generating a variable access expression.");
            }
        }

        private Operation ConvertResourceAccess(ResourceAccessSyntax resourceAccessSyntax)
        {
            if (context.SemanticModel.ResourceMetadata.TryLookup(resourceAccessSyntax) is { } resource)
            {
                return GetResourceReference(resource, null, true);
            }

            throw new NotImplementedException($"Unable to obtain resource metadata when generating a resource access expression.");
        }

        private Operation ConvertString(StringSyntax syntax)
        {
            if (syntax.TryGetLiteralValue() is string literalStringValue)
            {
                // no need to build a format string
                return new ConstantValueOperation(literalStringValue);
            }

            var formatArgs = new Operation[syntax.Expressions.Length + 1];

            var formatString = StringFormatConverter.BuildFormatString(syntax);
            formatArgs[0] = new ConstantValueOperation(formatString);

            for (var i = 0; i < syntax.Expressions.Length; i++)
            {
                formatArgs[i + 1] = ConvertExpressionOperation(syntax.Expressions[i]);
            }

            return new FunctionCallOperation("format", formatArgs);
        }

        /// <summary>
        /// Converts a given language expression into an ARM template expression tree.
        /// This always returns a function expression, which is useful when converting property access or array access
        /// on literals.
        /// </summary>
        /// <param name="expression">The expression</param>
        public static FunctionExpression ToFunctionExpression(LanguageExpression converted)
        {
            switch (converted)
            {
                case FunctionExpression functionExpression:
                    return functionExpression;

                case JTokenExpression valueExpression:
                    JToken value = valueExpression.Value;

                    switch (value.Type)
                    {
                        case JTokenType.Integer:
                            // convert integer literal to a function call via int() function
                            return CreateFunction("int", valueExpression);

                        case JTokenType.String:
                            // convert string literal to function call via string() function
                            return CreateFunction("string", valueExpression);
                    }

                    break;
            }

            throw new NotImplementedException($"Unexpected expression type '{converted.GetType().Name}'.");
        }

        private Operation ConvertArray(ArraySyntax syntax)
        {
            // we are using the createArray() function as a proxy for an array literal
            return new FunctionCallOperation(
                "createArray",
                syntax.Items.Select(item => ConvertExpressionOperation(item.Value)).ToImmutableArray());
        }

        private Operation ConvertObject(ObjectSyntax syntax)
        {
            // need keys and values in one array of parameters
            var parameters = new Operation[syntax.Properties.Count() * 2];

            int index = 0;
            foreach (var propertySyntax in syntax.Properties)
            {
                parameters[index] = propertySyntax.Key switch
                {
                    IdentifierSyntax identifier => new ConstantValueOperation(identifier.IdentifierName),
                    StringSyntax @string => ConvertString(@string),
                    _ => throw new NotImplementedException($"Encountered an unexpected type '{propertySyntax.Key.GetType().Name}' when generating object's property name.")
                };
                index++;

                parameters[index] = ConvertExpressionOperation(propertySyntax.Value);
                index++;
            }

            // we are using the createObject() function as a proxy for an object literal
            return new FunctionCallOperation("createObject", parameters);
        }

        private Operation ConvertBinary(BinaryOperationSyntax syntax)
        {
            var operand1 = ConvertExpressionOperation(syntax.LeftExpression);
            var operand2 = ConvertExpressionOperation(syntax.RightExpression);

            return syntax.Operator switch
            {
                BinaryOperator.LogicalOr => new FunctionCallOperation("or", operand1, operand2),
                BinaryOperator.LogicalAnd => new FunctionCallOperation("and", operand1, operand2),
                BinaryOperator.Equals => new FunctionCallOperation("equals", operand1, operand2),
                BinaryOperator.NotEquals => new FunctionCallOperation("not",
                    new FunctionCallOperation("equals", operand1, operand2)),
                BinaryOperator.EqualsInsensitive => new FunctionCallOperation("equals",
                    new FunctionCallOperation("toLower", operand1),
                    new FunctionCallOperation("toLower", operand2)),
                BinaryOperator.NotEqualsInsensitive => new FunctionCallOperation("not",
                    new FunctionCallOperation("equals",
                        new FunctionCallOperation("toLower", operand1),
                        new FunctionCallOperation("toLower", operand2))),
                BinaryOperator.LessThan => new FunctionCallOperation("less", operand1, operand2),
                BinaryOperator.LessThanOrEqual => new FunctionCallOperation("lessOrEquals", operand1, operand2),
                BinaryOperator.GreaterThan => new FunctionCallOperation("greater", operand1, operand2),
                BinaryOperator.GreaterThanOrEqual => new FunctionCallOperation("greaterOrEquals", operand1, operand2),
                BinaryOperator.Add => new FunctionCallOperation("add", operand1, operand2),
                BinaryOperator.Subtract => new FunctionCallOperation("sub", operand1, operand2),
                BinaryOperator.Multiply => new FunctionCallOperation("mul", operand1, operand2),
                BinaryOperator.Divide => new FunctionCallOperation("div", operand1, operand2),
                BinaryOperator.Modulo => new FunctionCallOperation("mod", operand1, operand2),
                BinaryOperator.Coalesce => new FunctionCallOperation("coalesce", operand1, operand2),
                _ => throw new NotImplementedException($"Cannot emit unexpected binary operator '{syntax.Operator}'."),
            };
        }

        private Operation ConvertUnary(UnaryOperationSyntax syntax)
        {
            switch (syntax.Operator)
            {
                case UnaryOperator.Not:
                    return new FunctionCallOperation("not", ConvertExpressionOperation(syntax.Expression));

                case UnaryOperator.Minus:
                    if (syntax.Expression is IntegerLiteralSyntax integerLiteral)
                    {
                        var integerValue = integerLiteral.Value switch {
                            <= long.MaxValue => -(long)integerLiteral.Value,
                            (ulong)long.MaxValue + 1 => long.MinValue,
                            _ => throw new InvalidOperationException($"Integer syntax hs value {integerLiteral.Value} which will overflow"),
                        };

                        return new ConstantValueOperation(integerValue);
                    }

                    return new FunctionCallOperation(
                        "sub",
                        new[] {
                            new ConstantValueOperation(0),
                            ConvertExpressionOperation(syntax.Expression),
                        });

                default:
                    throw new NotImplementedException($"Cannot emit unexpected unary operator '{syntax.Operator}.");
            }
        }

        public LanguageExpression GenerateSymbolicReference(string symbolName, IndexReplacementContext? indexContext)
        {
            if (indexContext is null)
            {
                return new JTokenExpression(symbolName);
            }

            return CreateFunction(
                "format",
                new JTokenExpression($"{symbolName}[{{0}}]"),
                ConvertOperation(indexContext.Index));
        }

        public static LanguageExpression GenerateUnqualifiedResourceId(string fullyQualifiedType, IEnumerable<LanguageExpression> nameSegments)
        {
            var typeSegments = fullyQualifiedType.Split("/");

            // Generate a format string that looks like: My.Rp/type1/{0}/type2/{1}
            var formatString = $"{typeSegments[0]}/" + string.Join('/', typeSegments.Skip(1).Select((type, i) => $"{type}/{{{i}}}"));

            return CreateFunction(
                "format",
                new JTokenExpression(formatString).AsEnumerable().Concat(nameSegments));
        }

        public static LanguageExpression GenerateExtensionResourceId(LanguageExpression scope, string fullyQualifiedType, IEnumerable<LanguageExpression> nameSegments)
            => CreateFunction(
                "extensionResourceId",
                new[] { scope, new JTokenExpression(fullyQualifiedType), }.Concat(nameSegments));

        public static LanguageExpression GenerateResourceGroupScope(LanguageExpression subscriptionId, LanguageExpression resourceGroup)
            => CreateFunction(
                "format",
                new JTokenExpression("/subscriptions/{0}/resourceGroups/{1}"),
                subscriptionId,
                resourceGroup);

        public static LanguageExpression GenerateTenantResourceId(string fullyQualifiedType, IEnumerable<LanguageExpression> nameSegments)
            => CreateFunction(
                "tenantResourceId",
                new[] { new JTokenExpression(fullyQualifiedType), }.Concat(nameSegments));

        public static LanguageExpression GenerateResourceGroupResourceId(string fullyQualifiedType, IEnumerable<LanguageExpression> nameSegments)
            => CreateFunction(
                "resourceId",
                new[] { new JTokenExpression(fullyQualifiedType), }.Concat(nameSegments));

        public LanguageExpression GenerateManagementGroupResourceId(SyntaxBase managementGroupNameProperty, bool fullyQualified)
        {
            const string managementGroupType = "Microsoft.Management/managementGroups";
            var managementGroupName = ConvertExpression(managementGroupNameProperty);

            if (fullyQualified)
            {
                return GenerateTenantResourceId(managementGroupType, new[] { managementGroupName });
            }
            else
            {
                return GenerateUnqualifiedResourceId(managementGroupType, new[] { managementGroupName });
            }
        }

        /// <summary>
        /// Generates a management group id, using the managementGroup() function. Only suitable for use if the template being generated is targeting the management group scope.
        /// </summary>
        public static LanguageExpression GenerateCurrentManagementGroupId()
            => AppendProperties(
                CreateFunction("managementGroup"),
                new JTokenExpression("id"));

        private static FunctionExpression CreateFunction(string name, params LanguageExpression[] parameters)
            => CreateFunction(name, parameters as IEnumerable<LanguageExpression>);

        private static FunctionExpression CreateFunction(string name, IEnumerable<LanguageExpression> parameters)
            => new(name, parameters.ToArray(), Array.Empty<LanguageExpression>());

        public static FunctionExpression AppendProperties(FunctionExpression function, params LanguageExpression[] properties)
            => AppendProperties(function, properties as IEnumerable<LanguageExpression>);

        public static FunctionExpression AppendProperties(FunctionExpression function, IEnumerable<LanguageExpression> properties)
            => new(function.Function, function.Parameters, function.Properties.Concat(properties).ToArray());
    }
}


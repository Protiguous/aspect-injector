using FluentIL.Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;

using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using System.Runtime.InteropServices;

namespace AspectInjector.Analyzer
{
    public static class Extensions
    {
        private static readonly ConcurrentDictionary<Rule, DiagnosticDescriptor> DescriptorCache = new();

        public static TNode RemoveTokenKeepTrivia<TNode>(this TNode node, SyntaxToken token)
            where TNode : SyntaxNode
        {
            var next = token.GetNextToken();

            var newnode = node.ReplaceTokens([token, next], (o, r) => o == token ? SyntaxFactory.Token(SyntaxKind.None) : next.WithLeadingTrivia(token.LeadingTrivia));

            return newnode;
        }


        public static DiagnosticDescriptor AsDescriptor(this Rule rule)
        {
			//TODO streamline this extension

            //ref var valOrNew = System.Runtime.InteropServices.Marshal.
            // Only available in net8.0 ?!?!?!
            //ref var value = ref CollectionsMarshal.GetValueRefOrAddDefault( dictionary, key, out var existed );
            //DescriptorCache.Tr

            if (!DescriptorCache.TryGetValue(rule, out var descriptor))
                DescriptorCache[rule] = descriptor = new DiagnosticDescriptor(rule.Id, rule.Title, rule.Message, "Aspects", (DiagnosticSeverity)rule.Severity, true, rule.Description, rule.HelpLinkUri);

            return descriptor;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ExamplesFx
{
    public class ExampleParser
    {
        public string CreateFromFile(string fileContent)
        {
            var tree = CSharpSyntaxTree.ParseText(fileContent);

            var comments = new CommentWalker();
            comments.Visit(tree.GetRoot());

            var nodes = tree.GetRoot().DescendantNodes(descendIntoTrivia: true);

            foreach (var syntaxNode in nodes)
            {
                
            }
            return "";
            //var res = new ExampleCode(null, )
        }

    }

    class CommentWalker : CSharpSyntaxWalker
    {
        public readonly List<string> htmls = new List<string>();
        public override void Visit(SyntaxNode node)
        {
            //var content = trivia.Token.ToString();

            //if (content.StartsWith("HTML->", StringComparison.OrdinalIgnoreCase))
            //    htmls.Add(content.Substring(6));

            base.Visit(node);
        }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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

            //var comments = new CommentWalker();
            //comments.Visit(tree.GetRoot());

            var nodes = tree.GetRoot().DescendantNodes(descendIntoTrivia: true);

            var baseClassIndicator = nodes.First(x => x.IsKind(SyntaxKind.SimpleBaseType));
            var classDeclarator = (ClassDeclarationSyntax) baseClassIndicator.Parent.Parent;

            var res = new List<string>();
            foreach (var declaration in classDeclarator.DescendantNodes(descendIntoTrivia: true))
            {
                if (declaration.HasLeadingTrivia)
                {
                    foreach (var trivia in declaration.GetLeadingTrivia())
                        ProcessTrivia(trivia, res);
                }

                if (declaration.Kind() == SyntaxKind.MethodDeclaration)
                {
                    ProcessMethod(declaration, res);
                }

                if (declaration.HasTrailingTrivia)
                {
                    foreach (var trivia in declaration.GetTrailingTrivia())
                        ProcessTrivia(trivia, res);
                }

            }

            var page = String.Join(Environment.NewLine, res);

            return page;

        }

        private static void ProcessMethod(SyntaxNode declaration, List<string> res)
        {
            var method = (MethodDeclarationSyntax) declaration;

            if (method.Identifier.Text.Equals("Run", StringComparison.OrdinalIgnoreCase))
            {
                res.Add("{% highlight csharp %}");
                var body = method.Body.Statements.ToFullString();
                var lines = Regex.Split(body, "\r\n|\r|\n");
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Trim().Length == 0)
                        lines[i] = "";
                    else
                        lines[i] = lines[i].Replace("\t", "    ");
                }

                var firstChar = lines.Min(x => FirstChar(x));
                if (firstChar < int.MaxValue)
                {
                    for (int i = 0; i < lines.Length; i++)
                    {
                        if (lines[i].Trim().Length == 0)
                            lines[i] = "";
                        else
                            lines[i] = "    " + lines[i].Substring(firstChar);
                    }
                }

                res.AddRange(lines);
                res.Add("{% endhighlight %}");
            }
        }

        private static int FirstChar(string line)
        {
            for (int i = 0; i < line.Length; i++)
            {
                if (char.IsWhiteSpace(line[i]))
                    continue;

                return i;
            }
            return Int32.MaxValue;
        }

        private static void ProcessTrivia(SyntaxTrivia trivia, List<string> res)
        {
            if (trivia.IsKind(SyntaxKind.SingleLineCommentTrivia))
            {
                var comment = trivia.ToString().Trim().Substring(2).Trim();
                if (comment.StartsWith("->", StringComparison.Ordinal))
                    res.Add("<p>" +  comment.Substring(2).Trim() + "</p>");
                else if (comment.StartsWith("Html ->", StringComparison.OrdinalIgnoreCase))
                    res.Add("<p>" + comment.Substring(6).Trim() + "</p>");
                else if (comment.StartsWith("Html->", StringComparison.OrdinalIgnoreCase))
                    res.Add("<p>" + comment.Substring(6).Trim() + "</p>");

                res.Add("");
            }
        }
    }
    
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using NUnit.Framework;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ExamplesFx.Controls;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace ExamplesFx.Tests
{
    [TestFixture, RequiresSTA]
    public class ParseTest
    {
        [TestCase]
        public void SimpleFile()
        {
            var parser = new ExampleParser();
            var html =
                parser.CreateFromFile(File.ReadAllText(@"..\..\ExamplesFx.Demo.WinForms\Examples\10.Basics\1.Demo.cs"));
            Assert.IsNotNull(html);

        }

        [TestCase]
        public void GenerateFileHelpersDoc()
        {
            var workspace2 = MSBuildWorkspace.Create();

            var result = workspace2.OpenSolutionAsync(@"d:\Desarrollo\Devoo\GitHub\FileHelpers\FileHelpers.sln");
            var projects = result.Result;

            var project =
                projects.Projects.First(x => x.Name.Equals("FileHelpers.Examples", StringComparison.OrdinalIgnoreCase));

            var examples = project.Documents.Where(x => x.Folders.Count > 0 && x.Folders[0] == "Examples").ToList();

            var res = new List<ExampleCode>();
            foreach (var doc in examples)
            {
                if (doc.Name.IndexOf("ExamplesGenerator", StringComparison.OrdinalIgnoreCase) >= 0)
                    continue;

                var extraPath = string.Join("_", doc.Folders.Skip(1).Select(x => RemoveOrder(x)));

                var ex = CreateExample(doc);
                res.Add(ex);

                var examplePath = Path.Combine(@"d:\Desarrollo\Devoo\GitHub\FileHelpersHome",
                    ex.Url.Substring(1) + ".html");

                var html = new StringBuilder(@"---
layout: default
title: " + ex.Name + @"
permalink: " + ex.Url + @"/
---
<h4>" + ex.Category + ": " + ex.Name + @"</h4>
<h5>" + ex.Description + @"</h5>
");


                foreach (var part in ex.Parts)
                {
                    html.AppendLine();
                    html.AppendLine(part.Render());
                    html.AppendLine();
                }

                if (!Directory.Exists(Path.GetDirectoryName(examplePath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(examplePath));

                File.WriteAllText(examplePath, html.ToString());
            }

            CreateIndex(res);

            var frm = new Form();

            var container = new ExamplesContainerWithHeader();
            container.Dock = DockStyle.Fill;
            frm.Controls.Add(container);

            frm.Height = 800;
            frm.Width = 1024;
            container.LoadExamples(res);

            //frm.ShowDialog();
        }

        private ExampleCode CreateExample(Document doc)
        {
            var category = string.Join(@"/", doc.Folders.Skip(1).Select(x => RemoveOrder(x)));
            var exampleFileName = Path.GetFileNameWithoutExtension(RemoveOrder(doc.Name));
            var exampleName = exampleFileName;
            var fileText = doc.GetTextAsync().Result;
            var fileContent = fileText.ToString();


            var tree = doc.GetSyntaxTreeAsync().Result;
            var res = new ExampleCode(null, exampleName, category, doc.FilePath);

            ProcessAllTrivias(res, tree, fileText);

            var regexOptions = RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace;


            res.Url = "/example/" + (res.Category.Length > 0 ? res.Category.Replace(" ", "_") + "/" : "") +
                      RemoveOrder(Path.GetFileNameWithoutExtension(doc.Name)).Replace(" ", "_");

            var mainExample = new ExampleFile("");
            res.Files.Add(mainExample);

            mainExample.Contents = fileContent;

            return res;


        }

        private int? mStartFileNode = null;
        private ExampleFile mStartFile = null;
        private HashSet<SyntaxTrivia> mVisited;
        private void ProcessAllTrivias(ExampleCode res, SyntaxTree tree, SourceText fileText)
        {
            mStartFileNode = null;
            mStartFile = null;
            mVisited = new HashSet<SyntaxTrivia>();

            var allNodes = tree.GetRoot().DescendantNodes(descendIntoTrivia: true);
            foreach (var node in allNodes)
            {
                if (node.HasLeadingTrivia)
                {
                    ProcessTrivias(res, tree, fileText, node.GetLeadingTrivia());
                }

                //if (node.HasTrailingTrivia)
                //{
                //    ProcessTrivias(res, tree, fileText, node.GetTrailingTrivia());
                //}
            }

        }


        private void ProcessTrivias(ExampleCode ex, SyntaxTree tree, SourceText fileText, SyntaxTriviaList triviaList)
        {
            foreach (var trivia in triviaList)
            {
                if (mVisited.Contains(trivia))
                    continue;
                mVisited.Add(trivia);

                if (trivia.IsKind(SyntaxKind.SingleLineCommentTrivia))
                {
                    var triviaText = trivia.ToFullString();
                    var matchCmd = Regex.Match(triviaText, @"\/\/\-\>\s*(?<command>\w+)\s*\:(?<body>.+)");
                    if (matchCmd.Success)
                    {
                        var command = matchCmd.Groups["command"].Value.Trim().ToLower();
                        var body = matchCmd.Groups["body"].Value.Trim();

                        ProcessCommand(ex, command, body, fileText, trivia);
                    }
                    else
                    {
                        matchCmd = Regex.Match(triviaText, @"\/\/\-\>\s*(?<command>\/?\w+)\s*\:?(?<body>.+)?");
                        if (matchCmd.Success)
                        {
                            var command = matchCmd.Groups["command"].Value.Trim().ToLower();
                            var body = matchCmd.Groups["body"].Value.Trim();

                            ProcessCommand(ex, command, body, fileText, trivia);
                        }
                        else
                        {
                            // Check for  //->

                            matchCmd = Regex.Match(triviaText, @"\/\/\-\>\s*(?<body>.+)");
                            if (matchCmd.Success)
                            {
                                var body = matchCmd.Groups["body"].Value.Trim().ToLower();
                                // Html
                                ex.Parts.Add(new HtmlPart("<p>" + body + "</p>"));
                            }
                        }
                    }
                }

                if (trivia.IsKind(SyntaxKind.MultiLineCommentTrivia))
                {
                    var triviaText = trivia.ToFullString();
                    var matchCmd = Regex.Match(triviaText, @"\/\*-\>\s*(?<command>\w+)\s*\:(?<body>.+)\*\/", RegexOptions.Multiline);
                    if (matchCmd.Success)
                    {
                        var command = matchCmd.Groups["command"].Value.Trim().ToLower();
                        var body = matchCmd.Groups["body"].Value.Trim();

                        ProcessCommand(ex, command, body, fileText, trivia);
                    }
                    else
                    {
                        matchCmd = Regex.Match(triviaText, @"\/\/\-\>\s*(?<command>\/?\w+)\s*\:?(?<body>.+)?");
                        if (matchCmd.Success)
                        {
                            var command = matchCmd.Groups["command"].Value.Trim().ToLower();
                            var body = matchCmd.Groups["body"].Value.Trim();

                            ProcessCommand(ex, command, body, fileText, trivia);
                        }
                        else
                        {
                            // Check for  //->

                            matchCmd = Regex.Match(triviaText, @"\/\/\-\>\s*(?<body>.+)");
                            if (matchCmd.Success)
                            {
                                var body = matchCmd.Groups["body"].Value.Trim().ToLower();
                                // Html
                                ex.Parts.Add(new HtmlPart("<p>" + body + "</p>"));
                            }
                        }
                    }
                }

            }

        }

        private void ProcessCommand(ExampleCode ex, string command, string body, SourceText fileText, SyntaxTrivia trivia)
        {
            switch (command)
            {
                case "name":
                    ex.Name = body;
                    break;

                case "description":
                    ex.Description = body;
                    break;

                case "runnable":
                    ex.Runnable = body.Equals("yes", StringComparison.OrdinalIgnoreCase) ||
                                  body.Equals("true", StringComparison.OrdinalIgnoreCase) ||
                                  body.Equals("1", StringComparison.OrdinalIgnoreCase);
                    break;

                case "autorun":
                    ex.AutoRun = body.Equals("yes", StringComparison.OrdinalIgnoreCase) ||
                                 body.Equals("true", StringComparison.OrdinalIgnoreCase) ||
                                 body.Equals("1", StringComparison.OrdinalIgnoreCase);
                    break;

                case "html":
                    ex.Parts.Add(new HtmlPart("<p>" + body + "</p>"));
                    break;

                case "file":
                    mStartFile = new ExampleFile(body);
                    mStartFileNode = trivia.Span.End;
                    ex.Parts.Add(mStartFile);
                    break;

                case "/file":
                    if (mStartFileNode == null || trivia.SpanStart < mStartFileNode.Value)
                        break;

                    mStartFile.Contents = fileText.GetSubText(new TextSpan(mStartFileNode.Value, trivia.SpanStart - mStartFileNode.Value)).ToString();
                    // Grab The File
                    //mStartFileNode = trivia.;
                    //ex.Parts.Add(mStartFile);
                    break;

                default:
                    Debugger.Break();
                    break;
            }
        }
    

        private void CreateIndex(List<ExampleCode> res)
        {
            var indexPath = @"d:\Desarrollo\Devoo\GitHub\FileHelpersHome\examples.html";
            //examplePath = Path.ChangeExtension(examplePath, "html");

            var fileContent = new StringBuilder();
            fileContent.AppendLine(@"---
layout: default
title: Library Examples
permalink: /examples/
---");
            foreach (var category in res.GroupBy(x => x.Category))
            {
                fileContent.AppendLine("<h5>" + category.Key + "</h5>");
                fileContent.AppendLine("<div class='indent'><ul class='collection'>");

                foreach (var example in category)
                {
                    var exampleUrl = example.Url;

                    fileContent.AppendLine(
                        "<li class='collection-item' style='cursor: pointer;' onclick=\"location.href = '" +
                        exampleUrl + "'; \"><a href='" + exampleUrl + "' >" + example.Name + "</a><br/>" +
                        "<span class='example-description'>" + example.Description + "</span></li>");
                }

                fileContent.AppendLine("</ul></div>");
            }

            File.WriteAllText(indexPath, fileContent.ToString());


        }

        private string RemoveOrder(string s)
        {
            var parts = s.IndexOf(".", StringComparison.Ordinal);
            if (parts < 0)
                return s;

            var number = s.Substring(0, parts);
            return s.Substring(parts + 1);
        }
    }

}

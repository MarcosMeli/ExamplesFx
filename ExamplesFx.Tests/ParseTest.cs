﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using NUnit.Framework;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using ExamplesFx.Controls;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace ExamplesFx.Tests
{
    [TestFixture, RequiresSTA]
    public class ParseTest
    {
      

        [TestCase]
        public void GenerateFileHelpersDoc()
        {
            var workspace2 = MSBuildWorkspace.Create();

            var result = workspace2.OpenSolutionAsync(@"d:\Desarrollo\Devoo\GitHub\FileHelpers\FileHelpers.sln");
            var projects = result.Result;

            var project =
                projects.Projects.First(x => x.Name.Equals("FileHelpers.Examples", StringComparison.OrdinalIgnoreCase));

            var examples = project.Documents.Where(x => x.Folders.Count > 0 && x.Folders[0] == "Examples").ToList();
            var compilation = project.GetCompilationAsync().Result;

            var exePath = Path.Combine(Directory.GetCurrentDirectory(), "Examples.exe");
            var pdbPath = Path.Combine(Directory.GetCurrentDirectory(), "Examples.pdb");

            using (FileStream dllStream = new FileStream(exePath, FileMode.OpenOrCreate))
            using (FileStream pdbStream = new FileStream(pdbPath, FileMode.OpenOrCreate))
            {
                var emit  = compilation.Emit(dllStream, pdbStream);
            }

            var assembly = Assembly.LoadFile(exePath);

            var res = new List<ExampleCode>();
            foreach (var doc in examples)
            {
                if (doc.Name.IndexOf("ExamplesGenerator", StringComparison.OrdinalIgnoreCase) >= 0)
                    continue;

                var extraPath = string.Join("_", doc.Folders.Skip(1).Select(x => RemoveOrder(x)));

                var ex = CreateExample(doc, assembly);
                res.Add(ex);

                var examplePath = Path.Combine(@"d:\Desarrollo\Devoo\GitHub\FileHelpersHome",
                    ex.Url.Substring(1).Replace("/", "\\") + ".html");

                var html = new StringBuilder(@"---
layout: default
title: " + ex.Name + @"
permalink: " + ex.Url + @"/
---

<div class='card-panel waves-effect waves-dark yellow darken-4' style=' width: 100%; padding: 10px;' >
    <span class='white-text'>Work in progress, these are autogenerated examples from code</span>
</div>

<h4>" + ex.Category + ": " + ex.Name + @"</h4>
<div class='indent'>
  <h5>" + ex.Description + @"</h5>
  <div class='indent'>
");


                foreach (var part in ex.Parts)
                {
                    html.AppendLine();
                    html.AppendLine(part.Render());
                    html.AppendLine();
                }

                if (!string.IsNullOrWhiteSpace(ex.ConsoleOutput) && ex.Parts.FirstOrDefault(x => x is ConsolePart) == null)
                {
                    var consolePart = new ConsolePart();
                    consolePart.ConsoleOutput = ex.ConsoleOutput;

                    html.AppendLine();
                    html.AppendLine(consolePart.Render());
                    html.AppendLine();

                }

                html.AppendLine("  </div>");
                html.AppendLine("</div>");
                if (!Directory.Exists(Path.GetDirectoryName(examplePath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(examplePath));

                File.WriteAllText(examplePath, html.ToString());
                //return;
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

        private ExampleCode CreateExample(Document doc, Assembly assembly)
        {
            var category = string.Join(@"/", doc.Folders.Skip(1).Select(x => RemoveOrder(x)));
            var exampleFileName = Path.GetFileNameWithoutExtension(RemoveOrder(doc.Name));
            var exampleName = exampleFileName;
            var fileText = doc.GetTextAsync().Result;
            var fileContent = fileText.ToString();


            var tree = doc.GetSyntaxTreeAsync().Result;
            var res = new ExampleCode(null, exampleName, category, doc.FilePath);
            res.AutoRun = true;

            var namespaceDeclaration = (NamespaceDeclarationSyntax) tree.GetRoot().DescendantNodes().First(x => x.IsKind(SyntaxKind.NamespaceDeclaration));
            var node = (ClassDeclarationSyntax) tree.GetRoot().DescendantNodes().First(x => x.IsKind(SyntaxKind.ClassDeclaration));

            var exampleType = assembly.GetType(namespaceDeclaration.Name.ToString() + "." + node.Identifier.Text);

            res.Example = (ExampleBase) Activator.CreateInstance(exampleType);

            ProcessAllTrivias(res, tree, fileText);

            var regexOptions = RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace;


            res.Url = "/example/" + (res.Category.Length > 0 ? res.Category.Replace(" ", "") + "/" : "") +
                      RemoveOrder(Path.GetFileNameWithoutExtension(doc.Name)).Replace(" ", "");


            if (res.Runnable && res.AutoRun)
                res.RunExample();

            return res;
        }

        class TriviaWalker : SyntaxWalker
        {
            public TriviaWalker()
                :base(depth: SyntaxWalkerDepth.Token)
            {

            }
            public ExampleCode Example { get; internal set; }
            public SourceText FileText { get; internal set; }
            public ParseTest Parent { get; internal set; }

            public override void Visit(SyntaxNode node)
            {
                if (node.HasLeadingTrivia)
                {
                    Parent.ProcessTrivias(Example, FileText, node.GetLeadingTrivia());
                }


                base.Visit(node);
            }

            protected override void VisitToken(SyntaxToken token)
            {
                //if (token.IsKind(SyntaxKind.CloseBraceToken))
                //    Debugger.Break();

                if (token.HasLeadingTrivia)
                {
            
                    Parent.ProcessTrivias(Example, FileText, token.LeadingTrivia);
                }


                base.VisitToken(token);
            }
           
        }

        private int? mStartFileNode = null;
        private ExampleFile mStartFile = null;
        private HashSet<SyntaxTrivia> mVisited;
        private void ProcessAllTrivias(ExampleCode res, SyntaxTree tree, SourceText fileText)
        {
            mStartFileNode = null;
            mStartFile = null;
            mVisited = new HashSet<SyntaxTrivia>();


            var writer = new TriviaWalker();
            writer.Example = res;
            writer.FileText = fileText;
            writer.Parent = this;
            writer.Visit(tree.GetRoot()); 

        }


        private void ProcessTrivias(ExampleCode ex, SourceText fileText, SyntaxTriviaList triviaList)
        {
            foreach (var trivia in triviaList)
            {
                if (mVisited.Contains(trivia))
                    continue;
                mVisited.Add(trivia);

                if (trivia.IsKind(SyntaxKind.EndOfLineTrivia) || trivia.IsKind(SyntaxKind.WhitespaceTrivia))
                    continue;

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
                        matchCmd = Regex.Match(triviaText, @"\/\/\-\>\s*(?<command>\/\w+)\s*\:?(?<body>.+)?");
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
                                var body = matchCmd.Groups["body"].Value.Trim();

                                if (body.ToLower() == "console")
                                {
                                    ex.Parts.Add(new ConsolePart());
                                }
                                else
                                {
                                    // Html
                                    if (body.IndexOf("<p", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                        body.IndexOf("<div", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                        body.IndexOf("<span", StringComparison.OrdinalIgnoreCase) >= 0
                                        )
                                        ex.Parts.Add(new HtmlPart(body));
                                    else
                                        ex.Parts.Add(new HtmlPart("<p>" + body + "</p>"));
                                }
                            }
                        }
                    }
                }

                if (trivia.IsKind(SyntaxKind.MultiLineCommentTrivia))
                {
                    var triviaText = trivia.ToFullString();
                    var matchCmd = Regex.Match(triviaText, @"\s*\/\*-\>\s*(?<command>\w+)\s*\:(?<body>.+)\*\/\s*", RegexOptions.Singleline);
                    if (matchCmd.Success)
                    {
                        var command = matchCmd.Groups["command"].Value.Trim().ToLower();
                        var body = matchCmd.Groups["body"].Value.Trim();

                        ProcessCommand(ex, command, body, fileText, trivia);
                    }
                    else
                    {
                        matchCmd = Regex.Match(triviaText, @"\s*\/\*-\>\s*(?<command>\/\w+)\s*\:?(?<body>.+)?\*\/\s*", RegexOptions.Singleline);
                        if (matchCmd.Success)
                        {
                            var command = matchCmd.Groups["command"].Value.Trim().ToLower();
                            var body = matchCmd.Groups["body"].Value.Trim();

                            ProcessCommand(ex, command, body, fileText, trivia);
                        }
                        else
                        {
                            // Check for  //->

                            matchCmd = Regex.Match(triviaText, @"\s*\/\*-\>\s*(?<body>.+)\*\/\s*", RegexOptions.Singleline);
                            if (matchCmd.Success)
                            {
                                var body = matchCmd.Groups["body"].Value.Trim();
                                // Html
                                if (body.IndexOf("<p", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                    body.IndexOf("<div", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                    body.IndexOf("<span", StringComparison.OrdinalIgnoreCase) >= 0
                                    )
                                    ex.Parts.Add(new HtmlPart(body));
                                else
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
                    if (body.IndexOf("<p", StringComparison.OrdinalIgnoreCase) >= 0 ||
    body.IndexOf("<div", StringComparison.OrdinalIgnoreCase) >= 0 ||
    body.IndexOf("<span", StringComparison.OrdinalIgnoreCase) >= 0
    )
                        ex.Parts.Add(new HtmlPart(body));
                    else
                        ex.Parts.Add(new HtmlPart("<p>" + body + "</p>"));
                    break;

                case "file":
                case "filein":
                    mStartFile = new ExampleFile(body);
                    if (command == "filein" || body.Trim().ToLower() == "input.txt")
                        mStartFile.Status = ExampleFile.FileType.InputFile;
                    if (body.Trim().ToLower() == "output.txt")
                        mStartFile.Status = ExampleFile.FileType.OutputFile;
                    mStartFileNode = trivia.Span.End;
                    ex.Parts.Add(mStartFile);
                    ex.Files.Add(mStartFile);
                    break;

                case "/file":
                case "/filein":
                    if (mStartFileNode == null)
                        break;

                    mStartFile.Contents = fileText.GetSubText(new TextSpan(mStartFileNode.Value, trivia.SpanStart - mStartFileNode.Value)).ToString();
                    var contentsTrimmed = mStartFile.Contents.Trim();
                    if (contentsTrimmed.StartsWith("/*") && contentsTrimmed.EndsWith("*/"))
                    {
                        mStartFile.Contents = contentsTrimmed.Substring(2, contentsTrimmed.Length - 4);
                    }

                    // Grab The File
                    //mStartFileNode = trivia.;
                    //ex.Parts.Add(mStartFile);
                        break;

                case "fileout":
                    var fileOut = new ExampleFile(body) { Status = ExampleFile.FileType.OutputFile };

                    ex.Parts.Add(fileOut);
                    ex.Files.Add(fileOut);
                    break;

                case "/fileout":
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
---

<div class='card-panel waves-effect waves-dark yellow darken-4' style=' width: 100%; padding: 10px;' >
    <span class='white-text'>Work in progress, these are autogenerated examples from code</span>
</div>
<div class='row'>
");

            var row1 = new StringBuilder();
            var row2 = new StringBuilder();

            var currentRow = row2;

            foreach (var category in res.GroupBy(x => x.Category))
            {
                if (currentRow == row1)
                    currentRow = row2;
                else
                    currentRow = row1;

                currentRow.AppendLine("<h5>" + category.Key + "</h5>");
                currentRow.AppendLine("<div class='indent'><ul class='collection examples'>");

                foreach (var example in category)
                {
                    var exampleUrl = example.Url;

                    currentRow.AppendLine(
                        "<li class='collection-item' style='cursor: pointer;' onclick=\"location.href = '" +
                        exampleUrl + "'; \"><a href='" + exampleUrl + "' >" + example.Name + "</a><br/>" +
                        "<span class='example-description'>" + example.Description + "</span></li>");
                }

                currentRow.AppendLine("</ul></div>");
            }

            fileContent.AppendLine("<div class='col s12 m6 l6'>" + row1+"</div>");
            fileContent.AppendLine("<div class='col s12 m6 l6'>" + row2 + "</div>");

            currentRow.AppendLine("</div>");
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

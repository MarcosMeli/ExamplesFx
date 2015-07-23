using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using NUnit.Framework;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ExamplesFx.Controls;
using System.Text.RegularExpressions;

namespace ExamplesFx.Tests
{
    [TestFixture, RequiresSTA]
    public class ParseTest
    {
        [TestCase]
        public void SimpleFile()
        {
            var parser = new ExampleParser();
            var html = parser.CreateFromFile(File.ReadAllText(@"..\..\ExamplesFx.Demo.WinForms\Examples\10.Basics\1.Demo.cs"));
            Assert.IsNotNull(html);
            
        }

        [TestCase]
        public void GenerateFileHelpersDoc()
        {
            var workspace2 = MSBuildWorkspace.Create();

            var result = workspace2.OpenSolutionAsync(@"d:\Desarrollo\Devoo\GitHub\FileHelpers\FileHelpers.sln");
            var projects = result.Result;

            var project = projects.Projects.First(x => x.Name.Equals("FileHelpers.Examples", StringComparison.OrdinalIgnoreCase));

            var examples = project.Documents.Where(x => x.Folders.Count > 0 && x.Folders[0] == "Examples").ToList();

            var res = new List<ExampleCode>();
            foreach (var doc in examples)
            {
                if (doc.Name.IndexOf("ExamplesGenerator", StringComparison.OrdinalIgnoreCase) >= 0)
                    continue;

                var extraPath = string.Join("_", doc.Folders.Skip(1).Select(x => RemoveOrder(x)));

                var ex = CreateExample(doc); 
                res.Add(ex);

                var examplePath = Path.Combine(@"d:\Desarrollo\Devoo\GitHub\FileHelpersHome", ex.Url.Substring(1) + ".html");
                
                var html = @"---
layout: default
title: "+ ex.Name + @"
permalink: "+ ex.Url + @"/
---
{% highlight csharp %}
" + ex.Files[0].Contents +
@"
{%  endhighlight %}
";

                if (!Directory.Exists(Path.GetDirectoryName(examplePath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(examplePath));

                File.WriteAllText(examplePath, html);
            }

            CreateIndex(res);

            var frm = new Form();

            var container = new ExamplesContainerWithHeader();
            container.Dock = DockStyle.Fill;
            frm.Controls.Add(container);

            frm.Height = 800;
            frm.Width = 1024;
            container.LoadExamples(res);

           frm.ShowDialog();
        }

        private ExampleCode CreateExample(Document doc)
        {
            var category = string.Join(@"/", doc.Folders.Skip(1).Select(x => RemoveOrder(x)));
            var exampleFileName = Path.GetFileNameWithoutExtension(RemoveOrder(doc.Name));
            var exampleName = exampleFileName;
            var exampleDescription = "";
            var fileContent = doc.GetTextAsync().Result.ToString();


            var tree = doc.GetSyntaxTreeAsync().Result;

            var comments = tree.GetRoot().DescendantTrivia(n => true, true);
            var regexOptions = RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace;
            var match = Regex.Match(fileContent, @"\/\/\-\>\s*(Example\.)?Name\s*:(?<name>.*)", regexOptions);

            if (match.Success)
                exampleName = match.Groups["name"].Value.Trim();

            match = Regex.Match(fileContent, @"\/\/\-\>\s*(Example\.)?Description\s*:(?<description>.*)", regexOptions | RegexOptions.Multiline);

            if (match.Success)
                exampleDescription = match.Groups["description"].Value.Trim();

        
            var res = new ExampleCode(null, exampleName, category, doc.FilePath);
            res.Url = "/example/" + (res.Category.Length > 0 ? res.Category.Replace(" ", "_") + "/" : "") + RemoveOrder(Path.GetFileNameWithoutExtension(doc.Name)).Replace(" ", "_");

            match = Regex.Match(fileContent, @"\/\/\-\>\s*(Example\.)?Runnable\s*:(?<runnable>.*)", regexOptions | RegexOptions.Multiline);
            if (match.Success)
            {
                var runnable = match.Groups["runnable"].Value.Trim();
                res.Runnable = runnable.Equals("yes", StringComparison.OrdinalIgnoreCase) ||
                                runnable.Equals("true", StringComparison.OrdinalIgnoreCase) ||
                                runnable.Equals("1", StringComparison.OrdinalIgnoreCase);

            }

            match = Regex.Match(fileContent, @"\/\/\-\>\s*(Example\.)?AutoRun\s*:(?<autorun>.*)", regexOptions | RegexOptions.Multiline);
            if (match.Success)
            {
                var autorun = match.Groups["runnable"].Value.Trim();
                res.AutoRun = autorun.Equals("yes", StringComparison.OrdinalIgnoreCase) ||
                                autorun.Equals("true", StringComparison.OrdinalIgnoreCase) ||
                                autorun.Equals("1", StringComparison.OrdinalIgnoreCase);

            }


            res.Description = exampleDescription;
            var mainExample = new ExampleFile("");
            res.Files.Add(mainExample);

            mainExample.Contents = fileContent;

            return res;


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

                    fileContent.AppendLine("<li class='collection-item' style='cursor: pointer;' onclick=\"location.href = '"+
                        exampleUrl +"'; \"><a href='" +exampleUrl+"' >" + example.Name+"</a><br/>" +
                        "<span class='example-description'>"+ example.Description+"</span></li>");
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

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


                var examplePath = Path.Combine(@"d:\Desarrollo\Devoo\GitHub\FileHelpersHome\examples", (string.IsNullOrEmpty(extraPath) ? "" : extraPath+ "_") + ex.Name) + ".html";
                
                var html = @"---
layout: default
title: "+ ex.Name + @"
permalink: /example/"+ (ex.Category.Length > 0 ? ex.Category + "/" : "") + ex.Name + @"/
---
" + ex.Files[0].Contents ;

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

           // frm.ShowDialog();
        }

        private ExampleCode CreateExample(Document doc)
        {
            var category = string.Join(@"/", doc.Folders.Skip(1).Select(x => RemoveOrder(x)));
            var exampleName = Path.GetFileNameWithoutExtension(RemoveOrder(doc.Name));
            var fileContent = doc.GetTextAsync().Result.ToString();

            var res = new ExampleCode(null, exampleName, category, doc.FilePath);

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
                fileContent.AppendLine("<h4>" + category.Key + "</h4>");
                fileContent.AppendLine("<div class='indent'><ul>");

                foreach (var example in category)
                {
                    fileContent.AppendLine("<li><a href='/example/"+ (example.Category.Length > 0 ? example.Category + "/" : "") + example.Name + "/' >" + example.Name+"</a></li>");
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

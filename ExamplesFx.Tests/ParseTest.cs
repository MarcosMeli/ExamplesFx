using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using NUnit.Framework;
using System.Linq;
using System.Text;

namespace ExamplesFx.Tests
{
    [TestFixture]
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
                var category = string.Join(@"\", doc.Folders.Skip(1).Select(x => RemoveOrder(x)));

                var ex = new ExampleCode(null, RemoveOrder(doc.Name), category, doc.FilePath);
                res.Add(ex);

                var fileContent = doc.GetTextAsync().Result.ToString();

                var mainExample = new ExampleFile("");
                ex.Files.Add(mainExample);
                mainExample.Contents = fileContent;

                var examplePath = Path.Combine(@"d:\Desarrollo\Devoo\GitHub\FileHelpersPage\examples", (string.IsNullOrEmpty(extraPath) ? "" : extraPath+ "_") + RemoveOrder(doc.Name));
                examplePath = Path.ChangeExtension(examplePath, "html");

                File.WriteAllText(examplePath, fileContent);
            }
            
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

    public class ExampleRenderer
    {
    }
}

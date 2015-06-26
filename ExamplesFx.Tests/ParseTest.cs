using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit;
using NUnit.Framework;
using NFluent.Extensions;
namespace ExamplesFx.Tests
{
    [TestFixture]
    public class ParseTest
    {
        [TestCase()]
        public void SimpleFile()
        {
            var parser = new ExampleParser();
            var html = parser.CreateFromFile(File.ReadAllText(@"..\..\ExamplesFx.Demo.WinForms\Examples\10.Basics\1.Demo.cs"));
            Assert.IsNotNull(html);
        }
    }
}

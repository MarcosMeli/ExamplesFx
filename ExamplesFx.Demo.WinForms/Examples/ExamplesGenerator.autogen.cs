

using System;
using System.Collections.Generic;
using System.Text;
using ExamplesFx;


namespace ExamplesFx.Demo
{
    public class ExamplesFactory
    {
	    static ExampleFile file;

        public static List<ExampleCode> GetExamples()
        {
		    var examples = new List<ExampleCode>();
            ExampleCode example;
example = new ExampleCode(new ProgramDemo(), "Example", "Basics", @"D:\Desarrollo\Devoo\ExamplesFx\ExamplesFx.Demo.WinForms\Examples\10.Basics\1.Demo.cs");
examples.Add(example);

		
           return examples;
        }
    }
}



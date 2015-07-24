using System;
using System.Collections;
using System.Collections.Generic;

namespace ExamplesFx
{
    public class ConsolePart : IExamplePart
    {
        public ConsolePart()
        {
            //Html = html;
        }

        public string ConsoleOutput { get; set; }
        public string Render()
        {
            return ExampleFile.RenderFile("Console", ConsoleOutput, ExampleFile.FileType.Console);
        }
    }
}
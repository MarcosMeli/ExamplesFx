using System;
using System.Collections.Generic;
using System.IO;

namespace ExamplesFx
{
    public class ExampleFile: 
        IExamplePart
    {
        public ExampleFile(string filename)
            : this(filename, NetLanguage.CSharp) {}

        public ExampleFile(string filename, NetLanguage language)
        {
            Filename = filename;
            Language = language;
        }


        public string Filename { get; set; }
        public string Contents { get; set; }

        public enum FileType
        {
            SourceFile,
            InputFile,
            OutputFile,
            Console
        }

        /// <summary>
        /// Type of file to list
        /// </summary>
        public FileType Status = FileType.SourceFile;

        public NetLanguage Language { get; set; }
        public string Render()
        {
            return RenderFile(Filename, Contents, Status);
        }

        public static string RenderFile(string fileName, string contents, FileType type)
        {
            if (type == FileType.Console)
            {
                return @"<div class='highlight-title example-console'> Console </div>
<div class='example-console-high'>
<div class='highlight'>
<pre>
" + Normalize(contents) +
@"
</pre>
</div>
</div>
";
            }

            var classStatus = "";
            switch (type)
            {
                case FileType.Console:
                    classStatus = "example-console";
                    break;
                case FileType.SourceFile:
                    classStatus = "example-code";
                    break;
                case FileType.InputFile:
                    classStatus = "example-input";
                    break;
                case FileType.OutputFile:
                    classStatus = "example-output";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            var html = "";
            if (!string.IsNullOrEmpty(fileName))
                html += "<div class='highlight-title " + classStatus + "'> " + fileName + " </div>";
            html += @"<div class='"+classStatus+@"-high'>
{% highlight csharp %}
" + Normalize(contents) +
                    @"
{%  endhighlight %}
</div>
";
            return html;
        }

        private static string Normalize(string contents)
        {
            var lines = new List<string>();

            using (var reader = new StringReader(contents))
            {
                string currentLine;
                while ((currentLine = reader.ReadLine()) != null)
                    lines.Add(currentLine);
            }


            var minIndex = int.MaxValue;
            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i];
                for (int j = 0; j < line.Length; j++)
                {
                    if (char.IsWhiteSpace(line[j]))
                        continue;

                    minIndex = Math.Min(minIndex, j);
                }

            }

            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].Length >= minIndex)
                    lines[i] = lines[i].Substring(minIndex);
            }
            
            return string.Join(Environment.NewLine, lines);


        }
    }
}
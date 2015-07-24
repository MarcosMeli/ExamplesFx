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
            HtmlFile
        }

        /// <summary>
        /// Type of file to list
        /// </summary>
        public FileType Status = FileType.SourceFile;

        public NetLanguage Language { get; set; }
        public string Render()
        {
            var html = "";
            if (!string.IsNullOrEmpty(Filename))
                html += "<p>" + Filename + "</p>";
            html += @"
{% highlight csharp %}
" + Contents +
                    @"
{%  endhighlight %}
";
            return html;
        }
    }
}
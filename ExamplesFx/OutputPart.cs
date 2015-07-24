using System;
using System.Collections;
using System.Collections.Generic;

namespace ExamplesFx
{
    public class OutputPart : IExamplePart
    {
        private readonly string mFileName;

        public OutputPart(string fileName)
        {
            mFileName = fileName;
        }

        public string Render()
        {
            var html = "";
            html += "<div class='highlight-title'>Output: " + mFileName + "</div>";
            html += @"
{% highlight csharp %}
 .
{%  endhighlight %}
";
            return html;
        }
    }
}
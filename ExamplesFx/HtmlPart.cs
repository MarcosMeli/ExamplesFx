namespace ExamplesFx
{
    public class HtmlPart : IExamplePart
    {
        public HtmlPart(string html)
        {
            Html = html;
        }

        public string Html { get; private set; }
        public string Render()
        {
            return Html;
        }
    }
}
using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ExamplesFx.TreeView
{
    /// <summary>
    /// Create a demo code container
    /// </summary>
    public class ExampleTreeNode
        : TreeNode, IHtmlWriter, ISearchableNode
    {
        /// <summary>
        /// Create a demo tree node with text based on Name
        /// </summary>
        /// <param name="example">DemoCode value to base node upon</param>
        public ExampleTreeNode(ExampleCode example)
            : base(example.Name)
        {
            Example = example;
            SelectedImageKey = "folder";
            ImageKey = "folder";
        }

        public override object Clone()
        {
            return new ExampleTreeNode(Example);
        }

        /// <summary>
        /// demo to create detail from
        /// </summary>
        public ExampleCode Example { get; set; }


        /// <summary>
        /// Output HTML,  in this case a heading
        /// </summary>
        /// <param name="index"></param>
        public void OutputHtml(StringBuilder index, int indent)
        {
            bool error = false;
            Exception MyException = null;
            try {
                Example.RunExample();
            }
            catch (Exception ex) {
                error = true;
                MyException = ex;
            }
            bool found = false;
            
            if (!found) {
                index.Append("<dt><u>Missing</u> ");
                index.Append(Example.Name);
                index.AppendLine("</dt>");
                index.Append("<dd>");
                index.Append(Example.Description);
                index.AppendLine("</dd>");
            }
        }

        string ISearchableNode.GetName()
        {
            return Example.Name;
        }

        string ISearchableNode.GetDescription()
        {
            return Example.Description;
        }

        string ISearchableNode.GetDescriptionExtra()
        {
            var sb = new StringBuilder();
            sb.AppendLine(Example.SourceCode);
            foreach (var file in Example.Files)
                sb.AppendLine(file.Contents);
            return sb.ToString();
        }
    }
}
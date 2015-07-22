using System.Text;
using System.Windows.Forms;

namespace ExamplesFx.TreeView
{
    /// <summary>
    /// Container for category as a tree node leaf
    /// </summary>
    public class CategoryTreeNode
        : TreeNode, IHtmlWriter
    {
        public CategoryTreeNode()
        {
            SelectedImageKey = "demo";
            ImageKey = "demo";
        }

        public override object Clone()
        {
            return new CategoryTreeNode(Text);
        }

        /// <summary>
        /// Create a category tree node using text description
        /// </summary>
        /// <param name="text"></param>
        public CategoryTreeNode(string text)
            : base(text)
        {
            SelectedImageKey = "demo";
            ImageKey = "demo";
        }


        /// <summary>
        /// Output HTML, in this case a heading
        /// </summary>
        /// <param name="index"></param>
        public void OutputHtml(StringBuilder index, int indent)
        {
            index.Append("<h3>");
            index.Append(Text);
            index.AppendLine("</h3>");

            if (Nodes.Count > 0) {
                int newIndent = indent + 1;
                index.AppendLine("<blockquote><dl>");
                foreach (var node in Nodes) {
                    if (node is IHtmlWriter)
                        ((IHtmlWriter) node).OutputHtml(index, newIndent);
                }
                index.AppendLine("</dl></blockquote>");
            }
        }
    }
}
using System.Windows.Controls;

namespace ExamplesFx.Controls
{
    /// <summary>
    /// Interaction logic for TextEditorWpf.xaml
    /// </summary>
    public partial class TextEditorWpf : UserControl
    {
        public TextEditorWpf()
        {
            InitializeComponent();
        }

        public string Text
        {
            get
            {
                return txtText.Text; 
            }
            set
            {
                if (txtText.Text == value)
                    return;

                txtText.Text = value;
                txtText.IsReadOnly = true;
                //var doc2 = factory.CreateDocument();
                //doc2.HighlightingStrategy = HighlightingStrategyFactory.CreateHighlightingStrategy("C#");
                //doc2.TextContent = value;
                //doc2.ReadOnly = true;
                //txtCode.Document = doc2;
                //txtCode.Refresh();
            }
        }
    }
}

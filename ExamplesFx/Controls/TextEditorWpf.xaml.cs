using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

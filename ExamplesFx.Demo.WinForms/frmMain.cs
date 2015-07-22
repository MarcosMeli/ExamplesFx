using System.Windows.Forms;

namespace ExamplesFx.Demo.WinForms
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();

            exampleContainer.LoadExamples(ExamplesFactory.GetExamples());
        }
    }
}

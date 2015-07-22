﻿using System;
using System.Windows.Forms;

namespace ExamplesFx.Controls
{
    public partial class ExampleRenderer : UserControl
    {
        public ExampleRenderer()
        {
            InitializeComponent();
            DoubleBuffered = true;
        }

        private ExampleCode mExample;
        public ExampleCode Example
        {
            get { return mExample; }
            set
            {
                if (mExample == value)
                    return;

                mExample = value;
                RenderExample();
            }
        }

        private void RenderExample()
        {
            SuspendLayout();
            
            Clear();

            lblTestDescription.Text = Example.Description;
            //lblDescription.Text = Example.Description;
            cmdRunDemo.Visible = Example.Runnable;

            splitFiles.Panel2Collapsed = true;
            tableLayoutPanel1.RowCount = Example.Files.Count;
            for (int i = 0; i < Example.Files.Count; i++)
            {
                var file = Example.Files[i];
                CreateNewDemoFile(i, file);
            }

            ResumeLayout();
        }


        public void Clear()
        {
            lblTestDescription.Text = string.Empty;
            cmdRunDemo.Visible = false;
            tableLayoutPanel1.Controls.Clear();
        }

        private void cmdRunDemo_Click(object sender, EventArgs e)
        {
            if (Example == null)
                return;
            Example.AddedFile += FileHandler;
            Example.RunExample();
            Example.AddedFile -= FileHandler;

        }

        private void FileHandler(object sender, ExampleCode.NewFileEventArgs e)
        {
            if (e.File.Status == ExampleFile.FileType.OutputFile)
            {
                fileOutput.File = e.File;
                splitFiles.Panel2Collapsed = false;
            }
            else
                CreateNewDemoFile(int.MaxValue, e.File);
        }

        private void CreateNewDemoFile(int i, ExampleFile file)
        {
            var ctrl = new FileRenderer(file);
            ctrl.Dock = DockStyle.Fill;
            if (i > tableLayoutPanel1.RowCount)
            {
                tableLayoutPanel1.RowCount++;
                tableLayoutPanel1.Controls.Add(ctrl, 0, tableLayoutPanel1.RowCount - 1);
            }
            else
            {
                tableLayoutPanel1.Controls.Add(ctrl, 0, i);
                
            }


        }

    }
}

namespace ExamplesFx.Demo.WinForms
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.examplesContainerWithHeader1 = new ExamplesFx.Controls.ExamplesContainerWithHeader();
            this.SuspendLayout();
            // 
            // examplesContainerWithHeader1
            // 
            this.examplesContainerWithHeader1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.examplesContainerWithHeader1.Location = new System.Drawing.Point(0, 0);
            this.examplesContainerWithHeader1.Name = "examplesContainerWithHeader1";
            this.examplesContainerWithHeader1.Size = new System.Drawing.Size(944, 730);
            this.examplesContainerWithHeader1.TabIndex = 0;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(944, 730);
            this.Controls.Add(this.examplesContainerWithHeader1);
            this.Name = "frmMain";
            this.Text = "Examples Demo";
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.ExamplesContainerWithHeader examplesContainerWithHeader1;
    }
}


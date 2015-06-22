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
            this.exampleContainer = new ExamplesFx.Controls.ExamplesContainerWithHeader();
            this.SuspendLayout();
            // 
            // exampleContainer
            // 
            this.exampleContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.exampleContainer.Location = new System.Drawing.Point(0, 0);
            this.exampleContainer.Name = "exampleContainer";
            this.exampleContainer.Size = new System.Drawing.Size(944, 730);
            this.exampleContainer.TabIndex = 0;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(944, 730);
            this.Controls.Add(this.exampleContainer);
            this.Name = "frmMain";
            this.Text = "Examples Demo";
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.ExamplesContainerWithHeader exampleContainer;
    }
}


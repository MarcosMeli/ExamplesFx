namespace ExamplesFx.Controls
{
    partial class ExamplesContainerWithHeader
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExamplesContainerWithHeader));
            this.reflectionHeader1 = new Devoo.WinForms.ReflectionHeader();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Location = new System.Drawing.Point(0, 85);
            this.splitContainer1.Size = new System.Drawing.Size(1052, 668);
            // 
            // reflectionHeader1
            // 
            this.reflectionHeader1.BandDown.Height = 18;
            this.reflectionHeader1.BandUp.Height = 0;
            this.reflectionHeader1.Dock = System.Windows.Forms.DockStyle.Top;
            this.reflectionHeader1.GradientBack.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(1)))), ((int)(((byte)(74)))));
            this.reflectionHeader1.GradientBack.Color2 = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(0)))), ((int)(((byte)(107)))));
            this.reflectionHeader1.GradientBack.Direction = Devoo.WinForms.GradientDirection.Vertical;
            this.reflectionHeader1.Header.Font = new System.Drawing.Font("Trebuchet MS", 27.75F, System.Drawing.FontStyle.Bold);
            this.reflectionHeader1.Header.Position = new System.Drawing.Point(80, 17);
            this.reflectionHeader1.Header.ReflectionOpacity = ((byte)(200));
            this.reflectionHeader1.Header.Text = "Library Examples";
            this.reflectionHeader1.Images.AddRange(new Devoo.WinForms.ImageShape[] {
            new Devoo.WinForms.ImageShape(((System.Drawing.Bitmap)(resources.GetObject("reflectionHeader1.Images"))), true, ((byte)(255)), new System.Drawing.Point(3, 1), ((byte)(0)), 0)});
            this.reflectionHeader1.Location = new System.Drawing.Point(0, 0);
            this.reflectionHeader1.Name = "reflectionHeader1";
            this.reflectionHeader1.Size = new System.Drawing.Size(1052, 85);
            this.reflectionHeader1.Text = "Library Examples";
            // 
            // ExampleContainerWithHeader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.reflectionHeader1);
            this.Name = "ExampleContainerWithHeader";
            this.Size = new System.Drawing.Size(1052, 753);
            this.Controls.SetChildIndex(this.reflectionHeader1, 0);
            this.Controls.SetChildIndex(this.splitContainer1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Devoo.WinForms.ReflectionHeader reflectionHeader1;
    }
}

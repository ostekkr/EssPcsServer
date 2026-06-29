namespace EssPcsServer
{
    partial class frmPcsControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPcsControl));
            this.btnOK = new C1.Win.Input.C1Button();
            this.c1CommandHolder1 = new C1.Win.Command.C1CommandHolder();
            this.aboutThemeController = new C1.Win.Themes.C1ThemeController();
            this.labelPcsControl3 = new C1.Win.Input.C1Label();
            this.labelPcsControl2 = new C1.Win.Input.C1Label();
            this.labelPcsControl1 = new C1.Win.Input.C1Label();
            this.numericPcsControl1 = new System.Windows.Forms.NumericUpDown();
            this.numericPcsControl2 = new System.Windows.Forms.NumericUpDown();
            this.numericPcsControl3 = new System.Windows.Forms.NumericUpDown();
            this.btnPcsControl1 = new C1.Win.Input.C1Button();
            this.btnPcsControl2 = new C1.Win.Input.C1Button();
            this.btnPcsControl3 = new C1.Win.Input.C1Button();
            ((System.ComponentModel.ISupportInitialize)(this.btnOK)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1CommandHolder1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.aboutThemeController)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.labelPcsControl3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.labelPcsControl2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.labelPcsControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericPcsControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericPcsControl2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericPcsControl3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnPcsControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnPcsControl2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnPcsControl3)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Image = global::EssPcsServer.Properties.Resources.check_24x24;
            this.btnOK.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOK.Location = new System.Drawing.Point(297, 90);
            this.btnOK.Name = "btnOK";
            this.btnOK.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.btnOK.Size = new System.Drawing.Size(75, 35);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "확인";
            this.btnOK.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.aboutThemeController.SetTheme(this.btnOK, "(default)");
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // c1CommandHolder1
            // 
            this.c1CommandHolder1.Owner = this;
            // 
            // aboutThemeController
            // 
            this.aboutThemeController.Theme = "VS2013Light";
            // 
            // labelPcsControl3
            // 
            this.labelPcsControl3.AutoSize = true;
            this.labelPcsControl3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.labelPcsControl3.Location = new System.Drawing.Point(12, 79);
            this.labelPcsControl3.Name = "labelPcsControl3";
            this.labelPcsControl3.Size = new System.Drawing.Size(102, 12);
            this.labelPcsControl3.TabIndex = 8;
            this.labelPcsControl3.Tag = null;
            this.labelPcsControl3.Text = "고정식 PCS [kW]";
            this.aboutThemeController.SetTheme(this.labelPcsControl3, "(default)");
            // 
            // labelPcsControl2
            // 
            this.labelPcsControl2.AutoSize = true;
            this.labelPcsControl2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.labelPcsControl2.Location = new System.Drawing.Point(12, 47);
            this.labelPcsControl2.Name = "labelPcsControl2";
            this.labelPcsControl2.Size = new System.Drawing.Size(102, 12);
            this.labelPcsControl2.TabIndex = 7;
            this.labelPcsControl2.Tag = null;
            this.labelPcsControl2.Text = "단축식 PCS [kW]";
            this.aboutThemeController.SetTheme(this.labelPcsControl2, "(default)");
            // 
            // labelPcsControl1
            // 
            this.labelPcsControl1.AutoSize = true;
            this.labelPcsControl1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.labelPcsControl1.Location = new System.Drawing.Point(12, 15);
            this.labelPcsControl1.Name = "labelPcsControl1";
            this.labelPcsControl1.Size = new System.Drawing.Size(102, 12);
            this.labelPcsControl1.TabIndex = 6;
            this.labelPcsControl1.Tag = null;
            this.labelPcsControl1.Text = "양축식 PCS [kW]";
            this.aboutThemeController.SetTheme(this.labelPcsControl1, "(default)");
            // 
            // numericPcsControl1
            // 
            this.numericPcsControl1.ForeColor = System.Drawing.Color.Black;
            this.numericPcsControl1.Location = new System.Drawing.Point(115, 11);
            this.numericPcsControl1.Name = "numericPcsControl1";
            this.numericPcsControl1.Size = new System.Drawing.Size(75, 21);
            this.numericPcsControl1.TabIndex = 9;
            this.numericPcsControl1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.aboutThemeController.SetTheme(this.numericPcsControl1, "(default)");
            // 
            // numericPcsControl2
            // 
            this.numericPcsControl2.ForeColor = System.Drawing.Color.Black;
            this.numericPcsControl2.Location = new System.Drawing.Point(115, 43);
            this.numericPcsControl2.Name = "numericPcsControl2";
            this.numericPcsControl2.Size = new System.Drawing.Size(75, 21);
            this.numericPcsControl2.TabIndex = 10;
            this.numericPcsControl2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.aboutThemeController.SetTheme(this.numericPcsControl2, "(default)");
            // 
            // numericPcsControl3
            // 
            this.numericPcsControl3.ForeColor = System.Drawing.Color.Black;
            this.numericPcsControl3.Location = new System.Drawing.Point(115, 75);
            this.numericPcsControl3.Name = "numericPcsControl3";
            this.numericPcsControl3.Size = new System.Drawing.Size(75, 21);
            this.numericPcsControl3.TabIndex = 11;
            this.numericPcsControl3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.aboutThemeController.SetTheme(this.numericPcsControl3, "(default)");
            // 
            // btnPcsControl1
            // 
            this.btnPcsControl1.AutoSize = true;
            this.btnPcsControl1.Image = global::EssPcsServer.Properties.Resources.button_rounded_blue_next;
            this.btnPcsControl1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPcsControl1.Location = new System.Drawing.Point(195, 9);
            this.btnPcsControl1.Name = "btnPcsControl1";
            this.btnPcsControl1.Size = new System.Drawing.Size(26, 26);
            this.btnPcsControl1.TabIndex = 12;
            this.btnPcsControl1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.aboutThemeController.SetTheme(this.btnPcsControl1, "(default)");
            // 
            // btnPcsControl2
            // 
            this.btnPcsControl2.AutoSize = true;
            this.btnPcsControl2.Image = global::EssPcsServer.Properties.Resources.button_rounded_blue_next;
            this.btnPcsControl2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPcsControl2.Location = new System.Drawing.Point(195, 41);
            this.btnPcsControl2.Name = "btnPcsControl2";
            this.btnPcsControl2.Size = new System.Drawing.Size(26, 26);
            this.btnPcsControl2.TabIndex = 13;
            this.btnPcsControl2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.aboutThemeController.SetTheme(this.btnPcsControl2, "(default)");
            // 
            // btnPcsControl3
            // 
            this.btnPcsControl3.AutoSize = true;
            this.btnPcsControl3.Image = global::EssPcsServer.Properties.Resources.button_rounded_blue_next;
            this.btnPcsControl3.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPcsControl3.Location = new System.Drawing.Point(195, 73);
            this.btnPcsControl3.Name = "btnPcsControl3";
            this.btnPcsControl3.Size = new System.Drawing.Size(26, 26);
            this.btnPcsControl3.TabIndex = 14;
            this.btnPcsControl3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.aboutThemeController.SetTheme(this.btnPcsControl3, "(default)");
            // 
            // frmPcsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 137);
            this.Controls.Add(this.btnPcsControl3);
            this.Controls.Add(this.btnPcsControl2);
            this.Controls.Add(this.btnPcsControl1);
            this.Controls.Add(this.numericPcsControl3);
            this.Controls.Add(this.numericPcsControl2);
            this.Controls.Add(this.numericPcsControl1);
            this.Controls.Add(this.labelPcsControl3);
            this.Controls.Add(this.labelPcsControl2);
            this.Controls.Add(this.labelPcsControl1);
            this.Controls.Add(this.btnOK);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmPcsControl";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PCS 출력 제어";
            this.aboutThemeController.SetTheme(this, "(default)");
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.btnOK)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1CommandHolder1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.aboutThemeController)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.labelPcsControl3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.labelPcsControl2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.labelPcsControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericPcsControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericPcsControl2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericPcsControl3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnPcsControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnPcsControl2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnPcsControl3)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private C1.Win.Input.C1Button btnOK;
        private C1.Win.Command.C1CommandHolder c1CommandHolder1;
        private C1.Win.Themes.C1ThemeController aboutThemeController;
        private C1.Win.Input.C1Label labelPcsControl3;
        private C1.Win.Input.C1Label labelPcsControl2;
        private C1.Win.Input.C1Label labelPcsControl1;
        private System.Windows.Forms.NumericUpDown numericPcsControl3;
        private System.Windows.Forms.NumericUpDown numericPcsControl2;
        private System.Windows.Forms.NumericUpDown numericPcsControl1;
        private C1.Win.Input.C1Button btnPcsControl1;
        private C1.Win.Input.C1Button btnPcsControl3;
        private C1.Win.Input.C1Button btnPcsControl2;
    }
}
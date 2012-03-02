namespace XboxDvdMenu
{
    partial class Form1
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
            System.Windows.Forms.Label label1;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.cmbDrive = new System.Windows.Forms.ComboBox();
            this.btScan = new System.Windows.Forms.Button();
            this.btDVDStyle = new System.Windows.Forms.Button();
            this.btCopy = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbTheme = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.Log = new System.Windows.Forms.TextBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.chkArtwork = new System.Windows.Forms.CheckBox();
            this.chkTraillers = new System.Windows.Forms.CheckBox();
            label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(112)))), ((int)(((byte)(164)))), ((int)(((byte)(28)))));
            label1.Location = new System.Drawing.Point(190, 160);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(58, 24);
            label1.TabIndex = 5;
            label1.Text = "Drive:";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.panel1.Location = new System.Drawing.Point(-5, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(971, 113);
            this.panel1.TabIndex = 1;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.panel2.Location = new System.Drawing.Point(-3, 194);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(102, 450);
            this.panel2.TabIndex = 2;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(-2, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(440, 194);
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // cmbDrive
            // 
            this.cmbDrive.FormattingEnabled = true;
            this.cmbDrive.Location = new System.Drawing.Point(271, 165);
            this.cmbDrive.Name = "cmbDrive";
            this.cmbDrive.Size = new System.Drawing.Size(121, 21);
            this.cmbDrive.TabIndex = 6;
            // 
            // btScan
            // 
            this.btScan.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btScan.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btScan.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(112)))), ((int)(((byte)(164)))), ((int)(((byte)(28)))));
            this.btScan.Location = new System.Drawing.Point(271, 250);
            this.btScan.Name = "btScan";
            this.btScan.Size = new System.Drawing.Size(156, 23);
            this.btScan.TabIndex = 7;
            this.btScan.Text = "1. Scan ISOS";
            this.btScan.UseVisualStyleBackColor = false;
            this.btScan.Click += new System.EventHandler(this.Button1Click);
            // 
            // btDVDStyle
            // 
            this.btDVDStyle.Enabled = false;
            this.btDVDStyle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btDVDStyle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(112)))), ((int)(((byte)(164)))), ((int)(((byte)(28)))));
            this.btDVDStyle.Location = new System.Drawing.Point(271, 280);
            this.btDVDStyle.Name = "btDVDStyle";
            this.btDVDStyle.Size = new System.Drawing.Size(156, 23);
            this.btDVDStyle.TabIndex = 8;
            this.btDVDStyle.Text = "2. Start DVD Styler";
            this.btDVDStyle.UseVisualStyleBackColor = false;
            this.btDVDStyle.Click += new System.EventHandler(this.Button2Click);
            // 
            // btCopy
            // 
            this.btCopy.Enabled = false;
            this.btCopy.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btCopy.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(112)))), ((int)(((byte)(164)))), ((int)(((byte)(28)))));
            this.btCopy.Location = new System.Drawing.Point(271, 309);
            this.btCopy.Name = "btCopy";
            this.btCopy.Size = new System.Drawing.Size(156, 23);
            this.btCopy.TabIndex = 9;
            this.btCopy.Text = "3. Copy to drive";
            this.btCopy.UseVisualStyleBackColor = false;
            this.btCopy.Click += new System.EventHandler(this.Button3Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(593, 119);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(21, 23);
            this.button4.TabIndex = 10;
            this.button4.Text = "X";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.Button4Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.Color.White;
            this.pictureBox2.Location = new System.Drawing.Point(558, 305);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(65, 44);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 11;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Click += new System.EventHandler(this.pictureBox2_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.Green;
            this.label2.Location = new System.Drawing.Point(556, 286);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Powered By:";
            // 
            // cmbTheme
            // 
            this.cmbTheme.FormattingEnabled = true;
            this.cmbTheme.Location = new System.Drawing.Point(271, 200);
            this.cmbTheme.Name = "cmbTheme";
            this.cmbTheme.Size = new System.Drawing.Size(343, 21);
            this.cmbTheme.TabIndex = 14;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(112)))), ((int)(((byte)(164)))), ((int)(((byte)(28)))));
            this.label3.Location = new System.Drawing.Point(190, 200);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(76, 24);
            this.label3.TabIndex = 15;
            this.label3.Text = "Theme:";
            // 
            // Log
            // 
            this.Log.Location = new System.Drawing.Point(105, 384);
            this.Log.Multiline = true;
            this.Log.Name = "Log";
            this.Log.ReadOnly = true;
            this.Log.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.Log.Size = new System.Drawing.Size(518, 145);
            this.Log.TabIndex = 16;
            this.Log.TextChanged += new System.EventHandler(this.Log_TextChanged);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(105, 355);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(518, 23);
            this.progressBar1.TabIndex = 17;
            // 
            // chkArtwork
            // 
            this.chkArtwork.AutoSize = true;
            this.chkArtwork.Checked = true;
            this.chkArtwork.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkArtwork.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(112)))), ((int)(((byte)(164)))), ((int)(((byte)(28)))));
            this.chkArtwork.Location = new System.Drawing.Point(194, 227);
            this.chkArtwork.Name = "chkArtwork";
            this.chkArtwork.Size = new System.Drawing.Size(185, 17);
            this.chkArtwork.TabIndex = 18;
            this.chkArtwork.Text = "Download artwork from Xbox.com";
            this.chkArtwork.UseVisualStyleBackColor = true;
            // 
            // chkTraillers
            // 
            this.chkTraillers.AutoSize = true;
            this.chkTraillers.Checked = true;
            this.chkTraillers.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTraillers.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(112)))), ((int)(((byte)(164)))), ((int)(((byte)(28)))));
            this.chkTraillers.Location = new System.Drawing.Point(385, 227);
            this.chkTraillers.Name = "chkTraillers";
            this.chkTraillers.Size = new System.Drawing.Size(182, 17);
            this.chkTraillers.TabIndex = 19;
            this.chkTraillers.Text = "Download traillers from Xbox.com";
            this.chkTraillers.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(626, 541);
            this.Controls.Add(this.chkTraillers);
            this.Controls.Add(this.chkArtwork);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.Log);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cmbTheme);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.btCopy);
            this.Controls.Add(this.btDVDStyle);
            this.Controls.Add(this.btScan);
            this.Controls.Add(this.cmbDrive);
            this.Controls.Add(label1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Form1";
            this.Text = "Dvd Menu";
            this.TransparencyKey = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.Load += new System.EventHandler(this.Form1Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ComboBox cmbDrive;
        private System.Windows.Forms.Button btScan;
        private System.Windows.Forms.Button btDVDStyle;
        private System.Windows.Forms.Button btCopy;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbTheme;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox Log;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.CheckBox chkArtwork;
        private System.Windows.Forms.CheckBox chkTraillers;
    }
}


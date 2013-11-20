namespace ChartConverter
{
    partial class WordPlayTest
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
            this.txtOutput = new System.Windows.Forms.TextBox();
            this.btnGo = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.lbl1 = new System.Windows.Forms.Label();
            this.lblStartDate = new System.Windows.Forms.Label();
            this.lbl2 = new System.Windows.Forms.Label();
            this.lblFinishDate = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblCurrentFile = new System.Windows.Forms.Label();
            this.lblStatusCount = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnDocToTif = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtOutput
            // 
            this.txtOutput.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.txtOutput.Location = new System.Drawing.Point(12, 191);
            this.txtOutput.Multiline = true;
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.ReadOnly = true;
            this.txtOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtOutput.Size = new System.Drawing.Size(408, 152);
            this.txtOutput.TabIndex = 0;
            this.txtOutput.WordWrap = false;
            // 
            // btnGo
            // 
            this.btnGo.Location = new System.Drawing.Point(244, 44);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(176, 23);
            this.btnGo.TabIndex = 1;
            this.btnGo.Text = "Process All Charts";
            this.btnGo.UseVisualStyleBackColor = true;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(244, 12);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(176, 23);
            this.btnExport.TabIndex = 2;
            this.btnExport.Text = "Export all charts to text files";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // lbl1
            // 
            this.lbl1.AutoSize = true;
            this.lbl1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl1.Location = new System.Drawing.Point(12, 20);
            this.lbl1.Name = "lbl1";
            this.lbl1.Size = new System.Drawing.Size(71, 15);
            this.lbl1.TabIndex = 3;
            this.lbl1.Text = "Start Date";
            // 
            // lblStartDate
            // 
            this.lblStartDate.AutoSize = true;
            this.lblStartDate.Location = new System.Drawing.Point(12, 44);
            this.lblStartDate.Name = "lblStartDate";
            this.lblStartDate.Size = new System.Drawing.Size(0, 13);
            this.lblStartDate.TabIndex = 4;
            // 
            // lbl2
            // 
            this.lbl2.AutoSize = true;
            this.lbl2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl2.Location = new System.Drawing.Point(12, 136);
            this.lbl2.Name = "lbl2";
            this.lbl2.Size = new System.Drawing.Size(80, 15);
            this.lbl2.TabIndex = 5;
            this.lbl2.Text = "Finish Date";
            // 
            // lblFinishDate
            // 
            this.lblFinishDate.AutoSize = true;
            this.lblFinishDate.Location = new System.Drawing.Point(12, 160);
            this.lblFinishDate.Name = "lblFinishDate";
            this.lblFinishDate.Size = new System.Drawing.Size(0, 13);
            this.lblFinishDate.TabIndex = 6;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(12, 73);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(157, 15);
            this.label5.TabIndex = 7;
            this.label5.Text = "Current Processing File";
            // 
            // lblCurrentFile
            // 
            this.lblCurrentFile.AutoSize = true;
            this.lblCurrentFile.Location = new System.Drawing.Point(12, 102);
            this.lblCurrentFile.Name = "lblCurrentFile";
            this.lblCurrentFile.Size = new System.Drawing.Size(0, 13);
            this.lblCurrentFile.TabIndex = 8;
            // 
            // lblStatusCount
            // 
            this.lblStatusCount.AutoSize = true;
            this.lblStatusCount.Location = new System.Drawing.Point(307, 151);
            this.lblStatusCount.Name = "lblStatusCount";
            this.lblStatusCount.Size = new System.Drawing.Size(0, 13);
            this.lblStatusCount.TabIndex = 10;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(283, 114);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 15);
            this.label2.TabIndex = 9;
            this.label2.Text = "Status Count";
            // 
            // btnDocToTif
            // 
            this.btnDocToTif.Location = new System.Drawing.Point(244, 73);
            this.btnDocToTif.Name = "btnDocToTif";
            this.btnDocToTif.Size = new System.Drawing.Size(176, 23);
            this.btnDocToTif.TabIndex = 11;
            this.btnDocToTif.Text = "Convert DOC to TIF";
            this.btnDocToTif.UseVisualStyleBackColor = true;
            this.btnDocToTif.Click += new System.EventHandler(this.btnDocToTif_Click);
            // 
            // WordPlayTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(432, 355);
            this.Controls.Add(this.btnDocToTif);
            this.Controls.Add(this.lblStatusCount);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblCurrentFile);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.lblFinishDate);
            this.Controls.Add(this.lbl2);
            this.Controls.Add(this.lblStartDate);
            this.Controls.Add(this.lbl1);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.btnGo);
            this.Controls.Add(this.txtOutput);
            this.MaximizeBox = false;
            this.Name = "WordPlayTest";
            this.Text = "Patient Charts Importer";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.WordPlayTest_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtOutput;
        private System.Windows.Forms.Button btnGo;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Label lbl1;
        private System.Windows.Forms.Label lblStartDate;
        private System.Windows.Forms.Label lbl2;
        private System.Windows.Forms.Label lblFinishDate;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblCurrentFile;
        private System.Windows.Forms.Label lblStatusCount;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnDocToTif;
    }
}


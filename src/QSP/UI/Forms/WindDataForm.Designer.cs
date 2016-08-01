﻿namespace QSP.UI.Forms
{
    partial class WindDataForm
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
            this.statusLbl = new System.Windows.Forms.Label();
            this.downloadBtn = new System.Windows.Forms.Button();
            this.saveFileBtn = new System.Windows.Forms.Button();
            this.loadFileBtn = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.statusPicBox = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.statusPicBox)).BeginInit();
            this.SuspendLayout();
            // 
            // statusLbl
            // 
            this.statusLbl.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.statusLbl.AutoSize = true;
            this.statusLbl.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusLbl.Location = new System.Drawing.Point(23, 0);
            this.statusLbl.Name = "statusLbl";
            this.statusLbl.Size = new System.Drawing.Size(71, 23);
            this.statusLbl.TabIndex = 0;
            this.statusLbl.Text = "Status : ";
            // 
            // downlaodBtn
            // 
            this.downloadBtn.BackColor = System.Drawing.Color.DarkSlateGray;
            this.downloadBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.downloadBtn.Cursor = System.Windows.Forms.Cursors.Default;
            this.downloadBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.downloadBtn.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.downloadBtn.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.downloadBtn.Location = new System.Drawing.Point(0, 50);
            this.downloadBtn.Margin = new System.Windows.Forms.Padding(0);
            this.downloadBtn.Name = "downlaodBtn";
            this.downloadBtn.Size = new System.Drawing.Size(200, 32);
            this.downloadBtn.TabIndex = 26;
            this.downloadBtn.Text = "Download / Refresh";
            this.downloadBtn.UseVisualStyleBackColor = false;
            // 
            // saveFileBtn
            // 
            this.saveFileBtn.BackColor = System.Drawing.Color.DarkSlateGray;
            this.saveFileBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.saveFileBtn.Cursor = System.Windows.Forms.Cursors.Default;
            this.saveFileBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.saveFileBtn.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.saveFileBtn.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.saveFileBtn.Location = new System.Drawing.Point(0, 120);
            this.saveFileBtn.Margin = new System.Windows.Forms.Padding(0);
            this.saveFileBtn.Name = "saveFileBtn";
            this.saveFileBtn.Size = new System.Drawing.Size(200, 32);
            this.saveFileBtn.TabIndex = 27;
            this.saveFileBtn.Text = "Save wind data";
            this.saveFileBtn.UseVisualStyleBackColor = false;
            // 
            // loadFileBtn
            // 
            this.loadFileBtn.BackColor = System.Drawing.Color.DarkSlateGray;
            this.loadFileBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.loadFileBtn.Cursor = System.Windows.Forms.Cursors.Default;
            this.loadFileBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.loadFileBtn.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.loadFileBtn.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.loadFileBtn.Location = new System.Drawing.Point(0, 85);
            this.loadFileBtn.Margin = new System.Windows.Forms.Padding(0);
            this.loadFileBtn.Name = "loadFileBtn";
            this.loadFileBtn.Size = new System.Drawing.Size(200, 32);
            this.loadFileBtn.TabIndex = 28;
            this.loadFileBtn.Text = "Load wind data";
            this.loadFileBtn.UseVisualStyleBackColor = false;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.downloadBtn, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.loadFileBtn, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.saveFileBtn, 0, 4);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(50, 33);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 15F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(493, 160);
            this.tableLayoutPanel1.TabIndex = 29;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.statusPicBox, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.statusLbl, 1, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(97, 23);
            this.tableLayoutPanel2.TabIndex = 31;
            // 
            // statusPicBox
            // 
            this.statusPicBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.statusPicBox.BackgroundImage = global::QSP.Properties.Resources.GreenLight;
            this.statusPicBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.statusPicBox.Location = new System.Drawing.Point(0, 1);
            this.statusPicBox.Margin = new System.Windows.Forms.Padding(0);
            this.statusPicBox.Name = "statusPicBox";
            this.statusPicBox.Size = new System.Drawing.Size(20, 20);
            this.statusPicBox.TabIndex = 30;
            this.statusPicBox.TabStop = false;
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label1.Location = new System.Drawing.Point(3, 40);
            this.label1.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(452, 2);
            this.label1.TabIndex = 32;
            // 
            // WindDataForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(561, 212);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WindDataForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Wind Data ";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.statusPicBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label statusLbl;
        private System.Windows.Forms.Button downloadBtn;
        private System.Windows.Forms.Button saveFileBtn;
        private System.Windows.Forms.Button loadFileBtn;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.PictureBox statusPicBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label1;
    }
}
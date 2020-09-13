namespace Forex.Forms
{
    partial class ModelsForm
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
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.labelLog = new System.Windows.Forms.Label();
            this.labelLogDetails = new System.Windows.Forms.TextBox();
            this.buttonCreateModel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView
            // 
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Location = new System.Drawing.Point(12, 41);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.Size = new System.Drawing.Size(348, 317);
            this.dataGridView.TabIndex = 0;
            // 
            // labelLog
            // 
            this.labelLog.AutoSize = true;
            this.labelLog.Font = new System.Drawing.Font("Segoe UI Semilight", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelLog.Location = new System.Drawing.Point(531, 23);
            this.labelLog.Name = "labelLog";
            this.labelLog.Size = new System.Drawing.Size(27, 15);
            this.labelLog.TabIndex = 1;
            this.labelLog.Text = "Log";
            // 
            // labelLogDetails
            // 
            this.labelLogDetails.AcceptsReturn = true;
            this.labelLogDetails.AcceptsTab = true;
            this.labelLogDetails.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.labelLogDetails.Font = new System.Drawing.Font("Segoe UI Semilight", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelLogDetails.Location = new System.Drawing.Point(366, 41);
            this.labelLogDetails.Multiline = true;
            this.labelLogDetails.Name = "labelLogDetails";
            this.labelLogDetails.ReadOnly = true;
            this.labelLogDetails.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.labelLogDetails.Size = new System.Drawing.Size(358, 317);
            this.labelLogDetails.TabIndex = 2;
            // 
            // buttonCreateModel
            // 
            this.buttonCreateModel.Font = new System.Drawing.Font("Segoe UI Semilight", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonCreateModel.Location = new System.Drawing.Point(121, 12);
            this.buttonCreateModel.Name = "buttonCreateModel";
            this.buttonCreateModel.Size = new System.Drawing.Size(100, 23);
            this.buttonCreateModel.TabIndex = 3;
            this.buttonCreateModel.Text = "Create Models";
            this.buttonCreateModel.UseVisualStyleBackColor = true;
            this.buttonCreateModel.Click += new System.EventHandler(this.buttonCreateModel_Click);
            // 
            // ModelsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(736, 385);
            this.Controls.Add(this.buttonCreateModel);
            this.Controls.Add(this.labelLogDetails);
            this.Controls.Add(this.labelLog);
            this.Controls.Add(this.dataGridView);
            this.Name = "ModelsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Models";
            // this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ModelsForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.Label labelLog;
        private System.Windows.Forms.TextBox labelLogDetails;
        private System.Windows.Forms.Button buttonCreateModel;
    }
}
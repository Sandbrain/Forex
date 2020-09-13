namespace Forex.Forms
{
    partial class XMLMergeForm
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
            this.ccbModels = new Forex.CheckedComboBox();
            this.buttonMerge = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ccbModels
            // 
            this.ccbModels.CheckOnClick = true;
            this.ccbModels.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.ccbModels.DropDownHeight = 1;
            this.ccbModels.FormattingEnabled = true;
            this.ccbModels.IntegralHeight = false;
            this.ccbModels.Location = new System.Drawing.Point(62, 38);
            this.ccbModels.MaxDropDownItems = 20;
            this.ccbModels.Name = "ccbModels";
            this.ccbModels.Size = new System.Drawing.Size(459, 21);
            this.ccbModels.TabIndex = 0;
            this.ccbModels.ValueSeparator = ", ";
            // 
            // buttonMerge
            // 
            this.buttonMerge.Location = new System.Drawing.Point(237, 271);
            this.buttonMerge.Name = "buttonMerge";
            this.buttonMerge.Size = new System.Drawing.Size(101, 31);
            this.buttonMerge.TabIndex = 1;
            this.buttonMerge.Text = "Merge";
            this.buttonMerge.UseVisualStyleBackColor = true;
            this.buttonMerge.Click += new System.EventHandler(this.buttonMerge_Click);
            // 
            // XMLMergeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(587, 314);
            this.Controls.Add(this.buttonMerge);
            this.Controls.Add(this.ccbModels);
            this.Name = "XMLMergeForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "XMLMergeForm";
            this.ResumeLayout(false);

        }

        #endregion

        private CheckedComboBox ccbModels;
        private System.Windows.Forms.Button buttonMerge;
    }
}
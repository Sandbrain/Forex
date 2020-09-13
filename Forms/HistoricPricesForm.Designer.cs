namespace Forex.Forms
{
    partial class HistoricPricesForm
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
            this.btnGetHistoricPrices = new System.Windows.Forms.Button();
            this.monthCalendarStart = new System.Windows.Forms.MonthCalendar();
            this.monthCalendarEnd = new System.Windows.Forms.MonthCalendar();
            this.cbInstrument = new System.Windows.Forms.ComboBox();
            this.cbInterval = new System.Windows.Forms.ComboBox();
            this.labelInstrument = new System.Windows.Forms.Label();
            this.labelInterval = new System.Windows.Forms.Label();
            this.labelEndDate = new System.Windows.Forms.Label();
            this.labelStartDate = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnGetHistoricPrices
            // 
            this.btnGetHistoricPrices.Font = new System.Drawing.Font("Segoe UI Semilight", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGetHistoricPrices.Location = new System.Drawing.Point(261, 345);
            this.btnGetHistoricPrices.Name = "btnGetHistoricPrices";
            this.btnGetHistoricPrices.Size = new System.Drawing.Size(126, 27);
            this.btnGetHistoricPrices.TabIndex = 0;
            this.btnGetHistoricPrices.Text = "Go";
            this.btnGetHistoricPrices.UseVisualStyleBackColor = true;
            this.btnGetHistoricPrices.Click += new System.EventHandler(this.btnGetHistoricPrices_Click);
            // 
            // monthCalendarStart
            // 
            this.monthCalendarStart.Font = new System.Drawing.Font("Segoe UI Semilight", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.monthCalendarStart.Location = new System.Drawing.Point(18, 67);
            this.monthCalendarStart.Name = "monthCalendarStart";
            this.monthCalendarStart.TabIndex = 1;
            // 
            // monthCalendarEnd
            // 
            this.monthCalendarEnd.Font = new System.Drawing.Font("Segoe UI Semilight", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.monthCalendarEnd.Location = new System.Drawing.Point(397, 67);
            this.monthCalendarEnd.Name = "monthCalendarEnd";
            this.monthCalendarEnd.TabIndex = 2;
            // 
            // cbInstrument
            // 
            this.cbInstrument.Font = new System.Drawing.Font("Segoe UI Semilight", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbInstrument.FormattingEnabled = true;
            this.cbInstrument.Location = new System.Drawing.Point(86, 264);
            this.cbInstrument.Name = "cbInstrument";
            this.cbInstrument.Size = new System.Drawing.Size(159, 23);
            this.cbInstrument.TabIndex = 3;
            // 
            // cbInterval
            // 
            this.cbInterval.Font = new System.Drawing.Font("Segoe UI Semilight", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbInterval.FormattingEnabled = true;
            this.cbInterval.Items.AddRange(new object[] {
            "m1",
            "m5",
            "m10",
            "m15",
            "m30",
            "H1",
            "H2",
            "H4",
            "D1",
            "W1"});
            this.cbInterval.Location = new System.Drawing.Point(446, 264);
            this.cbInterval.Name = "cbInterval";
            this.cbInterval.Size = new System.Drawing.Size(178, 23);
            this.cbInterval.TabIndex = 4;
            // 
            // labelInstrument
            // 
            this.labelInstrument.AutoSize = true;
            this.labelInstrument.Font = new System.Drawing.Font("Segoe UI Semilight", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelInstrument.Location = new System.Drawing.Point(15, 267);
            this.labelInstrument.Name = "labelInstrument";
            this.labelInstrument.Size = new System.Drawing.Size(64, 15);
            this.labelInstrument.TabIndex = 5;
            this.labelInstrument.Text = "Instrument";
            // 
            // labelInterval
            // 
            this.labelInterval.AutoSize = true;
            this.labelInterval.Font = new System.Drawing.Font("Segoe UI Semilight", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelInterval.Location = new System.Drawing.Point(394, 267);
            this.labelInterval.Name = "labelInterval";
            this.labelInterval.Size = new System.Drawing.Size(46, 15);
            this.labelInterval.TabIndex = 6;
            this.labelInterval.Text = "Interval";
            // 
            // labelEndDate
            // 
            this.labelEndDate.AutoSize = true;
            this.labelEndDate.Font = new System.Drawing.Font("Segoe UI Semilight", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelEndDate.Location = new System.Drawing.Point(489, 45);
            this.labelEndDate.Name = "labelEndDate";
            this.labelEndDate.Size = new System.Drawing.Size(54, 15);
            this.labelEndDate.TabIndex = 7;
            this.labelEndDate.Text = "End Date";
            // 
            // labelStartDate
            // 
            this.labelStartDate.AutoSize = true;
            this.labelStartDate.Font = new System.Drawing.Font("Segoe UI Semilight", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelStartDate.Location = new System.Drawing.Point(105, 45);
            this.labelStartDate.Name = "labelStartDate";
            this.labelStartDate.Size = new System.Drawing.Size(58, 15);
            this.labelStartDate.TabIndex = 8;
            this.labelStartDate.Text = "Start Date";
            // 
            // HistoricPricesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(642, 395);
            this.Controls.Add(this.labelStartDate);
            this.Controls.Add(this.labelEndDate);
            this.Controls.Add(this.labelInterval);
            this.Controls.Add(this.labelInstrument);
            this.Controls.Add(this.cbInterval);
            this.Controls.Add(this.cbInstrument);
            this.Controls.Add(this.monthCalendarEnd);
            this.Controls.Add(this.monthCalendarStart);
            this.Controls.Add(this.btnGetHistoricPrices);
            this.Name = "HistoricPricesForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Historic Data";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnGetHistoricPrices;
        private System.Windows.Forms.MonthCalendar monthCalendarStart;
        private System.Windows.Forms.MonthCalendar monthCalendarEnd;
        private System.Windows.Forms.ComboBox cbInstrument;
        private System.Windows.Forms.ComboBox cbInterval;
        private System.Windows.Forms.Label labelInstrument;
        private System.Windows.Forms.Label labelInterval;
        private System.Windows.Forms.Label labelEndDate;
        private System.Windows.Forms.Label labelStartDate;
    }
}
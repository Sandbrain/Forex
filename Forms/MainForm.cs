using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using fxcore2;
using System.Threading;
using System.Reflection;

namespace Forex
{
    public partial class MainForm : Form
    {
        private string m_version = "TS v2.2Beta";

        public MainForm()
        {
            InitializeComponent();
            this.Text = m_version;
        }
        
        Session CurrentSession;
        string CurrentTable = "";

        public void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (CurrentSession != null)
            {
                CurrentSession.Logout();
            }
        }

        private void connectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurrentSession = new Session(this);
        }

        public void StatusLabel(int num)
        {
            switch (num)
            {
                case 1:
                    labelStatus.Text = "Table last updated at: " + DateTime.Now.ToString();
                    break;
                case 2:
                    labelStatus.Text = "Connection Established at: " + DateTime.Now.ToString();
                    break;
                case 3:
                    labelStatus.Text = "User cancelled connection.";
                    break;
            }
        }

        #region ToolStripMenus

        private void modelsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StatusLabel(1);
            //CurrentTable = "Models";
            CurrentSession.GetSessionTables(this, "Models");
        }

        private void accountsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StatusLabel(1);
            CurrentTable = "Accounts";
            CurrentSession.GetSessionTables(this, "Accounts");
        }

        private void closedTradesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StatusLabel(1);
            CurrentTable = "ClosedTrades";
            CurrentSession.GetSessionTables(this, "ClosedTrades");           
        }

        private void messagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StatusLabel(1);
            CurrentTable = "Messages";
            CurrentSession.GetSessionTables(this, "Messages");            
        }

        private void offersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StatusLabel(1);
            CurrentTable = "Offers";
            CurrentSession.GetSessionTables(this, "Offers");            
        }

        private void ordersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StatusLabel(1);
            CurrentTable = "Orders";
            CurrentSession.GetSessionTables(this, "Orders");            
        }

        private void summaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StatusLabel(1);
            CurrentTable = "Summary";
            CurrentSession.GetSessionTables(this, "Summary");            
        }

        private void tradesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StatusLabel(1);
            CurrentTable = "Trades";
            CurrentSession.GetSessionTables(this, "Trades");            
        }

        private void historicDataToolStripMenuItem_Click(object sender, EventArgs e)
        {            
            StatusLabel(1);
            //CurrentTable = "HistoricData";
            CurrentSession.GetSessionTables(this, "HistoricData");
        }

        private void xmlMergeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StatusLabel(1);
            CurrentSession.GetSessionTables(this, "XmlMerge");
        } 

        #endregion ToolStripMenus

        public void GetTableData(O2GTable table, IO2GTableListener listener,
            O2GRow row, O2GTableManager mTblMgr)
        {
            try
            {
                table.subscribeUpdate(O2GTableUpdateType.Insert, listener);
                table.subscribeUpdate(O2GTableUpdateType.Update, listener);
                try
                {
                    mTblMgr.lockUpdates();
                    for (int ii = 0; ii < table.Count; ii++)
                    {
                        row = table.getGenericRow(ii);

                        foreach (O2GTableColumn CurrentColumn in table.Columns)
                        {
                            for (int jj = 0; jj < row.Columns.Count; jj++)
                            {
                                if (CurrentColumn.ID == row.Columns[jj].ID)
                                {
                                    dataGridView.Rows[ii].Cells[CurrentColumn.ID].Value = row.getCell(jj);
                                }
                            }
                        }
                    }
                }
                finally
                {
                    mTblMgr.unlockUpdates();
                }

                table.unsubscribeUpdate(O2GTableUpdateType.Insert, listener);
                table.unsubscribeUpdate(O2GTableUpdateType.Update, listener);
            }
            catch(Exception e)
            {
                LogDirector.DoAction(4, e);
            }
        }

        public void PopulateTable(DataTable CurrentTable)
        {
            dataGridView.DataSource = CurrentTable;
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            if (CurrentTable != "")
            {
                StatusLabel(1);
                CurrentSession.GetSessionTables(this, CurrentTable);
            }
        }                 
    }
}


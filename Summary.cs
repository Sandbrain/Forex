using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using fxcore2;
using System.Threading;

namespace Forex
{
    class Summary
    {
        private DataTable m_summary = new DataTable("Summary");

        public DataTable SummaryTable { get { return m_summary; } }

        public Summary()
        {
        }

        public Summary(MainForm CurrentForm, O2GTableManager mTblMgr)
        {
            CreateTable();

            CurrentForm.PopulateTable(SummaryTable);            


            while (mTblMgr.getStatus() != O2GTableManagerStatus.TablesLoaded && mTblMgr.getStatus() != O2GTableManagerStatus.TablesLoadFailed)
            {
                Thread.Sleep(50);
            }
            O2GSummaryTable table = (O2GSummaryTable)mTblMgr.getTable(O2GTableType.Summary);
            SummaryListener listener = new SummaryListener();
            O2GSummaryTableRow row = null;

            for (int i = 0; i < table.Count; i++)
            {
                DataRow CurrentRow = SummaryTable.NewRow();
                SummaryTable.Rows.Add(CurrentRow);
            }

            CurrentForm.GetTableData(table, listener, row, mTblMgr);
        }

        public void CreateTable()
        {
            m_summary.Columns.Add("OfferID", typeof(String));
            m_summary.Columns.Add("DefaultSortOrder", typeof(Int32));
            m_summary.Columns.Add("Instrument", typeof(String));
            m_summary.Columns.Add("SellNetPL", typeof(Double));
            m_summary.Columns.Add("SellAmount", typeof(Double));
            m_summary.Columns.Add("SellAvgOpen", typeof(Double));
            m_summary.Columns.Add("BuyClose", typeof(Double));
            m_summary.Columns.Add("SellClose", typeof(Double));
            m_summary.Columns.Add("BuyAvgOpen", typeof(Double));
            m_summary.Columns.Add("BuyAmount", typeof(Double));
            m_summary.Columns.Add("BuyNetPL", typeof(Double));
            m_summary.Columns.Add("Amount", typeof(Double));
            m_summary.Columns.Add("GrossPL", typeof(Double));
            m_summary.Columns.Add("NetPL", typeof(Double));
        }
    }


    class SummaryListener : IO2GTableListener
    {
        public void onAdded(string rowID, O2GRow rowData)
        {
            O2GSummaryTableRow trade = (O2GSummaryTableRow)rowData;
        }

        public void onChanged(string rowID, O2GRow rowData)
        {
            O2GSummaryTableRow trade = (O2GSummaryTableRow)rowData;
        }

        public void onDeleted(string rowID, O2GRow rowData)
        {

        }

        public void onStatusChanged(O2GTableStatus status)
        {

        }   
    }
}

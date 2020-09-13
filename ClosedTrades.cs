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
    class ClosedTrades
    {
        private DataTable m_closedtrades = new DataTable("ClosedTrades");

        public DataTable ClosedTradesTable { get { return m_closedtrades; } }

        public ClosedTrades()
        {
        }

        public ClosedTrades(MainForm CurrentForm, O2GTableManager mTblMgr)
        {
            CreateTable();

            CurrentForm.PopulateTable(ClosedTradesTable);            


            while (mTblMgr.getStatus() != O2GTableManagerStatus.TablesLoaded && mTblMgr.getStatus() != O2GTableManagerStatus.TablesLoadFailed)
            {
                Thread.Sleep(50);
            }
            O2GClosedTradesTable table = (O2GClosedTradesTable)mTblMgr.getTable(O2GTableType.ClosedTrades);
            ClosedTradesListener listener = new ClosedTradesListener();
            O2GClosedTradeTableRow row = null;

            for (int i = 0; i < table.Count; i++)
            {
                DataRow CurrentRow = ClosedTradesTable.NewRow();
                ClosedTradesTable.Rows.Add(CurrentRow);
            }

            CurrentForm.GetTableData(table, listener, row, mTblMgr);
        }

        public void CreateTable()
        {
            m_closedtrades.Columns.Add("TradeID", typeof(String));
            m_closedtrades.Columns.Add("AccountID", typeof(String));
            m_closedtrades.Columns.Add("AccountName", typeof(String));
            m_closedtrades.Columns.Add("AccountKind", typeof(String));
            m_closedtrades.Columns.Add("OfferID", typeof(String));
            m_closedtrades.Columns.Add("Amount", typeof(Int32));
            m_closedtrades.Columns.Add("BuySell", typeof(String));
            m_closedtrades.Columns.Add("GrossPL", typeof(Double));
            m_closedtrades.Columns.Add("Commission", typeof(Double));
            m_closedtrades.Columns.Add("RolloverInterest", typeof(Double));
            m_closedtrades.Columns.Add("OpenRate", typeof(Double));
            m_closedtrades.Columns.Add("OpenQuoteID", typeof(String));
            m_closedtrades.Columns.Add("OpenTime", typeof(DateTime));
            m_closedtrades.Columns.Add("OpenOrderID", typeof(String));
            m_closedtrades.Columns.Add("OpenOrderReqID", typeof(String));
            m_closedtrades.Columns.Add("OpenOrderRequestTXT", typeof(String));
            m_closedtrades.Columns.Add("OpenOrderParties", typeof(String));
            m_closedtrades.Columns.Add("CloseRate", typeof(Double));
            m_closedtrades.Columns.Add("CloseQuoteID", typeof(String));
            m_closedtrades.Columns.Add("CloseTime", typeof(DateTime));
            m_closedtrades.Columns.Add("CloseOrderID", typeof(String));
            m_closedtrades.Columns.Add("CloseOrderReqID", typeof(String));
            m_closedtrades.Columns.Add("CloseOrderRequestTXT", typeof(String));
            m_closedtrades.Columns.Add("CloseOrderParties", typeof(String));
            m_closedtrades.Columns.Add("TradeIDOrigin", typeof(String));
            m_closedtrades.Columns.Add("TradeIDRemain", typeof(String));
            m_closedtrades.Columns.Add("ValueDate", typeof(String));
            m_closedtrades.Columns.Add("PL", typeof(Double));
        }
    }


    class ClosedTradesListener : IO2GTableListener
    {
        public void onAdded(string rowID, O2GRow rowData)
        {
            O2GClosedTradeTableRow trade = (O2GClosedTradeTableRow)rowData;
        }

        public void onChanged(string rowID, O2GRow rowData)
        {
            O2GClosedTradeTableRow trade = (O2GClosedTradeTableRow)rowData;
        }

        public void onDeleted(string rowID, O2GRow rowData)
        {

        }

        public void onStatusChanged(O2GTableStatus status)
        {

        }
    }
}

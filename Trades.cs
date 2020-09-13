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
    class Trades
    {
        private DataTable m_trades = new DataTable("Trades");

        public DataTable TradesTable { get { return m_trades; } }

        public Trades()
        {
        }


        public Trades(MainForm CurrentForm, O2GTableManager mTblMgr)
        {
            CreateTable();

            CurrentForm.PopulateTable(TradesTable);            


            while (mTblMgr.getStatus() != O2GTableManagerStatus.TablesLoaded && mTblMgr.getStatus() != O2GTableManagerStatus.TablesLoadFailed)
            {
                Thread.Sleep(50);
            }
            O2GTradesTable table = (O2GTradesTable)mTblMgr.getTable(O2GTableType.Trades);
            TradesListener listener = new TradesListener();
            O2GTradeTableRow row = null;

            for (int i = 0; i < table.Count; i++)
            {
                DataRow CurrentRow = TradesTable.NewRow();
                TradesTable.Rows.Add(CurrentRow);
            }

            CurrentForm.GetTableData(table, listener, row, mTblMgr);
        }

        public void CreateTable()
        {
            m_trades.Columns.Add("TradeID", typeof(String));
            m_trades.Columns.Add("AccountID", typeof(String));
            m_trades.Columns.Add("AccountName", typeof(String));
            m_trades.Columns.Add("AccountKind", typeof(String));
            m_trades.Columns.Add("OfferID", typeof(String));
            m_trades.Columns.Add("Amount", typeof(Int32));
            m_trades.Columns.Add("BuySell", typeof(String));
            m_trades.Columns.Add("OpenRate", typeof(Double));
            m_trades.Columns.Add("OpenTime", typeof(DateTime));
            m_trades.Columns.Add("OpenQuoteID", typeof(String));
            m_trades.Columns.Add("OpenOrderID", typeof(String));
            m_trades.Columns.Add("OpenOrderReqID", typeof(String));
            m_trades.Columns.Add("OpenOrderRequestTXT", typeof(String));
            m_trades.Columns.Add("Commission", typeof(Double));
            m_trades.Columns.Add("RolloverInterest", typeof(Double));
            m_trades.Columns.Add("TradeIDOrigin", typeof(String));
            m_trades.Columns.Add("UsedMargin", typeof(Double));
            m_trades.Columns.Add("ValueDate", typeof(String));
            m_trades.Columns.Add("Parties", typeof(String));
            m_trades.Columns.Add("Close", typeof(Double));
            m_trades.Columns.Add("GrossPL", typeof(Double));
            m_trades.Columns.Add("Limit", typeof(Double));
            m_trades.Columns.Add("PL", typeof(Double));
            m_trades.Columns.Add("Stop", typeof(Double));
        }
    }


    class TradesListener : IO2GTableListener
    {
        public void onAdded(string rowID, O2GRow rowData)
        {
            O2GTradeTableRow trade = (O2GTradeTableRow)rowData;
        }

        public void onChanged(string rowID, O2GRow rowData)
        {
            O2GTradeTableRow trade = (O2GTradeTableRow)rowData;
        }

        public void onDeleted(string rowID, O2GRow rowData)
        {

        }

        public void onStatusChanged(O2GTableStatus status)
        {

        }
    }
}

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
    class Offers
    {
        private DataTable m_offers = new DataTable("Offers");

        public DataTable OffersTable { get { return m_offers; } }

        public static List<string> CurrencyList = new List<string>();

        public Offers()
        {
        }

        public Offers(O2GTableManager mTblMgr)
        {
            while (mTblMgr.getStatus() != O2GTableManagerStatus.TablesLoaded && mTblMgr.getStatus() != O2GTableManagerStatus.TablesLoadFailed)
            {
                Thread.Sleep(50);
            }
            O2GOffersTable table = (O2GOffersTable)mTblMgr.getTable(O2GTableType.Offers);
            OffersListener listener = new OffersListener();
            O2GOfferTableRow row = null;
            table.subscribeUpdate(O2GTableUpdateType.Insert, listener);
            table.subscribeUpdate(O2GTableUpdateType.Update, listener);
            try
            {
                mTblMgr.lockUpdates();
                for (int ii = 0; ii < table.Count; ii++)
                {
                    row = table.getRow(ii);

                    foreach (O2GTableColumn CurrentColumn in table.Columns)
                    {
                        if (CurrentColumn.ID == "Instrument")
                        {
                            CurrencyList.Add(row.getCell(1).ToString());
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

        public Offers(MainForm CurrentForm, O2GTableManager mTblMgr)
        {
            CreateTable();

            CurrentForm.PopulateTable(OffersTable);            

            while (mTblMgr.getStatus() != O2GTableManagerStatus.TablesLoaded && mTblMgr.getStatus() != O2GTableManagerStatus.TablesLoadFailed)
            {
                Thread.Sleep(50);
            }
            O2GOffersTable table = (O2GOffersTable)mTblMgr.getTable(O2GTableType.Offers);
            OffersListener listener = new OffersListener();
            O2GOfferTableRow row = null;

            for(int i = 0; i < table.Count; i++)
            {
                DataRow CurrentRow = OffersTable.NewRow();
                OffersTable.Rows.Add(CurrentRow);
            }

            CurrentForm.GetTableData(table, listener, row, mTblMgr);
        }

        public void CreateTable()
        {           
            m_offers.Columns.Add("OfferID", typeof(String));
            m_offers.Columns.Add("Instrument", typeof(String));
            m_offers.Columns.Add("QuoteID", typeof(String));
            m_offers.Columns.Add("Bid", typeof(Double));
            m_offers.Columns.Add("Ask", typeof(Double));
            m_offers.Columns.Add("BidTradable", typeof(String));
            m_offers.Columns.Add("AskTradable", typeof(String));
            m_offers.Columns.Add("High", typeof(Double));
            m_offers.Columns.Add("Low", typeof(Double));
            m_offers.Columns.Add("BuyInterest", typeof(Double));
            m_offers.Columns.Add("SellInterest", typeof(Double));
            m_offers.Columns.Add("Volume", typeof(Int32));
            m_offers.Columns.Add("ContractCurrency", typeof(String));
            m_offers.Columns.Add("Digits", typeof(Int32));
            m_offers.Columns.Add("PointSize", typeof(Double));
            m_offers.Columns.Add("SubscriptionStatus", typeof(String));
            m_offers.Columns.Add("TradingStatus", typeof(String));
            m_offers.Columns.Add("InstrumentType", typeof(Int32));
            m_offers.Columns.Add("ContractMultiplier", typeof(Int32));
            m_offers.Columns.Add("ValueDate", typeof(String));
            m_offers.Columns.Add("Time", typeof(DateTime));
            m_offers.Columns.Add("PipCost", typeof(Double));
        }
    }


    class OffersListener : IO2GTableListener
    {
        public void onAdded(string rowID, O2GRow rowData)
        {
            O2GOfferTableRow trade = (O2GOfferTableRow)rowData;
        }

        public void onChanged(string rowID, O2GRow rowData)
        {
            O2GOfferTableRow trade = (O2GOfferTableRow)rowData;
        }

        public void onDeleted(string rowID, O2GRow rowData)
        {

        }

        public void onStatusChanged(O2GTableStatus status)
        {

        }
    }
}

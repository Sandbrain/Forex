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
    class Orders
    {
        private DataTable m_orders = new DataTable("Orders");

        public DataTable OrdersTable { get { return m_orders; } }       

        public Orders()
        {
        }

        public Orders(MainForm CurrentForm, O2GTableManager mTblMgr)
        {
            CreateTable();

            CurrentForm.PopulateTable(OrdersTable);


            while (mTblMgr.getStatus() != O2GTableManagerStatus.TablesLoaded && mTblMgr.getStatus() != O2GTableManagerStatus.TablesLoadFailed)
            {
                Thread.Sleep(50);
            }
            O2GOrdersTable table = (O2GOrdersTable)mTblMgr.getTable(O2GTableType.Orders);
            OrdersListener listener = new OrdersListener();
            O2GOrderTableRow row = null;

            for (int i = 0; i < table.Count; i++)
            {
                DataRow CurrentRow = OrdersTable.NewRow();
                OrdersTable.Rows.Add(CurrentRow);
            }

            CurrentForm.GetTableData(table, listener, row, mTblMgr);
        }

        public void CreateTable()
        {
            m_orders.Columns.Add("OrderID", typeof(String));
            m_orders.Columns.Add("RequestID", typeof(String));
            m_orders.Columns.Add("Rate", typeof(Double));
            m_orders.Columns.Add("ExecutionRate", typeof(Double));
            m_orders.Columns.Add("RateMin", typeof(Double));
            m_orders.Columns.Add("RateMax", typeof(Double));
            m_orders.Columns.Add("TradeID", typeof(String));
            m_orders.Columns.Add("AccountID", typeof(String));
            m_orders.Columns.Add("AccountName", typeof(String));
            m_orders.Columns.Add("OfferID", typeof(String));
            m_orders.Columns.Add("NetQuantity", typeof(Boolean));
            m_orders.Columns.Add("BuySell", typeof(String));
            m_orders.Columns.Add("Stage", typeof(String));
            m_orders.Columns.Add("Type", typeof(String));
            m_orders.Columns.Add("Status", typeof(String));
            m_orders.Columns.Add("Amount", typeof(Int32));
            m_orders.Columns.Add("StatusTime", typeof(DateTime));
            m_orders.Columns.Add("Lifetime", typeof(Int32));
            m_orders.Columns.Add("AtMarket", typeof(Double));
            m_orders.Columns.Add("TrailStep", typeof(Int32));
            m_orders.Columns.Add("TrailRate", typeof(Double));
            m_orders.Columns.Add("TimeInForce", typeof(String));
            m_orders.Columns.Add("AccountKind", typeof(String));
            m_orders.Columns.Add("RequestTXT", typeof(String));
            m_orders.Columns.Add("ContingentOrderID", typeof(String));
            m_orders.Columns.Add("ContingencyType", typeof(Int32));
            m_orders.Columns.Add("PrimaryID", typeof(String));
            m_orders.Columns.Add("OriginAmount", typeof(Int32));
            m_orders.Columns.Add("FilledAmount", typeof(Int32));
            m_orders.Columns.Add("WorkingIndicator", typeof(Boolean));
            m_orders.Columns.Add("PegType", typeof(String));
            m_orders.Columns.Add("PegOffset", typeof(Double));
            m_orders.Columns.Add("ExpireDate", typeof(DateTime));
            m_orders.Columns.Add("ValueDate", typeof(String));
            m_orders.Columns.Add("Parties", typeof(String));
            m_orders.Columns.Add("Limit", typeof(Double));
            m_orders.Columns.Add("Stop", typeof(Double));
            m_orders.Columns.Add("StopTrailStep", typeof(Int32));
            m_orders.Columns.Add("StopTrailRate", typeof(Double));
        }
    }


    class OrdersListener : IO2GTableListener
    {
        public void onAdded(string rowID, O2GRow rowData)
        {
            O2GOrderTableRow trade = (O2GOrderTableRow)rowData;
        }

        public void onChanged(string rowID, O2GRow rowData)
        {
            O2GOrderTableRow trade = (O2GOrderTableRow)rowData;
        }

        public void onDeleted(string rowID, O2GRow rowData)
        {

        }

        public void onStatusChanged(O2GTableStatus status)
        {

        }
    }    
}

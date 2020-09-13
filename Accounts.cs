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
    class Accounts
    {       
        private DataTable m_accounts = new DataTable("Accounts");

        public DataTable AccountsTable { get { return m_accounts; } }       

        public Accounts()
        {
        }       

        public Accounts(MainForm CurrentForm, O2GTableManager mTblMgr)
        {
            CreateTable();

            CurrentForm.PopulateTable(AccountsTable);            


            while (mTblMgr.getStatus() != O2GTableManagerStatus.TablesLoaded && mTblMgr.getStatus() != O2GTableManagerStatus.TablesLoadFailed)
            {
                Thread.Sleep(50);
            }
            O2GAccountsTable table = (O2GAccountsTable)mTblMgr.getTable(O2GTableType.Accounts);
            AccountsListener listener = new AccountsListener();
            O2GAccountTableRow row = null;

            for (int i = 0; i < table.Count; i++)
            {
                DataRow CurrentRow = AccountsTable.NewRow();
                AccountsTable.Rows.Add(CurrentRow);
            }

            CurrentForm.GetTableData(table, listener, row, mTblMgr);
        }

        public void CreateTable()
        {
            m_accounts.Columns.Add("AccountID", typeof(string));
            m_accounts.Columns.Add("AccountName", typeof(string));
            m_accounts.Columns.Add("Balance", typeof(Double));
            m_accounts.Columns.Add("NonTradeEquity", typeof(Double));
            m_accounts.Columns.Add("M2MEquity", typeof(Double));
            m_accounts.Columns.Add("UsedMargin", typeof(Double));
            m_accounts.Columns.Add("UsedMargin3", typeof(Double));
            m_accounts.Columns.Add("AccountKind", typeof(String));
            m_accounts.Columns.Add("MarginCallFlag", typeof(String));
            m_accounts.Columns.Add("LastMarginCallDate", typeof(DateTime));
            m_accounts.Columns.Add("MaintenanceType", typeof(String));
            m_accounts.Columns.Add("AmountLimit", typeof(Int32));
            m_accounts.Columns.Add("BaseUnitSize", typeof(Int32));
            m_accounts.Columns.Add("MaintenanceFlag", typeof(Boolean));
            m_accounts.Columns.Add("ManagerAccountID", typeof(String));
            m_accounts.Columns.Add("LeverageProfileID", typeof(String));
            m_accounts.Columns.Add("DayPL", typeof(Double));
            m_accounts.Columns.Add("Equity", typeof(Double));
            m_accounts.Columns.Add("GrossPL", typeof(Double));
            m_accounts.Columns.Add("UsableMargin", typeof(Double));
        }
    }



    class AccountsListener : IO2GTableListener
    {
        public void onAdded(string rowID, O2GRow rowData)
        {
            O2GAccountTableRow trade = (O2GAccountTableRow)rowData;
        }

        public void onChanged(string rowID, O2GRow rowData)
        {
            O2GAccountTableRow trade = (O2GAccountTableRow)rowData;
        }

        public void onDeleted(string rowID, O2GRow rowData)
        {

        }

        public void onStatusChanged(O2GTableStatus status)
        {

        }
    }
}

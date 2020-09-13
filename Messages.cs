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
    class Messages
    {
        private DataTable m_messages = new DataTable("Messages");

        public DataTable MessagesTable { get { return m_messages; } }

        public Messages()
        {
        }

        public Messages(MainForm CurrentForm, O2GTableManager mTblMgr)
        {
            CreateTable();

            CurrentForm.PopulateTable(MessagesTable);            


            while (mTblMgr.getStatus() != O2GTableManagerStatus.TablesLoaded && mTblMgr.getStatus() != O2GTableManagerStatus.TablesLoadFailed)
            {
                Thread.Sleep(50);
            }
            O2GMessagesTable table = (O2GMessagesTable)mTblMgr.getTable(O2GTableType.Messages);
            MessagesListener listener = new MessagesListener();
            O2GMessageTableRow row = null;

            for (int i = 0; i < table.Count; i++)
            {
                DataRow CurrentRow = MessagesTable.NewRow();
                MessagesTable.Rows.Add(CurrentRow);
            }

            CurrentForm.GetTableData(table, listener, row, mTblMgr);
        }

        public void CreateTable()
        {
            m_messages.Columns.Add("Time", typeof(DateTime));
            m_messages.Columns.Add("From", typeof(String));
            m_messages.Columns.Add("Type", typeof(Int32));
            m_messages.Columns.Add("Feature", typeof(String));
            m_messages.Columns.Add("Text", typeof(String));
            m_messages.Columns.Add("Subject", typeof(String));
            m_messages.Columns.Add("HTMLFragmentFlag", typeof(Boolean));
        }
    }


    class MessagesListener : IO2GTableListener
    {
        public void onAdded(string rowID, O2GRow rowData)
        {
            O2GMessageTableRow trade = (O2GMessageTableRow)rowData;
        }

        public void onChanged(string rowID, O2GRow rowData)
        {
            O2GMessageTableRow trade = (O2GMessageTableRow)rowData;
        }

        public void onDeleted(string rowID, O2GRow rowData)
        {

        }

        public void onStatusChanged(O2GTableStatus status)
        {

        }   
    }
}

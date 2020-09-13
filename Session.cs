using System;
using System.Collections.Generic;
using System.Text;
using fxcore2;
using System.Threading;
using System.Windows.Forms;
using System.ComponentModel;
using System.Xml.Serialization;
using System.IO;
using System.Net.NetworkInformation;
using System.Reflection;

namespace Forex
{
    public class Session
    {
        private O2GSession m_o2gsession;
        private O2GTableManager mTblMgr;
        private SessionStatusListener statusListener;

        private string m_username = "";
        private string m_password = "";
        private string m_url = "";
        private string m_accounttype = "";

        private bool m_isinternetavailable;

        private Models m_models = null;

        private Candles m_candle;

        private Forms.ModelsForm CurrentModelsForm;
        private Accounts CurrentAccounts;
        private ClosedTrades CurrentClosedTrades;
        private Messages CurrentMessages;
        private Offers CurrentOffers;
        private Orders CurrentOrders;
        private Summary CurrentSummary;
        private Trades CurrentTrades;
        private Forex.Forms.HistoricPricesForm CurrentHistoricPrices;
        private MainForm CurrentMainForm;
        private Forms.XMLMergeForm CurrentXMLMerge;

        private bool state = false;

        private bool m_flagsetservertime = false;
        private DateTime m_utctime;
        private DateTime m_servertime;
        private TimeSpan m_rtcservertimegap;
        private double m_rtcserversumgap = 0;
        private long m_rtcserveravggap = 0;
        private double m_updatescounter = 0;
        private System.ComponentModel.BackgroundWorker m_servertimebackgroundworker;



        //public DateTime UTCTime { get { return m_utctime; } set { m_utctime = value; } }
        //public DateTime ServerTime { get { return m_servertime; } set { m_servertime = value; } }
        //public TimeSpan RTCServerTimeGap { get { return m_rtcservertimegap; } set { m_rtcservertimegap = value; } }
        //public double RTCServerSumGap { get { return m_rtcserversumgap; } set { m_rtcserversumgap = value; } }
        public long RTCServerAvgGAP { get { return m_rtcserveravggap; } }
        //public double UpdatesCounter { get { return m_updatescounter; } set { m_updatescounter = value; } }

        public bool IsInternetAvailable { get { return m_isinternetavailable; } set { m_isinternetavailable = value; } }
        public O2GSession O2GSession { get { return m_o2gsession; } }
        public Models Models { get { return m_models; } }
        public Candles Candle { get { return m_candle; } }
        public string UserName { get { return m_username; } }
        public string Password { get { return m_password; } }
        public string URL { get { return m_url; } }
        public string AccountType { get { return m_accounttype; } }


        # region Constructors

        public Session()
        {
            NetworkStatus.AvailabilityChanged += new NetworkStatusChangedHandler(DoAvailabilityChanged);

            m_models = new Models();

            m_models = Models.LoadModelsList(m_models);
        }

        public Session(MainForm CurrentForm) : this()
        {
            try
            {
                bool IsConnected = IsNetworkAvailable(0);

                if (IsConnected)
                {
                    CurrentMainForm = CurrentForm;
                    using (var Form = new Forms.SessionForm())
                    {
                        var Result = Form.ShowDialog();
                        if (Result == System.Windows.Forms.DialogResult.OK)
                        {
                            m_username = Form.UserName;
                            m_password = Form.Password;
                            m_url = Form.URL;
                            m_accounttype = Form.AccountType;
                            state = false;
                        }
                        else
                        {
                            CurrentMainForm.StatusLabel(3);
                            state = true;
                        }
                    }

                    if (!state)
                    {
                        Login(m_username, m_password, m_url, m_accounttype);

                        CurrentMainForm.StatusLabel(2);

                        m_candle = new Candles();
                        m_o2gsession.TablesUpdates += new EventHandler<TablesUpdatesEventArgs>(mSession_TablesUpdates);

                        mTblMgr = m_o2gsession.getTableManager();

                        if (m_o2gsession.getSessionStatus() == O2GSessionStatusCode.Connected)
                        {
                            InitializeServerTime();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("No internet connection detected.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception e)
            {
                LogDirector.DoAction(4, e);
            }
        }

        # endregion Constructors



        # region Methods

        private void DoAvailabilityChanged(object sender, NetworkStatusChangedArgs e)
        {
            ReportAvailability();
        }

        /// <summary>
        /// Report the current network availability.
        /// </summary>
        private void ReportAvailability()
        {
            if (NetworkStatus.IsAvailable)
            {
                IsInternetAvailable = true;
            }
            else
            {
                IsInternetAvailable = false;
                RestartModels();
            }
        }

        public void StartModel(string ModelName, BackgroundWorker CurrentWorker, DoWorkEventArgs e_dowork)
        {
            try
            {
                string ModelToRunClass = m_models.GetModelClassByName(ModelName);

                // @ HERE GL add new implemented Model Classes
                if (ModelToRunClass == "EMAShort_EMALong_Filter_CCI")
                {
                    EMAShort_EMALong_Filter_CCI SessionCurrentModel = (EMAShort_EMALong_Filter_CCI)m_models.GetModelByName(ModelName);
                    SessionCurrentModel.StartEMAShort_EMALong_Filter_CCI(this, CurrentWorker, e_dowork);
                }
            }
            catch(Exception e)
            {
                LogDirector.DoAction(4, e);
            }
        }

        public void RestartModels()
        {
            try
            {
                foreach (Model CurrentModel in Models.ModelsList)
                {
                    if (CurrentModel.IsRunning)
                    {
                        // @ HERE GL add new implemented Model Classes
                        if (CurrentModel.ModelClass == "EMAShort_EMALong_Filter_CCI")
                        {
                            EMAShort_EMALong_Filter_CCI RestartModel = (EMAShort_EMALong_Filter_CCI)CurrentModel;
                            RestartModel.InternetRestartModel();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogDirector.DoAction(4, e);
            }
        }

        public void StopModel(string ModelName, DataGridViewButtonCell ValueCell)
        {
            try
            {
                string ModelToRunClass = m_models.GetModelClassByName(ModelName);

                // @ HERE GL add new implemented Model Classes
                if (ModelToRunClass == "EMAShort_EMALong_Filter_CCI")
                {
                    EMAShort_EMALong_Filter_CCI SessionCurrentModel = (EMAShort_EMALong_Filter_CCI)m_models.GetModelByName(ModelName);
                    SessionCurrentModel.StopEMAShort_EMALong_Filter_CCI(this, ValueCell);
                }
            }
            catch (Exception e)
            {
                LogDirector.DoAction(4, e);
            }
        }
        
        public void GetSessionTables(MainForm CurrentForm, string SelectedClass)
        {
            try
            {
                switch (SelectedClass)
                {
                    case "Accounts":
                        CurrentAccounts = new Accounts(CurrentForm, mTblMgr);
                        break;
                    case "ClosedTrades":
                        CurrentClosedTrades = new ClosedTrades(CurrentForm, mTblMgr);
                        break;
                    case "Messages":
                        CurrentMessages = new Messages(CurrentForm, mTblMgr);
                        break;
                    case "Offers":
                        CurrentOffers = new Offers(CurrentForm, mTblMgr);
                        break;
                    case "Orders":
                        CurrentOrders = new Orders(CurrentForm, mTblMgr);
                        break;
                    case "Summary":
                        CurrentSummary = new Summary(CurrentForm, mTblMgr);
                        break;
                    case "Trades":
                        CurrentTrades = new Trades(CurrentForm, mTblMgr);
                        break;
                    case "Models":
                        CurrentModelsForm = new Forms.ModelsForm(this);
                        break;
                    case "HistoricData":
                        if (Offers.CurrencyList.Count == 0)
                            CurrentOffers = new Offers(mTblMgr);
                        CurrentHistoricPrices = new Forms.HistoricPricesForm(m_o2gsession);
                        break;
                    case "XmlMerge":
                        CurrentXMLMerge = new Forms.XMLMergeForm(this,m_models);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                LogDirector.DoAction(4, e);
            }
        }            

        private void mSession_TablesUpdates(object sender, TablesUpdatesEventArgs e)
        {
            try
            {
                O2GResponseReaderFactory pFactory = m_o2gsession.getResponseReaderFactory();
                O2GTablesUpdatesReader pReader = pFactory.createTablesUpdatesReader(e.Response);

                for (int i = 0; i < pReader.Count; i++)
                {
                    switch (pReader.getUpdateTable(i))
                    {
                        case O2GTableType.Accounts:
                            break;

                        case O2GTableType.ClosedTrades:
                            break;

                        case O2GTableType.Messages:
                            break;

                        case O2GTableType.Offers:
                            if (pReader.getUpdateTable(i) == O2GTableType.Offers)
                            {
                                if (pReader.getUpdateType(i) == O2GTableUpdateType.Update)
                                {
                                    O2GOfferRow offer = pReader.getOfferRow(i);

                                    if (offer.Instrument.Equals("EUR/USD"))
                                    {
                                        m_candle.RefreshCandles(offer.Ask, offer.isAskValid, offer.Bid, offer.isBidValid, offer.Time);
                                    }
                                }
                            }
                            break;

                        case O2GTableType.Orders:
                            break;

                        case O2GTableType.Summary:
                            break;

                        case O2GTableType.Trades:
                            break;

                        default:
                            break;
                    }
                }
            }
            catch(Exception ex)
            {
                LogDirector.DoAction(2, ex);
            }
        }

        public void mSession_SessionStatusChanged(object sender, SessionStatusEventArgs e)
        {
            try
            {
                int Attempts = 0;

                while (e.SessionStatus == O2GSessionStatusCode.SessionLost)
                {
                    if (Attempts < 10)
                    {
                        Login(m_username, m_password, m_url, m_accounttype);
                        Attempts++;
                        //Reconnect to models
                    }
                    else
                    {
                        break;
                    }
                }

                while (e.SessionStatus == O2GSessionStatusCode.Disconnected)
                {
                    if (Attempts < 10)
                    {
                        Login(m_username, m_password, m_url, m_accounttype);
                        Attempts++;
                        //Reconnect to models
                    }
                    else
                    {
                        break;
                    }
                }

                if (Attempts >= 10)
                {
                    System.Diagnostics.Process.Start(Application.ExecutablePath);
                    Application.Exit();
                }
            }
            catch (Exception ex)
            {
                LogDirector.DoAction(4, ex);
            }
        }

        public void Login(string sUsername, string sPassword, string sUrl, string sConnection)
        {
            try
            {
                m_o2gsession = O2GTransport.createSession();
                m_o2gsession.SessionStatusChanged += new EventHandler<SessionStatusEventArgs>(mSession_SessionStatusChanged);
                m_o2gsession.useTableManager(O2GTableManagerMode.Yes, null);
                statusListener = new SessionStatusListener();
                m_o2gsession.subscribeSessionStatus(statusListener);
                m_o2gsession.login(sUsername, sPassword, sUrl, sConnection);
                while (statusListener.LastStatus != O2GSessionStatusCode.Connected && !statusListener.Failed)
                    Thread.Sleep(50);                                
            }
            catch (Exception e)
            {
                LogDirector.DoAction(2, e);
            }
        }

        public void Logout()
        {
            try
            {
                m_o2gsession.logout();
                while (statusListener.LastStatus != O2GSessionStatusCode.Disconnected && !statusListener.Failed)
                    Thread.Sleep(50);
                m_o2gsession.unsubscribeSessionStatus(statusListener);
                m_o2gsession.Dispose();
                m_o2gsession = null;
            }
            catch(Exception e)
            {
                LogDirector.DoAction(2, e);
            }
        }
        
        /// <summary>
        /// Indicates whether any network connection is available.
        /// Filter connections below a specified speed, as well as virtual network cards.
        /// </summary>
        /// <param name="minimumSpeed">The minimum speed required. Passing 0 will not filter connection using speed.</param>
        /// <returns>
        ///     <c>true</c> if a network connection is available; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNetworkAvailable(long minimumSpeed)
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
                return false;

            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                // discard because of standard reasons
                if ((ni.OperationalStatus != OperationalStatus.Up) ||
                    (ni.NetworkInterfaceType == NetworkInterfaceType.Loopback) ||
                    (ni.NetworkInterfaceType == NetworkInterfaceType.Tunnel))
                    continue;

                // this allow to filter modems, serial, etc.
                // I use 10000000 as a minimum speed for most cases
                if (ni.Speed < minimumSpeed)
                    continue;

                // discard virtual cards (virtual box, virtual pc, etc.)
                if ((ni.Description.IndexOf("virtual", StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (ni.Name.IndexOf("virtual", StringComparison.OrdinalIgnoreCase) >= 0))
                    continue;

                // discard "Microsoft Loopback Adapter", it will not show as NetworkInterfaceType.Loopback but as Ethernet Card.
                if (ni.Description.Equals("Microsoft Loopback Adapter", StringComparison.OrdinalIgnoreCase))
                    continue;

                return true;
            }
            return false;
        }

        private void SetServerTime()
        {
            while (m_flagsetservertime == true)
            {
                Thread.Sleep(100);

                m_utctime = DateTime.UtcNow;

                // Server time is UTC @ HERE GL TO BE CHECKED!!!
                m_servertime = O2GSession.getServerTime();

                m_rtcservertimegap = (m_servertime - m_utctime);

                // dump outliers
                if (Math.Abs(m_rtcservertimegap.Ticks) < m_rtcserveravggap * 2)
                {
                    m_updatescounter = m_updatescounter + 1;
                    m_rtcserversumgap = m_rtcserversumgap + m_rtcservertimegap.TotalMilliseconds;
                    m_rtcserveravggap = Convert.ToInt64(m_rtcserversumgap / m_updatescounter);
                }
                else
                {
                    m_updatescounter = m_updatescounter + 1;
                    m_rtcserversumgap = m_rtcserversumgap + m_rtcserveravggap;
                    m_rtcserveravggap = Convert.ToInt64(m_rtcserversumgap / m_updatescounter);
                }
            }
        }

        private void InitializeServerTime()
        {
            m_flagsetservertime = false;

            m_rtcserversumgap = 0;
            m_rtcserveravggap = 0;
            m_updatescounter = 0;

            // Inizialize RTCServerAvgGAP
            for (int i = 1; i < 100; i++)
            {
                m_utctime = DateTime.UtcNow;

                // Server time is UTC @ HERE GL TO BE CHECKED!!!
                m_servertime = O2GSession.getServerTime();

                TimeSpan CurrentSpan = m_servertime - m_utctime;

                if (Math.Abs(Convert.ToDecimal(CurrentSpan.TotalMilliseconds)) < 1000)
                {
                    m_rtcservertimegap = (m_servertime - m_utctime);
                    m_updatescounter = m_updatescounter + 1;
                    m_rtcserversumgap = m_rtcserversumgap + m_rtcservertimegap.TotalMilliseconds;
                    m_rtcserveravggap = Convert.ToInt64(m_rtcserversumgap / m_updatescounter);
                }

                Thread.Sleep(10);
            }

            m_flagsetservertime = true;

            m_servertimebackgroundworker = new BackgroundWorker();

            InitializeBackgroundWorker();
            m_servertimebackgroundworker.WorkerReportsProgress = true;
            m_servertimebackgroundworker.WorkerSupportsCancellation = true;
            m_servertimebackgroundworker.RunWorkerAsync();

        }

        private void ResetServerTime()
        {
            InitializeServerTime();
        }

        public void EndWeekendRestart()
        {
            Logout();

            // after restart, login again in case connection was lost
            O2GSessionStatusCode CurrentStatus = O2GSession.getSessionStatus();

            while (CurrentStatus != O2GSessionStatusCode.Connected)
            {
                Login(UserName, Password, URL, AccountType);
                Thread.Sleep(100);
                CurrentStatus = O2GSession.getSessionStatus();
            }

            // logged in
            ResetServerTime();
        }

        #region Backgroundworker

        private void InitializeBackgroundWorker()
        {
            m_servertimebackgroundworker.DoWork +=
                new DoWorkEventHandler(backgroundWorker_DoWork);

            m_servertimebackgroundworker.RunWorkerCompleted +=
                new RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            this.SetServerTime();
        }


        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
                return;
            }
        }

        #endregion Backgroundworker

        # endregion Methods

    }



    class SessionStatusListener : IO2GSessionStatus
    {
        private object mSyncObj = new Object();

        public O2GSessionStatusCode LastStatus
        {
            get
            {
                lock (mSyncObj)
                    return mLastStatus;
            }
            set
            {
                lock (mSyncObj)
                    mLastStatus = value;
            }
        }
        private O2GSessionStatusCode mLastStatus = O2GSessionStatusCode.Unknown;

        public bool Failed
        {
            get { return mFailed; }
        }
        private volatile bool mFailed = false;

        public string LastError
        {
            get { return mLastError; }
        }
        private string mLastError = string.Empty;

        #region IO2GSessionStatus Members

        public void onLoginFailed(string error)
        {
            mFailed = true;
            mLastError = error;
        }

        public void onSessionStatusChanged(O2GSessionStatusCode status)
        {
            LastStatus = status;
            mFailed = false;
            mLastError = string.Empty;
        }

        #endregion

    }
}

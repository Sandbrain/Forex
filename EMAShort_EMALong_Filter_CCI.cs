using System;
using System.Collections.Generic;
using fxcore2;
using System.Threading;
using System.Timers;
using System.IO;
using System.ComponentModel;
using System.Windows.Forms;
using System.Security.Permissions;
using System.Net.NetworkInformation;
using System.Xml.Serialization;


namespace Forex
{
    [XmlType("EMAShort_EMALong_Filter_CCI")] // define Type
    public class EMAShort_EMALong_Filter_CCI : Model
    {

        #region Variables

        private HistoricPrices m_historicprices = null;
        
        private CreateOrders m_order = new CreateOrders();

        private Session m_currentsession;

        private BackgroundWorker m_currentworker;

        // --------------------------------------------------------------------- //
        //                                                                       //
        //                  EMA40_80_CCI21_Sell model variables                   //
        //                                                                       //
        // --------------------------------------------------------------------- //

        private List<double> m_ema40 = new List<double>();
        private List<double> m_ema80 = new List<double>();
        private List<double> m_cci21 = new List<double>();
        private List<double> m_typicalprice = new List<double>();
        private List<double> m_meandev = new List<double>();
        private List<double> m_smatypicalprice = new List<double>();
        private List<double> m_sma40 = new List<double>();
        private List<double> m_sma80 = new List<double>();

        private int m_ccidays;
        private int m_ema40days;
        private int m_ema80days;

        private int m_sma40days { get { return m_ema40days; } }
        private int m_sma80days { get { return m_ema80days; } }
        private int m_smatypicalpricedays { get { return m_ccidays; } }
        private int m_meandevdays { get { return m_ccidays; } }
        private bool m_ema40crossdownema80 = false;

        private string m_ccivar;
        private int m_ccival1;
        private int m_ccival2;
        private string m_ccicond1;
        private string m_ccicond2;



        [XmlElement("CCIDays")]
        public int CCIDays {get {return m_ccidays; } set {m_ccidays =  value; } }
        [XmlElement("EMA40Days")]
        public int EMA40Days { get { return m_ema40days; } set { m_ema40days = value; } }
        [XmlElement("EMA80Days")]
        public int EMA80Days { get { return m_ema80days; } set { m_ema80days = value; } }

        [XmlElement]
        public string CCIVar { get { return m_ccivar; } set { m_ccivar = value; } }
        [XmlElement]
        public int CCIVal1 { get { return m_ccival1; } set { m_ccival1 = value; } }
        [XmlElement]
        public int CCIVal2 { get { return m_ccival2; } set { m_ccival2 = value; } }
        [XmlElement]
        public string CCICond1 { get { return m_ccicond1; } set { m_ccicond1 = value; } }
        [XmlElement]
        public string CCICond2 { get { return m_ccicond2; } set { m_ccicond2 = value; } }



        #endregion Variables


        # region Constructors

        public EMAShort_EMALong_Filter_CCI()
        {
        }

        // create New EMAShort_EMALong_Filter_CCI Model based on user input
        public EMAShort_EMALong_Filter_CCI(Forex.Forms.ModelCreateForm form)
        {
            ModelName = form.Controls["tbModelName"].Text;

            ModelClass = form.Controls["tbModelClass"].Text;
            CrossType = form.Controls["cbType"].Text;

            Instrument = form.Controls["cbInstrument"].Text;

            Minutes = Convert.ToInt32(form.Controls["tbMinutes"].Text);
            TimeFrame = form.Controls["tbTimeFrame"].Text;
            
            Amount = Convert.ToInt32(form.Controls["tbAmount"].Text);
            
            TakeProfitValue = Convert.ToDouble(form.Controls["tbTakeProfit"].Text);
            StopLossValue = Convert.ToDouble(form.Controls["tbStopLoss"].Text);

            WeekeendStartMinute = Convert.ToInt32(form.Controls["tbStartWeekendMinute"].Text);
            WeekeendStartETHour = Convert.ToInt32(form.Controls["tbStartWeekendHour"].Text);
            WeekendStartDayOfWeek = (DayOfWeek)DOW(form.Controls["tbStartWeekendDay"].Text);

            WeekeendEndMinute = Convert.ToInt32(form.Controls["tbEndWeekendMinute"].Text);
            WeekeendEndETHour = Convert.ToInt32(form.Controls["tbEndWeekendHour"].Text);
            WeekendEndDayOfWeek = (DayOfWeek)DOW(form.Controls["tbEndWeekendDay"].Text);

            m_ccidays = Convert.ToInt32(form.Controls["tbCCIDays"].Text);
            m_ema40days = Convert.ToInt32(form.Controls["tbema40days"].Text);
            m_ema80days = Convert.ToInt32(form.Controls["tbema80days"].Text);

            m_ccivar = Convert.ToString(form.Controls["tbccivar"].Text);
            m_ccival1 = Convert.ToInt32(form.Controls["tbccival1"].Text);
            m_ccival2 = Convert.ToInt32(form.Controls["tbccival2"].Text);
            m_ccicond1 = Convert.ToString(form.Controls["tbccicond1"].Text);
            m_ccicond2 = Convert.ToString(form.Controls["tbccicond2"].Text);

        }

        ~EMAShort_EMALong_Filter_CCI()
        {
        }



        # endregion Constructors


        # region Methods

        public void StartEMAShort_EMALong_Filter_CCI(Session Session, BackgroundWorker CurrentWorker, DoWorkEventArgs e_dowork)
        {
            try
            {
                MainLoop = true;

                FlagRunModelStop = false;

                ModelLogFile = AppDomain.CurrentDomain.BaseDirectory + ModelName + ".txt";

                System.IO.File.WriteAllText(ModelLogFile, "");

                m_currentworker = CurrentWorker;

                m_currentsession = Session;

                this.IsRunning = true;

                m_currentworker.ReportProgress(0, "Start " + ModelName + " @ UTC " + DateTime.Now.ToUniversalTime());

                //RTCServerSumGap = 0;
                //RTCServerAvgGAP = 0;
                //UpdatesCounter = 0;

                //// Inizialize RTCServerAvgGAP
                //for (int i = 1; i < 10; i++)
                //{
                //    UTCTime = DateTime.UtcNow;

                //    // Server time is UTC @ HERE GL TO BE CHECKED!!!
                //    ServerTime = m_currentsession.O2GSession.getServerTime();

                //    RTCServerTimeGap = (ServerTime - UTCTime);
                //    UpdatesCounter = UpdatesCounter + 1;
                //    RTCServerSumGap = RTCServerSumGap + RTCServerTimeGap.TotalMilliseconds;
                //    RTCServerAvgGAP = Convert.ToInt64(RTCServerSumGap / UpdatesCounter);

                //    Thread.Sleep(10);
                //}

                while (MainLoop)
                {
                    // Start Model (it can be as well Restart after Internet Disconnection or Weekend Pause)
                    m_currentworker.ReportProgress(0, "Pre-processing " + ModelName + " @ UTC " + DateTime.Now.ToUniversalTime());

                    ModelPrelimAction();

                    // Notify open Position
                    m_currentworker.ReportProgress(0, ModelName + " is into a Sell order = " + IsIn.ToString() + "...");

                    m_currentworker.ReportProgress(0, "Start Running " + ModelName + " @ UTC " + DateTime.Now.ToUniversalTime());

                    // run the model
                    RunModelEventHandler();

                    if (FlagIsWeekendIdleCycle == true)
                    {
                        // exited main cycle because is in weekend

                        TimeSpan SleepingInterval = WeekendEnd - ToET(DateTime.UtcNow);

                        m_currentworker.ReportProgress(0, "Weekend start " + ModelName + " @ UTC " + DateTime.Now.ToUniversalTime());
                        m_currentworker.ReportProgress(0, "Weekend duration " + ModelName + " @ " + SleepingInterval.ToString());
                        m_currentworker.ReportProgress(0, "Weekend end " + ModelName + " @ UTC " + DateTime.Now.ToUniversalTime().Add(SleepingInterval));

                        Thread.Sleep(SleepingInterval);

                        FlagRunModelStop = false;

                        m_currentworker.ReportProgress(0, "Restart after weekend " + ModelName + " @ UTC " + DateTime.Now.ToUniversalTime());

                        // m_currentsession.O2GSession.logout();
                        m_currentsession.EndWeekendRestart();

                        // after restart, login again in case connection was lost
                        O2GSessionStatusCode CurrentStatus = m_currentsession.O2GSession.getSessionStatus();

                        while (CurrentStatus != O2GSessionStatusCode.Connected)
                        {
                            m_currentsession.O2GSession.login(m_currentsession.UserName, m_currentsession.Password, m_currentsession.URL, m_currentsession.AccountType);
                            Thread.Sleep(100);
                            CurrentStatus = m_currentsession.O2GSession.getSessionStatus();
                        }
                    }
                    else if (FlagInternetStop == true)
                    {
                        while (!m_currentsession.IsInternetAvailable)
                        {
                            Thread.Sleep(100);
                        }
                        FlagInternetStop = false;
                        m_currentworker.ReportProgress(0, "Internet ReStart " + ModelName + " @ " + m_currentsession.O2GSession.getServerTime());

                        StringToWrite = "INTERNET RESTART @ " + m_currentsession.O2GSession.getServerTime().ToString() + ":" + m_currentsession.O2GSession.getServerTime().Millisecond.ToString();
                        WriteToFile(ModelLogFile, StringToWrite);

                        StringToWrite = "####################################";
                        WriteToFile(ModelLogFile, StringToWrite);

                    }
                    else if (FlagRunModelStop == true)
                    {
                        MainLoop = false;
                    }
                }

                this.IsRunning = false;

                ModelStatusCell.Value = "Status";

                StringToWrite = "STOP RUNNING MODEL @ " + m_currentsession.O2GSession.getServerTime().ToString() + ":" + m_currentsession.O2GSession.getServerTime().Millisecond.ToString();
                WriteToFile(ModelLogFile, StringToWrite);

                m_currentworker.ReportProgress(0, "Stop Running Model " + ModelName + " @ " + m_currentsession.O2GSession.getServerTime());

            }
            catch (Exception e)
            {
                LogDirector.DoAction(4, e);
            }
        }

        public void StopEMAShort_EMALong_Filter_CCI(Session Session, DataGridViewButtonCell Current)
        {
            ModelStatusCell = Current;

            try
            {
                FlagRunModelStop = true;
                ModelStatusCell.Value = "Stopping model...";
            }
            catch (Exception e)
            {
                LogDirector.DoAction(4, e);
            }
        }

        private void ModelPrelimAction()
        {
            // Get Historic Prices
            GetHistoricPrices();

            // Calculate Indicators
            CalcIndicators();

            // Check if the model is into a signal according to historic data
            RunVirtualModelEventHandler();

            // after calculating indicators, remove from all lists the items that are no more needed when updating the indicators
            CleanLists();
        }

        public void InternetRestartModel()
        {
            // @ here GL Internet
            m_currentworker.ReportProgress(0, "Internet Disconnection Detected " + ModelName + " @ UTC " + m_currentsession.O2GSession.getServerTime());
            FlagInternetStop = true;
        }

        private void RunModelEventHandler()
        {
            // DateTime for the last Candle received from Server
            DateTime LastCandleStartDateTime;

            // Expected DateTime for current Candle
            DateTime CurrentCandleStartDateTime;

            // the number of missing updates in a row (Server didn't return expected Candle)
            int MissingUpdatesCounter = 0;

            m_currentworker.ReportProgress(0, "Last candle received time " + ModelName + " @ " + m_historicprices.DateTimeList[m_historicprices.DateTimeList.Count - 1]);

            m_currentworker.ReportProgress(0, "Model " + ModelName + " Start loop @ UTC " + DateTime.Now.ToUniversalTime());

            // set next Candle expected DateTime to the most recent Candle DateTime + Model number of Minutes
            DateTime NextCandleExpectedStartDateTime = m_historicprices.DateTimeList[m_historicprices.DateTimeList.Count - 1].AddMinutes(Minutes);

            DateTime HistoricPricesToDate;
            DateTime HistoricPricesFromDate;

            DateTime StartServerRequestDateTime;

            // M5Candle LastCandleReceived;
            Candle LastCandleReceived = null;

            TimeSpan SleepingInterval = TimeSpan.MinValue;

            while (!FlagRunModelStop && !FlagInternetStop)
            {
                if (MissingUpdatesCounter > 0)
                {
                    // last update failed; set last received Candle DateTime = to last valid update + m_minutes
                    // (the value that should have been stored in the data if the update was successful 
                    LastCandleStartDateTime = m_historicprices.DateTimeList[m_historicprices.DateTimeList.Count - 1].AddMinutes(Minutes * MissingUpdatesCounter);
                }
                else
                {
                    // last update was regular
                    LastCandleStartDateTime = m_historicprices.DateTimeList[m_historicprices.DateTimeList.Count - 1];
                }

                // @ here gl Debug
                // StringToWrite = "LAST CANDLE START DATE TIME: " + LastCandleStartDateTime.ToString() + ":" + LastCandleStartDateTime.Millisecond.ToString();
                // ModelLog.WriteLine(StringToWrite);

                // StringToWrite = "NEXT EXPECTED CANDLE START DATE TIME: " + NextCandleExpectedStartDateTime.ToString() + ":" + NextCandleExpectedStartDateTime.Millisecond.ToString();
                // ModelLog.WriteLine(StringToWrite);


                // check if Next Candle is in weekend trading idle cycle (waiting for trading session to restart;

                FlagIsWeekendIdleCycle = IsWeekendIdleCycle(LastCandleStartDateTime.AddMinutes(Minutes));

                if (!FlagIsWeekendIdleCycle)
                {

                    // check consistency between Last received update and expected date time
                    // at this stage it should be no needed as already trapped in the code
                    if (LastCandleStartDateTime.AddMinutes(Minutes).Equals(NextCandleExpectedStartDateTime))
                    {
                        // set DateTime for current Candle equal to last update received + m_minutes

                        // int CurrentCandleStartMinute = LastCandleStartDateTime.Minute / m_minutes * m_minutes + m_minutes * 2;
                        int CurrentCandleStartMinute = LastCandleStartDateTime.Minute / Minutes * Minutes + Minutes;

                        if (CurrentCandleStartMinute < 60)
                        {
                            CurrentCandleStartDateTime = new DateTime(LastCandleStartDateTime.Year, LastCandleStartDateTime.Month, LastCandleStartDateTime.Day, LastCandleStartDateTime.Hour, CurrentCandleStartMinute, 0);
                        }
                        else
                        {
                            // new hour

                            CurrentCandleStartMinute = CurrentCandleStartMinute - 60;

                            if (LastCandleStartDateTime.Hour < 23)
                            {
                                CurrentCandleStartDateTime = new DateTime(LastCandleStartDateTime.Year, LastCandleStartDateTime.Month, LastCandleStartDateTime.Day, LastCandleStartDateTime.Hour + 1, CurrentCandleStartMinute, 0);
                            }
                            else
                            {
                                // new day
                                CurrentCandleStartDateTime = new DateTime(LastCandleStartDateTime.Year, LastCandleStartDateTime.Month, LastCandleStartDateTime.Day + 1, 0, CurrentCandleStartMinute, 0);
                            }
                        }

                        // @ here gl Debug
                        // StringToWrite = "CURRENT CANDLE START DATE TIME: " + CurrentCandleStartDateTime.ToString() + ":" + CurrentCandleStartDateTime.Millisecond.ToString();
                        // ModelLog.WriteLine(StringToWrite);

                        // request Historic Prices; set From and To Date

                        // From Date
                        // Add 59 seconds in order to cover all time period
                        HistoricPricesFromDate = CurrentCandleStartDateTime.AddSeconds(59);

                        // @ here gl Debug
                        // StringToWrite = "HISTORIC PRICES FROM DATE: " + HistoricPricesFromDate.ToString() + ":" + HistoricPricesFromDate.Millisecond.ToString();
                        // ModelLog.WriteLine(StringToWrite);

                        // To Date
                        // HistoricPricesToDate = CurrentCandleStartDateTime.Add(-HistoricPricesToDateAdjustFactor);
                        HistoricPricesToDate = CurrentCandleStartDateTime.AddSeconds(60 * Minutes - 1);

                        // @ here gl Debug
                        // StringToWrite = "HISTORIC PRICES TO DATE: " + HistoricPricesToDate.ToString() + ":" + HistoricPricesToDate.Millisecond.ToString();
                        // ModelLog.WriteLine(StringToWrite);

                        //UTCTime = DateTime.UtcNow;

                        //// @ here gl Debug
                        //// StringToWrite = "RTC TIME: " + UTCTime.ToString() + ":" + UTCTime.Millisecond.ToString();
                        //// ModelLog.WriteLine(StringToWrite);

                        //// Server time is UTC @ HERE GL TO BE CHECKED!!!
                        //ServerTime = m_currentsession.O2GSession.getServerTime();

                        //RTCServerTimeGap = (ServerTime - UTCTime);

                        //// dump outliers
                        //if (RTCServerTimeGap.Ticks < RTCServerAvgGAP * 2)
                        //{
                        //    UpdatesCounter = UpdatesCounter + 1;
                        //    RTCServerSumGap = RTCServerSumGap + RTCServerTimeGap.TotalMilliseconds;
                        //    RTCServerAvgGAP = Convert.ToInt64(RTCServerSumGap / UpdatesCounter);
                        //}
                        //else
                        //{
                        //    UpdatesCounter = UpdatesCounter + 1;
                        //    RTCServerSumGap = RTCServerSumGap + RTCServerAvgGAP;
                        //    RTCServerAvgGAP = Convert.ToInt64(RTCServerSumGap / UpdatesCounter);
                        //}

                        // SleepingInterval = (CurrentCandleStartDateTime.AddMinutes(Minutes) - UTCTime.AddMilliseconds(RTCServerAvgGAP));
                        try
                        {
                            SleepingInterval = (CurrentCandleStartDateTime.AddMinutes(Minutes) - DateTime.UtcNow.AddMilliseconds(m_currentsession.RTCServerAvgGAP));
                        }
                        catch (Exception f)
                        {

                        }

                        // @ here gl Debug
                        // StringToWrite = "SLEEPING INTERVAL: " + SleepingInterval;
                        // ModelLog.WriteLine(StringToWrite);

                        // @ here gl Debug
                        // StringToWrite = "AVERAGE GAP: " + RTCServerAvgGAP.ToString();
                        // ModelLog.WriteLine(StringToWrite);

                        // @ here gl Debug
                        // StringToWrite = "SLEEPING INTERVAL: " + SleepingInterval.ToString();
                        // ModelLog.WriteLine(StringToWrite);

                        // @ here gl Debug
                        // StringToWrite = "SERVER TIME BEFORE SLEEPING: " + m_currentsession.O2GSession.getServerTime().ToString() + ":" + m_currentsession.O2GSession.getServerTime().Millisecond.ToString();
                        // ModelLog.WriteLine(StringToWrite);

                        // @ here gl Debug
                        // StringToWrite = "ESTIMATED SERVER TIME BEFORE SLEEPING: " + UTCTime.AddMilliseconds(RTCServerAvgGAP).ToString() + ":" + UTCTime.AddMilliseconds(RTCServerAvgGAP).Millisecond.ToString();
                        // ModelLog.WriteLine(StringToWrite);

                        // @ here gl Debug
                        // StringToWrite = "REAL TIME BEFORE SLEEPING: " + DateTime.Now.ToString() + ":" + DateTime.Now.Millisecond.ToString();
                        // ModelLog.WriteLine(StringToWrite);

                        // ModelLog.Flush();

                        Thread.Sleep(SleepingInterval);

                        // @ here gl Debug
                        StringToWrite = "SERVER TIME AFTER SLEEPING: " + m_currentsession.O2GSession.getServerTime().ToString() + ":" + m_currentsession.O2GSession.getServerTime().Millisecond.ToString();
                        //ModelLog.WriteLine(StringToWrite);
                        
                        WriteToFile(ModelLogFile, StringToWrite);

                        // @ here gl Debug
                        // StringToWrite = "ESTIMATED SERVER TIME AFTER SLEEPING: " + DateTime.Now.AddMilliseconds(RTCServerAvgGAP).ToString() + ":" + DateTime.Now.AddMilliseconds(RTCServerAvgGAP).Millisecond.ToString();
                        // ModelLog.WriteLine(StringToWrite);

                        // @ here gl Debug
                        StringToWrite = "UTC REAL TIME AFTER SLEEPING: " + DateTime.Now.ToUniversalTime().ToString() + ":" + DateTime.Now.ToUniversalTime().Millisecond.ToString();
                        //ModelLog.WriteLine(StringToWrite);
                        
                        WriteToFile(ModelLogFile, StringToWrite);

                        // Start Server Request Time
                        StartServerRequestDateTime = DateTime.Now;

                        // @ here gl Debug
                        // StringToWrite = "SERVER TIME BEFORE CANDLE REQUEST: " + m_currentsession.O2GSession.getServerTime().ToString() + ":" + m_currentsession.O2GSession.getServerTime().Millisecond.ToString();
                        // ModelLog.WriteLine(StringToWrite);

                        // update data
                        if (Minutes == 5)
                            LastCandleReceived = m_currentsession.Candle.M5CandlesList[Convert.ToInt16(HistoricPricesToDate.Minute / Minutes)];
                        else if (Minutes == 1)
                            LastCandleReceived = m_currentsession.Candle.M1CandlesList[Convert.ToInt16(HistoricPricesToDate.Minute / Minutes)];

                        // @ here gl Debug
                        // StringToWrite = "CANDLE POSITION IN LIST: " + Convert.ToInt16(HistoricPricesToDate.Minute / Minutes).ToString();
                        // ModelLog.WriteLine(StringToWrite);

                        // @ here gl Debug
                        // StringToWrite = "SERVER TIME AFTER CANDLE REQUEST: " + m_currentsession.O2GSession.getServerTime().ToString() + ":" + m_currentsession.O2GSession.getServerTime().Millisecond.ToString();
                        // ModelLog.WriteLine(StringToWrite);

                        // @ here gl Debug
                        // StringToWrite = "REAL TIME AFTER CANDLE REQUEST: " + DateTime.Now.ToString() + ":" + DateTime.Now.Millisecond.ToString();
                        // ModelLog.WriteLine(StringToWrite);

                        // ModelLog.Flush();

                        // check that update was successful
                        // check that Candle got valid values (received Candle with 0 items from Server
                        // check that date is as expected to identify missing Candles
                        if (LastCandleReceived != null && LastCandleReceived.CandleStartTime == NextCandleExpectedStartDateTime)
                        {
                            // @ here gl Debug
                            StringToWrite = "LAST UPDATED CANDLE TIME: " + LastCandleReceived.CandleStartTime.ToString();
                            //ModelLog.WriteLine(StringToWrite);                            
                            WriteToFile(ModelLogFile, StringToWrite);

                            // @ here gl Debug
                            StringToWrite = "ASK CLOSE: " + LastCandleReceived.AskClose.ToString();
                            //ModelLog.WriteLine(StringToWrite);                            
                            WriteToFile(ModelLogFile, StringToWrite);

                            // @ here gl Debug
                            StringToWrite = "ASK OPEN: " + LastCandleReceived.AskOpen.ToString();
                            //ModelLog.WriteLine(StringToWrite);
                            WriteToFile(ModelLogFile, StringToWrite);

                            // @ here gl Debug
                            StringToWrite = "ASK HIGH: " + LastCandleReceived.AskHigh.ToString();
                            //ModelLog.WriteLine(StringToWrite);
                            WriteToFile(ModelLogFile, StringToWrite);

                            // @ here gl Debug
                            StringToWrite = "ASK LOW: " + LastCandleReceived.AskLow.ToString();
                            //ModelLog.WriteLine(StringToWrite);
                            WriteToFile(ModelLogFile, StringToWrite);

                            // ModelLog.Flush();

                            if (!FlagIsWeekendIdleCycle && !FlagRunModelStop && !FlagInternetStop)
                                // update was successful; update lists and indicators and run the model on the new data just received
                                RunModelEventHandler(LastCandleReceived.AskClose, LastCandleReceived.AskHigh, LastCandleReceived.AskLow, LastCandleReceived.AskOpen, LastCandleReceived.BidClose, LastCandleReceived.CandleStartTime);

                            // reset MissingUpdatesCounter
                            MissingUpdatesCounter = 0;

                        }
                        else
                        {
                            // update failed
                            MissingUpdatesCounter = MissingUpdatesCounter + 1;

                            // @ here gl Debug; notify missing data update; no actions (at the moment), system will wait for next update
                            StringToWrite = "MISSING CANDLE";
                            //ModelLog.WriteLine(StringToWrite);
                            WriteToFile(ModelLogFile, StringToWrite);

                            // @ here gl Debug
                            StringToWrite = "SERVER TIME AT MISSING CANDLE: " + m_currentsession.O2GSession.getServerTime().ToString() + ":" + m_currentsession.O2GSession.getServerTime().Millisecond.ToString();
                            //ModelLog.WriteLine(StringToWrite);
                            WriteToFile(ModelLogFile, StringToWrite);

                            // ModelLog.Flush();
                        }
                    }
                    else
                    {
                        // date received is less recent than previously update; data update not received;
                        MissingUpdatesCounter = MissingUpdatesCounter + 1;

                        FlagRunModelStop = true;

                        // @ here gl Debug
                        StringToWrite = "INTERNAL ERROR 666 @ " + m_currentsession.O2GSession.getServerTime().ToString() + ":" + m_currentsession.O2GSession.getServerTime().Millisecond.ToString();
                        //ModelLog.WriteLine(StringToWrite);
                        WriteToFile(ModelLogFile, StringToWrite);

                        //ModelLog.Flush();
                    }

                    // update next expected date time
                    NextCandleExpectedStartDateTime = NextCandleExpectedStartDateTime.AddMinutes(Minutes);

                    // @ here gl Debug
                    // StringToWrite = "NEXT CANDLE EXPECTED TIME: " + NextCandleExpectedStartDateTime.ToString() + ":" + NextCandleExpectedStartDateTime.Millisecond.ToString();
                    // ModelLog.WriteLine(StringToWrite);

                    // @ here gl Debug
                    StringToWrite = "####################################";
                    
                    //JSCODE
                    //ModelLog.WriteLine(StringToWrite);
                    WriteToFile(ModelLogFile, StringToWrite);

                    //ModelLog.Flush();
                }
                else
                {
                    // weekend; time to relax :-)
                    FlagRunModelStop = true;
                }
            }

            // @ here gl Debug
            StringToWrite = "STOP DETECTED @ " + m_currentsession.O2GSession.getServerTime().ToString() + ":" + m_currentsession.O2GSession.getServerTime().Millisecond.ToString();
            WriteToFile(ModelLogFile, StringToWrite);

            // @ here GL Internet
            m_currentworker.ReportProgress(0, "Stop Detected " + ModelName + " @ " + m_currentsession.O2GSession.getServerTime());
            m_currentworker.ReportProgress(0, "Internet Stop " + FlagInternetStop.ToString());
            m_currentworker.ReportProgress(0, "Model Stop " + FlagRunModelStop.ToString());

            // ModelLog.Close();
        }

        // this method is the event handler fired each time the model process new values to manage signals
        private void RunModelEventHandler(double NewTestAskCloseValue, double NewTestAskHighValue, double NewTestAskLowValue, double NewTestAskOpeValue, double NewTestBidCloseValue, DateTime NewTestDateTimeValue)
        {
            UpdatesIndicators(NewTestAskCloseValue, NewTestAskHighValue, NewTestAskLowValue, NewTestAskOpeValue, NewTestDateTimeValue);

            UpdatesPriceLists(NewTestAskCloseValue, NewTestAskHighValue, NewTestAskLowValue, NewTestAskOpeValue, NewTestBidCloseValue, NewTestDateTimeValue);

            RunModel();
        }

        private void RunVirtualModelEventHandler()
        {
            // from the 2nd obs
            for (int i = 1; i < m_historicprices.AskCloseList.Count - 1; i++)
            {
                RunVirtualModel(i);
            }
        }

        private void RunModel()
        {
            if (m_ema40[m_ema40.Count - 1] < m_ema80[m_ema80.Count - 1])
            {
                m_ema40crossdownema80 = true;
            }
            else
            {
                m_ema40crossdownema80 = false;
            }

            if (m_cci21[m_cci21.Count - 2] > 0 && m_cci21[m_cci21.Count - 1] < 0 && m_ema40crossdownema80 == true && IsIn == false)
            {
                // not yet into a Sell Signal; execute order

                SellPrice = m_historicprices.BidCloseList[m_historicprices.BidCloseList.Count - 1];

                StringToWrite = "SELL PRICE: " + SellPrice.ToString();
                
                //JSCODE
                //ModelLog.WriteLine(StringToWrite);
                WriteToFile(ModelLogFile, StringToWrite);

                TakeProfit = SellPrice - TakeProfitValue;
                StopLoss = SellPrice + StopLossValue;

                //@JS
                m_order.ModelCreateLimitOrder(m_currentsession, this, "OpenMarketOrder", Amount, 0, 0, 0, CrossType, TakeProfit, StopLoss);

                // IsIn = true is set when an Order has been opened on the platform and the program trap the feedback from Server;
                // BUT IF SOMETHING GOES WRONG
                // IT IS SAFER TO SET THE FLAG LOCALLY AS WELL IN ORDER TO AVOID OPENING MULTIPLE ORDERS...
                // IsIn = false is set through the platform; when the Order hit TakeProfit or StopLoss
                // at that point the model is ready to open a new Order when conditions are met
                UpdateIsIn(true);

                StringToWrite = "OPEN A NEW ORDER @: " + DateTime.Now.ToString() + ":" + DateTime.Now.Millisecond.ToString();
                //ModelLog.WriteLine(StringToWrite);
                WriteToFile(ModelLogFile, StringToWrite);

            }

            if (IsIn == true && m_historicprices.AskHighList[m_historicprices.AskHighList.Count - 1] >= StopLoss)
            {
                // Stop Loss Close Signal;

                // reset model variables
                UpdateIsIn(false);
                m_ema40crossdownema80 = false;
                SellPrice = double.NaN;
                TakeProfit = double.NaN;
                StopLoss = double.NaN;
            }

            if (IsIn == true && m_historicprices.AskLowList[m_historicprices.AskLowList.Count - 1] <= TakeProfit)
            {
                // Take Profit Close Signal

                // reset model variables
                UpdateIsIn(false);
                m_ema40crossdownema80 = false;
                SellPrice = double.NaN;
                TakeProfit = double.NaN;
                StopLoss = double.NaN;
            }

            // StringToWrite = "INDICATORS: " +
            //                     m_ema40[m_ema40.Count - 1].ToString() + ";" +
            //                     m_ema80[m_ema80.Count - 1].ToString() + ";" +
            //                     m_cci21[m_cci21.Count - 2].ToString() + ";" +
            //                     m_cci21[m_cci21.Count - 1].ToString();
            // ModelLog.WriteLine(StringToWrite);

            //ModelLog.Flush();
        }

        private void RunVirtualModel(int CurrentObs)
        {
            if (m_ema40[CurrentObs] < m_ema80[CurrentObs])
            {
                m_ema40crossdownema80 = true;
            }
            else
            {
                m_ema40crossdownema80 = false;
            }

            if (m_cci21[CurrentObs - 1] > 0 && m_cci21[CurrentObs] < 0 && m_ema40crossdownema80 == true && IsIn == false)
            {
                // not yet into a Sell Signal; execute order
                // IsIn = true;
                UpdateIsIn(true);
                SellPrice = m_historicprices.BidCloseList[CurrentObs];
                TakeProfit = SellPrice - TakeProfitValue;
                StopLoss = SellPrice + StopLossValue;
            }

            if (IsIn == true && m_historicprices.AskHighList[CurrentObs] >= StopLoss)
            {
                // Stop Loss Close Signal;

                // reset model variables
                UpdateIsIn(false);
                m_ema40crossdownema80 = false;
                SellPrice = double.NaN;
                TakeProfit = double.NaN;
                StopLoss = double.NaN;
            }

            if (IsIn == true && m_historicprices.AskLowList[CurrentObs] <= TakeProfit)
            {
                // Take Profit Close Signal

                // reset model variables
                UpdateIsIn(false);
                m_ema40crossdownema80 = false;
                SellPrice = double.NaN;
                TakeProfit = double.NaN;
                StopLoss = double.NaN;
            }
        }


        private void GetHistoricPrices()
        {
            // @ HERE GL DEBUGGING
            // StringToWrite = "CURRENT SERVER TIME: " + m_currentsession.O2GSession.getServerTime();
            // ModelLog.WriteLine(StringToWrite);

            // SET REQUEST HISTORIC PRICES START DATE

            int RequestNumDaysBack = 10 * Minutes;

            // DateTime HistoricPricesFromDate = m_currentsession.O2GSession.getServerTime().AddDays(-140);
            DateTime HistoricPricesFromDate = m_currentsession.O2GSession.getServerTime().AddDays(-RequestNumDaysBack);

            // SET REQUEST HISTORIC PRICES TO DATE
            DateTime HistoricPricesToDate = m_currentsession.O2GSession.getServerTime();

            int HistoricPricesToDateMin = HistoricPricesToDate.Minute;

            int HistoricPricesToDateNewMin = HistoricPricesToDate.Minute / Minutes * Minutes + Minutes;

            if (HistoricPricesToDateNewMin < 60)
            {
                HistoricPricesToDate = new DateTime(HistoricPricesToDate.Year, HistoricPricesToDate.Month, HistoricPricesToDate.Day, HistoricPricesToDate.Hour, HistoricPricesToDateNewMin, 0);
            }
            else
            {
                // new hour

                HistoricPricesToDateNewMin = HistoricPricesToDateNewMin - 60;

                if (HistoricPricesToDate.Hour < 23)
                {
                    HistoricPricesToDate = new DateTime(HistoricPricesToDate.Year, HistoricPricesToDate.Month, HistoricPricesToDate.Day, HistoricPricesToDate.Hour + 1, HistoricPricesToDateNewMin, 0);
                }
                else
                {
                    // new day

                    HistoricPricesToDate = new DateTime(HistoricPricesToDate.Year, HistoricPricesToDate.Month, HistoricPricesToDate.Day + 1, 0, HistoricPricesToDateNewMin, 0);
                }
            }

            // @ HERE GL DEBUGGING
            // StringToWrite = "HISTORIC PRICES FROM DATE: " + HistoricPricesFromDate.ToString();
            // ModelLog.WriteLine(StringToWrite);

            // @ HERE GL DEBUGGING
            // StringToWrite = "HISTORIC PRICES TO DATE: " + HistoricPricesToDate.ToString();
            // ModelLog.WriteLine(StringToWrite);

            // wait until ServerTime reach HistoricPricesToDate
            int interval = (int)(HistoricPricesToDate - m_currentsession.O2GSession.getServerTime()).TotalMilliseconds;

            Thread.Sleep(interval);

            // REQUEST HISTORIC PRICES
            TimeSpan CurrentTimeSpan = new TimeSpan(1000);

            m_historicprices = new HistoricPrices(m_currentsession.O2GSession, Instrument, TimeFrame, HistoricPricesFromDate, HistoricPricesToDate.Add(-CurrentTimeSpan), false, AppDomain.CurrentDomain.BaseDirectory + "\\HistoricData.txt");

            StringToWrite = "RECEIVED PRICE START TIME: " + m_historicprices.DateTimeList[m_historicprices.DateTimeList.Count - 1];
            WriteToFile(ModelLogFile, StringToWrite);

            StringToWrite = "RECEIVED PRICE END TIME: " + m_historicprices.DateTimeList[m_historicprices.DateTimeList.Count - 1].AddMinutes(Minutes);
            WriteToFile(ModelLogFile, StringToWrite);

            StringToWrite = "@ UTC: " + DateTime.Now.ToUniversalTime();
            WriteToFile(ModelLogFile, StringToWrite);

            StringToWrite = "####################################";
            WriteToFile(ModelLogFile, StringToWrite);
        }

        private void CalcIndicators()
        {
            m_sma40 = ModelIndicators.CalcSMASeries(m_historicprices.AskCloseList, m_sma40days);
            m_sma80 = ModelIndicators.CalcSMASeries(m_historicprices.AskCloseList, m_sma80days);

            m_ema40 = ModelIndicators.CalcEMASeries(m_historicprices.AskCloseList, m_ema40days);
            m_ema80 = ModelIndicators.CalcEMASeries(m_historicprices.AskCloseList, m_ema80days);

            m_typicalprice = ModelIndicators.CalcTypicalPriceSeries(m_historicprices.AskHighList, m_historicprices.AskLowList, m_historicprices.AskCloseList);
            m_smatypicalprice = ModelIndicators.CalcSMASeries(m_typicalprice, m_smatypicalpricedays);
            m_meandev = ModelIndicators.CalcMeanDevSeries(m_meandevdays, m_smatypicalprice, m_typicalprice);

            m_cci21 = ModelIndicators.CalcCCISeries(m_typicalprice, m_smatypicalprice, m_meandev, m_historicprices.AskCloseList, m_ccidays);
        }

        public void UpdatesIndicators(double NewAskCloseValue, double NewAskHighValue, double NewAskLowValue, double NewAskOpenValue, DateTime NewDateTime)
        {
            // a new price is received; update indicators

            m_ema40 = ModelIndicators.UpdateEMA(NewAskCloseValue, m_ema40days, m_ema40);
            m_ema80 = ModelIndicators.UpdateEMA(NewAskCloseValue, m_ema80days, m_ema80);

            m_typicalprice = ModelIndicators.UpdateTypicalPrice(NewAskHighValue, NewAskLowValue, NewAskCloseValue, m_typicalprice);
            m_smatypicalprice = ModelIndicators.UpdateSMA(m_typicalprice, m_smatypicalpricedays, m_smatypicalprice);
            m_meandev = ModelIndicators.UpdateMeanDev(m_smatypicalprice[m_smatypicalprice.Count - 1], m_meandev.Count - 1, m_meandevdays, m_typicalprice, m_meandev);

            m_cci21 = ModelIndicators.UpdateCCIValue(m_typicalprice[m_typicalprice.Count - 1], m_smatypicalprice[m_smatypicalprice.Count - 1], m_ccidays, m_cci21, m_meandev[m_meandev.Count - 1]);
        }

        private void UpdatesPriceLists(double NewAskCloseValue, double NewAskHighValue, double NewAskLowValue, double NewAskOpenValue, double NewBidCloseValue, DateTime NewDateTime)
        {
            m_historicprices.AskHighList.RemoveAt(0);
            m_historicprices.AskCloseList.RemoveAt(0);
            m_historicprices.AskLowList.RemoveAt(0);
            m_historicprices.AskOpenList.RemoveAt(0);
            m_historicprices.BidCloseList.RemoveAt(0);
            m_historicprices.DateTimeList.RemoveAt(0);

            m_historicprices.AskHighList.Add(NewAskHighValue);
            m_historicprices.AskCloseList.Add(NewAskCloseValue);
            m_historicprices.AskLowList.Add(NewAskLowValue);
            m_historicprices.AskOpenList.Add(NewAskOpenValue);
            m_historicprices.BidCloseList.Add(NewBidCloseValue);
            m_historicprices.DateTimeList.Add(NewDateTime);
        }

        private void CleanLists()
        {
            // only the last item is needed in all Price lists
            m_historicprices.AskHighList.RemoveRange(0, m_historicprices.AskHighList.Count - 1);
            m_historicprices.AskCloseList.RemoveRange(0, m_historicprices.AskCloseList.Count - 1);
            m_historicprices.AskLowList.RemoveRange(0, m_historicprices.AskLowList.Count - 1);
            m_historicprices.AskOpenList.RemoveRange(0, m_historicprices.AskOpenList.Count - 1);
            m_historicprices.BidCloseList.RemoveRange(0, m_historicprices.BidCloseList.Count - 1);
            m_historicprices.DateTimeList.RemoveRange(0, m_historicprices.DateTimeList.Count - 1);

            // only m_ccidays = 21 items are needed in indicators
            m_ema40.RemoveRange(0, m_ema40.Count - m_ccidays);
            m_ema80.RemoveRange(0, m_ema80.Count - m_ccidays);
            m_typicalprice.RemoveRange(0, m_typicalprice.Count - m_ccidays);
            m_smatypicalprice.RemoveRange(0, m_smatypicalprice.Count - m_ccidays);
            m_meandev.RemoveRange(0, m_meandev.Count - m_ccidays);
            m_cci21.RemoveRange(0, m_cci21.Count - m_ccidays);
        }

        # endregion Methods
    }
}

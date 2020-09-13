using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using fxcore2;

namespace Forex.Forms
{
    public partial class HistoricPricesForm : Form
    {
        private O2GSession m_session = null;
        private DateTime m_datetimestart = DateTime.MinValue;
        private DateTime m_datetimeend = DateTime.MinValue;
        private string m_instrument = "";
        private string m_interval = "";        

        public static string OutData = AppDomain.CurrentDomain.BaseDirectory + "\\HistoricData.txt";

        public HistoricPricesForm()
        {            
            InitializeComponent();
            Offers.CurrencyList.Sort();
            foreach (string Current in Offers.CurrencyList)
            {
                cbInstrument.Items.Add(Current);
            }
        }

        public HistoricPricesForm(O2GSession CurrentSession)
            : this()
        {
            m_session = CurrentSession;
            // JS ADD TIMEFRAMES FROM timeframecollection TO DROPDOWN BOX
            this.ShowDialog();
        }

        private void btnGetHistoricPrices_Click(object sender, EventArgs e)
        {
            try
            {
                m_datetimestart = monthCalendarStart.SelectionRange.Start.Date;
                m_datetimeend = monthCalendarEnd.SelectionRange.Start.Date;
                m_instrument = cbInstrument.SelectedItem.ToString();
                m_interval = cbInterval.SelectedItem.ToString();

                ResponseListener responseListener = new ResponseListener(m_session);
                m_session.subscribeResponse(responseListener);

                O2GRequestFactory factory = m_session.getRequestFactory();

                O2GTimeframeCollection timeframecollection = factory.Timeframes;
                O2GTimeframe stimeframe = timeframecollection[m_interval];

                GetHistoryPrices(m_session, m_instrument, m_interval, m_datetimestart, m_datetimeend, responseListener);

                MessageBox.Show("Historic Data Received", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch(Exception ex)
            {
                LogDirector.DoAction(2, ex);
                MessageBox.Show("Unable to connect to server", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void GetHistoryPrices(O2GSession session, string sInstrument, string sTimeframe, DateTime dtFrom, DateTime dtTo, ResponseListener responseListener)
        {
            try
            {
                StreamWriter m_data = new StreamWriter(OutData);

                string m_string_to_write = "";

                O2GRequestFactory factory = session.getRequestFactory();
                O2GTimeframe timeframe = factory.Timeframes[sTimeframe];
                if (timeframe == null)
                {
                    throw new Exception(string.Format("Timeframe '{0}' is incorrect!", sTimeframe));
                }
                O2GRequest request = factory.createMarketDataSnapshotRequestInstrument(sInstrument, timeframe, 300);
                DateTime dtFirst = dtTo;
                do // cause there is limit for returned candles amount
                {
                    factory.fillMarketDataSnapshotRequestTime(request, dtFrom, dtFirst, false);
                    responseListener.SetRequestID(request.RequestID);
                    session.sendRequest(request);
                    if (!responseListener.WaitEvents())
                    {
                        throw new Exception("Response waiting timeout expired");
                    }
                    // shift "to" bound to oldest datetime of returned data
                    O2GResponse response = responseListener.GetResponse();
                    if (response != null && response.Type == O2GResponseType.MarketDataSnapshot)
                    {
                        O2GResponseReaderFactory readerFactory = session.getResponseReaderFactory();
                        if (readerFactory != null)
                        {
                            O2GMarketDataSnapshotResponseReader reader = readerFactory.createMarketDataSnapshotReader(response);
                            if (reader.Count > 0)
                            {
                                if (DateTime.Compare(dtFirst, reader.getDate(0)) != 0)
                                {
                                    dtFirst = reader.getDate(0); // earliest datetime of returned data

                                    for (int nData = reader.Count - 1; nData > -1; nData--)
                                    {
                                        // reader.getDate(0);

                                        m_string_to_write = reader.getDate(nData).ToString() + ";" +
                                                            reader.getAsk(nData).ToString() + ";" +
                                                            reader.getAskOpen(nData).ToString() + ";" +
                                                            reader.getAskClose(nData).ToString() + ";" +
                                                            reader.getAskLow(nData).ToString() + ";" +
                                                            reader.getAskHigh(nData).ToString() + ";" +

                                                            reader.getBid(nData).ToString() + ";" +
                                                            reader.getBidOpen(nData).ToString() + ";" +
                                                            reader.getBidClose(nData).ToString() + ";" +
                                                            reader.getBidLow(nData).ToString() + ";" +
                                                            reader.getBidHigh(nData).ToString() + ";" +

                                                            reader.getVolume(nData).ToString() + ";" +

                                                            reader.getLastBarTime().ToString() + ";" +
                                                            reader.getLastBarVolume().ToString();


                                        m_data.WriteLine(m_string_to_write);
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                            else
                            {
                                Console.WriteLine("0 rows received");
                                break;
                            }
                        }
                        // PrintPrices(session, response);
                    }
                    else
                    {
                        break;
                    }
                } while (dtFirst > dtFrom);

                m_data.Close();
            }
            catch (Exception e)
            {
                int ErrorCounter = 0;

                if (ErrorCounter > 5)
                {
                    LogDirector.DoAction(4, e);
                }
                else
                {
                    ErrorCounter++;
                    LogDirector.DoAction(2, e);
                    GetHistoryPrices(session, sInstrument, sTimeframe, dtFrom, dtTo, responseListener);
                }
            }
        }
    }
}

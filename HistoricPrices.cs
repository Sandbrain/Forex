using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using fxcore2;
using System.IO;
using System.Timers;


namespace Forex
{
    class HistoricPrices
    {
        private ResponseListener m_responseListener = null;

        private List<DateTime> m_datetime = new List<DateTime>();
        
        private List<double> m_ask = new List<double>();
        private List<double> m_askopen = new List<double>();
        private List<double> m_askclose = new List<double>();
        private List<double> m_askhigh = new List<double>();
        private List<double> m_asklow = new List<double>();

        private List<double> m_bid = new List<double>();
        private List<double> m_bidopen = new List<double>();
        private List<double> m_bidclose = new List<double>();
        private List<double> m_bidhigh = new List<double>();
        private List<double> m_bidlow = new List<double>();


        public List<DateTime> DateTimeList { get { return m_datetime; } }

        public List<double> AskList { get { return m_ask; } }
        public List<double> AskOpenList { get { return m_askopen; } }
        public List<double> AskCloseList { get { return m_askclose; } }
        public List<double> AskHighList { get { return m_askhigh; } }
        public List<double> AskLowList { get { return m_asklow; } }

        public List<double> BidList { get { return m_bid; } }
        public List<double> BidOpenList { get { return m_bidopen; } }
        public List<double> BidCloseList { get { return m_bidclose; } }
        public List<double> BidHighList { get { return m_bidhigh; } }
        public List<double> BidLowList { get { return m_bidlow; } }



        public HistoricPrices()
        {
        }

        public HistoricPrices(O2GSession O2GSession, string Instrument, string Interval, DateTime DtFrom, DateTime DtTo, bool Save, string FileName)
        {
            GetHistoryPrices(O2GSession, Instrument, Interval, DtFrom, DtTo);

            if (Save == true)
            {
                SaveHistoricPrices(FileName);
            }
        }

        private void GetHistoryPrices(O2GSession O2GSession, string Instrument, string Interval, DateTime DtFrom, DateTime DtTo)
        {

            m_responseListener = new ResponseListener(O2GSession);
            O2GSession.subscribeResponse(m_responseListener);

            O2GRequestFactory factory = O2GSession.getRequestFactory();

            O2GTimeframeCollection timeframecollection = factory.Timeframes;
            O2GTimeframe Timeframe = timeframecollection[Interval];

            if (Timeframe == null)
            {
                throw new Exception(string.Format("Timeframe '{0}' is incorrect!", Timeframe));
            }

            O2GRequest request = factory.createMarketDataSnapshotRequestInstrument(Instrument, Timeframe, 300);
            DateTime DtFirst = DtTo;
            DateTime DatePrec = System.DateTime.MinValue;

            //TimeSpan PricesTimeSpan = System.TimeSpan.MinValue;

            //if (Interval == "m5")
            //{
            //    PricesTimeSpan = new TimeSpan(0, 0, 5, 0, 0);
            //}

            do // cause there is limit for returned candles amount
            {
                factory.fillMarketDataSnapshotRequestTime(request, DtFrom, DtFirst, false);
                m_responseListener.SetRequestID(request.RequestID);
                O2GSession.sendRequest(request);

                if (!m_responseListener.WaitEvents())
                {
                    throw new Exception("Response waiting timeout expired");
                }
                // shift "to" bound to oldest datetime of returned data
                O2GResponse response = m_responseListener.GetResponse();
                if (response != null && response.Type == O2GResponseType.MarketDataSnapshot)
                {
                    O2GResponseReaderFactory readerFactory = O2GSession.getResponseReaderFactory();
                    if (readerFactory != null)
                    {
                        O2GMarketDataSnapshotResponseReader reader = readerFactory.createMarketDataSnapshotReader(response);
                        if (reader.Count > 0)
                        {
                            if (DateTime.Compare(DtFirst, reader.getDate(0)) != 0)
                            {
                                DtFirst = reader.getDate(0); // earliest datetime of returned data

                                for (int nData = reader.Count - 1; nData > -1; nData--)
                                {
                                    if (reader.getDate(nData) != DatePrec)
                                    {
                                        m_datetime.Add(reader.getDate(nData));

                                        m_ask.Add(reader.getAsk(nData));
                                        m_askopen.Add(reader.getAskOpen(nData));
                                        m_askclose.Add(reader.getAskClose(nData));
                                        m_askhigh.Add(reader.getAskHigh(nData));
                                        m_asklow.Add(reader.getAskLow(nData));

                                        m_bid.Add(reader.getBid(nData));
                                        m_bidopen.Add(reader.getBidOpen(nData));
                                        m_bidclose.Add(reader.getBidClose(nData));
                                        m_bidhigh.Add(reader.getBidHigh(nData));
                                        m_bidlow.Add(reader.getBidLow(nData));
                                    }
                                    //else
                                    //{
                                    //    break;
                                    //}

                                    DatePrec = reader.getDate(nData);

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
            } while (DtFirst > DtFrom);

            m_datetime.Reverse();

            m_ask.Reverse();
            m_askopen.Reverse();
            m_askclose.Reverse();
            m_askhigh.Reverse();
            m_asklow.Reverse();

            m_bid.Reverse();
            m_bidopen.Reverse();
            m_bidclose.Reverse();
            m_bidhigh.Reverse();
            m_bidlow.Reverse();


        }

        private void SaveHistoricPrices(string FileName)
        {
            StreamWriter m_data = new StreamWriter(FileName);

            string m_string_to_write = "";

            for (int i = 0; i < m_datetime.Count; i++)
            {
                m_string_to_write = m_datetime[i].ToString() + ";" +
                                    m_ask[i].ToString() + ";" +
                                    m_askopen[i].ToString() + ";" +
                                    m_askclose[i].ToString() + ";" +
                                    m_asklow[i].ToString() + ";" +
                                    m_askhigh[i].ToString() + ";" +
                                    m_bid[i].ToString() + ";" +
                                    m_bidopen[i].ToString() + ";" +
                                    m_bidclose[i].ToString();

                m_data.WriteLine(m_string_to_write);
            }

            m_data.Close();
        }
    }
}

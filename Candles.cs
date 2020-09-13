using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using fxcore2;


namespace Forex
{
    public class Candles
    {
        private M1Candle m_currentm1candle;
        private M5Candle m_currentm5candle;

        // m1 candles
        private List<M1Candle> m_m1candleslist = new List<M1Candle>(60);

        // m5 candles
        private List<M5Candle> m_m5candleslist = new List<M5Candle>(12);

        private bool m_m1candlefirstprice;
        private bool m_m5candlefirstprice;



        public List<M1Candle> M1CandlesList { get { return m_m1candleslist; } }
        public List<M5Candle> M5CandlesList { get { return m_m5candleslist; } }

    

        public Candles()
        {
            // initialize M1 Candles list
            for (int i = 0; i < 60; i++)
            {
                M1CandlesList.Add(new M1Candle());
            }

            // initialize M5 Candles list
            for (int i = 0; i < 12; i++)
            {
                M5CandlesList.Add(new M5Candle());
            }

            m_currentm1candle = new M1Candle();
            m_currentm5candle = new M5Candle();
        }

        ~Candles()
        {
        }

        public void RefreshCandles(double CurrentAsk, bool IsCurrentAskValid, double CurrentBid, bool IsCurrentBidValid, DateTime CurrentPriceDateTime)
        {
            RefreshM1Candle(CurrentAsk, IsCurrentAskValid, CurrentBid, IsCurrentBidValid, CurrentPriceDateTime);
            RefreshM5Candle(CurrentAsk, IsCurrentAskValid, CurrentBid, IsCurrentBidValid, CurrentPriceDateTime);
        }

        public void RefreshM1Candle(double CurrentAsk, bool IsCurrentAskValid, double CurrentBid, bool IsCurrentBidValid, DateTime CurrentPriceDateTime)
        {
            if (CurrentPriceDateTime >= m_currentm1candle.CandleEndTime)
            {
                m_currentm1candle = new M1Candle();

                m_m1candlefirstprice = true;

                m_currentm1candle.CandleStartTime = new DateTime(CurrentPriceDateTime.Year, CurrentPriceDateTime.Month, CurrentPriceDateTime.Day, CurrentPriceDateTime.Hour, CurrentPriceDateTime.Minute, 0, 0);

                m_currentm1candle.CandleEndTime = m_currentm1candle.CandleStartTime.AddMinutes(1);
            }

            m_currentm1candle.RefreshCandle(CurrentAsk, IsCurrentAskValid, CurrentBid, IsCurrentBidValid, CurrentPriceDateTime, m_m1candlefirstprice);

            m_m1candlefirstprice = false;

            // update list item
            M1CandlesList[Convert.ToInt16(CurrentPriceDateTime.Minute / 1)] = m_currentm1candle;

        }

        public void RefreshM5Candle(double CurrentAsk, bool IsCurrentAskValid, double CurrentBid, bool IsCurrentBidValid, DateTime CurrentPriceDateTime)
        {
            if (CurrentPriceDateTime >= m_currentm5candle.CandleEndTime)
            {
                m_currentm5candle = new M5Candle();

                m_m5candlefirstprice = true;

                m_currentm5candle.CandleStartTime = new DateTime(CurrentPriceDateTime.Year, CurrentPriceDateTime.Month, CurrentPriceDateTime.Day, CurrentPriceDateTime.Hour, Convert.ToInt16(CurrentPriceDateTime.Minute / 5 * 5), 0, 0);

                m_currentm5candle.CandleEndTime = m_currentm5candle.CandleStartTime.AddMinutes(5);
            }

            m_currentm5candle.RefreshCandle(CurrentAsk, IsCurrentAskValid, CurrentBid, IsCurrentBidValid, CurrentPriceDateTime, m_m5candlefirstprice);

            if (IsCurrentAskValid == true)
            {
                m_m5candlefirstprice = false;
            }

            // update list item
            M5CandlesList[Convert.ToInt16(CurrentPriceDateTime.Minute / 5)] = m_currentm5candle;
        }
    }

    public class Candle
    {
        private double m_askhigh;
        private double m_askclose;
        private double m_asklow;
        private double m_askopen;
        private double m_bidclose;

        private DateTime m_candlestarttime;
        private DateTime m_candleendtime;

        public double AskHigh { get { return m_askhigh; } set { m_askhigh = value; } }
        public double AskLow { get { return m_asklow; } set { m_asklow = value; } }
        public double AskClose { get { return m_askclose; } set { m_askclose = value; } }
        public double AskOpen { get { return m_askopen; } set { m_askopen = value; } }
        public double BidClose { get { return m_bidclose; } set { m_bidclose = value; } }

        public DateTime CandleEndTime { get { return m_candleendtime; } set { m_candleendtime = value; } }
        public DateTime CandleStartTime { get { return m_candlestarttime; } set { m_candlestarttime = value; } }

        public Candle()
        {
            AskHigh = Double.MinValue;
            AskLow = Double.MaxValue;
            AskOpen = Double.NaN;
            AskClose = Double.NaN;
            BidClose = Double.NaN;

            CandleStartTime = DateTime.MinValue;
            CandleEndTime = DateTime.MinValue;
        }

        public void RefreshCandle(double CurrentAsk, bool IsCurrentAskValid, double CurrentBid, bool IsCurrentBidValid, DateTime CurrentPriceDateTime, bool IsFirstPrice)
        {
            if (IsCurrentAskValid == true)
            {
                if (IsFirstPrice == true)
                {
                    // first price
                    this.AskOpen = CurrentAsk;
                }

                this.AskHigh = Math.Max(this.AskHigh, CurrentAsk);
                this.AskLow = Math.Min(this.AskLow, CurrentAsk);
                this.AskClose = CurrentAsk;

            }
            if (IsCurrentBidValid == true)
            {
                this.BidClose = CurrentBid;
            }
        }
    }

    public class M5Candle : Candle
    {
        public M5Candle()
        {
        }
    }

    public class M1Candle : Candle
    {
        public M1Candle()
        {
        }
    }
}

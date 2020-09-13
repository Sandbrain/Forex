using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Forex
{
    public class Indicators
    {
        public Indicators()
        { 
        }

        public List<double> CalcSMASeries(List<double> Values, int SMADays)
        {
            List<double> SMASeries = new List<double>();
            double SumValues = 0;

            for (int i = 0; i < SMADays - 1; i++)
            {
                SMASeries.Add(double.NaN);
                SumValues = SumValues + Values[i];
            }

            SumValues = SumValues + Values[SMADays - 1];
            SMASeries.Add(SumValues / SMADays);

            for (int j = SMADays; j < Values.Count; j++)
            {
                SumValues = SumValues - Values[j - SMADays] + Values[j];
                SMASeries.Add(SumValues / SMADays);
            }

            return SMASeries;
        }

        //private double CalcSMAValue(List<double> Values)
        //{
        //    double SMAValue = double.NaN;
        //    double SumValues = 0;

        //    for (int i = 0; i < Values.Count; i++)
        //    {
        //        SumValues = SumValues + Values[i];
        //    }

        //    SMAValue = SumValues / Values.Count;

        //    return SMAValue;
        //}

        //private double CalcSMAValue(List<double> Values, double SumValues, double NewValue)
        //{
        //    double SMAValue = double.NaN;

        //    SumValues = SumValues - Values[0] + NewValue;

        //    SMAValue = SumValues / Values.Count;

        //    return SMAValue;
        //}

        public List<double> CalcEMASeries(List<double> Values, int EMADays)
        {
            List<double> EMASeries = new List<double>();
            List<double> SMASeries = new List<double>();

            SMASeries = CalcSMASeries(Values, EMADays);

            for (int i = 0; i < EMADays - 1; i++)
            {
                EMASeries.Add(double.NaN);
            }

            EMASeries.Add(SMASeries[EMADays - 1]);

            for (int j = EMADays; j < Values.Count; j++)
            {
                EMASeries.Add(CalcEMAValue(Values[j], EMADays, EMASeries[j - 1]));
            }

            return EMASeries;
        }

        private double CalcEMAValue(double Value, int EMADays, double EMAPrevious)
        {
            double EMAValue = double.NaN;

            double Multiplier = (double)2 / (EMADays + 1);

            EMAValue = ((Value - EMAPrevious) * Multiplier) + EMAPrevious;

            return EMAValue;
        }

        public List<double> UpdateEMA(double Value, int EMADays, List<double> EMASeries)
        {
            double NewEMAValue = CalcEMAValue(Value, EMADays, EMASeries[EMASeries.Count - 1]);

            EMASeries.RemoveAt(0);

            EMASeries.Add(NewEMAValue);

            return EMASeries;
        }

        public List<double> UpdateSMA(List<double> Value, int SMADays, List<double> SMASeries)
        {
            double NewSMAValue = 0;

            for (int i = Value.Count - 1; i > Value.Count - SMADays - 1; i--)
            {
                NewSMAValue = NewSMAValue + Value[i];
            }

            NewSMAValue = NewSMAValue / SMADays;

            SMASeries.RemoveAt(0);

            SMASeries.Add(NewSMAValue);

            return SMASeries;
        }

        public List<double> UpdateTypicalPrice(double Value1, double Value2, double Value3, List<double> TypicalPriceSeries)
        {
            double NewTypicalPriceValue = CalcTypicalPriceValue(Value1, Value2, Value3);

            TypicalPriceSeries.RemoveAt(0);

            TypicalPriceSeries.Add(NewTypicalPriceValue);

            return TypicalPriceSeries;
        }

        public List<double> UpdateMeanDev(double SMAValue, int j, int k, List<double> TypicalPriceSeries, List<double> MeanDevSeries)
        {
            double NewMeanDevValue = CalcMeanDevValue(SMAValue, j, k, TypicalPriceSeries);

            MeanDevSeries.RemoveAt(0);

            MeanDevSeries.Add(NewMeanDevValue);

            return MeanDevSeries;
        }

        public List<double> UpdateCCIValue(double TypicalPriceValue, double SMAValue, int CCIDays, List<double> CCISeries, double MeanDevValue)
        {
            double NewCCIValue = CalcCCIValue(TypicalPriceValue, SMAValue, MeanDevValue);

            CCISeries.RemoveAt(0);

            CCISeries.Add(NewCCIValue);

            return CCISeries;
        }

        public List<double> CalcTypicalPriceSeries(List<double> Value1, List<double> Value2, List<double> Value3)
        {
            List<double> TypicalPriceSeries = new List<double>();

            for (int i = 0; i < Value1.Count; i++)
            {
                TypicalPriceSeries.Add(CalcTypicalPriceValue(Value1[i], Value2[i], Value3[i]));

            }

            return TypicalPriceSeries;
        }

        public double CalcTypicalPriceValue(double Value1, double Value2, double Value3)
        {
            return (Value1 + Value2 + Value3) / 3;
        }

        //public List<double> CalcCCISeriesFromRawData(List<double> TypicalValues1, List<double> TypicalValues2, List<double> TypicalValues3, List<double> Values, int CCIDays)
        //{
        //    List<double> CCISeries = new List<double>();
        //    List<double> TypicalPriceSeries = new List<double>();
        //    List<double> SMASeries = new List<double>();
        //    double MeanDev = double.NaN;

        //    TypicalPriceSeries = CalcTypicalPriceSeries(TypicalValues1, TypicalValues2, TypicalValues3);

        //    SMASeries = CalcSMASeries(TypicalPriceSeries, CCIDays);

        //    for (int i = 0; i < CCIDays - 1; i++)
        //    {
        //        CCISeries.Add(double.NaN);
        //    }

        //    for (int j = CCIDays - 1; j < Values.Count; j++)
        //    {
        //        MeanDev = CalcMeanDevValue(SMASeries[j], j, CCIDays, TypicalPriceSeries);

        //        CCISeries.Add(CalcCCIValue(TypicalPriceSeries[j], SMASeries[j], MeanDev));
        //    }

        //    return CCISeries;
        //}

        public List<double> CalcCCISeries(List<double> TypicalPriceSeries, List<double> SMASeries, List<double> MeanDevSeries, List<double> Values, int CCIDays)
        {
            try
            {

                List<double> CCISeries = new List<double>();

                for (int i = 0; i < CCIDays - 1; i++)
                {
                    CCISeries.Add(double.NaN);
                }

                for (int j = CCIDays - 1; j < Values.Count; j++)
                {
                    CCISeries.Add(CalcCCIValue(TypicalPriceSeries[j], SMASeries[j], MeanDevSeries[j]));
                }

                return CCISeries;

            }
            catch
            {
                return null;
            }
        }

        public List<double> CalcMeanDevSeries(int MeanDevDays, List<double> SMASeries, List<double> TypicalPriceSeries)
        {
            List<double> MeanDevSeries = new List<double>();

            double MeanDev = double.NaN;

            for (int i = 0; i < MeanDevDays - 1; i++)
            {
                MeanDevSeries.Add(double.NaN);
            }

            for (int j = MeanDevDays - 1; j < SMASeries.Count; j++)
            {
                MeanDev = CalcMeanDevValue(SMASeries[j], j, MeanDevDays, TypicalPriceSeries);

                MeanDevSeries.Add(MeanDev);
            }

            return MeanDevSeries;
        }

        private double CalcCCIValue(double TypicalPriceValue, double SMAValue, double MeanDevValue)
        {
            double CCIValue = double.NaN;

            CCIValue = (TypicalPriceValue - SMAValue) / (0.015 * MeanDevValue);

            return CCIValue;
        }

        private double CalcMeanDevValue(double SMAValue, int j, int CCIDays, List<double> TypicalPriceSeries)
        {
            double MeanDev = 0;

            for (int k = j; k > j - CCIDays; k--)
            {
                MeanDev = MeanDev + Math.Abs(SMAValue - TypicalPriceSeries[k]);
            }

            MeanDev = MeanDev / CCIDays;

            return MeanDev;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using fxcore2;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using System.Xml;
using System.Net.NetworkInformation;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.Threading;

namespace Forex
{
    [XmlType("Model")] // define Type
    // @ HERE GL add new type when implementing a new Model Class
    [XmlInclude(typeof(EMAShort_EMALong_Filter_CCI))] 

    public class Model
    {
        // Common models variables

        // Model Main loop
        private bool m_mainloop;

        // Internet disconnection
        private bool m_flaginternetstop = false;

        // Stop model user request
        private bool m_flagrunmodelstop = false;

        private string m_modelname;
        private string m_modelclass;
        private string m_instrument;
        private string m_crosstype;
        private int m_amount;
        private int m_minutes;
        private string m_timeframe;
        private double m_takeprofitlimit;
        private double m_stoplosslimit;
        private Indicators m_modelindicators = new Indicators();
        private double m_takeprofit;
        private double m_stoploss;
        private double m_buyprice;
        private double m_sellprice;
        private bool m_isrunning = false;
        private bool m_isin = false;
        private bool m_flagisweekendidlecycle;
        // Weekend Start
        private DateTime m_weekendstart;
        private DayOfWeek m_weekendstartdayofweek;
        private int m_weekendstartethour;
        private int m_weekendstartminute;
        private int m_weekendstartsecond = 0;
        // Weekend End
        private DateTime m_weekendend;
        private DayOfWeek m_weekendenddayofweek;
        private int m_weekendendethour;
        private int m_weekendendminute;
        private int m_weekendendsecond = 0;
        private string m_modellogfile;
        private string m_stringtowrite;

        //private DateTime m_utctime;
        //private DateTime m_servertime;
        //private TimeSpan m_rtcservertimegap;
        //private double m_rtcserversumgap = 0;
        //private long m_rtcserveravggap = 0;
        //private double m_updatescounter = 0;

        private DataGridViewButtonCell m_modelstatus;

        private static object locker = new Object();

        private string m_ccivar;
        private int m_ccival1;
        private int m_ccival2;
        private string m_ccicond1;
        private string m_ccicond2;


        //[XmlIgnore]
        //public DateTime UTCTime { get { return m_utctime; } set { m_utctime = value; } }

        //[XmlIgnore]
        //public DateTime ServerTime { get { return m_servertime; } set { m_servertime = value; } }

        //[XmlIgnore]
        //public TimeSpan RTCServerTimeGap { get { return m_rtcservertimegap; } set { m_rtcservertimegap = value; } }

        //[XmlIgnore]
        //public double RTCServerSumGap { get { return m_rtcserversumgap; } set { m_rtcserversumgap = value; } }

        //[XmlIgnore]
        //public long RTCServerAvgGAP { get { return m_rtcserveravggap; } set { m_rtcserveravggap = value; } }

        //[XmlIgnore]
        //public double UpdatesCounter { get { return m_updatescounter; } set { m_updatescounter = value; } }

        [XmlIgnore]
        public DataGridViewButtonCell ModelStatusCell {get {return m_modelstatus;} set {m_modelstatus=value; } }

        [XmlIgnore]
        public bool MainLoop { get { return m_mainloop; } set { m_mainloop = value; } }

        [XmlIgnore]
        public bool FlagInternetStop { get { return m_flaginternetstop; } set { m_flaginternetstop = value; } }

        [XmlIgnore]
        public bool FlagRunModelStop { get { return m_flagrunmodelstop; } set { m_flagrunmodelstop = value; } }

        [XmlIgnore]
        public bool IsRunning { get { return m_isrunning; } set { m_isrunning = value; } }

        [XmlIgnore]
        public Indicators ModelIndicators { get { return m_modelindicators; } }

        [XmlIgnore]
        public DateTime WeekendStart { get { return m_weekendstart; } set { m_weekendstart = value; } }

        [XmlIgnore]
        public DateTime WeekendEnd { get { return m_weekendend; } set { m_weekendend = value; } }

        [XmlIgnore]
        public bool FlagIsWeekendIdleCycle { get { return m_flagisweekendidlecycle; } set { m_flagisweekendidlecycle = value; } }

        [XmlElement]
        public DayOfWeek WeekendStartDayOfWeek { get { return m_weekendstartdayofweek; } set { m_weekendstartdayofweek = value; } }

        [XmlElement]
        public DayOfWeek WeekendEndDayOfWeek { get { return m_weekendenddayofweek; } set { m_weekendenddayofweek = value; } }

        [XmlElement]
        public int WeekeendStartETHour { get { return m_weekendstartethour; } set { m_weekendstartethour = value; } }

        [XmlElement]
        public int WeekeendEndETHour { get { return m_weekendendethour; } set { m_weekendendethour = value; } }
        
        [XmlElement]
        public int WeekeendStartMinute { get { return m_weekendstartminute; } set { m_weekendstartminute = value; } }
        
        [XmlElement]
        public int WeekeendEndMinute { get { return m_weekendendminute; } set { m_weekendendminute = value; } }

        [XmlElement]
        public string ModelName { get { return m_modelname; } set { m_modelname = value; } }

        [XmlElement]
        public string ModelClass { get { return m_modelclass; } set { m_modelclass = value; } }
        
        [XmlIgnore]
        public double BuyPrice { get { return m_buyprice; } set { m_buyprice = value; } }
        
        [XmlIgnore]
        public double SellPrice { get { return m_sellprice; } set { m_sellprice = value; } }
        
        [XmlIgnore]
        public double TakeProfit { get { return m_takeprofit; } set { m_takeprofit = value; } }
        
        [XmlIgnore]
        public double StopLoss { get { return m_stoploss; } set { m_stoploss = value; } }
        
        [XmlElement]
        public double TakeProfitValue { get { return m_takeprofitlimit; } set { m_takeprofitlimit = value; } }
        
        [XmlElement]
        public double StopLossValue { get { return m_stoplosslimit; } set { m_stoplosslimit = value; } }
        
        [XmlElement]
        public string Instrument { get { return m_instrument; } set { m_instrument = value; } }
        
        [XmlElement]
        public int Minutes { get { return m_minutes; } set { m_minutes = value; } }

        [XmlElement]
        public string TimeFrame { get { return m_timeframe; } set { m_timeframe = value; } }

        [XmlElement]
        public int Amount { get { return m_amount; } set { m_amount = value; } }

        [XmlIgnore]
        public string ModelLogFile { get { return m_modellogfile; } set { m_modellogfile = value; } }
        
        [XmlIgnore]
        public string StringToWrite { get { return m_stringtowrite; } set { m_stringtowrite = value; } }

        [XmlIgnore]
        public bool IsIn { get { return m_isin; } set { m_isin = value; } }

        [XmlElement]
        public string CrossType { get { return m_crosstype; } set { m_crosstype = value; } }



        /* Filter Parts */

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




        public Model()
        {
        }



        public int DOW(string day)
        {
            int daynum = 0;

            switch (day)
            {
                case "Monday":
                    daynum = 1;
                    break;
                case "Tuesday":
                    daynum = 2;
                    break;
                case "Wednesday":
                    daynum = 3;
                    break;
                case "Thursday":
                    daynum = 4;
                    break;
                case "Friday":
                    daynum = 5;
                    break;
                case "Saturday":
                    daynum = 6;
                    break;
                case "Sunday":
                    daynum = 0;
                    break;
            }
            return daynum;
        }

        public bool IsWeekendIdleCycle(DateTime CurrentDateTime)
        {
            DateTime CurrentETDateTime = ToET(CurrentDateTime);

            SetWeekendInterval(CurrentETDateTime);

            return (CurrentETDateTime >= WeekendStart && CurrentETDateTime <= WeekendEnd);
        }

        private void SetWeekendInterval(DateTime CurrentDateTime)
        {
            // The (... + 7) % 7 ensures we end up with a value in the range [0, 6]
            int daysUntilWeekendStart = ((int)m_weekendstartdayofweek - (int)CurrentDateTime.DayOfWeek + 7) % 7;
            m_weekendstart = CurrentDateTime.AddDays(daysUntilWeekendStart);
            m_weekendstart = new DateTime(m_weekendstart.Year, m_weekendstart.Month, m_weekendstart.Day, m_weekendstartethour, m_weekendstartminute, m_weekendstartsecond);

            int daysUntilWeekendEnd = ((int)m_weekendenddayofweek - (int)CurrentDateTime.DayOfWeek + 7) % 7;
            m_weekendend = CurrentDateTime.AddDays(daysUntilWeekendEnd);
            m_weekendend = new DateTime(m_weekendend.Year, m_weekendend.Month, m_weekendend.Day, m_weekendendethour, m_weekendendminute, m_weekendendsecond);

            if (m_weekendstart > m_weekendend)
            {
                m_weekendstart = m_weekendstart.AddDays(-7);
            }

            if (CurrentDateTime > m_weekendend)
            {
                m_weekendstart = m_weekendstart.AddDays(7);
                m_weekendend = m_weekendend.AddDays(7);
            }
        }

        private bool IsWeekend(DateTime CurrentDateTime)
        {
            SetWeekendInterval(CurrentDateTime);
            return (CurrentDateTime >= m_weekendstart && CurrentDateTime <= m_weekendend);
        }

        public static DateTime ToET(DateTime DateTimeToConvert)
        {
            DateTime ETDateTime = new DateTime();
            TimeZoneInfo EasternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            ETDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTimeToConvert, EasternZone);
            return ETDateTime;
        }

        public void WriteToFile(string Filepath, string text)
        {
            lock (locker)
            {
                using (FileStream file = new FileStream(Filepath, FileMode.Append, FileAccess.Write, FileShare.Read))
                using (StreamWriter writer = new StreamWriter(file, Encoding.Default))
                {
                    writer.Write(text + "\n");
                }
            }
        }

        public void UpdateIsIn(bool Status)
        {
            this.IsIn = Status;
        }
    }



    [XmlRoot("ModelsFullList")]
    [XmlInclude(typeof(Model))] // include type Model Class
    public class Models
    {
        // @ HERE GL: Add typeof(NewModelClass) when implementing new Model Class
        private Type[] m_modeltypes = { typeof(Model), typeof(EMAShort_EMALong_Filter_CCI) };

        private string XmlModelsFile = AppDomain.CurrentDomain.BaseDirectory + "XMLModels.xml";

        private List<Model> m_modelslist = new List<Model>();

        [XmlIgnore]
        public Type[] ModelTypes { get { return m_modeltypes; } set { m_modeltypes = value; } }

        [XmlArray("ModelsArray")]
        [XmlArrayItem("ModelObject")]
        public List<Model> ModelsList { get { return m_modelslist; } set { m_modelslist = value; } }

        public Models()
        {
        }

        public static Models LoadModelsList(Models Current)
        {
            try
            {
                // Deserialize
                FileStream fs = new FileStream(Current.XmlModelsFile, FileMode.Open);
                XmlSerializer serializer = new XmlSerializer(typeof(Models), Current.ModelTypes);
                Current = (Models)serializer.Deserialize(fs);
                fs.Close();
                return Current;
            }
            catch (Exception e)
            {
                LogDirector.DoAction(4, e);
                return Current;
            }        
        }

        public void SaveModelsList()
        {
            try
            {
                // Serialize 
                XmlSerializer serializer = new XmlSerializer(typeof(Models), ModelTypes);
                FileStream fs = new FileStream(XmlModelsFile, FileMode.Create);
                serializer.Serialize(fs, this);
                fs.Close();

            }
            catch (Exception e)
            {
                LogDirector.DoAction(4, e);
            }        
        }

        public void AddModel(Model ModelToAdd)
        {
            ModelsList.Add(ModelToAdd);
        }

        public void RemoveModel(Model ModelToRemove)
        {
            ModelsList.Remove(ModelToRemove);
        }

        public Model GetModelByName(string ModelName)
        {
            Model ModelByName = null;

            foreach (Model CurrentModel in ModelsList)
            {
                if (CurrentModel.ModelName == ModelName)
                {
                    ModelByName = CurrentModel;
                    break;
                }
            }
            return ModelByName;
        }

        public bool ModelExist(string ModelName)
        {
            bool ModelExist = false;

            foreach (Model CurrentModel in ModelsList)
            {
                if (CurrentModel.ModelName == ModelName)
                {
                    ModelExist = true;
                    break;
                }
            }
            return ModelExist;
        }

        public string GetModelClassByName(string ModelName)
        {
            string ModelClassByName = null;

            foreach (Model CurrentModel in ModelsList)
            {
                if (CurrentModel.ModelName == ModelName)
                {
                    ModelClassByName = CurrentModel.ModelClass;
                }
            }
            return ModelClassByName;
        }
    }
}
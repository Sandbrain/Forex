//using System;
//using System.Collections.Generic;
//using System.Xml.Serialization;
//using System.IO;
//using fxcore2;

//namespace Forex
//{
//    public class RunningOrder
//    {
//        private string m_runningordermodelname;
//        private string m_runningorderaccountid;
//        private string m_runningorderofferid;
//        private string m_runningordertradeid;
//        private string m_runningorderbuysell;
//        private string m_runningorderid;
//        private int m_runningorderamount;
//        private double m_runningorderrate;
//        private string m_runningorderrequesttxt;



//        [XmlElement]
//        public string RunningOrderModelName { get { return m_runningordermodelname; } set { m_runningordermodelname = value; } }

//        [XmlElement]
//        public string RunningOrderID { get { return m_runningorderid; } set { m_runningorderid = value; } }

//        [XmlElement]
//        public string RunningOrderAccountID { get { return m_runningorderaccountid; } set { m_runningorderaccountid = value; } }

//        [XmlElement]
//        public string RunningOrderOfferID { get { return m_runningorderofferid; } set { m_runningorderofferid = value; } }

//        [XmlElement]
//        public string RunningOrderTradeID { get { return m_runningordertradeid; } set { m_runningordertradeid = value; } }

//        [XmlElement]
//        public string RunningOrderBuySell { get { return m_runningorderbuysell; } set { m_runningorderbuysell = value; } }

//        [XmlElement]
//        public int RunningOrderAmount { get { return m_runningorderamount; } set { m_runningorderamount = value; } }

//        [XmlElement]
//        public double RunningOrderRate { get { return m_runningorderrate; } set { m_runningorderrate = value; } }

//        [XmlElement]
//        public string RunningOrderRequestTXT { get { return m_runningorderrequesttxt; } set { m_runningorderrequesttxt = value; } }

//        public RunningOrder()
//        {
//        }

//        public RunningOrder(string RunningOrderModelName, O2GOrderRow CurrentOrder)
//        {
//            m_runningordermodelname = RunningOrderModelName;
//            m_runningorderaccountid = CurrentOrder.AccountID;
//            m_runningorderid = CurrentOrder.OrderID;
//            m_runningorderofferid = CurrentOrder.OfferID;
//            m_runningordertradeid = CurrentOrder.TradeID;
//            m_runningorderamount = CurrentOrder.Amount;
//            m_runningorderbuysell = CurrentOrder.BuySell;
//            m_runningorderrequesttxt = CurrentOrder.RequestTXT;
//        }
//    }


//    public class RunningOrders
//    {
//        private List<RunningOrder> m_runningorderslist = new List<RunningOrder>();
//        private string m_runningordersfile = AppDomain.CurrentDomain.BaseDirectory + "RunningOrders.xml";

//        public List<RunningOrder> RunningOrdersList { get { return m_runningorderslist; } set { m_runningorderslist = value; } }

//        public RunningOrders()
//        {
//            LoadRunningOrders(m_runningordersfile);
//        }

//        public void UpdateOrderList(Model CurrentOrderModel, O2GOrderRow CurrentOrder)
//        {

//            string CurrentOrderModelName = CurrentOrderModel.ModelName;

//            if (CurrentOrder.RequestTXT == "TrueMarketOrder")
//            {
//                // Opening a new Order; add it to List
//                AddOrderToList(new RunningOrder(CurrentOrderModelName, CurrentOrder));

//                CurrentOrderModel.UpdateIsIn(true);
//            }
//            else if (CurrentOrder.RequestTXT == "CloseMarketOrder" || CurrentOrder.RequestTXT == "FXTC")
//            {
//                // Close Order; remove from List
//                RemoveOrderFromList(CurrentOrder.TradeID);

//                CurrentOrderModel.UpdateIsIn(false);
//            }
//        }

//        public void RemoveOrderFromList(string OrderToRemoveID)
//        {
//            RunningOrder OrderToRemove = null;
//            foreach (RunningOrder CurrentRunningOrder in this.m_runningorderslist)
//            {
//                if (CurrentRunningOrder.RunningOrderTradeID == OrderToRemoveID)
//                {
//                    OrderToRemove = CurrentRunningOrder;
//                    break;
//                }
//            }

//            if (OrderToRemove != null)
//            {
//                this.m_runningorderslist.Remove(OrderToRemove);
//                this.SaveRunningOrders(m_runningordersfile);
//            }
//        }

//        public void AddOrderToList(RunningOrder CurrentOrder)
//        {
//            if (!OrderExist(CurrentOrder.RunningOrderID))
//            {
//                this.RunningOrdersList.Add(CurrentOrder);

//                this.SaveRunningOrders(m_runningordersfile);
//            }
//        }

//        //public RunningOrder GetOrderByModelName(string CurrentModelName)
//        //{
//        //    RunningOrder Order = null;

//        //    foreach (RunningOrder CurrentRunningOrder in this.m_runningorderslist)
//        //    {
//        //        if (CurrentRunningOrder.RunningOrderModelName == CurrentModelName)
//        //        {
//        //            Order = CurrentRunningOrder;
//        //            break;
//        //        }
//        //    }

//        //    return Order;
//        //}

//        private bool OrderExist(string OrderID)
//        {
//            bool OrderExist = false;
//            for (int i = 0; i < this.m_runningorderslist.Count; i++)
//            {
//                if (this.m_runningorderslist[i].RunningOrderID == OrderID)
//                {
//                    OrderExist = true;
//                    break;
//                }
//            }

//            return OrderExist;
//        }

//        private void LoadRunningOrders(string RunningOrdersFileName)
//        {
//            if (File.Exists(RunningOrdersFileName))
//            {
//                XmlSerializer deserializer = new XmlSerializer(typeof(List<RunningOrder>));
//                TextReader reader = new StreamReader(RunningOrdersFileName);
//                object obj = deserializer.Deserialize(reader);
//                this.m_runningorderslist = (List<RunningOrder>)obj;
//                reader.Close();
//            }
//        }

//        private void SaveRunningOrders(string RunningOrdersFileName)
//        {
//            XmlSerializer serializer = new XmlSerializer(typeof(List<RunningOrder>));
//            using (TextWriter writer = new StreamWriter(RunningOrdersFileName))
//            {
//                serializer.Serialize(writer, this.m_runningorderslist);
//            }
//        }
//    }
//}

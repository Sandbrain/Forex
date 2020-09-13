using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using fxcore2;
using System.Xml;
using System.Windows.Forms;
using System.IO;

namespace Forex
{
    class CreateOrders
    {
        private O2GSession m_o2gsession = null;
        // private O2GLoginRules m_loginrules = null;

        /* JS ADDED RESPONSE */
        //private ResponseListener responseListener = null;

        string m_accountid = "";
        int m_baseamount = 0;
        string m_offerid = "";
        double m_ask = 0;
        double m_bid = 0;
        double m_pointsize = 0;
        string m_instrument = "";
        // double m_drate = 0;           

        public CreateOrders()
        {
        }

        public void ModelCreateOrder(Session Session, Model CurrentModel, string OrderType, int iAmount,
            double idRate, double idPointSize, int iAtMarket, string sBuySell)
        {
            m_o2gsession = Session.O2GSession;

            O2GTableManager tableManager = Session.O2GSession.getTableManager();
            O2GAccountRow account = GetAccount(tableManager, null);
            O2GOfferRow offer = GetOffer(tableManager, CurrentModel.Instrument);
            O2GTradeRow trade = GetTrade(tableManager, account.AccountID, offer.OfferID);

            switch (OrderType)
            {
                case "OpenMarketOrder":
                    CreateTrueMarketOrder(offer.OfferID, account.AccountID, iAmount, sBuySell);
                    break;
                case "CloseMarketOrder":
                    PrepareParamsAndCallTrueMarketCloseOrder(trade);
                    break;
                case "OpenLimitOrder":
                    CreateOpenLimitOrder(offer.OfferID, account.AccountID, iAmount, idRate, sBuySell);
                    break;
                case "CloseLimitOrder":
                    PrepareParamsAndCallLimitCloseOrder(trade);
                    break;
                case "OpenOpenOrder":
                    CreateMarketOrder(offer.OfferID, account.AccountID, iAmount, idRate, sBuySell);
                    break;
                case "CloseOpenOrder":
                    // @ here gl 
                    PrepareParamsAndCallMarketCloseOrder(trade);
                    break;
                case "OpenRangeOrder":
                    PrepareParamsAndCallRangeOrder(offer.OfferID, account.AccountID, iAmount, idPointSize, iAtMarket, sBuySell);
                    break;
                case "CloseRangeOrder":
                    PrepareParamsAndCallRangeCloseOrder(trade, iAtMarket);
                    break;
                default:
                    break;
            }
        }

        public void ModelCreateLimitOrder(Session Session, Model CurrentModel, string OrderType, int iAmount,
                    double idRate, double idPointSize, int iAtMarket, string sBuySell, double TakeProfit, double StopLoss)
        {
            m_o2gsession = Session.O2GSession;

            O2GTableManager tableManager = Session.O2GSession.getTableManager();
            O2GAccountRow account = GetAccount(tableManager, null);
            O2GOfferRow offer = GetOffer(tableManager, CurrentModel.Instrument);
            O2GTradeRow trade = GetTrade(tableManager, account.AccountID, offer.OfferID);

            switch (OrderType)
            {
                case "OpenMarketOrder":
                    CreateTrueMarketLimitOrder(offer.OfferID, account.AccountID, iAmount, sBuySell, TakeProfit, StopLoss);
                    break;
                default:
                    break;
            }
        }

        private static O2GAccountRow GetAccount(O2GTableManager tableManager, string sAccountID)
        {
            bool bHasAccount = false;
            O2GAccountRow account = null;
            O2GAccountsTable accountsTable = (O2GAccountsTable)tableManager.getTable(O2GTableType.Accounts);
            for (int i = 0; i < accountsTable.Count; i++)
            {
                account = accountsTable.getRow(i);
                string sAccountKind = account.AccountKind;
                if (sAccountKind.Equals("32") || sAccountKind.Equals("36"))
                {
                    if (account.MarginCallFlag.Equals("N"))
                    {
                        if (string.IsNullOrEmpty(sAccountID) || sAccountID.Equals(account.AccountID))
                        {
                            bHasAccount = true;
                            break;
                        }
                    }
                }
            }
            if (!bHasAccount)
            {
                return null;
            }
            else
            {
                return account;
            }
        }

        private static O2GOfferRow GetOffer(O2GTableManager tableManager, string sInstrument)
        {
            bool bHasOffer = false;
            O2GOfferRow offer = null;
            O2GOffersTable offersTable = (O2GOffersTable)tableManager.getTable(O2GTableType.Offers);
            for (int i = 0; i < offersTable.Count; i++)
            {
                offer = offersTable.getRow(i);
                if (offer.Instrument.Equals(sInstrument))
                {
                    if (offer.SubscriptionStatus.Equals("T"))
                    {
                        bHasOffer = true;
                        break;
                    }
                }
            }
            if (!bHasOffer)
            {
                return null;
            }
            else
            {
                return offer;
            }
        }

        public static O2GTradeRow GetTrade(O2GTableManager tableManager, string sAccountID, string sOfferID)
        {
            bool bHasTrade = false;
            O2GTradeRow trade = null;
            O2GTradesTable tradesTable = (O2GTradesTable)tableManager.getTable(O2GTableType.Trades);
            for (int i = 0; i < tradesTable.Count; i++)
            {
                trade = tradesTable.getRow(i);
                if (trade.AccountID.Equals(sAccountID) && trade.OfferID.Equals(sOfferID))
                {
                    bHasTrade = true;
                    break;
                }
            }
            if (!bHasTrade)
            {
                return null;
            }
            else
            {
                return trade;
            }
        }

        /* Prepare Params for Orders */
        public void PrepareParamsFromLoginRules(O2GLoginRules loginRules)
        {
            O2GResponseReaderFactory factory = m_o2gsession.getResponseReaderFactory();
            if (factory == null)
                return;
            // Gets first account from login.
            O2GResponse accountsResponse = loginRules.getTableRefreshResponse(O2GTableType.Accounts);
            O2GAccountsTableResponseReader accountsReader = factory.createAccountsTableReader(accountsResponse);
            O2GAccountRow account = accountsReader.getRow(0);
            // Store account id
            m_accountid = account.AccountID;
            // Store base iAmount
            m_baseamount = account.BaseUnitSize;
            // Get offers for eur/usd
            O2GResponse offerResponse = loginRules.getTableRefreshResponse(O2GTableType.Offers);
            O2GOffersTableResponseReader offersReader = factory.createOffersTableReader(offerResponse);
            for (int i = 0; i < offersReader.Count; i++)
            {
                O2GOfferRow offer = offersReader.getRow(i);
                if (string.Compare(offer.Instrument, m_instrument/*"EUR/USD"*/, true) == 0)
                {
                    m_offerid = offer.OfferID;
                    m_ask = offer.Ask;
                    m_bid = offer.Bid;
                    m_pointsize = offer.PointSize;
                    break;
                }
            }
        }

        public void SaveOrders(string sOfferID, string sAccountID, string sTradeID, int iAmount, string sBuySell)
        {
            using (XmlWriter m_savesettings = XmlWriter.Create(FileFolder(Application.ExecutablePath) + @"\Orders.xml"))
            {
                m_savesettings.WriteStartDocument();

                m_savesettings.WriteStartElement("Orders");

                m_savesettings.WriteElementString("OfferID", sOfferID);
                m_savesettings.WriteElementString("AccountID", sAccountID);
                m_savesettings.WriteElementString("TradeID", sTradeID);
                m_savesettings.WriteElementString("Amount", iAmount.ToString());
                m_savesettings.WriteElementString("BuySell", sBuySell);

                m_savesettings.WriteEndElement();
                m_savesettings.WriteEndDocument();
            }
        }

        public static string FileFolder(string FilePath)
        {
            DirectoryInfo dinfo = new DirectoryInfo(FilePath);
            string folderName = dinfo.Parent.FullName;
            return folderName;
        }


        /***************************************************************************************************************/
        /*****                                      TRUE MARKET ORDERS                                             *****/
        /***************************************************************************************************************/

        /* True Market Order */
        public void CreateTrueMarketOrder(string sOfferID, string sAccountID, int iAmount, string sBuySell)
        {

            try
            {
                O2GRequestFactory factory = m_o2gsession.getRequestFactory();
                if (factory == null)
                    return;
                O2GValueMap valuemap = factory.createValueMap();
                valuemap.setString(O2GRequestParamsEnum.Command, Constants.Commands.CreateOrder);
                valuemap.setString(O2GRequestParamsEnum.OrderType, Constants.Orders.TrueMarketOpen);
                valuemap.setString(O2GRequestParamsEnum.AccountID, sAccountID);            // The identifier of the account the order should be placed for.
                valuemap.setString(O2GRequestParamsEnum.OfferID, sOfferID);                // The identifier of the instrument the order should be placed for.
                valuemap.setString(O2GRequestParamsEnum.BuySell, sBuySell);                // The order direction: Constants.Sell for "Sell", Constants.Buy for "Buy".
                valuemap.setInt(O2GRequestParamsEnum.Amount, iAmount);                    // The quantity of the instrument to be bought or sold.
                valuemap.setString(O2GRequestParamsEnum.CustomID, "TrueMarketOrder");    // The custom identifier of the order.

                if (sBuySell == "Buy")
                {
                    valuemap.setString(O2GRequestParamsEnum.BuySell, Constants.Buy);
                }
                else
                {
                    valuemap.setString(O2GRequestParamsEnum.BuySell, Constants.Sell);
                }

                O2GRequest request = factory.createOrderRequest(valuemap);
                m_o2gsession.sendRequest(request);
            }
            catch
            {
            
            }
        }

        /* True Market Order */
        public void CreateTrueMarketLimitOrder(string sOfferID, string sAccountID, int iAmount, string sBuySell, double TakeProfit, double StopLoss)
        {
            try
            {
                O2GRequestFactory factory = m_o2gsession.getRequestFactory();
                if (factory == null)
                    return;
                O2GValueMap valuemap = factory.createValueMap();
                valuemap.setString(O2GRequestParamsEnum.Command, Constants.Commands.CreateOrder);
                valuemap.setString(O2GRequestParamsEnum.OrderType, Constants.Orders.TrueMarketOpen);
                valuemap.setString(O2GRequestParamsEnum.AccountID, sAccountID);            // The identifier of the account the order should be placed for.
                valuemap.setString(O2GRequestParamsEnum.OfferID, sOfferID);                // The identifier of the instrument the order should be placed for.
                valuemap.setString(O2GRequestParamsEnum.BuySell, sBuySell);                // The order direction: Constants.Sell for "Sell", Constants.Buy for "Buy".
                valuemap.setInt(O2GRequestParamsEnum.Amount, iAmount);                    // The quantity of the instrument to be bought or sold.
                valuemap.setString(O2GRequestParamsEnum.CustomID, "TrueMarketOrder");    // The custom identifier of the order.
                valuemap.setDouble(O2GRequestParamsEnum.RateLimit, TakeProfit);
                valuemap.setDouble(O2GRequestParamsEnum.RateStop, StopLoss);

                if (sBuySell == "Buy")
                {
                    valuemap.setString(O2GRequestParamsEnum.BuySell, Constants.Buy);
                }
                else
                {
                    valuemap.setString(O2GRequestParamsEnum.BuySell, Constants.Sell);
                }

                O2GRequest request = factory.createOrderRequest(valuemap);
                m_o2gsession.sendRequest(request);
            }
            catch
            {

            }
        }

        /* Close True Market Order */
        public void PrepareParamsAndCallTrueMarketCloseOrder(O2GTradeRow tradeRow)
        {
            //The order direcion of the close order must be opposite to the direction of the trade.
            string sTradeBuySell = tradeRow.BuySell;
            if (sTradeBuySell == Constants.Buy)                    // trade BuySell=Constants.Buy = order BuySell = Constants.Sell
                CreateTrueMarketCloseOrder(tradeRow.OfferID, tradeRow.AccountID, tradeRow.TradeID, tradeRow.Amount, Constants.Sell);
            else                                            // trade BuySell=Constants.Sell = order BuySell = Constants.Buy
                CreateTrueMarketCloseOrder(tradeRow.OfferID, tradeRow.AccountID, tradeRow.TradeID, tradeRow.Amount, Constants.Buy);
        }

        public void CreateTrueMarketCloseOrder(string sOfferID, string sAccountID, string sTradeID, int iAmount, string sBuySell)
        {
            O2GRequestFactory factory = m_o2gsession.getRequestFactory();
            if (factory == null)
                return;
            O2GValueMap valuemap = factory.createValueMap();
            valuemap.setString(O2GRequestParamsEnum.Command, Constants.Commands.CreateOrder);
            valuemap.setString(O2GRequestParamsEnum.OrderType, Constants.Orders.TrueMarketClose);
            valuemap.setString(O2GRequestParamsEnum.AccountID, sAccountID);                // The identifier of the account the order should be placed for.
            valuemap.setString(O2GRequestParamsEnum.OfferID, sOfferID);                    // The identifier of the instrument the order should be placed for.
            valuemap.setString(O2GRequestParamsEnum.NetQuantity, "Y");
            valuemap.setString(O2GRequestParamsEnum.BuySell, sBuySell);                    // The order direction: Constants.Buy for Buy, Constants.Sell for Sell. Must be opposite to the direction of the trade.
            valuemap.setString(O2GRequestParamsEnum.CustomID, "CloseTrueMarketOrder");  // The custom identifier of the order.

            O2GRequest request = factory.createOrderRequest(valuemap);
            m_o2gsession.sendRequest(request);            
         
            SaveOrders(sOfferID, sAccountID, sTradeID, iAmount, sBuySell);
        }
        /***************************************************************************************************************/




        /***************************************************************************************************************/
        /*****                                      LIMIT ORDERS                                                   *****/
        /***************************************************************************************************************/


        /* Open Limit Order */
        public void CreateOpenLimitOrder(string sOfferID, string sAccountID, int iAmount, double dRate, string sBuySell)
        {
            O2GRequestFactory factory = m_o2gsession.getRequestFactory();
            if (factory == null)
                return;
            O2GValueMap valuemap = factory.createValueMap();
            valuemap.setString(O2GRequestParamsEnum.Command, Constants.Commands.CreateOrder);
            valuemap.setString(O2GRequestParamsEnum.OrderType, Constants.Orders.OpenLimit);
            valuemap.setString(O2GRequestParamsEnum.AccountID, sAccountID);                // The identifier of the account the order should be placed for.
            valuemap.setString(O2GRequestParamsEnum.OfferID, sOfferID);                    // The identifier of the instrument the order should be placed for.
            valuemap.setString(O2GRequestParamsEnum.BuySell, sBuySell);                    // The order direction (use Constants.Buy for Buy, Constants.Sell for Sell)
            valuemap.setDouble(O2GRequestParamsEnum.Rate, dRate);                          // The dRate at which the order must be filled.
            valuemap.setInt(O2GRequestParamsEnum.Amount, iAmount);                         // The quantity of the instrument to be bought or sold.
            valuemap.setString(O2GRequestParamsEnum.CustomID, "OpenLimitOrder");         // The custom identifier of the order.

            O2GRequest request = factory.createOrderRequest(valuemap);
            m_o2gsession.sendRequest(request);
        }

        /* Close Limit Order */
        public void PrepareParamsAndCallLimitCloseOrder(O2GTradeRow tradeRow)
        {
            string sTradeBuySell = tradeRow.BuySell;
            // Close order is opposite to the trade
            bool bBuyOrder = false;
            if (sTradeBuySell == Constants.Sell)
                bBuyOrder = true;

            string sTradeOfferID = tradeRow.OfferID;
            double dCurrentRate = (bBuyOrder) ? m_ask : m_bid;

            CreateLimitCloseOrder(sTradeOfferID,
                            tradeRow.AccountID,
                            tradeRow.TradeID,
                            tradeRow.Amount,
                            dCurrentRate,
                            bBuyOrder ? Constants.Buy : Constants.Sell);
        }

        public void CreateLimitCloseOrder(string sOfferID, string sAccountID, string sTradeID, int iAmount, double dRate, string sBuySell)
        {
            O2GRequestFactory factory = m_o2gsession.getRequestFactory();
            if (factory == null)
                return;
            O2GValueMap valuemap = factory.createValueMap();
            valuemap.setString(O2GRequestParamsEnum.Command, Constants.Commands.CreateOrder);
            valuemap.setString(O2GRequestParamsEnum.OrderType, Constants.Orders.CloseLimit);
            valuemap.setString(O2GRequestParamsEnum.AccountID, sAccountID);        // The identifier of the account the order should be placed for
            valuemap.setString(O2GRequestParamsEnum.OfferID, sOfferID);            // The identifier of the instrument the order should be placed for
            valuemap.setString(O2GRequestParamsEnum.TradeID, sTradeID);            // The identifier of the trade to be closed
            valuemap.setString(O2GRequestParamsEnum.BuySell, sBuySell);            // The order direction (Constants.Buy for Buy, Constants.Sell for Sell). Must be opposite to the direction of the trade
            valuemap.setInt(O2GRequestParamsEnum.Amount, iAmount);                // The quantity of the instrument to be bought or sold. Must be = size of the position (Lot of the trade). Must be divisible by baseUnitSize
            valuemap.setDouble(O2GRequestParamsEnum.Rate, dRate);                // The dRate at which the order must be filled (< market for "Sell" order,  market for "Buy" order)
            valuemap.setString(O2GRequestParamsEnum.CustomID, "CloseMarketOrder");        // The custom identifier of the order
            O2GRequest request = factory.createOrderRequest(valuemap);
            m_o2gsession.sendRequest(request);

            SaveOrders(sOfferID, sAccountID, sTradeID, iAmount, sBuySell);
        }

        /***************************************************************************************************************/



        /***************************************************************************************************************/
        /*****                                      OPEN ORDERS                                                    *****/
        /***************************************************************************************************************/

        /* Open Order (Market Order) */
        public void CreateMarketOrder(string sOfferID, string sAccountID, int iAmount, double dRate, string sBuySell)
        {
            O2GRequestFactory factory = m_o2gsession.getRequestFactory();
            if (factory == null)
                return;
            O2GValueMap valuemap = factory.createValueMap();
            valuemap.setString(O2GRequestParamsEnum.Command, Constants.Commands.CreateOrder);
            valuemap.setString(O2GRequestParamsEnum.OrderType, Constants.Orders.MarketOpen);
            valuemap.setString(O2GRequestParamsEnum.AccountID, sAccountID);                // The identifier of the account the order should be placed for.
            valuemap.setString(O2GRequestParamsEnum.OfferID, sOfferID);                    // The identifier of the instrument the order should be placed for.
            valuemap.setString(O2GRequestParamsEnum.BuySell, sBuySell);                    // The order direction (use Constants.Buy for Buy, Constants.Sell for Sell)
            valuemap.setDouble(O2GRequestParamsEnum.Rate, dRate);                          // The dRate at which the order must be filled.
            valuemap.setInt(O2GRequestParamsEnum.Amount, iAmount);                         // The quantity of the instrument to be bought or sold.
            valuemap.setString(O2GRequestParamsEnum.CustomID, "OpenMarketOrder");         // The custom identifier of the order.

            O2GRequest request = factory.createOrderRequest(valuemap);
            m_o2gsession.sendRequest(request);
        }

        /* Close Open Order */
        public void PrepareParamsAndCallMarketCloseOrder(O2GTradeRow tradeRow)
        {
            string sTradeBuySell = tradeRow.BuySell;
            // Close order is opposite to the trade
            bool bBuyOrder = (sTradeBuySell == Constants.Sell);

            string sTradeOfferID = tradeRow.OfferID;
            double dRate = 0;
            // Ask price for Buy and Bid price for Sell
            dRate = bBuyOrder ? m_ask : m_bid;

            CreateMarketCloseOrder(sTradeOfferID,
                                   tradeRow.AccountID,
                                   tradeRow.TradeID,
                                   tradeRow.Amount,
                                   dRate,
                                   bBuyOrder ? Constants.Buy : Constants.Sell);
        }

        public void CreateMarketCloseOrder(string sOfferID, string sAccountID, string sTradeID, int iAmount, double dRate, string sBuySell)
        {
            O2GRequestFactory factory = m_o2gsession.getRequestFactory();
            if (factory == null)
                return;
            O2GValueMap valuemap = factory.createValueMap();
            valuemap.setString(O2GRequestParamsEnum.Command, Constants.Commands.CreateOrder);
            valuemap.setString(O2GRequestParamsEnum.OrderType, Constants.Orders.MarketClose);
            valuemap.setString(O2GRequestParamsEnum.AccountID, sAccountID);                // The identifier of the account the order should be placed for.
            valuemap.setString(O2GRequestParamsEnum.OfferID, sOfferID);                    // The identifier of the instrument the order should be placed for.
            valuemap.setString(O2GRequestParamsEnum.TradeID, sTradeID);                    // The identifier of the trade to be closed.
            valuemap.setString(O2GRequestParamsEnum.BuySell, sBuySell);                    // The order direction (Constants.Buy - for Buy, Constants.Sell - for Sell). Must be opposite to the direction of the trade.
            valuemap.setInt(O2GRequestParamsEnum.Amount, iAmount);                         // The quantity of the instrument to be bought or sold.  Must <= to the size of the position (Lot column of the trade).
            valuemap.setDouble(O2GRequestParamsEnum.Rate, dRate);                          // The dRate at which the order must be filled.
            valuemap.setString(O2GRequestParamsEnum.CustomID, "CloseMarketOrder");        // The custom identifier of the order.
            O2GRequest request = factory.createOrderRequest(valuemap);
            m_o2gsession.sendRequest(request);

            SaveOrders(sOfferID, sAccountID, sTradeID, iAmount, sBuySell);
        }
        /***************************************************************************************************************/



        /***************************************************************************************************************/
        /*****                                      RANGE ORDERS                                                    *****/
        /***************************************************************************************************************/

        /* Range Order with Params Preparation */
        public void PrepareParamsAndCallRangeOrder(string sOfferID, string sAccountID, int iAmount, double dPointSize, int iAtMarket, string sBuySell)
        {
            bool bBuy = (sBuySell == Constants.Buy);
            double dCurrentRate = bBuy ? m_ask : m_bid;
            double dRateMin = dCurrentRate - dPointSize * iAtMarket;
            double dRateMax = dCurrentRate + dPointSize * iAtMarket;
            CreateRangeOrder(sOfferID, sAccountID, iAmount, dRateMin, dRateMax, sBuySell);
        }

        public void CreateRangeOrder(string sOfferID, string sAccountID, int iAmount, double dRateMin, double dRateMax, string sBuySell)
        {
            O2GRequestFactory factory = m_o2gsession.getRequestFactory();
            if (factory == null)
                return;
            O2GValueMap valuemap = factory.createValueMap();
            valuemap.setString(O2GRequestParamsEnum.Command, Constants.Commands.CreateOrder);
            valuemap.setString(O2GRequestParamsEnum.OrderType, Constants.Orders.MarketOpenRange);
            valuemap.setString(O2GRequestParamsEnum.AccountID, sAccountID);       // The identifier of the account the order should be placed for.
            valuemap.setString(O2GRequestParamsEnum.OfferID, sOfferID);           // The identifier of the instrument the order should be placed for.
            valuemap.setString(O2GRequestParamsEnum.BuySell, sBuySell);           // The order direction (Constants.Buy for buy, Constants.Sell for sell).
            valuemap.setDouble(O2GRequestParamsEnum.RateMin, dRateMin);           // The minimum dRate at which the order can be filled.
            valuemap.setDouble(O2GRequestParamsEnum.RateMax, dRateMax);           // The maximum dRate at which the order can be filled.
            valuemap.setString(O2GRequestParamsEnum.CustomID, "OpenRangeOrder"); // The custom identifier of the order.
            valuemap.setInt(O2GRequestParamsEnum.Amount, iAmount);

            O2GRequest request = factory.createOrderRequest(valuemap);
            m_o2gsession.sendRequest(request);
        }

        /* Close Range Order */
        public void PrepareParamsAndCallRangeCloseOrder(O2GTradeRow tradeRow, int iAtMarket)
        {
            string sTradeBuySell = tradeRow.BuySell;
            // Close order is opposite to the trade
            bool bBuyOrder = (sTradeBuySell == Constants.Sell);

            // Get dRate, then calculate dRateMin and dRateMax
            string sTradeOfferID = tradeRow.OfferID;
            double dRate = 0;
            dRate = bBuyOrder ? m_ask : m_bid;

            double dPointSize = m_pointsize;
            double dRateMin = dRate - iAtMarket * dPointSize;
            double dRateMax = dRate + iAtMarket * dPointSize;

            CreateRangeCloseOrder(sTradeOfferID,
                     tradeRow.AccountID,
                     tradeRow.TradeID,
                     tradeRow.Amount,
                     dRateMin, dRateMax,
                     bBuyOrder ? Constants.Buy : Constants.Sell);
        }

        public void CreateRangeCloseOrder(string sOfferID, string sAccountID, string sTradeID, int iAmount, double dRateMin, double dRateMax, string sBuySell)
        {
            O2GRequestFactory factory = m_o2gsession.getRequestFactory();
            if (factory == null)
                return;
            O2GValueMap valuemap = factory.createValueMap();
            valuemap.setString(O2GRequestParamsEnum.Command, Constants.Commands.CreateOrder);
            valuemap.setString(O2GRequestParamsEnum.OrderType, Constants.Orders.MarketCloseRange);
            valuemap.setString(O2GRequestParamsEnum.AccountID, sAccountID);
            valuemap.setString(O2GRequestParamsEnum.OfferID, sOfferID);                // The identifier of the instrument the order should be placed for.
            valuemap.setString(O2GRequestParamsEnum.TradeID, sTradeID);                // The identifier of the trade to be closed.
            valuemap.setString(O2GRequestParamsEnum.BuySell, sBuySell);                // The order direction (Constants.Buy for Buy, Constants.Sell for Sell). Must be opposite to the direction of the trade.
            valuemap.setInt(O2GRequestParamsEnum.Amount, iAmount);                    // The quantity of the instrument to be bought or sold. Must be <= size of the position (Lot of the trade). Must be divisible by baseUnitSize.
            valuemap.setDouble(O2GRequestParamsEnum.RateMin, dRateMin);                // The minimum dRate at which the order can be filled.
            valuemap.setDouble(O2GRequestParamsEnum.RateMax, dRateMax);                // The maximum dRate at which the order can be filled.
            valuemap.setString(O2GRequestParamsEnum.CustomID, "CloseRangeOrder");    // The custom identifier of the order.
            O2GRequest request = factory.createOrderRequest(valuemap);
            m_o2gsession.sendRequest(request);

            SaveOrders(sOfferID, sAccountID, sTradeID, iAmount, sBuySell);
        }

        /***************************************************************************************************************/
    }
}

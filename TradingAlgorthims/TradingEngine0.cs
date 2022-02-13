using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleNahedTest
{
    public static class TradingEngine0
    {
        public static int TradeCount = 0;
        public static double TradeSeconds = 0;


        internal static int askMin = Global.MAX_PRICE;
        internal static int askMax = Global.MIN_PRICE;

        internal static int bidMax = Global.MIN_PRICE;
        internal static int bidMin = Global.MAX_PRICE;


        public static Dictionary<string, LinkedListNode<Order>> OrdersBookmarks = new Dictionary<string, LinkedListNode<Order>>();
        public static LinkedList<Order>[] BuyOrdersBook = new LinkedList<Order>[Global.MAX_PRICE + 1];
        public static LinkedList<Order>[] SellOrdersBook = new LinkedList<Order>[Global.MAX_PRICE + 1];


        public static (int TradeCount, double TradeSeconds) RunExample(string[] commands)
        {
            ExecuteCommands(commands);
            return (TradeCount, TradeSeconds);
        }
        public static void ExecuteCommands(string[] commands)
        {
            Reset();
            Random random = new Random();
            DateTime strDate = DateTime.Now;
            foreach (string cmd in commands)
            {
                ExecuteCommand(cmd);
            }
            DateTime endDate = DateTime.Now;
            TimeSpan duration = endDate - strDate;
            TradeSeconds = duration.TotalSeconds;

        }


        public static void ExecuteCommand(string command)
        {
            string[] row = command.Split(' ');

            switch (row[0])
            {
                case Global.CMD_BUY:
                    ushort price0 = Convert.ToUInt16(row[2]);
                    if (bidMax < price0)
                    {
                        //bidMax holds the maximum price of buy orders.
                        bidMax = price0;
                    }
                    if (bidMin > price0)
                    {
                        //bidMax holds the maximum price of buy orders.
                        bidMin = price0;
                    }
                    AddBuyOrder(row[4], row[0], row[1], price0, Convert.ToUInt16(row[3])); break;
                case Global.CMD_SELL:
                    ushort price1 = Convert.ToUInt16(row[2]);
                    if (askMin > price1)
                    {  //askMin holds the lowest  price of sell orders.
                        askMin = price1;
                    }
                    if (askMax < price1)
                    {  //askMin holds the lowest  price of sell orders.
                        askMax = price1;
                    }
                    AddSellOrder(row[4], row[0], row[1], price1, Convert.ToUInt16(row[3])); break;
                case Global.CMD_MODIFY:
                    ModifyOrder(row[1], row[2], Convert.ToUInt16(row[3]), int.Parse(row[4])); break;
                case Global.CMD_CANCEL:
                    CancelOrder(row[1]); break;
                case Global.CMD_PRINT:
                    Print(); break;
                default:
                    Console.WriteLine("Testing..."); break;
            }
        }

        public static void AddBuyOrder(string orderId, string OrderSide, string orderType, ushort price, int quantity)
        {
            Order order = new Order(orderId, OrderSide, orderType, price, quantity);
            LinkedList<Order> _orders;
            LinkedListNode<Order> OrderNode;
            // look for the ones that are higher than the limit price, BuyOrdersBook is sorted in desc order 
            // askMin holds the lowest  price of sell orders.
            if (order.Price >= askMin)
            {
                while (true)
                {
                    if (askMin > order.Price || askMin > askMax || order.Quantity == 0)
                    {
                        break;
                    }
                    if (SellOrdersBook[askMin] == null || SellOrdersBook[askMin].Count == 0)
                    {
                        askMin++;
                        continue;
                    }
                    _orders = SellOrdersBook[askMin];
                    OrderNode = _orders.First;
                    while (OrderNode != null && order.Quantity > 0)
                    {
                        if (order.Quantity <= 0)
                        {
                            break;
                        }
                        int minQuantity = Math.Min(order.Quantity, OrderNode.Value.Quantity);
                        TradeCount++;
                        Global.PrintToScreen(OrderNode.Value.OrderId, OrderNode.Value.Price, minQuantity, order.OrderId, order.Price, minQuantity);

                        if (OrderNode.Value.Quantity <= order.Quantity)
                        { 
                            _orders.Remove(OrderNode);
                            OrdersBookmarks.Remove(OrderNode.Value.OrderId);
                            OrderNode = _orders.First; 
                            if (_orders.Count == 0)
                            {
                                SellOrdersBook[askMin] = null;
                            }
                        }
                        else
                        {
                            OrderNode.Value.Quantity -= order.Quantity;
                        }
                        order.Quantity -= minQuantity;
                    }


                }


                if (bidMax < order.Price)
                {
                    bidMax = order.Price;
                }

                if (bidMin > order.Price)
                {
                    bidMin = order.Price;
                }
            }
            if (order.Quantity > 0 && order.OrderType == Global.CMD_GOODFORDAY)
            {
                _orders = BuyOrdersBook[order.Price];
                if (_orders == null || _orders.Count == 0)
                {
                    _orders = new LinkedList<Order>();
                    OrderNode = _orders.AddLast(order);
                    BuyOrdersBook[order.Price] = _orders;
                }
                else
                {
                    OrderNode = _orders.AddLast(order);

                }
                BookMark(  OrderNode);
            }
        }

        public static void AddSellOrder(string orderId, string OrderSide, string orderType, ushort price, int quantity)
        {
            Order order = new Order(orderId, OrderSide, orderType, price, quantity);
            LinkedList<Order> _orders;
            LinkedListNode<Order> OrderNode;
            // look for the ones that are higher than the limit price, BuyOrdersBook is sorted in desc order 
            // bidMax holds the maximum price of buy orders.
            if (price <= bidMax)
            {

                while (true)
                {
                    if (price > bidMax || bidMin > bidMax || order.Quantity == 0)
                    {
                        break;
                    }
                    if (BuyOrdersBook[bidMax] == null || BuyOrdersBook[bidMax].Count == 0)
                    {
                        bidMax--;
                        continue; ;
                    }

                    _orders = BuyOrdersBook[bidMax];
                    OrderNode = _orders.First;
                    while (OrderNode != null && order.Quantity > 0)
                    {
                        if (order.Quantity <= 0)
                        {
                            break;
                        }
                        int minQuantity = Math.Min(order.Quantity, OrderNode.Value.Quantity);
                        TradeCount++;
                        Global.PrintToScreen(OrderNode.Value.OrderId, OrderNode.Value.Price, minQuantity, order.OrderId, order.Price, minQuantity);

                        if (OrderNode.Value.Quantity <= order.Quantity)
                        { 
                            _orders.Remove(OrderNode);
                            OrdersBookmarks.Remove(OrderNode.Value.OrderId);
                            OrderNode = _orders.First;

                            if (_orders.Count == 0)
                            {
                                SellOrdersBook[askMin] = null;
                            }
                        }
                        else
                        {
                            OrderNode.Value.Quantity -= order.Quantity;
                        }
                        order.Quantity -= minQuantity;
                    }


                }


                if (askMin > order.Price)
                {
                    askMin = order.Price;
                }
                if (askMax < order.Price)
                {
                    askMax = order.Price;
                }

            }
            if (order.Quantity > 0 && order.OrderType == Global.CMD_GOODFORDAY)
            {
                //int aaa = Marshal.SizeOf(order);
                _orders = SellOrdersBook[order.Price];
                if (_orders == null || _orders.Count == 0)
                {
                    _orders = new LinkedList<Order>();
                    OrderNode = _orders.AddLast(order);
                    SellOrdersBook[order.Price] = _orders;
                }
                else
                {
                    OrderNode = _orders.AddLast(order);

                }
                BookMark(  OrderNode);
            }
        }

        private static void BookMark( LinkedListNode<Order> order)
        {
            //int aaa = Marshal.SizeOf(order);
            if (OrdersBookmarks.ContainsKey(order.Value.OrderId)) return;
            OrdersBookmarks.Add(order.Value.OrderId, order);
        }

        public static void ModifyOrder(string orderId, string orderSide, ushort newPrice, int newQty)
        { 
            if (!OrdersBookmarks.ContainsKey(orderId)) return;
            var _order = OrdersBookmarks[orderId];
            if (_order.Value.OrderType == Global.CMD_INSERTORCANCEL) return;
            if (_order.Value.OrderSide != orderSide || _order.Value.Price != newPrice || _order.Value.Quantity != newQty)
            { 
                LinkedList<Order> _orders;
                if (_order.Value.OrderSide == Global.CMD_BUY)
                {
                    _orders = BuyOrdersBook[_order.Value.Price];
                    _orders.Remove(_order);

                }
                else if (_order.Value.OrderSide == Global.CMD_SELL)
                {
                    _orders = SellOrdersBook[_order.Value.Price];
                    _orders.Remove(_order);
                }
                OrdersBookmarks.Remove(orderId);

                if (orderSide == Global.CMD_BUY)
                {

                    AddBuyOrder(orderId, orderSide, Global.CMD_GOODFORDAY, newPrice, newQty);
                }
                else if (orderSide == Global.CMD_SELL)
                {

                    AddSellOrder(orderId, orderSide, Global.CMD_GOODFORDAY, newPrice, newQty);
                }
            }
        }

        public static void CancelOrder(string orderId)
        {
            if (!OrdersBookmarks.ContainsKey(orderId)) return;
            var _order = OrdersBookmarks[orderId]; 
            LinkedList<Order> _orders;
            if (_order.Value.OrderSide == Global.CMD_BUY)
            {
                _orders = BuyOrdersBook[_order.Value.Price];
                _orders.Remove(_order);

            }
            else if (_order.Value.OrderSide == Global.CMD_SELL)
            {
                _orders = SellOrdersBook[_order.Value.Price];
                _orders.Remove(_order);
            }
            OrdersBookmarks.Remove(orderId);
        }

        public static void Print()
        {
            Global.PrintLine($"SELL:");

            for (int priceEntry = BuyOrdersBook.Count() - 1; priceEntry >= 0; priceEntry--)
            {

                if (SellOrdersBook[priceEntry] == null || SellOrdersBook[priceEntry].Count == 0)
                {
                    continue;
                }
                var asks_sum = SellOrdersBook[priceEntry].Sum(x => x.Quantity);
                Global.PrintLine($"{priceEntry} {asks_sum}");

            }

            Global.PrintLine($"BUY:");
            for (int priceEntry = BuyOrdersBook.Count() - 1; priceEntry >= 0; priceEntry--)
            {

                if (BuyOrdersBook[priceEntry] == null || BuyOrdersBook[priceEntry].Count == 0)
                {
                    continue;
                }
                var asks_sum = BuyOrdersBook[priceEntry].Sum(x => x.Quantity);
                Global.PrintLine($"{priceEntry} {asks_sum}");

            }

        }

        public static void Reset()
        {
            askMin = Global.MAX_PRICE;
            askMax = Global.MIN_PRICE;
            bidMax = Global.MIN_PRICE;
            bidMin = Global.MAX_PRICE;
            TradeCount = 0;
            TradeSeconds = 0;
            Global.DebugOutput.Clear();
            OrdersBookmarks = new Dictionary<string, LinkedListNode<Order>>();
            BuyOrdersBook = new LinkedList<Order>[Global.MAX_PRICE + 1];
            for (int size = Global.MAX_PRICE; size > 0; size--)
                BuyOrdersBook[size] = new LinkedList<Order>();

            SellOrdersBook = new LinkedList<Order>[Global.MAX_PRICE + 1];
            for (int size = Global.MAX_PRICE; size > 0; size--)
                SellOrdersBook[size] = new LinkedList<Order>();
        }
        public static void Clean()
        {

            Global.DebugOutput.Clear();
            OrdersBookmarks.Clear();
            Array.Clear(BuyOrdersBook, 0, BuyOrdersBook.Length);
            Array.Clear(SellOrdersBook, 0, SellOrdersBook.Length);

        }


    }




}

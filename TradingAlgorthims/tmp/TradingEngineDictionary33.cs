using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleNahedTest
{

    public class DescendingComparer<T> : IComparer<T> where T : IComparable<T>
    {
        public int Compare(T x, T y)
        {
            return y.CompareTo(x);
        }
    }
    public static class TradingEngineDictionary33
    {
        
        internal static int askMin = Global.MAX_PRICE;
        internal static int askMax = Global.MIN_PRICE;

        internal static int bidMax = Global.MIN_PRICE;
        internal static int bidMin = Global.MAX_PRICE;
        public static int TradeCount = 0;
        public static double TradeSeconds = 0;
       
        public static Dictionary<string, Order> OrdersBookmarks = new Dictionary<string, Order>();

        public class OrderBook
        {
            public SortedDictionary<long, Order> BuyOrdersBook = new SortedDictionary<long, Order>();
            public SortedDictionary<long, Order> SellOrdersBook = new SortedDictionary<long, Order>();
            public OrderBook()
            {

            }
        }
        public static OrderBook[] orderBook = new OrderBook[Global.MAX_PRICE + 1];
        

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
            SortedDictionary<long, Order> _orders;
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
                    if (orderBook[askMin] == null || orderBook[askMin].SellOrdersBook == null || orderBook[askMin].SellOrdersBook.Count == 0)
                    {
                        askMin++;
                        continue;
                    }


                    _orders = orderBook[askMin].SellOrdersBook;
                    for (int i = 0; i < _orders.Count; i++)
                    {
                        if (order.Quantity <= 0)
                        {
                            break;
                        }

                        var orderItem = _orders.ElementAt(i);
                        var buysOrder = orderItem.Value;
                        TradeCount++;
                        int minQuantity = Math.Min(order.Quantity, buysOrder.Quantity);
                       Global. PrintToScreen(buysOrder.OrderId, buysOrder.Price, minQuantity, order.OrderId, order.Price, minQuantity);
                        if (order.Quantity >= buysOrder.Quantity)
                        {
                            i--;
                            _orders.Remove(buysOrder.TimeTicks);
                            OrdersBookmarks.Remove(buysOrder.OrderId);

                        }
                        else
                        {
                            buysOrder.Quantity -= minQuantity;
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
                    //bidMax holds the maximum price of buy orders.
                    bidMin = order.Price;
                }


            }
            if (order.Quantity > 0 && order.OrderType == Global.CMD_GOODFORDAY)
            {
                orderBook[order.Price].BuyOrdersBook.Add(order.TimeTicks, order);
                BookMark(ref order);
            }
        }

        public static void AddSellOrder(string orderId, string OrderSide, string orderType, ushort price, int quantity)
        {
            Order order = new Order(orderId, OrderSide, orderType, price, quantity);
            SortedDictionary<long, Order> _orders;
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
                    if (orderBook[askMin] == null || orderBook[bidMax].BuyOrdersBook == null || orderBook[bidMax].BuyOrdersBook.Count == 0)
                    {
                        bidMax--;
                        continue; ;
                    }


                    _orders = orderBook[bidMax].BuyOrdersBook;
                    for (int i = 0; i < _orders.Count; i++)
                    {
                        if (order.Quantity <= 0)
                        {
                            break;
                        }

                        var orderItem = _orders.ElementAt(i);
                        var buysOrder = orderItem.Value;
                        TradeCount++;
                        int minQuantity = Math.Min(order.Quantity, buysOrder.Quantity);
                       Global. PrintToScreen(buysOrder.OrderId, buysOrder.Price, minQuantity, order.OrderId, order.Price, minQuantity);
                        if (order.Quantity >= buysOrder.Quantity)
                        {
                            i--;
                            _orders.Remove(buysOrder.TimeTicks);
                            OrdersBookmarks.Remove(buysOrder.OrderId);

                        }
                        else
                        {
                            buysOrder.Quantity -= minQuantity;
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
                orderBook[order.Price].SellOrdersBook.Add(order.TimeTicks, order);
                BookMark(ref order);
            }

        }

        private static void BookMark(ref Order order)
        {
            if (OrdersBookmarks.ContainsKey(order.OrderId)) return;
            OrdersBookmarks.Add(order.OrderId, order);
        }

        public static void ModifyOrder(string orderId, string orderSide, ushort newPrice, int newQty)
        {

            if (!OrdersBookmarks.ContainsKey(orderId)) return;
            var _old_order = OrdersBookmarks[orderId];
            if (_old_order.OrderType == Global.CMD_INSERTORCANCEL) return;
            if (_old_order.OrderSide != orderSide || _old_order.Price != newPrice || _old_order.Quantity != newQty)
            {
                Order _order;
                SortedDictionary<long, Order> _orders;

                if (_old_order.OrderSide == Global.CMD_BUY)
                {
                    _orders = orderBook[_old_order.Price].BuyOrdersBook;
                    if (_orders.TryGetValue(_old_order.TimeTicks, out _order))
                    {
                        _orders.Remove(_old_order.TimeTicks);
                        if (_orders.Count == 0)
                        {
                            //orderBook[_old_order.Price].BuyOrdersBook = null;
                        }

                    }
                }
                else if (_old_order.OrderSide == Global.CMD_SELL)
                {
                    _orders = orderBook[_old_order.Price].SellOrdersBook;
                    if (_orders.TryGetValue(_old_order.TimeTicks, out _order))
                    {
                        _orders.Remove(_old_order.TimeTicks);

                    }
                }

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
            SortedDictionary<long, Order> _orders;
            if (_order.OrderSide == Global.CMD_BUY)
            {
                _orders = orderBook[_order.Price].BuyOrdersBook;
                if (_orders.TryGetValue(_order.TimeTicks, out _order))
                {
                    _orders.Remove(_order.TimeTicks);

                }

            }
            else if (_order.OrderSide == Global.CMD_SELL)
            {
                _orders = orderBook[_order.Price].SellOrdersBook;
                if (_orders.TryGetValue(_order.TimeTicks, out _order))
                {
                    _orders.Remove(_order.TimeTicks);

                }

            }
            OrdersBookmarks.Remove(orderId);
        }

        public static void Print()
        {
            Global.PrintLine($"SELL:"); 
            //foreach (var priceEntry in SellOrdersBook.Where(x => x.Value.Count > 0).OrderByDescending(p => p.Key))
            //{
            //    var asks_sum = priceEntry.Value.Sum(x => x.Value.Quantity);
            //    Global.DebugOutput.Add($"{priceEntry.Key} {asks_sum}");
            //    Console.WriteLine($"{priceEntry.Key} {asks_sum}");

            //}
            //Global.DebugOutput.Add($"BUY:");
            //Console.WriteLine("BUY:");
            //foreach (var priceEntry in BuyOrdersBook.Where(x => x.Value.Count > 0).OrderByDescending(p => p.Key))
            //{
            //    var bids_sum = priceEntry.Value.Sum(x => x.Value.Quantity);
            //    Global.DebugOutput.Add($"{priceEntry.Key} {bids_sum}");
            //    Console.WriteLine($"{priceEntry.Key} {bids_sum}");

            //}
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
            OrdersBookmarks = new Dictionary<string, Order>();

            for (int size = Global.MAX_PRICE; size >= 0; size--)
                orderBook[size] = new OrderBook();

        }
        public static void Clean()
        {
            Global.DebugOutput.Clear();
            OrdersBookmarks.Clear();
            for (int size = Global.MAX_PRICE; size > 0; size--)
                orderBook[size] = null;
            GC.Collect();
        }
    }

     


}

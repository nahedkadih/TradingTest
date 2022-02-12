using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleNahedTest
{
    public static class TradingEngineDictionary2
    {
        
        static int curOrderID = 0;

        internal static int askMin = Global.MAX_PRICE;
        internal static int askMax = Global.MIN_PRICE;

        internal static int bidMax = Global.MIN_PRICE;
        internal static int bidMin = Global.MAX_PRICE;

        public static int TradeCount = 0;
        public static double TradeSeconds = 0;
       
        public static Dictionary<string, Order> OrdersBookmarks = new Dictionary<string, Order>();

        public static Queue<Order>[] BuyOrdersBook = new Queue<Order>[Global.MAX_PRICE + 1];
        public static Queue<Order>[] SellOrdersBook = new Queue<Order>[Global.MAX_PRICE + 1];

      
 
       
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
            Queue<Order> _orders;
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
                    while (_orders.Count > 0)
                    {
                        Order SellOrder = _orders.Peek(); // removed
                        TradeCount++;
                        int minQuantity = Math.Min(order.Quantity, SellOrder.Quantity);
                        Global.PrintToScreen(SellOrder.OrderId, SellOrder.Price, minQuantity, order.OrderId, order.Price, minQuantity);
                        if (order.Quantity >= SellOrder.Quantity)
                        {
                            _orders.Dequeue();
                            OrdersBookmarks.Remove(SellOrder.OrderId);
                            if (_orders.Count == 0)
                            {
                                SellOrdersBook[askMin] = null;
                            }
                        }
                        else
                        { 
                            _orders.Peek().Quantity -= minQuantity;
                        }
                        order.Quantity -= minQuantity;
                        if (order.Quantity <= 0)
                        {
                            break;
                        }
                    }

                    
                }

                curOrderID++;
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
                _orders = BuyOrdersBook[order.Price];
                if (_orders == null || _orders.Count == 0)
                {
                    _orders = new Queue<Order>();
                    _orders.Enqueue(order);
                    BuyOrdersBook[order.Price] = _orders;
                }
                else
                {
                    _orders.Enqueue(order);

                }
                BookMark(ref order);
            }
        }

        public static void AddSellOrder(string orderId, string OrderSide, string orderType, ushort price, int quantity)
        {
            Order order = new Order(orderId, OrderSide, orderType, price, quantity);
            Queue<Order> _orders;
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

                    while (_orders.Count > 0)
                    {
                        Order buysOrder = _orders.Peek();  
                        TradeCount++;
                        int minQuantity = Math.Min(order.Quantity, buysOrder.Quantity);
                        Global. PrintToScreen(buysOrder.OrderId, buysOrder.Price, minQuantity, order.OrderId, order.Price, minQuantity);
                        if (order.Quantity >= buysOrder.Quantity)
                        {
                            _orders.Dequeue();
                            OrdersBookmarks.Remove(buysOrder.OrderId);
                            if (_orders.Count == 0)
                            {
                                BuyOrdersBook[bidMax] = null;
                            }
                        }
                        else
                        {
                            _orders.Peek().Quantity -= minQuantity;
                        }
                        order.Quantity -= minQuantity;

                        if (order.Quantity <= 0)
                        {
                            break;
                        }

                    }

                }

                curOrderID++;
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
                _orders = SellOrdersBook[order.Price];
                if (_orders == null || _orders.Count == 0)
                {
                    _orders = new Queue<Order>();
                    _orders.Enqueue(order);
                    SellOrdersBook[order.Price] = _orders;
                }
                else
                {
                    _orders.Enqueue(order);

                }
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
                Queue<Order>[] _currentBook = null;
                Queue<Order> _orders;


                if (_old_order.OrderSide == Global.CMD_BUY)
                {
                    _currentBook = BuyOrdersBook;
                    _orders = _currentBook[_old_order.Price];
                    if (_orders != null)
                    {
                        var doomed = _orders.FirstOrDefault(x => x.OrderId == orderId);
                        if (doomed != null)
                            _orders.Remove(doomed);
                    }

                }
                else if (_old_order.OrderSide == Global.CMD_SELL)
                {
                    _currentBook = SellOrdersBook;
                    _orders = _currentBook[_old_order.Price];
                    if (_orders != null)
                    {
                        var doomed = _orders.FirstOrDefault(x => x.OrderId == orderId);
                        if (doomed != null)
                            _orders.Remove(doomed);
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
            Queue<Order> _orders;
            if (_order.OrderSide == Global.CMD_BUY)
            {
                _orders = BuyOrdersBook[_order.Price];
                if (_orders != null)
                {
                     if (_orders.Contains(_order))
                        _orders.Remove(_order);
                }
            }
            else if (_order.OrderSide == Global.CMD_SELL)
            {
                _orders = SellOrdersBook[_order.Price];
                if (_orders != null)
                {
                    if (_orders.Contains(_order))
                        _orders.Remove(_order);
                    //var doomed = _orders.FirstOrDefault(x => x.OrderId == orderId);
                    //if (doomed != null)
                    //    _orders.Remove(doomed);
                }
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
            OrdersBookmarks = new Dictionary<string, Order>();

            for (int size = Global.MAX_PRICE; size >= 0; size--)
                BuyOrdersBook[size] = new Queue<Order>();

            for (int size = Global.MAX_PRICE; size >= 0; size--)
                SellOrdersBook[size] = new Queue<Order>();

        }
        public static void Clean()
        {

            Global.DebugOutput.Clear();
            OrdersBookmarks.Clear();
            Array.Clear(BuyOrdersBook, 0, BuyOrdersBook.Length);
            Array.Clear(SellOrdersBook, 0, SellOrdersBook.Length);
        }
  
        //public static void Remove<T>(this Queue<T> queue, T itemToRemove) where T : class
        //{
        //    var list = queue.ToList(); //Needs to be copy, so we can clear the queue
        //    queue.Clear();
        //    foreach (var item in list)
        //    {
        //        if (item == itemToRemove)
        //            continue;

        //        queue.Enqueue(item);
        //    }
        //}

        public static void Remove<T>(this Queue<T> queue,   T itemToRemove) where T : class
        {
            Contract.Requires(queue != null);
            Contract.Requires(itemToRemove != null); 
            var icount = queue.Count();
            var index = 0;
            T dequed; 
            while (index < icount)
            { 
                dequed = queue.Dequeue();
                index++;
                if (dequed== itemToRemove)
                {
                    break;
                } 
                queue.Enqueue(dequed);
            } 
            while (index < icount)
            {
                index++;
                dequed = queue.Dequeue(); 
                queue.Enqueue(dequed);
            } 
        }
    }


}

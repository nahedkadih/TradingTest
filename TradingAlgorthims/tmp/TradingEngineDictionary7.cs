using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleNahedTest
{
   

    public static class TradingEngineDictionary7
    {
        public static int TradeCount = 0; 
        public static double TradeSeconds = 0;
           
        public static Dictionary<string, Order> OrdersBookmarks =   new Dictionary<string, Order>();
        public static SortedDictionary<int, SortedDictionary<long, Order>> BuyOrdersBook = new SortedDictionary<int, SortedDictionary<long, Order>>(new DescendingComparer<int>());
        public static SortedDictionary<int, SortedDictionary<long, Order>> SellOrdersBook = new SortedDictionary<int, SortedDictionary<long, Order>>();

    
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
                    AddBuyOrder(row[4], row[0], row[1], Convert.ToUInt16(row[2]), Convert.ToUInt16(row[3])); break;
                case Global.CMD_SELL:
                    AddSellOrder(row[4], row[0], row[1], Convert.ToUInt16(row[2]), Convert.ToUInt16(row[3])); break;
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
            Order order = new Order(orderId, OrderSide,  orderType, price, quantity);
            SortedDictionary<long, Order> _orders;

            var lowerPricesLists = SellOrdersBook.Where(x => x.Key <= price) ;
            foreach ( var OrdersListOfPriceLimit in lowerPricesLists)
            {
                if (order.Quantity <= 0)
                {
                    break;
                }
                var sellingQtys = OrdersListOfPriceLimit.Value ;
                for (int i = 0; i < sellingQtys.Count; i++)
                {
                    if (order.Quantity <= 0)
                    {
                        break;
                    }
                    
                    var orderItem = sellingQtys.ElementAt(i);
                    var sellingOrder = orderItem.Value;
                    int minQuantity = Math.Min(order.Quantity, sellingOrder.Quantity);
                    TradeCount++;
                    Global. PrintToScreen(sellingOrder.OrderId, sellingOrder.Price, minQuantity, order.OrderId, order.Price, minQuantity);


                    if (order.Quantity >= sellingOrder.Quantity)
                    {
                        sellingQtys.Remove(sellingOrder.TimeTicks);
                        i--;
                        OrdersBookmarks.Remove(sellingOrder.OrderId);
                        if (sellingQtys.Count == 0)
                        {
                           // SellOrdersBook.Remove(price);
                        }
                    }
                    else
                    {
                        sellingOrder.Quantity -= minQuantity;
                    }
                    order.Quantity -= minQuantity; 
                }
            }
            if (order.Quantity>0 && order.OrderType == Global.CMD_GOODFORDAY)
            {
                if (!BuyOrdersBook.TryGetValue(order.Price, out _orders))
                {
                    _orders = new SortedDictionary<long, Order>();
                    _orders.Add(order.TimeTicks, order);
                    BuyOrdersBook.Add(order.Price, _orders);
                }
                else
                {
                    if (!_orders.ContainsKey(order.TimeTicks))
                    {
                        _orders.Add(order.TimeTicks, order);
                    }
                  
                }
                BookMark(ref order);
            }
                
        }
        public static void AddSellOrder(string orderId, string OrderSide, string orderType, ushort price, int quantity)
        {
            Order order = new Order(orderId, OrderSide, orderType, price, quantity);
            SortedDictionary<long, Order> _orders;

            var higherPricesLists = BuyOrdersBook.Where(x => x.Key >= price).ToList();
            foreach (var OrdersListOfPriceLimit in higherPricesLists)
            {
                if (order.Quantity <= 0)
                {
                    break;
                }

                var buysQtys = OrdersListOfPriceLimit.Value;
                
                for (int i = 0; i < buysQtys.Count; i++)
                {
                    if (order.Quantity <= 0)
                    {
                        break;
                    }
                    
                    var orderItem = buysQtys.ElementAt(i);
                    var buysOrder = orderItem.Value;
                    TradeCount++;
                    int minQuantity = Math.Min(order.Quantity, buysOrder.Quantity);
                   Global. PrintToScreen(buysOrder.OrderId, buysOrder.Price, minQuantity, order.OrderId, order.Price, minQuantity);
                     
                    if (order.Quantity >= buysOrder.Quantity)
                    {
                        buysQtys.Remove(buysOrder.TimeTicks);
                        i--;
                        OrdersBookmarks.Remove(buysOrder.OrderId);
                        if (buysQtys.Count == 0)
                        {
                             //BuyOrdersBook.Remove(price);
                        }
                    }
                    else
                    {
                        buysOrder.Quantity -= minQuantity;
                    }
                    order.Quantity -= minQuantity; 
                } 
            }
            if (order.Quantity > 0 && order.OrderType == Global.CMD_GOODFORDAY)
            {
                if (!SellOrdersBook.TryGetValue(order.Price, out _orders))
                {
                    _orders = new SortedDictionary<long, Order>();
                    _orders.Add(order.TimeTicks, order);
                    SellOrdersBook.Add(order.Price, _orders);
                }
                else
                {
                    if (!_orders.ContainsKey(order.TimeTicks))
                    {
                        _orders.Add(order.TimeTicks, order);
                    }
                    
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
                SortedDictionary<int, SortedDictionary<long, Order>> _currentBook = null;
                SortedDictionary<long, Order> _orders;
                if (_old_order.OrderSide == Global.CMD_BUY)
                {
                    _currentBook = BuyOrdersBook;
                    if (_currentBook.TryGetValue(_old_order.Price, out _orders))
                    {
                        _orders.Remove(_old_order.TimeTicks);
                        if (_orders.Count == 0)
                        {
                            //_currentBook.Remove(_old_order.Price);
                        }

                    } 
                }
                else if (_old_order.OrderSide == Global.CMD_SELL)
                {
                    _currentBook = SellOrdersBook; 
                    if (_currentBook.TryGetValue(_old_order.Price, out _orders))
                    {
                        _orders.Remove(_old_order.TimeTicks);
                        if (_orders.Count == 0)
                        {
                            //_currentBook.Remove(_old_order.Price);
                        }
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
                if (BuyOrdersBook.TryGetValue(_order.Price, out _orders))
                {
                    _orders.Remove(_order.TimeTicks);
                }
            }
            else if (_order.OrderSide == Global.CMD_SELL)
            {
                if (SellOrdersBook.TryGetValue(_order.Price, out _orders))
                {
                    _orders.Remove(_order.TimeTicks);
                }
            } 
            OrdersBookmarks.Remove(orderId);
        }

        public static void Print()
        {
            Global.PrintLine($"SELL:");
           
            foreach (var priceEntry in SellOrdersBook.Where(x => x.Value.Count > 0).OrderByDescending(p => p.Key))
            {
                var asks_sum = priceEntry.Value.Sum(x => x.Value.Quantity);
                Global.PrintLine($"{priceEntry.Key} {asks_sum}");
               

            }
            Global.PrintLine($"BUY:");
           
            foreach (var priceEntry in BuyOrdersBook.Where(x=>x.Value.Count>0).OrderByDescending(p => p.Key))
            {
                var bids_sum = priceEntry.Value.Sum(x => x.Value.Quantity);
                Global.PrintLine($"{priceEntry.Key} {bids_sum}"); 
            }
            //var buys = (from list in BuyOrdersBook.Values
            //              from s in list.Orders
            //              select s).Distinct().ToList();

            //var sells = (from list in SellOrdersBook.Values
            //            from s in list.Orders
            //            select s).Distinct().ToList();



            //Console.WriteLine("SELL:");
            //foreach (var item in sells)
            //{
            //    Console.WriteLine($"{item.Value.Price} {item.Value.Quantity}");
            //}

            //Console.WriteLine("BUY:");
            //foreach (var item in buys)
            //{
            //    Console.WriteLine($"{item.Value.Price} {item.Value.Quantity}");
            //}

            //Console.WriteLine("BookMarks");
            //var books = (from s in OrdersBookmarks.Values 
            //            select s).ToList();
            //foreach (var ord in books)
            //{
            //    Console.WriteLine($"{ord.OrderId} {ord.OrderSide} {ord.Price} {ord.Quantity}");
            //}
        }

        public static void Reset()
        {   
            TradeCount = 0;
            TradeSeconds = 0;
             Global.DebugOutput.Clear();
            OrdersBookmarks = new Dictionary<string, Order>();
            BuyOrdersBook = new SortedDictionary<int, SortedDictionary<long, Order>>(new DescendingComparer<int>());
            SellOrdersBook = new SortedDictionary<int, SortedDictionary<long, Order>>();
        }
        public static void Clean()
        {

            Global.DebugOutput.Clear();
            OrdersBookmarks.Clear();
            BuyOrdersBook.Clear();
            SellOrdersBook.Clear();
            OrdersBookmarks = null;
            BuyOrdersBook = null;
            SellOrdersBook = null;

        }

    }

    

}

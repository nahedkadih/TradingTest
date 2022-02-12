using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleNahedTest
{
    internal class SearchTest
    {
        public static double TradeSeconds = 0; 
        public static List<Order> SellOrdersList = new List<Order>();
        public static SortedDictionary<string, Order> SellOrdersDict = new SortedDictionary<string, Order>();
        public static LinkedList<Order> SellOrdersLinked = new LinkedList<Order>();

        public static void Test_Execute()
        {
            FillCommands();

            Find_SellOrdersList();

        }
        public static void Find_SellOrdersList()
        { 
            uint increment = (int) Global.MAX_RandomCommands / 5;
           
            for (uint cmdCount = 1; cmdCount < Global.MAX_RandomCommands; cmdCount = cmdCount + increment)
            {
                long strDate1 = DateTime.Now.Ticks;
                AddBuyOrderList();
                long endDate1 = DateTime.Now.Ticks;
                long duration1 = (endDate1 - strDate1)  ;

                long strDate2 = DateTime.Now.Ticks;
                AddBuyOrderDict();
                long endDate2 = DateTime.Now.Ticks;
                long duration2 = (endDate2 - strDate2) ;

                long strDate3 = DateTime.Now.Ticks;
                AddBuyOrderLinkedList() ;
                long endDate3 = DateTime.Now.Ticks;
                long duration3 = (endDate3 - strDate3);

                Global.FastConsole.WriteLine(string.Format("List = {0}  Dict= {1}  LinkedList= {2}", duration1, duration2, duration3));
                Global.FastConsole.Flush();
                //Console.WriteLine(string.Format("List = {0}  Dict= {1}", duration1, duration2));
                Thread.Sleep(300);
            }

            

        }
        public static void FindOrder(string _orderId)
        {

            DateTime strDate = DateTime.Now;
            var yyy = SellOrdersList.FirstOrDefault(x => x.OrderId == _orderId);
            Console.WriteLine(yyy.OrderId);
            DateTime endDate = DateTime.Now;
            TimeSpan duration = endDate - strDate;
            double TradeSeconds1 = duration.TotalMilliseconds;

            strDate = DateTime.Now;
            var _order = new Order();
            if (SellOrdersDict.TryGetValue(_orderId, out _order))
            {
                Console.WriteLine(yyy.OrderId);
                endDate = DateTime.Now;
            }

            duration = endDate - strDate;
            double TradeSeconds2 = duration.TotalMilliseconds;

            Console.WriteLine(string.Format("List   = {0}  Dict= {1} ", TradeSeconds, TradeSeconds2));


        }

        public static void AddBuyOrderList()
        {
            Random random = new Random();
            int quantity = random.Next(5, 70);
            Order order = new Order("orderId", "BUY", Global.CMD_GOODFORDAY, 100, quantity);
            ArrayList resultList = new ArrayList();
            while (true)
            {
                if (order.Quantity == 0)
                {
                    break;
                }
               
                for (int i = 0; i < SellOrdersList.Count; i++)
                {
                    if (order.Quantity <= 0)
                    {
                        break;
                    }
                    Order SellOrder = SellOrdersList[i];

                    int minQuantity = Math.Min(order.Quantity, SellOrder.Quantity);
                   // Global.PrintToScreen(SellOrder.OrderId, SellOrder.Price, minQuantity, order.OrderId, order.Price, minQuantity);
                    if (order.Quantity >= SellOrder.Quantity)
                    {
                        resultList.Add(i)  ;
                           //SellOrdersList.Remove(SellOrder);
                         //  i--;
                    }
                    else
                    {
                        SellOrder.Quantity -= minQuantity;
                    }
                    order.Quantity -= minQuantity;

                }

            }
            int q = 0;
            while (q < resultList.Count)
            {
               // SellOrdersList.RemoveAt(Convert.ToInt32(resultList[q]));//[q]= null; 
                q++;
            }
            resultList.Clear();

        }

        public static void AddBuyOrderLinkedList( )
        {
            Random random = new Random();
            int quantity = random.Next(5, 70); 
            Order order = new Order("orderId", "BUY", Global.CMD_GOODFORDAY, 100, quantity);

            LinkedList<Order> _orders;
            LinkedListNode<Order> OrderNode;
            // look for the ones that are higher than the limit price, BuyOrdersBook is sorted in desc order 
            // askMin holds the lowest  price of sell orders.
           
                while (true)
                {
                    if ( order.Quantity == 0)
                    {
                        break;
                    }
                   
                    _orders = SellOrdersLinked;
                    OrderNode = _orders.First;
                    while (OrderNode != null && order.Quantity > 0)
                    {
                        if (order.Quantity <= 0)
                        {
                            break;
                        }
                        int minQuantity = Math.Min(order.Quantity, OrderNode.Value.Quantity);
                      
                        Global.PrintToScreen(OrderNode.Value.OrderId, OrderNode.Value.Price, minQuantity, order.OrderId, order.Price, minQuantity);

                        if (OrderNode.Value.Quantity <= order.Quantity)
                        {  
                            _orders.Remove(OrderNode);
                            OrderNode = _orders.First; 
                        }
                        else
                        {
                            OrderNode.Value.Quantity -= order.Quantity;
                        }
                        order.Quantity -= minQuantity;
                    }


                }


                
           
        }
        public static void AddBuyOrderDict()
        {
            Random random = new Random();
            int quantity = random.Next(5, 70);
            Order order = new Order("orderId", "BUY", Global.CMD_GOODFORDAY, 100, quantity);

            SortedDictionary<string, Order> _orders;
            // look for the ones that are higher than the limit price, BuyOrdersBook is sorted in desc order 
            // askMin holds the lowest  price of sell orders.

            while (true)
            {
                if ( order.Quantity == 0)
                {
                    break;
                }
                
                _orders = SellOrdersDict;
                for (int i = 0; i < _orders.Count; i++)
                {
                    if (order.Quantity <= 0)
                    {
                        break;
                    }
                    var orderItem = _orders.ElementAt(i);
                    var SellOrder = orderItem.Value;

                   
                    int minQuantity = Math.Min(order.Quantity, SellOrder.Quantity);
                   // Global.PrintToScreen(SellOrder.OrderId, SellOrder.Price, minQuantity, order.OrderId, order.Price, minQuantity);
                    if (order.Quantity >= SellOrder.Quantity)
                    {

                        _orders.Remove(SellOrder.OrderId);
                        i--;
                        
                    }
                    else
                    {
                        SellOrder.Quantity -= minQuantity;
                    }
                    order.Quantity -= minQuantity;


                }

            }




        }


        public static void Reset()
        {
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            GC.WaitForPendingFinalizers();
            GC.Collect();
            SellOrdersDict = new SortedDictionary<string, Order>();
            SellOrdersList = new List<Order>();

        }
        public static void FillCommands()
        {
            Reset();
            getRandomCommands(Global.MAX_RandomCommands);

        }
        static void getRandomCommands(uint count = 0)
        {
            if (count == 0)
            {
                count = 100000;
            }
            List<string> commands = new List<string>();
            Random random = new Random();
            DateTime strDate = DateTime.Now;
            for (int q = 1; q < count + 1; q++)
            {
                string cmdtype = "GFD";
                string cmd = "BUY";
                int quantity = random.Next(5, 70);
                string orderId = string.Format("order_{0}", q.ToString());
                Order order = new Order(orderId, cmd, cmdtype, 100, quantity);
                SellOrdersList.Add(order);
                SellOrdersDict.Add(orderId, order);
                SellOrdersLinked.AddLast(order);
            }

        }
        
    }
}

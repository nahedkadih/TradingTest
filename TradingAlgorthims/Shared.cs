using ConsoleNahedTest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;


namespace ConsoleNahedTest
{
    static class Side
    {
        public const string BUY = "BUY";
        public const string SELL = "SELL";
    }


    public static class Global
    {
        public static List<string> DebugOutput = new List<string>();

        private static bool printToScreen = false;
        public const uint MAX_RandomCommands = 1000000;
        public const ushort MAX_PRICE = 500;
        public const ushort MIN_PRICE = 500;
        public const string CMD_BUY = Side.BUY;
        public const string CMD_SELL = Side.SELL;
        public const string CMD_GOODFORDAY = "GFD";
        public const string CMD_INSERTORCANCEL = "IOC";
        public const string CMD_CANCEL = "CANCEL";
        public const string CMD_MODIFY = "MODIFY";
        public const string CMD_PRINT = "PRINT";

        public static void PrintToScreen(string OrderId, ushort price, int minQuantity, string _orderid, ushort _price, int _minQuantity)
        {
            if (printToScreen)
            {
                Console.WriteLine($"TRADE {OrderId} {price} {minQuantity} {_orderid} {_price} {_minQuantity}");
                DebugOutput.Add($"TRADE {OrderId} {price} {minQuantity} {_orderid} {_price} {_minQuantity}");
            }
        }

        public static void PrintLine(string sLine)
        {
            if (printToScreen)
            {
                Global.FastConsole.WriteLine(sLine);
                DebugOutput.Add(sLine);
            }
        }
        public static void WriteOnBottomLine(string text)
        {
            int x = Console.CursorLeft;
            int y = Console.CursorTop;
            Console.CursorTop = Console.WindowTop + Console.WindowHeight - 1;
            Console.Write(text);
            // Restore previous position
            Console.SetCursorPosition(x, y);
        }
        public static class FastConsole
        {
            static readonly BufferedStream str;

            static FastConsole()
            {
                Console.OutputEncoding = Encoding.Unicode;  // crucial

                // avoid special "ShadowBuffer" for hard-coded size 0x14000 in 'BufferedStream' 
                str = new BufferedStream(Console.OpenStandardOutput(), 0x15000);
            }

            public static void WriteLine(String s) => Write(s + "\r\n");

            public static void Write(String s)
            {
                // avoid endless 'GetByteCount' dithering in 'Encoding.Unicode.GetBytes(s)'
                var rgb = new byte[s.Length << 1];
                Encoding.Unicode.GetBytes(s, 0, s.Length, rgb, 0);

                lock (str)   // (optional, can omit if appropriate)
                    str.Write(rgb, 0, rgb.Length);
            }

            public static void Flush() { lock (str) str.Flush(); }
        };

        static private int GetObjectSize(object TestObject)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            byte[] Array;
            bf.Serialize(ms, TestObject);
            Array = ms.ToArray();
            return Array.Length;
        }
        private static long? GetSizeOfObjectInBytes(object item)
        {
            if (item == null) return 0;
            try
            {
                // hackish solution to get an approximation of the size
                var jsonSerializerSettings = new JsonSerializerSettings
                {
                    DateFormatHandling = DateFormatHandling.IsoDateFormat,
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                    MaxDepth = 10,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };
                var formatter = new JsonMediaTypeFormatter { SerializerSettings = jsonSerializerSettings };
                using (var stream = new MemoryStream())
                {
                    formatter.WriteToStream(item.GetType(), item, stream, Encoding.UTF32);
                    return stream.Length / 4; // 32 bits per character = 4 bytes per character
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        private static int DumpApproximateObjectSize(object toWeight)
        {
            return Marshal.ReadInt32(toWeight.GetType().TypeHandle.Value, 4);
        }
    }
    public class Order
    {
        public string OrderId { get; set; }
        public string OrderType { get; set; }
        public string OrderSide { get; set; }
        public ushort Price { get; set; }
        public int Quantity { get; set; }
        public long TimeTicks { get; set; }
        public int Index { get; set; }
        public Order()
        {

        }

        public Order(string orderId, string orderSide, string orderType, ushort price, int quantity)
        {
            OrderId = orderId;
            OrderType = orderType;
            OrderSide = orderSide;
            Price = price;
            Quantity = quantity;
            //DateTime date = new DateTime.Now;
            //long ticks = date.Ticks; // long is the date-Ticks  - 10 million ticks in a second
            TimeTicks = DateTime.Now.Ticks;
        }

    }

}




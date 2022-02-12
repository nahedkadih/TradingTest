using ConsoleNahedTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class Program
{


    static List<int> MyGloballist = new List<int> { 100 };
    static void Main(string[] args)
    {
        //RunSampels();
        //FindErrors();
        // ProcessRandomCommands();
         ProcessRandomCommands_Patch();
        //SearchTest.Test_Execute();
        Console.WriteLine("*************************  Completed *************************");
        Console.ReadLine();

    }
    static void RunSampels()
    {
        string[] commands = getExample();
        RunApps(commands);
    }

    static void ProcessRandomCommands_Patch()
    {

        Console.Clear();
        Console.WriteLine("");
        for (uint cmdCount = 100000; cmdCount < Global.MAX_RandomCommands; cmdCount = cmdCount + 100000)
        {
            Global.FastConsole.WriteLine(string.Format("Processing Commands count = {0} ", cmdCount));
            Global.FastConsole.WriteLine("-------------------------------------------------------------------------");
            ProcessRandomCommands(cmdCount);

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            GC.WaitForPendingFinalizers();
            GC.Collect();
        } 
    }
    static void ProcessRandomCommands(uint commandsCount = 0)
    {
        if (commandsCount == 0)
        {
            commandsCount = Global.MAX_RandomCommands;
        }
        string[] commands = getRandomCommands(commandsCount);
        RunApps(commands);
        Console.WriteLine("--------------------------  Completed --------------------------------");
    }

    static void RunApps(string[] commands)
    {
        (int TradeCountLists0, double TradeSecondsLists0) = TradingEngineDictionary0.RunExample(commands);
        Console.WriteLine(string.Format("LinkedList  - TradeSecondsLists0     TradeCount = {0}  TradeSeconds= {1} ", TradeCountLists0, TradeSecondsLists0));

        (int TradeCountLists1, double TradeSecondsLists1) = TradingEngineDictionary1.RunExample(commands);
        Console.WriteLine(string.Format("List        - TradeSecondsLists1     TradeCount = {0}  TradeSeconds= {1} ", TradeCountLists1, TradeSecondsLists1));

        (int TradeCountLists2, double TradeSecondsLists2) = TradingEngineDictionary2.RunExample(commands);
        Console.WriteLine(string.Format("Queue       - TradeSecondsLists2     TradeCount = {0}  TradeSeconds= {1} ", TradeCountLists2, TradeSecondsLists2));

        (int TradeCountLists3, double TradeSecondsLists3) = TradingEngineDictionary3.RunExample(commands);
        Console.WriteLine(string.Format("Dictionary  - TradeSecondsLists3     TradeCount = {0}  TradeSeconds= {1} ", TradeCountLists3, TradeSecondsLists3));

        (int TradeCountLists4, double TradeSecondsLists4) = TradingEngineDictionary4.RunExample(commands);
        Console.WriteLine(string.Format("SortedDict  - TradeSecondsLists4     TradeCount = {0}  TradeSeconds= {1} ", TradeCountLists4, TradeSecondsLists4));

        

        // (int TradeCountLists4, double TradeSecondsLists4) = TradingEngineDictionary4.RunExample(commands);
        // Console.WriteLine(string.Format("TradeSecondsLists4   TradeCount = {0}  TradeSeconds= {1} ", TradeCountLists4, TradeSecondsLists4));
        // Console.WriteLine("*****************************");

        // (int TradeCountLists5, double TradeSecondsLists5) = TradingEngineDictionary5.RunExample(commands);
        // Console.WriteLine(string.Format("TradeSecondsLists5   TradeCount = {0}  TradeSeconds= {1} ", TradeCountLists5, TradeSecondsLists5));
        // Console.WriteLine("*****************************");


        //(int TradeCountLists3, double TradeSecondsLists3) = TradingEngineDictionary3.RunExample(commands);
        //Console.WriteLine(string.Format("TradeSecondsLists3    TradeCount = {0}  TradeSeconds= {1} ", TradeCountLists3, TradeSecondsLists3));

        //(int TradeCountLists7, double TradeSecondsLists7) = TradingEngineDictionary7.RunExample(commands);
        //Console.WriteLine(string.Format("TradeSecondsLists7    TradeCount = {0}  TradeSeconds= {1} ", TradeCountLists7, TradeSecondsLists7));


        //if (TradeCountLists1 != TradeCountLists4)
        //{
        //    foreach (var command in commands)
        //    {
        //        Global.FastConsole.WriteLine(command);
        //    }
        //    Console.ReadLine();
        //}

    }
    static int getPricesFromSet()
    {
        int price = 100;
        var random = new Random();
        int index = random.Next(MyGloballist.Count);
        return MyGloballist[index];
    }
    static string[] getRandomCommands(uint count)
    {
        List<string> commands = new List<string>();
        Random random = new Random();
        DateTime strDate = DateTime.Now;
        for (int q = 1; q < count + 1; q++)
        {
            string Command = "";
            string cmdtype = "GFD";
            string cmd = "BUY";
            int qty = random.Next(5, 70);
            int price = random.Next(Global.MIN_PRICE, Global.MAX_PRICE);
            //  int price = random.Next(40,50);
            //int price = getPricesFromSet();
            //price += q % 300;

            if (q % 2 == 0)
            {
                cmd = "SELL";
            }
            if (q % 40 == 0)
            {
                 cmd = "MODIFY"; //MODIFY order1 BUY 1005 10
            }
            if (cmd == "MODIFY")
            {
                Command = string.Format("{0} order_{4} {1} {2} {3} ", cmd, "BUY", price.ToString(), qty.ToString(), (q - 2).ToString());
            }
            else
            {
                Command = string.Format("{0} {1} {2} {3} order_{4}", cmd, cmdtype, price.ToString(), qty.ToString(), q.ToString());
            }
            if (q % 60 == 0)
            {
               Command = string.Format("{0} order_{1}", "CANCEL", (q - 4).ToString());
            }
            commands.Add(Command);
        }
        //commands.Add("PRINT");
        //foreach (var command in commands)
        //{
        //    Console.WriteLine(command);
        //}

        return commands.ToArray();
    }

    static string[] getExample()
    {
        string cmds = @"
BUY GFD 61 35 order_1
BUY GFD 63 45 order_2
SELL GFD 76 30 order_3
BUY GFD 77 15 order_4
BUY GFD 92 20 order_5
";

        string[] lines = cmds.Split(
            new string[] { Environment.NewLine },
            StringSplitOptions.None
        );

        var myList = new List<string>();
        foreach (string command in lines)
        {
            if (command.Trim().Length > 0)
            {
                myList.Add(command.Trim());
            }
        }
        foreach (var command in myList)
        {
            Global.FastConsole.WriteLine(command);
        }
        Global.FastConsole.WriteLine(Environment.NewLine + "*****************************" + Environment.NewLine);
        return myList.ToArray();
    }

    static void FindErrors()
    {
        for (int q = 0; q < 3000; q++)
        {

            string[] commands = getRandomCommands(50);
            RunApps(commands);
        }
    }
    public static void ClearCurrentConsoleLine()
    {
        int currentLineCursor = Console.CursorTop;
        Console.SetCursorPosition(0, Console.CursorTop);
        Console.Write(new string(' ', Console.WindowWidth));
        Console.SetCursorPosition(0, currentLineCursor);
    }
    public static void ClearLine()
    {
        Console.SetCursorPosition(0, Console.CursorTop - 1);
        Console.Write(new string(' ', Console.WindowWidth));
        Console.SetCursorPosition(0, Console.CursorTop);
    }


}



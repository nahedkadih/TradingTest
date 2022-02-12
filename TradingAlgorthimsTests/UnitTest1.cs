using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using ConsoleNahedTest;
 
namespace TestProject3
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {

            //TradingEngineDictionary3.Reset();
            //TradingEngineDictionary3.ExecuteCommand("BUY GFD 1000 10 order1");
            //TradingEngineDictionary3.ExecuteCommand("SELL GFD 900 10 order2");
            //Assert.AreEqual("TRADE order1 1000 10 order2 900 10",
            //TradingEngineDictionary3.DebugOutput[TradingEngineDictionary3.DebugOutput.Count - 1], "Case0010");

            //TradingEngineDictionary3.Reset();

            //TradingEngineDictionary3.ExecuteCommand("SELL GFD 900 10 order2");
            //TradingEngineDictionary3.ExecuteCommand("BUY GFD 1000 10 order1");
            //Assert.AreEqual("TRADE order2 900 10 order1 1000 10",
            //    TradingEngineDictionary3.DebugOutput[TradingEngineDictionary3.DebugOutput.Count - 1], "Case0020");

            //TradingEngineDictionary3.Reset();

            //TradingEngineDictionary3.ExecuteCommand("BUY GFD 1000 10 order1");
            //TradingEngineDictionary3.ExecuteCommand("BUY GFD 1000 10 order2");
            //TradingEngineDictionary3.ExecuteCommand("SELL GFD 900 20 order3");
            //Assert.AreEqual("TRADE order1 1000 10 order3 900 10",
            //    TradingEngineDictionary3.DebugOutput[TradingEngineDictionary3.DebugOutput.Count - 2], "Case0030");
            //Assert.AreEqual("TRADE order2 1000 10 order3 900 10",
            //    TradingEngineDictionary3.DebugOutput[TradingEngineDictionary3.DebugOutput.Count - 1], "Case0040");

            //TradingEngineDictionary3.Reset();

            //TradingEngineDictionary3.ExecuteCommand("BUY GFD 950 10 order1");
            //TradingEngineDictionary3.ExecuteCommand("BUY GFD 1000 15 order2");
            //TradingEngineDictionary3.ExecuteCommand("SELL GFD 900 20 order3");
            //Assert.AreEqual("TRADE order2 1000 15 order3 900 15",
            //    TradingEngineDictionary3.DebugOutput[TradingEngineDictionary3.DebugOutput.Count - 2], "Case0050");
            //Assert.AreEqual("TRADE order1 950 5 order3 900 5",
            //    TradingEngineDictionary3.DebugOutput[TradingEngineDictionary3.DebugOutput.Count - 1], "Case0060");

            //TradingEngineDictionary3.Reset();
            //TradingEngineDictionary3.ExecuteCommand("BUY GFD 1000 10 order1");
            //TradingEngineDictionary3.ExecuteCommand("BUY GFD 1000 10 order2");
            //TradingEngineDictionary3.ExecuteCommand("MODIFY order1 BUY 1000 20");
            //TradingEngineDictionary3.ExecuteCommand("SELL GFD 900 20 order3");
            //Assert.AreEqual("TRADE order2 1000 10 order3 900 10",
            //    TradingEngineDictionary3.DebugOutput[TradingEngineDictionary3.DebugOutput.Count - 2], "Case0070");
            //Assert.AreEqual("TRADE order1 1000 10 order3 900 10",
            //    TradingEngineDictionary3.DebugOutput[TradingEngineDictionary3.DebugOutput.Count - 1], "Case0080");

            TradingEngineDictionary33.Reset();

            TradingEngineDictionary33.ExecuteCommand("BUY GFD 1000 10 order1");
            TradingEngineDictionary33.ExecuteCommand("BUY GFD 1001 20 order2");
            TradingEngineDictionary33.ExecuteCommand("PRINT");
            Assert.AreEqual("1001 20",
                Global.DebugOutput[Global.DebugOutput.Count - 2], "Case0090");
            Assert.AreEqual("1000 10",
                Global.DebugOutput[Global.DebugOutput.Count - 1], "Case0100");


            TradingEngineDictionary33.Reset();
            TradingEngineDictionary33.ExecuteCommand("BUY GFD 1000 10 order1");
            TradingEngineDictionary33.ExecuteCommand("SELL GFD 900 20 order2");
            TradingEngineDictionary33.ExecuteCommand("PRINT");
            Assert.AreEqual("TRADE order1 1000 10 order2 900 10",
                Global.DebugOutput[Global.DebugOutput.Count - 4], "Case0110");
            Assert.AreEqual("900 10",
                Global.DebugOutput[Global.DebugOutput.Count - 2], "Case0120");


            TradingEngineDictionary33.Reset();

            TradingEngineDictionary33.ExecuteCommand("BUY GFD 1000 10 order1");
            TradingEngineDictionary33.ExecuteCommand("BUY GFD 1010 10 order2");
            TradingEngineDictionary33.ExecuteCommand("SELL GFD 1000 15 order3");
            Assert.AreEqual("TRADE order2 1010 10 order3 1000 10",
                Global.DebugOutput[Global.DebugOutput.Count - 2], "Case0130");
            Assert.AreEqual("TRADE order1 1000 5 order3 1000 5",
                Global.DebugOutput[Global.DebugOutput.Count - 1], "Case0140");

            TradingEngineDictionary33.Reset();

            TradingEngineDictionary33.ExecuteCommand("BUY GFD 1000 10 order1");
            TradingEngineDictionary33.ExecuteCommand("BUY GFD 1001 10 order2");
            TradingEngineDictionary33.ExecuteCommand("SELL IOC 1000 15 order3");
            TradingEngineDictionary33.ExecuteCommand("SELL GFD 1000 15 order4");

            Assert.AreEqual("TRADE order2 1001 10 order3 1000 10",
                Global.DebugOutput[Global.DebugOutput.Count - 3], "Case0150");
            Assert.AreEqual("TRADE order1 1000 5 order3 1000 5",
                Global.DebugOutput[Global.DebugOutput.Count - 2], "Case0160");
            Assert.AreEqual("Global order1 1000 5 order4 1000 5",
                 Global.DebugOutput[Global.DebugOutput.Count - 1], "Case0170");



            TradingEngineDictionary33.Reset();

            TradingEngineDictionary33.ExecuteCommand("BUY GFD 1000 10 order1");
            TradingEngineDictionary33.ExecuteCommand("SELL IOC 900 15 order2");
            TradingEngineDictionary33.ExecuteCommand("BUY GFD 1001 10 order3");
            TradingEngineDictionary33.ExecuteCommand("BUY GFD 1004 10 order4");
            TradingEngineDictionary33.ExecuteCommand("SELL GFD 1000 15 order5");

            Assert.AreEqual("TRADE order1 1000 10 order2 900 10",
                 Global.DebugOutput[Global.DebugOutput.Count - 3], "Case0180");
            Assert.AreEqual("TRADE order4 1004 10 order5 1000 10",
                 Global.DebugOutput[Global.DebugOutput.Count - 2], "Case0190");
            Assert.AreEqual("TRADE order3 1001 5 order5 1000 5",
                 Global.DebugOutput[Global.DebugOutput.Count - 1], "Case0200");

            //TradeTest8.txt
            TradingEngineDictionary33.Reset();

            TradingEngineDictionary33.ExecuteCommand("BUY GFD 1000 25 order1");
            TradingEngineDictionary33.ExecuteCommand("SELL IOC 900 15 order4");
            TradingEngineDictionary33.ExecuteCommand("BUY GFD 1001 10 order2");
            TradingEngineDictionary33.ExecuteCommand("BUY GFD 1004 10 order3");
            TradingEngineDictionary33.ExecuteCommand("MODIFY order1 BUY 1005 10");
            TradingEngineDictionary33.ExecuteCommand("SELL GFD 1000 15 order5");
            TradingEngineDictionary33.ExecuteCommand("PRINT");

            Assert.AreEqual("TRADE order1 1000 15 order4 900 15",
                 Global.DebugOutput[0], "Case0210");
            Assert.AreEqual("TRADE order1 1005 10 order5 1000 10",
                 Global.DebugOutput[1], "Case0220");
            Assert.AreEqual("TRADE order3 1004 5 order5 1000 5",
                 Global.DebugOutput[2], "Case0230");
            Assert.AreEqual("SELL:",
                 Global.DebugOutput[3], "Case0240");
            Assert.AreEqual("BUY:",
                 Global.DebugOutput[4], "Case0250");
            Assert.AreEqual("1004 5",
                 Global.DebugOutput[5], "Case0260");
            Assert.AreEqual("1001 10",
                 Global.DebugOutput[6], "Case0270");

            //TradeTest9.txt
            TradingEngineDictionary33.Reset();

            TradingEngineDictionary33.ExecuteCommand("BUY GFD 1000 6 order1");
            TradingEngineDictionary33.ExecuteCommand("BUY GFD 1001 5 order2");
            TradingEngineDictionary33.ExecuteCommand("BUY GFD 1004 5 order3");
            TradingEngineDictionary33.ExecuteCommand("SELL IOC 900 15 order4");
            TradingEngineDictionary33.ExecuteCommand("MODIFY order1 BUY 1005 5");
            TradingEngineDictionary33.ExecuteCommand("SELL GFD 1000 15 order5");
            TradingEngineDictionary33.ExecuteCommand("BUY GFD 1006 5 order7");
            TradingEngineDictionary33.ExecuteCommand("BUY GFD 800 5 order8");
            TradingEngineDictionary33.ExecuteCommand("PRINT");

            Assert.AreEqual("TRADE order3 1004 5 order4 900 5",
                Global.DebugOutput[0], "Case0280");
            Assert.AreEqual("TRADE order2 1001 5 order4 900 5",
                 Global.DebugOutput[1], "Case0290");
            Assert.AreEqual("TRADE order1 1000 5 order4 900 5",
                 Global.DebugOutput[2], "Case0300");
            Assert.AreEqual("TRADE order1 1005 5 order5 1000 5",
                 Global.DebugOutput[3], "Case0310");
            Assert.AreEqual("TRADE order5 1000 5 order7 1006 5",
                 Global.DebugOutput[4], "Case0320");
            Assert.AreEqual("1000 5",
                 Global.DebugOutput[6], "Case0330");
            Assert.AreEqual("800 5",
                 Global.DebugOutput[8], "Case0340");


            //TradeTest10.txt
            TradingEngineDictionary33.Reset();

            TradingEngineDictionary33.ExecuteCommand("SELL GFD 1000 5 order5");
            TradingEngineDictionary33.ExecuteCommand("BUY GFD 800 5 order8");
            TradingEngineDictionary33.ExecuteCommand("PRINT");

            Assert.AreEqual("1000 5",
                 Global.DebugOutput[1], "Case0350");
            Assert.AreEqual("800 5",
                 Global.DebugOutput[3], "Case0360");


        }

    }
}
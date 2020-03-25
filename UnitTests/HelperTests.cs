using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace FormSim.Tests
{
    [TestClass()]
    public class HelperTests
    {
        [TestMethod()]
        public void stripAPIParametersTest()
        {
            Helper helper = Helper.getInstance;
            Dictionary<string, string> FRC_Dict = new Dictionary<string, string>();
            FRC_Dict.Add("APIOptions", "ALLDATA,ALLOWPARTIALAUTH,BYPASSAMOUNTOK,BYPASSSIGCAP,ENHANCEDRECEIPTS,PRINTTIPLINE");
            FRC_Dict.Add("STX", "YES");
            FRC_Dict = helper.stripAPIParameters(FRC_Dict, "PRINTTIPLINE");
            Assert.IsFalse(FRC_Dict["APIOptions"].Contains("PRINTTIPLINE"));
        }

        [TestMethod()]
        public void stripAPIParametersTest1()
        {
            Helper helper = Helper.getInstance;
            Dictionary<string, string> FRC_Dict = new Dictionary<string, string>();
            FRC_Dict.Add("APIOptions", "ALLDATA,ALLOWPARTIALAUTH,BYPASSAMOUNTOK,BYPASSSIGCAP,ENHANCEDRECEIPTS,PRINTTIPLINE");
            FRC_Dict.Add("STX", "YES");
            string[] list = { "PRINTTIPLINE", "ENHANCEDRECEIPTS" };
            FRC_Dict = helper.stripAPIParameters(FRC_Dict, list);
            Assert.IsFalse(FRC_Dict["APIOptions"].Contains("PRINTTIPLINE"));
            Assert.IsFalse(FRC_Dict["APIOptions"].Contains("ENHANCEDRECEIPTS"));
        }

        [TestMethod()]
        public void parseDictTest()
        {
            Helper helper = Helper.getInstance;
            Dictionary<string, string> FRC_Dict = new Dictionary<string, string>();
            FRC_Dict.Add("APIOptions", "ALLDATA,ALLOWPARTIALAUTH,BYPASSAMOUNTOK,BYPASSSIGCAP,ENHANCEDRECEIPTS,PRINTTIPLINE");
            FRC_Dict.Add("STX", "YES");
            string res = helper.parseDict(FRC_Dict);
            Assert.AreEqual(" APIOptions: ALLDATA:ALLOWPARTIALAUTH:BYPASSAMOUNTOK:BYPASSSIGCAP:ENHANCEDRECEIPTS:PRINTTIPLINE \r\n STX: YES ", res);
        }

        [TestMethod()]
        public void getDateTest()
        {
            Helper helper = Helper.getInstance;
            DateTime localDateTime = DateTime.Now;
            string date = helper.getDate();
            string month = localDateTime.Month.ToString();
            string day = localDateTime.Day.ToString();
            char[] year = localDateTime.Year.ToString().ToCharArray();
            if (month.Length < 2) { month = "0" + month; }
            if (day.Length < 2) { day = "0" + day; }
            string Syear = year[2].ToString() + year[3].ToString();
            Assert.AreEqual(month.ToString() + day.ToString() + Syear.ToString(), date);
        }

        [TestMethod()]
        public void getTimeTest()
        {
            Helper helper = Helper.getInstance;
            DateTime localDateTime = DateTime.Now;
            string time = helper.getTime();
            string local = localDateTime.ToString("HH:mm:ss");
            local = local.Remove(5, 1);
            local = local.Remove(2, 1);
            Assert.AreEqual(local, time);
        }
    }
}
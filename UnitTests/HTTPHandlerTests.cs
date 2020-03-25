using Microsoft.VisualStudio.TestTools.UnitTesting;
using FormSim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormSim.Tests
{
    [TestClass()]
    public class HTTPHandlerTests
    {
        Dictionary<string, string> parameters;
        Helper helper;

        [TestInitialize()]
        public void Initialize()
        {
            helper = Helper.getInstance;
            parameters = new Dictionary<string, string>
            {
                { "STX", "YES" },
                { "VERBOSE", "YES" },
                { "CONTENTTYPE", "XML" },
                { "APISignature", "$" },
                { "APIFormat", "0" }
            };

        }

        // Likely will need more than a few of these for different use cases.
        
        /* E-Commerce Straight Settle 
         * Test 2A (with raw CHD, not a token)
         * S4 TestScript_E-commerce_i4Go_StraightSettle.xlsx
         */
        [TestMethod()]
        public void performTransactionTest()
        {
            HTTPHandler handler = new HTTPHandler("D15D6347-E3D6-4BB4-7B5632F470EBC751", "21B00A88-E976-66F2-88EA7F78281AFAE2", "10.0.2.124", "16448");
            Assert.IsTrue(handler.performTokenExchange().Result);
            parameters.Add("AccessToken", handler.getAccessToken());

            // Insert Front End Passed Params here
            parameters.Add("CardNumber", "2221000000000009");
            parameters.Add("ExpirationDate", "1022");
            parameters.Add("CVV2", "333");
            parameters.Add("CVV2Indicator", "Y");
            parameters.Add("CardPresent", "N");
            parameters.Add("StreetAddress", "65 Main Street");
            parameters.Add("DestinationZipCode", "65000");
            //parameters.Add("CardType", "NotUsed");
            parameters.Add("TrackData", "");
            //parameters.Add("UniqueID", "");
            parameters.Add("TokenSerial", "");
            parameters.Add("APIOptions", "ALLDATA");
            parameters.Add("Invoice", "151858444");
            parameters.Add("PrimaryAmount", "100.31");
            parameters.Add("SaleFlag", "S");
            parameters.Add("ZipCode", "65000");
            parameters.Add("SecondaryAmount", "0.00");
            parameters.Add("FunctionRequestCode", "1D");
            parameters.Add("TaxIndicator", "Y");
            parameters.Add("TaxAmount", "11.14");
            //parameters.Add("TranID", "");
            parameters.Add("Clerk", "24");
            //parameters.Add("TerminalID", "");
            parameters.Add("Date", this.helper.getDate());
            parameters.Add("Time", helper.getTime());
            parameters.Add("VoidInvalidAVS", "Y");
            parameters.Add("VoidInvalidCVV2", "Y");
            parameters.Add("RequestorReference", "1350744");
            parameters.Add("UseTokenStore", "N");
            parameters.Add("UseMetaToken", "N");
            parameters.Add("CustomerName", "Tommy Tester");
            //parameters.Add("P2PEBlock", "");
            parameters.Add("UseBasicTranFlow", "Y");
            parameters.Add("UseRollbacks", "N");
            parameters.Add("CardDataType", "UnencryptedCardData");

            // Send the Transaction and get the result
            parameters.Add("ETX", "YES");
            Dictionary<string, string> res = handler.start(parameters).Result;
            Assert.AreEqual("A", res["response"]);
        }

        /* E-Commerce Straight Settle 
         * Test 2A with Token (May need to update token if test fails. Check this first)
         * S4 TestScript_E-commerce_i4Go_StraightSettle.xlsx
         */
        [TestMethod()]
        public void performTransactionTest2()
        {
            HTTPHandler handler = new HTTPHandler("D15D6347-E3D6-4BB4-7B5632F470EBC751", "21B00A88-E976-66F2-88EA7F78281AFAE2", "10.0.2.124", "16448");
            Assert.IsTrue(handler.performTokenExchange().Result);
            parameters.Add("AccessToken", handler.getAccessToken());

            // Insert Front End Passed Params here
            parameters.Add("CardNumber", "");
            parameters.Add("ExpirationDate", "");
            parameters.Add("CVV2", "");
            parameters.Add("CVV2Indicator", "");
            parameters.Add("CardPresent", "");
            parameters.Add("StreetAddress", "65 Main Street");
            parameters.Add("DestinationZipCode", "65000");
            //parameters.Add("CardType", "NotUsed");
            parameters.Add("TrackData", "");
            parameters.Add("UniqueID", "0009sp8mw7fqr8wf");
            parameters.Add("TokenSerial", "");
            parameters.Add("APIOptions", "ALLDATA");
            parameters.Add("Invoice", "151858445");
            parameters.Add("PrimaryAmount", "100.31");
            parameters.Add("SaleFlag", "S");
            parameters.Add("ZipCode", "65000");
            parameters.Add("SecondaryAmount", "0.00");
            parameters.Add("FunctionRequestCode", "1D");
            parameters.Add("TaxIndicator", "Y");
            parameters.Add("TaxAmount", "11.14");
            //parameters.Add("TranID", "");
            parameters.Add("Clerk", "24");
            //parameters.Add("TerminalID", "");
            parameters.Add("Date", this.helper.getDate());
            parameters.Add("Time", helper.getTime());
            parameters.Add("VoidInvalidAVS", "Y");
            parameters.Add("VoidInvalidCVV2", "Y");
            parameters.Add("RequestorReference", "1350744");
            parameters.Add("UseTokenStore", "N");
            parameters.Add("UseMetaToken", "N");
            parameters.Add("CustomerName", "Tommy Tester");
            //parameters.Add("P2PEBlock", "");
            parameters.Add("UseBasicTranFlow", "Y");
            parameters.Add("UseRollbacks", "N");
            parameters.Add("CardDataType", "TrueToken");

            // Send the Transaction and get the result
            parameters.Add("ETX", "YES");
            Dictionary<string, string> res = handler.start(parameters).Result;
            Assert.AreEqual("A", res["response"]);
        }

        /* E-Commerce Straight Settle 
         * Test 3 with Token (May need to update token if test fails. Check this first)
         * Online Sale (1D) -> Declined -> Auto Void (08)
         * S4 TestScript_E-commerce_i4Go_StraightSettle.xlsx
         */
        [TestMethod()]
        public void performTransactionTest3()
        {
            HTTPHandler handler = new HTTPHandler("D15D6347-E3D6-4BB4-7B5632F470EBC751", "21B00A88-E976-66F2-88EA7F78281AFAE2", "10.0.2.124", "16448");
            Assert.IsTrue(handler.performTokenExchange().Result);
            parameters.Add("AccessToken", handler.getAccessToken());

            // Insert Front End Passed Params here
            parameters.Add("CardNumber", "");
            parameters.Add("ExpirationDate", "");
            parameters.Add("CVV2", "333");
            parameters.Add("CVV2Indicator", "");
            parameters.Add("CardPresent", "");
            parameters.Add("StreetAddress", "65 Main Street");
            parameters.Add("DestinationZipCode", "65000");
            //parameters.Add("CardType", "NotUsed");
            parameters.Add("TrackData", "");
            parameters.Add("UniqueID", "0009sp8mw7fqr8wf");
            parameters.Add("TokenSerial", "");
            parameters.Add("APIOptions", "ALLDATA");
            parameters.Add("Invoice", "151858446");
            parameters.Add("PrimaryAmount", "1200.00");
            parameters.Add("SaleFlag", "S");
            parameters.Add("ZipCode", "65000");
            parameters.Add("SecondaryAmount", "0.00");
            parameters.Add("FunctionRequestCode", "1D");
            parameters.Add("TaxIndicator", "N");
            parameters.Add("TaxAmount", "0.00");
            //parameters.Add("TranID", "");
            parameters.Add("Clerk", "5188");
            //parameters.Add("TerminalID", "");
            parameters.Add("Date", this.helper.getDate());
            parameters.Add("Time", helper.getTime());
            parameters.Add("VoidInvalidAVS", "Y");
            parameters.Add("VoidInvalidCVV2", "Y");
            parameters.Add("RequestorReference", "1350744");
            parameters.Add("UseTokenStore", "N");
            parameters.Add("UseMetaToken", "N");
            parameters.Add("CustomerName", "Tommy Tester");
            //parameters.Add("P2PEBlock", "");
            parameters.Add("UseBasicTranFlow", "Y");
            parameters.Add("UseRollbacks", "N");
            parameters.Add("CardDataType", "TrueToken");

            // Send the Transaction and get the result
            parameters.Add("ETX", "YES");
            Dictionary<string, string> res = handler.start(parameters).Result;
            Assert.AreEqual("08", res["functionrequestcode"]);
        }

        /* E-Commerce Straight Settle 
         * Test 4 with Token (May need to update token if test fails. Check this first)
         * Online Sale (1D) -> Referral -> Auto Void (08)
         * S4 TestScript_E-commerce_i4Go_StraightSettle.xlsx
         */
        [TestMethod()]
        public void performTransactionTest4()
        {
            HTTPHandler handler = new HTTPHandler("D15D6347-E3D6-4BB4-7B5632F470EBC751", "21B00A88-E976-66F2-88EA7F78281AFAE2", "10.0.2.124", "16448");
            Assert.IsTrue(handler.performTokenExchange().Result);
            parameters.Add("AccessToken", handler.getAccessToken());

            // Insert Front End Passed Params here
            parameters.Add("CardNumber", "");
            parameters.Add("ExpirationDate", "");
            parameters.Add("CVV2", "333");
            parameters.Add("CVV2Indicator", "");
            parameters.Add("CardPresent", "");
            parameters.Add("StreetAddress", "65 Main Street");
            parameters.Add("DestinationZipCode", "65000");
            //parameters.Add("CardType", "NotUsed");
            parameters.Add("TrackData", "");
            parameters.Add("UniqueID", "0009sp8mw7fqr8wf");
            parameters.Add("TokenSerial", "");
            parameters.Add("APIOptions", "ALLDATA");
            parameters.Add("Invoice", "151858447");
            parameters.Add("PrimaryAmount", "625.00");
            parameters.Add("SaleFlag", "S");
            parameters.Add("ZipCode", "65000");
            parameters.Add("SecondaryAmount", "0.00");
            parameters.Add("FunctionRequestCode", "1D");
            parameters.Add("TaxIndicator", "N");
            parameters.Add("TaxAmount", "0.00");
            //parameters.Add("TranID", "");
            parameters.Add("Clerk", "5188");
            //parameters.Add("TerminalID", "");
            parameters.Add("Date", this.helper.getDate());
            parameters.Add("Time", helper.getTime());
            parameters.Add("VoidInvalidAVS", "Y");
            parameters.Add("VoidInvalidCVV2", "Y");
            parameters.Add("RequestorReference", "1350744");
            parameters.Add("UseTokenStore", "N");
            parameters.Add("UseMetaToken", "N");
            parameters.Add("CustomerName", "Tommy Tester");
            //parameters.Add("P2PEBlock", "");
            parameters.Add("UseBasicTranFlow", "Y");
            parameters.Add("UseRollbacks", "N");
            parameters.Add("CardDataType", "TrueToken");

            // Send the Transaction and get the result
            parameters.Add("ETX", "YES");
            Dictionary<string, string> res = handler.start(parameters).Result;
            Assert.AreEqual("08", res["functionrequestcode"]);
        }

        /* E-Commerce Straight Settle 
         * Test 5 with Token (May need to update token if test fails. Check this first)
         * Online Sale (1D) -> Time Out -> Get Invoice Status (07)
         * S4 TestScript_E-commerce_i4Go_StraightSettle.xlsx
         */
        [TestMethod()]
        public void performTransactionTest5()
        {
            HTTPHandler handler = new HTTPHandler("D15D6347-E3D6-4BB4-7B5632F470EBC751", "21B00A88-E976-66F2-88EA7F78281AFAE2", "10.0.2.124", "16448");
            Assert.IsTrue(handler.performTokenExchange().Result);
            parameters.Add("AccessToken", handler.getAccessToken());

            // Insert Front End Passed Params here
            parameters.Add("CardNumber", "");
            parameters.Add("ExpirationDate", "");
            parameters.Add("CVV2", "333");
            parameters.Add("CVV2Indicator", "");
            parameters.Add("CardPresent", "");
            parameters.Add("StreetAddress", "65 Main Street");
            parameters.Add("DestinationZipCode", "65000");
            //parameters.Add("CardType", "NotUsed");
            parameters.Add("TrackData", "");
            parameters.Add("UniqueID", "00093zepbsfqre8z");
            parameters.Add("TokenSerial", "");
            parameters.Add("APIOptions", "ALLDATA");
            parameters.Add("Invoice", "151858447");
            parameters.Add("PrimaryAmount", "111.61");
            parameters.Add("SaleFlag", "S");
            parameters.Add("ZipCode", "65000");
            parameters.Add("SecondaryAmount", "0.00");
            parameters.Add("FunctionRequestCode", "1D");
            parameters.Add("TaxIndicator", "N");
            parameters.Add("TaxAmount", "0.00");
            //parameters.Add("TranID", "");
            parameters.Add("Clerk", "5188");
            //parameters.Add("TerminalID", "");
            parameters.Add("Date", this.helper.getDate());
            parameters.Add("Time", helper.getTime());
            parameters.Add("VoidInvalidAVS", "Y");
            parameters.Add("VoidInvalidCVV2", "Y");
            parameters.Add("RequestorReference", "1350744");
            parameters.Add("UseTokenStore", "N");
            parameters.Add("UseMetaToken", "N");
            parameters.Add("CustomerName", "Tommy Tester");
            //parameters.Add("P2PEBlock", "");
            parameters.Add("UseBasicTranFlow", "Y");
            parameters.Add("UseRollbacks", "N");
            parameters.Add("CardDataType", "TrueToken");

            // Send the Transaction and get the result
            parameters.Add("ETX", "YES");
            Dictionary<string, string> res = handler.start(parameters).Result;
            Assert.AreEqual("07", res["functionrequestcode"]);
            Assert.AreEqual("A", res["response"]);
        }

        /* E-Commerce Straight Settle 
         * Test 5 with Token (May need to update token if test fails. Check this first)
         * Online Sale (1D) -> Demo Host Error -> Automatic Void (08)
         * S4 TestScript_E-commerce_i4Go_StraightSettle.xlsx
         */
        [TestMethod()]
        public void performTransactionTest6()
        {
            HTTPHandler handler = new HTTPHandler("D15D6347-E3D6-4BB4-7B5632F470EBC751", "21B00A88-E976-66F2-88EA7F78281AFAE2", "10.0.2.124", "16448");
            Assert.IsTrue(handler.performTokenExchange().Result);
            parameters.Add("AccessToken", handler.getAccessToken());

            // Insert Front End Passed Params here
            parameters.Add("CardNumber", "");
            parameters.Add("ExpirationDate", "");
            parameters.Add("CVV2", "333");
            parameters.Add("CVV2Indicator", "");
            parameters.Add("CardPresent", "");
            parameters.Add("StreetAddress", "65 Main Street");
            parameters.Add("DestinationZipCode", "65000");
            //parameters.Add("CardType", "NotUsed");
            parameters.Add("TrackData", "");
            parameters.Add("UniqueID", "0009sp8mw7fqr8wf");
            parameters.Add("TokenSerial", "");
            parameters.Add("APIOptions", "ALLDATA");
            parameters.Add("Invoice", "151858448");
            parameters.Add("PrimaryAmount", "12000.00");
            parameters.Add("SaleFlag", "S");
            parameters.Add("ZipCode", "65000");
            parameters.Add("SecondaryAmount", "0.00");
            parameters.Add("FunctionRequestCode", "1D");
            parameters.Add("TaxIndicator", "N");
            parameters.Add("TaxAmount", "0.00");
            //parameters.Add("TranID", "");
            parameters.Add("Clerk", "5188");
            //parameters.Add("TerminalID", "");
            parameters.Add("Date", this.helper.getDate());
            parameters.Add("Time", helper.getTime());
            parameters.Add("VoidInvalidAVS", "Y");
            parameters.Add("VoidInvalidCVV2", "Y");
            parameters.Add("RequestorReference", "1350744");
            parameters.Add("UseTokenStore", "N");
            parameters.Add("UseMetaToken", "N");
            parameters.Add("CustomerName", "Tommy Tester");
            //parameters.Add("P2PEBlock", "");
            parameters.Add("UseBasicTranFlow", "Y");
            parameters.Add("UseRollbacks", "N");
            parameters.Add("CardDataType", "TrueToken");

            // Send the Transaction and get the result
            parameters.Add("ETX", "YES");
            Dictionary<string, string> res = handler.start(parameters).Result;
            Assert.AreEqual("08", res["functionrequestcode"]);
        }

        [TestMethod()]
        public void HTTPHandlerTest()
        {
            HTTPHandler handler = new HTTPHandler();
            Assert.IsNull(handler.getAccessToken());
            Assert.IsNull(handler.getAuthToken());
            Assert.IsNull(handler.getClientGUID());
            Assert.IsNull(handler.getIPAddress());
            Assert.IsNull(handler.getPort());
            Assert.AreEqual("MoToEcom", handler.getMerchantType());
            Assert.IsFalse(handler.performTokenExchange().Result);
        }

        [TestMethod()]
        public void HTTPHandlerTest1()
        {
            HTTPHandler handler = new HTTPHandler("D15D6347-E3D6-4BB4-7B5632F470EBC751", "21B00A88-E976-66F2-88EA7F78281AFAE2");
            Assert.IsNull(handler.getAccessToken());
            Assert.IsNull(handler.getIPAddress());
            Assert.IsNull(handler.getPort());
            Assert.IsNotNull(handler.getAuthToken());
            Assert.IsNotNull(handler.getClientGUID());
            Assert.AreEqual("D15D6347-E3D6-4BB4-7B5632F470EBC751", handler.getAuthToken());
            Assert.AreEqual("21B00A88-E976-66F2-88EA7F78281AFAE2", handler.getClientGUID());
            Assert.IsFalse(handler.performTokenExchange().Result);
        }

        [TestMethod()]
        public void HTTPHandlerTest2()
        {
            HTTPHandler handler = new HTTPHandler("D15D6347-E3D6-4BB4-7B5632F470EBC751", "21B00A88-E976-66F2-88EA7F78281AFAE2", "10.0.2.124", "16448");
            Assert.IsNull(handler.getAccessToken());
            Assert.IsNotNull(handler.getIPAddress());
            Assert.IsNotNull(handler.getPort());
            Assert.IsNotNull(handler.getAuthToken());
            Assert.IsNotNull(handler.getClientGUID());
            Assert.AreEqual("10.0.2.124", handler.getIPAddress());
            Assert.AreEqual("16448", handler.getPort());
            Assert.AreEqual("D15D6347-E3D6-4BB4-7B5632F470EBC751", handler.getAuthToken());
            Assert.AreEqual("21B00A88-E976-66F2-88EA7F78281AFAE2", handler.getClientGUID());
            Assert.IsTrue(handler.performTokenExchange().Result);
        }

        [TestMethod()]
        public void performTokenExchangeTest()
        {
            HTTPHandler handler = new HTTPHandler("D15D6347-E3D6-4BB4-7B5632F470EBC751", "21B00A88-E976-66F2-88EA7F78281AFAE2", "10.0.2.124", "16448");
            Assert.IsNull(handler.getAccessToken());
            Assert.IsNotNull(handler.getIPAddress());
            Assert.IsNotNull(handler.getPort());
            Assert.IsNotNull(handler.getAuthToken());
            Assert.IsNotNull(handler.getClientGUID());
            Assert.AreEqual("10.0.2.124", handler.getIPAddress());
            Assert.AreEqual("16448", handler.getPort());
            Assert.AreEqual("D15D6347-E3D6-4BB4-7B5632F470EBC751", handler.getAuthToken());
            Assert.AreEqual("21B00A88-E976-66F2-88EA7F78281AFAE2", handler.getClientGUID());
            Assert.IsTrue(handler.performTokenExchange().Result);
            Assert.AreEqual("390B2B95-E74C-4774-A88B-C07E5B020488", handler.getAccessToken());
        }

        [TestMethod()]
        public void sendRawTest()
        {
            string raw = "STX=YES&VERBOSE=YES&CONTENTTYPE=XML&APISignature=%24&APIFormat=0&AccessToken=390B2B95-E74C-4774-A88B-C07E5B020488&APIOptions=ALLDATA%2CALLOWPARTIALAUTH%2CBYPASSAMOUNTOK%2CBYPASSSIGCAP%2CENHANCEDRECEIPTS%2CPRINTTIPLINE&Clerk=545&Date=090518&FunctionRequestCode=1B&Invoice=7641&PrimaryAmount=100.00&ReceiptTextColumns=30&RequestorReference=95750500&SaleFlag=S&Time=103132&Vendor=CottaCapital%3AFormSim%3A0.2&CardPresent=Y&CardNumber=4761739001010010&ExpirationDate=1220&CVV2Indicator=1&CVV2Code=333&CustomerName=Sterling+Archer&StreetAddress=65+Main+Street&ZipCode=65000&CustomerReference=randCust64012578&TaxIndicator=N&TaxAmount=0&DestinationZipCode=65000&ProductDescriptor1=Electronics&ProductDescriptor2=A%28%5B%2A%3F6%24b%26C%25d%23%2F1%3D2%5C4%7C%3A%3B%2F%2F%40www.shift4%2B%2C%27%5D%29&Notes=Random+Misc+Notes&ETX=YES";
            HTTPHandler handler = new HTTPHandler("D15D6347-E3D6-4BB4-7B5632F470EBC751", "21B00A88-E976-66F2-88EA7F78281AFAE2", "10.0.2.124", "16448");
            Dictionary<string, string> res = handler.sendRaw(raw).Result;
            Assert.AreEqual("N", res["errorindicator"]);
            Assert.AreEqual("A", res["response"]);
            Assert.AreEqual("A", res["avsresult"]);
        }
    }
}
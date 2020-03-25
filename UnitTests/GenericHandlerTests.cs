using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FormSim.Tests
{
    [TestClass()]
    
    public class GenericHandlerTests
    {

        [TestMethod()]
        public void GenericHandlerTest()
        {
            GenericHandler handler = new GenericHandler();
            Assert.IsNull(handler.getPort());
            Assert.IsNull(handler.getIPAddress());
            Assert.IsNull(handler.getClientGUID());
            Assert.IsNull(handler.getAuthToken());
            Assert.IsNull(handler.getAccessToken());
        }

        [TestMethod()]
        public void GenericHandlerTest1()
        {
            GenericHandler handler = new GenericHandler("D15D6347-E3D6-4BB4-7B5632F470EBC751", "21B00A88-E976-66F2-88EA7F78281AFAE2");
            Assert.IsNull(handler.getIPAddress());
            Assert.IsNull(handler.getPort());
            Assert.IsNull(handler.getAccessToken());
            Assert.IsNotNull(handler.getClientGUID());
            Assert.IsNotNull(handler.getAuthToken());
        }

        [TestMethod()]
        public void GenericHandlerTest2()
        {
            GenericHandler handler = new GenericHandler("D15D6347-E3D6-4BB4-7B5632F470EBC751", "21B00A88-E976-66F2-88EA7F78281AFAE2","10.0.2.124", "16448");
            Assert.IsNotNull(handler.getClientGUID());
            Assert.IsNotNull(handler.getAuthToken());
            Assert.IsNotNull(handler.getIPAddress());
            Assert.IsNotNull(handler.getPort());
            Assert.IsNull(handler.getAccessToken());
        }

        [TestMethod()]
        [ExpectedException(typeof(AggregateException))]
        public void performTokenExchangeTest()
        {
            GenericHandler handler = new GenericHandler("D15D6347-E3D6-4BB4-7B5632F470EBC751", "21B00A88-E976-66F2-88EA7F78281AFAE2", "10.0.2.124", "16448");
            bool pass = handler.performTokenExchange().Result;
            Assert.Fail();
        }

        [TestMethod()]
        [ExpectedException(typeof(AggregateException))]
        public void startTest()
        {
            GenericHandler handler = new GenericHandler("D15D6347-E3D6-4BB4-7B5632F470EBC751", "21B00A88-E976-66F2-88EA7F78281AFAE2", "10.0.2.124", "16448");
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("Sure", "Why Not");
            Task<Dictionary<string, string>> go = handler.start(parameters);
            Dictionary<string, string> res = go.Result;
            Assert.Fail();
        }

        [TestMethod()]
        [ExpectedException(typeof(AggregateException))]
        public void sendRawTest()
        {
            GenericHandler handler = new GenericHandler("D15D6347-E3D6-4BB4-7B5632F470EBC751", "21B00A88-E976-66F2-88EA7F78281AFAE2", "10.0.2.124", "16448");
            Task<Dictionary<string, string>> go = handler.sendRaw("AnyRandomString=Home+WhoKnows=IDon't");
            Dictionary<string, string> res = go.Result;
            Assert.Fail();
        }

        [TestMethod()]
        public void setIPAddressTest()
        {
            GenericHandler handler = new GenericHandler("D15D6347-E3D6-4BB4-7B5632F470EBC751", "21B00A88-E976-66F2-88EA7F78281AFAE2");
            handler.setIPAddress("10.0.2.124");
            Assert.IsNotNull(handler.getIPAddress());
        }

        [TestMethod()]
        public void getIPAddressTest()
        {
            GenericHandler handler = new GenericHandler("D15D6347-E3D6-4BB4-7B5632F470EBC751", "21B00A88-E976-66F2-88EA7F78281AFAE2");
            handler.setIPAddress("10.0.2.124");
            Assert.IsNotNull(handler.getIPAddress());
            Assert.AreEqual("10.0.2.124", handler.getIPAddress());
        }

        [TestMethod()]
        public void setPortTest()
        {
            GenericHandler handler = new GenericHandler("D15D6347-E3D6-4BB4-7B5632F470EBC751", "21B00A88-E976-66F2-88EA7F78281AFAE2");
            handler.setPort("16448");
            Assert.IsNotNull(handler.getPort());
        }

        [TestMethod()]
        public void getPortTest()
        {
            GenericHandler handler = new GenericHandler("D15D6347-E3D6-4BB4-7B5632F470EBC751", "21B00A88-E976-66F2-88EA7F78281AFAE2");
            handler.setPort("16448");
            Assert.IsNotNull(handler.getPort());
            Assert.AreEqual("16448", handler.getPort());
        }

        [TestMethod()]
        public void setClientGUIDTest()
        {
            GenericHandler handler = new GenericHandler();
            Assert.IsNull(handler.getClientGUID());
            handler.setClientGUID("21B00A88-E976-66F2-88EA7F78281AFAE2");
            Assert.IsNotNull(handler.getClientGUID());
        }

        [TestMethod()]
        public void getClientGUIDTest()
        {
            GenericHandler handler = new GenericHandler();
            Assert.IsNull(handler.getClientGUID());
            handler.setClientGUID("21B00A88-E976-66F2-88EA7F78281AFAE2");
            Assert.IsNotNull(handler.getClientGUID());
            Assert.AreEqual("21B00A88-E976-66F2-88EA7F78281AFAE2", handler.getClientGUID());
        }

        [TestMethod()]
        public void setAuthTokenTest()
        {
            GenericHandler handler = new GenericHandler();
            Assert.IsNull(handler.getAuthToken());
            handler.setAuthToken("D15D6347-E3D6-4BB4-7B5632F470EBC751");
            Assert.IsNotNull(handler.getAuthToken());
        }

        [TestMethod()]
        public void getAuthTokenTest()
        {
            GenericHandler handler = new GenericHandler();
            Assert.IsNull(handler.getAuthToken());
            handler.setAuthToken("D15D6347-E3D6-4BB4-7B5632F470EBC751");
            Assert.IsNotNull(handler.getAuthToken());
            Assert.AreEqual("D15D6347-E3D6-4BB4-7B5632F470EBC751", handler.getAuthToken());
        }

        [TestMethod()]
        public void setAccessTokenTest()
        {
            GenericHandler handler = new GenericHandler();
            Assert.IsNull(handler.getAccessToken());
            handler.setAccessToken("390B2B95-E74C-4774-A88B-C07E5B020488");
            Assert.IsNotNull(handler.getAccessToken());
        }

        [TestMethod()]
        public void getAccessTokenTest()
        {
            GenericHandler handler = new GenericHandler();
            Assert.IsNull(handler.getAccessToken());
            handler.setAccessToken("390B2B95-E74C-4774-A88B-C07E5B020488");
            Assert.IsNotNull(handler.getAccessToken());
            Assert.AreEqual("390B2B95-E74C-4774-A88B-C07E5B020488", handler.getAccessToken());
        }

        [TestMethod()]
        public void setMerchantTypeTest()
        {
            GenericHandler handler = new GenericHandler();
            Assert.IsNotNull(handler.getMerchantType());
            handler.setMerchantType("Retail");
            Assert.IsNotNull(handler.getMerchantType());
        }

        [TestMethod()]
        public void getMerchantTypeTest()
        {
            GenericHandler handler = new GenericHandler();
            Assert.IsNotNull(handler.getMerchantType());
            Assert.AreEqual("MoToEcom", handler.getMerchantType());
        }
    }
}
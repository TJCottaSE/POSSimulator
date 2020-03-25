using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using JSONConverter;
using Newtonsoft.Json.Linq;

namespace FormSim
{
    /// <summary>
    /// The RestHandler class is designed to handle FRC's over the HTTP protocol to the RESTful API. This implementation uses
    /// JSON as the payload format, so serializing and deserializing are handled here. It also acts as a translation layer
    /// between the required Dictionary of strings, and the returned JSON from the REST interface. 
    /// </summary>
    public class RestHandler : HTTPHandler, FRC_Handler
    {
        private bool ignoreSSLerrors = false;
        private readonly string BLOCK_CARD = "cards/block";
        private readonly string CARD_BLOCK_STATUS = "cards/blockstatus";
        private readonly string IDENTIFY_CARD = "cards/identify";
        private readonly string UNBLOCK_CARD = "cards/unblock";
        private readonly string VERIFY_CARD = "cards/verify";
        private readonly string ACCESS_TOKEN = "credentials/accesstoken";
        private readonly string GET_DEVICE_INFO = "devices/info";
        private readonly string INITIALIZE_READERS = "devices/initializereaders";
        private readonly string DISPLAY_LINE_ITEMS = "devices/lineitems";                       // DEPRECATED
        private readonly string DISPLAY_LINE_ITEM = "devices/lineitems/{item}";                 //  <-- LOOK
        private readonly string CLEAR_LINE_ITEMS = "devices/lineitems";
        private readonly string PRINT_RECEIPT = "devices/print";
        private readonly string PROCESS_FORMS = "devices/processform";
        private readonly string PROMPT_CONFIRMATION = "devices/promptconfirmation";
        private readonly string PROMPT_INPUT = "devices/promptinput";
        private readonly string ON_DEMAND_CARD_READ = "devices/promptcardread";
        private readonly string REQUEST_SIGNATURE = "devices/promptsignature";
        private readonly string DEVICE_RESET = "devices/reset";
        private readonly string TERMS_AND_CONDITIONS = "devices/termsandconditions";
        private readonly string GC_ACTIVATE = "giftcards/activate";
        private readonly string GC_BALANCE = "giftcards/balance";
        private readonly string GC_CANCEL = "giftcards/cancel";
        private readonly string GC_CASHOUT = "giftcards/cashout";
        private readonly string GC_DEACTIVATE = "giftcards/deactivate";
        private readonly string GC_REACTIVATE = "giftcards/reactivate";
        private readonly string GC_RELOAD = "giftcards/reload";
        private readonly string MERCHANT = "merchants/merchant";
        private readonly string GET_NEXT_INVOICE = "reports/batchdetails";
        private readonly string TOTALS_REPORT = "reports/batchtotals";
        private readonly string STATUS_REQUEST = "sessions";
        private readonly string GET_4_WORDS = "tokens/4words";
        private readonly string TOKEN_ADD = "tokens/add";
        private readonly string TOKEN_DELETE = "tokens/delete";
        private readonly string TOKEN_DUPLICATE = "tokens/duplicate";
        private readonly string AUTHORIZATION = "transactions/authorization";
        private readonly string CAPTURE = "transactions/capture";
        private readonly string CHECK = "transactions/check";
        private readonly string INVOICE = "transactions/invoice";                               // GET & DELETE
        private readonly string MANUAL_AUTHORIZATION = "transactions/manualauthorization";
        private readonly string MANUAL_SALE = "transactions/manualsale";
        private readonly string REFUND = "transactions/refund";
        private readonly string SALE = "transactions/sale";
        private readonly string SIGNATURE = "transactions/signature";
        private readonly string SESSION = "status";                                             // MAYBE DEPRECATED
        private string json_out;
        private bool headersSet;

        /******************************************************************************************************
         * Constructors                                                                                       *
         ******************************************************************************************************/
        /// <summary>
        /// Default Constructor
        /// </summary>
        public RestHandler() : base() { headersSet = false; }

        /// <summary>
        /// Optional Constructor
        /// </summary>
        /// <param name="AuthToken">The AuthToken provided by Shift4 for a given MID.</param>
        /// <param name="ClientGUID">The ClientGUID assigned by Shift4 for a given interface.</param>
        public RestHandler(string AuthToken, string ClientGUID) : base() { headersSet = false; }

        /// <summary>
        /// Optional Constructor
        /// </summary>
        /// <param name="AuthToken">The AuthToken provided by Shift4 for a given MID.</param>
        /// <param name="ClientGUID">The ClientGUID assigned by Shift4 for a given interface.</param>
        /// <param name="IPAddress">The IP address, or base URL of the Shift4 UTG.</param>
        /// <param name="Port">The port over which to communicate.</param>
        public RestHandler(string AuthToken, string ClientGUID, string IPAddress, string Port) : base() { headersSet = false; }


        /******************************************************************************************************
         * Primary Functions                                                                                  *
         ******************************************************************************************************/

        /// <summary>
        /// Performs token echange over http to the REST interface by exchanging a clientGUID and AuthToken for an AccessToken.
        /// </summary>
        public override async Task<bool> performTokenExchange()
        {
            if (!headersSet)
            {
                client.BaseAddress = new Uri(IPAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("CompanyName", "CottaTest");
                client.DefaultRequestHeaders.Add("InterfaceName", "FormSim");
                client.DefaultRequestHeaders.Add("InterfaceVersion", "1.2");
                headersSet = true;
            }
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("AuthToken", AuthToken);
            dict.Add("ClientGUID", ClientGUID);

            StringContent content = buildJsonObject(dict);
            string endPoint = ACCESS_TOKEN;
            HttpResponseMessage res = null;
            Console.WriteLine("\r\nSending: POST to " + IPAddress + endPoint + "\r\n\r\n" + client.DefaultRequestHeaders + json_out);
            log.Write("\r\nSending: POST to " + IPAddress + endPoint + "\r\n\r\n" + client.DefaultRequestHeaders + json_out);
            res = await client.PostAsync(endPoint, content);

            Console.WriteLine(res.ToString());
            Console.WriteLine(res.Content.ReadAsStringAsync().Result);

            log.Write(res.ToString());
            log.Write(res.Content.ReadAsStringAsync().Result);

            var st = res.Content.ReadAsStringAsync();
            JSONConverter.Message message = JsonConvert.DeserializeObject<JSONConverter.Message>(st.Result);
            Task<Dictionary<string, string>> task = Task.Run(() => parseToDict(message));
            try
            {
                AccessToken = message.result[0].credential.accessToken;
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// Performs a given FRC via the RESTful API, provided in the Dictionary.
        /// </summary>
        /// <param name="parameters">See <see cref="FRC_Handler.start(Dictionary{string, string})"/></param>
        /// <returns>A thread with the results of the transaction in a dictionary.</returns>
        protected override async Task<Dictionary<string, string>> performTransaction(Dictionary<string, string> parameters)
        {
            // Set the base headers
            if (!headersSet)
            {
                client.BaseAddress = new Uri(IPAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("CompanyName", "CottaTest");
                client.DefaultRequestHeaders.Add("InterfaceName", "FormSim");
                client.DefaultRequestHeaders.Add("InterfaceVersion", "1.2");
                headersSet = true;
            }

            StringContent content = buildJsonObject(parameters);
            string endPoint = this.pickEndPoint(parameters);
            string method = this.pickHttpVerb(parameters);
            HttpResponseMessage res = null;

            // Send the message, and set any http verb specific headers
            if (method == "POST")
            {
                res = sendPOST(parameters, endPoint, content).Result;
            }
            else if (method == "GET")
            {
                res = sendGET(parameters, endPoint).Result;          
            }
            else if (method == "DELETE")
            {
                res = sendDELETE(parameters, endPoint).Result;
            }

            Console.WriteLine(res.ToString());
            log.Write(res.ToString());

            string jsonBody = res.Content.ReadAsStringAsync().Result;
            JToken parsedJson = JToken.Parse(jsonBody);
            var beautified = parsedJson.ToString(Formatting.Indented);

            Console.WriteLine(beautified);
            log.Write(beautified);

            var st = res.Content.ReadAsStringAsync();
            JSONConverter.Message message = JsonConvert.DeserializeObject<JSONConverter.Message>(st.Result);
            Task<Dictionary<string, string>> task = Task.Run(() => parseToDict(message));

            return task.Result;
        }

        /// <summary>
        /// INTENTIONALLY NOT IMPLEMENTED - The REST interface now perfroms automatic voids when necessary, and employs the 
        /// http delete verb to void transactions that have directed intent. 
        /// </summary>
        /// <param name="req">A Dictionary that represent the fields that were sent in the initial request.</param>
        /// <param name="res">A Dictionary that represents the fields that were returned from the initial requst.</param>
        private Dictionary<string, string> voidTransaction(Dictionary<string, string> req, Dictionary<string, string> res)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Allows the user to pass in a raw JSON string, representing the POST body of a request. This is useful for testing results and 
        /// odd requests that a vendor my supply to us.
        /// </summary>
        /// <param name="rawString">The JSON body of a post request.</param>
        /// <param name="parameters">A Dictionary with just enough fields to indicate what endpoint to use.</param>
        /// <returns>A thread with a Dictionary of the results of the "raw" request.</returns>
        public async Task<Dictionary<string, string>> sendRaw(string rawString, Dictionary<string, string> parameters)
        {
            // Set the base headers
            if (!headersSet)
            {
                client.BaseAddress = new Uri(IPAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("CompanyName", "CottaTest");
                client.DefaultRequestHeaders.Add("InterfaceName", "FormSim");
                client.DefaultRequestHeaders.Add("InterfaceVersion", "1.2");
                headersSet = true;
            }

            StringContent content = new StringContent(rawString);

            string endPoint = this.pickEndPoint(parameters);
            string method = this.pickHttpVerb(parameters);
            HttpResponseMessage res = null;

            // Send the message, and set any http verb specific headers
            if (method == "POST")
            {
                res = sendPOST(null, endPoint, content).Result;
            }
            else if (method == "GET")
            {
                res = sendGET(parameters, endPoint).Result;
            }
            else if (method == "DELETE")
            {
                res = sendDELETE(parameters, endPoint).Result;
            }

            Console.WriteLine(res.ToString());
            Console.WriteLine(res.Content.ReadAsStringAsync().Result);

            log.Write(res.ToString());
            log.Write(res.Content.ReadAsStringAsync().Result);

            var st = res.Content.ReadAsStringAsync();
            JSONConverter.Message message = JsonConvert.DeserializeObject<JSONConverter.Message>(st.Result);
            Task<Dictionary<string, string>> task = Task.Run(() => parseToDict(message));

            return task.Result;
        }

        /// <summary>
        /// Builds the JSON object from the passed dictionary using the JSONConverter project classes.
        /// </summary>
        private StringContent buildJsonObject(Dictionary<string, string> parameters)
        {
            var message = new JSONConverter.Message();
            DateTime time = System.DateTime.Now;
            message.dateTime = time.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fff-7:00");
            // Fix for Rest Field Values
            parameters["PrimaryAmount"] = (Double.Parse(parameters["PrimaryAmount"]) - Double.Parse(parameters["TaxAmount"])).ToString();
            // Add Signature for FRC 20
            if (parameters["FunctionRequestCode"] == "20")
            {
                if (message.signature == null) { message.signature = new JSONConverter.signature(); }
                message.signature.data = "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";
                //message.signature.data = "1";
                message.signature.format = "Tr";
            }
            else if (parameters["FunctionRequestCode"] == "95" || parameters["FunctionRequestCode"] == "97")
            {
                if (message.device == null) { message.device = new JSONConverter.device(); }
                message.device.terminalID = "LocalPinPad";
                if (parameters["FunctionRequestCode"] == "95")
                    message.device.lineitems = new string[] { "RestPiano", "RestHeadphones", "RestTurnip" };
            }
            else if (parameters["FunctionRequestCode"] == "82")
            {
                if (message.device == null) { message.device = new JSONConverter.device(); }
                message.device.terminalID = "LocalPinPad";
                if (message.device.promptConfirmation == null) { message.device.promptConfirmation = new JSONConverter.promptConfirmation(); }
                message.device.promptConfirmation.question = "Are You Sure?";
                message.device.promptConfirmation.value = "Yes / No";
            }
            else if (parameters["FunctionRequestCode"] == "DB")
            {
                if (message.device == null) { message.device = new JSONConverter.device(); }
                message.device.terminalID = "LocalPinPad";
                if (message.device.promptInput == null) { message.device.promptInput = new JSONConverter.promptInput(); }
                message.device.promptInput.index = "005";
            }
            else
            {
                var keys = parameters.Keys.ToList<string>();
                for (int i = 0; i < parameters.Count; i++)
                {
                    if (keys[i] == "CardNumber")
                    {
                        if (parameters["CardDataType"] == "UnencryptedCardData")
                        {
                            if (message.card == null) { message.card = new JSONConverter.card(); }
                            message.card.number = (parameters["CardDataType"] == "UnencryptedCardData") ? parameters["CardNumber"] : null;
                            //message.card.entryMode = (parameters["CardEntryMode"] != "") ? parameters["CardEntryMode"] : null;
                        }
                    }
                    else if (keys[i] == "ExpirationDate")
                    {
                        if (parameters["CardDataType"] == "UnencryptedCardData")
                        {
                            if (message.card == null) { message.card = new JSONConverter.card(); }
                            message.card.expirationDate = Int32.Parse(parameters["ExpirationDate"]);
                        }
                    }
                    else if (keys[i] == "CVV2")
                    {
                        if (parameters["CardDataType"] == "UnencryptedCardData")
                        {
                            if (message.card == null) { message.card = new JSONConverter.card(); }
                            if (message.card.securityCode == null) { message.card.securityCode = new JSONConverter.securityCode(); }
                            message.card.securityCode.value = parameters["CVV2"];
                        }
                    }
                    else if (keys[i] == "CVV2Indicator")
                    {
                        if (parameters["CardDataType"] == "UnencryptedCardData")
                        {
                            if (message.card == null) { message.card = new JSONConverter.card(); }
                            if (message.card.securityCode == null) { message.card.securityCode = new JSONConverter.securityCode(); }
                            message.card.securityCode.indicator = parameters["CVV2Indicator"];
                        }
                    }
                    else if (keys[i] == "CardPresent")
                    {
                        if (message.card == null) { message.card = new JSONConverter.card(); }
                        message.card.present = parameters["CardPresent"];
                    }
                    else if (keys[i] == "StreetAddress")
                    {
                        if (message.customer == null) { message.customer = new JSONConverter.customer(); }
                        //message.customer.streetAddress = parameters["StreetAddress"]; -> Deprecated
                        message.customer.addressLine1 = parameters["StreetAddress"];
                    }
                    else if (keys[i] == "ZipCode")
                    {
                        if (message.customer == null) { message.customer = new JSONConverter.customer(); }
                        message.customer.postalCode = parameters["ZipCode"];
                    }
                    else if (keys[i] == "CardType")
                    {
                        if (parameters["CardDataType"] == "UnencryptedCardData" && parameters["CardType"] != "")
                        {
                            if (message.card == null) { message.card = new JSONConverter.card(); }
                            message.card.type = parameters["CardType"];
                        }
                    }
                    else if (keys[i] == "TrackData")
                    {
                        if (parameters["CardDataType"] == "UnencryptedTrackData")
                        {
                            if (message.card == null) { message.card = new JSONConverter.card(); }
                            message.card.trackData = parameters["TrackData"];
                            message.card.entryMode = "2";
                        }
                    }
                    else if (keys[i] == "UniqueID")
                    {
                        if (parameters["CardDataType"] == "TrueToken")
                        {
                            if (message.card == null) { message.card = new JSONConverter.card(); }
                            if (message.card.token == null) { message.card.token = new JSONConverter.token(); }
                            message.card.token.value = parameters["UniqueID"];
                        }
                    }
                    else if (keys[i] == "TokenSerial")
                    {
                        if (parameters["UseTokenStore"] == "Y")
                        {
                            if (message.card == null) { message.card = new JSONConverter.card(); }
                            if (message.card.token == null) { message.card.token = new JSONConverter.token(); }
                            message.card.token.serialNumber = parameters["TokenSerial"];
                        }
                    }
                    else if (keys[i] == "APIOptions")
                    {
                        message.apiOptions = parameters["APIOptions"].Split(',').ToArray<string>();
                    }
                    else if (keys[i] == "Invoice")
                    {
                        if (message.transaction == null) { message.transaction = new JSONConverter.transaction(); }
                        message.transaction.invoice = parameters["Invoice"];
                    }
                    else if (keys[i] == "PrimaryAmount")
                    {
                        if (message.amount == null) { message.amount = new JSONConverter.amount(); }
                        message.amount.total += Double.Parse(parameters["PrimaryAmount"]);
                    }
                    else if (keys[i] == "DestinationZipCode")
                    {
                        if (message.transaction == null) { message.transaction = new JSONConverter.transaction(); }
                        if (message.transaction.purchaseCard == null) { message.transaction.purchaseCard = new JSONConverter.purchaseCard(); }
                        message.transaction.purchaseCard.destinationPostalCode = parameters["DestinationZipCode"];
                        string[] a = { "Online Services", "Consulting" };
                        message.transaction.purchaseCard.productDescriptors = a;
                    }
                    else if (keys[i] == "SecondaryAmount")
                    {
                        if (message.amount == null) { message.amount = new JSONConverter.amount(); }
                        message.amount.tip = Double.Parse(parameters["SecondaryAmount"]);
                        message.amount.total += Double.Parse(parameters["SecondaryAmount"]);
                    }
                    else if (keys[i] == "TaxAmount")
                    {
                        if (message.amount == null) { message.amount = new JSONConverter.amount(); }
                        message.amount.tax += Double.Parse(parameters["TaxAmount"]);
                        message.amount.total += Double.Parse(parameters["TaxAmount"]);
                    }
                    else if (keys[i] == "TranID")
                    {
                        //message.transaction.tranID = parameters["TranID"];
                    }
                    else if (keys[i] == "Clerk")
                    {
                        if (message.clerk == null) { message.clerk = new JSONConverter.clerk(); }
                        message.clerk.numericId = Int32.Parse(parameters["Clerk"]);
                    }
                    else if (keys[i] == "TerminalID")
                    {
                        if (parameters["CardDataType"] == "UTGControlledPINPad")
                        {
                            if (message.device == null) { message.device = new JSONConverter.device(); }
                            message.device.terminalID = parameters["TerminalID"];
                        }
                    }
                    else if (keys[i] == "CustomerName")
                    {
                        if (message.customer == null) { message.customer = new JSONConverter.customer(); }
                        message.customer.firstName = parameters["CustomerName"].Split(' ')[0];
                        message.customer.lastName = parameters["CustomerName"].Split(' ')[1];
                    }
                    else if (keys[i] == "P2PEBlock")
                    {
                        if (parameters["CardDataType"] == "P2PE")
                        {
                            if (message.p2pe == null) { message.p2pe = new JSONConverter.p2pe(); }
                            if (parameters["UseEMV"] == "N")
                            {
                                message.p2pe.data = parameters["P2PEBlock"];
                                //message.card.entryMode = "2"; //                                        This probably needs some work to know what type of data is being received.
                                if (parameters["UseIDTech"] == "Y") { message.p2pe.format = "02"; }
                            }
                            else
                            {
                                if (message.emv == null) { message.emv = new JSONConverter.emv(); }
                                message.emv.tlvData = parameters["P2PEBlock"];
                                message.card.entryMode = "E";
                                message.p2pe.format = "05";
                                message.p2pe.ksn = parameters["KSN"];
                            }

                            //message.p2pe.format = "02";
                            //message.p2pe.ksn = "6299495001000000017e";
                        }
                    }
                    else if (keys[i] == "RequestorReference")
                    {
                        if (message.transaction == null) { message.transaction = new JSONConverter.transaction(); }
                        if (message.transaction.purchaseCard == null) { message.transaction.purchaseCard = new JSONConverter.purchaseCard(); }
                        message.transaction.purchaseCard.customerReference = parameters["RequestorReference"];
                    }
                    else if (keys[i] == "AuthToken")
                    {
                        if (message.credential == null) { message.credential = new JSONConverter.credential(); }
                        message.credential.authToken = parameters["AuthToken"];
                    }
                    else if (keys[i] == "ClientGUID")
                    {
                        if (message.credential == null) { message.credential = new JSONConverter.credential(); }
                        message.credential.clientGuid = parameters["ClientGUID"];
                    }
                }
            }
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
                Formatting = Newtonsoft.Json.Formatting.Indented
                //ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            };

            string content = JsonConvert.SerializeObject(message);
            json_out = content;
            var ret = new StringContent(content);
            return ret;
        }

        /// <summary>
        /// Parses the REST response body into a consumable Dictionary.
        /// </summary>
        /// <param name="message">The top level JSON Object sent or returned from Shift4's RESTful API.</param>
        /// <returns>A Dictionary representation of a response or request body from a JSON Message Object.</returns>
        private Dictionary<string, string> parseToDict(JSONConverter.Message message)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            result res = message.result[0];
            if (res.dateTime != null) { dict.Add("DateTime", res.dateTime); }
            if (res.amount != null)
            {
                dict.Add("PrimaryAmount", res.amount.total.ToString());
                if (res.amount.tax != 0) { dict.Add("TaxAmount", res.amount.tax.ToString()); }
                if (res.amount.tip != 0) { dict.Add("TipAmount", res.amount.tip.ToString()); }
                if (res.amount.surcharge != 0) { dict.Add("Surcharge", res.amount.surcharge.ToString()); }
                if (res.amount.cashback != 0) { dict.Add("CashBack", res.amount.cashback.ToString()); }
            }
            if (res.transaction != null)
            {
                dict.Add("Invoice", res.transaction.invoice);
                dict.Add("AuthorizationCode", res.transaction.authorizationCode);
                dict.Add("Response", res.transaction.responseCode);
                dict.Add("AuthSource", res.transaction.authSource);
                dict.Add("BusinessDate", res.transaction.businessDate);
                if (res.transaction.notes != null) { dict.Add("Notes", res.transaction.notes); }
                if (res.transaction.avs != null)
                {
                    dict.Add("AVSResult", res.transaction.avs.result);
                    dict.Add("StreetVerified", res.transaction.avs.streetVerified);
                    dict.Add("PostalCodeVerified", res.transaction.avs.postalCodeVerified);
                    dict.Add("ValidAVS", res.transaction.avs.valid);
                }
            }
            if (res.clerk != null) { dict.Add("Clerk", res.clerk.numericId.ToString()); }
            if (res.credential != null) { dict.Add("AccessToken", res.credential.accessToken.ToString()); }
            if (res.card != null)
            {
                dict.Add("CardNumber", res.card.number);
                dict.Add("CardPresent", res.card.present);
                dict.Add("EntryMode", res.card.entryMode);
                dict.Add("CardType", res.card.type);
                if (res.card.expirationDate != 0) { dict.Add("ExpirationDate", res.card.expirationDate.ToString()); }
                if (res.card.trackData != null) { dict.Add("TrackData", res.card.trackData.ToString()); }
                if (res.card.securityCode != null)
                {
                    dict.Add("CVV2Valid", res.card.securityCode.valid);
                    dict.Add("CVV2Result", res.card.securityCode.result);
                }
                if (res.card.token != null)
                {
                    dict.Add("UniqueID", res.card.token.value);
                    if (res.card.token.serialNumber != null) { dict.Add("TokenSerialNumber", res.card.token.serialNumber.ToString()); }
                }
                if (res.card.balance != null)
                {
                    dict.Add("CardBalance", res.card.balance.amount.ToString());
                }
            }
            if (res.customer != null)
            {
                dict.Add("CustomerName", res.customer.firstName + " " + res.customer.lastName);
                dict.Add("DestinationZipCode", res.customer.postalCode);
                dict.Add("BillingAddress", res.customer.addressLine1);
            }
            if (res.merchant != null)
            {
                if (res.merchant.name != null) { dict.Add("Merchant Name", res.merchant.name); }
                if (res.merchant.addressLine1 != null) { dict.Add("AddressLine1", res.merchant.addressLine1); }
                if (res.merchant.addressLine2 != null) { dict.Add("AddressLine2", res.merchant.addressLine2); }
                if (res.merchant.city != null) { dict.Add("City", res.merchant.city); }
                if (res.merchant.region != null) { dict.Add("Region", res.merchant.region); }
                if (res.merchant.postalCode != null) { dict.Add("PostalCode", res.merchant.postalCode); }
                if (res.merchant.phone != null) { dict.Add("Phone", res.merchant.phone); }
                if (res.merchant.dayEndingTime != null) { dict.Add("DayEndingTime", res.merchant.dayEndingTime); }
                if (res.merchant.industry != null) { dict.Add("Industry", res.merchant.industry); }
                if (res.merchant.serialNumber != null) { dict.Add("SerialNumber", res.merchant.serialNumber); }
                if (res.merchant.revision != null) { dict.Add("Revision", res.merchant.revision); }
                if (res.merchant.mid != 0) { dict.Add("MID", res.merchant.mid.ToString()); }
                if (res.merchant.cardTypes != null)
                {
                    for (int i = 0; i < res.merchant.cardTypes.Length; i++)
                    {
                        dict.Add("CardType_" + (i + 1).ToString(), res.merchant.cardTypes[i].type);
                        if (res.merchant.cardTypes[i].voiceCenter != null)
                        {
                            dict.Add("CardType_" + (i + 1).ToString() + "_Phone", res.merchant.cardTypes[i].voiceCenter.phoneNumber);
                        }
                    }
                }
            }
            if (res.receipt != null)
            {
                // Do some receipt processing.
                string recieptText = Environment.NewLine;
                for (int i = 0; i < res.receipt.Length; i++)
                {
                    recieptText += res.receipt[i].printName + " " + res.receipt[i].printValue + Environment.NewLine;
                }

                recieptText = buildReceipt(res.receipt);


                dict.Add("ReceiptText", recieptText);
            }
            if (res.device != null)
            {
                dict.Add("TerminalID", res.device.terminalID);
                if (res.device.info != null)
                {
                    string deviceInfo = Environment.NewLine;
                    for (int i = 0; i < res.device.info.Length; i++)
                    {
                        deviceInfo += res.device.info[i] + Environment.NewLine;
                    }
                    dict.Add("DeviceInfo", deviceInfo);
                }
                if (res.device.promptConfirmation != null)
                {
                    dict.Add("ConfirmationResult", res.device.promptConfirmation.result);
                }
            }
            if (res.signature != null) { dict.Add("SignatureFormat", res.signature.format.ToString()); }
            if (res.server != null) { dict.Add("Server", res.server.name); }
            if (res.error != null)
            {
                dict.Add("LongText", res.error.longText);
                dict.Add("PrimaryCode", res.error.primaryCode.ToString());
                dict.Add("SecondaryCode", res.error.secondaryCode.ToString());
                dict.Add("ShortText", res.error.shortText);
            }
            return dict;
        }

        /// <summary>
        /// Builds a receipt from the returned data that meets EMV requirements.
        /// </summary>
        /// <param name="receipt">An array of receipt items returned from Shift4.</param>
        /// <returns>A string representation of the receipt text. The receipt is left justified and 30 chars wide.</returns>
        private string buildReceipt(receipt[] receipt)
        {
            // Enumerate Keys
            string text = Environment.NewLine;
            string CVM = "";
            string[] keys = new string[500];
            for (int i = 0; i < receipt.Length; i++)
            {
                keys[i] = receipt[i].key;
            }

            // CENTER ALIGN
            // Set the Transaction Type
            if (keys.Contains("TransactionType"))
            {
                for (int j = 0; j < receipt.Length; j++)
                {
                    if (receipt[j].key == "TransactionType")
                    {
                        // Center Align Text
                        string line = "";
                        int len = receipt[j].printValue.Length;
                        int start = 15 - (len / 2);
                        for (int k = 0; k < start; k++)
                        {
                            line += " ";
                        }
                        line += receipt[j].printValue;
                        int fin = 30 - line.Length;
                        for (int m = 0; m < fin; m++)
                        {
                            line += " ";
                        }
                        text += line + Environment.NewLine;
                    }
                }
            }

            // Set the PAN Sequence Number
            if (keys.Contains("PANSequenceNumber"))
            {
                for (int j = 0; j < receipt.Length; j++)
                {
                    if (receipt[j].key == "PANSequenceNumber")
                    {
                        // Left Align Print Name
                        string line = buildLeftRightAlign(receipt[j]);
                        text += line + Environment.NewLine;
                    }
                }
            }

            // Set the Application Interchange Profile
            if (keys.Contains("ApplicationInterchangeProfile"))
            {
                for (int j = 0; j < receipt.Length; j++)
                {
                    if (receipt[j].key == "ApplicationInterchangeProfile")
                    {
                        // Left Align Print Name
                        string line = buildLeftRightAlign(receipt[j]);
                        text += line + Environment.NewLine;
                    }
                }
            }

            // Set the Transaction Date
            if (keys.Contains("TransactionDate"))
            {
                for (int j = 0; j < receipt.Length; j++)
                {
                    if (receipt[j].key == "TransactionDate")
                    {
                        // Left Align Print Name
                        string line = buildLeftRightAlign(receipt[j]);
                        text += line + Environment.NewLine;
                    }
                }
            }

            // Set the Transaction Time
            if (keys.Contains("TransactionTime"))
            {
                for (int j = 0; j < receipt.Length; j++)
                {
                    if (receipt[j].key == "TransactionTime")
                    {
                        // Left Align Print Name
                        string line = buildLeftRightAlign(receipt[j]);
                        text += line + Environment.NewLine;
                    }
                }
            }

            // Set the Amount Authorized
            if (keys.Contains("AmountAuthorized"))
            {
                for (int j = 0; j < receipt.Length; j++)
                {
                    if (receipt[j].key == "AmountAuthorized")
                    {
                        // Left Align Print Name
                        string line = buildLeftRightAlign(receipt[j]);
                        text += line + Environment.NewLine;
                    }
                }
            }

            // Set the Other Amount
            if (keys.Contains("OtherAmount"))
            {
                for (int j = 0; j < receipt.Length; j++)
                {
                    if (receipt[j].key == "OtherAmount")
                    {
                        // Left Align Print Name
                        string line = buildLeftRightAlign(receipt[j]);
                        text += line + Environment.NewLine;
                    }
                }
            }

            // Set the Application Usage Control
            if (keys.Contains("ApplicationUsageControl"))
            {
                for (int j = 0; j < receipt.Length; j++)
                {
                    if (receipt[j].key == "ApplicationUsageControl")
                    {
                        // Left Align Print Name
                        string line = buildLeftRightAlign(receipt[j]);
                        text += line + Environment.NewLine;
                    }
                }
            }

            // Set the Issuer Action Code Default
            if (keys.Contains("IssuerActionCodeDefault"))
            {
                for (int j = 0; j < receipt.Length; j++)
                {
                    if (receipt[j].key == "IssuerActionCodeDefault")
                    {
                        // Left Align Print Name
                        string line = buildLeftRightAlign(receipt[j]);
                        text += line + Environment.NewLine;
                    }
                }
            }

            // Set the Issuer Action Code Denial
            if (keys.Contains("IssuerActionCodeDenial"))
            {
                for (int j = 0; j < receipt.Length; j++)
                {
                    if (receipt[j].key == "IssuerActionCodeDenial")
                    {
                        // Left Align Print Name
                        string line = buildLeftRightAlign(receipt[j]);
                        text += line + Environment.NewLine;
                    }
                }
            }

            // Set the Issuer Action Code Online
            if (keys.Contains("IssuerActionCodeOnline"))
            {
                for (int j = 0; j < receipt.Length; j++)
                {
                    if (receipt[j].key == "IssuerActionCodeOnline")
                    {
                        // Left Align Print Name
                        string line = buildLeftRightAlign(receipt[j]);
                        text += line + Environment.NewLine;
                    }
                }
            }

            // Set the Terminal Country Code
            if (keys.Contains("TerminalCountryCode"))
            {
                for (int j = 0; j < receipt.Length; j++)
                {
                    if (receipt[j].key == "TerminalCountryCode")
                    {
                        // Left Align Print Name
                        string line = buildLeftRightAlign(receipt[j]);
                        text += line + Environment.NewLine;
                    }
                }
            }

            // Set the Application Cryptogram
            if (keys.Contains("ApplicationCryptogram"))
            {
                for (int j = 0; j < receipt.Length; j++)
                {
                    if (receipt[j].key == "ApplicationCryptogram")
                    {
                        // Left Align Print Name
                        string line = buildLeftRightAlign(receipt[j]);
                        text += line + Environment.NewLine;
                    }
                }
            }

            // Set the Cryptogram Information Data
            if (keys.Contains("CryptogramInformationData"))
            {
                for (int j = 0; j < receipt.Length; j++)
                {
                    if (receipt[j].key == "CryptogramInformationData")
                    {
                        // Left Align Print Name
                        string line = buildLeftRightAlign(receipt[j]);
                        text += line + Environment.NewLine;
                    }
                }
            }

            // Set the CVM Results
            if (keys.Contains("CVMResults"))
            {
                for (int j = 0; j < receipt.Length; j++)
                {
                    if (receipt[j].key == "CVMResults")
                    {
                        // Left Align Print Name
                        string line = buildLeftRightAlign(receipt[j]);
                        text += line + Environment.NewLine;
                    }
                }
            }

            // Set the Application Transaction Data
            if (keys.Contains("ApplicationTransactionData"))
            {
                for (int j = 0; j < receipt.Length; j++)
                {
                    if (receipt[j].key == "ApplicationTransactionData")
                    {
                        // Left Align Print Name
                        string line = buildLeftRightAlign(receipt[j]);
                        text += line + Environment.NewLine;
                    }
                }
            }

            // Set the Unpredictable Number
            if (keys.Contains("UnpredictableNumber"))
            {
                for (int j = 0; j < receipt.Length; j++)
                {
                    if (receipt[j].key == "UnpredictableNumber")
                    {
                        // Left Align Print Name
                        string line = buildLeftRightAlign(receipt[j]);
                        text += line + Environment.NewLine;
                    }
                }
            }

            // Set the Terminal Action Code Default
            if (keys.Contains("TerminalActionCodeDefault"))
            {
                for (int j = 0; j < receipt.Length; j++)
                {
                    if (receipt[j].key == "TerminalActionCodeDefault")
                    {
                        // Left Align Print Name
                        string line = buildLeftRightAlign(receipt[j]);
                        text += line + Environment.NewLine;
                    }
                }
            }

            // Set the Terminal Action Code Denial
            if (keys.Contains("TerminalActionCodeDenial"))
            {
                for (int j = 0; j < receipt.Length; j++)
                {
                    if (receipt[j].key == "TerminalActionCodeDenial")
                    {
                        // Left Align Print Name
                        string line = buildLeftRightAlign(receipt[j]);
                        text += line + Environment.NewLine;
                    }
                }
            }

            // Set the Terminal Action Code Online
            if (keys.Contains("TerminalActionCodeOnline"))
            {
                for (int j = 0; j < receipt.Length; j++)
                {
                    if (receipt[j].key == "TerminalActionCodeOnline")
                    {
                        // Left Align Print Name
                        string line = buildLeftRightAlign(receipt[j]);
                        text += line + Environment.NewLine;
                    }
                }
            }

            // Set the Card Holder Verification Method
            if (keys.Contains("CardholderVerificationMethod"))
            {
                for (int j = 0; j < receipt.Length; j++)
                {
                    if (receipt[j].key == "CardholderVerificationMethod")
                    {
                        CVM = receipt[j].printValue;
                        // Left Align Print Name
                        string line = buildLeftRightAlign(receipt[j]);
                        text += line + Environment.NewLine;
                    }
                }
            }

            // Set the Application Identifier
            if (keys.Contains("ApplicationIdentifier"))
            {
                for (int j = 0; j < receipt.Length; j++)
                {
                    if (receipt[j].key == "ApplicationIdentifier")
                    {
                        // Left Align Print Name
                        string line = buildLeftRightAlign(receipt[j]);
                        text += line + Environment.NewLine;
                    }
                }
            }

            // Set the Terminal Verificaiton Results
            if (keys.Contains("TerminalVerificationResults"))
            {
                for (int j = 0; j < receipt.Length; j++)
                {
                    if (receipt[j].key == "TerminalVerificationResults")
                    {
                        // Left Align Print Name
                        string line = buildLeftRightAlign(receipt[j]);
                        text += line + Environment.NewLine;
                    }
                }
            }

            // Set the Issuer Application Data
            if (keys.Contains("IssuerApplicationData"))
            {
                for (int j = 0; j < receipt.Length; j++)
                {
                    if (receipt[j].key == "IssuerApplicationData")
                    {
                        // Left Align Print Name
                        string line = buildLeftRightAlign(receipt[j]);
                        text += line + Environment.NewLine;
                    }
                }
            }

            // Set the Transaction Status Indicator	
            if (keys.Contains("TransactionStatusIndicator"))
            {
                for (int j = 0; j < receipt.Length; j++)
                {
                    if (receipt[j].key == "TransactionStatusIndicator")
                    {
                        // Left Align Print Name
                        string line = buildLeftRightAlign(receipt[j]);
                        text += line + Environment.NewLine;
                    }
                }
            }

            // Set the Authorization Response Code	
            if (keys.Contains("AuthorizationResponseCode"))
            {
                for (int j = 0; j < receipt.Length; j++)
                {
                    if (receipt[j].key == "AuthorizationResponseCode")
                    {
                        // Left Align Print Name
                        string line = buildLeftRightAlign(receipt[j]);
                        text += line + Environment.NewLine + Environment.NewLine;
                    }
                }
            }

            // Set the Card Entry Mode	
            if (keys.Contains("CardEntryMode"))
            {
                for (int j = 0; j < receipt.Length; j++)
                {
                    if (receipt[j].key == "CardEntryMode")
                    {
                        // Left Align Print Name
                        string line = buildLeftRightAlign(receipt[j]);
                        text += line + Environment.NewLine;
                    }
                }
            }

            // Set the Transaction Response	
            if (keys.Contains("TransactionResponse"))
            {
                for (int j = 0; j < receipt.Length; j++)
                {
                    if (receipt[j].key == "TransactionResponse")
                    {
                        // Left Align Print Name
                        string line = buildLeftRightAlign(receipt[j]);
                        text += line + Environment.NewLine;
                    }
                }
            }

            // Set the Application Label	
            if (keys.Contains("ApplicationLabel"))
            {
                for (int j = 0; j < receipt.Length; j++)
                {
                    if (receipt[j].key == "ApplicationLabel")
                    {
                        // Left Align Print Name
                        string line = buildLeftRightAlign(receipt[j]);
                        text += line + Environment.NewLine;
                    }
                }
            }

            // Set the Masked PAN	
            if (keys.Contains("MaskedPAN"))
            {
                for (int j = 0; j < receipt.Length; j++)
                {
                    if (receipt[j].key == "MaskedPAN")
                    {
                        // Left Align Print Name
                        string line = buildLeftRightAlign(receipt[j]);
                        text += line + Environment.NewLine;
                    }
                }
            }

            // Set the Amount Authorized
            if (keys.Contains("AmountAuthorized"))
            {
                for (int j = 0; j < receipt.Length; j++)
                {
                    if (receipt[j].key == "AmountAuthorized")
                    {
                        // Left Align Print Name
                        string line = "";
                        int amount = Int32.Parse(receipt[j].printValue);
                        double spend = amount / 100.0;
                        int start = 30 - spend.ToString("0.00").Length;
                        for (int k = 0; k < start - 1; k++)
                        {
                            line += " ";
                        }

                        // Go get the currency Code
                        if (keys.Contains("TransactionCurrencyCode"))
                        {
                            for (int m = 0; m < receipt.Length; m++)
                            {
                                if (receipt[m].key == "TransactionCurrencyCode")
                                {
                                    line += receipt[m].printValue;
                                }
                            }
                        }
                        else
                        {
                            line += "$";    // Assume Dollars
                        }
                        line += spend.ToString("0.00");
                        text += line + Environment.NewLine;
                    }
                }
            }

            if (CVM == "SIGN")
            {
                text += Environment.NewLine + Environment.NewLine;
                text += "______________________________";
            }

            return text;
        }

        /// <summary>
        /// Helper funciton that left justifies the printName and right justifies the printValue to 30 chars wide.
        /// </summary>
        /// <param name="receiptItem">A single line item for the receipt.</param>
        /// <returns>The properly spaced string representation of the line item.</returns>
        private string buildLeftRightAlign(receipt receiptItem)
        {
            // Left Align Print Name
            string line = (receiptItem.printName != null) ? receiptItem.printName : "";
            int len = receiptItem.printValue.Length;
            // Right Alignt Print Value
            int start = 30 - len;
            for (int k = line.Length; k < start; k++)
            {
                line += " ";
            }
            line += receiptItem.printValue;
            return line;
        }

        /// <summary>
        /// Picks the proper REST endpoint given the FRC.
        /// </summary>
        /// <param name="parameters">A Dictionary of the request fields that will be sent.</param>
        /// <returns>The string representaiton of the URI for the given FRC.</returns>
        private string pickEndPoint(Dictionary<string, string> parameters)
        {
            string endpoint = SESSION;
            string code = parameters["FunctionRequestCode"];
            if (code == "D7")
                endpoint = BLOCK_CARD;
            else if (code == "D9")
                endpoint = CARD_BLOCK_STATUS;
            else if (code == "23")
                endpoint = IDENTIFY_CARD;
            else if (code == "D8")
                endpoint = UNBLOCK_CARD;
            else if (code == "2F")
                endpoint = VERIFY_CARD;
            else if (code == "CE")
                endpoint = ACCESS_TOKEN;
            else if (code == "F2")
                endpoint = GET_DEVICE_INFO;
            else if (code == "96")
                endpoint = INITIALIZE_READERS;
            else if (code == "92")
                endpoint = DISPLAY_LINE_ITEM;   // What happened to this?
            else if (code == "94")
                endpoint = CLEAR_LINE_ITEMS;
            else if (code == "95")              // should be deprecated
                endpoint = DISPLAY_LINE_ITEMS;
            else if (code == "F1")
                endpoint = PRINT_RECEIPT;
            else if (code == "86")
                endpoint = PROCESS_FORMS;
            else if (code == "82")
                endpoint = PROMPT_CONFIRMATION;
            else if (code == "DB")
                endpoint = PROMPT_INPUT;
            else if (code == "DA")
                endpoint = ON_DEMAND_CARD_READ;
            else if (code == "47")
                endpoint = REQUEST_SIGNATURE;
            else if (code == "97")
                endpoint = DEVICE_RESET;
            else if (code == "CF")
                endpoint = TERMS_AND_CONDITIONS;
            else if (code == "24")
                endpoint = GC_ACTIVATE;
            else if (code == "61")
                endpoint = GC_BALANCE;
            else if (code == "2A")
                endpoint = GC_CANCEL;
            else if (code == "25")
            {
                endpoint = GC_DEACTIVATE;
                if (parameters["APIOptions"].Contains("GCCASHOUT"))  // Point it at the cashout endpoint when passed the appropriate API Option
                {
                    endpoint = GC_CASHOUT;
                }
            }
            else if (code == "26")
                endpoint = GC_REACTIVATE;
            else if (code == "24")
                endpoint = GC_RELOAD;
            else if (code == "0B")
                endpoint = MERCHANT;
            else if (code == "0D")
                endpoint = GET_NEXT_INVOICE;
            else if (code == "5F")
                endpoint = TOTALS_REPORT;
            else if (code == "CA")
                endpoint = STATUS_REQUEST;
            else if (code == "64")
                endpoint = GET_4_WORDS;
            else if (code == "E0")
                endpoint = TOKEN_ADD;
            else if (code == "E1")
                endpoint = TOKEN_DELETE;
            else if (code == "E2")
                endpoint = TOKEN_DUPLICATE;
            else if (code == "1B")
                endpoint = AUTHORIZATION;
            else if (code == "1F")
                endpoint = CHECK;
            else if (code == "07")
                endpoint = INVOICE;
            else if (code == "08")
                endpoint = INVOICE;
            else if (code == "05")
                endpoint = MANUAL_AUTHORIZATION;
            else if (code == "06")
                endpoint = MANUAL_SALE;
            else if (code == "1D")
            {
                if (parameters["SaleFlag"] == "S")
                {
                    if (parameters["UseAuthCapture"] == "Y")
                    {
                        endpoint = CAPTURE; // Auth Capture Model
                    }
                    else
                    {
                        endpoint = SALE; // Straight Capture Model
                    }
                }
                else
                {
                    endpoint = REFUND;
                }
            }
            else if (code == "20")
                endpoint = SIGNATURE;
            else if (code == "22")
                endpoint = MERCHANT; // <-- maybe no longer correct?
            else
                throw new Exception("Function Request Code does not have an appropriate endpoint mapping.");
            //helper.write("Using endpoint: " + endpoint);
            return endpoint;
        }

        /// <summary>
        /// Picks the proper HTTP verb (GET, POST, DELETE)
        /// </summary>
        /// <param name="parameters">A Dictionary of the request fields that will be sent.</param>
        /// <returns>The string representation of the HTTP verb to use in the request.</returns>
        private string pickHttpVerb(Dictionary<string, string> parameters)
        {
            string verb = "GET";
            if (parameters["FunctionRequestCode"] == "07" ||
                parameters["FunctionRequestCode"] == "0B" ||
                parameters["FunctionRequestCode"] == "0D" ||
                parameters["FunctionRequestCode"] == "17" ||
                parameters["FunctionRequestCode"] == "22" ||
                parameters["FunctionRequestCode"] == "5F" ||
                parameters["FunctionRequestCode"] == "CA" ||
                parameters["FunctionRequestCode"] == "D9" ||              
                parameters["FunctionRequestCode"] == "F2") /* ---> */ { verb = "GET"; }

            else if (parameters["FunctionRequestCode"] == "05" ||
                parameters["FunctionRequestCode"] == "06" ||
                parameters["FunctionRequestCode"] == "1B" ||
                parameters["FunctionRequestCode"] == "1D" ||
                parameters["FunctionRequestCode"] == "1F" ||
                parameters["FunctionRequestCode"] == "20" ||
                parameters["FunctionRequestCode"] == "23" ||
                parameters["FunctionRequestCode"] == "24" ||
                parameters["FunctionRequestCode"] == "25" ||
                parameters["FunctionRequestCode"] == "26" ||
                parameters["FunctionRequestCode"] == "2A" ||
                parameters["FunctionRequestCode"] == "2F" ||
                parameters["FunctionRequestCode"] == "47" ||
                parameters["FunctionRequestCode"] == "61" ||
                parameters["FunctionRequestCode"] == "64" ||
                parameters["FunctionRequestCode"] == "82" ||
                parameters["FunctionRequestCode"] == "86" ||
                parameters["FunctionRequestCode"] == "92" ||
                parameters["FunctionRequestCode"] == "95" ||
                parameters["FunctionRequestCode"] == "96" ||
                parameters["FunctionRequestCode"] == "97" ||
                parameters["FunctionRequestCode"] == "CE" ||
                parameters["FunctionRequestCode"] == "CF" ||
                parameters["FunctionRequestCode"] == "D7" ||
                parameters["FunctionRequestCode"] == "D8" ||
                parameters["FunctionRequestCode"] == "DA" ||
                parameters["FunctionRequestCode"] == "DB" ||
                parameters["FunctionRequestCode"] == "E0" ||
                parameters["FunctionRequestCode"] == "E2" ||
                parameters["FunctionRequestCode"] == "F1") /* ---> */ { verb = "POST"; }

            else if (parameters["FunctionRequestCode"] == "08" ||
                parameters["FunctionRequestCode"] == "94" ||
                parameters["FunctionRequestCode"] == "E1") /* ---> */ { verb = "DELETE"; }

            else
            {
                throw new Exception("Error: FRC does not have an appropriately mapped HTTP verb.");
            }

            return verb;
        }

        /// <summary>
        /// Sends a GET request with the appropriate headers
        /// </summary>
        /// <param name="parameters">A Dictionary of the request fields that will be sent.</param>
        /// <param name="endPoint">The endpoint the GET request is supposed to hit.</param>
        /// <returns>A thread with the HttpResponseMessage returned from the endpoint.</returns>
        private async Task<HttpResponseMessage> sendGET(Dictionary<string, string> parameters, string endPoint)
        {
            HttpResponseMessage res = null;
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, endPoint))
            {
                // Set the appropriate headers based on FRC
                if (parameters["FunctionRequestCode"] == "07")
                {
                    requestMessage.Headers.Add("Invoice", parameters["Invoice"]);
                }
                else if (parameters["FunctionRequestCode"] == "F2")
                {
                    requestMessage.Headers.Add("TerminalID", "LocalPinPad");
                }
                requestMessage.Headers.Add("AccessToken", AccessToken);
                Console.WriteLine("\r\nSending: GET to " + IPAddress + endPoint + "\r\n\r\n" + client.DefaultRequestHeaders + requestMessage.Headers);
                log.Write("\r\nSending: GET to " + IPAddress + endPoint + "\r\n\r\n" + client.DefaultRequestHeaders + requestMessage.Headers);
                res = await client.SendAsync(requestMessage);
            }
            return res;
        }

        /// <summary>
        /// Sends a POST request with the appropriate headers
        /// </summary>
        /// <param name="parameters">A Dictionary of the request fields that will be sent.</param>
        /// <param name="endPoint">The endpoint the POST request is supposed to hit.</param>
        /// <param name="jsonBody">The JSON body of the POST request.</param>
        /// <returns>A thread with the HttpResponseMessage returned from the endpoint.</returns>
        private async Task<HttpResponseMessage> sendPOST(Dictionary<string, string> parameters, string endPoint, StringContent jsonBody)
        {
            // This call nullifies all certificate errors ** SHOULD NOT BE USED IN PRODUCTION **
            if (ignoreSSLerrors)
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);
            }
            //System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

            HttpResponseMessage res = null;
            var httpContent = new HttpRequestMessage(HttpMethod.Post, endPoint);
            httpContent.Headers.Add("AccessToken", AccessToken);
            httpContent.Content = jsonBody;


            Console.WriteLine("\r\nSending: POST to " + IPAddress + endPoint + "\r\n" + client.DefaultRequestHeaders + httpContent.Headers);
            log.Write("\r\nSending: POST to " + IPAddress + endPoint + "\r\n" + client.DefaultRequestHeaders + httpContent.Headers);

            JToken parsedJson = JToken.Parse(jsonBody.ReadAsStringAsync().Result);
            var beautified = parsedJson.ToString(Formatting.Indented);
            Console.WriteLine("\r\n" + beautified + "\r\n\r\n");
            log.Write("\r\n" + beautified + "\r\n\r\n");

            res = await client.SendAsync(httpContent);
            return res;
        }

        /// <summary>
        /// Sends a DELETE request with the appropriate headers
        /// </summary>
        /// <param name="parameters">A Dictionary of the request fields that will be sent.</param>
        /// <param name="endPoint">The endpoint the DELETE request is supposed to hit.</param>
        /// <returns>A thread with the HttpResponseMessage returned from the endpoint.</returns>
        private async Task<HttpResponseMessage> sendDELETE(Dictionary<string, string> parameters, string endPoint)
        {
            HttpResponseMessage res = null;
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Delete, endPoint))
            {
                if (parameters["FunctionRequestCode"] == "94")
                {
                    requestMessage.Headers.Add("TerminalID", "LocalPinPad");
                }
                else
                {
                    requestMessage.Headers.Add("Invoice", parameters["Invoice"]);
                }

                requestMessage.Headers.Add("AccessToken", AccessToken);
                Console.WriteLine("\r\nSending: DELETE to " + IPAddress + endPoint + "\r\n\r\n" + client.DefaultRequestHeaders + requestMessage.Headers);
                log.Write("\r\nSending: DELETE to " + IPAddress + endPoint + "\r\n\r\n" + client.DefaultRequestHeaders + requestMessage.Headers);
                res = await client.SendAsync(requestMessage);
            }
            return res;
        }
    }
}

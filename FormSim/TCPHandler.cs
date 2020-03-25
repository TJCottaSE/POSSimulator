using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FormSim
{
    /// <summary>
    /// The TCPHandler class is designed to handle FRCs over the TCP/IP protocol. Since this
    /// protocol uses byte streams careful consideration must be made to ensure that transmission
    /// blocks are built appropraitely, and that received blocks are parsed appropriately. This 
    /// implementation handles both of those scenarios. See <see cref="GenericHandler"/> for the 
    /// best implementation strategy of dealing with FRCs in general.
    /// </summary>
    public class TCPHandler : GenericHandler, FRC_Handler
    {
        private TcpClient client;    

        /******************************************************************************************************
         * Constructors                                                                                       *
         ******************************************************************************************************/

        public TCPHandler() : base()
        {
            client = new TcpClient();
        }

        public TCPHandler(string AuthToken, string ClientGUID) : base(AuthToken, ClientGUID)
        {
            client = new TcpClient();
        }

        public TCPHandler(string AuthToken, string ClientGUID, string IPAddress, string Port) : base(AuthToken, ClientGUID, IPAddress, Port)
        {
            client = new TcpClient();
        }


        /******************************************************************************************************
         * Primary Functions                                                                                  *
         ******************************************************************************************************/

        /// <summary>
        /// Exchanges AuthToken and ClientGUID for an AccessToken that is to be used in all subsequent transactions.
        /// </summary>
        /// <returns>True if an AccessToken was received.</returns>
        public override async Task<bool> performTokenExchange()
        {
            bool success = false;
            try {
                if (IPAddress.Contains("http"))
                {
                    client.Connect(IPAddress, 80);
                }
                else
                {
                    client.Connect(IPAddress, Int32.Parse(Port));
                }
                Stream stream = client.GetStream();
                Random rand = new Random();
                // Build the message
                string message = "";
                // Include start Character
                message += (char)2;
                message += buildTransactionHeaderDataBlock("CE", rand.Next(0, 1000000).ToString(), "N", "      ", "   ", "", "          ", "          ", "");
                message += buildVendorBlock("CottaCapital:FormSim:0.2");
                message += buildAPIOptionsDataBlock("ALLDATA");
                message += buildTokenExchangeDataBlock();
                // Include end Character
                message += (char)3;
                // Include Longitudinal Redundancy Character
                message += (char)1;

                // Send the message
                ASCIIEncoding asen = new ASCIIEncoding();
                byte[] ba = asen.GetBytes(message);

                log.Write("Transmitting message: \n" + message);
                log.Write("AS: ");
                string tran = "";
                for (int u = 0; u < ba.Length; u++)
                {
                    tran += (ba[u] + " ");
                }
                log.Write(tran);
                // Write the message
                stream.Write(ba, 0, ba.Length);

                // Read the response
                byte[] bb = new byte[1024];
                int k = stream.Read(bb, 0, 1024);
                // Print the response 
                log.Write("\nToken Exchange Response Recieved");
                string res = "";
                for (int i = 0; i < k; i++)
                {
                    res += Convert.ToChar(bb[i]) + " ";
                }
                log.Write(res);
                parseTCPResponseToken(bb);
                success = true;
            }
            catch (Exception e)
            {
                log.Write("Error.... " + e.StackTrace);
            }

            return success;
        }

        /// <summary>
        /// Perfroms the FRC that has been requested
        /// </summary>
        /// <param name="parameters">The dictionary of passed parameters</param>
        /// <returns>A dictionary of return values from UTG.</returns>
        protected override async Task<Dictionary<string, string>> performTransaction(Dictionary<string, string> parameters)
        {
            Dictionary<string, string> res = new Dictionary<string, string>();
            // Removed to support API Serial MID and Password
            if (AccessToken == null)
            {
                useAPIMIDPWD = true;
                if (API_SERIAL == null || API_MID == null || API_PASSWORD == null)
                {
                    log.Write("FAIL: No Access Token or credentials");
                    throw new Exception();
                }
            }

            try
            {
                Stream stream = null;
                if (!client.Connected)
                {
                    if (IPAddress.Contains("http")) // Direct to Engine
                    {
                        client.Connect(IPAddress, 80);
                        stream = client.GetStream();
                    }
                    else if (Port == "21845") // USE SSL TLS 1.2
                    {
                        client.Connect(IPAddress, Int32.Parse(Port));
                        SslStream sslStream = new SslStream(client.GetStream());
                        sslStream.AuthenticateAsClient(IPAddress);
                        // this line has to be this way since M$ wont allow
                        // the inherited method to be called.
                        stream = sslStream;
                    }
                    else // DON'T USE SSL
                    {
                        client.Connect(IPAddress, Int32.Parse(Port));
                        stream = client.GetStream();
                    }
                }
                if (stream == null)
                { // Used to ensure subsequent requests have access to the stream
                    stream = client.GetStream();
                }

                // Build the message
                string message = "";
                // Include start Character
                message += (char)2;
                message += buildTransaction(parameters);
                message += (char)3;
                message += (char)1;

                // Send the message
                ASCIIEncoding asen = new ASCIIEncoding();
                byte[] ba = asen.GetBytes(message);

                log.Write("Sending Request: \n");
                // log out as dict
                //res = parseTCPResponse(ba);
                //log.Write(helper.parseDict(res));

                // LOG REQUEST AS INDIVIDUAL BLOCKS
                string[] messageParts = message.Split((char)028);
                for (int i = 0; i < messageParts.Length; i++)
                {
                    log.Write(messageParts[i] + "\n");
                }
                

                //  LOG REQUEST AS RAW BYTES
                /*log.Write("AS: ");
                string asout = "";
                for (int u = 0; u < ba.Length; u++)
                {
                    asout += ba[u] + " ";
                }
                log.Write(asout);
                */

                // Write the message
                stream.Write(ba, 0, ba.Length);

                // Read the response
                byte[] bb = new byte[4096];
                int k = stream.Read(bb, 0, 4096);
                // Print the response 
                log.Write("Response Recieved:");

                string result = "";
                for (int i = 0; i < k; i++)
                {
                    result += (Convert.ToChar(bb[i]) + " ");
                }
                //log.Write(result);

                res = parseTCPResponse(bb);
                log.Write(helper.parseDict(res));

            }
            catch (Exception e)
            {
                log.Write(e.StackTrace);
            }

            if (parameters["UseBasicTranFlow"] == "N") { /* Do nothing, and return the response */}
            else if (res["ErrorIndicator"] == "Y")
            {
                if (res["PrimaryErrorCode"] == "001001" || res["PrimaryErrorCode"] == "002011" || res["PrimaryErrorCode"] == "004003" || res["PrimaryErrorCode"] == "009012" ||
                    res["PrimaryErrorCode"] == "009018" || res["PrimaryErrorCode"] == "009020" || res["PrimaryErrorCode"] == "009023" || res["PrimaryErrorCode"] == "009033" ||
                    res["PrimaryErrorCode"] == "009489" || res["PrimaryErrorCode"] == "009901" || res["PrimaryErrorCode"] == "009902" || res["PrimaryErrorCode"] == "009951" ||
                    res["PrimaryErrorCode"] == "009957" || res["PrimaryErrorCode"] == "009960" || res["PrimaryErrorCode"] == "009961" || res["PrimaryErrorCode"] == "009962" ||
                    res["PrimaryErrorCode"] == "009964" || res["PrimaryErrorCode"] == "009978")
                {

                    if (parameters["FunctionRequestCode"] == "07")
                    {
                        log.Write("An error has occured with invoice # " + parameters["Invoice"]);
                        res.Add("controlerror", ("An error has occured with invoice # " + parameters["Invoice"]));
                    }
                    else
                    {
                        parameters["FunctionRequestCode"] = "07";
                        parameters["RequestorReference"] = (Int32.Parse(parameters["RequestorReference"]) + 1).ToString();
                        if (parameters["CardDataType"] == "UnencryptedCardData")
                        {
                            string card = parameters["CardNumber"];
                            char[] numbers = card.ToCharArray();
                            int size = numbers.Length;
                            parameters["CardNumber"] = numbers[size - 4].ToString() + numbers[size - 3].ToString() + numbers[size - 2].ToString() + numbers[size - 1].ToString();
                        }
                        if (parameters["CardDataType"] == "UTGControlledPINPad")
                        {
                            parameters["CardNumber"] = res["CardNumber"].TrimEnd(' ');  // The most important line right here
                            string card = parameters["CardNumber"];
                            char[] numbers = card.ToCharArray();
                            int size = numbers.Length;
                            parameters["CardNumber"] = numbers[size - 4].ToString() + numbers[size - 3].ToString() + numbers[size - 2].ToString() + numbers[size - 1].ToString();
                        }
                        System.Threading.Thread.Sleep(5000);
                        res = await startTransaction(parameters);
                        return res;
                    }

                }
                else if (res["PrimaryErrorCode"] == "009551")
                {
                    if (res["SecondaryErrorCode"] == "004")
                    {
                        log.Write("Transaction approved with no electronic signature");
                        res.Add("controlerror", "Transaction approved with no electronic signature");
                    }
                    else
                    {
                        log.Write("PIN Pad Error");
                        res.Add("controlerror", "PIN Pad Error");
                    }
                }
                else if (res["PrimaryErrorCode"] == "009402" || res["PrimaryErrorCode"] == "009501" || res["PrimaryErrorCode"] == "009775" || res["PrimaryErrorCode"] == "009819" ||
                         res["PrimaryErrorCode"] == "009825" || res["PrimaryErrorCode"] == "009836" || res["PrimaryErrorCode"] == "009847" || res["PrimaryErrorCode"] == "009864" ||
                         res["PrimaryErrorCode"] == "009955" || res["PrimaryErrorCode"] == "009956" || res["PrimaryErrorCode"] == "009992" || res["PrimaryErrorCode"] == "009999")
                {
                    log.Write("PIN Pad Error");
                    res.Add("controlerror", "PIN Pad Error");
                }
                else
                {
                    log.Write("Error Detected: " + parameters["Invoice"] + " errored with code: " + res["PrimaryErrorCode"]);
                    parameters["RequestorReference"] = (Int32.Parse(parameters["RequestorReference"]) + 1).ToString();
                    res = await startTransaction(voidTransaction(parameters, res));
                }

            }
            else
            {
                // No Error Scenario
                if (res["Response"] == "A" || res["Response"] == "C")
                {
                    // Transaction is approved .... DO Stuff
                    string cvvValid = "Y";
                    string avsValid = "Y";
                    try { cvvValid = res["CVV2Valid"]; }
                    catch (Exception e) { log.Write("cvv2valid was not returned"); }
                    try { avsValid = (res["ValidAVS"] == "N") ? "N" : "Y"; }
                    catch (Exception e) { log.Write("validavs not returned"); }

                    if (res["FunctionRequestCode"] == "08")
                    {
                        return res;
                    }
                    else if (parameters["VoidInvalidAVS"] == "Y" && res["SaleFlag"] == "S" && avsValid == "N")
                    {
                        log.Write("Voiding due to Invalid AVS");
                        parameters["RequestorReference"] = (Int32.Parse(parameters["RequestorReference"]) + 1).ToString();
                        res = await startTransaction(voidTransaction(parameters, res));
                    }
                    else if (parameters["VoidInvalidCVV2"] == "Y" && res["SaleFlag"] == "S" && cvvValid == "N")
                    {
                        log.Write("Voiding due to Invalid CVV2");
                        parameters["RequestorReference"] = (Int32.Parse(parameters["RequestorReference"]) + 1).ToString();
                        res = await startTransaction(voidTransaction(parameters, res));
                    }
                    else
                    {
                        log.Write("Processing Transaction anyway");
                        return res;
                    }
                }
                else if (res["Response"] == "D")
                {
                    // Transaction is Declined ... Void Transaction
                    log.Write("Transaction was declined");
                    parameters["RequestorReference"] = (Int32.Parse(parameters["RequestorReference"]) + 1).ToString();
                    res = await startTransaction(voidTransaction(parameters, res));
                }
                else if (res["Response"] == "R")
                {
                    // Referral, do voice or decline
                    // Doing Void ... REFERAL NOT SUPPORTED
                    log.Write("Referral Not Supported");
                    parameters["RequestorReference"] = (Int32.Parse(parameters["RequestorReference"]) + 1).ToString();
                    res = await startTransaction(voidTransaction(parameters, res));
                }
                else if (res["Response"] == "f")
                {
                    // CC Only, AVS or CVV2 failure ... Void Transaction
                    log.Write("Invalid AVS or CVV2");
                    if (parameters["VoidInvalidAVS"] == "Y" && res["ValidAVS"] == "N")
                    {
                        parameters["RequestorReference"] = (Int32.Parse(parameters["RequestorReference"]) + 1).ToString();
                        res = await startTransaction(voidTransaction(parameters, res));
                    }
                    else if (parameters["VoidInvalidCVV2"] == "Y" && res["CVV2Valid"] == "N")
                    {
                        parameters["RequestorReference"] = (Int32.Parse(parameters["RequestorReference"]) + 1).ToString();
                        res = await startTransaction(voidTransaction(parameters, res));
                    }
                    else
                    {
                        log.Write("Processing Transaction anyway");
                    }
                }
                else if (res["Response"] == "e")
                {
                    // Error condition exists
                    if (res["Authorization"] == "001001" || res["Authorization"] == "002011" || res["Authorization"] == "004003" || res["Authorization"] == "009012" ||
                         res["Authorization"] == "009018" || res["Authorization"] == "009020" || res["Authorization"] == "009023" || res["Authorization"] == "009033" ||
                         res["Authorization"] == "009489" || res["Authorization"] == "009901" || res["Authorization"] == "009902" || res["Authorization"] == "009951" ||
                         res["Authorization"] == "009957" || res["Authorization"] == "009960" || res["Authorization"] == "009961" || res["Authorization"] == "009962" ||
                         res["Authorization"] == "009964" || res["Authorization"] == "009978")
                    {
                        log.Write("An error has occured with invoice # " + parameters["Invoice"]);
                        res.Add("controlerror", ("An error has occured with invoice # " + parameters["Invoice"]));
                    }
                    else
                    {
                        // This may need help
                        log.Write("Transaction had an error");
                        parameters["RequestorReference"] = (Int32.Parse(parameters["RequestorReference"]) + 1).ToString();
                        res = await startTransaction(voidTransaction(parameters, res));
                    }
                }
                else
                {
                    // Log Transaction and notify
                    if (res["FunctionRequestCode"] == "08")
                    {
                        return res;
                    }
                    else
                    {
                        log.Write("A blank response field was received for invoice # " + parameters["Invoice"]);
                        res.Add("controlerror", "Blank Response Field. Probable Time Out");
                    }
                }
            }

            return res;
        }

        /// <summary>
        /// Build the recursive void function
        /// </summary>
        /// <param name="req">A dictionary of request parameters</param>
        /// <param name="res">A dictionary of response parameters</param>
        /// <returns>A dictionary of return values from UTG</returns>
        private Dictionary<string, string> voidTransaction(Dictionary<string, string> req, Dictionary<string, string> res)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            result.Add("FunctionRequestCode", "08");
            result.Add("Invoice", req["Invoice"]);
            result.Add("APIOptions", req["APIOptions"]);
            result.Add("Date", helper.getDate());
            result.Add("Time", helper.getTime());
            result.Add("ReceiptTextColumns", "30");
            result.Add("Vendor", "CottaCapital:FormSim:0.2");

            // TCP Specific
            result.Add("CardType", req["CardType"]);
            result.Add("CardPresent", "");
            result.Add("ExpirationDate", req["ExpirationDate"]);
            result.Add("Clerk", req["Clerk"]);
            result.Add("SaleFlag", req["SaleFlag"]);
            result.Add("PrimaryAmount", req["PrimaryAmount"]);
            result.Add("SecondaryAmount", req["SecondaryAmount"]);
            result.Add("TrackData", req["TrackData"]);

            if (req["CardNumber"] != "")
            {
                string card = req["CardNumber"];
                char[] numbers = card.ToCharArray();
                int size = numbers.Length;
                req["CardNumber"] = numbers[size - 4].ToString() + numbers[size - 3].ToString() + numbers[size - 2].ToString() + numbers[size - 1];
            }
            try
            {
                if (res["UniqueID"] == "")
                {
                    result.Add("CardNumber", req["CardNumber"]);
                }
                else
                {
                    result.Add("UniqueID", res["UniqueID"]);
                    result.Add("CardNumber", "");
                }
            }
            catch (Exception e)
            {
                result.Add("CardNumber", req["CardNumber"]);
            }
            if (req["CardDataType"] == "UTGControlledPINPad")
            {
                result.Add("TerminalID", req["TerminalID"]);
            }
            
            if (req["UseTokenStore"] == "Y")
            {
                result.Add("TokenSerial", req["TokenSerial"]);
            }

            result.Add("UseBasicTranFlow", req["UseBasicTranFlow"]);
            result.Add("TranID", res["TranID"]);
            result.Add("CardDataType", req["CardDataType"]);
            result.Add("UseRollbacks", req["UseRollbacks"]);
            result.Add("TaxIndicator", req["TaxIndicator"]);
            result.Add("TaxAmount", req["TaxAmount"]);
            result.Add("VoidInvalidAVS", req["VoidInvalidAVS"]);
            result.Add("VoidInvalidCVV2", req["VoidInvalidCVV2"]);
            result.Add("RequestorReference", (Int32.Parse(req["RequestorReference"]) + 1).ToString());
            result.Add("UseTokenStore", req["UseTokenStore"]);
            result.Add("UseMetaToken", req["UseMetaToken"]);
            return result;
        }

        /// <summary>
        /// Sends the raw string over a TCP connection bypassing all builder methods.
        /// </summary>
        /// <param name="rawData">The raw string to be chunked and sent.</param>
        /// <returns>A dictionary of return values from UTG</returns>
        public override async Task<Dictionary<string, string>> sendRaw(string rawData)
        {
            string result = "";
            Dictionary<string, string> res = new Dictionary<string, string>();
            try
            {
                if (!client.Connected)
                {
                    client.Connect(IPAddress, Int32.Parse(Port));
                }
                Stream stream = client.GetStream();
                ASCIIEncoding asen = new ASCIIEncoding();
                string rawDataUnescaped = "";
                char[] chars = rawData.ToCharArray();
                for (int i = 0; i < chars.Length; i++)
                {
                    if (chars[i] == '\\')
                    {
                        if (chars[i + 1] == 'x')
                        {
                            byte[] m = asen.GetBytes(chars, i, 4);
                            string sub = rawData.Substring(i + 2, 2);
                            try
                            {
                                //int dec = Int32.Parse(sub);
                                //char sub2 = (char)dec;
                                uint decval = System.Convert.ToUInt32(sub, 16);
                                char character = System.Convert.ToChar(decval);
                                rawDataUnescaped += character;
                                i = i + 3;
                            }
                            catch (Exception e)
                            {
                                string l = "Yikes";
                            }
                        }
                        else
                        {
                            rawDataUnescaped += chars[i];
                        }
                    }
                    else
                    {
                        rawDataUnescaped += chars[i];
                    }
                }

                byte[] ba = asen.GetBytes(rawDataUnescaped);
                log.Write("Sending raw stream: " + ba.ToString());
                stream.Write(ba, 0, ba.Length);

                byte[] br = new byte[4096];
                int k = stream.Read(br, 0, 4096);

                for (int i = 0; i < k; i++)
                {
                    result += (Convert.ToChar(br[i]) + " ");
                }
                res = parseTCPResponse(br);
            }
            catch (Exception e)
            {
                log.Write(e.StackTrace);
            }

            return res;
        }


        /******************************************************************************************************
         * Transaction / Block Builder Methods                                                                *
         ******************************************************************************************************/

        /// <summary>
        /// Selects and sets up all the blocks for each FRC
        /// </summary>
        /// <param name="parameters">A dictionary of request parameters</param>
        /// <returns>A string representation of all the blocks used in the request.</returns>
        private string buildTransaction(Dictionary<string, string> parameters)
        {
            string message = "";
            if (parameters["FunctionRequestCode"] == "1B")
            {
                // Building Online Authorization
                log.Write("Building Online Authorization (1B)");
                if (parameters["CardDataType"] == "UnencryptedCardData")
                {
                    message += buildTransactionHeaderDataBlock(parameters["FunctionRequestCode"], parameters["RequestorReference"], "", "", "", "", "", parameters["Invoice"], parameters["CardNumber"]);
                    message += buildStandardTransactionDataBlock(parameters["CardType"], "", parameters["CardPresent"], parameters["ExpirationDate"], parameters["Clerk"], helper.getDate(),
                                                             helper.getTime(), parameters["SaleFlag"], parameters["PrimaryAmount"], parameters["SecondaryAmount"], "", "", "", "", "", "",
                                                             parameters["TrackData"]);
                    message += buildCardSecurityCode(parameters["CVV2Indicator"], parameters["CVV2"], "", "");
                    message += buildAVS(parameters["CustomerName"], parameters["StreetAddress"], parameters["ZipCode"]);
                }
                else if (parameters["CardDataType"] == "UnencryptedTrackData")
                {
                    message += buildTransactionHeaderDataBlock(parameters["FunctionRequestCode"], parameters["RequestorReference"], "", "", "", "", "", parameters["Invoice"], parameters["CardNumber"]);
                    message += buildStandardTransactionDataBlock(parameters["CardType"], "", parameters["CardPresent"], parameters["ExpirationDate"], parameters["Clerk"], helper.getDate(),
                                                             helper.getTime(), parameters["SaleFlag"], parameters["PrimaryAmount"], parameters["SecondaryAmount"], "", "", "", "", "", "",
                                                             parameters["TrackData"]);
                    message += buildAVS(parameters["CustomerName"], parameters["StreetAddress"], parameters["ZipCode"]);
                }
                else if (parameters["CardDataType"] == "UTGControlledPINPad")
                {
                    message += buildTransactionHeaderDataBlock(parameters["FunctionRequestCode"], parameters["RequestorReference"], "", "", "", "", "", parameters["Invoice"], "");
                    message += buildStandardTransactionDataBlock(parameters["CardType"], "", parameters["CardPresent"], "", parameters["Clerk"], helper.getDate(),
                                                             helper.getTime(), parameters["SaleFlag"], parameters["PrimaryAmount"], parameters["SecondaryAmount"], "", "", "", "", "", "", "");
                    message += buildTerminalID(parameters["TerminalID"]);
                    //message += buildAVS(parameters["CustomerName"], parameters["StreetAddress"], parameters["ZipCode"]);
                    //message += buildAVS_CVVPrompt("Y", "N", "N");
                }
                else if (parameters["CardDataType"] == "TrueToken")
                {
                    message += buildTransactionHeaderDataBlock(parameters["FunctionRequestCode"], parameters["RequestorReference"], "", "", "", "", "", parameters["Invoice"], parameters["CardNumber"]);
                    message += buildStandardTransactionDataBlock(parameters["CardType"], "", parameters["CardPresent"], "", parameters["Clerk"], helper.getDate(),
                                                             helper.getTime(), parameters["SaleFlag"], parameters["PrimaryAmount"], parameters["SecondaryAmount"], "", "", "", "", "", "",
                                                             parameters["TrackData"]);
                    message += buildUniqueID(parameters["UniqueID"], (parameters["UseTokenStore"] == "Y") ? parameters["TokenSerial"] : "");
                }
                else if (parameters["CardDataType"] == "P2PE")
                {
                    message += buildTransactionHeaderDataBlock(parameters["FunctionRequestCode"], parameters["RequestorReference"], "", "", "", "", "", parameters["Invoice"], parameters["CardNumber"]);
                    message += buildStandardTransactionDataBlock(parameters["CardType"], "", parameters["CardPresent"], parameters["ExpirationDate"], parameters["Clerk"], helper.getDate(),
                                                             helper.getTime(), parameters["SaleFlag"], parameters["PrimaryAmount"], parameters["SecondaryAmount"], "", "", "", "", "", "",
                                                             parameters["TrackData"]);
                    message += buildP2PE(parameters["P2PEBlock"]);
                }

                message += buildVendorBlock("CottaCapital:FormSim:0.2");
                //message += buildDebitPINPad("03", "71", "01", "202438028");

                message += buildLevel2CardBlock(parameters["CustomerReference"], parameters["TaxIndicator"], parameters["TaxAmount"], parameters["DestinationZipCode"]);
                message += buildPurchasingCardBlock("Electronics", "VR Headset", "A([*?6$b&C%d#/1=2\\4|:;//@www.shift4+,'])", "");

                if (MerchantType == "Hotel")
                {
                    message += buildHotelCheckIn("10");
                }
                if (MerchantType == "Auto")
                {
                    message += buildAutoRental("7");
                }

                message += buildNotes("No Special notes!");
                message += buildAPIOptionsDataBlock(parameters["APIOptions"]);
                //--message += buildOverrideBusinessDate("070418");
                message += buildReceiptText("30", "");
                //message += buildInventoryInformationApprovalSystem("", "", "", "", "", "", "", "");

                if (parameters["UseMetaToken"] == "Y")
                {
                    message += buildMetaToken("F6", "");
                }

                if (useAPIMIDPWD) { message += buildAPISerialAndPass(API_PASSWORD, API_SERIAL); }
                else { message += buildAccessTokenDataBlock(AccessToken); }
                
            }
            else if (parameters["FunctionRequestCode"] == "1D")
            {
                // Building Online Sale
                log.Write("Building Online Sale (1D)");
                if (parameters["CardDataType"] == "UnencryptedCardData")
                {
                    message += buildTransactionHeaderDataBlock(parameters["FunctionRequestCode"], parameters["RequestorReference"], "", "", "", "", "", parameters["Invoice"], parameters["CardNumber"]);
                    message += buildStandardTransactionDataBlock(parameters["CardType"], "", parameters["CardPresent"], parameters["ExpirationDate"], parameters["Clerk"], helper.getDate(),
                                                             helper.getTime(), parameters["SaleFlag"], parameters["PrimaryAmount"], parameters["SecondaryAmount"], "", "", "", "", "", "",
                                                             parameters["TrackData"]);
                    message += buildCardSecurityCode(parameters["CVV2Indicator"], parameters["CVV2"], "", "");
                    message += buildAVS(parameters["CustomerName"], parameters["StreetAddress"], parameters["DestinationZipCode"]);
                }
                else if (parameters["CardDataType"] == "UnencryptedTrackData")
                {
                    message += buildTransactionHeaderDataBlock(parameters["FunctionRequestCode"], parameters["RequestorReference"], "", "", "", "", "", parameters["Invoice"], parameters["CardNumber"]);
                    message += buildStandardTransactionDataBlock(parameters["CardType"], "", parameters["CardPresent"], parameters["ExpirationDate"], parameters["Clerk"], helper.getDate(),
                                                             helper.getTime(), parameters["SaleFlag"], parameters["PrimaryAmount"], parameters["SecondaryAmount"], "", "", "", "", "", "",
                                                             parameters["TrackData"]);
                    message += buildAVS(parameters["CustomerName"], parameters["StreetAddress"], parameters["DestinationZipCode"]);
                }
                else if (parameters["CardDataType"] == "UTGControlledPINPad")
                {
                    message += buildTransactionHeaderDataBlock(parameters["FunctionRequestCode"], parameters["RequestorReference"], "", "", "", "", "", parameters["Invoice"], "");
                    message += buildStandardTransactionDataBlock(parameters["CardType"], "", parameters["CardPresent"], "", parameters["Clerk"], helper.getDate(),
                                                             helper.getTime(), parameters["SaleFlag"], parameters["PrimaryAmount"], parameters["SecondaryAmount"], "", "", "", "", "", "", "");
                    message += buildTerminalID(parameters["TerminalID"]);
                    //message += buildAVS(parameters["CustomerName"], parameters["StreetAddress"], parameters["DestinationZipCode"]);
                    //message += buildAVS_CVVPrompt("Y", "N", "N");
                }
                else if (parameters["CardDataType"] == "TrueToken")
                {
                    message += buildTransactionHeaderDataBlock(parameters["FunctionRequestCode"], parameters["RequestorReference"], "", "", "", "", "", parameters["Invoice"], parameters["CardNumber"]);
                    message += buildStandardTransactionDataBlock(parameters["CardType"], "", parameters["CardPresent"], "", parameters["Clerk"], helper.getDate(),
                                                             helper.getTime(), parameters["SaleFlag"], parameters["PrimaryAmount"], parameters["SecondaryAmount"], "", "", "", "", "", "",
                                                             parameters["TrackData"]);
                    message += buildUniqueID(parameters["UniqueID"], (parameters["UseTokenStore"] == "Y") ? parameters["TokenSerial"] : "");
                }
                else if (parameters["CardDataType"] == "P2PE")
                {
                    message += buildTransactionHeaderDataBlock(parameters["FunctionRequestCode"], parameters["RequestorReference"], "", "", "", "", "", parameters["Invoice"], parameters["CardNumber"]);
                    message += buildStandardTransactionDataBlock(parameters["CardType"], "", parameters["CardPresent"], parameters["ExpirationDate"], parameters["Clerk"], helper.getDate(),
                                                             helper.getTime(), parameters["SaleFlag"], parameters["PrimaryAmount"], parameters["SecondaryAmount"], "", "", "", "", "", "",
                                                             parameters["TrackData"]);
                    message += buildAVS(parameters["CustomerName"], parameters["StreetAddress"], parameters["DestinationZipCode"]);
                    message += buildP2PE(parameters["P2PEBlock"]);
                }

                message += buildVendorBlock("CottaCapital:FormSim:0.2");
                //message += buildDebitPINPad("03", "71", "01", "202438028");

                if (MerchantType == "Hotel")
                {
                    message += buildHotelCheckOut("1", "1", "", "070418", helper.getDate());
                }
                if (MerchantType == "Auto")
                {
                    message += buildAutoReturn("09541875", "Timmy Tambor", "725", "Las Vegas", "NV", "89109", helper.getDate(), "113015", "Las Vegas", "NV", "89109", helper.getDate(), "232010",
                                                "745", "Y", "");
                }

                message += buildLevel2CardBlock(parameters["CustomerReference"], parameters["TaxIndicator"], parameters["TaxAmount"], parameters["DestinationZipCode"]);
                message += buildPurchasingCardBlock("Electronics", "VR Headset", "A([*?6$b&C%d#/1=2\\4|:;//@www.shift4+,'])", "");

                if (MerchantType == "Auto")
                {
                    message += buildAutoRental("7");
                }

                message += buildNotes("No Special notes!");
                message += buildAPIOptionsDataBlock(parameters["APIOptions"]);
                //message += buildItsYourCard("", "", "", "", "", "");
                //--message += buildOverrideBusinessDate("070418");
                message += buildReceiptText("30", "");
                //message += buildInventoryInformationApprovalSystem("", "", "", "", "", "", "", "");

                if (parameters["UseMetaToken"] == "Y")
                {
                    message += buildMetaToken("IL", "");
                }

                if (useAPIMIDPWD) { message += buildAPISerialAndPass(API_PASSWORD, API_SERIAL); }
                else { message += buildAccessTokenDataBlock(AccessToken); }

            }
            else if (parameters["FunctionRequestCode"] == "05")
            {
                // Building Offline Authorization
                log.Write("Building Offline Authorization (05)");
                if (parameters["CardDataType"] == "UnencryptedCardData")
                {
                    message += buildTransactionHeaderDataBlock(parameters["FunctionRequestCode"], parameters["RequestorReference"], "", "", "", "", "", parameters["Invoice"], parameters["CardNumber"]);
                    message += buildStandardTransactionDataBlock(parameters["CardType"], "", parameters["CardPresent"], parameters["ExpirationDate"], parameters["Clerk"], helper.getDate(),
                                                             helper.getTime(), parameters["SaleFlag"], parameters["PrimaryAmount"], parameters["SecondaryAmount"], "", "123456", "", "", "", "",
                                                             parameters["TrackData"]);
                    message += buildCardSecurityCode(parameters["CVV2Indicator"], parameters["CVV2"], "", "");
                }
                else if (parameters["CardDataType"] == "UnencryptedTrackData")
                {
                    message += buildTransactionHeaderDataBlock(parameters["FunctionRequestCode"], parameters["RequestorReference"], "", "", "", "", "", parameters["Invoice"], parameters["CardNumber"]);
                    message += buildStandardTransactionDataBlock(parameters["CardType"], "", parameters["CardPresent"], parameters["ExpirationDate"], parameters["Clerk"], helper.getDate(),
                                                             helper.getTime(), parameters["SaleFlag"], parameters["PrimaryAmount"], parameters["SecondaryAmount"], "", "123456", "", "", "", "",
                                                             parameters["TrackData"]);
                }
                else if (parameters["CardDataType"] == "UTGControlledPINPad")
                {
                    message += buildTransactionHeaderDataBlock(parameters["FunctionRequestCode"], parameters["RequestorReference"], "", "", "", "", "", parameters["Invoice"], "");
                    message += buildStandardTransactionDataBlock(parameters["CardType"], "", parameters["CardPresent"], "", parameters["Clerk"], helper.getDate(),
                                                             helper.getTime(), parameters["SaleFlag"], parameters["PrimaryAmount"], parameters["SecondaryAmount"], "", "123456", "", "", "", "", "");
                    message += buildTerminalID(parameters["TerminalID"]);
                    //message += buildAVS_CVVPrompt("Y", "N", "N");
                }
                else if (parameters["CardDataType"] == "TrueToken")
                {
                    message += buildTransactionHeaderDataBlock(parameters["FunctionRequestCode"], parameters["RequestorReference"], "", "", "", "", "", parameters["Invoice"], parameters["CardNumber"]);
                    message += buildStandardTransactionDataBlock(parameters["CardType"], "", parameters["CardPresent"], "", parameters["Clerk"], helper.getDate(),
                                                             helper.getTime(), parameters["SaleFlag"], parameters["PrimaryAmount"], parameters["SecondaryAmount"], "", "123456", "", "", "", "",
                                                             parameters["TrackData"]);
                    message += buildUniqueID(parameters["UniqueID"], (parameters["UseTokenStore"] == "Y") ? parameters["TokenSerial"] : "");
                }
                else if (parameters["CardDataType"] == "P2PE")
                {
                    message += buildTransactionHeaderDataBlock(parameters["FunctionRequestCode"], parameters["RequestorReference"], "", "", "", "", "", parameters["Invoice"], parameters["CardNumber"]);
                    message += buildStandardTransactionDataBlock(parameters["CardType"], "", parameters["CardPresent"], parameters["ExpirationDate"], parameters["Clerk"], helper.getDate(),
                                                             helper.getTime(), parameters["SaleFlag"], parameters["PrimaryAmount"], parameters["SecondaryAmount"], "", "123456", "", "", "", "",
                                                             parameters["TrackData"]);
                    message += buildP2PE(parameters["P2PEBlock"]);
                }

                message += buildVendorBlock("CottaCapital:FormSim:0.2");
                //message += buildDebitPINPad("03", "71", "01", "202438028");
                message += buildLevel2CardBlock(parameters["CustomerReference"], parameters["TaxIndicator"], parameters["TaxAmount"], parameters["DestinationZipCode"]);
                message += buildPurchasingCardBlock("Electronics", "VR Headset", "A([*?6$b&C%d#/1=2\\4|:;//@www.shift4+,'])", "");

                if (MerchantType == "Hotel")
                {
                    message += buildHotelCheckIn("10");
                }
                if (MerchantType == "Auto")
                {
                    message += buildAutoRental("7");
                }

                message += buildNotes("No Special notes!");
                message += buildAPIOptionsDataBlock(parameters["APIOptions"]);
                //--message += buildOverrideBusinessDate("070418");
                message += buildReceiptText("30", "");
                //message += buildInventoryInformationApprovalSystem("", "", "", "", "", "", "", "");

                if (parameters["UseMetaToken"] == "Y")
                {
                    message += buildMetaToken("IL", "");
                }

                if (useAPIMIDPWD) { message += buildAPISerialAndPass(API_PASSWORD, API_SERIAL); }
                else { message += buildAccessTokenDataBlock(AccessToken); }
            }
            else if (parameters["FunctionRequestCode"] == "06")
            {
                // Building Offline Sale
                log.Write("Building Offline Sale (06)");
                if (parameters["CardDataType"] == "UnencryptedCardData")
                {
                    message += buildTransactionHeaderDataBlock(parameters["FunctionRequestCode"], parameters["RequestorReference"], "", "", "", "", "", parameters["Invoice"], parameters["CardNumber"]);
                    message += buildStandardTransactionDataBlock(parameters["CardType"], "", parameters["CardPresent"], parameters["ExpirationDate"], parameters["Clerk"], helper.getDate(),
                                                             helper.getTime(), parameters["SaleFlag"], parameters["PrimaryAmount"], parameters["SecondaryAmount"], "", "123456", "", "", "", "",
                                                             parameters["TrackData"]);
                    message += buildCardSecurityCode(parameters["CVV2Indicator"], parameters["CVV2"], "", "");
                    message += buildAVS(parameters["CustomerName"], parameters["StreetAddress"], parameters["ZipCode"]);
                }
                else if (parameters["CardDataType"] == "UnencryptedTrackData")
                {
                    message += buildTransactionHeaderDataBlock(parameters["FunctionRequestCode"], parameters["RequestorReference"], "", "", "", "", "", parameters["Invoice"], parameters["CardNumber"]);
                    message += buildStandardTransactionDataBlock(parameters["CardType"], "", parameters["CardPresent"], parameters["ExpirationDate"], parameters["Clerk"], helper.getDate(),
                                                             helper.getTime(), parameters["SaleFlag"], parameters["PrimaryAmount"], parameters["SecondaryAmount"], "", "123456", "", "", "", "",
                                                             parameters["TrackData"]);
                    message += buildAVS(parameters["CustomerName"], parameters["StreetAddress"], parameters["ZipCode"]);
                }
                else if (parameters["CardDataType"] == "UTGControlledPINPad")
                {
                    message += buildTransactionHeaderDataBlock(parameters["FunctionRequestCode"], parameters["RequestorReference"], "", "", "", "", "", parameters["Invoice"], parameters["CardNumber"]);
                    message += buildStandardTransactionDataBlock(parameters["CardType"], "", parameters["CardPresent"], "", parameters["Clerk"], helper.getDate(),
                                                             helper.getTime(), parameters["SaleFlag"], parameters["PrimaryAmount"], parameters["SecondaryAmount"], "", "123456", "", "", "", "", "");
                    message += buildTerminalID(parameters["TerminalID"]);
                    //message += buildAVS(parameters["CustomerName"], parameters["StreetAddress"], parameters["ZipCode"]);
                    //message += buildAVS_CVVPrompt("Y", "N", "N");
                }
                else if (parameters["CardDataType"] == "TrueToken")
                {
                    message += buildTransactionHeaderDataBlock(parameters["FunctionRequestCode"], parameters["RequestorReference"], "", "", "", "", "", parameters["Invoice"], parameters["CardNumber"]);
                    message += buildStandardTransactionDataBlock(parameters["CardType"], "", parameters["CardPresent"], "", parameters["Clerk"], helper.getDate(),
                                                             helper.getTime(), parameters["SaleFlag"], parameters["PrimaryAmount"], parameters["SecondaryAmount"], "", "123456", "", "", "", "",
                                                             parameters["TrackData"]);
                    message += buildUniqueID(parameters["UniqueID"], (parameters["UseTokenStore"] == "Y") ? parameters["TokenSerial"] : "");
                }
                else if (parameters["CardDataType"] == "P2PE")
                {
                    message += buildTransactionHeaderDataBlock(parameters["FunctionRequestCode"], parameters["RequestorReference"], "", "", "", "", "", parameters["Invoice"], parameters["CardNumber"]);
                    message += buildStandardTransactionDataBlock(parameters["CardType"], "", parameters["CardPresent"], parameters["ExpirationDate"], parameters["Clerk"], helper.getDate(),
                                                             helper.getTime(), parameters["SaleFlag"], parameters["PrimaryAmount"], parameters["SecondaryAmount"], "", "123456", "", "", "", "",
                                                             parameters["TrackData"]);
                    message += buildP2PE(parameters["P2PEBlock"]);
                    message += buildAVS(parameters["CustomerName"], parameters["StreetAddress"], parameters["ZipCode"]);
                }

                message += buildVendorBlock("CottaCapital:FormSim:0.2");


                if (MerchantType == "Hotel")
                {
                    message += buildHotelCheckOut("1", "1", "", "070418", helper.getDate());
                }
                if (MerchantType == "Auto")
                {
                    message += buildAutoReturn("09541875", "Timmy Tambor", "725", "Las Vegas", "NV", "89109", helper.getDate(), "113015", "Las Vegas", "NV", "89109", helper.getDate(), "232010",
                                            "745", "Y", "");
                }

                //message += buildDebitPINPad("03", "71", "01", "202438028");
                message += buildLevel2CardBlock(parameters["CustomerReference"], parameters["TaxIndicator"], parameters["TaxAmount"], parameters["DestinationZipCode"]);
                message += buildPurchasingCardBlock("Electronics", "VR Headset", "A([*?6$b&C%d#/1=2\\4|:;//@www.shift4+,'])", "");

                if (MerchantType == "Auto")
                {
                    message += buildAutoRental("7");
                }

                message += buildNotes("No Special notes!");
                message += buildAPIOptionsDataBlock(parameters["APIOptions"]);
                //message += buildItsYourCard("", "", "", "", "", "");
                //--message += buildOverrideBusinessDate("070418");
                message += buildReceiptText("30", "");
                //message += buildInventoryInformationApprovalSystem("", "", "", "", "", "", "", "");

                if (parameters["UseMetaToken"] == "Y")
                {
                    message += buildMetaToken("IL", "");
                }

                if (useAPIMIDPWD) { message += buildAPISerialAndPass(API_PASSWORD, API_SERIAL); }
                else { message += buildAccessTokenDataBlock(AccessToken); }
            }
            else if (parameters["FunctionRequestCode"] == "08")
            {
                // Building Void
                log.Write("Building Void (08)");
                if (parameters["UniqueID"] == "")
                {
                    int len = parameters["CardNumber"].Length;
                    char[] chars = parameters["CardNumber"].ToCharArray();
                    parameters["CardNumber"] = chars[len - 4].ToString() + chars[len - 3].ToString() + chars[len - 2].ToString() + chars[len - 1].ToString();
                }
                else { parameters["CardNumber"] = ""; }
                if (parameters["UseRollbacks"] == "Y")
                {
                    message += buildTransactionHeaderDataBlock(parameters["FunctionRequestCode"], parameters["RequestorReference"], "", "", "", "", parameters["TranID"], parameters["Invoice"], parameters["CardNumber"]);
                }
                else
                {
                    message += buildTransactionHeaderDataBlock(parameters["FunctionRequestCode"], parameters["RequestorReference"], "", "", "", "", "", parameters["Invoice"], parameters["CardNumber"]);
                }
                message += buildVendorBlock("CottaCapital:FormSim:0.2");
                message += buildStandardTransactionDataBlock("", "", "", "", "", helper.getDate(), helper.getTime(), "", "", "", "", "", "", "", "", "", "");
                string[] toBeStripped = { "ALLOWPARTIALAUTH", "BYPASSAMOUNTOK", "BYPASSSIGCAP", "NOSIGNATURE", "PRINTTIPLINE" };
                parameters = helper.stripAPIParameters(parameters, toBeStripped);
                message += buildAPIOptionsDataBlock(parameters["APIOptions"]);
                message += buildReceiptText("30", "");
                message += buildUniqueID(parameters["UniqueID"], (parameters["UseTokenStore"] == "Y") ? parameters["TokenSerial"] : "");
                if (useAPIMIDPWD) { message += buildAPISerialAndPass(API_PASSWORD, API_SERIAL); }
                else { message += buildAccessTokenDataBlock(AccessToken); }
                if (parameters["CardDataType"] == "UTGControlledPINPad")
                {
                    message += buildTerminalID(parameters["TerminalID"]);
                }
            }
            else if (parameters["FunctionRequestCode"] == "07")
            {
                // Building Get Invoice Information
                log.Write("Building Get Invoice Information (07)");
                if (parameters["UniqueID"] == "")
                {
                    int len = parameters["CardNumber"].Length;
                    char[] chars = parameters["CardNumber"].ToCharArray();
                    parameters["CardNumber"] = chars[len - 4].ToString() + chars[len - 3].ToString() + chars[len - 2].ToString() + chars[len - 1].ToString();
                }
                else { parameters["CardNumber"] = ""; }
                message += buildTransactionHeaderDataBlock(parameters["FunctionRequestCode"], parameters["RequestorReference"], "", "", "", "", "", parameters["Invoice"], parameters["CardNumber"]);
                message += buildVendorBlock("CottaCapital:FormSim:0.2");
                message += buildStandardTransactionDataBlock(parameters["CardType"], "", "", "", "", helper.getDate(), helper.getTime(), "", "", "", "", "", "", "", "", "", parameters["TrackData"]);

                // Disallows sending of ALLOWPARTIALAUTH and ENHANCEDRECEIPTS etc. on 07 Requests
                string[] options = { "ALLOWPARTIALAUTH" };
                parameters = helper.stripAPIParameters(parameters, options);
                message += buildAPIOptionsDataBlock(helper.stripAPIParameters(parameters, "ALLOWPARTIALAUTH")["APIOptions"]);
                message += buildReceiptText("30", "");
                message += buildUniqueID(parameters["UniqueID"], (parameters["UseTokenStore"] == "Y") ? parameters["TokenSerial"] : "");
                if (parameters["UseMetaToken"] == "Y")
                {
                    message += buildMetaToken("IL", "");
                }
                if (useAPIMIDPWD) { message += buildAPISerialAndPass(API_PASSWORD, API_SERIAL); }
                else { message += buildAccessTokenDataBlock(AccessToken); }
            }
            else if (parameters["FunctionRequestCode"] == "0B")
            {
                // Building Get DBA Information
                log.Write("Building Get DBA Information (0B)");
                message += buildTransactionHeaderDataBlock(parameters["FunctionRequestCode"], parameters["RequestorReference"], "", "", "", "", "", parameters["Invoice"], parameters["CardNumber"]);
                message += buildVendorBlock("CottaCapital:FormSim:0.2");
                message += buildTerminalID(parameters["TerminalID"]);                                         // Forces UTG Controlled PIN Pad
                message += buildAPIOptionsDataBlock(parameters["APIOptions"]);
                if (useAPIMIDPWD) { message += buildAPISerialAndPass(API_PASSWORD, API_SERIAL); }
                else { message += buildAccessTokenDataBlock(AccessToken); }
            }
            else if (parameters["FunctionRequestCode"] == "20")
            {
                // Building Upload Signature
                log.Write("Building Upload Signature (20)");
                message += buildTransactionHeaderDataBlock(parameters["FunctionRequestCode"], parameters["RequestorReference"], "", "", "", "", "", parameters["Invoice"], parameters["CardNumber"]);
                message += buildVendorBlock("CottaCapital:FormSim:0.2");
                message += buildStandardTransactionDataBlock(parameters["CardType"], "M", parameters["CardPresent"], parameters["ExpirationDate"], parameters["Clerk"], helper.getDate(),
                                                             helper.getTime(), parameters["SaleFlag"], parameters["PrimaryAmount"], parameters["SecondaryAmount"], "", "", "", "", "", "",
                                                             parameters["TrackData"]);
                //message += buildSignatureCapture("04", "1", "1", "");                                       // Used for Custom PIN Pads
                message += buildTerminalID(parameters["TerminalID"]);                                         // Forces UTG Controlled PIN Pad
                message += buildAPIOptionsDataBlock(parameters["APIOptions"]);
                message += buildUniqueID(parameters["UniqueID"], (parameters["UseTokenStore"] == "Y") ? parameters["TokenSerial"] : "");
                message += buildPhoto("P", "");
                if (useAPIMIDPWD) { message += buildAPISerialAndPass(API_PASSWORD, API_SERIAL); }
                else { message += buildAccessTokenDataBlock(AccessToken); }
            }
            else if (parameters["FunctionRequestCode"] == "22")
            {
                // Building Get Voice Center Information
                log.Write("Building Get Voice Center Information (22)");
                message += buildTransactionHeaderDataBlock(parameters["FunctionRequestCode"], parameters["RequestorReference"], "", "", "", "", "", "", parameters["CardNumber"]);
                message += buildVendorBlock("CottaCapital:FormSim:0.2");
                message += buildStandardTransactionDataBlock(parameters["CardType"], "", "", parameters["ExpirationDate"], "", helper.getDate(), helper.getTime(), "", "", 
                                                             parameters["SecondaryAmount"], "", "", "", "", "", "", parameters["TrackData"]);
                string[] toBeStripped = { "ALLOWPARTIALAUTH", "BYPASSAMOUNTOK", "BYPASSSIGCAP", "ENHANCEDRECEIPTS", "NOSIGNATURE", "PRINTTIPLINE" };
                parameters = helper.stripAPIParameters(parameters, toBeStripped);
                message += buildAPIOptionsDataBlock(parameters["APIOptions"]);
                message += buildUniqueID(parameters["UniqueID"], (parameters["UseTokenStore"] == "Y") ? parameters["TokenSerial"] : "");
                if (useAPIMIDPWD) { message += buildAPISerialAndPass(API_PASSWORD, API_SERIAL); }
                else { message += buildAccessTokenDataBlock(AccessToken); }
            }
            else if (parameters["FunctionRequestCode"] == "23")
            {
                // Building Identify Card Type
                log.Write("Building Identify Card Type (23)");
                message += buildTransactionHeaderDataBlock(parameters["FunctionRequestCode"], parameters["RequestorReference"], "", "", "", "", "", parameters["Invoice"], parameters["CardNumber"]);
                message += buildVendorBlock("CottaCapital:FormSim:0.2");
                message += buildStandardTransactionDataBlock(parameters["CardType"], "M", parameters["CardPresent"], parameters["ExpirationDate"], parameters["Clerk"], helper.getDate(),
                                                             helper.getTime(), parameters["SaleFlag"], parameters["PrimaryAmount"], parameters["SecondaryAmount"], "", "", "", "", "", "",
                                                             parameters["TrackData"]);
                message += buildTerminalID(parameters["TerminalID"]);                                         // Forces UTG Controlled PIN Pad
                message += buildAPIOptionsDataBlock(parameters["APIOptions"]);
                if (useAPIMIDPWD) { message += buildAPISerialAndPass(API_PASSWORD, API_SERIAL); }
                else { message += buildAccessTokenDataBlock(AccessToken); }
            }
            else if (parameters["FunctionRequestCode"] == "2F")
            {
                // Building Verify Card with Processor
                log.Write("Building Verify Card with Processor (2F)");
                message += buildTransactionHeaderDataBlock(parameters["FunctionRequestCode"], parameters["RequestorReference"], "", "", "", "", "", parameters["Invoice"], parameters["CardNumber"]);
                message += buildVendorBlock("CottaCapital:FormSim:0.2");
                message += buildStandardTransactionDataBlock(parameters["CardType"], "M", parameters["CardPresent"], parameters["ExpirationDate"], parameters["Clerk"], helper.getDate(),
                                                             helper.getTime(), parameters["SaleFlag"], parameters["PrimaryAmount"], parameters["SecondaryAmount"], "", "", "", "", "", "",
                                                             parameters["TrackData"]);
                message += buildAVS(parameters["CustomerName"], parameters["StreetAddress"], parameters["ZipCode"]);
                message += buildTerminalID(parameters["TerminalID"]);                                         // Forces UTG Controlled PIN Pad
                message += buildCardSecurityCode(parameters["CVV2Indicator"], parameters["CVV2"], "", "");
                message += buildAPIOptionsDataBlock(parameters["APIOptions"]);
                if (useAPIMIDPWD) { message += buildAPISerialAndPass(API_PASSWORD, API_SERIAL); }
                else { message += buildAccessTokenDataBlock(AccessToken); }
            }
            else if (parameters["FunctionRequestCode"] == "47")
            {
                // Building Prompt for Signature
                log.Write("Building Prompt for Signature (47)");
                message += buildTransactionHeaderDataBlock(parameters["FunctionRequestCode"], parameters["RequestorReference"], "", "", "", "", "", parameters["Invoice"], parameters["CardNumber"]);
                message += buildVendorBlock("CottaCapital:FormSim:0.2");
                message += buildTerminalID(parameters["TerminalID"]);                                         // Forces UTG Controlled PIN Pad
                message += buildAPIOptionsDataBlock(parameters["APIOptions"]);
                if (useAPIMIDPWD) { message += buildAPISerialAndPass(API_PASSWORD, API_SERIAL); }
                else { message += buildAccessTokenDataBlock(AccessToken); }
            }
            else if (parameters["FunctionRequestCode"] == "64")
            {
                // Building Get Four Words
                log.Write("Building Get Four Words (64)");
                message += buildTransactionHeaderDataBlock(parameters["FunctionRequestCode"], parameters["RequestorReference"], "", "", "", "", "", parameters["Invoice"], parameters["CardNumber"]);
                message += buildVendorBlock("CottaCapital:FormSim:0.2");
                message += buildAPIOptionsDataBlock(parameters["APIOptions"]);
                message += buildUniqueID(parameters["UniqueID"], (parameters["UseTokenStore"] == "Y") ? parameters["TokenSerial"] : "");
                if (useAPIMIDPWD) { message += buildAPISerialAndPass(API_PASSWORD, API_SERIAL); }
                else { message += buildAccessTokenDataBlock(AccessToken); }
            }
            else if (parameters["FunctionRequestCode"] == "CA")
            {
                // Building Status Request
                log.Write("Building Status Request (CA)");
                message += buildTransactionHeaderDataBlock(parameters["FunctionRequestCode"], parameters["RequestorReference"], "", "", "", "", "", parameters["Invoice"], parameters["CardNumber"]);
                message += buildVendorBlock("CottaCapital:FormSim:0.2");
                message += buildAPIOptionsDataBlock(parameters["APIOptions"]);
                if (useAPIMIDPWD) { message += buildAPISerialAndPass(API_PASSWORD, API_SERIAL); }
                else { message += buildAccessTokenDataBlock(AccessToken); }
            }
            else if (parameters["FunctionRequestCode"] == "F1")
            {
                // Building Print Receipt to PIN Pad
                log.Write("Building Print Reciept to PIN Pad (F1)");
                message += buildTransactionHeaderDataBlock(parameters["FunctionRequestCode"], parameters["RequestorReference"], "", "", "", "", "", parameters["Invoice"], parameters["CardNumber"]);
                message += buildVendorBlock("CottaCapital:FormSim:0.2");
                message += buildStandardTransactionDataBlock(parameters["CardType"], "M", parameters["CardPresent"], parameters["ExpirationDate"], parameters["Clerk"], helper.getDate(),
                                                             helper.getTime(), parameters["SaleFlag"], parameters["PrimaryAmount"], parameters["SecondaryAmount"], "", "", "", "", "", "",
                                                             parameters["TrackData"]);
                message += buildTerminalID(parameters["TerminalID"]);                                         // Forces UTG Controlled PIN Pad
                message += buildAPIOptionsDataBlock(parameters["APIOptions"]);
                message += buildReceiptText("30", "Test Receipt");
                if (useAPIMIDPWD) { message += buildAPISerialAndPass(API_PASSWORD, API_SERIAL); }
                else { message += buildAccessTokenDataBlock(AccessToken); }
            }
            else if (parameters["FunctionRequestCode"] == "F2")
            {
                // Building Get Device Information
                log.Write("Building Get Device Information (F2)");
                message += buildTransactionHeaderDataBlock(parameters["FunctionRequestCode"], parameters["RequestorReference"], "", "", "", "", "", parameters["Invoice"], parameters["CardNumber"]);
                message += buildVendorBlock("CottaCapital:FormSim:0.2");
                message += buildStandardTransactionDataBlock(parameters["CardType"], "M", parameters["CardPresent"], parameters["ExpirationDate"], parameters["Clerk"], helper.getDate(),
                                                             helper.getTime(), parameters["SaleFlag"], parameters["PrimaryAmount"], parameters["SecondaryAmount"], "", "", "", "", "", "",
                                                             parameters["TrackData"]);
                message += buildTerminalID(parameters["TerminalID"]);                                         // Forces UTG Controlled PIN Pad
                message += buildAPIOptionsDataBlock(parameters["APIOptions"]);
                if (useAPIMIDPWD) { message += buildAPISerialAndPass(API_PASSWORD, API_SERIAL); }
                else { message += buildAccessTokenDataBlock(AccessToken); }
            }
            else if (parameters["FunctionRequestCode"] == "82")
            {
                // Building Prompt For Confirmation
                log.Write("Building Prompt For Confirmation (82)");
                message += buildTransactionHeaderDataBlock(parameters["FunctionRequestCode"], parameters["RequestorReference"], "", "", "", "", "", parameters["Invoice"], parameters["CardNumber"]);
                message += buildVendorBlock("CottaCapital:FormSim:0.2");
                message += buildStandardTransactionDataBlock(parameters["CardType"], "M", parameters["CardPresent"], parameters["ExpirationDate"], parameters["Clerk"], helper.getDate(),
                                                             helper.getTime(), parameters["SaleFlag"], parameters["PrimaryAmount"], parameters["SecondaryAmount"], "", "", "", "", "", "",
                                                             parameters["TrackData"]);
                message += buildTerminalID(parameters["TerminalID"]);                                         // Forces UTG Controlled PIN Pad
                message += buildAPIOptionsDataBlock(parameters["APIOptions"]);
                message += buildPromptConfirmation("Y", "Test Question", "");
                if (useAPIMIDPWD) { message += buildAPISerialAndPass(API_PASSWORD, API_SERIAL); }
                else { message += buildAccessTokenDataBlock(AccessToken); }
            }
            else if (parameters["FunctionRequestCode"] == "86")
            {
                // Building Display Custom Form
                log.Write("Building Display Custom Form (86)");
                message += buildTransactionHeaderDataBlock(parameters["FunctionRequestCode"], parameters["RequestorReference"], "", "", "", "", "", parameters["Invoice"], parameters["CardNumber"]);
                message += buildVendorBlock("CottaCapital:FormSim:0.2");
                message += buildTerminalID(parameters["TerminalID"]);                                         // Forces UTG Controlled PIN Pad
                message += buildAPIOptionsDataBlock(parameters["APIOptions"]);
                if (useAPIMIDPWD) { message += buildAPISerialAndPass(API_PASSWORD, API_SERIAL); }
                else { message += buildAccessTokenDataBlock(AccessToken); }
                message += buildProcessForms("Special Form", "", "Key1", "", "", "", "", "", "", "", "", "");
            }
            else if (parameters["FunctionRequestCode"] == "CF")
            {
                // Building Prompt for Terms and Conditions
                log.Write("Building Prompt for Terms and Conditions (CF)");
                message += buildTransactionHeaderDataBlock(parameters["FunctionRequestCode"], parameters["RequestorReference"], "", "", "", "", "", parameters["Invoice"], parameters["CardNumber"]);
                message += buildVendorBlock("CottaCapital:FormSim:0.2");
                message += buildTerminalID(parameters["TerminalID"]);                                         // Forces UTG Controlled PIN Pad
                message += buildAPIOptionsDataBlock(parameters["APIOptions"]);
                if (useAPIMIDPWD) { message += buildAPISerialAndPass(API_PASSWORD, API_SERIAL); }
                else { message += buildAccessTokenDataBlock(AccessToken); }
                message += buildTermsAndConditions("", "See My Terms and Conditions");
            }
            else if (parameters["FunctionRequestCode"] == "DA")
            {
                // Building On-Demand Card Read
                log.Write("Building On-Demand Card Read (DA)");
                message += buildTransactionHeaderDataBlock(parameters["FunctionRequestCode"], parameters["RequestorReference"], "", "", "", "", "", parameters["Invoice"], "");
                message += buildVendorBlock("CottaCapital:FormSim:0.2");
                message += buildTerminalID(parameters["TerminalID"]);                                         // Forces UTG Controlled PIN Pad
                message += buildAPIOptionsDataBlock(parameters["APIOptions"]);
                if (useAPIMIDPWD) { message += buildAPISerialAndPass(API_PASSWORD, API_SERIAL); }
                else { message += buildAccessTokenDataBlock(AccessToken); }
            }
            else if (parameters["FunctionRequestCode"] == "DB")
            {
                // Building Prompt for Input
                log.Write("Building Prompt for Input (DB)");
                message += buildTransactionHeaderDataBlock(parameters["FunctionRequestCode"], parameters["RequestorReference"], "", "", "", "", "", parameters["Invoice"], parameters["CardNumber"]);
                message += buildVendorBlock("CottaCapital:FormSim:0.2");
                message += buildTerminalID(parameters["TerminalID"]);                                         // Forces UTG Controlled PIN Pad
                message += buildAPIOptionsDataBlock(parameters["APIOptions"]);
                if (useAPIMIDPWD) { message += buildAPISerialAndPass(API_PASSWORD, API_SERIAL); }
                else { message += buildAccessTokenDataBlock(AccessToken); }
                message += buildInputPrompt("", "");
            }
            else if (parameters["FunctionRequestCode"] == "17")
            {
                // Building Get Acceptable Identification Types for Checks
                log.Write("Building Get Acceptable Identification Types for Checks (17)");
                message += buildTransactionHeaderDataBlock(parameters["FunctionRequestCode"], parameters["RequestorReference"], "", "", "", "", "", parameters["Invoice"], parameters["CardNumber"]);
                message += buildVendorBlock("CottaCapital:FormSim:0.2");
                message += buildAPIOptionsDataBlock(parameters["APIOptions"]);
                if (useAPIMIDPWD) { message += buildAPISerialAndPass(API_PASSWORD, API_SERIAL); }
                else { message += buildAccessTokenDataBlock(AccessToken); }
            }
            else if (parameters["FunctionRequestCode"] == "1F")
            {
                // Building Check Approval
                log.Write("Building Check Approval (1F)");
                message += buildTransactionHeaderDataBlock(parameters["FunctionRequestCode"], parameters["RequestorReference"], "", "", "", "", "", parameters["Invoice"], parameters["CardNumber"]);
                message += buildVendorBlock("CottaCapital:FormSim:0.2");
                message += buildCheckApproval("1", "234239423", "121380", parameters["PrimaryAmount"], "", "1072", "", "1", "Y", "223423422", "18188299");
                message += buildAPIOptionsDataBlock(parameters["APIOptions"]);
                if (useAPIMIDPWD) { message += buildAPISerialAndPass(API_PASSWORD, API_SERIAL); }
                else { message += buildAccessTokenDataBlock(AccessToken); }
            }
            else if (parameters["FunctionRequestCode"] == "5F")
            {
                // Building Get Totals Report
                log.Write("Building Get Totals Report (5F)");
                message += buildTransactionHeaderDataBlock(parameters["FunctionRequestCode"], parameters["RequestorReference"], "", "", "", "", "", parameters["Invoice"], parameters["CardNumber"]);
                message += buildVendorBlock("CottaCapital:FormSim:0.2");
                message += buildAPIOptionsDataBlock(parameters["APIOptions"]);
                message += buildTotalsReport("070318", "120000", "080618", "123000", "0546", "", parameters["TerminalID"]);
                if (useAPIMIDPWD) { message += buildAPISerialAndPass(API_PASSWORD, API_SERIAL); }
                else { message += buildAccessTokenDataBlock(AccessToken); }
            }
            else if (parameters["FunctionRequestCode"] == "92")
            {
                // Building Display Line Item
                log.Write("Building Display Line Item (92)");
                message += buildTransactionHeaderDataBlock(parameters["FunctionRequestCode"], parameters["RequestorReference"], "", "", "", "", "", parameters["Invoice"], parameters["CardNumber"]);
                message += buildVendorBlock("CottaCapital:FormSim:0.2");
                message += buildTerminalID(parameters["TerminalID"]);                                         // Forces UTG Controlled PIN Pad
                message += buildAPIOptionsDataBlock(parameters["APIOptions"]);
                message += buildLineItem("Special Line Item");
                if (useAPIMIDPWD) { message += buildAPISerialAndPass(API_PASSWORD, API_SERIAL); }
                else { message += buildAccessTokenDataBlock(AccessToken); }
            }
            else if (parameters["FunctionRequestCode"] == "94")
            {
                // Building Clear Line Items
                log.Write("Building Clear Line Items (94)");
                message += buildTransactionHeaderDataBlock(parameters["FunctionRequestCode"], parameters["RequestorReference"], "", "", "", "", "", parameters["Invoice"], parameters["CardNumber"]);
                message += buildVendorBlock("CottaCapital:FormSim:0.2");
                message += buildTerminalID(parameters["TerminalID"]);                                         // Forces UTG Controlled PIN Pad
                message += buildAPIOptionsDataBlock(parameters["APIOptions"]);
                if (useAPIMIDPWD) { message += buildAPISerialAndPass(API_PASSWORD, API_SERIAL); }
                else { message += buildAccessTokenDataBlock(AccessToken); }
            }
            else if (parameters["FunctionRequestCode"] == "95")
            {
                // Building Display Line Items
                log.Write("Building Display Line Items (95)");
                message += buildTransactionHeaderDataBlock(parameters["FunctionRequestCode"], parameters["RequestorReference"], "", "", "", "", "", parameters["Invoice"], parameters["CardNumber"]);
                message += buildVendorBlock("CottaCapital:FormSim:0.2");
                message += buildTerminalID(parameters["TerminalID"]);                                         // Forces UTG Controlled PIN Pad
                message += buildAPIOptionsDataBlock(parameters["APIOptions"]);
                message += buildLineItems("1", "Test Line Item", "", "", "", "", "", "", "", "", "");
                if (useAPIMIDPWD) { message += buildAPISerialAndPass(API_PASSWORD, API_SERIAL); }
                else { message += buildAccessTokenDataBlock(AccessToken); }
            }
            else if (parameters["FunctionRequestCode"] == "96")
            {
                // Building Swipe Ahead
                log.Write("Building Swipe Ahead (96)");
                message += buildTransactionHeaderDataBlock(parameters["FunctionRequestCode"], parameters["RequestorReference"], "", "", "", "", "", parameters["Invoice"], parameters["CardNumber"]);
                message += buildVendorBlock("CottaCapital:FormSim:0.2");
                message += buildTerminalID(parameters["TerminalID"]);                                         // Forces UTG Controlled PIN Pad
                message += buildAPIOptionsDataBlock(parameters["APIOptions"]);
                if (useAPIMIDPWD) { message += buildAPISerialAndPass(API_PASSWORD, API_SERIAL); }
                else { message += buildAccessTokenDataBlock(AccessToken); }
            }
            else if (parameters["FunctionRequestCode"] == "97")
            {
                // Building Reset PIN Pad
                log.Write("Building Reset PIN Pad (97)");
                message += buildTransactionHeaderDataBlock(parameters["FunctionRequestCode"], parameters["RequestorReference"], "", "", "", "", "", parameters["Invoice"], parameters["CardNumber"]);
                message += buildVendorBlock("CottaCapital:FormSim:0.2");
                message += buildTerminalID(parameters["TerminalID"]);                                         // Forces UTG Controlled PIN Pad
                message += buildAPIOptionsDataBlock(parameters["APIOptions"]);
                if (useAPIMIDPWD) { message += buildAPISerialAndPass(API_PASSWORD, API_SERIAL); }
                else { message += buildAccessTokenDataBlock(AccessToken); }
            }
            else if (parameters["FunctionRequestCode"] == "24")
            {
                // Building Activate/Reload Gift Card
                log.Write("Building Activate/Reload Gift Card (24)");
                message += buildTransactionHeaderDataBlock(parameters["FunctionRequestCode"], parameters["RequestorReference"], "", "", "", "", "", parameters["Invoice"], parameters["CardNumber"]);
                message += buildVendorBlock("CottaCapital:FormSim:0.2");
                message += buildStandardTransactionDataBlock(parameters["CardType"], "M", parameters["CardPresent"], parameters["ExpirationDate"], parameters["Clerk"], helper.getDate(),
                                                             helper.getTime(), parameters["SaleFlag"], parameters["PrimaryAmount"], parameters["SecondaryAmount"], "", "", "", "", "", "",
                                                             parameters["TrackData"]);
                message += buildTerminalID(parameters["TerminalID"]);                                         // Forces UTG Controlled PIN Pad
                message += buildAPIOptionsDataBlock(parameters["APIOptions"]);
                message += buildItsYourCard("", "", "", "", "", "");
                message += buildUniqueID(parameters["UniqueID"], (parameters["UseTokenStore"] == "Y") ? parameters["TokenSerial"] : "");
                if (useAPIMIDPWD) { message += buildAPISerialAndPass(API_PASSWORD, API_SERIAL); }
                else { message += buildAccessTokenDataBlock(AccessToken); }
            }
            else if (parameters["FunctionRequestCode"] == "25")
            {
                // Building Deactivate Gift Card
                log.Write("Building Deactivate Gift Card (25)");
                message += buildTransactionHeaderDataBlock(parameters["FunctionRequestCode"], parameters["RequestorReference"], "", "", "", "", "", parameters["Invoice"], parameters["CardNumber"]);
                message += buildVendorBlock("CottaCapital:FormSim:0.2");
                message += buildStandardTransactionDataBlock(parameters["CardType"], "M", parameters["CardPresent"], parameters["ExpirationDate"], parameters["Clerk"], helper.getDate(),
                                                             helper.getTime(), parameters["SaleFlag"], parameters["PrimaryAmount"], parameters["SecondaryAmount"], "", "", "", "", "", "",
                                                             parameters["TrackData"]);
                message += buildTerminalID(parameters["TerminalID"]);                                         // Forces UTG Controlled PIN Pad
                message += buildAPIOptionsDataBlock(parameters["APIOptions"]);
                message += buildItsYourCard("", "", "", "", "", "");
                message += buildItsYourCardReasonText("Test Reason");
                message += buildUniqueID(parameters["UniqueID"], (parameters["UseTokenStore"] == "Y") ? parameters["TokenSerial"] : "");
                if (useAPIMIDPWD) { message += buildAPISerialAndPass(API_PASSWORD, API_SERIAL); }
                else { message += buildAccessTokenDataBlock(AccessToken); }
            }
            else if (parameters["FunctionRequestCode"] == "26")
            {
                // Building Reactivate Gift Card
                log.Write("Building Reactivate Gift Card (26)");
                message += buildTransactionHeaderDataBlock(parameters["FunctionRequestCode"], parameters["RequestorReference"], "", "", "", "", "", parameters["Invoice"], parameters["CardNumber"]);
                message += buildVendorBlock("CottaCapital:FormSim:0.2");
                message += buildStandardTransactionDataBlock(parameters["CardType"], "M", parameters["CardPresent"], parameters["ExpirationDate"], parameters["Clerk"], helper.getDate(),
                                                             helper.getTime(), parameters["SaleFlag"], parameters["PrimaryAmount"], parameters["SecondaryAmount"], "", "", "", "", "", "",
                                                             parameters["TrackData"]);
                message += buildTerminalID(parameters["TerminalID"]);                                         // Forces UTG Controlled PIN Pad
                message += buildAPIOptionsDataBlock(parameters["APIOptions"]);
                message += buildItsYourCard("", "", "", "", "", "");
                message += buildUniqueID(parameters["UniqueID"], (parameters["UseTokenStore"] == "Y") ? parameters["TokenSerial"] : "");
                if (useAPIMIDPWD) { message += buildAPISerialAndPass(API_PASSWORD, API_SERIAL); }
                else { message += buildAccessTokenDataBlock(AccessToken); }
            }
            else if (parameters["FunctionRequestCode"] == "61")
            {
                // Building Get Balance Inquiry
                log.Write("Building Get Balance Inquiry (61)");
                message += buildTransactionHeaderDataBlock(parameters["FunctionRequestCode"], parameters["RequestorReference"], "", "", "", "", "", parameters["Invoice"], parameters["CardNumber"]);
                message += buildVendorBlock("CottaCapital:FormSim:0.2");
                message += buildStandardTransactionDataBlock(parameters["CardType"], "M", parameters["CardPresent"], parameters["ExpirationDate"], parameters["Clerk"], helper.getDate(),
                                                             helper.getTime(), parameters["SaleFlag"], parameters["PrimaryAmount"], parameters["SecondaryAmount"], "", "", "", "", "", "",
                                                             parameters["TrackData"]);
                message += buildTerminalID(parameters["TerminalID"]);                                         // Forces UTG Controlled PIN Pad
                message += buildAPIOptionsDataBlock(parameters["APIOptions"]);
                message += buildItsYourCard("", "", "", "", "", "");
                message += buildUniqueID(parameters["UniqueID"], (parameters["UseTokenStore"] == "Y") ? parameters["TokenSerial"] : "");
                if (useAPIMIDPWD) { message += buildAPISerialAndPass(API_PASSWORD, API_SERIAL); }
                else { message += buildAccessTokenDataBlock(AccessToken); }
            }
            else if (parameters["FunctionRequestCode"] == "E0")
            {
                // Building TokenStore Add
                log.Write("Building TokenStore Add (E0)");
                if (parameters["CardDataType"] == "UnencryptedCardData")
                {
                    message += buildTransactionHeaderDataBlock(parameters["FunctionRequestCode"], parameters["RequestorReference"], "", "", "", "", "", "", parameters["CardNumber"]);
                    message += buildStandardTransactionDataBlock(parameters["CardType"], "M", "", parameters["ExpirationDate"], "", helper.getDate(), helper.getTime(), "", "", "", "", "", "", "", "", "", parameters["TrackData"]);
                    message += buildCardSecurityCode(parameters["CVV2Indicator"], parameters["CVV2"], "", "");
                }
                else if (parameters["CardDataType"] == "UnencryptedTrackData")
                {
                    message += buildTransactionHeaderDataBlock(parameters["FunctionRequestCode"], parameters["RequestorReference"], "", "", "", "", "", parameters["Invoice"], parameters["CardNumber"]);
                    message += buildStandardTransactionDataBlock(parameters["CardType"], "M", "", parameters["ExpirationDate"], "", helper.getDate(), helper.getTime(), "", "", "", "", "", "", "", "", "", parameters["TrackData"]);
                }
                else if (parameters["CardDataType"] == "UTGControlledPINPad")
                {
                    message += buildTransactionHeaderDataBlock(parameters["FunctionRequestCode"], parameters["RequestorReference"], "", "", "", "", "", parameters["Invoice"], parameters["CardNumber"]);
                    message += buildStandardTransactionDataBlock(parameters["CardType"], "M", "", parameters["ExpirationDate"], "", helper.getDate(), helper.getTime(), "", "", "", "", "", "", "", "", "", parameters["TrackData"]);
                    message += buildTerminalID(parameters["TerminalID"]);
                }
                else if (parameters["CardDataType"] == "TrueToken")
                {
                    message += buildTransactionHeaderDataBlock(parameters["FunctionRequestCode"], parameters["RequestorReference"], "", "", "", "", "", parameters["Invoice"], parameters["CardNumber"]);
                    message += buildStandardTransactionDataBlock(parameters["CardType"], "M", "", parameters["ExpirationDate"], "", helper.getDate(), helper.getTime(), "", "", "", "", "", "", "", "", "", parameters["TrackData"]);
                    message += buildUniqueID(parameters["UniqueID"], (parameters["UseTokenStore"] == "Y") ? parameters["TokenSerial"] : "");
                }
                else if (parameters["CardDataType"] == "P2PE")
                {
                    message += buildTransactionHeaderDataBlock(parameters["FunctionRequestCode"], parameters["RequestorReference"], "", "", "", "", "", parameters["Invoice"], parameters["CardNumber"]);
                    message += buildStandardTransactionDataBlock(parameters["CardType"], "M", "", parameters["ExpirationDate"], "", helper.getDate(), helper.getTime(), "", "", "", "", "", "", "", "", "", parameters["TrackData"]);
                    message += buildP2PE(parameters["P2PEBlock"]);
                }
                message += buildVendorBlock("CottaCapital:FormSim:0.2");
                message += buildAVS(parameters["CustomerName"], parameters["StreetAddress"], parameters["ZipCode"]);
                message += buildAPIOptionsDataBlock(parameters["APIOptions"]);

                if (parameters["UseMetaToken"] == "Y")
                {
                    message += buildMetaToken("IL", "");
                }
                if (useAPIMIDPWD) { message += buildAPISerialAndPass(API_PASSWORD, API_SERIAL); }
                else { message += buildAccessTokenDataBlock(AccessToken); }
            }
            else if (parameters["FunctionRequestCode"] == "E2")
            {
                // Building TokenStore Duplicate
                log.Write("Building TokenStore Duplicate (E2)");
                message += buildTransactionHeaderDataBlock(parameters["FunctionRequestCode"], parameters["RequestorReference"], "", "", "", "", "", parameters["Invoice"], parameters["CardNumber"]);
                message += buildVendorBlock("CottaCapital:FormSim:0.2");
                message += buildCardSecurityCode(parameters["CVV2Indicator"], parameters["CVV2"], "", "");
                message += buildAPIOptionsDataBlock(parameters["APIOptions"]);
                message += buildUniqueID(parameters["UniqueID"], (parameters["UseTokenStore"] == "Y") ? parameters["TokenSerial"] : "");
                if (parameters["UseMetaToken"] == "Y")
                {
                    message += buildMetaToken("IL", "");
                }
                if (useAPIMIDPWD) { message += buildAPISerialAndPass(API_PASSWORD, API_SERIAL); }
                else { message += buildAccessTokenDataBlock(AccessToken); }
            }
            else if (parameters["FunctionRequestCode"] == "D7")
            {
                // Building Block Card
                log.Write("Building Block Card (D7)");
                message += buildTransactionHeaderDataBlock(parameters["FunctionRequestCode"], parameters["RequestorReference"], "", "", "", "", "", parameters["Invoice"], parameters["CardNumber"]);
                message += buildVendorBlock("CottaCapital:FormSim:0.2");
                message += buildStandardTransactionDataBlock(parameters["CardType"], "M", parameters["CardPresent"], parameters["ExpirationDate"], parameters["Clerk"], helper.getDate(),
                                                             helper.getTime(), parameters["SaleFlag"], parameters["PrimaryAmount"], parameters["SecondaryAmount"], "", "", "", "", "", "",
                                                             parameters["TrackData"]);
                message += buildAPIOptionsDataBlock(parameters["APIOptions"]);
                message += buildUniqueID(parameters["UniqueID"], (parameters["UseTokenStore"] == "Y") ? parameters["TokenSerial"] : "");
                if (useAPIMIDPWD) { message += buildAPISerialAndPass(API_PASSWORD, API_SERIAL); }
                else { message += buildAccessTokenDataBlock(AccessToken); }
            }
            else if (parameters["FunctionRequestCode"] == "D8")
            {
                // Building Unblock Card
                log.Write("Building Unblock Card (D8)");
                message += buildTransactionHeaderDataBlock(parameters["FunctionRequestCode"], parameters["RequestorReference"], "", "", "", "", "", parameters["Invoice"], parameters["CardNumber"]);
                message += buildVendorBlock("CottaCapital:FormSim:0.2");
                message += buildStandardTransactionDataBlock(parameters["CardType"], "M", parameters["CardPresent"], parameters["ExpirationDate"], parameters["Clerk"], helper.getDate(),
                                                             helper.getTime(), parameters["SaleFlag"], parameters["PrimaryAmount"], parameters["SecondaryAmount"], "", "", "", "", "", "",
                                                             parameters["TrackData"]);
                message += buildAPIOptionsDataBlock(parameters["APIOptions"]);
                message += buildUniqueID(parameters["UniqueID"], (parameters["UseTokenStore"] == "Y") ? parameters["TokenSerial"] : "");
                if (useAPIMIDPWD) { message += buildAPISerialAndPass(API_PASSWORD, API_SERIAL); }
                else { message += buildAccessTokenDataBlock(AccessToken); }
            }
            else if (parameters["FunctionRequestCode"] == "D9")
            {
                // Building Card Block Status
                log.Write("Building Card Block Status (D9)");
                message += buildTransactionHeaderDataBlock(parameters["FunctionRequestCode"], parameters["RequestorReference"], "", "", "", "", "", parameters["Invoice"], parameters["CardNumber"]);
                message += buildVendorBlock("CottaCapital:FormSim:0.2");
                message += buildStandardTransactionDataBlock(parameters["CardType"], "M", parameters["CardPresent"], parameters["ExpirationDate"], parameters["Clerk"], helper.getDate(),
                                                             helper.getTime(), parameters["SaleFlag"], parameters["PrimaryAmount"], parameters["SecondaryAmount"], "", "", "", "", "", "",
                                                             parameters["TrackData"]);
                message += buildAPIOptionsDataBlock(parameters["APIOptions"]);
                message += buildUniqueID(parameters["UniqueID"], (parameters["UseTokenStore"] == "Y") ? parameters["TokenSerial"] : "");
                if (useAPIMIDPWD) { message += buildAPISerialAndPass(API_PASSWORD, API_SERIAL); }
                else { message += buildAccessTokenDataBlock(AccessToken); }
            }
            else if (parameters["FunctionRequestCode"] == "CD")
            {
                // Building Get MetaToken
                log.Write("Building Get MetaToken (CD)");
                message += buildTransactionHeaderDataBlock(parameters["FunctionRequestCode"], parameters["RequestorReference"], "", "", "", "", "", parameters["Invoice"], parameters["CardNumber"]);
                message += buildVendorBlock("CottaCapital:FormSim:0.2");
                message += buildStandardTransactionDataBlock(parameters["CardType"], "M", parameters["CardPresent"], parameters["ExpirationDate"], parameters["Clerk"], helper.getDate(),
                                                             helper.getTime(), parameters["SaleFlag"], parameters["PrimaryAmount"], parameters["SecondaryAmount"], "", "", "", "", "", "",
                                                             parameters["TrackData"]);
                message += buildAPIOptionsDataBlock(parameters["APIOptions"]);
                message += buildUniqueID(parameters["UniqueID"], (parameters["UseTokenStore"] == "Y") ? parameters["TokenSerial"] : "");
                message += buildP2PE(parameters["P2PEBlock"]);
                if (parameters["UseMetaToken"] == "Y")
                {
                    message += buildMetaToken("IL", "");
                }
                if (useAPIMIDPWD) { message += buildAPISerialAndPass(API_PASSWORD, API_SERIAL); }
                else { message += buildAccessTokenDataBlock(AccessToken); }
            }

            return message;
        }

        /// <summary>
        /// - Transaction Header Data Block -
        /// NOTE: Sending values short of the space requirements will be right padded with spaces, Values in excess will be truncated to space requirements.
        /// WARNING: Sending of the MerchantID is required for this method, but not for transmission.
        /// The MerchantID field has been replaced with the Vendor Data Block.
        /// </summary>
        /// <param name="FunctionRequestCode">The alphanumeric code for the function to be performed (2-spaces)</param>
        /// <param name="RequestorReference">Unique alphanumeric descriptor to facilitate your interface matching up requests with responses (12-spaces)</param>
        /// <param name="ErrorIndicator">"Y" or "N" to indicate an error condition (1-space)</param>
        /// <param name="PrimaryErrorCode">Code for primary error (6-spaces)</param>
        /// <param name="SecondaryErrorCode">Code for secondary error (3-spaces)</param>
        /// <param name="MerchantID">The Shift4 Merchant ID, or all 0's when using an AccessToken</param>
        /// <param name="TranID">The transaction ID</param>
        /// <param name="Invoice">The Invoice number</param>
        /// <param name="CardNumber">The card number to be processed ** Note Padding and Truncation do NOT apply ** (up to 32-spaces)</param>
        /// <returns>The string representation of the transaction header data block</returns>
        private string buildTransactionHeaderDataBlock(string FunctionRequestCode, string RequestorReference, string ErrorIndicator, string PrimaryErrorCode,
                                                        string SecondaryErrorCode, string MerchantID, string TranID, string Invoice, string CardNumber)
        {
            string ret = "$";                                           // API Signature "generally always $"
            ret += "0";                                                 // API Format "generally always 0"
            ret += FunctionRequestCode;
            ret += normalize(RequestorReference, 12);                   // Unique 12-char alphanumeric descriptor
            ret += normalize(ErrorIndicator, 1);                        // One Space
            ret += normalize(PrimaryErrorCode, 6);                      // Six Spaces
            ret += normalize(SecondaryErrorCode, 3);                    // Three Spaces
            ret += (useAPIMIDPWD) ? leftPad(API_MID, 10) : "0000000000";// DEPRECATED: Ten 0's - MerchantID
            ret += normalize(TranID, 10);                               // Ten Spaces
            ret += normalize(Invoice, 10);                              // Ten Spaces
            ret += CardNumber;                                          // Up To 32 chars
            return ret;
        }

        /// <summary>
        /// - 000 Vendor Description Data Block - 
        /// Used to describe the Vendor.Vendor Descriptions will be truncated beyond the 64 byte size.
        /// </summary>
        /// <param name="VendorDescription">Description of Vendor (UP to 64-spaces)</param>
        /// <returns>String representation of the Vendor Block</returns>
        private string buildVendorBlock(string VendorDescription)
        {
            string ret = "";
            ret += (char)028;                                           // Field Seperator
            ret += "000";                                               // Data Block Indicator
            ret += truncate(VendorDescription, 64);                     // Up to 64 Characters
            return ret;
        }

        /// <summary>
        /// - 001 Standard Tansaction Data Block -
        /// Used to send standard transactions with transaction information.
        /// </summary>
        /// <param name="CardType">The Card Type (VS, AX, MC, NS, or DB) </param>
        /// <param name="CardEntryMode">The card Entry Mode</param>
        /// <param name="CardPresent">Card Present Flag</param>
        /// <param name="ExpirationDate">Card Expiration Date</param>
        /// <param name="Clerk">Clerk number</param>
        /// <param name="Date">Transaction Date</param>
        /// <param name="Time">Transaction Time</param>
        /// <param name="SaleFlag">Sale vs Refund Indicator</param>
        /// <param name="PrimaryAmount">The total tranasction amount</param>
        /// <param name="SecondaryAmount">The tip amount</param>
        /// <param name="Response">The response from UTG</param>
        /// <param name="Authorization">The Authorization Code</param>
        /// <param name="AVSResult">The result of AVS checking</param>
        /// <param name="AVSStreetVerified">Was the street verified for AVS</param>
        /// <param name="AVSZipVerified">Was the zip verfied for AVS</param>
        /// <param name="ValidAVS">Are the AVS values valid</param>
        /// <param name="TrackInformation">Card Track Information</param>
        /// <returns>String representation of the standard transaction data block</returns>
        private string buildStandardTransactionDataBlock(string CardType, string CardEntryMode, string CardPresent, string ExpirationDate, string Clerk,
                                                            string Date, string Time, string SaleFlag, string PrimaryAmount, string SecondaryAmount, string Response,
                                                            string Authorization, string AVSResult, string AVSStreetVerified, string AVSZipVerified, string ValidAVS,
                                                            string TrackInformation)
        {
            string ret = "";
            ret += (char)028;                                           // Field Seperator
            ret += "001";                                               // Data Block Indicator
            ret += normalize(CardType, 2);
            ret += normalize(CardEntryMode, 1);
            ret += normalize(CardPresent, 1);
            ret += normalize(ExpirationDate, 4);
            ret += leftPad(Clerk, 5);
            ret += truncate(helper.getDate(), 6);
            ret += leftPad(helper.getTime(), 6);
            ret += truncate(SaleFlag, 1);
            ret += rightPad(PrimaryAmount, 14);
            ret += rightPad(SecondaryAmount, 14);
            ret += normalize(Response, 1);
            ret += normalize(Authorization, 6);
            ret += normalize(AVSResult, 1);
            ret += normalize(AVSStreetVerified, 1);
            ret += normalize(AVSZipVerified, 1);
            ret += normalize(ValidAVS, 1);
            ret += normalize(TrackInformation, 128);
            log.Write("\n\nbuildStandardTransactionDataBlock\n" + ret);
            return ret;
        }

        /// <summary>
        /// - 002 AVS -
        /// Builds the Address Verification Services data block that are used to verify customer address info against
        /// what is on file with the card brand to prevent fraud.
        /// </summary>
        /// <param name="CustomerName">The customer name</param>
        /// <param name="StreetAddress">The customer street address</param>
        /// <param name="ZipCode">The billing zip code</param>
        /// <returns>String representation of the AVS data block</returns>
        private string buildAVS(string CustomerName, string StreetAddress, string ZipCode)
        {
            string ret = "";
            ret += (char)028;
            ret += "002";
            ret += normalize(CustomerName, 35);
            ret += normalize(StreetAddress, 30);
            ret += normalize(ZipCode, 9);
            return ret;
        }

        /// <summary>
        /// - 003 Hotel Check Out -
        /// Builds the hotel checkout data block.This block is used when a customer is checking out of a hotel
        /// reservation, and allows for things that are hotel specific like minibar charges and incidentals.
        /// </summary>
        /// <param name="PrimaryChargeType">Guest transaction type at a hotel</param>
        /// <param name="SpecialCode">The special code indicating what the charge is for</param>
        /// <param name="HotelAdditionalCharges">Additional charges from hotel</param>
        /// <param name="ArrivalDate">The guest arrival date</param>
        /// <param name="DepartureDate">The guest departure date</param>
        /// <returns>String representation of the hotel checkout block</returns>
        private string buildHotelCheckOut(string PrimaryChargeType, string SpecialCode, string HotelAdditionalCharges, string ArrivalDate,
                                           string DepartureDate)
        {
            string ret = "";
            ret += (char)028;
            ret += "003";
            ret += normalize(PrimaryChargeType, 1);
            ret += normalize(SpecialCode, 1);
            ret += normalize(HotelAdditionalCharges, 6);
            ret += normalize(ArrivalDate, 6);
            ret += normalize(DepartureDate, 6);
            return ret;
        }

        /// <summary>
        /// - 004 Auto Return -
        /// Builds the auto return data block.This block is used for when a customer returns an auto rental back
        /// to the rental location or company. It allows for auto rental specific charges and related information
        /// such as where the vehicle is retuned to, late adjustments, and where the rental was issued from.
        /// </summary>
        /// <param name="RentalAgreement">The rental agreement indicator</param>
        /// <param name="DriverName">The drivers name</param>
        /// <param name="LateAdjustment">Any late adjustments</param>
        /// <param name="RentalCity">The city where the rental was rented from</param>
        /// <param name="RentalState">The state where the rental was rented from</param>
        /// <param name="RentalZipCode">The zipcode of the rental facility</param>
        /// <param name="RentalDate">The date of the rental</param>
        /// <param name="RentalTime">The time of day the rental was picked up</param>
        /// <param name="ReturnCity">The city the rental will be returned to</param>
        /// <param name="ReturnState">The state where the rental will be returned to</param>
        /// <param name="ReturnZipCode">The zip code where the rental will be returned to</param>
        /// <param name="ReturnDate">The date the rental is returned</param>
        /// <param name="ReturnTime">The time the rental is returned</param>
        /// <param name="AutoAdditionalCharges">Any additional charges for the rental</param>
        /// <param name="NoShowIndicator">Did the customer show up to pick up or return the rental</param>
        /// <param name="ReturnCountryCode">The country code where the rental will be returned</param>
        /// <returns></returns>
        private string buildAutoReturn(string RentalAgreement, string DriverName, string LateAdjustment, string RentalCity, string RentalState,
                                       string RentalZipCode, string RentalDate, string RentalTime, string ReturnCity, string ReturnState,
                                       string ReturnZipCode, string ReturnDate, string ReturnTime, string AutoAdditionalCharges,
                                       string NoShowIndicator, string ReturnCountryCode)
        {
            string ret = "";
            ret += (char)028;
            ret += "004";
            ret += normalize(RentalAgreement, 9);
            ret += normalize(DriverName, 35);
            ret += normalize(LateAdjustment, 14);
            ret += normalize(RentalCity, 18);
            ret += normalize(RentalState, 2);
            ret += normalize(RentalZipCode, 9);
            ret += normalize(RentalDate, 6);
            ret += normalize(RentalTime, 6);
            ret += normalize(ReturnCity, 18);
            ret += normalize(ReturnState, 2);
            ret += normalize(ReturnZipCode, 9);
            ret += normalize(ReturnDate, 6);
            ret += normalize(ReturnTime, 6);
            ret += normalize(AutoAdditionalCharges, 6);
            ret += normalize(NoShowIndicator, 1);
            ret += normalize(ReturnCountryCode, 3);
            return ret;
        }

        /// <summary>
        /// - 005 Debit PIN pad -
        /// </summary>
        /// <param name="PinPadType">The Pin Pad Type</param>
        /// <param name="PinPadBlockFormat">The Pin Pad's block format</param>
        /// <param name="PinPadKey">The Pin Pad key</param>
        /// <param name="PinBlock">The Pin Pad block with the actual PIN</param>
        /// <returns>String representation of the Debit PIN block</returns>
        private string buildDebitPINPad(string PinPadType, string PinPadBlockFormat, string PinPadKey, string PinBlock)
        {
            string ret = "";
            ret += (char)028;
            ret += "005";
            ret += normalize(PinPadType, 2);
            ret += normalize(PinPadBlockFormat, 2);
            for (int i = PinPadKey.Length; i < 20; i++)
            {
                ret += "F";
            }
            ret += truncate(PinPadKey, 20);
            ret += normalize(PinBlock, 16);
            return ret;
        }

        /// <summary>
        /// - 006 Signature Capture -
        /// </summary>
        /// <param name="SignatureDeviceType">Device type of the device capturing the signature</param>
        /// <param name="SignatureBlockNumber">The signature block number</param>
        /// <param name="SignatureTotalBlocks">Total number of signature blocks</param>
        /// <param name="SignatureBlock">The block with the signature data</param>
        /// <returns>String representation of the signature capture block</returns>
        private string buildSignatureCapture(string SignatureDeviceType, string SignatureBlockNumber, string SignatureTotalBlocks, string SignatureBlock)
        {
            string ret = "";
            ret += (char)028;
            ret += "006";
            ret += normalize(SignatureDeviceType, 2);
            ret += normalize(SignatureBlockNumber, 1);
            ret += normalize(SignatureTotalBlocks, 1);
            ret += truncate(SignatureBlock, 16384);
            return ret;
        }

        /// <summary>
        /// - 008 Level 2 Card Data -
        /// </summary>
        /// <param name="CustomerReference">The Customer reference</param>
        /// <param name="TaxIndicator">Indicates if tax was charged as a part of this transaction</param>
        /// <param name="TaxAmount">The amount of tax charged</param>
        /// <param name="DestinationZipCode">The zip code of where the product will be delivered to or zip code of where services are rendered</param>
        /// <returns>String representation of the Level 2 Card Data</returns>
        private string buildLevel2CardBlock(string CustomerReference, string TaxIndicator, string TaxAmount, string DestinationZipCode)
        {
            string ret = "";
            ret += (char)028;                                           // Field Seperator
            ret += "008";                                               // Data Block Indicator
            ret += normalize(CustomerReference, 25);
            ret += normalize(TaxIndicator, 1);
            ret += leftPad(TaxAmount, 14);
            ret += truncate(DestinationZipCode, 9);
            char[] c = ret.ToCharArray();
            return ret;
        }

        /// <summary>
        /// - 009 Purchasing Card -
        /// Used for sending product descriptions to the processor for better discount and interchange rates
        /// </summary>
        /// <param name="ProductDescriptor1">1st Product Descriptor</param>
        /// <param name="ProductDescriptor2">2nd Product Descriptor</param>
        /// <param name="ProductDescriptor3">3rd Product Descriptor</param>
        /// <param name="ProductDescriptor4">4th Product Descriptor</param>
        /// <returns>String representation of the all the product descriptors</returns>
        private string buildPurchasingCardBlock(string ProductDescriptor1, string ProductDescriptor2, string ProductDescriptor3, string ProductDescriptor4)
        {
            string ret = "";
            ret += (char)028;
            ret += "009";
            ret += truncate(ProductDescriptor1, 40);
            ret += (char)094;
            ret += truncate(ProductDescriptor2, 40);
            ret += (char)094;
            ret += truncate(ProductDescriptor3, 40);
            ret += (char)094;
            ret += truncate(ProductDescriptor4, 40);
            return ret;
        }

        /// <summary>
        /// - 010 Valid Check ID Types -
        /// </summary>
        /// <param name="ValidIDTypes">The valid ID Types</param>
        /// <returns>String representation of Valid ID Types</returns>
        private string buildValidCheckIDTypes(string ValidIDTypes)
        {
            string ret = "";
            ret += (char)028;
            ret += "010";
            ret += truncate(ValidIDTypes, 100);
            return ret;
        }

        /// <summary>
        /// - 011 Check Approval -
        /// </summary>
        /// <param name="IDTypeCode">Customer ID Type</param>
        /// <param name="IDNumber">Customer ID number</param>
        /// <param name="Birthdate">Customer birthday</param>
        /// <param name="CheckAmount">Total amount of check</param>
        /// <param name="HostResponse">Response from proc</param>
        /// <param name="ManualCheckNumber">Check number</param>
        /// <param name="RawMagneticData">Raw magnetic reader data</param>
        /// <param name="CheckType">Check type</param>
        /// <param name="ReaderIndicator">Use of a check reader indicator</param>
        /// <param name="TransitRoutingNumber">Customer Routing number</param>
        /// <param name="CheckingAccountNumber">Customer Checking Account Number</param>
        /// <returns>String representation of check approval block</returns>
        private string buildCheckApproval(string IDTypeCode, string IDNumber, string Birthdate, string CheckAmount, string HostResponse, string ManualCheckNumber,
                                           string RawMagneticData, string CheckType, string ReaderIndicator, string TransitRoutingNumber, string CheckingAccountNumber)
        {
            string ret = "";
            ret += (char)028;
            ret += "011";
            ret += normalize(IDTypeCode, 2);
            ret += normalize(IDNumber, 24);
            ret += normalize(Birthdate, 6);
            ret += normalize(CheckAmount, 14);
            ret += normalize(HostResponse, 24);
            ret += normalize(ManualCheckNumber, 10);
            ret += normalize(RawMagneticData, 80);
            ret += normalize(CheckType, 1);
            ret += normalize(ReaderIndicator, 1);
            ret += normalize(TransitRoutingNumber, 10);
            ret += normalize(CheckingAccountNumber, 24);
            return ret;
        }

        /// <summary>
        /// - 012 DBA -
        /// </summary>
        /// <param name="DBA">Doing Business As (Business Name)</param>
        /// <param name="DBAAddressLine1">Business Address Line 1</param>
        /// <param name="DBAAddressLine2">Business Address Line 2</param>
        /// <param name="DBACity">Business City</param>
        /// <param name="DBAState">Business State</param>
        /// <param name="DBAZipCode">Business Zip Code</param>
        /// <param name="MerchantType">Merchant Type (MCC)</param>
        /// <param name="CardAbbreviations">Accepted Card Type abbreviaions</param>
        /// <param name="SerialNumber">Shift4 Serial Number</param>
        /// <param name="Revision">Update revision</param>
        /// <param name="DBAPhone">Business Phone Number</param>
        /// <param name="BusinessDayEndingTime">End of business day time</param>
        /// <returns>String representation of DBA block</returns>
        private string buildDBA(string DBA, string DBAAddressLine1, string DBAAddressLine2, string DBACity, string DBAState, string DBAZipCode,
                               string MerchantType, string CardAbbreviations, string SerialNumber, string Revision, string DBAPhone, string BusinessDayEndingTime)
        {
            string ret = "";
            ret += (char)028;
            ret += "012";
            ret += normalize(DBA, 22);
            ret += normalize(DBAAddressLine1, 38);
            ret += normalize(DBAAddressLine2, 38);
            ret += normalize(DBACity, 13);
            ret += normalize(DBAState, 3);
            ret += normalize(DBAZipCode, 9);
            ret += normalize(MerchantType, 1);
            ret += normalize(CardAbbreviations, 20);
            ret += normalize(SerialNumber, 10);
            ret += normalize(Revision, 8);
            ret += normalize(DBAPhone, 15);
            ret += normalize(BusinessDayEndingTime, 6);
            return ret;
        }

        /// <summary>
        /// - 013 Hotel Check In -
        /// Builds the Hotel Check-in Data Block.Used when a customer is checking into a hotel.
        /// </summary>
        /// <param name="HotelEstimatedDays">Number of days the customer plans to stay at the hotel.</param>
        /// <returns>String representation of the Hotel Check In block</returns>
        private string buildHotelCheckIn(string HotelEstimatedDays)
        {
            string ret = "";
            ret += (char)028;
            ret += "013";
            ret += normalize(HotelEstimatedDays, 2);
            return ret;
        }

        /// <summary>
        /// - 014 Auto Rental -
        /// Builds the Auto Rental Data Block.Used when a customer is starting their auto rental.
        /// </summary>
        /// <param name="AutoEstimatedDays">Number of days that the customer plans to have the car rented.</param>
        /// <returns>String representation of the Auto Rental block</returns>
        private string buildAutoRental(string AutoEstimatedDays)
        {
            string ret = "";
            ret += (char)028;
            ret += "014";
            ret += normalize(AutoEstimatedDays, 2);
            return ret;
        }

        /// <summary>
        /// - 015 Voice Authorization Center -
        /// Builds the voice authorization data block.Used in FRC 22 requests to get the voice number
        /// to call when the response code to an authorization/sale returns a referral.
        /// </summary>
        /// <param name="VoicePhoneNumber">Voice Referal phone number</param>
        /// <param name="VoiceMerchantAccount">Merchant Voice Account</param>
        /// <returns>String representation of the voice authorization block</returns>
        private string buildVoiceAuthorizationCenter(string VoicePhoneNumber, string VoiceMerchantAccount)
        {
            string ret = "";
            ret += (char)028;
            ret += "015";
            ret += normalize(VoicePhoneNumber, 20);
            ret += normalize(VoiceMerchantAccount, 20);
            return ret;
        }

        /// <summary>
        /// - 016 Extended Authorization -
        /// </summary>
        /// <param name="PreauthorizedAmount">Previously authorized amount</param>
        /// <param name="PreauthorizedTolerance">Tolerance on settle of the preauthorized amount</param>
        /// <param name="RetrievalReference">Reference to recall preauthorized transaction</param>
        /// <param name="AuthSource">Source of authorization</param>
        /// <returns>String representation of the Extended Authorization block</returns>
        private string buildExtendedAuthorization(string PreauthorizedAmount, string PreauthorizedTolerance, string RetrievalReference, string AuthSource)
        {
            string ret = "";
            ret += (char)028;
            ret += "016";
            ret += normalize(PreauthorizedAmount, 14);
            ret += normalize(PreauthorizedTolerance, 14);
            ret += normalize(RetrievalReference, 12);
            ret += normalize(AuthSource, 1);
            return ret;
        }

        /// <summary>
        /// - 017 Notes -
        /// </summary>
        /// <param name="Notes">The notes to be appended to the transaction in LTM</param>
        /// <returns>String representation of the notes block</returns>
        private string buildNotes(string Notes)
        {
            string ret = "";
            ret += (char)028;
            ret += "017";
            ret += truncate(Notes, 4096);
            return ret;
        }

        /// <summary>
        /// - 018 Terminal ID -
        /// Builds the terminal ID block.Used to send the TerminalID to UTG, so that a UTG Controlled PIN Pad
        /// can be used in the transaction.
        /// </summary>
        /// <param name="TerminalID">The Terminal ID</param>
        /// <returns>String representation of the Terminal ID block</returns>
        private string buildTerminalID(string TerminalID)
        {
            string ret = "";
            ret += (char)028;
            ret += "018";
            ret += truncate(TerminalID, 32);
            return ret;
        }

        /// <summary>
        /// ** DEPRECATED **
        /// - 019 API Serial Number and Password
        /// Builds the API Serial and Password Block. 
        /// </summary>
        /// <param name="pass">The Password</param>
        /// <param name="serial">The Shift4 Serial Number</param>
        /// <returns>String representation of the API Serial and Password block</returns>
        private string buildAPISerialAndPass(string pass, string serial)
        {
            string ret = "";
            ret += (char)028;
            ret += "019";
            ret += normalize(pass, 32);
            ret += normalize(serial, 10);
            return ret;
        }

        /// <summary>
        /// - 022 Card Security Code CVV2 -
        /// Builds the card security code block.Used to send CVV2 values to Shift4 for CVV2 verificaiton.
        /// </summary>
        /// <param name="CVV2Indicator">Indicator for CVV present in request</param>
        /// <param name="CVV2Code">The CVV value</param>
        /// <param name="CVV2Result">Result of CVV verification</param>
        /// <param name="CVV2Valid">Is CVV valid</param>
        /// <returns>String represntation of the Card Security Code block</returns>
        private string buildCardSecurityCode(string CVV2Indicator, string CVV2Code, string CVV2Result, string CVV2Valid)
        {
            string ret = "";
            ret += (char)028;
            ret += "022";
            ret += normalize(CVV2Indicator, 1);
            ret += normalize(CVV2Code, 4);
            ret += normalize(CVV2Result, 1);
            ret += normalize(CVV2Valid, 1);
            return ret;
        }

        /// <summary>
        /// - 023 API Options -
        /// Builds the APIOptions data block.Used to pass APIOptions to Shift4 for a given request.
        /// </summary>
        /// <param name="APIOptions">Comma seperated list of API Options</param>
        /// <returns>String representation of the API Options block</returns>
        private string buildAPIOptionsDataBlock(string APIOptions)
        {
            string ret = "";
            ret += (char)028;                                           // Field Seperator
            ret += "023";                                               // Data Block Indicator
            ret += APIOptions;                                          // Up to 255 Characters
            return ret;
        }

        /// <summary>
        /// - 025 IT'S YOUR CARD -
        /// </summary>
        /// <param name="IYCCardType">IYC Card Type</param>
        /// <param name="IYCDiscount">IYC Discount</param>
        /// <param name="IYCBalance">IYC Balance</param>
        /// <param name="IYCAvailableBalance">IYC available balance</param>
        /// <param name="IYCExpiration">IYC Expiration date</param>
        /// <param name="IYCCardFormatted">IYC Card format type</param>
        /// <returns>String representation of IYC block</returns>
        private string buildItsYourCard(string IYCCardType, string IYCDiscount, string IYCBalance, string IYCAvailableBalance, string IYCExpiration,
                                       string IYCCardFormatted)
        {
            string ret = "";
            ret += (char)028;
            ret += "025";
            ret += normalize(IYCCardType, 1);
            ret += normalize(IYCDiscount, 5);
            ret += normalize(IYCBalance, 14);
            ret += normalize(IYCAvailableBalance, 14);
            ret += normalize(IYCExpiration, 6);
            ret += truncate(IYCCardFormatted, 24);
            return ret;
        }

        /// <summary>
        /// - 026 IT'S YOUR CARD Reason Text -
        /// </summary>
        /// <param name="IYCReasonText">Reason for IYC transaction</param>
        /// <returns>String representation of IYC reason text block</returns>
        private string buildItsYourCardReasonText(string IYCReasonText)
        {
            string ret = "";
            ret += (char)028;
            ret += "026";
            ret += truncate(IYCReasonText, 32);
            return ret;
        }

        /// <summary>
        /// - 031 Override Business Date -
        /// </summary>
        /// <param name="OverrideBusinessDate">The business date to use on this transaction instead of current</param>
        /// <returns>String representation of Override Business date block</returns>
        private string buildOverrideBusinessDate(string OverrideBusinessDate)
        {
            string ret = "";
            ret += (char)028;
            ret += "031";
            ret += truncate(OverrideBusinessDate, 6);
            return ret;
        }

        /// <summary>
        /// - 032 Totals Report -
        /// </summary>
        /// <param name="ReportStartDate">Batch start date</param>
        /// <param name="ReportStartTime">Batch start time</param>
        /// <param name="ReportEndDate">Batch end date</param>
        /// <param name="ReportEndTime">Batch end time</param>
        /// <param name="ReportClerk">Batch clerk number</param>
        /// <param name="ReportCardType">Batch card type</param>
        /// <param name="ReportTerminalID">Batch Terminal ID</param>
        /// <returns>String representation of the Totals Report block</returns>
        private string buildTotalsReport(string ReportStartDate, string ReportStartTime, string ReportEndDate, string ReportEndTime, string ReportClerk,
                                           string ReportCardType, string ReportTerminalID)
        {
            string ret = "";
            ret += (char)028;
            ret += "032";
            ret += normalize(ReportStartDate, 6);
            ret += normalize(ReportStartTime, 6);
            ret += normalize(ReportEndDate, 6);
            ret += normalize(ReportEndTime, 6);
            ret += leftPad(ReportClerk, 5);
            ret += normalize(ReportCardType, 2);
            ret += truncate(ReportTerminalID, 32);
            return ret;
        }

        /// <summary>
        /// - 033 Receipt Text -
        /// </summary>
        /// <param name="ReceiptTextColumns">Number of characters wide a receipt will be printed to</param>
        /// <param name="ReceiptText">Text of the receipt</param>
        /// <returns>String representation of the receipt text block</returns>
        private string buildReceiptText(string ReceiptTextColumns, string ReceiptText)
        {
            string ret = "";
            ret += (char)028;
            ret += "033";
            ret += normalize(ReceiptTextColumns, 3);
            ret += truncate(ReceiptText, 4000);
            return ret;
        }

        /// <summary>
        /// - 039 UniqueID -
        /// </summary>
        /// <param name="UniqueID">The TrueToken representing card holder data</param>
        /// <param name="TokenSerialNumber">Serial number of the TokenStore of this TrueToken</param>
        /// <returns>String representation of the UniqueID block</returns>
        private string buildUniqueID(string UniqueID, string TokenSerialNumber)
        {
            string ret = "";
            ret += (char)028;
            ret += "039";
            ret += normalize(UniqueID, 16);
            ret += truncate(TokenSerialNumber, 10);
            return ret;
        }

        /// <summary>
        /// - 054 Line Items -
        /// </summary>
        /// <param name="LineItemCount">Number of line items</param>
        /// <param name="LineItem1">1st Line Item</param>
        /// <param name="LineItem2">2nd Line Item</param>
        /// <param name="LineItem3">3rd Line Item</param>
        /// <param name="LineItem4">4th Line Item</param>
        /// <param name="LineItem5">5th Line Item</param>
        /// <param name="LineItem6">6th Line Item</param>
        /// <param name="LineItem7">7th Line Item</param>
        /// <param name="LineItem8">8th Line Item</param>
        /// <param name="LineItem9">9th Line Item</param>
        /// <param name="LineItem10">10th Line Item</param>
        /// <returns>String representation of the Line Items block</returns>
        private string buildLineItems(string LineItemCount, string LineItem1, string LineItem2, string LineItem3, string LineItem4,
                                   string LineItem5, string LineItem6, string LineItem7, string LineItem8, string LineItem9, string LineItem10)
        {
            string ret = "";
            ret += (char)028;
            ret += "054";
            ret += normalize(LineItem1, 30);
            ret += normalize(LineItem2, 30);
            ret += normalize(LineItem3, 30);
            ret += normalize(LineItem4, 30);
            ret += normalize(LineItem5, 30);
            ret += normalize(LineItem6, 30);
            ret += normalize(LineItem7, 30);
            ret += normalize(LineItem8, 30);
            ret += normalize(LineItem9, 30);
            ret += normalize(LineItem10, 30);
            return ret;
        }

        /// <summary>
        /// - 055 Line Item - 
        /// </summary>
        /// <param name="LineItem">Value of the line item</param>
        /// <returns>String representatin of the line item block</returns>
        private string buildLineItem(string LineItem)
        {
            string ret = "";
            ret += (char)028;
            ret += "055";
            ret += normalize(LineItem, 30);
            return ret;
        }

        /// <summary>
        /// - 056 Cashback and Surcharge Amounts -
        /// </summary>
        /// <param name="Surcharge">The surcharge charged on this transaction</param>
        /// <param name="CashBack">Amount of customer requested cash back</param>
        /// <returns>String representation of the Cashback and Surcharge amounts block</returns>
        private string buildCashbackAndSurchargeAmounts(string Surcharge, string CashBack)
        {
            string ret = "";
            ret += (char)028;
            ret += "056";
            ret += normalize(Surcharge, 14);
            ret += normalize(CashBack, 14);
            return ret;
        }

        /*
         * - 064 BIN Management -
         */
        private string buildBINManagement(string SpinPrefix, string SpinAbb, string SpinIsDebit, string SpinIsDCC, string SpinResult)
        {
            string ret = "";
            ret += (char)028;
            ret += "064";
            ret += normalize(SpinPrefix, 10);
            ret += normalize(SpinAbb, 2);
            ret += normalize(SpinIsDebit, 1);
            ret += normalize(SpinIsDCC, 1);
            ret += normalize(SpinResult, 2);
            return ret;
        }

        /*
         * - 068 Enhanced Authorization - 
         */
        private string buildEnhancedAuthorization(string EnhancedDataID, string EnhancedDataValues)
        {
            string ret = "";
            ret += (char)028;
            ret += "068";
            ret += normalize(EnhancedDataID, 2);
            ret += truncate(EnhancedDataValues, 1024);
            return ret;
        }

        /*
         * - 070 Inventory Information Approval System (IIAS) - 
         */
        private string buildInventoryInformationApprovalSystem(string IIASType1, string IIASAmount1, string IIASType2, string IIASAmount2,
                                                                 string IIASType3, string IIASAmount3, string IIASType4, string IIASAmount4)
        {
            string ret = "";
            ret += (char)028;
            ret += "070";
            ret += normalize(IIASType1, 2);
            ret += normalize(IIASAmount1, 14);
            ret += normalize(IIASType2, 2);
            ret += normalize(IIASAmount2, 14);
            ret += normalize(IIASType3, 2);
            ret += normalize(IIASAmount3, 14);
            ret += normalize(IIASType4, 2);
            ret += normalize(IIASAmount4, 14);
            return ret;
        }

        /*
         * - 073 Four Words -
         */
        private string buildFourWords(string FourWords)
        {
            string ret = "";
            ret += (char)028;
            ret += "073";
            ret += truncate(FourWords, 28);
            return ret;
        }

        /*
         * - 074 Card Level Results -
         */
        private string buildCardLevelResults(string CardLevelResults)
        {
            string ret = "";
            ret += (char)028;
            ret += "074";
            ret += normalize(CardLevelResults, 2);
            return ret;
        }

        /*
         * - 075 Balance Return -
         */
        private string buildBalanceReturn(string BalanceReturnIndicator, string Balance)
        {
            string ret = "";
            ret += (char)028;
            ret += "075";
            ret += normalize(BalanceReturnIndicator, 1);
            ret += normalize(Balance, 14);
            return ret;
        }

        /*
         * - 076 P2PE -
         */
        private string buildP2PE(string P2PEBlock)
        {
            string P2PEDeviceType;
            string P2PEBlockLength;
            if (P2PEBlock.Contains("*"))
            {
                P2PEDeviceType = "01";
            }
            else
            {
                P2PEDeviceType = "02";
            }
            P2PEBlockLength = P2PEBlock.Length.ToString();
            string ret = "";
            ret += (char)028;
            ret += "076";
            ret += normalize(P2PEDeviceType, 2);
            ret += normalize(P2PEBlockLength, 4);
            ret += normalize(P2PEBlock, 2048);
            return ret;
        }

        /*
         * - 086 MetaToken -
         */
        private string buildMetaToken(string MetaTokenType, string MetaTokenData)
        {
            string ret = "";
            ret += (char)028;
            ret += "086";
            ret += normalize(MetaTokenType, 2);
            ret += normalize(MetaTokenData, 16);
            return ret;
        }

        /*
         * - 087 Prompt Confirmation -
         */
        private string buildPromptConfirmation(string PromptConfirmResult, string PromptConfirmQuestion, string PromptConfirmValue)
        {
            string ret = "";
            ret += (char)028;
            ret += "087";
            ret += normalize(PromptConfirmResult, 1);
            ret += normalize(PromptConfirmQuestion, 64);
            ret += truncate(PromptConfirmValue, 4096);
            return ret;
        }

        /*
         * - 088 Photo - 
         */
        private string buildPhoto(string PhotoType, string PhotoData)
        {
            string ret = "";
            ret += (char)028;
            ret += "088";
            ret += normalize(PhotoType, 1);
            ret += truncate(PhotoData, 4194304);
            return ret;
        }

        /*
         * - 089 Gift Card Extended data - 
         */
        private string buildGiftCardExtendedData(string GiftCardExtendedDataValues)
        {
            string ret = "";
            ret += (char)028;
            ret += "089";
            ret += truncate(GiftCardExtendedDataValues, 1024);
            return ret;
        }

        /*
         * - 090 Merchant Receipt Text -
         */
        private string buildMerchantReceiptText(string MerchantReceiptText)
        {
            string ret = "";
            ret += (char)028;
            ret += "090";
            ret += truncate(MerchantReceiptText, 4096);
            return ret;
        }

        /*
         * - 091 Customer Receipt Text - 
         */
        private string buildCustomerReceiptText(string CustomerReceiptText)
        {
            string ret = "";
            ret += (char)028;
            ret += "091";
            ret += truncate(CustomerReceiptText, 4096);
            return ret;
        }

        /*
         * - 094 AccessToken Data Block -
         */
        private string buildAccessTokenDataBlock(string AccessToken)
        {
            string ret = "";
            ret += (char)028;
            ret += "094";
            ret += AccessToken;
            return ret;
        }

        /*
         * - 095 Token Exchange Data Block -
         */
        private string buildTokenExchangeDataBlock()
        {
            string ret = "";
            ret += (char)028;                                           // Field Seperator
            ret += "095";                                               // Data Block Indicator
            while (AuthToken.Length < 51)                               // Supplied Auth Token has to be 51 Bytes
            {
                AuthToken = AuthToken + " ";
            }
            ret += AuthToken;
            ret += ClientGUID;                                          // Supplied Client GUID
            return ret;
        }

        /*
         * - 097 Cloud Parameters -
         */
        private string buildCloudParameters(string DeviceService, string DeviceGuid)
        {
            string ret = "";
            ret += (char)028;
            ret += "097";
            ret += normalize(DeviceService, 10);
            ret += truncate(DeviceGuid, 200);
            return ret;
        }

        /*
         * - 098 Cloud Extended Parameters - 
         */
        private string buildCloudExtendedParameters(string DeviceExtensions)
        {
            string ret = "";
            ret += (char)028;
            ret += "098";
            ret += truncate(DeviceExtensions, 1024);
            return ret;
        }

        /*
         * - 100 Process Forms -
         */
        private string buildProcessForms(string FormName, string FormResponse, string KeyValue1, string KeyValue2, string KeyValue3, 
                                            string KeyValue4, string KeyValue5, string KeyValue6, string KeyValue7, string KeyValue8, 
                                            string KeyValue9, string KeyValue10)
        {
            string ret = "";
            ret += (char)028;
            ret += "100";
            ret += normalize(FormName, 12);
            ret += normalize(FormResponse, 5);
            ret += truncate(KeyValue1, 200) + (char)094;
            ret += truncate(KeyValue2, 200) + (char)094;
            ret += truncate(KeyValue3, 200) + (char)094;
            ret += truncate(KeyValue4, 200) + (char)094;
            ret += truncate(KeyValue5, 200) + (char)094;
            ret += truncate(KeyValue6, 200) + (char)094;
            ret += truncate(KeyValue7, 200) + (char)094;
            ret += truncate(KeyValue8, 200) + (char)094;
            ret += truncate(KeyValue9, 200) + (char)094;
            ret += truncate(KeyValue10, 200) + (char)094;
            return ret;
        }

        /*
         * - 108 Terms and Conditions -
         */
        private string buildTermsAndConditions(string TermsAndConditionsResult, string TermsAndConditions)
        {
            string ret = "";
            ret += (char)028;
            ret += "108";
            ret += normalize(TermsAndConditionsResult, 1);
            ret += truncate(TermsAndConditions, 4096);
            return ret;
        }

        /*
         * - 111 Signature Suppressed -
         */
        private string buildSignatureSuppressed(string SignatureSuppressed)
        {
            string ret = "";
            ret += (char)028;
            ret += "111";
            ret += normalize(SignatureSuppressed, 1);
            return ret;
        }

        /*
         * - 112 AVS/CVV Prompt -
         */
        private string buildAVS_CVVPrompt(string CVV2Prompt, string StreetNumberPrompt, string PostalCodePrompt)
        {
            string ret = "";
            ret += (char)028;
            ret += "112";
            ret += normalize(CVV2Prompt, 1);
            ret += normalize(StreetNumberPrompt, 1);
            ret += normalize(PostalCodePrompt, 1);
            return ret;
        }

        /*
         * - 113 Input Prompt -
         */
        private string buildInputPrompt(string DeviceInputIndex, string DeviceInputResponse)
        {
            string ret = "";
            ret += (char)028;
            ret += "113";
            ret += normalize(DeviceInputIndex, 3);
            ret += truncate(DeviceInputResponse, 4096);
            return ret;
        }

        /*
         * - 116 Device Language -
         */
        private string buildDeviceLanguage(string DeviceLanguage)
        {
            string ret = "";
            ret += (char)028;
            ret += "116";
            ret += normalize(DeviceLanguage, 3);
            return ret;
        }

        /*
         * - 999 Extended Error - 
         */
        private string buildExtendedError(string ShortError, string LongError)
        {
            string ret = "";
            ret += (char)028;
            ret += "999";
            ret += normalize(ShortError, 16);
            ret += truncate(LongError, 255);
            return ret;
        }


        /******************************************************************************************************
         * Block Parser Methods                                                                               *
         ******************************************************************************************************/

        // Parse out the TCP response into TCP blocks
        private Dictionary<string, string> parseTCPResponse(byte[] response)
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();

            ASCIIEncoding asen = new ASCIIEncoding();
            string rawString = asen.GetString(response);
            string[] blocks = rawString.Split((char)028);
            int last = blocks.Length - 1;
            int end = blocks[last].IndexOf((char)003);
            int size = blocks[last].Length;
            char[] chars2 = blocks[last].ToCharArray();
            System.Threading.Thread.Sleep(1000);
            blocks[last] = blocks[last].Remove(end, blocks[last].Length - end);
            // Process header Block seperately
            log.Write("Processing Header Block");
            ret = processHeaderBlock(blocks[0], ret);

            // Process Remaining Blocks
            int i = 1;
            while (i < blocks.Length)
            {
                char[] chars = blocks[i].ToCharArray();
                if (chars[0] == '0' && chars[1] == '0' && chars[2] == '0')
                {
                    // 000 Vendor Block
                    log.Write("Processing Vendor Block");
                    ret = processVendorBlock(chars, ret);
                }
                else if (chars[0] == '0' && chars[1] == '0' && chars[2] == '1')
                {
                    // 001 standard Transaction
                    log.Write("Processing Standard Transaction Block");
                    ret = processStandardTransactionBlock(chars, ret);
                }
                else if (chars[0] == '0' && chars[1] == '0' && chars[2] == '2')
                {
                    // 002 AVS
                    log.Write("Processing AVS Block");
                    ret = processAVSBlock(chars, ret);
                }
                else if (chars[0] == '0' && chars[1] == '0' && chars[2] == '3')
                {
                    // 003 Hotel Check Out
                    log.Write("Processing Hotel Check Out Block");
                    ret = processHotelCheckOut(chars, ret);
                }
                else if (chars[0] == '0' && chars[1] == '0' && chars[2] == '4')
                {
                    // 004 Auto Return
                    log.Write("Processing Auto Return Block");
                    ret = processAutoReturn(chars, ret);
                }
                else if (chars[0] == '0' && chars[1] == '0' && chars[2] == '5')
                {
                    // 005 Debit PIN Pad
                    log.Write("Processing Debit PIN Pad Block");
                    ret = processDebitPINPad(chars, ret);
                }
                else if (chars[0] == '0' && chars[1] == '0' && chars[2] == '6')
                {
                    // 006 Signature Capture
                    log.Write("Processing Signature Capture Block");
                    ret = processSignatureCapture(chars, ret);
                }
                else if (chars[0] == '0' && chars[1] == '0' && chars[2] == '8')
                {
                    // 008 Level 2 Data
                    log.Write("Processing Level 2 Data Block");
                    ret = processLevel2CardBlock(chars, ret);
                }
                else if (chars[0] == '0' && chars[1] == '0' && chars[2] == '9')
                {
                    // 009 Purchasing Card Data
                    log.Write("Processing Purchase Card Data Block");
                    ret = processPurchasingCardBlock(chars, ret);
                }
                else if (chars[0] == '0' && chars[1] == '1' && chars[2] == '0')
                {
                    // 010 Valid Check ID Types
                    log.Write("Processing Valid Check ID Types Block");
                    ret = processValidCheckIDTypes(chars, ret);
                }
                else if (chars[0] == '0' && chars[1] == '1' && chars[2] == '1')
                {
                    // 011 Check Approval
                    log.Write("Processing Check Approval Block");
                    ret = processCheckApproval(chars, ret);
                }
                else if (chars[0] == '0' && chars[1] == '1' && chars[2] == '2')
                {
                    // 012 DBA
                    log.Write("Processing DBA Block");
                    ret = processDBA(chars, ret);
                }
                else if (chars[0] == '0' && chars[1] == '1' && chars[2] == '3')
                {
                    // 013 Hotel Check In
                    log.Write("Processing Hotel Check In Block");
                    ret = processHotelCheckIn(chars, ret);
                }
                else if (chars[0] == '0' && chars[1] == '1' && chars[2] == '4')
                {
                    // 014 Auto Rental
                    log.Write("Processing Auto Rental Block");
                    ret = processAutoRental(chars, ret);
                }
                else if (chars[0] == '0' && chars[1] == '1' && chars[2] == '5')
                {
                    // 015 Voice Authorization Center
                    log.Write("Processing Voice Authorization Center Block");
                    ret = processVoiceAuthorizationCenter(chars, ret);
                }
                else if (chars[0] == '0' && chars[1] == '1' && chars[2] == '6')
                {
                    // 016 Extended Authorization
                    log.Write("Processing Extended Authorization Block");
                    ret = processExtendedAuthorizationBlock(chars, ret);
                }
                else if (chars[0] == '0' && chars[1] == '1' && chars[2] == '7')
                {
                    // 017 Notes
                    log.Write("Processing Notes Block");
                    ret = processNotes(chars, ret);
                }
                else if (chars[0] == '0' && chars[1] == '1' && chars[2] == '8')
                {
                    // 018 Terminal ID
                    log.Write("Processing Terminal ID Block");
                    ret = processTerminalID(chars, ret);
                }
                else if (chars[0] == '0' && chars[1] == '1' && chars[2] == '9')
                {
                    // 019 API Serial Number and Password    DEPRECATED
                    log.Write("Processing DEPRECATED API Serial Number and Password Block");

                }
                else if (chars[0] == '0' && chars[1] == '2' && chars[2] == '2')
                {
                    // 022 Card Security Code (CVV2)
                    log.Write("Processing Card Security Code (CVV2) Block");
                    ret = processCardSecurityCode(chars, ret);
                }
                else if (chars[0] == '0' && chars[1] == '2' && chars[2] == '3')
                {
                    // 023 API Options
                    log.Write("Processing API Options Block");
                    ret = processAPIOptionsBlock(chars, ret);
                }
                else if (chars[0] == '0' && chars[1] == '2' && chars[2] == '5')
                {
                    // 025 IT'S YOUR CARD
                    log.Write("Processing IT'S YOUR CARD Block");
                    ret = processItsYourCard(chars, ret);
                }
                else if (chars[0] == '0' && chars[1] == '2' && chars[2] == '6')
                {
                    // 026 IT'S YOUR CARD Reason Text
                    log.Write("Processing IT'S YOUR CARD Reason Text Block");
                    ret = processItsYourCardReasonText(chars, ret);
                }
                else if (chars[0] == '0' && chars[1] == '3' && chars[2] == '1')
                {
                    // 031 Override Business Date
                    log.Write("Processing Override Business Date Block");
                    ret = processOverrideBusinessDate(chars, ret);
                }
                else if (chars[0] == '0' && chars[1] == '3' && chars[2] == '2')
                {
                    // 032 Totals Report
                    log.Write("Processing Totals Report Block");
                    ret = processTotalsReport(chars, ret);
                }
                else if (chars[0] == '0' && chars[1] == '3' && chars[2] == '3')
                {
                    // 033 Reciept Text
                    log.Write("Processing Reciept Text Block");
                    ret = processReceiptTextBlock(chars, ret);
                }
                else if (chars[0] == '0' && chars[1] == '3' && chars[2] == '9')
                {
                    // 039 UniqueID
                    log.Write("Processing UniqueID Block");
                    ret = processUniqueIDBlock(chars, ret);
                }
                else if (chars[0] == '0' && chars[1] == '5' && chars[2] == '4')
                {
                    // 054 Line Items
                    log.Write("Processing Line Items Block");
                    ret = processLineItems(chars, ret);
                }
                else if (chars[0] == '0' && chars[1] == '5' && chars[2] == '5')
                {
                    // 055 Line Item (Singular)
                    log.Write("Processing Line Item Block");
                    ret = processLineItem(chars, ret);
                }
                else if (chars[0] == '0' && chars[1] == '5' && chars[2] == '6')
                {
                    // 056 Cashback and Surcharge Amounts
                    log.Write("Processing Cashback and Surcharge Amounts Block");
                    ret = processCashbackAndSurchargeAmounts(chars, ret);
                }
                else if (chars[0] == '0' && chars[1] == '6' && chars[2] == '4')
                {
                    // 064 BIN Management
                    log.Write("Processing BIN Management Block");
                    ret = processBINManagement(chars, ret);
                }
                else if (chars[0] == '0' && chars[1] == '6' && chars[2] == '8')
                {
                    // 068 Enhanced Authorization
                    log.Write("Processing Enhanced Authorization Block");
                    ret = processEnhancedAuthorizationBlock(chars, ret);
                }
                else if (chars[0] == '0' && chars[1] == '7' && chars[2] == '0')
                {
                    // 070 Inventory Information Approval System (IIAS)
                    log.Write("Processing Inventory Information Approval System (IIAS) Block");
                    ret = processInventoryInformationApprovalSystem(chars, ret);
                }
                else if (chars[0] == '0' && chars[1] == '7' && chars[2] == '2')
                {
                    // 072 EMV Receipt Information   DEPRECATED
                    log.Write("Processing DEPRECATED EMV Receipt Information Block");

                }
                else if (chars[0] == '0' && chars[1] == '7' && chars[2] == '3')
                {
                    // 073 Four Words
                    log.Write("Processing Four Words Block");
                    ret = processFourWords(chars, ret);
                }
                else if (chars[0] == '0' && chars[1] == '7' && chars[2] == '4')
                {
                    // 074 Card Level Results
                    log.Write("Processing Card Level Results Block");
                    ret = processCardLevelResults(chars, ret);
                }
                else if (chars[0] == '0' && chars[1] == '7' && chars[2] == '5')
                {
                    // 075 Balance Return
                    log.Write("Processing Balance Return Block");
                    ret = processBalanceReturnBlock(chars, ret);
                }
                else if (chars[0] == '0' && chars[1] == '7' && chars[2] == '6')
                {
                    // 076 P2PE
                    log.Write("Processing P2PE Block");
                    ret = processP2PE(chars, ret);
                }
                else if (chars[0] == '0' && chars[1] == '8' && chars[2] == '5')
                {
                    // 085 Verify ID   DEPRECATED
                    log.Write("Processing DEPRECATED Verify ID Block");

                }
                else if (chars[0] == '0' && chars[1] == '8' && chars[2] == '6')
                {
                    // 086 Meta Token
                    log.Write("Processing Meta Token Block");
                    ret = processMetaToken(chars, ret);
                }
                else if (chars[0] == '0' && chars[1] == '8' && chars[2] == '7')
                {
                    // 087 Prompt Confirmation
                    log.Write("Processing Prompt Confirmation Block");
                    ret = processPromptConfirmation(chars, ret);
                }
                else if (chars[0] == '0' && chars[1] == '8' && chars[2] == '8')
                {
                    // 088 Photo
                    log.Write("Processing Photo Block");
                    ret = processPhoto(chars, ret);
                }
                else if (chars[0] == '0' && chars[1] == '8' && chars[2] == '9')
                {
                    // 089 Gift Card Extended Data
                    log.Write("Processing Gift Card Extended Data Block");
                    ret = processGiftCardExtendedData(chars, ret);
                }
                else if (chars[0] == '0' && chars[1] == '9' && chars[2] == '0')
                {
                    // 090 Merchant Receipt Text
                    log.Write("Processing Merchant Receipt Text Block");
                    ret = processMerchantReceiptText(chars, ret);
                }
                else if (chars[0] == '0' && chars[1] == '9' && chars[2] == '1')
                {
                    // 091 Customer Reciept Text
                    log.Write("Processing Customer Reciept Text Block");
                    ret = processCustomerReceiptText(chars, ret);
                }
                else if (chars[0] == '0' && chars[1] == '9' && chars[2] == '4')
                {
                    // 094 Access Token 
                    log.Write("Processing Access Token Block");
                    ret = processAccessTokenDataBlock(chars, ret);
                }
                else if (chars[0] == '0' && chars[1] == '9' && chars[2] == '5')
                {
                    // 095 Token Exchange
                    log.Write("Processing Balance Return Block");
                    ret = processBalanceReturnBlock(chars, ret);
                }
                else if (chars[0] == '0' && chars[1] == '9' && chars[2] == '7')
                {
                    // 097 Cloud Parameters
                    log.Write("Processing Cloud Parameters Block");
                    ret = processCloudParameters(chars, ret);
                }
                else if (chars[0] == '0' && chars[1] == '9' && chars[2] == '8')
                {
                    // 098 Cloud Extended Parameters
                    log.Write("Processing Cloud Extended Parameters Block");
                    ret = processCloudExtendedParameters(chars, ret);
                }
                else if (chars[0] == '1' && chars[1] == '0' && chars[2] == '0')
                {
                    // 100 Process Forms
                    log.Write("Processing Process Forms Block");
                    ret = processProcessForms(chars, ret);
                }
                else if (chars[0] == '1' && chars[1] == '0' && chars[2] == '8')
                {
                    // 108 Terms and Conditions
                    log.Write("Processing Terms and Conditions Block");
                    ret = processTermsAndConditions(chars, ret);
                }
                else if (chars[0] == '1' && chars[1] == '1' && chars[2] == '1')
                {
                    // 111 Signature Supressed 
                    log.Write("Processing Signature Suppressed Block");
                    ret = processSignatureSuppressed(chars, ret);
                }
                else if (chars[0] == '1' && chars[1] == '1' && chars[2] == '2')
                {
                    // 112 AVS/CVV Prompt
                    log.Write("Processing AVS/CVV Prompt Block");
                    ret = processAVS_CVVPrompt(chars, ret);
                }
                else if (chars[0] == '1' && chars[1] == '1' && chars[2] == '3')
                {
                    // 113 Input Prompt
                    log.Write("Processing Input Prompt Block");
                    ret = processInputPrompt(chars, ret);
                }
                else if (chars[0] == '1' && chars[1] == '1' && chars[2] == '6')
                {
                    // 116 Device Language
                    log.Write("Processing Device Language Block");
                    ret = processDeviceLanguage(chars, ret);
                }
                else if (chars[0] == '9' && chars[1] == '9' && chars[2] == '9')
                {
                    // 999 Extended Error
                    log.Write("Processing Extended Error Block");
                    ret = processExtendedError(chars, ret);
                }
                else
                {
                    // Unknown Block
                    log.Write("Attempting to Process Unkown Block (Problem Indicated!)");
                }
                i++;
            }

            return ret;
        }

        // Parse out the Access Token from the response of the CE request
        private void parseTCPResponseToken(byte[] response)
        {
            ASCIIEncoding asen = new ASCIIEncoding();
            byte[] TransactionHeader = new byte[88];
            byte[] VendorDescription = new byte[68];
            byte[] APIOptions = new byte[259];
            byte[] AccessToken = new byte[55];
            int i = 1;                                      // Start at 1 since first byte is start msg symbol
            while (response[i] != (char)028)
            {
                TransactionHeader[i] = response[i];
                i++;
            }
            log.Write("\nHeader " + asen.GetString(TransactionHeader));

            int j = 1;
            VendorDescription[0] = response[i];
            i++;
            while (response[i] != (char)028)
            {
                VendorDescription[j] = response[i];
                i++; j++;
            }
            log.Write("\nVendorDesc " + asen.GetString(VendorDescription));

            int k = 1;
            APIOptions[0] = response[i];
            i++;
            while (response[i] != (char)028)
            {
                APIOptions[k] = response[i];
                k++; i++;
            }
            log.Write("\nAPI Options " + asen.GetString(APIOptions));

            int m = 1;
            AccessToken[0] = response[i];
            i++;
            while (response[i] != (char)003)
            {
                AccessToken[m] = response[i];
                m++; i++;
            }
            log.Write("\nAccess Token " + asen.GetString(AccessToken));
            byte[] token = new byte[55];
            for (int n = 4; n < AccessToken.Length; n++)
            {
                token[n - 4] = AccessToken[n];
            }
            setAccessToken(asen.GetString(token));
        }

        // - 0 Header block
        private Dictionary<string, string> processHeaderBlock(string block, Dictionary<string, string> dict)
        {
            string ret = "";
            char[] chars = block.ToCharArray();
            dict.Add("FunctionRequestCode", (chars[3].ToString() + chars[4].ToString()));
            
            for (int i = 5; i < 17; i++)
            {
                ret += chars[i];
            }
            dict.Add("RequestorReference", ret);

            ret = "";
            dict.Add("ErrorIndicator", chars[17].ToString());
            
            for (int j = 18; j < 24; j++)
            {
                ret += chars[j];
            }
            dict.Add("PrimaryErrorCode", ret);
            ret = "";

            dict.Add("SecondaryErrorCode", chars[24].ToString() + chars[25].ToString() + chars[26].ToString());
            
            for (int k = 37; k < 47; k++)
            {
                ret += chars[k];
            }
            dict.Add("TranID", ret);
            ret = "";

            for (int l = 47; l < 57; l++)
            {
                ret += chars[l];
            }
            dict.Add("Invoice", ret);
            ret = "";

            for (int m = 57; m < chars.Length; m++)
            {
                ret += chars[m];
            }
            dict.Add("CardNumber", ret);
            
            return dict;
        }

        // - 000 Vendor Description Block
        private Dictionary<string, string> processVendorBlock(char[] chars, Dictionary<string, string> dict)
        {
            string ret = "";
            for (int i = 3; i < chars.Length; i++)
            {
                ret += chars[i];
            }
            dict.Add("Vendor", ret);
            return dict;
        }

        // - 001 Standard Transaction Data Block
        private Dictionary<string, string> processStandardTransactionBlock(char[] chars, Dictionary<string, string> dict)
        {
            string ret = "";
            dict.Add("CardType", (chars[3].ToString() + chars[4].ToString()));
            dict.Add("CardEntryMode", chars[5].ToString());
            dict.Add("CardPresent", chars[6].ToString());
            dict.Add("ExpirationDate", chars[7].ToString() + chars[8].ToString() + chars[9].ToString() + chars[10].ToString());
            
            for (int i = 11; i < 16; i++)
            {
                ret += chars[i];
            }
            dict.Add("Clerk", ret);
            ret = "";
            
            for (int j = 16; j < 22; j++)
            {
                ret += chars[j];
            }
            dict.Add("Date", ret);
            ret = "";
            
            for (int k = 22; k < 28; k++)
            {
                ret += chars[k];
            }
            dict.Add("Time", ret);
            ret = "";
            dict.Add("SaleFlag", chars[28].ToString());
            
            for (int l = 29; l < 41; l++)
            {
                ret += chars[l];
            }
            dict.Add("PrimaryAmount", ret + "." + chars[41].ToString() + chars[42].ToString());
            ret = "";

            
            for (int m = 43; m < 55; m++)
            {
                ret += chars[m];
            }
            dict.Add("SecondaryAmount", ret + "." + chars[55].ToString() + chars[56].ToString());
            ret = "";

            dict.Add("Response", chars[57].ToString());
            
            for (int n = 58; n < 64; n++)
            {
                ret += chars[n];
            }
            dict.Add("Authorization", ret);
            ret = "";
            dict.Add("AVSResult", chars[64].ToString());
            dict.Add("AVSStreetVerified", chars[65].ToString());
            dict.Add("AVSZipVerified", chars[66].ToString());
            dict.Add("ValidAVS", chars[67].ToString());
            
            for (int o = 68; o < chars.Length; o++)
            {
                ret += chars[o];
            }
            dict.Add("TrackInformation", ret);
            return dict;
        }

        // - 002 AVS
        private Dictionary<string, string> processAVSBlock(char[] chars, Dictionary<string, string> dict)
        {
            string ret = "";
            
            for (int i = 3; i < 38; i++)
            {
                ret += chars[i];
            }
            dict.Add("CustomerName", ret);
            ret = "";
            
            for (int j = 38; j < 68; j++)
            {
                ret += chars[j];
            }
            dict.Add("StreetAddress", ret);
            ret = "";
            
            for (int k = 68; k < chars.Length; k++)
            {
                ret += chars[k];
            }
            dict.Add("ZipCode", ret);
            return dict;
        }

        // - 003 Hotel Check Out
        private Dictionary<string, string> processHotelCheckOut(char[] chars, Dictionary<string, string> dict)
        {
            string ret = "";
            dict.Add("PrimaryChargeType", chars[3].ToString());
            dict.Add("SpecialCode", chars[4].ToString());
            
            for (int i = 5; i < 11; i++)
            {
                ret += chars[i];
            }
            dict.Add("HotelAdditionalCharges", ret);
            ret = "";
            
            for (int j = 11; j < 17; j++)
            {
                ret += chars[j];
            }
            dict.Add("ArrivalDate", ret);
            ret = "";

            for (int k = 17; k < 23; k++)
            {
                ret += chars[k];
            }
            dict.Add("DepartureDate", ret);
            return dict;
        }

        // - 004 Auto Return
        private Dictionary<string, string> processAutoReturn(char[] chars, Dictionary<string, string> dict)
        {
            string ret = "";
            
            for (int i = 3; i < 12; i++)
            {
                ret += chars[i];
            }
            dict.Add("RentalAgreement", ret);
            ret = "";
            
            for (int j = 12; j < 47; j++)
            {
                ret += chars[j];
            }
            dict.Add("DriverName", ret);
            ret = "";
            
            for (int k = 47; k < 61; k++)
            {
                ret += chars[k];
            }
            dict.Add("LateAdjustment", ret);
            ret = "";
            
            for (int l = 61; l < 79; l++)
            {
                ret += chars[l];
            }
            dict.Add("RentalCity", ret);
            ret = "";
            dict.Add("RentalState", chars[79].ToString() + chars[80].ToString());
            
            for (int m = 81; m < 90; m++)
            {
                ret += chars[m];
            }
            dict.Add("RentalZipCode", ret);
            ret = "";
            
            for (int n = 90; n < 96; n++)
            {
                ret += chars[n];
            }
            dict.Add("RentalDate", ret);
            ret = "";
            
            for (int o = 96; o < 101; o++)
            {
                ret += chars[o];
            }
            dict.Add("RentalTime", ret);
            ret = "";
            
            for (int p = 101; p < 120; p++)
            {
                ret += chars[p];
            }
            dict.Add("ReturnCity", ret);
            ret = "";
            dict.Add("ReturnState", chars[120].ToString() + chars[121].ToString());
            
            for (int q = 122; q < 131; q++)
            {
                ret += chars[q];
            }
            dict.Add("ReturnZipCode", ret);
            ret = "";
            
            for (int r = 131; r < 137; r++)
            {
                ret += chars[r];
            }
            dict.Add("ReturnDate", ret);
            ret = "";
            
            for (int s = 137; s < 143; s++)
            {
                ret += chars[s];
            }
            dict.Add("ReturnTime", ret);
            ret = "";
            
            for (int t = 143; t < 149; t++)
            {
                ret += chars[t];
            }
            dict.Add("AutoAdditionalCharges", ret);
            ret = "";
            dict.Add("NoShowIndicator", chars[149].ToString());
            dict.Add("ReturnCountryCode", chars[150].ToString() + chars[151].ToString() + chars[152].ToString());

            return dict;
        }

        // - 005 Debit PIN Pad
        private Dictionary<string, string> processDebitPINPad(char[] chars, Dictionary<string, string> dict)
        {
            string ret = "";
            dict.Add("PINPadType", chars[3].ToString() + chars[4].ToString());
            dict.Add("PINPadBlockFormat", chars[5].ToString() + chars[6].ToString());
            
            for (int i = 7; i < 27; i++)
            {
                ret += chars[i];
            }
            dict.Add("PINPadKey", ret);
            ret = "";

            for (int j = 27; j < chars.Length; j++)
            {
                ret += chars[j];
            }
            dict.Add("PINBlock", ret);
            return dict;
        }
        
        // - 006 Signature Capture
        private Dictionary<string, string> processSignatureCapture(char[] chars, Dictionary<string, string> dict)
        {
            string ret = "";
            dict.Add("SignatureDeviceType", chars[3].ToString() + chars[4].ToString());
            dict.Add("SignatureBlockNumber", chars[5].ToString());
            dict.Add("SignatureTotalBlocks", chars[6].ToString());
            
            for (int i = 7; i < chars.Length; i++)
            {
                ret += chars[i];
            }
            dict.Add("SignatureData", ret);

            return dict;
        }

        // - 008 Level 2 Card Block
        private Dictionary<string, string> processLevel2CardBlock(char[] chars, Dictionary<string, string> dict)
        {
            string ret = "";
            
            for (int i = 3; i < 28; i++)
            {
                ret += chars[i];
            }
            dict.Add("CustomerReference", ret);
            ret = "";
            dict.Add("TaxIndicator", chars[28].ToString());
            
            for (int j = 29; j < 41; j++)
            {
                ret += chars[j];
            }
            dict.Add("TaxAmount", ret + "." + chars[41].ToString() + chars[42].ToString());
            ret = "";
            
            for (int k = 43; k < chars.Length; k++)
            {
                ret += chars[k];
            }
            dict.Add("DestinationZipCode", ret);
            return dict;
        }

        // - 009 Purchase Card Block
        private Dictionary<string, string> processPurchasingCardBlock(char[] chars, Dictionary<string, string> dict)
        {
            string ret = "";

            int i = 3;
            while (chars[i] != (char)094)
            {
                ret += chars[i];
                i++;
            }
            i++;
            dict.Add("ProductDescriptor1", ret);
            ret = "";

            while (chars[i] != (char)094)
            {
                ret += chars[i];
                i++;
            }
            i++;
            dict.Add("ProductDescriptor2", ret);
            ret = "";

            while (chars[i] != (char)094)
            {
                ret += chars[i];
                i++;
            }
            i++;
            dict.Add("ProductDescriptor3", ret);
            ret = "";

            while (i < chars.Length)
            {
                ret += chars[i];
                i++;
            }
            dict.Add("ProductDescriptor4", ret);
            return dict;
        }

        // - 010 Valid Check ID Types
        private Dictionary<string, string> processValidCheckIDTypes(char[] chars, Dictionary<string, string> dict)
        {
            string ret = "";
            for (int i = 3; i < chars.Length; i++)
            {
                ret += chars[i];
            }
            dict.Add("ValidIDTypes", ret);
            return dict;
        }

        // - 11 Check Approval
        private Dictionary<string, string> processCheckApproval(char[] chars, Dictionary<string, string> dict)
        {
            string ret = "";
            dict.Add("IDTypeCode", chars[3].ToString() + chars[4].ToString());
            
            for (int i = 5; i < 29; i++)
            {
                ret += chars[i];
            }
            dict.Add("IDNumber", ret);
            ret = "";
            
            for (int j = 29; j < 35; j++)
            {
                ret += chars[j];
            }
            dict.Add("Birthdate", ret);
            ret = "";
            
            for (int k = 35; k < 49; k++)
            {
                ret += chars[k];
            }
            dict.Add("CheckAmount", ret);
            ret = "";
            
            for (int l = 49; l < 73; l++)
            {
                ret += chars[l];
            }
            dict.Add("HostResponse", ret);
            ret = "";
            
            for (int m = 73; m < 83; m++)
            {
                ret += chars[m];
            }
            dict.Add("ManualCheckNumber", ret);
            ret = "";
            
            for (int n = 83; n < 163; n++)
            {
                ret += chars[n];
            }
            dict.Add("RawMagneticData", ret);
            ret = "";
            dict.Add("CheckType", chars[163].ToString());
            dict.Add("ReaderIndicator", chars[164].ToString());
            
            for (int o = 165; o < 175; o++)
            {
                ret += chars[o];
            }
            dict.Add("TransitRoutingNumber", ret);
            ret = "";
            
            for (int p = 175; p < chars.Length; p++)
            {
                ret += chars[p];
            }
            dict.Add("CheckingAccountNumber", ret);
            return dict;
        }

        // - 012 DBA
        private Dictionary<string, string> processDBA(char[] chars, Dictionary<string, string> dict)
        {
            string ret = "";
            
            for (int i = 3; i < 25; i++)
            {
                ret += chars[i];
            }
            dict.Add("DBA", ret);
            ret = "";
            
            for (int j = 25; j < 63; j++)
            {
                ret += chars[j];
            }
            dict.Add("DBAAddressLine1", ret);
            ret = "";
            
            for (int k = 63; k < 101; k++)
            {
                ret += chars[k];
            }
            dict.Add("DBAAddressLine2", ret);
            ret = "";
            
            for (int l = 101; l < 114; l++)
            {
                ret += chars[l];
            }
            dict.Add("DBACity", ret);
            ret = "";
            dict.Add("DBAState", chars[114].ToString() + chars[115].ToString() + chars[116].ToString());
            
            for (int m = 117; m < 126; m++)
            {
                ret += chars[m];
            }
            dict.Add("DBAZipCode", ret);
            ret = "";
            dict.Add("MerchantType", chars[126].ToString());
            
            for (int n = 127; n < 147; n++)
            {
                ret += chars[n];
            }
            dict.Add("CardAbbreviations", ret);
            ret = "";
            
            for (int o = 147; o < 157; o++)
            {
                ret += chars[o];
            }
            dict.Add("SerialNumber", ret);
            ret = "";
            
            for (int p = 157; p < 165; p++)
            {
                ret += chars[p];
            }
            dict.Add("Revision", ret);
            ret = "";
            
            for (int q = 165; q < 180; q++)
            {
                ret += chars[q];
            }
            dict.Add("DBAPhone", ret);
            ret = "";
            
            for (int r = 180; r < chars.Length; r++)
            {
                ret += chars[r];
            }
            dict.Add("BusinessDayEndingTime", ret);
            return dict;
        }

        // - 013 Hotel Check In
        private Dictionary<string, string> processHotelCheckIn(char[] chars, Dictionary<string, string> dict)
        {
            dict.Add("HotelEstimatedDays", chars[3].ToString() + chars[4].ToString());
            return dict;
        }

        // - 014 Auto Rental
        private Dictionary<string, string> processAutoRental(char[] chars, Dictionary<string, string> dict)
        {
            dict.Add("AutoEstimatedDays", chars[3].ToString() + chars[4].ToString());
            return dict;
        }

        // - 015 Voice Authorization Center
        private Dictionary<string, string> processVoiceAuthorizationCenter(char[] chars, Dictionary<string, string> dict)
        {
            string ret = "";
            
            for (int i = 3; i < 23; i++)
            {
                ret += chars[i];
            }
            dict.Add("VoicePhoneNumber", ret);
            ret = "";
            for (int j = 23; j < chars.Length; j++)
            {
                ret += chars[j];
            }
            dict.Add("VoiceMerchantAccount", ret);
            return dict;
        }

        // - 016 Extended Authorization Block
        private Dictionary<string, string> processExtendedAuthorizationBlock(char[] chars, Dictionary<string, string> dict)
        {
            string ret = "";
            
            for (int i = 3; i < 15; i++)
            {
                ret += chars[i];
            }
            dict.Add("PreauthorizedAmount", ret + "." + chars[15].ToString() + chars[16].ToString());
            ret = "";
            
            for (int j = 17; j < 29; j++)
            {
                ret += chars[j];
            }
            dict.Add("PreauthorizedTolerance", ret + "." + chars[29].ToString() + chars[30].ToString());
            ret = "";
            
            for (int k = 31; k < 43; k++)
            {
                ret += chars[k];
            }
            dict.Add("RetrievalReference", ret);
            dict.Add("AuthSource", chars[43].ToString());

            return dict;
        }

        // - 017 Notes
        private Dictionary<string, string> processNotes(char[] chars, Dictionary<string, string> dict)
        {
            string ret = "";
            
            for (int i = 3; i < chars.Length; i++)
            {
                ret += chars[i];
            }
            dict.Add("Notes", ret);
            return dict;
        }

        // - 018 Terminal ID 
        private Dictionary<string, string> processTerminalID(char[] chars, Dictionary<string, string> dict)
        {
            string ret = "";
            for (int i = 3; i < chars.Length; i++)
            {
                ret += chars[i];
            }
            dict.Add("TerminalID", ret);
            return dict;
        }

        // - 022 Card Security Code (CVV2)
        private Dictionary<string, string> processCardSecurityCode(char[] chars, Dictionary<string, string> dict)
        {
            dict.Add("CVV2Indicator", chars[3].ToString());
            dict.Add("CVV2Code", chars[4].ToString() + chars[5].ToString() + chars[6].ToString() + chars[7].ToString());
            dict.Add("CVV2Result", chars[8].ToString());
            dict.Add("CVV2Valid", chars[9].ToString());
            return dict;
        }

        // 023 API Options
        private Dictionary<string, string> processAPIOptionsBlock(char[] chars, Dictionary<string, string> dict)
        {
            string ret = "";
            int i = 3;
            while (i < chars.Length)
            {
                ret += chars[i];
                i++;
            }
            dict.Add("APIOptions", ret);
            return dict;
        }

        // 025 IT'S YOUR CARD
        private Dictionary<string, string> processItsYourCard(char[] chars, Dictionary<string, string> dict)
        {
            string ret = "";
            dict.Add("IYCCardType", chars[3].ToString());
            
            for (int i = 4; i < 9; i++)
            {
                ret += chars[i];
            }
            dict.Add("IYCDiscount", ret);
            ret = "";
            
            for (int j = 9; j < 23; j++)
            {
                ret += chars[j];
            }
            dict.Add("IYCBalance", ret);
            ret = "";
            
            for (int k = 23; k < 37; k++)
            {
                ret += chars[k];
            }
            dict.Add("IYCAvailableBalance", ret);
            ret = "";
            
            for (int l = 37; l < 43; l++)
            {
                ret += chars[l];
            }
            dict.Add("IYCExpiration", ret);
            ret = "";
            
            for (int m = 43; m < chars.Length; m++)
            {
                ret += chars[m];
            }
            dict.Add("IYCCardFormatted", ret);
            return dict;
        }

        // 026 IT'S YOUR CARD Reason Text
        private Dictionary<string, string> processItsYourCardReasonText(char[] chars, Dictionary<string, string> dict)
        {
            string ret = "";
            
            for (int i = 3; i < chars.Length; i++)
            {
                ret += chars[i];
            }
            dict.Add("IYCReasonText", ret);
            return dict;
        }

        // 031 Override Business Date
        private Dictionary<string, string> processOverrideBusinessDate(char[] chars, Dictionary<string, string> dict)
        {
            string ret = "";
            
            for (int i = 3; i < chars.Length; i++)
            {
                ret += chars[i];
            }
            dict.Add("OverrideBusinessDate", ret);
            return dict;
        }

        // 032 Totals Report
        private Dictionary<string, string> processTotalsReport(char[] chars, Dictionary<string, string> dict)
        {
            string ret = "";
            
            for (int i = 3; i < 9; i++)
            {
                ret += chars[i];
            }
            dict.Add("ReportStartDate", ret);
            ret = "";

            for (int j = 9; j < 15; j++)
            {
                ret += chars[j];
            }
            dict.Add("ReportStartTime", ret);
            ret = "";

            for (int k = 15; k < 21; k++)
            {
                ret += chars[k];
            }
            dict.Add("ReportEndDate", ret);
            ret = "";
            
            for (int l = 21; l < 27; l++)
            {
                ret += chars[l];
            }
            dict.Add("ReportEndTime", ret);
            ret = "";
            
            for (int m = 27; m < 32; m++)
            {
                ret += chars[m];
            }
            dict.Add("ReportClerk", ret);
            ret = "";

            dict.Add("ReportCardType", chars[32].ToString() + chars[33].ToString());
            
            for (int n = 34; n < chars.Length; n++)
            {
                ret += chars[n];
            }
            dict.Add("ReportTerminalID", ret);
            return dict;
        }

        // 033 Receipt Text Block
        private Dictionary<string, string> processReceiptTextBlock(char[] chars, Dictionary<string, string> dict)
        {
            string ret = "";
            dict.Add("ReceiptTextColumns", chars[3].ToString() + chars[4].ToString() + chars[5].ToString());
            for (int i = 6; i < chars.Length; i++)
            {
                ret += chars[i];
            }
            dict.Add("ReceiptText", ret);
            return dict;
        }

        // 039 UniqueID Block
        private Dictionary<string, string> processUniqueIDBlock(char[] chars, Dictionary<string, string> dict)
        {
            string ret = "";         
            for (int i = 3; i < 19; i++)
            {
                ret += chars[i];
            }
            dict.Add("UniqueID", ret);
            ret = "";
            
            for (int j = 19; j < chars.Length; j++)
            {
                ret += chars[j];
            }
            dict.Add("TokenSerialNumber", ret);
            return dict;
        }

        // 054 Line Items
        private Dictionary<string, string> processLineItems(char[] chars, Dictionary<string, string> dict)
        {
            string ret = "";
            dict.Add("LineItemCount", chars[3].ToString() + chars[4].ToString());
            
            for (int i = 5; i < 35; i++)
            {
                ret += chars[i];
            }
            dict.Add("LineItem1", ret);
            ret = "";
            
            for (int i = 35; i < 65; i++)
            {
                ret += chars[i];
            }
            dict.Add("LineItem2", ret);
            ret = "";
            
            for (int i = 65; i < 95; i++)
            {
                ret += chars[i];
            }
            dict.Add("LineItem3", ret);
            ret = "";
            
            for (int i = 95; i < 125; i++)
            {
                ret += chars[i];
            }
            dict.Add("LineItem4", ret);
            ret = "";
            
            for (int i = 125; i < 155; i++)
            {
                ret += chars[i];
            }
            dict.Add("LineItem5", ret);
            ret = "";
            
            for (int i = 155; i < 185; i++)
            {
                ret += chars[i];
            }
            dict.Add("LineItem6", ret);
            ret = "";
            
            for (int i = 185; i < 215; i++)
            {
                ret += chars[i];
            }
            dict.Add("LineItem7", ret);
            ret = "";
            
            for (int i = 215; i < 245; i++)
            {
                ret += chars[i];
            }
            dict.Add("LineItem8", ret);
            ret = "";
            
            for (int i = 245; i < 275; i++)
            {
                ret += chars[i];
            }
            dict.Add("LineItem9", ret);
            ret = "";
            
            for (int i = 275; i < 305; i++)
            {
                ret += chars[i];
            }
            dict.Add("LineItem10", ret);
            return dict;
        }

        // 055 Line Item
        private Dictionary<string, string> processLineItem(char[] chars, Dictionary<string, string> dict)
        {
            string ret = "";
            for (int i = 3; i < chars.Length; i++)
            {
                ret += chars[i];
            }
            dict.Add("LineItem", ret);
            return dict;
        }

        // 056 Cashback and Surcharge Amounts
        private Dictionary<string, string> processCashbackAndSurchargeAmounts(char[] chars, Dictionary<string, string> dict)
        {
            string ret = "";
            
            for (int i = 3; i < 17; i++)
            {
                ret += chars[i];
            }
            dict.Add("Surcharge", ret);
            ret = "";
            
            for (int j = 17; j < chars.Length; j++)
            {
                ret += chars[j];
            }
            dict.Add("CashBack", ret);
            return dict;
        }

        // 064 BIN Management
        private Dictionary<string, string> processBINManagement(char[] chars, Dictionary<string, string> dict)
        {
            string ret = "";
            
            for (int i = 3; i < 13; i++)
            {
                ret += chars[i];
            }
            dict.Add("SpinPrefix", ret);
            dict.Add("SpinAbb", chars[13].ToString() + chars[14].ToString());
            dict.Add("SpinIsDebit", chars[15].ToString());
            dict.Add("SpinIsDCC", chars[16].ToString());
            dict.Add("SpinResult", chars[17].ToString() + chars[18].ToString());

            return dict;
        }

        // 068 Enhanced Authorization Block
        private Dictionary<string, string> processEnhancedAuthorizationBlock(char[] chars, Dictionary<string, string> dict)
        {
            string ret = "";
            dict.Add("EnhancedDataID", chars[3].ToString() + chars[4].ToString());
            
            for (int i = 5; i < chars.Length; i++)
            {
                ret += chars[i];
            }
            dict.Add("EnhancedDataValues", ret);
            return dict;
        }

        // 070 Inventory Information Approval System (IIAS)
        private Dictionary<string, string> processInventoryInformationApprovalSystem(char[] chars, Dictionary<string, string> dict)
        {
            string ret = "";
            dict.Add("IIASType1", chars[3].ToString() + chars[4].ToString());
            for (int i = 5; i < 19; i++)
            {
                ret += chars[i];
            }
            dict.Add("IIASAmount1", ret);
            ret = "";
            dict.Add("IIASType2", chars[19].ToString() + chars[20].ToString());
            for (int i = 21; i < 35; i++)
            {
                ret += chars[i];
            }
            dict.Add("IIASAmount2", ret);
            ret = "";
            dict.Add("IIASType3", chars[35].ToString() + chars[36].ToString());
            for (int i = 37; i < 51; i++)
            {
                ret += chars[i];
            }
            dict.Add("IIASAmount3", ret);
            ret = "";
            dict.Add("IIASType4", chars[52].ToString() + chars[53].ToString());
            for (int i = 54; i < chars.Length; i++)
            {
                ret += chars[i];
            }
            dict.Add("IIASAmount4", ret);
            return dict;
        }

        // 073 Four Words
        private Dictionary<string, string> processFourWords(char[] chars, Dictionary<string, string> dict)
        {
            string ret = "";
            
            for (int i = 3; i < chars.Length; i++)
            {
                ret += chars[i];
            }
            dict.Add("FourWords", ret);
            return dict;
        }

        // 074 Card Level Results
        private Dictionary<string, string> processCardLevelResults(char[] chars, Dictionary<string, string> dict)
        {
            dict.Add("CardLevelResults", chars[3].ToString() + chars[4].ToString());
            return dict;
        }

        // 075 Balance Return Block
        private Dictionary<string, string> processBalanceReturnBlock(char[] chars, Dictionary<string, string> dict)
        {
            string ret = "";
            dict.Add("BalanceReturnIndicator", chars[3].ToString());
            
            for (int i = 4; i < chars.Length; i++)
            {
                ret += chars[i];
            }
            dict.Add("Balance", ret);
            return dict;
        }

        // 076 P2PE
        private Dictionary<string, string> processP2PE(char[] chars, Dictionary<string, string> dict)
        {
            string ret = "";
            dict.Add("P2PEDeviceType", chars[3].ToString() + chars[4].ToString());
            //P2PEDeviceType = (chars[3].ToString() + chars[4].ToString());
            dict.Add("P2PEBlockLength", chars[5].ToString() + chars[6].ToString() + chars[7].ToString() + chars[8].ToString());
            //P2PEBlockLength = (chars[5].ToString() + chars[6].ToString() + chars[7].ToString() + chars[8].ToString());
            
            //P2PEBlockData = "";
            for (int i = 9; i < chars.Length; i++)
            {
                ret += chars[i];
                //P2PEBlockData += chars[i];
            }
            dict.Add("P2PEBlock", ret);
            return dict;
        }

        // 086 MetaToken
        private Dictionary<string, string> processMetaToken(char[] chars, Dictionary<string, string> dict)
        {
            string ret = "";
            dict.Add("MetaTokenType", chars[3].ToString() + chars[4].ToString());
            
            for (int i = 5; i < chars.Length; i++)
            {
                ret += chars[i];
            }
            dict.Add("MetaTokenData", ret);
            return dict;
        }

        // 087 Prompt Confirmation
        private Dictionary<string, string> processPromptConfirmation(char[] chars, Dictionary<string, string> dict)
        {
            string ret = "";
            dict.Add("PromptConfirmResult", chars[3].ToString());
            dict.Add("PromptConfirmQuestion", chars[4].ToString());
            
            for (int i = 5; i < chars.Length; i++)
            {
                ret += chars[i];
            }
            dict.Add("PromptConfrimValue", ret);
            return dict;
        }

        // 088 Photo
        private Dictionary<string, string> processPhoto(char[] chars, Dictionary<string, string> dict)
        {
            string ret = "";
            dict.Add("PhotoType", chars[3].ToString());
            
            for (int i = 4; i < chars.Length; i++)
            {
                ret += chars[i];
            }
            dict.Add("PhotoData", ret);
            return dict;
        }

        // 089 Gift Card Extended Data
        private Dictionary<string, string> processGiftCardExtendedData(char[] chars, Dictionary<string, string> dict)
        {
            string ret = "";
            
            for (int i = 3; i < chars.Length; i++)
            {
                ret += chars[i];
            }
            dict.Add("GiftCardExtendedDataValues", ret);
            return dict;
        }

        // 090 Merchant Receipt Text
        private Dictionary<string, string> processMerchantReceiptText(char[] chars, Dictionary<string, string> dict)
        {
            string ret = "";
            
            for (int i = 3; i < chars.Length; i++)
            {
                ret += chars[i];
            }
            dict.Add("MerchantReceiptText", ret);
            return dict;
        }

        // 091 Customer Receipt Text
        private Dictionary<string, string> processCustomerReceiptText(char[] chars, Dictionary<string, string> dict)
        {
            string ret = "";
            
            for (int i = 3; i < chars.Length; i++)
            {
                ret += chars[i];
            }
            dict.Add("CustomerReceiptText", ret);
            return dict;
        }

        // 094 Access Token Data Block
        private Dictionary<string, string> processAccessTokenDataBlock(char[] chars, Dictionary<string, string> dict)
        {
            string ret = "";
            
            int i = 3;
            while (i < chars.Length)
            {
                ret += chars[i];
                i++;
            }
            dict.Add("AccessToken", ret);
            return dict;
        }

        // 095 Token Exchange
        private Dictionary<string, string> processTokenExchange(char[] chars, Dictionary<string, string> dict)
        {
            string ret = "";
            
            for (int i = 3; i < 54; i++)
            {
                ret += chars[i];
            }
            dict.Add("AuthToken", ret);
            ret = "";
            
            for (int j = 54; j < chars.Length; j++)
            {
                ret += chars[j];
            }
            dict.Add("ClientGUID", ret);
            return dict;
        }

        // 097 Cloud Parameters
        private Dictionary<string, string> processCloudParameters(char[] chars, Dictionary<string, string> dict)
        {
            string ret = "";
            
            for (int i = 3; i < 13; i++)
            {
                ret += chars[i];
            }
            dict.Add("DeviceService", ret);
            ret = "";
            
            for (int j = 13; j < chars.Length; j++)
            {
                ret += chars[j];
            }
            dict.Add("DeviceGUID", ret);
            return dict;
        }

        // 098 Cloud Extended Parameters
        private Dictionary<string, string> processCloudExtendedParameters(char[] chars, Dictionary<string, string> dict)
        {
            string ret = "";
            
            for (int i = 3; i < chars.Length; i++)
            {
                ret += chars[i];
            }
            dict.Add("DeviceExtensions", ret);
            return dict;
        }

        // 100 Process Forms
        private Dictionary<string, string> processProcessForms(char[] chars, Dictionary<string, string> dict)
        {
            string ret = "";
            
            for (int i = 3; i < 15; i++)
            {
                ret += chars[i];
            }
            dict.Add("FormName", ret);
            ret = "";
            
            for (int j = 15; j < 20; j++)
            {
                ret += chars[j];
            }
            dict.Add("FormResponse", ret);
            ret = "";

            int count = 20;
            while (chars[count] != (char)094)
            {
                ret += chars[count];
                count++;
            }
            count++;
            dict.Add("KeyValue1", ret);
            ret = "";
            
            while (chars[count] != (char)094)
            {
                ret += chars[count];
                count++;
            }
            count++;
            dict.Add("KeyValue2", ret);
            ret = "";

            while (chars[count] != (char)094)
            {
                ret += chars[count];
                count++;
            }
            count++;
            dict.Add("KeyValue3", ret);
            ret = "";

            while (chars[count] != (char)094)
            {
                ret += chars[count];
                count++;
            }
            count++;
            dict.Add("KeyValue4", ret);
            ret = "";

            while (chars[count] != (char)094)
            {
                ret += chars[count];
                count++;
            }
            count++;
            dict.Add("KeyValue5", ret);
            ret = "";
            
            while (chars[count] != (char)094)
            {
                ret += chars[count];
                count++;
            }
            count++;
            dict.Add("KeyValue6", ret);
            ret = "";
            
            while (chars[count] != (char)094)
            {
                ret += chars[count];
                count++;
            }
            count++;
            dict.Add("KeyValue7", ret);
            ret = "";
            
            while (chars[count] != (char)094)
            {
                ret += chars[count];
                count++;
            }
            count++;
            dict.Add("KeyValue8", ret);
            ret = "";
            
            while (chars[count] != (char)094)
            {
                ret += chars[count];
                count++;
            }
            count++;
            dict.Add("KeyValue9", ret);
            ret = "";
            
            while (chars[count] != (char)094)
            {
                ret += chars[count];
                count++;
            }
            count++;
            dict.Add("KeyValue10", ret);
            return dict;
        }

        // 108 Terms And Conditions
        private Dictionary<string, string> processTermsAndConditions(char[] chars, Dictionary<string, string> dict)
        {
            string ret = "";
            dict.Add("TermsandConditionsResult", chars[3].ToString());
            
            for (int i = 4; i < chars.Length; i++)
            {
                ret += chars[i];
            }
            dict.Add("TermsandConditions", ret);
            return dict;
        }

        // 111 Signature Suppressed
        private Dictionary<string, string> processSignatureSuppressed(char[] chars, Dictionary<string, string> dict)
        {
            dict.Add("SignatureSuppressed", chars[3].ToString());
            return dict;
        }

        // 112 AVS/CVV Prompt
        private Dictionary<string, string> processAVS_CVVPrompt(char[] chars, Dictionary<string, string> dict)
        {
            dict.Add("CVV2Prompt", chars[3].ToString());
            dict.Add("StreetNumberPrompt", chars[4].ToString());
            dict.Add("PostalCodePrompt", chars[5].ToString());
            return dict;
        }

        // 113 Input Prompt 
        private Dictionary<string, string> processInputPrompt(char[] chars, Dictionary<string, string> dict)
        {
            string ret = "";
            dict.Add("DeviceInputIndex", chars[3].ToString() + chars[4].ToString() + chars[5].ToString());
            
            for (int i = 6; i < chars.Length; i++)
            {
                ret += chars[i];
            }
            dict.Add("DeviceInputResponse", ret);
            return dict;
        }

        // 116 Device Language
        private Dictionary<string, string> processDeviceLanguage(char[] chars, Dictionary<string, string> dict)
        {
            dict.Add("DeviceLanguage", chars[3].ToString() + chars[4].ToString() + chars[5].ToString());
            return dict;
        }

        // 999 Extended Error
        private Dictionary<string, string> processExtendedError(char[] chars, Dictionary<string, string> dict)
        {
            string ret = "";
            
            for (int i = 3; i < 19; i++)
            {
                ret += chars[i];
            }
            dict.Add("ShortError", ret);
            ret = "";

            for (int j = 19; j < chars.Length; j++)
            {
                ret += chars[j];
            }
            dict.Add("LongError", ret);
            return dict;
        }


        /******************************************************************************************************
         * Helper Methods                                                                                     *
         ******************************************************************************************************/

        private string normalize(string toBeNormalized, int size)
        {
            if (toBeNormalized.Length < size)
            {
                return rightPad(toBeNormalized, size);
            }
            else
            {
                return truncate(toBeNormalized, size);
            }
        }

        // Right pads a string with spaces to a given size
        private string rightPad(string toBePadded, int size)
        {
            string ret = toBePadded;
            for (int i = toBePadded.Length; i < size; i++)
            {
                ret += " ";
            }
            return ret;
        }

        // Left Pads a string with 0's to a given size
        private string leftPad(string toBePadded, int size)
        {
            string ret = "";
            char[] chars = toBePadded.ToCharArray();
            int pad = size - toBePadded.Length;
            for (int i = 0; i < pad; i++)
            {
                ret += "0";
            }
            ret += toBePadded;
            return ret;
        }

        // Truncates a string to a given size.
        private string truncate(string toBeTruncated, int size)
        {
            if (toBeTruncated.Length < size)
            {
                return toBeTruncated;
            }
            else
            {
                string ret = "";
                char[] chars = toBeTruncated.ToCharArray();
                for (int i = 0; i < size; i++)
                {
                    ret += chars[i];
                }
                return ret;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FormSim
{
    /// <summary>
    /// The HTTPHandler class is designed to handle FRCs over the HTTP protocol. Since this protocol uses key value
    /// pairs, it is important that the passed arguments be formatted and parsed correctly. This implementation
    /// handles both of those scenarios. See <see cref="GenericHandler"/> for the best implementation strategy 
    /// of dealing with FRCs in general.
    /// </summary>
    public class HTTPHandler : GenericHandler, FRC_Handler
    {
        protected HttpClient client;

        /******************************************************************************************************
         * Constructors                                                                                       *
         ******************************************************************************************************/

        public HTTPHandler() : base()
        {
            client = new HttpClient();
        }

        public HTTPHandler(string AuthToken, string ClientGUID) : base(AuthToken, ClientGUID)
        {
            client = new HttpClient();
        }

        public HTTPHandler(string AuthToken, string ClientGUID, string IPAddress, string Port) : base(AuthToken, ClientGUID, IPAddress, Port)
        {
            client = new HttpClient();
        }


        /******************************************************************************************************
         * Primary Functions                                                                                  *
         ******************************************************************************************************/
        
        //Exchanges AuthToken and ClientGUID for an AccessToken that is to be used in all subsequent transactions.
        public override async Task<bool> performTokenExchange()
        {
            bool success = false;
            if (AuthToken == null || ClientGUID == null || IPAddress == null || Port == null)
            {
                return success;
            }
            Random rand = new Random();
            var values = new Dictionary<string, string>
                {
                    { "STX", "YES" },
                    { "VERBOSE", "YES" },
                    { "CONTENTTYPE", "XML" },
                    { "APISignature", "$" },
                    { "APIFormat", "0" },
                    { "FunctionRequestCode", "CE" },
                    { "RequestorReference", rand.Next(0, 1000000).ToString() },
                    { "Vendor", "CottaCapital:FormSim:0.2" },
                    { "Date", helper.getDate() },
                    { "Time", helper.getTime() },
                    { "AuthToken",  AuthToken},
                    { "ClientGUID", ClientGUID },
                    { "APIOptions", "ALLDATA" },
                    { "ETX", "YES" }
                };
            log.Write("Sending Request" + Environment.NewLine + helper.parseDict(values));
            var content = new FormUrlEncodedContent(values);
            log.Write("Request:" + Environment.NewLine + content);
            HttpResponseMessage response;
            if (IPAddress.Contains("http"))
            {
                response = await client.PostAsync(new Uri(IPAddress), content);
            }
            else
            {
                response = await client.PostAsync(new Uri("http://" + IPAddress + ":" + Port), content);
            }
            string responseBody = await response.Content.ReadAsStringAsync();
            log.Write("Response:" + Environment.NewLine + responseBody);
            success = parseAccessToken(responseBody);

            return success;
        }

        // Performs the given FRC
        protected override async Task<Dictionary<string, string>> performTransaction(Dictionary<string, string> parameters)
        {
            var values = new Dictionary<string, string>();
            if (AccessToken == null)
            {
                if (useAPIMIDPWD)
                {
                    values.Add("APISerialNumber", API_SERIAL);
                    values.Add("APIPassword", API_PASSWORD);
                    values.Add("MerchantID", API_MID);
                }
                else
                {
                    bool t = await performTokenExchange();
                    values.Add("AccessToken", AccessToken);
                }
            }
            else
            {
                values.Add("AccessToken", AccessToken);
            }
            
            values.Add("STX", "YES");                               
            values.Add("VERBOSE", "YES");                           
            values.Add("CONTENTTYPE", "XML");                       
            values.Add("APISignature", "$");                        
            values.Add("APIFormat", "0");
            //values.Add("APISerialNumber", "265");
            //values.Add("APIPassword", "certifymega");
            //values.Add("MerchantID", "76646");
            

            values = buildRequestDict(values, parameters);
            values.Add("ETX", "YES");

            log.Write("Sending Request" + Environment.NewLine + helper.parseDict(values));
            var content = new FormUrlEncodedContent(values);
            //log.Write("Raw - " + content.ReadAsStringAsync().Result);
            HttpResponseMessage response;
            if (IPAddress.Contains("http")) // Direct to engine
            {
                response = await client.PostAsync(new Uri(IPAddress), content);
            }
            else if (Port == "16450" || Port == "16455") // USE SSL TLS 1.2
            {
                response = await client.PostAsync(new Uri("https://" + IPAddress + ":" + Port), content);
            }
            else // DON'T USE SSL
            {
                response = await client.PostAsync(new Uri("http://" + IPAddress + ":" + Port), content);
            }
            string responseBody = await response.Content.ReadAsStringAsync();
            Dictionary<string, string> res = parseXMLbasedResults(responseBody);
            log.Write("Response Received" + Environment.NewLine + helper.parseDict(res));

            if (parameters["UseBasicTranFlow"] == "N") { /* Do nothing, and return the response */}
            else if (res["errorindicator"] == "Y")
            {
                if (res["primaryerrorcode"] == "1001" || res["primaryerrorcode"] == "2011" || res["primaryerrorcode"] == "4003" || res["primaryerrorcode"] == "9012" ||
                    res["primaryerrorcode"] == "9018" || res["primaryerrorcode"] == "9020" || res["primaryerrorcode"] == "9023" || res["primaryerrorcode"] == "9033" ||
                    res["primaryerrorcode"] == "9489" || res["primaryerrorcode"] == "9901" || res["primaryerrorcode"] == "9902" || res["primaryerrorcode"] == "9951" ||
                    res["primaryerrorcode"] == "9957" || res["primaryerrorcode"] == "9960" || res["primaryerrorcode"] == "9961" || res["primaryerrorcode"] == "9962" ||
                    res["primaryerrorcode"] == "9964" || res["primaryerrorcode"] == "9978")
                {

                    if (parameters["FunctionRequestCode"] == "07")
                    {
                        log.Write("An error has occured with invoice # " + parameters["Invoice"]);
                        res.Add("controlerror", ("An error has occured with invoice # " + parameters["Invoice"]));
                    }
                    else
                    {
                        parameters["FunctionRequestCode"] = "07";
                        parameters["RequestorRefernce"] = (Int32.Parse(parameters["RequestorReference"]) + 1).ToString();
                        if (parameters["CardDataType"] == "UnencryptedCardData")
                        {
                            string card = parameters["CardNumber"];
                            char[] numbers = card.ToCharArray();
                            int size = numbers.Length;
                            parameters["CardNumber"] = numbers[size - 4].ToString() + numbers[size - 3].ToString() + numbers[size - 2].ToString() + numbers[size - 1];
                        }
                        else if (parameters["CardDataType"] == "UTGControlledPINPad")
                        {
                            string card = res["cardnumber"];
                            char[] numbers = card.ToCharArray();
                            int size = numbers.Length;
                            parameters["CardNumber"] = numbers[size - 4].ToString() + numbers[size - 3].ToString() + numbers[size - 2].ToString() + numbers[size - 1];
                        }
                        System.Threading.Thread.Sleep(5000);
                        res = await startTransaction(parameters);
                        return res;
                    }

                }
                else if (res["primaryerrorcode"] == "9551")
                {
                    if (res["secondaryerrorcode"] == "4")
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
                else if (res["primaryerrorcode"] == "9402" || res["primaryerrorcode"] == "9501" || res["primaryerrorcode"] == "9775" || res["primaryerrorcode"] == "9819" ||
                         res["primaryerrorcode"] == "9825" || res["primaryerrorcode"] == "9836" || res["primaryerrorcode"] == "9847" || res["primaryerrorcode"] == "9864" ||
                         res["primaryerrorcode"] == "9955" || res["primaryerrorcode"] == "9956" || res["primaryerrorcode"] == "9992" || res["primaryerrorcode"] == "9999")
                {
                    log.Write("PIN Pad Error");
                    res.Add("controlerror", "PIN Pad Error");
                }
                else
                {
                    log.Write("Error Detected: " + parameters["Invoice"] + " errored with code: " + res["primaryerrorcode"]);
                    parameters["RequestorRefernce"] = (Int32.Parse(parameters["RequestorReference"]) + 1).ToString();
                    res = await startTransaction(voidTransaction(parameters, res));
                }

            }
            else
            {
                // No Error Scenario
                if (res["response"] == "A" || res["response"] == "C")
                {
                    // Transaction is approved .... DO Stuff
                    string cvvValid = "Y";
                    string avsValid = "Y";
                    try { cvvValid = res["cvv2valid"]; }
                    catch (Exception e) { log.Write("cvv2valid was not retuned"); }
                    try { avsValid = (res["validavs"] == "N") ? "N": "Y"; }
                    catch (Exception e) { log.Write("validavs not returned"); }

                    if (res["functionrequestcode"] == "08")
                    {
                        return res;
                    }
                    else if (parameters["VoidInvalidAVS"] == "Y" && res["saleflag"] == "S" && avsValid == "N")
                    {
                        log.Write("Voiding due to Invalid AVS");
                        parameters["RequestorRefernce"] = (Int32.Parse(parameters["RequestorReference"]) + 1).ToString();
                        res = await startTransaction(voidTransaction(parameters, res));
                    }
                    else if (parameters["VoidInvalidCVV2"] == "Y" && res["saleflag"] == "S" && cvvValid == "N")
                    {
                        log.Write("Voiding due to Invalid CVV2");
                        parameters["RequestorRefernce"] = (Int32.Parse(parameters["RequestorReference"]) + 1).ToString();
                        res = await startTransaction(voidTransaction(parameters, res));
                    }
                    else
                    {
                        log.Write("Processing Transaction anyway");
                        return res;
                    }
                }
                else if (res["response"] == "D")
                {
                    // Transaction is Declined ... Void Transaction
                    log.Write("Transaction was declined");
                    parameters["RequestorRefernce"] = (Int32.Parse(parameters["RequestorReference"]) + 1).ToString();
                    res = await startTransaction(voidTransaction(parameters, res));
                }
                else if (res["response"] == "R")
                {
                    // Referral, do voice or decline
                    // Doing Void ... REFERAL NOT SUPPORTED
                    log.Write("Referral Not Supported");
                    parameters["RequestorRefernce"] = (Int32.Parse(parameters["RequestorReference"]) + 1).ToString();
                    res = await startTransaction(voidTransaction(parameters, res));
                }
                else if (res["response"] == "f")
                {
                    // CC Only, AVS or CVV2 failure ... Void Transaction
                    log.Write("Invalid AVS or CVV2");
                    if (parameters["VoidInvalidAVS"] == "Y" && res["validavs"] == "N")
                    {
                        parameters["RequestorRefernce"] = (Int32.Parse(parameters["RequestorReference"]) + 1).ToString();
                        res = await startTransaction(voidTransaction(parameters, res));
                    }
                    else if (parameters["VoidInvalidCVV2"] == "Y" && res["cvv2valid"] == "N")
                    {
                        parameters["RequestorRefernce"] = (Int32.Parse(parameters["RequestorReference"]) + 1).ToString();
                        res = await startTransaction(voidTransaction(parameters, res));
                    }
                    else
                    {
                        log.Write("Processing Transaction anyway");
                    }
                    
                }
                else if (res["response"] == "e")
                {
                    // Error condition exists
                    if (res["authorization"] == "001001" || res["authorization"] == "002011" || res["authorization"] == "004003" || res["authorization"] == "009012" ||
                         res["authorization"] == "009018" || res["authorization"] == "009020" || res["authorization"] == "009023" || res["authorization"] == "009033" ||
                         res["authorization"] == "009489" || res["authorization"] == "009901" || res["authorization"] == "009902" || res["authorization"] == "009951" ||
                         res["authorization"] == "009957" || res["authorization"] == "009960" || res["authorization"] == "009961" || res["authorization"] == "009962" ||
                         res["authorization"] == "009964" || res["authorization"] == "009978")
                    {
                        log.Write("An error has occured with invoice # " + parameters["Invoice"]);
                        res.Add("controlerror", ("An error has occured with invoice # " + parameters["Invoice"]));
                    }
                    else
                    {
                        // This may need help
                        log.Write("Transaction had an error");
                        parameters["RequestorRefernce"] = (Int32.Parse(parameters["RequestorReference"]) + 1).ToString();
                        res = await startTransaction(voidTransaction(parameters, res));
                    }

                }
                else
                {
                    // Log Transaction and notify
                    if (res["functionrequestcode"] == "08")
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

        // Builds the recursive void functionality
        private Dictionary<string, string> voidTransaction(Dictionary<string, string> req, Dictionary<string, string> res)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            result.Add("STX", "YES");
            result.Add("VERBOSE", "YES");
            result.Add("CONTENTTYPE", "XML");
            result.Add("APISignature", "$");
            result.Add("APIFormat", "0");
            result.Add("AccessToken", AccessToken);
            result.Add("ETX", "YES");
            result.Add("FunctionRequestCode", "08");
            result.Add("Invoice", req["Invoice"]);
            result.Add("APIOptions", req["APIOptions"]);
            result.Add("Date", helper.getDate());
            result.Add("Time", helper.getTime());
            result.Add("ReceiptTextColumns", "30");
            result.Add("Vendor", "CottaCapital:FormSim:0.2");

            if (req["CardNumber"] != "")
            {
                string card = req["CardNumber"];
                char[] numbers = card.ToCharArray();
                int size = numbers.Length;
                req["CardNumber"] = numbers[size - 4].ToString() + numbers[size - 3].ToString() + numbers[size - 2].ToString() + numbers[size - 1];
            }
            try
            {
                if (res["uniqueid"] == "")
                {
                    result.Add("CardNumber", req["CardNumber"]);
                }
                else
                {
                    result.Add("UniqueID", res["uniqueid"]);
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

            result.Add("UseBasicTranFlow", req["UseBasicTranFlow"]);
            result.Add("TranID", res["tranid"]);                                                      // Only used for roll backs of incremental auths
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

        // Sends the raw string to Shift4 
        public override async Task<Dictionary<string, string>> sendRaw(string rawString)
        {
            HttpResponseMessage response;
            var content = System.Text.Encoding.UTF8.GetBytes(rawString);
            ByteArrayContent bytes = new ByteArrayContent(content);
            if (IPAddress.Contains("http"))
            {
                response = await client.PostAsync(new Uri(IPAddress), bytes);
            }
            else
            {
                response = await client.PostAsync(new Uri("http://" + IPAddress + ":" + Port), bytes);
            }
            string responseBody = await response.Content.ReadAsStringAsync();
            Dictionary<string, string> res = parseXMLbasedResults(responseBody);
            log.Write("Response Received" + Environment.NewLine + helper.parseDict(res));
            return res;
        }


        /******************************************************************************************************
         * Dict Builder Methods                                                                               *
         ******************************************************************************************************/

        // Used as a selector for which message structure to build
        private Dictionary<string, string> buildRequestDict(Dictionary<string, string> dict, Dictionary<string, string> parameters)
        {
            if (parameters["FunctionRequestCode"] == "1B")
            {
                // Building Online Authorization
                log.Write("Building Online Authorization (1B)");
                dict = buildOnlineAuthorization(dict, parameters);
            }
            else if (parameters["FunctionRequestCode"] == "1D")
            {
                // Building Online Sale
                log.Write("Building Online Sale (1D)");
                dict = buildOnlineSale(dict, parameters);
            }
            else if (parameters["FunctionRequestCode"] == "05")
            {
                // Building Offline Authorization
                log.Write("Building Offline Authorization (05)");
                dict = buildOfflineAuthorization(dict, parameters);
            }
            else if (parameters["FunctionRequestCode"] == "06")
            {
                // Building Offline Sale
                log.Write("Building Offline Sale (06)");
                dict = buildOfflineSale(dict, parameters);
            }
            else if (parameters["FunctionRequestCode"] == "08")
            {
                // Building Void
                log.Write("Building Void (08)");
                dict = buildVoid(dict, parameters);
            }
            else if (parameters["FunctionRequestCode"] == "07")
            {
                // Building Get Invoice Information
                log.Write("Building Get Invoice Information (07)");
                dict = buildGetInvoiceInformation(dict, parameters);
            }
            else if (parameters["FunctionRequestCode"] == "0B")
            {
                // Building Get DBA Information
                log.Write("Building Get DBA Information (0B)");
                dict = buildGetDBAInformation(dict, parameters);
            }
            else if (parameters["FunctionRequestCode"] == "20")
            {
                // Building Upload Signature
                log.Write("Building Upload Signature (20)");
                dict = buildUploadSignature(dict, parameters);
            }
            else if (parameters["FunctionRequestCode"] == "22")
            {
                // Building Get Voice Center Information
                log.Write("Building Get Voice Center Information (22)");
                dict = buildGetVoiceCenterInformation(dict, parameters);
            }
            else if (parameters["FunctionRequestCode"] == "23")
            {
                // Building Identify Card Type
                log.Write("Building Identify Card Type (23)");
                dict = buildIdentifyCardType(dict, parameters);
            }
            else if (parameters["FunctionRequestCode"] == "2F")
            {
                // Building Verify Card with Processor
                log.Write("Building Verify Card with Processor (2F)");
                dict = buildVerifyCardWithProcessor(dict, parameters);
            }
            else if (parameters["FunctionRequestCode"] == "47")
            {
                // Building Prompt for Signature
                log.Write("Building Prompt for Signature (47)");
                dict = buildPromptForSignature(dict, parameters);
            }
            else if (parameters["FunctionRequestCode"] == "64")
            {
                // Building Get Four Words
                log.Write("Building Get Four Words (64)");
                dict = buildGetFourWords(dict, parameters);
            }
            else if (parameters["FunctionRequestCode"] == "CA")
            {
                // Building Status Request
                log.Write("Building Status Request (CA)");
                dict = buildStatusRequest(dict, parameters);
            }
            else if (parameters["FunctionRequestCode"] == "F1")
            {
                // Building Print Receipt to PIN Pad
                log.Write("Building Print Reciept to PIN Pad (F1)");
                dict = buildPrintReceiptToPINPad(dict, parameters);
            }
            else if (parameters["FunctionRequestCode"] == "F2")
            {
                // Building Get Device Information
                log.Write("Building Get Device Information (F2)");
                dict = buildGetDeviceInformation(dict, parameters);
            }
            else if (parameters["FunctionRequestCode"] == "82")
            {
                // Building Prompt For Confirmation
                log.Write("Building Prompt For Confirmation (82)");
                dict = buildPromptForConfirmation(dict, parameters);
            }
            else if (parameters["FunctionRequestCode"] == "86")
            {
                // Building Display Custom Form
                log.Write("Building Display Custom Form (86)");
                dict = buildDisplayCustomForm(dict, parameters);
            }
            else if (parameters["FunctionRequestCode"] == "CF")
            {
                // Building Prompt for Terms and Conditions
                log.Write("Building Prompt for Terms and Conditions (CF)");
                dict = buildPromptForTermsAndConditions(dict, parameters);
            }
            else if (parameters["FunctionRequestCode"] == "DA")
            {
                // Building On-Demand Card Read
                log.Write("Building On-Demand Card Read (DA)");
                dict = buildOnDemandCardRead(dict, parameters);
            }
            else if (parameters["FunctionRequestCode"] == "DB")
            {
                // Building Prompt for Input
                log.Write("Building Prompt for Input (DB)");
                dict = buildPromptForInput(dict, parameters);
            }
            else if (parameters["FunctionRequestCode"] == "17")
            {
                // Building Get Acceptable Identification Types for Checks
                log.Write("Building Get Acceptable Identification Types for Checks (17)");
                dict = buildGetAcceptableIDTypesForChecks(dict, parameters);
            }
            else if (parameters["FunctionRequestCode"] == "1F")
            {
                // Building Check Approval
                log.Write("Building Check Approval (1F)");
                dict = buildCheckApproval(dict, parameters);
            }
            else if (parameters["FunctionRequestCode"] == "5F")
            {
                // Building Get Totals Report
                log.Write("Building Get Totals Report (5F)");
                dict = buildGetTotalsReport(dict, parameters);
            }
            else if (parameters["FunctionRequestCode"] == "92")
            {
                // Building Display Line Item
                log.Write("Building Display Line Item (92)");
                dict = buildDisplayLineItem(dict, parameters);
            }
            else if (parameters["FunctionRequestCode"] == "94")
            {
                // Building Clear Line Items
                log.Write("Building Clear Line Items (94)");
                dict = buildClearLineItems(dict, parameters);
            }
            else if (parameters["FunctionRequestCode"] == "95")
            {
                // Building Display Line Items
                log.Write("Building Display Line Items (95)");
                dict = buildDisplayLineItems(dict, parameters);
            }
            else if (parameters["FunctionRequestCode"] == "96")
            {
                // Building Swipe Ahead
                log.Write("Building Swipe Ahead (96)");
                dict = buildSwipeAhead(dict, parameters);
            }
            else if (parameters["FunctionRequestCode"] == "97")
            {
                // Building Reset PIN Pad
                log.Write("Building Reset PIN Pad (97)");
                dict = buildResetPINPad(dict, parameters);
            }
            else if (parameters["FunctionRequestCode"] == "24")
            {
                // Building Activate/Reload Gift Card
                log.Write("Building Activate/Reload Gift Card (24)");
                dict = buildActivateReloadGiftCard(dict, parameters);
            }
            else if (parameters["FunctionRequestCode"] == "25")
            {
                // Building Deactivate Gift Card
                log.Write("Building Deactivate Gift Card (25)");
                dict = buildDeactivateGiftCard(dict, parameters);
            }
            else if (parameters["FunctionRequestCode"] == "26")
            {
                // Building Reactivate Gift Card
                log.Write("Building Reactivate Gift Card (26)");
                dict = buildReactivateGiftCard(dict, parameters);
            }
            else if (parameters["FunctionRequestCode"] == "61")
            {
                // Building Get Balance Inquiry
                log.Write("Building Get Balance Inquiry (61)");
                dict = buildGetBalanceInquiry(dict, parameters);
            }
            else if (parameters["FunctionRequestCode"] == "E0")
            {
                // Building TokenStore Add
                log.Write("Building TokenStore Add (E0)");
                dict = buildTokenStoreAdd(dict, parameters);
            }
            else if (parameters["FunctionRequestCode"] == "E2")
            {
                // Building TokenStore Duplicate
                log.Write("Building TokenStore Duplicate (E2)");
                dict = buildTokenStoreDuplicate(dict, parameters);
            }
            else if (parameters["FunctionRequestCode"] == "D7")
            {
                // Building Block Card
                log.Write("Building Block Card (D7)");
                dict = buildBlockCard(dict, parameters);
            }
            else if (parameters["FunctionRequestCode"] == "D8")
            {
                // Building Unblock Card
                log.Write("Building Unblock Card (D8)");
                dict = buildUnblockCard(dict, parameters);
            }
            else if (parameters["FunctionRequestCode"] == "D9")
            {
                // Building Card Block Status
                log.Write("Building Card Block Status (D9)");
                dict = buildCardBlockStatus(dict, parameters);
            }
            else if (parameters["FunctionRequestCode"] == "CD")
            {
                // Building Get MetaToken
                log.Write("Building Get MetaToken (CD)");
                dict = buildGetMetaToken(dict, parameters);
            }

            return dict;
        }

        // 1B - Online Authorization
        private Dictionary<string, string> buildOnlineAuthorization(Dictionary<string, string> dict, Dictionary<string, string> parameters)
        {
            dict.Add("APIOptions", parameters["APIOptions"]);
            dict.Add("Clerk", parameters["Clerk"]);
            dict.Add("Date", helper.getDate());
            dict.Add("FunctionRequestCode", parameters["FunctionRequestCode"]);
            dict.Add("Invoice", parameters["Invoice"]);
            dict.Add("PrimaryAmount", parameters["PrimaryAmount"]);
            dict.Add("ReceiptTextColumns", "30");
            dict.Add("RequestorReference", parameters["RequestorReference"]);
            dict.Add("SaleFlag", parameters["SaleFlag"]);
            dict.Add("Time", helper.getTime());
            dict.Add("Vendor", "CottaCapital:FormSim:0.2");

            // Only Send on initial Auth/Sale Request
            dict.Add("CardPresent", parameters["CardPresent"]);

            //dict = buildCardDataType(dict, parameters);
            // Card Entry [Send ONE of the Five - XOR]
            // ONE - Unencrypted Card Data
            if (parameters["CardDataType"] == "UnencryptedCardData")
            {
                dict.Add("CardNumber", parameters["CardNumber"]);
                dict.Add("ExpirationDate", parameters["ExpirationDate"]);
                if (parameters["CVV2"] != "")
                {
                    dict.Add("CVV2Indicator", "1");
                    dict.Add("CVV2Code", parameters["CVV2"]);
                }
                // AVS
                dict = buildAVSData(dict, parameters);
            }
            // TWO - Unencrypted MSR (Track Data)
            else if (parameters["CardDataType"] == "UnencryptedTrackData")
            {
                dict.Add("TrackInformation", parameters["TrackData"]);
                // AVS
                dict = buildAVSData(dict, parameters);
            }
            // THREE - UTG-Controlled PIN Pad
            else if (parameters["CardDataType"] == "UTGControlledPINPad")
            {
                dict.Add("TerminalID", parameters["TerminalID"]);
            }
            // FOUR - Card-on-file/TrueToken
            else if (parameters["CardDataType"] == "TrueToken")
            {
                dict.Add("UniqueID", parameters["UniqueID"]);
            }
            // FIVE - P2PE
            else if (parameters["CardDataType"] == "P2PE")
            {
                dict.Add("P2PEBlock", parameters["P2PEBlock"]);
                dict.Add("P2PEDeviceType", (parameters["P2PEBlock"].Contains('*')) ? "01" : "02");
                //dict.Add("P2PEBlockLength", "4");
                // AVS
                dict = buildAVSData(dict, parameters);
            }
            // End XOR

            // Level 2 Card Data
            dict = buildLevel2CardData(dict, parameters);

            // Purchasing Card Data
            dict = buildPurchasingCardData(dict, parameters);

            // Tip Entry
            if (parameters["SecondaryAmount"] != "0.00")
            {
                dict = buildTipData(dict, parameters);
            }

            // Hotel
            if (MerchantType == "Hotel")
            {
                dict = buildHotelAuth(dict, parameters);
            }

            // MetaToken
            if (parameters["UseMetaToken"] == "Y")
            {
                dict = buildMetaToken(dict, parameters);
            }

            // Miscellaneous
            dict = buildMiscData(dict, parameters);

            // Global TokenStore
            if (parameters["UseTokenStore"] == "Y")
            {
                dict = buildTokenStoreData(dict, parameters);
            }

            // Signature Capture
            //dict = buildSignatureCaptureData(dict, parameters);

            // HSA/FSA
            //dict = buildHSA_FSAData(dict, parameters);

            // Non-UTG PIN Debit
            //dict = buildNonUTGPinDebitData(dict, parameters);

            // Auto Rental
            if (MerchantType == "Auto")
            {
                dict = buildAutoRentalAuthData(dict, parameters);
            }

            return dict;
        }

        // 1D - Online Sale
        private Dictionary<string, string> buildOnlineSale(Dictionary<string, string> dict, Dictionary<string, string> parameters)
        {
            dict.Add("APIOptions", parameters["APIOptions"]);
            dict.Add("Clerk", parameters["Clerk"]);
            dict.Add("Date", helper.getDate());
            dict.Add("FunctionRequestCode", parameters["FunctionRequestCode"]);
            dict.Add("Invoice", parameters["Invoice"]);
            dict.Add("PrimaryAmount", parameters["PrimaryAmount"]);
            dict.Add("ReceiptTextColumns", "30");
            dict.Add("RequestorReference", parameters["RequestorReference"]);
            dict.Add("SaleFlag", parameters["SaleFlag"]);
            dict.Add("Time", helper.getTime());
            dict.Add("Vendor", "CottaCapital:FormSim:0.2");

            // Only Send on initial Auth/Sale Request
            dict.Add("CardPresent", parameters["CardPresent"]);

            // Card Entry [Send ONE of the Five - XOR]
            // ONE - Unencrypted Card Data
            if (parameters["CardDataType"] == "UnencryptedCardData")
            {
                dict.Add("CardNumber", parameters["CardNumber"]);
                dict.Add("ExpirationDate", parameters["ExpirationDate"]);
                if (parameters["CVV2"] != "")
                {
                    dict.Add("CVV2Indicator", "1");
                    dict.Add("CVV2Code", parameters["CVV2"]);
                }
                // AVS
                dict = buildAVSData(dict, parameters);
            }
            // TWO - Unencrypted MSR (Track Data)
            else if (parameters["CardDataType"] == "UnencryptedTrackData")
            {
                dict.Add("TrackInformation", parameters["TrackData"]);
                // AVS
                dict = buildAVSData(dict, parameters);
            }
            // THREE - UTG-Controlled PIN Pad
            else if (parameters["CardDataType"] == "UTGControlledPINPad")
            {
                dict.Add("TerminalID", parameters["TerminalID"]);
            }
            // FOUR - Card-on-file/TrueToken
            else if (parameters["CardDataType"] == "TrueToken")
            {
                dict.Add("UniqueID", parameters["UniqueID"]);
            }
            // FIVE - P2PE
            else if (parameters["CardDataType"] == "P2PE")
            {
                dict.Add("P2PEBlock", parameters["P2PEBlock"]);
                dict.Add("P2PEDeviceType", (parameters["P2PEBlock"].Contains('*')) ? "01" : "02");
                //dict.Add("P2PEBlockLength", "4");
                // AVS
                dict = buildAVSData(dict, parameters);
            }
            // End XOR


            // Level 2 Card Data
            dict = buildLevel2CardData(dict, parameters);

            // Purchasing Card Data
            dict = buildPurchasingCardData(dict, parameters);

            // Tip Entry
            if (parameters["SecondaryAmount"] != "0.00")
            {
                dict = buildTipData(dict, parameters);
            }

            // Cash Back Entry
            if (parameters["CashBack"] != "0.00")
            {
                dict = buildCashBackData(dict, parameters);
            }

            // Hotel
            if (MerchantType == "Hotel")
            {
                dict = buildHotelSale(dict, parameters);
            }

            // MetaToken
            if (parameters["UseMetaToken"] == "Y")
            {
                dict = buildMetaToken(dict, parameters);
            }

            // Miscellaneous
            dict = buildMiscData(dict, parameters);

            // Global TokenStore
            if (parameters["UseTokenStore"] == "Y")
            {
                dict = buildTokenStoreData(dict, parameters);
            }

            // Signature Capture
            //dict = buildSignatureCaptureData(dict, parameters);

            // HSA/FSA
            //dict = buildHSA_FSAData(dict, parameters);

            // Non-UTG PIN Debit
            //dict = buildNonUTGPinDebitData(dict, parameters);

            // Auto Rental
            if (MerchantType == "Auto")
            {
                dict = buildAutoRentalSaleData(dict, parameters);
            }

            return dict;
        }

        // 05 - Offline Authorization 
        private Dictionary<string, string> buildOfflineAuthorization(Dictionary<string, string> dict, Dictionary<string, string> parameters)
        {
            dict.Add("APIOptions", parameters["APIOptions"]);
            dict.Add("Clerk", parameters["Clerk"]);
            dict.Add("Date", helper.getDate());
            dict.Add("FunctionRequestCode", parameters["FunctionRequestCode"]);
            dict.Add("Invoice", parameters["Invoice"]);
            dict.Add("PrimaryAmount", parameters["PrimaryAmount"]);
            dict.Add("ReceiptTextColumns", "30");
            dict.Add("RequestorReference", parameters["RequestorReference"]);
            dict.Add("SaleFlag", parameters["SaleFlag"]);
            dict.Add("Time", helper.getTime());
            dict.Add("Vendor", "CottaCapital:FormSim:0.2");

            // Auth
            dict.Add("Authorization", "123456");

            // Card Entry [Send ONE of the Five - XOR]
            // ONE - Unencrypted Card Data
            if (parameters["CardDataType"] == "UnencryptedCardData")
            {
                dict.Add("CardNumber", parameters["CardNumber"]);
                dict.Add("ExpirationDate", parameters["ExpirationDate"]);
                if (parameters["CVV2"] != "")
                {
                    dict.Add("CVV2Indicator", "1");
                    dict.Add("CVV2Code", parameters["CVV2"]);
                }
                // AVS
                dict = buildAVSData(dict, parameters);
            }
            // TWO - Unencrypted MSR (Track Data)
            else if (parameters["CardDataType"] == "UnencryptedTrackData")
            {
                dict.Add("TrackInformation", parameters["TrackData"]);
                // AVS
                dict = buildAVSData(dict, parameters);
            }
            // THREE - UTG-Controlled PIN Pad
            else if (parameters["CardDataType"] == "UTGControlledPINPad")
            {
                dict.Add("TerminalID", parameters["TerminalID"]);
            }
            // FOUR - Card-on-file/TrueToken
            else if (parameters["CardDataType"] == "TrueToken")
            {
                dict.Add("UniqueID", parameters["UniqueID"]);
            }
            // FIVE - P2PE
            else if (parameters["CardDataType"] == "P2PE")
            {
                dict.Add("P2PEBlock", parameters["P2PEBlock"]);
                dict.Add("P2PEDeviceType", (parameters["P2PEBlock"].Contains('*')) ? "01" : "02");
                //dict.Add("P2PEBlockLength", "4");
                // AVS
                dict = buildAVSData(dict, parameters);
            }
            // End XOR


            // Level 2 Card Data
            dict = buildLevel2CardData(dict, parameters);

            // Purchasing Card Data
            dict = buildPurchasingCardData(dict, parameters);

            // Tip Entry
            if (parameters["SecondaryAmount"] != "0.00")
            {
                dict = buildTipData(dict, parameters);
            }

            // Hotel
            if (MerchantType == "Hotel")
            {
                dict = buildHotelAuth(dict, parameters);
            }

            // MetaToken
            if (parameters["UseMetaToken"] == "Y")
            {
                dict = buildMetaToken(dict, parameters);
            }

            // Miscellaneous
            dict = buildMiscData(dict, parameters);

            // Global TokenStore
            if (parameters["UseTokenStore"] == "Y")
            {
                dict = buildTokenStoreData(dict, parameters);
            }

            // Signature Capture
            //dict = buildSignatureCaptureData(dict, parameters);

            // HSA/FSA
            //dict = buildHSA_FSAData(dict, parameters);

            // Non-UTG PIN Debit
            //dict = buildNonUTGPinDebitData(dict, parameters);

            // Auto Rental
            if (MerchantType == "Auto")
            {
                dict = buildAutoRentalAuthData(dict, parameters);
            }

            return dict;
        }

        // 06 - Offline Sale
        private Dictionary<string, string> buildOfflineSale(Dictionary<string, string> dict, Dictionary<string, string> parameters)
        {
            dict.Add("APIOptions", parameters["APIOptions"]);
            dict.Add("Clerk", parameters["Clerk"]);
            dict.Add("Date", helper.getDate());
            dict.Add("FunctionRequestCode", parameters["FunctionRequestCode"]);
            dict.Add("Invoice", parameters["Invoice"]);
            dict.Add("PrimaryAmount", parameters["PrimaryAmount"]);
            dict.Add("ReceiptTextColumns", "30");
            dict.Add("RequestorReference", parameters["RequestorReference"]);
            dict.Add("SaleFlag", parameters["SaleFlag"]);
            dict.Add("Time", helper.getTime());
            dict.Add("Vendor", "CottaCapital:FormSim:0.2");

            // Auth
            dict.Add("Authorization", "123456");

            // Card Entry [Send ONE of the Five - XOR]
            // ONE - Unencrypted Card Data
            if (parameters["CardDataType"] == "UnencryptedCardData")
            {
                dict.Add("CardNumber", parameters["CardNumber"]);
                dict.Add("ExpirationDate", parameters["ExpirationDate"]);
                if (parameters["CVV2"] != "")
                {
                    dict.Add("CVV2Indicator", "1");
                    dict.Add("CVV2Code", parameters["CVV2"]);
                }
                // AVS
                dict = buildAVSData(dict, parameters);
            }
            // TWO - Unencrypted MSR (Track Data)
            else if (parameters["CardDataType"] == "UnencryptedTrackData")
            {
                dict.Add("TrackInformation", parameters["TrackData"]);
                // AVS
                dict = buildAVSData(dict, parameters);
            }
            // THREE - UTG-Controlled PIN Pad
            else if (parameters["CardDataType"] == "UTGControlledPINPad")
            {
                dict.Add("TerminalID", parameters["TerminalID"]);
            }
            // FOUR - Card-on-file/TrueToken
            else if (parameters["CardDataType"] == "TrueToken")
            {
                dict.Add("UniqueID", parameters["UniqueID"]);
            }
            // FIVE - P2PE
            else if (parameters["CardDataType"] == "P2PE")
            {
                dict.Add("P2PEBlock", parameters["P2PEBlock"]);
                dict.Add("P2PEDeviceType", (parameters["P2PEBlock"].Contains('*')) ? "01" : "02");
                //dict.Add("P2PEBlockLength", "4");
                // AVS
                dict = buildAVSData(dict, parameters);
            }
            // End XOR


            // Level 2 Card Data
            dict = buildLevel2CardData(dict, parameters);

            // Purchasing Card Data
            dict = buildPurchasingCardData(dict, parameters);

            // Tip Entry
            if (parameters["SecondaryAmount"] != "0.00")
            {
                dict = buildTipData(dict, parameters);
            }

            // Hotel
            if (MerchantType == "Hotel")
            {
                dict = buildHotelSale(dict, parameters);
            }

            // MetaToken
            if (parameters["UseMetaToken"] == "Y")
            {
                dict = buildMetaToken(dict, parameters);
            }

            // Miscellaneous
            dict = buildMiscData(dict, parameters);

            // Global TokenStore
            if (parameters["UseTokenStore"] == "Y")
            {
                dict = buildTokenStoreData(dict, parameters);
            }

            // Signature Capture
            //dict = buildSignatureCaptureData(dict, parameters);

            // HSA/FSA
            //dict = buildHSA_FSAData(dict, parameters);

            // Non-UTG PIN Debit
            //dict = buildNonUTGPinDebitData(dict, parameters);

            // Auto Rental
            if (MerchantType == "Auto")
            {
                dict = buildAutoRentalSaleData(dict, parameters);
            }

            return dict;
        }

        // 08 - Void
        private Dictionary<string, string> buildVoid(Dictionary<string, string> dict, Dictionary<string, string> parameters)
        {
            string[] toBeStripped = { "ALLOWPARTIALAUTH", "BYPASSAMOUNTOK", "BYPASSSIGCAP", "NOSIGNATURE", "PRINTTIPLINE" };
            parameters = helper.stripAPIParameters(parameters, toBeStripped);
            dict.Add("APIOptions", parameters["APIOptions"]);
            dict.Add("Date", helper.getDate());
            dict.Add("FunctionRequestCode", parameters["FunctionRequestCode"]);
            dict.Add("Invoice", parameters["Invoice"]);
            dict.Add("ReceiptTextColumns", "30");
            dict.Add("RequestorReference", parameters["RequestorReference"]);
            dict.Add("Time", helper.getTime());
            dict.Add("Vendor", "CottaCapital:FormSim:0.2");

            // Send one of the Three - XOR
                // One
                if (parameters["UniqueID"] == "")
                {
                    dict.Add("CardNumber", parameters["CardNumber"]);
                }
                else
                {
                    dict.Add("UniqueID", parameters["UniqueID"]);
                }
                
                if (parameters["CardDataType"] == "UTGControlledPINPad")
                {
                    dict.Add("TerminalID", parameters["TerminalID"]);
                }
                // Two
                //dict.Add("UniqueID", parameters["UniqueID"]);

                // Three
                //dict.Add("P2PEBlock", "Random Val");
                //dict.Add("P2PEDeviceType", "Special");
                //dict.Add("P2PEBlockLength", "1");
            // End XOR
            
            if (parameters["UseRollbacks"] == "Y")
            {
                dict.Add("TranID", parameters["TranID"]);                                     //Only used for rollbacks of incremental auths
            }
            
            return dict;
        }

        // 07 - Get Invoice Information
        private Dictionary<string, string> buildGetInvoiceInformation(Dictionary<string, string> dict, Dictionary<string, string> parameters)
        {
            string[] options = { "ALLOWPARTIALAUTH" };
            parameters = helper.stripAPIParameters(parameters, options);
            dict.Add("APIOptions", parameters["APIOptions"]);
            dict.Add("Date", helper.getDate());
            dict.Add("FunctionRequestCode", parameters["FunctionRequestCode"]);
            dict.Add("Invoice", parameters["Invoice"]);
            dict.Add("ReceiptTextColumns", "30");
            dict.Add("RequestorReference", parameters["RequestorReference"]);
            dict.Add("Time", helper.getTime());
            dict.Add("Vendor", "CottaCapital:FormSim:0.2");

            // Send one of the Three - XOR
                // One
                dict.Add("CardNumber", parameters["CardNumber"]);

                // Two
                //dict.Add("UniqueID", parameters["UniqueID"]);

                // Three
                //dict.Add("P2PEBlock", "Random Val");
                //dict.Add("P2PEDeviceType", "Special");
                //dict.Add("P2PEBlockLength", "1");
                // End XOR

            if (parameters["UseMetaToken"] == "Y")
            {
                dict = buildMetaToken(dict, parameters);
            }
            if (parameters["UseTokenStore"] == "Y")
            {
                dict.Add("TokenSerialNumber", parameters["TokenSerial"]);
            }

            return dict;
        }

        // 0B - Get DBA Information
        private Dictionary<string, string> buildGetDBAInformation(Dictionary<string, string> dict, Dictionary<string, string> parameters)
        {
            parameters = helper.stripAPIParameters(parameters, "ALLOWPARTIALAUTH");
            dict.Add("APIOptions", parameters["APIOptions"]);
            dict.Add("Date", helper.getDate());
            dict.Add("FunctionRequestCode", parameters["FunctionRequestCode"]);
            dict.Add("RequestorReference", parameters["RequestorReference"]);
            dict.Add("Time", helper.getTime());
            dict.Add("Vendor", "CottaCapital:FormSim:0.2");
            return dict;
        }

        // 20 - Upload Signature
        private Dictionary<string, string> buildUploadSignature(Dictionary<string, string> dict, Dictionary<string, string> parameters)
        {
            dict.Add("APIOptions", parameters["APIOptions"]);
            dict.Add("Date", helper.getDate());
            dict.Add("FunctionRequestCode", parameters["FunctionRequestCode"]);
            dict.Add("Invoice", parameters["Invoice"]);
            dict.Add("RequestorReference", parameters["RequestorReference"]);
            dict.Add("Time", helper.getTime());
            dict.Add("Vendor", "CottaCapital:FormSim:0.2");

            dict.Add("PhotoType", "P");
            dict.Add("PhotoData", "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");
            //dict.Add("PhotoData", "1");

            //dict.Add("SignatureDeviceType", "4");
            //dict.Add("SignatureBlockNumber", "1");
            //dict.Add("SignatureTotalBlocks", "1");
            //dict.Add("SignatureBlock", "");
            return dict;
        }

        // 22 - Get Voice Center Information
        private Dictionary<string, string> buildGetVoiceCenterInformation(Dictionary<string, string> dict, Dictionary<string, string> parameters)
        {
            string[] toBeStripped = { "ALLOWPARTIALAUTH", "BYPASSAMOUNTOK", "BYPASSSIGCAP", "ENHANCEDRECEIPTS", "NOSIGNATURE", "PRINTTIPLINE" };
            parameters = helper.stripAPIParameters(parameters, toBeStripped);
            dict.Add("APIOptions", parameters["APIOptions"]);
            dict.Add("Date", helper.getDate());
            dict.Add("FunctionRequestCode", parameters["FunctionRequestCode"]);
            dict.Add("RequestorReference", parameters["RequestorReference"]);
            dict.Add("Time", helper.getTime());
            dict.Add("UniqueID", parameters["UniqueID"]);
            dict.Add("Vendor", "CottaCapital:FormSim:0.2");
            if (parameters["UseTokenStore"] == "Y")
            {
                dict.Add("TokenSerialNumber", parameters["TokenSerial"]);
            }
            return dict;
        }

        // 23 - Indetify Card Type
        private Dictionary<string, string> buildIdentifyCardType(Dictionary<string, string> dict, Dictionary<string, string> parameters)
        {
            dict.Add("APIOptions", parameters["APIOptions"]);
            dict.Add("Date", helper.getDate());
            dict.Add("FunctionRequestCode", parameters["FunctionRequestCode"]);
            dict.Add("RequestorReference", parameters["RequestorReference"]);
            dict.Add("Time", helper.getTime());
            dict.Add("Vendor", "CottaCapital:FormSim:0.2");

            // Send One of Four - XOR
            if (parameters["CardDataType"] == "UnencryptedCardData")
            {
                // One
                dict.Add("CardNumber", parameters["CardNumber"]);
                dict.Add("ExpirationDate", parameters["ExpirationDate"]);
                dict.Add("CVV2Indicator", "1");
                dict.Add("CVV2Code", parameters["CVV2"]);
            }
            else if (parameters["CardDataType"] == "UnencryptedTrackData")
            {
                // Two
                dict.Add("TrackInformation", parameters["TrackData"]);
            }
            else if (parameters["CardDataType"] == "UTGControlledPINPad")
            {
                // Three
                dict.Add("TerminalID", parameters["TerminalID"]);
            }
            // FOUR - Card-on-file/TrueToken
            else if (parameters["CardDataType"] == "TrueToken")
            {
                dict.Add("UniqueID", parameters["UniqueID"]);
            }
            else if (parameters["CardDataType"] == "P2PE")
            {
                // Five
                dict.Add("P2PEBlock", "");
                dict.Add("P2PEDeviceType", "");
                dict.Add("P2PEBlockLength", "1024");
            }
            // End XOR

            return dict;
        }

        // 2F - Verify Card with Processor
        private Dictionary<string, string> buildVerifyCardWithProcessor(Dictionary<string, string> dict, Dictionary<string, string> parameters)
        {
            dict.Add("APIOptions", parameters["APIOptions"]);
            dict.Add("Date", helper.getDate());
            dict.Add("FunctionRequestCode", parameters["FunctionRequestCode"]);
            dict.Add("RequestorReference", parameters["RequestorReference"]);
            dict.Add("Time", helper.getTime());
            dict.Add("Vendor", "CottaCapital:FormSim:0.2");

            // Send One of Five - XOR
                // One
                dict.Add("CardNumber", parameters["CardNumber"]);
                dict.Add("ExpirationDate", parameters["ExpirationDate"]);
                dict.Add("CVV2Indicator", "1");
                dict.Add("CVV2Code", parameters["CVV2"]);

                // Two
                //dict.Add("TrackInformation", parameters["TrackData"]);

                // Three
                //dict.Add("TerminalID", parameters["TerminalID"]);

                // Four
                //dict.Add("UniqueID", parameters["UniqueID"]);

                // Five
                //dict.Add("P2PEBlock", "");
                //dict.Add("P2PEDeviceType", "");
                //dict.Add("P2PEBlockLength", "1024");

            // End XOR

            dict.Add("StreetAddress", parameters["StreetAddress"]);
            dict.Add("ZipCode", parameters["ZipCode"]);
            return dict;
        }

        // 47 - Prompt for Signature
        private Dictionary<string, string> buildPromptForSignature(Dictionary<string, string> dict, Dictionary<string, string> parameters)
        {
            dict.Add("APIOptions", parameters["APIOptions"]);
            dict.Add("Date", helper.getDate());
            dict.Add("FunctionRequestCode", parameters["FunctionRequestCode"]);
            dict.Add("RequestorReference", parameters["RequestorReference"]);
            dict.Add("Time", helper.getTime());
            dict.Add("Vendor", "CottaCapital:FormSim:0.2");
            dict.Add("TerminalID", parameters["TerminalID"]);
            return dict;
        }

        // 64 - Get Four Words
        private Dictionary<string, string> buildGetFourWords(Dictionary<string, string> dict, Dictionary<string, string> parameters)
        {
            dict.Add("APIOptions", parameters["APIOptions"]);
            dict.Add("Date", helper.getDate());
            dict.Add("FunctionRequestCode", parameters["FunctionRequestCode"]);
            dict.Add("RequestorReference", parameters["RequestorReference"]);
            dict.Add("Time", helper.getTime());
            dict.Add("Vendor", "CottaCapital:FormSim:0.2");
            dict.Add("UniqueID", parameters["UniqueID"]);
            try
            {
                dict.Add("TokenSerialNumber", parameters["TokenSerial"]);
            }
            catch (Exception e) { }
            return dict;
        }

        // CA - Status Request
        private Dictionary<string, string> buildStatusRequest(Dictionary<string, string> dict, Dictionary<string, string> parameters)
        {
            dict.Add("APIOptions", parameters["APIOptions"]);
            dict.Add("Date", helper.getDate());
            dict.Add("FunctionRequestCode", parameters["FunctionRequestCode"]);
            dict.Add("RequestorReference", parameters["RequestorReference"]);
            dict.Add("Time", helper.getTime());
            dict.Add("Vendor", "CottaCapital:FormSim:0.2");
            return dict;
        }

        // F1 - Print Receipt to PIN Pad
        private Dictionary<string, string> buildPrintReceiptToPINPad(Dictionary<string, string> dict, Dictionary<string, string> parameters)
        {
            dict.Add("APIOptions", parameters["APIOptions"]);
            dict.Add("Date", helper.getDate());
            dict.Add("FunctionRequestCode", parameters["FunctionRequestCode"]);
            dict.Add("RequestorReference", parameters["RequestorReference"]);
            dict.Add("Time", helper.getTime());
            dict.Add("Vendor", "CottaCapital:FormSim:0.2");
            dict.Add("TerminalID", parameters["TerminalID"]);
            dict.Add("ReceiptText", "SIMPLE TEST RECEIPT TEXT");
            return dict;
        }

        // F2 - Get Device Information
        private Dictionary<string, string> buildGetDeviceInformation(Dictionary<string, string> dict, Dictionary<string, string> parameters)
        {
            dict.Add("APIOptions", parameters["APIOptions"]);
            dict.Add("Date", helper.getDate());
            dict.Add("FunctionRequestCode", parameters["FunctionRequestCode"]);
            dict.Add("RequestorReference", parameters["RequestorReference"]);
            dict.Add("Time", helper.getTime());
            dict.Add("Vendor", "CottaCapital:FormSim:0.2");
            dict.Add("TerminalID", parameters["TerminalID"]);
            return dict;
        }

        // 82 - Prompt For Confirmation
        private Dictionary<string, string> buildPromptForConfirmation(Dictionary<string, string> dict, Dictionary<string, string> parameters)
        {
            dict.Add("APIOptions", parameters["APIOptions"]);
            dict.Add("Date", helper.getDate());
            dict.Add("FunctionRequestCode", parameters["FunctionRequestCode"]);
            dict.Add("RequestorReference", parameters["RequestorReference"]);
            dict.Add("Time", helper.getTime());
            dict.Add("Vendor", "CottaCapital:FormSim:0.2");
            dict.Add("TerminalID", parameters["TerminalID"]);
            dict.Add("PromptConfirmQuestion", "Are you sure?");
            dict.Add("PromptConfirmValue", "Confirm/Deny");
            return dict;
        }

        // 86 - Display Custom Form
        private Dictionary<string, string> buildDisplayCustomForm(Dictionary<string, string> dict, Dictionary<string, string> parameters)
        {
            dict.Add("APIOptions", parameters["APIOptions"]);
            dict.Add("Date", helper.getDate());
            dict.Add("FunctionRequestCode", parameters["FunctionRequestCode"]);
            dict.Add("RequestorReference", parameters["RequestorReference"]);
            dict.Add("Time", helper.getTime());
            dict.Add("Vendor", "CottaCapital:FormSim:0.2");
            dict.Add("TerminalID", parameters["TerminalID"]);
            dict.Add("FormName", "please");

            dict.Add("KeyValue1", "KeyValue1 as a test value");
            return dict;
        }

        // CF - Prompt for Terms and Conditions
        private Dictionary<string, string> buildPromptForTermsAndConditions(Dictionary<string, string> dict, Dictionary<string, string> parameters)
        {
            dict.Add("APIOptions", parameters["APIOptions"]);
            dict.Add("Date", helper.getDate());
            dict.Add("FunctionRequestCode", parameters["FunctionRequestCode"]);
            dict.Add("RequestorReference", parameters["RequestorReference"]);
            dict.Add("Time", helper.getTime());
            dict.Add("Vendor", "CottaCapital:FormSim:0.2");
            dict.Add("TerminalID", parameters["TerminalID"]);
            dict.Add("TermsandConditions", "Terms and Conditions Apply");
            return dict;
        }

        // DA - On Demand Card Read
        private Dictionary<string, string> buildOnDemandCardRead(Dictionary<string, string> dict, Dictionary<string, string> parameters)
        {
            dict.Add("APIOptions", parameters["APIOptions"]);
            dict.Add("Date", helper.getDate());
            dict.Add("FunctionRequestCode", parameters["FunctionRequestCode"]);
            dict.Add("RequestorReference", parameters["RequestorReference"]);
            dict.Add("Time", helper.getTime());
            dict.Add("Vendor", "CottaCapital:FormSim:0.2");
            dict.Add("TerminalID", parameters["TerminalID"]);
            return dict;
        }

        // DB - Prompt for Input
        private Dictionary<string, string> buildPromptForInput(Dictionary<string, string> dict, Dictionary<string, string> parameters)
        {
            dict.Add("APIOptions", parameters["APIOptions"]);
            dict.Add("Date", helper.getDate());
            dict.Add("FunctionRequestCode", parameters["FunctionRequestCode"]);
            dict.Add("RequestorReference", parameters["RequestorReference"]);
            dict.Add("Time", helper.getTime());
            dict.Add("Vendor", "CottaCapital:FormSim:0.2");
            dict.Add("TerminalID", parameters["TerminalID"]);
            dict.Add("DeviceInputIndex", "010");
            return dict;
        }

        // 17 - Get Acceptable Indentification Types for Checks
        private Dictionary<string, string> buildGetAcceptableIDTypesForChecks(Dictionary<string, string> dict, Dictionary<string, string> parameters)
        {
            dict.Add("APIOptions", parameters["APIOptions"]);
            dict.Add("Date", helper.getDate());
            dict.Add("FunctionRequestCode", parameters["FunctionRequestCode"]);
            dict.Add("RequestorReference", parameters["RequestorReference"]);
            dict.Add("Time", helper.getTime());
            dict.Add("Vendor", "CottaCapital:FormSim:0.2");
            return dict;
        }

        // 1F - Check Approval
        private Dictionary<string, string> buildCheckApproval(Dictionary<string, string> dict, Dictionary<string, string> parameters)
        {
            dict.Add("APIOptions", parameters["APIOptions"]);
            dict.Add("Date", helper.getDate());
            dict.Add("FunctionRequestCode", parameters["FunctionRequestCode"]);
            dict.Add("RequestorReference", parameters["RequestorReference"]);
            dict.Add("Time", helper.getTime());
            dict.Add("Vendor", "CottaCapital:FormSim:0.2");
            dict.Add("CheckAmount", "75.99");
            dict.Add("IDTypeCode", "38");
            dict.Add("ReaderIndicator", "1");
            dict.Add("CheckingAccountNumber", "123456789");
            dict.Add("IDNumber", "4096");
            dict.Add("ManualCheckNumber", "5000");
            dict.Add("TransitRoutingNumber", "30354009265");
            dict.Add("RawMagneticData", "685181381313");
            dict.Add("BirthDate", "121275");
            dict.Add("Clerk", parameters["Clerk"]);
            return dict;
        }

        // 5F - Get Totals Report
        private Dictionary<string, string> buildGetTotalsReport(Dictionary<string, string> dict, Dictionary<string, string> parameters)
        {
            dict.Add("APIOptions", parameters["APIOptions"]);
            dict.Add("Date", helper.getDate());
            dict.Add("FunctionRequestCode", parameters["FunctionRequestCode"]);
            dict.Add("RequestorReference", parameters["RequestorReference"]);
            dict.Add("Time", helper.getTime());
            dict.Add("Vendor", "CottaCapital:FormSim:0.2");
            dict.Add("ReportStartDate", (Int32.Parse(helper.getDate()) - 100).ToString());
            dict.Add("ReportEndDate", helper.getDate());

            //Optional
            //dict.Add("ReportCardType", "VS");
            //dict.Add("ReportClerk", parameters["Clerk"]);
            dict.Add("ReportStartTime", "000101");
            dict.Add("ReportEndTime", "235959");
            //dict.Add("ReportTerminalID", parameters["TerminalID"]);

            return dict;
        }

        // 92 - Display Line Item
        // If the APIOPTION APPENDLINEITEM is sent the UTG will append the line item to the existing line item(s) being displayed.
        private Dictionary<string, string> buildDisplayLineItem(Dictionary<string, string> dict, Dictionary<string, string> parameters)
        {
            dict.Add("APIOptions", parameters["APIOptions"]);
            dict.Add("Date", helper.getDate());
            dict.Add("FunctionRequestCode", parameters["FunctionRequestCode"]);
            dict.Add("RequestorReference", parameters["RequestorReference"]);
            dict.Add("Time", helper.getTime());
            dict.Add("Vendor", "CottaCapital:FormSim:0.2");
            dict.Add("TerminalID", parameters["TerminalID"]);
            dict.Add("LineItem", "Random Line Item");
            return dict;
        }

        // 94 - Clear Line Items
        private Dictionary<string, string> buildClearLineItems(Dictionary<string, string> dict, Dictionary<string, string> parameters)
        {
            dict.Add("APIOptions", parameters["APIOptions"]);
            dict.Add("Date", helper.getDate());
            dict.Add("FunctionRequestCode", parameters["FunctionRequestCode"]);
            dict.Add("RequestorReference", parameters["RequestorReference"]);
            dict.Add("Time", helper.getTime());
            dict.Add("Vendor", "CottaCapital:FormSim:0.2");
            dict.Add("TerminalID", parameters["TerminalID"]);
            return dict;
        }

        // 95 - Display Line Items
        private Dictionary<string, string> buildDisplayLineItems(Dictionary<string, string> dict, Dictionary<string, string> parameters)
        {
            dict.Add("APIOptions", parameters["APIOptions"]);
            dict.Add("Date", helper.getDate());
            dict.Add("FunctionRequestCode", parameters["FunctionRequestCode"]);
            dict.Add("RequestorReference", parameters["RequestorReference"]);
            dict.Add("Time", helper.getTime());
            dict.Add("Vendor", "CottaCapital:FormSim:0.2");
            dict.Add("TerminalID", parameters["TerminalID"]);
            dict.Add("LineItemCount", "3");
            dict.Add("LineItem1", "Piano");
            dict.Add("LineItem2", "Headphones");
            dict.Add("LineItem3", "Turnip");
            return dict;
        }

        // 96 - Swipe Ahead
        private Dictionary<string, string> buildSwipeAhead(Dictionary<string, string> dict, Dictionary<string, string> parameters)
        {
            dict.Add("APIOptions", parameters["APIOptions"]);
            dict.Add("Date", helper.getDate());
            dict.Add("FunctionRequestCode", parameters["FunctionRequestCode"]);
            dict.Add("RequestorReference", parameters["RequestorReference"]);
            dict.Add("Time", helper.getTime());
            dict.Add("Vendor", "CottaCapital:FormSim:0.2");
            dict.Add("TerminalID", parameters["TerminalID"]);
            return dict;
        }

        // 97 - Reset PIN Pad
        private Dictionary<string, string> buildResetPINPad(Dictionary<string, string> dict, Dictionary<string, string> parameters)
        {
            dict.Add("APIOptions", parameters["APIOptions"]);
            dict.Add("Date", helper.getDate());
            dict.Add("FunctionRequestCode", parameters["FunctionRequestCode"]);
            dict.Add("RequestorReference", parameters["RequestorReference"]);
            dict.Add("Time", helper.getTime());
            dict.Add("Vendor", "CottaCapital:FormSim:0.2");
            dict.Add("TerminalID", parameters["TerminalID"]);
            return dict;
        }

        // 24 - Activate/Reload Gift Card
        private Dictionary<string, string> buildActivateReloadGiftCard(Dictionary<string, string> dict, Dictionary<string, string> parameters)
        {
            dict.Add("APIOptions", parameters["APIOptions"]);
            dict.Add("Date", helper.getDate());
            dict.Add("FunctionRequestCode", parameters["FunctionRequestCode"]);
            dict.Add("RequestorReference", parameters["RequestorReference"]);
            dict.Add("Time", helper.getTime());
            dict.Add("Vendor", "CottaCapital:FormSim:0.2");
            dict.Add("CardPresent", parameters["CardPresent"]);
            dict.Add("IYCBalance", parameters["PrimaryAmount"]);
            dict.Add("IYCCardType", "G");

            // Send One of five XOR
            // ONE
            dict.Add("CardNumber", parameters["CardNumber"]);
            dict.Add("ExpirationDate", "1220");
            // TWO
            //dict.Add("TrackInformation", parameters["Trackdata"]);
            // THREE
            //dict.Add("TerminalID", parameters["TerminalID"]);
            // FOUR
            //dict.Add("UniqueID", parameters["UniqueID"]);
            // FIVE
            //dict.Add("P2PEBlock", "");
            //dict.Add("P2PEDeviceType", "Unknown");
            //dict.Add("P2PEBlockLength", "1024");
            return dict;
        }

        // 25 - Deactivate Gift Card
        private Dictionary<string, string> buildDeactivateGiftCard(Dictionary<string, string> dict, Dictionary<string, string> parameters)
        {
            dict.Add("APIOptions", parameters["APIOptions"]);
            dict.Add("Date", helper.getDate());
            dict.Add("FunctionRequestCode", parameters["FunctionRequestCode"]);
            dict.Add("Invoice", parameters["Invoice"]);
            dict.Add("ReceiptTextColumns", "30");
            dict.Add("RequestorReference", parameters["RequestorReference"]);
            dict.Add("Time", helper.getTime());
            dict.Add("Vendor", "CottaCapital:FormSim:0.2");
            dict.Add("CardPresent", parameters["CardPresent"]);
            dict.Add("IYCCardType", "G");
            dict.Add("IYCReasonText", "Empty");

            // Send One of five XOR
            // ONE
            dict.Add("CardNumber", parameters["CardNumber"]);
            dict.Add("ExpirationDate", "1220");
            // TWO
            //dict.Add("TrackInformation", parameters["Trackdata"]);
            // THREE
            //dict.Add("TerminalID", parameters["TerminalID"]);
            // FOUR
            //dict.Add("UniqueID", parameters["UniqueID"]);
            // FIVE
            //dict.Add("P2PEBlock", "");
            //dict.Add("P2PEDeviceType", "Unknown");
            //dict.Add("P2PEBlockLength", "1024");
            return dict;
        }

        // 26 - Reactivate Gift Card
        private Dictionary<string, string> buildReactivateGiftCard(Dictionary<string, string> dict, Dictionary<string, string> parameters)
        {
            dict.Add("APIOptions", parameters["APIOptions"]);
            dict.Add("Date", helper.getDate());
            dict.Add("FunctionRequestCode", parameters["FunctionRequestCode"]);
            dict.Add("Invoice", parameters["Invoice"]);
            dict.Add("ReceiptTextColumns", "30");
            dict.Add("RequestorReference", parameters["RequestorReference"]);
            dict.Add("Time", helper.getTime());
            dict.Add("Vendor", "CottaCapital:FormSim:0.2");
            dict.Add("IYCCardType", "G");

            // Send One of five XOR
            // ONE
            dict.Add("CardNumber", parameters["CardNumber"]);
            dict.Add("ExpirationDate", "1220");
            // TWO
            //dict.Add("TrackInformation", parameters["Trackdata"]);
            // THREE
            //dict.Add("TerminalID", parameters["TerminalID"]);
            // FOUR
            //dict.Add("UniqueID", parameters["UniqueID"]);
            // FIVE
            //dict.Add("P2PEBlock", "");
            //dict.Add("P2PEDeviceType", "Unknown");
            //dict.Add("P2PEBlockLength", "1024");
            return dict;
        }

        // 61 - Get Balance Inquiry
        private Dictionary<string, string> buildGetBalanceInquiry(Dictionary<string, string> dict, Dictionary<string, string> parameters)
        {
            dict.Add("APIOptions", parameters["APIOptions"]);
            dict.Add("Date", helper.getDate());
            dict.Add("FunctionRequestCode", parameters["FunctionRequestCode"]);
            dict.Add("Invoice", parameters["Invoice"]);
            dict.Add("ReceiptTextColumns", "30");
            dict.Add("RequestorReference", parameters["RequestorReference"]);
            dict.Add("Time", helper.getTime());
            dict.Add("Vendor", "CottaCapital:FormSim:0.2");

            // Send One of five XOR
            // ONE
            dict.Add("CardNumber", parameters["CardNumber"]);
            dict.Add("ExpirationDate", "1220");
            // TWO
            //dict.Add("TrackInformation", parameters["Trackdata"]);
            // THREE
            //dict.Add("TerminalID", parameters["TerminalID"]);
            // FOUR
            //dict.Add("UniqueID", parameters["UniqueID"]);
            // FIVE
            //dict.Add("P2PEBlock", "");
            //dict.Add("P2PEDeviceType", "Unknown");
            //dict.Add("P2PEBlockLength", "1024");
            return dict;
        }

        // E0 - TokenStore Add
        private Dictionary<string, string> buildTokenStoreAdd(Dictionary<string, string> dict, Dictionary<string, string> parameters)
        {
            dict.Add("APIOptions", parameters["APIOptions"]);
            dict.Add("Date", helper.getDate());
            dict.Add("FunctionRequestCode", parameters["FunctionRequestCode"]);
            dict.Add("RequestorReference", parameters["RequestorReference"]);
            dict.Add("Time", helper.getTime());
            dict.Add("Vendor", "CottaCapital:FormSim:0.2");

            // Send One of five XOR
            // ONE
            if (parameters["CardDataType"] == "UnencryptedCardData")
            {
                dict.Add("CardNumber", parameters["CardNumber"]);
                dict.Add("ExpirationDate", "1220");
                dict.Add("CVV2Indicator", (parameters["CVV2"] == "") ? "0" : "1");
                dict.Add("CVV2Code", parameters["CVV2"]);
            }
            // TWO
            else if (parameters["CardDataType"] == "UnencryptedTrackData")
            {
                dict.Add("TrackInformation", parameters["Trackdata"]);
            }
            // THREE   
            else if (parameters["CardDataType"] == "UTGControlledPINPad")
            {
                dict.Add("CVV2Prompt", "N");
                dict.Add("PostalCodePrompt", "N");
                dict.Add("StreetNumberPrompt", "N");
                dict.Add("TerminalID", parameters["TerminalID"]);
            }
            // FOUR
            else if (parameters["CardDataType"] == "TrueToken")
            {
                dict.Add("UniqueID", parameters["UniqueID"]);
            }
            // FIVE
            else if (parameters["CardDataType"] == "P2PE")
            {
                dict.Add("P2PEBlock", parameters["P2PEBlock"]);
                dict.Add("P2PEDeviceType", (parameters["P2PEBlock"].Contains('*')) ? "01" : "02");
                //dict.Add("P2PEBlockLength", "1024");
            }

            dict.Add("CustomerName", parameters["CustomerName"]);
            dict.Add("StreetAddress", parameters["StreetAddress"]);
            dict.Add("ZipCode", parameters["ZipCode"]);

            if (parameters["UseMetaToken"] == "Y")
            {
                dict = buildMetaToken(dict, parameters);
            }
            return dict;
        }

        // E2 - TokenStore Duplicate
        private Dictionary<string, string> buildTokenStoreDuplicate(Dictionary<string, string> dict, Dictionary<string, string> parameters)
        {
            dict.Add("APIOptions", parameters["APIOptions"]);
            dict.Add("Date", helper.getDate());
            dict.Add("FunctionRequestCode", parameters["FunctionRequestCode"]);
            dict.Add("RequestorReference", parameters["RequestorReference"]);
            dict.Add("Time", helper.getTime());
            dict.Add("Vendor", "CottaCapital:FormSim:0.2");
            dict.Add("UniqueID", parameters["UniqueID"]);

            dict.Add("CVV2Indicator", (parameters["CVV2"] == "") ? "0" : "1");
            dict.Add("CVV2Code", parameters["CVV2"]);

            dict.Add("CustomerName", parameters["CustomerName"]);
            dict.Add("StreetAddress", parameters["StreetAddress"]);
            dict.Add("ZipCode", parameters["ZipCode"]);

            dict.Add("CVV2Prompt", "N");
            //dict.Add("PostalCodePrompt", "N");
            dict.Add("StreetNumberPrompt", "N");

            if (parameters["UseMetaToken"] == "Y")
            {
                dict = buildMetaToken(dict, parameters);
            }
            if (parameters["UseTokenStore"] == "Y")
            {
                dict.Add("TokenSerialNumber", parameters["TokenSerial"]);
            }
            return dict;
        }

        // D7 - Block Card
        private Dictionary<string, string> buildBlockCard(Dictionary<string, string> dict, Dictionary<string, string> parameters)
        {
            dict.Add("APIOptions", parameters["APIOptions"]);
            dict.Add("Date", helper.getDate());
            dict.Add("FunctionRequestCode", parameters["FunctionRequestCode"]);
            dict.Add("RequestorReference", parameters["RequestorReference"]);
            dict.Add("Time", helper.getTime());
            dict.Add("Vendor", "CottaCapital:FormSim:0.2");

            // Send One of five XOR
            // ONE
            dict.Add("CardNumber", parameters["CardNumber"]);
            dict.Add("ExpirationDate", "1220");
            dict.Add("CVV2Indicator", (parameters["CVV2"] == "") ? "0" : "1");
            dict.Add("CVV2Code", parameters["CVV2"]);
            // TWO
            //dict.Add("TrackInformation", parameters["Trackdata"]);
            // THREE
            //dict.Add("TerminalID", parameters["TerminalID"]);
            // FOUR
            //dict.Add("UniqueID", parameters["UniqueID"]);
            // FIVE
            //dict.Add("P2PEBlock", "");
            //dict.Add("P2PEDeviceType", "Unknown");
            //dict.Add("P2PEBlockLength", "1024");

            return dict;
        }

        // D8 - Unblock Card
        private Dictionary<string, string> buildUnblockCard(Dictionary<string, string> dict, Dictionary<string, string> parameters)
        {
            dict.Add("APIOptions", parameters["APIOptions"]);
            dict.Add("Date", helper.getDate());
            dict.Add("FunctionRequestCode", parameters["FunctionRequestCode"]);
            dict.Add("RequestorReference", parameters["RequestorReference"]);
            dict.Add("Time", helper.getTime());
            dict.Add("Vendor", "CottaCapital:FormSim:0.2");

            // Send One of five XOR
            // ONE
            dict.Add("CardNumber", parameters["CardNumber"]);
            dict.Add("ExpirationDate", "1220");
            dict.Add("CVV2Indicator", (parameters["CVV2"] == "") ? "0" : "1");
            dict.Add("CVV2Code", parameters["CVV2"]);
            // TWO
            //dict.Add("TrackInformation", parameters["Trackdata"]);
            // THREE
            //dict.Add("TerminalID", parameters["TerminalID"]);
            // FOUR
            //dict.Add("UniqueID", parameters["UniqueID"]);
            // FIVE
            //dict.Add("P2PEBlock", "");
            //dict.Add("P2PEDeviceType", "Unknown");
            //dict.Add("P2PEBlockLength", "1024");

            return dict;
        }

        // D9 - Card Block Status
        private Dictionary<string, string> buildCardBlockStatus(Dictionary<string, string> dict, Dictionary<string, string> parameters)
        {
            dict.Add("APIOptions", parameters["APIOptions"]);
            dict.Add("Date", helper.getDate());
            dict.Add("FunctionRequestCode", parameters["FunctionRequestCode"]);
            dict.Add("RequestorReference", parameters["RequestorReference"]);
            dict.Add("Time", helper.getTime());
            dict.Add("Vendor", "CottaCapital:FormSim:0.2");

            // Send One of five XOR
            // ONE
            dict.Add("CardNumber", parameters["CardNumber"]);
            dict.Add("ExpirationDate", "1220");
            dict.Add("CVV2Code", parameters["CVV2"]);
            // TWO
            //dict.Add("TrackInformation", parameters["Trackdata"]);
            // THREE
            //dict.Add("TerminalID", parameters["TerminalID"]);
            // FOUR
            //dict.Add("UniqueID", parameters["UniqueID"]);
            // FIVE
            //dict.Add("P2PEBlock", "");
            //dict.Add("P2PEDeviceType", "Unknown");
            //dict.Add("P2PEBlockLength", "1024");

            return dict;
        }

        // CD - Get MetaToken
        private Dictionary<string, string> buildGetMetaToken(Dictionary<string, string> dict, Dictionary<string, string> parameters)
        {
            dict.Add("APIOptions", parameters["APIOptions"]);
            dict.Add("Date", helper.getDate());
            dict.Add("FunctionRequestCode", parameters["FunctionRequestCode"]);
            dict.Add("RequestorReference", parameters["RequestorReference"]);
            dict.Add("Time", helper.getTime());
            dict.Add("Vendor", "CottaCapital:FormSim:0.2");

            // Send One of five XOR
            // ONE
            dict.Add("CardNumber", parameters["CardNumber"]);
            dict.Add("ExpirationDate", "1220");
            dict.Add("CVV2Indicator", (parameters["CVV2"] == "") ? "0" : "1");
            dict.Add("CVV2Code", parameters["CVV2"]);
            // TWO
            //dict.Add("TrackInformation", parameters["Trackdata"]);
            // THREE
            //dict.Add("TerminalID", parameters["TerminalID"]);
            // FOUR
            //dict.Add("UniqueID", parameters["UniqueID"]);
            // FIVE
            //dict.Add("P2PEBlock", "");
            //dict.Add("P2PEDeviceType", "Unknown");
            //dict.Add("P2PEBlockLength", "1024");
            if (parameters["UseMetaToken"] == "Y")
            {
                dict = buildMetaToken(dict, parameters);
            }

            return dict;
        }


        /******************************************************************************************************
         * Block Sub-Component Builders                                                                       *
         ******************************************************************************************************/

        private Dictionary<string, string> buildLevel2CardData(Dictionary<string, string> dict, Dictionary<string, string> parameters)
        {
            try
            {
                dict.Add("CustomerReference", parameters["CustomerReference"]);
                if (parameters["TaxIndicator"] == "Y")
                {
                    dict.Add("TaxIndicator", "Y");
                    dict.Add("TaxAmount", parameters["TaxAmount"]);
                }
                else
                {
                    dict.Add("TaxIndicator", "N");
                    dict.Add("TaxAmount", "0");
                }
                dict.Add("DestinationZipCode", parameters["DestinationZipCode"]);
            }
            catch (Exception e)
            {
                log.Write("An error was thrown building Level 2 Card Data.");
            }

            return dict;
        }

        private Dictionary<string, string> buildPurchasingCardData(Dictionary<string, string> dict, Dictionary<string, string> parameters)
        {
            try
            {
                dict.Add("ProductDescriptor1", "Electronics");
                dict.Add("ProductDescriptor2", "A([*?6$b&C%d#/1=2\\4|:;//@www.shift4+,'])");
            }
            catch (Exception e)
            {
                log.Write("An error was thrown building Purchasing Card Data");
            }
            return dict;
        }

        private Dictionary<string, string> buildAVSData(Dictionary<string, string> dict, Dictionary<string, string> parameters)
        {
            try
            {
                dict.Add("CustomerName", parameters["CustomerName"]);
                dict.Add("StreetAddress", parameters["StreetAddress"]);
                dict.Add("ZipCode", parameters["ZipCode"]);
            }
            catch (Exception e)
            {
                log.Write("An error was thrown building AVS Data");
            }
            return dict;
        }

        private Dictionary<string, string> buildTipData(Dictionary<string, string> dict, Dictionary<string, string> parameters)
        {
            try
            {
                dict.Add("SecondaryAmount", parameters["SecondaryAmount"]);
            }
            catch (Exception e)
            {
                log.Write("An error was thrown building Tip Data (Secondary Amount)");
            }
            return dict;
        }

        private Dictionary<string, string> buildCashBackData(Dictionary<string, string> dict, Dictionary<string, string> parameters)
        {
            try
            {
                dict.Add("CashBack", parameters["CashBack"]);
            }
            catch (Exception e)
            {
                log.Write("An error was thrown building Cash Back Data (CashBack)");
            }
            return dict;
        }

        private Dictionary<string, string> buildHotelAuth(Dictionary<string, string> dict, Dictionary<string, string> parameters)
        {
            try
            {
                dict.Add("HotelEstimatedDays", "1");
            }catch (Exception e)
            {
                log.Write("An error was thrown building Hotel Auth Data");
            }
            return dict;
        }

        private Dictionary<string, string> buildHotelSale(Dictionary<string, string> dict, Dictionary<string, string> parameters)
        {
            try
            {
                dict.Add("ArrivalDate", "070318");
                dict.Add("DepartureDate", "070518");
                dict.Add("HotelAdditionalCharges", "4");
                dict.Add("PrimaryChargeType", "1");
                dict.Add("SpecialCode", "1");
            }
            catch (Exception e)
            {
                log.Write("An error was thrown building Hotel Sale Data");
            }
            return dict;
        }

        private Dictionary<string, string> buildMetaToken(Dictionary<string, string> dict, Dictionary<string, string> parameters)
        {
            try
            {
                dict.Add("MetaTokenType", "IL");
            }
            catch (Exception e)
            {
                log.Write("An error was thrown building Meta Token");
            }
            return dict;
        }

        private Dictionary<string, string> buildMiscData(Dictionary<string, string> dict, Dictionary<string, string> parameters)
        {
            try
            {
                dict.Add("Notes", "Random Misc Notes");
                dict.Add("CardType", parameters["CardType"]);
                /*
                if (parameters["CardDataType"] == "UTGControlledPINPad")
                {
                    dict.Add("CardType", "CC");                          // Send this in the case you want to force the card to be
                    dict.Add("OverrideBusDate", "070518");               // processed as a credit card only.
                }                
                */
            }
            catch (Exception e)
            {
                log.Write("An error was thrown building Misc. Data");
            }
            return dict;
        }

        private Dictionary<string, string> buildTokenStoreData(Dictionary<string, string> dict, Dictionary<string, string> parameters)
        {
            try
            {
                dict.Add("TokenSerialNumber", parameters["TokenSerial"]);
            }
            catch (Exception e)
            {
                log.Write("An error was thrown building Token Store Data");
            }
            return dict;
        }

        private Dictionary<string, string> buildAVS_CVV2PromptData(Dictionary<string, string> dict, Dictionary<string, string> parameters)
        {
            try
            {
                dict.Add("CVV2Prompt", "Y");
                dict.Add("PostalCodePrompt", "Y");
                dict.Add("StreetNumberPrompt", "N");
            }
            catch (Exception e)
            {
                log.Write("An error was thrown building AVS/CVV2 Prompt Data");
            }
            return dict;
        }

        private Dictionary<string, string> buildSignatureCaptureData(Dictionary<string, string> dict, Dictionary<string, string> parameters)
        {
            try
            {
                dict.Add("SignatureBlockNumber", "1");
                dict.Add("SignatureBlock", "Test Signature");
                dict.Add("SignatureDeviceType", "1");
                dict.Add("SignatureTotalBlocks", "1");
            }
            catch (Exception e)
            {
                log.Write("An error was thrown building Signature Capture Data");
            }
            return dict;
        }

        private Dictionary<string, string> buildHSA_FSAData(Dictionary<string, string> dict, Dictionary<string, string> parameters)
        {
            try
            {
                dict.Add("IIASAmount1", "23");
                dict.Add("IIASType4", "1");
            }
            catch (Exception e)
            {
                log.Write("An error was thrown building HSA/FSA Data");
            }
            return dict;
        }

        private Dictionary<string, string> buildNonUTGPinDebitData(Dictionary<string, string> dict, Dictionary<string, string> parameters)
        {
            // Used for people who want to use their own debit pin pad...
            try
            {
                dict.Add("PINBlock", "54");
                dict.Add("PINPadBlockFormat", "71");
                dict.Add("PINPadKey", "5555");
                dict.Add("PINPadType", "1");
            }
            catch (Exception e)
            {
                log.Write("An error was thrown building Non-UTG PIN Debit data");
            }
            return dict;
        }

        private Dictionary<string, string> buildAutoRentalAuthData(Dictionary<string, string> dict, Dictionary<string, string> parameters)
        {
            try
            {
                dict.Add("AutoEstimatedDays", "2");
            }
            catch (Exception e)
            {
                log.Write("An error was thrown building Auto Rental Auth Data");
            }
            return dict;
        }

        private Dictionary<string, string> buildAutoRentalSaleData(Dictionary<string, string> dict, Dictionary<string, string> parameters)
        {
            try
            {
                dict.Add("AutoAdditionalCharges", "0");
                dict.Add("DriverName", "Tina Tamber");
                dict.Add("LateAdjustment", "0");
                dict.Add("NoShowIndicator", "Y");
                dict.Add("RentalAgreement", "Yes");
                dict.Add("RentalCity", "Las Vegas");
                dict.Add("RentalState", "NV");
                dict.Add("RentalTime", "113131");
                dict.Add("RentalZipCode", "89189");
            }
            catch (Exception e)
            {
                log.Write("An error was thrown building Auto Rental Sale Data");
            }
            return dict;
        }

        /******************************************************************************************************
         * Response Parser Methods                                                                            *
         ******************************************************************************************************/

        // Parses out the access token from the returned CE request and sets it as a property
        private bool parseAccessToken(string results)
        {
            bool ret = false;
            XElement root = XElement.Parse(results);
            IEnumerable<XElement> TokenExchange =
                from el in root.Elements("accesstoken")
                select el;
            string token = (string)TokenExchange.First();
            if (token != "")
            {
                string store = (string)TokenExchange.First();

                string tok = System.Web.HttpUtility.UrlDecode(store);
                setAccessToken(tok);
                ret = true;
                Console.WriteLine("The Access Token has been set.");
            }
            else { Console.WriteLine("The AccessToken was not recieved in XML format"); }

            return ret;
        }

        // Parses the XML response into a Dictionary
        private Dictionary<string, string> parseXMLbasedResults(string xml)
        {
            Dictionary<string, string> kvp = new Dictionary<string, string>();
            XDocument doc = XDocument.Parse(xml);
            foreach (XElement element in doc.Descendants().Where(p => p.HasElements == false))
            {
                int keyInt = 0;
                string keyName = element.Name.LocalName;
                while (kvp.ContainsKey(keyName))
                {
                    keyName = element.Name.LocalName + "_" + keyInt++;
                }
                kvp.Add(keyName, System.Web.HttpUtility.UrlDecode(element.Value));
            }
            return kvp;
        }
    }
}

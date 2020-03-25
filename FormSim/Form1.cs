using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FormSim
{
    public partial class Form1 : Form
    {
        private FRC_Handler httpHandler;
        private FRC_Handler tcpHandler;
        private FRC_Handler restHandler;
        private Helper helper;
        private int requestorRefNum;

        // Constructor
        public Form1()
        {
            InitializeComponent();
            httpHandler = new HTTPHandler();
            tcpHandler = new TCPHandler();
            restHandler = new RestHandler();
            helper = Helper.getInstance;
            Random random = new Random();
            int randomNumber = random.Next(0, 10000);
            Invoice.Text = randomNumber.ToString();
            requestorRefNum = random.Next(0, 1000000000);
        }

        // Exchange Token Button
        private async void ExchangeToken_ClickAsync(object sender, EventArgs e)
        {
            // PROBABLY WILL NEED A TLS CHECK HERE TOO LATER

            // Check TCP/HTTP Radio button is checked
            if (!TCP.Checked && !HTTP.Checked && !Rest.Checked)
            {
                MessageBox.Show("Please make sure to check a connection type.");
                return;
            }
            // Check for IP and Port #
            if (IPAddress.Text == "" || Port.Text == "")
            {
                MessageBox.Show("Please make sure you have entered an IP and Port Number.");
                return;
            }
            // Check Auth Token and Client GUID
            if (AuthToken.Text == "" || ClientGUID.Text == "")
            {
                MessageBox.Show("Please make sure to enter an Auth Token, and Client GUID.");
                return;
            }

            // Exchange the token via the appropriate method, and place the Access Token in 
            // memory and the box on screen.
            bool x = false;
            if (TCP.Checked)
            {
                Output.AppendText(Environment.NewLine);
                Output.AppendText("Starting TCP/IP Connection");
                tcpHandler.setClientGUID(ClientGUID.Text);
                tcpHandler.setAuthToken(AuthToken.Text);
                tcpHandler.setIPAddress(IPAddress.Text);
                tcpHandler.setPort(Port.Text);

                x = await tcpHandler.performTokenExchange();
                if (x)
                {
                    AccessToken.Text = tcpHandler.getAccessToken();
                }
            }
            else if (HTTP.Checked)
            {
                Output.AppendText(Environment.NewLine);
                Output.AppendText("Starting HTTP Connection");
                httpHandler.setClientGUID(ClientGUID.Text);
                httpHandler.setAuthToken(AuthToken.Text);
                httpHandler.setIPAddress(IPAddress.Text);
                httpHandler.setPort(Port.Text);

                x = await httpHandler.performTokenExchange();
                if (x)
                {
                    AccessToken.Text = httpHandler.getAccessToken();
                }  
            }
            else if (Rest.Checked)
            {
                Output.AppendText(Environment.NewLine);
                Output.AppendText("Starting REST Connection");
                restHandler.setClientGUID(ClientGUID.Text);
                restHandler.setAuthToken(AuthToken.Text);
                restHandler.setIPAddress(IPAddress.Text);
                restHandler.setPort(Port.Text);

                x = await restHandler.performTokenExchange();
                if (x)
                {
                    AccessToken.Text = restHandler.getAccessToken();
                }
            }

            if (x)
            {
                Output.AppendText(Environment.NewLine);
                Output.AppendText("SUCCESS: Access Token Recieved");
            }
            else
            {
                Output.AppendText(Environment.NewLine);
                Output.AppendText("FAIL: Access Token Not Recieved");
            }
        }

        // Starts a transaction based on the selected values and FRC
        private async void SendTransaction_Click(object sender, EventArgs e)
        {
            // Call the TCP handler
            if (TCP.Checked)
            {
                // Set up basic connection parameters
                if (AccessToken.Text != null && AccessToken.Text != "" && !UseAPISerialMID.Checked)
                {
                    tcpHandler.setAccessToken(AccessToken.Text);
                    tcpHandler.setIPAddress(IPAddress.Text);
                    tcpHandler.setPort(Port.Text);
                }
                else
                {
                    tcpHandler.setAPISerialMIDPass(APISerial.Text, MID.Text, Password.Text);
                    tcpHandler.setIPAddress(IPAddress.Text);
                    tcpHandler.setPort(Port.Text);
                }
                // Pass all fields
                var values = new Dictionary<string, string>
                {
                    { "CardNumber", (UnencryptedCardData.Checked) ? CardNumber.Text.Replace(" ","") : ""},
                    { "ExpirationDate", ExpirationMonth.Text + ExpirationYear.Text },
                    { "CVV2", CVV2.Text },
                    { "CVV2Indicator", (CVV2.Text != "") ? "1" : "0" },
                    { "CardPresent", CardPresent.Text },
                    { "StreetAddress", StreetAddress.Text },
                    { "DestinationZipCode", ZipCode.Text },
                    { "CardType", CardType.Text },
                    { "TrackData", (UnencryptedTrackData.Checked) ? TrackData.Text : ""},
                    { "UniqueID", (TrueToken.Checked) ? UniqueID.Text : ""},
                    { "TokenSerial", TokenSerial.Text },
                    { "APIOptions", stripped(APIOptions.Text) },
                    { "Invoice", Invoice.Text },
                    { "PrimaryAmount", PrimaryDollars.Text + PrimaryCents.Text },
                    { "SaleFlag", SaleFlag.Text },
                    { "ZipCode", ZipCode.Text },
                    { "SecondaryAmount", SecondaryDollars.Text + SecondaryCents.Text },
                    { "FunctionRequestCode", FunctionRequestCode.Text },
                    { "TaxIndicator", TaxIndicator.Checked ? "Y" : "N" },
                    { "TaxAmount", TaxAmountDollars.Text + TaxAmountCents.Text },
                    { "TranID", TranID.Text },
                    { "Clerk", Clerk.Text },
                    { "TerminalID", TerminalID.Text },
                    { "Date", helper.getDate() },
                    { "Time", helper.getTime() },
                    { "VoidInvalidAVS", (VoidInvalidAVS.Checked) ? "Y" : "N"},
                    { "VoidInvalidCVV2", (VoidInvalidCVV2.Checked) ? "Y" : "N" },
                    { "RequestorReference", requestorRefNum.ToString() },
                    { "UseTokenStore", (UseTokenStore.Checked) ? "Y" : "N" },
                    { "UseMetaToken", (UseMetaToken.Checked) ? "Y" : "N" },
                    { "CustomerName", CustomerName.Text },
                    { "P2PEBlock", (P2PE.Checked) ? TrackData.Text : "" },
                    { "UseBasicTranFlow", (UseBasicTranFlow.Checked) ? "Y" : "N" },
                    { "UseRollbacks", (UseRollbacks.Checked) ? "Y" : "N" },
                    { "UseEMV", (UseEMV.Checked) ? "Y" : "N" },
                    { "KSN", KSN.Text },
                    { "CashBack", CashBackAmountDollars.Text + "." + CashBackAmountCents.Text }
                };

                if (UnencryptedCardData.Checked) { values.Add("CardDataType", "UnencryptedCardData"); }
                else if (UnencryptedTrackData.Checked) { values.Add("CardDataType", "UnencryptedTrackData"); }
                else if (UTGControlledPINPad.Checked) { values.Add("CardDataType", "UTGControlledPINPad"); }
                else if (TrueToken.Checked) { values.Add("CardDataType", "TrueToken"); }
                else if (P2PE.Checked) { values.Add("CardDataType", "P2PE"); }
                Dictionary<string, string> x = new Dictionary<string, string>();
                try
                {
                    x = await Task.Run(() => tcpHandler.start(values));
                }
                catch (System.Net.Sockets.SocketException se)
                {
                    Console.WriteLine(se.StackTrace);
                    Output.AppendText(Environment.NewLine);
                    Output.AppendText("###################################");
                    Output.AppendText("#SOCKET EXCEPTION: UTG NOT STARTED#");
                    Output.AppendText("###################################");
                    Output.AppendText(Environment.NewLine);
                }

                var asString = helper.parseDict(x);
                requestorRefNum++;
                // Update Field Values here
                string partialAuth = "";
                double offset = 0.00;
                try
                {
                    UniqueID.Text = x["UniqueID"];
                    if (Double.Parse(values["PrimaryAmount"])/100.0 > Double.Parse(x["PrimaryAmount"]) &&
                        (x["FunctionRequestCode"] == "1B" || x["FunctionRequestCode"] == "1D" || x["FunctionRequestCode"] == "05" || x["FunctionRequestCode"] == "06"))
                    {
                        offset = Double.Parse(values["PrimaryAmount"])/100.0 + Double.Parse(values["TaxAmount"])/100.0 - Double.Parse(x["PrimaryAmount"]);
                        partialAuth = "WARNING: PARTIAL AUTH RECIEVED. REMAINING TENDER NEEDED: " + offset.ToString();
                    }
                    TranID.Text = x["TranID"];
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                    // Do Nothing
                }

                Output.AppendText(Environment.NewLine);
                Output.AppendText(Environment.NewLine);
                Output.AppendText("-----------------------------------------------");
                Output.AppendText(Environment.NewLine);
                Output.AppendText(asString);
                Output.AppendText(Environment.NewLine + partialAuth);
                Output.AppendText(Environment.NewLine);
            }

            // Call the HTTP Handler
            else if (HTTP.Checked)
            {
                // Set up basic connection parameters
                if (AccessToken.Text != null && AccessToken.Text != "" && !UseAPISerialMID.Checked)
                {
                    httpHandler.setAccessToken(AccessToken.Text);
                    httpHandler.setIPAddress(IPAddress.Text);
                    httpHandler.setPort(Port.Text);
                }
                else
                {
                    httpHandler.setAPISerialMIDPass(APISerial.Text, MID.Text, Password.Text);
                    httpHandler.setIPAddress(IPAddress.Text);
                    httpHandler.setPort(Port.Text);
                }

                // Pass all fields
                var values = new Dictionary<string, string>
                {
                    { "CardNumber", (UnencryptedCardData.Checked) ? CardNumber.Text.Replace(" ","") : ""},
                    { "ExpirationDate", ExpirationMonth.Text + ExpirationYear.Text },
                    { "CVV2", CVV2.Text },
                    { "CVV2Indicator", (CVV2.Text != "") ? "1" : "0" },
                    { "CardPresent", CardPresent.Text },
                    { "StreetAddress", StreetAddress.Text },
                    { "DestinationZipCode", ZipCode.Text },
                    { "CardType", CardType.Text },
                    { "TrackData", (UnencryptedTrackData.Checked) ? TrackData.Text : ""},
                    { "UniqueID", (TrueToken.Checked) ? UniqueID.Text : ""},
                    { "TokenSerial", TokenSerial.Text },
                    { "APIOptions", stripped(APIOptions.Text) },
                    { "Invoice", Invoice.Text },
                    { "PrimaryAmount", PrimaryDollars.Text + "." + PrimaryCents.Text },
                    { "SaleFlag", SaleFlag.Text },
                    { "ZipCode", ZipCode.Text },
                    { "SecondaryAmount", SecondaryDollars.Text + "." + SecondaryCents.Text },
                    { "FunctionRequestCode", FunctionRequestCode.Text },
                    { "TaxIndicator", TaxIndicator.Checked ? "Y" : "N" },
                    { "TaxAmount", TaxAmountDollars.Text + "." + TaxAmountCents.Text },
                    { "TranID", TranID.Text },
                    { "Clerk", Clerk.Text },
                    { "TerminalID", TerminalID.Text },
                    { "Date", helper.getDate() },
                    { "Time", helper.getTime() },
                    { "VoidInvalidAVS", (VoidInvalidAVS.Checked) ? "Y" : "N"},
                    { "VoidInvalidCVV2", (VoidInvalidCVV2.Checked) ? "Y" : "N" },
                    { "RequestorReference", requestorRefNum.ToString() },
                    { "UseTokenStore", (UseTokenStore.Checked) ? "Y" : "N" },
                    { "UseMetaToken", (UseMetaToken.Checked) ? "Y" : "N" },
                    { "CustomerName", CustomerName.Text },
                    { "P2PEBlock", (P2PE.Checked) ? TrackData.Text : "" },
                    { "UseBasicTranFlow", (UseBasicTranFlow.Checked) ? "Y" : "N" },
                    { "UseRollbacks", (UseRollbacks.Checked) ? "Y" : "N" },
                    { "UseEMV", (UseEMV.Checked) ? "Y" : "N" },
                    { "KSN", KSN.Text },
                    { "CashBack", CashBackAmountDollars.Text + "." + CashBackAmountCents.Text }
                };

                if (UnencryptedCardData.Checked) { values.Add("CardDataType", "UnencryptedCardData"); }
                else if (UnencryptedTrackData.Checked) { values.Add("CardDataType", "UnencryptedTrackData"); }
                else if (UTGControlledPINPad.Checked) { values.Add("CardDataType", "UTGControlledPINPad"); }
                else if (TrueToken.Checked) { values.Add("CardDataType", "TrueToken"); }
                else if (P2PE.Checked) { values.Add("CardDataType", "P2PE"); }
                Dictionary<string, string> x = new Dictionary<string, string>();
                try
                {
                   x = await Task.Run(() => httpHandler.start(values));
                }
                catch (System.Net.Sockets.SocketException se)
                {
                    Console.WriteLine(se.StackTrace);
                    Output.AppendText(Environment.NewLine);
                    Output.AppendText("###################################");
                    Output.AppendText("#SOCKET EXCEPTION: UTG NOT STARTED#");
                    Output.AppendText("###################################");
                    Output.AppendText(Environment.NewLine);
                }
                             
                var asString = helper.parseDict(x);
                requestorRefNum++;
                // Update Field Values here
                string partialAuth = "";
                double offset = 0.00;
                try
                {
                    UniqueID.Text = x["uniqueid"];
                    if (Double.Parse(values["PrimaryAmount"]) > Double.Parse(x["primaryamount"]) &&
                        (x["functionrequestcode"] == "1B" || x["functionrequestcode"] == "1D" || x["functionrequestcode"] == "05" || x["functionrequestcode"] == "06"))
                    {
                        offset = Double.Parse(values["PrimaryAmount"]) + Double.Parse(values["TaxAmount"]) - Double.Parse(x["primaryamount"]);
                        partialAuth = "WARNING: PARTIAL AUTH RECIEVED. REMAINING TENDER NEEDED: " + offset.ToString();
                    }
                    TranID.Text = x["tranid"];
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                    // Do Nothing
                }

                Output.AppendText(Environment.NewLine);
                Output.AppendText(Environment.NewLine);
                Output.AppendText("-----------------------------------------------");
                Output.AppendText(Environment.NewLine);
                Output.AppendText(asString);
                Output.AppendText(Environment.NewLine + partialAuth);
                Output.AppendText(Environment.NewLine);
            }

            // Call the REST Handler
            else if (Rest.Checked)
            {
                // Set up basic connection parameters
                if (AccessToken.Text != null && AccessToken.Text != "")
                {
                    restHandler.setAccessToken(AccessToken.Text);
                    restHandler.setIPAddress(IPAddress.Text);
                    restHandler.setPort(Port.Text);
                }
                // Pass all fields
                var values = new Dictionary<string, string>
                {
                    { "CardNumber", (UnencryptedCardData.Checked) ? CardNumber.Text.Replace(" ","") : ""},
                    { "ExpirationDate", ExpirationMonth.Text + ExpirationYear.Text },
                    { "CVV2", CVV2.Text },
                    { "CVV2Indicator", (CVV2.Text != "") ? "1" : "0" },
                    { "CardPresent", CardPresent.Text },
                    { "StreetAddress", StreetAddress.Text },
                    { "DestinationZipCode", ZipCode.Text },
                    { "CardType", CardType.Text },
                    { "TrackData", (UnencryptedTrackData.Checked) ? TrackData.Text : ""},
                    { "UniqueID", (TrueToken.Checked) ? UniqueID.Text : ""},
                    { "TokenSerial", TokenSerial.Text },
                    { "APIOptions", stripped(APIOptions.Text) },
                    { "Invoice", Invoice.Text },
                    { "PrimaryAmount", PrimaryDollars.Text + "." + PrimaryCents.Text },
                    { "SaleFlag", SaleFlag.Text },
                    { "ZipCode", ZipCode.Text },
                    { "SecondaryAmount", SecondaryDollars.Text + "." + SecondaryCents.Text },
                    { "FunctionRequestCode", FunctionRequestCode.Text },
                    { "TaxIndicator", TaxIndicator.Checked ? "Y" : "N" },
                    { "TaxAmount", TaxAmountDollars.Text + "." + TaxAmountCents.Text },
                    { "TranID", TranID.Text },
                    { "Clerk", Clerk.Text },
                    { "TerminalID", TerminalID.Text },
                    { "Date", helper.getDate() },
                    { "Time", helper.getTime() },
                    { "VoidInvalidAVS", (VoidInvalidAVS.Checked) ? "Y" : "N"},
                    { "VoidInvalidCVV2", (VoidInvalidCVV2.Checked) ? "Y" : "N" },
                    { "RequestorReference", requestorRefNum.ToString() },
                    { "UseTokenStore", (UseTokenStore.Checked) ? "Y" : "N" },
                    { "UseMetaToken", (UseMetaToken.Checked) ? "Y" : "N" },
                    { "CustomerName", CustomerName.Text },
                    { "P2PEBlock", (P2PE.Checked) ? TrackData.Text : "" },
                    { "UseBasicTranFlow", (UseBasicTranFlow.Checked) ? "Y" : "N" },
                    { "UseRollbacks", (UseRollbacks.Checked) ? "Y" : "N" },
                    { "UseAuthCapture", UseAuthCapture.Checked ? "Y" : "N" },
                    { "UseEMV", (UseEMV.Checked) ? "Y" : "N" },
                    { "KSN", KSN.Text },
                    { "UseIDTech", (UseIDTech.Checked) ? "Y" :"N" },
                    { "CashBack", CashBackAmountDollars.Text + "." + CashBackAmountCents.Text }
                };

                if (UnencryptedCardData.Checked) { values.Add("CardDataType", "UnencryptedCardData"); }
                else if (UnencryptedTrackData.Checked) { values.Add("CardDataType", "UnencryptedTrackData"); }
                else if (UTGControlledPINPad.Checked) { values.Add("CardDataType", "UTGControlledPINPad"); }
                else if (TrueToken.Checked) { values.Add("CardDataType", "TrueToken"); }
                else if (P2PE.Checked) { values.Add("CardDataType", "P2PE"); }
                Dictionary<string, string> x = new Dictionary<string, string>();
                try
                {
                    x = await Task.Run(() => restHandler.start(values));
                }
                catch (System.Net.Sockets.SocketException se)
                {
                    Console.WriteLine(se.StackTrace);
                    Output.AppendText(Environment.NewLine);
                    Output.AppendText("###################################");
                    Output.AppendText("#SOCKET EXCEPTION: UTG NOT STARTED#");
                    Output.AppendText("###################################");
                    Output.AppendText(Environment.NewLine);
                }

                var asString = helper.parseDict(x);
                requestorRefNum++;
                // Update Field Values here
                string partialAuth = "";
                double offset = 0.00;
                try
                {
                    UniqueID.Text = x["UniqueID"];
                    if (Double.Parse(values["PrimaryAmount"]) > Double.Parse(x["PrimaryAmount"]) &&
                        (x["functionrequestcode"] == "1B" || x["functionrequestcode"] == "1D" || x["functionrequestcode"] == "05" || x["functionrequestcode"] == "06"))
                    {
                        offset = Double.Parse(values["PrimaryAmount"]) + Double.Parse(values["TaxAmount"]) - Double.Parse(x["primaryamount"]);
                        partialAuth = "WARNING: PARTIAL AUTH RECIEVED. REMAINING TENDER NEEDED: " + offset.ToString();
                    }
                    TranID.Text = x["tranid"];
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                    // Do Nothing
                }

                Output.AppendText(Environment.NewLine);
                Output.AppendText(Environment.NewLine);
                Output.AppendText("-----------------------------------------------");
                Output.AppendText(Environment.NewLine);
                Output.AppendText(asString);
                Output.AppendText(Environment.NewLine + partialAuth);
                Output.AppendText(Environment.NewLine);
            }

            // Update the invoice number
            if (!UseSameInvoice.Checked)
            {
                int txt = Int32.Parse(Invoice.Text);
                txt++;
                Invoice.Text = txt.ToString();
            }
        }

        // Sends a transaction given the raw string passed. ** This may still need work in parsing **
        private async void SendRawString_Click(object sender, EventArgs e)
        {
            // Send the Raw string here
            Dictionary<string, string> x;
            if (TCP.Checked)
            {
                tcpHandler.setAccessToken(AccessToken.Text);
                tcpHandler.setAuthToken(AuthToken.Text);
                tcpHandler.setClientGUID(ClientGUID.Text);
                tcpHandler.setIPAddress(IPAddress.Text);
                tcpHandler.setPort(Port.Text);
                x = await Task.Run(() => tcpHandler.sendRaw(rawString.Text));
                var asString = helper.parseDict(x);
                requestorRefNum++;
                // Update Field Values here
                string partialAuth = "";

                Output.AppendText(Environment.NewLine);
                Output.AppendText(Environment.NewLine);
                Output.AppendText("-----------------------------------------------");
                Output.AppendText(Environment.NewLine);
                Output.AppendText(asString);
                Output.AppendText(Environment.NewLine + partialAuth);
                Output.AppendText(Environment.NewLine);
            }
            else if (HTTP.Checked)
            {
                httpHandler.setAccessToken(AccessToken.Text);
                httpHandler.setAuthToken(AuthToken.Text);
                httpHandler.setClientGUID(ClientGUID.Text);
                httpHandler.setIPAddress(IPAddress.Text);
                httpHandler.setPort(Port.Text);
                x = await Task.Run(() => httpHandler.sendRaw(rawString.Text));
                var asString = helper.parseDict(x);
                requestorRefNum++;
                // Update Field Values here
                string partialAuth = "";

                Output.AppendText(Environment.NewLine);
                Output.AppendText(Environment.NewLine);
                Output.AppendText("-----------------------------------------------");
                Output.AppendText(Environment.NewLine);
                Output.AppendText(asString);
                Output.AppendText(Environment.NewLine + partialAuth);
                Output.AppendText(Environment.NewLine);
            }


            else if (Rest.Checked)
            {
                restHandler.setAccessToken(AccessToken.Text);
                restHandler.setAuthToken(AuthToken.Text);
                restHandler.setClientGUID(ClientGUID.Text);
                restHandler.setIPAddress(IPAddress.Text);
                restHandler.setPort(Port.Text);
                // Pass all fields
                var values = new Dictionary<string, string>
                {
                    { "APIOptions", stripped(APIOptions.Text) },
                    { "SaleFlag", SaleFlag.Text },
                    { "UseAuthCapture", UseAuthCapture.Checked ? "Y" : "N" },
                    { "FunctionRequestCode", FunctionRequestCode.Text },
                };
                x = await Task.Run(() => restHandler.sendRaw(rawString.Text, values));
                var asString = helper.parseDict(x);
                requestorRefNum++;
                // Update Field Values here
                string partialAuth = "";

                Output.AppendText(Environment.NewLine);
                Output.AppendText(Environment.NewLine);
                Output.AppendText("-----------------------------------------------");
                Output.AppendText(Environment.NewLine);
                Output.AppendText(asString);
                Output.AppendText(Environment.NewLine + partialAuth);
                Output.AppendText(Environment.NewLine);
            }
        }

        // TCP Selector
        private void TCP_CheckedChanged(object sender, EventArgs e)
        {
            if (TCP.Checked && TLS.Checked)
            {
                Port.Text = "21845";
            }
            else
            {
                Port.Text = "17476";
            }
        }

        // HTTP Selector
        private void HTTP_CheckedChanged(object sender, EventArgs e)
        {
            if (HTTP.Checked && TLS.Checked)
            {
                Port.Text = "16450";
            }
            else
            {
                Port.Text = "16448";
            }
        }

        // TLS Selector
        private void TLS_CheckedChanged(object sender, EventArgs e)
        {
            if (TCP.Checked && TLS.Checked)
            {
                Port.Text = "21845";
            }
            else if (HTTP.Checked && TLS.Checked)
            {
                Port.Text = "16450";
            }
            else if (TCP.Checked && !TLS.Checked)
            {
                Port.Text = "17476";
            }
            else if (HTTP.Checked && !TLS.Checked)
            {
                Port.Text = "16448";
            }
        }

        // Allows Multi-Select of API Options
        private void APIOptionsSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            string options = "";
            foreach (var item in APIOptionsSelect.SelectedItems)
            {
                options += item.ToString() + ",";
            }
            APIOptions.Text = options;
        }

        // Strips Trailing Comma at the end of the API Options List 
        private string stripped(string str)
        {
            int len = str.Length;
            return str.Remove(len - 1, 1);
        }

        // IP Address Drop Down Selector
        private void IPAddress_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IPAddress.Text == "https://cfapi.shift4test.com/api/S4Tran_Action.cfm" ||
                IPAddress.Text == "https://utgapi.shift4test.com/api/rest/v1/")
            {
                AccessToken.Text = "390B2B95-E74C-4774-A88B-C07E5B020488";
                Output.AppendText(Environment.NewLine);
                Output.AppendText("\"Certify\" AccessToken has been set. -> CERTIFY");
            }
            else if (IPAddress.Text == "10.0.2.76" ||
                     IPAddress.Text == "https://10.0.2.76:277/api/rest/v1/" ||
                     IPAddress.Text == "https://10.0.2.130:277/api/rest/v1/" ||
                     IPAddress.Text == "https://wh-cf.s4-test.com/api/s4tran_action.cfm" ||
                     IPAddress.Text == "https://wh-utgapi.s4-test.com/api/rest/v1/" ||
                     IPAddress.Text == "https://wh-utgapi01.s4-test.com/api/rest/v1/" ||
                     IPAddress.Text == "https://wh-utgapi02.s4-test.com/api/rest/v1/")
            {
                //AccessToken.Text = "5773BE4B-9D55-4D65-8D01-5EA4A74CB12E";                AccessToken for Serial 442-WH
                AccessToken.Text = "D1D11203-E30C-2F91-6341A422BD9B9188";                // AccessToken for Serial 542-WH-TonyResort
                Output.AppendText(Environment.NewLine);
                Output.AppendText("\"WartHog\" AccessToken has been set. -> WARTHOG");
            }
        }

        // REST endpoint selection - FRC mapper
        private void restEndpoints_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (restEndpoints.SelectedIndex == 1){
                FunctionRequestCode.Text = "CE";
            }
            else if (restEndpoints.SelectedIndex == 2)
            {
                FunctionRequestCode.Text = "94";
            }
            else if (restEndpoints.SelectedIndex == 3)
            {
                FunctionRequestCode.Text = "E1";
            }
            else if (restEndpoints.SelectedIndex == 4)
            {
                FunctionRequestCode.Text = "08";
            }
            else if (restEndpoints.SelectedIndex == 5)
            {
                FunctionRequestCode.Text = "D9";
            }
            else if (restEndpoints.SelectedIndex == 6)
            {
                FunctionRequestCode.Text = "F2";
            }
            else if (restEndpoints.SelectedIndex == 7)
            {
                FunctionRequestCode.Text = "0B";
            }
            else if (restEndpoints.SelectedIndex == 8)
            {
                FunctionRequestCode.Text = "0D";
            }
            else if (restEndpoints.SelectedIndex == 9)
            {
                FunctionRequestCode.Text = "5F";
            }
            else if (restEndpoints.SelectedIndex == 10)
            {
                FunctionRequestCode.Text = "CA";
            }
            else if (restEndpoints.SelectedIndex == 11)
            {
                FunctionRequestCode.Text = "07";
            }
            else if (restEndpoints.SelectedIndex == 12)
            {
                FunctionRequestCode.Text = "17";
            }
            else if (restEndpoints.SelectedIndex == 13)
            {
                FunctionRequestCode.Text = "22";
            }
            else if (restEndpoints.SelectedIndex == 14)
            {
                FunctionRequestCode.Text = "1B";
            }
            else if (restEndpoints.SelectedIndex == 15)
            {
                FunctionRequestCode.Text = "1D";
            }
            else if (restEndpoints.SelectedIndex == 16)
            {
                FunctionRequestCode.Text = "1D";
            }
            else if (restEndpoints.SelectedIndex == 17)
            {
                FunctionRequestCode.Text = "1D";
            }
            else if (restEndpoints.SelectedIndex == 18)
            {
                FunctionRequestCode.Text = "05";
            }
            else if (restEndpoints.SelectedIndex == 19)
            {
                FunctionRequestCode.Text = "06";
            }
            else if (restEndpoints.SelectedIndex == 20)
            {
                FunctionRequestCode.Text = "20";
            }
            else if (restEndpoints.SelectedIndex == 21)
            {
                FunctionRequestCode.Text = "E0";
            }
            else if (restEndpoints.SelectedIndex == 22)
            {
                FunctionRequestCode.Text = "E2";
            }
            else if (restEndpoints.SelectedIndex == 23)
            {
                FunctionRequestCode.Text = "64";
            }
            else if (restEndpoints.SelectedIndex == 24)
            {
                FunctionRequestCode.Text = "24";
            }
            else if (restEndpoints.SelectedIndex == 25)
            {
                FunctionRequestCode.Text = "61";
            }
            else if (restEndpoints.SelectedIndex == 26)
            {
                FunctionRequestCode.Text = "2A";
            }
            else if (restEndpoints.SelectedIndex == 27)
            {
                FunctionRequestCode.Text = "25";
            }
            else if (restEndpoints.SelectedIndex == 28)
            {
                FunctionRequestCode.Text = "25";
            }
            else if (restEndpoints.SelectedIndex == 29)
            {
                FunctionRequestCode.Text = "26";
            }
            else if (restEndpoints.SelectedIndex == 30)
            {
                FunctionRequestCode.Text = "24";
            }
            else if (restEndpoints.SelectedIndex == 31)
            {
                FunctionRequestCode.Text = "96";
            }
            else if (restEndpoints.SelectedIndex == 32)
            {
                FunctionRequestCode.Text = "95";
            }
            else if (restEndpoints.SelectedIndex == 33)
            {
                FunctionRequestCode.Text = "F1";
            }
            else if (restEndpoints.SelectedIndex == 34)
            {
                FunctionRequestCode.Text = "86";
            }
            else if (restEndpoints.SelectedIndex == 35)
            {
                FunctionRequestCode.Text = "82";
            }
            else if (restEndpoints.SelectedIndex == 36)
            {
                FunctionRequestCode.Text = "DB";
            }
            else if (restEndpoints.SelectedIndex == 37)
            {
                FunctionRequestCode.Text = "DA";
            }
            else if (restEndpoints.SelectedIndex == 38)
            {
                FunctionRequestCode.Text = "47";
            }
            else if (restEndpoints.SelectedIndex == 39)
            {
                FunctionRequestCode.Text = "CF";
            }
            else if (restEndpoints.SelectedIndex == 40)
            {
                FunctionRequestCode.Text = "97";
            }
            else if (restEndpoints.SelectedIndex == 41)
            {
                FunctionRequestCode.Text = "D7";
            }
            else if (restEndpoints.SelectedIndex == 42)
            {
                FunctionRequestCode.Text = "23";
            }
            else if (restEndpoints.SelectedIndex == 43)
            {
                FunctionRequestCode.Text = "D8";
            }
            else if (restEndpoints.SelectedIndex == 44)
            {
                FunctionRequestCode.Text = "2F";
            }
        }

        /******************************************************************************************************
         * Industry Selector Switches                                                                         *
         ******************************************************************************************************/

        private void MoToEcom_CheckedChanged(object sender, EventArgs e)
        {
            tcpHandler.setMerchantType("MoToEcom");
            httpHandler.setMerchantType("MoToEcom");
            restHandler.setMerchantType("MotoEcom");
        }

        private void Hotel_CheckedChanged(object sender, EventArgs e)
        {
            tcpHandler.setMerchantType("Hotel");
            httpHandler.setMerchantType("Hotel");
            restHandler.setMerchantType("Hotel");
        }

        private void FoodAndBeverage_CheckedChanged(object sender, EventArgs e)
        {
            tcpHandler.setMerchantType("FoodAndBeverage");
            httpHandler.setMerchantType("FoodAndBeverage");
            restHandler.setMerchantType("FoodAndBeverage");
        }

        private void Retail_CheckedChanged(object sender, EventArgs e)
        {
            tcpHandler.setMerchantType("Retail");
            httpHandler.setMerchantType("Retail");
            restHandler.setMerchantType("Retail");
        }

        private void AutoRental_CheckedChanged(object sender, EventArgs e)
        {
            tcpHandler.setMerchantType("Auto");
            httpHandler.setMerchantType("Auto");
            restHandler.setMerchantType("Auto");
        }

        /******************************************************************************************************
         * IDTech Related Methods                                                                             *
         ******************************************************************************************************/

        delegate void StringArgReturningVoidDelegate(string text);

        private void appendOutput(string text)
        {
            if (this.Output.InvokeRequired)
            {
                StringArgReturningVoidDelegate d = new StringArgReturningVoidDelegate(appendOutput);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.Output.AppendText(text);
            }
        }

        private void setP2PEText(string text)
        {
            if (this.TrackData.InvokeRequired)
            {
                StringArgReturningVoidDelegate d = new StringArgReturningVoidDelegate(setP2PEText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.TrackData.Text = text;
            }
        }

        private void StartIDTechDevices_Click(object sender, EventArgs e)
        {
            //IPAddress.Text = "https://wh-cf.s4-test.com/api/s4tran_action.cfm";
            //AccessToken.Text = "5773BE4B-9D55-4D65-8D01-5EA4A74CB12E";
            P2PE.PerformClick();
            UseIDTech.Checked = true;
            Thread dataThread = new Thread(new ThreadStart(getData));
            dataThread.Start();
            Output.AppendText(Environment.NewLine);
            Output.AppendText("Get Data Thread Started");
        }

        // Gets the data from the IDTech reader device
        private void getData()
        {
            IDTechHandler handler = new IDTechHandler();
            this.appendOutput(Environment.NewLine);
            // Initialize the devices
            bool ready = handler.InitializeDevices();
            while (!ready)
            {
                this.appendOutput("Devices not initialized.");
            }
            this.appendOutput("Devices Initialized...");
            
            // Get the encryption type
            EncryptionInfo info = handler.getEncryptionInfo();
            this.appendOutput(Environment.NewLine);
            this.appendOutput("ENCRYPTION_TYPE: " + info.DukptFormatType + "   KEY_TYPE: " + info.DukptKeyType);

            // Pair the read heads
            bool paired = handler.pairDevices();
            this.appendOutput(Environment.NewLine);
            if (paired || handler.augusta)
            {
                // Start a transaction
                double pAmount = Double.Parse(PrimaryDollars.Text + "." + PrimaryCents.Text) + Double.Parse(TaxAmountDollars.Text + "." + TaxAmountCents.Text);
                double sAmount = Double.Parse(SecondaryDollars.Text + "." + SecondaryCents.Text);
                this.appendOutput("Starting Threads to get Card Reader Data");
                this.appendOutput(Environment.NewLine);
                this.appendOutput(handler.startTransaction(pAmount, sAmount));
            }
            else
            {
                // Return code indicates paired device not operating in proper mode.
                this.appendOutput("Device Pairing Failed");
            }
            string prev = null;
            // Update the display
            while (handler.FastEMV == null)
            {
                while (handler.msg != null && handler.msg != prev)
                {
                    this.appendOutput(Environment.NewLine + handler.msg);
                    prev = handler.msg;
                } 
            }
            this.appendOutput(Environment.NewLine);
            this.appendOutput("Setting Fast EMV Field from returned data. OK to process transaction.");
            setP2PEText(handler.FastEMV);
        }
    }
}

using System;

namespace FormSim
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.Connection = new System.Windows.Forms.GroupBox();
            this.Password = new System.Windows.Forms.TextBox();
            this.label35 = new System.Windows.Forms.Label();
            this.MID = new System.Windows.Forms.TextBox();
            this.Label444 = new System.Windows.Forms.Label();
            this.APISerial = new System.Windows.Forms.TextBox();
            this.label34 = new System.Windows.Forms.Label();
            this.UseAPISerialMID = new System.Windows.Forms.CheckBox();
            this.UseAuthCapture = new System.Windows.Forms.CheckBox();
            this.Rest = new System.Windows.Forms.RadioButton();
            this.IPAddress = new System.Windows.Forms.ComboBox();
            this.ExchangeToken = new System.Windows.Forms.Button();
            this.AccessToken = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.ClientGUID = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.AuthToken = new System.Windows.Forms.TextBox();
            this.Port = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.TLSSettings = new System.Windows.Forms.GroupBox();
            this.TLS1_2 = new System.Windows.Forms.RadioButton();
            this.TLS1_0 = new System.Windows.Forms.RadioButton();
            this.TLS1_1 = new System.Windows.Forms.RadioButton();
            this.SSL3 = new System.Windows.Forms.RadioButton();
            this.AnySupported = new System.Windows.Forms.RadioButton();
            this.TLS = new System.Windows.Forms.CheckBox();
            this.HTTP = new System.Windows.Forms.RadioButton();
            this.TCP = new System.Windows.Forms.RadioButton();
            this.Output = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.SendTransaction = new System.Windows.Forms.Button();
            this.CardData = new System.Windows.Forms.GroupBox();
            this.UseIDTech = new System.Windows.Forms.CheckBox();
            this.KSN = new System.Windows.Forms.TextBox();
            this.label33 = new System.Windows.Forms.Label();
            this.UseEMV = new System.Windows.Forms.CheckBox();
            this.CardType = new System.Windows.Forms.ComboBox();
            this.label32 = new System.Windows.Forms.Label();
            this.CardPresent = new System.Windows.Forms.TextBox();
            this.UseRollbacks = new System.Windows.Forms.CheckBox();
            this.label31 = new System.Windows.Forms.Label();
            this.CustomerName = new System.Windows.Forms.TextBox();
            this.UseMetaToken = new System.Windows.Forms.CheckBox();
            this.UseTokenStore = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.P2PE = new System.Windows.Forms.RadioButton();
            this.TrueToken = new System.Windows.Forms.RadioButton();
            this.UTGControlledPINPad = new System.Windows.Forms.RadioButton();
            this.UnencryptedTrackData = new System.Windows.Forms.RadioButton();
            this.UnencryptedCardData = new System.Windows.Forms.RadioButton();
            this.TerminalID = new System.Windows.Forms.TextBox();
            this.label28 = new System.Windows.Forms.Label();
            this.Clerk = new System.Windows.Forms.TextBox();
            this.label27 = new System.Windows.Forms.Label();
            this.TranID = new System.Windows.Forms.TextBox();
            this.label26 = new System.Windows.Forms.Label();
            this.TokenSerial = new System.Windows.Forms.ComboBox();
            this.label15 = new System.Windows.Forms.Label();
            this.UniqueID = new System.Windows.Forms.ComboBox();
            this.label14 = new System.Windows.Forms.Label();
            this.TrackData = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.ZipCode = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.StreetAddress = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.CVV2 = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.ExpirationYear = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.ExpirationMonth = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.CardNumber = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.CashBackAmountCents = new System.Windows.Forms.TextBox();
            this.label24 = new System.Windows.Forms.Label();
            this.CashBackAmountDollars = new System.Windows.Forms.TextBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.restEndpoints = new System.Windows.Forms.ComboBox();
            this.UseSameInvoice = new System.Windows.Forms.CheckBox();
            this.TaxAmountCents = new System.Windows.Forms.TextBox();
            this.TaxAmountDollars = new System.Windows.Forms.TextBox();
            this.label25 = new System.Windows.Forms.Label();
            this.TaxIndicator = new System.Windows.Forms.CheckBox();
            this.FunctionRequestCode = new System.Windows.Forms.ComboBox();
            this.label23 = new System.Windows.Forms.Label();
            this.SecondaryCents = new System.Windows.Forms.TextBox();
            this.SecondaryDollars = new System.Windows.Forms.TextBox();
            this.label22 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.SaleFlag = new System.Windows.Forms.ComboBox();
            this.PrimaryCents = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.PrimaryDollars = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.Invoice = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.APIOptionsSelect = new System.Windows.Forms.ListBox();
            this.APIOptions = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.UseBasicTranFlow = new System.Windows.Forms.CheckBox();
            this.MoToEcom = new System.Windows.Forms.RadioButton();
            this.AutoRental = new System.Windows.Forms.RadioButton();
            this.FoodAndBeverage = new System.Windows.Forms.RadioButton();
            this.Hotel = new System.Windows.Forms.RadioButton();
            this.Retail = new System.Windows.Forms.RadioButton();
            this.label29 = new System.Windows.Forms.Label();
            this.VoidInvalidCVV2 = new System.Windows.Forms.CheckBox();
            this.VoidInvalidAVS = new System.Windows.Forms.CheckBox();
            this.label30 = new System.Windows.Forms.Label();
            this.SendRawString = new System.Windows.Forms.Button();
            this.StartIDTechDevices = new System.Windows.Forms.Button();
            this.rawString = new System.Windows.Forms.TextBox();
            this.Connection.SuspendLayout();
            this.TLSSettings.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.CardData.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // Connection
            // 
            this.Connection.Controls.Add(this.Password);
            this.Connection.Controls.Add(this.label35);
            this.Connection.Controls.Add(this.MID);
            this.Connection.Controls.Add(this.Label444);
            this.Connection.Controls.Add(this.APISerial);
            this.Connection.Controls.Add(this.label34);
            this.Connection.Controls.Add(this.UseAPISerialMID);
            this.Connection.Controls.Add(this.UseAuthCapture);
            this.Connection.Controls.Add(this.Rest);
            this.Connection.Controls.Add(this.IPAddress);
            this.Connection.Controls.Add(this.ExchangeToken);
            this.Connection.Controls.Add(this.AccessToken);
            this.Connection.Controls.Add(this.label5);
            this.Connection.Controls.Add(this.ClientGUID);
            this.Connection.Controls.Add(this.label4);
            this.Connection.Controls.Add(this.label3);
            this.Connection.Controls.Add(this.AuthToken);
            this.Connection.Controls.Add(this.Port);
            this.Connection.Controls.Add(this.label2);
            this.Connection.Controls.Add(this.label1);
            this.Connection.Controls.Add(this.TLSSettings);
            this.Connection.Controls.Add(this.TLS);
            this.Connection.Controls.Add(this.HTTP);
            this.Connection.Controls.Add(this.TCP);
            this.Connection.ForeColor = System.Drawing.Color.Lime;
            this.Connection.Location = new System.Drawing.Point(12, 12);
            this.Connection.Name = "Connection";
            this.Connection.Size = new System.Drawing.Size(605, 186);
            this.Connection.TabIndex = 1;
            this.Connection.TabStop = false;
            this.Connection.Text = "Connection";
            // 
            // Password
            // 
            this.Password.Location = new System.Drawing.Point(479, 158);
            this.Password.Name = "Password";
            this.Password.Size = new System.Drawing.Size(120, 20);
            this.Password.TabIndex = 23;
            this.Password.Text = "EPRISE$10-TONY";
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.Location = new System.Drawing.Point(425, 161);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(53, 13);
            this.label35.TabIndex = 22;
            this.label35.Text = "Password";
            // 
            // MID
            // 
            this.MID.Location = new System.Drawing.Point(317, 158);
            this.MID.Name = "MID";
            this.MID.Size = new System.Drawing.Size(100, 20);
            this.MID.TabIndex = 21;
            this.MID.Text = "8003600";
            // 
            // Label444
            // 
            this.Label444.AutoSize = true;
            this.Label444.Location = new System.Drawing.Point(284, 161);
            this.Label444.Name = "Label444";
            this.Label444.Size = new System.Drawing.Size(27, 13);
            this.Label444.TabIndex = 20;
            this.Label444.Text = "MID";
            // 
            // APISerial
            // 
            this.APISerial.Location = new System.Drawing.Point(216, 158);
            this.APISerial.Name = "APISerial";
            this.APISerial.Size = new System.Drawing.Size(54, 20);
            this.APISerial.TabIndex = 19;
            this.APISerial.Text = "542";
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Location = new System.Drawing.Point(157, 161);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(53, 13);
            this.label34.TabIndex = 18;
            this.label34.Text = "API Serial";
            // 
            // UseAPISerialMID
            // 
            this.UseAPISerialMID.AutoSize = true;
            this.UseAPISerialMID.Location = new System.Drawing.Point(10, 160);
            this.UseAPISerialMID.Name = "UseAPISerialMID";
            this.UseAPISerialMID.Size = new System.Drawing.Size(141, 17);
            this.UseAPISerialMID.TabIndex = 17;
            this.UseAPISerialMID.Text = "Use API Serial MID Pwd";
            this.UseAPISerialMID.UseVisualStyleBackColor = true;
            // 
            // UseAuthCapture
            // 
            this.UseAuthCapture.AutoSize = true;
            this.UseAuthCapture.Location = new System.Drawing.Point(180, 20);
            this.UseAuthCapture.Name = "UseAuthCapture";
            this.UseAuthCapture.Size = new System.Drawing.Size(204, 17);
            this.UseAuthCapture.TabIndex = 9;
            this.UseAuthCapture.Text = "Use Auth-Capture Model (REST Only)";
            this.UseAuthCapture.UseVisualStyleBackColor = true;
            // 
            // Rest
            // 
            this.Rest.AutoSize = true;
            this.Rest.Location = new System.Drawing.Point(119, 19);
            this.Rest.Name = "Rest";
            this.Rest.Size = new System.Drawing.Size(54, 17);
            this.Rest.TabIndex = 16;
            this.Rest.TabStop = true;
            this.Rest.Text = "REST";
            this.Rest.UseVisualStyleBackColor = true;
            // 
            // IPAddress
            // 
            this.IPAddress.DisplayMember = "10.17.2.218";
            this.IPAddress.FormattingEnabled = true;
            this.IPAddress.Items.AddRange(new object[] {
            "10.17.2.218",
            "https://cfapi.shift4test.com/api/S4Tran_Action.cfm",
            "https://wh-cf.s4-test.com/api/s4tran_action.cfm",
            "https://10.17.2.218:277/api/rest/v1/",
            "https://10.0.2.130:277/api/rest/v1/",
            "https://utgapi.shift4test.com/api/rest/v1/",
            "https://wh-utgapi.s4-test.com/api/rest/v1/",
            "https://wh-utgapi01.s4-test.com/api/rest/v1/",
            "https://wh-utgapi02.s4-test.com/api/rest/v1/"});
            this.IPAddress.Location = new System.Drawing.Point(187, 41);
            this.IPAddress.Name = "IPAddress";
            this.IPAddress.Size = new System.Drawing.Size(310, 21);
            this.IPAddress.TabIndex = 15;
            this.IPAddress.Text = "10.17.2.218";
            this.IPAddress.SelectedIndexChanged += new System.EventHandler(this.IPAddress_SelectedIndexChanged);
            // 
            // ExchangeToken
            // 
            this.ExchangeToken.ForeColor = System.Drawing.Color.Maroon;
            this.ExchangeToken.Location = new System.Drawing.Point(500, 16);
            this.ExchangeToken.Name = "ExchangeToken";
            this.ExchangeToken.Size = new System.Drawing.Size(99, 23);
            this.ExchangeToken.TabIndex = 14;
            this.ExchangeToken.Text = "Exchange Token";
            this.ExchangeToken.UseVisualStyleBackColor = true;
            this.ExchangeToken.Click += new System.EventHandler(this.ExchangeToken_ClickAsync);
            // 
            // AccessToken
            // 
            this.AccessToken.Location = new System.Drawing.Point(238, 129);
            this.AccessToken.Name = "AccessToken";
            this.AccessToken.Size = new System.Drawing.Size(361, 20);
            this.AccessToken.TabIndex = 13;
            this.AccessToken.Text = "D1D11203-E30C-2F91-6341A422BD9B9188";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(156, 132);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(76, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Access Token";
            // 
            // ClientGUID
            // 
            this.ClientGUID.Location = new System.Drawing.Point(238, 99);
            this.ClientGUID.Name = "ClientGUID";
            this.ClientGUID.Size = new System.Drawing.Size(361, 20);
            this.ClientGUID.TabIndex = 11;
            this.ClientGUID.Text = "21B00A88-E976-66F2-88EA7F78281AFAE2";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(164, 102);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(63, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Client GUID";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(164, 71);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Auth Token";
            // 
            // AuthToken
            // 
            this.AuthToken.Location = new System.Drawing.Point(238, 68);
            this.AuthToken.Name = "AuthToken";
            this.AuthToken.Size = new System.Drawing.Size(361, 20);
            this.AuthToken.TabIndex = 8;
            this.AuthToken.Text = "D15D6347-E3D6-4BB4-7B5632F470EBC751";
            // 
            // Port
            // 
            this.Port.Location = new System.Drawing.Point(535, 42);
            this.Port.Name = "Port";
            this.Port.Size = new System.Drawing.Size(65, 20);
            this.Port.TabIndex = 7;
            this.Port.Text = "16448";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(503, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Port";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(58, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(123, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Main (or Central) UTG IP";
            // 
            // TLSSettings
            // 
            this.TLSSettings.Controls.Add(this.TLS1_2);
            this.TLSSettings.Controls.Add(this.TLS1_0);
            this.TLSSettings.Controls.Add(this.TLS1_1);
            this.TLSSettings.Controls.Add(this.SSL3);
            this.TLSSettings.Controls.Add(this.AnySupported);
            this.TLSSettings.ForeColor = System.Drawing.Color.Lime;
            this.TLSSettings.Location = new System.Drawing.Point(6, 65);
            this.TLSSettings.Name = "TLSSettings";
            this.TLSSettings.Size = new System.Drawing.Size(144, 90);
            this.TLSSettings.TabIndex = 3;
            this.TLSSettings.TabStop = false;
            this.TLSSettings.Text = "TLS Settings";
            // 
            // TLS1_2
            // 
            this.TLS1_2.AutoSize = true;
            this.TLS1_2.Location = new System.Drawing.Point(75, 65);
            this.TLS1_2.Name = "TLS1_2";
            this.TLS1_2.Size = new System.Drawing.Size(63, 17);
            this.TLS1_2.TabIndex = 5;
            this.TLS1_2.TabStop = true;
            this.TLS1_2.Text = "TLS 1.2";
            this.TLS1_2.UseVisualStyleBackColor = true;
            // 
            // TLS1_0
            // 
            this.TLS1_0.AutoSize = true;
            this.TLS1_0.Location = new System.Drawing.Point(6, 65);
            this.TLS1_0.Name = "TLS1_0";
            this.TLS1_0.Size = new System.Drawing.Size(63, 17);
            this.TLS1_0.TabIndex = 4;
            this.TLS1_0.TabStop = true;
            this.TLS1_0.Text = "TLS 1.0";
            this.TLS1_0.UseVisualStyleBackColor = true;
            // 
            // TLS1_1
            // 
            this.TLS1_1.AutoSize = true;
            this.TLS1_1.Location = new System.Drawing.Point(75, 42);
            this.TLS1_1.Name = "TLS1_1";
            this.TLS1_1.Size = new System.Drawing.Size(63, 17);
            this.TLS1_1.TabIndex = 4;
            this.TLS1_1.TabStop = true;
            this.TLS1_1.Text = "TLS 1.1";
            this.TLS1_1.UseVisualStyleBackColor = true;
            // 
            // SSL3
            // 
            this.SSL3.AutoSize = true;
            this.SSL3.Location = new System.Drawing.Point(6, 42);
            this.SSL3.Name = "SSL3";
            this.SSL3.Size = new System.Drawing.Size(54, 17);
            this.SSL3.TabIndex = 4;
            this.SSL3.TabStop = true;
            this.SSL3.Text = "SSL 3";
            this.SSL3.UseVisualStyleBackColor = true;
            // 
            // AnySupported
            // 
            this.AnySupported.AutoSize = true;
            this.AnySupported.Location = new System.Drawing.Point(6, 19);
            this.AnySupported.Name = "AnySupported";
            this.AnySupported.Size = new System.Drawing.Size(95, 17);
            this.AnySupported.TabIndex = 4;
            this.AnySupported.TabStop = true;
            this.AnySupported.Text = "Any Supported";
            this.AnySupported.UseVisualStyleBackColor = true;
            // 
            // TLS
            // 
            this.TLS.AutoSize = true;
            this.TLS.Location = new System.Drawing.Point(6, 43);
            this.TLS.Name = "TLS";
            this.TLS.Size = new System.Drawing.Size(46, 17);
            this.TLS.TabIndex = 2;
            this.TLS.Text = "TLS";
            this.TLS.UseVisualStyleBackColor = true;
            this.TLS.CheckedChanged += new System.EventHandler(this.TLS_CheckedChanged);
            // 
            // HTTP
            // 
            this.HTTP.AutoSize = true;
            this.HTTP.Checked = true;
            this.HTTP.Location = new System.Drawing.Point(6, 19);
            this.HTTP.Name = "HTTP";
            this.HTTP.Size = new System.Drawing.Size(54, 17);
            this.HTTP.TabIndex = 1;
            this.HTTP.TabStop = true;
            this.HTTP.Text = "HTTP";
            this.HTTP.UseVisualStyleBackColor = true;
            this.HTTP.CheckedChanged += new System.EventHandler(this.HTTP_CheckedChanged);
            // 
            // TCP
            // 
            this.TCP.AutoSize = true;
            this.TCP.Location = new System.Drawing.Point(66, 19);
            this.TCP.Name = "TCP";
            this.TCP.Size = new System.Drawing.Size(46, 17);
            this.TCP.TabIndex = 0;
            this.TCP.Text = "TCP";
            this.TCP.UseVisualStyleBackColor = true;
            this.TCP.CheckedChanged += new System.EventHandler(this.TCP_CheckedChanged);
            // 
            // Output
            // 
            this.Output.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Output.Location = new System.Drawing.Point(6, 16);
            this.Output.Multiline = true;
            this.Output.Name = "Output";
            this.Output.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.Output.Size = new System.Drawing.Size(459, 503);
            this.Output.TabIndex = 2;
            this.Output.Text = "welcome";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.AutoSize = true;
            this.groupBox1.Controls.Add(this.Output);
            this.groupBox1.ForeColor = System.Drawing.Color.Lime;
            this.groupBox1.Location = new System.Drawing.Point(620, 12);
            this.groupBox1.MaximumSize = new System.Drawing.Size(936, 1060);
            this.groupBox1.MinimumSize = new System.Drawing.Size(468, 530);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(471, 538);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Output";
            // 
            // SendTransaction
            // 
            this.SendTransaction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.SendTransaction.ForeColor = System.Drawing.Color.Maroon;
            this.SendTransaction.Location = new System.Drawing.Point(945, 666);
            this.SendTransaction.Name = "SendTransaction";
            this.SendTransaction.Size = new System.Drawing.Size(143, 54);
            this.SendTransaction.TabIndex = 4;
            this.SendTransaction.Text = "Send Transaction";
            this.SendTransaction.UseVisualStyleBackColor = true;
            this.SendTransaction.Click += new System.EventHandler(this.SendTransaction_Click);
            // 
            // CardData
            // 
            this.CardData.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.CardData.Controls.Add(this.UseIDTech);
            this.CardData.Controls.Add(this.KSN);
            this.CardData.Controls.Add(this.label33);
            this.CardData.Controls.Add(this.UseEMV);
            this.CardData.Controls.Add(this.CardType);
            this.CardData.Controls.Add(this.label32);
            this.CardData.Controls.Add(this.CardPresent);
            this.CardData.Controls.Add(this.UseRollbacks);
            this.CardData.Controls.Add(this.label31);
            this.CardData.Controls.Add(this.CustomerName);
            this.CardData.Controls.Add(this.UseMetaToken);
            this.CardData.Controls.Add(this.UseTokenStore);
            this.CardData.Controls.Add(this.groupBox3);
            this.CardData.Controls.Add(this.TerminalID);
            this.CardData.Controls.Add(this.label28);
            this.CardData.Controls.Add(this.Clerk);
            this.CardData.Controls.Add(this.label27);
            this.CardData.Controls.Add(this.TranID);
            this.CardData.Controls.Add(this.label26);
            this.CardData.Controls.Add(this.TokenSerial);
            this.CardData.Controls.Add(this.label15);
            this.CardData.Controls.Add(this.UniqueID);
            this.CardData.Controls.Add(this.label14);
            this.CardData.Controls.Add(this.TrackData);
            this.CardData.Controls.Add(this.label13);
            this.CardData.Controls.Add(this.label12);
            this.CardData.Controls.Add(this.ZipCode);
            this.CardData.Controls.Add(this.label11);
            this.CardData.Controls.Add(this.StreetAddress);
            this.CardData.Controls.Add(this.label10);
            this.CardData.Controls.Add(this.CVV2);
            this.CardData.Controls.Add(this.label9);
            this.CardData.Controls.Add(this.ExpirationYear);
            this.CardData.Controls.Add(this.label8);
            this.CardData.Controls.Add(this.ExpirationMonth);
            this.CardData.Controls.Add(this.label7);
            this.CardData.Controls.Add(this.label6);
            this.CardData.Controls.Add(this.CardNumber);
            this.CardData.ForeColor = System.Drawing.Color.Lime;
            this.CardData.Location = new System.Drawing.Point(12, 204);
            this.CardData.Name = "CardData";
            this.CardData.Size = new System.Drawing.Size(605, 257);
            this.CardData.TabIndex = 5;
            this.CardData.TabStop = false;
            this.CardData.Text = "Card Data";
            // 
            // UseIDTech
            // 
            this.UseIDTech.AutoSize = true;
            this.UseIDTech.Location = new System.Drawing.Point(12, 222);
            this.UseIDTech.Name = "UseIDTech";
            this.UseIDTech.Size = new System.Drawing.Size(84, 17);
            this.UseIDTech.TabIndex = 41;
            this.UseIDTech.Text = "Use IDTech";
            this.UseIDTech.UseVisualStyleBackColor = true;
            // 
            // KSN
            // 
            this.KSN.Location = new System.Drawing.Point(320, 169);
            this.KSN.Name = "KSN";
            this.KSN.Size = new System.Drawing.Size(113, 20);
            this.KSN.TabIndex = 40;
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Location = new System.Drawing.Point(281, 172);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(29, 13);
            this.label33.TabIndex = 39;
            this.label33.Text = "KSN";
            // 
            // UseEMV
            // 
            this.UseEMV.AutoSize = true;
            this.UseEMV.Location = new System.Drawing.Point(362, 195);
            this.UseEMV.Name = "UseEMV";
            this.UseEMV.Size = new System.Drawing.Size(71, 17);
            this.UseEMV.TabIndex = 38;
            this.UseEMV.Text = "Use EMV";
            this.UseEMV.UseVisualStyleBackColor = true;
            // 
            // CardType
            // 
            this.CardType.FormattingEnabled = true;
            this.CardType.Location = new System.Drawing.Point(431, 51);
            this.CardType.Name = "CardType";
            this.CardType.Size = new System.Drawing.Size(163, 21);
            this.CardType.TabIndex = 37;
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Location = new System.Drawing.Point(484, 23);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(68, 13);
            this.label32.TabIndex = 36;
            this.label32.Text = "Card Present";
            // 
            // CardPresent
            // 
            this.CardPresent.Location = new System.Drawing.Point(558, 20);
            this.CardPresent.Name = "CardPresent";
            this.CardPresent.Size = new System.Drawing.Size(36, 20);
            this.CardPresent.TabIndex = 35;
            // 
            // UseRollbacks
            // 
            this.UseRollbacks.AutoSize = true;
            this.UseRollbacks.Location = new System.Drawing.Point(255, 195);
            this.UseRollbacks.Name = "UseRollbacks";
            this.UseRollbacks.Size = new System.Drawing.Size(95, 17);
            this.UseRollbacks.TabIndex = 34;
            this.UseRollbacks.Text = "Use Rollbacks";
            this.UseRollbacks.UseVisualStyleBackColor = true;
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(9, 172);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(82, 13);
            this.label31.TabIndex = 33;
            this.label31.Text = "Customer Name";
            // 
            // CustomerName
            // 
            this.CustomerName.Location = new System.Drawing.Point(97, 169);
            this.CustomerName.Name = "CustomerName";
            this.CustomerName.Size = new System.Drawing.Size(177, 20);
            this.CustomerName.TabIndex = 32;
            this.CustomerName.Text = "Sterling Archer";
            // 
            // UseMetaToken
            // 
            this.UseMetaToken.AutoSize = true;
            this.UseMetaToken.Location = new System.Drawing.Point(140, 195);
            this.UseMetaToken.Name = "UseMetaToken";
            this.UseMetaToken.Size = new System.Drawing.Size(106, 17);
            this.UseMetaToken.TabIndex = 31;
            this.UseMetaToken.Text = "Use Meta Token";
            this.UseMetaToken.UseVisualStyleBackColor = true;
            // 
            // UseTokenStore
            // 
            this.UseTokenStore.AutoSize = true;
            this.UseTokenStore.Location = new System.Drawing.Point(12, 195);
            this.UseTokenStore.Name = "UseTokenStore";
            this.UseTokenStore.Size = new System.Drawing.Size(107, 17);
            this.UseTokenStore.TabIndex = 30;
            this.UseTokenStore.Text = "Use Token Store";
            this.UseTokenStore.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.P2PE);
            this.groupBox3.Controls.Add(this.TrueToken);
            this.groupBox3.Controls.Add(this.UTGControlledPINPad);
            this.groupBox3.Controls.Add(this.UnencryptedTrackData);
            this.groupBox3.Controls.Add(this.UnencryptedCardData);
            this.groupBox3.ForeColor = System.Drawing.Color.Lime;
            this.groupBox3.Location = new System.Drawing.Point(444, 110);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(155, 138);
            this.groupBox3.TabIndex = 29;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Card Data Type";
            // 
            // P2PE
            // 
            this.P2PE.AutoSize = true;
            this.P2PE.Location = new System.Drawing.Point(6, 111);
            this.P2PE.Name = "P2PE";
            this.P2PE.Size = new System.Drawing.Size(52, 17);
            this.P2PE.TabIndex = 4;
            this.P2PE.TabStop = true;
            this.P2PE.Text = "P2PE";
            this.P2PE.UseVisualStyleBackColor = true;
            // 
            // TrueToken
            // 
            this.TrueToken.AutoSize = true;
            this.TrueToken.Location = new System.Drawing.Point(6, 88);
            this.TrueToken.Name = "TrueToken";
            this.TrueToken.Size = new System.Drawing.Size(145, 17);
            this.TrueToken.TabIndex = 3;
            this.TrueToken.TabStop = true;
            this.TrueToken.Text = "Card-on-File / TrueToken";
            this.TrueToken.UseVisualStyleBackColor = true;
            // 
            // UTGControlledPINPad
            // 
            this.UTGControlledPINPad.AutoSize = true;
            this.UTGControlledPINPad.Location = new System.Drawing.Point(6, 65);
            this.UTGControlledPINPad.Name = "UTGControlledPINPad";
            this.UTGControlledPINPad.Size = new System.Drawing.Size(141, 17);
            this.UTGControlledPINPad.TabIndex = 2;
            this.UTGControlledPINPad.TabStop = true;
            this.UTGControlledPINPad.Text = "UTG Controlled PIN Pad";
            this.UTGControlledPINPad.UseVisualStyleBackColor = true;
            // 
            // UnencryptedTrackData
            // 
            this.UnencryptedTrackData.AutoSize = true;
            this.UnencryptedTrackData.Location = new System.Drawing.Point(6, 42);
            this.UnencryptedTrackData.Name = "UnencryptedTrackData";
            this.UnencryptedTrackData.Size = new System.Drawing.Size(143, 17);
            this.UnencryptedTrackData.TabIndex = 1;
            this.UnencryptedTrackData.TabStop = true;
            this.UnencryptedTrackData.Text = "Unencrypted Track Data";
            this.UnencryptedTrackData.UseVisualStyleBackColor = true;
            // 
            // UnencryptedCardData
            // 
            this.UnencryptedCardData.AutoSize = true;
            this.UnencryptedCardData.Checked = true;
            this.UnencryptedCardData.Location = new System.Drawing.Point(6, 19);
            this.UnencryptedCardData.Name = "UnencryptedCardData";
            this.UnencryptedCardData.Size = new System.Drawing.Size(137, 17);
            this.UnencryptedCardData.TabIndex = 0;
            this.UnencryptedCardData.TabStop = true;
            this.UnencryptedCardData.Text = "Unencrypted Card Data";
            this.UnencryptedCardData.UseVisualStyleBackColor = true;
            // 
            // TerminalID
            // 
            this.TerminalID.Location = new System.Drawing.Point(173, 139);
            this.TerminalID.Name = "TerminalID";
            this.TerminalID.Size = new System.Drawing.Size(100, 20);
            this.TerminalID.TabIndex = 28;
            this.TerminalID.Text = "LocalPinPad";
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(115, 142);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(58, 13);
            this.label28.TabIndex = 27;
            this.label28.Text = "TerminalID";
            // 
            // Clerk
            // 
            this.Clerk.Location = new System.Drawing.Point(41, 139);
            this.Clerk.Name = "Clerk";
            this.Clerk.Size = new System.Drawing.Size(62, 20);
            this.Clerk.TabIndex = 26;
            this.Clerk.Text = "545";
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(9, 142);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(31, 13);
            this.label27.TabIndex = 25;
            this.label27.Text = "Clerk";
            // 
            // TranID
            // 
            this.TranID.Location = new System.Drawing.Point(320, 139);
            this.TranID.Name = "TranID";
            this.TranID.Size = new System.Drawing.Size(113, 20);
            this.TranID.TabIndex = 24;
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(281, 142);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(40, 13);
            this.label26.TabIndex = 23;
            this.label26.Text = "TranID";
            // 
            // TokenSerial
            // 
            this.TokenSerial.FormattingEnabled = true;
            this.TokenSerial.Items.AddRange(new object[] {
            "333"});
            this.TokenSerial.Location = new System.Drawing.Point(343, 110);
            this.TokenSerial.Name = "TokenSerial";
            this.TokenSerial.Size = new System.Drawing.Size(90, 21);
            this.TokenSerial.TabIndex = 22;
            this.TokenSerial.Text = "265";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(260, 113);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(77, 13);
            this.label15.TabIndex = 21;
            this.label15.Text = "Token Serial #";
            // 
            // UniqueID
            // 
            this.UniqueID.FormattingEnabled = true;
            this.UniqueID.Location = new System.Drawing.Point(111, 110);
            this.UniqueID.Name = "UniqueID";
            this.UniqueID.Size = new System.Drawing.Size(135, 21);
            this.UniqueID.TabIndex = 20;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(9, 113);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(94, 13);
            this.label14.TabIndex = 19;
            this.label14.Text = "UniqueID / Token";
            // 
            // TrackData
            // 
            this.TrackData.FormattingEnabled = true;
            this.TrackData.Location = new System.Drawing.Point(178, 80);
            this.TrackData.Name = "TrackData";
            this.TrackData.Size = new System.Drawing.Size(416, 21);
            this.TrackData.TabIndex = 16;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(9, 83);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(163, 13);
            this.label13.TabIndex = 15;
            this.label13.Text = "Track Data / P2PE Block / EMV";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(365, 54);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(56, 13);
            this.label12.TabIndex = 13;
            this.label12.Text = "Card Type";
            // 
            // ZipCode
            // 
            this.ZipCode.FormattingEnabled = true;
            this.ZipCode.Items.AddRange(new object[] {
            "65000",
            "78000"});
            this.ZipCode.Location = new System.Drawing.Point(280, 51);
            this.ZipCode.Name = "ZipCode";
            this.ZipCode.Size = new System.Drawing.Size(70, 21);
            this.ZipCode.TabIndex = 12;
            this.ZipCode.Text = "65000";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(224, 54);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(50, 13);
            this.label11.TabIndex = 11;
            this.label11.Text = "Zip Code";
            // 
            // StreetAddress
            // 
            this.StreetAddress.FormattingEnabled = true;
            this.StreetAddress.Items.AddRange(new object[] {
            "65 Main Street",
            "78 Main Street"});
            this.StreetAddress.Location = new System.Drawing.Point(92, 51);
            this.StreetAddress.Name = "StreetAddress";
            this.StreetAddress.Size = new System.Drawing.Size(121, 21);
            this.StreetAddress.TabIndex = 10;
            this.StreetAddress.Text = "65 Main Street";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(9, 54);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(76, 13);
            this.label10.TabIndex = 9;
            this.label10.Text = "Street Address";
            // 
            // CVV2
            // 
            this.CVV2.FormattingEnabled = true;
            this.CVV2.Items.AddRange(new object[] {
            "333",
            "444",
            "555"});
            this.CVV2.Location = new System.Drawing.Point(405, 19);
            this.CVV2.Name = "CVV2";
            this.CVV2.Size = new System.Drawing.Size(69, 21);
            this.CVV2.TabIndex = 7;
            this.CVV2.Text = "333";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(365, 22);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(34, 13);
            this.label9.TabIndex = 6;
            this.label9.Text = "CVV2";
            // 
            // ExpirationYear
            // 
            this.ExpirationYear.Location = new System.Drawing.Point(327, 19);
            this.ExpirationYear.Name = "ExpirationYear";
            this.ExpirationYear.Size = new System.Drawing.Size(23, 20);
            this.ExpirationYear.TabIndex = 5;
            this.ExpirationYear.Text = "20";
            this.ExpirationYear.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(309, 23);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(12, 13);
            this.label8.TabIndex = 4;
            this.label8.Text = "/";
            // 
            // ExpirationMonth
            // 
            this.ExpirationMonth.Location = new System.Drawing.Point(280, 19);
            this.ExpirationMonth.Name = "ExpirationMonth";
            this.ExpirationMonth.Size = new System.Drawing.Size(23, 20);
            this.ExpirationMonth.TabIndex = 3;
            this.ExpirationMonth.Text = "12";
            this.ExpirationMonth.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(221, 22);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 13);
            this.label7.TabIndex = 2;
            this.label7.Text = "Expiration";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 22);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(69, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Card Number";
            // 
            // CardNumber
            // 
            this.CardNumber.FormattingEnabled = true;
            this.CardNumber.Items.AddRange(new object[] {
            "4761739001010010",
            "5413330089604111",
            "374245455400001",
            "3112030205926211",
            "6011000000004444",
            "6510000000000034",
            "3095000000009894",
            "980000111",
            "6034591400025529"});
            this.CardNumber.Location = new System.Drawing.Point(81, 19);
            this.CardNumber.Name = "CardNumber";
            this.CardNumber.Size = new System.Drawing.Size(132, 21);
            this.CardNumber.TabIndex = 0;
            this.CardNumber.Text = "4761739001010010";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox2.Controls.Add(this.CashBackAmountCents);
            this.groupBox2.Controls.Add(this.label24);
            this.groupBox2.Controls.Add(this.CashBackAmountDollars);
            this.groupBox2.Controls.Add(this.checkBox1);
            this.groupBox2.Controls.Add(this.restEndpoints);
            this.groupBox2.Controls.Add(this.UseSameInvoice);
            this.groupBox2.Controls.Add(this.TaxAmountCents);
            this.groupBox2.Controls.Add(this.TaxAmountDollars);
            this.groupBox2.Controls.Add(this.label25);
            this.groupBox2.Controls.Add(this.TaxIndicator);
            this.groupBox2.Controls.Add(this.FunctionRequestCode);
            this.groupBox2.Controls.Add(this.label23);
            this.groupBox2.Controls.Add(this.SecondaryCents);
            this.groupBox2.Controls.Add(this.SecondaryDollars);
            this.groupBox2.Controls.Add(this.label22);
            this.groupBox2.Controls.Add(this.label21);
            this.groupBox2.Controls.Add(this.label20);
            this.groupBox2.Controls.Add(this.SaleFlag);
            this.groupBox2.Controls.Add(this.PrimaryCents);
            this.groupBox2.Controls.Add(this.label19);
            this.groupBox2.Controls.Add(this.PrimaryDollars);
            this.groupBox2.Controls.Add(this.label18);
            this.groupBox2.Controls.Add(this.Invoice);
            this.groupBox2.Controls.Add(this.label17);
            this.groupBox2.Controls.Add(this.APIOptionsSelect);
            this.groupBox2.Controls.Add(this.APIOptions);
            this.groupBox2.Controls.Add(this.label16);
            this.groupBox2.ForeColor = System.Drawing.Color.Lime;
            this.groupBox2.Location = new System.Drawing.Point(12, 467);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(579, 253);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Authorization Details";
            // 
            // CashBackAmountCents
            // 
            this.CashBackAmountCents.Location = new System.Drawing.Point(370, 172);
            this.CashBackAmountCents.Name = "CashBackAmountCents";
            this.CashBackAmountCents.Size = new System.Drawing.Size(29, 20);
            this.CashBackAmountCents.TabIndex = 27;
            this.CashBackAmountCents.Text = "00";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(359, 175);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(10, 13);
            this.label24.TabIndex = 26;
            this.label24.Text = ".";
            // 
            // CashBackAmountDollars
            // 
            this.CashBackAmountDollars.Location = new System.Drawing.Point(300, 172);
            this.CashBackAmountDollars.Name = "CashBackAmountDollars";
            this.CashBackAmountDollars.Size = new System.Drawing.Size(57, 20);
            this.CashBackAmountDollars.TabIndex = 25;
            this.CashBackAmountDollars.Text = "0";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(178, 174);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(117, 17);
            this.checkBox1.TabIndex = 24;
            this.checkBox1.Text = "Cash Back Amount";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // restEndpoints
            // 
            this.restEndpoints.FormattingEnabled = true;
            this.restEndpoints.Items.AddRange(new object[] {
            "Select a RESTful Endpoint ...",
            "POST AccessToken Exchange - /credentials/accesstoken (CE)",
            "DELETE Clear Line Items - /devices/lineitems (94)",
            "DELETE Token Store Delete - /tokens (E1)",
            "DELETE Full Void - transactions/invoice (08)",
            "GET Card Block Status - cards/blockstatus (D9)",
            "GET Device Information - /devices/info (F2)",
            "GET DBA Information - /merchants/merchant (0B)",
            "GET Next Invoice - /reports/batchdetails (0D)",
            "GET Totals Report - /reports/batchtotals (5F)",
            "GET Status Request - /sessions (CA)",
            "GET Invoice - /transactions/invoice (07)",
            "GET Valid Check IDs - / (17)",
            "GET Voice Center Information - / (22)",
            "POST Online Authorization - /transactions/authorization (1B)",
            "POST Capture - /transactions/capture (1D)",
            "POST Online Sale - /transactions/sale (1D)",
            "POST Online Refund - /transactions/refund (1D)",
            "POST Offline Authorization - /transactions/manualauthorization (05)",
            "POST Offline Sale - /transactions/manualsale (06)",
            "POST Signature Upload - /transactions/signature (20)",
            "POST Token Store Add - /tokens/add (E0)",
            "POST Token Duplicate - /tokens/duplicate (E2)",
            "POST 4 Words from TrueToken - /tokens/4words (64)",
            "POST Gift Card Activate - /giftcards/activate (24)",
            "POST Gift Card Inquiry - /giftcards/balance (61)",
            "POST Gift Card Cancel - /giftcards/cancel (2A)",
            "POST Gift Card Cashout - /giftcards/cashout (25)",
            "POST Gift Card Deactivate - /giftcards/deactivate (25)",
            "POST Gift Card Reactivate - /giftcards/reactivate (26)",
            "POST Gift Card Activate - /giftcards/reload (24)",
            "POST Swipe / Insert Ahead - /devices/initializereaders (96)",
            "POST Display Line Items - /devices/lineitems (95)",
            "POST Print Receipt - /devices/print (F1)",
            "POST Process Form - /devices/processform (86)",
            "POST Prompt Confirmation - /devices/promptconfirmation (82)",
            "POST Prompt for Input - /devices/promptinput (DB)",
            "POST On-Demand Card Read - /devices/promptcardread (DA)",
            "POST Prompt for Signature - /devices/promptsignature (47)",
            "POST Prompt Terms and Conditions - /devices/termsandconditions (CF)",
            "POST PIN Pad Reset - /devices/reset (97)",
            "POST Block Card - /cards/block (D7)",
            "POST Identify Card Type - /cards/identify (23)",
            "POST Unblock Card - /cards/unblock (D8)",
            "POST Verify Card - /cards/verify (2F)"});
            this.restEndpoints.Location = new System.Drawing.Point(180, 214);
            this.restEndpoints.Name = "restEndpoints";
            this.restEndpoints.Size = new System.Drawing.Size(386, 21);
            this.restEndpoints.TabIndex = 23;
            this.restEndpoints.Text = "Select a RESTful Endpoint ...";
            this.restEndpoints.SelectedIndexChanged += new System.EventHandler(this.restEndpoints_SelectedIndexChanged);
            // 
            // UseSameInvoice
            // 
            this.UseSameInvoice.AutoSize = true;
            this.UseSameInvoice.Location = new System.Drawing.Point(341, 60);
            this.UseSameInvoice.Name = "UseSameInvoice";
            this.UseSameInvoice.Size = new System.Drawing.Size(113, 17);
            this.UseSameInvoice.TabIndex = 22;
            this.UseSameInvoice.Text = "Use Same Invoice";
            this.UseSameInvoice.UseVisualStyleBackColor = true;
            // 
            // TaxAmountCents
            // 
            this.TaxAmountCents.Location = new System.Drawing.Point(370, 146);
            this.TaxAmountCents.Name = "TaxAmountCents";
            this.TaxAmountCents.Size = new System.Drawing.Size(29, 20);
            this.TaxAmountCents.TabIndex = 21;
            this.TaxAmountCents.Text = "00";
            // 
            // TaxAmountDollars
            // 
            this.TaxAmountDollars.Location = new System.Drawing.Point(300, 146);
            this.TaxAmountDollars.Name = "TaxAmountDollars";
            this.TaxAmountDollars.Size = new System.Drawing.Size(57, 20);
            this.TaxAmountDollars.TabIndex = 20;
            this.TaxAmountDollars.Text = "0";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(359, 150);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(10, 13);
            this.label25.TabIndex = 19;
            this.label25.Text = ".";
            // 
            // TaxIndicator
            // 
            this.TaxIndicator.AutoSize = true;
            this.TaxIndicator.Location = new System.Drawing.Point(178, 149);
            this.TaxIndicator.Name = "TaxIndicator";
            this.TaxIndicator.Size = new System.Drawing.Size(83, 17);
            this.TaxIndicator.TabIndex = 17;
            this.TaxIndicator.Text = "Tax Amount";
            this.TaxIndicator.UseVisualStyleBackColor = true;
            // 
            // FunctionRequestCode
            // 
            this.FunctionRequestCode.FormattingEnabled = true;
            this.FunctionRequestCode.Items.AddRange(new object[] {
            "1B",
            "1D",
            "05",
            "06",
            "08",
            "07",
            "0B",
            "20",
            "22",
            "23",
            "2F",
            "47",
            "64",
            "CA",
            "F1",
            "F2",
            "82",
            "86",
            "CF",
            "DA",
            "DB",
            "17",
            "1F",
            "5F",
            "92",
            "94",
            "95",
            "96",
            "97",
            "24",
            "25",
            "26",
            "61",
            "E0",
            "E2",
            "D7",
            "D8",
            "D9",
            "CD"});
            this.FunctionRequestCode.Location = new System.Drawing.Point(500, 58);
            this.FunctionRequestCode.Name = "FunctionRequestCode";
            this.FunctionRequestCode.Size = new System.Drawing.Size(66, 21);
            this.FunctionRequestCode.TabIndex = 16;
            this.FunctionRequestCode.Text = "1B";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(465, 61);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(28, 13);
            this.label23.TabIndex = 15;
            this.label23.Text = "FRC";
            // 
            // SecondaryCents
            // 
            this.SecondaryCents.Location = new System.Drawing.Point(370, 119);
            this.SecondaryCents.Name = "SecondaryCents";
            this.SecondaryCents.Size = new System.Drawing.Size(29, 20);
            this.SecondaryCents.TabIndex = 14;
            this.SecondaryCents.Text = "00";
            // 
            // SecondaryDollars
            // 
            this.SecondaryDollars.Location = new System.Drawing.Point(300, 120);
            this.SecondaryDollars.Name = "SecondaryDollars";
            this.SecondaryDollars.Size = new System.Drawing.Size(57, 20);
            this.SecondaryDollars.TabIndex = 13;
            this.SecondaryDollars.Text = "0";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(359, 123);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(10, 13);
            this.label22.TabIndex = 12;
            this.label22.Text = ".";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(177, 123);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(97, 13);
            this.label21.TabIndex = 11;
            this.label21.Text = "Secondary Amount";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(422, 95);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(51, 13);
            this.label20.TabIndex = 10;
            this.label20.Text = "Sale Flag";
            // 
            // SaleFlag
            // 
            this.SaleFlag.FormattingEnabled = true;
            this.SaleFlag.Items.AddRange(new object[] {
            "S",
            "C"});
            this.SaleFlag.Location = new System.Drawing.Point(479, 90);
            this.SaleFlag.Name = "SaleFlag";
            this.SaleFlag.Size = new System.Drawing.Size(34, 21);
            this.SaleFlag.TabIndex = 9;
            this.SaleFlag.Text = "S";
            // 
            // PrimaryCents
            // 
            this.PrimaryCents.Location = new System.Drawing.Point(370, 91);
            this.PrimaryCents.Name = "PrimaryCents";
            this.PrimaryCents.Size = new System.Drawing.Size(29, 20);
            this.PrimaryCents.TabIndex = 8;
            this.PrimaryCents.Text = "00";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(359, 94);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(10, 13);
            this.label19.TabIndex = 7;
            this.label19.Text = ".";
            // 
            // PrimaryDollars
            // 
            this.PrimaryDollars.Location = new System.Drawing.Point(280, 91);
            this.PrimaryDollars.Name = "PrimaryDollars";
            this.PrimaryDollars.Size = new System.Drawing.Size(77, 20);
            this.PrimaryDollars.TabIndex = 6;
            this.PrimaryDollars.Text = "100";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(177, 94);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(80, 13);
            this.label18.TabIndex = 5;
            this.label18.Text = "Primary Amount";
            // 
            // Invoice
            // 
            this.Invoice.Location = new System.Drawing.Point(225, 58);
            this.Invoice.Name = "Invoice";
            this.Invoice.Size = new System.Drawing.Size(100, 20);
            this.Invoice.TabIndex = 4;
            this.Invoice.Text = "8465";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(177, 61);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(42, 13);
            this.label17.TabIndex = 3;
            this.label17.Text = "Invoice";
            // 
            // APIOptionsSelect
            // 
            this.APIOptionsSelect.FormattingEnabled = true;
            this.APIOptionsSelect.Items.AddRange(new object[] {
            "ALLDATA",
            "ALLOWCASHBACK",
            "ALLOWPARTIALAUTH",
            "APPENDLINEITEM",
            "BYPASSAMOUNTOK",
            "BYPASSSIGCAP",
            "BYPASSTIP",
            "BYPASSUTG",
            "DISABLECONTACTLESS",
            "DISABLEEMV",
            "DISABLEMCE",
            "DISCARDTRACKINFO",
            "EBTCASH",
            "EBTFOOD",
            "EBTWITHDRAW",
            "ENHANCEDRECEIPTS",
            "FULLDBANAME",
            "GCCASHOUT",
            "GCCASHOUT",
            "INVMUSTEXIST",
            "IYCDEACTIVEONLY",
            "IYCRECHARGE",
            "LANECLOSED",
            "NONPROBLEMSONLY",
            "NOSIGNATURE",
            "ONLINEREFUND",
            "PRINTTIPLINE",
            "RETURNCURRENCYCODE",
            "RETURNEXPDATE",
            "RETURNMETATOKEN",
            "RETURNSIGNATURE",
            "TOKENAUTH",
            "USECARDNAME",
            "USEMCE"});
            this.APIOptionsSelect.Location = new System.Drawing.Point(12, 49);
            this.APIOptionsSelect.Name = "APIOptionsSelect";
            this.APIOptionsSelect.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.APIOptionsSelect.Size = new System.Drawing.Size(152, 186);
            this.APIOptionsSelect.TabIndex = 2;
            this.APIOptionsSelect.SelectedIndexChanged += new System.EventHandler(this.APIOptionsSelect_SelectedIndexChanged);
            // 
            // APIOptions
            // 
            this.APIOptions.Location = new System.Drawing.Point(81, 23);
            this.APIOptions.Name = "APIOptions";
            this.APIOptions.Size = new System.Drawing.Size(490, 20);
            this.APIOptions.TabIndex = 1;
            this.APIOptions.Text = "ALLDATA,";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(9, 26);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(63, 13);
            this.label16.TabIndex = 0;
            this.label16.Text = "API Options";
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.UseBasicTranFlow);
            this.groupBox4.Controls.Add(this.MoToEcom);
            this.groupBox4.Controls.Add(this.AutoRental);
            this.groupBox4.Controls.Add(this.FoodAndBeverage);
            this.groupBox4.Controls.Add(this.Hotel);
            this.groupBox4.Controls.Add(this.Retail);
            this.groupBox4.Controls.Add(this.label29);
            this.groupBox4.Controls.Add(this.VoidInvalidCVV2);
            this.groupBox4.Controls.Add(this.VoidInvalidAVS);
            this.groupBox4.ForeColor = System.Drawing.Color.Lime;
            this.groupBox4.Location = new System.Drawing.Point(597, 590);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(340, 130);
            this.groupBox4.TabIndex = 7;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Switches";
            // 
            // UseBasicTranFlow
            // 
            this.UseBasicTranFlow.AutoSize = true;
            this.UseBasicTranFlow.Checked = true;
            this.UseBasicTranFlow.CheckState = System.Windows.Forms.CheckState.Checked;
            this.UseBasicTranFlow.Location = new System.Drawing.Point(6, 25);
            this.UseBasicTranFlow.Name = "UseBasicTranFlow";
            this.UseBasicTranFlow.Size = new System.Drawing.Size(158, 17);
            this.UseBasicTranFlow.TabIndex = 8;
            this.UseBasicTranFlow.Text = "Use Basic Transaction Flow";
            this.UseBasicTranFlow.UseVisualStyleBackColor = true;
            // 
            // MoToEcom
            // 
            this.MoToEcom.AutoSize = true;
            this.MoToEcom.Checked = true;
            this.MoToEcom.Location = new System.Drawing.Point(127, 48);
            this.MoToEcom.Name = "MoToEcom";
            this.MoToEcom.Size = new System.Drawing.Size(127, 17);
            this.MoToEcom.TabIndex = 7;
            this.MoToEcom.TabStop = true;
            this.MoToEcom.Text = "MO/TO/E-Commerce";
            this.MoToEcom.UseVisualStyleBackColor = true;
            this.MoToEcom.CheckedChanged += new System.EventHandler(this.MoToEcom_CheckedChanged);
            // 
            // AutoRental
            // 
            this.AutoRental.AutoSize = true;
            this.AutoRental.Location = new System.Drawing.Point(127, 95);
            this.AutoRental.Name = "AutoRental";
            this.AutoRental.Size = new System.Drawing.Size(81, 17);
            this.AutoRental.TabIndex = 6;
            this.AutoRental.Text = "Auto Rental";
            this.AutoRental.UseVisualStyleBackColor = true;
            this.AutoRental.CheckedChanged += new System.EventHandler(this.AutoRental_CheckedChanged);
            // 
            // FoodAndBeverage
            // 
            this.FoodAndBeverage.AutoSize = true;
            this.FoodAndBeverage.Location = new System.Drawing.Point(127, 71);
            this.FoodAndBeverage.Name = "FoodAndBeverage";
            this.FoodAndBeverage.Size = new System.Drawing.Size(119, 17);
            this.FoodAndBeverage.TabIndex = 5;
            this.FoodAndBeverage.Text = "Food and Beverage";
            this.FoodAndBeverage.UseVisualStyleBackColor = true;
            this.FoodAndBeverage.CheckedChanged += new System.EventHandler(this.FoodAndBeverage_CheckedChanged);
            // 
            // Hotel
            // 
            this.Hotel.AutoSize = true;
            this.Hotel.Location = new System.Drawing.Point(272, 48);
            this.Hotel.Name = "Hotel";
            this.Hotel.Size = new System.Drawing.Size(50, 17);
            this.Hotel.TabIndex = 4;
            this.Hotel.Text = "Hotel";
            this.Hotel.UseVisualStyleBackColor = true;
            this.Hotel.CheckedChanged += new System.EventHandler(this.Hotel_CheckedChanged);
            // 
            // Retail
            // 
            this.Retail.AutoSize = true;
            this.Retail.Location = new System.Drawing.Point(272, 71);
            this.Retail.Name = "Retail";
            this.Retail.Size = new System.Drawing.Size(52, 17);
            this.Retail.TabIndex = 3;
            this.Retail.Text = "Retail";
            this.Retail.UseVisualStyleBackColor = true;
            this.Retail.CheckedChanged += new System.EventHandler(this.Retail_CheckedChanged);
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(191, 26);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(79, 13);
            this.label29.TabIndex = 2;
            this.label29.Text = "Merchant Type";
            // 
            // VoidInvalidCVV2
            // 
            this.VoidInvalidCVV2.AutoSize = true;
            this.VoidInvalidCVV2.Checked = true;
            this.VoidInvalidCVV2.CheckState = System.Windows.Forms.CheckState.Checked;
            this.VoidInvalidCVV2.Location = new System.Drawing.Point(6, 73);
            this.VoidInvalidCVV2.Name = "VoidInvalidCVV2";
            this.VoidInvalidCVV2.Size = new System.Drawing.Size(105, 17);
            this.VoidInvalidCVV2.TabIndex = 1;
            this.VoidInvalidCVV2.Text = "Void Ivalid CVV2";
            this.VoidInvalidCVV2.UseVisualStyleBackColor = true;
            // 
            // VoidInvalidAVS
            // 
            this.VoidInvalidAVS.AutoSize = true;
            this.VoidInvalidAVS.Checked = true;
            this.VoidInvalidAVS.CheckState = System.Windows.Forms.CheckState.Checked;
            this.VoidInvalidAVS.Location = new System.Drawing.Point(6, 49);
            this.VoidInvalidAVS.Name = "VoidInvalidAVS";
            this.VoidInvalidAVS.Size = new System.Drawing.Size(105, 17);
            this.VoidInvalidAVS.TabIndex = 0;
            this.VoidInvalidAVS.Text = "Void Invalid AVS";
            this.VoidInvalidAVS.UseVisualStyleBackColor = true;
            // 
            // label30
            // 
            this.label30.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label30.AutoSize = true;
            this.label30.ForeColor = System.Drawing.Color.Maroon;
            this.label30.Location = new System.Drawing.Point(600, 562);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(59, 13);
            this.label30.TabIndex = 9;
            this.label30.Text = "Raw String";
            // 
            // SendRawString
            // 
            this.SendRawString.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SendRawString.ForeColor = System.Drawing.Color.Maroon;
            this.SendRawString.Location = new System.Drawing.Point(945, 555);
            this.SendRawString.Name = "SendRawString";
            this.SendRawString.Size = new System.Drawing.Size(143, 25);
            this.SendRawString.TabIndex = 10;
            this.SendRawString.Text = "Send Raw String";
            this.SendRawString.UseVisualStyleBackColor = true;
            this.SendRawString.Click += new System.EventHandler(this.SendRawString_Click);
            // 
            // StartIDTechDevices
            // 
            this.StartIDTechDevices.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.StartIDTechDevices.ForeColor = System.Drawing.Color.Maroon;
            this.StartIDTechDevices.Location = new System.Drawing.Point(945, 594);
            this.StartIDTechDevices.Name = "StartIDTechDevices";
            this.StartIDTechDevices.Size = new System.Drawing.Size(143, 26);
            this.StartIDTechDevices.TabIndex = 11;
            this.StartIDTechDevices.Text = "Start IDTech Devices";
            this.StartIDTechDevices.UseVisualStyleBackColor = true;
            this.StartIDTechDevices.Click += new System.EventHandler(this.StartIDTechDevices_Click);
            // 
            // rawString
            // 
            this.rawString.AcceptsReturn = true;
            this.rawString.AcceptsTab = true;
            this.rawString.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.rawString.Location = new System.Drawing.Point(666, 559);
            this.rawString.Multiline = true;
            this.rawString.Name = "rawString";
            this.rawString.Size = new System.Drawing.Size(271, 20);
            this.rawString.TabIndex = 12;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ClientSize = new System.Drawing.Size(1103, 733);
            this.Controls.Add(this.rawString);
            this.Controls.Add(this.StartIDTechDevices);
            this.Controls.Add(this.SendRawString);
            this.Controls.Add(this.label30);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.CardData);
            this.Controls.Add(this.SendTransaction);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.Connection);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Tony\'s Sim";
            this.Connection.ResumeLayout(false);
            this.Connection.PerformLayout();
            this.TLSSettings.ResumeLayout(false);
            this.TLSSettings.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.CardData.ResumeLayout(false);
            this.CardData.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox Connection;
        private System.Windows.Forms.TextBox Port;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox TLSSettings;
        private System.Windows.Forms.RadioButton TLS1_2;
        private System.Windows.Forms.RadioButton TLS1_0;
        private System.Windows.Forms.RadioButton TLS1_1;
        private System.Windows.Forms.RadioButton SSL3;
        private System.Windows.Forms.RadioButton AnySupported;
        private System.Windows.Forms.CheckBox TLS;
        private System.Windows.Forms.RadioButton HTTP;
        private System.Windows.Forms.RadioButton TCP;
        private System.Windows.Forms.Button ExchangeToken;
        private System.Windows.Forms.TextBox AccessToken;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox ClientGUID;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox AuthToken;
        private System.Windows.Forms.TextBox Output;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button SendTransaction;
        private System.Windows.Forms.GroupBox CardData;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ComboBox ZipCode;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox StreetAddress;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox CVV2;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox ExpirationYear;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox ExpirationMonth;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox CardNumber;
        private System.Windows.Forms.ComboBox TokenSerial;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.ComboBox UniqueID;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.ComboBox TrackData;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListBox APIOptionsSelect;
        private System.Windows.Forms.TextBox APIOptions;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox Invoice;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox PrimaryCents;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TextBox PrimaryDollars;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.ComboBox SaleFlag;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox SecondaryCents;
        private System.Windows.Forms.TextBox SecondaryDollars;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.ComboBox FunctionRequestCode;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.TextBox TaxAmountCents;
        private System.Windows.Forms.TextBox TaxAmountDollars;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.CheckBox TaxIndicator;
        private System.Windows.Forms.TextBox TranID;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.TextBox Clerk;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.TextBox TerminalID;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton UnencryptedTrackData;
        private System.Windows.Forms.RadioButton UnencryptedCardData;
        private System.Windows.Forms.RadioButton P2PE;
        private System.Windows.Forms.RadioButton TrueToken;
        private System.Windows.Forms.RadioButton UTGControlledPINPad;
        private System.Windows.Forms.CheckBox UseSameInvoice;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.CheckBox VoidInvalidCVV2;
        private System.Windows.Forms.CheckBox VoidInvalidAVS;
        private System.Windows.Forms.RadioButton MoToEcom;
        private System.Windows.Forms.RadioButton AutoRental;
        private System.Windows.Forms.RadioButton FoodAndBeverage;
        private System.Windows.Forms.RadioButton Hotel;
        private System.Windows.Forms.RadioButton Retail;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.CheckBox UseTokenStore;
        private System.Windows.Forms.CheckBox UseMetaToken;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.Button SendRawString;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.TextBox CustomerName;
        private System.Windows.Forms.CheckBox UseBasicTranFlow;
        private System.Windows.Forms.CheckBox UseRollbacks;
        private System.Windows.Forms.ComboBox CardType;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.TextBox CardPresent;
        private System.Windows.Forms.ComboBox IPAddress;
        private System.Windows.Forms.Button StartIDTechDevices;
        private System.Windows.Forms.RadioButton Rest;
        private System.Windows.Forms.CheckBox UseAuthCapture;
        private System.Windows.Forms.CheckBox UseEMV;
        private System.Windows.Forms.TextBox rawString;
        private System.Windows.Forms.ComboBox restEndpoints;
        private System.Windows.Forms.TextBox KSN;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.CheckBox UseIDTech;
        private System.Windows.Forms.TextBox Password;
        private System.Windows.Forms.Label label35;
        private System.Windows.Forms.TextBox MID;
        private System.Windows.Forms.Label Label444;
        private System.Windows.Forms.TextBox APISerial;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.CheckBox UseAPISerialMID;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.TextBox CashBackAmountCents;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.TextBox CashBackAmountDollars;
    }
}


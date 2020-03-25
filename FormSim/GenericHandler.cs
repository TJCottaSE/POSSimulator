using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace FormSim
{
    /// <summary>
    /// The GenericHandler class is used as a super class for the more specific <see cref="HTTPHandler"/> and
    /// the <see cref="TCPHandler"/> classes which implement the protocol specific functionality. This class
    /// sets up the basic entry point for handling Function Request Codes (FRCs) that should be implemented 
    /// with the protocol specific handlers. By utilizing this entry point, Global timers are set up to check 
    /// for time out occurences and customer references are maintained and tracked.
    /// </summary>
    public class GenericHandler : FRC_Handler
    {
        protected string ClientGUID;
        protected string AuthToken;
        protected string AccessToken;
        protected string IPAddress;
        protected string Port;
        protected string API_SERIAL;
        protected string API_MID;
        protected string API_PASSWORD;
        protected LogWriter log = LogWriter.getInstance;
        protected Helper helper = Helper.getInstance;
        protected string MerchantType = "MoToEcom";
        protected bool useAPIMIDPWD = false;

        /******************************************************************************************************
         * Constructors                                                                                       *
         ******************************************************************************************************/

        public GenericHandler()
        {
            // Intentionally blank
        }
        
        public GenericHandler(string AuthToken, string ClientGUID)
        {
            this.AuthToken = AuthToken;
            this.ClientGUID = ClientGUID;
        }
        
        public GenericHandler(string AuthToken, string ClientGUID, string IPAddress, string Port)
        {
            this.AuthToken = AuthToken;
            this.ClientGUID = ClientGUID;
            this.IPAddress = IPAddress;
            this.Port = Port;
        }

        public GenericHandler(string API_SERIAL, string API_MID, string API_PASSWORD, string IPAddress, string Port)
        {
            this.API_SERIAL = API_SERIAL;
            this.API_MID = API_MID;
            this.API_PASSWORD = API_PASSWORD;
            this.IPAddress = IPAddress;
            this.Port = Port;
            useAPIMIDPWD = true;
        }

        /******************************************************************************************************
         * Primary Functions                                                                                  *
         ******************************************************************************************************/

        /// <summary>
        /// This method needs to be Overridden by a subclass. Method exists here only to allow complete compilation. 
        /// </summary>
        #pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public virtual async Task<bool> performTokenExchange()
        #pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The primary entry point for executing FRCs.
        /// </summary>
        /// <param name="parameters">A dictionary of parameters sent from the front end. See 
        /// <see cref="FRC_Handler.start(Dictionary{string, string})"/> for a listing of the parameters in the Dictionary.</param>
        /// <returns>A thread with the results of the FRC in a dictionary.</returns>
        public async Task<Dictionary<string, string>> start(Dictionary<string, string> parameters)
        {
            Dictionary<string, string> ret = null;
            Random rnd = new Random();
            parameters.Add("CustomerReference", "randCust" + rnd.Next(1, 100000000).ToString());
            ret = startTransaction(parameters).Result;
            return ret;
        }

        /// <summary>
        /// Method that sets up the global timers needed to check for timeouts. This method is used internally
        /// and should not generally be used except in rare or specific cases. Use of <see cref="start(Dictionary{string, string})"/>
        /// is advised for best results.
        /// </summary>
        /// <param name="parameters">A dictionary of parameters sent from the front end. See 
        /// <see cref="FRC_Handler.start(Dictionary{string, string})"/> for a listing of the parameters in the Dictionary.</param>
        /// <returns>A thread with the results of the FRC in a dictionary.</returns>
        protected async Task<Dictionary<string, string>> startTransaction(Dictionary<string, string> parameters)
        {
            Dictionary<string, string> ret = null;
            if (parameters["TaxIndicator"] == "Y" && parameters["FunctionRequestCode"] != "08")
            {
                parameters["PrimaryAmount"] = (Double.Parse(parameters["PrimaryAmount"]) + Double.Parse(parameters["TaxAmount"])).ToString();
            }
            parameters["RequestorReference"] = (Int32.Parse(parameters["RequestorReference"]) + 1).ToString();

            Task<Dictionary<string, string>> task = Task.Run(() => performTransaction(parameters));

            Stopwatch globalTimer = new Stopwatch();
            globalTimer.Start();

            int timer = 65;
            if (parameters["CardDataType"] == "UTGControlledPINPad")
            {
                timer = 120;
                if (parameters["FunctionRequestCode"] == "96")
                {
                    timer = 600; // Generally should just be 120.
                }
            }

            while (!task.IsCompleted)
            {
                globalTimer.Stop();
                TimeSpan ts = globalTimer.Elapsed;
                if (ts.Seconds < timer)
                {
                    globalTimer.Start();
                }
                else // Response not received before global time out.
                {
                    log.Write("Communication Error");
                    if (parameters["FunctionCode"] == "07")
                    {
                        log.Write("An error has occured with invoice # " + parameters["Invoice"]);
                    }
                    else
                    {
                        parameters["FunctionCode"] = "07";
                        parameters["RequestorRefernce"] = (Int32.Parse(parameters["RequestorReference"]) + 1).ToString();
                        System.Threading.Thread.Sleep(5000);
                        ret = await startTransaction(parameters);
                        return ret;
                    }
                }
            }

            ret = task.Result;
            return ret;
        }

        /// <summary>
        /// This method needs to be Overridden by a subclass. Method exists here only to allow complete compilation. 
        /// </summary>
        /// <exception cref="NotImplementedException">If not overridden.</exception>
        /// <param name="parameters">A dictionary of parameters sent from the front end. See 
        /// <see cref="FRC_Handler.start(Dictionary{string, string})"/> for a listing of the parameters in the Dictionary.</param>
        /// <returns>A thread with the results of the FRC in a dictionary.</returns>
        protected virtual async Task<Dictionary<string, string>> performTransaction(Dictionary<string, string> parameters)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method needs to be Overridden by a subclass. Method exists here only to allow complete compilation. 
        /// </summary>
        /// <param name="rawString">The raw string to be sent.</param>
        /// <returns>A thread with a Dictionary Representation of the response.</returns>
        public virtual async Task<Dictionary<string, string>> sendRaw(string rawString)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method needs to be Overridden by a subclass. Method exists here only to allow complete compilation. 
        /// </summary>
        /// <param name="rawString">The raw string to be sent.</param>
        /// <param name="parameters">A Dictionary containing additional parameters to be sent with the raw string.</param>
        /// <returns>A thread with a Dictionary Representation of the response.</returns>
        public virtual async Task<Dictionary<string, string>> sendRaw(string rawString, Dictionary<string, string> parameters)
        {
            throw new NotImplementedException();
        }

        /******************************************************************************************************
         * Getters and Setters                                                                                *
         ******************************************************************************************************/

        public void setAPISerialMIDPass(string serial, string mid, string password)
        {
            this.API_SERIAL = serial;
            this.API_MID = mid;
            this.API_PASSWORD = password;
            useAPIMIDPWD = true;
        }

        public void setIPAddress(string address)
        {
            IPAddress = address;
        }

        public string getIPAddress()
        {
            return IPAddress;
        }

        public void setPort(string Port)
        {
            this.Port = Port;
        }

        public string getPort()
        {
            return Port;
        }

        public void setClientGUID(string id)
        {
            this.ClientGUID = id;
        }

        public string getClientGUID()
        {
            return ClientGUID;
        }

        public void setAuthToken(string token)
        {
            this.AuthToken = token;
        }

        public string getAuthToken()
        {
            return AuthToken;
        }

        public void setAccessToken(string token)
        {
            this.AccessToken = token;
        }

        public string getAccessToken()
        {
            return AccessToken;
        }

        public void setMerchantType(string type)
        {
            this.MerchantType = type;
        }

        public string getMerchantType()
        {
            return MerchantType;
        }

    }
}

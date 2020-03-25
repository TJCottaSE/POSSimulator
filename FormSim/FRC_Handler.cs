using System.Collections.Generic;
using System.Threading.Tasks;

namespace FormSim
{
    interface FRC_Handler
    {
        /******************************************************************************************************
         * Primary Functions                                                                                  *
         ******************************************************************************************************/

        /// <summary>
        /// Performs the token exchange with the clientGUID and Auth Token in exchange for an access token.
        /// NOTE: Both the clientGUID and the AuthToken must have been previously set or token exchange will fail.
        /// See <see cref="setClientGUID(string)"/> AND See <see cref="setAuthToken(string)"/>
        /// </summary>
        /// <returns>True if an Access token was received and set, false otherwise</returns>
        Task<bool> performTokenExchange();
        
        /// <summary>
        /// This provides the main entry point for the performing of a function based on the function request 
        /// code that is passed in <paramref name="parameters"/>. It automatically adjusts requestor references,
        /// sets a global timer for each function (for timeouts), and performs automatic voids and lookups based
        /// on the "Basic Transaction Flow" diagram found in the API Integrations guide. (Ver. 2.34, Pub. 6/6/18)
        /// </summary>
        /// <param name="parameters"><br></br>
        /// <b>CardNumber</b> : Represents the unencrypted card number. This is often used when manual card
        /// entry is the method of choice. During voids, this number should be the last 4 of the card number, if a 
        /// token is not available. In transactions where a card number is not being used, it should be blank.<br></br>
        /// <b>ExpirationDate</b> : Represents the expiration date of the card. Expected for is MMYY. Note that 
        /// no colons or slashes are used, as well as the date is of two digit format.<br></br>
        /// <b>CVV2</b> : This is the CVV2 value of the card, typically 3 or 4 digits.<br></br>
        /// <b>CVV2Indicator</b> : "1" if the CVV2 value is present in the parameter CVV2 above, "0" otherwise.<br></br>
        /// <b>CardPresent</b> : "Y" if the card is present for the transaction, "N" if not. It may be 
        /// necessary to leave this field blank when performing follow up transactions where this value was 
        /// previously indicated.<br></br>
        /// <b>StreetAddress</b> : The billing street address.<br></br>
        /// <b>DestinationZipCode</b> : The location the item will be shipped to for mail order / telephone
        /// order (MOTO) or E-Commerce. For Retail, Lodging, and Auto, it should be the zip code of where the 
        /// services are rendered, or goods are being sold from.<br></br>
        /// <b>CardType</b> : NOT CURRENTLY USED - leave blank<br></br>
        /// <b>TrackData</b> : Used for passing unencrypted Track Data when using a standard mag stripe reader (MSR).<br></br>
        /// <b>UniqueID</b> : Used to pass a token representing Card Holder Data (CHD), Required in TrueToken
        /// transactions.<br></br>
        /// <b>TokenSerial</b> : Serial of the MID. Should be used when using TokenSharing, or a Global Token Store.<br></br>
        /// <b>APIOptions</b> : A comma delimited list of API Options to be sent with the request.<br></br>
        /// <b>Invoice</b> : In invoice number of size not larger than 10 characters.<br></br>
        /// <b>PrimaryAmount</b> : The subtotal amount not including tax and or tip amounts. Note for TCP 
        /// transactions omit the decimal point.<br></br>
        /// <b>SaleFlag</b> : "S" if performing a sale, and "C" if performing a refund.<br></br>
        /// <b>ZipCode</b> : The billing zip code.<br></br>
        /// <b>SecondaryAmount</b> : The tip amount.<br></br>
        /// <b>FunctionRequestCode</b> : The function request code desired to be performed.<br></br>
        /// <b>TaxIndicator</b> : "Y" if tax was charged, "N" otherwise.<br></br>
        /// <b>TaxAmount</b> : The Amount of Tax Charged. Note for TCP Transactions omit the decimal point.<br></br>
        /// <b>TranID</b> : The transaction ID sent back from a transaction. Used for rollbacks of failed
        /// incremental authorizations.<br></br>
        /// <b>Clerk</b> : Five digit clerk ID.<br></br>
        /// <b>TerminalID</b> : The ID of the PIN pad set in UTG Tune-Up. Used to prompt and display to PIN Pad.<br></br>
        /// <b>Date</b> : Current Date in MMDDYY format.<br></br>
        /// <b>Time</b> : Current Time in HHMMSS format.<br></br>
        /// <b>RequestorReference</b> : A refernece number for the request.<br></br>
        /// <b>P2PEBlock</b> : The P2PE Block form a native P2PE device.<br></br>
        /// <b>CustomerName</b> : The name of the Customer.<br></br>
        /// <b>VoidInvalidAVS</b> : A "Y" / "N" switch used to automatically void responses with invalid AVS responses.<br></br>
        /// <b>VoidInvalidCVV2</b> : A "Y" / "N" switch used to automatically void responses with invalid CVV2 responses.<br></br>
        /// <b>UseTokenStore</b> : A "Y" / "N" switch used to use a Token Store.<br></br>
        /// <b>UseMetaToken</b> : A "Y" / "N" switch used to use a MetaToken.<br></br>
        /// <b>UseBasicTranFlow</b> : A "Y" / "N" switch used to control if the automaticity of the basic transaction flow is
        /// used when issuing function request codes. By turning this switch off, only single FRCs will be executed regardlesss
        /// of the result that comes back from the server.<br></br>
        /// <b>UseRollbacks</b> :  A "Y" / "N" switch used to control if a void (FRC 08) will be used as a complete transaction
        /// void or if it will be used to roll back the transaction to it's last approved state.<br></br>
        /// </param>
        /// <returns>A Dictionary of the returned parameters and results of the last function called. 
        /// <para>Note: In cases where automatic voids are called for, the response of the original request will
        /// NOT be retuned, but rather the result of the automatic void will be called. To see all of the transaction
        /// requests and responses, please check the log file that is generated.</para>
        /// </returns>
        Task<Dictionary<string, string>> start(Dictionary<string, string> parameters);

        /// <summary>
        /// Sends a raw request to Shift4 for processing. This is mainly used as a debugging tool, to figure
        /// out if there are issues with the request, that can be deciphered ahead of hitting the Shift4 
        /// processing engine on the back side. This likely should be run in debug mode when full source is available
        /// as common errors that get thrown are not very descriptive. As time permits the error catching should be 
        /// improved to hopefully throw more useful information about the actual error. 
        /// </summary>
        /// <param name="rawString">The Raw String to be sent</param>
        /// <returns>A Dictionary of the returned parameters of the last function called.
        /// See NOTE at: <see cref="start(Dictionary{string, string})"/> as this note also applies here.</returns>
        Task<Dictionary<string, string>> sendRaw(string rawString);

        /// <summary>
        /// Overload
        /// Sends a raw request to Shift4 for processing. This is mainly used as a debugging tool, to figure
        /// out if there are issues with the request, that can be deciphered ahead of hitting the Shift4 
        /// processing engine on the back side. This likely should be run in debug mode when full source is available
        /// as common errors that get thrown are not very descriptive. As time permits the error catching should be 
        /// improved to hopefully throw more useful information about the actual error. 
        /// </summary>
        /// <param name="rawString">The raw string to be sent</param>
        /// <param name="parameters">A list of parameters from the front end</param>
        /// <returns></returns>
        Task<Dictionary<string, string>> sendRaw(string rawString, Dictionary<string, string> parameters);

        /******************************************************************************************************
         * Getters and Setters                                                                                *
         ******************************************************************************************************/

        /// <summary>
        /// ** DEPRECATED **
        /// Sets the API Serial Number, MID, and Password.
        /// </summary>
        /// <param name="Serial">Shift4 Serial number</param>
        /// <param name="MID">Shift4 Merchant ID</param>
        /// <param name="Password">Password</param>
        void setAPISerialMIDPass(string Serial, string MID, string Password);

        /// <summary>
        /// Sets the IP Address for which the UTG / Direct Post endpoint is located. 
        /// </summary>
        /// <param name="address">The IP address of the UTG, 
        /// <para>or</para>
        /// <para>The web address of the direct post endpoint<example>https://www.shift4test.com</example></para>
        /// </param>
        void setIPAddress(string address);

        /// <summary>
        /// Gets the IP Address 
        /// </summary>
        /// <returns>The string representation of the IP Address</returns>
        string getIPAddress();

        /// <summary>
        /// Sets the Port to connect to.
        /// NOTE: The use of the Port number only applies when the IP Address is not of the http form.
        /// </summary>
        /// <param name="port">The port number</param>
        void setPort(string port);

        /// <summary>
        /// Gets the Current Port Number
        /// </summary>
        /// <returns>String Representation of the current port</returns>
        string getPort();

        /// <summary>
        /// Sets the client GUID. This really should not change for production uses, but for a sim it can be changed.
        /// </summary>
        /// <param name="id">clientGUID provided by Shift4</param>
        void setClientGUID(string id);

        /// <summary>
        /// Gets the current clientGUID. This really should not change for production uses, but for a sim it can be changed.
        /// </summary>
        /// <returns>the current clientGUID</returns>
        string getClientGUID();

        /// <summary>
        /// Sets the Auth token that was provided by Shift4. This token needs to be able to be changed in production, so it 
        /// should not be hard coded like the clientGUID should be.
        /// </summary>
        /// <param name="token">The AuthToken for a particular mid</param>
        void setAuthToken(string token);

        /// <summary>
        /// Gets the currently set AuthToken
        /// </summary>
        /// <returns>The AuthToken</returns>
        string getAuthToken();

        /// <summary>
        /// Sets the AccessToken. This token is sent in every request and is acquired by perfroming a token exchange.
        /// <see cref="performTokenExchange()"/>
        /// Failure to get an AccessToken will result in refused transactions to Shift4.
        /// </summary>
        /// <param name="token">The Access Token</param>
        void setAccessToken(string token);

        /// <summary>
        /// Gets the Access Token
        /// </summary>
        /// <returns>The Access Token</returns>
        string getAccessToken();

        /// <summary>
        /// Sets the Merchant Type. In a production environment this is likely already preselected based on their industry,
        /// however for a sim it is helpful in managing what fields get sent to the engine. For example, sending the expected
        /// length of a hotel stay for a food and beverage vendor does not make sense. Thus setting this to the right values, 
        /// will prevent unnecessary parameters being passed to the engine.
        /// </summary>
        /// <param name="type">The Merchant Industry Type</param>
        void setMerchantType(string type);

        /// <summary>
        /// Gets the Industry Type of the current Merchant
        /// </summary>
        /// <returns>Industry Type of Merchant</returns>
        string getMerchantType();
    }
}

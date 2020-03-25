using IDTechSDK;
using System;
using System.Security.Authentication;

namespace FormSim
{
    /// <summary>
    /// The IDTechHandler Class is designed to interface with IDTech Common Kernal devices. Since the IDTech devices have been designed to 
    /// return a fastEMV block upon card read, this class facilitates the startup of the devices, issuing the card read commands, and getting
    /// the returned block of data from the device.
    /// Currently Known Working Devices:
    /// [Augusta, Augusta S, MiniSmart II]
    /// </summary>
    class IDTechHandler
    {
        private string fastEMV;
        public bool minismartII;
        public bool secureMag;
        public bool augusta;
        public string msg;

        /******************************************************************************************************
         * Constructor                                                                                        *
         ******************************************************************************************************/
        public IDTechHandler()
        {
            IDT_Device.startUSBMonitoring();
            IDT_Device.setCallback(MessageCallBack);
            minismartII = false;
            secureMag = false;
        }

        public string FastEMV { get => fastEMV; set => fastEMV = value; }

        /******************************************************************************************************
         * Primary Functions                                                                                  *
         ******************************************************************************************************/
        /// <summary>
        /// Checks to make sure that at least one device is connected to the machine.
        /// </summary>
        /// <returns>True if at least the Augusta OR the (Minismart II AND Securemag) are connected, false otherwise.</returns>
        public bool InitializeDevices()
        {
            bool ret = false;
            minismartII = Profile.deviceIsInitialized(IDT_DEVICE_Types.IDT_DEVICE_MINISMARTII, DEVICE_INTERFACE_Types.DEVICE_INTERFACE_USB);
            secureMag = Profile.deviceIsInitialized(IDT_DEVICE_Types.IDT_DEVICE_SECUREMAG, DEVICE_INTERFACE_Types.DEVICE_INTERFACE_USB);
            augusta = Profile.deviceIsInitialized(IDT_DEVICE_Types.IDT_DEVICE_AUGUSTA, DEVICE_INTERFACE_Types.DEVICE_INTERFACE_USB);
            ret = (minismartII && secureMag) || augusta;
            return ret;
        }
        /// <summary>
        /// Pairs the Minismart II and Securemag devices onto the same controller to act as one.
        /// </summary>
        /// <returns>True if devices successfully paired.</returns>
        public bool pairDevices()
        {
            IDTechSDK.RETURN_CODE code = IDT_Device.SharedController.device_enableSecureHeadForMSII();
            return (code == 0) ? true : false;
        }

        /// <summary>
        /// Starts a transaction on the device by turning on each of the connected sensors so they are ready for swipe or insert.
        /// </summary>
        /// <param name="primaryAmount">The Total of the Transactions</param>
        /// <param name="secondaryAmount">A tip amount if one exists</param>
        /// <returns>True if no errors on sensor start.</returns>
        public string startTransaction(double primaryAmount, double secondaryAmount)
        {
            string ret = "";
            IDTechSDK.RETURN_CODE res = IDT_Device.SharedController.device_startTransaction(primaryAmount, secondaryAmount, 2, 0, 90, null, null, true);
            if (res != RETURN_CODE.RETURN_CODE_DO_SUCCESS)
            {
                ret = "Error starting transaction";
            }
            else
            {
                ret = "Transaction Started";
            }
            return ret;
        }

        /// <summary>
        /// Handles response messages from the SDK.
        /// </summary>
        /// <param name="sender">IDTech Device Type (enum)</param>
        /// <param name="state">The device state</param>
        /// <param name="data">Data associated with the callback</param>
        /// <param name="card">Card information in the callback</param>
        /// <param name="emvCallback">EMV information in the callback</param>
        /// <param name="transactionResultCode">IDTech Return Code from device</param>
        private void MessageCallBack(IDT_DEVICE_Types sender, DeviceState state, byte[] data, IDTTransactionData card, EMV_Callback emvCallback, RETURN_CODE transactionResultCode)
        {
            switch (state)
            {
                case DeviceState.TransactionData:
                    Console.WriteLine("Callback: Transaction Data");
                    if (card.emv_resultCode == EMV_RESULT_CODE.EMV_RESULT_CODE_GO_ONLINE)
                    {
                        // Used in the case of normal EMV transactions, not for FastEMV
                        /*
                        Console.WriteLine(Environment.NewLine + "Authenticating Transaction...");
                        var auth = IDT_Device.SharedController.emv_authenticateTransaction(null);
                        byte[] responseCode = { 0x30, 0x30 };
                        byte[] iad = null;
                        Console.WriteLine(Environment.NewLine + "Completing Transaction");
                        var rt = IDT_Device.SharedController.emv_completeTransaction(false, responseCode, iad, null, null);
                        return;
                        */
                    }
                    Console.WriteLine(Environment.NewLine + Environment.NewLine + "fastEMV: " + card.fastEMV + Environment.NewLine + Environment.NewLine);
                    this.fastEMV = card.fastEMV;
                    break;
                case DeviceState.DataReceived:
                    Console.WriteLine("Callback: Data Received");

                    break;
                case DeviceState.DataSent:
                    Console.WriteLine("Callback: Data Sent");

                    break;
                case DeviceState.CardAction:
                    Console.WriteLine("Callback: Card Action");

                    break;
                case DeviceState.EMVCallback:
                    Console.WriteLine("Callback: EMVCallBack");
                    if (emvCallback.callbackType == EMV_CALLBACK_TYPE.EMV_CALLBACK_TYPE_LCD)
                    {
                        if (emvCallback.lcd_displayMode == EMV_LCD_DISPLAY_MODE.EMV_LCD_DISPLAY_MODE_CLEAR_SCREEN)
                        {
                            Console.WriteLine("LCD Callback: Clearing the Screen");
                        }
                        else if (emvCallback.lcd_displayMode == EMV_LCD_DISPLAY_MODE.EMV_LCD_DISPLAY_MODE_MESSAGE)
                        {
                            Console.WriteLine("LCD Callback: Display Message");
                            msg = getDisplayMessage(emvCallback.lcd_messages[1]);
                        }
                        else
                        {
                            //Display message with menu/language or prompt, and return result to emv_callbackResponseLCD
                            //Kernel will not proceed until this step is complete
                            //seeing to default value of 1.  See other test app included with SDK for a complete example
                            Console.WriteLine("LCD Callback: Menu Display Request. Sending result 1");
                            IDT_Device.SharedController.emv_callbackResponseLCD(emvCallback.lcd_displayMode, 1);
                        }
                    }
                    break;
                default:
                    Console.WriteLine($"Callback: (default): {state}");
                    break;
            }
        }

        /// <summary>
        /// Gets the encryption information for a given device.
        /// </summary>
        /// <returns>EncryptionInfo.</returns>
        /// <exception cref="Exception">
        /// </exception>
        /// <remarks>
        /// A device must be first set active so that the SDK knows which device to send the command to.
        /// </remarks>
        public EncryptionInfo getEncryptionInfo()
        {
            var result = new EncryptionInfo();

            byte format = 0;
            var rt = IDT_Device.SharedController.icc_getKeyFormatForICCDUKPT(ref format);

            if (rt != RETURN_CODE.RETURN_CODE_DO_SUCCESS)
            {
                throw new Exception($"Error retrieving encryption format.{GetFormattedErrorMessage(rt)}");
            }

            switch (format)
            {
                case 0x00:
                    result.DukptFormatType = CipherAlgorithmType.TripleDes;
                    break;
                case 0x01:
                    result.DukptFormatType = CipherAlgorithmType.Aes;
                    break;
                default:
                    result.DukptFormatType = CipherAlgorithmType.None;
                    break;
            }

            byte type = 0;
            rt = IDT_Device.SharedController.icc_getKeyTypeForICCDUKPT(ref type);

            if (rt != RETURN_CODE.RETURN_CODE_DO_SUCCESS)
            {
                throw new Exception($"Error retrieving encryption format.{GetFormattedErrorMessage(rt)}");
            }
            switch (type)
            {
                case 0x00:
                    result.DukptKeyType = "Data";
                    break;
                case 0x01:
                    result.DukptKeyType = "PIN";
                    break;
                default:
                    result.DukptKeyType = "None";
                    break;
            }

            return result;
        }

        /// <summary>
        /// Gets the formatted error message for a given RETURN_CODE.
        /// </summary>
        /// <param name="returnCode">The return code.</param>
        /// <returns>Return Code (enum string)</returns>
        private static string GetFormattedErrorMessage(RETURN_CODE returnCode)
        {
            return "Error code: " + "0x" + $"{(ushort)returnCode:X}" + ": " + errorCode.getErrorString(returnCode);
        }

        private string getDisplayMessage(int id)
        {
            switch (id)
            {
                case 0: return "";
                case 1: return "AMOUNT";
                case 2: return "AMOUNT OK ? ";
                case 3: return "APPROVED";
                case 4: return "CALL YOUR BANK";
                case 5: return "CANCEL OR ENTER";
                case 6: return "CARD ERROR";
                case 7: return "DECLINED";
                case 8: return "ENTER AMOUNT";
                case 9: return "ENTER PIN:";
                case 10: return "INCORRECT PIN";
                case 11: return "SWIPE OR INSERT";
                case 12: return "CARD";
                case 13: return "INSERT CARD";
                case 14: return "USE CHIP READER";
                case 15: return "NOT ACCEPTED";
                case 16: return "GET PIN OK";
                case 17: return "PLEASE WAIT...";
                case 18: return "PROCESSING ERROR";
                case 19: return "USE MAGSTRIPE";
                case 20: return "TRY AGAIN";
                case 21: return "GO ONLINE";
                case 22: return "TRANSACTION ERR";
                case 23: return "TERMINATE";
                case 24: return "ADVICE";
                case 25: return "TIME OUT";
                case 26: return "PROCESSING...";
                case 27: return "PIN TRY LIMIT EX";
                case 28: return "ISSUER AUTH FAIL";
                case 29: return "CONTINUE PROCESS";
                case 30: return "GET PIN ERROR";
                case 31: return "GET PIN FAIL";
                case 32: return "NO KEY GET PIN";
                case 33: return "CANCELLED";
                case 34: return "LAST PIN TRY";
                case 66: return "TRY ICC AGAIN";
            }
            return "";
        }
    }
}

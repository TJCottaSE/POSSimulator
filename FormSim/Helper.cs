using System;
using System.Linq;
using System.Collections.Generic;

namespace FormSim
{
    /// <summary>
    /// This is a helper class designed to do some parsing and padding.
    /// It was designed to reduce code re-use across both the HTTPHandler
    /// and TCPHandler classes. Due to the shared nature of this class,
    /// any changes made here should be tested with both the TCP and HTTP
    /// handlers after an update. 
    /// </summary>
    public sealed class Helper
    {
        private static readonly Helper instance = new Helper();

        /// <summary>
        /// Private Constructor
        /// </summary>
        private Helper() { }

        /// <summary>
        /// Get instance method of this SINGLETON class
        /// </summary>
        public static Helper getInstance
        {
            get
            {
                return instance;
            }
        }

        /// <summary>
        /// Strips the given parameter from the list of APIOptions contained within the parameters dictionary.
        /// </summary>
        /// <param name="parameters">The Dictionary of all the parameters for an FRC.</param>
        /// <param name="toBeStripped">The API Option to be removed from the list of options.</param>
        /// <returns>A Dictionary of all the parameters for an FRC minus the API Option that was removed.</returns>
        public Dictionary<string, string> stripAPIParameters(Dictionary<string, string> parameters, string toBeStripped)
        {
            // Strip the APIOption given
            int index = parameters["APIOptions"].IndexOf(toBeStripped);
            bool csv = (parameters["APIOptions"].IndexOf(toBeStripped + ",") != -1);
            if (index != -1 && csv)
            {
                parameters["APIOptions"] = parameters["APIOptions"].Remove(index, toBeStripped.Length + 1);
            }
            else if (index != -1)
            {
                parameters["APIOptions"] = parameters["APIOptions"].Remove(index, toBeStripped.Length);
            }
            // Strip the trailing ","
            if (parameters["APIOptions"].ElementAt<char>(parameters["APIOptions"].Length - 1) == ',')
            {
                parameters["APIOptions"] = parameters["APIOptions"].Remove(parameters["APIOptions"].Length - 1);
            }
            return parameters;
        }

        /// <summary>
        /// Strips a list of APIOptions from the list of APIOptions contained within the parameters dictionary.
        /// </summary>
        /// <param name="parameters">The Dictionary of all the parameters for an FRC.</param>
        /// <param name="toBeStripped">The API Option list to be removed from the list of options.</param>
        /// <returns>A Dictionary of all the parameters for an FRC minus the API Options that were removed.</returns>
        public Dictionary<string, string> stripAPIParameters(Dictionary<string, string> parameters, string[] toBeStripped)
        {
            foreach (string param in toBeStripped)
            {
                parameters = stripAPIParameters(parameters, param);
            }
            return parameters;
        }

        /// <summary>
        /// Parses out a Dictionary of strings into a user readable string that can be used for printing or logging.
        /// </summary>
        /// <param name="dict">The Dictionary to be parsed.</param>
        /// <returns>String representation of the contents of the Dictionary.</returns>
        public string parseDict(Dictionary<string, string> dict)
        {
            var asString = string.Join(Environment.NewLine, dict);
            asString = asString.Replace('[', ' ');
            asString = asString.Replace(']', ' ');
            asString = asString.Replace(',', ':');
            return asString;
        }

        /// <summary>
        /// Gets the current date, and returns it in the proper format.
        /// </summary>
        /// <returns>Returns todays date in MMDDYY format.</returns>
        public string getDate()
        {
            DateTime localDateTime = DateTime.Now;
            string temp = datePad(localDateTime.Month.ToString());
            temp += datePad(localDateTime.Day.ToString());
            char[] year = localDateTime.Year.ToString().ToCharArray();
            temp += year[2].ToString() + year[3].ToString();
            return temp;
        }

        /// <summary>
        /// Pads the numerical date with a prepended 0 if not already in two digit format.
        /// Only used in <see cref="Helper.getDate()"/>.
        /// </summary>
        /// <param name="value">String representation of the date.</param>
        /// <returns>A string representation of the date with preceeding 0's for months and days below 10.</returns>
        private string datePad(string value)
        {
            string padded = "0";
            if (value.Length == 1)
            {
                return padded += value;
            }
            else { return value; }
        }

        /// <summary>
        /// Gets the current time, and returns it in proper format.
        /// </summary>
        /// <returns>The current time returned in HHMMSS format.</returns>
        public string getTime()
        {
            DateTime localDateTime = DateTime.Now;
            string temp = localDateTime.ToString("HH:mm:ss");
            temp = temp.Remove(5, 1);
            temp = temp.Remove(2, 1);
            return temp;
        }
    }
}


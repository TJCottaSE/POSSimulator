using System;
using System.IO;

namespace FormSim
{
    /// <summary>
    /// The LogWriter class is used as a logging device. The singleton provides a single
    /// point of logging to all threads which can be useful in debugging, as not all 
    /// intermediate results will be returned to the UI when using the basic transaction
    /// flow. Current output path is c:/Users/tcotta.shift4/Desktop/FormSimLog.log.
    /// </summary>
    public sealed class LogWriter
    {
        private static readonly LogWriter instance = new LogWriter();
        private string path;

        /// <summary>
        /// Constructor that instantiates the Log Writer.
        /// </summary>
        private LogWriter()
        {
            string folderName = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string logFileName = "FormSimLog.log";
            path = System.IO.Path.Combine(folderName, logFileName);
        }

        /// <summary>
        /// Gets a handle to the LogWriter.
        /// </summary>
        public static LogWriter getInstance
        {
            get
            {
                return instance;
            }
        }

        /// <summary>
        /// Writes a string to the log.
        /// </summary>
        /// <param name="value">The string to be written.</param>
        public void Write(string value)
        {
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine(value + "\n");
            }
        }

        /// <summary>
        /// Writes a character to the log.
        /// </summary>
        /// <param name="value">The character to be written.</param>
        public void Write(char value)
        {
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.Write(value);
            }
        }

        /// <summary>
        /// Writes a byte to the log.
        /// </summary>
        /// <param name="value">The byte to be written.</param>
        public void Write(byte value)
        {
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.Write(value);
            }
        }
    }
}

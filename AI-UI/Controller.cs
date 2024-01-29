using System.Windows;

namespace AI_UI{
    /// <summary>
    /// Helper Class
    /// </summary>
    public static class Controller {

        static LogWindow log;

        /// <summary>
        /// Set Log Window
        /// </summary>
        /// <param name="_log">LogWindow that will be used</param>
        public static void SetLog(LogWindow _log) {
            log = _log;
        }

        /// <summary>
        /// Write a message to the log window
        /// </summary>
        /// <param name="output">Message to write to log</param>
        public static void WriteToLog(string output) {
            Application.Current.Dispatcher.Invoke(() => log.WriteToLog(output));
        }

    }
}

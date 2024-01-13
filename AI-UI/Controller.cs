using System.Windows;

namespace AI_UI{
    public static class Controller {

        static LogWindow log;

        /// <summary>
        /// Set Log
        /// </summary>
        /// <param name="_log">LogWindow that will be used</param>
        public static void SetLog(LogWindow _log) {
            log = _log;
        }

        public static void WriteToLog(string output) {
            Application.Current.Dispatcher.Invoke(() => log.WriteToLog(output));
        }

    }
}

using System.Windows;

namespace AI_UI
{
    /// <summary>
    /// Interaction logic for LogWindow.xaml
    /// </summary>
    public partial class LogWindow : Window
    {
        public LogWindow() {
            InitializeComponent();

            LogBlock.Text = "Log Initialized";
        }

        public void WriteToLog(string output) {
            LogBlock.Text = output + "\n" + LogBlock.Text;
        }

        //TODO: Save Log On Exit

    }
}

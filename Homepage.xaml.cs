using System.Windows;
using System.Windows.Controls;

namespace Form1
{
    /// <summary>
    /// Interaction logic for homepage.xaml
    /// </summary>
    public partial class Homepage : Page
    {
        public Homepage()
        {
            InitializeComponent();
        }

        private void GoToApplication(object sender, RoutedEventArgs e)
        {
            var btn = e.Source as Button;
            Window newWindow = null;

            switch (btn.Content.ToString())
            {
                case "EDIT":
                    newWindow = new Edit();
                    break;
                case "BROWSER":
                    newWindow = new Browser();
                    break;
                case "LOGTEMPO":
                    newWindow = new LogTempo();
                    break;
                case "LOG":
                    newWindow = new Log();
                    break;
            }

            newWindow.Show();
        }
    }
}

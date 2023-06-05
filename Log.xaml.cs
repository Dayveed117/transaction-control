using System.Data;
using System.Windows;
using System.Windows.Controls;
using Form1.Utils;


namespace Form1
{
    /// <summary>
    /// Interaction logic for Log.xaml
    /// </summary>
    public partial class Log : Window
    {
        private OurTimer timer;
        private BDhelper bdhelper;

        public Log()
        {
            InitializeComponent();
            
            //  Primeiro RadioButton como Default
            bdhelper = new BDhelper(field_r1);

            DataTable dt = bdhelper.GetLogTableNlines(field_Nrows.Text);
            tabelaLog.ItemsSource = dt.DefaultView;

            timer = new OurTimer(10, referenceTimer, tabelaLog, bdhelper.GetLogTableNlines, field_Nrows.Text);

            timer.StartTimer();
        }

        private void nrowsBtn_Click(object sender, RoutedEventArgs e)
        {
            string n = field_Nrows.Text;

            if (!n.Equals(string.Empty) && int.TryParse(n, out _))
            {
                timer.UpdateLines(n);
                timer.RefreshTimer();
                timer.UpdateDataGrid();
            }
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            var radioButton = sender as RadioButton;
            bdhelper.RadioBtn = radioButton;
        }
    }
}

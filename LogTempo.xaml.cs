using System.Data;
using System.Windows;
using System.Windows.Controls;
using Form1.Utils;

namespace Form1
{
    /// <summary>
    /// Interaction logic for LogTempo.xaml
    /// </summary>
    public partial class LogTempo : Window
    {

        private BDhelper bdhelper;

        public LogTempo()
        {
            InitializeComponent();

            //  Primeiro RadioButton como Default
            bdhelper = new BDhelper(field_r1);

            DataTable dt = bdhelper.GetLogTempoTable(string.Empty);
            MyDataGrid.ItemsSource = dt.DefaultView;
           
            var timer = new OurTimer(10, labelTimer, MyDataGrid, bdhelper.GetLogTempoTable, string.Empty);
            
            timer.StartTimer();
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            var radioButton = sender as RadioButton;
            bdhelper.RadioBtn = radioButton;
        }


    }
}

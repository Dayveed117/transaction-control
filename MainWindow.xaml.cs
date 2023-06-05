using System;
using System.Data.SqlClient;
using System.Windows;
using Form1.Utils;

namespace Form1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //  Default field values
            field_dbname.Text = "MEI_TRAB";
            field_hostname.Text = Environment.MachineName;
            field_username.Text = "sa";
        }

        private void ConnectToDB(object sender, RoutedEventArgs e)
        {
            string dbname = field_dbname.Text;
            string hostname = field_hostname.Text;
            string username = field_username.Text;
            string pass = field_pass.Password;

            //  Conector string
            Vars.connectionString = string.Format
                ("Data Source={0};" +
                "Initial Catalog={1};" +
                "User ID={2};" +
                "Password={3};" +
                "MultipleActiveResultSets=true", hostname, dbname, username, pass);

            using (SqlConnection connection = new SqlConnection(Vars.connectionString))
            {
                try
                {
                    connection.Open();
                    Content = new Homepage();
                }
                catch (Exception)
                {
                    MessageBox.Show("Failed to connect!");
                }

            }            
        }
    }
}

using System;
using System.Data;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

using Form1.Utils;

namespace Form1
{
    /// <summary>
    /// Interaction logic for Browser.xaml
    /// </summary>
    public partial class Browser : Window
    {
        private Int32 startAt = 5;
        private Int32 index;
        private DataTable dtEncomenda, dtEncLinha;
        private int clickedRowIndex = 0;
        private BDhelper bdhelper;

        public Browser()
        {
            InitializeComponent();

            //  Primeiro RadioButton como Default
            bdhelper = new BDhelper(field_r1);

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            index = startAt;

            timer.Tick += (s, e) =>
            {
                lbTime.Content = index.ToString();
                index--; // increment index

                // Stop if this event has been raised max number of times
                if (index == -1)
                {
                    index = startAt;
                    fillTable();
                }
            };
            timer.Start();

            fillTable();
        }

        public void fillTable()
        {
            try
            {
                bdhelper.BeginOurTransaction();

                dtEncomenda = bdhelper.GetEncomendaTable();

                if (dtEncomenda == null)
                {
                    MessageBox.Show("Algo completamente esperado aconteceu, tente mais tarde.");
                    bdhelper.OurRollback(false);
                }
                else
                {
                    string encid = dtEncomenda.Rows[clickedRowIndex].Field<int>("EncID").ToString();
                    dtEncLinha = bdhelper.GetEncLinhaTableFromId(encid);

                    bdhelper.OurCommit(false);

                    encomendaTab.ItemsSource = dtEncomenda.DefaultView;
                    encLinhaTab.ItemsSource = dtEncLinha.DefaultView;
                }
                
            }

            catch (Exception)
            {
                MessageBox.Show("Algo completamente esperado aconteceu, tente mais tarde.");
                bdhelper.OurRollback(false);
            }
        }

        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;

            if (row == null)
                return;

            clickedRowIndex = row.GetIndex();

            //Atribui valor ao EncId com DoubleClick
            try
            {
                bdhelper.BeginOurTransaction();

                string encid = dtEncomenda.Rows[clickedRowIndex].Field<int>("EncID").ToString();
                dtEncLinha = bdhelper.GetEncLinhaTableFromId(encid);

                if (dtEncLinha == null)
                {
                    MessageBox.Show("Algo completamente esperado aconteceu, tenta mais tarde.");
                    bdhelper.OurRollback(false);
                }
                else
                {
                    bdhelper.OurCommit(false);
                    encLinhaTab.ItemsSource = dtEncLinha.DefaultView;
                }
                
            }
            catch (Exception)
            {
                MessageBox.Show("Algo completamente esperado aconteceu, tente mais tarde.");
                bdhelper.OurRollback(false);
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            index = startAt;
            fillTable();
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            var radioButton = sender as RadioButton;
            bdhelper.RadioBtn = radioButton;
        }
    }
}

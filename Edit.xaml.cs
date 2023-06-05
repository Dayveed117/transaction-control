using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Windows;
using System.Windows.Controls;

using Form1.Utils;

namespace Form1
{
    /// <summary>
    /// Interaction logic for Edit.xaml
    /// </summary>
    public partial class Edit : Window
    {
        private string encid;
        private string firstReference;
        private BDhelper bdhelper;
        private DataTable dt;
        private bool firstchange = true;
        List<(int, string)> changestable;
        private bool moradaChanged = false;
        private bool hasSearched = false;

        public Edit()
        {
            InitializeComponent();

            //  Primeiro RadioButton como Default
            bdhelper = new BDhelper(field_r1);

            saveBtn.IsEnabled = false;
            disBtn.IsEnabled = false;
            editBtn.IsEnabled = false;

            changestable = new List<(int, string)>();

            tabela.CellEditEnding += myDG_CellEditEnding;
        }

        void myDG_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                if (e.Column is DataGridBoundColumn column)
                {

                    int rowIndex = e.Row.GetIndex();
                    var qtd = column.GetCellContent(e.Row) as TextBox;

                    try
                    {
                        SqlDecimal decim = SqlDecimal.Parse(qtd.Text);
                        int prodId = dt.Rows[rowIndex].Field<int>("ProdutoID");

                        changestable.Add((prodId, "" + decim));

                        if (!saveBtn.IsEnabled)
                        {
                            saveBtn.IsEnabled = true;
                            disBtn.IsEnabled = true;

                        }
                    }
                    catch(FormatException)
                    {
                        MessageBox.Show(qtd.Text + " não está em um formato correto");
                        qtd.Text = "";
                    }

                }
            }
        }

        public void RefreshTable()
        {
            string timeStamp = Vars.GetTimestamp(DateTime.Now);
            encid = field_encid.Text;
            saveBtn.IsEnabled = false;
            disBtn.IsEnabled = false;
            editBtn.IsEnabled = false;
            firstchange = true;
            moradaChanged = false;
            changestable = new List<(int, string)>();


            if (!encid.Equals(string.Empty) && int.TryParse(encid, out _))
            {
                //  Criar primeira referencia para posterior utilização ao acabar de editar
                firstReference = string.Format("G10-{0}", timeStamp);
                
                bdhelper.BeginOurTransaction();
                hasSearched = true;


                //  Inserir na tabela LogOperations primeiro registo
                int check = bdhelper.InsertInLogOperations(
                    (encid, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), firstReference));

                if (check < 1)
                    MessageBox.Show("Erro ao inserir.");

                dt = bdhelper.GetEncLinhaTableFromId(encid);

                if (dt == null)
                {
                    ClearFields();
                    bdhelper.OurRollback(false);
                    MessageBox.Show("Não foi possível preencher a tabela.");
                }
                else
                {
                    dt.Columns[0].ReadOnly = true;
                    dt.Columns[1].ReadOnly = true;
                    dt.Columns[2].ReadOnly = true;
                    dt.Columns[3].ReadOnly = true;
                    dt.Columns[4].ReadOnly = false;

                    if (dt.Rows.Count > 0)
                    {
                        FillFields();
                        bdhelper.OurCommit(false);
                    }

                    else
                    {
                        ClearFields();
                        bdhelper.OurRollback(false);
                    }
                }

                
            }
        }

        private void FillFields()
        {
            field_morada.Text = bdhelper.GetMoradaFromId(encid);
            field_morada.IsEnabled = true;
            tabela.ItemsSource = dt.DefaultView;
        }

        private void ClearFields()
        {
            field_morada.Text = "";
            field_morada.IsEnabled = false;
            tabela.ItemsSource = null;
            saveBtn.IsEnabled = false;
            disBtn.IsEnabled = false;
        }

        private void searchBtn_Click(object sender, RoutedEventArgs e)
        {
            RefreshTable();
        }

        private void editBtn_Click(object sender, RoutedEventArgs e)
        {
            moradaChanged = true;
            editBtn.IsEnabled = false;

            if (!saveBtn.IsEnabled)
            {
                saveBtn.IsEnabled = true;
                disBtn.IsEnabled = true;
            }

        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {

            bdhelper.BeginOurTransaction();

            foreach ((int, string) pair in changestable)
            {
                // MessageBox.Show(pair.Item1 + "\n" + pair.Item2);
                bdhelper.UpdateQTD(encid, pair.Item1, pair.Item2);
            }

            changestable = new List<(int, string)>();

            if (moradaChanged)
            {
                string morada = field_morada.Text;

                int check1 = bdhelper.UpdateMorada(encid, morada);
                if (check1 < 1)
                    MessageBox.Show("Erro ao alterar");
            }

            int check = bdhelper.InsertInLogOperations(
                    (encid, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), firstReference));

            if (check < 1)
                MessageBox.Show("Erro ao inserir");


            bdhelper.OurCommit(false);

            saveBtn.IsEnabled = false;
            disBtn.IsEnabled = false;
            hasSearched = false;

            ClearFields();
        }

        private void disBtn_Click(object sender, RoutedEventArgs e)
        {
            hasSearched = false;
            ClearFields();
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {

            if (!firstchange)
            {
                if (field_morada.Text.Equals(string.Empty))
                {
                    editBtn.IsEnabled = false;
                    moradaChanged = false;
                    return;

                }
                editBtn.IsEnabled = true;
                

            }
            firstchange = false;

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            if (hasSearched)
            {
                bdhelper.BeginOurTransaction();

                int check = bdhelper.InsertInLogOperations(
                                 (encid, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), firstReference));

                bdhelper.OurCommit(false);

            if (check < 1)
                MessageBox.Show("Erro ao inserir");
            }

            
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            var radioButton = sender as RadioButton;
            bdhelper.RadioBtn = radioButton;
        }
    }
}

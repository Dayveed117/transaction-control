using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;


namespace Form1.Utils
{
    partial class BDhelper
    {

        public SqlConnection connection;
        public SqlTransaction transaction;
        public SqlCommand command;
        public RadioButton RadioBtn { get; set; }

        public BDhelper(RadioButton RadioBtn)
        {
            this.RadioBtn = RadioBtn;
            OpenConnection();
        }

        public void BeginOurTransaction()
        {
            try
            {
                switch (RadioBtn.Content)
                {
                    case "Read Uncommitted":
                        transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted);
                        break;
                    case "Read Committed":
                        transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                        break;
                    case "Repeatable Read":
                        transaction = connection.BeginTransaction(IsolationLevel.RepeatableRead);
                        break;
                    case "Serializable":
                        transaction = connection.BeginTransaction(IsolationLevel.Serializable);
                        break;
                    default:
                        transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted);
                        break;
                }
                EnableCommandTransaction();
            }
            catch (Exception)
            {
                OurRollback(false);
            }
        }

        private int ExecuteQuery(List<(string, string)> paramAndArg)
        {
            if (paramAndArg != null || paramAndArg.Count > 0)
            {
                foreach ((string, string) pair in paramAndArg)
                {
                    // MessageBox.Show(pair.Item1 + "\n" + pair.Item2);
                    command.Parameters.AddWithValue(pair.Item1, pair.Item2);
                }
                paramAndArg.Clear();
                
                try
                {
                    int check = command.ExecuteNonQuery();
                    command.Parameters.Clear();
                    return check;
                }
                catch(SqlException e)
                {
                    MessageBox.Show("Conflito com outras transações\n" + e);
                    OurRollback(false);
                }
            }

            return -1;
        }

        public DataTable Filldt(SqlDataAdapter da, DataTable dt)
        {
            try
            {
                da.Fill(dt);
                return dt;
            }
            catch (Exception e)
            {
                MessageBox.Show("Unable to fill table :\n" + e);
            }

            return null;
        }

        public void OurCommit(bool refresh)
        {
            try
            {
                transaction.Commit();

                if (refresh)
                    BeginOurTransaction();
            }
            catch (Exception e)
            {
                MessageBox.Show("Exception Type : "
                    + e.GetType() + "\n" + e.Message);
            }
        }

        public void OurRollback(bool refresh)
        {
            try
            {
                transaction.Rollback();
                ResetConnection();

                if (refresh)
                {
                    BeginOurTransaction();
                }

            }
            catch (Exception e)
            {
                MessageBox.Show("Rollback Exception Type : " 
                    + e.GetType() + "\n" + e.Message);
            }
        }

        private void EnableCommandTransaction()
        {
            command.Connection = connection;
            command.Transaction = transaction;
        }

        private void OpenConnection()
        {
            connection = new SqlConnection(Vars.connectionString);
            connection.Open();
            command = connection.CreateCommand();
        }

        private void BreakConnection()
        {
            command.Cancel();
            transaction.Dispose();
            connection.Close();
        }

        private void ResetConnection()
        {
            switch (connection.State)
            {
                case ConnectionState.Open:
                    BreakConnection();
                    OpenConnection();
                    EnableCommandTransaction();
                    break;
                case ConnectionState.Closed:
                    OpenConnection();
                    EnableCommandTransaction();
                    break;
                default:
                    BreakConnection();
                    break;
            }
        }

    }
}

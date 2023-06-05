using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Form1.Utils
{
    partial class BDhelper
    {
        // Browser
        public DataTable GetEncomendaTable()
        {
            command.CommandText = "SELECT * FROM Encomenda ORDER BY EncId DESC";

            var da = new SqlDataAdapter(command);
            var dt = new DataTable("Encomenda");

            return Filldt(da, dt);
        }

        // Browser e Edit
        public DataTable GetEncLinhaTableFromId(string EncId)
        {
            command.CommandText = "SELECT * FROM EncLinha WHERE EncId = " + EncId;

            if (!EncId.Equals(string.Empty))
            {
                var da = new SqlDataAdapter(command);
                var dt = new DataTable("EncLinha");
                
                return Filldt(da, dt);
            }
            
            return null;
        }

        // LogTempo
        public DataTable GetLogTempoTable(string _)
        {
            BeginOurTransaction();

            command.CommandText = "SELECT LO1.UserID, LO1.Objecto AS EncId, DATEDIFF(SS, LO1.Valor, LO2.Valor) AS Tempo " +
                "FROM LogOperations LO1, LogOperations LO2 " +
                "WHERE LO1.Referencia = LO2.Referencia AND LO1.EventType='O' AND LO2.EventType='O' AND LO1.DCriacao < LO2.DCriacao";

            var da = new SqlDataAdapter(command);

            OurCommit(false);

            var dt = new DataTable("LogOperations");
            
            return Filldt(da, dt);
        }

        // Log
        public DataTable GetLogTableNlines(string n)
        {
            BeginOurTransaction();

            command.CommandText = string.Format("SELECT TOP {0} * FROM LogOperations WHERE " +
                    "EventType = 'I' OR EventType = 'U' OR EventType = 'D' " +
                    "ORDER BY NumReg DESC", n);

            var da = new SqlDataAdapter(command);

            OurCommit(false);

            var dt = new DataTable("Log");
            
            return Filldt(da, dt);
        }

         
        public string GetMoradaFromId(string encid)
        {
            command.CommandText = "SELECT Morada FROM Encomenda WHERE EncId = " + encid;

            var reader = command.ExecuteReader();

            if (reader.Read())
            {
                string morada = reader[0] as string;
                reader.Close();

                return morada;
            }
            return string.Empty;
        }


        public int InsertInLogOperations((string, string, string) insertArgs)
        {

            /* Colunas em LogOperations
             * 
             * NumRegisto   -> automatico autoincr
             * EventType    -> char
             * Objecto      -> string/int
             * Valor        -> datetime?
             * Referencia   -> string
             * UserId       -> automatico string
             * TerminalId   -> automatico string
             * TerminalName -> automatico string
             * DCriacao     -> automatico string
            */

            command.CommandText =
                "INSERT INTO LogOperations (EventType, Objecto, Valor, Referencia) " +
                "VALUES('O', @Objecto, @Valor, @Referencia)";

            List<(string, string)> paramAndArg = new List<(string, string)>
            {
                ("@Objecto",    insertArgs.Item1),
                ("@Valor",      insertArgs.Item2),
                ("@Referencia", insertArgs.Item3)
            };

            int check = ExecuteQuery(paramAndArg);

            return check;
        }


        public int UpdateMorada(string EncId, string morada)
        {
            if (!morada.Equals(string.Empty))
            {
                command.CommandText = "UPDATE Encomenda SET Morada = @Morada WHERE EncId = @EncId";

                List<(string, string)> paramAndArg = new List<(string, string)>
                {
                    ("@Morada", morada),
                    ("@EncId", EncId)
                };

                return ExecuteQuery(paramAndArg);
            }

            return -1;
        }


        public int UpdateQTD(string EncId, int ProdId, string qtd)
        {
            if (!qtd.Equals(string.Empty))
            {
                command.CommandText = "UPDATE EncLinha SET Qtd = @Qtd WHERE EncId = @EncId and ProdutoID = @ProdutoID";

                List<(string, string)> paramAndArg = new List<(string, string)>
                {
                    ("@EncId", EncId),
                    ("@ProdutoID", ProdId.ToString()),
                    ("@Qtd", qtd),
                };

                return ExecuteQuery(paramAndArg);
            }

            return -1;
        }
    }
}

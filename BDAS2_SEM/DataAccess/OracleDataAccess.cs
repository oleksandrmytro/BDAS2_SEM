using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Configuration;
using BDAS2_SEM.Model;

namespace BDAS2_SEM.DataAccess
{
    public class OracleDataAccess
    {
        private readonly string connectionString;

        public OracleDataAccess()
        {
            // Отримуємо рядок підключення з App.config
            connectionString = ConfigurationManager.ConnectionStrings["OracleDbConnection"].ConnectionString;
        }

        // Виконує запит SELECT і повертає результати як DataTable
        public DataTable ExecuteQuery(string query, params OracleParameter[] parameters)
        {
            DataTable result = new DataTable();

            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                using (OracleCommand command = new OracleCommand(query, connection))
                {
                    if (parameters != null && parameters.Length > 0)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                    {
                        adapter.Fill(result);
                    }
                }
            }

            return result;
        }

        // Виконує запит INSERT, UPDATE, DELETE і повертає кількість заторкнутих рядків
        public int ExecuteNonQuery(string query, params OracleParameter[] parameters)
        {
            int affectedRows = 0;

            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                using (OracleCommand command = new OracleCommand(query, connection))
                {
                    if (parameters != null && parameters.Length > 0)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    affectedRows = command.ExecuteNonQuery();
                }
            }

            return affectedRows;
        }

        // Метод для вставки користувача
        public int InsertUzivatel(UZIVATEL_DATA uzivatel)
        {
            string insertQuery = "INSERT INTO UZIVATEL_DATA (EMAIL, PASSWORD) VALUES (:email, :password)";
            OracleParameter[] parameters = new OracleParameter[]
            {
                new OracleParameter("email", uzivatel.Email),
                new OracleParameter("password", uzivatel.Heslo)
            };

            return ExecuteNonQuery(insertQuery, parameters);
        }
    }
}

using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Configuration;
using BDAS2_SEM.Model;
using BDAS2_SEM.Model.Enum;

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
            string insertQuery = @"
        INSERT INTO UZIVATEL_DATA 
            (id_user_data, email, heslo, role, pacient_id_c, zamestnanec_id_c) 
        VALUES 
            (user_data_seq.NEXTVAL, :email, :password, :role, :pacient_id, :zamestnanec_id)";

            OracleParameter[] parameters = new OracleParameter[]
            {
                new OracleParameter("email", uzivatel.Email),
                new OracleParameter("password", uzivatel.Heslo),
                new OracleParameter("role", RoleService.GetRoleName(Role.NEOVERENY)),
                new OracleParameter("pacient_id", (object)uzivatel.pacientId ?? DBNull.Value),
                new OracleParameter("zamestnanec_id", (object)uzivatel.zamestnanecId ?? DBNull.Value)
            };

            return ExecuteNonQuery(insertQuery, parameters);
        }

    }
}

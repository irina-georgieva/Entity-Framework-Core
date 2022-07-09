using System;
using System.Data.SqlClient;
using System.Text;
using System.Xml;
using ADO.NET_Exercise;

namespace P09_Increase_Age_Stored_Procedure
{
    class Program
    {
        static void Main(string[] args)
        {
            int minionId = int.Parse(Console.ReadLine());

            using SqlConnection sqlConnection
                = new SqlConnection(Config.ConnectionString);

            sqlConnection.Open();
            string result = IncreaseMinionAge(sqlConnection, minionId);
            Console.WriteLine(result);
            sqlConnection.Close();
        }

        private static string IncreaseMinionAge(SqlConnection sqlConnection, int minionId)
        {
            StringBuilder output = new StringBuilder();

            string increaseAgeQuery = @"EXEC [dbo].[usp_GetOlder] @MinionId";
            SqlCommand increaseAgeCommand = new SqlCommand(increaseAgeQuery, sqlConnection);
            increaseAgeCommand.Parameters.AddWithValue("@MinionId", minionId);

            increaseAgeCommand.ExecuteNonQuery();

            string minionInfoQuery = @"SELECT [Name], [Age]
	                                     FROM [Minions]
	                                    WHERE [Id] = @MinionId";
            SqlCommand minionInfoCOmmand = new SqlCommand(minionInfoQuery, sqlConnection);
            minionInfoCOmmand.Parameters.AddWithValue("@MinionId", minionId);

            using SqlDataReader infoReader = minionInfoCOmmand.ExecuteReader();
            while (infoReader.Read())
            {
                output.AppendLine($"{infoReader["Name"]} - {infoReader["Age"]} years old");
            }

            return output.ToString().TrimEnd();
        }
    }
}

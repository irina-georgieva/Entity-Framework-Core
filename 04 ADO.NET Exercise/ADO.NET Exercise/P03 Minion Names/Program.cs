using System;
using System.Data.SqlClient;
using System.Text;
using ADO.NET_Exercise;

namespace P03_Minion_Names
{
    class Program
    {
        static void Main(string[] args)
        {
            int villainId = int.Parse(Console.ReadLine());
            using SqlConnection sqlConnection
                = new SqlConnection(Config.ConnectionString);

            sqlConnection.Open();
            string result = GetVillainWithMinions(sqlConnection, villainId);
            Console.WriteLine(result);
            sqlConnection.Close();
        }

        private static string GetVillainWithMinions(SqlConnection sqlConnection, int villainId)
        {
            StringBuilder output = new StringBuilder();

            string villaingNameQuery = @"SELECT [Name]
	                                       FROM [Villains]
	                                       WHERE [Id] = @VillainId";

            SqlCommand getVillainNameCmd = new SqlCommand(villaingNameQuery, sqlConnection);
            getVillainNameCmd.Parameters.AddWithValue("@VillainId", villainId);

            string villainName = (string)getVillainNameCmd.ExecuteScalar();
            if (villainName == null)
            {
                return $"No villain with ID {villainId} exists in the database.";
            }

            output.AppendLine($"Villain: {villainName}");

            string minionsQuery = @"SELECT [m].[Name], [m].[Age]
	                                   FROM [MinionsVillains] AS [mv]
	                              LEFT JOIN [Minions] AS [m]
	                                     ON [m].[Id] = [mv].[MinionId]
	                                  WHERE [mv].[VillainId] = @VillainId
	                               ORDER BY [m].[Name]";

            SqlCommand getMinionsCommand = new SqlCommand(minionsQuery, sqlConnection);
            getMinionsCommand.Parameters.AddWithValue("@VillainId", villainId);

            using SqlDataReader minionReader = getMinionsCommand.ExecuteReader();
            if (!minionReader.HasRows)
            {
                output.AppendLine($"(no minions)");
            }
            else
            {
                int rowNum = 1;
                while (minionReader.Read())
                {
                    output.AppendLine($"{rowNum}. {minionReader["Name"]} {minionReader["Age"]}");
                    rowNum++;
                }
            }

            return output.ToString().TrimEnd();
        }
    }
}

using System;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using ADO.NET_Exercise;

namespace P06_Remove_Villain
{
    class Program
    {
        static void Main(string[] args)
        {
            int villainId = int.Parse(Console.ReadLine());

            using SqlConnection sqlConnection
                = new SqlConnection(Config.ConnectionString);

            sqlConnection.Open();
            string result = DeleteVillain(sqlConnection, villainId);
            Console.WriteLine(result);
            sqlConnection.Close();
        }

        private static string DeleteVillain(SqlConnection sqlConnection, int villainId)
        {
            StringBuilder output = new StringBuilder();

            string villainNameQuery = @"SELECT [Name]
	                                      FROM [Villains]
	                                     WHERE [Id] = @VillainId";

            SqlCommand villainNameCommand = new SqlCommand(villainNameQuery, sqlConnection);
            villainNameCommand.Parameters.AddWithValue("@VillainId", villainId);

            string villainName = (string)villainNameCommand.ExecuteScalar();

            if (villainName == null)
            {
                return "No such villain was found.";
            }

            SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();

            try
            {
                string releaseMinionsQuery = @"DELETE FROM [MinionsVillains]
	                                             WHERE [VillainId] = @VillainId";

                SqlCommand releaseMinionsCommand
                    = new SqlCommand(releaseMinionsQuery, sqlConnection, sqlTransaction);

                releaseMinionsCommand.Parameters.AddWithValue("@VillainId", villainId);

                int minionsReleased = releaseMinionsCommand.ExecuteNonQuery();

                string deleteVillainQuery = @"	DELETE FROM [Villains]
		                                          WHERE [Id] = @VillainId";

                SqlCommand deleteVillainCommand =
                    new SqlCommand(deleteVillainQuery, sqlConnection, sqlTransaction);

                deleteVillainCommand.Parameters.AddWithValue("@VillainId", villainId);

                int villainsDeleted = deleteVillainCommand.ExecuteNonQuery();

                if (villainsDeleted != 1)
                {
                    sqlTransaction.Rollback();
                }
                
                output.AppendLine($"{villainName} was deleted.")
                    .AppendLine($"{minionsReleased} minions were released.");
            }
            catch (Exception e)
            {
                sqlTransaction.Rollback();
                return e.ToString();
            }
            
            sqlTransaction.Commit();

            return output.ToString().TrimEnd();
        }
    }
}

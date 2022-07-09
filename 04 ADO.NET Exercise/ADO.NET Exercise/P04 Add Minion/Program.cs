using System;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using ADO.NET_Exercise;

namespace P04_Add_Minion
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] minionInfo = Console.ReadLine()
                .Split(": ", StringSplitOptions.RemoveEmptyEntries)[1]
                .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                .ToArray();
            string villainName = Console.ReadLine()
                .Split(": ", StringSplitOptions.RemoveEmptyEntries)[1];
            
            using SqlConnection sqlConnection
                = new SqlConnection(Config.ConnectionString);

            sqlConnection.Open();
            string result = AddNewMinion(sqlConnection, minionInfo, villainName);
            Console.WriteLine(result);
            sqlConnection.Close();
        }

        private static string AddNewMinion(SqlConnection sqlConnection, 
            string[] minionInfo, string villainName)
        {
            StringBuilder output = new StringBuilder();

            string minionName = minionInfo[0];
            int minionAge = int.Parse(minionInfo[1]);
            string townName = minionInfo[2];

            SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();
            try
            {
                int townId = GetTownId(sqlConnection, sqlTransaction, output, townName);
                int villainId = GetVillainId(sqlConnection, sqlTransaction, output, villainName);
                int minionId = AddMinionAndGetId(sqlConnection, sqlTransaction, minionName, minionAge, townId);

                string addMinionToVillainQuery = @"INSERT INTO [MinionsVillains]([MinionId], [VillainId])
	                                                    VALUES
	                                                    (@MinionId,@VillainId)";
                SqlCommand addMinionToVillainCommand =
                    new SqlCommand(addMinionToVillainQuery, sqlConnection, sqlTransaction);
                addMinionToVillainCommand.Parameters.AddWithValue("@MinionId", minionId);
                addMinionToVillainCommand.Parameters.AddWithValue("@VillainId", villainId);

                addMinionToVillainCommand.ExecuteNonQuery();
                output.AppendLine($"Successfully added {minionName} to be minion of {villainName}.");

                sqlTransaction.Commit();
            }
            catch (Exception e)
            {
                sqlTransaction.Rollback();
                return e.ToString();
            }

            return output.ToString().TrimEnd();
        }

        private static int GetTownId(SqlConnection sqlConnection, SqlTransaction sqlTransaction, StringBuilder output, string townName)
        {
            string townIdQuery = @"SELECT [Id]
	                                 FROM [Towns]
	                                WHERE [Name] = @TownName";

            SqlCommand townIdCommand = new SqlCommand(townIdQuery, sqlConnection, sqlTransaction);
            townIdCommand.Parameters.AddWithValue("@TownName", townName);

            object townIdObj = townIdCommand.ExecuteScalar();

            if (townIdObj == null)
            {
                string addTownQuery = @"INSERT INTO [Towns]([Name])
	                                         VALUES
	                                                (@TownName)";
                SqlCommand addTownCommand = new SqlCommand(addTownQuery, sqlConnection, sqlTransaction);
                addTownCommand.Parameters.AddWithValue("@TownName", townName);

                addTownCommand.ExecuteNonQuery();

                output.AppendLine($"Town {townName} was added to the database.");

                townIdObj = townIdCommand.ExecuteScalar();
            }

            return (int)townIdObj;
        }

        private static int GetVillainId(SqlConnection sqlConnection, SqlTransaction sqlTransaction, 
            StringBuilder output, string villainName)
        {
            string villainIdQuery = @"SELECT [Id]
	                                        FROM [Villains]
	                                       WHERE [Name] = @VillainName";
            SqlCommand villainIdCommand = new SqlCommand(villainIdQuery, sqlConnection, sqlTransaction);
            villainIdCommand.Parameters.AddWithValue("@VillainName", villainName);

            object villainIdObj = villainIdCommand.ExecuteScalar();
            if (villainIdObj == null)
            {
                string evilnessFactorQuery = @"SELECT [Id]
	                                                 FROM [EvilnessFactors]
	                                                WHERE [Name] = 'Evil'";
                SqlCommand evilnessFactorCommand =
                    new SqlCommand(evilnessFactorQuery, sqlConnection, sqlTransaction);
                int evilnessFactorId = (int)evilnessFactorCommand.ExecuteScalar();
                
                string insertVillainQuery = @"INSERT INTO [Villains]([Name], [EvilnessFactorId])
	                                                     VALUES
		                                                    (@VillainName, @EvilnessFactorId)";
                SqlCommand insertVillain =
                    new SqlCommand(insertVillainQuery, sqlConnection, sqlTransaction);
                insertVillain.Parameters.AddWithValue("@VillainName", villainName);
                insertVillain.Parameters.AddWithValue("@EvilnessFactorId", evilnessFactorId);

                insertVillain.ExecuteNonQuery();
                output.AppendLine($"Villain {villainName} was added to the database.");

                villainIdObj = villainIdCommand.ExecuteScalar();
            }

            return (int)villainIdObj;
        }

        private static int AddMinionAndGetId(SqlConnection sqlConnection, SqlTransaction sqlTransaction,
            string minionName, int minionAge, int townId)
        {
            string addMinionQuery = @"INSERT INTO [Minions]([Name], [Age], [TownId])
	                                           VALUES
	                                           (@MinionName, @MinionAge, @TownId)";
            SqlCommand addMinionCommand = new SqlCommand(addMinionQuery, sqlConnection, sqlTransaction);
            addMinionCommand.Parameters.AddWithValue("@MinionName", minionName);
            addMinionCommand.Parameters.AddWithValue("@MinionAge", minionAge);
            addMinionCommand.Parameters.AddWithValue("@TownId", townId);

            addMinionCommand.ExecuteNonQuery();

            string addedMinionIdQuery = @"SELECT [Id]
	                                   FROM [Minions]	
	                                  WHERE [Name] = @MinionName AND [Age] = @MinionAge AND [TownId] = @TownId";
            
            SqlCommand getMinionIdCommand = new SqlCommand(addedMinionIdQuery, sqlConnection, sqlTransaction);
            getMinionIdCommand.Parameters.AddWithValue("@MinionName", minionName);
            getMinionIdCommand.Parameters.AddWithValue("@MinionAge", minionAge);
            getMinionIdCommand.Parameters.AddWithValue("@TownId", townId);

            int minionId = (int)getMinionIdCommand.ExecuteScalar();

            return minionId;
        }
    }
}

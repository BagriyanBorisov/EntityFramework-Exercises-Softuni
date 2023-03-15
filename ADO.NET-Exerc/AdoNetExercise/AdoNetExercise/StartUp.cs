using System.Text;
using Microsoft.Data.SqlClient;

namespace AdoNetExercise
{
    internal class StartUp
    {
        static async Task Main(string[] args)
        {
          await using SqlConnection connection =
               new SqlConnection(Config.ConnectionString);
            await connection.OpenAsync();

            string output = await AddMinionsToDb(connection);
            Console.WriteLine(output);

        }
        //problem 04
        private static async Task<string> AddMinionsToDb(SqlConnection connection)
        {
            var sb = new StringBuilder();

            string[] minionArgs = Console.ReadLine().Split(' ', StringSplitOptions.RemoveEmptyEntries).ToArray();
            string minionName = minionArgs[1];
            int minionAge = int.Parse(minionArgs[2]);
            string townName = minionArgs[3];
            string villainName = Console.ReadLine().Split(' ')[1];

            SqlTransaction transaction = connection.BeginTransaction();
            try
            {
               int townId = await GetIdByName(connection, sb, townName, transaction);
               int villainId = await GetVillainByName(connection, sb, villainName, transaction);
               int minionId = await GetMinionByName(connection, sb, minionName, minionAge, townId, transaction);

               SqlCommand addMinionToVillainCmd 
                   = new SqlCommand(SqlQueries.InsertIntoMinionsVillains, connection, transaction);
               addMinionToVillainCmd.Parameters.AddWithValue("@minionId", minionId);
               addMinionToVillainCmd.Parameters.AddWithValue("@villainId", villainId);
                
               addMinionToVillainCmd.ExecuteNonQuery();
               sb.AppendLine($"Successfully added {minionName} to be minion of {villainName}");

                transaction.Commit();
            }
            catch (Exception e)
            {
               Console.WriteLine("error bro cmon");
               transaction.Rollback();
            }



            return sb.ToString();
        }

        private static async Task<int> GetMinionByName(SqlConnection connection, StringBuilder sb,
            string minionName,int minionAge, int townId, SqlTransaction transaction)
        {
            SqlCommand insertMinionCmd = new SqlCommand(SqlQueries.InsertIntoMinions, connection, transaction);
                insertMinionCmd.Parameters.AddWithValue("@Name", minionName);
                insertMinionCmd.Parameters.AddWithValue("@age", minionAge);
                insertMinionCmd.Parameters.AddWithValue("@townId", townId);
                await insertMinionCmd.ExecuteNonQueryAsync();

                SqlCommand getIdMinionCmd = new SqlCommand(SqlQueries.GetMinionId, connection, transaction);
                getIdMinionCmd.Parameters.AddWithValue("@Name", minionName);
               

            return (int)await getIdMinionCmd.ExecuteScalarAsync();
        }

        private static async Task<int> GetVillainByName(SqlConnection connection, StringBuilder sb, string villainName, SqlTransaction transaction)
        {
            SqlCommand getIdVillainCmd = new SqlCommand(SqlQueries.GetVillainIdByName, connection, transaction);
            getIdVillainCmd.Parameters.AddWithValue("@Name", villainName);

            object? villainIdObj = await getIdVillainCmd.ExecuteScalarAsync();
            if (villainIdObj == null)
            {
                SqlCommand insertVillainCmd = new SqlCommand(SqlQueries.InsertIntoVillains, connection, transaction);
                insertVillainCmd.Parameters.AddWithValue("@villainName", villainName);
                await insertVillainCmd.ExecuteNonQueryAsync();
                villainIdObj = await getIdVillainCmd.ExecuteScalarAsync();
                sb.AppendLine($"Villain {villainName} was added to the database.");
            }
            return (int)villainIdObj;
        }

        private static async Task<int> GetIdByName(SqlConnection connection, StringBuilder sb, string townName, SqlTransaction transaction)
        {
            SqlCommand getIdCmd = new SqlCommand(SqlQueries.GetTownIdByName, connection, transaction);
            getIdCmd.Parameters.AddWithValue("@townName", townName);
            object? townIdObj = await getIdCmd.ExecuteScalarAsync();
            if (townIdObj == null)
            {
                SqlCommand insertTownCmd
                    = new SqlCommand(SqlQueries.InsertIntoTowns, connection,transaction);
                insertTownCmd.Parameters.AddWithValue("@townName", townName);

                await insertTownCmd.ExecuteNonQueryAsync();
                townIdObj = await getIdCmd.ExecuteScalarAsync();
                sb.AppendLine($"Town {townName} was added to the database.");
            }

            return (int)townIdObj;
        }

        //Problem 01
        static async Task<string> GetAllVillainsWithTheirMinionsAsync(SqlConnection connection)
        {
            var sb = new StringBuilder();
            SqlCommand command =
                new SqlCommand(SqlQueries.GetAllVillainsAndCountOfTheirMinions, connection);
            SqlDataReader reader = await command.ExecuteReaderAsync();
            while (reader.Read())
            {
                string villainName = (string)reader["Name"];
                int minionCount = (int)reader["MinionsCount"];
                sb.AppendLine($"{villainName} - {minionCount}");
            }
            return sb.ToString().TrimEnd();
        }

        //Problem 02
        static async Task<string> GetByVillainIdHisMinionsAsync(SqlConnection connection, int villainId)
        {
            var sb = new StringBuilder();
            SqlCommand getNameCmd =
                new SqlCommand(SqlQueries.GetVillainNameById, connection);
            getNameCmd.Parameters.AddWithValue("@Id", villainId);
            string? villainName =(string?) await getNameCmd.ExecuteScalarAsync();

            if (villainName == null)
            {
                return $"No villain with ID {villainId} exists in the database.";
            }
            sb.AppendLine($"Villain: {villainName}");

            SqlCommand getMinionsCmd = 
                new SqlCommand(SqlQueries.GetByVillainIdHisMinions, connection);
            getMinionsCmd.Parameters.AddWithValue("@Id", villainId);

            SqlDataReader minionsReader = await getMinionsCmd.ExecuteReaderAsync();

            if (!minionsReader.HasRows)
            {
                sb.AppendLine("(no minions)");
            }
            else
            {
                while (minionsReader.Read())
                {
                    long rowNum = (long)minionsReader["RowNum"];
                    string minionName = (string)minionsReader["Name"];
                    int minionAge = (int)minionsReader["Age"];
                    sb.AppendLine($"{rowNum}. {minionName} {minionAge}");
                }
            }
            return sb.ToString().TrimEnd();
        }
    }
}
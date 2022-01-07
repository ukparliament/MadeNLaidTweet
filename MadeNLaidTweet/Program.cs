namespace MadeNLaidTweet
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;

    class Program
    {
        static void Main(string[] args)
        {
            List<StatutoryInstrument> statutoryInstruments = GetSIs();
            foreach (var si in statutoryInstruments)
            {
                Tweet(si);
            }
            UpdateTweetStatus(statutoryInstruments);
        }

        static void UpdateTweetStatus(List<StatutoryInstrument> statutoryInstruments)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MadeNLaidSqlServer"].ConnectionString;

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            foreach (var si in statutoryInstruments.Where(x=>x.IsTweeted))
            {
                using (SqlCommand cmd = new SqlCommand("Add to database", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "InsertUpdateMadeNLaidStatutoryInstrument";
                    cmd.Parameters.AddWithValue("@StatutoryInstrumentName", si.Name != null ? (object)si.Name : DBNull.Value);
                    cmd.Parameters.AddWithValue("@ProcedureName", si.ProcedureName != null ? (object)si.ProcedureName : DBNull.Value);
                    cmd.Parameters.AddWithValue("@LayingBodyName", si.LayingBodyName != null ? (object)si.LayingBodyName : DBNull.Value);
                    cmd.Parameters.AddWithValue("@MadeDate", si.MadeDate != null ? (object)si.MadeDate : DBNull.Value);
                    cmd.Parameters.AddWithValue("@LaidDate", si.LaidDate != null ? (object)si.LaidDate : DBNull.Value);
                    cmd.Parameters.AddWithValue("@StatutoryInstrumentUri", si.Id != null ? (object)si.Id : DBNull.Value);
                    cmd.Parameters.AddWithValue("@WorkPackageUri", si.WorkPackageId != null ? (object)si.WorkPackageId : DBNull.Value);
                    cmd.Parameters.AddWithValue("@TnaUri", si.Link != null ? (object)si.Link : DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsTweeted", (object)1);
                    cmd.Parameters.Add("@Message", SqlDbType.NVarChar, 50).Direction = ParameterDirection.Output;
                    cmd.ExecuteNonQuery();
                    string msg = cmd.Parameters["@Message"].Value.ToString();
                    Console.WriteLine($"Title: {si.Id}, {msg}");
                }
            }
            connection.Close();
        }

        static List<StatutoryInstrument> GetSIs()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MadeNLaidSqlServer"].ConnectionString;

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            List<StatutoryInstrument> statutoryInstruments = new List<StatutoryInstrument>();
            using (SqlCommand cmd = new SqlCommand("Read from database", connection))
            {
                String sql = @"SELECT 
	                                  [StatutoryInstrumentName]
                                      ,[ProcedureName]
                                      ,[LayingBodyName]
                                      ,[LaidDate]
                                      ,[MadeDate]
                                      ,[StatutoryInstrumentUri]
                                      ,[WorkPackageUri]
                                      ,[TnaUri]
                                FROM [dbo].[MadeNLaidStatutoryInstrument]
                                WHERE IsTweeted = 0";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var si = new StatutoryInstrument();
                            si.Name = reader.GetString(0);
                            si.ProcedureName = reader.GetString(1);
                            si.LayingBodyName = reader.GetString(2);
                            si.LaidDate = reader.GetDateTimeOffset(3);
                            si.MadeDate = reader.GetDateTimeOffset(4);
                            si.Id = reader.GetString(5);
                            si.WorkPackageId = reader.GetString(6);
                            si.Link = reader.GetString(7);
                            si.IsTweeted = false;
                            statutoryInstruments.Add(si);
                        }
                    }
                }
            }

            connection.Close();
            return statutoryInstruments;
        }

        static void Tweet (StatutoryInstrument si)
        {
            string oauth_consumer_key = ConfigurationManager.AppSettings["oauth_consumer_key"];
            string oauth_consumer_secret = ConfigurationManager.AppSettings["oauth_consumer_secret"];
            string oauth_token = ConfigurationManager.AppSettings["oauth_token"];
            string oauth_token_secret = ConfigurationManager.AppSettings["oauth_token_secret"];

            var twitter = new TwitterApi(oauth_consumer_key, oauth_consumer_secret, oauth_token, oauth_token_secret);
            var response = twitter.Tweet(si.TweetText);
            si.IsTweeted = true;
            Console.WriteLine(response);
        }
    }
}

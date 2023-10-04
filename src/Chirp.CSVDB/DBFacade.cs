using System;
using System.Data;
using System.Data.SQLite;

public class DBFacade
{
    private string connectionString;
    
    public DBFacade(string dbFilePath)
    {
        connectionString = $"Data Source={dbFilePath}";
    }

    public void CreateDatabase()
    {
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

     
            string schemaSql = System.IO.File.ReadAllText("schema.sql");        // Execute schema.sql to create tables
            using (var cmd = new SQLiteCommand(schemaSql, connection))
            {
                cmd.ExecuteNonQuery();
            }


            string dumpSql = System.IO.File.ReadAllText("dump.sql");             // Execute dump.sql to insert data
            using (var cmd = new SQLiteCommand(dumpSql, connection))
            {
                cmd.ExecuteNonQuery();
            }

            connection.Close();
        }
    }

    public void AddUser(int userId, string username, string email)
    {
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            string insertSql = "INSERT INTO user (user_id, username, email) VALUES (@userId, @username, @email)";

            using (var cmd = new SQLiteCommand(insertSql, connection))
            {
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.ExecuteNonQuery();
            }

            connection.Close();
        }
    }

    public void AddCheep(int messageId, int authorId, string text, int pubDate)
    {
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            string insertSql = "INSERT INTO message (message_id, author_id, text, pub_date) VALUES (@messageId, @authorId, @text, @pubDate)";

            using (var cmd = new SQLiteCommand(insertSql, connection))
            {
                cmd.Parameters.AddWithValue("@messageId", messageId);
                cmd.Parameters.AddWithValue("@authorId", authorId);
                cmd.Parameters.AddWithValue("@text", text);
                cmd.Parameters.AddWithValue("@pubDate", pubDate);
                cmd.ExecuteNonQuery();
            }

            connection.Close();
        }
    }

       public List<Tuple<string, string, int>> ReadCheeps()
    {
        List<Tuple<string, string, int>> messages = new List<Tuple<string, string, int>>();
        
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            string selectSql = "SELECT message.text, user.username, message.pub_date " +
                               "FROM message " +
                               "INNER JOIN user ON message.author_id = user.user_id"; // Only message, user and pubDate is chosen

            using (var cmd = new SQLiteCommand(selectSql, connection))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    string text = reader.GetString(0);
                    string username = reader.GetString(1);
                    int pubDate = reader.GetInt32(2);
                    
                    messages.Add(new Tuple<string, string, int>(text, username, pubDate));
                }
            }

            connection.Close();
        }

        return messages;
    }
}
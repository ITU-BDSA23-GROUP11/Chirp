using Chirp.CSVDB;
using Chirp.Utilities;
using Chirp.Utilities.Models;
using System.Data;
using System.Data.SQLite;

/***

STILL UNDER CONSTRUCTION!

Responsible: ANBC

**/
public class DBFacade : IDatabaseRepository
{
    private SQLiteConnection _connection;
    private DBFacade? _instance; 
    public DBFacade()
    {  
        _connection = new SQLiteConnection("Data Source=dump.sql");
        _connection.Open();
    }

    public  DBFacade GetInstance()
    {
        if (_instance == null)
        {
            _instance = new DBFacade();
        }

        return _instance;
    }
    

    public void OpenConnection()
    {
        if (_connection.State != ConnectionState.Open)
        {
            _connection.Open();
        }
    }

    public void CloseConnection()
    {
        if (_connection.State != ConnectionState.Closed)
        {
            _connection.Close();
        }
    }


    public void WriteCheep(int messageId, int authorId, string text, int pubDate)
    {
        OpenConnection();

        using (SQLiteCommand cmd = new SQLiteCommand("INSERT INTO message (message_id, author_id, text, pub_date) VALUES (@messageId, @authorId, @text, @pubDate)", _connection))
        {
            cmd.Parameters.AddWithValue("@messageId", messageId);
            cmd.Parameters.AddWithValue("@authorId", authorId);
            cmd.Parameters.AddWithValue("@text", text);
            cmd.Parameters.AddWithValue("@pubDate", pubDate);

            cmd.ExecuteNonQuery();
        }

        CloseConnection();
    }

    public List<Cheep> ReadCheeps()
    {
        OpenConnection();

        List<Cheep> messagesList = new List<Cheep>();

        using (SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM message", _connection))
        using (SQLiteDataReader reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                string messageId = reader.GetString(0);
                string authorId = reader.GetString(1);
                int pubDate = reader.GetInt32(2);

                Cheep cheep = new Cheep(messageId, authorId, pubDate);
                messagesList.Add(cheep);
            }
        }

        CloseConnection();

        return messagesList;
    }

    public void AddCheep(Cheep cheep)
    {
        throw new NotImplementedException();
    }

    public List<Cheep> GetCheeps()
    {
        throw new NotImplementedException();
    }
}

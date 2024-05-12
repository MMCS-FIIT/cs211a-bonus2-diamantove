using System.Data.SQLite;

public class UserDatabase
{
    static SQLiteConnection connection;
    static SQLiteCommand Command;

    public UserDatabase()
    {
        if (Connect())
    
            Console.WriteLine("Connected");
            Command = new SQLiteCommand(connection)
            {
                CommandText = "CREATE TABLE IF NOT EXISTS [User]([id] INTEGER, [name] TEXT, [regData] INTEGER, [countGuessedWords] INTEGER, [balance] INTEGER);"
            };
            Command.ExecuteNonQuery();
        }

    public bool Connect()
    {
        try
        {
            connection = new SQLiteConnection("Data Source=..\\..\\..\\users.db; Version=3; FailIfMissing=False");
            connection.Open();
            return true;
        }
        catch (SQLiteException ex)
        {
            Console.WriteLine($"Ошибка доступа к базе данных. Исключение: {ex.Message}");
            return false;
        }
    }

    //public void Insert(string line)
    //{
    //    command.CommandText = line;
    //    command.ExecuteNonQuery();
    //}

    public void RegUser(int id, string name)
    {
        Command.CommandText = $"INSERT INTO User (id, name, regData, countGuessedWords, balance) VALUES ({id}, {name}, {DateTime.Now}, 0, 0)";
        Command.ExecuteNonQuery();
    }

    public User GetUser(int id)
    {
        connection.Open();

        Command command = new Command(sqlExpression, connection);
        using (SqliteDataReader reader = command.ExecuteReader())
    }
}

using System.Data.SQLite;

public class UserDatabase
{
    static SQLiteConnection connection;
    static SQLiteCommand command;

    public UserDatabase()
    {
        if (Connect("users.sqlite"))
    
            Console.WriteLine("Connected");
            command = new SQLiteCommand(connection)
            {
                CommandText = "CREATE TABLE IF NOT EXISTS [User]([id] INTEGER, [name] TEXT, [regData] TEXT, [countGuessedWords] INTEGER, [balance] INTEGER);"
            };
            command.ExecuteNonQuery();
        }

    public bool Connect(string fileName)
    {
        try
        {
            connection = new SQLiteConnection("Data Source=" + fileName + ";Version=3; FailIfMissing=False");
            connection.Open();
            return true;
        }
        catch (SQLiteException ex)
        {
            Console.WriteLine($"Ошибка доступа к базе данных. Исключение: {ex.Message}");
            return false;
        }
    }

    public void Insert(string line)
    {
        command.CommandText = line;
        command.ExecuteNonQuery();
    }

    //public void RegUser(int id, string name)
    //{
    //    command.CommandText = $"INSERT INTO User (id, name, regData, countGuessedWords, balance) VALUES ({id}, {name}, {DateTime.Now}, 0, 0)";
    //    command.ExecuteNonQuery();
    //}
}

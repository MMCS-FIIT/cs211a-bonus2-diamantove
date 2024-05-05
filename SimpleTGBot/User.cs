using System.Runtime.Serialization.Json;

public class User
{
    // Ид пользователя (ид телеграмма)
    public int id { get; init; }
    // Дата регистрации
    public string name { get; set; }
    // Количество отгаданных слов
    public DateTime regData { get; init; }
    // Количество отгаданных слов
    public int countGuessedWords { get; private set; }
    // Баланс пользователя
    public int balance { get; private set; }

    public User(int id) 
    {
        if (!this.IsRegistered())
        {
            this.Register();
            Console.WriteLine($"Зарегистрирован новый пользователь с ид {id}.");
        }

        this.id = id;
        this.name = string.Empty;
        this.regData = DateTime.Now;
        this.countGuessedWords = 0;
        this.balance = 0;
    }

    public bool IsRegistered()
    {
        Dictionary<int, int[]> users;
        var formatter = new DataContractJsonSerializer(typeof(Dictionary<int, int[]>));

        using (FileStream fs = new FileStream(@"..\..\..\users.json", FileMode.Open))
        {
            users = (Dictionary<int, int[]>)formatter.ReadObject(fs)!;

            return users.ContainsKey((int)this.id);
        }
    }

    public void Register(string name)
    {
        Dictionary<int, int[]> users;
        var formatter = new DataContractJsonSerializer(typeof(Dictionary<int, int[]>));

        using (FileStream fs = new FileStream(@"..\..\..\users.json", FileMode.Open))
        {
            users = (Dictionary<int, int[]>)formatter.ReadObject(fs)!;

            users.Add()

            return users.ContainsKey((int)this.id);
        }
    }
}


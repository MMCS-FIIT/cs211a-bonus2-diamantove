using System.Runtime.Serialization.Json;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;

public class MyUser
{
    // Ид пользователя (ид телеграмма)
    public long id { get; init; }

    // Имя
    public string name { get; set; }

    // Количество отгаданных слов
    public int countGuessedWords { get; private set; }

    // Состояние в боте
    public int stage { get; }
    //
    public int level { get; }

    public MyUser(long id)
    {
        this.id = id;

        if (!this.IsRegistered())
        {
            this.Register();
            Console.WriteLine($"Зарегистрирован новый пользователь с ид {id}.");
        }

        var data = GetStat();
          
        this.countGuessedWords = data[0];
        this.stage = data[1];
        this.level = this.countGuessedWords / 10;

        this.name = GetName();
    }

    public bool IsRegistered()
    {
        Dictionary<long, int[]> users;
        var formatter = new DataContractJsonSerializer(typeof(Dictionary<long, int[]>));

        using (FileStream fs = new FileStream(@"..\..\..\users.json", FileMode.Open))
        {
            users = (Dictionary<long, int[]>)formatter.ReadObject(fs)!;

            return users.ContainsKey(this.id);
        }
    }

    public void Register()
    {
        Dictionary<long, int[]> users;
        var formatter = new DataContractJsonSerializer(typeof(Dictionary<long, int[]>));

        using (FileStream fs = new FileStream(@"..\..\..\users.json", FileMode.Open))
        {
            users = (Dictionary<long, int[]>)formatter.ReadObject(fs)!;

            users.Add(this.id, new int[] { 0, 0 });
        }

        using (FileStream fs = new FileStream(@"..\..\..\users.json", FileMode.Open))
        {
            formatter.WriteObject(fs, users);
        }
    }

    public void IncCountGuessedWords()
    {
        Dictionary<long, int[]> users;
        var formatter = new DataContractJsonSerializer(typeof(Dictionary<long, int[]>));

        using (FileStream fs = new FileStream(@"..\..\..\users.json", FileMode.Open))
        {
            users = (Dictionary<long, int[]>)formatter.ReadObject(fs)!;

            users[this.id][0] = this.countGuessedWords + 1;
        }

        using (FileStream fs = new FileStream(@"..\..\..\users.json", FileMode.Open))
        {
            formatter.WriteObject(fs, users);
        }
    }


    public int[] GetStat()
    {
        Dictionary<long, int[]> users;
        var formatter = new DataContractJsonSerializer(typeof(Dictionary<long, int[]>));

        using (FileStream fs = new FileStream(@"..\..\..\users.json", FileMode.Open))
        {
            users = (Dictionary<long, int[]>)formatter.ReadObject(fs)!;

            var user = users[this.id];

            return new int[] { user[0], user[1] };
        }
    }

    public void SetStage(int stage)
    {
        Dictionary<long, int[]> users;
        var formatter = new DataContractJsonSerializer(typeof(Dictionary<long, int[]>));

        using (FileStream fs = new FileStream(@"..\..\..\users.json", FileMode.Open))
        {
            users = (Dictionary<long, int[]>)formatter.ReadObject(fs)!;

            users[this.id][1] = stage;
        }

        using (FileStream fs = new FileStream(@"..\..\..\users.json", FileMode.Open))
        {
            formatter.WriteObject(fs, users);
        }
    }

    public void SetName(string name)
    {
        Dictionary<long, string> names;
        var formatter = new DataContractJsonSerializer(typeof(Dictionary<long, string>));

        using (FileStream fs = new FileStream(@"..\..\..\names.json", FileMode.Open))
        {
            names = (Dictionary<long, string>)formatter.ReadObject(fs)!;

            names[this.id] = name;
        }

        using (FileStream fs = new FileStream(@"..\..\..\names.json", FileMode.Open))
        {
            formatter.WriteObject(fs, names);
        }
    }

    public string GetName()
    {
        Dictionary<long, string> names;
        var formatter = new DataContractJsonSerializer(typeof(Dictionary<long, string>));

        using (FileStream fs = new FileStream(@"..\..\..\names.json", FileMode.Open))
        {
            names = (Dictionary<long, string>)formatter.ReadObject(fs)!;

            return names[this.id];
        }
    }

    public string GetStringStat()
    {
        return $"Ваше имя: {this.name}.\nУровень: {this.level}.\nКоличество отгаданных слов: {this.countGuessedWords}.";
    }

    public string GetActualWord()
    {
        Dictionary<long, string> words;
        var formatter = new DataContractJsonSerializer(typeof(Dictionary<long, string>));

        using (FileStream fs = new FileStream(@"..\..\..\words.json", FileMode.Open))
        {
            words = (Dictionary<long, string>)formatter.ReadObject(fs)!;

            return words[this.id];
        }
    }

    public void ReAssignNewWord()
    {
        int length;
        switch (this.level)
        {
            case >= 0 and <= 3: 
                {
                    length = 4;
                    break;
                }
            case >= 4 and <= 7:
                {
                    length = 5;
                    break;
                }
            case >= 8 and <= 10:
                {
                    length = 6;
                    break;
                }
            default:
                {
                    length = int.MaxValue;
                    break;
                }
        }

        Dictionary<long, string> words;
        var formatter = new DataContractJsonSerializer(typeof(Dictionary<long, string>));

        using (FileStream fs = new FileStream(@"..\..\..\words.json", FileMode.Open))
        {
            words = (Dictionary<long, string>)formatter.ReadObject(fs)!;
            
            words[this.id] = Words.GetRandomWord(length);
        }

        using (FileStream fs = new FileStream(@"..\..\..\words.json", FileMode.Open))
        {
            formatter.WriteObject(fs, words);
        }
    }

    public bool IsPhotoExist()
    {
        string path = "../../../photos/";

        return Directory.GetFiles(path).Contains($"{path}{this.id}.png");
    }
}
using System.Runtime.Serialization.Json;
using TeleBot.Entities;
using Telegram.Bot.Types;

namespace SimpleTGBot;

public static class Program
{
    public static async Task Main(string[] args)
    {
        TelegramBot telegramBot = new TelegramBot();
        await telegramBot.Run();

        //MyUser user = new MyUser(1);

        //Console.WriteLine(MyFuncs.ShuffleString(user.GetActualWord()));
        //Console.WriteLine(MyFuncs.IsUserPhotoExist(856593098));
    }
}

class MyFuncs 
{
    public static string ShuffleString(string str)
    {
        Random random = new Random();

        char[] charArray = str.ToCharArray();
        int n = charArray.Length;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            char value = charArray[k];
            charArray[k] = charArray[n];
            charArray[n] = value;
        }
        return new string(charArray);
    } 

    public static string GetTopString()
    {
        Dictionary<long, int[]> users;
        var formatter = new DataContractJsonSerializer(typeof(Dictionary<long, int[]>));
        string top = "";

        using (FileStream fs = new FileStream(@"..\..\..\users.json", FileMode.Open))
            users = (Dictionary<long, int[]>)formatter.ReadObject(fs)!;

        int i = 1;

        foreach (var user in users.OrderByDescending(x => x.Value[0]))
        {
            MyUser myUser = new MyUser(user.Key);

            top += $"{i}. {myUser.name}. Слов: {myUser.countGuessedWords}.\n";

            i++;
        }

        return top;
    }
}
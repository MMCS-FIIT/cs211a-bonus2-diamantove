using System;
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
}
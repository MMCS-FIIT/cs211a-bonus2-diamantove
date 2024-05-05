using Newtonsoft.Json.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization.Json;

namespace SimpleTGBot;

public static class Program
{
    // Метод main немного видоизменился для асинхронной работы
    public static async Task Main(string[] args)
    {
        //TelegramBot telegramBot = new TelegramBot();
        //await telegramBot.Run();

        Dictionary<int, int[]> users;

        var formatter = new DataContractJsonSerializer(typeof(Dictionary<int, int[]>));

        using (FileStream fs = new FileStream(@"..\..\..\users.json", FileMode.Open))
        {

                users = (Dictionary<int, int[]>)formatter.ReadObject(fs)!;

                Console.WriteLine(users.ContainsKey(123)); 
        }
    }
}
using Telegram.Bot.Types.ReplyMarkups;

public class MyKeyBoards
{
    public static ReplyKeyboardMarkup mainKeyBoard = new(new[]
    {
    new KeyboardButton[] { "👑 Моя статистика" },
    new KeyboardButton[] { "🎯 Играть" },
    new KeyboardButton[] { "🔍 Обучение" },
    });

    public static ReplyKeyboardMarkup intoMenuKeyBoard = new(new[]
    {
    new KeyboardButton[] { "🚪 В главное меню" },
    //new KeyboardButton[] { "✏️ Новая игра" }
    });

    public static ReplyKeyboardMarkup activeGameKeyBoard = new(new[]
    {
    new KeyboardButton[] { "🚪 В главное меню" },
    new KeyboardButton[] { "✏️ Новая игра" }
    });
}
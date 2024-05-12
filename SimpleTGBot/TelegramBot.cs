namespace SimpleTGBot;

using System;
using System.Threading.Tasks;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using System.Net;
using System.Net.Http;

public class TelegramBot
{
    private const string BotToken = "7028697496:AAEfNe6Cx910AOR16MwLFChuZj3bdc8rIsU";
    private const string rules = "📚 Правила игры в слова:\r\n\r\nОсновная суть игры - угадывать слова, буквы которого показываются. В зависимости от количества отгаданных слов меняется уровень. С каждым уровнем повышается сложность: увеличивается количество букв в слове.\r\n\r\nУровни: \r\n0️⃣ 0-39 отгаданных слов\r\n1️⃣ 40-79 отгаданных слов\r\n2️⃣ 80-109 отгаданных слов\r\n3️⃣ 110+ отгаданных слов\r\n\r\nЕсть топ, включающий всех игроков. Чем больше отгаданных слов, тем выше ты в топе.";

    public async Task Run()
    {

        var botClient = new TelegramBotClient(BotToken);
        using CancellationTokenSource cts = new CancellationTokenSource();
        
        ReceiverOptions receiverOptions = new ReceiverOptions()
        {
            AllowedUpdates = new [] { UpdateType.Message }
        };

        botClient.StartReceiving(
            updateHandler: OnMessageReceived,
            pollingErrorHandler: OnErrorOccured,
            receiverOptions: receiverOptions,
            cancellationToken: cts.Token
        );

        // Проверяем что токен верный и получаем информацию о боте
        var me = await botClient.GetMeAsync(cancellationToken: cts.Token);
        Console.WriteLine($"Бот @{me.Username} запущен.\nДля остановки нажмите клавишу Esc...");

        // Ждём, пока будет нажата клавиша Esc, тогда завершаем работу бота
        while (Console.ReadKey().Key != ConsoleKey.Escape) { }

        // Отправляем запрос для остановки работы клиента.
        cts.Cancel();
    }

    /// <summary>
    /// Обработчик события получения сообщения.
    /// </summary>
    /// <param name="botClient">Клиент, который получил сообщение</param>
    /// <param name="update">Событие, произошедшее в чате. Новое сообщение, голос в опросе, исключение из чата и т. д.</param>
    /// <param name="cancellationToken">Служебный токен для работы с многопоточностью</param>
    async Task OnMessageReceived(ITelegramBotClient botClient, Telegram.Bot.Types.Update update, CancellationToken cancellationToken)
    {
        var message = update.Message;
        MyUser user = new MyUser(message.From!.Id);

        if (message.Photo != null)
        {
            if (user.stage == 2)
            {
                var filePath = botClient.GetFileAsync(message.Photo[^1].FileId).Result.FilePath;
                using (var c = new WebClient())
                    c.DownloadFile(@$"https://api.telegram.org/file/bot{BotToken}/{filePath}", @$"../../../photos/{message.From.Id}.png");

                await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: $"Новая аватарка успешно установлена!", cancellationToken: cancellationToken);
            }

            return;
        }

        if (message is null)
        {
            return;
        }


        if (message.Text == "🚪 В главное меню")
        {
            await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: $"Меню:", cancellationToken: cancellationToken, replyMarkup: MyKeyBoards.mainKeyBoard);
            user.SetStage(2);
            return;
        }

        // Обработка состояния пользователя.
        switch (user.stage)
        {
            case 0:
                {
                    await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: "Приветствую тебя в игре слов! Напиши свое имя.", cancellationToken: cancellationToken);
                    user.SetStage(1);
                    break;
                }
            case 1:
                {
                    await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: $"Твое имя установлено на: {message.Text}.\n\nОзнакомиться со мной ты можешь по кнопкам ниже:", cancellationToken: cancellationToken, replyMarkup: MyKeyBoards.mainKeyBoard);
                    user.SetName(message.Text!);
                    user.SetStage(2);
                    break;
                }
            // Главное меню.
            case 2:
                {
                    switch (message.Text)
                    {
                        case "👑 Моя статистика":
                            {
                                await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: $"📌 Вот твоя статистика:\n\n{user.GetStringStat()}\n\nВы можете установить аватарку, отправив ее здесь.", cancellationToken: cancellationToken, replyMarkup: MyKeyBoards.intoMenuKeyBoard);

                                if (!user.IsPhotoExist())
                                    return;

                                await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: $"Твоя аватарка:", cancellationToken: cancellationToken, replyMarkup: MyKeyBoards.intoMenuKeyBoard);

                                using (var httpClient = new HttpClient())
                                {
                                    using (var formData = new MultipartFormDataContent())
                                    {
                                        byte[] photoBytes = File.ReadAllBytes($"../../../photos/{message.From.Id}.png");

                                        formData.Add(new ByteArrayContent(photoBytes), "photo", "photo.png");
                                        formData.Add(new StringContent(message.From.Id.ToString()), "chat_id");

                                        var response = await httpClient.PostAsync($"https://api.telegram.org/bot{BotToken}/sendPhoto", formData);
                                    }
                                }

                                break;
                            }

                        case "🎯 Играть":
                            {
                                await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: $"🔒 Игра начата.", cancellationToken: cancellationToken, replyMarkup: MyKeyBoards.mainKeyBoard);

                                user.ReAssignNewWord();

                                string word = user.GetActualWord();
                                await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: $"Уровень {user.level}. Длина слова - {word.Count()}.\n\nНабор букв: {MyFuncs.ShuffleString(word)}", cancellationToken: cancellationToken, replyMarkup: MyKeyBoards.intoMenuKeyBoard);

                                user.SetStage(3);
                                break;
                            }

                        case "💎 Топ":
                            {
                                await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: $"Топ:\n\n{MyFuncs.GetTopString()}", cancellationToken: cancellationToken);

                                break;
                            }

                        case "🔍 Обучение":
                            {
                                await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: rules, cancellationToken: cancellationToken);

                                break;
                            }

                    }
                    break;
                }

            // Активная игра.
            case 3:
                {
                    switch (message.Text)
                    {
                        // Если слово угадано
                        case var text when text == user.GetActualWord():
                            {
                                await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: $"[+1] 🎉 Слово угадано.\n\nВсего отгадано: {user.countGuessedWords + 1}\n\nДля новой игры нажмите кнопку ниже.", cancellationToken: cancellationToken, replyMarkup: MyKeyBoards.activeGameKeyBoard);

                                user.IncCountGuessedWords();

                                break;
                            }
                        case "✏️ Новая игра":
                            {
                                user.ReAssignNewWord();


                                await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: $"Уровень {user.level}. Длина слова - 4.\n\nНабор букв: {MyFuncs.ShuffleString(user.GetActualWord())}", cancellationToken: cancellationToken, replyMarkup: MyKeyBoards.intoMenuKeyBoard);

                                break;
                            }
                        default:
                            {
                                await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: $"🔒 Неверное слово.\n\nНабор букв: {MyFuncs.ShuffleString(user.GetActualWord())}", cancellationToken: cancellationToken, replyMarkup: MyKeyBoards.intoMenuKeyBoard);
                                break;
                            }
                    }

                    break;
                }

        }
    }

    /// <summary>
    /// Обработчик исключений, возникших при работе бота
    /// </summary>
    /// <param name="botClient">Клиент, для которого возникло исключение</param>
    /// <param name="exception">Возникшее исключение</param>
    /// <param name="cancellationToken">Служебный токен для работы с многопоточностью</param>
    /// <returns></returns>
    Task OnErrorOccured(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        // В зависимости от типа исключения печатаем различные сообщения об ошибке
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            
            _ => exception.ToString()
        };

        Console.WriteLine(errorMessage);
        
        // Завершаем работу
        return Task.CompletedTask;
    }
}
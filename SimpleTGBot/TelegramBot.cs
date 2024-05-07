namespace SimpleTGBot;

using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

public class TelegramBot
{
    private const string BotToken = "7028697496:AAEfNe6Cx910AOR16MwLFChuZj3bdc8rIsU";
    
    public async Task Run()
    {
        var botClient = new TelegramBotClient(BotToken);
        using CancellationTokenSource cts = new CancellationTokenSource();
        
        ReceiverOptions receiverOptions = new ReceiverOptions()
        {
            AllowedUpdates = new [] { UpdateType.Message }
        };

        // Привязываем все обработчики и начинаем принимать сообщения для бота
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
        while (Console.ReadKey().Key != ConsoleKey.Escape){}

        // Отправляем запрос для остановки работы клиента.
        cts.Cancel();
    }

    /// <summary>
    /// Обработчик события получения сообщения.
    /// </summary>
    /// <param name="botClient">Клиент, который получил сообщение</param>
    /// <param name="update">Событие, произошедшее в чате. Новое сообщение, голос в опросе, исключение из чата и т. д.</param>
    /// <param name="cancellationToken">Служебный токен для работы с многопоточностью</param>
    async Task OnMessageReceived(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        // Работаем только с сообщениями. Остальные события игнорируем
        var message = update.Message;
        if (message is null)
        {
            return;
        }

        MyUser user = new MyUser(message.From!.Id);

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
                                await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: $"📌 Вот твоя статистика:\n\n{user.GetStringStat()}", cancellationToken: cancellationToken, replyMarkup: MyKeyBoards.mainKeyBoard);
                                break;
                            }
                        case "🎯 Играть":
                            {
                                await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: $"🔒 Игра начата.", cancellationToken: cancellationToken, replyMarkup: MyKeyBoards.mainKeyBoard);

                                user.ReAssignNewWord();

                                await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: $"Уровень {user.level}. Длина слова - 4.\n\nНабор букв: {MyFuncs.ShuffleString(user.GetActualWord())}", cancellationToken: cancellationToken, replyMarkup: MyKeyBoards.intoMenuKeyBoard);

                                user.SetStage(3);
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
                                int award = new Random().Next(1, 3);
                                await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: $"🎉 Слово угадано.\nНачислено {award} бонусов.\nДля новой игры нажмите кнопку ниже.", cancellationToken: cancellationToken, replyMarkup: MyKeyBoards.activeGameKeyBoard);

                                user.IncCountGuessedWords();

                                break;
                            }
                        case "✏️ Новая игра":
                            {
                                user.ReAssignNewWord();


                                await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: $"Уровень {user.level}. Длина слова - 4.\n\nНабор букв: {MyFuncs.ShuffleString(user.GetActualWord())}", cancellationToken: cancellationToken, replyMarkup: MyKeyBoards.intoMenuKeyBoard);

                                break;
                            }
                        case "🚪 В главное меню":
                            {
                                await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: $"Меню:", cancellationToken: cancellationToken, replyMarkup: MyKeyBoards.mainKeyBoard);
                                user.SetStage(2);
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
            case 4:
                {
                    switch (message.Text)
                    {

                        case "":
                            {
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
using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

class Program
{

    private static readonly string[] Predictions =
    {
        "Застрял на 10 минут? Улыбнись — день будет лёгким! 🙃",
        "Пробка на час? Тебя ждёт приятная встреча.😊 ",
        "Застрял на 2 часа? Настало время обдумать важные планы.😉",
        "3 часа в пробке? Жди перемен! 😁",
        "Полдня в пути? Тебе выпадет неожиданная удача! 😃"
    };

    static async Task Main()
    {
       
        var botClient = new TelegramBotClient("8245221462:AAE6YTVvjJYvvand8o4WV4ldPAx2D_Hko_I");

        using var cts = new CancellationTokenSource();

        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = Array.Empty<UpdateType>() 
        };

        botClient.StartReceiving(
            updateHandler: HandleUpdateAsync,
            errorHandler: HandleErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: cts.Token
        );

        var me = await botClient.GetMe();
        Console.WriteLine($"Бот {me.Username} запущен. Нажми Enter для выхода.");
        Console.ReadLine();

        cts.Cancel();
    }

    
    static async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
    {
        if (update.Type != UpdateType.Message || update.Message?.Text is null)
            return;

        var chatId = update.Message.Chat.Id;
        var messageText = update.Message.Text;

        string reply;

        
        if (int.TryParse(messageText, out int hours))
        {
            if (hours <= 0)
                reply = "Без пробок — без забот! Жди лёгкого дня.";
            else if (hours == 1)
                reply = Predictions[1];
            else if (hours == 2)
                reply = Predictions[2];
            else if (hours >= 3)
                reply = Predictions[3];
            else
                reply = Predictions[0];
        }
        else
        {
            
            var rnd = new Random();
            reply = Predictions[rnd.Next(Predictions.Length)];
        }

        await bot.SendMessage(
            chatId: chatId,
            text: reply,
            cancellationToken: cancellationToken
        );
    }

    
    static Task HandleErrorAsync(ITelegramBotClient bot, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Ошибка: {exception.Message}");
        return Task.CompletedTask;
    }
}
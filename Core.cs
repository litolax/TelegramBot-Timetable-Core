using Microsoft.Extensions.DependencyInjection;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;
using TelegramBot_Timetable_Core.Config;
using TelegramBot_Timetable_Core.Services;

namespace TelegramBot_Timetable_Core;

public class Core
{
    public delegate void MessageReceiveDelegate(Message message);

    public static MessageReceiveDelegate? OnMessageReceive;

    public static async Task Start()
    {
        var serviceProvider = new ServiceCollection()
            .AddSingleton<ISpamService, SpamService>()
            .AddSingleton<IMongoService, MongoService>()
            .BuildServiceProvider(true);

        var spamService = serviceProvider.GetService<ISpamService>()!;

        //Bot init
        var mainConfig = new Config<MainConfig>();
        var bot = new BotClient(mainConfig.Entries.Token);
        var updates = await bot.GetUpdatesAsync();

        // Set slash command to bot
        bot.SetMyCommands(
            new BotCommand("start", "Запустить приложение"),
            new BotCommand("help", "Помощь"), new BotCommand("menu", "Открыть меню"));

        Console.WriteLine("Bot started!");

        while (true)
        {
            if (!updates.Any())
            {
                updates = await bot.GetUpdatesAsync();
                continue;
            }

            foreach (var update in updates)
            {
                try
                {
                    switch (update.Type)
                    {
                        case UpdateType.Message:
                            if (update.Message is not { } message) continue;

                            if (message.Date < DateTimeOffset.UtcNow.AddMinutes(-3).ToUnixTimeSeconds())
                                continue;

                            if (message.From is not { } sender) continue;
                            if (sender.IsBot) continue;

                            if (spamService.IsSpammer(sender.Id)) continue;

                            if (updates.Count(u => u.Message?.From!.Id == sender.Id) >= 5)
                            {
                                spamService.AddToSpam(sender.Id);
                                bot.SendMessage(sender.Id,
                                    "Вы были добавлены в спам лист на 2 минуты. Не переживайте, передохните, и попробуйте еще раз");
                                continue;
                            }

                            OnMessageReceive?.Invoke(message);
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            var offset = updates.Last().UpdateId + 1;
            updates = await bot.GetUpdatesAsync(offset);
        }
    }
}
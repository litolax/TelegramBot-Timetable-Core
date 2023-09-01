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

    public delegate void ChannelPostReceiveDelegate(Message message);

    public static MessageReceiveDelegate? OnMessageReceive;
    public static ChannelPostReceiveDelegate? OnChannelPostReceive;

    public static List<long> Administrators = new();

    public static async Task Start(BotCommand[] botCommands)
    {
        var serviceProvider = new ServiceCollection()
            .AddSingleton<ISpamService, SpamService>()
            .AddSingleton<IBotService, BotService>()
            .AddSingleton<IMongoService, MongoService>()
            .AddSingleton(typeof(IConfig<>), typeof(Config<>))
            .BuildServiceProvider(true);

        var spamService = serviceProvider.GetService<ISpamService>()!;

        // Init admin list
        var config = serviceProvider.GetService<IConfig<MainConfig>>()!;
        if (config.Entries.Administrators is not null)
            foreach (var adminId in config.Entries.Administrators)
                Administrators.Add(adminId);

        // Bot initialization
        var botService = serviceProvider.GetService<IBotService>()!;

        var updates = Array.Empty<Update>();
        try
        {
            // Set slash command to bot
            botService.BotClient.SetMyCommands(botCommands);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        Console.WriteLine("Bot started!");
        botService.SendAdminMessage(new SendMessageArgs(0, "Bot started!"));
        
        while (true)
        {
            try
            {
                if (!updates.Any())
                {
                    updates = await botService.BotClient.GetUpdatesAsync();
                    continue;
                }

                foreach (var update in updates)
                {
                    switch (update.Type)
                    {
                        case UpdateType.Message:
                        {
                            if (update.Message is not { } message) continue;

                            if (message.Date < DateTimeOffset.UtcNow.AddMinutes(-3).ToUnixTimeSeconds())
                                continue;

                            if (message.From is not { } sender) continue;
                            if (sender.IsBot) continue;

                            if (spamService.IsSpammer(sender.Id)) continue;

                            if (updates.Count(u => u.Message?.From!.Id == sender.Id) >= 5)
                            {
                                spamService.AddToSpam(sender.Id);
                                botService.SendMessage(new SendMessageArgs(sender.Id,
                                    "Вы были добавлены в спам лист на 2 минуты. Не переживайте, передохните, и попробуйте еще раз"));
                                continue;
                            }

                            OnMessageReceive?.Invoke(message);
                            break;
                        }
                        case UpdateType.ChannelPost:
                        {
                            if (update.ChannelPost is not { } channelPost) continue;

                            OnChannelPostReceive?.Invoke(channelPost);
                            break;
                        }
                    }
                }

                var offset = updates.Last().UpdateId + 1;
                updates = await botService.BotClient.GetUpdatesAsync(offset);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
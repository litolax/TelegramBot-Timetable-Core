using System.Text.RegularExpressions;
using MongoDB.Driver;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using TelegramBot_Timetable_Core.Config;
using User = TelegramBot_Timetable_Core.Models.User;

namespace TelegramBot_Timetable_Core.Services;

public interface IBotService
{
    BotClient BotClient { get; set; }
    bool SendMessage(SendMessageArgs sendMessageArgs);
    Task<bool> SendMessageAsync(SendMessageArgs sendMessageArgs);
    bool SendPhoto(SendPhotoArgs sendPhotoArgs);
    Task<bool> SendPhotoAsync(SendPhotoArgs sendPhotoArgs);
    Task NotifyAllUsers(Message message);
}

public class BotService : IBotService
{
    private readonly IMongoService _mongoService;
    private Dictionary<string, List<PhotoSize>> _photos = new();
    private static readonly Regex SayRE = new(@"\/sayall(.+)", RegexOptions.Compiled);
    public BotClient BotClient { get; set; }

    public BotService(IConfig<MainConfig> config, IMongoService mongoService)
    {
        this._mongoService = mongoService;
        BotClient = new BotClient(config.Entries.Token);
    }


    public bool SendMessage(SendMessageArgs sendMessageArgs)
    {
        try
        {
            this.BotClient.SendMessage(sendMessageArgs);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    public async Task<bool> SendMessageAsync(SendMessageArgs sendMessageArgs)
    {
        try
        {
            await this.BotClient.SendMessageAsync(sendMessageArgs);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }
    
    public bool SendPhoto(SendPhotoArgs sendPhotoArgs)
    {
        try
        {
            this.BotClient.SendPhoto(sendPhotoArgs);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }
    
    public async Task<bool> SendPhotoAsync(SendPhotoArgs sendPhotoArgs)
    {
        try
        {
            await this.BotClient.SendPhotoAsync(sendPhotoArgs);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }
    
    public async Task NotifyAllUsers(Message message)
        {
            if (message.MediaGroupId is not null)
            {
                if (this._photos.TryGetValue(message.MediaGroupId, out var images))
                {
                    if (message.Photo is not null) images.Add(message.Photo.First());
                }
                else
                {
                    if (message.Photo is not null) this._photos.Add(message.MediaGroupId, new List<PhotoSize>()
                    {
                        message.Photo.First()
                    });
                }
            }

            var (result, messageText) = ValidationAllRegexNotification(message);
            if (!result && message.Poll is null) return;

            var userCollection = this._mongoService.Database.GetCollection<User>("Users");
            var users = (await userCollection.FindAsync(u => true)).ToList();
            if (users is null || users.Count <= 0) return;
            
            await Task.Delay(2000);
            
            var tasks = new List<Task>();
            List<InputMediaPhoto> mediaPhotos = new();

            if (message.MediaGroupId is not null)
            {
                foreach (var p in this._photos[message.MediaGroupId])
                {
                    mediaPhotos.Add(new InputMediaPhoto(p.FileId));
                }
            }

            
            foreach (var user in users)
            {
                tasks.Add(this.SendMessageAsync(new SendMessageArgs(user.UserId, $"📙Рассылка от бота📙: {messageText}")));
                try
                {
                    if (mediaPhotos.Count > 0) tasks.Add(this.BotClient.SendMediaGroupAsync(new SendMediaGroupArgs(user.UserId, mediaPhotos)));
                    
                    if (message.Poll is not null && message.From is not null)
                    {
                        tasks.Add(this.BotClient.ForwardMessageAsync(user.UserId, message.From.Id, 
                            message.MessageId));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            
            await Task.WhenAll(tasks);
            if (message.MediaGroupId is not null) this._photos[message.MediaGroupId].Clear();
        }

        private (bool result, string? messageText) ValidationAllRegexNotification(Message message)
        {
            var sayRegex = Match.Empty;

            if (message.Text is { } messageText)
            {
                sayRegex = SayRE.Match(messageText);
            }
            else if (message.Caption is { } msgCaption)
            {
                sayRegex = SayRE.Match(msgCaption);
            }

            return (sayRegex.Length > 0, sayRegex.Groups[1].Value.Trim());
        }
}
﻿using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using TelegramBot_Timetable_Core.Config;

namespace TelegramBot_Timetable_Core.Services;

public interface IBotService
{
    BotClient BotClient { get; set; }
    bool SendMessage(SendMessageArgs sendMessageArgs);
    Task<bool> SendMessageAsync(SendMessageArgs sendMessageArgs);
    bool SendPhoto(SendPhotoArgs sendPhotoArgs);
    Task<bool> SendPhotoAsync(SendPhotoArgs sendPhotoArgs);
}

public class BotService : IBotService
{
    public BotClient BotClient { get; set; }

    public BotService(IConfig<MainConfig> config)
    {
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
}
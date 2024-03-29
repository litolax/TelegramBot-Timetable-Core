﻿using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.UpdatingMessages;
using TelegramBot_Timetable_Core.Config;

namespace TelegramBot_Timetable_Core.Services;

public interface IBotService
{
    BotClient BotClient { get; set; }
    bool SendMessage(SendMessageArgs sendMessageArgs);
    Task<bool> SendMessageAsync(SendMessageArgs sendMessageArgs);
    bool SendPhoto(SendPhotoArgs sendPhotoArgs);
    Task<bool> SendPhotoAsync(SendPhotoArgs sendPhotoArgs);

    bool EditMessageText(long chatId, int messageId, string text, string parseMode = null,
        MessageEntity[] entities = null);

    Task<bool> EditMessageTextAsync(long chatId, int messageId, string text, string parseMode = null,
        MessageEntity[] entities = null);

    bool SendAdminMessage(SendMessageArgs sendMessageArgs);
    Task<bool> SendAdminMessageAsync(SendMessageArgs sendMessageArgs);
}

public class BotService : IBotService
{
    private readonly IConfig<MainConfig> _config;
    public BotClient BotClient { get; set; }

    public BotService(IConfig<MainConfig> config)
    {
        this._config = config;
        this.BotClient = new BotClient(config.Entries.Token);
    }


    public bool SendMessage(SendMessageArgs sendMessageArgs)
    {
        try
        {
            int chunkSize = 4000;

            if (sendMessageArgs.Text.Length > chunkSize)
            {
                int strLength = sendMessageArgs.Text.Length;
                for (int i = 0; i < strLength; i += chunkSize)
                {
                    if (i + chunkSize > strLength) chunkSize = strLength - i;

                    this.BotClient.SendMessage((long)sendMessageArgs.ChatId,
                        sendMessageArgs.Text.Substring(i, chunkSize), sendMessageArgs.MessageThreadId,
                        sendMessageArgs.ParseMode,
                        sendMessageArgs.Entities, sendMessageArgs.DisableWebPagePreview,
                        sendMessageArgs.DisableNotification, sendMessageArgs.ProtectContent,
                        sendMessageArgs.ReplyToMessageId, sendMessageArgs.AllowSendingWithoutReply,
                        sendMessageArgs.ReplyMarkup);
                }
            }
            else this.BotClient.SendMessage(sendMessageArgs);

            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    public bool SendAdminMessage(SendMessageArgs sendMessageArgs)
    {
        if (this._config.Entries.Administrators is null) return false;
        try
        {
            int chunkSize = 4000;

            if (sendMessageArgs.Text.Length > chunkSize)
            {
                int strLength = sendMessageArgs.Text.Length;
                for (int i = 0; i < strLength; i += chunkSize)
                {
                    if (i + chunkSize > strLength) chunkSize = strLength - i;

                    foreach (var administrator in this._config.Entries.Administrators)
                    {
                        this.BotClient.SendMessage(administrator,
                            sendMessageArgs.Text.Substring(i, chunkSize), sendMessageArgs.MessageThreadId,
                            sendMessageArgs.ParseMode,
                            sendMessageArgs.Entities, sendMessageArgs.DisableWebPagePreview,
                            sendMessageArgs.DisableNotification, sendMessageArgs.ProtectContent,
                            sendMessageArgs.ReplyToMessageId, sendMessageArgs.AllowSendingWithoutReply,
                            sendMessageArgs.ReplyMarkup);
                    }
                }
            }
            else
            {
                foreach (var administrator in this._config.Entries.Administrators)
                {
                    this.BotClient.SendMessage(administrator,
                        sendMessageArgs.Text,
                        sendMessageArgs.MessageThreadId,
                        sendMessageArgs.ParseMode,
                        sendMessageArgs.Entities, sendMessageArgs.DisableWebPagePreview,
                        sendMessageArgs.DisableNotification, sendMessageArgs.ProtectContent,
                        sendMessageArgs.ReplyToMessageId, sendMessageArgs.AllowSendingWithoutReply,
                        sendMessageArgs.ReplyMarkup);
                }
            }

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
            int chunkSize = 4000;

            if (sendMessageArgs.Text.Length > chunkSize)
            {
                int strLength = sendMessageArgs.Text.Length;
                for (int i = 0; i < strLength; i += chunkSize)
                {
                    if (i + chunkSize > strLength) chunkSize = strLength - i;

                    await this.BotClient.SendMessageAsync((long)sendMessageArgs.ChatId,
                        sendMessageArgs.Text.Substring(i, chunkSize), sendMessageArgs.MessageThreadId,
                        sendMessageArgs.ParseMode,
                        sendMessageArgs.Entities, sendMessageArgs.DisableWebPagePreview,
                        sendMessageArgs.DisableNotification, sendMessageArgs.ProtectContent,
                        sendMessageArgs.ReplyToMessageId, sendMessageArgs.AllowSendingWithoutReply,
                        sendMessageArgs.ReplyMarkup);
                }
            }
            else await this.BotClient.SendMessageAsync(sendMessageArgs);

            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    public async Task<bool> SendAdminMessageAsync(SendMessageArgs sendMessageArgs)
    {
        if (this._config.Entries.Administrators is null) return false;
        try
        {
            int chunkSize = 4000;

            if (sendMessageArgs.Text.Length > chunkSize)
            {
                int strLength = sendMessageArgs.Text.Length;
                for (int i = 0; i < strLength; i += chunkSize)
                {
                    if (i + chunkSize > strLength) chunkSize = strLength - i;

                    foreach (var administrator in this._config.Entries.Administrators)
                    {
                        await this.BotClient.SendMessageAsync(administrator,
                            sendMessageArgs.Text.Substring(i, chunkSize), sendMessageArgs.MessageThreadId,
                            sendMessageArgs.ParseMode,
                            sendMessageArgs.Entities, sendMessageArgs.DisableWebPagePreview,
                            sendMessageArgs.DisableNotification, sendMessageArgs.ProtectContent,
                            sendMessageArgs.ReplyToMessageId, sendMessageArgs.AllowSendingWithoutReply,
                            sendMessageArgs.ReplyMarkup);
                    }
                }
            }
            else
            {
                foreach (var administrator in this._config.Entries.Administrators)
                {
                    await this.BotClient.SendMessageAsync(administrator,
                        sendMessageArgs.Text,
                        sendMessageArgs.MessageThreadId,
                        sendMessageArgs.ParseMode,
                        sendMessageArgs.Entities, sendMessageArgs.DisableWebPagePreview,
                        sendMessageArgs.DisableNotification, sendMessageArgs.ProtectContent,
                        sendMessageArgs.ReplyToMessageId, sendMessageArgs.AllowSendingWithoutReply,
                        sendMessageArgs.ReplyMarkup);
                }
            }

            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    public bool EditMessageText(long chatId, int messageId, string text, string parseMode = null,
        MessageEntity[] entities = null)
    {
        try
        {
            this.BotClient.EditMessageText(chatId, messageId, text, parseMode, entities);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    public async Task<bool> EditMessageTextAsync(long chatId, int messageId, string text, string parseMode = null,
        MessageEntity[] entities = null)
    {
        try
        {
            await this.BotClient.EditMessageTextAsync(chatId, messageId, text, parseMode, entities);
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
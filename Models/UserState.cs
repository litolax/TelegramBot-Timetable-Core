using MongoDB.Bson;

namespace TelegramBot_Timetable_Core.Models;

public class UserState
{
    public UserState(long chatId, string stateKey)
    {
        this.ChatId = chatId;
        this.StateKey = stateKey;
    }
    
    public ObjectId Id { get; set; }
    public long ChatId { get; set; }
    public string StateKey { get; set; }
}
using MongoDB.Bson;

namespace TelegramBot_Timetable_Core.Models;

public class StudentsUser
{
    public ObjectId Id { get; set; }
    public long UserId { get; set; }
    public string? Username { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Group { get; set; }
    public bool Notifications { get; set; }

    public StudentsUser(long userId, string? username, string firstName, string? lastName)
    {
        this.UserId = userId;
        this.Username = username;
        this.FirstName = firstName;
        this.LastName = lastName;
    }
}
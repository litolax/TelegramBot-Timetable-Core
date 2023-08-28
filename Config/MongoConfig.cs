namespace TelegramBot_Timetable_Core.Config
{
    public record MongoConfig(
        string DbName, string AuthDb, string Host, int Port, string AuthorizationName, string AuthorizationPassword
    );
}
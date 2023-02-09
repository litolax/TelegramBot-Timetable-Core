﻿using MongoDB.Driver;
using TelegramBot_Timetable_Core.Config;
using TelegramBot_Timetable_Core.Models;

namespace TelegramBot_Timetable_Core.Services
{
    public interface IMongoService
    {
        IMongoDatabase Database { get; set; }
        Task<string?> GetLastState(long chatId);
        void CreateState(UserState state);
        void RemoveState(long chatId);
    }

    public class MongoService : IMongoService
    {
        public string TableDBName;

        private static MongoClientSettings Settings;

        public MongoClient Client;
        public IMongoDatabase Database { get; set; }

        public MongoService()
        {
            this.GetSettings();
        }

        public void GetSettings()
        {
            var mongoConfig = new Config<MongoConfig>();
            this.TableDBName = mongoConfig.Entries.DbName;
#if !DEBUG
            Settings = new()
            {
                Server = new MongoServerAddress(mongoConfig.Entries.Host, mongoConfig.Entries.Port),
                Credential = MongoCredential.CreateCredential(mongoConfig.Entries.DbName,
                    mongoConfig.Entries.AuthorizationName, mongoConfig.Entries.AuthorizationPassword)
            };
            Client = new(Settings);
            Database = Client.GetDatabase(TableDBName);
#endif
         
#if DEBUG
            Client = new("mongodb://localhost:27017/?readPreference=primary&appname=MongoDB%20Compass&directConnection=true&ssl=false");
            Database = Client.GetDatabase(TableDBName);
#endif
        }

        public async Task<string?> GetLastState(long chatId)
        {
            var userStatesCollection = Database.GetCollection<UserState>("UserStates");
            var state = (await userStatesCollection.FindAsync(s => s.ChatId == chatId)).ToList();
            if (state is null || state.Count <= 0) return null;
            return state.First().StateKey;
        }

        public void CreateState(UserState state)
        {
            var userStatesCollection = Database.GetCollection<UserState>("UserStates");
            userStatesCollection.InsertOne(state);
        }
        
        public void RemoveState(long chatId)
        {
            var userStatesCollection = Database.GetCollection<UserState>("UserStates");
            userStatesCollection.DeleteMany(s => s.ChatId == chatId);
        }
    }
}
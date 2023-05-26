using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using TelegramBotApi.Model1;

namespace Kursach
{
    public class Constants
    {
        public static string address = "https://telegrambotapi20230525124723.azurewebsites.net/";
        public static MongoClient mongoClient;
        public static IMongoDatabase database;
        public static IMongoCollection<BsonDocument> collection0;
        public static IMongoCollection<BsonDocument> movie_list;
        public static IMongoCollection<BsonDocument> selected_list;
        public static Selected_Array selected_movie;
        public static Movie watched_movie = new Movie();
    }
}

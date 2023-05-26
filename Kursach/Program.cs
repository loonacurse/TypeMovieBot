using Kursach.Client;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using MongoDB.Driver;
using MongoDB.Bson;
using Constants = Kursach.Constants;
using TelegramBotApi.Client2;
using TelegramBotApi;
using Kursach.Client1;
using Message = Telegram.Bot.Types.Message;
using System.Text.RegularExpressions;
using UpdateType = Telegram.Bot.Types.Enums.UpdateType;

namespace TestChatBot
{
    class Program
    {
        public static void Main(string[] args)
        {
            Constants.mongoClient = new MongoClient("mongodb+srv://loonacurse:04(Darinka)10@typemovie.n4kn7xo.mongodb.net/");
            Constants.database = Constants.mongoClient.GetDatabase("typemovie");
            Constants.collection0 = Constants.database.GetCollection<BsonDocument>("collection0");
            Constants.movie_list = Constants.database.GetCollection<BsonDocument>("movie_list");
            Constants.selected_list = Constants.database.GetCollection<BsonDocument>("selected_list");
            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var bot = new TelegramBotClient("6053790778:AAFNrbvBcG5_L2wSC3VBMXaW4dvL59CNiUc");
            ReceiverOptions receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }
            };
            bot.StartReceiving(UpdateAsync, ErrorAsync, receiverOptions, cancellationToken);
            var builder = WebApplication.CreateBuilder(args);
            // Add services to the container.
            builder.Services.AddRazorPages();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
            //Console.ReadLine();
        }
        public static async Task UpdateAsync(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
            {
                var message = update.Message;
                try
                {
                    if (update.Type == UpdateType.Message && update?.Message?.Text != null)
                    {
                        await HandleMessageAsync(bot, update.Message);
                        return;
                    }
                    else if (update.Type == UpdateType.CallbackQuery)
                    {
                        await HandleCallBackQueryAsync(bot, update.CallbackQuery);
                        return;
                    }
                    else if (update?.Message?.Text != null)
                    {
                        await bot.SendTextMessageAsync(update.Message.Chat.Id, "☆я не вмію працювати з таким форматом повідомлень☆");
                        await bot.SendStickerAsync(update.Message.Chat.Id,
                             InputFile.FromUri("https://stickerswiki.ams3.cdn.digitaloceanspaces.com/GanwIrdou11/925294.160.webp")
                              );
                        UpdateDatabase(message);
                        return;
                    }
                    else
                    {
                        return;
                    }
                }
                catch (Exception e)
                {
                    await bot.SendTextMessageAsync(update.Message.Chat.Id, $"☆сталася помилка☆");
                    await bot.SendStickerAsync(update.Message.Chat.Id,
                           InputFile.FromUri("https://stickerswiki.ams3.cdn.digitaloceanspaces.com/GanwIrdou11/925844.512.webp")
                             );
                    UpdateDatabase(message);
                }
            }
        public static async Task HandleMessageAsync(ITelegramBotClient bot, Message message)
            {
                ReplyKeyboardMarkup keyboard = new(new[]
                {
                    new KeyboardButton[] { "☆меню☆", "☆пошук фільму☆" },
                    new KeyboardButton[] { "☆список переглянутих фільмів☆" },
                    new KeyboardButton[] { "☆список обраних фільмів☆" }
                })
                {
                    ResizeKeyboard = true
                };
                var document = new BsonDocument
                {
                                    { "user_id", message.Chat.Id},
                                    { "user_firstname", message.Chat.FirstName},
                                    {"bot_is_waiting_for_movie_name", false },
                                    {"movie_name", ""},
                                    {"bot_is_waiting_for_person_name", false },
                                    {"person_name", "" },
                                    {"bot_is_waiting_for_ADD_selected_movie", false },
                                    {"bot_is_waiting_for_DELETE_selected_movie", false },
                                    {"bot_is_waiting_for_UPDATE_selected_movie_NAME", false },
                                    {"bot_is_waiting_for_UPDATE_selected_movie", false },
                                    {"bot_is_waiting_for_ADD_watched_movie_NAME", false },
                                    {"bot_is_waiting_for_ADD_watched_movie_RATE", false },
                                    {"bot_is_waiting_for_ADD_watched_movie_COMMENT", false },
                                    {"bot_is_waiting_for_DELETE_watched_movie", false },
                                    {"bot_is_waiting_for_UPDATE_watched_movie", false },
                                    {"bot_is_waiting_for_UPDATE_watched_movie_NAME", false },
                                    {"bot_is_waiting_for_UPDATE_watched_movie_RATE", false },
                                    {"bot_is_waiting_for_UPDATE_watched_movie_COMMENT", false },
                                    {"bot_is_waiting_for_GETone_watched_movie", false },
                };
                var filter = Builders<BsonDocument>.Filter.Eq("user_id", message.Chat.Id);
                var exists = Constants.collection0.Find(filter).Any();
                if (!exists)
                {
                    Constants.collection0.InsertOne(document);
                }
                BsonDocument updatedDocument = Constants.collection0.Find(filter).FirstOrDefault();
                bool bot_is_waiting_for_movie_name = updatedDocument["bot_is_waiting_for_movie_name"].AsBoolean;
                bool bot_is_waiting_for_person_name = updatedDocument["bot_is_waiting_for_person_name"].AsBoolean;
                bool bot_is_waiting_for_ADD_selected_movie = updatedDocument["bot_is_waiting_for_ADD_selected_movie"].AsBoolean;
                bool bot_is_waiting_for_DELETE_selected_movie = updatedDocument["bot_is_waiting_for_DELETE_selected_movie"].AsBoolean;
                bool bot_is_waiting_for_UPDATE_selected_movie = updatedDocument["bot_is_waiting_for_UPDATE_selected_movie"].AsBoolean;
                bool bot_is_waiting_for_UPDATE_selected_movie_NAME = updatedDocument["bot_is_waiting_for_UPDATE_selected_movie_NAME"].AsBoolean;
                bool bot_is_waiting_for_ADD_watched_movie_NAME = updatedDocument["bot_is_waiting_for_ADD_watched_movie_NAME"].AsBoolean;
                bool bot_is_waiting_for_ADD_watched_movie_RATE = updatedDocument["bot_is_waiting_for_ADD_watched_movie_RATE"].AsBoolean;
                bool bot_is_waiting_for_ADD_watched_movie_COMMENT = updatedDocument["bot_is_waiting_for_ADD_watched_movie_COMMENT"].AsBoolean;
                bool bot_is_waiting_for_DELETE_watched_movie = updatedDocument["bot_is_waiting_for_DELETE_watched_movie"].AsBoolean;
                bool bot_is_waiting_for_UPDATE_watched_movie = updatedDocument["bot_is_waiting_for_UPDATE_watched_movie"].AsBoolean;
                bool bot_is_waiting_for_UPDATE_watched_movie_NAME = updatedDocument["bot_is_waiting_for_UPDATE_watched_movie_NAME"].AsBoolean;
                bool bot_is_waiting_for_UPDATE_watched_movie_RATE = updatedDocument["bot_is_waiting_for_UPDATE_watched_movie_RATE"].AsBoolean;
                bool bot_is_waiting_for_UPDATE_watched_movie_COMMENT = updatedDocument["bot_is_waiting_for_UPDATE_watched_movie_COMMENT"].AsBoolean;
                bool bot_is_waiting_for_GETone_watched_movie = updatedDocument["bot_is_waiting_for_GETone_watched_movie"].AsBoolean;
                if (bot_is_waiting_for_movie_name)
                {
                    try
                    {
                        UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Set("bot_is_waiting_for_movie_name", false);
                        UpdateResult result = Constants.collection0.UpdateMany(filter, update);
                        if (Regex.IsMatch(message.Text, @"^\w", RegexOptions.IgnoreCase))
                        {
                            update = Builders<BsonDocument>.Update.Set("movie_name", message.Text);
                            result = Constants.collection0.UpdateOne(filter, update);
                            updatedDocument = Constants.collection0.Find(filter).FirstOrDefault();
                            string movie_name = updatedDocument["movie_name"].AsString;
                            MovieInfoClient movieInfoClient = new MovieInfoClient();
                            var result1 = await movieInfoClient.GetMovieInfoAsync(movie_name);
                            await bot.SendTextMessageAsync(message.Chat.Id, "☆Зачекай, доки я знайду для тебе всі фільми☆");
                            Thread.Sleep(2000);
                            if (result1.Results.Length != 0)
                            {
                                for (int i = 0; i < result1.Results.Length; i++)
                                {
                                    await bot.SendTextMessageAsync(message.Chat.Id, $"☆назва фільму --> {result1.Results[i].Title}\n\n" +
                                        $"☆опис фільму --> {result1.Results[i].Overview}\n\n" +
                                        $"☆оцінка фільму --> {result1.Results[i].Vote_average}\n\n");
                                }
                                Thread.Sleep(1000);
                                await bot.SendTextMessageAsync(message.Chat.Id, "☆Ось усі фільми, які мені вдалося знайти☆");
                            }
                            else
                            {
                                await bot.SendTextMessageAsync(message.Chat.Id, "☆вибач, я не зміг нічого знайти☆");
                                await bot.SendStickerAsync(message.Chat.Id,
                                   InputFile.FromUri("https://stickerswiki.ams3.cdn.digitaloceanspaces.com/GanwIrdou11/925304.160.webp")
                                     );
                                Thread.Sleep(1000);
                                await bot.SendTextMessageAsync(message.Chat.Id, "☆ти точно правильно написав?☆", replyToMessageId: message.MessageId);
                            }
                            update = Builders<BsonDocument>.Update.Set("bot_is_waiting_for_movie_name", false);
                            result = Constants.collection0.UpdateMany(filter, update);
                            return;
                        }
                        else
                        {
                            await bot.SendTextMessageAsync(message.Chat.Id, "☆Назва фільму не має починатися із символу☆");
                            await bot.SendTextMessageAsync(message.Chat.Id, "☆спробуй ще раз☆");
                        }

                    }
                    catch (Exception ex)
                    {
                        await bot.SendTextMessageAsync(message.Chat.Id, "☆виникла помилка запиту☆");
                        await bot.SendStickerAsync(message.Chat.Id,
                            InputFile.FromUri("https://stickerswiki.ams3.cdn.digitaloceanspaces.com/kittykittykitty_by_fStikBot/975533.512.webp")
                              );
                    }
                }
                else if (bot_is_waiting_for_person_name)
                {
                    try
                    {
                        if (Regex.IsMatch(message.Text, @"^\w", RegexOptions.IgnoreCase))
                        {
                            UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Set("bot_is_waiting_for_person_name", false);
                            UpdateResult result = Constants.collection0.UpdateMany(filter, update);
                            update = Builders<BsonDocument>.Update.Set("person_name", message.Text);
                            result = Constants.collection0.UpdateOne(filter, update);
                            updatedDocument = Constants.collection0.Find(filter).FirstOrDefault();
                            string person_name = updatedDocument["person_name"].AsString;
                            GetActorMovieClient getActorMovieClient = new GetActorMovieClient();
                            var result2 = await getActorMovieClient.GetActorMovieAsync(person_name);
                            await bot.SendTextMessageAsync(message.Chat.Id, "☆Зачекай, доки я знайду для тебе всі фільми☆");
                            Thread.Sleep(2000);
                            if (result2.results.Length != 0)
                            {
                                bool flag = true;
                                for (int i = 0; flag == true && i < result2.results.Length; i++)
                                {
                                    await bot.SendTextMessageAsync(message.Chat.Id, $"☆Ім'я --> {result2.results[i].Name}\n\n");
                                    if (result2.results[i].Known_for.Count != 0)
                                    {
                                        for (int j = 0; j < result2.results[i].Known_for.Count; j++)
                                        {
                                            await bot.SendTextMessageAsync(message.Chat.Id, $"☆назва фільму --> {result2.results[i].Known_for[j].title}\n\n" +
                                            $"☆опис фільму --> {result2.results[i].Known_for[j].overview}\n\n" +
                                            $"☆оцінка фільму --> {result2.results[i].Known_for[j].vote_average}\n\n\n\n\n");
                                        }
                                    }
                                    else
                                    {
                                        await bot.SendTextMessageAsync(message.Chat.Id, "☆вибач, я не зміг нічого знайти☆");
                                        await bot.SendStickerAsync(message.Chat.Id,
                                       InputFile.FromUri("https://stickerswiki.ams3.cdn.digitaloceanspaces.com/GanwIrdou11/925304.160.webp")
                                         );
                                        flag = false;
                                    }
                                }
                            }
                            else
                            {
                                await bot.SendTextMessageAsync(message.Chat.Id, "☆вибач, я не зміг нічого знайти☆");
                                await bot.SendStickerAsync(message.Chat.Id,
                                     InputFile.FromUri("https://stickerswiki.ams3.cdn.digitaloceanspaces.com/GanwIrdou11/925304.160.webp")
                                       );
                                Thread.Sleep(1000);
                                await bot.SendTextMessageAsync(message.Chat.Id, "☆ти точно правильно написав?☆", replyToMessageId: message.MessageId);
                            }
                            update = Builders<BsonDocument>.Update.Set("bot_is_waiting_for_person_name", false);
                            result = Constants.collection0.UpdateMany(filter, update);
                            return;
                        }
                        else
                        {
                            await bot.SendTextMessageAsync(message.Chat.Id, "☆Назва фільму не має починатися із символу☆");
                            await bot.SendTextMessageAsync(message.Chat.Id, "☆спробуй ще раз☆");
                        }
                    }
                    catch (Exception ex)
                    {
                        await bot.SendTextMessageAsync(message.Chat.Id, "☆виникла помилка запиту☆");
                        await bot.SendStickerAsync(message.Chat.Id,
                            InputFile.FromUri("https://stickerswiki.ams3.cdn.digitaloceanspaces.com/kittykittykitty_by_fStikBot/975533.512.webp")
                              );
                    }
                }
                else if (bot_is_waiting_for_ADD_selected_movie)
                {
                    try
                    {
                        UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Set("bot_is_waiting_for_ADD_selected_movie", false);
                        UpdateResult result = Constants.collection0.UpdateMany(filter, update);
                        SelectedMovieClient selectedMovieClient = new SelectedMovieClient();
                        var _result = await selectedMovieClient.AddSelectedMovieAsync(message.Text, message.Chat.Id);
                        await bot.SendTextMessageAsync(message.Chat.Id, _result);
                        return;
                    }
                    catch
                    {
                        throw new Exception();
                    }
                }
                else if (bot_is_waiting_for_DELETE_selected_movie)
                {
                    try
                    {
                        int movie_number = int.Parse(message.Text);
                        if (movie_number >= 1)
                        {
                            SelectedMovieClient selectedMovieClient = new SelectedMovieClient();
                            var general_selected_movie_list = await selectedMovieClient.GetSelectedMovieListAsync(message.Chat.Id);
                            for (int i = 0; i < general_selected_movie_list.Count; i++)
                            {
                                if (movie_number == i + 1)
                                {
                                    Constants.selected_movie = general_selected_movie_list[i];
                                }
                            }
                            if (general_selected_movie_list.Count > movie_number)
                            {
                                var _result = await selectedMovieClient.DeleteSelectedMovieAsync(Constants.selected_movie.Movie_name, message.Chat.Id);
                                await bot.SendTextMessageAsync(message.Chat.Id, _result);
                            }
                            else
                            {
                                await bot.SendTextMessageAsync(message.Chat.Id, "☆фільму під таким номером не існує☆");
                            }
                            UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Set("bot_is_waiting_for_DELETE_selected_movie", false);
                            UpdateResult result = Constants.collection0.UpdateMany(filter, update);
                        }
                        else
                        {
                            await bot.SendTextMessageAsync(message.Chat.Id, "☆ти увів якийсь дивний номер фільму☆");
                            await bot.SendTextMessageAsync(message.Chat.Id, "☆спробуй ще раз☆");
                        }
                    }
                    catch
                    {
                        throw new Exception();
                    }
                }
                else if (bot_is_waiting_for_UPDATE_selected_movie)
                {
                    try
                    {
                        int movie_number = int.Parse(message.Text);
                        if (movie_number >= 1)
                        {
                            SelectedMovieClient selectedMovieClient = new SelectedMovieClient();
                            var general_selected_movie_list = await selectedMovieClient.GetSelectedMovieListAsync(message.Chat.Id);
                            for (int i = 0; i < general_selected_movie_list.Count; i++)
                            {
                                if (movie_number == i + 1)
                                {
                                    Constants.selected_movie = general_selected_movie_list[i];
                                    exists = true;
                                }
                            }
                            if (general_selected_movie_list.Count >= movie_number)
                            {
                                UpdateDefinition<BsonDocument> inner_update = Builders<BsonDocument>.Update.Set("bot_is_waiting_for_UPDATE_selected_movie_NAME", true);
                                UpdateResult inner_result = Constants.collection0.UpdateMany(filter, inner_update);
                                await bot.SendTextMessageAsync(message.Chat.Id, "☆Напиши нову назву☆");
                            }
                            else
                            {
                                await bot.SendTextMessageAsync(message.Chat.Id, "☆фільму під таким номером не існує☆");
                            }
                            UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Set("bot_is_waiting_for_UPDATE_selected_movie", false);
                            UpdateResult result = Constants.collection0.UpdateMany(filter, update);
                        }
                        else
                        {
                            await bot.SendTextMessageAsync(message.Chat.Id, "☆ти увів якийсь дивний номер фільму☆");
                            await bot.SendTextMessageAsync(message.Chat.Id, "☆спробуй ще раз☆");
                        }

                    }
                    catch
                    {
                        throw new Exception();
                    }
                }
                else if (bot_is_waiting_for_UPDATE_selected_movie_NAME)
                {
                    UpdateDefinition<BsonDocument> inner_update = Builders<BsonDocument>.Update.Set("bot_is_waiting_for_UPDATE_selected_movie_NAME", false);
                    UpdateResult inner_result = Constants.collection0.UpdateMany(filter, inner_update);
                    if (Constants.selected_movie != null)
                    {
                        SelectedMovieClient selectedMovieClient = new SelectedMovieClient();
                        var _result = await selectedMovieClient.UpdateSelectedMovieAsync(Constants.selected_movie.Movie_name, message.Text, message.Chat.Id);
                        await bot.SendTextMessageAsync(message.Chat.Id, _result);
                    }
                    else
                    {
                        await bot.SendTextMessageAsync(message.Chat.Id, "☆не вдалося додати фільм до списку обраних☆");
                    }
                    return;
                }
                else if (bot_is_waiting_for_ADD_watched_movie_NAME)
                {
                    try
                    {
                        UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update
                            .Set("bot_is_waiting_for_ADD_watched_movie_NAME", false)
                            .Set("bot_is_waiting_for_ADD_watched_movie_RATE", true);
                        UpdateResult result = Constants.collection0.UpdateMany(filter, update);
                        Constants.watched_movie.Movie_name = message.Text;
                        await bot.SendTextMessageAsync(message.Chat.Id, "☆напиши рейтинг фільму☆\n\n(рейтинг має бути в діапазоні від 1 до 10)");
                        return;
                    }
                    catch
                    {
                        throw new Exception();
                    }
                }
                else if (bot_is_waiting_for_ADD_watched_movie_RATE)
                {
                    try
                    {
                        var movie_rate = int.Parse(message.Text);
                        if (movie_rate > 10 || movie_rate < 1)
                        {
                            await bot.SendTextMessageAsync(message.Chat.Id, "☆рентинг не може бути більше 10 чи менше 1☆");
                            await bot.SendTextMessageAsync(message.Chat.Id, "☆напиши рейтинг ще раз☆");
                        }
                        else
                        {
                            Constants.watched_movie.Movie_rate = movie_rate;
                            await bot.SendTextMessageAsync(message.Chat.Id, "☆напиши коментар до фільму☆\n\n(якщо не знаєш, що написати, то просто відправ --> \" - \")");
                            UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update
                                .Set("bot_is_waiting_for_ADD_watched_movie_RATE", false)
                                .Set("bot_is_waiting_for_ADD_watched_movie_COMMENT", true);
                            UpdateResult result = Constants.collection0.UpdateMany(filter, update);
                        }
                        return;
                    }
                    catch
                    {
                        throw new Exception();
                    }
                }
                else if (bot_is_waiting_for_ADD_watched_movie_COMMENT)
                {
                    try
                    {
                        UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update
                            .Set("bot_is_waiting_for_ADD_watched_movie_COMMENT", false);
                        UpdateResult result = Constants.collection0.UpdateMany(filter, update);
                        Constants.watched_movie.Movie_comment = message.Text;
                        WatchedMovieClient watchedMovieClient = new WatchedMovieClient();
                        var _result = await watchedMovieClient.AddWatchedMovieAsync(Constants.watched_movie, message.Chat.Id);
                        await bot.SendTextMessageAsync(message.Chat.Id, _result);
                        return;
                    }
                    catch
                    {
                        throw new Exception();
                    }
                }
                else if (bot_is_waiting_for_DELETE_watched_movie)
                {
                    try
                    {
                        int movie_number = int.Parse(message.Text);
                        if (movie_number >= 1)
                        {
                            WatchedMovieClient watchedMovieClient = new WatchedMovieClient();
                            var general_watched_movie_list = await watchedMovieClient.GetWatchedMovieListAsync(message.Chat.Id);
                            for (int i = 0; i < general_watched_movie_list.Count; i++)
                            {
                                if (movie_number == i + 1)
                                {
                                    Constants.watched_movie = general_watched_movie_list[i];
                                    exists = true;
                                }
                            }
                            if (general_watched_movie_list.Count >= movie_number)
                            {
                                var _result = await watchedMovieClient.DeleteWatchedMovieAsync(Constants.watched_movie.Movie_name, message.Chat.Id);
                                await bot.SendTextMessageAsync(message.Chat.Id, _result);
                            }
                            else
                            {
                                await bot.SendTextMessageAsync(message.Chat.Id, "☆фільму під таким номером не існує☆");
                            }
                            UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Set("bot_is_waiting_for_DELETE_watched_movie", false);
                            UpdateResult result = Constants.collection0.UpdateMany(filter, update);
                        }
                        else
                        {
                            await bot.SendTextMessageAsync(message.Chat.Id, "☆ти увів якийсь дивний номер фільму☆");
                            await bot.SendTextMessageAsync(message.Chat.Id, "☆спробуй ще раз☆");
                        }
                    }
                    catch
                    {
                        throw new Exception();
                    }
                }
                else if (bot_is_waiting_for_UPDATE_watched_movie)
                {
                    try
                    {
                        int movie_number = int.Parse(message.Text);
                        if (movie_number >= 1)
                        {
                            WatchedMovieClient watchedMovieClient = new WatchedMovieClient();
                            var general_watched_movie_list = await watchedMovieClient.GetWatchedMovieListAsync(message.Chat.Id);
                            for (int i = 0; i < general_watched_movie_list.Count; i++)
                            {
                                if (movie_number == i + 1)
                                {
                                    Constants.watched_movie = general_watched_movie_list[i];
                                }
                            }
                            if (general_watched_movie_list.Count < movie_number)
                            {
                                await bot.SendTextMessageAsync(message.Chat.Id, "☆фільму під таким номером не існує☆");

                            }
                            else
                            {
                                InlineKeyboardMarkup update_watched_movie_inlineKey = new(new[]
                                  {
                                            new[]
                                            {
                                                InlineKeyboardButton.WithCallbackData("☆назву☆", "змінити_назву"),
                                            },
                                            new[]
                                            {
                                                InlineKeyboardButton.WithCallbackData("☆рейтинг☆", "змінити_рейтинг"),
                                            },
                                            new[]
                                            {
                                                InlineKeyboardButton.WithCallbackData("☆коментар☆", "змінити_коментар"),
                                            }
                                });
                                await bot.SendTextMessageAsync(message.Chat.Id, "☆що будемо змінювати?☆", replyMarkup: update_watched_movie_inlineKey);
                            }
                            UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Set("bot_is_waiting_for_UPDATE_watched_movie", false);
                            UpdateResult result = Constants.collection0.UpdateMany(filter, update);
                        }
                        else
                        {
                            await bot.SendTextMessageAsync(message.Chat.Id, "☆ти увів якийсь дивний номер фільму☆");
                            await bot.SendTextMessageAsync(message.Chat.Id, "☆спробуй ще раз☆");
                        }

                    }
                    catch
                    {
                        throw new Exception();
                    }
                }
                else if (bot_is_waiting_for_UPDATE_watched_movie_NAME)
                {
                    try
                    {
                        UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Set("bot_is_waiting_for_UPDATE_watched_movie_NAME", false);
                        UpdateResult result = Constants.collection0.UpdateMany(filter, update);
                        WatchedMovieClient watchedMovieClient = new WatchedMovieClient();
                        var _result = await watchedMovieClient.UpdateWatchedMovieNameAsync(Constants.watched_movie.Movie_name, message.Text, message.Chat.Id);
                        await bot.SendTextMessageAsync(message.Chat.Id, _result);
                        return;
                    }
                    catch
                    {
                        throw new Exception();
                    }
                }
                else if (bot_is_waiting_for_UPDATE_watched_movie_RATE)
                {
                    try
                    {
                        var new_rate = int.Parse(message.Text);
                        if (new_rate < 10 && new_rate >= 1)
                        {
                            WatchedMovieClient watchedMovieClient = new WatchedMovieClient();
                            var _result = await watchedMovieClient.UpdateWatchedMovieRateAsync(Constants.watched_movie.Movie_name, int.Parse(message.Text), message.Chat.Id);
                            await bot.SendTextMessageAsync(message.Chat.Id, _result);
                            UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Set("bot_is_waiting_for_UPDATE_watched_movie_RATE", false);
                            UpdateResult result = Constants.collection0.UpdateMany(filter, update);
                        }
                        else
                        {
                            await bot.SendTextMessageAsync(message.Chat.Id, "☆рентинг не може бути більше 10 чи менше 1☆");
                            await bot.SendTextMessageAsync(message.Chat.Id, "☆спробуй ще раз☆");
                        }

                        return;
                    }
                    catch
                    {
                        throw new Exception();
                    }
                }
                else if (bot_is_waiting_for_UPDATE_watched_movie_COMMENT)
                {
                    try
                    {
                        UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Set("bot_is_waiting_for_UPDATE_watched_movie_COMMENT", false);
                        UpdateResult result = Constants.collection0.UpdateMany(filter, update);
                        WatchedMovieClient watchedMovieClient = new WatchedMovieClient();
                        var _result = await watchedMovieClient.UpdateWatchedMovieCommentAsync(Constants.watched_movie.Movie_name, message.Text, message.Chat.Id);
                        await bot.SendTextMessageAsync(message.Chat.Id, _result);
                        return;
                    }
                    catch
                    {
                        throw new Exception();
                    }
                }
                else if (bot_is_waiting_for_GETone_watched_movie)
                {
                    try
                    {
                        int movie_number = int.Parse(message.Text);
                        if (movie_number >= 1)
                        {
                            WatchedMovieClient watchedMovieClient = new WatchedMovieClient();
                            var general_watched_movie_list = await watchedMovieClient.GetWatchedMovieListAsync(message.Chat.Id);
                            for (int i = 0; i < general_watched_movie_list.Count; i++)
                            {
                                if (movie_number == i + 1)
                                {
                                    await bot.SendTextMessageAsync(message.Chat.Id, $"☆    {i + 1}    ☆\n\n☆назва фільму --> {general_watched_movie_list[i].Movie_name}\n\n☆рейтинг фільму --> {general_watched_movie_list[i].Movie_rate}\n\n" +
                                         $"☆коментар до фільму --> {general_watched_movie_list[i].Movie_comment}");
                                }
                            }
                            if (general_watched_movie_list.Count < movie_number)
                            {
                                await bot.SendTextMessageAsync(message.Chat.Id, "☆фільму під таким номером не існує☆");
                            }
                            UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Set("bot_is_waiting_for_GETone_watched_movie", false);
                            UpdateResult result = Constants.collection0.UpdateMany(filter, update);
                        }
                        else
                        {
                            await bot.SendTextMessageAsync(message.Chat.Id, "☆ти увів якийсь дивний номер фільму☆");
                            await bot.SendTextMessageAsync(message.Chat.Id, "☆спробуй ще раз☆");
                        }
                        return;
                    }
                    catch
                    {
                        throw new Exception();
                    }
                }
                else
                {
                    switch (message.Text)
                    {
                        case "/start":
                            await bot.SendTextMessageAsync(message.Chat.Id, $"☆☆☆\n\nПривітик, {message.Chat.FirstName}\nя бот, який буде допомагати тобі працювати з фільмами<3" +
                                $"\n\n☆щоб зрозуміти, як працювати зі мною тицяй кнопочку\n\n-->   ☆меню☆" +
                                "\n\n☆☆☆",
                                 replyMarkup: keyboard);
                            return;
                        case "☆меню☆":
                            await bot.SendTextMessageAsync(message.Chat.Id, "☆☆☆\n\nПривітик\n\n☆що будемо робити?" +
                                "\n\n☆пошук фільму☆\n\n\t-->\tзнайти фільми, що зараз у прокаті у кіно\n\t-->\tзнайти інформацію по фільму\n\t-->\tзнайти фільм за актором чи режисером" +
                                "\n\n☆список переглянутих фільмів☆\n\n\t-->\tпереглянути список\n\t-->\tстворити список\n\t-->\tредагувати список\n\t-->\tвидалити фільм зі списку" +
                                "\n\n☆список обраних фільмів☆\n\n\t-->\tпереглянути список\n\t-->\tстворити список\n\t-->\tредагувати список\n\t-->\tвидалити фільм зі списку\n\n☆☆☆"
                                , replyMarkup: keyboard);
                            return;
                        case "☆пошук фільму☆":
                            InlineKeyboardMarkup inlineKeyboard = new(new[]
                            {
                                    new[]
                                    {
                                        InlineKeyboardButton.WithCallbackData("☆зараз у кіно☆", "фільми, що зараз у прокаті"),
                                        InlineKeyboardButton.WithCallbackData("☆про фільм☆", "пошук інформації по фільму"),
                                    },
                                    new[]
                                    {
                                        InlineKeyboardButton.WithCallbackData("☆за актором/режисером☆", "пошук за людиною"),
                                    },
                        });

                            await bot.SendTextMessageAsync(message.Chat.Id, "☆Який саме пошук по фільму здійснити?☆", replyMarkup: inlineKeyboard);
                            return;
                        case "☆список переглянутих фільмів☆":
                            InlineKeyboardMarkup inlineKeyboard_listmovie = new(new[]
                           {
                                    new[]
                                    {
                                        InlineKeyboardButton.WithCallbackData("☆переглянути список☆", "переглянуті: переглянутм список"),
                                    },
                                     new[]
                                    {
                                        InlineKeyboardButton.WithCallbackData("☆знайти фільм за номером☆", "переглянуті: знайти фільм"),
                                    },
                                    new[]
                                    {
                                        InlineKeyboardButton.WithCallbackData("☆додати фільм☆", "переглянуті: додати до списку"),
                                    },
                                    new[]
                                    {
                                        InlineKeyboardButton.WithCallbackData("☆редагувати фільм☆", "переглянуті: редагувати фільм"),
                                    },
                                    new[]
                                    {
                                        InlineKeyboardButton.WithCallbackData("☆видалити фільм☆", "переглянуті: видалити зі списку"),
                                    },
                        });
                            await bot.SendTextMessageAsync(message.Chat.Id, "☆що робити зі списком переглянутих фільмів?☆", replyMarkup: inlineKeyboard_listmovie);
                            return;
                        case "☆список обраних фільмів☆":
                            InlineKeyboardMarkup inlineKeyboard_selectedlist = new(new[]
                          {
                                    new[]
                                    {
                                        InlineKeyboardButton.WithCallbackData("☆переглянути список☆", "обрані: переглянути список"),
                                    },
                                    new[]
                                    {
                                        InlineKeyboardButton.WithCallbackData("☆додати фільм☆", "обрані: додати до списку"),
                                    },
                                    new[]
                                    {
                                        InlineKeyboardButton.WithCallbackData("☆редагувати фільм☆", "обрані: редагувати фільм"),
                                    },
                                    new[]
                                    {
                                        InlineKeyboardButton.WithCallbackData("☆видалити фільм☆", "обрані: видалити фільм"),
                                    },
                        });
                            await bot.SendTextMessageAsync(message.Chat.Id, "☆що робити зі списком обраних фільмів?☆", replyMarkup: inlineKeyboard_selectedlist);
                            return;
                        default:
                            await bot.SendTextMessageAsync(message.Chat.Id, "☆вибач, такої операції не існує☆", replyToMessageId: message.MessageId);
                            await bot.SendStickerAsync(message.Chat.Id,
                            InputFile.FromUri("https://stickerswiki.ams3.cdn.digitaloceanspaces.com/GanwIrdou11/925304.160.webp")
                              );
                            return;
                    }
                }
            }
        public static async Task HandleCallBackQueryAsync(ITelegramBotClient bot, CallbackQuery callbackQuery)
            {
                if (callbackQuery.Data.StartsWith("фільми, що зараз у прокаті"))
                {
                    MovieClient movieClient = new MovieClient();
                    var result = await movieClient.GetMovieNowPlayingAsync();
                    await bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "☆Зачекай, доки я знайду для тебе всі фільми☆");
                    Thread.Sleep(2000);
                    for (int i = 0; i < result.Results.Length; i++)
                    {
                        try
                        {

                            Message photo = await bot.SendPhotoAsync(
                            chatId: callbackQuery.Message.Chat.Id,
                            photo: InputFile.FromUri($"http://image.tmdb.org/t/p/original{result.Results[i].Poster_path}"),
                            caption: $"{i}\n☆ Назва фільму:\t{result.Results[i].Title}\n\n☆ Опис фільму:\t{result.Results[i].Overview}\n\n☆ Дата виходу:\t{result.Results[i].Release_date}\n\n☆ Середня оцінка:\t{result.Results[i].Vote_average}\n\n",
                            parseMode: ParseMode.Html);
                        }
                        catch (Exception)
                        {
                            try
                            {
                                Message photo = await bot.SendPhotoAsync(
                               chatId: callbackQuery.Message.Chat.Id,
                               photo: InputFile.FromUri($"http://image.tmdb.org/t/p/w780{result.Results[i].Poster_path}"),
                               caption: $"{i}\n☆ Назва фільму:\t{result.Results[i].Title}\n\n☆ Опис фільму:\t{result.Results[i].Overview}\n\n☆ Дата виходу:\t{result.Results[i].Release_date}\n\n☆ Середня оцінка:\t{result.Results[i].Vote_average}\n\n",
                               parseMode: ParseMode.Html);
                            }
                            catch (Exception)
                            {
                                try
                                {
                                    Message photo = await bot.SendPhotoAsync(
                                    chatId: callbackQuery.Message.Chat.Id,
                                    photo: InputFile.FromUri($"http://image.tmdb.org/t/p/w500{result.Results[i].Poster_path}"),
                                    caption: $"{i}\n☆ Назва фільму:\t{result.Results[i].Title}\n\n☆ Опис фільму:\t{result.Results[i].Overview}\n\n☆ Дата виходу:\t{result.Results[i].Release_date}\n\n☆ Середня оцінка:\t{result.Results[i].Vote_average}\n\n",
                                    parseMode: ParseMode.Html);
                                }
                                catch (Exception)
                                {
                                    try
                                    {
                                        Message photo = await bot.SendPhotoAsync(
                                        chatId: callbackQuery.Message.Chat.Id,
                                        photo: InputFile.FromUri($"http://image.tmdb.org/t/p/w342{result.Results[i].Poster_path}"),
                                        caption: $"{i}\n☆ Назва фільму:\t{result.Results[i].Title}\n\n☆ Опис фільму:\t{result.Results[i].Overview}\n\n☆ Дата виходу:\t{result.Results[i].Release_date}\n\n☆ Середня оцінка:\t{result.Results[i].Vote_average}\n\n",
                                        parseMode: ParseMode.Html);
                                    }
                                    catch (Exception)
                                    {
                                        try
                                        {
                                            Message photo = await bot.SendPhotoAsync(
                                            chatId: callbackQuery.Message.Chat.Id,
                                            photo: InputFile.FromUri($"http://image.tmdb.org/t/p/w185{result.Results[i].Poster_path}"),
                                            caption: $"{i}\n☆ Назва фільму:\t{result.Results[i].Title}\n\n☆ Опис фільму:\t{result.Results[i].Overview}\n\n☆ Дата виходу:\t{result.Results[i].Release_date}\n\n☆ Середня оцінка:\t{result.Results[i].Vote_average}\n\n",
                                            parseMode: ParseMode.Html);
                                        }
                                        catch (Exception)
                                        {
                                            try
                                            {
                                                Message photo = await bot.SendPhotoAsync(
                                                chatId: callbackQuery.Message.Chat.Id,
                                                photo: InputFile.FromUri($"http://image.tmdb.org/t/p/w154{result.Results[i].Poster_path}"),
                                                caption: $"{i}\n☆ Назва фільму:\t{result.Results[i].Title}\n\n☆ Опис фільму:\t{result.Results[i].Overview}\n\n☆ Дата виходу:\t{result.Results[i].Release_date}\n\n☆ Середня оцінка:\t{result.Results[i].Vote_average}\n\n",
                                                parseMode: ParseMode.Html);
                                            }
                                            catch (Exception)
                                            {
                                                try
                                                {
                                                    Message photo = await bot.SendPhotoAsync(
                                                    chatId: callbackQuery.Message.Chat.Id,
                                                    photo: InputFile.FromUri($"http://image.tmdb.org/t/p/w92{result.Results[i].Poster_path}"),
                                                    caption: $"{i}\n☆ Назва фільму:\t{result.Results[i].Title}\n\n☆ Опис фільму:\t{result.Results[i].Overview}\n\n☆ Дата виходу:\t{result.Results[i].Release_date}\n\n☆ Середня оцінка:\t{result.Results[i].Vote_average}\n\n",
                                                    parseMode: ParseMode.Html);
                                                }
                                                catch (Exception)
                                                {
                                                    Message photo = await bot.SendPhotoAsync(
                                                    chatId: callbackQuery.Message.Chat.Id,
                                                    photo: InputFile.FromUri("https://www.movienewz.com/img/films/poster-holder.jpg"),
                                                    caption: $"{i}\n☆ Назва фільму:\t{result.Results[i].Title}\n\n☆ Опис фільму:\t{result.Results[i].Overview}\n\n☆ Дата виходу:\t{result.Results[i].Release_date}\n\n☆ Середня оцінка:\t{result.Results[i].Vote_average}\n\n",
                                                    parseMode: ParseMode.Html);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                    }
                    Thread.Sleep(1000);
                    await bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "☆Ось усі фільми, які мені вдалося знайти☆");
                    return;
                }
                else if (callbackQuery.Data.StartsWith("пошук інформації по фільму"))
                {
                    var filter = Builders<BsonDocument>.Filter.Eq("user_id", callbackQuery.Message.Chat.Id);
                    UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Set("bot_is_waiting_for_movie_name", true);
                    UpdateResult result = Constants.collection0.UpdateMany(filter, update);
                    await bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "☆напиши назву фільму☆");
                    return;
                }
                else if (callbackQuery.Data.StartsWith("пошук за людиною"))
                {
                    var filter = Builders<BsonDocument>.Filter.Eq("user_id", callbackQuery.Message.Chat.Id);
                    UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Set("bot_is_waiting_for_person_name", true);
                    UpdateResult result = Constants.collection0.UpdateMany(filter, update);
                    await bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "☆напиши ім'я актора чи режисера☆");
                    return;
                }
                else if (callbackQuery.Data.StartsWith("обрані: переглянути список"))
                {
                    SelectedMovieClient selectedMovieClient = new SelectedMovieClient();
                    var result3 = await selectedMovieClient.GetSelectedMovieListAsync(callbackQuery.Message.Chat.Id);
                    await bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "☆Зачекай, доки я знайду всі обрані тобою фільми☆");
                    Thread.Sleep(2000);
                    if (result3.Count == 0)
                    {
                        await bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "☆У списку поки немає обраних фільмів☆");
                    }
                    else
                    {
                        for (int i = 0; i < result3.Count; i++)
                        {
                            await bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id, $"☆   {i + 1}    ☆\n\n☆назва фільму --> {result3[i].Movie_name}");
                        }
                        Thread.Sleep(1000);
                        await bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "☆Ось усі фільми, які мені вдалося знайти☆");
                    }
                    return;
                }
                else if (callbackQuery.Data.StartsWith("обрані: додати до списку"))
                {
                    var filter = Builders<BsonDocument>.Filter.Eq("user_id", callbackQuery.Message.Chat.Id);
                    UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Set("bot_is_waiting_for_ADD_selected_movie", true);
                    UpdateResult result = Constants.collection0.UpdateMany(filter, update);
                    await bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "☆напиши назву фільму☆");
                    return;
                }
                else if (callbackQuery.Data.StartsWith("обрані: редагувати фільм"))
                {
                    var filter = Builders<BsonDocument>.Filter.Eq("user_id", callbackQuery.Message.Chat.Id);
                    UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Set("bot_is_waiting_for_UPDATE_selected_movie", true);
                    UpdateResult result = Constants.collection0.UpdateMany(filter, update);
                    await bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "☆напиши номер фільму, назву якого хочеш змінити☆");
                    return;
                }
                else if (callbackQuery.Data.StartsWith("обрані: видалити фільм"))
                {
                    var filter = Builders<BsonDocument>.Filter.Eq("user_id", callbackQuery.Message.Chat.Id);
                    UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Set("bot_is_waiting_for_DELETE_selected_movie", true);
                    UpdateResult result = Constants.collection0.UpdateMany(filter, update);
                    await bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "☆напиши номер фільму, назву який хочеш видалити☆");
                    return;
                }
                else if (callbackQuery.Data.StartsWith("переглянуті: переглянутм список"))
                {
                    WatchedMovieClient watchedMovieClient = new WatchedMovieClient();
                    var result = await watchedMovieClient.GetWatchedMovieListAsync(callbackQuery.Message.Chat.Id);
                    await bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "☆Зачекай, доки я знайду всі переглянуті тобою фільми☆");
                    Thread.Sleep(2000);
                    if (result.Count == 0)
                    {
                        await bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "☆У списку поки немає переглянутих фільмів☆");
                    }
                    else
                    {
                        for (int i = 0; i < result.Count; i++)
                        {
                            await bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id, $"☆    {i + 1}    ☆\n\n☆назва фільму --> {result[i].Movie_name}\n\n☆рейтинг фільму --> {result[i].Movie_rate}\n\n" +
                                $"☆коментар до фільму --> {result[i].Movie_comment}");
                        }
                        Thread.Sleep(1000);
                        await bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "☆Ось усі фільми, які мені вдалося знайти☆");
                    }
                    return;
                }
                else if (callbackQuery.Data.StartsWith("переглянуті: додати до списку"))
                {
                    var filter = Builders<BsonDocument>.Filter.Eq("user_id", callbackQuery.Message.Chat.Id);
                    UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Set("bot_is_waiting_for_ADD_watched_movie_NAME", true);
                    UpdateResult result = Constants.collection0.UpdateMany(filter, update);
                    await bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "☆напиши назву фільму☆");
                    return;
                }
                else if (callbackQuery.Data.StartsWith("переглянуті: редагувати фільм"))
                {
                    var filter = Builders<BsonDocument>.Filter.Eq("user_id", callbackQuery.Message.Chat.Id);
                    UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Set("bot_is_waiting_for_UPDATE_watched_movie", true);
                    UpdateResult result = Constants.collection0.UpdateMany(filter, update);
                    await bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "☆напиши номер фільму, дані якого будеш змінювати☆");
                    return;
                }
                else if (callbackQuery.Data.StartsWith("переглянуті: видалити зі списку"))
                {
                    var filter = Builders<BsonDocument>.Filter.Eq("user_id", callbackQuery.Message.Chat.Id);
                    UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Set("bot_is_waiting_for_DELETE_watched_movie", true);
                    UpdateResult result = Constants.collection0.UpdateMany(filter, update);
                    await bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "☆напиши номер фільму, назву який хочеш видалити☆");
                    return;
                }
                else if (callbackQuery.Data.StartsWith("змінити_назву"))
                {
                    var filter = Builders<BsonDocument>.Filter.Eq("user_id", callbackQuery.Message.Chat.Id);
                    UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Set("bot_is_waiting_for_UPDATE_watched_movie_NAME", true);
                    UpdateResult result = Constants.collection0.UpdateMany(filter, update);
                    await bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "☆напиши нову назву фільму☆");
                    return;
                }
                else if (callbackQuery.Data.StartsWith("змінити_рейтинг"))
                {
                    var filter = Builders<BsonDocument>.Filter.Eq("user_id", callbackQuery.Message.Chat.Id);
                    UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Set("bot_is_waiting_for_UPDATE_watched_movie_RATE", true);
                    UpdateResult result = Constants.collection0.UpdateMany(filter, update);
                    await bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "☆напиши новий рейтинг фільму☆\n\n(рейтинг має бути в діапазоні від 1 до 10)");
                    return;
                }
                else if (callbackQuery.Data.StartsWith("змінити_коментар"))
                {
                    var filter = Builders<BsonDocument>.Filter.Eq("user_id", callbackQuery.Message.Chat.Id);
                    UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Set("bot_is_waiting_for_UPDATE_watched_movie_COMMENT", true);
                    UpdateResult result = Constants.collection0.UpdateMany(filter, update);
                    await bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "☆напиши новий коментар до фільму☆");
                    return;
                }
                else if (callbackQuery.Data.StartsWith("переглянуті: знайти фільм"))
                {
                    var filter = Builders<BsonDocument>.Filter.Eq("user_id", callbackQuery.Message.Chat.Id);
                    UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Set("bot_is_waiting_for_GETone_watched_movie", true);
                    UpdateResult result = Constants.collection0.UpdateMany(filter, update);
                    await bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "☆напиши номер фільму☆");
                    return;
                }
            }
        public static void UpdateDatabase(Message message)
            {
                var filter = Builders<BsonDocument>.Filter.Eq("user_id", message.Chat.Id);
                UpdateDefinition<BsonDocument> update_database = Builders<BsonDocument>.Update
                .Set("bot_is_waiting_for_movie_name", false)
                .Set("bot_is_waiting_for_person_name", false)
                .Set("bot_is_waiting_for_ADD_selected_movie", false)
                .Set("bot_is_waiting_for_DELETE_selected_movie", false)
                .Set("bot_is_waiting_for_UPDATE_selected_movie_NAME", false)
                .Set("bot_is_waiting_for_UPDATE_selected_movie", false)
                .Set("bot_is_waiting_for_ADD_watched_movie_NAME", false)
                .Set("bot_is_waiting_for_ADD_watched_movie_RATE", false)
                .Set("bot_is_waiting_for_ADD_watched_movie_COMMENT", false)
                .Set("bot_is_waiting_for_DELETE_watched_movie", false)
                .Set("bot_is_waiting_for_UPDATE_watched_movie", false)
                .Set("bot_is_waiting_for_UPDATE_watched_movie_NAME", false)
                .Set("bot_is_waiting_for_UPDATE_watched_movie_RATE", false)
                .Set("bot_is_waiting_for_UPDATE_watched_movie_COMMENT", false)
                .Set("bot_is_waiting_for_GETone_watched_movie", false);
                var result = Constants.collection0.UpdateMany(filter, update_database);
            }
        public static async Task ErrorAsync(ITelegramBotClient bot, Exception exception, CancellationToken cancellationToken)
            {
                var error_message = exception switch
                {
                    ApiRequestException apiRequestException => $"Помилка телеграм API:\n{apiRequestException.ErrorCode}\n{apiRequestException.Message}",
                    _ => exception.ToString()
                };
                Console.WriteLine(error_message);

                return;// Task.CompletedTask();
            }            
    }
}

namespace Battleships.ConsoleClient
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;

    class ConsoleClient
    {
        private static User currentUser;
        private static string RegisterUserEndpoint = "http://localhost:62858/api/account/register";
        private static string TokenEndpoint = "http://localhost:62858/token";
        private static string CreateGameEndpoint = "http://localhost:62858/api/games/create";
        private static string JoinGameEndpoint = "http://localhost:62858/api/games/join";
        private static string PlayGameEndpoint = "http://localhost:62858/api/games/play";
        
        static void Main()
        {
            Console.WriteLine("Please enter commands in the form specified in the homework instructions:");
            Console.WriteLine("Type quit to quit.");
            string line = Console.ReadLine();
            while (line != "quit")
            {
                if (line != null)
                {
                    string[] tokens = line.Split(' ');
                    if (tokens.Count() > 1)
                    {
                        switch (tokens[1])
                        {
                            case "register" :
                                CreateUser(tokens);
                                break;
                            case "login" :
                                LogInUser(tokens);
                                break;
                            case "create-game" :
                                CreateGame();
                                break;
                            case "join-game" :
                                JoinGame(tokens);
                                break;
                            case "play" :
                                PlayGame(tokens);
                                break;
                            default :
                                Console.WriteLine("Please enter the commands in the specified format!");
                                break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Please enter the commands in the specified format!");
                    }
                }

                line = Console.ReadLine();
            }
            Console.WriteLine("Thank you for playing");

        }

        private static async void CreateUser(string[] tokens)
        {

            string userEmail = tokens[2];
            string userPassowrd = tokens[3];
            string confirmPassword = tokens[4];

            using (var httpClient = new HttpClient())
            {
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("Email", userEmail),
                    new KeyValuePair<string, string>("Password", userPassowrd),
                    new KeyValuePair<string, string>("ConfirmPassword", confirmPassword)
                });
                var response = await httpClient.PostAsync(RegisterUserEndpoint, content);
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("User created!");
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(error);
                }
            }
        }

        private static void LogInUser(string[] tokens)
        {
            string userEmail = tokens[2];
            string userPassowrd = tokens[3];

            currentUser = new User()
            {
                UserName = userEmail,
                Password = userPassowrd
            };

            ObtainToken();
        }

        private static async void ObtainToken()
        {
            using (var httpClient = new HttpClient())
            {
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("Username", currentUser.UserName),
                    new KeyValuePair<string, string>("Password", currentUser.Password),
                    new KeyValuePair<string, string>("grant_type", "password"),
                });

                var response = await httpClient.PostAsync(TokenEndpoint, content);
                
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(error);
                    return;
                }

                Token token = await response.Content.ReadAsAsync<Token>();
                currentUser.Token = token.Access_Token;
                Console.WriteLine("Welcome, {0}! You are now logged in.", currentUser.UserName);
            }
        }

        private static async void CreateGame()
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + currentUser.Token);
                
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("test", "test"), 
                });

                var response = await httpClient.PostAsync(CreateGameEndpoint, null);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(error);
                    return;
                }

                string gameId = await response.Content.ReadAsStringAsync();
                
                Console.WriteLine(gameId);
            }
        }

        private static async void JoinGame(string[] tokens)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + currentUser.Token);

                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("GameId", tokens[2]), 
                });

                var response = await httpClient.PostAsync(JoinGameEndpoint, content);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(error);
                    return;
                }

                string gameId = await response.Content.ReadAsStringAsync();

                Console.WriteLine("Joined " + gameId);
            }
        }

        private static async void PlayGame(string[] tokens)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + currentUser.Token);

                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("GameId", tokens[2]), 
                    new KeyValuePair<string, string>("PositionX", tokens[3]), 
                    new KeyValuePair<string, string>("PositionY", tokens[4]), 
                });

                var response = await httpClient.PostAsync(PlayGameEndpoint, content);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(error);
                    return;
                }

                Console.WriteLine("Your turn is over. Awaiting next player's turn...");

            }
        }
    }
}

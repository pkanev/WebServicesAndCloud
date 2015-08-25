namespace Battleships.ConsoleClient
{
    using System;
    using System.Security.AccessControl;

    public class User
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
    }
}
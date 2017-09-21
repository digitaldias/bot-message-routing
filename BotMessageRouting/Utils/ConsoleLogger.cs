using System;
using System.Diagnostics;
using Underscore.Bot.MessageRouting.Models;


namespace Underscore.Bot.MessageRouting.Utils
{
    public class ConsoleLogger : ILogger
    {
        public void LogException(Exception ex)
        {
            Debug.WriteLine($"EXCEPTION-->'{ex.Message}'");
        }


        public void LogInformation(string message)
        {
            Debug.WriteLine(message);
        }
    }
}

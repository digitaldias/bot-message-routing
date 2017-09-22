using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Underscore.Bot.MessageRouting.Models;


namespace Underscore.Bot.MessageRouting.Utils
{
    public class ConsoleLogger : ILogger
    {
        public void Enter(string className, [CallerMemberName]string methodName = "")
        {
            Debug.WriteLine($"Entering: {className}.{methodName}(...)");
        }


        public void LogException(Exception ex)
        {
            Debug.WriteLine($"EXCEPTION-->'{ex.Message}'");
        }


        public void LogInformation(string message)
        {
            Debug.WriteLine(message);
        }


        public void LogWarning(string message)
        {
            Debug.WriteLine($"WARNING: {message}");
        }
    }
}

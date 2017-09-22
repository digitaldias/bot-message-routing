using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Underscore.Bot.MessageRouting.Models
{
    public interface ILogger
    {
        void Enter(string className, string methodName = "");

        void LogInformation(string message);

        void LogException(Exception ex);

        void LogWarning(string message);
    }
}

using System;
using System.Threading.Tasks;

namespace Underscore.Bot.MessageRouting.Utils
{
    public class ExceptionHandler
    {
        public TResult Get<TResult>(Func<TResult> unsafeFunction)
        {
            try
            {
                return unsafeFunction.Invoke();
            }
            catch(AggregateException aex)
            {
                // Logging?
            }
            catch(Exception ex)
            {
                // Logging?
            }
            return default(TResult);
        }


        public Task<TResult> GetAsync<TResult>(Func<Task<TResult>> unsafeFunction)
        {
            try
            {
                return unsafeFunction.Invoke();
            }
            catch(AggregateException ex)
            {
                // Logging?
            }
            catch(Exception ex)
            {
                // Logging?
            }
            return Task.FromResult(default(TResult));
        }
    }
}

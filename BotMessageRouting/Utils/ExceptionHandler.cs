using System;
using System.Threading.Tasks;
using Underscore.Bot.MessageRouting.Models;

namespace Underscore.Bot.MessageRouting.Utils
{
    public class ExceptionHandler
    {
        private readonly ILogger _logger;


        public ExceptionHandler(ILogger logger)
        {
            _logger = logger;
        }


        public TResult Get<TResult>(Func<TResult> unsafeFunction)
        {
            try
            {
                return unsafeFunction.Invoke();
            }
            catch(AggregateException ex)
            {
                _logger.LogException(ex.InnerException);
            }
            catch(Exception ex)
            {
                _logger.LogException(ex);
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
                _logger.LogException(ex.InnerException);
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
            }
            return Task.FromResult(default(TResult));
        }
    }
}

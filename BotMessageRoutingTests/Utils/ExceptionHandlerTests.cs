using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Should;
using Underscore.Bot.MessageRouting.Models;
using Underscore.Bot.MessageRouting.Utils;
using Xunit;

namespace BotMessageRoutingTests.Utils
{
    
    public class ExceptionHandlerTests : TestsFor<ExceptionHandler>
    {
        [Fact]
        public void Get_FunctionIsNull_LogsItButDoesNotCrash()
        {
            // Arrange
            Func<int> nullFunc = null;

            // Act
            var result = Instance.Get(nullFunc);

            // Assert
            GetMockFor<ILogger>().Verify(l => l.LogWarning(It.IsAny<string>()), Times.Once());
        }

        [Fact]
        public void Get_FunctionIsNull_ReturnsDefaultValue()
        {
            // Arrange
            Func<int> nullFunc = null;

            // Act
            var result = Instance.Get(nullFunc);

            // Assert
            result.ShouldEqual(default(int));
        }


        [Fact]
        public void Get_FunctionIsValid_ReturnsFunctionsResult()
        {
            // Arrange
            Func<int> validFunction = () => 10;

            // Act
            var result = Instance.Get(validFunction);

            // Assert
            result.ShouldEqual(10);
        }


        [Fact]
        public void Get_FunctionThrowsException_ExceptionIsLogged()
        {
            // Arrange
            var exceptionMessage = Guid.NewGuid().ToString();
            var badException = new Exception(exceptionMessage);
            Func<int> badFunction = () => throw badException;

            // Act
            var result = Instance.Get(badFunction);

            // Assert
            GetMockFor<ILogger>()
                .Verify(l => l.LogException(badException), Times.Once());
        }


        [Fact]
        public void Get_FunctionThrowsException_ReturnsDefaultValueForType()
        {
            // Arrange
            var exceptionMessage = Guid.NewGuid().ToString();
            var badException = new Exception(exceptionMessage);
            Func<int> badFunction = () => throw badException;

            // Act
            var result = Instance.Get(badFunction);

            // Assert
            result.ShouldEqual(default(int));
        }


        [Fact]
        public void Get_FunctionThrowsAggregateException_InnerExceptionIsLogged()
        {
            // Arrange
            var exceptionMessage = Guid.NewGuid().ToString();
            var innerException = new Exception(exceptionMessage);
            var aggregateException = new AggregateException(new []{ innerException});
            Func<int> badFunction = () => throw aggregateException;

            // Act
            var result = Instance.Get(badFunction);

            // Assert
            GetMockFor<ILogger>()
                .Verify(l => l.LogException(innerException), Times.Once());
        }


        [Fact]
        public void Get_FunctionThrowsAggregateException_ReturnsDefaultValue()
        {
            // Arrange
            var exceptionMessage = Guid.NewGuid().ToString();
            var innerException = new Exception(exceptionMessage);
            var aggregateException = new AggregateException(new[] { innerException });
            Func<int> badFunction = () => throw aggregateException;

            // Act
            var result = Instance.Get(badFunction);

            // Assert
            result.ShouldEqual(default(int));
        }
    }
}

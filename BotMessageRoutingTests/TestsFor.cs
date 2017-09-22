using Moq;
using StructureMap.AutoMocking;
using StructureMap.AutoMocking.Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotMessageRoutingTests
{
    public class TestsFor<TEntity> where TEntity : class
    {
        public TEntity Instance { get; set; }
        public MoqAutoMocker<TEntity> AutoMock { get; set; }

        public TestsFor()
        {
            AutoMock = new MoqAutoMocker<TEntity>();

            Instance = AutoMock.ClassUnderTest;
        }

        public Mock<TContract> GetMockFor<TContract>() where TContract : class
        {
            return Mock.Get(AutoMock.Get<TContract>());
        }
    }
}

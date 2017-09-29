using Moq;
using StructureMap.AutoMocking.Moq;

namespace BotMessageRoutingTests
{
    public class TestsFor<TEntity> where TEntity : class
    {
        public TEntity Instance { get; set; }
        public MoqAutoMocker<TEntity> AutoMock { get; set; }

        public TestsFor()
        {
            AutoMock = new MoqAutoMocker<TEntity>();

            BeforeInstanceCreation();

            Instance = AutoMock.ClassUnderTest;
        }

        public Mock<TContract> GetMockFor<TContract>() where TContract : class
        {
            return Mock.Get(AutoMock.Get<TContract>());
        }

        public virtual void BeforeInstanceCreation(){
            // For overrides only. No code here - ever.
        }
    }
}

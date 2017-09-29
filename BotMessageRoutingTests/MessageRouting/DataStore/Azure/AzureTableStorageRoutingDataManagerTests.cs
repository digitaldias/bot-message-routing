using FizzWare.NBuilder;
using Should;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Underscore.Bot.MessageRouting.DataStore.Azure;
using Underscore.Bot.Models;
using Microsoft.Bot.Connector;
using Xunit;

namespace BotMessageRoutingTests.MessageRouting.DataStore.Azure
{
    public class AzureTableStorageRoutingDataManagerTests : TestsFor<AzureTableStorageRoutingDataManager>
    {
        [Fact]
        public void AddParty_PartyIsNull_ReturnsFalse()
        {
            // Arrange
            Party nullParty = null;

            // Act
            var result = Instance.AddParty(nullParty);

            // Assert
            result.ShouldBeFalse();
        }


        [Fact]
        public void AddParty_PartyIsValid_DoesSometihng()
        {
            // Arrange
            var serviceUrl          = "http://underscore";
            string channelId        = "channel9";
            var channelAccount      = Builder<ChannelAccount>.CreateNew().Build();
            var conversationAccount = Builder<ConversationAccount>.CreateNew().Build();
            var conversationParty   = Builder<Party>
                .CreateNew()
                .WithConstructor( () => new Party(serviceUrl, channelId, channelAccount, conversationAccount) )
                .Build();

            // Act
            var result = Instance.AddParty(conversationParty, false);

            // Assert

        }
    }
}

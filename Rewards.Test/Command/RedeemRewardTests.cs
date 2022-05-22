using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Rewards.Data;
using Rewards.Domain;
using Rewards.Middleware;
using Rewards.Services;
using Xunit;

namespace Rewards.Command.Test
{
    public class RedeemRewardTests
    {
        [Fact]
        public async Task UserNotFound()
        {
            // Arrange
            var storageProviderMock = new Mock<IStorageProvider>();
            var dateTimeServiceMock = new Mock<IDateTimeService>();
            var command = new RedeemReward(storageProviderMock.Object, dateTimeServiceMock.Object);
            // Act
            command.SetParameters(1, new DateTime());
            var exception = await Assert.ThrowsAsync<HttpResponseException>(async () => await command.ExecuteAsync());
            // Assert
            Assert.Equal("User 1 not found", exception.Message);
            storageProviderMock.Verify(s => s.FindUserByIdAsync(1));
            storageProviderMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task RewardNotFound()
        {
            // Arrange
            var storageProviderMock = new Mock<IStorageProvider>();
            storageProviderMock.Setup(s => s.FindUserByIdAsync(1)).ReturnsAsync(new User { Id = 1 });
            var dateTimeServiceMock = new Mock<IDateTimeService>();
            var command = new RedeemReward(storageProviderMock.Object, dateTimeServiceMock.Object);
            // Act
            command.SetParameters(1, new DateTime());
            var exception = await Assert.ThrowsAsync<HttpResponseException>(async () => await command.ExecuteAsync());
            // Assert
            Assert.Equal("This reward could not be found for user 1", exception.Message);
            storageProviderMock.Verify(s => s.FindUserByIdAsync(1));
            storageProviderMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task RewardExpired()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Rewards = new List<Reward>
                {
                    new Reward { AvailableAt = new DateTime(2022, 5, 23), ExpiresAt = new DateTime(2022, 5, 24) }
                }
            };
            var storageProviderMock = new Mock<IStorageProvider>();
            storageProviderMock.Setup(s => s.FindUserByIdAsync(1)).ReturnsAsync(user);

            var dateTimeServiceMock = new Mock<IDateTimeService>();
            dateTimeServiceMock.SetupGet(s => s.Now).Returns(new DateTime(2022, 6, 19));

            var command = new RedeemReward(storageProviderMock.Object, dateTimeServiceMock.Object);
            // Act
            command.SetParameters(1, new DateTime(2022, 5, 23));
            var exception = await Assert.ThrowsAsync<HttpResponseException>(async () => await command.ExecuteAsync());
            // Assert
            Assert.Equal("This reward is already expired", exception.Message);
            storageProviderMock.Verify(s => s.FindUserByIdAsync(1));
            storageProviderMock.VerifyNoOtherCalls();
            dateTimeServiceMock.VerifyGet(s => s.Now);
        }

        [Fact]
        public async Task RewardNotReady()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Rewards = new List<Reward>
                {
                    new Reward { AvailableAt = new DateTime(2022, 5, 23), ExpiresAt = new DateTime(2022, 5, 24) }
                }
            };
            var storageProviderMock = new Mock<IStorageProvider>();
            storageProviderMock.Setup(s => s.FindUserByIdAsync(1)).ReturnsAsync(user);

            var dateTimeServiceMock = new Mock<IDateTimeService>();
            dateTimeServiceMock.SetupGet(s => s.Now).Returns(new DateTime(2022, 5, 19));

            var command = new RedeemReward(storageProviderMock.Object, dateTimeServiceMock.Object);
            // Act
            command.SetParameters(1, new DateTime(2022, 5, 23));
            var exception = await Assert.ThrowsAsync<HttpResponseException>(async () => await command.ExecuteAsync());
            // Assert
            Assert.Equal("This reward is not ready to claim", exception.Message);
            storageProviderMock.Verify(s => s.FindUserByIdAsync(1));
            storageProviderMock.VerifyNoOtherCalls();
            dateTimeServiceMock.VerifyGet(s => s.Now);
        }

        [Fact]
        public async Task RewardAlreadyRedeemed()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Rewards = new List<Reward>
                {
                    new Reward { AvailableAt = new DateTime(2022, 5, 23), ExpiresAt = new DateTime(2022, 5, 24), RedeemedAt = new DateTime(2022, 5, 23, 9, 32, 45) }
                }
            };
            var storageProviderMock = new Mock<IStorageProvider>();
            storageProviderMock.Setup(s => s.FindUserByIdAsync(1)).ReturnsAsync(user);

            var dateTimeServiceMock = new Mock<IDateTimeService>();
            dateTimeServiceMock.SetupGet(s => s.Now).Returns(new DateTime(2022, 5, 23, 11, 23, 32));

            var command = new RedeemReward(storageProviderMock.Object, dateTimeServiceMock.Object);
            // Act
            command.SetParameters(1, new DateTime(2022, 5, 23));
            var result = await command.ExecuteAsync();
            // Assert
            Assert.Equal(user.Rewards[0].AvailableAt, result.Data.AvailableAt);
            Assert.Equal(user.Rewards[0].ExpiresAt, result.Data.ExpiresAt);
            Assert.Equal(user.Rewards[0].RedeemedAt, result.Data.RedeemedAt);
            storageProviderMock.Verify(s => s.FindUserByIdAsync(1));
            storageProviderMock.VerifyNoOtherCalls();
            dateTimeServiceMock.VerifyGet(s => s.Now);
        }

        [Fact]
        public async Task RewardRedeemedSuccessfully()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Rewards = new List<Reward>
                {
                    new Reward { AvailableAt = new DateTime(2022, 5, 23), ExpiresAt = new DateTime(2022, 5, 24), UserId = 1 }
                }
            };
            var storageProviderMock = new Mock<IStorageProvider>();
            storageProviderMock.Setup(s => s.FindUserByIdAsync(1)).ReturnsAsync(user);

            var mockedNow = new DateTime(2022, 5, 23, 11, 23, 32);
            var dateTimeServiceMock = new Mock<IDateTimeService>();
            dateTimeServiceMock.SetupGet(s => s.Now).Returns(mockedNow);

            var command = new RedeemReward(storageProviderMock.Object, dateTimeServiceMock.Object);
            // Act
            command.SetParameters(1, new DateTime(2022, 5, 23));
            var result = await command.ExecuteAsync();
            // Assert
            Assert.Equal(user.Rewards[0].AvailableAt, result.Data.AvailableAt);
            Assert.Equal(user.Rewards[0].ExpiresAt, result.Data.ExpiresAt);
            Assert.Equal(mockedNow, result.Data.RedeemedAt);
            storageProviderMock.Verify(s => s.FindUserByIdAsync(1));
            storageProviderMock.Verify(s => s.AddOrUpdateRewardAsync(It.Is<Reward>(r => 
                r.UserId == user.Id
                && r.AvailableAt == user.Rewards[0].AvailableAt
                && r.ExpiresAt == user.Rewards[0].ExpiresAt
                && r.RedeemedAt == (DateTime?)mockedNow)));
            storageProviderMock.VerifyNoOtherCalls();
            dateTimeServiceMock.VerifyGet(s => s.Now);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Rewards.Data;
using Rewards.Domain;
using Rewards.Middleware;
using Rewards.Services;
using Xunit;

namespace Rewards.Command.Test
{
    public class GetUserRewardsTests
    {
        [Fact]
        public async Task NewUserCreated()
        {
            // Arrange
            var storageProviderMock = new Mock<IStorageProvider>();
            storageProviderMock.Setup(s => s.FindUserByIdAsync(1)).ReturnsAsync((User)null);
            var dateTimeServiceMock = new Mock<IDateTimeService>();
            var command = new GetUserRewards(storageProviderMock.Object, dateTimeServiceMock.Object);
            // Act
            command.SetParameters(1, new Contracts.Request.UserRewardRequest { At = new DateTime(2022, 5, 17) }); // Tuesday (Week starts from 15/05/22)
            var result = await command.ExecuteAsync();
            // Assert
            Assert.Equal(7, result.Data.Count);
            for (var i = 0; i < result.Data.Count; i++)
            {
                Assert.Equal(new DateTime(2022, 5, 15 + i), result.Data[i].AvailableAt);
                Assert.Equal(new DateTime(2022, 5, 15 + i + 1), result.Data[i].ExpiresAt);
                Assert.Null(result.Data[i].RedeemedAt);
                storageProviderMock.Verify(s => s.AddOrUpdateRewardAsync(It.Is<Reward>(r =>
                    r.UserId == 1
                    && r.AvailableAt == result.Data[i].AvailableAt
                    && r.ExpiresAt == result.Data[i].ExpiresAt
                    && r.RedeemedAt == null)));
            }
            storageProviderMock.Verify(s => s.FindUserByIdAsync(1));
            storageProviderMock.Verify(s => s.AddUserAsync(It.Is<User>(u => u.Id == 1)));
            storageProviderMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ExistingUserNoRewards()
        {
            // Arrange
            var storageProviderMock = new Mock<IStorageProvider>();
            storageProviderMock.Setup(s => s.FindUserByIdAsync(1)).ReturnsAsync(new User { Id = 1});
            var dateTimeServiceMock = new Mock<IDateTimeService>();
            var command = new GetUserRewards(storageProviderMock.Object, dateTimeServiceMock.Object);
            // Act
            command.SetParameters(1, new Contracts.Request.UserRewardRequest { At = new DateTime(2022, 5, 17) }); // Tuesday (Week starts from 15/05/22)
            var result = await command.ExecuteAsync();
            // Assert
            Assert.Equal(7, result.Data.Count);
            for (var i = 0; i < result.Data.Count; i++)
            {
                Assert.Equal(new DateTime(2022, 5, 15 + i), result.Data[i].AvailableAt);
                Assert.Equal(new DateTime(2022, 5, 15 + i + 1), result.Data[i].ExpiresAt);
                Assert.Null(result.Data[i].RedeemedAt);
                storageProviderMock.Verify(s => s.AddOrUpdateRewardAsync(It.Is<Reward>(r =>
                    r.UserId == 1
                    && r.AvailableAt == result.Data[i].AvailableAt
                    && r.ExpiresAt == result.Data[i].ExpiresAt
                    && r.RedeemedAt == null)));
            }
            storageProviderMock.Verify(s => s.FindUserByIdAsync(1));
            storageProviderMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ExistingUserWithRewards()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Rewards = new List<Reward>
                {
                    new Reward { AvailableAt = new DateTime(2022, 5, 15), ExpiresAt = new DateTime(2022, 5, 16), RedeemedAt = new DateTime(2022, 5, 15, 7, 32, 45) },
                    new Reward { AvailableAt = new DateTime(2022, 5, 16), ExpiresAt = new DateTime(2022, 5, 17), RedeemedAt = null },
                    new Reward { AvailableAt = new DateTime(2022, 5, 17), ExpiresAt = new DateTime(2022, 5, 18), RedeemedAt = null },
                    new Reward { AvailableAt = new DateTime(2022, 5, 18), ExpiresAt = new DateTime(2022, 5, 19), RedeemedAt = null },
                    new Reward { AvailableAt = new DateTime(2022, 5, 19), ExpiresAt = new DateTime(2022, 5, 20), RedeemedAt = null },
                    new Reward { AvailableAt = new DateTime(2022, 5, 20), ExpiresAt = new DateTime(2022, 5, 21), RedeemedAt = null },
                    new Reward { AvailableAt = new DateTime(2022, 5, 21), ExpiresAt = new DateTime(2022, 5, 22), RedeemedAt = null }
                }
            };
            var storageProviderMock = new Mock<IStorageProvider>();
            storageProviderMock.Setup(s => s.FindUserByIdAsync(1)).ReturnsAsync(user);
            var dateTimeServiceMock = new Mock<IDateTimeService>();
            var command = new GetUserRewards(storageProviderMock.Object, dateTimeServiceMock.Object);
            // Act
            command.SetParameters(1, new Contracts.Request.UserRewardRequest { At = new DateTime(2022, 5, 17) }); // Tuesday (Week starts from 15/05/22)
            var result = await command.ExecuteAsync();
            // Assert
            Assert.Equal(7, result.Data.Count);
            for (var i = 0; i < result.Data.Count; i++)
            {
                Assert.Equal(user.Rewards[i].AvailableAt, result.Data[i].AvailableAt);
                Assert.Equal(user.Rewards[i].ExpiresAt, result.Data[i].ExpiresAt);
                Assert.Equal(user.Rewards[i].RedeemedAt, result.Data[i].RedeemedAt);
            }
            storageProviderMock.Verify(s => s.FindUserByIdAsync(1));
            storageProviderMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task NoDateProvided_UseCurrentDate()
        {
            // Arrange
            var storageProviderMock = new Mock<IStorageProvider>();
            storageProviderMock.Setup(s => s.FindUserByIdAsync(1)).ReturnsAsync(new User { Id = 1 });
            var dateTimeServiceMock = new Mock<IDateTimeService>();
            dateTimeServiceMock.SetupGet(s => s.Now).Returns(new DateTime(2022, 5, 23)); // Monday (Week starst from 22/05/22)
            var command = new GetUserRewards(storageProviderMock.Object, dateTimeServiceMock.Object);
            // Act
            command.SetParameters(1, new Contracts.Request.UserRewardRequest());
            var result = await command.ExecuteAsync();
            // Assert
            Assert.Equal(7, result.Data.Count);
            for (var i = 0; i < result.Data.Count; i++)
            {
                Assert.Equal(new DateTime(2022, 5, 22 + i), result.Data[i].AvailableAt);
                Assert.Equal(new DateTime(2022, 5, 22 + i + 1), result.Data[i].ExpiresAt);
                Assert.Null(result.Data[i].RedeemedAt);
                storageProviderMock.Verify(s => s.AddOrUpdateRewardAsync(It.Is<Reward>(r =>
                    r.UserId == 1
                    && r.AvailableAt == result.Data[i].AvailableAt
                    && r.ExpiresAt == result.Data[i].ExpiresAt
                    && r.RedeemedAt == null)));
            }
            storageProviderMock.Verify(s => s.FindUserByIdAsync(1));
            storageProviderMock.VerifyNoOtherCalls();
            dateTimeServiceMock.VerifyGet(s => s.Now);
        }

        [Fact]
        public async Task UserInvalidRewards_ExceptionThrown()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Rewards = new List<Reward>
                {
                    new Reward { AvailableAt = new DateTime(2022, 5, 15), ExpiresAt = new DateTime(2022, 5, 16), RedeemedAt = new DateTime(2022, 5, 15, 7, 32, 45) },
                    new Reward { AvailableAt = new DateTime(2022, 5, 16), ExpiresAt = new DateTime(2022, 5, 17) },
                    new Reward { AvailableAt = new DateTime(2022, 5, 17), ExpiresAt = new DateTime(2022, 5, 18) },
                    new Reward { AvailableAt = new DateTime(2022, 5, 19), ExpiresAt = new DateTime(2022, 5, 20) },
                    new Reward { AvailableAt = new DateTime(2022, 5, 21), ExpiresAt = new DateTime(2022, 5, 22) },
                    new Reward { AvailableAt = new DateTime(2022, 6, 22), ExpiresAt = new DateTime(2022, 6, 23) },
                    new Reward { AvailableAt = new DateTime(2022, 6, 23), ExpiresAt = new DateTime(2022, 6, 24) }
                }
            };
            var storageProviderMock = new Mock<IStorageProvider>();
            storageProviderMock.Setup(s => s.FindUserByIdAsync(1)).ReturnsAsync(user);
            var dateTimeServiceMock = new Mock<IDateTimeService>();
            var command = new GetUserRewards(storageProviderMock.Object, dateTimeServiceMock.Object);
            // Act
            command.SetParameters(1, new Contracts.Request.UserRewardRequest { At = new DateTime(2022, 5, 17) }); // Tuesday (Week starts from 15/05/22)
            await Assert.ThrowsAsync<HttpResponseException>(async() => await command.ExecuteAsync());
            // Assert
            storageProviderMock.Verify(s => s.FindUserByIdAsync(1));
            storageProviderMock.VerifyNoOtherCalls();
        }
    }
}

using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Domain.Repositories;
using Application.Commands;
using Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Application.Tests
{
    public class CreateUserHandlerTests
    {
        [Fact]
        public async Task CreateUser_Success_When_Email_Not_Exists()
        {
            var repoMock = new Mock<IUserRepository>();
            repoMock.Setup(r => r.EmailExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
            repoMock.Setup(r => r.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

            var handler = new CreateUserHandler(repoMock.Object);
            var cmd = new CreateUserCommand { Name = "Test", Email = "test@example.com", Password = "P@ssw0rd" };

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.Equal(cmd.Email, result.Email);
            repoMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
        }
    }
}

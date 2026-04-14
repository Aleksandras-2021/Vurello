using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Moq;
using PSK.Server.Data.Entities;
using PSK.Server.Services;

namespace psk.tests;

public class AuthServiceTests
{
        
    IConfiguration config = new ConfigurationBuilder()
    .AddInMemoryCollection(new Dictionary<string, string>
    {
    { "JWT_KEY", "JWT_KEY_FOR_TESTING_RANDOM_:87439874389349873" }
    })
    .Build();

    private Mock<UserManager<User>> GetUserManagerMock()
    {
        var store = new Mock<IUserStore<User>>();
        return new Mock<UserManager<User>>(
            store.Object,
            null, null, null, null, null, null, null, null);
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnToken_WhenUserCreated()
    {
        var userManagerMock = GetUserManagerMock();

        userManagerMock
            .Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        var service = new AuthService(userManagerMock.Object, config);

        var model = new Register
        {
            Username = "test",
            Password = "Password123"
        };

        var result = await service.RegisterAsync(model);


        Assert.Null(result.error);
        Assert.NotNull(result.token);
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnNull_WhenCreationFails()
    {
        var userManagerMock = GetUserManagerMock();

        userManagerMock
            .Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed());

        var service = new AuthService(userManagerMock.Object,config);

        var result = await service.RegisterAsync(new Register());

        Assert.NotNull(result.error);
        Assert.Null(result.token);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnToken_WhenCredentialsValid()
    {
        var userManagerMock = GetUserManagerMock();

        var user = new User { Id = Guid.NewGuid(), UserName = "test" };

        userManagerMock
            .Setup(x => x.FindByNameAsync("test"))
            .ReturnsAsync(user);

        userManagerMock
            .Setup(x => x.CheckPasswordAsync(user, "Password123"))
            .ReturnsAsync(true);

        var service = new AuthService(userManagerMock.Object,config);

        var result = await service.LoginAsync(new Login
        {
            Username = "test",
            Password = "Password123"
        });

        Assert.NotNull(result);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnNull_WhenPasswordInvalid()
    {
        var userManagerMock = GetUserManagerMock();

        var user = new User { UserName = "test" };

        userManagerMock
            .Setup(x => x.FindByNameAsync("test"))
            .ReturnsAsync(user);

        userManagerMock
            .Setup(x => x.CheckPasswordAsync(user, "incorrectPassword"))
            .ReturnsAsync(false);

        var service = new AuthService(userManagerMock.Object, config);

        var result = await service.LoginAsync(new Login
        {
            Username = "test",
            Password = "incorrectPassword"
        });

        Assert.Null(result);
    }

}

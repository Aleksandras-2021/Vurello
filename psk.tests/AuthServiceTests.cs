using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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

        var mock = new Mock<UserManager<User>>(
            store.Object,
            new Mock<IOptions<IdentityOptions>>().Object,
            new Mock<IPasswordHasher<User>>().Object,
            new IUserValidator<User>[0],
            new IPasswordValidator<User>[0],
            new Mock<ILookupNormalizer>().Object,
            new IdentityErrorDescriber(),
            new Mock<IServiceProvider>().Object,
            new Mock<ILogger<UserManager<User>>>().Object
        );

        return mock;
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

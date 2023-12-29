using FakeItEasy;
using MonsterCardGame.Repositories;
using MonsterCardGame.Controllers;
using MonsterCardGame.Models.DB;
using MonsterCardGame.Models.PJWT;
using Server;
using Server.Enums;

namespace UnitTests.Controllers.Casino;

public class CoinflipTests
{
    private readonly UserRepository _dummyUserRepository = A.Fake<UserRepository>();

    private readonly CasinoController _casinoController;

    public CoinflipTests()
    {
        _casinoController = new CasinoController(_dummyUserRepository);
    }
    
    [SetUp]
    public void Setup()
    {
        
    }

    [Test]
    public void PostCoinflip_InternalServerError1()
    {
        // Arrange
        TokenContent tokenContent = new() { UserId = 1 };
        int gambleAmount = 20;

        A.CallTo(() => _dummyUserRepository.SelectById(A<int>._))
            .Returns(null);

        // Act
        object result = _casinoController.PostCoinflip(tokenContent, gambleAmount);

        // Assert
        Assert.IsInstanceOf<ActionResult>(result);
        Assert.That( ((ActionResult)result).ResponseCode, Is.EqualTo(ResponseCode.InternalServerError) );
    }
    
    [Test]
    public void PostCoinflip_BadRequest()
    {
        // Arrange
        TokenContent tokenContent = new() { UserId = 1 };
        User dummyUser = new()
        {
            Id = 1,
            Coins = 0,
        };
        int gambleAmount = 20;

        A.CallTo(() => _dummyUserRepository.SelectById(A<int>._))
            .Returns(dummyUser);

        // Act
        object result = _casinoController.PostCoinflip(tokenContent, gambleAmount);

        // Assert
        Assert.IsInstanceOf<ActionResult>(result);
        Assert.That( ((ActionResult)result).ResponseCode, Is.EqualTo(ResponseCode.BadRequest) );
    }
    
    [Test]
    public void PostCoinflip_InternalServerError2()
    {
        // Arrange
        TokenContent tokenContent = new() { UserId = 1 };
        User dummyUser = new()
        {
            Id = 1,
            Coins = 100,
        };
        int gambleAmount = 20;

        A.CallTo(() => _dummyUserRepository.SelectById(A<int>._))
            .Returns(dummyUser);
        A.CallTo(() => _dummyUserRepository.SetCoins(A<int>._, A<int>._))
            .Returns(false);

        // Act
        object result = _casinoController.PostCoinflip(tokenContent, gambleAmount);

        // Assert
        Assert.IsInstanceOf<ActionResult>(result);
        Assert.That( ((ActionResult)result).ResponseCode, Is.EqualTo(ResponseCode.InternalServerError) );
    }
    
    [Test]
    public void PostCoinflip_Success()
    {
        // Arrange
        TokenContent tokenContent = new() { UserId = 1 };
        User dummyUser = new()
        {
            Id = 1,
            Coins = 100,
        };
        int gambleAmount = 20;

        A.CallTo(() => _dummyUserRepository.SelectById(A<int>._))
            .Returns(dummyUser);
        A.CallTo(() => _dummyUserRepository.SetCoins(A<int>._, A<int>._))
            .Returns(true);

        // Act
        object result = _casinoController.PostCoinflip(tokenContent, gambleAmount);

        // Assert
        Assert.IsInstanceOf<ActionResult>(result);
        Assert.That( ((ActionResult)result).ResponseCode, Is.EqualTo(ResponseCode.Ok) );
    }
}
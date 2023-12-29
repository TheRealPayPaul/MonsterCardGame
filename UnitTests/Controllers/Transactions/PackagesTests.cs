using DTO;
using FakeItEasy;
using MonsterCardGame.Controllers;
using MonsterCardGame.Models.DB;
using MonsterCardGame.Models.PJWT;
using MonsterCardGame.Repositories;
using MonsterCardGame.Utilities;
using Server;
using Server.Enums;

namespace UnitTests.Controllers.Transactions;

public class PackagesTests
{
    private readonly UserRepository _dummyUserRepository = A.Fake<UserRepository>();
    private readonly CompositeRepository _dummyCompositeRepository = A.Fake<CompositeRepository>();
    private readonly Mapper _dummyMapper = A.Fake<Mapper>();

    private readonly TransactionController _transactionController;

    public PackagesTests()
    {
        _transactionController = new TransactionController(_dummyUserRepository, _dummyCompositeRepository, _dummyMapper);
    }
    
    [SetUp]
    public void Setup()
    {
        
    }

    [Test]
    public void Packages_InternalServerError1()
    {
        // Arrange
        TokenContent tokenContent = new TokenContent() { UserId = 1 };

        A.CallTo(() => _dummyUserRepository.SelectById(A<int>._))
            .Returns(null);

        // Act
        object result = _transactionController.Packages(tokenContent);

        // Assert
        Assert.IsInstanceOf<ActionResult>(result);
        Assert.That( ((ActionResult)result).ResponseCode, Is.EqualTo(ResponseCode.InternalServerError) );
    }
    
    [Test]
    public void Packages_Forbidden()
    {
        // Arrange
        TokenContent tokenContent = new TokenContent() { UserId = 1 };
        User dummyUser = new()
        {
            Coins = 0,
        };

        A.CallTo(() => _dummyUserRepository.SelectById(A<int>._))
            .Returns(dummyUser);

        // Act
        object result = _transactionController.Packages(tokenContent);

        // Assert
        Assert.IsInstanceOf<ActionResult>(result);
        Assert.That( ((ActionResult)result).ResponseCode, Is.EqualTo(ResponseCode.Forbidden) );
    }
    
    [Test]
    public void Packages_InternalServerError2()
    {
        // Arrange
        TokenContent tokenContent = new TokenContent() { UserId = 1 };
        User dummyUser = new()
        {
            Coins = 100,
        };

        A.CallTo(() => _dummyUserRepository.SelectById(A<int>._))
            .Returns(dummyUser);
        A.CallTo(() => _dummyCompositeRepository.BuyCards(A<IEnumerable<Card>>._, A<int>._, A<int>._))
            .Returns(false);

        // Act
        object result = _transactionController.Packages(tokenContent);

        // Assert
        Assert.IsInstanceOf<ActionResult>(result);
        Assert.That( ((ActionResult)result).ResponseCode, Is.EqualTo(ResponseCode.InternalServerError) );
    }
    
    [Test]
    public void Packages_Success()
    {
        // Arrange
        TokenContent tokenContent = new TokenContent() { UserId = 1 };
        CardDTO[] successResult = A.Dummy<CardDTO[]>();
        User dummyUser = new()
        {
            Coins = 100,
        };

        A.CallTo(() => _dummyUserRepository.SelectById(A<int>._))
            .Returns(dummyUser);
        A.CallTo(() => _dummyCompositeRepository.BuyCards(A<IEnumerable<Card>>._, A<int>._, A<int>._))
            .Returns(true);
        A.CallTo(() => _dummyMapper.ToDTO(A<Card[]>._))
            .Returns(successResult);

        // Act
        object result = _transactionController.Packages(tokenContent);

        // Assert
        Assert.IsInstanceOf<CardDTO[]>(result);
        Assert.That(result, Is.EqualTo(successResult));
    }
}
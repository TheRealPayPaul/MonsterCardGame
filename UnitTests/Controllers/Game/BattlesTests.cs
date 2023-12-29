using DTO;
using FakeItEasy;
using MonsterCardGame.Controllers;
using MonsterCardGame.Models.DB;
using MonsterCardGame.Models.PJWT;
using MonsterCardGame.Repositories;
using MonsterCardGame.Utilities;
using Server;
using Server.Enums;

namespace UnitTests.Controllers.Game;

public class BattlesTests
{
    private readonly CardRepository _dummyCardRepository = A.Fake<CardRepository>();
    private readonly UserRepository _dummyUserRepository = A.Fake<UserRepository>();
    private readonly Mapper _dummyMapper = A.Fake<Mapper>();

    private readonly GameController _gameController;

    public BattlesTests()
    {
        _gameController = new GameController(_dummyCardRepository, _dummyUserRepository, _dummyMapper);
    }
    
    [SetUp]
    public void Setup()
    {
        
    }
    
    [Test]
    public void GetBattles_InternalServerError()
    {
        // Arrange
        TokenContent tokenContent = new() { UserId = 1 };
        
        A.CallTo(() => _dummyUserRepository.SelectById(A<int>._))
            .Returns(null);

        // Act
        object result = _gameController.GetBattles(tokenContent);

        // Test
        Assert.IsInstanceOf<ActionResult>(result);
        Assert.That( ((ActionResult)result).ResponseCode, Is.EqualTo(ResponseCode.InternalServerError) );
    }
}
using DTO;
using FakeItEasy;
using MonsterCardGame.Controllers;
using MonsterCardGame.Models.DB;
using MonsterCardGame.Models.PJWT;
using MonsterCardGame.Repositories;
using MonsterCardGame.Utilities;
using Server;
using Server.Enums;

namespace UnitTests.Controllers.Users;

public class StatsTests
{
    private readonly UserRepository _dummyUserRepository = A.Fake<UserRepository>();
    private readonly Mapper _dummyMapper = A.Fake<Mapper>();

    private readonly UserController _userController;

    public StatsTests()
    {
        _userController = new UserController(_dummyUserRepository, _dummyMapper);
    }
    
    [SetUp]
    public void Setup()
    {
        
    }
    
    [Test]
    public void GetStats_InternalServerError()
    {
        // Arrange
        TokenContent tokenContent = new() { UserId = 1 };
        
        A.CallTo(() => _dummyUserRepository.SelectById(A<int>._))
            .Returns(null);

        // Act
        object result = _userController.GetStats(tokenContent);

        // Assert
        Assert.IsInstanceOf<ActionResult>(result);
        Assert.That( ((ActionResult)result).ResponseCode, Is.EqualTo(ResponseCode.InternalServerError) );
    }
    
    [Test]
    public void GetStats_Success()
    {
        // Arrange
        TokenContent tokenContent = new() { UserId = 1 };
        UserStatsDTO successResult = A.Dummy<UserStatsDTO>();
        
        A.CallTo(() => _dummyUserRepository.SelectById(A<int>._))
            .Returns(A.Dummy<User>());
        A.CallTo(() => _dummyMapper.ToStats(A<User>._))
            .Returns(successResult);

        // Act
        object result = _userController.GetStats(tokenContent);

        // Assert
        Assert.IsInstanceOf<UserStatsDTO>(result);
        Assert.That( result, Is.EqualTo(successResult) );
    }
}
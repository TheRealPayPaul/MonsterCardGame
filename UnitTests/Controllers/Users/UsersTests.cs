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

public class UsersTests
{
    private readonly UserRepository _dummyUserRepository = A.Fake<UserRepository>();
    private readonly Mapper _dummyMapper = A.Fake<Mapper>();

    private readonly UserController _userController;

    public UsersTests()
    {
        _userController = new UserController(_dummyUserRepository, _dummyMapper);
    }
    
    [SetUp]
    public void Setup()
    {
        
    }

    [Test]
    public void Get_NotFound()
    {
        // Arrange
        string usernamePathVar = A.Dummy<string>();

        A.CallTo(() => _dummyUserRepository.SelectByUsername(A<string>._))
            .Returns(null);

        // Act
        object result = _userController.Get(usernamePathVar);

        // Assert
        Assert.IsInstanceOf<ActionResult>(result);
        Assert.That( ((ActionResult)result).ResponseCode, Is.EqualTo(ResponseCode.NotFound) );
    }
    
    [Test]
    public void Get_Success()
    {
        // Arrange
        string usernamePathVar = A.Dummy<string>();
        User successResult = A.Dummy<User>();
        
        A.CallTo(() => _dummyUserRepository.SelectByUsername(A<string>._))
            .Returns(successResult);

        // Act
        object result = _userController.Get(usernamePathVar);

        // Assert
        Assert.IsInstanceOf<User>(result);
        Assert.That( result, Is.EqualTo(successResult) );
    }
    
    [Test]
    public void Put_BadRequest1()
    {
        // Arrange
        TokenContent tokenContent = new() { UserId = 1 };
        UserDataDTO userDataDto = new() { Username = "Ma" };

        // Act
        object result = _userController.Put(tokenContent, userDataDto);

        // Assert
        Assert.IsInstanceOf<ActionResult>(result);
        Assert.That( ((ActionResult)result).ResponseCode, Is.EqualTo(ResponseCode.BadRequest) );
    }
    
    [Test]
    public void Put_BadRequest2()
    {
        // Arrange
        TokenContent tokenContent = new() { UserId = 1 };
        UserDataDTO userDataDto = new() { Username = "Max" };
        
        A.CallTo(() => _dummyUserRepository.SelectByUsername(A<string>._))
            .Returns(A.Dummy<User>());

        // Act
        object result = _userController.Put(tokenContent, userDataDto);

        // Assert
        Assert.IsInstanceOf<ActionResult>(result);
        Assert.That( ((ActionResult)result).ResponseCode, Is.EqualTo(ResponseCode.BadRequest) );
    }
    
    [Test]
    public void Put_InternalServerError()
    {
        // Arrange
        TokenContent tokenContent = new() { UserId = 1 };
        UserDataDTO userDataDto = new() { Username = "Max" };
        
        A.CallTo(() => _dummyUserRepository.SelectByUsername(A<string>._))
            .Returns(null);
        A.CallTo(() => _dummyUserRepository.UpdateUsername(A<int>._, A<string>._))
            .Returns(false);

        // Act
        object result = _userController.Put(tokenContent, userDataDto);

        // Assert
        Assert.IsInstanceOf<ActionResult>(result);
        Assert.That( ((ActionResult)result).ResponseCode, Is.EqualTo(ResponseCode.InternalServerError) );
    }
    
    [Test]
    public void Put_Success()
    {
        // Arrange
        TokenContent tokenContent = new() { UserId = 1 };
        UserDataDTO userDataDto = new() { Username = "Max" };
        
        A.CallTo(() => _dummyUserRepository.SelectByUsername(A<string>._))
            .Returns(null);
        A.CallTo(() => _dummyUserRepository.UpdateUsername(A<int>._, A<string>._))
            .Returns(true);

        // Act
        object result = _userController.Put(tokenContent, userDataDto);

        // Assert
        Assert.IsInstanceOf<ResponseCode>(result);
        Assert.That( (ResponseCode)result, Is.EqualTo(ResponseCode.Ok) );
    }
}
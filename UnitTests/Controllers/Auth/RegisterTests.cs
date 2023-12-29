using DTO;
using FakeItEasy;
using MonsterCardGame.Controllers;
using MonsterCardGame.Models.DB;
using MonsterCardGame.Models.PJWT;
using MonsterCardGame.Repositories;
using MonsterCardGame.Utilities;
using Server;
using Server.Enums;

namespace UnitTests.Controllers.Auth;

public class RegisterTests
{
    private readonly UserRepository _dummyUserRepository = A.Fake<UserRepository>();
    private readonly PJWToken _dummyPjwToken = A.Fake<PJWToken>();
    private readonly Mapper _dummyMapper = A.Fake<Mapper>();

    private readonly AuthController _authController;

    public RegisterTests()
    {
        _authController = new AuthController(_dummyUserRepository, _dummyPjwToken, _dummyMapper);
    }
    
    [SetUp]
    public void Setup()
    {
        
    }

    [Test]
    public void PostRegister_BadRequest()
    {
        // Arrange
        UserCredentialsDTO credentialsDto = new()
        {
            Username = "Ma",
            Password = "1234",
        };

        // Act
        object result = _authController.PostRegister(credentialsDto);

        // Assert
        Assert.IsInstanceOf<ActionResult>(result);
        Assert.That( ((ActionResult)result).ResponseCode, Is.EqualTo(ResponseCode.BadRequest) );
    }
    
    [Test]
    public void PostRegister_Conflict()
    {
        // Arrange
        UserCredentialsDTO credentialsDto = new()
        {
            Username = "Max",
            Password = "1234",
        };

        A.CallTo(() => _dummyUserRepository.SelectByUsername(A<string>._))
            .Returns(A.Dummy<User>());

        // Act
        object result = _authController.PostRegister(credentialsDto);

        // Assert
        Assert.IsInstanceOf<ActionResult>(result);
        Assert.That( ((ActionResult)result).ResponseCode, Is.EqualTo(ResponseCode.Conflict) );
    }
    
    [Test]
    public void PostRegister_Success()
    {
        // Arrange
        string dummyToken = A.Dummy<string>();
        UserCredentialsDTO credentialsDto = new()
        {
            Username = "Max",
            Password = "1234",
        };
        
        A.CallTo(() => _dummyUserRepository.SelectByUsername(A<string>._))
            .Returns(null);
        A.CallTo(() => _dummyUserRepository.Create(A<string>._, A<string>._, A<int>._, A<int>._))
            .Returns(A.Dummy<User>());
        A.CallTo(() => _dummyMapper.ToTokenContent(A<User>._))
            .Returns(A.Dummy<TokenContent>());
        A.CallTo(() => _dummyPjwToken.CreateToken(A<TokenContent>._, A<DateTime>._, A<string>._))
            .Returns(dummyToken);

        // Act
        object result = _authController.PostRegister(credentialsDto);

        // Assert
        Assert.IsInstanceOf<string>(result);
        Assert.That(result, Is.EqualTo(dummyToken));
    }
}
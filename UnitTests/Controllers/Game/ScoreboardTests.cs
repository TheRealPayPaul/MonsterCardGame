using DTO;
using FakeItEasy;
using MonsterCardGame.Controllers;
using MonsterCardGame.Models.DB;
using MonsterCardGame.Repositories;
using MonsterCardGame.Utilities;

namespace UnitTests.Controllers.Game;

public class ScoreboardTests
{
    private readonly CardRepository _dummyCardRepository = A.Fake<CardRepository>();
    private readonly UserRepository _dummyUserRepository = A.Fake<UserRepository>();
    private readonly Mapper _dummyMapper = A.Fake<Mapper>();

    private readonly GameController _gameController;

    public ScoreboardTests()
    {
        _gameController = new GameController(_dummyCardRepository, _dummyUserRepository, _dummyMapper);
    }
    
    [SetUp]
    public void Setup()
    {
        
    }

    [Test]
    public void GetScoreboard_Success()
    {
        // Arrange
        A.CallTo(() => _dummyUserRepository.SelectAll())
            .Returns(A.Dummy<IEnumerable<User>>());
        A.CallTo(() => _dummyMapper.ToStats(A<User>._))
            .Returns(A.Dummy<UserStatsDTO>());

        // Act
        object result = _gameController.GetScoreboard();

        // Test
        Assert.IsInstanceOf<List<UserStatsDTO>>(result);
    }
}
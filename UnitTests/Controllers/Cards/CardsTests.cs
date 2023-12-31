using DTO;
using FakeItEasy;
using MonsterCardGame.Controllers;
using MonsterCardGame.Models.DB;
using MonsterCardGame.Models.PJWT;
using MonsterCardGame.Repositories;
using MonsterCardGame.Utilities;

namespace UnitTests.Controllers.Cards;

public class CardsTests
{
    private readonly TradeRepository _dummyTradeRepository = A.Fake<TradeRepository>();
    private readonly CardRepository _dummyCardRepository = A.Fake<CardRepository>();
    private readonly Mapper _dummyMapper = A.Fake<Mapper>();

    private readonly CardsController _cardsController;

    public CardsTests()
    {
        _cardsController = new CardsController(_dummyTradeRepository, _dummyCardRepository, _dummyMapper);
    }
    
    [SetUp]
    public void Setup()
    {
        
    }

    [Test]
    public void GetCards_Success()
    {
        // Arrange
        CardDTO[] successResult = A.Dummy<CardDTO[]>();
        TokenContent tokenContent = new() { UserId = 1 };

        A.CallTo(() => _dummyCardRepository.SelectAllOfUser(A<int>._, A<SelectOwnerOptions>._))
            .Returns(A.Dummy<List<Card>>());
        A.CallTo(() => _dummyMapper.ToDTO(A<Card[]>._))
            .Returns(successResult);
        
        // Act
        object result = _cardsController.GetCards(tokenContent);
        
        // Assert
        Assert.IsInstanceOf<CardDTO[]>(result);
        Assert.That((CardDTO[])result, Is.EqualTo(successResult));
    }
}
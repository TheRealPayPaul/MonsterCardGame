using DTO;
using FakeItEasy;
using MonsterCardGame.Controllers;
using MonsterCardGame.Models.DB;
using MonsterCardGame.Models.PJWT;
using MonsterCardGame.Repositories;
using MonsterCardGame.Utilities;
using Server;
using Server.Enums;

namespace UnitTests.Controllers.Cards;

public class DeckTests
{
    private readonly TradeRepository _dummyTradeRepository = A.Fake<TradeRepository>();
    private readonly CardRepository _dummyCardRepository = A.Fake<CardRepository>();
    private readonly Mapper _dummyMapper = A.Fake<Mapper>();

    private readonly CardsController _cardsController;

    public DeckTests()
    {
        _cardsController = new CardsController(_dummyTradeRepository, _dummyCardRepository, _dummyMapper);
    }
    
    [SetUp]
    public void Setup()
    {
        
    }

    [Test]
    public void GetDeck_Success()
    {
        // Arrange
        CardDTO[] successResult = A.Dummy<CardDTO[]>();
        TokenContent tokenContent = new() { UserId = 1 };

        A.CallTo(() => _dummyCardRepository.SelectAllOfUser(A<int>._, A<SelectOwnerOptions>._))
            .Returns(A.Dummy<List<Card>>());
        A.CallTo(() => _dummyMapper.ToDTO(A<Card[]>._))
            .Returns(successResult);
        
        // Act
        object result = _cardsController.GetDeck(tokenContent);
        
        // Assert
        Assert.IsInstanceOf<CardDTO[]>(result);
        Assert.That((CardDTO[])result, Is.EqualTo(successResult));
    }
    
    [Test]
    public void PutDeck_BadRequest1()
    {
        // Arrange
        int[] cardIds = new[] { 1, 2, 3 };
        TokenContent tokenContent = new() { UserId = 1 };
        
        // Act
        object result = _cardsController.PutDeck(tokenContent, cardIds);
        
        // Assert
        Assert.IsInstanceOf<ActionResult>(result);
        Assert.That( ((ActionResult)result).ResponseCode, Is.EqualTo(ResponseCode.BadRequest) );
    }
    
    [Test]
    public void PutDeck_BadRequest2()
    {
        // Arrange
        int[] cardIds = new[] { 1, 2, 4, 4 };
        TokenContent tokenContent = new() { UserId = 1 };
        
        // Act
        object result = _cardsController.PutDeck(tokenContent, cardIds);
        
        // Assert
        Assert.IsInstanceOf<ActionResult>(result);
        Assert.That( ((ActionResult)result).ResponseCode, Is.EqualTo(ResponseCode.BadRequest) );
    }
    
    [Test]
    public void PutDeck_BadRequest3()
    {
        // Arrange
        int[] cardIds = new[] { 1, 2, 3, 4 };
        TokenContent tokenContent = new() { UserId = 1 };

        A.CallTo(() => _dummyTradeRepository.IsCardInTrade(A<int>._))
            .Returns(true);
        
        // Act
        object result = _cardsController.PutDeck(tokenContent, cardIds);
        
        // Assert
        Assert.IsInstanceOf<ActionResult>(result);
        Assert.That( ((ActionResult)result).ResponseCode, Is.EqualTo(ResponseCode.BadRequest) );
    }
    
    [Test]
    public void PutDeck_Forbidden()
    {
        // Arrange
        int[] cardIds = new[] { 1, 2, 3, 4 };
        TokenContent tokenContent = new() { UserId = 1 };

        A.CallTo(() => _dummyTradeRepository.IsCardInTrade(A<int>._))
            .Returns(false);
        A.CallTo(() => _dummyCardRepository.SelectById(A<int>._, A<int>._))
            .Returns(null);
        
        // Act
        object result = _cardsController.PutDeck(tokenContent, cardIds);
        
        // Assert
        Assert.IsInstanceOf<ResponseCode>(result);
        Assert.That( (ResponseCode)result, Is.EqualTo(ResponseCode.Forbidden) );
    }
}
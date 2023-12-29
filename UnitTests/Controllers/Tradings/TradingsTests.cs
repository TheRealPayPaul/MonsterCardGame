using DTO;
using FakeItEasy;
using MonsterCardGame.Controllers;
using MonsterCardGame.Enums;
using MonsterCardGame.Models.DB;
using MonsterCardGame.Models.PJWT;
using MonsterCardGame.Repositories;
using MonsterCardGame.Utilities;
using Server;
using Server.Enums;

namespace UnitTests.Controllers.Tradings;

public class TradingsTests
{
    private readonly TradeRepository _dummyTradeRepository = A.Fake<TradeRepository>();
    private readonly CardRepository _dummyCardRepository = A.Fake<CardRepository>();
    private readonly CompositeRepository _dummyCompositeRepository = A.Fake<CompositeRepository>();
    private readonly Mapper _dummyMapper = A.Fake<Mapper>();

    private readonly TradingsController _tradingsController;

    public TradingsTests()
    {
        _tradingsController = new TradingsController(
            _dummyTradeRepository,
            _dummyCardRepository,
            _dummyCompositeRepository,
            _dummyMapper);
    }
    
    [SetUp]
    public void Setup()
    {
        
    }

    [Test]
    public void GetAllOpenTrades_Success()
    {
        // Arrange
        A.CallTo(() => _dummyTradeRepository.SelectAll())
            .Returns(A.Dummy<IEnumerable<Trade>>());
        A.CallTo(() => _dummyMapper.ToDTO(A<Trade>._))
            .Returns(A.Dummy<TradeDTO>());

        // Act
        object result = _tradingsController.GetAllOpenTrades();

        // Assert
        Assert.IsInstanceOf<List<TradeDTO>>(result);
    }

    [Test]
    public void PostNewTrade_Forbidden1()
    {
        // Arrange
        CreateTradeDTO createTradeDto = new() { CardToTradeId = 1 };
        TokenContent tokenContent = new() { UserId = 1 };

        A.CallTo(() => _dummyCardRepository.SelectById(A<int>._, A<int>._))
            .Returns(null);

        // Act
        object result = _tradingsController.PostNewTrade(createTradeDto, tokenContent);

        // Assert
        Assert.IsInstanceOf<ActionResult>(result);
        Assert.That( ((ActionResult)result).ResponseCode, Is.EqualTo(ResponseCode.Forbidden) );
    }
    
    [Test]
    public void PostNewTrade_Forbidden2()
    {
        // Arrange
        CreateTradeDTO createTradeDto = new() { CardToTradeId = 1 };
        TokenContent tokenContent = new() { UserId = 1 };
        Card card = new() { DeckPos = 1 };

        A.CallTo(() => _dummyCardRepository.SelectById(A<int>._, A<int>._))
            .Returns(card);

        // Act
        object result = _tradingsController.PostNewTrade(createTradeDto, tokenContent);

        // Assert
        Assert.IsInstanceOf<ActionResult>(result);
        Assert.That( ((ActionResult)result).ResponseCode, Is.EqualTo(ResponseCode.Forbidden) );
    }
    
    [Test]
    public void PostNewTrade_Forbidden3()
    {
        // Arrange
        CreateTradeDTO createTradeDto = new() { CardToTradeId = 1 };
        TokenContent tokenContent = new() { UserId = 1 };
        Card card = new() { DeckPos = null };

        A.CallTo(() => _dummyCardRepository.SelectById(A<int>._, A<int>._))
            .Returns(card);
        A.CallTo(() => _dummyTradeRepository.IsCardInTrade(A<int>._))
            .Returns(true);
        
        // Act
        object result = _tradingsController.PostNewTrade(createTradeDto, tokenContent);

        // Assert
        Assert.IsInstanceOf<ActionResult>(result);
        Assert.That( ((ActionResult)result).ResponseCode, Is.EqualTo(ResponseCode.Forbidden) );
    }
    
    [Test]
    public void PostNewTrade_BadRequest4()
    {
        // Arrange
        CreateTradeDTO createTradeDto = new() { CardToTradeId = 1, WantedMinDamage = 0 };
        TokenContent tokenContent = new() { UserId = 1 };
        Card card = new() { DeckPos = null };

        A.CallTo(() => _dummyCardRepository.SelectById(A<int>._, A<int>._))
            .Returns(card);
        A.CallTo(() => _dummyTradeRepository.IsCardInTrade(A<int>._))
            .Returns(false);
        
        // Act
        object result = _tradingsController.PostNewTrade(createTradeDto, tokenContent);

        // Assert
        Assert.IsInstanceOf<ActionResult>(result);
        Assert.That( ((ActionResult)result).ResponseCode, Is.EqualTo(ResponseCode.BadRequest) );
    }
    
    [Test]
    public void PostNewTrade_BadRequest5()
    {
        // Arrange
        CreateTradeDTO createTradeDto = new() { CardToTradeId = 1, WantedMinDamage = 60, WantedCardType = "dwadhjiaw" };
        TokenContent tokenContent = new() { UserId = 1 };
        Card card = new() { DeckPos = null };

        A.CallTo(() => _dummyCardRepository.SelectById(A<int>._, A<int>._))
            .Returns(card);
        A.CallTo(() => _dummyTradeRepository.IsCardInTrade(A<int>._))
            .Returns(false);
        
        // Act
        object result = _tradingsController.PostNewTrade(createTradeDto, tokenContent);

        // Assert
        Assert.IsInstanceOf<ActionResult>(result);
        Assert.That( ((ActionResult)result).ResponseCode, Is.EqualTo(ResponseCode.BadRequest) );
    }
    
    [Test]
    public void PostNewTrade_InternalServerError()
    {
        // Arrange
        CreateTradeDTO createTradeDto = new() { CardToTradeId = 1, WantedMinDamage = 60, WantedCardType = "Monster" };
        TokenContent tokenContent = new() { UserId = 1 };
        Card card = new() { DeckPos = null };

        A.CallTo(() => _dummyCardRepository.SelectById(A<int>._, A<int>._))
            .Returns(card);
        A.CallTo(() => _dummyTradeRepository.IsCardInTrade(A<int>._))
            .Returns(false);
        A.CallTo(() => _dummyTradeRepository.Create(A<int>._, A<CardType>._, A<int>._))
            .Returns(false);
        
        // Act
        object result = _tradingsController.PostNewTrade(createTradeDto, tokenContent);

        // Assert
        Assert.IsInstanceOf<ResponseCode>(result);
        Assert.That((ResponseCode)result, Is.EqualTo(ResponseCode.InternalServerError));
    }
    
    [Test]
    public void PostNewTrade_Success()
    {
        // Arrange
        CreateTradeDTO createTradeDto = new() { CardToTradeId = 1, WantedMinDamage = 60, WantedCardType = "Monster" };
        TokenContent tokenContent = new() { UserId = 1 };
        Card card = new() { DeckPos = null };

        A.CallTo(() => _dummyCardRepository.SelectById(A<int>._, A<int>._))
            .Returns(card);
        A.CallTo(() => _dummyTradeRepository.IsCardInTrade(A<int>._))
            .Returns(false);
        A.CallTo(() => _dummyTradeRepository.Create(A<int>._, A<CardType>._, A<int>._))
            .Returns(true);
        
        // Act
        object result = _tradingsController.PostNewTrade(createTradeDto, tokenContent);

        // Assert
        Assert.IsInstanceOf<ResponseCode>(result);
        Assert.That((ResponseCode)result, Is.EqualTo(ResponseCode.Created));
    }
    
    [Test]
    public void DeleteTrade_NotFound()
    {
        // Arrange
        TokenContent tokenContent = new() { UserId = 1 };

        A.CallTo(() => _dummyTradeRepository.SelectById(A<int>._))
            .Returns(null);
        
        // Act
        object result = _tradingsController.DeleteTrade(A.Dummy<int>(), tokenContent);

        // Assert
        Assert.IsInstanceOf<ActionResult>(result);
        Assert.That( ((ActionResult)result).ResponseCode, Is.EqualTo(ResponseCode.NotFound) );
    }
    
    [Test]
    public void DeleteTrade_Forbidden()
    {
        // Arrange
        TokenContent tokenContent = new() { UserId = 1 };
        Trade trade = new( new Card() { OwnerId = 2 } );

        A.CallTo(() => _dummyTradeRepository.SelectById(A<int>._))
            .Returns(trade);
        
        // Act
        object result = _tradingsController.DeleteTrade(A.Dummy<int>(), tokenContent);

        // Assert
        Assert.IsInstanceOf<ActionResult>(result);
        Assert.That( ((ActionResult)result).ResponseCode, Is.EqualTo(ResponseCode.Forbidden) );
    }
    
    [Test]
    public void DeleteTrade_InternalServerError()
    {
        // Arrange
        TokenContent tokenContent = new() { UserId = 1 };
        Trade trade = new( new Card() { OwnerId = 1 } );

        A.CallTo(() => _dummyTradeRepository.SelectById(A<int>._))
            .Returns(trade);
        A.CallTo(() => _dummyTradeRepository.Delete(A<int>._))
            .Returns(false);
        
        // Act
        object result = _tradingsController.DeleteTrade(A.Dummy<int>(), tokenContent);

        // Assert
        Assert.IsInstanceOf<ResponseCode>(result);
        Assert.That( (ResponseCode)result, Is.EqualTo(ResponseCode.InternalServerError) );
    }
    
    [Test]
    public void DeleteTrade_Success()
    {
        // Arrange
        TokenContent tokenContent = new() { UserId = 1 };
        Trade trade = new( new Card() { OwnerId = 1 } );

        A.CallTo(() => _dummyTradeRepository.SelectById(A<int>._))
            .Returns(trade);
        A.CallTo(() => _dummyTradeRepository.Delete(A<int>._))
            .Returns(true);
        
        // Act
        object result = _tradingsController.DeleteTrade(A.Dummy<int>(), tokenContent);

        // Assert
        Assert.IsInstanceOf<ResponseCode>(result);
        Assert.That( (ResponseCode)result, Is.EqualTo(ResponseCode.Ok) );
    }

    [Test]
    public void PostAcceptTrade_NotFound()
    {
        // Arrange
        TokenContent tokenContent = new() { UserId = 1 };

        A.CallTo(() => _dummyTradeRepository.SelectById(A<int>._))
            .Returns(null);
        
        // Act
        object result = _tradingsController.PostAcceptTrade(A.Dummy<int>(), A.Dummy<int>(), tokenContent);

        // Assert
        Assert.IsInstanceOf<ActionResult>(result);
        Assert.That( ((ActionResult)result).ResponseCode, Is.EqualTo(ResponseCode.NotFound) );
    }
    
    [Test]
    public void PostAcceptTrade_Forbidden1()
    {
        // Arrange
        TokenContent tokenContent = new() { UserId = 1 };
        Trade trade = new( new Card() { OwnerId = 1 } );

        A.CallTo(() => _dummyTradeRepository.SelectById(A<int>._))
            .Returns(trade);
        
        // Act
        object result = _tradingsController.PostAcceptTrade(A.Dummy<int>(), A.Dummy<int>(), tokenContent);

        // Assert
        Assert.IsInstanceOf<ActionResult>(result);
        Assert.That( ((ActionResult)result).ResponseCode, Is.EqualTo(ResponseCode.Forbidden) );
    }
    
    [Test]
    public void PostAcceptTrade_Forbidden2()
    {
        // Arrange
        TokenContent tokenContent = new() { UserId = 1 };
        Trade trade = new( new Card() { OwnerId = 2 } );

        A.CallTo(() => _dummyTradeRepository.SelectById(A<int>._))
            .Returns(trade);
        A.CallTo(() => _dummyCardRepository.SelectById(A<int>._, A<int>._))
            .Returns(null);
        
        // Act
        object result = _tradingsController.PostAcceptTrade(A.Dummy<int>(), A.Dummy<int>(), tokenContent);

        // Assert
        Assert.IsInstanceOf<ActionResult>(result);
        Assert.That( ((ActionResult)result).ResponseCode, Is.EqualTo(ResponseCode.Forbidden) );
    }
    
    [Test]
    public void PostAcceptTrade_Forbidden3()
    {
        // Arrange
        TokenContent tokenContent = new() { UserId = 1 };
        Trade trade = new( new Card() { OwnerId = 2 } );
        Card toTradeCard = new() { Type = CardType.Spell };

        A.CallTo(() => _dummyTradeRepository.SelectById(A<int>._))
            .Returns(trade);
        A.CallTo(() => _dummyCardRepository.SelectById(A<int>._, A<int>._))
            .Returns(toTradeCard);
        
        // Act
        object result = _tradingsController.PostAcceptTrade(A.Dummy<int>(), A.Dummy<int>(), tokenContent);

        // Assert
        Assert.IsInstanceOf<ActionResult>(result);
        Assert.That( ((ActionResult)result).ResponseCode, Is.EqualTo(ResponseCode.Forbidden) );
    }
    
    [Test]
    public void PostAcceptTrade_Forbidden4()
    {
        // Arrange
        TokenContent tokenContent = new() { UserId = 1 };
        Trade trade = new( new Card() { OwnerId = 2 } ) { WantedMinDamage = 100, WantedCardType = CardType.Spell };
        Card toTradeCard = new() { Type = CardType.Monster, Damage = 90 };

        A.CallTo(() => _dummyTradeRepository.SelectById(A<int>._))
            .Returns(trade);
        A.CallTo(() => _dummyCardRepository.SelectById(A<int>._, A<int>._))
            .Returns(toTradeCard);
        
        // Act
        object result = _tradingsController.PostAcceptTrade(A.Dummy<int>(), A.Dummy<int>(), tokenContent);

        // Assert
        Assert.IsInstanceOf<ActionResult>(result);
        Assert.That( ((ActionResult)result).ResponseCode, Is.EqualTo(ResponseCode.Forbidden) );
    }
    
    [Test]
    public void PostAcceptTrade_InternalServerError()
    {
        // Arrange
        TokenContent tokenContent = new() { UserId = 1 };
        Trade trade = new( new Card() { OwnerId = 2 } ) { WantedMinDamage = 80, WantedCardType = CardType.Monster };
        Card toTradeCard = new() { Type = CardType.Monster, Damage = 90 };

        A.CallTo(() => _dummyTradeRepository.SelectById(A<int>._))
            .Returns(trade);
        A.CallTo(() => _dummyCardRepository.SelectById(A<int>._, A<int>._))
            .Returns(toTradeCard);
        A.CallTo(() => _dummyCompositeRepository.TradeCards(A<int>._, A<Card>._))
            .Returns(false);
        
        // Act
        object result = _tradingsController.PostAcceptTrade(A.Dummy<int>(), A.Dummy<int>(), tokenContent);

        // Assert
        Assert.IsInstanceOf<ResponseCode>(result);
        Assert.That( (ResponseCode)result, Is.EqualTo(ResponseCode.InternalServerError) );
    }
    
    [Test]
    public void PostAcceptTrade_Success()
    {
        // Arrange
        TokenContent tokenContent = new() { UserId = 1 };
        Trade trade = new( new Card() { OwnerId = 2 } ) { WantedMinDamage = 80, WantedCardType = CardType.Monster };
        Card toTradeCard = new() { Type = CardType.Monster, Damage = 90 };

        A.CallTo(() => _dummyTradeRepository.SelectById(A<int>._))
            .Returns(trade);
        A.CallTo(() => _dummyCardRepository.SelectById(A<int>._, A<int>._))
            .Returns(toTradeCard);
        A.CallTo(() => _dummyCompositeRepository.TradeCards(A<int>._, A<Card>._))
            .Returns(true);
        
        // Act
        object result = _tradingsController.PostAcceptTrade(A.Dummy<int>(), A.Dummy<int>(), tokenContent);

        // Assert
        Assert.IsInstanceOf<ResponseCode>(result);
        Assert.That( (ResponseCode)result, Is.EqualTo(ResponseCode.Ok) );
    }
}
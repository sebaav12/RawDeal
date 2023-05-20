using RawDealView;
using RawDealView.Formatters;

namespace RawDeal;

public class CardData
{
    public string Title { get; set; }
    public List<string> Types { get; set; }
    public List<string> Subtypes { get; set; }
    public string Fortitude { get; set; }
    public string Damage { get; set; }
    public string StunValue { get; set; }
    public string CardEffect { get; set; }
}

public interface ICard
{
    string Title { get; set; }
    List<string> Types { get; set; }
    List<string> Subtypes { get; set; }
    string Fortitude { get; set; }
    string Damage { get; set; }
    string StunValue { get; set; }
    string CardEffect { get; set; }

    string NoEffect(Player playerOne, Player playerTwo, View _view, List<(string card, int positionInHand, int positionInHandJugable, int positionInCartasJugablesList, string type)> cartaEscogidaPositionInHand);
    string Effect(Player player, int positionCardInHand, View _view);
}

public class CardsWithoutEffects : ICard
{
    public string Title { get; set; }
    public List<string> Types { get; set; }
    public List<string> Subtypes { get; set; }
    public string Fortitude { get; set; }
    public string Damage { get; set; }
    public string StunValue { get; set; }
    public string CardEffect { get; set; }
    
    public string Effect(Player player, int positionCardInHand, View _view)
    {
        List<ICard> cartasHand = player.GetCardsHand();
        ICard cartaToDiscard= cartasHand[positionCardInHand];
        
        player.RemoveCardOfHand(positionCardInHand);
        player.AddCardToRingArea(cartaToDiscard);
        
        return "hola";
    }
    
    public string NoEffect(Player playerOne, Player playerTwo, View _view, List<(string card, int positionInHand, int positionInHandJugable, int positionInCartasJugablesList, string type)> cartaEscogidaPositionInHand)
    {
        
        List<ICard> cartasHand = playerOne.GetCardsHand();
        int position = cartaEscogidaPositionInHand[0].positionInHand;
        ICard cartaToDiscard= cartasHand[cartaEscogidaPositionInHand[0].positionInHand];
        
        playerOne.RemoveCardOfHand(position);
        playerOne.AddCardToRingArea(cartaToDiscard);
        
        // Obtenemos el daño a aplicar // ojo si oponente es "MANKIND"
        string damageDone = Damage;
        int damageNumber = Int32.Parse(damageDone);
        int plusFotitude = damageNumber;

        if (playerTwo.GetName() == "MANKIND")
        {
            if (damageNumber >= 1)
            {
                damageNumber = damageNumber - 1;
            }
        }

        // Lista para almacenar la info de las cartas que seran mostradas
        List<string> cardsinfo = new List<string> { };

        int contador = 0;
        List<ICard> cardsArsenal = playerTwo.GetCardsArsenal();
        int totalCards = cardsArsenal.Count;
        int startIndex = Math.Max(0, totalCards - damageNumber);

        for (int i = totalCards - 1; i >= startIndex; i--)
        {
            ICard card = cardsArsenal[i];

            if (contador < damageNumber)
            {
                cardsinfo.Add(InfoCardInString(card));
                playerTwo.AddCardToRingSide(card);
                contador++;
            }
            else
            {
                break;
            }
        }

        _view.SayThatPlayerSuccessfullyPlayedACard();

        if (damageNumber > 0)
        {
            _view.SayThatOpponentWillTakeSomeDamage(playerTwo.GetName(), damageNumber);
        }
        

        // Mostramos en pantalla las cartas del oponente que fueron pasadas de su arsenal a su ringside
        int currentDamage = 1;
        foreach (var card in cardsinfo)
        {
            var carta = new List<string>();
            carta.Add(card);
            _view.ShowCardOverturnByTakingDamage(card, currentDamage, damageNumber);
            currentDamage++;
        }
        
        // Removemos las cartas del arsenal del oponente surante el turno pierde 
        if (playerTwo.NumberOfCardInArsenal() < damageNumber)
        {
            _view.CongratulateWinner(playerOne.GetName());
            return "Gana Player One";
        }
        if (playerTwo.NumberOfCardInArsenal() >= damageNumber)
        {
            playerTwo.RemoveManyCardsArsenal(damageNumber);
            playerOne.AddFortitude(plusFotitude);
        }
        
        // Fin Jugada como Maniobra

        return null;
    }
   
    private string InfoCardInString(ICard card)
    {
        string titulo = card.Title;
        List<string> cardTypes = card.Types;
        List<string> subtypes = card.Subtypes;
        string fortitude = card.Fortitude;
        string damage = card.Damage;
        string stunValue = card.StunValue;
        string cardEffect = card.CardEffect;
        
        //string cardInfo = Formatter.CardToString(titulo, fortitude, damage, stunValue, cardTypes, subtypes, cardEffect);
        
        ViewableCardInfo cardInfo = new ViewableCardInfo(titulo, fortitude, damage, stunValue, cardTypes, subtypes, cardEffect);
        string cardStr = Formatter.CardToString(cardInfo);
        return cardStr;
    }
}


public class CardsThatForceYouToDiscardACard : ICard
{
    public string Title { get; set; }
    public List<string> Types { get; set; }
    public List<string> Subtypes { get; set; }
    public string Fortitude { get; set; }
    public string Damage { get; set; }
    public string StunValue { get; set; }
    public string CardEffect { get; set; }

    // Descartar la carta (agregar al ringside) y robar una carta del arsenal sin aplicar daño
    public string Effect(Player player, int positionCardInHand, View _view)
    {
        List<ICard> cartasHand = player.GetCardsHand();
        ICard cartaToDiscard= cartasHand[positionCardInHand];
        string titleOfCard = cartaToDiscard.Title;
        
        player.RemoveCardOfHand(positionCardInHand);
        player.AddCardToRingSide(cartaToDiscard);
        player.takeOneCard();
        
        _view.SayThatPlayerSuccessfullyPlayedACard();
        _view.SayThatPlayerMustDiscardThisCard(player.GetName(), titleOfCard);
        _view.SayThatPlayerDrawCards(player.GetName(), 1);
        
        return "hola";
    }
    
    public string NoEffect(Player playerOne, Player playerTwo, View _view, List<(string card, int positionInHand, int positionInHandJugable , int positionInCartasJugablesList, string type)> cartaEscogidaPositionInHand)
    {
        List<ICard> cartasHand = playerOne.GetCardsHand();
        int position = cartaEscogidaPositionInHand[0].positionInHand;
        ICard cartaToDiscard= cartasHand[cartaEscogidaPositionInHand[0].positionInHand];
        
        playerOne.RemoveCardOfHand(position);
        playerOne.AddCardToRingArea(cartaToDiscard);
        
        // Obtenemos el daño a aplicar // ojo si oponente es "MANKIND"
        string damageDone = Damage;
        int damageNumber = Int32.Parse(damageDone);
        int plusFotitude = damageNumber;

        if (playerTwo.GetName() == "MANKIND")
        {
            if (damageNumber >= 1)
            {
                damageNumber = damageNumber - 1;
            }
        }

        // Lista para almacenar la info de las cartas que seran mostradas
        List<string> cardsinfo = new List<string> { };

        int contador = 0;
        List<ICard> cardsArsenal = playerTwo.GetCardsArsenal();
        int totalCards = cardsArsenal.Count;
        int startIndex = Math.Max(0, totalCards - damageNumber);

        for (int i = totalCards - 1; i >= startIndex; i--)
        {
            ICard card = cardsArsenal[i];

            if (contador < damageNumber)
            {
                cardsinfo.Add(InfoCardInString(card));
                playerTwo.AddCardToRingSide(card);
                contador++;
            }
            else
            {
                break;
            }
        }

        _view.SayThatPlayerSuccessfullyPlayedACard();
        
        if (damageNumber > 0)
        {
            _view.SayThatOpponentWillTakeSomeDamage(playerTwo.GetName(), damageNumber);
        }

        // Mostramos en pantalla las cartas del oponente que fueron pasadas de su arsenal a su ringside
        int currentDamage = 1;
        foreach (var card in cardsinfo)
        {
            var carta = new List<string>();
            carta.Add(card);
            _view.ShowCardOverturnByTakingDamage(card, currentDamage, damageNumber);
            currentDamage++;
        }
        
        // Removemos las cartas del arsenal del oponente surante el turno pierde 
        if (playerTwo.NumberOfCardInArsenal() < damageNumber)
        {
            _view.CongratulateWinner(playerOne.GetName());
            return "Gana Player One";
        }
        if (playerTwo.NumberOfCardInArsenal() >= damageNumber)
        {
            playerTwo.RemoveManyCardsArsenal(damageNumber);
            playerOne.AddFortitude(plusFotitude);
        }
        
        // Fin Jugada como Maniobra

        return null;
    }    
    private string InfoCardInString(ICard card)
    {
        string titulo = card.Title;
        List<string> cardTypes = card.Types;
        List<string> subtypes = card.Subtypes;
        string fortitude = card.Fortitude;
        string damage = card.Damage;
        string stunValue = card.StunValue;
        string cardEffect = card.CardEffect;
        
        //string cardInfo = Formatter.CardToString(titulo, fortitude, damage, stunValue, cardTypes, subtypes, cardEffect);
        
        ViewableCardInfo cardInfo = new ViewableCardInfo(titulo, fortitude, damage, stunValue, cardTypes, subtypes, cardEffect);
        string cardStr = Formatter.CardToString(cardInfo);
        return cardStr;
    }
}


public class CardsWithOtherEffects : ICard
{
    public string Title { get; set; }
    public List<string> Types { get; set; }
    public List<string> Subtypes { get; set; }
    public string Fortitude { get; set; }
    public string Damage { get; set; }
    public string StunValue { get; set; }
    public string CardEffect { get; set; }

    public string Effect(Player player, int positionCardInHand, View _view)
    {
        List<ICard> cartasHand = player.GetCardsHand();
        ICard cartaToDiscard= cartasHand[positionCardInHand];
        
        player.RemoveCardOfHand(positionCardInHand);
        player.AddCardToRingArea(cartaToDiscard);
        
        return "hola";
    }
    
    public string NoEffect(Player playerOne, Player playerTwo, View _view, List<(string card, int positionInHand, int positionInHandJugable, int positionInCartasJugablesList, string type)> cartaEscogidaPositionInHand)
    {
        
        List<ICard> cartasHand = playerOne.GetCardsHand();
        int position = cartaEscogidaPositionInHand[0].positionInHand;
        ICard cartaToDiscard= cartasHand[cartaEscogidaPositionInHand[0].positionInHand];
        
        playerOne.RemoveCardOfHand(position);
        playerOne.AddCardToRingArea(cartaToDiscard);
        
        // Obtenemos el daño a aplicar // ojo si oponente es "MANKIND"
        string damageDone = Damage;
        int damageNumber = Int32.Parse(damageDone);
        int plusFotitude = damageNumber;

        if (playerTwo.GetName() == "MANKIND")
        {
            if (damageNumber >= 1)
            {
                damageNumber = damageNumber - 1;
            }
        }

        // Lista para almacenar la info de las cartas que seran mostradas
        List<string> cardsinfo = new List<string> { };

        int contador = 0;
        List<ICard> cardsArsenal = playerTwo.GetCardsArsenal();
        int totalCards = cardsArsenal.Count;
        int startIndex = Math.Max(0, totalCards - damageNumber);

        for (int i = totalCards - 1; i >= startIndex; i--)
        {
            ICard card = cardsArsenal[i];

            if (contador < damageNumber)
            {
                cardsinfo.Add(InfoCardInString(card));
                playerTwo.AddCardToRingSide(card);
                contador++;
            }
            else
            {
                break;
            }
        }

        _view.SayThatPlayerSuccessfullyPlayedACard();
        
        if (damageNumber > 0)
        {
            _view.SayThatOpponentWillTakeSomeDamage(playerTwo.GetName(), damageNumber);
        }

        // Mostramos en pantalla las cartas del oponente que fueron pasadas de su arsenal a su ringside
        int currentDamage = 1;
        foreach (var card in cardsinfo)
        {
            var carta = new List<string>();
            carta.Add(card);
            _view.ShowCardOverturnByTakingDamage(card, currentDamage, damageNumber);
            currentDamage++;
        }
        
        // Removemos las cartas del arsenal del oponente surante el turno pierde 
        if (playerTwo.NumberOfCardInArsenal() < damageNumber)
        {
            _view.CongratulateWinner(playerOne.GetName());
            return "Gana Player One";
        }
        if (playerTwo.NumberOfCardInArsenal() >= damageNumber)
        {
            playerTwo.RemoveManyCardsArsenal(damageNumber);
            playerOne.AddFortitude(plusFotitude);
        }
        
        // Fin Jugada como Maniobra

        return null;
    }
    
    private string InfoCardInString(ICard card)
    {
        string titulo = card.Title;
        List<string> cardTypes = card.Types;
        List<string> subtypes = card.Subtypes;
        string fortitude = card.Fortitude;
        string damage = card.Damage;
        string stunValue = card.StunValue;
        string cardEffect = card.CardEffect;
        
        //string cardInfo = Formatter.CardToString(titulo, fortitude, damage, stunValue, cardTypes, subtypes, cardEffect);
        
        ViewableCardInfo cardInfo = new ViewableCardInfo(titulo, fortitude, damage, stunValue, cardTypes, subtypes, cardEffect);
        string cardStr = Formatter.CardToString(cardInfo);
        return cardStr;
    }
}



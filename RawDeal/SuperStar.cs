using RawDealView;

namespace RawDeal.data;
using System.Text.Json.Serialization;
using RawDealView.Formatters;

public class SuperStar
{
    public string Name { get; set; }
    public string Logo { get; set; }

    [JsonPropertyName("Hand Size")]
    public int HandSize { get; set; }

    [JsonPropertyName("Superstar Value")]
    public int SuperstarValue { get; set; }

    [JsonPropertyName("Superstar Ability")]
    public string SuperstarAbility { get; set; }
    
    public List<string> GetInfoCardsInRingSide(Player player)
    {
        List<string> cardsinfo = new List<string> { };
                        
        foreach (ICard card in player.GetCardsRingSide())
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
            cardsinfo.Add(cardStr);
        }

        return cardsinfo;
    }
    public List<string> GetInfoCardsInHand(Player player)
    {
        List<string> cardsinfo = new List<string> { };
        foreach (ICard card in player.GetCardsHand())
        {
            string titulo = card.Title;
            List<string> cardTypes = card.Types;
            List<string> subtypes = card.Subtypes;
            string fortitude = card.Fortitude;
            string damage = card.Damage;
            string stunValue = card.StunValue;
            string cardEffect = card.CardEffect;
            
            ViewableCardInfo cardInfo = new ViewableCardInfo(titulo, fortitude, damage, stunValue, cardTypes, subtypes, cardEffect);
            string cardStr = Formatter.CardToString(cardInfo);
            //string cardInfo = Formatter.CardToString(titulo, fortitude, damage, stunValue, cardTypes, subtypes, cardEffect);
            cardsinfo.Add(cardStr);
        }

        return cardsinfo;
    }
    public string InfoCardInString(ICard card)
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

public class TheRock : SuperStar
{
    public void Habilidad(Player player, View _view)
    {
        int numCartasRingSide = player.NumberOfCardsInRingSide();

        if (numCartasRingSide > 0)
        {
            bool useHability = _view.DoesPlayerWantToUseHisAbility("THE ROCK");
            if (useHability == true)
            {
                List<string> cardsinfo = GetInfoCardsInRingSide(player);
                int numCartas = 1;
                int cartaElegida = _view.AskPlayerToSelectCardsToRecover("THE ROCK", numCartas, cardsinfo);
                
                // Retiramos la carta elegida del RingSide y la dejamos en la posición 0 del arsenal
                player.moveOneCardRingSideToArsenal(cartaElegida);
            }
        }
    }
}

public class Mankind : SuperStar
{
    public void Hability(Player player, View _view)
    {
        if (player.NumberOfCardInArsenal() >= 2)
        {
            player.takeOneCard();
            player.takeOneCard();
        }
        else
        {
            player.takeOneCard();
        }
    }
}

public class Kane : SuperStar
{
    public void Hability(Player playerOne, Player playerTwo, View _view)
    {
        // El jugador es KANE => usa su habilidad
            if (playerTwo.NumberOfCardInArsenal() <= 1)
            {
                // Oponente pierde la partida, Jugador gana
                _view.CongratulateWinner(playerOne.GetName());
            }
            else
            {
                    int damageNumber = 1;
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
                    
                    // Decimos que el jugador usa su habilidad y el oponente toma daño 
                    _view.SayThatPlayerIsGoingToUseHisAbility(playerOne.GetName(), playerOne.CardSuperStar().SuperstarAbility);
                    _view.SayThatOpponentWillTakeSomeDamage(playerTwo.GetName(), 1);
                    
                    // Mostramos en pantalla las cartas del oponente que fueron pasadas de su arsenal a su ringside
                    int currentDamage = 1;
                    foreach (var card in cardsinfo)
                    {
                        var carta = new List<string>();
                        carta.Add(card);
                        _view.ShowCardOverturnByTakingDamage(card,currentDamage,damageNumber);
                        currentDamage++;
                    }
                    playerTwo.RemoveManyCardsArsenal(damageNumber);
            }
    }
}

public class TheUnderTaker : SuperStar
{
    public void Hability(Player player, View _view)
    {
        int numCardsInHand = player.NumberOfCardsInHand();
        if (numCardsInHand >= 2)
        {
            // Se descarta la primera carta
            _view.SayThatPlayerIsGoingToUseHisAbility(player.GetName(),
                player.CardSuperStar().SuperstarAbility);
            List<string> cardsHandInfo = GetInfoCardsInHand(player);
            int totalCardsToDiscard = 2;
            int idCartaDescartada = _view.AskPlayerToSelectACardToDiscard(cardsHandInfo,
                player.GetName(), player.GetName(), totalCardsToDiscard);
            player.moveOneCardOfHandToRingSide(idCartaDescartada);

            // Se descarta la segunda carta 
            List<string> cardsHandInfo2 = GetInfoCardsInHand(player);
            int totalCardsToDiscard2 = 1;
            int idCartaDescartada2 = _view.AskPlayerToSelectACardToDiscard(cardsHandInfo2,
                player.GetName(), player.GetName(), totalCardsToDiscard2);
            player.moveOneCardOfHandToRingSide(idCartaDescartada2);

            // Se pasa una carta del RingSide a la Mano
            List<string> cardsRingSideInfo = GetInfoCardsInRingSide(player);
            int numCartas = 1;
            int cartaElegida =
                _view.AskPlayerToSelectCardsToPutInHisHand("THE UNDERTAKER", numCartas, cardsRingSideInfo);

            // Retiramos la carta elegida del RingSide y la dejamos en la posición 0 del arsenal
            player.moveOneCardRingSideToHand(cartaElegida);
        }
    }
}

public class ChrisJericho : SuperStar
{
    public void Hability(Player playerOne, Player playerTwo, View _view)
    {
        int numCardsInHand = playerOne.NumberOfCardsInHand();
        if (numCardsInHand >= 1)
        {
            // Primera Parte: Juagdor descarta una carta
            _view.SayThatPlayerIsGoingToUseHisAbility(playerOne.GetName(),
                playerOne.CardSuperStar().SuperstarAbility);
            List<string> cardsHandInfo = GetInfoCardsInHand(playerOne);

            // Se descarta 1 carta del jugador 
            int totalCardsToDiscard = 1;
            int idCartaDescartada = _view.AskPlayerToSelectACardToDiscard(cardsHandInfo,
                playerOne.GetName(), playerOne.GetName(), totalCardsToDiscard);
            playerOne.moveOneCardOfHandToRingSide(idCartaDescartada);

            // Parte 2: Oponente descarta una carta 
            List<string> cardsHandInfoOpponent = GetInfoCardsInHand(playerTwo);

            // Se descarta 1 carta del jugador Oponente
            int totalCardsToDiscardOpponent = 1;
            int idCartaDescartadaOpponent = _view.AskPlayerToSelectACardToDiscard(cardsHandInfoOpponent,
                playerTwo.GetName(), playerTwo.GetName(), totalCardsToDiscardOpponent);
            playerTwo.moveOneCardOfHandToRingSide(idCartaDescartadaOpponent);
        }
    }
}

public class SteveColdSteveAustin : SuperStar
{
    public void Hability(Player playerOne, View _view)
    {
        int numCardsInArsenal = playerOne.NumberOfCardInArsenal();
        if (numCardsInArsenal >= 1)
        {
            // Se roba una carta del arsenenal
            playerOne.takeOneCard();
            _view.SayThatPlayerIsGoingToUseHisAbility(playerOne.GetName(),
                playerOne.CardSuperStar().SuperstarAbility);
            _view.SayThatPlayerDrawCards(playerOne.GetName(), 1);

            List<string> cardsHandInfo = new List<string> { };
            foreach (ICard card in playerOne.GetCardsHand())
            {
                string titulo = card.Title;
                List<string> cardTypes = card.Types;
                List<string> subtypes = card.Subtypes;
                string fortitude = card.Fortitude;
                string damage = card.Damage;
                string stunValue = card.StunValue;
                string cardEffect = card.CardEffect;
                
                ViewableCardInfo cardInfo = new ViewableCardInfo(titulo, fortitude, damage, stunValue, cardTypes, subtypes, cardEffect);
                string cardStr = Formatter.CardToString(cardInfo);
                
                //string cardInfo = Formatter.CardToString(titulo, fortitude, damage, stunValue, cardTypes,subtypes, cardEffect);
                cardsHandInfo.Add(cardStr);
            }

            // Se escoge una carta del Hand
            int idCartaDescartada =
                _view.AskPlayerToReturnOneCardFromHisHandToHisArsenal(playerOne.GetName(), cardsHandInfo);

            // Se mueve la carta seleccionada del Hand al Arsenal en posición 0
            playerOne.MoveSelectedCardOfHandToArsenal(idCartaDescartada);
        }
    }
}
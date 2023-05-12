using RawDealView;
using System.Text.Json ;
using RawDeal.data;
using RawDealView.Formatters;
using RawDealView.Options;

namespace RawDeal;

public class Game
{
    private View _view;
    private string _deckFolder;

    public Game(View view, string deckFolder)
    {
        _view = view;
        _deckFolder = deckFolder;
    }
    
    public void Play()
    {
        var cards = GenerateCardsInstances();
        var superStars = GenerateSuperStarsInstances();

        Validate validate = new Validate();
        
        List<string> listCardsPlayer1 = SelectDeck();
        List<object> deckJugadorInfo1 = validate.ValidateDeck(listCardsPlayer1, cards, superStars);
        bool deckJugador1Valido = Convert.ToBoolean(deckJugadorInfo1[0]);

        if (deckJugador1Valido == true)
        {
            List<string> listCardsPlayer2 = SelectDeck();
            List<object> deckJugadorInfo2 = validate.ValidateDeck(listCardsPlayer2, cards, superStars);
            bool deckJugador2Valido = Convert.ToBoolean(deckJugadorInfo2[0]);

            if (deckJugador2Valido == true)
            {
                Player playerOne = new Player("Jugador 1");
                Player playerTwo = new Player("Jugador 2");

                GiveCardsToPlayer(playerOne, superStars, listCardsPlayer1);
                SaveCardsInArsenal(playerOne, listCardsPlayer1, cards);

                GiveCardsToPlayer(playerTwo, superStars, listCardsPlayer2);
                SaveCardsInArsenal(playerTwo, listCardsPlayer2, cards);
                
                int valueSuperStar1 = Convert.ToInt32(deckJugadorInfo1[1]);
                int valueSuperStar2 = Convert.ToInt32(deckJugadorInfo2[1]);
                
                List<int> numSuperStars = new List<int>() {
                    valueSuperStar1, valueSuperStar2
                };
                
                LoopOfGame(numSuperStars, playerOne, playerTwo);
            }
            else if (deckJugador2Valido == false)
            {
                _view.SayThatDeckIsInvalid();
            }
        }
        else
        {
            // caso: primer deck esta mal
            _view.SayThatDeckIsInvalid();
        }
    }
    
    // Funciones para generar las instancias basicas
    private List<SuperStar>  GenerateSuperStarsInstances()
    {
        // almacenamos las cartas de SuperStar en una lista de instancias de la clase SuperStar
        string rutaSuperStar = Path.Combine("data", "superstar.json");
        string jsonSuperStar = File.ReadAllText(rutaSuperStar);
        var superStars = JsonSerializer.Deserialize<List<SuperStar>>(jsonSuperStar);
        return superStars;
    }
    private List<Card> GenerateCardsInstances()
    {
        // almacenamos las cartas en una lista de instancias de la clase Carta
        string cardsPath = Path.Combine("data", "cards.json");
        string jsonCards = File.ReadAllText(cardsPath);
        var cards = JsonSerializer.Deserialize<List<Card>>(jsonCards);
        return cards;
    }
    
    
    //  Seleccionar Deck
    private List<string> SelectDeck()
    {
        // Le pedimos al jugador elegir un maso, obtenemos una lista con los nombres de las cartas
        string addressPlayer1 = _view.AskUserToSelectDeck(_deckFolder);
        string[] lineas = File.ReadAllLines(addressPlayer1);
        List<string> listCardsPlayer1 = new List<string>(lineas);
        return listCardsPlayer1;
    }
    
    
    // Inicio del Juego
    private void GiveCardsToPlayer(Player player, List<SuperStar> superStars, List<string> listCardsPlayer)
    {
        // Entrega Cartas Jugadoe 1: Se guarda la super estrella del jugador 1
        string originalString1 = listCardsPlayer[0];
        string nameSuperStar1 = originalString1.Substring(0, originalString1.Length - 17);

        // Removemos la Super estrella de la lista de cartas del jugador
        foreach (var superStar in superStars)
        {
            if (superStar.Name == nameSuperStar1)
            {
                player.GetSuperStar(superStar);
                listCardsPlayer.RemoveAt(0);
            }
        }
    }
    private void LoopOfGame(List<int> numSuperStars, Player playerOne, Player playerTwo)
    {
        // parte quien tenga mayor superstar value
        int numSuper1 = numSuperStars[0];
        int numSuper2 = numSuperStars[1];
                
        // Se define quien inicia el juego
        if (numSuper1 >= numSuper2)
                {
                    playerOne.CreateInitialHand();
                    playerTwo.CreateInitialHand();
                    
                    string juego = "";
                    while (string.IsNullOrEmpty(juego))
                    {
                        string next1 = Turn(playerOne, playerTwo);
                        if (next1 != "Siguiente")
                        {
                            juego = "Fin";
                        }
                        else
                        {
                            string next2 = Turn(playerTwo, playerOne);
                            if (next2 != "Siguiente")
                            {
                                juego = "Fin";
                            }
                        }
                    }
                }
        else
                {
                    // Jugadores roban mano inicial 
                    playerOne.CreateInitialHand();
                    playerTwo.CreateInitialHand();

                    // Inician los turnos 
                    string juego = "";
                    while (string.IsNullOrEmpty(juego))
                    {
                        string next1 = Turn(playerTwo, playerOne);
                        if (next1 != "Siguiente")
                        {
                            juego = "Fin";
                        }
                        else
                        {
                            string next2 = Turn(playerOne, playerTwo);
                            if (next2 != "Siguiente")
                            {
                                juego = "Fin";
                            }
                        }
                    }
                }
    }
    
    
    // aca ocurren los turnos, se considera player One al jugador "que esta jugando"
    private string Turn(Player playerOne, Player playerTwo)
    {
        // Before your draw segment
        _view.SayThatATurnBegins(Convert.ToString(playerOne.GetName()));
        ExecuteIfPlayTheRock(playerOne);

        // Draw segment: Roba una Carta del Arsenal y se pone en la mano del jugador 
        ExecuteIfPlayMankind(playerOne);
        ExecuteIfPlayKane(playerOne, playerTwo);

        string result = null;
        bool habilidadUsada = false;

        while (string.IsNullOrEmpty(result))
        {
            ShowBasicInfoOfGame(playerOne, playerTwo);
            NextPlay menuPrincipal = GenerateMenuPrincipal(playerOne, habilidadUsada);
            if (menuPrincipal == NextPlay.ShowCards)
            {
                IfPlayerSelectShowCards(playerOne, playerTwo);
            }
            else if (menuPrincipal == NextPlay.PlayCard)
            {
                string value = IfPlayerSelectPlayCard(playerOne, playerTwo);
                if (value != null)
                {
                    result = "Alto";
                }
            }
            else if (menuPrincipal == NextPlay.EndTurn)
            {
                if (playerTwo.NumberOfCardInArsenal() == 0)
                {
                    _view.CongratulateWinner(playerOne.GetName());
                    result = "Rendirse";
                }
                else
                {
                    result = "Siguiente";
                }
            }
            else if (menuPrincipal == NextPlay.GiveUp)
            {
                _view.CongratulateWinner(playerTwo.GetName());
                result = "Rendirse";
            }
            else if (menuPrincipal == NextPlay.UseAbility)
            {
                if (playerOne.GetName() == "THE UNDERTAKER")
                {
                    ExecuteIfPlayTheUnderTaker(playerOne);
                }

                if (playerOne.GetName() == "CHRIS JERICHO")
                {
                    ExecuteIfPlayChrisJericho(playerOne, playerTwo);
                }

                if (playerOne.GetName() == "STONE COLD STEVE AUSTIN")
                {
                    ExecuteIfPlaySteveColdSteveAustin(playerOne);
                }
                habilidadUsada = true;
            }
        }
        return result;
    }

    
    // Funciones para el menu principal
    private void ShowBasicInfoOfGame(Player playerOne, Player playerTwo)
    {
        // Desplegamos info basica
        PlayerInfo PlayerOneInfo = new PlayerInfo(playerOne.GetName(), playerOne.GetFortitude(),playerOne.NumberOfCardsInHand(), playerOne.NumberOfCardInArsenal());
        PlayerInfo PlayerTwoInfo = new PlayerInfo(playerTwo.GetName(), playerTwo.GetFortitude(),playerTwo.NumberOfCardsInHand(), playerTwo.NumberOfCardInArsenal());
        _view.ShowGameInfo(PlayerOneInfo, PlayerTwoInfo);
    }
    private NextPlay GenerateMenuPrincipal(Player playerOne, bool habilidadUsada)
    {
        // Se despliega uno de los dos tipos de menu: Con y Sin habilidad (Habilidad Opcional)
        List<string> playersWhitOptionalHabilities = new List<string>()
            { "STONE COLD STEVE AUSTIN", "CHRIS JERICHO", "THE UNDERTAKER" };
        List<string> playersWithoutOptionalHabilities = new List<string>() { "MANKIND", "KANE", "HHH", "THE ROCK" };

        NextPlay menuPrincipal;

        if (playersWhitOptionalHabilities.Contains(playerOne.GetName()))
        {
            if (habilidadUsada == false)
            {
                return _view.AskUserWhatToDoWhenUsingHisAbilityIsPossible();
            }
            else
            {
                return _view.AskUserWhatToDoWhenHeCannotUseHisAbility();
            }
        }
        else
        {
            return _view.AskUserWhatToDoWhenHeCannotUseHisAbility();
        }
    }
    private string IfPlayerSelectPlayCard(Player playerOne, Player playerTwo)
    {
        // Generamos una lista con las cartas jugables 
        List<List<(string, int)>> cartasJugablesPosition = new List<List<(string, int)>>();
        List<Card> cartasJugablesInstancia = new List<Card>();
        List<string> cartasJugables = new List<string>();

        // Almacenaremos la posición en el hand de las cartas
        int position = 0;

        foreach (Card card in playerOne.GetCardsHand())
        {
            if (int.Parse(card.Fortitude) <= playerOne.GetFortitude())
            {
                if (!card.Types.Contains("Reversal"))
                {
                    InfoCardInViewableClass(card);
                    string tipoCard = card.Types[0].ToUpper(); // estamos asumiendo que las cartas no son hibridas
                    
                    ViewablePlayInfo playInfo = new ViewablePlayInfo(InfoCardInViewableClass(card), tipoCard);
                    string cardToPlay = Formatter.PlayToString(playInfo);
                    
                    List<(string, int)> cardToPlayInfo = new List<(string, int)>();
                    cardToPlayInfo.Add((cardToPlay, position));
                    cartasJugablesPosition.Add(cardToPlayInfo);
                    cartasJugables.Add(cardToPlay);
                    cartasJugablesInstancia.Add(card);
                }
            }

            position++;
        }

        // Obtenemos un integer con la opción escogida
        int actionPlay = _view.AskUserToSelectAPlay(cartasJugables);

        // El jugador decide jugar una carta
        if (actionPlay != -1)
        {
            List<(string card, int valor)> cartaElegidaPosition = cartasJugablesPosition[actionPlay];
            Card cartaElegida = cartasJugablesInstancia[actionPlay];
            
            List<Player> players = new List<Player>() {
                playerOne, playerTwo
            };
            
            string play = PlayCard(players, cartaElegida, cartaElegidaPosition);
            if (play != null)
            {
                return "Fin";
            }
        }

        // El jugador decide volver al menu principal
        else if (actionPlay == -1)
        {
            // seguimos con nuestras vidas y se retorna al menu principal
            return null;
        }

        return null;
    }
    private void IfPlayerSelectShowCards(Player playerOne, Player playerTwo)
    {
        CardSet menuShow = _view.AskUserWhatSetOfCardsHeWantsToSee();
        if (menuShow == CardSet.Hand)
        {
            List<string> cardsinfo = GetInfoCardsInHand(playerOne);
            _view.ShowCards(cardsinfo);
        }
        else if (menuShow == CardSet.RingArea)
        {
            List<string> cardsinfo = GetInfoCardsInRingArea(playerOne);
            _view.ShowCards(cardsinfo);
        }
        else if (menuShow == CardSet.RingsidePile)
        {
            List<string> cardsinfo = GetInfoCardsInRingSide(playerOne);
            _view.ShowCards(cardsinfo);
        }
        else if (menuShow == CardSet.OpponentsRingArea)
        {
            List<string> cardsinfo = GetInfoCardsInRingArea(playerTwo);
            _view.ShowCards(cardsinfo);
        }
        else if (menuShow == CardSet.OpponentsRingsidePile)
        {
            List<string> cardsinfo = GetInfoCardsInRingSide(playerTwo);
            _view.ShowCards(cardsinfo);
        }
    }

    private string PlayCard(List<Player> players,  Card cartaEscogida, List<(string card, int valor)> cartaEscogidaPosition)
    {
        Player playerOne = players[0];
        Player playerTwo = players[1];
        
        Card cardPlayed = cartaEscogida;
        string superstarName = playerOne.GetName();
        string tipoCard = cartaEscogida.Types[0].ToUpper(); // estamos asumiendo que las cartas no son hibridas
        
        ViewablePlayInfo playInfo = new ViewablePlayInfo(InfoCardInViewableClass(cartaEscogida), tipoCard);
        string cardToPlay = Formatter.PlayToString(playInfo);
        
        // Se despliega info: Se intenta jugar la sgte carta:
        _view.SayThatPlayerIsTryingToPlayThisCard(superstarName, cardToPlay);

        // Removemos la carta del hand segun su posicion en el hand
        
        playerOne.RemoveCardOfHand(cartaEscogidaPosition[0].valor);

        // Agregamos la carta a su ring area
        playerOne.AddCardToRingArea(cardPlayed);

        // Obtenemos el daño a aplicar // ojo si oponente es "MANKIND"
        string damageDone = cardPlayed.Damage;
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
        List<Card> cardsArsenal = playerTwo.GetCardsArsenal();
        int totalCards = cardsArsenal.Count;
        int startIndex = Math.Max(0, totalCards - damageNumber);

        for (int i = totalCards - 1; i >= startIndex; i--)
        {
            Card card = cardsArsenal[i];

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

        // Se despliega info: La carta fue jugada exitosamente
        _view.SayThatPlayerSuccessfullyPlayedACard();

        // Se menciona al oponente y se dice que recibe daño 
        _view.SayThatOpponentWillTakeSomeDamage(playerTwo.GetName(), damageNumber);

        // Mostramos en pantalla las cartas del oponente que fueron pasadas de su arsenal a su ringside
        int currentDamage = 1;
        foreach (var card in cardsinfo)
        {
            var carta = new List<string>();
            carta.Add(card);
            _view.ShowCardOverturnByTakingDamage(card, currentDamage, damageNumber);
            currentDamage++;
        }
        
        Console.WriteLine("LLEGO ACA");

        // Removemos las cartas del arsenal del oponente surante el turno pierde 
        if (playerTwo.NumberOfCardInArsenal() < damageNumber)
        {
            Console.WriteLine("Llego aca");
            _view.CongratulateWinner(playerOne.GetName());
            return "Gana Player One";
        }
        if (playerTwo.NumberOfCardInArsenal() >= damageNumber)
        {
            // Solo removemos las cartas del oponente de su arsenal
            playerTwo.RemoveManyCardsArsenal(damageNumber);
            playerOne.AddFortitude(plusFotitude);

            // Revisamos si el jugador 1 se quedo sin cartas en arsenal, en ese caso pierde
            // if (playerOne.NumberOfCardInArsenal() == 0)
            // {
            //     // Jugador pierde la partida, Oponente gana
            //     Console.WriteLine("Llego aca 2");
            //     _view.CongratulateWinner(playerTwo.GetName());
            //     return "Pierde Player One";
            // }
            
            // if (playerTwo.NumberOfCardInArsenal() == 0)
            // {
            //     // Jugador pierde la partida, Oponente gana
            //     _view.CongratulateWinner(playerOne.GetName());
            //     return "Pierde Player One";
            // }
        }

        return null;
    }
    
    // Funciones que ejecutan las habilidades de los jugadores
    private void ExecuteIfPlayTheUnderTaker(Player player)
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
    private void ExecuteIfPlayChrisJericho(Player playerOne, Player playerTwo)
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
    private void ExecuteIfPlaySteveColdSteveAustin(Player playerOne)
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
            foreach (Card card in playerOne.GetCardsHand())
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
    private void ExecuteIfPlayTheRock(Player player)
    {
        if (player.GetName() == "THE ROCK")
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
    private void ExecuteIfPlayMankind(Player player)
    {
        if (player.GetName() == "MANKIND")
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
        else
        {
            player.takeOneCard();
        }
    }
    private void ExecuteIfPlayKane(Player playerOne, Player playerTwo)
    {
        if (playerOne.GetName() == "KANE")
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
                    List<Card> cardsArsenal = playerTwo.GetCardsArsenal();
                    int totalCards = cardsArsenal.Count;
                    int startIndex = Math.Max(0, totalCards - damageNumber);

                    for (int i = totalCards - 1; i >= startIndex; i--)
                    {
                        Card card = cardsArsenal[i];

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

    
    // Funciones que nos permiten obtener información
    private List<string> GetInfoCardsInHand(Player player)
    {
        List<string> cardsinfo = new List<string> { };
        foreach (Card card in player.GetCardsHand())
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
    private List<string> GetInfoCardsInRingArea(Player player)
    {
        List<string> cardsinfo = new List<string> { };
        foreach (Card card in player.GetCardsRingArea())
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
    private List<string> GetInfoCardsInRingSide(Player player)
    {
        List<string> cardsinfo = new List<string> { };
                        
        foreach (Card card in player.GetCardsRingSide())
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
    
    private string InfoCardInString(Card card)
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
    private ViewableCardInfo InfoCardInViewableClass(Card card)
    {
        string titulo = card.Title;
        List<string> cardTypes = card.Types;
        List<string> subtypes = card.Subtypes;
        string fortitude = card.Fortitude;
        string damage = card.Damage;
        string stunValue = card.StunValue;
        string cardEffect = card.CardEffect;
        
        ViewableCardInfo cardInfo = new ViewableCardInfo(titulo, fortitude, damage, stunValue, cardTypes, subtypes, cardEffect);
        return cardInfo;
    }
    
    private void SaveCardsInArsenal(Player player,List<string> listCardsPlayer, List<Card> cards)
    {
        foreach (var nameCard in listCardsPlayer)
        {
            foreach (var card in cards)
            {
                if (nameCard == card.Title)
                {
                    player.AddCardToArsenal(card);
                }
            }
        }
    }
    
    
}
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
    
    private List<SuperStar>  GenerateSuperStarsInstances()
    {
        // almacenamos las cartas de SuperStar en una lista de instancias de la clase SuperStar
        string rutaSuperStar = Path.Combine("data", "superstar.json");
        string jsonSuperStar = File.ReadAllText(rutaSuperStar);
        var superStars = JsonSerializer.Deserialize<List<SuperStar>>(jsonSuperStar);
        return superStars;
    }
    private List<ICard> GenerateCardsInstances()
    {
        string cardsPath = Path.Combine("data", "cards.json");
        string jsonCards = File.ReadAllText(cardsPath);
        var cardsData = JsonSerializer.Deserialize<List<CardData>>(jsonCards);

        List<ICard> cardInstances = new List<ICard>();
    
        List<string> cardsWithoutEffects = new List<string>()
            { "Punch",
                "Head Butt",
                "Roundhouse Punch",
                "Haymaker",
                "Back Body Drop",
                "Big Boot",
                "Shoulder Block",
                "Kick",
                "Cross Body Block",
                "Ensugiri",
                "Running Elbow Smash",
                "Drop Kick",
                "Discus Punch",
                "Superkick",
                "Spinning Heel Kick",
                "Spear",
                "Clothesline",
                "Chair Shot",
                "Hurricanrana",
                "Hip Toss",
                "Arm Drag",
                "Russian Leg Sweep",
                "Snap Mare",
                "Gut Buster",
                "Body Slam",
                "Back Breaker",
                "Double Leg Takedown",
                "Fireman's Carry",
                "Headlock Takedown",
                "Belly to Belly Suplex",
                "Atomic Facebuster",
                "Atomic Drop",
                "Inverse Atomic Drop",
                "Vertical Suplex",
                "Belly to Back Suplex",
                "Pump Handle Slam",
                "Reverse DDT",
                "Samoan Drop",
                "Sit Out Powerbomb",
                "Bulldog",
                "Fisherman's Suplex",
                "DDT",
                "Power Slam",
                "Powerbomb",
                "Press Slam",
                "Wrist Lock",
                "Arm Bar",
                "Chin Lock",
                "Bear Hug",
                "Full Nelson",
                "Choke Hold",
                "Step Over Toe Hold",
                "Ankle Lock",
                "Standing Side Headlock",
                "Cobra Clutch",
                "Bow & Arrow",
                "Chicken Wing",
                "Sleeper",
                "Camel Clutch",
                "Boston Crab",
                "Guillotine Stretch",
                "Abdominal Stretch",
                "Torture Rack",
                "Figure Four Leg Lock",
                "Step Aside",
                "Escape Move",
                "Break the Hold",
                "Rolling Takedown",
                "Knee to the Gut",
                "Elbow to the Face",
                "Clean Break",
                "Manager Interferes",
                "Disqualification!",
                "No Chance in Hell",
                "Hmmm",
                "Don't Think Too Hard",
                "Whaddya Got?",
                "Not Yet",
                "Jockeying for Position",
                "Irish Whip",
                "Flash in the Pan",
                "View of Villainy",
                "Shake It Off",
                "Offer Handshake",
                "Roll Out of the Ring",
                "Distract the Ref",
                "Recovery",
                "Spit At Opponent",
                "Get Crowd Support",
                "Comeback!",
                "Ego Boost",
                "Deluding Yourself",
                "Stagger",
                "Diversion",
                "Marking Out",
                "Puppies! Puppies!",
                "Shane O'Mac",
                "Maintain Hold",
                "Pat & Gerry",
                "Austin Elbow Smash",
                "Lou Thesz Press",
                "Double Digits",
                "Stone Cold Stunner",
                "Open Up a Can of Whoop-A%$",
                "Undertaker's Chokeslam",
                "Undertaker's Flying Clothesline",
                "Undertaker Sits Up!",
                "Undertaker's Tombstone Piledriver",
                "Power of Darkness",
                "Have a Nice Day!",
                "Double Arm DDT",
                "Tree of Woe",
                "Mandible Claw",
                "Mr. Socko",
                "Leaping Knee to the Face",
                "Facebuster",
                "I Am the Game",
                "Pedigree",
                "Chyna Interferes",
                "Smackdown Hotel",
                "Take That Move, Shine It Up Real Nice, Turn That Sumb*tch Sideways, and Stick It Straight Up Your Roody Poo Candy A%$!",
                "Rock Bottom",
                "The People's Eyebrow",
                "The People's Elbow",
                "Kane's Chokeslam",
                "Kane's Flying Clothesline",
                "Kane's Return!",
                "Kane's Tombstone Piledriver",
                "Hellfire & Brimstone",
                "Lionsault",
                "Y2J",
                "Don't You Never... EVER!",
                "Walls of Jericho",
                "Ayatollah of Rock 'n' Roll-a",
            };
        List<string> cardsThatForceYouToDiscardACard = new List<string>()
            {"Chop", "Arm Bar Takedown", "Collar & Elbow Lockup"};
        List<string> cardsWithOtherEffects = new List<string>()
            {
            };
    
        foreach (var cardData in cardsData)
        {
            ICard cardInstance;

            if (cardsWithoutEffects.Contains(cardData.Title))
            {
                cardInstance = new CardsWithoutEffects
                {
                    Title = cardData.Title,
                    Types = cardData.Types,
                    Subtypes = cardData.Subtypes,
                    Fortitude = cardData.Fortitude,
                    Damage = cardData.Damage,
                    StunValue = cardData.StunValue,
                    CardEffect = cardData.CardEffect,
                };
                //cardInstances.Add(cardInstance);
            }
            else if (cardsThatForceYouToDiscardACard.Contains(cardData.Title))
            {
                cardInstance = new CardsThatForceYouToDiscardACard
                {
                    Title = cardData.Title,
                    Types = cardData.Types,
                    Subtypes = cardData.Subtypes,
                    Fortitude = cardData.Fortitude,
                    Damage = cardData.Damage,
                    StunValue = cardData.StunValue,
                    CardEffect = cardData.CardEffect,
                };
                //cardInstances.Add(cardInstance);
            }
            else
            {
                cardInstance = new CardsWithOtherEffects
                {
                    Title = cardData.Title,
                    Types = cardData.Types,
                    Subtypes = cardData.Subtypes,
                    Fortitude = cardData.Fortitude,
                    Damage = cardData.Damage,
                    StunValue = cardData.StunValue,
                    CardEffect = cardData.CardEffect,
                };
                //cardInstances.Add(cardInstance);
            }
        
            cardInstances.Add(cardInstance);
        }

        return cardInstances;
    }
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
                else if (playerOne.NumberOfCardInArsenal() == 0)
                {
                    _view.CongratulateWinner(playerTwo.GetName());
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
                if (playerOne.GetName() == "THE UNDERTAKER")
                {
                    int numCardsInHand = playerOne.NumberOfCardsInHand();
                    if (numCardsInHand >= 2)
                    {
                        return _view.AskUserWhatToDoWhenUsingHisAbilityIsPossible();
                    }
                    else
                    {
                        return _view.AskUserWhatToDoWhenHeCannotUseHisAbility();
                    }
                }
                
                else if (playerOne.GetName() == "CHRIS JERICHO")
                {
                    int numCardsInHand = playerOne.NumberOfCardsInHand();
                    if (numCardsInHand >= 1)
                    {
                        return _view.AskUserWhatToDoWhenUsingHisAbilityIsPossible();
                    }
                    else
                    {
                        return _view.AskUserWhatToDoWhenHeCannotUseHisAbility();
                    }
                }
                
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
        List<List<(string, int, int, int, string)>> cartasJugablesPosition = new List<List<(string, int, int, int, string)>>();
        List<ICard> cartasJugablesInstancia = new List<ICard>();
        List<string> cartasJugables = new List<string>();

        // Almacenaremos la posición en el hand de las cartas
        int positionInHand = 0;
        int positionInHandJugable = 0;
        int positionInCartasJugablesList = 0;

        foreach (ICard card in playerOne.GetCardsHand())
        {
            if (int.Parse(card.Fortitude) <= playerOne.GetFortitude())
            {
                if (!card.Types.Contains("Reversal"))
                {
                        string tyOfCardUpper = card.Types[0].ToUpper();
                        ViewablePlayInfo playInfo = new ViewablePlayInfo(InfoCardInViewableClass(card), tyOfCardUpper);
                        string cardToPlay = Formatter.PlayToString(playInfo);
                        List<(string, int, int, int,string)> cardToPlayInfo = new List<(string, int, int, int, string)>();
                        cardToPlayInfo.Add((cardToPlay, positionInHand, positionInHandJugable, positionInCartasJugablesList, tyOfCardUpper));
                        
                        cartasJugablesInstancia.Add(card);
                        cartasJugables.Add(cardToPlay);
                        cartasJugablesPosition.Add(cardToPlayInfo);

                        positionInCartasJugablesList++;
                        
                        
                        int numberOfTypes = card.Types.Count;

                        if (numberOfTypes > 1)
                        {
                            string tyOfCardUpper2 = card.Types[1].ToUpper();
                            ViewablePlayInfo playInfo2 = new ViewablePlayInfo(InfoCardInViewableClass(card), tyOfCardUpper2);
                            string cardToPlay2 = Formatter.PlayToString(playInfo2);
                            List<(string, int, int, int, string)> cardToPlayInfo2 = new List<(string, int, int, int, string)>();
                            cardToPlayInfo2.Add((cardToPlay2, positionInHand, positionInHandJugable, positionInCartasJugablesList,tyOfCardUpper2));
                            
                            cartasJugables.Add(cardToPlay2);
                            cartasJugablesPosition.Add(cardToPlayInfo2);
                            
                            positionInCartasJugablesList++;
                        }
                        
                        positionInHandJugable++;
                }
            }

            positionInHand++;
        }

        Console.WriteLine("Todas las cartas en el HAND");

        int positionhand = 0;
        foreach (var carta in playerOne.GetCardsHand())
        {
            Console.WriteLine(carta.Title);
            Console.WriteLine(positionhand);
            positionhand++;
        }
        
        Console.WriteLine("");
        Console.WriteLine("Cartas jugables en el HAND");

        foreach (var carta in cartasJugablesInstancia)
        {
            Console.WriteLine(carta.Title);
        }
        
        foreach (var lista in cartasJugablesPosition)
        {
            foreach (var tupla in lista)
            {
                string formato = string.Format("({0}, {1}, {2}, {3})", tupla.Item2, tupla.Item3, tupla.Item4, tupla.Item5);
                Console.WriteLine(formato);
            }
        }
        
        // foreach (var carta in cartasJugables)
        // {
        //     Console.WriteLine(carta);
        // }
        
        
        // Obtenemos un integer con la opción escogida
        int actionPlay = _view.AskUserToSelectAPlay(cartasJugables);

        // El jugador decide jugar una carta
        if (actionPlay != -1)
        {
            
            Console.WriteLine("Action play");
            Console.WriteLine(actionPlay);
            
            List<(string card, int positionInHand, int positionInHandJugable, int positionInCartasJugablesList,string type)> cartaElegidaPositionInHand = cartasJugablesPosition[actionPlay];
            int posicion = cartaElegidaPositionInHand[0].positionInCartasJugablesList;
            
            Console.WriteLine("Posición en lista de cartas jugables");
            Console.WriteLine(posicion);
            
            int posicionhand = cartaElegidaPositionInHand[0].positionInHand;
            Console.WriteLine("Posición en Hand");
            Console.WriteLine(posicionhand);
            
            int posicionhandJugable = cartaElegidaPositionInHand[0].positionInHandJugable;
            Console.WriteLine("Posición en Hand Jugable");
            Console.WriteLine(posicionhandJugable);
            
            Console.WriteLine("Cartas jugables en el HAND");

            foreach (var carta in cartasJugablesInstancia)
            {
                Console.WriteLine(carta.Title);
            }
            
            ICard cartaElegida = cartasJugablesInstancia[posicionhandJugable];
            
            Console.WriteLine("carta elegida ");
            Console.WriteLine(cartaElegida.Title);
            
            List<Player> players = new List<Player>() { playerOne, playerTwo };
            string play = PlayCard(players, cartaElegida, cartaElegidaPositionInHand);
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
    
    private string PlayCard(List<Player> players, ICard cardIntance, List<(string card, int positionInHand, int positionInHandJugable, int positionInCartasJugablesList, string type)> cartaEscogidaPositionInHand)
    {
        Player playerOne = players[0];
        Player playerTwo = players[1];
        
        string superstarName = playerOne.GetName();

        ICard cardPlayed = cardIntance;
        string typeCard = cartaEscogidaPositionInHand[0].type;
        
        ViewablePlayInfo playInfo = new ViewablePlayInfo(InfoCardInViewableClass(cardPlayed), typeCard);
        string cardToPlay = Formatter.PlayToString(playInfo);
        _view.SayThatPlayerIsTryingToPlayThisCard(superstarName, cardToPlay);
        
        string value;
        
        if (typeCard == "MANEUVER")
        {
            value = cardPlayed.NoEffect(playerOne, playerTwo, _view, cartaEscogidaPositionInHand);
        }
        else if (typeCard == "ACTION")
        {
            value = cardPlayed.Effect(playerOne,cartaEscogidaPositionInHand[0].positionInHand, _view );
        }
        else
        {
            value = cardPlayed.NoEffect(playerOne, playerTwo, _view, cartaEscogidaPositionInHand);
        }
        
        if (value == "Gana Player One")
        {
            return "Gana Player One";
        }
        else
        {
            return null;
        }
    }
    
    // Funciones que ejecutan las habilidades de los jugadores
    private void ExecuteIfPlayTheUnderTaker(Player player)
    {
        TheUnderTaker theUnderTaker = new TheUnderTaker();
        theUnderTaker.Hability(player, _view);
    }
    private void ExecuteIfPlayChrisJericho(Player playerOne, Player playerTwo)
    {
        ChrisJericho chrisJericho = new ChrisJericho();
        chrisJericho.Hability(playerOne, playerTwo, _view);
    }
    private void ExecuteIfPlaySteveColdSteveAustin(Player playerOne)
    {
        SteveColdSteveAustin steveColdSteveAustin = new SteveColdSteveAustin();
        steveColdSteveAustin.Hability(playerOne, _view);
    }
    private void ExecuteIfPlayTheRock(Player player)
    {
        if (player.GetName() == "THE ROCK")
        {
            TheRock theRock = new TheRock();
            theRock.Habilidad(player, _view);
        }
    }
    private void ExecuteIfPlayMankind(Player player)
    {
        if (player.GetName() == "MANKIND")
        {
            Mankind mankind = new Mankind();
            mankind.Hability(player, _view);
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
            Kane kane = new Kane();
            kane.Hability(playerOne, playerTwo, _view);
        }
    }

    // Funciones que nos permiten obtener información
    private List<string> GetInfoCardsInHand(Player player)
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
    private List<string> GetInfoCardsInRingArea(Player player)
    {
        List<string> cardsinfo = new List<string> { };
        foreach (ICard card in player.GetCardsRingArea())
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
    private ViewableCardInfo InfoCardInViewableClass(ICard card)
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
    private void SaveCardsInArsenal(Player player,List<string> listCardsPlayer, List<ICard> cards)
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
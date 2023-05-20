using RawDeal.data;

namespace RawDeal;
using System.Collections.Generic;

public class Validate
{
    public List<object> ValidateDeck(List<string> listCardsPlayer, List<ICard> cards, List<SuperStar> superStars)
    {
        List<object> miRetorno = SetupRetorno();
        
        bool correctNumerOfCards = ValidateDeckHaveCorrectNumberOfCards(listCardsPlayer);
        if (correctNumerOfCards == false)
        {
            return miRetorno;
        }
        
        List<object> deckHaveSuperStar = ValidateDeckHaveSuperStar(listCardsPlayer, superStars);
        if (deckHaveSuperStar != null)
        {

            miRetorno[1] = deckHaveSuperStar[0];
            miRetorno[2] = deckHaveSuperStar[1];
            miRetorno[3] = deckHaveSuperStar[2];
        }
        else
        {
            return miRetorno;
        }
        
        bool repeatCards = ValidateDeckDontHaveRepeatCards(listCardsPlayer, cards);
        if (repeatCards == true)
        {
            return miRetorno;
        }
        
        bool deckhaveHeelAndFace = DeckHaveHeelAndFace(listCardsPlayer, cards);
        if (deckhaveHeelAndFace == true)
        {
            return miRetorno;
        }

        string superStarName = miRetorno[2].ToString();
        bool deckHaveOnlyCardsWhitOneLogo = DeckHaveOnlyCardsWhitOneLogo(listCardsPlayer, cards, superStars, superStarName);
        if (deckHaveOnlyCardsWhitOneLogo == false)
        {
            return miRetorno;
        }
        
        // si logra llegar hasta aqui es return true
        miRetorno[0] = true;
        return miRetorno;
    }
    
    private List<object> SetupRetorno()
    {
        List<object> miRetorno = new List<object>();
        bool miBool = false;
        int superStarValue = -1;
        int handSize = -1;
        miRetorno.Add(miBool);     // bool que indica si el deck escogido es valido
        miRetorno.Add(superStarValue);  
        miRetorno.Add("star");  // este espacio esta para retornar el nombre de la super star
        miRetorno.Add(handSize);

        return miRetorno;
    }

    private bool ValidateDeckHaveCorrectNumberOfCards(List<string> listCardsPlayer)
    {
        int sizeDeck1 = 0;
        foreach (string elemento in listCardsPlayer)
        {
            sizeDeck1++;
        }

        if (sizeDeck1 != 61)
        {
            return false;
        }

        return true;
    }

    private List<object> ValidateDeckHaveSuperStar(List<string> listCardsPlayer, List<RawDeal.data.SuperStar> superStars)
    {
        // Verificamos que el deck tenga una superestrella
        bool deckExistSupeStar = false;
        string originalString = listCardsPlayer[0];
        string nameSuperStar = originalString.Substring(0, originalString.Length - 17);
        List<object> infoSuperStar = new List<object>();
        foreach ( var superStar in superStars)
        {
            if (superStar.Name == nameSuperStar)
            {
                deckExistSupeStar = true;
                infoSuperStar.Add(superStar.SuperstarValue);
                infoSuperStar.Add(superStar.Name);
                infoSuperStar.Add(superStar.HandSize);
            }
        }

        if (deckExistSupeStar == false)
        {
            return null;
        }
        
        return infoSuperStar;
    }

    private bool ValidateDeckDontHaveRepeatCards(List<string> listCardsPlayer, List<RawDeal.ICard> cards)
    {
        
        Dictionary<string, int> recuentos = new Dictionary<string, int>();
        foreach (string elemento in listCardsPlayer)
        {
            
            if (!recuentos.ContainsKey(elemento))
            {
                recuentos[elemento] = 1;
            }
            
            else
            {
                recuentos[elemento]++;
            }
        }
        foreach (KeyValuePair<string, int> recuento in recuentos)
        {
            if (recuento.Value > 1)
            {
                foreach (var card in cards)
                {
                    if (recuento.Key == card.Title)
                    {
                        bool existeSetup = false;
                        
                        foreach (var sub in card.Subtypes)
                        {
                            if (sub == "Unique")
                            {
                                return true;
                            }
                            if (sub == "SetUp")
                            {
                                existeSetup = true;
                            }
                        }

                        if (existeSetup == false)
                        {
                            if (recuento.Value > 3)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }

        return false;
    }

    private bool DeckHaveHeelAndFace(List<string> listCardsPlayer, List<RawDeal.ICard> cards)
    {
        bool haveCardHell = false;
        bool haveCardFace = false;

        foreach (string elemento in listCardsPlayer)
        {
            foreach (var card in cards)
            {
                if (elemento == card.Title)
                {
                    foreach (var sub in card.Subtypes)
                    {
                        if (sub == "Heel")
                        {
                            haveCardHell = true;
                        }
                        else if (sub == "Face")
                        {
                            haveCardFace = true;
                        }
                    }
                }
            }
        }

        if (haveCardFace == true && haveCardHell == true)
        {
            return true;
        }

        return false;
    }
    
    private bool DeckHaveOnlyCardsWhitOneLogo(List<string> listCardsPlayer, List<RawDeal.ICard> cards, List<RawDeal.data.SuperStar> superStars, string superStarName)
    {
        string logoValido = "";

        List<string> logosInvalidos = new List<string>();
        
        foreach (var star in superStars)
        {
            if (star.Name == superStarName)
            {
                logoValido = star.Logo;
            }
            else
            {
                logosInvalidos.Add(star.Logo);
            }
        }
        
        int lengthLogoValido = logoValido.Length;
        
        foreach (string elemento in listCardsPlayer)
        {
            foreach (var card in cards)
            {
                if (elemento == card.Title)
                {
                    foreach (var sub in card.Subtypes)
                    {
                        foreach (var logo in logosInvalidos)
                        {
                            int lengthLogo = logo.Length;

                            if (sub.Length >= lengthLogo)
                            {
                                string logoCarta = sub.Substring(0, lengthLogo);
                                if (logoCarta ==  logo)
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
            }
        }

        return true;
    }
    
}

using System.Runtime.Serialization;

namespace RawDeal;
using RawDeal.data;

public class Player
{
    private string name;
    private List<ICard> hand;
    private List<ICard> arsenal;
    private List<ICard> ringSide;
    private List<object> ringArea;
    private SuperStar superStar;
    private int fortitude;
    private int damage;
    
    public Player(string name)
    {
        this.name = name;
        this.hand = new List<ICard>();
        this.arsenal = new List<ICard>();
        this.ringSide = new List<ICard>();
        this.ringArea = new List<object>();
        this.fortitude = 0;
    }
    
    public string GetName()
    {
        return name;
    }
    
    public int GetFortitude()
    {
        return this.fortitude;
    }

    public void AddFortitude(int damage)
    {
        this.fortitude = this.fortitude + damage;
    }
    
    // Metodos que involucran cartas en Super Star
    public void GetSuperStar(SuperStar card)
    {
        this.superStar = card;
        this.name = card.Name;
    }

    public SuperStar CardSuperStar()
    {
        return this.superStar;
    }
    
    // Metodos que involucrna cartas en el Hand
    public int NumberOfCardsInHand()
    {
        int numberCardHand = this.hand.Count;
        return numberCardHand;
    }

    public List<ICard> GetCardsHand()
    {
        return this.hand;
    }
    
    public void RemoveCardOfHand(int position)
    {
        this.hand.RemoveAt(position);
    }
    
    // Metodos que involucran cartas en el Arsenal
    public List<ICard> GetCardsArsenal()
    {
        return this.arsenal;
    }
    
    public int NumberOfCardInArsenal()
    {
        int numberCardArsenal = this.arsenal.Count;
        return numberCardArsenal;
    }
    
    public void AddCardToArsenal(ICard card)
    {
        this.arsenal.Add(card);
    }
    
    public void RemoveManyCardsArsenal(int count)
    {

        if (NumberOfCardInArsenal() <= count)
        {
            this.arsenal.Clear();
        }
        else
        {
            int startIndex = this.arsenal.Count - count;
            this.arsenal.RemoveRange(startIndex, count);
        }
        
        
    }
    
    // Metodos que involucran cartas en el Ring Area
    public List<object> GetCardsRingArea()
    {
        return this.ringArea;
    }
    
    public void AddCardToRingArea(object card)
    {
        this.ringArea.Add(card);
    }
    
    // Metodos que involucran cartas en el Ring Side
    public List<ICard> GetCardsRingSide()
    {
        return this.ringSide;
    }

    public int NumberOfCardsInRingSide()
    {
        int num = this.ringSide.Count();
        return num;
    }
    
    public void AddCardToRingSide(ICard card)
    {
        this.ringSide.Add(card);
    }

    // Metodos que mueven cartas de una lista a otra
    public void moveOneCardRingSideToArsenal(int position)
    {
        ICard carta = this.ringSide[position];
        this.ringSide.RemoveAt(position);
        this.arsenal.Insert(0, carta);
    }
    
    public void moveOneCardRingSideToHand(int position)
    {
        ICard carta = this.ringSide[position];
        this.ringSide.RemoveAt(position);
        this.hand.Add(carta);
    }
    
    public void CreateInitialHand()
    {
        // Agregamos la carta al hand y la removemos de arsenal
        int handSize = this.superStar.HandSize;
        for (int i = 0; i < handSize; i++)
        {
            int lastCardIndex = this.arsenal.Count - 1;
            ICard carta = this.arsenal[lastCardIndex];
            AddCardToHand(carta);
            this.arsenal.RemoveAt(lastCardIndex);
        }
    }
    
    public void AddCardToHand(ICard card)
    {
        this.hand.Add(card);
    }
    
    public void takeOneCard()
    {
        // La carta se toma del final del arsenal
        int lastIndex = this.arsenal.Count - 1;
        ICard carta = this.arsenal[lastIndex];
        this.arsenal.RemoveAt(lastIndex);
        AddCardToHand(carta);
    }
    
    // Descartar: En este juego Descartar significa pasar cartas de la mano al ringside
    public void moveOneCardOfHandToRingSide(int position)
    {
        ICard carta = this.hand[position];
        this.hand.RemoveAt(position);
        this.ringSide.Add(carta);
    }

    public void MoveSelectedCardOfHandToArsenal(int position)
    {
        ICard carta = this.hand[position];
        this.hand.RemoveAt(position);
        this.arsenal.Insert(0, carta);
    }
}


using RawDealView.Formatters;
namespace RawDeal;

public class ViewableCardInfo : IViewableCardInfo
{
    public string Title { get; }
    public string Fortitude { get; }
    public string Damage { get; }
    public string StunValue { get; }
    public List<string> Types { get; }
    public List<string> Subtypes { get; }
    public string CardEffect { get; }

    public ViewableCardInfo(string title, string fortitude, string damage, string stunValue, List<string> types, List<string> subtypes, string cardEffect)
    {
        Title = title;
        Fortitude = fortitude;
        Damage = damage;
        StunValue = stunValue;
        Types = types;
        Subtypes = subtypes;
        CardEffect = cardEffect;
    }
}
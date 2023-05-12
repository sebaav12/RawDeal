using RawDealView.Formatters;
namespace RawDeal;

public class ViewablePlayInfo : IViewablePlayInfo
{
    public IViewableCardInfo CardInfo { get; private set; }
    public String PlayedAs { get; private set; }

    public ViewablePlayInfo(IViewableCardInfo cardInfo, string playedAs)
    {
        CardInfo = cardInfo;
        PlayedAs = playedAs;
    }
}
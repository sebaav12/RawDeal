namespace RawDeal.data;
using System.Text.Json.Serialization;

public class SuperStar
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<List<Root>>(myJsonResponse);
    public string Name { get; set; }
    public string Logo { get; set; }

    [JsonPropertyName("Hand Size")]
    public int HandSize { get; set; }

    [JsonPropertyName("Superstar Value")]
    public int SuperstarValue { get; set; }

    [JsonPropertyName("Superstar Ability")]
    public string SuperstarAbility { get; set; }
    
}
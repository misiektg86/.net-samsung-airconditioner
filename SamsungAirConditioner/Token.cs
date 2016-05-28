namespace SamsungAirConditioner
{
    public class Token
    {
        public Token(string tokenId)
        {
            Id = tokenId;
        }

        public string Id { get; private set; }
    }
}
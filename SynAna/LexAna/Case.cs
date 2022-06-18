namespace SynAna.LexAna
{
    internal struct Case
    {
        internal Case(char character, Token token, params Case[] cases)
        {
            Token = token;
            Character = character;
            Cases = cases;
        }

        internal readonly Token Token;

        internal readonly char Character;

        internal readonly Case[] Cases;
    }
}
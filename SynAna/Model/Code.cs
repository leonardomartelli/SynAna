namespace SynAna
{
    public struct Code
    {
        public Code()
            :this (string.Empty)
        {}

        public Code(string code)
        {
            _content = code;
        }

        string _content;

        public static implicit operator Code(string value) => new(value);

        public static implicit operator string(Code value) => value._content;

        public static Code operator +(Code one, Code two) =>
            string.IsNullOrEmpty(one)
            ? two
            : string.IsNullOrEmpty(two)
            ? one
            : new($"{one}\n{two}");

        public static Code operator +(Code one, string two) =>
            string.IsNullOrEmpty(one)
            ? two
            : string.IsNullOrEmpty(two)
            ? one
            : new($"{one}\n{two}");

        public static Code operator +(Code one, Production two) =>
            string.IsNullOrEmpty(one)
            ? two.Code
            : string.IsNullOrEmpty(two.Code)
            ? one
            : new($"{one}\n{two.Code}");

        public override string ToString() => _content;
    }
}
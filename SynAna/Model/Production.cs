namespace SynAna
{
    public struct Production
    {
        internal Production(Code code, string place, bool valid)
        {
            Code = code;
            Place = place;
            Valid = valid;
        }

        internal Production(string code, bool valid)
            :this(code, null, valid)
        {
        }

        internal Production(bool valid)
            : this(null, null, valid)
        {
        }

        public static implicit operator Production(bool value) => new Production(value);

        public static implicit operator bool(Production value) => value.Valid;

        public Code Code { get; set; }
        public string Place { get; set; }
        public bool Valid { get; set; }
    }
}
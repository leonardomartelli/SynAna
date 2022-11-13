using System.Collections.Generic;
using System.Linq;

namespace SynAna
{
    public struct Production
    {
        internal Production(Code code, string place, bool valid)
            : this(code, place, valid, new Dictionary<string, object>())
        { }

        internal Production(Code code, string place, bool valid, params (string, object)[] attributes)
            : this(code, place, valid, attributes.ToDictionary(k => k.Item1, v => v.Item2))
        { }

        internal Production(bool valid, params (string, object)[] attributes)
            : this(null, null, valid, attributes)
        { }

        internal Production(Code code, string place, bool valid, Dictionary<string, object> attributes)
        {
            Code = code;
            Place = place;
            Valid = valid;
            Attributes = attributes;
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

        internal T GetAttribute<T>(string key) =>
            Attributes.GetValueOrDefault(key) is object obj ? (T)obj : default;

        public Code Code { get; set; }
        public string Place { get; set; }
        public bool Valid { get; set; }
        public Dictionary <string, object> Attributes { get; set; }
    }
}
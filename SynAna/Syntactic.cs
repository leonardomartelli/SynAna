using System;
using System.Collections.Generic;
using SynAna.LexAna;

namespace SynAna
{
	public class Syntactic
	{
        private Token _currentToken;

        private readonly IEnumerator<Token> _lexicalResultEnumerator;

        public Syntactic(IEnumerable<Token> lexicalResult)
		{
            _lexicalResultEnumerator = lexicalResult.GetEnumerator();
        }

        private void ReadToken()
        {
   
            if(_lexicalResultEnumerator.MoveNext())
            {
                _currentToken = _lexicalResultEnumerator.Current;
            }
        }
	}
}


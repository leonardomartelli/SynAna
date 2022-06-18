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

        void ReadToken() =>
            _currentToken = _lexicalResultEnumerator.MoveNext()
                    ? _lexicalResultEnumerator.Current
                    : Token.EOF;

        // external_declaration -> function_defintion | declaration
        bool external_declaration()
        {
            if (function_definition())
                return true;

            else if (declaration())
                return true;

            return false;
        }

        bool function_definition()
        {
            if (declaration_specifiers())
            {
                ReadToken();

                if (declarator())
                {
                    ReadToken();

                    if (declaration_list())
                    {
                        ReadToken();

                        if (compound_statement())
                            return true;
                    }
                    else if (compound_statement())
                        return true;
                }

            }
            else if (declarator())
            {
                ReadToken();

                if (declaration_list())
                {
                    ReadToken();

                    if (compound_statement())
                        return true;
                }
                else if (compound_statement())
                    return true;
            }
            return false;
        }

        bool declaration_specifiers()
        {

        }

        bool declaration()
        {

        }
    }
}


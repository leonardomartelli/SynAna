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

        // external_declaration
        //  : function_defintion
        //  | declaration
        bool external_declaration()
        {
            if (function_definition())
                return true;

            else if (declaration())
                return true;

            return false;
        }

       // function_definition
       //   : declaration_specifiers declarator declaration_list compound_statement
	   //   | declaration_specifiers declarator compound_statement
	   //   | declarator declaration_list compound_statement
	   //   | declarator compound_statement
        bool function_definition()
        {
            //   declaration_specifiers declarator declaration_list compound_statement
            // | declaration_specifiers declarator compound_statement
            if (declaration_specifiers())
            {
                ReadToken();
                //   declarator declaration_list compound_statement
                // | declarator compound_statement
                if (declarator())
                {
                    ReadToken();

                    //declaration_list compound_statement
                    if (declaration_list())
                    {
                        ReadToken();

                        // compound_statement
                        if (compound_statement())
                            return true;
                    }
                    // compound_statement
                    else if (compound_statement())
                        return true;
                }

            }
            //   declarator declaration_list compound_statement
	        // | declarator compound_statement
            else if (declarator())
            {
                ReadToken();

                // declaration_list compound_statement
                if (declaration_list())
                {
                    ReadToken();
                    // compound_statement
                    if (compound_statement())
                        return true;
                }

                // compound_statement
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


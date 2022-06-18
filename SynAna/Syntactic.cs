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

        bool IsToken(Token token) =>
            _currentToken == token;

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

        // declaration_specifiers
        // : type_specifier
        // | type_specifier declaration_specifiers
        bool declaration_specifiers()
        {
            // type_specifier
            // | type_specifier declaration_specifiers
            if (type_specifier())
            {
                ReadToken();

                // declaration_specifiers
                if (declaration_specifiers())
                    return true;

                return true;
            }

            return false;
        }

        // type_specifier
        // : Void
        // | primitive_type_specifier
        // | unsigned_specifier
        // | struct_specifier
        bool type_specifier()
        {
            // Void
            if (IsToken(Token.Void))
                return true;
            // primitive_type_specifier
            else if (primitive_type_specifier())
                return true;
            // unsigned_specifier
            else if (unsigned_specifier())
                return true;
            // struct_specifier
            else if (struct_specifier())
                return true;

            return false;
        }


        // primitive_type_specifier
        // : Char
        // | Int
        // | Float
        // | Double
        // | long_int_specifier
        bool primitive_type_specifier()
        {
            // Char
            if (IsToken(Token.Char)
                // Int
                || IsToken(Token.Int)
                // Float
                || IsToken(Token.Float)
                // Double
                || IsToken(Token.Double)
                // long_int_specifier
                || long_int_specifier())
                return true;

            return false;
        }

        // long_int_specifier
        // : Long long_int_specifier
        // | Long Int
        bool long_int_specifier()
        {
            // Long long_int_specifier
            // | Long Int
            if (IsToken(Token.Long))
            {
                ReadToken();

                // long_int_specifier
                if (long_int_specifier())
                    return true;
                // Int
                else if (IsToken(Token.Int))
                    return true;
            }

            return false;
        }

        // unsigned_specifier
        // : Unsigned primitive_type_specifier
        bool unsigned_specifier()
        {
            // Unsigned primitive_type_specifier
            if (IsToken(Token.Unsigned))
            {
                ReadToken();

                // primitive_type_specifier
                if (primitive_type_specifier())
                    return true;
            }

            return false;
        }

        // struct_specifier
        // : Struct Identifier BraceOpen struct_declaration_list BraceClose
        // | Struct Identifier
        // | Struct BraceOpen struct_declaration_list BraceClose
        bool struct_specifier()
        {
            // Struct Identifier BraceOpen struct_declaration_list BraceClose
            // | Struct Identifier
            // | Struct BraceOpen struct_declaration_list BraceClose
            if (IsToken(Token.Struct))
            {
                ReadToken();

                // Identifier BraceOpen struct_declaration_list BraceClose
                // | Identifier
                if (IsToken(Token.Identifier))
                {
                    ReadToken();
                    // BraceOpen struct_declaration_list BraceClose
                    if (IsToken(Token.BraceOpen))
                    {
                        ReadToken();
                        // struct_declaration_list BraceClose
                        if (struct_declaration_list())
                        {
                            ReadToken();

                            // BraceClose
                            if (IsToken(Token.BraceClose))
                                return true;
                        }
                        return false;
                    }

                    return true;
                }
                else if (IsToken(Token.BraceOpen))
                {
                    ReadToken();
                    // struct_declaration_list BraceClose
                    if (struct_declaration_list())
                    {
                        ReadToken();

                        // BraceClose
                        if (IsToken(Token.BraceClose))
                            return true;
                    }
                    return false;
                }
            }

            return false;
        }

        // struct_declaration_list
        // : struct_declaration
        // | struct_declaration_list struct_declaration
        bool struct_declaration_list()
        {
            // struct_declaration
            if (struct_declaration())
                return true;

            // struct_declaration_list struct_declaration
            else if (struct_declaration_list())
            {
                ReadToken();

                // struct_declaration
                if (struct_declaration())
                    return true;
            }

            return false;
        }

        // struct_declaration
        // : specifier_list struct_declarator_list SemiCollon
        bool struct_declaration()
        {
            // pecifier_list struct_declarator_list SemiCollon
            if (specifier_list())
            {
                ReadToken();

                // struct_declarator_list SemiCollon
                if (struct_declaration_list())
                {
                    ReadToken();

                    // SemiCollon
                    if (IsToken(Token.SemiCollon))
                        return true;
                }

            }

            return false;
        }

        // specifier_list
        // : type_specifier specifier_list
        // | type_specifier
        bool specifier_list()
        {
            // type_specifier specifier_list
            // | type_specifier
            if (type_specifier())
            {
                ReadToken();

                // specifier_list
                if (specifier_list())
                    return true;

                return true;
            }

            // type_specifier
            return false;
        }


        // declarator
        // : pointer direct_declarator
        // | direct_declarator
        bool declarator()
        {
            // pointer direct_declarator
            if (pointer())
            {
                ReadToken();

                // direct_declarator
                if (direct_declarator())
                    return true;
            }
            // direct_declarator
            else if (direct_declarator())
                return true;

            return false;
        }

        // pointer
        // : Product
        // | Product pointer
        bool pointer()
        {
            // Product pointer
            // | Product
            if (IsToken(Token.Product))
            {
                // pointer
                if (pointer())
                    return true;

                return true;
            }

            return false;
        }

        // direct_declarator
        // : Identifier
        // | ParenthisOpen declarator ParenthisClose
        // | direct_declarator BracketOpen constant_expression BracketClose
        // | direct_declarator BracketOpen BracketClose
        // | direct_declarator ParenthisOpen parameter_type_list ParenthisClose
        // | direct_declarator ParenthisOpen identifier_list ParenthisClose
        // | direct_declarator ParenthisOpen ParenthisClose
        bool direct_declarator()
        {
            // Identifier
            if (IsToken(Token.Identifier))
                return true;

            // ParenthisOpen declarator ParenthisClose
            else if (IsToken(Token.ParenthesisOpen))
            {
                ReadToken();

                // declarator ParenthisClose
                if (declarator())
                {
                    ReadToken();

                    // ParenthisClose
                    if (IsToken(Token.ParenthesisClose))
                        return true;
                }
            }

            // direct_declarator BracketOpen constant_expression BracketClose
            // | direct_declarator BracketOpen BracketClose
            // | direct_declarator ParenthisOpen parameter_type_list ParenthisClose
            // | direct_declarator ParenthisOpen identifier_list ParenthisClose
            // | direct_declarator ParenthisOpen ParenthisClose
            if (direct_declarator())
            {
                ReadToken();

                // BracketOpen constant_expression BracketClose
                // | BracketOpen BracketClose
                if (IsToken(Token.BracketOpen))
                {
                    ReadToken();

                    // constant_expression BracketClose
                    if (constant_expression())
                    {
                        ReadToken();

                        // BracketClose
                        if (IsToken(Token.BracketClose))
                            return true;
                    }

                    // BracketClose
                    else if (IsToken(Token.BracketClose))
                        return true;
                }

                // ParenthisOpen parameter_type_list ParenthisClose
                // | ParenthisOpen identifier_list ParenthisClose
                // | ParenthisOpen ParenthisClose
                else if (IsToken(Token.ParenthesisOpen))
                {
                    ReadToken();

                    // parameter_type_list ParenthisClose
                    if (parameter_type_list())
                    {
                        ReadToken();

                        // ParenthisClose
                        if (IsToken(Token.ParenthesisClose))
                            return true;
                    }

                    // identifier_list ParenthisClose
                    else if (identifier_list())
                    {
                        ReadToken();

                        // ParenthisClose
                        if (IsToken(Token.ParenthesisClose))
                            return true;
                    }

                    // ParenthisClose
                    else if (IsToken(Token.ParenthesisClose))
                        return true;
                }
            }

            return false;
        }

        // constant_expression
        // : logical_or_expression
        bool constant_expression()
        {
            // logical_or_expression
            if (logical_or_expression())
                return true;

            return false;
        }

        // logical_or_expression
	    // : logical_and_expression
	    // | logical_or_expression LogicalOr logical_and_expression
        bool logical_or_expression()
        {
            // logical_and_expression
            if (logical_and_expression())
                return true;

            // logical_or_expression LogicalOr logical_and_expression
            else if(logical_or_expression())
            {
                ReadToken();

                // LogicalOr logical_and_expression
                if(IsToken(Token.LogicalOr))
                {
                    ReadToken();

                    // logical_and_expression
                    if (logical_and_expression())
                        return true;
                }
            }

            return false;
        }

        // logical_and_expression
	    // : inclusive_or_expression
	    // | logical_and_expression LogicalAnd inclusive_or_expression
        bool logical_and_expression()
        {
            // inclusive_or_expression
            if (inclusive_or_expression())
                return true;

            // logical_and_expression LogicalAnd inclusive_or_expression
            else if (logical_and_expression())
            {
                ReadToken();

                // LogicalAnd inclusive_or_expression
                if (IsToken(Token.LogicalAnd))
                {
                    ReadToken();

                    // inclusive_or_expression
                    if (inclusive_or_expression())
                        return true;
                }
            }

            return false;
        }

        // inclusive_or_expression
        // : exclusive_or_expression
        // | inclusive_or_expression Or exclusive_or_expression
        bool inclusive_or_expression()
        {
            // exclusive_or_expression
            if (exclusive_or_expression())
                return true;

            else if(inclusive_or_expression())
            {
                ReadToken();

                if(IsToken(Token.Or))
                {
                    ReadToken();

                    if (exclusive_or_expression())
                        return true;
                }
            }

            return false;
        }
    }
}


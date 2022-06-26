using System;
using System.Collections.Generic;
using System.Linq;
using SynAna.LexAna;

namespace SynAna
{
    public class Syntactic
    {
        private TokenResult _currentTokenResult;
        private int _position;

        private readonly IEnumerable<TokenResult> _lexicalResult;
        private readonly int _lexicalsCount;

        public Syntactic(IEnumerable<TokenResult> lexicalResult)
        {
            _lexicalResult = lexicalResult;
            _lexicalsCount = _lexicalResult.Count();

            _position = 0;
        }

        public void Analyze()
        {
            ReadToken();

            if (external_declaration())
                Console.WriteLine("Valid program");
            else
                Console.WriteLine("NOT Valid Program");
        }

        void ReadToken()
        {
            if (_position < _lexicalsCount)
                _currentTokenResult = _lexicalResult.ElementAt(_position++);
            else
                _currentTokenResult = default;
        }

        bool IsToken(Token token) =>
            _currentTokenResult?.Token == token;

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
                if (declarator())
                {
                    if (declaration_list())
                    {
                        if (compound_statement())
                            return true;
                    }
                    else if (compound_statement())
                        return true;
                }
            }
            else if (declarator())
            {
                if (declaration_list())
                {
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
            if (type_specifier())
            {
                if (declaration_specifiers())
                    return true;
                else
                    return true;
            }

            return false;
        }

        bool type_specifier()
        {
            if (IsToken(Token.Void))
            {
                ReadToken();
                return true;
            }
            else if (primitive_type_specifier())
                return true;
            else if (unsigned_specifier())
                return true;
            else if (struct_specifier())
                return true;

            return false;
        }

        bool primitive_type_specifier()
        {
            if (IsToken(Token.Char)
                || IsToken(Token.Int)
                || IsToken(Token.Float)
                || IsToken(Token.Double))
            {
                ReadToken();

                return true;
            }
            else if (long_int_specifier())
                return true;

            return false;
        }

        bool long_int_specifier()
        {
            if (IsToken(Token.Long))
            {
                ReadToken();

                if (long_int_specifier())
                    return true;

                else if (IsToken(Token.Int))
                {
                    ReadToken();
                    return true;
                }
            }

            return false;
        }

        bool unsigned_specifier()
        {
            if (IsToken(Token.Unsigned))
            {
                ReadToken();

                if (primitive_type_specifier())
                    return true;
            }

            return false;
        }

        bool struct_specifier()
        {
            if (IsToken(Token.Struct))
            {
                ReadToken();

                if (IsToken(Token.Identifier))
                {
                    ReadToken();

                    if (IsToken(Token.BraceOpen))
                    {
                        ReadToken();

                        if (struct_declaration_list())
                        {
                            if (IsToken(Token.BraceClose))
                            {
                                ReadToken();
                                return true;
                            }
                        }
                    }
                    else
                        return true;
                }
                else if (IsToken(Token.BraceOpen))
                {
                    ReadToken();

                    if (struct_declaration_list())
                        if (IsToken(Token.BraceClose))
                            return true;

                }
            }

            return false;
        }

        bool struct_declaration_list()
        {
            if (struct_declaration())
            {
                if (struct_declaration_list())
                    return true;

                return true;
            }

            return false;
        }

        bool struct_declaration()
        {
            if (specifier_list())
            {
                if (struct_declaration_list())
                {
                    if (IsToken(Token.SemiCollon))
                    {
                        ReadToken();
                        return true;
                    }
                }
            }

            return false;
        }

        bool specifier_list()
        {
            if (type_specifier())
            {
                if (specifier_list())
                    return true;

                return true;
            }

            return false;
        }

        bool declarator()
        {
            if (pointer())
            {
                if (direct_declarator())
                    return true;
            }

            else if (direct_declarator())
                return true;

            return false;
        }

        bool pointer()
        {
            if (IsToken(Token.Product))
            {
                ReadToken();

                if (pointer())
                    return true;

                return true;
            }

            return false;
        }

        bool direct_declarator()
        {
            if (IsToken(Token.Identifier))
            {
                ReadToken();

                if (direct_declarator_line())
                    return true;
            }

            else if (IsToken(Token.ParenthesisOpen))
            {
                ReadToken();

                if (declarator())
                {
                    if (IsToken(Token.ParenthesisClose))
                    {
                        ReadToken();

                        if (direct_declarator_line())
                            return true;
                    }
                }
            }

            return false;
        }

        bool direct_declarator_line()
        {
            if (IsToken(Token.BracketOpen))
            {
                ReadToken();

                if (logical_or_expression())
                {
                    if (IsToken(Token.BracketClose))
                    {
                        ReadToken();

                        if (direct_declarator_line())
                            return true;
                    }
                }

                else if (IsToken(Token.BracketClose))
                {
                    ReadToken();

                    if (direct_declarator_line())
                        return true;
                }
            }

            else if (IsToken(Token.ParenthesisOpen))
            {
                ReadToken();

                if (parameter_type_list())
                {
                    if (IsToken(Token.ParenthesisClose))
                    {
                        ReadToken();

                        if (direct_declarator_line())
                            return true;
                    }
                }

                else if (identifier_list())
                {
                    if (IsToken(Token.ParenthesisClose))
                    {
                        ReadToken();

                        if (direct_declarator_line())
                            return true;
                    }
                }
                else if (IsToken(Token.ParenthesisClose))
                {
                    ReadToken();

                    if (direct_declarator_line())
                        return true;
                }
            }

            return true;
        }

        bool logical_or_expression()
        {
            if (logical_and_expression())
                if (logical_or_expression_line())
                    return true;

            return false;
        }

        bool logical_or_expression_line()
        {
            if (IsToken(Token.LogicalOr))
            {
                ReadToken();

                if (logical_and_expression())
                    if (logical_or_expression_line())
                        return true;
            }

            return true;
        }

        bool logical_and_expression()
        {
            if (inclusive_or_expression())
                if (logical_and_expression_line())
                    return true;

            return false;
        }

        bool logical_and_expression_line()
        {
            if (IsToken(Token.LogicalAnd))
            {
                ReadToken();

                if (inclusive_or_expression())
                    if (logical_and_expression_line())
                        return true;
            }

            return true;
        }

        bool inclusive_or_expression()
        {
            if (exclusive_or_expression())
                if (inclusive_or_expression_line())
                    return true;

            return false;
        }

        bool inclusive_or_expression_line()
        {
            if (IsToken(Token.Or))
            {
                ReadToken();

                if (exclusive_or_expression())
                    if (inclusive_or_expression_line())
                        return true;
            }

            return true;
        }

        bool exclusive_or_expression()
        {
            if (and_expression())
                if (exclusive_or_expression_line())
                    return true;

            return false;
        }

        bool exclusive_or_expression_line()
        {
            if (IsToken(Token.Xor))
            {
                ReadToken();

                if (and_expression())
                    if (exclusive_or_expression_line())
                        return true;
            }

            return true;
        }

        bool and_expression()
        {
            if (equality_expression())
                if (and_expression_line())
                    return true;

            return false;
        }

        bool and_expression_line()
        {
            if (IsToken(Token.And))
            {
                ReadToken();

                if (and_expression_line())
                    return true;
            }


            return true;
        }

        bool equality_expression()
        {
            if (relational_expression())
                if (equality_expression_line())
                    return true;

            return false;
        }

        bool equality_expression_line()
        {
            if (IsToken(Token.Equals))
            {
                ReadToken();

                if (relational_expression())
                    if (equality_expression_line())
                        return true;
            }

            else if (IsToken(Token.NotEquals))
            {
                ReadToken();

                if (relational_expression())
                    if (equality_expression_line())
                        return true;
            }
            return true;
        }

        bool relational_expression()
        {
            if (shift_expression())
                if (relational_expression_line())
                    return true;

            return false;
        }

        bool relational_expression_line()
        {
            if (IsToken(Token.Less))
            {
                ReadToken();

                if (shift_expression())
                    if (relational_expression_line())
                        return true;
            }

            else if (IsToken(Token.Greater))
            {
                ReadToken();

                if (shift_expression())
                    if (relational_expression_line())
                        return true;
            }

            else if (IsToken(Token.LessOrEqual))
            {
                ReadToken();

                if (shift_expression())
                    if (relational_expression_line())
                        return true;
            }

            else if (IsToken(Token.GreaterOrEqual))
            {
                ReadToken();

                if (shift_expression())
                    if (relational_expression_line())
                        return true;
            }

            return true;
        }

        bool shift_expression()
        {
            if (additive_expression())
                if (shift_expression_line())
                    return true;

            return false;
        }

        bool shift_expression_line()
        {
            if (IsToken(Token.ShiftLeft))
            {
                ReadToken();

                if (additive_expression())
                    if (shift_expression_line())
                        return true;
            }

            else if (IsToken(Token.ShiftRight))
            {
                ReadToken();

                if (additive_expression())
                    if (shift_expression_line())
                        return true;
            }

            return true;
        }

        bool additive_expression()
        {
            if (multiplicative_expression())
                if (additive_expression_line())
                    return true;

            return false;
        }

        bool additive_expression_line()
        {
            if (IsToken(Token.Plus))
            {
                ReadToken();

                if (multiplicative_expression())
                    if (additive_expression_line())
                        return true;

            }
            else if (IsToken(Token.Minus))
            {
                ReadToken();

                if (multiplicative_expression())
                    if (additive_expression_line())
                        return true;
            }

            return true;
        }

        bool multiplicative_expression()
        {

            if (unary_expression())
                if (multiplicative_expression_line())
                    return true;


            return false;
        }

        bool multiplicative_expression_line()
        {
            if (IsToken(Token.Product))
            {
                ReadToken();

                if (unary_expression())
                    if (multiplicative_expression_line())
                        return true;
            }

            else if (IsToken(Token.Division))
            {
                ReadToken();

                if (unary_expression())
                    if (multiplicative_expression_line())
                        return true;
            }

            else if (IsToken(Token.Module))
            {
                ReadToken();

                if (unary_expression())
                    if (multiplicative_expression_line())
                        return true;
            }

            return true;
        }

        bool unary_expression()
        {
            if (postfix_expression())
                return true;

            if (IsToken(Token.Increment))
            {
                ReadToken();

                if (unary_expression())
                    return true;
            }

            if (IsToken(Token.Decrement))
            {
                ReadToken();

                if (unary_expression())
                    return true;
            }

            if (unary_operator())
                if (unary_expression())
                    return true;

            return false;
        }

        bool postfix_expression()
        {
            if (primary_expression())
                if (postfix_expression_line())
                    return true;

            return false;
        }

        bool postfix_expression_line()
        {
            if (IsToken(Token.ParenthesisOpen))
            {
                ReadToken();

                if (argument_expression_list())
                {
                    if (IsToken(Token.ParenthesisClose))
                    {
                        ReadToken();

                        if (postfix_expression_line())
                            return true;
                    }
                }

                else if (IsToken(Token.ParenthesisClose))
                {
                    ReadToken();

                    if (postfix_expression_line())
                        return true;
                }
            }

            else if (IsToken(Token.Dot))
            {
                ReadToken();

                if (IsToken(Token.Identifier))
                {
                    ReadToken();

                    if (postfix_expression_line())
                        return true;
                }
            }

            else if (IsToken(Token.StructAccessor))
            {
                ReadToken();

                if (IsToken(Token.Identifier))
                {
                    ReadToken();

                    if (postfix_expression_line())
                        return true;
                }
            }

            else if (IsToken(Token.Increment))
            {
                ReadToken();

                if (postfix_expression_line())
                    return true;
            }

            else if (IsToken(Token.Decrement))
            {
                ReadToken();

                if (postfix_expression_line())
                    return true;
            }

            return true;
        }

        bool primary_expression()
        {
            if (IsToken(Token.Identifier))
            {
                ReadToken();
                return true;
            }

            if (IsToken(Token.IntegerConstant))
            {
                ReadToken();
                return true;
            }

            if (IsToken(Token.FloatingPointConstant))
            {
                ReadToken();
                return true;
            }

            if (IsToken(Token.ParenthesisOpen))
            {
                ReadToken();

                if (expression())
                {
                    if (IsToken(Token.ParenthesisClose))
                    {
                        ReadToken();
                        return true;
                    }
                }
            }

            return false;
        }

        bool expression()
        {
            if (assignment_expression())
                if (expression_line())
                    return true;

            return false;
        }

        bool expression_line()
        {
            if (IsToken(Token.Comma))
            {
                ReadToken();

                if (assignment_expression())
                    if (expression_line())
                        return true;
            }

            return true;
        }

        bool assignment_expression()
        {
            if (logical_or_expression())
                return true;

            if (unary_expression())
            {
                if (assignment_operator())
                {
                    if (assignment_expression())
                        return true;
                }
            }

            return false;
        }

        bool assignment_operator()
        {
            if (IsToken(Token.Assign)
                || IsToken(Token.ProductAssign)
                || IsToken(Token.DivisionAssign)
                || IsToken(Token.ModuleAssign)
                || IsToken(Token.PlusAssign)
                || IsToken(Token.MinusAssign)
                || IsToken(Token.LeftAssign)
                || IsToken(Token.RightAssign))
            {
                ReadToken();
                return true;
            }

            return false;
        }

        bool argument_expression_list()
        {
            if (assignment_expression())
                if (argument_expression_list_line())
                    return true;

            return false;
        }

        bool argument_expression_list_line()
        {
            if (IsToken(Token.Comma))
            {
                ReadToken();

                if (assignment_expression())
                    if (argument_expression_list_line())
                        return true;
            }

            return true;
        }

        bool unary_operator()
        {
            if (IsToken(Token.And)
                || IsToken(Token.Product)
                || IsToken(Token.Plus)
                || IsToken(Token.Minus)
                || IsToken(Token.Negate)
                || IsToken(Token.LogicalNot))
            {
                ReadToken();
                return true;
            }

            return false;
        }

        bool parameter_type_list()
        {
            if (parameter_list())
            {
                if (IsToken(Token.Comma))
                {
                    ReadToken();

                    if (IsToken(Token.Ellipsis))
                    {
                        ReadToken();
                        return true;
                    }
                }
                return true;
            }

            return false;
        }

        bool parameter_list()
        {
            if (parameter_declaration())
                if (parameter_list_line())
                    return true;

            return false;
        }

        bool parameter_list_line()
        {
            if (IsToken(Token.Comma))
            {
                ReadToken();

                if (parameter_declaration())
                    if (parameter_list_line())
                        return true;
            }

            return true;
        }

        bool parameter_declaration()
        {
            if (declaration_specifiers())
            {
                if (declarator())
                    return true;

                else if (abstract_declarator())
                    return true;

                return true;
            }

            return false;
        }

        bool abstract_declarator()
        {
            if (pointer())
            {
                if (direct_abstract_declarator())
                    return true;

                return true;
            }

            else if (direct_abstract_declarator())
                return true;

            return false;
        }

        bool direct_abstract_declarator()
        {
            if (IsToken(Token.ParenthesisOpen))
            {
                ReadToken();

                if (parameter_type_list())
                {
                    if (IsToken(Token.ParenthesisClose))
                    {
                        ReadToken();

                        if (direct_abstract_declarator_line())
                            return true;
                    }
                }
                else if (abstract_declarator())
                {
                    if (IsToken(Token.ParenthesisClose))
                    {
                        ReadToken();

                        if (direct_abstract_declarator_line())
                            return true;
                    }
                }
                else if (IsToken(Token.ParenthesisClose))
                {
                    ReadToken();

                    if (direct_abstract_declarator_line())
                        return true;
                }
            }

            else if (IsToken(Token.BracketOpen))
            {
                ReadToken();

                if (logical_or_expression())
                {
                    if (IsToken(Token.BracketClose))
                    {
                        ReadToken();

                        if (direct_abstract_declarator_line())
                            return true;
                    }
                }
                else if (IsToken(Token.BracketClose))
                {
                    ReadToken();

                    if (direct_abstract_declarator_line())
                        return true;
                }

            }

            return false;
        }

        bool direct_abstract_declarator_line()
        {

            if (IsToken(Token.BracketOpen))
            {
                ReadToken();

                if (logical_or_expression())
                {
                    if (IsToken(Token.BracketClose))
                    {
                        ReadToken();
                        if (direct_abstract_declarator_line())
                            return true;
                    }
                }

                else if (IsToken(Token.BracketClose))
                {
                    ReadToken();
                    if (direct_abstract_declarator_line())
                        return true;
                }
            }

            else if (IsToken(Token.ParenthesisOpen))
            {
                ReadToken();

                if (parameter_type_list())
                {
                    if (IsToken(Token.ParenthesisClose))
                    {
                        ReadToken();
                        if (direct_abstract_declarator_line())
                            return true;
                    }
                }

                else if (IsToken(Token.ParenthesisClose))
                {
                    ReadToken();
                    if (direct_abstract_declarator_line())
                        return true;
                }
            }

            return true;
        }

        bool identifier_list()
        {
            if (IsToken(Token.Identifier))
                if (identifier_list_line())
                    return true;

            return false;
        }

        bool identifier_list_line()
        {
            if (IsToken(Token.Comma))
            {
                ReadToken();

                if (IsToken(Token.Identifier))
                {
                    ReadToken();

                    if (identifier_list_line())
                        return true;
                }
            }

            return true;
        }

        bool compound_statement()
        {
            if (IsToken(Token.BraceOpen))
            {
                ReadToken();

                if (IsToken(Token.BraceClose))
                {
                    ReadToken();
                    return true;
                }

                else if (declaration_list())
                {
                    if (IsToken(Token.BraceClose))
                    {
                        ReadToken();
                        return true;
                    }

                    else if (statement_list())
                    {
                        if (IsToken(Token.BraceClose))
                        {
                            ReadToken();
                            return true;
                        }
                    }
                }

                else if (statement_list())
                {
                    if (IsToken(Token.BraceClose))
                    {
                        ReadToken();
                        return true;
                    }
                }
            }

            return false;
        }

        bool declaration_list()
        {
            if (declaration())
            {
                if (declaration_list())
                    return true;

                return true;
            }
            return false;
        }

        bool declaration()
        {
            if (declaration_specifiers())
            {
                if (IsToken(Token.SemiCollon))
                {
                    ReadToken();
                    return true;
                }

                else if (init_declarator_list())
                {
                    if (IsToken(Token.SemiCollon))
                    {
                        ReadToken();
                        return true;
                    }
                }
            }

            return false;
        }

        bool init_declarator_list()
        {
            if (init_declarator())
                if (init_declarator_list_line())
                    return true;

            return false;
        }

        bool init_declarator_list_line()
        {
            if (IsToken(Token.Comma))
            {
                ReadToken();

                if (init_declarator())
                    if (init_declarator_list_line())
                        return true;
            }

            return true;
        }

        bool init_declarator()
        {
            if (declarator())
            {
                if (IsToken(Token.Assign))
                {
                    ReadToken();

                    if (initializer())
                        return true;
                }

                return true;
            }

            return false;
        }

        bool initializer()
        {
            if (assignment_expression())
                return true;

            else if (IsToken(Token.BraceOpen))
            {
                ReadToken();

                if (initializer_list())
                {
                    if (IsToken(Token.BraceClose))
                    {
                        ReadToken();
                        return true;
                    }

                    else if (IsToken(Token.Comma))
                    {
                        ReadToken();

                        if (IsToken(Token.BraceClose))
                        {
                            ReadToken();
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        bool initializer_list()
        {
            if (initializer())
                if (initializer_list_line())
                    return true;

            return false;
        }

        bool initializer_list_line()
        {
            if (IsToken(Token.Comma))
            {
                ReadToken();

                if (initializer())
                    if (initializer_list_line())
                        return true;
            }

            return true;
        }

        bool statement_list()
        {
            if (statement())
            {
                if (statement_list())
                    return true;

                return true;
            }

            return false;
        }

        bool statement()
        {
            if (labeled_statement()
               || compound_statement()
               || expression_statement()
               || selection_statement()
               || iteration_statement()
               || jump_statement())
                return true;

            return false;
        }

        bool labeled_statement()
        {
            if (IsToken(Token.Case))
            {
                ReadToken();

                if (logical_or_expression())
                {
                    if (IsToken(Token.Collon))
                    {
                        ReadToken();

                        if (statement())
                            return true;
                    }
                }
            }

            else if (IsToken(Token.Default))
            {
                ReadToken();

                if (IsToken(Token.Collon))
                {
                    ReadToken();

                    if (statement())
                        return true;
                }
            }

            return false;
        }

        bool expression_statement()
        {
            if (IsToken(Token.SemiCollon))
            {
                ReadToken();
                return true;
            }

            else if (expression())
            {
                if (IsToken(Token.SemiCollon))
                {
                    ReadToken();
                    return true;
                }
            }

            return false;
        }

        bool selection_statement()
        {
            if (IsToken(Token.If))
            {
                ReadToken();

                if (expression_statement_structure())
                {
                    if (IsToken(Token.Else))
                    {
                        ReadToken();

                        if (statement())
                            return true;
                    }
                    else
                        return true;
                }
            }

            return false;
        }

        bool expression_statement_structure()
        {
            if (IsToken(Token.ParenthesisOpen))
            {
                ReadToken();

                if (expression())
                {
                    if (IsToken(Token.ParenthesisClose))
                    {
                        ReadToken();

                        if (statement())
                            return true;
                    }
                }
            }

            return false;
        }

        bool iteration_statement()
        {
            if (IsToken(Token.While))
            {
                ReadToken();

                if (expression_statement_structure())
                    return true;
            }
            else if (IsToken(Token.Do))
            {
                ReadToken();

                if (statement())
                {
                    if (IsToken(Token.While))
                    {
                        ReadToken();

                        if (IsToken(Token.ParenthesisOpen))
                        {
                            ReadToken();

                            if (expression())
                            {
                                if (IsToken(Token.ParenthesisClose))
                                {
                                    ReadToken();

                                    if (IsToken(Token.SemiCollon))
                                    {
                                        ReadToken();
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (IsToken(Token.For))
            {
                ReadToken();

                if (IsToken(Token.ParenthesisOpen))
                {
                    ReadToken();
                    if (expression_statement())
                    {
                        if (expression_statement())
                        {
                            if (expression())
                            {
                                if (IsToken(Token.ParenthesisClose))
                                {
                                    ReadToken();

                                    // statement
                                    if (statement())
                                        return true;


                                }
                            }
                            else if (IsToken(Token.ParenthesisClose))
                            {
                                ReadToken();

                                // statement
                                if (statement())
                                    return true;


                            }
                        }
                    }
                }
            }

            return false;
        }

        bool jump_statement()
        {
            if (IsToken(Token.Continue))
            {
                ReadToken();

                if (IsToken(Token.SemiCollon))
                {
                    ReadToken();
                    return true;
                }
            }
            else if (IsToken(Token.Break))
            {
                ReadToken();

                if (IsToken(Token.SemiCollon))
                {
                    ReadToken();
                    return true;
                }


            }
            else if (IsToken(Token.Return))
            {
                ReadToken();
                if (IsToken(Token.SemiCollon))
                {
                    ReadToken();
                    return true;
                }
                else if (expression())
                {
                    // SemiCollon
                    if (IsToken(Token.SemiCollon))
                    {
                        ReadToken();
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
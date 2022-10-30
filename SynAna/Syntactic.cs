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

            _position = -1;
        }

        public void Analyze()
        {
            ReadToken();

            if (external_declaration_list())
                Console.WriteLine("\n\nValid program");
            else
                Console.WriteLine("NOT Valid Program");
        }

        void ReadToken() =>
            ReadToken(++_position);

        void ReadToken(int pos)
        {
            if (_position < _lexicalsCount)
                _currentTokenResult = _lexicalResult.ElementAt(pos);
            else
                _currentTokenResult = default;
        }

        Production CanBe(Func<Production> production)
        {
            var position = SetPosition();

            var canBe = production();

            RetrievePosition(position);

            return canBe;
        }

        Production IsToken(Token token) =>
            _currentTokenResult?.Token == token;

        int SetPosition() =>
            _position;

        void RetrievePosition(int position)
        {
            _position = position - 1;

            ReadToken();
        }

        Production external_declaration_list()
        {
            while (!IsToken(Token.EOF))
            {
                Production external = external_declaration();

                if (!external)
                    return false;

                Console.Write(external.Code);
            }

            return true;
        }

        Production external_declaration()
        {
            var pos = SetPosition();

            Production functionDef = function_definition();
            if (functionDef)
                return functionDef;

            else
            {
                RetrievePosition(pos);

                if (declaration())
                    return true;
            }

            return false;
        }

        Production function_definition()
        {
            var code = new Code();

            if (declaration_specifiers())
            {
                if (declarator())
                {
                    if (declaration_list())
                    {
                        var stat = compound_statement();
                        if (stat)
                            return stat;
                    }
                    else
                    {

                        var stat = compound_statement();

                        if (stat)
                            return stat;
                    }
                }
            }
            else if (declarator())
            {
                if (declaration_list())
                {
                    var stat = compound_statement();
                    if (stat)
                        return stat;
                }
                else
                {
                    var stat = compound_statement();
                    if (stat)
                        return stat;
                }
            }

            return false;
        }

        Production declaration_specifiers()
        {
            if (type_specifier())
            {
                if (declaration_specifiers())
                    return true;

                return true;
            }

            return false;
        }

        Production type_specifier()
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

        Production primitive_type_specifier()
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

        Production long_int_specifier()
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

        Production unsigned_specifier()
        {
            if (IsToken(Token.Unsigned))
            {
                ReadToken();

                if (primitive_type_specifier())
                    return true;
            }

            return false;
        }

        Production struct_specifier()
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

        Production struct_declaration_list()
        {
            if (struct_declaration())
            {
                if (CanBe(specifier_list))
                {
                    if (struct_declaration_list())
                        return true;
                }
                else
                    return true;
            }

            return false;
        }

        Production struct_declaration()
        {
            if (specifier_list())
            {
                if (struct_declarator_list())
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

        Production specifier_list()
        {
            if (type_specifier())
            {
                if (specifier_list())
                    return true;

                return true;
            }

            return false;
        }

        Production struct_declarator_list()
        {
            if (struct_declarator())
            {
                if (struct_declarator_list_line())
                    return true;
            }

            return false;
        }

        Production struct_declarator_list_line()
        {
            if (IsToken(Token.Comma))
            {
                ReadToken();

                if (struct_declarator())
                {
                    if (struct_declarator_list_line())
                        return true;
                }


            }

            return true;
        }

        Production struct_declarator()
        {
            if (declarator())
            {
                if (IsToken(Token.Collon))
                {
                    ReadToken();

                    if (logical_or_expression())
                        return true;
                    else
                    {

                        return false;
                    }
                }

                return true;
            }

            else if (IsToken(Token.Collon))
            {
                ReadToken();

                if (logical_or_expression())
                    return true;
            }

            return false;
        }

        Production declarator()
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

        Production pointer()
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

        Production direct_declarator()
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

        Production direct_declarator_line()
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

        Production logical_or_expression()
        {
            if (logical_and_expression())
                if (logical_or_expression_line())
                    return true;

            return false;
        }

        Production logical_or_expression_line()
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

        Production logical_and_expression()
        {
            if (inclusive_or_expression())
                if (logical_and_expression_line())
                    return true;

            return false;
        }

        Production logical_and_expression_line()
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

        Production inclusive_or_expression()
        {
            if (exclusive_or_expression())
                if (inclusive_or_expression_line())
                    return true;

            return false;
        }

        Production inclusive_or_expression_line()
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

        Production exclusive_or_expression()
        {
            if (and_expression())
                if (exclusive_or_expression_line())
                    return true;

            return false;
        }

        Production exclusive_or_expression_line()
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

        Production and_expression()
        {
            if (equality_expression())
                if (and_expression_line())
                    return true;

            return false;
        }

        Production and_expression_line()
        {
            if (IsToken(Token.And))
            {
                ReadToken();

                if (equality_expression())
                    if (and_expression_line())
                        return true;
            }

            return true;
        }

        Production equality_expression()
        {
            if (relational_expression())
                if (equality_expression_line())
                    return true;

            return false;
        }

        Production equality_expression_line()
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

        Production relational_expression()
        {
            if (shift_expression())
                if (relational_expression_line())
                    return true;

            return false;
        }

        Production relational_expression_line()
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

        Production shift_expression()
        {
            if (additive_expression())
                if (shift_expression_line())
                    return true;

            return false;
        }

        Production shift_expression_line()
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

        Production additive_expression()
        {
            if (multiplicative_expression())
                if (additive_expression_line())
                    return true;

            return false;
        }

        Production additive_expression_line()
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

        Production multiplicative_expression()
        {

            if (unary_expression())
                if (multiplicative_expression_line())
                    return true;


            return false;
        }

        Production multiplicative_expression_line()
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

        Production unary_expression()
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

        Production postfix_expression()
        {
            if (primary_expression())
                if (postfix_expression_line())
                    return true;

            return false;
        }

        Production postfix_expression_line()
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

        Production primary_expression()
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

            if (IsToken(Token.CharConstant))
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

        Production expression()
        {
            if (assignment_expression())
                if (expression_line())
                    return true;

            return false;
        }

        Production expression_line()
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

        Production assignment_expression()
        {
            var pos = SetPosition();

            if (unary_expression())
            {
                if (assignment_operator())
                {
                    var pos_assing = SetPosition();

                    if (assignment_expression())
                        return true;

                    else
                    {
                        RetrievePosition(pos_assing);

                        if (logical_or_expression())
                            return true;
                    }
                }
            }

            RetrievePosition(pos);

            if (logical_or_expression())
                return true;

            return false;
        }

        Production assignment_operator()
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

        Production argument_expression_list()
        {
            if (assignment_expression())
                if (argument_expression_list_line())
                    return true;

            return false;
        }

        Production argument_expression_list_line()
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

        Production unary_operator()
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

        Production parameter_type_list()
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
                else
                    return true;
            }

            return false;
        }

        Production parameter_list()
        {
            if (parameter_declaration())
                if (parameter_list_line())
                    return true;

            return false;
        }

        Production parameter_list_line()
        {
            if (IsToken(Token.Comma))
            {
                ReadToken();

                if (parameter_declaration())
                {
                    if (parameter_list_line())
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            }

            return true;
        }

        Production parameter_declaration()
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

        Production abstract_declarator()
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

        Production direct_abstract_declarator()
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

        Production direct_abstract_declarator_line()
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

        Production identifier_list()
        {
            if (IsToken(Token.Identifier))
            {
                ReadToken();

                if (identifier_list_line())
                    return true;


            }

            return false;
        }

        Production identifier_list_line()
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

        Production compound_statement(string escapeLabel = null)
        {
            if (IsToken(Token.BraceOpen))
            {
                ReadToken();

                if (IsToken(Token.BraceClose))
                {
                    ReadToken();

                    return true;
                }

                Production compoundBody = compound_body_list(escapeLabel);

                if (compoundBody)
                {
                    if (IsToken(Token.BraceClose))
                    {
                        ReadToken();

                        return compoundBody;
                    }
                }


            }

            return false;
        }

        Production compound_body_list(string escapeLabel)
        {
            var code = new Code();

            Production body = compound_body(escapeLabel);

            if (body)
            {
                code += body.Code;

                var list = compound_body_list(escapeLabel);

                if (list)
                    code += list.Code;

                if (IsToken(Token.BraceClose) || list)
                    return new(code, true);
            }

            return false;
        }

        Production compound_body(string escapeLabel)
        {
            var code = new Code();

            if (declaration_list())
                return true;

            var stat = statement_list(escapeLabel);

            if (stat)
            {
                return stat;
            }

            return false;
        }

        Production declaration_list()
        {
            if (declaration())
            {
                if (CanBe(declaration_specifiers))
                {
                    if (declaration_list())
                        return true;
                }
                else
                    return true;
            }

            return false;
        }

        Production declaration()
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

        Production init_declarator_list()
        {
            if (init_declarator())
                if (init_declarator_list_line())
                    return true;

            return false;
        }

        Production init_declarator_list_line()
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

        Production init_declarator()
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

        Production initializer()
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

        Production initializer_list()
        {
            if (initializer())
                if (initializer_list_line())
                    return true;

            return false;
        }

        Production initializer_list_line()
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

        Production statement_list(string escapeLabel)
        {
            Code code = new();

            var stat = statement(escapeLabel);

            if (stat)
            {
                code += stat.Code;

                var statList = statement_list(escapeLabel);

                if (statList)
                    code += statList.Code;

                return new(code, true);
            }

            return false;
        }

        Production statement(string escapeLabel = null)
        {
            var code = string.Empty;

            var pos = SetPosition();

            Production result = compound_statement(escapeLabel);

            if (result)
            {
                code += result.Code;

                return new(code, true);
            }

            RetrievePosition(pos);
            code = string.Empty;

            result = expression_statement();

            if (result)
            {
                code += result.Code;

                return new(code, true);
            }

            RetrievePosition(pos);
            code = string.Empty;

            result = selection_statement();

            if (result)
            {
                code += result.Code;

                return new(code, true);
            }

            RetrievePosition(pos);
            code = string.Empty;

            result = jump_statement(escapeLabel);

            if (result)
            {
                code += result.Code;

                return new(code, true);
            }

            return false;
        }


        Production expression_statement()
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

        Production selection_statement()
        {
            if (IsToken(Token.Switch))
            {
                var escapeLabel = CreateLabel("escape");

                Code code = new();

                ReadToken();

                Production exp = expression_statement_structure(escapeLabel);

                if (exp)
                {
                    code += exp.Code;
                    code += $"{escapeLabel}:";

                    return new(code, true);
                }
            }

            return false;
        }


        Dictionary<string, int> labels = new();

        private string CreateLabel(string v)
        {
            if (!labels.ContainsKey(v))
                labels.Add(v, 0);

            return $"{v}_{labels[v]++}";
        }

        static int tempCount = 0;

        private string CreateTemp()
        {
            return $"t{tempCount++}";
        }

        Production expression_statement_structure(string escapeLabel)
        {
            if (IsToken(Token.ParenthesisOpen))
            {
                ReadToken();

                var exp = expression();

                if (exp)
                {
                    if (IsToken(Token.ParenthesisClose))
                    {
                        ReadToken();

                        var stat = labeled_block(escapeLabel, exp.Place);

                        if (stat)
                            return stat;
                    }
                }

            }

            return false;
        }

        Production labeled_block(string escapeLabel, string valueToCompare)
        {
            if (IsToken(Token.BraceOpen))
            {
                ReadToken();

                var labeledList = labeled_statement_list(escapeLabel, valueToCompare);

                if (labeledList)
                {
                    if (IsToken(Token.BraceClose))
                    {
                        ReadToken();

                        return labeledList;
                    }
                }
            }
            return false;
        }

        Production labeled_statement_list(string escapeLabel, string valueToCompare)
        {
            Code code = new();

            var stat = labeled_statement(escapeLabel, valueToCompare);

            if (stat)
            {
                code += stat.Code;

                var statList = labeled_statement_list(escapeLabel, valueToCompare);

                if (statList)
                    code += statList.Code;

                return new(code, true);
            }

            return false;
        }

        Production labeled_statement(string escapeLabel, string valueToCompare)
        {
            Code code = new();

            if (IsToken(Token.Case))
            {
                var nextCaseLabel = CreateLabel("next");

                ReadToken();

                Production expression = logical_or_expression();

                var temp = CreateTemp();

                code += $"{temp} = {valueToCompare} == {expression.Place}";

                if (expression)
                {
                    code += $"goFalse {temp} {nextCaseLabel}";

                    if (IsToken(Token.Collon))
                    {
                        ReadToken();

                        var stat = statement(escapeLabel);

                        if (stat)
                        {
                            code += stat.Code;

                            code += $"{nextCaseLabel}:";

                            return new(code, true);
                        }
                    }
                }

            }

            else if (IsToken(Token.Default))
            {
                ReadToken();

                if (IsToken(Token.Collon))
                {
                    ReadToken();

                    var stat = statement(escapeLabel);

                    if (stat)
                    {
                        var defaultLabel = CreateLabel("default");

                        code += $"{defaultLabel}:";

                        code += stat.Code;

                        return new(code, true);
                    }
                }
            }

            return false;
        }

        Production jump_statement(string labelToReturn)
        {
            if (IsToken(Token.Break))
            {
                string code = null;

                ReadToken();

                if (IsToken(Token.SemiCollon))
                {
                    ReadToken();

                    code = $"go {labelToReturn};";

                    return new Production(code, true);
                }
            }

            return false;
        }
    }
}
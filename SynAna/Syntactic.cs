using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SynAna.LexAna;

namespace SynAna
{
    public class Syntactic
    {
        private TokenResult _currentTokenResult;
        private int _position;

        private readonly IEnumerable<TokenResult> _lexicalResult;
        private readonly int _lexicalsCount;

        private StringBuilder code;

        private IDictionary<string, (int line, string value)> _variables;
        private IDictionary<string, int> _labels;

        public Syntactic(IEnumerable<TokenResult> lexicalResult)
        {
            _lexicalResult = lexicalResult;
            _lexicalsCount = _lexicalResult.Count();

            _position = -1;

            _variables = new Dictionary<string, (int, string)>();
            _labels = new Dictionary<string, int>();

            code = new();
        }

        public void Analyze()
        {
            ReadToken();

            external_declaration_list();

            File.WriteAllText("result.txt", code.ToString());

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

        bool CanBe(Func<bool> production)
        {
            var position = SetPosition();

            var canBe = production();

            RetrievePosition(position);

            return canBe;
        }

        bool IsToken(Token token) =>
            _currentTokenResult?.Token == token;

        int SetPosition() =>
            _position;

        void RetrievePosition(int position)
        {
            _position = position - 1;

            ReadToken();
        }

        bool external_declaration_list()
        {
            while (!IsToken(Token.EOF))
                if (!external_declaration())
                    return false;

            return true;
        }

        bool external_declaration()
        {
            var pos = SetPosition();

            if (function_definition())
                return true;

            else
            {
                RetrievePosition(pos);

                if (declaration(code))
                    return true;
            }

            return false;
        }

        bool function_definition()
        {
            if (declaration_specifiers())
            {
                if (declarator(new()))
                {
                    if (declaration_list(code))
                    {
                        if (compound_statement(code))
                            return true;
                    }
                    else if (compound_statement(code))
                        return true;
                }
            }
            else if (declarator(code))
            {
                if (declaration_list(code))
                {
                    if (compound_statement(code))
                        return true;
                }
                else if (compound_statement(code))
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

        bool declarator(StringBuilder declaration)
        {

            if (pointer())
            {
                if (direct_declarator(declaration))
                    return true;
            }

            else if (direct_declarator(declaration))
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

        bool direct_declarator(StringBuilder declaration)
        {
            if (IsToken(Token.Identifier))
            {
                declaration.AppendLine($"valor-l {_currentTokenResult.Lexical}");

                ReadToken();

                if (direct_declarator_line())
                    return true;
            }

            else if (IsToken(Token.ParenthesisOpen))
            {
                ReadToken();

                if (declarator(declaration))
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

                if (logical_or_expression(new()))
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

        bool logical_or_expression(StringBuilder exp)
        {
            if (logical_and_expression(exp))
                if (logical_or_expression_line(exp))
                    return true;

            return false;
        }

        bool logical_or_expression_line(StringBuilder exp)
        {
            if (IsToken(Token.LogicalOr))
            {
                var @operator = _currentTokenResult.Lexical;
                ReadToken();

                if (logical_and_expression(exp))
                    if (logical_or_expression_line(exp))
                    {
                        exp.AppendLine(@operator);
                        return true;
                    }
            }

            return true;
        }

        bool logical_and_expression(StringBuilder exp)
        {
            if (inclusive_or_expression(exp))
                if (logical_and_expression_line(exp))
                    return true;

            return false;
        }

        bool logical_and_expression_line(StringBuilder exp)
        {
            if (IsToken(Token.LogicalAnd))
            {
                var @operator = _currentTokenResult.Lexical;
                ReadToken();

                if (inclusive_or_expression(exp))
                    if (logical_and_expression_line(exp))
                    {
                        exp.AppendLine(@operator);
                        return true;
                    }

            }

            return true;
        }

        bool inclusive_or_expression(StringBuilder exp)
        {
            if (exclusive_or_expression(exp))
                if (inclusive_or_expression_line(exp))
                    return true;

            return false;
        }

        bool inclusive_or_expression_line(StringBuilder exp)
        {
            if (IsToken(Token.Or))
            {
                var @operator = _currentTokenResult.Lexical;
                ReadToken();

                if (exclusive_or_expression(exp))
                    if (inclusive_or_expression_line(exp))
                    {
                        exp.AppendLine(@operator);
                        return true;
                    }

            }

            return true;
        }

        bool exclusive_or_expression(StringBuilder exp)
        {
            if (and_expression(exp))
                if (exclusive_or_expression_line(exp))
                    return true;

            return false;
        }

        bool exclusive_or_expression_line(StringBuilder exp)
        {
            if (IsToken(Token.Xor))
            {
                var @operator = _currentTokenResult.Lexical;
                ReadToken();

                if (and_expression(exp))
                    if (exclusive_or_expression_line(exp))
                    {
                        exp.AppendLine(@operator);
                        return true;
                    }

            }

            return true;
        }

        bool and_expression(StringBuilder exp)
        {
            if (equality_expression(exp))
                if (and_expression_line(exp))
                    return true;

            return false;
        }

        bool and_expression_line(StringBuilder exp)
        {
            if (IsToken(Token.And))
            {
                var @operator = _currentTokenResult.Lexical;
                ReadToken();

                if (equality_expression(exp))
                    if (and_expression_line(exp))
                    {
                        exp.AppendLine(@operator);
                        return true;
                    }
            }

            return true;
        }

        bool equality_expression(StringBuilder exp)
        {
            if (relational_expression(exp))
                if (equality_expression_line(exp))
                    return true;

            return false;
        }

        bool equality_expression_line(StringBuilder exp)
        {
            if (IsToken(Token.Equals))
            {
                var @operator = _currentTokenResult.Lexical;
                ReadToken();

                if (relational_expression(exp))
                    if (equality_expression_line(exp))
                    {
                        exp.AppendLine(@operator);
                        return true;
                    }

            }

            else if (IsToken(Token.NotEquals))
            {
                var @operator = _currentTokenResult.Lexical;
                ReadToken();

                if (relational_expression(exp))
                    if (equality_expression_line(exp))
                    {
                        exp.AppendLine(@operator);
                        return true;
                    }

            }
            return true;
        }

        bool relational_expression(StringBuilder exp)
        {
            if (shift_expression(exp))
                if (relational_expression_line(exp))
                    return true;

            return false;
        }

        bool relational_expression_line(StringBuilder exp)
        {
            if (IsToken(Token.Less))
            {
                var @operator = _currentTokenResult.Lexical;
                ReadToken();

                if (shift_expression(exp))
                    if (relational_expression_line(exp))
                    {
                        exp.AppendLine(@operator);
                        return true;
                    }

            }

            else if (IsToken(Token.Greater))
            {
                var @operator = _currentTokenResult.Lexical;
                ReadToken();

                if (shift_expression(exp))
                    if (relational_expression_line(exp))
                    {
                        exp.AppendLine(@operator);
                        return true;
                    }

            }

            else if (IsToken(Token.LessOrEqual))
            {
                var @operator = _currentTokenResult.Lexical;
                ReadToken();

                if (shift_expression(exp))
                    if (relational_expression_line(exp))
                    {
                        exp.AppendLine(@operator);
                        return true;
                    }

            }

            else if (IsToken(Token.GreaterOrEqual))
            {
                var @operator = _currentTokenResult.Lexical;
                ReadToken();

                if (shift_expression(exp))
                    if (relational_expression_line(exp))
                    {
                        exp.AppendLine(@operator);
                        return true;
                    }
            }

            return true;
        }

        bool shift_expression(StringBuilder exp)
        {
            if (additive_expression(exp))
                if (shift_expression_line(exp))
                    return true;

            return false;
        }

        bool shift_expression_line(StringBuilder exp)
        {
            if (IsToken(Token.ShiftLeft))
            {
                var @operator = _currentTokenResult.Lexical;
                ReadToken();

                if (additive_expression(exp))
                    if (shift_expression_line(exp))
                    {
                        exp.AppendLine(@operator);
                        return true;
                    }

            }

            else if (IsToken(Token.ShiftRight))
            {
                var @operator = _currentTokenResult.Lexical;
                ReadToken();

                if (additive_expression(exp))
                    if (shift_expression_line(exp))
                    {
                        exp.AppendLine(@operator);
                        return true;
                    }

            }

            return true;
        }

        bool additive_expression(StringBuilder exp)
        {
            if (multiplicative_expression(exp))
                if (additive_expression_line(exp))
                    return true;

            return false;
        }

        bool additive_expression_line(StringBuilder exp)
        {
            if (IsToken(Token.Plus))
            {
                var @operator = _currentTokenResult.Lexical;
                ReadToken();

                if (multiplicative_expression(exp))
                    if (additive_expression_line(exp))
                    {
                        exp.AppendLine(@operator);
                        return true;
                    }


            }
            else if (IsToken(Token.Minus))
            {
                var @operator = _currentTokenResult.Lexical;
                ReadToken();

                if (multiplicative_expression(exp))
                    if (additive_expression_line(exp))
                    {
                        exp.AppendLine(@operator);
                        return true;
                    }

            }

            return true;
        }

        bool multiplicative_expression(StringBuilder exp)
        {

            if (unary_expression(exp))
                if (multiplicative_expression_line(exp))
                    return true;


            return false;
        }

        bool multiplicative_expression_line(StringBuilder exp)
        {
            if (IsToken(Token.Product))
            {
                var @operator = _currentTokenResult.Lexical;
                ReadToken();

                if (unary_expression(exp))
                    if (multiplicative_expression_line(exp))
                    {
                        exp.AppendLine(@operator);
                        return true;
                    }

            }

            else if (IsToken(Token.Division))
            {
                var @operator = _currentTokenResult.Lexical;
                ReadToken();

                if (unary_expression(exp))
                    if (multiplicative_expression_line(exp))
                    {
                        exp.AppendLine(@operator);
                        return true;
                    }
            }

            else if (IsToken(Token.Module))
            {
                var @operator = _currentTokenResult.Lexical;
                ReadToken();

                if (unary_expression(exp))
                    if (multiplicative_expression_line(exp))
                    {
                        exp.AppendLine(@operator);
                        return true;
                    }

            }

            return true;
        }

        bool unary_expression(StringBuilder exp, bool isAssignment = false)
        {
            if (postfix_expression(exp, isAssignment))
                return true;

            if (IsToken(Token.Increment))
            {
                var @operator = _currentTokenResult.Lexical;
                ReadToken();

                if (unary_expression(exp))
                {
                    exp.AppendLine(@operator);
                    return true;
                }
            }

            if (IsToken(Token.Decrement))
            {
                var @operator = _currentTokenResult.Lexical;
                ReadToken();

                if (unary_expression(exp))
                {
                    exp.AppendLine(@operator);
                    return true;
                }
            }

            if (unary_operator())
                if (unary_expression(exp))
                    return true;

            return false;
        }

        bool postfix_expression(StringBuilder exp, bool isAssignment = false)
        {
            if (primary_expression(exp, isAssignment))
                if (postfix_expression_line(exp))
                    return true;

            return false;
        }

        bool postfix_expression_line(StringBuilder exp)
        {
            if (IsToken(Token.ParenthesisOpen))
            {
                ReadToken();

                if (assignment_expression(exp))
                {
                    if (IsToken(Token.ParenthesisClose))
                    {
                        ReadToken();

                        if (postfix_expression_line(exp))
                            return true;

                    }
                }

                else if (IsToken(Token.ParenthesisClose))
                {
                    ReadToken();

                    if (postfix_expression_line(exp))
                        return true;

                }

            }

            else if (IsToken(Token.Dot))
            {
                ReadToken();

                if (IsToken(Token.Identifier))
                {
                    var @operator = _currentTokenResult.Lexical;
                    ReadToken();

                    if (postfix_expression_line(exp))
                    {
                        exp.AppendLine(@operator);
                        return true;
                    }
                }

            }

            else if (IsToken(Token.StructAccessor))
            {
                ReadToken();

                if (IsToken(Token.Identifier))
                {
                    ReadToken();

                    if (postfix_expression_line(exp))
                        return true;

                }

            }

            else if (IsToken(Token.Increment))
            {
                ReadToken();


                if (postfix_expression_line(exp))
                {

                    var identifier = exp.ToString().Split(' ', StringSplitOptions.RemoveEmptyEntries)[^1];
                    exp.Clear();
                    exp.AppendLine($"valor-l {identifier}");
                    exp.AppendLine($"valor-r {identifier}");

                    exp.AppendLine("push 1");

                    exp.AppendLine("+");
                    exp.AppendLine("=");

                    return true;
                }
            }

            else if (IsToken(Token.Decrement))
            {
                ReadToken();


                if (postfix_expression_line(exp))
                {
                    var identifier = exp.ToString().Split(' ', StringSplitOptions.RemoveEmptyEntries)[^1];
                    exp.Clear();
                    exp.Append($"valor-l {identifier}");
                    exp.Append($"valor-r {identifier}");

                    exp.AppendLine("push 1");

                    exp.AppendLine("-");
                    exp.AppendLine("=");

                    return true;
                }
            }

            return true;
        }

        bool primary_expression(StringBuilder exp, bool isAssignment = false)
        {
            if (IsToken(Token.Identifier))
            {
                var preffix = isAssignment ? "valor-l" : "valor-r";

                exp.AppendLine($"{preffix} {_currentTokenResult.Lexical}");
                ReadToken();
                return true;
            }

            if (IsToken(Token.IntegerConstant))
            {
                exp.AppendLine("push " + _currentTokenResult.Lexical);
                ReadToken();
                return true;
            }

            if (IsToken(Token.FloatingPointConstant))
            {
                exp.AppendLine("push " + _currentTokenResult.Lexical);
                ReadToken();
                return true;
            }

            if (IsToken(Token.CharConstant))
            {
                exp.AppendLine("push " + _currentTokenResult.Lexical);
                ReadToken();
                return true;
            }

            if (IsToken(Token.ParenthesisOpen))
            {
                ReadToken();

                if (expression(exp))
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

        bool expression(StringBuilder exp)
        {
            if (assignment_expression(exp))
                if (expression_line(exp))
                    return true;

            return false;
        }

        bool expression_line(StringBuilder exp)
        {
            if (IsToken(Token.Comma))
            {
                ReadToken();

                var secondExp = new StringBuilder();
                if (assignment_expression(secondExp))
                    if (expression_line(secondExp))
                    {
                        exp.Append(secondExp);
                        return true;
                    }

                return false;
            }

            return true;
        }

        bool assignment_expression(StringBuilder exp)
        {
            var pos = SetPosition();

            var assingmentExp = new StringBuilder();

            if (unary_expression(assingmentExp, true))
            {
                var assignOperator = new StringBuilder();

                if (assignment_operator(assignOperator))
                {
                    var pos_assing = SetPosition();

                    var rightSideExp = new StringBuilder();

                    if (assignment_expression(rightSideExp))
                    {
                        var isNormalAssingment = assignOperator.Equals("=");

                        if (!isNormalAssingment)
                        {
                            var identifier = assingmentExp.ToString().Split(' ', StringSplitOptions.RemoveEmptyEntries)[^1];

                            assingmentExp.AppendLine($"valor-r {identifier}");
                        }

                        assingmentExp.AppendLine(rightSideExp.ToString());

                        if (!isNormalAssingment)
                            assingmentExp.AppendLine(assignOperator.ToString()[..^1].ToString());

                        assingmentExp.AppendLine("=");

                        exp.Append(assingmentExp);

                        return true;
                    }

                    else
                    {
                        RetrievePosition(pos_assing);

                        if (logical_or_expression(rightSideExp))
                        {
                            assingmentExp.AppendLine(rightSideExp.ToString());
                            assingmentExp.AppendLine(assignOperator.ToString());

                            exp.Append(assingmentExp);


                            return true;
                        }
                    }
                }
            }

            RetrievePosition(pos);

            if (logical_or_expression(exp))
                return true;

            return false;
        }

        bool assignment_operator(StringBuilder @operator)
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
                @operator.Append(_currentTokenResult.Lexical);

                ReadToken();
                return true;
            }

            return false;
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
                else
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

        bool parameter_declaration()
        {
            if (declaration_specifiers())
            {
                if (declarator(new()))
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
                if (direct_abstract_declarator(new()))
                    return true;

                return true;
            }

            else if (direct_abstract_declarator(new()))
                return true;

            return false;
        }

        bool direct_abstract_declarator(StringBuilder exp)
        {
            if (IsToken(Token.ParenthesisOpen))
            {
                ReadToken();

                if (parameter_type_list())
                {
                    if (IsToken(Token.ParenthesisClose))
                    {
                        ReadToken();

                        if (direct_abstract_declarator_line(exp))
                            return true;

                    }
                }
                else if (abstract_declarator())
                {
                    if (IsToken(Token.ParenthesisClose))
                    {
                        ReadToken();

                        if (direct_abstract_declarator_line(exp))
                            return true;


                    }
                }
                else if (IsToken(Token.ParenthesisClose))
                {
                    ReadToken();

                    if (direct_abstract_declarator_line(exp))
                        return true;

                }

            }

            else if (IsToken(Token.BracketOpen))
            {
                ReadToken();

                if (logical_or_expression(exp))
                {
                    if (IsToken(Token.BracketClose))
                    {
                        ReadToken();

                        if (direct_abstract_declarator_line(exp))
                            return true;

                    }
                }
                else if (IsToken(Token.BracketClose))
                {
                    ReadToken();

                    if (direct_abstract_declarator_line(exp))
                        return true;

                }


            }

            return false;
        }

        bool direct_abstract_declarator_line(StringBuilder exp)
        {

            if (IsToken(Token.BracketOpen))
            {
                ReadToken();

                if (logical_or_expression(exp))
                {
                    if (IsToken(Token.BracketClose))
                    {
                        ReadToken();
                        if (direct_abstract_declarator_line(exp))
                            return true;

                    }
                }

                else if (IsToken(Token.BracketClose))
                {
                    ReadToken();
                    if (direct_abstract_declarator_line(exp))
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
                        if (direct_abstract_declarator_line(exp))
                            return true;

                    }
                }

                else if (IsToken(Token.ParenthesisClose))
                {
                    ReadToken();
                    if (direct_abstract_declarator_line(exp))
                        return true;

                }

            }

            return true;
        }

        bool identifier_list()
        {
            if (IsToken(Token.Identifier))
            {
                ReadToken();

                if (identifier_list_line())
                    return true;


            }

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

        bool compound_statement(StringBuilder stat)
        {
            if (IsToken(Token.BraceOpen))
            {
                ReadToken();

                if (IsToken(Token.BraceClose))
                {
                    ReadToken();

                    return true;
                }

                if (compound_body_list(stat))
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

        bool compound_body_list(StringBuilder stat)
        {
            if (compound_body(stat))
            {
                if (IsToken(Token.BraceClose) || compound_body_list(stat))
                    return true;
            }

            return false;
        }

        bool compound_body(StringBuilder stat)
        {
            if (declaration_list(stat))
                return true;

            else if (statement_list(stat))
                return true;

            return false;
        }

        bool declaration_list(StringBuilder stat)
        {
            if (declaration(stat))
            {
                if (CanBe(declaration_specifiers))
                {
                    if (declaration_list(stat))
                        return true;
                }
                else
                    return true;
            }

            return false;
        }

        bool declaration(StringBuilder declaration)
        {
            if (declaration_specifiers())
            {
                if (IsToken(Token.SemiCollon))
                {
                    ReadToken();
                    return true;
                }

                else if (init_declarator_list(declaration))
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

        bool init_declarator_list(StringBuilder declaration)
        {

            if (init_declarator(declaration))
            {
                if (init_declarator_list_line(declaration))
                    return true;
            }

            return false;
        }

        bool init_declarator_list_line(StringBuilder declaration)
        {
            if (IsToken(Token.Comma))
            {
                ReadToken();

                if (init_declarator(declaration))
                {
                    if (init_declarator_list_line(declaration))
                        return true;
                }

            }

            return true;
        }

        bool init_declarator(StringBuilder stat)
        {
            var declaration = new StringBuilder();

            if (declarator(declaration))
            {
                if (IsToken(Token.Assign))
                {
                    ReadToken();

                    if (initializer(declaration))
                    {
                        declaration.AppendLine("=");

                        stat.Append(declaration);

                        return true;
                    }
                }

                return true;
            }

            return false;
        }

        bool initializer(StringBuilder stat)
        {
            if (assignment_expression(stat))
                return true;

            else if (IsToken(Token.BraceOpen))
            {
                ReadToken();

                if (initializer_list(stat))
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

        bool initializer_list(StringBuilder stat)
        {
            if (initializer(stat))
                if (initializer_list_line(stat))
                    return true;

            return false;
        }

        bool initializer_list_line(StringBuilder stat)
        {
            if (IsToken(Token.Comma))
            {
                ReadToken();

                if (initializer(stat))
                    if (initializer_list_line(stat))
                        return true;

            }

            return true;
        }

        bool statement_list(StringBuilder code)
        {
            var stat = new StringBuilder();

            if (statement(stat))
            {
                code.AppendLine(stat.ToString());

                if (statement_list(code))
                    return true;

                return true;
            }

            return false;
        }

        bool statement(StringBuilder code)
        {
            var stat = new StringBuilder();

            var pos = SetPosition();

            if (compound_statement(stat))
            {
                code.AppendLine(stat.ToString());
                return true;
            }

            RetrievePosition(pos);
            stat.Clear();

            if (expression_statement(stat))
            {
                code.AppendLine(stat.ToString());
                return true;
            }

            RetrievePosition(pos);
            stat.Clear();

            if (iteration_statement(stat))
            {
                code.AppendLine(stat.ToString());
                return true;
            }

            return false;
        }

        bool expression_statement(StringBuilder exp)
        {
            if (IsToken(Token.SemiCollon))
            {
                ReadToken();
                return true;
            }

            else if (expression(exp))
            {
                if (IsToken(Token.SemiCollon))
                {
                    ReadToken();
                    return true;
                }
            }

            return false;
        }

        bool iteration_statement(StringBuilder forCode)
        {
            if (IsToken(Token.For))
            {
                ReadToken();

                if (IsToken(Token.ParenthesisOpen))
                {
                    ReadToken();

                    var initializationExp = new StringBuilder();

                    if (expression_statement(initializationExp))
                    {
                        if (initializationExp.Length > 0)
                            forCode.AppendLine(initializationExp.ToString());

                        var loopLabel = RegisterLabel("for_loop");
                        var exitLabel = RegisterLabel("exit");

                        forCode.AppendLine(loopLabel + ':');

                        var validationExp = new StringBuilder();

                        if (expression_statement(validationExp))
                        {
                            if (validationExp.Length > 0)
                            {
                                forCode.Append(validationExp);
                                forCode.AppendLine("gofalse " + exitLabel);
                            }

                            var operationExp = new StringBuilder();

                            if (expression(operationExp))
                            {
                                if (IsToken(Token.ParenthesisClose))
                                {
                                    ReadToken();

                                    var forBody = new StringBuilder();

                                    if (statement(forBody))
                                    {
                                        if (forBody.Length > 0)
                                            forCode.Append(forBody);

                                        if (operationExp.Length > 0)
                                            forCode.Append(operationExp);

                                        forCode.AppendLine("go " + loopLabel);
                                        forCode.AppendLine(exitLabel + ':');

                                        return true;
                                    }

                                }
                            }
                            else if (IsToken(Token.ParenthesisClose))
                            {
                                ReadToken();

                                var forBody = new StringBuilder();

                                if (statement(forBody))
                                {
                                    if (forBody.Length > 0)
                                        forCode.Append(forBody);

                                    if (operationExp.Length > 0)
                                        forCode.Append(operationExp);

                                    forCode.AppendLine("go " + loopLabel);
                                    forCode.AppendLine(exitLabel + ':');

                                    return true;
                                }
                            }
                        }
                    }

                }
            }

            return false;
        }

        string RegisterLabel(string label)
        {
            if (_labels.ContainsKey(label))
            {
                _labels[label]++;

                return $"{label}_{_labels[label]}";
            }

            _labels.Add(label, 0);

            return $"{label}_0";
        }

    }
}
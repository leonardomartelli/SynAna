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
            Production logicalAndExp = logical_and_expression(); //left side

            if (logicalAndExp)
            {
                Code code = new();

                code += logicalAndExp.Code;

                Production logicalOrLine = logical_or_expression_line(logicalAndExp);

                if (logicalOrLine)
                {
                    code += logicalOrLine.Code;

                    return new(code, logicalOrLine.Place, true);
                }
            }

            return false;
        }

        Production logical_or_expression_line(Production leftSide)
        {
            if (IsToken(Token.LogicalOr))
            {
                Code code = new();

                ReadToken();

                Production rightSide = logical_and_expression();

                if (rightSide)
                {
                    code += rightSide;

                    var place = CreateTemp();

                    code += $"{place} = {leftSide.Place} || {rightSide.Place}";

                    Production logicalOrLine = logical_or_expression_line(new(code, place, true));

                    if (logicalOrLine)
                        return logicalOrLine;
                }
            }

            return new(string.Empty, leftSide.Place, true);
        }

        Production logical_and_expression()
        {
            Production leftSide = inclusive_or_expression(); //left side

            if (leftSide)
            {
                Code code = new();

                code += leftSide.Code;

                Production exp = logical_and_expression_line(leftSide);

                if (exp)
                {
                    code += exp.Code;

                    return new(code, exp.Place, true);
                }
            }

            return false;
        }

        Production logical_and_expression_line(Production leftSide)
        {
            if (IsToken(Token.LogicalAnd))
            {
                Code code = new();

                ReadToken();

                Production rightSide = inclusive_or_expression();

                if (rightSide)
                {
                    code += rightSide;

                    var place = CreateTemp();
                    code += $"{place} = {leftSide.Place} && {rightSide.Place}";

                    Production expLine = logical_and_expression_line(new(code, place, true));

                    if (expLine)
                        return expLine;
                }
            }

            return new(string.Empty, leftSide.Place, true);
        }

        Production inclusive_or_expression()
        {
            Production leftSide = exclusive_or_expression(); //left side

            if (leftSide)
            {
                Code code = new();

                code += leftSide.Code;

                Production exp = inclusive_or_expression_line(leftSide);

                if (exp)
                {
                    code += exp.Code;

                    return new(code, exp.Place, true);
                }
            }

            return false;
        }

        Production inclusive_or_expression_line(Production leftSide)
        {
            if (IsToken(Token.Or))
            {
                Code code = new();

                ReadToken();

                Production rightSide = exclusive_or_expression();

                if (rightSide)
                {
                    code += rightSide;

                    var place = CreateTemp();
                    code += $"{place} = {leftSide.Place} | {rightSide.Place}";

                    Production expLine = inclusive_or_expression_line(new(code, place, true));

                    if (expLine)
                        return expLine;
                }
            }

            return new(string.Empty, leftSide.Place, true);
        }

        Production exclusive_or_expression()
        {
            Production leftSide = and_expression(); //left side

            if (leftSide)
            {
                Code code = new();

                code += leftSide.Code;

                Production exp = exclusive_or_expression_line(leftSide);

                if (exp)
                {
                    code += exp.Code;

                    return new(code, exp.Place, true);
                }
            }

            return false;
        }

        Production exclusive_or_expression_line(Production leftSide)
        {
            if (IsToken(Token.Xor))
            {
                Code code = new();

                ReadToken();

                Production rightSide = and_expression();

                if (rightSide)
                {
                    code += rightSide;

                    var place = CreateTemp();
                    code += $"{place} = {leftSide.Place} ^ {rightSide.Place}";

                    Production expLine = exclusive_or_expression_line(new(code, place, true));

                    if (expLine)
                        return expLine;
                }
            }

            return new(string.Empty, leftSide.Place, true);
        }

        Production and_expression()
        {
            Production leftSide = equality_expression(); //left side

            if (leftSide)
            {
                Code code = new();

                code += leftSide.Code;

                Production exp = and_expression_line(leftSide);

                if (exp)
                {
                    code += exp.Code;

                    return new(code, exp.Place, true);
                }
            }

            return false;
        }

        Production and_expression_line(Production leftSide)
        {
            if (IsToken(Token.And))
            {
                Code code = new();

                ReadToken();

                Production rightSide = equality_expression();

                if (rightSide)
                {
                    code += rightSide;

                    var place = CreateTemp();

                    code += $"{place} = {leftSide.Place} & {rightSide.Place}";

                    Production expLine = and_expression_line(new(code, place, true));

                    if (expLine)
                        return expLine;
                }
            }

            return new(string.Empty, leftSide.Place, true);
        }

        Production equality_expression()
        {
            Production leftSide = relational_expression(); //left side

            if (leftSide)
            {
                Code code = new();

                code += leftSide.Code;

                Production exp = equality_expression_line(leftSide);

                if (exp)
                {
                    code += exp.Code;

                    return new(code, exp.Place, true);
                }
            }

            return false;
        }

        Production equality_expression_line(Production leftSide)
        {
            if (IsToken(Token.Equals))
            {
                Code code = new();

                ReadToken();

                Production rightSide = relational_expression();

                if (rightSide)
                {
                    code += rightSide;

                    var place = CreateTemp();

                    code += $"{place} = {leftSide.Place} == {rightSide.Place}";

                    Production expLine = equality_expression_line(new(code, place, true));

                    if (expLine)
                        return expLine;
                }
            }

            else if (IsToken(Token.NotEquals))
            {
                Code code = new();

                ReadToken();

                Production rightSide = relational_expression();

                if (rightSide)
                {
                    code += rightSide;

                    var place = CreateTemp();

                    code += $"{place} = {leftSide.Place} != {rightSide.Place}";

                    Production expLine = equality_expression_line(new(code, place, true));

                    if (expLine)
                        return expLine;
                }
            }

            return new(string.Empty, leftSide.Place, true);
        }

        Production relational_expression()
        {
            Production leftSide = shift_expression(); //left side

            if (leftSide)
            {
                Code code = new();

                code += leftSide.Code;

                Production exp = relational_expression_line(leftSide);

                if (exp)
                {
                    code += exp.Code;

                    return new(code, exp.Place, true);
                }
            }

            return false;
        }

        Production relational_expression_line(Production leftSide)
        {
            if (IsToken(Token.Less))
            {
                Code code = new();

                ReadToken();

                Production rightSide = shift_expression();

                if (rightSide)
                {
                    code += rightSide;

                    var place = CreateTemp();

                    code += $"{place} = {leftSide.Place} < {rightSide.Place}";

                    Production expLine = relational_expression_line(new(code, place, true));

                    if (expLine)
                        return expLine;
                }
            }

            else if (IsToken(Token.Greater))
            {
                Code code = new();

                ReadToken();

                Production rightSide = shift_expression();

                if (rightSide)
                {
                    code += rightSide;

                    var place = CreateTemp();

                    code += $"{place} = {leftSide.Place} > {rightSide.Place}";

                    Production expLine = relational_expression_line(new(code, place, true));

                    if (expLine)
                        return expLine;
                }
            }

            else if (IsToken(Token.LessOrEqual))
            {
                Code code = new();

                ReadToken();

                Production rightSide = shift_expression();

                if (rightSide)
                {
                    code += rightSide;

                    var place = CreateTemp();

                    code += $"{place} = {leftSide.Place} <= {rightSide.Place}";

                    Production expLine = relational_expression_line(new(code, place, true));

                    if (expLine)
                        return expLine;
                }
            }

            else if (IsToken(Token.GreaterOrEqual))
            {
                Code code = new();

                ReadToken();

                Production rightSide = shift_expression();

                if (rightSide)
                {
                    code += rightSide;

                    var place = CreateTemp();

                    code += $"{place} = {leftSide.Place} >= {rightSide.Place}";

                    Production expLine = relational_expression_line(new(code, place, true));

                    if (expLine)
                        return expLine;
                }
            }

            return new(string.Empty, leftSide.Place, true);
        }

        Production shift_expression()
        {
            Production leftSide = additive_expression(); //left side

            if (leftSide)
            {
                Code code = new();

                code += leftSide.Code;

                Production exp = shift_expression_line(leftSide);

                if (exp)
                {
                    code += exp.Code;

                    return new(code, exp.Place, true);
                }
            }

            return false;
        }

        Production shift_expression_line(Production leftSide)
        {
            if (IsToken(Token.ShiftLeft))
            {
                Code code = new();

                ReadToken();

                Production rightSide = additive_expression();

                if (rightSide)
                {
                    code += rightSide;

                    var place = CreateTemp();

                    code += $"{place} = {leftSide.Place} << {rightSide.Place}";

                    Production expLine = shift_expression_line(new(code, place, true));

                    if (expLine)
                        return expLine;
                }
            }


            else if(IsToken(Token.ShiftRight))
            {
                Code code = new();

                ReadToken();

                Production rightSide = additive_expression();

                if (rightSide)
                {
                    code += rightSide;

                    var place = CreateTemp();

                    code += $"{place} = {leftSide.Place} >> {rightSide.Place}";

                    Production expLine = shift_expression_line(new(code, place, true));

                    if (expLine)
                        return expLine;
                }
            }

            return new(string.Empty, leftSide.Place, true);
        }

        Production additive_expression()
        {
            Production leftSide = multiplicative_expression(); //left side

            if (leftSide)
            {
                Code code = new();

                code += leftSide.Code;

                Production exp = additive_expression_line(leftSide);

                if (exp)
                {
                    code += exp.Code;

                    return new(code, exp.Place, true);
                }
            }

            return false;
        }

        Production additive_expression_line(Production leftSide)
        {
            if (IsToken(Token.Plus))
            {
                Code code = new();

                ReadToken();

                Production rightSide = multiplicative_expression();

                if (rightSide)
                {
                    code += rightSide;

                    var place = CreateTemp();

                    code += $"{place} = {leftSide.Place} + {rightSide.Place}";

                    Production expLine = additive_expression_line(new(code, place, true));

                    if (expLine)
                        return expLine;
                }
            }

            else if (IsToken(Token.Minus))
            {
                Code code = new();

                ReadToken();

                Production rightSide = multiplicative_expression();

                if (rightSide)
                {
                    code += rightSide;

                    var place = CreateTemp();

                    code += $"{place} = {leftSide.Place} - {rightSide.Place}";

                    Production expLine = additive_expression_line(new(code, place, true));

                    if (expLine)
                        return expLine;
                }
            }

            return new(string.Empty, leftSide.Place, true);
        }

        Production multiplicative_expression()
        {
            Production leftSide = unary_expression(); //left side

            if (leftSide)
            {
                Code code = new();

                code += leftSide.Code;

                Production exp = multiplicative_expression_line(leftSide);

                if (exp)
                {
                    code += exp.Code;

                    return new(code, exp.Place, true);
                }
            }

            return false;
        }

        Production multiplicative_expression_line(Production leftSide)
        {
            if (IsToken(Token.Product))
            {
                Code code = new();

                ReadToken();

                Production rightSide = unary_expression();

                if (rightSide)
                {
                    code += rightSide;

                    var place = CreateTemp();

                    code += $"{place} = {leftSide.Place} * {rightSide.Place}";

                    Production expLine = multiplicative_expression_line(new(code, place, true));

                    if (expLine)
                        return expLine;
                }
            }

            else if (IsToken(Token.Division))
            {
                Code code = new();

                ReadToken();

                Production rightSide = unary_expression();

                if (rightSide)
                {
                    code += rightSide;

                    var place = CreateTemp();

                    code += $"{place} = {leftSide.Place} / {rightSide.Place}";

                    Production expLine = multiplicative_expression_line(new(code, place, true));

                    if (expLine)
                        return expLine;
                }
            }


            else if (IsToken(Token.Module))
            {
                Code code = new();

                ReadToken();

                Production rightSide = unary_expression();

                if (rightSide)
                {
                    code += rightSide;

                    var place = CreateTemp();

                    code += $"{place} = {leftSide.Place} % {rightSide.Place}";

                    Production expLine = multiplicative_expression_line(new(code, place, true));

                    if (expLine)
                        return expLine;
                }
            }

            return new(string.Empty, leftSide.Place, true);
        }

        Production unary_expression()
        {
            Production postFix_exp = postfix_expression();

            if (postFix_exp)
                return postFix_exp;

            if (IsToken(Token.Increment))
            {
                ReadToken();

                Production postFixExp = unary_expression();
                if (postFixExp)
                    return postFixExp;
            }

            if (IsToken(Token.Decrement))
            {
                ReadToken();

                Production postFixExp = unary_expression();
                if (postFixExp)
                    return postFixExp;
            }

            if (unary_operator())
            {
                Production postFixExp = unary_expression();

                if (postFixExp)
                    return postFixExp;
            }

            return false;
        }

        Production postfix_expression()
        {
            Production leftSide = primary_expression();

            if (leftSide)
                if (postfix_expression_line(leftSide))
                    return leftSide;

            return false;
        }

        Production postfix_expression_line(Production leftSide)
        {
            if (IsToken(Token.ParenthesisOpen))
            {
                ReadToken();

                if (argument_expression_list())
                {
                    if (IsToken(Token.ParenthesisClose))
                    {
                        ReadToken();

                        if (postfix_expression_line(leftSide))
                            return new(string.Empty, leftSide.Place, true);

                    }
                }

                else if (IsToken(Token.ParenthesisClose))
                {
                    ReadToken();

                    if (postfix_expression_line(leftSide))
                        return new(string.Empty, leftSide.Place, true);

                }

            }

            else if (IsToken(Token.Dot))
            {
                ReadToken();

                if (IsToken(Token.Identifier))
                {
                    ReadToken();

                    if (postfix_expression_line(leftSide))
                        return new(string.Empty, leftSide.Place, true);
                }
            }

            else if (IsToken(Token.StructAccessor))
            {
                ReadToken();

                if (IsToken(Token.Identifier))
                {
                    ReadToken();

                    if (postfix_expression_line(leftSide))
                        return new(string.Empty, leftSide.Place, true);

                }
            }

            //TODO make increment

            else if (IsToken(Token.Increment))
            {
                ReadToken();

                if (postfix_expression_line(leftSide))
                    return new(string.Empty, leftSide.Place, true);
            }

            else if (IsToken(Token.Decrement))
            {
                ReadToken();

                if (postfix_expression_line(leftSide))
                    return new(string.Empty, leftSide.Place, true);
            }

            return true;
        }

        Production primary_expression()
        {
            if (IsToken(Token.Identifier))
            {
                var currentIdentifier = _currentTokenResult.Lexical;

                ReadToken();

                return new(string.Empty, currentIdentifier, true);
            }

            if (IsToken(Token.IntegerConstant))
            {
                var place = CreateTemp();

                Code code = $"{place} = {_currentTokenResult.Lexical};";

                ReadToken();

                return new(code, place, true);
            }

            if (IsToken(Token.FloatingPointConstant))
            {
                var place = CreateTemp();

                Code code = $"{place} = {_currentTokenResult.Lexical};";

                ReadToken();

                return new(code, place, true);
            }

            if (IsToken(Token.CharConstant))
            {
                var place = CreateTemp();

                Code code = $"{place} = {_currentTokenResult.Lexical};";

                ReadToken();

                return new(code, place, true);
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

            Production leftSide = assignment_expression(); //left side

            if (leftSide)
            {
                Code code = new();

                code += leftSide.Code;

                Production exp = expression_line(leftSide);

                if (exp)
                {
                    code += exp.Code;

                    return new(code, exp.Place, true);
                }
            }

            return false;
        }

        Production expression_line(Production leftSide)
        {
            if (IsToken(Token.Comma))
            {
                Code code = new();

                ReadToken();

                Production rightSide = assignment_expression();

                if (rightSide)
                {
                    code += rightSide;

                    var place = CreateTemp();

                    code += $"{place} = {leftSide.Place} , {rightSide.Place}";

                    Production expLine = expression_line(new(code, place, true));

                    if (expLine)
                        return expLine;
                }
            }

            return new(string.Empty, leftSide.Place, true);
        }

        Production assignment_expression()
        {
            var pos = SetPosition();

            Code expCode = new();

            var unaryExp = unary_expression();

            if (unaryExp)
            {
                expCode += unaryExp.Code;

                Production assignmentOperator = assignment_operator();

                if (assignmentOperator)
                {
                    var pos_assing = SetPosition();

                    var rightSideExp = assignment_expression();

                    if (rightSideExp)
                    {
                        expCode += rightSideExp.Code;
                    
                        expCode += $"(* {unaryExp.Place}) = {rightSideExp.Place};";

                        return new(expCode, unaryExp.Place, true);
                    }

                    else
                    {
                        RetrievePosition(pos_assing);

                        Production rightLogicalOrExp = logical_or_expression();


                        if (rightLogicalOrExp)
                        {
                            Code code = $"{unaryExp.Place} = {rightLogicalOrExp.Place};";

                            expCode += rightLogicalOrExp.Code;
                            expCode += code;

                            return new(expCode, unaryExp.Place, true);
                        }
                    }
                }
            }

            RetrievePosition(pos);

            Production logicalOrExp = logical_or_expression();

            if (logicalOrExp)
                return logicalOrExp;

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

        Production compound_statement(string escapeLabel = null, string valueToCompare = null)
        {
            if (IsToken(Token.BraceOpen))
            {
                ReadToken();

                if (IsToken(Token.BraceClose))
                {
                    ReadToken();

                    return true;
                }

                Production compoundBody = compound_body_list(escapeLabel, valueToCompare);

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

        Production compound_body_list(string escapeLabel, string valueToCompare)
        {
            var code = new Code();

            Production body = compound_body(escapeLabel, valueToCompare);

            if (body)
            {
                code += body.Code;

                var list = compound_body_list(escapeLabel, valueToCompare);

                if (list)
                    code += list.Code;

                if (IsToken(Token.BraceClose) || list)
                    return new(code, true);
            }

            return false;
        }

        Production compound_body(string escapeLabel, string valueToCompare)
        {
            var code = new Code();

            if (declaration_list())
                return true;

            var stat = statement_list(escapeLabel, valueToCompare);

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

        Production statement_list(string escapeLabel, string valueToCompare)
        {
            Code code = new();

            var stat = statement(escapeLabel, valueToCompare);

            if (stat)
            {
                code += stat.Code;

                var statList = statement_list(escapeLabel, valueToCompare);

                if (statList)
                    code += statList.Code;

                return new(code, true);
            }

            return false;
        }

        Production statement(string escapeLabel = null, string exp = null)
        {
            var code = string.Empty;

            var pos = SetPosition();

            Production result = compound_statement(escapeLabel, exp);

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

            RetrievePosition(pos);
            code = string.Empty;

            result = labeled_statement(escapeLabel, exp);

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
            else if (expression() is Production exp && exp.Valid)
            {
                if (IsToken(Token.SemiCollon))
                {
                    ReadToken();
                    return exp;
                }
            }

            return false;
        }

        Production selection_statement() //tem que fazer suportar pelo statement()
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

                        var stat = statement(escapeLabel, exp.Place);

                        if (stat)
                            return stat;
                    }
                }

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

                code += expression;

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
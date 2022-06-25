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
                

                //   declarator declaration_list compound_statement
                // | declarator compound_statement
                if (declarator())
                {
                    

                    //declaration_list compound_statement
                    if (declaration_list())
                    {
                        

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
                

                // declaration_list compound_statement
                if (declaration_list())
                {
                    
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
                // declaration_specifiers
                if (declaration_specifiers())
                    return true;
                else
                {


                    return true;
                }
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
            {
                ReadToken();
                return true;
            }
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
                || IsToken(Token.Double))
            {
                ReadToken();

                return true;
            }

            // long_int_specifier
            else if (long_int_specifier())
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
                {
                    ReadToken();
                    return true;
                }


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
                            

                            // BraceClose
                            if (IsToken(Token.BraceClose))
                            {
                                ReadToken();
                                return true;
                            }


                        }


                    }
                    else
                    {



                        return true;
                    }


                }
                else if (IsToken(Token.BraceOpen))
                {
                    ReadToken();
                    // struct_declaration_list BraceClose
                    if (struct_declaration_list())
                    {
                        

                        // BraceClose
                        if (IsToken(Token.BraceClose))
                            return true;


                    }


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
                

                // struct_declarator_list SemiCollon
                if (struct_declaration_list())
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

        // specifier_list
        // : type_specifier specifier_list
        // | type_specifier
        bool specifier_list()
        {
            // type_specifier specifier_list
            // | type_specifier
            if (type_specifier())
            {
                

                // specifier_list
                if (specifier_list())
                    return true;
                else
                {


                    return true;
                }
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
                ReadToken();

                // pointer
                if (pointer())
                    return true;

                else
                {


                    return true;
                }
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
            {
                ReadToken();
                return true;
            }

            // ParenthisOpen declarator ParenthisClose
            else if (IsToken(Token.ParenthesisOpen))
            {
                ReadToken();

                // declarator ParenthisClose
                if (declarator())
                {

                    // ParenthisClose
                    if (IsToken(Token.ParenthesisClose))
                    {
                        ReadToken();
                        return true;
                    }


                }

            }

            // direct_declarator BracketOpen constant_expression BracketClose
            // | direct_declarator BracketOpen BracketClose
            // | direct_declarator ParenthisOpen parameter_type_list ParenthisClose
            // | direct_declarator ParenthisOpen identifier_list ParenthisClose
            // | direct_declarator ParenthisOpen ParenthisClose
            if (direct_declarator())
            {
                

                // BracketOpen constant_expression BracketClose
                // | BracketOpen BracketClose
                if (IsToken(Token.BracketOpen))
                {
                    ReadToken();

                    // constant_expression BracketClose
                    if (logical_or_expression())
                    {
                        

                        // BracketClose
                        if (IsToken(Token.BracketClose))
                        {
                            ReadToken();
                            return true;
                        }


                    }

                    // BracketClose
                    else if (IsToken(Token.BracketClose))
                    {
                        ReadToken();
                        return true;
                    }


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
                        

                        // ParenthisClose
                        if (IsToken(Token.ParenthesisClose))
                        {
                            ReadToken();
                            return true;
                        }


                    }

                    // identifier_list ParenthisClose
                    else if (identifier_list())
                    {
                        

                        // ParenthisClose
                        if (IsToken(Token.ParenthesisClose))
                        {
                            ReadToken();
                            return true;
                        }


                    }

                    // ParenthisClose
                    else if (IsToken(Token.ParenthesisClose))
                    {
                        ReadToken();
                        return true;
                    }


                }


            }

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
            else if (logical_or_expression())
            {
                

                // LogicalOr logical_and_expression
                if (IsToken(Token.LogicalOr))
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

            // inclusive_or_expression Or exclusive_or_expression
            else if (inclusive_or_expression())
            {
                

                // Or exclusive_or_expression
                if (IsToken(Token.Or))
                {
                    ReadToken();

                    // exclusive_or_expression
                    if (exclusive_or_expression())
                        return true;


                }


            }

            return false;
        }

        // exclusive_or_expression
        // : and_expression
        // | exclusive_or_expression Xor and_expression
        bool exclusive_or_expression()
        {
            // and_expression
            if (and_expression())
                return true;

            // exclusive_or_expression Xor and_expression
            else if (exclusive_or_expression())
            {
                

                // Xor and_expression
                if (IsToken(Token.Xor))
                {
                    ReadToken();

                    // and_expression
                    if (and_expression())
                        return true;


                }


            }

            return false;
        }

        // and_expression
        // : equality_expression
        // | and_expression And equality_expression
        bool and_expression()
        {
            //equality_expression
            if (equality_expression())
                return true;

            // and_expression And equality_expression
            else if (and_expression())
            {
                

                //And equality_expression
                if (IsToken(Token.And))
                {
                    ReadToken();

                    // equality_expression
                    if (equality_expression())
                        return true;


                }


            }

            return false;
        }

        // equality_expression
        // : relational_expression
        // | equality_expression Equals relational_expression
        // | equality_expression NotEquals relational_expression

        bool equality_expression()
        {
            // relational_expression
            if (relational_expression())
                return true;

            // equality_expression Equals relational_expression
            // | equality_expression NotEquals relational_expression
            else if (equality_expression())
            {
                

                // Equals relational_expression
                if (IsToken(Token.Equals))
                {
                    ReadToken();

                    // relational_expression
                    if (relational_expression())
                        return true;


                }

                // NotEquals relational_expression
                else if (IsToken(Token.NotEquals))
                {
                    ReadToken();

                    // relational_expression
                    if (relational_expression())
                        return true;


                }


            }

            return false;
        }

        // relational_expression
        // : shift_expression
        // | relational_expression Less shift_expression
        // | relational_expression Greater shift_expression
        // | relational_expression LessOrEqual shift_expression
        // | relational_expression GreaterOrEqual shift_expression
        bool relational_expression()
        {
            // shift_expression
            if (shift_expression())
                return true;

            // relational_expression Less shift_expression
            // | relational_expression Greater shift_expression
            // | relational_expression LessOrEqual shift_expression
            // | relational_expression GreaterOrEqual shift_expression
            if (relational_expression())
            {
                

                // Less shift_expression
                if (IsToken(Token.Less))
                {
                    ReadToken();

                    // shift_expression
                    if (shift_expression())
                        return true;


                }

                // Greater shift_expression
                else if (IsToken(Token.Greater))
                {
                    ReadToken();

                    // shift_expression
                    if (shift_expression())
                        return true;


                }

                // LessOrEqual shift_expression
                else if (IsToken(Token.LessOrEqual))
                {
                    ReadToken();

                    // shift_expression
                    if (shift_expression())
                        return true;


                }

                // GreaterOrEqual shift_expression
                else if (IsToken(Token.GreaterOrEqual))
                {
                    ReadToken();

                    // shift_expression
                    if (shift_expression())
                        return true;


                }


            }

            return false;
        }

        // shift_expression
        // : additive_expression
        // | shift_expression ShiftLeft additive_expression
        // | shift_expression ShiftRight additive_expression
        bool shift_expression()
        {
            // additive_expression
            if (additive_expression())
                return true;

            // shift_expression ShiftLeft additive_expression
            // | shift_expression ShiftRight additive_expression
            if (shift_expression())
            {
                

                // ShiftLeft additive_expression
                if (IsToken(Token.ShiftLeft))
                {
                    ReadToken();

                    // additive_expression
                    if (additive_expression())
                        return true;


                }

                // ShiftRight additive_expression
                else if (IsToken(Token.ShiftRight))
                {
                    ReadToken();

                    // additive_expression
                    if (additive_expression())
                        return true;


                }


            }

            return false;
        }

        // additive_expression
        // : multiplicative_expression
        // | additive_expression Plus multiplicative_expression
        // | additive_expression Minus multiplicative_expression
        bool additive_expression()
        {
            // multiplicative_expression
            if (multiplicative_expression())
                return true;

            // additive_expression Plus multiplicative_expression
            // | additive_expression Minus multiplicative_expression
            if (additive_expression())
            {
                

                // Plus multiplicative_expression
                if (IsToken(Token.Plus))
                {
                    ReadToken();

                    // multiplicative_expression
                    if (multiplicative_expression())
                        return true;


                }

                // Minus multiplicative_expression
                else if (IsToken(Token.Minus))
                {
                    ReadToken();

                    // multiplicative_expression
                    if (multiplicative_expression())
                        return true;


                }


            }

            return false;
        }

        // multiplicative_expression
        // : unary_expression
        // | multiplicative_expression Product unary_expression
        // | multiplicative_expression Division unary_expression
        // | multiplicative_expression Module unary_expression
        bool multiplicative_expression()
        {
            // unary_expression
            if (unary_expression())
                return false;

            // multiplicative_expression Product unary_expression
            // | multiplicative_expression Division unary_expression
            // | multiplicative_expression Module unary_expression
            if (multiplicative_expression())
            {
                

                // Product unary_expression
                if (IsToken(Token.Product))
                {
                    ReadToken();

                    // unary_expression
                    if (unary_expression())
                        return true;


                }

                // Division unary_expression
                else if (IsToken(Token.Division))
                {
                    ReadToken();

                    // unary_expression
                    if (unary_expression())
                        return true;


                }

                // Module unary_expression
                else if (IsToken(Token.Module))
                {
                    ReadToken();

                    // unary_expression
                    if (unary_expression())
                        return true;


                }


            }

            return false;
        }

        // unary_expression
        // : postfix_expression
        // | Increment unary_expression
        // | Decrement unary_expression
        // | unary_operator unary_expression
        bool unary_expression()
        {
            // postfix_expression
            if (postfix_expression())
                return true;

            // Increment unary_expression
            if (IsToken(Token.Increment))
            {
                ReadToken();

                if (unary_expression())
                    return true;


            }

            // Decrement unary_expression
            if (IsToken(Token.Decrement))
            {
                ReadToken();

                if (unary_expression())
                    return true;


            }

            // unary_operator unary_expression
            if (unary_operator())
            {
                

                if (unary_expression())
                    return true;


            }

            return false;
        }

        // postfix_expression
        // : primary_expression
        // | postfix_expression BracketOpen expression BracketClose
        // | postfix_expression ParenthisOpen ParenthisClose
        // | postfix_expression ParenthisOpen argument_expression_list ParenthisClose
        // | postfix_expression Dot Identifier
        // | postfix_expression StructAccessor Identifier
        // | postfix_expression Increment
        // | postfix_expression Decrement
        bool postfix_expression()
        {
            // primary_expression
            if (primary_expression())
                return true;

            // postfix_expression BracketOpen expression BracketClose
            // | postfix_expression ParenthisOpen ParenthisClose
            // | postfix_expression ParenthisOpen argument_expression_list ParenthisClose
            // | postfix_expression Dot Identifier
            // | postfix_expression StructAccessor Identifier
            // | postfix_expression Increment
            // | postfix_expression Decrement
            if (postfix_expression())
            {
                

                // ParenthisOpen argument_expression_list ParenthisClose
                // | ParenthisOpen ParenthisClose
                if (IsToken(Token.ParenthesisOpen))
                {
                    ReadToken();

                    // argument_expression_list ParenthisClose
                    if (argument_expression_list())
                    {
                        

                        // ParenthisClose
                        if (IsToken(Token.ParenthesisClose))
                        {
                            ReadToken();
                            return true;
                        }


                    }

                    // ParenthisClose
                    else if (IsToken(Token.ParenthesisClose))
                    {
                        ReadToken();
                        return true;
                    }


                }

                // Dot Identifier
                else if (IsToken(Token.Dot))
                {
                    ReadToken();

                    // Identifier
                    if (IsToken(Token.Identifier))
                    {
                        ReadToken();
                        return true;
                    }


                }

                // StructAccessor Identifier
                else if (IsToken(Token.StructAccessor))
                {
                    ReadToken();

                    // Identifier
                    if (IsToken(Token.Identifier))
                    {
                        ReadToken();
                        return true;
                    }


                }

                // Increment
                else if (IsToken(Token.Increment))
                {
                    ReadToken();
                    return true;
                }

                // Decrement
                else if (IsToken(Token.Decrement))
                {
                    ReadToken();
                    return true;
                }


            }

            return false;
        }

        // primary_expression
        // : Identifier
        // | IntegerConstant
        // | FloatingPointConstant
        // | ParenthisOpen expression ParenthisClose
        bool primary_expression()
        {
            // Identifier
            if (IsToken(Token.Identifier))
            {
                ReadToken();
                return true;
            }

            // IntegerConstant
            if (IsToken(Token.IntegerConstant))
            {
                ReadToken();
                return true;
            }

            // FloatingPointConstant
            if (IsToken(Token.FloatingPointConstant))
            {
                ReadToken();
                return true;
            }

            // ParenthisOpen expression ParenthisClose
            if (IsToken(Token.ParenthesisOpen))
            {
                ReadToken();

                // expression ParenthisClose
                if (expression())
                {
                    

                    // ParenthisClose
                    if (IsToken(Token.ParenthesisClose))
                    {
                        ReadToken();
                        return true;
                    }


                }


            }

            return false;
        }

        // expression
        // : assignment_expression
        // | expression Comma assignment_expression
        bool expression()
        {
            // assignment_expression
            if (assignment_expression())
                return true;

            // expression Comma assignment_expression
            if (expression())
            {
                

                // Comma assignment_expression
                if (IsToken(Token.Comma))
                {
                    ReadToken();

                    // assignment_expression
                    if (assignment_expression())
                        return true;


                }


            }

            return false;
        }

        // assignment_expression
        // : logical_or_expression
        // | unary_expression assignment_operator assignment_expression
        bool assignment_expression()
        {
            if (logical_or_expression())
                return true;

            // unary_expression assignment_operator assignment_expression
            if (unary_expression())
            {
                

                // assignment_operator assignment_expression
                if (assignment_operator())
                {
                    

                    // assignment_expression
                    if (assignment_expression())
                        return true;


                }


            }

            return false;
        }

        // assignment_operator
        // : Assign
        // | ProductAssign
        // | DivisionAssign
        // | ModuleAssign
        // | PlusAssign
        // | MinusAssign
        // | LeftAssign
        // | RightAssign
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

        // argument_expression_list
        // : assignment_expression
        // | argument_expression_list Comma assignment_expression
        bool argument_expression_list()
        {
            // assignment_expression
            if (assignment_expression())
                return true;

            // argument_expression_list Comma assignment_expression
            else if (argument_expression_list())
            {
                

                if (IsToken(Token.Comma))
                {
                    ReadToken();

                    if (assignment_expression())
                        return true;


                }


            }

            return false;
        }

        // unary_operator
        // : And
        // | Product
        // | Plus
        // | Minus
        // | Negate
        // | LogicalNot
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

        // parameter_type_list
        // : parameter_list
        // | parameter_list Comma Ellipsis
        bool parameter_type_list()
        {
            // parameter_list
            // | parameter_list Comma Ellipsis
            if (parameter_list())
            {
                

                // Comma Ellipsis
                if (IsToken(Token.Comma))
                {
                    ReadToken();

                    // Ellipsis
                    if (IsToken(Token.Ellipsis))
                    {
                        ReadToken();
                        return true;
                    }


                }
                else
                {


                    return true;
                }


            }

            return false;
        }

        // parameter_list
        // : parameter_declaration
        // | parameter_list Comma parameter_declaration
        bool parameter_list()
        {
            // parameter_declaration
            if (parameter_declaration())
                return true;

            // parameter_list Comma parameter_declaration
            else if (parameter_list())
            {
                

                // Comma parameter_declaration
                if (IsToken(Token.Comma))
                {
                    ReadToken();

                    // parameter_declaration
                    if (parameter_declaration())
                        return true;


                }


            }

            return false;
        }

        // parameter_declaration
        // : declaration_specifiers declarator
        // | declaration_specifiers abstract_declarator
        // | declaration_specifiers
        bool parameter_declaration()
        {
            // declaration_specifiers declarator
            // | declaration_specifiers abstract_declarator
            // | declaration_specifiers
            if (declaration_specifiers())
            {
                

                if (declarator())
                    return true;

                //abstract_declarator
                else if (abstract_declarator())
                    return true;

                else
                {


                    return true;
                }
            }

            return false;
        }

        // abstract_declarator
        // : pointer
        // | direct_abstract_declarator
        // | pointer direct_abstract_declarator
        bool abstract_declarator()
        {
            // pointer direct_abstract_declarator
            if (pointer())
            {
                

                // direct_abstract_declarator
                if (direct_abstract_declarator())
                    return true;

                else
                {


                    return true;
                }
            }

            // direct_abstract_declarator
            else if (direct_abstract_declarator())
                return true;

            return false;
        }

        // direct_abstract_declarator
        // : ParenthesisOpen ParenthesisClose
        // | ParenthesisOpen abstract_declarator ParenthesisClose
        // | ParenthesisOpen parameter_type_list ParenthesisClose
        // | BracketOpen BracketClose
        // | BracketOpen constant_expression BracketClose
        // | direct_abstract_declarator BracketOpen BracketClose
        // | direct_abstract_declarator BracketOpen constant_expression BracketClose
        // | direct_abstract_declarator ParenthesisOpen ParenthesisClose
        // | direct_abstract_declarator ParenthesisOpen parameter_type_list ParenthesisClose
        bool direct_abstract_declarator()
        {
            // ParenthesisOpen ParenthesisClose
            // | ParenthesisOpen abstract_declarator ParenthesisClose
            // | ParenthesisOpen parameter_type_list ParenthesisClose
            if (IsToken(Token.ParenthesisOpen))
            {
                ReadToken();

                if (parameter_type_list())
                {
                    

                    if (IsToken(Token.ParenthesisClose))
                    {
                        ReadToken();
                        return true;
                    }


                }
                else if (abstract_declarator())
                {
                    

                    if (IsToken(Token.ParenthesisClose))
                    {
                        ReadToken();
                        return true;
                    }


                }
                else if (IsToken(Token.ParenthesisClose))
                {
                    ReadToken();
                    return true;
                }


            }

            // BracketOpen BracketClose
            // | BracketOpen constant_expression BracketClose
            else if (IsToken(Token.BracketOpen))
            {
                ReadToken();

                if (logical_or_expression())
                {
                    

                    if (IsToken(Token.BracketClose))
                    {
                        ReadToken();
                        return true;
                    }


                }


                else if (IsToken(Token.BracketClose))
                {
                    ReadToken();
                    return true;
                }


            }

            // direct_abstract_declarator BracketOpen BracketClose
            // | direct_abstract_declarator BracketOpen constant_expression BracketClose
            // | direct_abstract_declarator ParenthesisOpen ParenthesisClose
            // | direct_abstract_declarator ParenthesisOpen parameter_type_list ParenthesisClose

            else if (direct_abstract_declarator())
            {
                

                // BracketOpen BracketClose
                // | BracketOpen constant_expression BracketClose
                if (IsToken(Token.BracketOpen))
                {
                    ReadToken();

                    if (logical_or_expression())
                    {
                        

                        if (IsToken(Token.BracketClose))
                        {
                            ReadToken();
                            return true;
                        }


                    }

                    else if (IsToken(Token.BracketClose))
                    {
                        ReadToken();
                        return true;
                    }


                }

                // ParenthesisOpen ParenthesisClose
                // | ParenthesisOpen parameter_type_list ParenthesisClose

                else if (IsToken(Token.ParenthesisOpen))
                {
                    ReadToken();

                    if (parameter_type_list())
                    {
                        

                        if (IsToken(Token.ParenthesisClose))
                        {
                            ReadToken();
                            return true;
                        }


                    }

                    else if (IsToken(Token.ParenthesisClose))
                    {
                        ReadToken();
                        return true;
                    }


                }



            }

            return false;
        }


        // identifier_list
        // : Identifier
        // | identifier_list comma Identifier

        bool identifier_list()
        {
            // Identifier
            if (IsToken(Token.Identifier))
                return true;

            // identifier_list comma Identifier
            if (identifier_list())
            {
                

                // comma Identifier
                if (IsToken(Token.Comma))
                {
                    ReadToken();

                    // Identifier
                    if (IsToken(Token.Identifier))
                    {
                        ReadToken();
                        return true;
                    }
                }
            }

            return false;
        }

        // compound_statement
        // : BraceOpen BraceClose
        // | BraceOpen statement_list BraceClose
        // | BraceOpen declaration_list BraceClose
        // | BraceOpen declaration_list statement_list BraceClose
        bool compound_statement()
        {
            //  BraceOpen BraceClose
            // | BraceOpen statement_list BraceClose
            // | BraceOpen declaration_list BraceClose
            // | BraceOpen declaration_list statement_list BraceClose
            if (IsToken(Token.BraceOpen))
            {
                ReadToken();

                // BraceClose
                if (IsToken(Token.BraceClose))
                {
                    ReadToken();
                    return true;
                }

                // declaration_list BraceClose
                // | declaration_list statement_list BraceClose
                else if (declaration_list())
                {
                    // BraceClose
                    if (IsToken(Token.BraceClose))
                    {
                        ReadToken();
                        return true;
                    }

                    // statement_list BraceClose
                    else if (statement_list())
                    {
                        // BraceClose
                        if (IsToken(Token.BraceClose))
                        {
                            ReadToken();
                            return true;
                        }
                    }
                }

                // statement_list BraceClose
                else if (statement_list())
                {
                    

                    // BraceClose
                    if (IsToken(Token.BraceClose))
                    {
                        ReadToken();
                        return true;
                    }
                }
            }

            return false;
        }

        // declaration_list
        // : declaration
        // | declaration_list declaration
        bool declaration_list()
        {
            // declaration
            if (declaration())
                return true;

            // declaration_list declaration
            else if (declaration_list())
            {
                // declaration
                if (declaration())
                    return true;
            }
            return false;
        }

        // declaration
        // : declaration_specifiers SemiCollon
        // | declaration_specifiers init_declarator_list SemiCollon
        bool declaration()
        {
            // declaration_specifiers SemiCollon
            // | declaration_specifiers init_declarator_list SemiCollon
            if (declaration_specifiers())
            {
                

                // SemiCollon
                if (IsToken(Token.SemiCollon))
                {
                    ReadToken();
                    return true;
                }

                // init_declarator_list SemiCollon
                else if (init_declarator_list())
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

        // init_declarator_list
        // : init_declarator
        // | init_declarator_list Comma init_declarator
        bool init_declarator_list()
        {
            // init_declarator
            if (init_declarator())
                return true;

            // init_declarator_list Comma init_declarator
            else if (init_declarator_list())
            {
                

                // Comma init_declarator
                if (IsToken(Token.Comma))
                {
                    ReadToken();

                    // init_declarator
                    if (init_declarator())
                        return true;


                }


            }

            return false;
        }

        // init_declarator
        // : declarator
        // | declarator Assign initializer
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
                else
                {

                    return true;
                }


            }

            return false;
        }

        // intitializer
        // : assignment_expression
        // | BraceOpen initializer_list BraceClose
        // | BraceOpen initializer_list Comma BraceClose
        bool initializer()
        {
            // assignment_expression
            if (assignment_expression())
                return true;

            // BraceOpen initializer_list BraceClose
            // | BraceOpen initializer_list Comma BraceClose
            else if (IsToken(Token.BraceOpen))
            {
                ReadToken();

                //initializer_list BraceClose
                // | initializer_list Comma BraceClose
                if (initializer_list())
                {
                    

                    // BraceClose
                    if (IsToken(Token.BraceClose))
                    {
                        ReadToken();
                        return true;
                    }

                    // Comma BraceClose
                    else if (IsToken(Token.Comma))
                    {
                        ReadToken();

                        // BraceClose
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

        //initializer_list
        // : initializer
        // | initializer_list Comma initializer
        bool initializer_list()
        {
            // initializer
            if (initializer())
                return true;

            // initializer_list Comma initializer
            else if (initializer_list())
            {
                

                // Comma initializer
                if (IsToken(Token.Comma))
                {
                    ReadToken();

                    // initializer
                    if (initializer())
                        return true;
                }
            }

            return false;
        }

        // statement_list
        // : statement
        // | statement_list statement
        bool statement_list()
        {
            if (statement())
                return true;

            else if (statement_list())
            {
                

                if (statement())
                    return true;
            }

            return false;
        }

        // statement
        // : labeled_statement
        // | compound_statement
        // | expression_statement
        // | selection_statement
        // | iteration_statement
        // | jump_statement
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

        // labeled_statement
        // : Case constant_expression Collon statement
        // | Default Collon statement
        bool labeled_statement()
        {
            // Case constant_expression Collon statement
            if (IsToken(Token.Case))
            {
                ReadToken();

                // constant_expression Collon statement
                if (logical_or_expression())
                {
                    

                    // Collon statement
                    if (IsToken(Token.Collon))
                    {
                        ReadToken();

                        // statement
                        if (statement())
                            return true;


                    }


                }


            }

            // Default Collon statement
            else if (IsToken(Token.Default))
            {
                ReadToken();

                // Collon statement
                if (IsToken(Token.Collon))
                {
                    ReadToken();

                    // statement
                    if (statement())
                        return true;


                }


            }

            return false;
        }

        // expression_statement
        // : SemiCollon
        // | expression SemiCollon
        bool expression_statement()
        {
            // SemiCollon
            if (IsToken(Token.SemiCollon))
            {
                ReadToken();
                return true;
            }

            // expression SemiCollon
            else if (expression())
            {
                

                // SemiCollon
                if (IsToken(Token.SemiCollon))
                {
                    ReadToken();
                    return true;
                }


            }

            return false;
        }

        // selection_statement
        // : If ParenthesisOpen expression ParenthesisClose statement
        // | If ParenthesisOpen expression ParenthesisClose statement Else statement
        // | Switch ParenthesisOpen expression ParenthesisClose statement
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
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        // expression_statement_structure
        // : ParenthesisOpen expression ParenthesisClose statement
        bool expression_statement_structure()
        {
            // ParenthesisOpen expression ParenthesisClose statement
            if (IsToken(Token.ParenthesisOpen))
            {
                ReadToken();

                // expression ParenthesisClose statement
                if (expression())
                {
                    

                    // ParenthesisClose statement
                    if (IsToken(Token.ParenthesisClose))
                    {
                        ReadToken();

                        // statement
                        if (statement())
                            return true;


                    }


                }


            }

            return false;
        }

        //iteration_statement
        // : While expression_statement_structure
        // | Do statement While ParenthesisOpen expression ParenthesisClose SemiCollon
        // | For ParenthesisOpen expression_statement expression_statement ParenthesisClose statement
        // | For ParenthesisOpen expression_statement expression_statement expression ParenthesisClose statement
        bool iteration_statement()
        {
            if (IsToken(Token.While))
            {
                ReadToken();

                if (expression_statement_structure())
                    return true;


            }

            // Do statement While ParenthesisOpen expression ParenthesisClose SemiCollon
            else if (IsToken(Token.Do))
            {
                ReadToken();

                // statement While ParenthesisOpen expression ParenthesisClose SemiCollon
                if (statement())
                {
                    

                    // While ParenthesisOpen expression ParenthesisClose SemiCollon
                    if (IsToken(Token.While))
                    {
                        ReadToken();

                        // ParenthesisOpen expression ParenthesisClose SemiCollon
                        if (IsToken(Token.ParenthesisOpen))
                        {
                            ReadToken();

                            // expression ParenthesisClose SemiCollon
                            if (expression())
                            {
                                

                                // ParenthesisClose SemiCollon
                                if (IsToken(Token.ParenthesisClose))
                                {
                                    ReadToken();

                                    // SemiCollon
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

            // For ParenthesisOpen expression_statement expression_statement ParenthesisClose statement
            // | For ParenthesisOpen expression_statement expression_statement expression ParenthesisClose statement
            else if (IsToken(Token.For))
            {
                ReadToken();

                // ParenthesisOpen expression_statement expression_statement ParenthesisClose statement
                // | ParenthesisOpen expression_statement expression_statement expression ParenthesisClose statement
                if (IsToken(Token.ParenthesisOpen))
                {
                    ReadToken();

                    // expression_statement expression_statement ParenthesisClose statement
                    // | expression_statement expression_statement expression ParenthesisClose statement
                    if (expression_statement())
                    {
                        

                        // expression_statement ParenthesisClose statement
                        // | expression_statement expression ParenthesisClose statement
                        if (expression_statement())
                        {
                            

                            // expression ParenthesisClose statement
                            if (expression())
                            {
                                

                                // ParenthesisClose statement
                                if (IsToken(Token.ParenthesisClose))
                                {
                                    ReadToken();

                                    // statement
                                    if (statement())
                                        return true;


                                }


                            }
                            // ParenthesisClose statement
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

        // jump_statement
        // : Continue SemiCollon
        // | Break SemiCollon
        // | Return SemiCollon
        // | Return expression SemiCollon
        bool jump_statement()
        {
            // Continue SemiCollon
            if (IsToken(Token.Continue))
            {
                ReadToken();

                if (IsToken(Token.SemiCollon))
                {
                    ReadToken();
                    return true;
                }


            }
            // Break SemiCollon
            else if (IsToken(Token.Break))
            {
                ReadToken();

                if (IsToken(Token.SemiCollon))
                {
                    ReadToken();
                    return true;
                }


            }
            // Return SemiCollon
            // | Return expression SemiCollon
            else if (IsToken(Token.Return))
            {
                ReadToken();

                // SemiCollon
                if (IsToken(Token.SemiCollon))
                {
                    ReadToken();
                    return true;
                }

                // expression SemiCollon
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


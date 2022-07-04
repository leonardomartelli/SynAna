namespace SynAna.LexAna
{
    public enum Token
	{
		LexicalError = -1,
		EOF,
		Identifier,
		IntegerConstant,
		FloatingPointConstant,
		StringConstant,
		CharConstant,

		//Aritmethical Operators
		Plus,				// +
		Minus,				// -
		Product,			// *
		Division,			// /
		Increment,			// ++
		Decrement,			// --
		Module,				// %

		//Logical Operators
		Equals,				// ==
		NotEquals,			// !=
		Less,				// <
		Greater,			// >
		LessOrEqual,		// <=
		GreaterOrEqual,		// >=
		LogicalAnd,			// &&
		LogicalOr,			// ||
		LogicalNot,			// !

		//Bitwise Operators
		ShiftLeft,			// <<
		ShiftRight,			// >>
		And,				// &
		Or,					// |
		Negate,				// ˜
		Xor,				// ˆ

		//Assignment Operators
		Assign,				// =
		PlusAssign,			// +=
		MinusAssign,		// -=
		ProductAssign,		// *=
		DivisionAssign,		// /=
		ModuleAssign,       // %=
		LeftAssign,			// <<=
		RightAssign,		// >>=
		AndAssign,			// &=
		XorAssign,			// ˆ=
		OrAssign,			// |=

        //Language Commands
        For,                // for
        Do,					// do
		While,				// while
		If,					// if
		ElseIf,				// else if
		Else,				// else
		Continue,			// continue
		Break,				// break
		Return,				// return

		//Switch-case Commands
		Switch,				// switch
		Case,				// case
		Default,			// default

		//Primitive types
		Int,				// int
		Char,				// char
		Double,				// double
		Float,				// float
		Struct,				// struct
		Void,				// void
		Long,				// long
		Unsigned,			// unsigned

		//Pointers
		StructAccessor,		// ->

		//Characters
		Underscore,			// _
		Dot,				// .
		Comma,				// ,
		SemiCollon,			// ;
		Collon,				// :
		ParenthesisOpen,	// (
		ParenthesisClose,	// )
		BracketOpen,		// [
		BracketClose,		// ]
		BraceOpen,			// {
		BraceClose,			// }

		//Parameters
		Ellipsis			// ...
	}
}


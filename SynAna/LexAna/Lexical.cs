using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SynAna.LexAna
{
    public class Lexical
    {
        private readonly IDictionary<string, Token> _reservedWords =
            new Dictionary<string, Token>
            {
                { "int", Token.Int },
                { "float", Token.Float },
                { "double", Token.Double },
                { "char", Token.Char },
                { "void", Token.Void },
                { "return", Token.Return },
                { "continue", Token.Continue },
                { "break", Token.Break },
                { "if", Token.If },
                { "else", Token.Else },
                { "for", Token.For },
                { "while", Token.While },
                { "do", Token.Do },
                { "struct", Token.Struct },
                { "unsigned", Token.Unsigned },
                { "long", Token.Long },
                { "switch", Token.Switch },
                { "case", Token.Case },
                { "default", Token.Default }
            };

        private readonly StreamReader _inputStream;
        private readonly StreamWriter _outputStream;

        private char _character;
        private string _line;
        private int _currentColumn;
        private int _currentLine;
        private string _lexical;
        private State _state;

        private bool _hasPoint;
        private bool _eof;
        private bool _scape;

        private IList<TokenResult> _lexicalResult;

        public Lexical(StreamReader inputStream, string resultsFolder)
        {
            if (inputStream is null)
                throw new ArgumentNullException(nameof(inputStream));

            _character = default;
            _line = _lexical = string.Empty;
            _currentColumn = _currentLine = 0;
            _state = State.Initial;

            _hasPoint = _eof = _scape = false;

            _inputStream = inputStream;

            _outputStream = SetupResultsWriting(resultsFolder);

            _lexicalResult = new List<TokenResult>();
        }

        public IEnumerable<TokenResult> Analyze()
        {
            ReadChar();

            while (!_eof)
            {
                var result = ReadToken();

                if (result is null)
                    continue;

                CleanLexical();

                WriteResult(result);
            }

            var finalResult = new TokenResult(Token.EOF, string.Empty, _currentLine, _currentColumn);

            WriteResult(finalResult);

            return _lexicalResult;
        }

        private void ReadLine()
        {
            _line = _inputStream.ReadLine();

            if (_line is null)
                _eof = true;

            _line += '\n';
            _currentLine++;
        }

        private void ReadChar()
        {
            if (string.IsNullOrEmpty(_line) || _currentColumn >= _line.Length)
            {
                ReadLine();
                _currentColumn = 0;
            }

            _character = _line[_currentColumn++];
        }

        private TokenResult ReadToken()
        {
            AppendCharacter();

            switch (_state)
            {
                case State.Initial:
                    return ReadInitialToken();

                case State.Word:
                    return ReadWordToken();

                case State.Number:
                    return ReadNumberToken();

                default:
                    break;
            }

            return default;
        }

        private TokenResult ReadInitialToken()
        {
            if (_character == ' ' || _character == '\t' || _character == '\n')
            {
                CleanLexical();
                ReadChar();
            }

            else if (char.IsLetter(_character) || _character == '_' || _character == '\'')
            {
                ReadChar();
                _state = State.Word;
            }

            else if (char.IsDigit(_character))
            {
                ReadChar();
                _state = State.Number;
            }

            else if (_character == '(')
                return ComputeToken(Token.ParenthesisOpen);

            else if (_character == ')')
                return ComputeToken(Token.ParenthesisClose);

            else if (_character == '[')
                return ComputeToken(Token.BracketOpen);

            else if (_character == ']')
                return ComputeToken(Token.BracketClose);

            else if (_character == '{')
                return ComputeToken(Token.BraceOpen);

            else if (_character == '}')
                return ComputeToken(Token.BraceClose);

            else if (_character == ';')
                return ComputeToken(Token.SemiCollon);

            else if (_character == ',')
                return ComputeToken(Token.Comma);

            else if (_character == '.')
                return AnalysePossibleCases(Token.Dot,
                    new Case('.', Token.LexicalError,
                        new Case('.', Token.Ellipsis)));

            else if (_character == ':')
                return AnalysePossibleCases(Token.Collon);

            else if (_character == '=')
                return AnalysePossibleCases(Token.Assign,
                    new Case('=', Token.Equals));

            else if (_character == '!')
                return AnalysePossibleCases(Token.LogicalNot,
                    new Case('=', Token.NotEquals));

            else if (_character == '>')
                return AnalysePossibleCases(Token.Greater,
                    new Case('=', Token.GreaterOrEqual),
                    new Case('>', Token.ShiftRight,
                        new Case('=', Token.RightAssign)));

            else if (_character == '<')
                return AnalysePossibleCases(Token.Less,
                    new Case('=', Token.LessOrEqual),
                    new Case('<', Token.ShiftLeft,
                        new Case('=', Token.LeftAssign)));

            else if (_character == '+')
                return AnalysePossibleCases(Token.Plus,
                    new Case('=', Token.PlusAssign),
                    new Case('+', Token.Increment));

            else if (_character == '-')
                return AnalysePossibleCases(Token.Minus,
                    new Case('=', Token.MinusAssign),
                    new Case('-', Token.Decrement),
                    new Case('>', Token.StructAccessor));

            else if (_character == '/')
                return AnalysePossibleCases(Token.Division,
                    new Case('=', Token.DivisionAssign));

            else if (_character == '*')
                return AnalysePossibleCases(Token.Product,
                    new Case('=', Token.ProductAssign));

            else if (_character == '%')
                return AnalysePossibleCases(Token.Module,
                    new Case('=', Token.ModuleAssign));

            else if (_character == '|')
                return AnalysePossibleCases(Token.Or,
                    new Case('|', Token.LogicalOr),
                    new Case('=', Token.OrAssign));

            else if (_character == '&')
                return AnalysePossibleCases(Token.And,
                    new Case('&', Token.LogicalAnd),
                    new Case('=', Token.AndAssign));

            else if (_character == '^')
                return AnalysePossibleCases(Token.Xor,
                    new Case('=', Token.XorAssign));

            else if (_character == '~')
                return AnalysePossibleCases(Token.Negate);

            return default;
        }

        private TokenResult ReadWordToken()
        {
            if (char.IsLetter(_character) || _character == '_' || char.IsDigit(_character))
            {
                ReadChar();
                return default;
            }
            else if(_character == '\\')
            {
                _scape = !_scape;

                ReadChar();
                return default;
            }
            else if (_character == '\'')
            {
                if(_scape)
                {
                    _scape = false;
                    ReadChar();
                    return default;
                }

                _state = State.Initial;

                var token = IsConstantCharValid()
                    ? Token.CharConstant
                    : Token.LexicalError;

                return ComputeToken(token);
            }
            else
            {
                BackSpace();
                UnreadChar();

                _state = State.Initial;

                if (!_reservedWords.TryGetValue(_lexical, out var token))
                    token = Token.Identifier;

                return ComputeToken(token);
            }
        }

        private TokenResult ReadNumberToken()
        {
            if (char.IsDigit(_character))
            {
                ReadChar();

                return default;
            }
            else if (_character == '.')
            {
                if (!_hasPoint)
                {
                    ReadChar();
                    _hasPoint = true;

                    return default;
                }
                else
                {
                    _state = State.Initial;

                    return ComputeToken(Token.LexicalError);
                }
            }
            else
            {
                _state = State.Initial;

                BackSpace();
                UnreadChar();

                _hasPoint = false;

                var token =
                    int.TryParse(_lexical, out _) ? Token.IntegerConstant :
                    float.TryParse(_lexical, out _) ? Token.FloatingPointConstant :
                    Token.LexicalError;

                return ComputeToken(token);
            }

        }

        private TokenResult ComputeToken(Token token)
        {
            GetContext(out int lineNumber, out int columnNumber, out string lexical);
            ReadChar();
            CleanLexical();

            return new TokenResult(token, lexical, lineNumber, columnNumber);
        }

        private void GetContext(out int line, out int column, out string lexical)
        {
            line = _currentLine;
            column = _currentColumn;
            lexical = _lexical;
        }

        private StreamWriter SetupResultsWriting(string resultsFolder)
        {
            var resultFile = Path.Combine(resultsFolder, $"{DateTime.Now:yyyy_MM_dd_hh_mm_ss}.lexi");

            return File.CreateText(resultFile);
        }

        private TokenResult AnalysePossibleCases(Token defaultToken, params Case[] cases)
        {
            ReadChar();

            foreach (var @case in cases)
                if (_character == @case.Character)
                {
                    AppendCharacter();
                    return AnalysePossibleCases(@case.Token, @case.Cases);
                }

            UnreadChar();
            return ComputeToken(defaultToken);
        }

        private void AppendCharacter() =>
            _lexical += _character;

        private TokenResult WriteResult(TokenResult tokenResult)
        {
            _outputStream.WriteLine(tokenResult);

            //Console.WriteLine(tokenResult);

            _lexicalResult.Add(tokenResult);

            if(tokenResult.Token == Token.LexicalError)
            {
                Close();

                throw new Exception($"Lexical Error: {tokenResult}");
            }

            return tokenResult;
        }

        public void Close()
        {
            _outputStream.Close();
            _inputStream.Close();
        }

        private void CleanLexical() =>
            _lexical = string.Empty;

        private void UnreadChar() =>
            _currentColumn--;

        private void BackSpace() =>
            _lexical = _lexical.Substring(0, _lexical.Length - 1);

        private bool IsConstantCharValid()
        {
            var lexicalSize = _lexical.Length;

            var hasOKSize = (lexicalSize == 4 && _lexical[1] == '\\') || lexicalSize == 3;

            return _lexical.First() == '\'' && hasOKSize;
        }
    }
}
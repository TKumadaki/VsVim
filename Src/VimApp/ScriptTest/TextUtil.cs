using Microsoft.FSharp.Core;
using Microsoft.VisualStudio.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vim.Extensions;

namespace VimApp.ScriptTest
{
    public enum TextDirection
    {
        Neither,
        Left,
        Right,
    }

    public enum WordKind
    {
        NormalWord,
        BigWord,
    }

    public class SearchPath
    {
        public static SearchPath Forward { get; } = new SearchPath();
        public static SearchPath Backward { get; } = new SearchPath();

        public bool IsSearchPathForward()
        {
            return object.ReferenceEquals(this, Forward) ? true : false;
        }

        public bool IsSearchPathBackward()
        {
            return !IsSearchPathForward();
        }

        public static SearchPath Reverse(SearchPath path)
        {
            if (object.ReferenceEquals(path, Forward))
            {
                return Backward;
            }
            else
            {
                return Forward;
            }
        }

        public static SearchPath Create(bool isForward)
        {
            if (isForward)
            {
                return Forward;
            }
            else
            {
                return Backward;
            }
        }
    }

    public class TextUtil
    {
        public static bool IsBigWordChar(Char c)
        {
            return !Char.IsWhiteSpace(c);
        }
        public static bool IsNormalWordChar(Char c)
        {
            return Char.IsLetterOrDigit(c) || c == '_';
        }
        public static bool IsNormalWordOtherChar(Char c)
        {
            return !IsNormalWordChar(c) && !Char.IsWhiteSpace(c);
        }
        public static bool IsWordChar(WordKind kind, Char c)
        {
            if (kind == WordKind.BigWord)
            {
                return IsBigWordChar(c);
            }
            else
            {
                return IsNormalWordChar(c);
            }
        }

        public static List<Span> GetWordSpans(WordKind kind, SearchPath path, string input)
        {
            //これが単語の始まりであるかどうか、
            //および単語の残りの部分と一致する述語を返すかどうかを判断する機能
            Func<char, FSharpOption<Func<char, bool>>> isWordStart = (c) =>
            {
                if (kind == WordKind.NormalWord)
                {
                    if (IsNormalWordChar(c))
                    {
                        return FSharpOption.Create<Func<char, bool>>(IsNormalWordChar);
                    }
                    else if (IsNormalWordOtherChar(c))
                    {
                        return FSharpOption.Create<Func<char, bool>>(IsNormalWordOtherChar);
                    }
                    else
                    {
                        return FSharpOption<Func<char, bool>>.None;
                    }
                }
                else
                {
                    if (IsBigWordChar(c))
                    {
                        return FSharpOption.Create<Func<char, bool>>(IsBigWordChar);
                    }
                    else
                    {
                        return FSharpOption<Func<char, bool>>.None;
                    }
                }
            };

            //行に単語を入れるためのシーケンスを作成する
            var limit = StringUtil.Length(input) - 1;

            Func<int, FSharpOption<Tuple<Span, int>>> getWord = null;
            getWord = (index) =>
            {
                if (index <= limit)
                {
                    var result = isWordStart(input[index]);
                    if (result.IsSome())
                    {
                        Func<int, int> inner = null;
                        inner = (idx) =>
                        {
                            if ((idx > limit) || !result.Value(input[idx]))
                            {
                                return idx;
                            }
                            else
                            {
                                return inner(idx + 1);
                            }
                        };
                        var endIndex = inner(index + 1);
                        return FSharpOption.Create<Tuple<Span, int>>(Tuple.Create(Span.FromBounds(index, endIndex), endIndex));
                    }
                    else
                    {
                        return getWord(index + 1);
                    }
                }
                else
                {
                    return FSharpOption<Tuple<Span, int>>.None;
                }
            };

            var wordsForward = Unfold.Do(0, getWord);
            if(path.IsSearchPathForward())
            {
                return wordsForward.ToList();
            }
            else
            {
                return wordsForward.Reverse().ToList();
            }
        }
    }
    public static class Unfold
    {
        public static IEnumerable<T> Do<S, T>(this S state, Func<S, FSharpOption<Tuple<T, S>>> generator)
        {
            var newState = generator(state);
            while (newState.IsSome())
            {
                yield return newState.Value.Item1;
                newState = generator(newState.Value.Item2);
            }
        }
    }
}

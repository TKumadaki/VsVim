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

        //public static List<Span> GetWordSpans(WordKind kind, SearchPath path, string input)
        //{
        //    //これが単語の始まりであるかどうか、
        //    //および単語の残りの部分と一致する述語を返すかどうかを判断する機能
        //    Func<char, FSharpOption<Func<char, bool>>> isWordStart = (c) =>
        //    {
        //        if (kind == WordKind.NormalWord)
        //        {
        //            if (IsNormalWordChar(c))
        //            {
        //                return FSharpOption.Create<Func<char, bool>>(IsNormalWordChar);
        //            }
        //            else if (IsNormalWordOtherChar(c))
        //            {
        //                return FSharpOption.Create<Func<char, bool>>(IsNormalWordOtherChar);
        //            }
        //            else
        //            {
        //                return FSharpOption<Func<char, bool>>.None;
        //            }
        //        }
        //        else
        //        {
        //            if (IsBigWordChar(c))
        //            {
        //                return FSharpOption.Create<Func<char, bool>>(IsBigWordChar);
        //            }
        //            else
        //            {
        //                return FSharpOption<Func<char, bool>>.None;
        //            }
        //        }
        //    };

        //    //行に単語を入れるためのシーケンスを作成する
        //    var limit = StringUtil.Length(input) - 1;
        //    Func<List<Span>> wordsForward;

        //}
    }
}

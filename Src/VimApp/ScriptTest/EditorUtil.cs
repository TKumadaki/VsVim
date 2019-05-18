using Microsoft.VisualStudio.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VimApp.ScriptTest
{
    public enum CodePointInfo
    {
        SimpleCharacter,
        SurrogatePairHighCharacter,
        SurrogatePairLowCharacter,
        BrokenSurrogatePair,
        EndPoint,
    }

    public class EditorUtil
    {

    }
    public class EditorCoreUtil
    {
        public static bool IsEndPoint(SnapshotPoint point)
        {
            return point.Position == point.Snapshot.Length;
        }

        public static SnapshotPoint AddOneOrCurrent(SnapshotPoint point)
        {
            if (IsEndPoint(point))
            {
                return point;
            }
            else
            {
                return point.Add(1);
            }
        }
        public static SnapshotPoint SubtractOneOrCurrent(SnapshotPoint point)
        {
            if (point.Position == 0)
            {
                return point;
            }
            else
            {
                return point.Subtract(1);
            }
        }

        public static CodePointInfo GetCodePointInfo(SnapshotPoint point)
        {
            if (IsEndPoint(point))
                return CodePointInfo.EndPoint;

            var c = point.GetChar();
            if (Char.IsHighSurrogate(c))
            {
                var nextPoint = point.Add(1);
                if (!IsEndPoint(nextPoint) && Char.IsLowSurrogate(nextPoint.GetChar()))
                {
                    return CodePointInfo.SurrogatePairHighCharacter;
                }
                else
                {
                    return CodePointInfo.BrokenSurrogatePair;
                }
            }
            else if (Char.IsLowSurrogate(c))
            {
                if (point.Position == 0)
                {
                    return CodePointInfo.BrokenSurrogatePair;
                }
                else
                {
                    var previousPoint = point.Subtract(1);
                    if (Char.IsHighSurrogate(previousPoint.GetChar()))
                    {
                        return CodePointInfo.SurrogatePairLowCharacter;
                    }
                    else
                    {
                        return CodePointInfo.BrokenSurrogatePair;
                    }
                }
            }
            else
            {
                return CodePointInfo.SimpleCharacter;
            }
        }

        public static bool IsInsideLineBreak(SnapshotPoint point, ITextSnapshotLine line)
        {
            return point.Position >= line.End.Position && !IsEndPoint(point);
        }

        /// <summary>
        /// ポイントが指す文字の範囲を取得します。 
        /// 通常これは簡単な操作です。 
        /// 唯一の難点は、ポイントが行末にある場合（この場合、スパンが改行をカバーする場合）、
        /// または文字がサロゲートペアの一部である場合です。
        /// </summary>
        /// <param name="line"></param>
        /// <param name="point"></param>
        public static SnapshotSpan GetCharacterSpan(ITextSnapshotLine line, SnapshotPoint point)
        {
            //その点が線に属することを要求します。
            Contract.Requires(point.Snapshot == line.Snapshot);
            Contract.Requires(point.Position >= line.Start.Position);
            Contract.Requires(point.Position <= line.EndIncludingLineBreak.Position);

            if (IsInsideLineBreak(point, line))
            {
                return new SnapshotSpan(line.End, line.EndIncludingLineBreak);
            }
            else
            {
                switch (GetCodePointInfo(point))
                {
                    case CodePointInfo.EndPoint:
                        return new SnapshotSpan(point, 0);
                    case CodePointInfo.SurrogatePairHighCharacter:
                        return new SnapshotSpan(point, 2);
                    case CodePointInfo.SurrogatePairLowCharacter:
                        return new SnapshotSpan(point.Subtract(1), 2);
                    default:
                        return new SnapshotSpan(point, 1);
                }
            }
        }
        /// <summary>
        /// 複数の行があり、スナップショットの最後の行（改行がない）が空の場合、
        /// スナップショットは改行で終了します。
        /// </summary>
        /// <param name="snapshot"></param>
        /// <returns></returns>
        public static bool EndsWithLineBreak(ITextSnapshot snapshot)
        {
            var lineNumber = snapshot.LineCount - 1;
            return (lineNumber > 0 && snapshot.GetLineFromLineNumber(lineNumber).Length == 0);
        }
    }
}

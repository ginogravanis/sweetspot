using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace SweetspotApp.Util
{
    public static class SplitString
    {

        public static List<string> GetFirstLine(List<string> words, int lineSize, SpriteFont font)
        {
            int maxLength = 0;
            for (int wordCount = 1; wordCount <= words.Count; wordCount++)
            {
                string line = String.Join(" ", words.GetRange(0, wordCount));
                if (font.MeasureString(line).X <= lineSize)
                    maxLength = wordCount;
                else
                    break;
            }
            return words.GetRange(0, maxLength);
        }

        public static List<string> SplitRows(String text, int lineSize, SpriteFont font)
        {
            List<string> words = new List<string>(
                 text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                 );

            List<string> lines = new List<string>();

            while (words.Count > 0)
            {
                var firstLine = GetFirstLine(words, lineSize, font);
                lines.Add(String.Join(" ", firstLine));
                words = words.GetRange(firstLine.Count, words.Count - firstLine.Count);
            }

            return lines;
        }
    }
}

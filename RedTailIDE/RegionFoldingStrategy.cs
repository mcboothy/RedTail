using System;
using System.Collections.Generic;
using System.Text;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;

namespace RedTailIDE
{
    public class RegionFoldingStrategy : AbstractFoldingStrategy
    {
        public override IEnumerable<NewFolding> CreateNewFoldings(TextDocument document, out int firstErrorOffset)
        {
            firstErrorOffset = -1;

            return CreateNewFoldings(document);
        }

        private IEnumerable<NewFolding> CreateNewFoldings(TextDocument document)
        {
            var newFoldings = new List<NewFolding>();

            var startOffsets = new Stack<int>();
            var titles = new Stack<string>();
            var lastNewLineOffset = 0;
            for (var i = 0; i < document.TextLength; i++)
            {
                var c = document.GetCharAt(i);
                if (c == '#')
                {
                    // see if the next word is 'region', or 'endregion'
                    var directive = new StringBuilder();

                    var j = i + 1;
                    while (j < document.TextLength && !Char.IsWhiteSpace(document.GetCharAt(j)))
                    {
                        directive.Append(document.GetCharAt(j++));
                    }

                    if (directive.ToString().Equals("region"))
                    {

                        j++;

                        var nameBuilder = new StringBuilder();
                        while (j < document.TextLength && !Char.IsWhiteSpace(document.GetCharAt(j)))
                        {
                            nameBuilder.Append(document.GetCharAt(j++));
                        }

                        titles.Push(nameBuilder.ToString());

                        // go to next newline (if there is one), otherwise we don't start an offset
                        while(j < document.TextLength)
                        {
                            c = document.GetCharAt(j++);
                            if (c != '\n' && c != '\r') continue;

                            lastNewLineOffset = j;
                            startOffsets.Push(i);
                            break;
                        }
                    }
                    else if (directive.ToString().Equals("endregion") && startOffsets.Count > 0)
                    {
                        var startOffset = startOffsets.Pop();
                        var name = titles.Pop();

                        // don't fold if opening and closing brace are on the same line
                        if (startOffset < lastNewLineOffset)
                        {
                            newFoldings.Add(new NewFolding(startOffset, j) {Name = name} );
                        }
                    }

                    i = j;
                }
                else if (c == '\n' || c == '\r')
                {
                    lastNewLineOffset = i + 1;
                }

            }

            lastNewLineOffset = 0;
            const char openingBrace = '{';
            const char closingBrace = '}';
            for (var i = 0; i < document.TextLength; i++)
            {
                var c = document.GetCharAt(i);
                if (c == openingBrace)
                {
                    startOffsets.Push(i);
                }
                else if (c == closingBrace && startOffsets.Count > 0)
                {
                    var startOffset = startOffsets.Pop();
                    // don't fold if opening and closing brace are on the same line
                    if (startOffset < lastNewLineOffset)
                    {
                        newFoldings.Add(new NewFolding(startOffset, i + 1));
                    }
                }
                else if (c == '\n' || c == '\r')
                {
                    lastNewLineOffset = i + 1;
                }
            }

            newFoldings.Sort((a, b) => a.StartOffset.CompareTo(b.StartOffset));
            return newFoldings;
        }
    }
}

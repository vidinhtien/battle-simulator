using System.Collections.Generic;

namespace ZeroX.DataTableSystem.Editors
{
    internal static class CsvUtility
    {
        private static readonly char lineSeparator = '\n';
        public static List<List<string>> ImportFromCSV(string csvText, char fieldSeparator, char quoteCharacter, bool trimField)
        {
            csvText = csvText.Replace("\r\n", "\n");
            List<List<string>> listRow = new List<List<string>>();
            int startIndex = 0;
            while (true)
            {
                List<string> listCell = new List<string>();
                while (true)
                {
                    if(startIndex >= csvText.Length)
                        break;
                    int endIndex;
                    char startChar = csvText[startIndex];
                    string cellValue = null;
                    if (startChar != quoteCharacter)
                    {
                        endIndex = FindEndIndexNormal(ref csvText, startIndex, fieldSeparator);
                        cellValue = csvText.Substring(startIndex, endIndex - startIndex);
                        cellValue = cellValue.Replace("\"\"", "\"");
                        //cellValue = RemoveEndOfLine(cellValue);
                        
                        if (trimField)
                            cellValue = cellValue.Trim();
                    }
                    else
                    {
                        startIndex++;
                        endIndex = FindEndIndexQuote(ref csvText, startIndex, fieldSeparator, quoteCharacter);
                        cellValue = csvText.Substring(startIndex, endIndex - startIndex);
                        cellValue = cellValue.Replace("\"\"", "\"");
                        //cellValue = RemoveEndOfLine(cellValue);
                        //cellValue = cellValue.Remove(cellValue.Length - 1); //Remove end quote character
                        if (trimField)
                            cellValue = cellValue.Trim();

                        endIndex++; //Để cho qua quoteCharacter. Vì trong trường hợp startChar thì quoteCharacter thì ký tự cuối chắc chắn cùng phải là quoteCharacter
                    }

                    listCell.Add(cellValue);

                    if(endIndex == csvText.Length)
                    {
                        startIndex = endIndex;
                        break;
                    }
                    
                    if(csvText[endIndex] == lineSeparator)
                    {
                        startIndex = endIndex + 1;
                        break;
                    }

                    startIndex = endIndex + 1;
                }
                
                listRow.Add(listCell);
                
                if(startIndex >= csvText.Length)
                    break;
            }

            return listRow;
        }

        static int FindEndIndexNormal(ref string csvText, int startIndex, char fieldSeparator)
        {
            for (int i = startIndex; i < csvText.Length; i++)
            {
                char c = csvText[i];
                if (c == fieldSeparator || c == lineSeparator)
                {
                    return i;
                }
            }

            return csvText.Length;
        }

        static int FindEndIndexQuote(ref string csvText, int startIndex, char fieldSeparator, char quoteCharacter)
        {
            for (int i = startIndex; i < csvText.Length; i++)
            {
                char c = csvText[i];
                if (c == quoteCharacter)
                {
                    int numberQuoteCharacter = CountPreviousQuoteCharacter(ref csvText, i, quoteCharacter, startIndex);
                    if (numberQuoteCharacter % 2 == 0)
                        return i;
                }
            }

            return csvText.Length;
        }

        /// <summary>
        /// Đếm từ i-1 đổ lại. Ko tính char tại i
        /// </summary>
        static int CountPreviousQuoteCharacter(ref string csvText, int index, char quoteCharacter, int previousLimitIndex)
        {
            if (index < csvText.Length - 1 && csvText[index + 1] == quoteCharacter) //Nếu ký tự tiếp theo mà là quoteCharacter thì ko cần đếm 
                return -1;
            
            int total = 0;
            for (int i = index - 1; i >= previousLimitIndex; i--)
            {
                if (csvText[i] == quoteCharacter)
                    total++;
                else
                    break;
            }
            
            return total;
        }

        static string RemoveEndOfLine(string text)
        {
            if (text == null)
                return null;

            if (text.Length == 1)
            {
                if (text[0] == '\n' || text[0] == '\r')
                    return "";
            }

            if (text[text.Length - 1] == '\n')
            {
                if (text[text.Length - 2] == '\r')
                    text = text.Substring(0, text.Length - 2);
                else
                    text = text.Substring(0, text.Length - 1);

                return text;
            }

            if (text[text.Length - 1] == '\r')
            {
                if (text[text.Length - 2] == '\n')
                    text = text.Substring(0, text.Length - 2);
                else
                    text = text.Substring(0, text.Length - 1);

                return text;
            }

            return text;
        }
    }
}
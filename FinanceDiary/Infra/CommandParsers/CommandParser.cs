using System.Collections.Generic;

namespace FinanceDiary.Infra.CommandParsers
{
    public class CommandParser : ICommandParser
    {
        public string[] Parse(string input)
        {
            List<string> commandResult = new List<string>();

            string[] splittedByHyphen = input.Split("-");
            foreach (string str in splittedByHyphen)
            {
                if (!str.Contains("\""))
                {
                    commandResult.AddRange(str.Split(" "));
                    continue;
                }

                ParseStringWithQuotationMarks(commandResult, str);
            }

            commandResult.RemoveAll(str => str.Length == 0);
            
            return CreateWithHyphens(commandResult);
        }

        private void ParseStringWithQuotationMarks(List<string> commandResult, string input)
        {
            string[] splittedByQuotationMark = input.Split("\"");
            foreach (string str2 in splittedByQuotationMark)
            {
                commandResult.Add(str2.Trim('\"'));
            }
        }

        private string[] CreateWithHyphens(List<string> commandResult)
        {
            string[] withHyphens = new string[commandResult.Count];

            for (int i = 0; i < commandResult.Count; ++i)
            {
                string str = commandResult[i].Trim();
                if (str.Length == 1 && char.IsLetter(str[0]))
                {
                    withHyphens[i] = $"-{str}";
                    continue;
                }

                withHyphens[i] = str;
            }

            return withHyphens;
        }
    }
}
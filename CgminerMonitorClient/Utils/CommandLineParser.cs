using System.Collections.Generic;
using Newtonsoft.Json.Serialization;

namespace CgminerMonitorClient.Utils
{
    public static class CommandLineParser
    {
        public static void SplitCommandLine(string commandLine, out string command, out string arguments)
        {
            var inQuotes = false;
            var result = Split(commandLine, c =>
            {
                if (c == '\"')
                    inQuotes = !inQuotes;

                return !inQuotes && c == ' ';
            });

            command = arguments = null;
            var first = true;
            foreach (var part in result)
            {
                if (first)
                {
                    // ReSharper disable RedundantAssignment
                    command = part;
                    first = false;
                }
                else
                {
                    arguments = part;
                    // ReSharper restore RedundantAssignment
                }
            }
        }

        public static IEnumerable<string> Split(string str, Func<char, bool> controller)
        {
            int nextPiece = 0;

            for (int c = 0; c < str.Length; c++)
            {
                if (controller(str[c]))
                {
                    yield return str.Substring(nextPiece, c - nextPiece);
                    nextPiece = c + 1;
                    break;
                }
            }

            yield return str.Substring(nextPiece);
        }
    }
}
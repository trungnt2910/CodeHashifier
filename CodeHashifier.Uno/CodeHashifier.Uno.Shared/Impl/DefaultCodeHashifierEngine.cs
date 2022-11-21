using CodeHashfier.Uno.Helpers;
using CodeHashifier.Uno.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CodeHashifier.Uno.Impl
{
    class DefaultCodeHashifierEngine : CodeHashifierEngine
    {
        public override string Hash(string source)
        {
            var sourceFile = new StringReader(source);
            var outputFile = new StringWriter();

            string line;
            bool isMultiLineComment = false;

            // The scope "stack". Affected when #ifdefs are used.
            var hashes = new Stack<Dictionary<string, string>>();
            hashes.Push(new Dictionary<string, string>());

            var defines = new List<string>();
            var code = new List<string>();

            while ((line = sourceFile.ReadLine()) != null)
            {
                var stringContextIdentifier = GetStringContextVector(line);
                var characterContextIdentifier = GetCharLiteralContextVector(line, stringContextIdentifier);

                //Remove evil multiline comments
                if (isMultiLineComment)
                {
                    int multiLineCommentEnd = line.IndexOf("*/");
                    if (multiLineCommentEnd != -1)
                    {
                        isMultiLineComment = false;
                        line = line.Substring(multiLineCommentEnd + 2);
                    }
                    else continue;
                }

                if (!isMultiLineComment)
                {
                    int multiLineCommentBegin = line.IndexOf("/*");
                    if ((multiLineCommentBegin != -1) && (!stringContextIdentifier[multiLineCommentBegin]))
                    {
                        isMultiLineComment = true;
                        int multiLineCommentEnd = line.IndexOf("*/");
                        if (multiLineCommentEnd != -1)
                        {
                            isMultiLineComment = false;
                            line.Remove(multiLineCommentBegin, multiLineCommentEnd + 2 - multiLineCommentBegin);
                        }
                        else
                        {
                            line = line.Substring(0, multiLineCommentBegin);
                        }
                    }
                }

                //Remove comments:
                int inlineCommentBegin = line.IndexOf("//");
                if ((inlineCommentBegin != -1) && (!stringContextIdentifier[inlineCommentBegin]))
                {
                    line = line.Substring(0, inlineCommentBegin);
                }

                //Ignore empty lines after comments stripped:
                if (string.IsNullOrWhiteSpace(line)) continue;

                line = line.Trim();

                //Ignore macros
                if (line[0] == '#')
                {
                    //Flush our #defines first else they may affect our macros
                    //And also flush our code else our other macros may affect it

                    FlushCode(outputFile, defines, code);

                    outputFile.WriteLine(line);

                    //#if preprocessor macros produces child scopes which may break previous hashified builds
                    //#if, #ifdef, #ifndef
                    if (line.Substring(0, 3) == "#if")
                    {
                        hashes.Push(new Dictionary<string, string>());
                    }
                    //#else, #elif
                    else if (line.Substring(0, 3) == "#el")
                    {
                        hashes.Pop();
                        hashes.Push(new Dictionary<string, string>());
                    }
                    else if (line.Substring(0, 6) == "#endif")
                    {
                        hashes.Pop();
                    }

                    continue;
                }

                line = line.Trim();

                var lineStream = new WordReader(line);
                string currentWord;

                while ((currentWord = lineStream.ReadWord()) != null)
                {

                    //Eat the whole line if it contains a string or a character literal
                    if ((currentWord.IndexOf('\"') != -1) || (currentWord.IndexOf('\'') != -1))
                    {
                        int startpos = line.IndexOf(currentWord);

                        while ((currentWord = lineStream.ReadWord()) != null) continue;

                        currentWord = line.Substring(startpos);
                    }

                    string token = null;

                    foreach (var hash in hashes)
                    {
                        if (hash.TryGetValue(currentWord, out token))
                        {
                            break;
                        }
                    }

                    if (token != null)
                    {
                        code.Add(token);
                    }
                    else
                    {
                        token = Random.Shared.NextAlphaNumericString();
                        hashes.Peek().Add(currentWord, token);
                        defines.Add($"#define {token} {currentWord}");
                        code.Add(token);
                    }
                }

                lineStream.Dispose();
            }

            //Flush defines the last time:
            FlushCode(outputFile, defines, code);

            hashes.Pop();

            string resultString = outputFile.ToString().Trim();

            sourceFile.Dispose();
            outputFile.Dispose();

            return resultString;
        }

        private static bool[] GetStringContextVector(string str)
        {
            bool isStringContext = false;
            bool isEscape = false;
            var result = new bool[str.Length];

            for (int i = 0; i < str.Length; ++i)
            {
                if ((!isEscape) && (str[i] == '\\')) isEscape = true;

                if ((!isEscape) && (str[i] == '\"')) isStringContext = !isStringContext;
                result[i] = isStringContext;

                if (isEscape) isEscape = false;
            }

            return result;
        }

        private static bool[] GetCharLiteralContextVector(string str, bool[] stringContextVector)
        {
            bool isCharLiteralContext = false;
            bool isEscape = false;
            var result = new bool[str.Length];

            for (int i = 0; i < str.Length; ++i)
            {
                if (stringContextVector[i])
                {
                    isCharLiteralContext = false;
                    result[i] = false;
                    continue;
                }

                if ((!isEscape) && (str[i] == '\\')) isEscape = true;

                if ((!isEscape) && (str[i] == '\'')) isCharLiteralContext = !isCharLiteralContext;
                result[i] = isCharLiteralContext;

                if (isEscape) isEscape = false;
            }

            return result;
        }

        private static void FlushCode(TextWriter os, List<string> defines, List<string> code)
        {
            //Refresh hashesPerLine each time this program flushes to increase randomness.
            int hashesPerLine = Random.Shared.Next() % 12 + 4;

            if (defines.Count != 0)
            {
                Random.Shared.ShuffleList(defines);

                while (defines.Count != 0)
                {
                    os.WriteLine(defines.Last());
                    defines.RemoveAt(defines.Count - 1);
                }
            }

            if (code.Count != 0)
            {
                for (int i = 0; i < code.Count; ++i)
                {
                    int j = i;
                    for (; j < Math.Min(i + hashesPerLine, code.Count); ++j)
                    {
                        os.Write($"{code[j]} ");
                    }
                    os.WriteLine();
                    i = j - 1;
                }
                code.Clear();
            }
        }
    }
}

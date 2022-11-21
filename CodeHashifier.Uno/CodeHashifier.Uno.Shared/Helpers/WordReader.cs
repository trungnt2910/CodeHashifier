using System.Collections.Generic;
using System.IO;

namespace CodeHashfier.Uno.Helpers
{
    class WordReader : StringReader
    {
        private readonly Queue<char> _currentLine = new();
        public WordReader(string s) : base(s)
        {

        }

        public string ReadWord()
        {
            if (_currentLine.Count == 0) ReloadQueue();
            if (_currentLine.Count != 0)
            {
                var result = new List<char>();
                while (_currentLine.Count != 0 && char.IsWhiteSpace(_currentLine.Peek())) _currentLine.Dequeue();
                while (_currentLine.Count != 0 && (!char.IsWhiteSpace(_currentLine.Peek()))) result.Add(_currentLine.Dequeue());
                return new string(result.ToArray());
            }
            else return null;
        }

        public override string ReadLine()
        {
            if (_currentLine.Count != 0)
            {
                string s = new string(_currentLine.ToArray());
                _currentLine.Clear();
                return s;
            }
            else return base.ReadLine();
        }

        private void ReloadQueue()
        {
            var line = base.ReadLine();
            if (line != null)
            {
                foreach (char ch in line)
                {
                    _currentLine.Enqueue(ch);
                }
            }
        }
    }
}
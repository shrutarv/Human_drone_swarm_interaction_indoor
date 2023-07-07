using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Helper
{
    public class EventHistory
    {
        private SortedDictionary<int, List<string>> history = new SortedDictionary<int, List<string>>();

        public void Add(int clock, string text) {
            if (!history.ContainsKey(clock)) {
                history.Add(clock, new List<string>());
            }
            history[clock].Add(text);
        }

        public string Text() {
            return history.Aggregate(new StringBuilder(),
            (sb, kvp) => sb.AppendFormat("{0}:\n     {1}\n", kvp.Key, string.Join("\n     ", kvp.Value)),
            sb => sb.ToString());
        }

        public string TextReversedTime() {
            return history.Reverse().Aggregate(new StringBuilder(),
            (sb, kvp) => sb.AppendFormat("{0}:\n     {1}\n", kvp.Key, string.Join("\n     ", kvp.Value)),
            sb => sb.ToString());
        }


    }
}

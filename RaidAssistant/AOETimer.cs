using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RaidAssistant
{
    public class AOETimer
    {
        public string Name { get; set; }
        public int Seconds { get; set; }

        private Regex regex;

        public AOETimer(string name, int seconds, string regex)
        {
            string prefix = "\\((?<timestamp>\\d*)\\)\\[(?<date>[^\\]].*)\\]";
            this.regex = new Regex(prefix + regex, RegexOptions.Compiled);
        }

        public bool Test(ref string line, out Dictionary<string, string> matches, out int timer)
        {
            timer = -1;
            matches = null;

            Match match = regex.Match(line);

            if (match.Success)
            {
                matches = new Dictionary<string, string>();

                for (var i = 0; i < match.Groups.Count; i++)
                {
                    matches.Add(regex.GroupNameFromNumber(i), match.Groups[i].Value);
                }

                System.Diagnostics.Debug.WriteLine("Matched " + Name);

                return true;
            }

            return false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Switch
{
    class ProxyJudge
    {
        public Dictionary<string, string> parse(string result)
        {
            Dictionary<string, string> values = new Dictionary<string, string>();

            foreach (Match match in Regex.Matches(result, @"[A-Z_]+ = [^\n]+"))
            {
                string[] info = Regex.Split(match.Value, " = ");
                values.Add(info[0], info[1]);
            }

            return values;
        }
    }
}

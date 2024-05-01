using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tesseract;
using CricketExt.DataTypes;
using static CricketExt.Analyzer.ProcessUtil;
using System.Text.RegularExpressions;

namespace CricketExt.Analyzer {
    //Gathers extracted data from ScoreParser and constructs a scoreDictionary record of scores for each ball.
    internal class ScoreGatherer {
        static ConcurrentDictionary<String, Ball> scoreDictionary = new();
        private readonly Regex scoreRegex = new(@"([0-9]+)/([0-9]+)");
        public ScoreGatherer() { }
        public async Task<int> Gather(int outs, int runs, String team, String batter1, String batter2, String bowler, String score) {
            String key = GenTurnString(team, outs, runs);
            team = RemoveNewLine(team);
            batter1 = RemoveNewLine(batter1);
            batter2 = RemoveNewLine(batter2);
            bowler = RemoveNewLine(bowler);
            Match match = scoreRegex.Match(score);
            int.TryParse(match.Groups[1].Value, out int totalRuns);
            int.TryParse(match.Groups[2].Value, out int totalWickets);
            scoreDictionary.TryAdd(key, new Ball());//Do we check this or is this unnecessary?
            scoreDictionary[key] = new Ball(outs, runs, team, bowler, batter1, batter2, totalRuns, totalWickets);
            Debug.WriteLine($"Added {scoreDictionary[key]}");
            return 2;
        }

        //Removes new lines (\n) from strings.
        private String RemoveNewLine(String str) {
            return str.Replace("\n", String.Empty);
        }
    }
}

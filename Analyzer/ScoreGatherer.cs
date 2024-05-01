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
        private String team1 = String.Empty, team2 = String.Empty;
        public ScoreGatherer() { }
        public int Gather(int overs, int balls, String team, String batter1, String batter2, String bowler, String score) {
            String key = GenTurnString(team, overs, balls);
            team = RemoveNewLine(team);
            if (team1.Equals(String.Empty)) team1 = team;
            else if (team2.Equals(String.Empty) && !team.Equals(team1)) team2 = team;
            batter1 = RemoveNewLine(batter1);
            batter2 = RemoveNewLine(batter2);
            bowler = RemoveNewLine(bowler);
            Match match = scoreRegex.Match(score);
            int.TryParse(match.Groups[1].Value, out int totalRuns);
            int.TryParse(match.Groups[2].Value, out int totalWickets);
            scoreDictionary.TryAdd(key, new Ball());//Do we check this or is this unnecessary?
            scoreDictionary[key] = new Ball(overs, balls, team, bowler, batter1, batter2, totalRuns, totalWickets);
            Debug.WriteLine($"Added {scoreDictionary[key]}");
            return 0;
        }

        
        public void postProcess() {
            List<Ball> balls = scoreDictionary.Values.ToList<Ball>();
            balls.Sort(delegate(Ball x, Ball y) {
                if (x.BattingTeam.Equals(y.BattingTeam)) {
                    if (x.Overs == y.Overs)
                        return x.Balls.CompareTo(y.Balls);
                    else return x.Overs.CompareTo(y.Overs);
                } else if (x.BattingTeam.Equals(team1)) return -1; 
                else if (x.BattingTeam.Equals(team2)) return 1;
                else return 0;
            });

            List<Ball> processedList = new();
            for (int i = 0; i < balls.Count; i++) {
                Ball ball = balls[i];
                String bowlingTeam;
                if (ball.BattingTeam.Equals(team1)) bowlingTeam = team2;
                else bowlingTeam = team1;

                int totalRuns, totalWickets, runs, wickets;
                if (i + 1 < balls.Count && balls[i + 1].BattingTeam.Equals(ball.BattingTeam)) {
                    totalRuns = balls[i + 1].TotalRuns;
                    totalWickets = balls[i + 1].TotalWickets;
                    runs = totalRuns - ball.TotalRuns;
                    wickets = totalWickets - ball.TotalWickets;
                    processedList.Add(new Ball(ball.Overs, ball.Balls, ball.BattingTeam, ball.Bowler, ball.Batter1, ball.Batter2, totalRuns, totalWickets, bowlingTeam, runs, wickets));
                } 
            }
            foreach (Ball b in processedList){
                Debug.WriteLine(b);
            }
        }

        //Removes new lines (\n) from strings.
        private String RemoveNewLine(String str) {
            return str.Replace("\n", String.Empty);
        }
    }
}

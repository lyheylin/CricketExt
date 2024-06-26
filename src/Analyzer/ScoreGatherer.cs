﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tesseract;
using static CricketExt.Analyzer.ProcessUtil;
using System.Text.RegularExpressions;
using CricketExt.DataTypes;

namespace CricketExt.Analyzer {
    /// <summary>
    /// Gathers extracted data from ScoreParser and constructs a scoreDictionary record of scores for each ball.
    /// </summary>
    internal class ScoreGatherer {
        private static ConcurrentDictionary<string, Ball> scoreDictionary = new();
        private readonly Regex scoreRegex = new(@"([0-9]+)/([0-9]+)");
        private string team1 = string.Empty, team2 = string.Empty;
        public ScoreGatherer() { }
        public int Gather(int overs, int balls, string team, string batter1, string batter2, string bowler, string score) {
            string key = GenTurnString(team, overs, balls, score);
            if (team1.Equals(string.Empty)) team1 = team;
            else if (team2.Equals(string.Empty) && !team.Equals(team1)) team2 = team;
            Match match = scoreRegex.Match(score);
            int.TryParse(match.Groups[1].Value, out int totalRuns);
            int.TryParse(match.Groups[2].Value, out int totalWickets);
            scoreDictionary[key] = new Ball(overs, balls, team, bowler, batter1, batter2, totalRuns, totalWickets);
            Debug.WriteLine($"Parsed {team}: {overs}.{balls}");
            return 0;
        }

        /// <summary>
        /// /// <summary>
        /// Populate data and return the proccessed data.
        /// <param name="header">Option to print header.</param>
        /// <returns>Header of </returns>
        public string[] PostProcess(bool header) {
            List<Ball> balls = scoreDictionary.Values.ToList();
            balls.Sort(delegate (Ball x, Ball y)
            {
                if (x.BattingTeam.Equals(y.BattingTeam))
                {
                    if (x.Overs == y.Overs) {
                        if(x.Balls == y.Balls) {
                            return (x.TotalRuns + x.TotalWickets).CompareTo(y.TotalRuns + y.TotalWickets);
                        } else return x.Balls.CompareTo(y.Balls);
                    }
                    else return x.Overs.CompareTo(y.Overs);
                }
                else if (x.BattingTeam.Equals(team1)) return -1;
                else if (x.BattingTeam.Equals(team2)) return 1;
                else return 0;
            });

            List<Ball> processedList = new();
            for (int i = 0; i < balls.Count; i++) {
                Ball ball = balls[i];
                string bowlingTeam;
                if (ball.BattingTeam.Equals(team1)) bowlingTeam = team2;
                else bowlingTeam = team1;

                int totalRuns, totalWickets, runs, wickets;
                if (i + 1 < balls.Count && balls[i + 1].BattingTeam.Equals(ball.BattingTeam))
                {
                    totalRuns = balls[i + 1].TotalRuns;
                    totalWickets = balls[i + 1].TotalWickets;
                    runs = totalRuns - ball.TotalRuns;
                    wickets = totalWickets - ball.TotalWickets;
                    processedList.Add(new Ball(ball.Overs, ball.Balls, ball.BattingTeam, ball.Bowler, ball.Batter1, ball.Batter2, totalRuns, totalWickets, bowlingTeam, runs, wickets));
                }
            }

            //Add csv file header.
            List<String> result = header ? GetHeader() : [];

            foreach (Ball b in processedList) {
                result.Add(b.ToString());
                //Debug.WriteLine(b);
            }

            return result.ToArray();
        }

        /// <summary>
        /// Creates header of output file.
        /// </summary>
        /// <returns>Header of outputfile.</returns>
        private List<String> GetHeader() {
            return 
            [
                $"Team 1,{team1}",
                $"Team 2,{team2}",
                string.Empty,
                "Over,Bowling Team,Batting Team,Bowler Name,Batter 1 Name,Batter 2 Name,Result - Runs,Result - Wickets,Total Runs,Total Wickets"
            ];
        }
    }
}

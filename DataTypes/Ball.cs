using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CricketExt.DataTypes {
    internal struct Ball {
        public Ball(int overs, int balls, String battingTeam, String bowler, String batter1, String batter2, int totalRuns, int totalWickets, String bowlingTeam = "", int runs = 0, int wickets = 0) {
            Overs = overs;
            Balls = balls;
            Bowler = bowler;
            BowlingTeam = bowlingTeam;
            BattingTeam = battingTeam;
            Batter1 = batter1;
            Batter2 = batter2;
            Runs = runs;
            Wickets = wickets;
            TotalRuns = totalRuns;
            TotalWickets = totalWickets;
        }

        public int Overs { get; set; }
        public int Balls { get; set; }
        public String BowlingTeam { get; set; }
        public String BattingTeam { get; set; }
        public String Bowler { get; set; }
        public String Batter1 { get; set; }
        public String Batter2 { get; set; }
        public int Runs { get; set; } 
        public int Wickets { get; set; }
        public int TotalRuns { get; set; }
        public int TotalWickets { get; set; }

        public override string ToString() {
            return ($"{Overs}.{Balls}, {BowlingTeam}, {BattingTeam}, {Bowler}, {Batter1}, {Batter2}, {Runs}, {Wickets}, {TotalRuns}, {TotalWickets}");
        }
    }
}

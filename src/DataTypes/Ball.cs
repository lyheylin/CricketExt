using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CricketExt.DataTypes
{
    /// <summary>
    /// Representation of a ball.
    /// </summary>
    /// <param name="overs"></param>
    /// <param name="balls"></param>
    /// <param name="battingTeam"></param>
    /// <param name="bowler"></param>
    /// <param name="batter1"></param>
    /// <param name="batter2"></param>
    /// <param name="totalRuns"></param>
    /// <param name="totalWickets"></param>
    /// <param name="bowlingTeam"></param>
    /// <param name="runs"></param>
    /// <param name="wickets"></param>
    internal struct Ball(int overs, int balls, string battingTeam, string bowler, string batter1, string batter2, int totalRuns, int totalWickets, string bowlingTeam = "", int runs = 0, int wickets = 0) {
        public int Overs { get; set; } = overs;
        public int Balls { get; set; } = balls;
        public string BowlingTeam { get; set; } = bowlingTeam;
        public string BattingTeam { get; set; } = battingTeam;
        public string Bowler { get; set; } = bowler;
        public string Batter1 { get; set; } = batter1;
        public string Batter2 { get; set; } = batter2;
        public int Runs { get; set; } = runs;
        public int Wickets { get; set; } = wickets;
        public int TotalRuns { get; set; } = totalRuns;
        public int TotalWickets { get; set; } = totalWickets;

        public override string ToString() {
            return $"{Overs}.{Balls},{BowlingTeam},{BattingTeam},{Bowler},{Batter1},{Batter2},{Runs},{Wickets},{TotalRuns},{TotalWickets}";
        }
    }
}

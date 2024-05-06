using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CricketExt.DataTypes
{
    /// <summary>
    /// Representation of a ball. The bowling team, and number of runs and wickets may not be known at initialization.
    /// </summary>
    /// <param name="overs">Number of overs so far.</param>
    /// <param name="balls">Number of balls this over.</param>
    /// <param name="battingTeam">The batting team name in AAA format.</param>
    /// <param name="bowler">The name of the bowler.</param>
    /// <param name="batter1">The name of the first (current) batter.</param>
    /// <param name="batter2">The name of the second batter.</param>
    /// <param name="totalRuns">Total runs this game.</param>
    /// <param name="totalWickets">Total wickets this game.</param>
    /// <param name="bowlingTeam">The bowling team name in AAA format.</param>
    /// <param name="runs">Total runs this ball.</param>
    /// <param name="wickets">Total wickets this ball.</param>
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

        /// <summary>
        /// Returns the ball summary as a single line of csv.
        /// </summary>
        /// <returns>Csv string representing the ball.</returns>
        public override string ToString() {
            return $"{Overs}.{Balls},{BowlingTeam},{BattingTeam},{Bowler},{Batter1},{Batter2},{Runs},{Wickets},{TotalRuns},{TotalWickets}";
        }
    }
}

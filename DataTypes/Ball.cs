using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CricketExt.DataTypes {
    internal struct Ball {
        public int Overs { get; set; }
        public int Balls { get; set; }
        public String BowlingTeam { get; set; }
        public String BattingTeam { get; set; }
        public String Batter1 { get; set; }
        public String Batter2 { get; set; }
        public int Runs { get; set; } 
        public int Wickets { get; set; }
        public int TotalRuns { get; set; }
        public int TotalWickets { get; set; }

        public override string ToString() {
            return ($"{Overs}.{Balls}, {BowlingTeam}, {BattingTeam}, {Batter1}, {Batter2}, {Runs}, {Wickets}, {TotalRuns}, {TotalWickets}");
        }
    }
}

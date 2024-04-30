using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tesseract;

namespace CricketExt.Analyzer {
    internal class ScoreGatherer {
        public ScoreGatherer() { }
        public async Task<int> Gather(int outs, int runs, String team, String name, String text, float confidence) {
            Debug.Write($"{name}: {text}");
            return 2;
        }
    }
}

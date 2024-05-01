using CricketExt.util;
using OpenCvSharp;
using System.Diagnostics;
using Tesseract;
using static CricketExt.Analyzer.ProcessUtil;

namespace CricketExt.Analyzer {
    //Takes a frame with a scoreboard identified, extract information from the scoreboard and send result to the gatherer.
    internal class ScoreParser {

        private readonly Mat scoreBoard;
        private readonly int outs, runs;
        private ScoreGatherer scoreGatherer;
        private String team;
        Page? page;
        public ScoreParser(ScoreGatherer scoreGatherer, Mat scoreBoard, String team, int outs, int runs) {
            this.scoreBoard = scoreBoard;
            this.outs = outs;
            this.runs = runs;
            this.scoreGatherer = scoreGatherer;
            this.team = team;
        }

        public async Task<int> Parse() {

            //Debug.WriteLine($"Over: Out {outs}, Run {runs}");
            String batter1, batter2, bowler, score;
            page = ReadTextFromROI(scoreBoard, ROIConsts.BAT_1_X, ROIConsts.BAT_1_Y, ROIConsts.BAT_1_W, ROIConsts.BAT_1_H, true);
            batter1 = page.GetText();
            page.Dispose();

            page = ReadTextFromROI(scoreBoard, ROIConsts.BAT_2_X, ROIConsts.BAT_2_Y, ROIConsts.BAT_2_W, ROIConsts.BAT_2_H, true);
            batter2 = page.GetText();
            page.Dispose();

            page = ReadTextFromROI(scoreBoard, ROIConsts.BOW_X, ROIConsts.BOW_Y, ROIConsts.BOW_W, ROIConsts.BOW_H, true);
            bowler = page.GetText();
            page.Dispose();

            page = ReadTextFromROI(scoreBoard, ROIConsts.SCORE_X, ROIConsts.SCORE_Y, ROIConsts.SCORE_W, ROIConsts.SCORE_H);
            score = page.GetText();
            scoreGatherer.Gather(outs, runs, team, batter1, batter2, bowler, score);
            page.Dispose();

            return 0;
        } 
        
    }
}

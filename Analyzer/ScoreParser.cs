using CricketExt.util;
using OpenCvSharp;
using System.Diagnostics;
using Tesseract;
using static CricketExt.Analyzer.ProcessUtil;

namespace CricketExt.Analyzer {
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

            page = ReadTextFromROI(scoreBoard, ROIConsts.BAT_1_X, ROIConsts.BAT_1_Y, ROIConsts.BAT_1_W, ROIConsts.BAT_1_H, true);
            scoreGatherer.Gather(outs, runs, team, StringConsts.BATTER_1, page.GetText(), page.GetMeanConfidence());
            page.Dispose();

            page = ReadTextFromROI(scoreBoard, ROIConsts.BAT_2_X, ROIConsts.BAT_2_Y, ROIConsts.BAT_2_W, ROIConsts.BAT_2_H, true);
            scoreGatherer.Gather(outs, runs, team, StringConsts.BATTER_2, page.GetText(), page.GetMeanConfidence());
            page.Dispose();

            page = ReadTextFromROI(scoreBoard, ROIConsts.BOW_X, ROIConsts.BOW_Y, ROIConsts.BOW_W, ROIConsts.BOW_H, true);
            scoreGatherer.Gather(outs, runs, team,StringConsts.BOWLER, page.GetText(), page.GetMeanConfidence());
            page.Dispose();

            page = ReadTextFromROI(scoreBoard, ROIConsts.SCORE_X, ROIConsts.SCORE_Y, ROIConsts.SCORE_W, ROIConsts.SCORE_H);
            scoreGatherer.Gather(outs, runs, team, StringConsts.SCORE, page.GetText(), page.GetMeanConfidence());
            page.Dispose();

            return 0;
        } 
        
    }
}

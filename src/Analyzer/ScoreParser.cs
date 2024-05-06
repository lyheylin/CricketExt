using CricketExt.util;
using OpenCvSharp;
using System.Diagnostics;
using Tesseract;
using static CricketExt.Analyzer.ProcessUtil;

namespace CricketExt.Analyzer {
    //Takes a frame with a scoreboard identified, extract information from the scoreboard and send result to the gatherer.
    internal class ScoreParser {

        private readonly Mat scoreBoard;
        private readonly int overs, runs;
        private ScoreGatherer scoreGatherer;
        private string team;
        Page? page;
        public ScoreParser(ScoreGatherer scoreGatherer, Mat scoreBoard, string team, int overs, int runs) {
            this.scoreBoard = scoreBoard;
            this.overs = overs;
            this.runs = runs;
            this.scoreGatherer = scoreGatherer;
            this.team = team;
        }

        /// <summary>
        /// Scan scoreboard and send the extracted data to ScoreGatherer
        /// </summary>
        /// <returns></returns>
        public async Task<int> Parse() {

            //Scan and extract infomation from scoreboard.
            string batter1, batter2, bowler, score;
            page = ReadTextFromROI(scoreBoard, ROIConsts.BAT_1_X, ROIConsts.BAT_1_Y, ROIConsts.BAT_1_W, ROIConsts.BAT_1_H, true);
            batter1 = page.GetText().Replace("\n", string.Empty);
            page.Dispose();

            page = ReadTextFromROI(scoreBoard, ROIConsts.BAT_2_X, ROIConsts.BAT_2_Y, ROIConsts.BAT_2_W, ROIConsts.BAT_2_H, true);
            batter2 = page.GetText().Replace("\n", string.Empty);
            page.Dispose();

            //Detect current batter by looking for > marker before batters to confirm batter 1.
            bool bat1 = CountZero(scoreBoard, ROIConsts.BAT_1_MARK_X, ROIConsts.BAT_1_MARK_Y, ROIConsts.BAT_1_MARK_W, ROIConsts.BAT_1_MARK_H) == ROIConsts.BAT_1_NUM;
            if (!bat1) (batter1, batter2) = (batter2, batter1);

            page = ReadTextFromROI(scoreBoard, ROIConsts.BOW_X, ROIConsts.BOW_Y, ROIConsts.BOW_W, ROIConsts.BOW_H, true);
            bowler = page.GetText().Replace("\n", string.Empty);
            page.Dispose();

            page = ReadTextFromROI(scoreBoard, ROIConsts.SCORE_X, ROIConsts.SCORE_Y, ROIConsts.SCORE_W, ROIConsts.SCORE_H, true, true,false);
            score = page.GetText().Replace("\n", string.Empty);
            page.Dispose();

            return await Task.Run(() => scoreGatherer.Gather(overs, runs, team, batter1, batter2, bowler, score));
        }

    }
}

using CricketExt.util;
using OpenCvSharp;
using System.Diagnostics;
using Tesseract;
using static CricketExt.Analyzer.ProcessUtil;

namespace CricketExt.Analyzer {
    internal class ScoreParser {

        private readonly Mat scoreBoard;
        private readonly int outs, runs;
        Page? page;
        public ScoreParser(Mat scoreBoard, int outs, int runs) {
            this.scoreBoard = scoreBoard;
            this.outs = outs;
            this.runs = runs;
        }

        public async Task<int> Gather() {

            Debug.WriteLine($"Over: Out {outs}, Run {runs}");
            //Cv2.ImShow("Display", scoreBoard);
            page = ReadTextFromROI(scoreBoard, ROI.TEAM_X, ROI.TEAM_Y, ROI.TEAM_W, ROI.TEAM_H, true);
            Debug.Write($"team: {page.GetText()}");
            page.Dispose();

            page = ReadTextFromROI(scoreBoard, ROI.BAT_1_X, ROI.BAT_1_Y, ROI.BAT_1_W, ROI.BAT_1_H, true);
            Debug.Write($"Batter1: {page.GetText()}");
            page.Dispose();

            page = ReadTextFromROI(scoreBoard, ROI.BAT_2_X, ROI.BAT_2_Y, ROI.BAT_2_W, ROI.BAT_2_H, true);
            Debug.Write($"Batter2: {page.GetText()}");
            page.Dispose();

            page = ReadTextFromROI(scoreBoard, ROI.BOW_X, ROI.BOW_Y, ROI.BOW_W, ROI.BOW_H, true);
            Debug.Write($"Bowler: {page.GetText()}");
            page.Dispose();

            page = ReadTextFromROI(scoreBoard, ROI.SCORE_X, ROI.SCORE_Y, ROI.SCORE_W, ROI.SCORE_H);
            Debug.Write($"Score: {page.GetText()}");
            page.Dispose();

            return 0;
        } 
        
    }
}

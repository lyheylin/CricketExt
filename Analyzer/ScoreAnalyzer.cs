using System.Diagnostics;
using OpenCvSharp;
using Tesseract;
using CricketExt.util;
using System.Text.RegularExpressions;
using static CricketExt.Analyzer.ProcessUtil;

namespace CricketExt.Analyzer {
    internal class ScoreAnalyzer : IAnalyzer {
       
        ScoreGatherer scoreGatherer = new();
        HashSet<String> parsed = new();
        //Takes a single frame and scan for scoreboard, if a scoreboard is identified, send the frame to ScoreParser.
        public void Scan(Mat frame) {
            Page page = ReadTextFromROI(frame, ROIConsts.CHECK_1_X, ROIConsts.CHECK_1_Y, ROIConsts.CHECK_1_W, ROIConsts.CHECK_1_H);
            
            if (page.GetText().Equals("OVERS\n")) {//TODO need better checker
                Mat scoreboard = frame.Clone();

                page.Dispose();
                //Debug.WriteLine("Checker 1 found.");
                
                page = ReadTextFromROI(frame, ROIConsts.OUT_X, ROIConsts.OUT_Y, ROIConsts.OUT_W, ROIConsts.OUT_H, true, true);
                String oversStr = page.GetText();
                page.Dispose();
                page = ReadTextFromROI(frame, ROIConsts.BALL_X, ROIConsts.BALL_Y, ROIConsts.BALL_W, ROIConsts.BALL_H, true, true);
                String ballsStr = page.GetText();
                page.Dispose();
                page = ReadTextFromROI(frame, ROIConsts.TEAM_X, ROIConsts.TEAM_Y, ROIConsts.TEAM_W, ROIConsts.TEAM_H, true);
                String teamStr = page.GetText();
                page.Dispose();

                if (Int32.TryParse(oversStr, out int oversInt) && Int32.TryParse(ballsStr, out int ballInt)) {
                    if (parsed.Add(GenTurnString(teamStr, oversInt, ballInt))) {//TODO sometimes a 'Ball' can consist of more than one ball throw. <= need change for this case?
                        ScoreParser parser = new(scoreGatherer, scoreboard, teamStr, oversInt, ballInt);
                        parser.Parse();
                    }
                }
                page.Dispose();
            }
            page.Dispose();
            frame.Dispose();
        }

        public void GetResult() {
            scoreGatherer.postProcess();
        }
    }
}

﻿using System.Diagnostics;
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

                /**
                page = ReadTextFromROI(frame, ROI.OVER_X, ROI.OVER_Y, ROI.OVER_W, ROI.OVER_H, true, true);
                Debug.Write($"Over: {page.GetText()}");
                Regex regex = new(@"([0-9]+).([0-9]+)\n");
                Match match = regex.Match(page.GetText());
                if (!match.Success) {
                    Debug.WriteLine($"Confidence: {page.GetMeanConfidence()}");
                    OpenCvSharp.Rect roi = new(ROI.OVER_X, ROI.OVER_Y, ROI.OVER_W, ROI.OVER_H);
                    Mat croppedMat = new(frame, roi);
                    Cv2.ImShow("Display", croppedMat);
                }
                **/

                String outStr = "", ballStr = "", teamStr = "";
                page = ReadTextFromROI(frame, ROIConsts.OUT_X, ROIConsts.OUT_Y, ROIConsts.OUT_W, ROIConsts.OUT_H, true, true);
                outStr = page.GetText();
                page.Dispose();
                page = ReadTextFromROI(frame, ROIConsts.BALL_X, ROIConsts.BALL_Y, ROIConsts.BALL_W, ROIConsts.BALL_H, true, true);
                ballStr = page.GetText();
                page.Dispose();
                page = ReadTextFromROI(frame, ROIConsts.TEAM_X, ROIConsts.TEAM_Y, ROIConsts.TEAM_W, ROIConsts.TEAM_H, true);
                teamStr = page.GetText();
                Debug.WriteLine($"team: {teamStr}");
                page.Dispose();

                if (Int32.TryParse(outStr, out int outInt) && Int32.TryParse(ballStr, out int ballInt)) {
                    if (parsed.Add(GenTurnString(ballStr, outInt, ballInt))) {//TODO sometimes a 'Ball' can consist of more than one ball throw. <= need change for this case?
                        ScoreParser parser = new(scoreGatherer, scoreboard, teamStr, outInt, ballInt);
                        parser.Parse();
                    }
                }
                page.Dispose();
            }
            page.Dispose();
        }
    }
}

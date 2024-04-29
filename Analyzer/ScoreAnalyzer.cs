using System.Diagnostics;
using OpenCvSharp;
using Tesseract;
using CricketExt.util;
using System.Text.RegularExpressions;
using static CricketExt.Analyzer.ProcessUtil;

namespace CricketExt.Analyzer {
    internal class ScoreAnalyzer : IAnalyzer {
       
        HashSet<String> parsed = new();
        //Takes a single frame and scan for scoreboard
        public void Scan(Mat frame) {
            Page page = ReadTextFromROI(frame, ROI.CHECK_1_X, ROI.CHECK_1_Y, ROI.CHECK_1_W, ROI.CHECK_1_H);
            
            if (page.GetText().Equals("OVERS\n")) {//need better checker
                Mat scoreboard = frame.Clone();

                page.Dispose();
                //Debug.WriteLine("Checker 1 found.");
                
                page = ReadTextFromROI(frame, ROI.OVER_X, ROI.OVER_Y, ROI.OVER_W, ROI.OVER_H);
                //Debug.Write($"Over: {page.GetText()}");
                Regex regex = new(@"([0-9]+).([0-9]+)\n");
                Match match = regex.Match(page.GetText());
                //Debug.WriteLine($"Over: Out{match.Groups[1]}, Run{match.Groups[2]}");
                int outs, runs;
                if (Int32.TryParse(match.Groups[1].Value, out outs) && Int32.TryParse(match.Groups[2].Value, out runs)) {
                    if (parsed.Add(page.GetText())) {
                        ScoreParser parser = new ScoreParser(scoreboard, outs, runs);
                        parser.Gather();
                    }
                }
                page.Dispose();
            }
            page.Dispose();
        }
    }
}

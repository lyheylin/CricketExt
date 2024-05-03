﻿using System.Diagnostics;
using OpenCvSharp;
using Tesseract;
using CricketExt.util;
using System.Text.RegularExpressions;
using static CricketExt.Analyzer.ProcessUtil;
using System.Threading.Tasks;
using CricketExt.DataTypes;
using System.Collections.Concurrent;
using static System.Formats.Asn1.AsnWriter;
using System;

namespace CricketExt.Analyzer {
    internal class ScoreAnalyzer : IAnalyzer {
        ScoreGatherer scoreGatherer = new();
        HashSet<String> parsed = new();
        const int JUMP_FRAMES = 59;
        private ConcurrentStack<(String, int)> latestFrame = new();
        //Takes a single frame and scan for scoreboard, if a scoreboard is identified, send the frame to ScoreParser.
        public async Task<int[]> ScanAsync(VideoCapture v) {

            List<Task<int>> tasks = new();
            while (v.IsOpened()) {
                using Mat frame = new(v.FrameHeight, v.FrameWidth, MatType.CV_8UC3);

                v.PosFrames += JUMP_FRAMES;
                bool next = v.Read(frame);
                if (!next) {
                    latestFrame.TryPeek(out (String, int) latest);
                    v.PosFrames = latest.Item2;
                    v.Read(frame);
                    Mat scoreboard = frame.Clone();
                    Regex r = new(@"([ABCDEFGHIJKLMNOPQRSTUVWXYZ]+)/([0-9]+).([0-9]+)");
                    Match m = r.Match(latest.Item1);
                    ScoreParser parser = new(scoreGatherer, scoreboard, m.Groups[1].Value, Int32.Parse(m.Groups[2].Value), Int32.Parse(m.Groups[3].Value));
                    tasks.Add(Task.Run(() => parser.Parse()));
                    Debug.WriteLine("End of video");
                    break;
                }

                if (IsScoreBoard(frame)) {
                    Page page;
                    page = ReadTextFromROI(frame, ROIConsts.OUT_X, ROIConsts.OUT_Y, ROIConsts.OUT_W, ROIConsts.OUT_H, true, true);
                    String oversStr = page.GetText();
                    page.Dispose();
                    page = ReadTextFromROI(frame, ROIConsts.BALL_X, ROIConsts.BALL_Y, ROIConsts.BALL_W, ROIConsts.BALL_H, true, true);
                    String ballsStr = page.GetText();
                    page.Dispose();
                    page = ReadTextFromROI(frame, ROIConsts.TEAM_X, ROIConsts.TEAM_Y, ROIConsts.TEAM_W, ROIConsts.TEAM_H, true);
                    String teamStr = page.GetText();
                    teamStr = teamStr.Replace("\n", String.Empty);
                    page.Dispose();
                    

                    if (Int32.TryParse(oversStr, out int oversInt) && Int32.TryParse(ballsStr, out int ballInt)) {
                        String roundStr = GenTurnString(teamStr, oversInt, ballInt);
                        if (!parsed.Add(roundStr)) {//TODO sometimes a 'Ball' can consist of more than one ball throw. <= need change for this case?
                            latestFrame.TryPop(out (String, int) latest);
                            latestFrame.Push((roundStr, v.PosFrames-1));
                        } else {
                            if (!latestFrame.IsEmpty) {
                                int i = v.PosFrames;
                                latestFrame.TryPeek(out (String, int) latest);
                                //Debug.WriteLine($"{latest}");
                                v.PosFrames = latest.Item2;
                                v.Read(frame);
                                Mat scoreboard = frame.Clone();
                                v.PosFrames = i;
                                Regex r = new (@"([ABCDEFGHIJKLMNOPQRSTUVWXYZ]+)/([0-9]+).([0-9]+)");
                                Match m = r.Match(latest.Item1);
                                Debug.WriteLine($"{roundStr}");
                                Debug.WriteLine($"{m.Groups[1].Value},{m.Groups[2].Value},{m.Groups[3].Value}, {latest.Item2}");
                                ScoreParser parser = new(scoreGatherer, scoreboard, m.Groups[1].Value, Int32.Parse(m.Groups[2].Value), Int32.Parse(m.Groups[3].Value));
                                tasks.Add(Task.Run(() => parser.Parse()));
                            }
                            latestFrame.Push((roundStr, v.PosFrames-1));
                        }
                    }
                    page.Dispose();
                }

                int key = Cv2.WaitKey(0);
                if ((key & 0xFF) == Convert.ToUInt32('q'))
                    break;
            }
            int[] results = await Task.WhenAll(tasks);
            v.Release();
            return results;
        }

        public String[] GetResult() {
            return scoreGatherer.PostProcess();
        }

        /// <summary>
        /// Check for existence of scoreboard in current image.
        /// </summary>
        /// <param name="frame">Image to check for scoreboard.</param>
        /// <returns>True if scoreboard is found. Otherwise False.</returns>
        private bool IsScoreBoard(Mat frame) {
            Regex reg = new(@"([0-9]+)/([0-9]+)");
            Page page = ReadTextFromROI(frame, ROIConsts.CHECK_1_X, ROIConsts.CHECK_1_Y, ROIConsts.CHECK_1_W, ROIConsts.CHECK_1_H);
            if (page.GetText().Equals("OVERS\n")) {
                page.Dispose();
                page = ReadTextFromROI(frame, ROIConsts.SCORE_X, ROIConsts.SCORE_Y, ROIConsts.SCORE_W, ROIConsts.SCORE_H, true, true);
                bool sucess = reg.Match(page.GetText()).Success;
                page.Dispose();
                return sucess;
            }
            page.Dispose();
            return false;
        }
    }
}

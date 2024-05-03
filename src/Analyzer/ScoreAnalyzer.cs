using System.Diagnostics;
using OpenCvSharp;
using Tesseract;
using System.Text.RegularExpressions;
using static CricketExt.Analyzer.ProcessUtil;
using System.Threading.Tasks;
using CricketExt.DataTypes;
using System.Collections.Concurrent;
using static System.Formats.Asn1.AsnWriter;
using System;
using CricketExt.util;

namespace CricketExt.Analyzer {
    internal class ScoreAnalyzer : IAnalyzer {
        ScoreGatherer scoreGatherer = new();
        HashSet<string> parsed = new();
        const int JUMP_FRAMES = 59;
        private ConcurrentStack<(string, int)> latestFrame = new();
        //Takes a single frame and scan for scoreboard, if a scoreboard is identified, send the frame to ScoreParser.
        public async Task<int[]> ScanAsync(VideoCapture v) {

            List<Task<int>> tasks = new();
            while (v.IsOpened()) {
                using Mat frame = new(v.FrameHeight, v.FrameWidth, MatType.CV_8UC3);

                v.PosFrames += JUMP_FRAMES;
                bool next = v.Read(frame);
                if (!next) {
                    latestFrame.TryPeek(out (string, int) latest);
                    v.PosFrames = latest.Item2;
                    v.Read(frame);
                    Mat scoreboard = frame.Clone();
                    Regex r = new(@"([ABCDEFGHIJKLMNOPQRSTUVWXYZ]+)/([0-9]+).([0-9]+)");
                    Match m = r.Match(latest.Item1);
                    ScoreParser parser = new(scoreGatherer, scoreboard, m.Groups[1].Value, int.Parse(m.Groups[2].Value), int.Parse(m.Groups[3].Value));
                    tasks.Add(Task.Run(() => parser.Parse()));
                    Debug.WriteLine("End of video");
                    break;
                }

                if (IsScoreBoard(frame)) {
                    Page page;
                    page = ReadTextFromROI(frame, ROIConsts.OUT_X, ROIConsts.OUT_Y, ROIConsts.OUT_W, ROIConsts.OUT_H, true, true);
                    string oversStr = page.GetText();
                    oversStr = oversStr.Replace("\n", string.Empty);
                    page.Dispose();
                    page = ReadTextFromROI(frame, ROIConsts.BALL_X, ROIConsts.BALL_Y, ROIConsts.BALL_W, ROIConsts.BALL_H, true, true);
                    string ballsStr = page.GetText();
                    ballsStr = ballsStr.Replace("\n", string.Empty);
                    page.Dispose();
                    page = ReadTextFromROI(frame, ROIConsts.TEAM_X, ROIConsts.TEAM_Y, ROIConsts.TEAM_W, ROIConsts.TEAM_H, true);
                    string teamStr = page.GetText();
                    teamStr = teamStr.Replace("\n", string.Empty);
                    page.Dispose();

                    if (int.TryParse(oversStr, out int oversInt) && int.TryParse(ballsStr, out int ballInt)) {
                        string roundStr = GenTurnString(teamStr, oversInt, ballInt);
                        Debug.WriteLine($"{roundStr}");
                        Regex r = new(@"([ABCDEFGHIJKLMNOPQRSTUVWXYZ]+)/([0-9]+).([0-9]+)");
                        if (r.Match(roundStr).Success) {
                            if (!parsed.Add(roundStr)) {
                                latestFrame.TryPop(out (string, int) latest);
                                latestFrame.Push((roundStr, v.PosFrames - 1));
                            } else {
                                if (!latestFrame.IsEmpty) {
                                    int i = v.PosFrames;
                                    latestFrame.TryPeek(out (string, int) latest);
                                    v.PosFrames = latest.Item2;
                                    v.Read(frame);
                                    Mat scoreboard = frame.Clone();
                                    v.PosFrames = i;
                                    Match m = r.Match(latest.Item1);
                                    ScoreParser parser = new(scoreGatherer, scoreboard, m.Groups[1].Value, int.Parse(m.Groups[2].Value), int.Parse(m.Groups[3].Value));
                                    tasks.Add(Task.Run(() => parser.Parse()));
                                }
                                latestFrame.Push((roundStr, v.PosFrames - 1));
                            }
                        }
                    }
                    page.Dispose();
                }

                int key = Cv2.WaitKey(1000);
                if ((key & 0xFF) == Convert.ToUInt32('q'))
                    break;
            }
            int[] results = await Task.WhenAll(tasks);
            v.Release();
            return results;
        }

        public string[] GetResult() {
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
            if (page.GetText().Equals("OVERS\n"))
            {
                page.Dispose();
                page = ReadTextFromROI(frame, ROIConsts.SCORE_X, ROIConsts.SCORE_Y, ROIConsts.SCORE_W, ROIConsts.SCORE_H, true, true);
                Debug.WriteLine(@$"{page.GetText().Replace("\n", String.Empty)}");
                bool sucess = reg.Match(page.GetText()).Success;
                page.Dispose();
                return sucess;
            }
            page.Dispose();
            return false;
        }
    }
}

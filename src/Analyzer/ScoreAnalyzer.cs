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
    /// <summary>
    /// Analyzer that scans for scoreboards and extract data from it.
    /// </summary>
    internal class ScoreAnalyzer : IAnalyzer {
        private readonly ScoreGatherer scoreGatherer = new();
        private HashSet<string> parsed = new();
        private const int JUMP_FRAMES = 59; //Constant for deciding the interval of frames being checked. (Skipping frames for efficiency.)
        private ConcurrentStack<(string, int)> latestFrame = new();
        private readonly Regex r = new(@"([ABCDEFGHIJKLMNOPQRSTUVWXYZ]+)/([0-9]+).([0-9]+):([0-9]+)/([0-9]+)");

        /// <summary>
        /// Takes a video and scan for scoreboards. If a scoreboard is identified check if it's a desirable frame and send the frame to ScoreParser.
        /// </summary>
        /// <param name="v">Video</param>
        /// <returns></returns>
        public async Task<int[]> ScanAsync(VideoCapture v) {
            String team1 = String.Empty;
            List<Task<int>> tasks = new();

            //Opens video and check every JUMP_FRAMES+1 frame.
            while (v.IsOpened()) {
                using Mat frame = new(v.FrameHeight, v.FrameWidth, MatType.CV_8UC3);

                v.PosFrames += JUMP_FRAMES;
                bool next = v.Read(frame);

                //End of video
                if (!next) {

                    //Pushes last frame available in queue before cleaning up.
                    if (latestFrame.TryPeek(out (string, int) latest)) {
                        v.PosFrames = latest.Item2;
                        v.Read(frame);
                        Mat scoreboard = frame.Clone();
                        Match m = r.Match(latest.Item1);
                        ScoreParser parser = new(scoreGatherer, scoreboard, m.Groups[1].Value, int.Parse(m.Groups[2].Value), int.Parse(m.Groups[3].Value));
                        tasks.Add(Task.Run(() => parser.Parse()));
                    }
                    
                    Debug.WriteLine("End of video");
                    break;
                }

                //Check for scoreboard and process.
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
                    page = ReadTextFromROI(frame, ROIConsts.MARKS_X, ROIConsts.MARKS_Y, ROIConsts.MARKS_W, ROIConsts.MARKS_H, true);
                    String marks = page.GetText().Replace("\n", string.Empty);
                    page.Dispose();

                    
                    if (int.TryParse(oversStr, out int oversInt) && int.TryParse(ballsStr, out int ballInt)) {
                        String score;
                        page = ReadTextFromROI(frame, ROIConsts.SCORE_X, ROIConsts.SCORE_Y, ROIConsts.SCORE_W, ROIConsts.SCORE_H, true, true, false);
                        score = page.GetText().Replace("\n", string.Empty);
                        page.Dispose();

                        string roundStr = GenTurnString(teamStr, oversInt, ballInt, score);

                        //Keeps track of the latest frame of a ball. Only sends out if the frame is a viable and updated frame.
                        if (r.Match(roundStr).Success) {
                            //This pushes out the last ball in queue of the first team.
                            if (!teamStr.Equals(team1) && !latestFrame.IsEmpty) {
                                int i = v.PosFrames;
                                latestFrame.TryPeek(out (string, int) latest);
                                Match m = r.Match(latest.Item1);
                                    v.PosFrames = latest.Item2;
                                    v.Read(frame);
                                    Mat scoreboard = frame.Clone();
                                    v.PosFrames = i;
                                    ScoreParser parser = new(scoreGatherer, scoreboard, m.Groups[1].Value, int.Parse(m.Groups[2].Value), int.Parse(m.Groups[3].Value));
                                    tasks.Add(Task.Run(() => parser.Parse()));
                            }
                            team1 = teamStr;

                            //Manage redundant frames. Make sure that only one frame per ball per bowl is sent to the parser.
                            if (!parsed.Add(roundStr)) {
                                latestFrame.TryPop(out (string, int) latest);
                                latestFrame.Push((roundStr, v.PosFrames - 1));
                            } else {
                                if (!latestFrame.IsEmpty) {
                                    int i = v.PosFrames;
                                    latestFrame.TryPeek(out (string, int) latest);
                                    Match m = r.Match(latest.Item1);
                                    if (v.PosFrames - latest.Item2 > 300) { //Discard trailing frames of each balls. 
                                        latestFrame.TryPop(out (string, int) _);
                                    } else {
                                        v.PosFrames = latest.Item2;
                                        v.Read(frame);
                                        Mat scoreboard = frame.Clone();
                                        v.PosFrames = i;
                                        ScoreParser parser = new(scoreGatherer, scoreboard, m.Groups[1].Value, int.Parse(m.Groups[2].Value), int.Parse(m.Groups[3].Value));
                                        tasks.Add(Task.Run(() => parser.Parse()));
                                    }
                                }
                                latestFrame.Push((roundStr, v.PosFrames - 1));
                            }
                        }
                    }
                    page.Dispose();
                }
            }

            int[] results = await Task.WhenAll(tasks);
            v.Release();
            return results;
        }

        /// <summary>
        /// /// <summary>
        /// Get result from gatherer.
        /// </summary>
        /// <param name="header">Option to return header.</param>
        /// <returns>Processed result as string array.</returns>
        public string[] GetResult(bool header) {
            return scoreGatherer.PostProcess(header);
        }

        /// <summary>
        /// Check for existence of scoreboard in current image.
        /// </summary>
        /// <param name="frame">Image to check for scoreboard.</param>
        /// <returns>True if scoreboard is found. Otherwise False.</returns>
        private static bool IsScoreBoard(Mat frame) {
            Regex reg = new(@"([0-9]+)/([0-9]+)");
            Page page = ReadTextFromROI(frame, ROIConsts.CHECK_1_X, ROIConsts.CHECK_1_Y, ROIConsts.CHECK_1_W, ROIConsts.CHECK_1_H);
            if (page.GetText().Equals("OVERS\n"))
            {
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

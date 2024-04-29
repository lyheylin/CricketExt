using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using Tesseract;
using OpenCvSharp.Text;
using System.Runtime.InteropServices;
using CricketExt.util;

namespace CricketExt.Analyzer
{
    internal class ScoreAnalyzer : IAnalyzer
    {
        const String TESS_FOLDER = @"./tessdata";
        const String TESS_LANGUAGE_ENG = "eng";
        static readonly TesseractEngine engine = new(TESS_FOLDER, TESS_LANGUAGE_ENG, EngineMode.Default);
        

        //Takes a single frame and scan for scoreboard
        public void Scan(Mat frame)
        {
            //Initialize Tesseract engine
            OpenCvSharp.Rect check1 = new OpenCvSharp.Rect(util.ROI.CHECK_1_X, util.ROI.CHECK_1_Y, util.ROI.CHECK_1_W, util.ROI.CHECK_1_H);
            Page page = ReadTextFromROI(frame, util.ROI.CHECK_1_X, util.ROI.CHECK_1_Y, util.ROI.CHECK_1_W, util.ROI.CHECK_1_H);  
            
            if (page.GetText().Equals("OVERS\n")) {
                page.Dispose();
                Debug.WriteLine("Checker 1 found.");
                Cv2.ImShow("Display", frame);
                
                page = ReadTextFromROI(frame, util.ROI.OVER_X, util.ROI.OVER_Y, util.ROI.OVER_W, util.ROI.OVER_H);
                Debug.Write($"Over: {page.GetText()}");
                page.Dispose();

                page = ReadTextFromROI(frame, util.ROI.TEAM_X, util.ROI.TEAM_Y, util.ROI.TEAM_W, util.ROI.TEAM_H, true);
                Debug.Write($"team: {page.GetText()}");
                page.Dispose();

                page = ReadTextFromROI(frame, util.ROI.BAT_1_X, util.ROI.BAT_1_Y, util.ROI.BAT_1_W, util.ROI.BAT_1_H, true);
                Debug.Write($"Batter1: {page.GetText()}");
                page.Dispose();

                page = ReadTextFromROI(frame, util.ROI.BAT_2_X, util.ROI.BAT_2_Y, util.ROI.BAT_2_W, util.ROI.BAT_2_H, true);
                Debug.Write($"Batter2: {page.GetText()}");
                page.Dispose();

                page = ReadTextFromROI(frame, util.ROI.BOW_X, util.ROI.BOW_Y, util.ROI.BOW_W, util.ROI.BOW_H, true);
                Debug.Write($"Bowler: {page.GetText()}");
                page.Dispose();

                page = ReadTextFromROI(frame, util.ROI.SCORE_X, util.ROI.SCORE_Y, util.ROI.SCORE_W, util.ROI.SCORE_H);
                Debug.Write($"Score: {page.GetText()}");
                page.Dispose();
            }

            page.Dispose();
        }

        private Pix Mat2Pix(Mat src) {
            return Pix.LoadFromMemory(src.ToBytes(".png", (int[]?)null));
        }

        //Preprocess image by greyscale then coverged to b/w inverted image.
        private Mat MatPreprocess(Mat mat) {
            Mat processed = new();
            Cv2.CvtColor(mat, processed, ColorConversionCodes.RGB2GRAY);
            Cv2.Threshold(processed, processed, 110, 255, ThresholdTypes.BinaryInv);
            return processed;
        }

        //Read single line text from ROI of image src.
        private Page ReadTextFromROI(Mat src, int x, int y, int w, int h, bool preprocess = false) {
            OpenCvSharp.Rect roi = new(x,y,w,h);
            Mat croppedMat = new(src, roi);
            if (preprocess) 
                croppedMat = MatPreprocess(croppedMat);
            Page page = engine.Process(Mat2Pix(croppedMat), PageSegMode.SingleLine);
            return page;
        }
    }
}

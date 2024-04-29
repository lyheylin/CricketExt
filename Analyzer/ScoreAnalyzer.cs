using System.Diagnostics;
using OpenCvSharp;
using Tesseract;
using CricketExt.util;

namespace CricketExt.Analyzer
{
    internal class ScoreAnalyzer : IAnalyzer
    {
        //Initialize Tesseract engine
        const String TESS_FOLDER = @"./tessdata";
        const String TESS_LANGUAGE_ENG = "eng";
        static readonly TesseractEngine engine = new(TESS_FOLDER, TESS_LANGUAGE_ENG, EngineMode.Default);
        

        //Takes a single frame and scan for scoreboard
        public void Scan(Mat frame)
        {
            Page page = ReadTextFromROI(frame, ROI.CHECK_1_X, ROI.CHECK_1_Y, ROI.CHECK_1_W, ROI.CHECK_1_H);  
            
            if (page.GetText().Equals("OVERS\n")) {
                page.Dispose();
                Debug.WriteLine("Checker 1 found.");
                Cv2.ImShow("Display", frame);
                
                page = ReadTextFromROI(frame, ROI.OVER_X, ROI.OVER_Y, ROI.OVER_W, ROI.OVER_H);
                Debug.Write($"Over: {page.GetText()}");
                page.Dispose();

                page = ReadTextFromROI(frame, ROI.TEAM_X, ROI.TEAM_Y, ROI.TEAM_W, ROI.TEAM_H, true);
                Debug.Write($"team: {page.GetText()}");
                page.Dispose();

                page = ReadTextFromROI(frame, ROI.BAT_1_X, ROI.BAT_1_Y, ROI.BAT_1_W, ROI.BAT_1_H, true);
                Debug.Write($"Batter1: {page.GetText()}");
                page.Dispose();

                page = ReadTextFromROI(frame, ROI.BAT_2_X, ROI.BAT_2_Y, ROI.BAT_2_W, ROI.BAT_2_H, true);
                Debug.Write($"Batter2: {page.GetText()}");
                page.Dispose();

                page = ReadTextFromROI(frame, ROI.BOW_X, ROI.BOW_Y, ROI.BOW_W, ROI.BOW_H, true);
                Debug.Write($"Bowler: {page.GetText()}");
                page.Dispose();

                page = ReadTextFromROI(frame, ROI.SCORE_X, ROI.SCORE_Y, ROI.SCORE_W, ROI.SCORE_H);
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

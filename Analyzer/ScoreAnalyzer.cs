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
using static System.Net.Mime.MediaTypeNames;

namespace CricketExt.Analyzer
{
    internal class ScoreAnalyzer : IAnalyzer
    {
        const String TESS_FOLDER = @"./tessdata";
        const String TESS_LANGUAGE_ENG = "eng";
        const int TEXTS = 256;


        //Takes a single frame and scan for scoreboard
        public void Scan(Mat frame)
        {
            using var engine = new TesseractEngine(TESS_FOLDER, TESS_LANGUAGE_ENG, EngineMode.Default);
            
            //Initialize Tesseract engine with output parameters
            //OCRTesseract engine = OCRTesseract.Create(TESS_FOLDER, TESS_LANGUAGE_ENG, "1234567890./abcdfghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ", 3, (int)PageSegMode.SingleLine);//psm_single_line = 7; psm_default_auto = 3
            OpenCvSharp.Rect[] r;// = new OpenCvSharp.Rect[TEXTS];
            String readText;
            String[] recTexts = new string[TEXTS];
            float[] confidents = new float[TEXTS];

            //OpenCvSharp.Rect rect = new OpenCvSharp.Rect(600, 888,460, 40);
            OpenCvSharp.Rect check1 = new OpenCvSharp.Rect(util.ROI.CHECK_1_X, util.ROI.CHECK_1_Y, util.ROI.CHECK_1_W, util.ROI.CHECK_1_H);
            OpenCvSharp.Rect rect = new OpenCvSharp.Rect(util.ROI.BAT_1_X, util.ROI.BAT_1_Y, util.ROI.BAT_1_W, util.ROI.BAT_1_H);
            Mat roi = new Mat(frame, check1);
           
            //var bytes = new byte[roi.Total() * roi.Channels()];
            //Marshal.Copy(roi.Data, bytes, 0, bytes.Length);
            var page = engine.Process(Mat2Pix(roi), PageSegMode.SingleLine);
            
            Debug.WriteLine($"Read: {page.GetText()}");
            
            //engine.Run(roi, out readText, out _, out _, out confidents);
            //frame.
            //frame.ImEncode() 
            //Cv2.ImShow("Display", roi);

            
            if (page.GetText().Equals("OVERS\n")) {
                Debug.WriteLine("Checker 1 found.");
                
                OpenCvSharp.Rect over = new OpenCvSharp.Rect(util.ROI.OVER_X, util.ROI.OVER_Y, util.ROI.OVER_W, util.ROI.OVER_H);
                OpenCvSharp.Rect team = new OpenCvSharp.Rect(util.ROI.TEAM_X, util.ROI.TEAM_Y, util.ROI.TEAM_W, util.ROI.TEAM_H);
                OpenCvSharp.Rect bat1 = new OpenCvSharp.Rect(util.ROI.BAT_1_X, util.ROI.BAT_1_Y, util.ROI.BAT_1_W, util.ROI.BAT_1_H);
                OpenCvSharp.Rect bat2 = new OpenCvSharp.Rect(util.ROI.BAT_2_X, util.ROI.BAT_2_Y, util.ROI.BAT_2_W, util.ROI.BAT_2_H);
                OpenCvSharp.Rect bowler = new OpenCvSharp.Rect(util.ROI.BOW_X, util.ROI.BOW_Y, util.ROI.BOW_W, util.ROI.BOW_H);
                OpenCvSharp.Rect score = new OpenCvSharp.Rect(util.ROI.SCORE_X, util.ROI.SCORE_Y, util.ROI.SCORE_W, util.ROI.SCORE_H);
                roi = new Mat(frame, over);
                Cv2.ImShow("Display", roi);
                page.Dispose();
                page = engine.Process(Mat2Pix(roi), PageSegMode.SingleLine);
                Debug.Write($"Over: {page.GetText()}");
                roi = new Mat(frame, team);
                Cv2.CvtColor(roi, roi, ColorConversionCodes.RGB2GRAY);
                Cv2.Threshold(roi, roi, 110, 255, ThresholdTypes.BinaryInv);
                page.Dispose();
                page = engine.Process(Mat2Pix(roi), PageSegMode.SingleLine);
                Debug.Write($"team: {page.GetText()}");
                roi = new Mat(frame, bat1);
                page.Dispose();
                page = engine.Process(Mat2Pix(roi), PageSegMode.SingleLine);
                Debug.Write($"Batter1: {page.GetText()}");
                roi = new Mat(frame, bat2);
                Cv2.CvtColor(roi, roi, ColorConversionCodes.RGB2GRAY);
                Cv2.Threshold(roi, roi, 110, 255, ThresholdTypes.BinaryInv);
                Cv2.ImShow("Display", roi);
                page.Dispose();
                page = engine.Process(Mat2Pix(roi), PageSegMode.SingleLine);
                Debug.Write($"Batter2: {page.GetText()}");
                
                roi = new Mat(frame, bowler);
                Cv2.CvtColor(roi,roi,ColorConversionCodes.RGB2GRAY);
                Cv2.Threshold(roi, roi, 110, 255, ThresholdTypes.BinaryInv);
                Cv2.ImShow("Display", roi);
                page.Dispose();
                page = engine.Process(Mat2Pix(roi), PageSegMode.SingleLine);
                Debug.Write($"Bowler: {page.GetText()}");
                roi = new Mat(frame, score);
                page.Dispose();
                page = engine.Process(Mat2Pix(roi), PageSegMode.SingleLine);
                Debug.Write($"Score: {page.GetText()}");
            }
            
                
            //engine.Run(roi, out readText, out _,out _, out confidents);
            

            //EngineConfig config = new EngineConfig();


            //engine.
            if (confidents.Length>0) {
                //Debug.WriteLine(confidents[0]);
                //Debug.WriteLine($"Text read as: {readText}.");
            }
            //using var img = Pix.LoadFromMemory(frame);
            //using var page = engine.Process(img);
            //Debug.WriteLine("Read: {0}", page.GetText());
        }

        private Pix Mat2Pix(Mat src) {
            return Pix.LoadFromMemory(src.ToBytes(".png", (int[]?)null));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using Tesseract;
using OpenCvSharp.Text;

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
            //using var engine = new TesseractEngine(@"./tessdata", TESS_LANGUAGE, EngineMode.Default);
            OCRTesseract engine = OCRTesseract.Create(TESS_FOLDER, TESS_LANGUAGE_ENG, null, 3, 3);//psm_single_line = 7; psm_default_auto = 3
            //OpenCvSharp.Rect rect = new OpenCvSharp.Rect(600, 888,460, 40);
            OpenCvSharp.Rect rect = new OpenCvSharp.Rect(util.ROI.BAT_1_X, util.ROI.BAT_1_Y, util.ROI.BAT_1_W, util.ROI.BAT_1_H);
            Mat roi = new Mat(frame, rect);
            //Mat roi = frame.AdjustROI(888, 930, 600, 960);
            //frame.
            //frame.ImEncode() 
            Cv2.ImShow("Display", roi);
            OpenCvSharp.Rect[] r;// = new OpenCvSharp.Rect[TEXTS];
            String readText;
            String[] recTexts = new string[TEXTS];
            float[] confidents = new float[TEXTS];

            engine.Run(roi, out readText, out _,out _, out _);

            EngineConfig config = new EngineConfig();
         

            //engine.

            Debug.WriteLine(readText);
            //using var img = Pix.LoadFromMemory(frame);
            //using var page = engine.Process(img);
            //Debug.WriteLine("Read: {0}", page.GetText());
        }
    }
}

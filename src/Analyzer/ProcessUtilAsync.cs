using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tesseract;

namespace CricketExt.Analyzer
{
    internal class ProcessUtilAsync
    {

        //Initialize Tesseract engine
        const string TESS_FOLDER = @"./tessdata";
        const string TESS_LANGUAGE_ENG = "eng";
        public readonly TesseractEngine engine = new(TESS_FOLDER, TESS_LANGUAGE_ENG, EngineMode.Default);
        public static readonly TesseractEngine engineDigits = new(TESS_FOLDER, TESS_LANGUAGE_ENG, EngineMode.Default);

        public ProcessUtilAsync()
        {
            engineDigits.SetVariable("tessedit_char_whitelist", "1234567890./");
            //Tesseract engine configuration
            //engine.SetVariable(" load_system_dawg", false);
        }

        public Pix Mat2Pix(Mat src)
        {
            return Pix.LoadFromMemory(src.ToBytes(".png", (int[]?)null));
        }

        //Preprocess image by greyscale then coverged to b/w inverted image.
        public Mat MatPreprocess(Mat mat)
        {
            Mat processed = new();
            Cv2.CvtColor(mat, processed, ColorConversionCodes.RGB2GRAY);
            Cv2.Threshold(processed, processed, 110, 255, ThresholdTypes.BinaryInv);
            return processed;
        }

        //Read single line text from ROI of image src.
        public async Task<string> ReadTextFromROIAsync(Mat src, int x, int y, int w, int h, bool preprocess = false, bool digits = false)
        {
            OpenCvSharp.Rect roi = new(x, y, w, h);
            Mat croppedMat = src.Clone(roi);//Use Clone() to leave src untouched.

            //preprocess image before Tesseract
            if (preprocess)
                croppedMat = MatPreprocess(croppedMat);

            //Debug.WriteLine("Processing");


            if (digits)
            {
                using Page resultDigitsPage = engineDigits.Process(Mat2Pix(croppedMat), PageSegMode.SingleLine);
                return await Task.FromResult(resultDigitsPage.GetText());
            }

            using Page resultPage = engine.Process(Mat2Pix(croppedMat), PageSegMode.SingleLine);
            return await Task.FromResult(resultPage.GetText());
        }

        public static string GenTurnString(string team, int outs, int balls)
        {
            return $"{team}/{outs}.{balls}";
        }
    }

}

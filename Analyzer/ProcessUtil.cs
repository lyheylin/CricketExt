using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tesseract;

namespace CricketExt.Analyzer {
    internal class ProcessUtil {

        //Initialize Tesseract engine
        const String TESS_FOLDER = @"./tessdata";
        const String TESS_LANGUAGE_ENG = "eng";
        public static readonly TesseractEngine engine = new(TESS_FOLDER, TESS_LANGUAGE_ENG, EngineMode.Default);
        public static readonly TesseractEngine engineDigits = new(TESS_FOLDER, TESS_LANGUAGE_ENG, EngineMode.Default);

        public ProcessUtil() {
            //Tesseract engine configuration
            engineDigits.SetVariable("tessedit_char_whitelist", "1234567890./");
            //engine.SetVariable(" load_system_dawg", false);
        }
        
        public static Pix Mat2Pix(Mat src) {
            return Pix.LoadFromMemory(src.ToBytes(".png", (int[]?)null));
        }

        //Preprocess image by greyscale then coverged to b/w inverted image.
        public static Mat MatPreprocess(Mat mat) {
            Mat processed = new();
            Cv2.CvtColor(mat, processed, ColorConversionCodes.RGB2GRAY);
            Cv2.Threshold(processed, processed, 110, 255, ThresholdTypes.BinaryInv);
            return processed;
        }

        /// <summary>
        /// Reads single line text from ROI of image src.
        /// </summary>
        /// <param name="src">Input image to scan.</param>
        /// <param name="x">X coordinate of top-left corner of region of interest.</param>
        /// <param name="y">Y coordinate of top-left corner of region of interest.</param>
        /// <param name="w">Width region of interest.</param>
        /// <param name="h">Height of region of interest.</param>
        /// <param name="preprocess">Preprocess image with greyscaling and inverting to black and white image.</param>
        /// <param name="digits">Reads only digits and '.', '/'.</param>
        /// <returns>Returns a Page file containing OCR result of region of interest of src.</returns>
        public static Page ReadTextFromROI(Mat src, int x, int y, int w, int h, bool preprocess = false, bool digits = false) {
            OpenCvSharp.Rect roi = new(x, y, w, h);
            Mat croppedMat = src.Clone(roi);//Use Clone() to leave src untouched.

            //preprocess image before Tesseract
            if (preprocess)
                croppedMat = MatPreprocess(croppedMat);
            if (digits)
                return engineDigits.Process(Mat2Pix(croppedMat), PageSegMode.SingleLine); 
            return engine.Process(Mat2Pix(croppedMat), PageSegMode.SingleLine);
        }

        public static String GenTurnString(String team, int outs, int balls) {
            return $"{team}/{outs}.{balls}";
        }
    }

}

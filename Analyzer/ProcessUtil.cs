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

        //Read single line text from ROI of image src.
        public static Page ReadTextFromROI(Mat src, int x, int y, int w, int h, bool preprocess = false) {
            OpenCvSharp.Rect roi = new(x, y, w, h);
            Mat croppedMat = new(src, roi);
            if (preprocess)
                croppedMat = MatPreprocess(croppedMat);
            Page page = engine.Process(Mat2Pix(croppedMat), PageSegMode.SingleLine);
            return page;
        }
    }

}

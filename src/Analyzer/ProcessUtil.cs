﻿using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tesseract;

namespace CricketExt.Analyzer {

    /// <summary>
    /// Provide utility functions using the configured Tessseract engine.
    /// </summary>
    internal class ProcessUtil {

        //Initialize Tesseract engine
        const string TESS_FOLDER = @"./tessdata";
        const string SCORE_PATTERN = @"./tessdata/scorepattern.patterns";
        const string TEAM_PATTERN = @"./tessdata/teampattern.patterns";
        const string TESS_LANGUAGE_ENG = "eng";
        public static readonly TesseractEngine engine = new(TESS_FOLDER, TESS_LANGUAGE_ENG, EngineMode.Default);
        public static readonly TesseractEngine engineDigits = new(TESS_FOLDER, TESS_LANGUAGE_ENG, EngineMode.Default);
        public static readonly TesseractEngine engineTeam = new(TESS_FOLDER, TESS_LANGUAGE_ENG, EngineMode.Default);

        public ProcessUtil() {
            
        }
        /// <summary>
        /// Initialize configurations for the tesseract engine.
        /// </summary>
        public static void SetVariables() {
            //Tesseract engine configuration
            engineDigits.SetVariable("tessedit_char_whitelist", @"1234567890./");
            //engineDigits.SetVariable("classify_bln_numeric_mode", 1);
            engineDigits.SetVariable("user_patterns_file", SCORE_PATTERN);
            engineTeam.SetVariable("tessedit_char_whitelist", "ABCDEFGHIJKLMNOPQRSTUVWXYZ");
            engineTeam.SetVariable("user_patterns_file", TEAM_PATTERN);
            engine.SetVariable(" load_system_dawg", false);
        }

        private static Pix Mat2Pix(Mat src) {
            return Pix.LoadFromMemory(src.ToBytes(".png", (int[]?)null));
        }

        //Preprocess image by greyscale then coverged to b/w inverted image.
        private static Mat MatPreprocess(Mat mat) {
            Mat processed = new();
            Cv2.CvtColor(mat, processed, ColorConversionCodes.RGB2GRAY);
            Cv2.Threshold(processed, processed, 120, 255, ThresholdTypes.BinaryInv);
            //Cv2.Resize(processed, processed, new Size(processed.Width*2, processed.Height*2));

            //var se = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(1, 1));
            //Cv2.GaussianBlur(processed, processed, new(5, 5), 0);
            //Cv2.MorphologyEx(processed, processed, MorphTypes.Open, se);
            //Cv2.Erode(processed, processed, se);
            //Cv2.ImShow("image", processed);
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
        public static Page ReadTextFromROI(Mat src, int x, int y, int w, int h, bool preprocess = false, bool digits = false, bool team = false) {

            OpenCvSharp.Rect roi = new(x, y, w, h);
            Mat croppedMat = src.Clone(roi);//Use Clone() to leave src untouched.

            //preprocess image before Tesseract
            if (preprocess)
                croppedMat = MatPreprocess(croppedMat);
            if (digits) {
                //Cv2.ImShow("display", croppedMat);
                //int key = Cv2.WaitKey(10);
                return engineDigits.Process(Mat2Pix(croppedMat), PageSegMode.SingleBlock);
            }
            if (team)
                return engineTeam.Process(Mat2Pix(croppedMat), PageSegMode.SingleBlock);
            return engine.Process(Mat2Pix(croppedMat), PageSegMode.SingleLine);
        }

        /// <summary>
        /// Returns identifier String using the batting team, and ball number.
        /// </summary>
        /// <param name="team">Batting team.</param>
        /// <param name="overs">Number of overs.</param>
        /// <param name="balls">Number of balls.</param>
        /// <returns></returns>
        public static string GenTurnString(string team, int overs, int balls, String score) {
            return $"{team}/{overs}.{balls}:{score}";
        }

        /// <summary>
        /// Counts number of white spaces of a specific roi of a image by preprocessing the image. 
        /// </summary>
        /// <param name="src">Image to be analyzed.</param>
        /// <param name="x">X coordinate of top-left corner of region of interest.</param>
        /// <param name="y">Y coordinate of top-left corner of region of interest.</param>
        /// <param name="w">Width of region of interest.</param>
        /// <param name="h">Height of region of interest.</param>
        /// <returns></returns>
        public static int CountZero(Mat src, int x, int y, int w, int h) {
            OpenCvSharp.Rect roi = new(x, y, w, h);
            Mat croppedMat = src.Clone(roi);
            Cv2.CvtColor(croppedMat, croppedMat, ColorConversionCodes.RGB2GRAY);
            Cv2.Threshold(croppedMat, croppedMat, 110, 255, ThresholdTypes.BinaryInv);
            return croppedMat.CountNonZero();
        }
    }

}

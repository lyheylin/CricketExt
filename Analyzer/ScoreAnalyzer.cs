using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;

namespace CricketExt.Analyzer
{
    internal class ScoreAnalyzer : IAnalyzer
    {

        //Takes a single frame and scan for scoreboard
        public void Scan(Mat frame)
        {
            Cv2.ImShow("Display", frame);
        }
    }
}

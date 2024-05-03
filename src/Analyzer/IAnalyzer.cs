using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CricketExt.Analyzer {
    internal interface IAnalyzer {
        Task<int[]> ScanAsync(VideoCapture video);
        string[] GetResult();
    }
}

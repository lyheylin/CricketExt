﻿using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CricketExt.Analyzer {
    internal interface IAnalyzer {
        void Scan(Mat frame);
    }
}
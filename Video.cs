using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CricketExt {
    public class Video {
        readonly VideoCapture video;
        public Video(FileInfo file) {
            String filePath = file.FullName;
            video = new(filePath);

            if (!video.IsOpened())
                Debug.WriteLine($"err: Can not open: {filePath}");
        }

        public VideoCapture Get() { return video;}
        
        public void Release() { video.Release(); }
    }
}

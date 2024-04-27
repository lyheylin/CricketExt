using System.CommandLine;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using CricketExt.Analyzer;
using OpenCvSharp;

namespace CricketExt {
    public class CricketExt {
        static Video? video;
        const int JUMP_FRAMES = 10;
        static async Task<int> Main(String[] args) {
            //Command line
            RootCommand rootCommand = ParseCL();
            await rootCommand.InvokeAsync(args);

            if (video == null) return 0;
            //Init

            VideoCapture v = video!.Get();
            IAnalyzer analyzer = new ScoreAnalyzer();


            //int frameCount = v.FrameCount;
            //Debug.WriteLine($"Frame Count: {frameCount}");
            
            while (v.IsOpened()) {
                using Mat frame = new(v.FrameHeight, v.FrameWidth, MatType.CV_8UC3);

                v.PosFrames = v.PosFrames + JUMP_FRAMES;
                bool next = v.Read(frame);
                if (next) 
                    analyzer.Scan(frame);
                else
                    Debug.WriteLine("End of video");                

                int key = Cv2.WaitKey(0);
                if ((key & 0xFF) == Convert.ToUInt32('q'))
                    break;
            }


            //Clean up
            video.Release();
            Cv2.DestroyAllWindows();//clean this

            return 0; 
        }

        static RootCommand ParseCL() {
            var fileOption = new Option<FileInfo?>(
               name: "--file",
               description: "Video file to analyze.");
            var threadsOption = new Option<int>(
                name: "--threads",
                description: "Number of threads to use.",
                getDefaultValue: () => 1
                );
            var rootCommand = new RootCommand("Capture screenshots from a video and read the data on the scoreboard shown in the video.");

            rootCommand.AddOption(fileOption);
            rootCommand.AddOption(threadsOption);
            //Bind handlers
            rootCommand.SetHandler(f => { ReadFile(f!); }, fileOption);

            return rootCommand; 
        }

        static void ReadFile(FileInfo file) {
            Debug.WriteLine($"Reading File: {file.Name}");
            video = new Video(file);
        }
    }
}   


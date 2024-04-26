using System.CommandLine;
using System.Diagnostics;
using OpenCvSharp;

namespace CricketExt {
    public class CricketExt {
        static async Task<int> Main(String[] args) {
            // Command line
            RootCommand rootCommand = ParseCL(args);
            await rootCommand.InvokeAsync(args);

            return 0; 
        }

        static RootCommand ParseCL(String[] args) {
            var file = new Option<FileInfo?>(
               name: "--file",
               description: "Video file to analyze.");
            var rootCommand = new RootCommand("Capture screenshots from a video and read the data on the scoreboard shown in the video.");

            rootCommand.AddOption(file);
            //Bind handlers
            rootCommand.SetHandler(f => { ReadFile(f!); }, file);

            return rootCommand; 
        }

        static void ReadFile(FileInfo file) {
            Debug.WriteLine($"Reading File: {file.Name}");
            String filePath = file.FullName;
            VideoCapture video = new VideoCapture(filePath);
            
            if (!video.IsOpened()) 
                Debug.WriteLine($"err: Can not open: {filePath}");

            while (video.IsOpened()) {
                Mat frame;
                frame = new Mat(video.FrameHeight, video.FrameWidth, MatType.CV_8UC3);
                bool next = video.Read(frame);
                if (next)
                    Cv2.ImShow("Display", frame);
                else
                    Debug.WriteLine("End of video");


                int key = Cv2.WaitKey(0);
                if ((key & 0xFF) == Convert.ToUInt32('q'))
                    break;
                
            }

            video.Release();
            Cv2.DestroyAllWindows();//debug;move
        }
    }
}   


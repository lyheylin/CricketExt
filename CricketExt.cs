using System.Collections.Concurrent;
using System.CommandLine;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using CricketExt.Analyzer;
using OpenCvSharp;
using CricketExt.DataTypes;

namespace CricketExt {
    public class CricketExt {
        static Video? video;
        static FileInfo? inputPath;
        static FileInfo? outputPath;
        static async Task<int> Main(String[] args) {
            //Command line
            RootCommand rootCommand = ParseCL();
            await rootCommand.InvokeAsync(args);

            video = new Video(inputPath!);
            if (video == null) return 0; //Add error message

            //Init
            VideoCapture v = video.Get();
            if (v == null) return 0; //Add error message
            IAnalyzer analyzer = new ScoreAnalyzer();

            await analyzer.ScanAsync(v);
            String[] result = analyzer.GetResult();

            OutputFile(result);
            return 0;
        }

        private static RootCommand ParseCL() {
            var fileOption = new Option<FileInfo?>(
               name: "--file",
               description: "Video file to analyze."
               );
            var threadsOption = new Option<int>(
                name: "--threads",
                description: "Number of threads to use.",
                getDefaultValue: () => 1
                ) ;
            var outputOption = new Option<FileInfo?>(
                name: "--output",
                description: "Optional destination .csv file path."
                );


            var rootCommand = new RootCommand("Capture screenshots from a video and read the data on the scoreboard shown in the video.");

            rootCommand.AddOption(fileOption);
            rootCommand.AddOption(threadsOption);
            rootCommand.AddOption(outputOption);
            //Bind handlers
            rootCommand.SetHandler( (f,o) => { HandleCommandLine(f!, o!); }, fileOption, outputOption);

            return rootCommand;
        }

        private static void HandleCommandLine(FileInfo input, FileInfo output) {
            Debug.WriteLine($"Reading File: {input.Name}");
            inputPath = input;
            outputPath = output;
        }

        private static void OutputFile(String[] result) {
            String output = @$"./output/{inputPath!.Name.Substring(0, inputPath!.Name.Length-inputPath!.Extension.Length)}.csv";
            if (outputPath != null) output = outputPath.FullName;
            Directory.CreateDirectory(@"./output");
            using StreamWriter outputFile = new StreamWriter(output);
            foreach (String s in result) outputFile.WriteLine(s);
        }
    }
}   


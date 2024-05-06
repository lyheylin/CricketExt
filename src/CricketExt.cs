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
        static bool header = true;
        static async Task<int> Main(string[] args) {
            //Command line
            RootCommand rootCommand = ParseCL();
            await rootCommand.InvokeAsync(args);

            video = new Video(inputPath!);
            if (video == null) return 0; //Add error message

            //Init
            ProcessUtil.SetVariables();
            VideoCapture v = video.Get();
            if (v == null) return 0; //Add error message
            IAnalyzer analyzer = new ScoreAnalyzer();

            await analyzer.ScanAsync(v);
            string[] result = analyzer.GetResult(header);

            OutputFile(result);
            return 0;
        }

        /// <summary>
        /// Command line parser.
        /// </summary>
        /// <returns></returns>
        private static RootCommand ParseCL() {
            var fileOption = new Option<FileInfo?>(
               name: "--file",
               description: "Video file to analyze."
               );
            var outputOption = new Option<FileInfo?>(
                name: "--output",
                description: "Optional destination .csv file path."
                );
            var headerOption = new Option<bool>(
                name: "--header",
                description: "Option to print header for output file.",
                getDefaultValue: () => true
                );


            var rootCommand = new RootCommand("Capture screenshots from a video and read the data on the scoreboard shown in the video.");

            rootCommand.AddOption(fileOption);
            rootCommand.AddOption(outputOption);
            rootCommand.AddOption(headerOption);
            //Bind handlers
            rootCommand.SetHandler((f, o, h) => { HandleCommandLine(f!, o!, h!); }, fileOption, outputOption, headerOption);

            return rootCommand;
        }

        /// <summary>
        /// Commandline handler.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        private static void HandleCommandLine(FileInfo input, FileInfo output, bool h) {
            Debug.WriteLine($"Reading File: {input.Name}");
            inputPath = input;
            outputPath = output;
            header = h;
        }

        private static void OutputFile(string[] result) {
            string output = @$"./output/{inputPath!.Name.Substring(0, inputPath!.Name.Length - inputPath!.Extension.Length)}.csv";
            if (outputPath != null) output = outputPath.FullName;
            Directory.CreateDirectory(@"./output");
            using StreamWriter outputFile = new StreamWriter(output);
            foreach (string s in result) outputFile.WriteLine(s);
        }
    }
}


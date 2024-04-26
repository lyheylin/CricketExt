using System.CommandLine;
using System.Diagnostics;

namespace CricketExt {
    public class CricketExt {
        static async Task<int> Main(String[] args) {
            Debug.WriteLine($"Start program");
            var file = new Option<FileInfo?>(
                name: "--file",
                description: "Video file to analyze.");

            var rootCommand = new RootCommand("Capture screenshots from a video and read the data on the scoreboard shown in the video.");

            rootCommand.AddOption(file);
            rootCommand.SetHandler(f => { ReadFile(f!); }, file);

            await rootCommand.InvokeAsync(args);
            return 0; 
        }

        static void ReadFile(FileInfo file) {
            Debug.WriteLine($"Reading File: {file.Name}");
        }
    }
}   

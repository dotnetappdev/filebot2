using CommandLine;
using RenameIt.CLI;
using Serilog;

// Configure logging
LoggingConfig.ConfigureLogging();

try
{
    // Configure parser to show help when no arguments provided
    var parser = new Parser(with => {
        with.HelpWriter = Console.Out;
        with.AutoHelp = true;
        with.AutoVersion = true;
    });

    // If no arguments provided, show help
    if (args.Length == 0)
    {
        Console.WriteLine("RenameIt CLI - Media File Renaming Tool");
        Console.WriteLine("========================================");
        Console.WriteLine();
        Console.WriteLine("USAGE:");
        Console.WriteLine("  renameit <command> [options]");
        Console.WriteLine();
        Console.WriteLine("COMMANDS:");
        Console.WriteLine("  rename      Rename files using a format pattern");
        Console.WriteLine("  preview     Preview renamed files without applying changes");
        Console.WriteLine("  batch       Execute a batch script file with multiple operations");
        Console.WriteLine();
        Console.WriteLine("OPTIONS:");
        Console.WriteLine("  --help      Display help for a specific command");
        Console.WriteLine("  --version   Display version information");
        Console.WriteLine();
        Console.WriteLine("EXAMPLES:");
        Console.WriteLine("  renameit rename \"/media/tv\" \"{n} - {s00e00} - {t}\" -s TheMovieDB");
        Console.WriteLine("  renameit preview \"/media/movies\" \"{n} ({y})\" -r");
        Console.WriteLine("  renameit batch organize-media.txt --dry-run");
        Console.WriteLine();
        Console.WriteLine("For more information on a specific command, use:");
        Console.WriteLine("  renameit <command> --help");
        Console.WriteLine();
        return 0;
    }

    Log.Information("Command line arguments: {Args}", string.Join(" ", args));

    return await parser.ParseArguments<RenameOptions, PreviewOptions, BatchOptions>(args)
        .MapResult(
            (RenameOptions opts) => CommandHandlers.RenameAsync(opts),
            (PreviewOptions opts) => CommandHandlers.PreviewAsync(opts),
            (BatchOptions opts) => CommandHandlers.BatchAsync(opts),
            errs => Task.FromResult(1));
}
finally
{
    LoggingConfig.CloseLogging();
}

namespace RenameIt.CLI
{
    [Verb("rename", HelpText = "Rename files using a format pattern")]
    public class RenameOptions
    {
        [Value(0, Required = true, MetaName = "input", HelpText = "Input file or directory path")]
        public string Input { get; set; } = string.Empty;

        [Value(1, Required = true, MetaName = "pattern", HelpText = "Format pattern (e.g., '{n} - {s00e00} - {t}')")]
        public string Pattern { get; set; } = string.Empty;

        [Option('s', "source", Default = "TheMovieDB", HelpText = "Metadata source: TheMovieDB, TheTVDB, or TVMaze")]
        public string Source { get; set; } = "TheMovieDB";

        [Option('r', "recursive", Default = false, HelpText = "Process subdirectories recursively")]
        public bool Recursive { get; set; }

        [Option('b', "backup", Default = false, HelpText = "Create backup before renaming")]
        public bool Backup { get; set; }

        [Option('d', "dry-run", Default = false, HelpText = "Preview changes without renaming")]
        public bool DryRun { get; set; }

        [Option('o', "output", Required = false, HelpText = "Output directory (default: rename in place)")]
        public string? OutputDirectory { get; set; }

        [Option('f', "filter", Required = false, HelpText = "File extension filter (e.g., 'mkv,mp4,avi')")]
        public string? FileFilter { get; set; }

        [Option('v', "verbose", Default = false, HelpText = "Enable verbose output")]
        public bool Verbose { get; set; }

        [Option('q', "quiet", Default = false, HelpText = "Suppress all output except errors")]
        public bool Quiet { get; set; }

        [Option("skip-existing", Default = false, HelpText = "Skip files if target already exists")]
        public bool SkipExisting { get; set; }

        [Option("overwrite", Default = false, HelpText = "Overwrite existing files")]
        public bool Overwrite { get; set; }

        [Option("max-depth", Default = -1, HelpText = "Maximum directory depth for recursive operations (-1 = unlimited)")]
        public int MaxDepth { get; set; }

        [Option("api-key", Required = false, HelpText = "API key for metadata provider")]
        public string? ApiKey { get; set; }

        [Option("language", Default = "en", HelpText = "Language code for metadata (e.g., 'en', 'de', 'fr')")]
        public string Language { get; set; } = "en";
    }

    [Verb("preview", HelpText = "Preview renamed files without applying changes")]
    public class PreviewOptions
    {
        [Value(0, Required = true, MetaName = "input", HelpText = "Input file or directory path")]
        public string Input { get; set; } = string.Empty;

        [Value(1, Required = true, MetaName = "pattern", HelpText = "Format pattern")]
        public string Pattern { get; set; } = string.Empty;

        [Option('s', "source", Default = "TheMovieDB", HelpText = "Metadata source")]
        public string Source { get; set; } = "TheMovieDB";

        [Option('r', "recursive", Default = false, HelpText = "Process subdirectories recursively")]
        public bool Recursive { get; set; }

        [Option('o', "output", Required = false, HelpText = "Output directory (default: rename in place)")]
        public string? OutputDirectory { get; set; }

        [Option('f', "filter", Required = false, HelpText = "File extension filter (e.g., 'mkv,mp4,avi')")]
        public string? FileFilter { get; set; }

        [Option('v', "verbose", Default = false, HelpText = "Enable verbose output")]
        public bool Verbose { get; set; }

        [Option("max-depth", Default = -1, HelpText = "Maximum directory depth for recursive operations (-1 = unlimited)")]
        public int MaxDepth { get; set; }

        [Option("api-key", Required = false, HelpText = "API key for metadata provider")]
        public string? ApiKey { get; set; }

        [Option("language", Default = "en", HelpText = "Language code for metadata (e.g., 'en', 'de', 'fr')")]
        public string Language { get; set; } = "en";
    }

    [Verb("batch", HelpText = "Execute a batch script file")]
    public class BatchOptions
    {
        [Value(0, Required = true, MetaName = "script", HelpText = "Path to batch script file")]
        public string Script { get; set; } = string.Empty;

        [Option('d', "dry-run", Default = false, HelpText = "Preview changes without renaming")]
        public bool DryRun { get; set; }

        [Option('v', "verbose", Default = false, HelpText = "Enable verbose output")]
        public bool Verbose { get; set; }

        [Option('q', "quiet", Default = false, HelpText = "Suppress all output except errors")]
        public bool Quiet { get; set; }

        [Option("continue-on-error", Default = false, HelpText = "Continue processing if a command fails")]
        public bool ContinueOnError { get; set; }
    }
}

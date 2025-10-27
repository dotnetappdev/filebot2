using CommandLine;
using RenameIt.CLI;

return await Parser.Default.ParseArguments<RenameOptions, PreviewOptions, BatchOptions>(args)
    .MapResult(
        (RenameOptions opts) => CommandHandlers.RenameAsync(opts.Input, opts.Pattern, opts.Source, opts.Recursive, opts.Backup, opts.DryRun),
        (PreviewOptions opts) => CommandHandlers.PreviewAsync(opts.Input, opts.Pattern, opts.Source, opts.Recursive),
        (BatchOptions opts) => CommandHandlers.BatchAsync(opts.Script, opts.DryRun),
        errs => Task.FromResult(1));

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
    }

    [Verb("batch", HelpText = "Execute a batch script file")]
    public class BatchOptions
    {
        [Value(0, Required = true, MetaName = "script", HelpText = "Path to batch script file")]
        public string Script { get; set; } = string.Empty;

        [Option('d', "dry-run", Default = false, HelpText = "Preview changes without renaming")]
        public bool DryRun { get; set; }
    }
}

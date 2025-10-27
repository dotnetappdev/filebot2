using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using RenameIt;

namespace RenameIt.CLI
{
    public static class CommandHandlers
    {
        public static async Task<int> RenameAsync(RenameOptions opts)
        {
            var files = GetFiles(opts.Input, opts.Recursive, opts.FileFilter, opts.MaxDepth);
            
            if (files.Count == 0)
            {
                if (!opts.Quiet)
                    Console.WriteLine("No files found.");
                return 1;
            }

            if (!opts.Quiet)
            {
                Console.WriteLine($"Found {files.Count} file(s).");
                Console.WriteLine($"Pattern: {opts.Pattern}");
                Console.WriteLine($"Source: {opts.Source}");
                if (opts.OutputDirectory != null)
                    Console.WriteLine($"Output Directory: {opts.OutputDirectory}");
                Console.WriteLine();
            }

            var parser = new FileNameParser();
            var renamer = new FileRenamer(opts.Source);
            var results = new List<(string Original, string Renamed, bool Success, string Error)>();

            foreach (var file in files)
            {
                try
                {
                    var fileName = Path.GetFileName(file);
                    var metadata = parser.ParseFileName(fileName);
                    
                    // Enhance metadata with online data (placeholder - can be expanded)
                    await EnhanceMetadataAsync(metadata, opts.Source);
                    
                    var newFileName = renamer.ApplyFormat(opts.Pattern, metadata, fileName);
                    
                    // Determine output path
                    string newPath;
                    if (!string.IsNullOrEmpty(opts.OutputDirectory))
                    {
                        Directory.CreateDirectory(opts.OutputDirectory);
                        newPath = Path.Combine(opts.OutputDirectory, newFileName);
                    }
                    else
                    {
                        newPath = Path.Combine(Path.GetDirectoryName(file) ?? "", newFileName);
                    }

                    // Check if target exists
                    if (File.Exists(newPath) && newPath != file)
                    {
                        if (opts.SkipExisting)
                        {
                            if (opts.Verbose && !opts.Quiet)
                                Console.WriteLine($"  SKIPPED (exists): {Path.GetFileName(file)}");
                            continue;
                        }
                        else if (!opts.Overwrite)
                        {
                            if (!opts.Quiet)
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine($"  WARNING: Target exists: {newFileName}");
                                Console.ResetColor();
                            results.Add((file, newPath, false, "Target file already exists"));
                            continue;
                        }
                    }

                    results.Add((file, newPath, true, string.Empty));
                }
                catch (Exception ex)
                {
                    results.Add((file, string.Empty, false, ex.Message));
                }
            }

            // Display results
            if (!opts.Quiet)
            {
                foreach (var result in results)
                {
                    if (result.Success)
                    {
                        if (opts.Verbose)
                        {
                            Console.WriteLine($"  {result.Original}");
                            Console.WriteLine($"    -> {result.Renamed}");
                        }
                        else
                        {
                            Console.WriteLine($"  {Path.GetFileName(result.Original)} -> {Path.GetFileName(result.Renamed)}");
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"  ERROR: {Path.GetFileName(result.Original)} - {result.Error}");
                        Console.ResetColor();
                    }
                }
            }

            if (opts.DryRun)
            {
                if (!opts.Quiet)
                {
                    Console.WriteLine();
                    Console.WriteLine("DRY RUN - No files were renamed.");
                }
                return 0;
            }

            // Perform actual renaming
            if (!opts.Quiet)
                Console.WriteLine();
            var successCount = 0;
            var failCount = 0;

            foreach (var result in results.Where(r => r.Success))
            {
                try
                {
                    if (opts.Backup)
                    {
                        var backupPath = result.Original + ".backup";
                        File.Copy(result.Original, backupPath, true);
                    }

                    // Use copy/delete for output directory, move for in-place
                    if (!string.IsNullOrEmpty(opts.OutputDirectory))
                    {
                        File.Copy(result.Original, result.Renamed, opts.Overwrite);
                    }
                    else
                    {
                        // Skip if source and target are the same
                        if (result.Original != result.Renamed)
                        {
                            File.Move(result.Original, result.Renamed, opts.Overwrite);
                        }
                    }
                    successCount++;
                    
                    if (opts.Verbose && !opts.Quiet)
                        Console.WriteLine($"  ✓ Renamed: {Path.GetFileName(result.Original)}");
                }
                catch (Exception ex)
                {
                    if (!opts.Quiet)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"  ✗ Failed to rename {Path.GetFileName(result.Original)}: {ex.Message}");
                        Console.ResetColor();
                    }
                    failCount++;
                }
            }

            if (!opts.Quiet)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Successfully renamed {successCount} file(s).");
                Console.ResetColor();
                
                if (failCount > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Failed to rename {failCount} file(s).");
                    Console.ResetColor();
                }
            }

            return failCount > 0 ? 1 : 0;
        }

        public static async Task<int> PreviewAsync(PreviewOptions opts)
        {
            var renameOpts = new RenameOptions
            {
                Input = opts.Input,
                Pattern = opts.Pattern,
                Source = opts.Source,
                Recursive = opts.Recursive,
                OutputDirectory = opts.OutputDirectory,
                FileFilter = opts.FileFilter,
                Verbose = opts.Verbose,
                MaxDepth = opts.MaxDepth,
                ApiKey = opts.ApiKey,
                Language = opts.Language,
                DryRun = true,
                Backup = false,
                Quiet = false
            };
            return await RenameAsync(renameOpts);
        }

        public static async Task<int> BatchAsync(BatchOptions opts)
        {
            if (!File.Exists(opts.Script))
            {
                if (!opts.Quiet)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Script file not found: {opts.Script}");
                    Console.ResetColor();
                }
                return 1;
            }

            var lines = await File.ReadAllLinesAsync(opts.Script);
            var commands = ParseBatchScript(lines);

            if (!opts.Quiet)
            {
                Console.WriteLine($"Executing batch script: {opts.Script}");
                Console.WriteLine($"Found {commands.Count} command(s).");
                Console.WriteLine();
            }

            int totalErrors = 0;
            foreach (var (index, command) in commands.Select((cmd, idx) => (idx + 1, cmd)))
            {
                if (!opts.Quiet)
                    Console.WriteLine($"--- Command {index}/{commands.Count} ---");
                
                try
                {
                    var renameOpts = new RenameOptions
                    {
                        Input = command.Input,
                        Pattern = command.Pattern,
                        Source = command.Source,
                        Recursive = command.Recursive,
                        Backup = command.Backup,
                        DryRun = opts.DryRun,
                        Quiet = opts.Quiet,
                        Verbose = opts.Verbose
                    };
                    
                    var result = await RenameAsync(renameOpts);
                    if (result != 0)
                    {
                        totalErrors++;
                        if (!opts.ContinueOnError)
                        {
                            if (!opts.Quiet)
                                Console.WriteLine("Batch execution stopped due to error.");
                            return result;
                        }
                    }
                }
                catch (Exception ex)
                {
                    totalErrors++;
                    if (!opts.Quiet)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Error executing command: {ex.Message}");
                        Console.ResetColor();
                    }
                    
                    if (!opts.ContinueOnError)
                        return 1;
                }

                if (!opts.Quiet)
                    Console.WriteLine();
            }

            if (!opts.Quiet)
            {
                if (totalErrors > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Batch script completed with {totalErrors} error(s).");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine("Batch script completed successfully.");
                }
            }
            
            return totalErrors > 0 ? 1 : 0;
        }

        private static List<string> GetFiles(string input, bool recursive, string? fileFilter = null, int maxDepth = -1)
        {
            var files = new List<string>();

            if (File.Exists(input))
            {
                files.Add(input);
            }
            else if (Directory.Exists(input))
            {
                var searchOption = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
                
                // Determine file extensions to filter
                var extensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                if (!string.IsNullOrEmpty(fileFilter))
                {
                    foreach (var ext in fileFilter.Split(','))
                    {
                        var trimmed = ext.Trim();
                        if (!trimmed.StartsWith("."))
                            trimmed = "." + trimmed;
                        extensions.Add(trimmed);
                    }
                }
                else
                {
                    // Default video extensions
                    var videoExtensions = new[] { ".mkv", ".mp4", ".avi", ".mov", ".wmv", ".flv", ".m4v", ".mpg", ".mpeg" };
                    foreach (var ext in videoExtensions)
                        extensions.Add(ext);
                }
                
                var allFiles = Directory.GetFiles(input, "*.*", searchOption)
                    .Where(f => extensions.Contains(Path.GetExtension(f)));

                // Apply max depth filter if specified
                if (maxDepth >= 0 && recursive)
                {
                    var inputDepth = input.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                        .Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).Length;
                    
                    allFiles = allFiles.Where(f =>
                    {
                        var fileDepth = Path.GetDirectoryName(f)!
                            .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                            .Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).Length;
                        return (fileDepth - inputDepth) <= maxDepth;
                    });
                }

                files.AddRange(allFiles);
            }

            return files;
        }

        private static async Task EnhanceMetadataAsync(FileMetadata metadata, string source)
        {
            // This is a placeholder for future metadata enhancement
            // In a real implementation, this would query the metadata provider
            await Task.CompletedTask;
        }

        private static List<BatchCommand> ParseBatchScript(string[] lines)
        {
            var commands = new List<BatchCommand>();
            BatchCommand? currentCommand = null;

            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                
                // Skip empty lines and comments
                if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("#") || trimmed.StartsWith("//"))
                    continue;

                if (trimmed.StartsWith("[") && trimmed.EndsWith("]"))
                {
                    // Start new command section
                    if (currentCommand != null)
                    {
                        commands.Add(currentCommand);
                    }
                    currentCommand = new BatchCommand();
                }
                else if (currentCommand != null)
                {
                    // Parse command properties
                    var parts = trimmed.Split('=', 2);
                    if (parts.Length == 2)
                    {
                        var key = parts[0].Trim().ToLowerInvariant();
                        var value = parts[1].Trim();

                        switch (key)
                        {
                            case "input":
                                currentCommand.Input = value;
                                break;
                            case "pattern":
                                currentCommand.Pattern = value;
                                break;
                            case "source":
                                currentCommand.Source = value;
                                break;
                            case "recursive":
                                currentCommand.Recursive = bool.Parse(value);
                                break;
                            case "backup":
                                currentCommand.Backup = bool.Parse(value);
                                break;
                        }
                    }
                }
            }

            // Add last command
            if (currentCommand != null)
            {
                commands.Add(currentCommand);
            }

            return commands;
        }
    }

    internal class BatchCommand
    {
        public string Input { get; set; } = string.Empty;
        public string Pattern { get; set; } = string.Empty;
        public string Source { get; set; } = "TheMovieDB";
        public bool Recursive { get; set; } = false;
        public bool Backup { get; set; } = false;
    }
}

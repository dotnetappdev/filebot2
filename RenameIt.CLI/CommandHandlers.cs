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
        public static async Task<int> RenameAsync(string input, string pattern, string source, bool recursive, bool backup, bool dryRun)
        {
            var files = GetFiles(input, recursive);
            
            if (files.Count == 0)
            {
                Console.WriteLine("No files found.");
                return 1;
            }

            Console.WriteLine($"Found {files.Count} file(s).");
            Console.WriteLine($"Pattern: {pattern}");
            Console.WriteLine($"Source: {source}");
            Console.WriteLine();

            var parser = new FileNameParser();
            var renamer = new FileRenamer(source);
            var results = new List<(string Original, string Renamed, bool Success, string Error)>();

            foreach (var file in files)
            {
                try
                {
                    var fileName = Path.GetFileName(file);
                    var metadata = parser.ParseFileName(fileName);
                    
                    // Enhance metadata with online data (placeholder - can be expanded)
                    await EnhanceMetadataAsync(metadata, source);
                    
                    var newFileName = renamer.ApplyFormat(pattern, metadata, fileName);
                    var newPath = Path.Combine(Path.GetDirectoryName(file) ?? "", newFileName);

                    results.Add((file, newPath, true, string.Empty));
                }
                catch (Exception ex)
                {
                    results.Add((file, string.Empty, false, ex.Message));
                }
            }

            // Display results
            foreach (var result in results)
            {
                if (result.Success)
                {
                    Console.WriteLine($"  {Path.GetFileName(result.Original)} -> {Path.GetFileName(result.Renamed)}");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"  ERROR: {Path.GetFileName(result.Original)} - {result.Error}");
                    Console.ResetColor();
                }
            }

            if (dryRun)
            {
                Console.WriteLine();
                Console.WriteLine("DRY RUN - No files were renamed.");
                return 0;
            }

            // Perform actual renaming
            Console.WriteLine();
            var successCount = 0;
            var failCount = 0;

            foreach (var result in results.Where(r => r.Success))
            {
                try
                {
                    if (backup)
                    {
                        var backupPath = result.Original + ".backup";
                        File.Copy(result.Original, backupPath, true);
                    }

                    File.Move(result.Original, result.Renamed);
                    successCount++;
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Failed to rename {Path.GetFileName(result.Original)}: {ex.Message}");
                    Console.ResetColor();
                    failCount++;
                }
            }

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

            return failCount > 0 ? 1 : 0;
        }

        public static async Task<int> PreviewAsync(string input, string pattern, string source, bool recursive)
        {
            return await RenameAsync(input, pattern, source, recursive, backup: false, dryRun: true);
        }

        public static async Task<int> BatchAsync(string scriptPath, bool dryRun)
        {
            if (!File.Exists(scriptPath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Script file not found: {scriptPath}");
                Console.ResetColor();
                return 1;
            }

            var lines = await File.ReadAllLinesAsync(scriptPath);
            var commands = ParseBatchScript(lines);

            Console.WriteLine($"Executing batch script: {scriptPath}");
            Console.WriteLine($"Found {commands.Count} command(s).");
            Console.WriteLine();

            foreach (var (index, command) in commands.Select((cmd, idx) => (idx + 1, cmd)))
            {
                Console.WriteLine($"--- Command {index}/{commands.Count} ---");
                
                try
                {
                    await RenameAsync(
                        command.Input,
                        command.Pattern,
                        command.Source,
                        command.Recursive,
                        command.Backup,
                        dryRun);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Error executing command: {ex.Message}");
                    Console.ResetColor();
                }

                Console.WriteLine();
            }

            Console.WriteLine("Batch script completed.");
            return 0;
        }

        private static List<string> GetFiles(string input, bool recursive)
        {
            var files = new List<string>();

            if (File.Exists(input))
            {
                files.Add(input);
            }
            else if (Directory.Exists(input))
            {
                var searchOption = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
                var videoExtensions = new[] { ".mkv", ".mp4", ".avi", ".mov", ".wmv", ".flv", ".m4v", ".mpg", ".mpeg" };
                
                files.AddRange(Directory.GetFiles(input, "*.*", searchOption)
                    .Where(f => videoExtensions.Contains(Path.GetExtension(f).ToLowerInvariant())));
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

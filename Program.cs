using System.Diagnostics;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace GBFRDataConverter;

public class Program
{
    private static ToolSettings _toolSettings = null!;

    public static void Main(string[] args)
    {
        Console.InputEncoding = Encoding.UTF8;
        Console.OutputEncoding = Encoding.UTF8;

        Console.WriteLine("=== GBFR Data Converter ===");
        Console.WriteLine();

        // Load configuration
        if (!LoadConfiguration())
        {
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            return;
        }

        // Get input path
        string? inputPath = GetInputPath();
        if (inputPath == null)
        {
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            return;
        }

        // Validate paths
        if (!ValidatePaths())
        {
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            return;
        }

        // Get all convertible files
        var files = GetConvertibleFiles(inputPath);
        if (files.Count == 0)
        {
            Console.WriteLine("No convertible files found (.msg or .bxm)");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            return;
        }

        Console.WriteLine($"Found {files.Count} file(s) to convert");
        Console.WriteLine();

        // Convert all files
        var result = ConvertFiles(files);

        // Display results
        DisplayResults(result);

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    /// <summary>
    /// Load configuration from appsettings.json
    /// </summary>
    private static bool LoadConfiguration()
    {
        try
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .Build();

            _toolSettings = configuration.GetSection("ToolSettings").Get<ToolSettings>()
                ?? throw new InvalidOperationException("Failed to load ToolSettings from configuration");

            Console.WriteLine("Configuration loaded successfully");
            Console.WriteLine();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading configuration: {ex.Message}");
            Console.WriteLine("Please ensure appsettings.json exists and is properly formatted.");
            return false;
        }
    }

    /// <summary>
    /// Get and validate user input path
    /// </summary>
    private static string? GetInputPath()
    {
        Console.Write("Enter the path to convert: ");
        string? inputPath = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(inputPath) || !Directory.Exists(inputPath))
        {
            Console.WriteLine("Error: Directory does not exist!");
            return null;
        }

        return inputPath;
    }

    /// <summary>
    /// Validate all required tool paths exist
    /// </summary>
    private static bool ValidatePaths()
    {
        if (!File.Exists(_toolSettings.MsgPack2JsonPath))
        {
            Console.WriteLine($"Error: Cannot find MsgPack2Json.exe at {_toolSettings.MsgPack2JsonPath}");
            return false;
        }

        if (!File.Exists(_toolSettings.NierCliPath))
        {
            Console.WriteLine($"Error: Cannot find nier_cli.exe at {_toolSettings.NierCliPath}");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Get all convertible files (.msg and .bxm) from directory
    /// </summary>
    private static List<string> GetConvertibleFiles(string directoryPath)
    {
        var files = new List<string>();

        try
        {
            // Get all .msg files
            files.AddRange(Directory.GetFiles(directoryPath, "*.msg", SearchOption.AllDirectories));

            // Get all .bxm files
            files.AddRange(Directory.GetFiles(directoryPath, "*.bxm", SearchOption.AllDirectories));

            return files;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error scanning directory: {ex.Message}");
            return files;
        }
    }

    /// <summary>
    /// Convert all files
    /// </summary>
    private static ConversionResult ConvertFiles(List<string> files)
    {
        int successCount = 0;
        int failCount = 0;
        int skippedCount = 0;

        foreach (string filePath in files)
        {
            Console.WriteLine($"Processing: {filePath}");

            string extension = Path.GetExtension(filePath).ToLowerInvariant();
            bool success = false;

            if (extension == ".msg")
            {
                success = ConvertMsgToJson(filePath);
            }
            else if (extension == ".bxm")
            {
                success = ConvertBxmToXml(filePath);
            }
            else
            {
                Console.WriteLine("  Skipped: Unsupported file type");
                skippedCount++;
                Console.WriteLine();
                continue;
            }

            if (success)
            {
                successCount++;
            }
            else
            {
                failCount++;
            }

            Console.WriteLine();
        }

        return new ConversionResult
        {
            SuccessCount = successCount,
            FailCount = failCount,
            SkippedCount = skippedCount
        };
    }

    /// <summary>
    /// Convert .msg file to .json using MsgPack2Json.exe
    /// </summary>
    private static bool ConvertMsgToJson(string msgFilePath)
    {
        try
        {
            string jsonFilePath = Path.ChangeExtension(msgFilePath, ".json");

            // Check if output file already exists
            if (File.Exists(jsonFilePath))
            {
                Console.WriteLine($"  Skipped: JSON file already exists at {jsonFilePath}");
                return true;
            }

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = _toolSettings.MsgPack2JsonPath,
                Arguments = $"json \"{msgFilePath}\" \"{jsonFilePath}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using (Process? process = Process.Start(startInfo))
            {
                if (process == null)
                {
                    Console.WriteLine("  Conversion failed: Unable to start process");
                    return false;
                }

                process.WaitForExit();

                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                if (process.ExitCode != 0)
                {
                    Console.WriteLine($"  Conversion failed: {error}");
                    return false;
                }
            }

            // Verify the converted file before deleting original
            if (!File.Exists(jsonFilePath) || new FileInfo(jsonFilePath).Length == 0)
            {
                Console.WriteLine("  Conversion failed: Output file is empty or missing");
                return false;
            }

            // Delete original file after successful conversion
            File.Delete(msgFilePath);
            Console.WriteLine($"  Success: Converted to {jsonFilePath}");
            Console.WriteLine($"  Deleted: {msgFilePath}");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  Error: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Convert .bxm file to .bxm.xml using nier_cli.exe
    /// </summary>
    private static bool ConvertBxmToXml(string bxmFilePath)
    {
        try
        {
            string xmlFilePath = bxmFilePath + ".xml";

            // Check if output file already exists
            if (File.Exists(xmlFilePath))
            {
                Console.WriteLine($"  Skipped: XML file already exists at {xmlFilePath}");
                return true;
            }

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = _toolSettings.NierCliPath,
                Arguments = $"\"{bxmFilePath}\" -o \"{xmlFilePath}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using (Process? process = Process.Start(startInfo))
            {
                if (process == null)
                {
                    Console.WriteLine("  Conversion failed: Unable to start process");
                    return false;
                }

                process.WaitForExit();

                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                if (process.ExitCode != 0)
                {
                    Console.WriteLine($"  Conversion failed: {error}");
                    return false;
                }
            }

            // Verify the converted file before deleting original
            if (!File.Exists(xmlFilePath) || new FileInfo(xmlFilePath).Length == 0)
            {
                Console.WriteLine("  Conversion failed: Output file is empty or missing");
                return false;
            }

            // Delete original file after successful conversion
            File.Delete(bxmFilePath);
            Console.WriteLine($"  Success: Converted to {xmlFilePath}");
            Console.WriteLine($"  Deleted: {bxmFilePath}");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  Error: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Display conversion results
    /// </summary>
    private static void DisplayResults(ConversionResult result)
    {
        Console.WriteLine("=== Conversion Complete ===");
        Console.WriteLine($"Success: {result.SuccessCount} file(s)");
        Console.WriteLine($"Failed: {result.FailCount} file(s)");
        Console.WriteLine($"Skipped: {result.SkippedCount} file(s)");
        Console.WriteLine($"Total: {result.TotalCount} file(s)");
        Console.WriteLine();
    }
}

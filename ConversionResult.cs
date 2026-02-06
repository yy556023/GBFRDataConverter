namespace GBFRDataConverter;

/// <summary>
/// Conversion result statistics
/// </summary>
public class ConversionResult
{
    /// <summary>
    /// Number of successfully converted files
    /// </summary>
    public int SuccessCount { get; set; }

    /// <summary>
    /// Number of failed files
    /// </summary>
    public int FailCount { get; set; }

    /// <summary>
    /// Number of skipped files
    /// </summary>
    public int SkippedCount { get; set; }

    /// <summary>
    /// Total number of files
    /// </summary>
    public int TotalCount => SuccessCount + FailCount + SkippedCount;
}

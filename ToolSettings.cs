namespace GBFRDataConverter;

/// <summary>
/// Tool configuration settings
/// </summary>
public class ToolSettings
{
    /// <summary>
    /// Path to MsgPack2Json.exe
    /// </summary>
    public string MsgPack2JsonPath { get; set; } = string.Empty;

    /// <summary>
    /// Path to nier_cli.exe
    /// </summary>
    public string NierCliPath { get; set; } = string.Empty;
}

# GBFR Data Converter

Simple tool to batch convert GBFR-related data files:
- `.msg` -> `.json`（透過 `MsgPack2Json.exe`）
- `.bxm` -> `.bxm.xml`（透過 `nier_cli.exe`）

## Requirements
- .NET SDK `net10.0`
- Two external tool executables
  - `MsgPack2Json.exe`
  - `nier_cli.exe`

## Configuration
Set the external tool paths in `appsettings.json`:
```json
{
  "ToolSettings": {
    "MsgPack2JsonPath": "E:\\path\\to\\MsgPack2Json.exe",
    "NierCliPath": "E:\\path\\to\\nier_cli.exe"
  }
}
```

## Usage
```powershell
dotnet restore
dotnet build
dotnet run --project GBFRDataConverter.csproj
```
After running, enter the folder path to convert, e.g. `E:\data\gbfr\dump`

## Notes
- After a successful conversion, the original `.msg` / `.bxm` files are deleted. Please back up first.
- If an output file with the same name already exists, it will be skipped.

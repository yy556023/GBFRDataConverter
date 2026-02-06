# GBFR Data Converter

將 GBFR 相關資料檔批次轉換的簡易工具：
- `.msg` -> `.json`（透過 `MsgPack2Json.exe`）
- `.bxm` -> `.bxm.xml`（透過 `nier_cli.exe`）

## 需求
- .NET SDK `net10.0`
- 兩個外部工具可執行檔
  - `MsgPack2Json.exe`
  - `nier_cli.exe`

## 設定
在 `appsettings.json` 設定外部工具路徑：
```json
{
  "ToolSettings": {
    "MsgPack2JsonPath": "E:\\path\\to\\MsgPack2Json.exe",
    "NierCliPath": "E:\\path\\to\\nier_cli.exe"
  }
}
```

## 使用方式
```powershell
dotnet restore
dotnet build
dotnet run --project GBFRDataConverter.csproj
```
執行後輸入要轉換的資料夾路徑，例如：`E:\data\gbfr\dump`

## 注意事項
- 成功轉換後，原始 `.msg` / `.bxm` 會被刪除，請先備份。
- 若同名輸出檔已存在，會跳過該檔案。

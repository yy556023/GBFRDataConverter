# Repository Guidelines

## Project Structure & Module Organization
- Root contains the C# console app: `Program.cs` (main workflow), `ToolSettings.cs` (config model), and `ConversionResult.cs` (summary stats).
- Configuration lives in `appsettings.json` and is copied to the build output on run.
- Build artifacts are in `bin/` and `obj/`.
 - Conversions are file-based: `.msg` -> `.json` via `MsgPack2Json.exe`, and `.bxm` -> `.bxm.xml` via `nier_cli.exe`.

## Build, Test, and Development Commands
- `dotnet restore`: Restore NuGet packages.
- `dotnet build`: Build the executable for `net10.0`.
- `dotnet run --project GBFRDataConverter.csproj`: Run the converter locally.
- `dotnet clean`: Remove build outputs.
 - Example run flow: launch the app, then enter a directory path when prompted (e.g., `E:\data\gbfr\dump`).

## Coding Style & Naming Conventions
- C# with nullable reference types enabled (`<Nullable>enable</Nullable>`).
- Indentation: 4 spaces.
- Naming: public types/members use `PascalCase`; private fields use `_camelCase` (see `_toolSettings`).
- Keep console output clear and user-directed; conversions log per file.

## Testing Guidelines
- No test project is present. If you add tests, use `dotnet test` and keep file names like `*Tests.cs`.
- Favor small, focused tests for conversion routines and path validation.

## Commit & Pull Request Guidelines
- This repository has no commit history yet. Use concise, imperative commits; `feat:`, `fix:`, `chore:` prefixes are recommended.
- PRs should include a summary, any relevant sample input/output paths, and note if behavior changes around file deletion.

## Configuration & Data Safety
- `appsettings.json` must point to `MsgPack2Json.exe` and `nier_cli.exe`.
- The converter deletes original `.msg` and `.bxm` files after successful conversion. Back up input directories before running.
 - Keep machine-specific paths out of commits when possible; prefer documenting required tools and adjusting `appsettings.json` locally.

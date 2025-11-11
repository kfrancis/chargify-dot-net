# .NET 8.0 Upgrade Plan

## Execution Steps

Execute steps below sequentially one by one in the order they are listed.

1. Validate that an .NET 8.0 SDK required for this upgrade is installed on the machine and if not, help to get it installed.
2. Ensure that the SDK version specified in global.json files is compatible with the .NET 8.0 upgrade.
3. Upgrade Source\ChargifyDotNet\ChargifyDotNet.csproj
4. Upgrade Source\ConsumerApp.NetCore\ConsumerApp.NetCore.csproj
5. Upgrade Source\ConsumerApp.Net452\ConsumerApp.Net452.csproj
6. Upgrade Source\ConsumerApp.Net40\ConsumerApp.Net40.csproj
7. Upgrade Source\ChargifyDotNet.Tests\ChargifyDotNet.Tests.csproj
8. Run unit tests to validate upgrade in the projects listed below:
   - Source\ChargifyDotNet.Tests\ChargifyDotNet.Tests.csproj

## Settings

This section contains settings and data used by execution steps.

### Excluded projects

Table below contains projects that do belong to the dependency graph for selected projects and should not be included in the upgrade.

| Project name | Description |
|:-------------|:-----------:|

### Aggregate NuGet packages modifications across all projects

NuGet packages used across all selected projects or their dependencies that need version update in projects that reference them.

| Package Name    | Current Version          | New Version | Description                                             |
|:----------------|:------------------------:|:-----------:|:--------------------------------------------------------|
| Newtonsoft.Json | 13.0.2                   | 13.0.4      | Recommended update for .NET 8.0 compatibility/perf      |
| PCLWebUtility   | 1.0.3                    |             | Incompatible with .NET 8.0; remove (no supported version) |

### Project upgrade details
This section contains details about each project upgrade and modifications that need to be done in the project.

#### Source\ChargifyDotNet\ChargifyDotNet.csproj modifications

Project properties changes:
  - Target frameworks should be changed from `netstandard2.0;net45;net40` to `netstandard2.0;net45;net40;net8.0`

NuGet packages changes:
  - Newtonsoft.Json should be updated from `13.0.2` to `13.0.4` (*recommended for .NET 8.0*)

Other changes:
  - Ensure code paths conditional on TFMs compile under net8.0.

#### Source\ConsumerApp.NetCore\ConsumerApp.NetCore.csproj modifications

Project properties changes:
  - Target framework should be changed from `netcoreapp3.1` to `net8.0`

NuGet packages changes:
  - Newtonsoft.Json should be updated from `13.0.2` to `13.0.4` (*recommended for .NET 8.0*)

Other changes:
  - Review any deprecated APIs between .NET Core 3.1 and .NET 8.0.

#### Source\ConsumerApp.Net452\ConsumerApp.Net452.csproj modifications

Project properties changes:
  - Convert project file to SDK-style.
  - Target framework should be changed from `.NETFramework,Version=v4.5.2` to `net8.0`

NuGet packages changes:
  - Newtonsoft.Json should be updated from `13.0.2` to `13.0.4` (*recommended for .NET 8.0*)
  - PCLWebUtility should be removed (`1.0.3`) (*incompatible with .NET 8.0*)

Other changes:
  - Replace any legacy configuration (App.config) usages with modern equivalents if needed.

#### Source\ConsumerApp.Net40\ConsumerApp.Net40.csproj modifications

Project properties changes:
  - Convert project file to SDK-style.
  - Target framework should be changed from `.NETFramework,Version=v4.0` to `net8.0`

NuGet packages changes:
  - Newtonsoft.Json should be updated from `13.0.2` to `13.0.4` (*recommended for .NET 8.0*)
  - PCLWebUtility should be removed (`1.0.3`) (*incompatible with .NET 8.0*)

Other changes:
  - Migrate any obsolete APIs from .NET 4.0 to modern equivalents.

#### Source\ChargifyDotNet.Tests\ChargifyDotNet.Tests.csproj modifications

Project properties changes:
  - Target frameworks should be changed from `net5.0;netcoreapp3.1;net45;net48` to `net5.0;netcoreapp3.1;net45;net48;net8.0`

NuGet packages changes:
  - Newtonsoft.Json should be updated from `13.0.2` to `13.0.4` (*recommended for .NET 8.0*)

Other changes:
  - Adjust any test framework dependencies if they have net8.0 specific versions.

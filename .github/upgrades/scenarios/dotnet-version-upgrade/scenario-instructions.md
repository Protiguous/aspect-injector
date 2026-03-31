# Scenario Instructions: dotnet-version-upgrade

## User Preferences

### Technical Preferences
- **Retarget to net10.0 when .NET 10 SDK is installed** — user requested that all projects be updated to `net10.0` once the SDK is available. Current behavior: projects are left at `net8.0` until SDK is installed.

### Execution Style
- Flow Mode: Automatic (run end-to-end, pause only when blocked)

## Notes
- Agent will prepare net10.0 changes but will not attempt to build or switch TFMs to net10.0 until the .NET 10 SDK is present on the machine.

# AMD Config

A powerful Windows Forms application for viewing and modifying AMD GPU registry DWORDs with built-in optimization tweaks.

## Features

- **DWORD Registry Viewer**: Browse and search through 1600+ AMD GPU registry entries
- **Live Registry Reading**: Real-time querying of Windows Registry values
- **Advanced Filtering**: Filter by status (Found/Missing/All) and search by name
- **Edit, Add, Delete**: Modify DWORD values with decimal/hex input support
- **Change History**: Track all modifications with undo and revert capabilities
- **Quick Tweaks Panel**: One-click application of 6 optimization tweaks:
  - Disable All Gatings (36 DWORDs)
  - Disable ASPM (8 DWORDs)
  - Disable Radeon Boost (1 DWORD)
  - Disable Logs (89 DWORDs)
  - Disable Debugs (27 DWORDs)
  - Disable Traces (12 DWORDs)
- **Automatic Backups**: Original values are backed up before applying tweaks
- **Smart Revert**: Restore original values or delete DWORDs that didn't exist before

## Requirements

- Windows 10/11
- .NET 6.0 Runtime
- Administrator privileges (for registry modifications)

## Usage

1. Run `AMD_DWORD_Viewer.exe` as Administrator
2. Browse and search AMD GPU DWORDs
3. Use the Tweaks panel on the right to apply optimizations
4. Track changes in History and use Undo if needed

## Building from Source

```bash
dotnet build -c Release
```

## Credits

Made by **yuu_0711**

## Warning

⚠️ Modifying registry values can affect system stability. Always create backups before making changes.

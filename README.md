# GPU Dword Manager

A Windows application for viewing and modifying AMD/Nvidia GPU registry DWORDs with built-in optimization tweaks.

## Features

- **Dual GPU Support**: Works with both AMD Radeon and Nvidia GeForce GPUs
- **DWORD Registry Viewer**: Browse and search through GPU registry entries
- **Live Registry Reading**: Real-time querying of Windows Registry values
- **Advanced Filtering**: Filter by status (Found/Missing/All) and search by name
- **Edit, Add, Delete**: Modify DWORD values with decimal/hex input support
- **Change History**: Track all modifications with undo and revert capabilities
- **Quick Tweaks Panel**: One-click optimization tweaks
  - **AMD Tweaks**: Disable Gatings, ASPM, Radeon Boost, Logs, Debugs, Traces
  - **Nvidia Tweaks**: Disable Power Management, Logging, Preemption, HDCP, ECC, Scrubbers + Enable Performance Mode
- **Automatic Backups**: Original values are backed up before applying tweaks
- **Smart Revert**: Restore original values or delete DWORDs that didn't exist before
  
## Screenshots

<img width="403" height="232" alt="image" src="https://github.com/user-attachments/assets/9c43d654-ac8e-4a62-8c6b-625c056c2f45" />
<img width="1202" height="753" alt="image" src="https://github.com/user-attachments/assets/55062d7a-84e2-4c18-9765-92e743886125" />
<img width="1201" height="749" alt="image" src="https://github.com/user-attachments/assets/4edd42a2-686d-43b1-a19e-91a15d7a46f7" />


## Requirements

- Windows 10/11
- .NET 8.0 Desktop Runtime ([Download](https://dotnet.microsoft.com/download/dotnet/8.0))
- Administrator privileges (for registry modifications)

## Usage

1. Run `GPU_Dword_Manager.exe` as Administrator
2. Select your GPU vendor (AMD or Nvidia)
3. Browse and search GPU DWORDs
4. Use the Tweaks panel on the right to apply optimizations
5. Track changes in History and use Undo if needed

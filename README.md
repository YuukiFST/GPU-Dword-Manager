# OUTDATED/BROKEN GPU Dword Manager

> [!CAUTION]
> ## ⚠️ THIS PROJECT IS ARCHIVED — NO LONGER MAINTAINED ⚠️
>
> **This project is no longer being updated and I have no plans to update it in the foreseeable future.**
>
> ### 🔴 FOR ADVANCED USERS ONLY — READ BEFORE USING
>
> **This tool should only be used by people who know exactly what they are doing.**
>
> I kept this repository private for a period of time because some tweaks in the **Quick Tweaks section for NVIDIA** were created purely for my own testing purposes and **can negatively impact NVIDIA GPU performance**. I do not own or use an NVIDIA GPU, so those settings were never validated for real-world use. Apply them with caution.
>
> Similarly, disabling **Logs, Debugs and Traces for AMD GPUs can also be harmful to performance** for users with an AMD GPU from the **6000 series or above**.
>
> You have been warned. Proceed at your own risk.

---

A Windows application for viewing and modifying AMD/Nvidia GPU registry DWORDs with built-in optimization tweaks.

> **Note:** The DWORD list does NOT reflect your actual GPU Driver. This is not a real-time monitor tool.

## Features
- **Dual GPU Support**: Works with both AMD Radeon and Nvidia GeForce GPUs
- **DWORD Registry Viewer**: Browse and search through GPU registry entries
- **Live Registry Reading**: Real-time querying of Windows Registry values
- **Advanced Filtering**: Filter by status (Found/Missing/All) and search by name
- **Edit, Add, Delete**: Modify DWORD values with decimal/hex input support
- **Change History**: Track all modifications with undo and revert capabilities
- **Automatic Backups**: Original values are backed up before applying tweaks
- **Smart Revert**: Restore original values or delete DWORDs that didn't exist before

## Screenshots
<img width="393" height="273" alt="image" src="https://github.com/user-attachments/assets/a4478b96-99cc-4215-bbc0-3097ecd0c589" />
<img width="1433" height="746" alt="image" src="https://github.com/user-attachments/assets/d4371c3d-89c8-4ficb-85d0-f6367bbdd7d8" />

## Requirements
- Windows 10/11
- Administrator privileges (for registry modifications)

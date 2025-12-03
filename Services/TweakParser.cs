using System.Collections.Generic;
using AMD_DWORD_Viewer.Models;

namespace AMD_DWORD_Viewer.Services
{
    public class TweakParser
    {
        public List<TweakDefinition> LoadTweaks()
        {
            var tweaks = new List<TweakDefinition>();

            tweaks.Add(CreateDisableAllGatingsTweak());
            tweaks.Add(CreateDisableAspmTweak());
            tweaks.Add(CreateDisableRadeonBoostTweak());
            tweaks.Add(CreateDisableLogsTweak());
            tweaks.Add(CreateDisableDebugsTweak());
            tweaks.Add(CreateDisableTracesTweak());

            return tweaks;
        }

        private TweakDefinition CreateDisableAllGatingsTweak()
        {
            var tweak = new TweakDefinition
            {
                Name = "Disable All Gatings",
                Description = "Disables all GPU clock and power gating"
            };

            tweak.Changes.Add(new TweakChange { KeyName = "DalDisableClockGating", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "DalFineGrainClockGating", TargetValue = 1 });

            tweak.Changes.Add(new TweakChange { KeyName = "DisableAcpPowerGating", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "DisableAllClockGating", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "DisableCpPowerGating", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "DisableDrmdmaPowerGating", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "DisableDynamicGfxMGPowerGating", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "DisableGDSPowerGating", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "DisableGfxCGPowerGating", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "DisableGFXPipelinePowerGating", TargetValue = 1 });

            tweak.Changes.Add(new TweakChange { KeyName = "DisableUVDPowerGatingDynamic", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "DisableGfxClockGating", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "DisableGfxCoarseGrainClockGating", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "DisableGfxMediumGrainClockGating", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "DisableMcMediumGrainClockGating", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "DisableNbioMediumGrainClockGating", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "DisablePowerGating", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "DisableQuickGfxMGPowerGating", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "DisableRomMediumGrainClockGating", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "DisableRomMGCGClockGating", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "DisableSamuClockGating", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "DisableSAMUPowerGating", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "DisableStaticGfxMGPowerGating", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "DisableSysClockGating", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "DisableUVDPowerGating", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "DisableVceClockGating", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "DisableVCEPowerGating", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "DisableXdmaPowerGating", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "DisableXdmaSclkGating", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "EnableUvdClockGating", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "EnableVceSwClockGating", TargetValue = 0 });

            tweak.Changes.Add(new TweakChange { KeyName = "EnableGfxClockGatingThruSmu", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "EnableSysClockGatingThruSmu", TargetValue = 0 });

            tweak.Changes.Add(new TweakChange { KeyName = "IRQMgrDisableIHClockGating", TargetValue = 1 });

            tweak.Changes.Add(new TweakChange { KeyName = "swGcClockGatingMask", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "swGcClockGatingOverride", TargetValue = 1 });

            return tweak;
        }

        private TweakDefinition CreateDisableAspmTweak()
        {
            var tweak = new TweakDefinition
            {
                Name = "Disable ASPM",
                Description = "Disables Active State Power Management"
            };

            tweak.Changes.Add(new TweakChange { KeyName = "DisableAspmSWL1", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "DisableAspmL0s", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "DisableAspmL1", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "EnableAspmL0s", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "EnableAspmL1", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "EnableAspmL1SS", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "AspmL0sTimeout", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "AspmL1Timeout", TargetValue = 0 });

            return tweak;
        }

        private TweakDefinition CreateDisableRadeonBoostTweak()
        {
            var tweak = new TweakDefinition
            {
                Name = "Disable Radeon Boost",
                Description = "Disables Radeon Boost feature"
            };

            tweak.Changes.Add(new TweakChange { KeyName = "KMD_RadeonBoostEnabled", TargetValue = 0 });

            return tweak;
        }

        private TweakDefinition CreateDisableLogsTweak()
        {
            var tweak = new TweakDefinition
            {
                Name = "Disable Logs",
                Description = "Disables all logging features"
            };

            tweak.Changes.Add(new TweakChange { KeyName = "CP_EnableWSLog", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "DALSTBLogEnable", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "DalAutoDPMTestLogs", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "DalDCELogFilePath", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "DalDmubLiveLogEnable", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "DalFSFTEnableDMUBLogging", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "DalLogBufferSize", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "DalLogEnable", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "DalLogMask_AutoTest", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "DalLogMask_BWM", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "DalLogMask_Backlight", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "DalLogMask_Bios", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "DalLogMask_DC", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "DalLogMask_DC2", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "DalLogMask_DCP", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "DalLogMask_DCS", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "DalLogMask_DSAT", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "DalLogMask_Display Connectivity", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "DalLogMask_EC", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "DalLogMask_Error", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "DalLogMask_FW", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "DalLogMask_HWSS", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "DalLogMask_HwTrace", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "DalLogMask_I2cAux", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "DalLogMask_ISR", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "DalLogMask_IfTrace", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "DalLogMask_InfoPacket", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "DalLogMask_Interrupts", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "DalLogMask_LineBuffer", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "DalLogMask_MPO", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "DalLogMask_MST", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "DalLogMask_ModeEnum", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "DalLogMask_Optimization", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "DalLogMask_PerfMeasure", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "DalLogMask_Protection", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "DalLogMask_Register", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "DalLogMask_SetMode", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "DalLogMask_Sync", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "DalLogMask_Testing", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "DalLogMask_Visual Trace", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "DalLogMask_Warning", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "EnableUserModeRingLog", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "EnableVCNFWLog", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "IRQMgrReqLogNumEntries", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "IRQMgr_LogUpdateThreshold", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "IRQMgr_LogUpdateTimeout", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "KMD_ActiveLogGroup", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "KMD_CpAceTopology", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "KMD_DirectCaptureAmdLog", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "KMD_EnableAcpLogging", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "KMD_EnableAmdlogFlipLogging", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "KMD_EnableBigPageAppLogging", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "KMD_EnableCPFrameSTBLog", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "KMD_EnableEventLog", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "KMD_EnableMesEventIntLogging", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "KMD_EnableMesLog", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "KMD_EnablePreemptionLogging", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "KMD_EnableRLCXTDebugLog", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "KMD_EnableResumeTimeLogging", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "KMD_EnableSDMAFrameSTBLog", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "KMD_EventLogCaptureConfig", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "KMD_EventLogFileDumpFormat", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "KMD_EventLogProductionCaptureConfig", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "KMD_HwsLoggingSharePagingVmid", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "KMD_IbLog", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "KMD_LogFile_DCS", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "KMD_LogGroupControl", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "KMD_LogLevel_DCS", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "KMD_LogLevel_DOPP", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "KMD_LogLevel_FM_Monitor", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "KMD_LogLevel_SDI", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "KMD_LogLevel_Stereo", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "KMD_LoggingControl", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "KMD_MaxLogBufferSizeInMB", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "KMD_PrivateFlipAmdLog", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "KMD_RLCXTDebugLogBufferSize", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "LogicalMemCfg", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "PP_EnableAllPMLogSensors", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "PP_LogDestination", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "PP_LogDumpTableBuffers", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "PP_LogField", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "PP_LogLevel", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "PP_LogSource", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "PP_PMLogGfxClkSource", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "PP_PMLogPreDsWorkloadsMask", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "SMU_ToolsLogSpaceSize", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "TraceLogCompMask", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "TraceLogIOPortAddr", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "TraceLogModeMask", TargetValue = 0 });

            return tweak;
        }

        private TweakDefinition CreateDisableDebugsTweak()
        {
            var tweak = new TweakDefinition
            {
                Name = "Disable Debugs",
                Description = "Disables all debug features"
            };

            tweak.Changes.Add(new TweakChange { KeyName = "BGM_DebugMsgControlLevel", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "CAILDebugLevel", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "CailDisableVbiosRegAccessDebug", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "DMMDebugOptions", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "DalDP20Debug", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "DalDebugOptions", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "DalDpiaDebug", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "DalEnableDriverSequenceDebug", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "DalHpdTimerDebug", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "DalReplayDebugOpt", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "EnableAtomworkDebugger", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "EnableParserDebugger", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "EnableswSqDebug", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "KMD_ATI_DebugPortBase", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "KMD_ATI_DebugPostParam", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "KMD_CpDebugDump", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "KMD_DebugBreak", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "KMD_DebugEnableHardware", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "KMD_DebugLevel", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "KMD_DebugToggleUvdNodeInit", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "KMD_EnableBsodFbDebugReservation", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "KMD_EnableDebugVmid", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "KMD_EnableRGDShaderDebug", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "KMD_EnableRLCXTDebugLog", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "KMD_RLCXTDebugLogBufferSize", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "KMD_Wddm2PtDebugPrintLevel", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "KMD_Wddm2PtDebugPrintOption", TargetValue = 0 });

            return tweak;
        }

        private TweakDefinition CreateDisableTracesTweak()
        {
            var tweak = new TweakDefinition
            {
                Name = "Disable Traces",
                Description = "Disables all tracing features"
            };

            tweak.Changes.Add(new TweakChange { KeyName = "DalDmubTraceEventMask", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "DalDmubTraceEventMask2", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "DalEnableAllocBackTrace", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "DalLogMask_HwTrace", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "DalLogMask_IfTrace", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "DalLogMask_VisualTrace", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "DalRegisterDmubTraceEventInterrupts", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "EnableSdmaSmartTraceBuffer", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "KMD_EnableAllocStackTrace", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "TraceLogCompMask", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "TraceLogIOPortAddr", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "TraceLogModeMask", TargetValue = 0 });

            return tweak;
        }
    }
}

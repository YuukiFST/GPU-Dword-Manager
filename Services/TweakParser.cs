using System.Collections.Generic;
using GPU_Dword_Manager_Avalonia.Models;

namespace GPU_Dword_Manager_Avalonia.Services
{
    public class TweakParser : ITweakParser
    {
        private readonly GpuVendor vendor;

        public TweakParser(GpuVendor selectedVendor = GpuVendor.AMD)
        {
            vendor = selectedVendor;
        }

        public List<TweakDefinition> LoadTweaks()
        {
            return vendor == GpuVendor.Nvidia ? LoadNvidiaTweaks() : LoadAmdTweaks();
        }

        private List<TweakDefinition> LoadAmdTweaks()
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

        private List<TweakDefinition> LoadNvidiaTweaks()
        {
            var tweaks = new List<TweakDefinition>();

            tweaks.Add(CreateNvidiaDisablePowerManagementTweak());
            tweaks.Add(CreateNvidiaDisableLoggingTweak());
            tweaks.Add(CreateNvidiaPerformanceModeTweak());
            tweaks.Add(CreateNvidiaDisablePreemptionTweak());
            tweaks.Add(CreateNvidiaDisableHDCPTweak());
            tweaks.Add(CreateNvidiaDisableECCTweak());
            tweaks.Add(CreateNvidiaDisableScrubbersTweak());

            return tweaks;
        }

        private TweakDefinition CreateNvidiaDisablePowerManagementTweak()
        {
            var tweak = new TweakDefinition
            {
                Name = "Disable Power Management",
                Description = "Disables GPU power management features for maximum performance"
            };

            tweak.Changes.Add(new TweakChange { KeyName = "EnableRuntimePowerManagement", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "EnableMsHybrid", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "EnableCoprocPowerControl", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "EnableGC6InDisplayOffState", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "DisableAsyncPstates", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "DisableDynamicPstate", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "PowerSavingTweaks", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "RMPowerFeature", TargetValue = 0x55455555 });
            tweak.Changes.Add(new TweakChange { KeyName = "RMPowerFeature2", TargetValue = 0x05555555 });
            tweak.Changes.Add(new TweakChange { KeyName = "EnableMClkSlowdown", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "RMDidleFeatureGC5", TargetValue = 0 });

            return tweak;
        }

        private TweakDefinition CreateNvidiaDisableLoggingTweak()
        {
            var tweak = new TweakDefinition
            {
                Name = "Disable Logging",
                Description = "Disables driver logging and tracing features"
            };

            tweak.Changes.Add(new TweakChange { KeyName = "LogErrorEntries", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "LogEventEntries", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "LogPagingEntries", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "LogWarningEntries", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "prtLevel", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "prtBreak", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "ENABLE_OCA_LOGGING", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "EnableLoggingInterface", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "SchedulerLogDebugMode", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "EnablePageFaultDebugOutput", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "EnableGpuFirmwareLogs", TargetValue = 0 });

            return tweak;
        }

        private TweakDefinition CreateNvidiaPerformanceModeTweak()
        {
            var tweak = new TweakDefinition
            {
                Name = "Enable Performance Mode",
                Description = "Enables maximum GPU performance settings"
            };
            tweak.Changes.Add(new TweakChange { KeyName = "EnablePerformanceMode", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "EnableAggressivePStateBoost", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "RMDisablePStates", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "DisableGrAuto", TargetValue = 0 });
            string graphicsDriversPath = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\GraphicsDrivers";
            tweak.Changes.Add(new TweakChange { KeyName = "DisableVersionMismatchCheck", TargetValue = 1, RegistryPath = graphicsDriversPath });
            tweak.Changes.Add(new TweakChange { KeyName = "EnableIgnoreWin32ProcessStatus", TargetValue = 1, RegistryPath = graphicsDriversPath });
            tweak.Changes.Add(new TweakChange { KeyName = "HwSchMode", TargetValue = 2, RegistryPath = graphicsDriversPath });
            tweak.Changes.Add(new TweakChange { KeyName = "HwSchTreatExperimentalAsStable", TargetValue = 1, RegistryPath = graphicsDriversPath });
            tweak.Changes.Add(new TweakChange { KeyName = "TdrDebugMode", TargetValue = 1, RegistryPath = graphicsDriversPath });
            tweak.Changes.Add(new TweakChange { KeyName = "TdrLevel", TargetValue = 0, RegistryPath = graphicsDriversPath });
            tweak.Changes.Add(new TweakChange { KeyName = "DisableBadDriverCheckForHwProtection", TargetValue = 1, RegistryPath = graphicsDriversPath });
            tweak.Changes.Add(new TweakChange { KeyName = "DisableBoostedVSyncVirtualization", TargetValue = 1, RegistryPath = graphicsDriversPath });
            tweak.Changes.Add(new TweakChange { KeyName = "DisableIndependentVidPnVSync", TargetValue = 1, RegistryPath = graphicsDriversPath });
            tweak.Changes.Add(new TweakChange { KeyName = "DisableMultiSourceMPOCheck", TargetValue = 1, RegistryPath = graphicsDriversPath });
            tweak.Changes.Add(new TweakChange { KeyName = "EnableFbrValidation", TargetValue = 0, RegistryPath = graphicsDriversPath });
            tweak.Changes.Add(new TweakChange { KeyName = "KnownProcessBoostMode", TargetValue = 0, RegistryPath = graphicsDriversPath });
            tweak.Changes.Add(new TweakChange { KeyName = "DisableOverlays", TargetValue = 1, RegistryPath = graphicsDriversPath });
            string nvlddmkmPath = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\nvlddmkm";
            tweak.Changes.Add(new TweakChange { KeyName = "LogWarningEntries", TargetValue = 0, RegistryPath = nvlddmkmPath });
            tweak.Changes.Add(new TweakChange { KeyName = "LogPagingEntries", TargetValue = 0, RegistryPath = nvlddmkmPath });
            tweak.Changes.Add(new TweakChange { KeyName = "LogEventEntries", TargetValue = 0, RegistryPath = nvlddmkmPath });
            tweak.Changes.Add(new TweakChange { KeyName = "LogErrorEntries", TargetValue = 0, RegistryPath = nvlddmkmPath });
            tweak.Changes.Add(new TweakChange { KeyName = "LogEnableMasks", TargetValue = 0, RegistryPath = nvlddmkmPath });
            tweak.Changes.Add(new TweakChange { KeyName = "LogDisableMasks", TargetValue = 0, RegistryPath = nvlddmkmPath });
            tweak.Changes.Add(new TweakChange { KeyName = "PreferSystemMemoryContiguous", TargetValue = 1, RegistryPath = nvlddmkmPath });
            tweak.Changes.Add(new TweakChange { KeyName = "PrimaryPushBufferSize", TargetValue = 1, RegistryPath = nvlddmkmPath });
            tweak.Changes.Add(new TweakChange { KeyName = "EnablePerformanceMode", TargetValue = 1, RegistryPath = nvlddmkmPath });
            tweak.Changes.Add(new TweakChange { KeyName = "DisableBugcheckCallback", TargetValue = 1, RegistryPath = nvlddmkmPath });
            tweak.Changes.Add(new TweakChange { KeyName = "DisableCudaContextPreemption", TargetValue = 0, RegistryPath = nvlddmkmPath });
            tweak.Changes.Add(new TweakChange { KeyName = "DisablePreemptionOnS3S4", TargetValue = 0, RegistryPath = nvlddmkmPath });
            tweak.Changes.Add(new TweakChange { KeyName = "EnableMidGfxPreemptionVGPU", TargetValue = 1, RegistryPath = nvlddmkmPath });
            tweak.Changes.Add(new TweakChange { KeyName = "EnableMidGfxpSharedBuffer", TargetValue = 1, RegistryPath = nvlddmkmPath });
            tweak.Changes.Add(new TweakChange { KeyName = "EnableMidGfxPreemption", TargetValue = 1, RegistryPath = nvlddmkmPath });
            tweak.Changes.Add(new TweakChange { KeyName = "PerfAnalyzeMidBufferPreemption", TargetValue = 1, RegistryPath = nvlddmkmPath });
            tweak.Changes.Add(new TweakChange { KeyName = "EnableSCGMidBufferPreemption", TargetValue = 1, RegistryPath = nvlddmkmPath });
            tweak.Changes.Add(new TweakChange { KeyName = "EnableAsyncMidBufferPreemption", TargetValue = 1, RegistryPath = nvlddmkmPath });
            tweak.Changes.Add(new TweakChange { KeyName = "EnableMidBufferPreemptionForHighTdrTimeout", TargetValue = 1, RegistryPath = nvlddmkmPath });
            tweak.Changes.Add(new TweakChange { KeyName = "EnableCEPreemption", TargetValue = 1, RegistryPath = nvlddmkmPath });
            tweak.Changes.Add(new TweakChange { KeyName = "EnableMidBufferPreemption", TargetValue = 1, RegistryPath = nvlddmkmPath });
            tweak.Changes.Add(new TweakChange { KeyName = "DisablePreemption", TargetValue = 0, RegistryPath = nvlddmkmPath });
            tweak.Changes.Add(new TweakChange { KeyName = "UvmDisable", TargetValue = 1, RegistryPath = nvlddmkmPath });
            tweak.Changes.Add(new TweakChange { KeyName = "EnableIommuIsolationOnWDDM216", TargetValue = 0, RegistryPath = nvlddmkmPath });
            tweak.Changes.Add(new TweakChange { KeyName = "WDDMV2_128K_PTE", TargetValue = 1, RegistryPath = nvlddmkmPath });
            tweak.Changes.Add(new TweakChange { KeyName = "EnableHDAudioD3Cold", TargetValue = 0, RegistryPath = nvlddmkmPath });
            tweak.Changes.Add(new TweakChange { KeyName = "WDDMv21Enable64KbSysmemSupport", TargetValue = 1, RegistryPath = nvlddmkmPath });
            tweak.Changes.Add(new TweakChange { KeyName = "DisableWriteCombining", TargetValue = 1, RegistryPath = nvlddmkmPath });
            tweak.Changes.Add(new TweakChange { KeyName = "WDDMv21Enable2MPageSupport", TargetValue = 1, RegistryPath = nvlddmkmPath });
            string nvlddmkmParamsPath = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\nvlddmkm\Parameters";
            tweak.Changes.Add(new TweakChange { KeyName = "DmaRemappingCompatible", TargetValue = 0, RegistryPath = nvlddmkmParamsPath });
            tweak.Changes.Add(new TweakChange { KeyName = "RMAERRForceDisable", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "RMNoECCFuseCheck", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "RMDisableRCOnDBE", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "RM1441072", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "RMAERRHandling", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "RMChkSuppl200405980Driv", TargetValue = 0x13deed31 });
            tweak.Changes.Add(new TweakChange { KeyName = "RmDisableACPI", TargetValue = 0x01FF });
            tweak.Changes.Add(new TweakChange { KeyName = "RMDisableGpuASPMFlags", TargetValue = 3 });
            tweak.Changes.Add(new TweakChange { KeyName = "RMPowerFeature", TargetValue = 0x54455555 });
            tweak.Changes.Add(new TweakChange { KeyName = "RMPowerFeature2", TargetValue = 0x05555555 });
            tweak.Changes.Add(new TweakChange { KeyName = "RMFspg", TargetValue = 0x0000000F });
            tweak.Changes.Add(new TweakChange { KeyName = "RMBlcg", TargetValue = 0x11111111 });
            tweak.Changes.Add(new TweakChange { KeyName = "RMElcg", TargetValue = 0x55555555 });
            tweak.Changes.Add(new TweakChange { KeyName = "RmElpg", TargetValue = 0x00000FFF });
            tweak.Changes.Add(new TweakChange { KeyName = "RMSlcg", TargetValue = 0x0003ffff });
            tweak.Changes.Add(new TweakChange { KeyName = "DisableDynamicPstate", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "RMOPSB", TargetValue = 0x00002AA2 });
            tweak.Changes.Add(new TweakChange { KeyName = "RMLpwrArch", TargetValue = 0x00055555 });
            tweak.Changes.Add(new TweakChange { KeyName = "RMGCOffFeature", TargetValue = 2 });
            tweak.Changes.Add(new TweakChange { KeyName = "RmClkPowerOffDramPllWhenUnused", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "RMDisableOptimalPowerForPadlinkPll", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "RMPcieLtrOverride", TargetValue = 2 });
            tweak.Changes.Add(new TweakChange { KeyName = "RmPgCtrlParameters", TargetValue = 0x55555155 });
            tweak.Changes.Add(new TweakChange { KeyName = "RmPgCtrlGrParameters", TargetValue = 0x55555555 });
            tweak.Changes.Add(new TweakChange { KeyName = "RmLpwrCtrlGrRgParameters", TargetValue = 0x05555555 });
            tweak.Changes.Add(new TweakChange { KeyName = "RmPgCtrlDiParameters", TargetValue = 0x00000015 });
            tweak.Changes.Add(new TweakChange { KeyName = "RMDidleFeatureGC5", TargetValue = 0x2AAAAAA });
            tweak.Changes.Add(new TweakChange { KeyName = "RMGC6Parameters", TargetValue = 0x00000055 });
            tweak.Changes.Add(new TweakChange { KeyName = "RMBandwidthFeature2", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "RmMIONoPowerOff", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "RMClkSlowDown", TargetValue = 0x05400000 });
            tweak.Changes.Add(new TweakChange { KeyName = "EnablePerformanceMode", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "RMLpwrEiClient", TargetValue = 5 });
            tweak.Changes.Add(new TweakChange { KeyName = "RmLpwrGrPgSwFilterFunction", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "RmLpwrCtrlMsLtcParameters", TargetValue = 5 });
            tweak.Changes.Add(new TweakChange { KeyName = "RmLpwrCtrlMsDifrCgParameters", TargetValue = 0x00000555 });
            tweak.Changes.Add(new TweakChange { KeyName = "RM2779240", TargetValue = 5 });
            tweak.Changes.Add(new TweakChange { KeyName = "RmLpwrCtrlMsDifrSwAsrParameters", TargetValue = 0x00001555 });
            tweak.Changes.Add(new TweakChange { KeyName = "RmOverrideIdleSlowdownSettings", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "RmPerfRatedTdpLimit", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "RmPmgrPwrPolicyOverride", TargetValue = 0x0000000F });
            tweak.Changes.Add(new TweakChange { KeyName = "RMGC6Feature", TargetValue = 0x002AAAAA });
            tweak.Changes.Add(new TweakChange { KeyName = "RmPerfCfOverride", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "GrCtxSwMode", TargetValue = 2 });
            tweak.Changes.Add(new TweakChange { KeyName = "RmRcWatchdog", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "ThermalPolicySW1", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "GlitchFreeMClk", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "RMEnablePowerSupplyCapacity", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "RMPowerSupplyCapacity", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "RMSkipACPIBattCap", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "RMDiWakeupTimer", TargetValue = 2 });
            tweak.Changes.Add(new TweakChange { KeyName = "RMD3Feature", TargetValue = 2 });
            tweak.Changes.Add(new TweakChange { KeyName = "RmDisableRegistryCaching", TargetValue = 0x0000000F });
            tweak.Changes.Add(new TweakChange { KeyName = "RMHdcpKeyglobZero", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "RMDisableEDC", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "RmGpsPowerSteeringEnable", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "RmGpsPsEnablePerCpuCoreDpc", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "RmHulkDisableFeatures", TargetValue = 7 });
            tweak.Changes.Add(new TweakChange { KeyName = "RMCtxswLog", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "RmThermPolicySwSlowdownOverride", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "RmThermPolicyOverride", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "RmPerfCfPolicyOverrides", TargetValue = 3 });
            tweak.Changes.Add(new TweakChange { KeyName = "MaxPerfWithPerfMon", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "RMAsrWakeup", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "RmEnableHda", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "RmFbsrPagedDMA", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "RMSupportUserdMapDma", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "RM572548", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "RmDwbMscg", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "RmValidateClientData", TargetValue = 0x000000AA });
            tweak.Changes.Add(new TweakChange { KeyName = "RmIgnoreHulkErrors", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "RmForceDisableIomapWC", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "RmClkControllersOverride", TargetValue = 3 });
            tweak.Changes.Add(new TweakChange { KeyName = "RMI2cSpeed", TargetValue = 0x00000400 });
            tweak.Changes.Add(new TweakChange { KeyName = "RM2644249", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "RmWar1760398", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "RmCeElcgWar1895530", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "RMBug2519005War", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "RmDisableInforomNvlink", TargetValue = 3 });
            tweak.Changes.Add(new TweakChange { KeyName = "RMDisableIntrIllegalCompstatAccess", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "RmSec2EnableApm", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "RMCBAllocVPR", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "RmGpsACPIType", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "RmDisableFanDiag", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "RmGpsPreferIntrinsicFuncs", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "RMNoECCFBScrub", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "RMDisableScrubOnFree", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "RMDisableAsyncMemScrub", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "RMDisableFastScrubber", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "RMUcodeEncryption", TargetValue = 0x00015555 });
            string schedulerPath = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\GraphicsDrivers\Scheduler";
            tweak.Changes.Add(new TweakChange { KeyName = "AdjustWorkerThreadPriority", TargetValue = 0, RegistryPath = schedulerPath });
            tweak.Changes.Add(new TweakChange { KeyName = "AudioDgAutoBoostPriority", TargetValue = 0, RegistryPath = schedulerPath });
            tweak.Changes.Add(new TweakChange { KeyName = "AutoSyncToCPUPriority", TargetValue = 0, RegistryPath = schedulerPath });
            tweak.Changes.Add(new TweakChange { KeyName = "DebugLargeSmoothenedDuration", TargetValue = 0, RegistryPath = schedulerPath });
            tweak.Changes.Add(new TweakChange { KeyName = "ForegroundPriorityBoost", TargetValue = 0, RegistryPath = schedulerPath });
            tweak.Changes.Add(new TweakChange { KeyName = "FrameServerAutoBoostPriority", TargetValue = 0, RegistryPath = schedulerPath });
            tweak.Changes.Add(new TweakChange { KeyName = "QueuedPresentLimit", TargetValue = 1, RegistryPath = schedulerPath });
            tweak.Changes.Add(new TweakChange { KeyName = "EnablePreemption", TargetValue = 1, RegistryPath = schedulerPath });
            tweak.Changes.Add(new TweakChange 
            { 
                ActionType = TweakActionType.Command,
                FilePath = "Configure Nvidia.bat"
            });

            return tweak;
        }

        private TweakDefinition CreateNvidiaDisablePreemptionTweak()
        {
            var tweak = new TweakDefinition
            {
                Name = "Disable Preemption",
                Description = "Disables shader preemption for reduced latency"
            };

            tweak.Changes.Add(new TweakChange { KeyName = "DisablePreemption", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "DisablePreemptionOnS3S4", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "DisableCudaContextPreemption", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "EnableMidBufferPreemption", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "EnableAsyncMidBufferPreemption", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "EnableMidGfxPreemption", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "EnableCEPreemption", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "EnableSCGMidBufferPreemption", TargetValue = 0 });

            return tweak;
        }

        private TweakDefinition CreateNvidiaDisableHDCPTweak()
        {
            var tweak = new TweakDefinition
            {
                Name = "Disable HDCP",
                Description = "Disables HDCP copy protection (may break Twitch/Netflix)"
            };

            tweak.Changes.Add(new TweakChange { KeyName = "RMHdcpKeyglobZero", TargetValue = 1 });

            return tweak;
        }

        private TweakDefinition CreateNvidiaDisableECCTweak()
        {
            var tweak = new TweakDefinition
            {
                Name = "Disable ECC",
                Description = "Disables Error Correction Code memory checking"
            };

            tweak.Changes.Add(new TweakChange { KeyName = "RMAERRForceDisable", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "RMNoECCFuseCheck", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "RMDisableRCOnDBE", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "RM1441072", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "RMAERRHandling", TargetValue = 0 });
            tweak.Changes.Add(new TweakChange { KeyName = "RMChkSuppl200405980Driv", TargetValue = 0x13deed31 });

            return tweak;
        }

        private TweakDefinition CreateNvidiaDisableScrubbersTweak()
        {
            var tweak = new TweakDefinition
            {
                Name = "Disable Scrubbers",
                Description = "Disables memory scrubbing for faster VRAM operations"
            };

            tweak.Changes.Add(new TweakChange { KeyName = "RMNoECCFBScrub", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "RMDisableScrubOnFree", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "RMDisableAsyncMemScrub", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "RMDisableFastScrubber", TargetValue = 1 });
            tweak.Changes.Add(new TweakChange { KeyName = "RMUcodeEncryption", TargetValue = 0x15555 });

            return tweak;
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


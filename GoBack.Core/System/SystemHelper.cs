using System.Runtime.InteropServices;
using static System.Runtime.InteropServices.OSPlatform;

namespace GoBack.Core.System;

public static class SystemHelper
{
    public static bool IsWindows => CheckPlatform(Windows);
    public static bool IsLinux => CheckPlatform(Linux);
    public static bool IsMacOs => CheckPlatform(OSX);

    private static bool CheckPlatform(OSPlatform platform)
    {
        return RuntimeInformation.IsOSPlatform(platform);
    }
}
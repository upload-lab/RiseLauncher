

using Microsoft.VisualBasic.Devices;
using System;

namespace RiseLauncher
{
  internal class UtilRAM
  {
    private static ulong GetTotalMemoryInBytes() => new ComputerInfo().TotalPhysicalMemory;

    public static int getRam() => Convert.ToInt32(UtilRAM.GetTotalMemoryInBytes() / 1073741824UL);
  }
}

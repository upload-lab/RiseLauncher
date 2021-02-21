

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace RiseLauncher.Properties
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class Resources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal Resources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (RiseLauncher.Properties.Resources.resourceMan == null)
          RiseLauncher.Properties.Resources.resourceMan = new ResourceManager("RiseLauncher.Properties.Resources", typeof (RiseLauncher.Properties.Resources).Assembly);
        return RiseLauncher.Properties.Resources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => RiseLauncher.Properties.Resources.resourceCulture;
      set => RiseLauncher.Properties.Resources.resourceCulture = value;
    }

    internal static byte[] Gilroy_Black => (byte[]) RiseLauncher.Properties.Resources.ResourceManager.GetObject(nameof (Gilroy_Black), RiseLauncher.Properties.Resources.resourceCulture);

    internal static byte[] Gilroy_Bold => (byte[]) RiseLauncher.Properties.Resources.ResourceManager.GetObject(nameof (Gilroy_Bold), RiseLauncher.Properties.Resources.resourceCulture);

    internal static byte[] Gilroy_Light => (byte[]) RiseLauncher.Properties.Resources.ResourceManager.GetObject(nameof (Gilroy_Light), RiseLauncher.Properties.Resources.resourceCulture);

    internal static byte[] Gilroy_Medium => (byte[]) RiseLauncher.Properties.Resources.ResourceManager.GetObject(nameof (Gilroy_Medium), RiseLauncher.Properties.Resources.resourceCulture);

    internal static byte[] Gilroy_Regular => (byte[]) RiseLauncher.Properties.Resources.ResourceManager.GetObject(nameof (Gilroy_Regular), RiseLauncher.Properties.Resources.resourceCulture);

    internal static byte[] Gilroy_Thin => (byte[]) RiseLauncher.Properties.Resources.ResourceManager.GetObject(nameof (Gilroy_Thin), RiseLauncher.Properties.Resources.resourceCulture);

    internal static Bitmap icon => (Bitmap) RiseLauncher.Properties.Resources.ResourceManager.GetObject(nameof (icon), RiseLauncher.Properties.Resources.resourceCulture);

    internal static Bitmap logo_215 => (Bitmap) RiseLauncher.Properties.Resources.ResourceManager.GetObject("logo-215", RiseLauncher.Properties.Resources.resourceCulture);
  }
}

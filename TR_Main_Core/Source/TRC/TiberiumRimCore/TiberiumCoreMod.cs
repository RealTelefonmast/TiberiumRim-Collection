using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace TR;

public class TiberiumCoreMod : Mod
{
    //Static Data
    public static TiberiumCoreMod mod;
    private static Harmony tiberium;

    //
    public static bool isDebug = true;

    public static Harmony Tiberium => tiberium ??= new Harmony("tele.tr.core");
    public static TiberiumCoreSettings CoreSettings => (TiberiumCoreSettings) mod.modSettings;
    
    public TiberiumCoreMod(ModContentPack content) : base(content)
    {
        mod = this;
        var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        TRLog.Message($"[TiberiumRim] - Init", Color.cyan);
        modSettings = GetSettings<TiberiumCoreSettings>();
        
        Tiberium.PatchAll(Assembly.GetExecutingAssembly());
    }
    
    public AssetBundle MainBundle
    {
        get
        {
            string pathPart = "";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                pathPart = "StandaloneOSX";
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                pathPart = "StandaloneWindows";
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                pathPart = "StandaloneLinux64";

            string mainBundlePath = Path.Combine(Content.RootDir, $@"Materials\Bundles\{pathPart}\tiberiumrimbundle");
            return AssetBundle.LoadFromFile(mainBundlePath);
        }
    }

    /*
PlatformID pid = System.Environment.OSVersion.Platform;
switch (pid)
{
case PlatformID.Win32NT:
case PlatformID.Win32S:
case PlatformID.Win32Windows:
case PlatformID.WinCE:
    Console.WriteLine("I'm on windows!");
    break;
case PlatformID.Unix:
    Console.WriteLine("I'm a linux box!");
    break;
case PlatformID.MacOSX:
    Console.WriteLine("I'm a mac!");
    break;
default:
    Console.WriteLine("No Idea what I'm on!");
    break;
}
*/

    /*
    public void LoadAssetBundles()
    {
        string mainBundlePath = Path.Combine(Content.RootDir, @"Materials\Shaders\tiberiumrimbundle");
        TRContentDatabase.SetBundle(AssetBundle.LoadFromFile(mainBundlePath));

        //string path = Path.Combine(Content.RootDir, @"Materials\Shaders\shaderbundle");
        //assetBundle = AssetBundle.LoadFromFile(path);
        //TiberiumContent.AlphaShader = (Shader)assetBundle.LoadAsset("AlphaShader");
        //TiberiumContent.AlphaShaderMaterial = (Material)assetBundle.LoadAsset("ShaderMaterial");
    }
    */
}
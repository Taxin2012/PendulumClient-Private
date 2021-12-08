using System.Resources;
using System.Reflection;
using System.Runtime.InteropServices;
using MelonLoader;

[assembly: AssemblyTitle(PendulumClient.Main.BuildInfo.Name)]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany(PendulumClient.Main.BuildInfo.Company)]
[assembly: AssemblyProduct(PendulumClient.Main.BuildInfo.Name)]
[assembly: AssemblyCopyright("Created by " + PendulumClient.Main.BuildInfo.Author)]
[assembly: AssemblyTrademark(PendulumClient.Main.BuildInfo.Company)]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
//[assembly: Guid("")]
[assembly: AssemblyVersion(PendulumClient.Main.BuildInfo.Version)]
[assembly: AssemblyFileVersion(PendulumClient.Main.BuildInfo.Version)]
[assembly: NeutralResourcesLanguage("en")]
[assembly: MelonInfo(typeof(PendulumClient.Main.PendulumClientMain), PendulumClient.Main.BuildInfo.Name, PendulumClient.Main.BuildInfo.Version, PendulumClient.Main.BuildInfo.Author, PendulumClient.Main.BuildInfo.DownloadLink)]


// Create and Setup a MelonModGame to mark a Mod as Universal or Compatible with specific Games.
// If no MelonModGameAttribute is found or any of the Values for any MelonModGame on the Mod is null or empty it will be assumed the Mod is Universal.
// Values for MelonModGame can be found in the Game's app.info file or printed at the top of every log directly beneath the Unity version.
[assembly: MelonGame("VRChat", "VRChat")]
namespace Chickensoft.GodotEnv.Features.Godot.Models;

using Chickensoft.GodotEnv.Common.Clients;
using Chickensoft.GodotEnv.Common.Utilities;
using global::GodotEnv.Common.Utilities;

public class Linux : Unix {
  public Linux(
    ISystemInfo systemInfo,
    IFileClient fileClient,
    IComputer computer,
    IVersionStringConverter versionStringConverter
  )
    : base(systemInfo, fileClient, computer, versionStringConverter) { }

  public override string GetInstallerNameSuffix(bool isDotnetVersion, GodotVersion version) {
    var (platformName, architecture) = GetPlatformNameAndArchitecture(version);

    return isDotnetVersion ? $"_mono_{platformName}_{architecture}" : $"_{platformName}.{architecture}";
  }

  public override void Describe(ILog log) => log.Info("🐧 Running on Linux");

  public override string GetRelativeExtractedExecutablePath(
    GodotVersion version, bool isDotnetVersion
  ) {
    var fsVersionString = GetFilenameVersionString(version);
    var nameSuffix = GetInstallerNameSuffix(isDotnetVersion, version);
    var (platformName, architecture) = GetPlatformNameAndArchitecture(version);

    var pathSuffix = fsVersionString +
               $"{(isDotnetVersion ? "_mono" : "")}_{platformName}.{architecture}";

    if (isDotnetVersion) {
      // Dotnet version extracts to a folder, whereas the non-dotnet version
      // does not.
      return FileClient.Combine(fsVersionString + nameSuffix, pathSuffix);
    }

    return pathSuffix;
  }

  public override string GetRelativeGodotSharpPath(
    GodotVersion version,
    bool isDotnetVersion
  ) => FileClient.Combine(
      GetFilenameVersionString(version) + GetInstallerNameSuffix(isDotnetVersion, version),
      "GodotSharp/"
    );

  private static (string platformName, string architecture) GetPlatformNameAndArchitecture(
    GodotVersion version
  ) {
    var architecture = "x86_64";
    var platformName = "linux";

    if (version.Major == 3) {
      architecture = "64";
      platformName = "x11";
    }

    return (platformName, architecture);
  }
}

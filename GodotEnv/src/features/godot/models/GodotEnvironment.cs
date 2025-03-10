namespace Chickensoft.GodotEnv.Features.Godot.Models;

using System;
using Chickensoft.GodotEnv.Common.Clients;
using Chickensoft.GodotEnv.Common.Models;
using Chickensoft.GodotEnv.Common.Utilities;
using global::GodotEnv.Common.Utilities;

public interface IGodotEnvironment {
  ISystemInfo SystemInfo { get; }
  /// <summary>
  /// File client used by the platform to manipulate file paths.
  /// </summary>
  IFileClient FileClient { get; }

  IComputer Computer { get; }

  IVersionStringConverter VersionStringConverter { get; }

  /// <summary>
  /// Godot installation filename suffix.
  /// </summary>
  /// <param name="isDotnetVersion">True if using the .NET-enabled version of Godot,
  /// false otherwise.</param>
  /// <param name="version">Godot version.</param>
  /// <returns>Godot filename suffix.</returns>
  string GetInstallerNameSuffix(bool isDotnetVersion, GodotVersion version);

  /// <summary>
  /// Computes the Godot download url.
  /// </summary>
  /// <param name="version">Godot version.</param>
  /// <param name="isDotnetVersion">
  /// True if referencing the .NET version of Godot.
  /// </param>
  /// <param name="isTemplate">True if computing the download url to the
  /// export templates, false to compute the download url to the Godot
  /// application.</param>
  string GetDownloadUrl(
    GodotVersion version,
    bool isDotnetVersion,
    bool isTemplate
  );

  /// <summary>
  /// Gets the filename as which an installer is known.
  /// </summary>
  /// <param name="version">Godot version.</param>
  /// <param name="isDotnetVersion">
  /// True if referencing the .NET version of Godot.
  /// </param>
  public string GetInstallerFilename(
    GodotVersion version, bool isDotnetVersion
  );

  /// <summary>
  /// Outputs a description of the platform to the log.
  /// </summary>
  /// <param name="log">Output log.</param>
  void Describe(ILog log);

  /// <summary>
  /// Returns the path where the Godot executable itself is located, relative
  /// to the extracted Godot installation directory.
  /// </summary>
  /// <param name="version">Godot version.</param>
  /// <param name="isDotnetVersion">Dotnet version indicator.</param>
  /// <returns>Relative path.</returns>
  string GetRelativeExtractedExecutablePath(
    GodotVersion version, bool isDotnetVersion
  );

  /// <summary>
  /// For dotnet-enabled versions, this gets the path to the GodotSharp
  /// directory that is included with Godot.
  /// </summary>
  /// <param name="version">Godot version.</param>
  /// <param name="isDotnetVersion">Dotnet version indicator.</param>
  /// <returns>Path to the GodotSharp directory.</returns>
  string GetRelativeGodotSharpPath(GodotVersion version, bool isDotnetVersion);
}

public abstract class GodotEnvironment : IGodotEnvironment {
  public const string GODOT_FILENAME_PREFIX = "Godot_v";
  public const string GODOT_URL_PREFIX =
    "https://github.com/godotengine/godot-builds/releases/download/";

  /// <summary>
  /// Creates a platform for the given OS.
  /// </summary>
  /// <param name="systemInfo"></param>
  /// <param name="fileClient">File client.</param>
  /// <param name="computer">Computer.</param>
  /// <param name="versionStringConverter">Version-string converter.</param>
  /// <returns>Platform instance.</returns>
  /// <exception cref="InvalidOperationException" />
  public static GodotEnvironment Create(
    ISystemInfo systemInfo,
    IFileClient fileClient,
    IComputer computer,
    IVersionStringConverter versionStringConverter
  ) =>
    systemInfo.OS switch {
      OSType.Windows => new Windows(systemInfo, fileClient, computer, versionStringConverter),
      OSType.MacOS => new MacOS(systemInfo, fileClient, computer, versionStringConverter),
      OSType.Linux => new Linux(systemInfo, fileClient, computer, versionStringConverter),
      OSType.Unknown => throw GetUnknownOSException(),
      _ => throw GetUnknownOSException()
    };

  protected GodotEnvironment(
    ISystemInfo systemInfo,
    IFileClient fileClient,
    IComputer computer,
    IVersionStringConverter versionStringConverter
  ) {
    SystemInfo = systemInfo;
    FileClient = fileClient;
    Computer = computer;
    VersionStringConverter = versionStringConverter;
  }

  public ISystemInfo SystemInfo { get; }
  public IFileClient FileClient { get; }
  public IComputer Computer { get; }
  public IVersionStringConverter VersionStringConverter { get; }

  public string ExportTemplatesBasePath => throw new NotImplementedException();

  public abstract string GetInstallerNameSuffix(bool isDotnetVersion, GodotVersion version);
  public abstract void Describe(ILog log);
  public abstract string GetRelativeExtractedExecutablePath(
    GodotVersion version, bool isDotnetVersion
  );
  public abstract string GetRelativeGodotSharpPath(
    GodotVersion version,
    bool isDotnetVersion
  );

  public string GetDownloadUrl(
    GodotVersion version,
    bool isDotnetVersion,
    bool isTemplate
  ) {
    // We need to be sure this is a release-style version string to get the
    // correct url
    var versionConverter = new ReleaseVersionStringConverter();
    var url = $"{GODOT_URL_PREFIX}{versionConverter.VersionString(version)}/";

    // Godot application download url.
    if (!isTemplate) {
      return url + GetInstallerFilename(version, isDotnetVersion);
    }

    // Export template download url.
    return
      url + GetExportTemplatesInstallerFilename(version, isDotnetVersion);
  }

  protected string GetFilenameVersionString(GodotVersion version) =>
    GODOT_FILENAME_PREFIX + VersionStringConverter.VersionString(version);

  // Gets the filename of the Godot installation download for the platform.
  public string GetInstallerFilename(
    GodotVersion version, bool isDotnetVersion
  ) => GetFilenameVersionString(version) + GetInstallerNameSuffix(isDotnetVersion, version) +
    ".zip";

  // Gets the filename of the Godot export templates installation download for
  // the platform.
  private string GetExportTemplatesInstallerFilename(
    GodotVersion version, bool isDotnetVersion
  ) => GetFilenameVersionString(version) + (isDotnetVersion ? "_mono" : "") +
      "_export_templates.tpz";

  private static InvalidOperationException GetUnknownOSException() =>
    new("🚨 Cannot create a platform an unknown operating system.");
}

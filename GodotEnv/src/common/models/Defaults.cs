namespace Chickensoft.GodotEnv.Common.Models;

using Downloader;

public static class Defaults {
  /// <summary>Binary name of this app.</summary>
  public const string BIN_NAME = "godotenv";

  /// <summary>Default git checkout branch.</summary>
  public const string CHECKOUT = "main";

  /// <summary>
  /// Default asset source (where the asset is copied or referenced from).
  /// </summary>
  public const AssetSource SOURCE = AssetSource.Remote;

  /// <summary>
  /// Which folder inside the addon that should actually be copied or
  /// referenced.
  /// </summary>
  public const string SUBFOLDER = "/";

  /// <summary>Default cache path, relative to the project.</summary>
  public const string CACHE_PATH = ".addons";

  /// <summary>
  /// Default addons installation path, relative to the project.
  /// </summary>
  public const string ADDONS_PATH = "addons";

  /// <summary>
  /// Name of the application configuration file in the config directory.
  /// </summary>
  public const string CONFIG_FILE_NAME = "godotenv.json";

  #region ConfigDefaults
  /// <summary>
  /// Default directory where Godot installations should be stored, relative to
  /// <see cref="GODOT_PATH" />.
  /// </summary>
  public const string CONFIG_GODOT_INSTALLATIONS_PATH = "versions";

  /// <summary>
  /// Whether to use emoji in the terminal.
  /// </summary>
  public const bool CONFIG_TERMINAL_DISPLAY_EMOJI = true;
  #endregion ConfigDefaults

  /// <summary>User-wide GODOT environment variable name.</summary>
  public const string GODOT_ENV_VAR_NAME = "GODOT";

  /// <summary>User-wide PATH environment variable name.</summary>
  public const string PATH_ENV_VAR_NAME = "PATH";

  /// <summary>
  /// Where Godot-related commands store data, relative to the application
  /// storage location.
  /// </summary>
  public const string GODOT_PATH = "godot";

  /// <summary>
  /// Symlink directory where the current Godot installation can be referenced,
  /// relative to <see cref="GODOT_PATH" />.
  /// </summary>
  public const string GODOT_BIN_PATH = "bin";

  public const string GODOT_BIN_NAME = "godot";

  public const string GODOT_SHARP_PATH = "GodotSharp";

  /// <summary>
  /// Directory where Godot installer downloads are cached temporarily,
  /// relative to <see cref="GODOT_PATH" />.
  /// </summary>
  public const string GODOT_CACHE_PATH = "cache";

  /// <summary>
  /// Default network client download configuration.
  /// </summary>
  public static readonly DownloadConfiguration DownloadConfiguration = new() {
    ChunkCount = 8,
    ParallelDownload = true,
    ReserveStorageSpaceBeforeStartingDownload = true
  };

  /// <summary>
  /// Filename used to indicate that a Godot download was completely downloaded.
  /// Used internally.
  /// </summary>
  public const string DID_FINISH_DOWNLOAD_FILE_NAME = ".done";

  /// <summary>
  /// Addons file that generated when initializing addons. This gives
  /// users a nice starting point to work with, as well as a reference for
  /// a typical addons file entry.
  /// </summary>
  public const string ADDONS_FILE = /*lang=json*/"""
  // Godot addons configuration file for use with the GodotEnv tool.
  // See https://github.com/chickensoft-games/GodotEnv for more info.
  // -------------------------------------------------------------------- //
  // Note: this is a JSONC file, so you can use comments!
  // If using Rider, see https://youtrack.jetbrains.com/issue/RIDER-41716
  // for any issues with JSONC.
  // -------------------------------------------------------------------- //
  {
    "$schema": "https://chickensoft.games/schemas/addons.schema.json",
    // "path": "addons", // default
    // "cache": ".addons", // default
    "addons": {
      "imrp": { // name must match the folder name in the repository
        "url": "https://github.com/MakovWait/improved_resource_picker",
        // "source": "remote", // default
        // "checkout": "main", // default
        "subfolder": "addons/imrp"
      }
    }
  }
  """;

  /// <summary>
  /// Editor config file generated when initializing addons. This prevents
  /// C# script code in addons from causing analyzer issues in the local
  /// user's project environment.
  /// </summary>
  public const string ADDONS_EDITOR_CONFIG_FILE = /*lang=editorconfig*/"""
  # Editor configs in nested directories override those in parent directories
  # for the directory in which they are placed.
  #
  # This editor config prevents the code editor from analyzing C# files which
  # belong to addons.
  #
  # Ignoring C# addon scripts is generally preferable, since C# can be coded
  # in a variety of ways that may or may not trigger warnings based on your
  # own editorconfig or IDE settings.

  [*.cs]
  generated_code = true
  """;

  public const string ADDONS_GIT_IGNORE_FILE = /*lang=gitignore*/$"""
  # .gitignore file
  # https://github.com/github/gitignore
  #
  # This file was generated by GodotEnv.
  # https://github.com/chickensoft-games/GodotEnv
  #
  # -------------------------------------------------------------------- #

  # Godot
  # https://github.com/github/gitignore/blob/main/Godot.gitignore
  #
  # Godot 4+ specific ignores
  .godot/
  #
  # Godot-specific ignores
  .import/
  export.cfg
  export_presets.cfg
  #
  # Imported translations (automatically generated from CSV files)
  *.translation
  #
  # Mono-specific ignores
  .mono/
  data_*/
  mono_crash.*.json
  # -------------------------------------------------------------------- #

  # GodotEnv Addons
  #
  # Ignore all addons since they are managed by GodotEnv:
  {ADDONS_PATH}/*
  #
  # Don't ignore the editorconfig file in the addons directory.
  !{ADDONS_PATH}/.editorconfig
  #
  # Ignore the addons cache
  {CACHE_PATH}/*
  # -------------------------------------------------------------------- #
  """;
}

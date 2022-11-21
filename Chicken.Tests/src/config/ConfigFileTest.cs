namespace Chickensoft.Chicken.Tests;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

public class ConfigFileTest {
  private const string URL
= "https://github.com/chickensoft-games/Chicken";
  private const string SUBFOLDER = "Chicken";
  private const string CHECKOUT = "main";
  private const string ADDON_NAME = "chicken";
  private readonly string _addonJson = "{" +
    $"  \"url\": \"{URL}\"," +
    $"  \"subfolder\": \"{SUBFOLDER}\"," +
    $"  \"checkout\": \"{CHECKOUT}\"" +
  "}";

  private const string CACHE_PATH = ".addons";
  private const string ADDONS_PATH = "addons";

  [Fact]
  public void Deserializes() {
    var json = "{" +
      "  \"addons\": {" + ADDON_NAME + $": {_addonJson}" + "}," +
      $"  \"cache\": \"{CACHE_PATH}\"," +
      $"  \"path\": \"{ADDONS_PATH}\"" +
    "}";
    var config = JsonConvert.DeserializeObject<ConfigFile>(json);
    config.ShouldNotBeNull();
    config.Addons.ShouldNotBeNull();
    config.Addons.ShouldNotBeEmpty();
    config.Addons.ShouldContainKey(ADDON_NAME);
    config.Addons[ADDON_NAME].ShouldBe(
      new AddonConfig(url: URL, subfolder: SUBFOLDER, checkout: CHECKOUT)
    );
    config.CachePath.ShouldBe(CACHE_PATH);
    config.AddonsPath.ShouldBe(ADDONS_PATH);
  }

  [Fact]
  public void DeserializesWithDefaults() {
    var json = "{" +
      "  \"addons\": null," +
      $"  \"cache\": null," +
      $"  \"path\": null" +
    "}";
    var config = JsonConvert.DeserializeObject<ConfigFile>(json);
    config.ShouldNotBeNull();
    config.Addons.ShouldNotBeNull();
    config.Addons.ShouldBeEmpty();
    config.CachePath.ShouldBe(App.DEFAULT_CACHE_PATH);
    config.AddonsPath.ShouldBe(App.DEFAULT_ADDONS_PATH);
  }

  [Fact]
  public void DeserializesWithMissingValues() {
    var json = "{}";
    var config = JsonConvert.DeserializeObject<ConfigFile>(json);
    config.ShouldNotBeNull();
    config.Addons.ShouldNotBeNull();
    config.Addons.ShouldBeEmpty();
    config.CachePath.ShouldBe(App.DEFAULT_CACHE_PATH);
    config.AddonsPath.ShouldBe(App.DEFAULT_ADDONS_PATH);
  }

  [Fact]
  public void CreatesConfig() {
    var configFile = new ConfigFile(
      addons: new(),
      cachePath: CACHE_PATH,
      addonsPath: ADDONS_PATH
    );
    var config = configFile.ToConfig(".");
    config.ShouldNotBeNull();
    config.ProjectPath.ShouldBe(".");
    config.CachePath.ShouldBe($"./{CACHE_PATH}");
    config.AddonsPath.ShouldBe($"./{ADDONS_PATH}");
  }
}

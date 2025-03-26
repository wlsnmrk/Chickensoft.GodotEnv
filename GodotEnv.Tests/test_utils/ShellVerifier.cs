namespace Chickensoft.GodotEnv.Tests;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chickensoft.GodotEnv.Common.Utilities;
using Moq;
using Shouldly;

public enum RunMode {
  Run = 0,
  RunUnchecked = 1,
  RunWithUpdates = 2,
}

/// <summary>
/// Test utility class that makes it easier to verify the order of
/// shell commands that were executed.
/// </summary>
public class ShellVerifier {
  private readonly Dictionary<string, Mock<IShell>> _shells = new();
  private readonly MockSequence _sequence = new();
  private int _calls;
  private int _added;

  public ShellVerifier(params string[] paths) {
    foreach (var path in paths) {
      CreateShell(path);
    }
  }

  /// <summary>
  /// Creates a mock shell for the given working directory.
  /// </summary>
  /// <param name="workingDir">Directory in which the shell commands should
  /// be run from.</param>
  /// <returns>The mocked shell.</returns>
  public Mock<IShell> CreateShell(string workingDir) {
    var shell = new Mock<IShell>(MockBehavior.Strict);
    _shells.Add(workingDir, shell);
    return shell;
  }

  /// <summary>
  /// Gets a mocked shell, or throws if one is not mocked.
  /// </summary>
  /// <param name="workingDir">Shell path.</param>
  /// <returns>Mock shell.</returns>
  /// <exception cref="InvalidOperationException" />
  public Mock<IShell> GetShell(string workingDir) =>
    _shells[workingDir] ??
    throw new InvalidOperationException($"Shell not found: {workingDir}");

  /// <summary>
  /// Adds a mock shell command to be verified later.
  /// </summary>
  /// <param name="workingDir">Directory in which the shell command should
  /// be run. Must have created a mock shell previously for this
  /// directory.</param>
  /// <param name="result">Execution result.</param>
  /// <param name="exe">Cli executable.</param>
  /// <param name="args">Executable args.</param>
  /// <exception cref="InvalidOperationException" />
  public void Runs(
    string workingDir, ProcessResult result, string exe, params string[] args
  ) =>
    MockProcess(
      workingDir,
      result,
      RunMode.Run,
      exe,
      (str) => { },
      (str) => { },
      args
    );

  /// <summary>
  /// Adds a mock shell command to be verified later (doesn't care if the
  /// process returns non-zero).
  /// </summary>
  /// <param name="workingDir">Directory in which the shell command should
  /// be run. Must have created a mock shell previously for this
  /// directory.</param>
  /// <param name="result">Execution result.</param>
  /// <param name="exe">Cli executable.</param>
  /// <param name="args">Executable args.</param>
  /// <exception cref="InvalidOperationException" />
  public void RunsUnchecked(
    string workingDir, ProcessResult result, string exe, params string[] args
  ) =>
    MockProcess(
      workingDir,
      result,
      RunMode.RunUnchecked,
      exe,
      (str) => { },
      (str) => { },
      args
    );

  /// <summary>
  /// Adds a mock shell command to be verified later (doesn't care if the
  /// process returns non-zero).
  /// </summary>
  /// <param name="workingDir">Directory in which the shell command should
  /// be run. Must have created a mock shell previously for this
  /// directory.</param>
  /// <param name="result">Execution result.</param>
  /// <param name="exe">Cli executable.</param>
  /// <param name="onStdOut">Standard output callback.</param>
  /// <param name="onStdError">Standard error callback.</param>
  /// <param name="args">Executable args.</param>
  /// <exception cref="InvalidOperationException" />
  public void RunsWithUpdates(
    string workingDir,
    ProcessResult result,
    string exe,
    Action<string> onStdOut,
    Action<string> onStdError,
    params string[] args
  ) =>
    MockProcess(
      workingDir,
      result,
      RunMode.RunWithUpdates,
      exe,
      onStdOut,
      onStdError,
      args
    );

  /// <summary>
  /// After creating mock shells and setting up verification calls, call
  /// this to verify that all of your mocked calls are actually run in the
  /// expected order by the system under test.
  /// </summary>
  /// <exception cref="InvalidOperationException" />
  public void VerifyAll() {
    foreach (var (_, value) in _shells) {
      value.VerifyAll();
    }
    if (_calls < _added) {
      throw new InvalidOperationException(
        $"{_calls} calls were made, but {_added} were added. " +
        $"Missing {_added - _calls} calls."
      );
    }
  }

  private void MockProcess(
    string workingDir,
    ProcessResult result,
    RunMode runMode,
    string exe,
    Action<string> onStdOut,
    Action<string> onStdError,
    params string[] args
  ) {
    if (_shells.TryGetValue(workingDir, out var sh)) {
      MockShell(sh, result, runMode, exe, onStdOut, onStdError, args);
    }
    else {
      throw new InvalidOperationException($"Shell not found: {workingDir}");
    }
  }

  private void MockShell(
    Mock<IShell> shell,
    ProcessResult result,
    RunMode runMode,
    string exe,
    Action<string> onStdOut,
    Action<string> onStdError,
    string[] args
  ) {
    var call = _added++;
    switch (runMode) {
      case RunMode.Run:
        shell.InSequence(_sequence).Setup(shell => shell.Run(exe, args))
        .Returns(Task.FromResult(result))
        .Callback(() => _calls++.ShouldBe(call));
        break;
      case RunMode.RunUnchecked:
        shell.InSequence(_sequence).Setup(
          shell => shell.RunUnchecked(exe, args)
        )
        .Returns(Task.FromResult(result))
        .Callback(() => _calls++.ShouldBe(call));
        break;
      case RunMode.RunWithUpdates:
        shell.InSequence(_sequence).Setup(
          shell => shell.RunWithUpdates(exe, onStdOut, onStdError, args)
        )
        .Returns(Task.FromResult(result))
        .Callback(() => _calls++.ShouldBe(call));
        break;
      default:
        throw new ArgumentException($"Unrecognized run mode {runMode}");
    }
  }
}

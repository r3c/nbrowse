using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace NBrowse.CLI.Test;

public class EngineTest
{
    /// <summary>
    /// Relative path to NBrowse DLL.
    /// </summary>
    private const string NBrowsePath = "../../../../NBrowse/bin/Debug/net10.0/NBrowse.dll";

    [Test]
    public async Task AssemblyText()
    {
        await Run(new[]
        {
            "project.Assemblies.Select(a => a.Name)",
            "data/AssemblyText/assemblies.txt"
        }, "data/AssemblyText/expected.out");
    }

    [Test]
    public async Task QueryConstant()
    {
        await Run(new[] { "42", NBrowsePath }, "data/QueryConstant/expected.out");
    }

    [Test]
    public async Task QueryFile()
    {
        await Run(new[] { "-f", "data/QueryFile/input.nbrowse", NBrowsePath }, "data/QueryFile/expected.out");
    }

    private static async Task Run(IEnumerable<string> projectArguments, string expectedPath)
    {
        var dotnetArguments = new[]
        {
            "run",
            "--framework",
            "net10.0",
            "--no-build",
            "--no-restore",
            "--project",
            "../../../../NBrowse.CLI/NBrowse.CLI.csproj",
            "--"
        };

        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

        var processStartInfo = new ProcessStartInfo("dotnet")
        {
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            WorkingDirectory = baseDirectory
        };

        foreach (var argument in dotnetArguments.Concat(projectArguments))
            processStartInfo.ArgumentList.Add(argument);

        var process = Process.Start(processStartInfo);

        Assert.That(process, Is.Not.Null);

        var tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));

        await process!.WaitForExitAsync(tokenSource.Token);

        Assert.That(process.ExitCode, Is.EqualTo(0), await process.StandardError.ReadToEndAsync());

        var expectedFullPath = Path.Combine(baseDirectory, expectedPath);
        var expected = await File.ReadAllLinesAsync(expectedFullPath, tokenSource.Token);

        var lines = new List<string>();

        while (true)
        {
            var line = await process.StandardOutput.ReadLineAsync();

            if (line is not null)
                lines.Add(line);
            else
                break;
        }

        Assert.That(lines, Is.EqualTo(expected));
    }
}
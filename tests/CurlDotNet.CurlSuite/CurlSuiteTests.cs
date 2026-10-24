using System.Collections.Concurrent;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace CurlDotNet.CurlSuite;

/// <summary>
/// Collects outcomes across the whole run and writes <c>curl-suite-report.md</c> next to the
/// project (or to $CURL_SUITE_REPORT) so the pass rate against curl's own suite is visible.
/// </summary>
public sealed class CurlSuiteReport : IDisposable
{
    public static readonly ConcurrentBag<SuiteOutcome> Outcomes = new();

    public void Dispose()
    {
        var outcomes = Outcomes.OrderBy(o => o.Number).ToList();
        if (outcomes.Count == 0) return;

        var pass = outcomes.Count(o => o.Status == SuiteStatus.Pass);
        var strict = outcomes.Count(o => o.Status == SuiteStatus.Pass && o.Strict);
        var fail = outcomes.Count(o => o.Status == SuiteStatus.Fail);
        var skip = outcomes.Count(o => o.Status == SuiteStatus.Skip);
        var known = KnownFailures.Numbers;

        var sb = new StringBuilder();
        sb.AppendLine("# curl test suite replay report");
        sb.AppendLine();
        sb.AppendLine($"Source: {ReadSource()}");
        sb.AppendLine();
        sb.AppendLine("| Result | Count |");
        sb.AppendLine("|---|---|");
        sb.AppendLine($"| Pass | {pass} |");
        sb.AppendLine($"| Pass (byte-exact incl. header order) | {strict} |");
        sb.AppendLine($"| Fail | {fail} |");
        sb.AppendLine($"| Fail (known, listed in known-failures.txt) | {outcomes.Count(o => o.Status == SuiteStatus.Fail && known.Contains(o.Number))} |");
        sb.AppendLine($"| Skip (needs servers/features CurlDotNet lacks) | {skip} |");
        sb.AppendLine($"| Total files | {outcomes.Count} |");
        sb.AppendLine();
        sb.AppendLine($"Pass rate over runnable tests: {pass}/{pass + fail} ({(pass + fail == 0 ? 0 : 100.0 * pass / (pass + fail)):F1}%)");
        sb.AppendLine();
        sb.AppendLine("## Failures");
        sb.AppendLine();
        sb.AppendLine("| Test | Name | Reason |");
        sb.AppendLine("|---|---|---|");
        foreach (var o in outcomes.Where(o => o.Status == SuiteStatus.Fail))
        {
            sb.AppendLine($"| {o.Number} | {Escape(o.Name)} | {Escape(o.Reason)} |");
        }
        sb.AppendLine();
        sb.AppendLine("## Skipped");
        sb.AppendLine();
        foreach (var group in outcomes.Where(o => o.Status == SuiteStatus.Skip).GroupBy(o => o.Reason).OrderByDescending(g => g.Count()))
        {
            sb.AppendLine($"- {Escape(group.Key)}: {group.Count()} ({string.Join(" ", group.Take(12).Select(o => o.Number))}{(group.Count() > 12 ? " ..." : string.Empty)})");
        }

        var target = Environment.GetEnvironmentVariable("CURL_SUITE_REPORT");
        if (string.IsNullOrEmpty(target))
        {
            target = Path.Combine(ProjectDirectory(), "curl-suite-report.md");
        }
        File.WriteAllText(target, sb.ToString());
    }

    private static string Escape(string s) => s.Replace("|", "\\|").Replace("\n", " ");

    private static string ReadSource()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "data", "SOURCE.txt");
        return File.Exists(path) ? File.ReadAllText(path).Trim() : "unknown";
    }

    public static string ProjectDirectory()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null && !File.Exists(Path.Combine(dir.FullName, "CurlDotNet.CurlSuite.csproj")))
        {
            dir = dir.Parent;
        }
        return dir?.FullName ?? AppContext.BaseDirectory;
    }
}

/// <summary>Reads known-failures.txt: one test number per line, optional "# reason".</summary>
public static class KnownFailures
{
    public static readonly HashSet<int> Numbers = Load();

    private static HashSet<int> Load()
    {
        var set = new HashSet<int>();
        var path = Path.Combine(AppContext.BaseDirectory, "known-failures.txt");
        if (!File.Exists(path)) return set;
        foreach (var raw in File.ReadAllLines(path))
        {
            var line = raw.Split('#')[0].Trim();
            if (int.TryParse(line, out var n)) set.Add(n);
        }
        return set;
    }
}

[Trait("Category", "CurlSuite")]
public sealed class CurlSuiteTests : IClassFixture<CurlSuiteReport>
{
    private readonly ITestOutputHelper _output;

    public CurlSuiteTests(ITestOutputHelper output)
    {
        _output = output;
    }

    public static IEnumerable<object[]> TestNumbers()
    {
        var dataDir = Path.Combine(AppContext.BaseDirectory, "data");
        return Directory.GetFiles(dataDir, "test*")
            .Select(f => Path.GetFileName(f).Substring(4))
            .Where(n => int.TryParse(n, out _))
            .Select(int.Parse)
            .OrderBy(n => n)
            .Select(n => new object[] { n });
    }

    [Theory]
    [MemberData(nameof(TestNumbers))]
    public async Task CurlTest(int number)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "data", $"test{number}");
        var outcome = await CurlSuiteRunner.RunAsync(path);
        CurlSuiteReport.Outcomes.Add(outcome);

        _output.WriteLine($"test{number}: {outcome.Name}");
        _output.WriteLine($"  {outcome.Status}{(outcome.Strict ? " (byte-exact)" : string.Empty)} {outcome.Reason}");
        if (outcome.Command.Length > 0) _output.WriteLine($"  {outcome.Command}");

        var known = KnownFailures.Numbers.Contains(number);
        switch (outcome.Status)
        {
            case SuiteStatus.Skip:
                return;
            case SuiteStatus.Pass when known:
                Assert.Fail($"curl test {number} ({outcome.Name}) now passes: remove it from known-failures.txt");
                return;
            case SuiteStatus.Fail when known:
                return; // expected for now
            case SuiteStatus.Fail:
                Assert.Fail($"curl test {number} ({outcome.Name}): {outcome.Reason}\n  command: {outcome.Command}");
                return;
        }
    }
}

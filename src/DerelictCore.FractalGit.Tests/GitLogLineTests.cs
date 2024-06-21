using DerelictCore.FractalGit.Models;

namespace DerelictCore.FractalGit.Tests;

public class GitLogLineTests
{
    public const string TestRepositoryUrl = "https://github.com/submodule-test-archive/Open-Source-Orchard-Core-Extensions.git";

    private readonly ITestOutputHelper _output;

    public GitLogLineTests(ITestOutputHelper output) => _output = output;

    [Fact]
    public async Task GitGraphShouldMatchExpectation()
    {
        // Calculate a unique test path inside the operating system's temporary directory. If anything fails here there
        // is nothing to clean up yet.
        var path = CreateGitDirectory();

        try
        {
            // Clone an archived test repository for consistent results.
            await GitAsync(
                    workingDirectory: Path.GetDirectoryName(path)!,
                    "clone",
                    TestRepositoryUrl,
                    Path.GetFileName(path));

            // Get all git log results.
            var lines = (await GitLogLine.ExecuteAsync("git", path)).ToList();

            // Uncomment this if GitLogLine has changed to update the sample.
            //// await File.WriteAllTextAsync(
            ////     "/home/sarah/Projects/DerelictCore.FractalGit/src/DerelictCore.FractalGit/Models/GitLogLine.json",
            ////     JsonSerializer.Serialize(lines));

            // Take and format the first several results for comparison.
            var actual = lines
                .Take(15)
                .Select(line => (
                    line.Graph ?? string.Empty,
                    line.Hash ?? string.Empty,
                    line.AuthorName ?? string.Empty,
                    line.AuthorEmail ?? string.Empty,
                    line.AuthorDate?.Ticks ?? 0,
                    string.Join('|', line.RefNames),
                    line.Subject ?? string.Empty))
                .ToArray();

            actual.ShouldBe(new[]
            {
#pragma warning disable
                ("*"                 , "198c21c83dd84646b349b9491b2895eb0beba4ba", "Lombiq Bot"     , "bot@lombiq.com"            , 638532870150000000, "HEAD -> dev|origin/dev|origin/HEAD", "Merge pull request #773 from Lombiq/issue/OSOE-862"),
                ("| *"               , "94ce07e27a18d3444d5c87f43c7dcf6e7b325a35", "Benedek Farkas" , "benedek.farkas@lombiq.com" , 638532857610000000, string.Empty                        , "Updating LGHA to dev after merging issue branch"),
                ("| *"               , "d23946f55115c6f08c99d25754746000a2ece940", "Zoltán Lehóczky", "zoltan.lehoczky@lombiq.com", 638532802020000000, string.Empty                        , "Grammar and commit message fix in Lombiq.GitHub.Actions"),
                ("| *"               , "9baadb847c4b46018627e91dc01fbf07f72553a5", "Zoltán Lehóczky", "zoltan.lehoczky@lombiq.com", 638532799660000000, string.Empty                        , "Merge remote-tracking branch 'origin/dev' into issue/OSOE-862"),
                ("| * \\"            , "8e0e45d83ff7cdc9f9fcb16c39385dc8e7b3d6ce", "Zoltán Lehóczky", "zoltan.lehoczky@lombiq.com", 638532031760000000, string.Empty                        , "Merge remote-tracking branch 'origin/dev' into issue/OSOE-862"),
                ("| * \\ \\"         , "c34af5c2d1936578d09998ba16e1a1544a636f1d", "Zoltán Lehóczky", "zoltan.lehoczky@lombiq.com", 638531917750000000, string.Empty                        , "Merge remote-tracking branch 'origin/dev' into issue/OSOE-862"),
                ("| * \\ \\ \\"      , "3576bb9d6f3389b78c209ba001a97269f61fa5ac", "Zoltán Lehóczky", "zoltan.lehoczky@lombiq.com", 638527691460000000, string.Empty                        , "Merge remote-tracking branch 'origin/dev' into issue/OSOE-862"),
                ("| * \\ \\ \\ \\"   , "ff8ec4f0b925c0bcb7060f9c6162ff6cc37af0cd", "Zoltán Lehóczky", "zoltan.lehoczky@lombiq.com", 638518430630000000, string.Empty                        , "Merge remote-tracking branch 'origin/dev' into issue/OSOE-862"),
                ("| * \\ \\ \\ \\ \\", "9711aa0bdd362ccadb8cd8c603700cddbfe160ca", "Zoltán Lehóczky", "zoltan.lehoczky@lombiq.com", 638518363970000000, string.Empty                        , "Merge remote-tracking branch 'origin/dev' into issue/OSOE-862"),
                ("| * | | | | | |"   , "03882fb6ee4ca129a568f1e6bde1bfeb23957189", "Zoltán Lehóczky", "zoltan.lehoczky@lombiq.com", 638518357180000000, string.Empty                        , "Updating Lombiq.GitHub.Actions to latest"),
                ("| * | | | | | |"   , "9aa93b2ddffbd0c883216d2f8bf33c1af164e0c5", "Zoltán Lehóczky", "zoltan.lehoczky@lombiq.com", 638518342870000000, string.Empty                        , "Updating Lombiq.GitHub.Actions to latest"),
                ("* | | | | | | |"   , "8676bf8e6dcfe1c575e814365664375fc860bd4b", "Lombiq Bot"     , "bot@lombiq.com"            , 638532853590000000, string.Empty                        , "Merge pull request #783 from Lombiq/issue/OCC-245-cloudsmith"),
                ("| * | | | | | |"   , "ef44d4e08b94f2b21b6dfaab796791d895fb0565", "Sára El-Saig"   , "sara.el-saig@lombiq.com"   , 638532842250000000, string.Empty                        , "submodules"),
                ("| * | | | | | |"   , "cd6506c63b0041cc4fe43df4a14d6eb0c3217674", "Sára El-Saig"   , "sara.el-saig@lombiq.com"   , 638532686380000000, string.Empty                        , "Merge remote-tracking branch 'origin/dev' into issue/OCC-245-cloudsmith"),
                ("* | | | | | | |"   , "bc50f9f589d18f4fea6205ce755e2a3935f2efa7", "Lombiq Bot"     , "bot@lombiq.com"            , 638532153610000000, string.Empty                        , "Merge pull request #786 from Lombiq/issue/OSOE-744"),
#pragma warning restore
            });
        }
        finally
        {
            // Try to clean up clone in the test directory. If this fails that's not a huge problem but could lead to a
            // memory/storage leak, so it should be avoided.
            try
            {
                Directory.Delete(path, recursive: true);
            }
            catch (Exception exception)
            {
                _output.WriteLine($"Failed to delete \"{path}\":\n{exception}");
            }
        }
    }

    private async Task GitAsync(string workingDirectory, params string[] arguments)
    {
        var gitCommand = "git " + string.Join(' ', arguments);
        _output.WriteLine($"Starting command [{gitCommand}] in {workingDirectory}...");

        await Cli.Wrap("git")
            .WithArguments(arguments)
            .WithWorkingDirectory(workingDirectory)
            .WithStandardOutputPipe(PipeTarget.ToDelegate(_output.WriteLine))
            .WithStandardErrorPipe(PipeTarget.ToDelegate(_output.WriteLine))
            .ExecuteAsync();

        _output.WriteLine($"Finished command [{gitCommand}] in {workingDirectory}.\n\n");
    }

    private static string CreateGitDirectory()
    {
        if (!OperatingSystem.IsWindows() && !OperatingSystem.IsLinux())
        {
            throw new NotSupportedException("Only Windows and Linux are supported.");
        }

        var temp = OperatingSystem.IsWindows()
            ? Environment.GetEnvironmentVariable("TEMP")
            : "/tmp/";

        if (!Directory.Exists(temp))
        {
            throw new InvalidOperationException($"The temp directory ({temp}) does not exist!");
        }

        return Path.Join(temp, $"fractalgit-test-{Guid.NewGuid():D}");
    }
}

using Avalonia.Media;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using DerelictCore.FractalGit.Abstractions;
using DerelictCore.FractalGit.Abstractions.Services;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using GitLogLine = DerelictCore.FractalGit.Abstractions.Models.GitLogLine;

namespace DerelictCore.FractalGit.ViewModels;

public partial class CommitDetailsViewModel : ViewModelBase
{
    private static HttpClient _gravatarClient = new() { BaseAddress = new("https://gravatar.com/avatar/") };

    public static string AuthorImageCachePath { get; }
    public static ConcurrentDictionary<string, string> AuthorImageCache { get; }

    [ObservableProperty]
    private GitLogLine _line;

    [ObservableProperty]
    private string? _body;

    // Get via `git symbolic-ref refs/remotes/origin/HEAD`, add converter to trim out 'refs/remotes/origin/' for display.
    [ObservableProperty]
    private string? _defaultBranchName;

    // Get via `git rev-list --left-right --count refs/remotes/origin/dev...cd6506c63b0041cc4fe43df4a14d6eb0c3217674`.
    [ObservableProperty]
    private int _commitsAhead;

    [ObservableProperty]
    private int _commitsBehind;

    [ObservableProperty]
    private IImage? _authorImage;

    public ObservableCollection<string> ParentHashes { get; set; } = [];
    public ObservableCollection<string> LineHash { get; set; } = [];

    // Commands to check:
    // git branch -a --contains cd6506c63b0041cc4fe43df4a14d6eb0c3217674
    // git name-rev cd6506c63b0041cc4fe43df4a14d6eb0c3217674
    // git name-rev --name-only --exclude=tags/* $SHA
    // git log c0118fa..HEAD --ancestry-path --merges --oneline --color | tail -n 1
    public ObservableCollection<string> ContainingBranches { get; set; } = [];

    public MainWindowViewModel? Main { get; }

    public CommitDetailsViewModel(MainWindowViewModel? main, GitLogLine? line = null)
    {
        Main = main;
        Line = line ?? new();

        // Intentional fire-and-forget call.
#pragma warning disable VSTHRD110
#pragma warning disable MA0134
        UpdateDetailsAsync();
#pragma warning restore MA0134
#pragma warning restore VSTHRD110
    }

    protected override async void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.PropertyName is nameof(Line))
        {
            await UpdateDetailsAsync();
        }
    }

    private Task UpdateDetailsAsync() =>
        Main?.Graph?.WorkingDirectory?.Trim() is { Length: > 0 } cwd && Line.Hash is { } hash
            ? UpdateDetailsInnerAsync(new GitService("git", cwd, NullLogger.Instance), hash)
            : Task.CompletedTask;

    private async Task UpdateDetailsInnerAsync(GitService service, string hash)
    {
        try
        {
            Body = string.Join('\n', await service.GitWithOutputAsync("show", "--format=format:%B", hash)).Trim();
            ParentHashes.SetItems((await service.GitWithOutputAsync("show", "--format=format:%P", hash))
                .WhereNot(string.IsNullOrWhiteSpace)
                .FirstOrDefault()?
                .Split()
                .WhereNot(string.IsNullOrEmpty));
            LineHash.SetItems([hash]);

            if (Line.AuthorEmail?.Trim().ToLowerInvariant() is { Length: > 0 } authorEmail)
            {
                if (!AuthorImageCache.TryGetValue(authorEmail, out var imagePath) || !File.Exists(imagePath))
                {
                    var gravatarHash = Convert
                        .ToHexString(SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(authorEmail)))
                        .ToLowerInvariant();
                    var imageBytes = await _gravatarClient.GetByteArrayAsync($"{gravatarHash}.jpg?s={128}");

                    imagePath = Path.Join(AuthorImageCachePath, $"{WebUtility.UrlEncode(authorEmail)}.jpg");
                    await File.WriteAllBytesAsync(imagePath, imageBytes);
                    AuthorImageCache[authorEmail] = imagePath;
                }

                (AuthorImage as IDisposable)?.Dispose();
                AuthorImage = new Bitmap(imagePath);
            }
        }
        catch
        {
            // Ignore errors in async event handling.
        }
    }

    static CommitDetailsViewModel()
    {
        AuthorImageCachePath = Path.Join(Path.GetTempPath(), CommonConstants.FractalGit, nameof(AuthorImageCache));

        if (Directory.Exists(AuthorImageCachePath))
        {
            AuthorImageCache = new(Directory
                .GetFiles(AuthorImageCachePath, "*.jpg", SearchOption.TopDirectoryOnly)
                .ToDictionary(
                    fileName => WebUtility.UrlDecode(Path.GetFileNameWithoutExtension(fileName)),
                    fileName => fileName));
        }
        else
        {
            Directory.CreateDirectory(AuthorImageCachePath);
            AuthorImageCache = new();
        }
    }
}

public class DesignCommitDetailsViewModel : CommitDetailsViewModel
{
    public DesignCommitDetailsViewModel()
        : base(main: null, line: new()
        {
            Graph = "| * | | | | | |",
            Hash = "cd6506c63b0041cc4fe43df4a14d6eb0c3217674",
            AuthorName = "Sára El-Saig",
            AuthorEmail = "sara.el-saig@lombiq.com",
            AuthorDate = new DateTime(2024, 6, 6, 11, 3, 58, DateTimeKind.Local),
            RefNames = ["HEAD -\u003E dev", "origin/dev", "origin/HEAD"],
            Subject = "Merge remote-tracking branch 'origin/dev' into issue/OCC-245-cloudsmith",
        })
    {
    }
}

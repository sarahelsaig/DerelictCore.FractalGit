using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.LogicalTree;
using DerelictCore.FractalGit.Services;
using DerelictCore.FractalGit.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace DerelictCore.FractalGit.Controls;

public partial class CommitDetails : UserControl
{
    public CommitDetails() => InitializeComponent();

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);

        if (Application.Current is App { ServiceProvider: { } provider }) _ = AddTabsAsync(provider);
    }

    private async Task AddTabsAsync(IServiceProvider provider)
    {
        foreach (var handler in provider.GetServices<ICommitDetailsHandler>())
        {
            foreach (var (header, child) in await handler.AddTabAsync(this))
            {
                TabControl.Items.Add(new TabItem
                {
                    Header = header,
                    Content = child,
                });
            }
        }
    }

    private async void LineHash_OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (DataContext is CommitDetailsViewModel { Line.Hash: { Length: >0 } hash })
            await TopLevel.GetTopLevel(this)!.Clipboard!.SetTextAsync(hash);
    }
}

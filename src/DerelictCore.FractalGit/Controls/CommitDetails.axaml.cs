using Avalonia.Controls;
using Avalonia.Input;
using DerelictCore.FractalGit.ViewModels;

namespace DerelictCore.FractalGit.Controls;

public partial class CommitDetails : UserControl
{
    public CommitDetails() => InitializeComponent();

    private async void LineHash_OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (DataContext is CommitDetailsViewModel { Line.Hash: { Length: >0 } hash })
            await TopLevel.GetTopLevel(this)!.Clipboard!.SetTextAsync(hash);
    }
}

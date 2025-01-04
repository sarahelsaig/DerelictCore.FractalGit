using Avalonia.Controls;
using Avalonia.Interactivity;
using DerelictCore.FractalGit.ViewModels;
using MsBox.Avalonia;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace DerelictCore.FractalGit.Views;

public partial class MainWindow : Window
{
    private MainWindowViewModel? ViewModel => DataContext as MainWindowViewModel;

    public MainWindow() => InitializeComponent();

    private void OpenRepository_OnClick(object? sender, RoutedEventArgs e) => ToDoMessage();

    private void Settings_OnClick(object? sender, RoutedEventArgs e) => ToDoMessage();

    private void ExploreDirectory_OnClick(object? sender, RoutedEventArgs e)
    {
        if (ViewModel?.Graph.WorkingDirectory is not { Length: > 0 } workingDirectory) return;

        var info = new ProcessStartInfo(workingDirectory) { UseShellExecute = true, Verb = "open" };
        Process.Start(info);
    }

    private void Exit_OnClick(object? sender, RoutedEventArgs e) => Close();

    private void Refresh_OnClick(object? sender, RoutedEventArgs e) => ToDoMessage();
    private void Fetch_OnClick(object? sender, RoutedEventArgs e) => ToDoMessage();
    private void Pull_OnClick(object? sender, RoutedEventArgs e) => ToDoMessage();
    private void Merge_OnClick(object? sender, RoutedEventArgs e) => ToDoMessage();
    private void Commit_OnClick(object? sender, RoutedEventArgs e) => ToDoMessage();

#pragma warning disable // We are doing some weird stuff here, but it's only a simple placeholder so it doesn't matter.
    private void ToDoMessage() =>
        MessageBoxManager.GetMessageBoxStandard("TODO", "Not yet implemented.").ShowAsync();
#pragma warning enable
}

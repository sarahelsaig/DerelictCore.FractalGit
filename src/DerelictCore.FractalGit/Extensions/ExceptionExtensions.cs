using MsBox.Avalonia;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia.Models;
using System.Threading.Tasks;

namespace System;

public static class ExceptionExtensions
{
    public static Task AlertAsync(this Exception exception, Icon icon = Icon.Error)
    {
        var box = MessageBoxManager.GetMessageBoxCustom(new MessageBoxCustomParams
        {
            ButtonDefinitions = [new ButtonDefinition { Name = "Ok" }],
            ContentTitle = exception.GetType().Name,
            ContentHeader = exception.Message.Split('\n', '\r')[0],
            ContentMessage = exception.ToString(),
            Icon = icon,
        });

        return box.ShowAsync();
    }
}

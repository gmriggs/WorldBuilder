using Avalonia.Controls;
using Avalonia.Controls.Templates;
using HanumanInstitute.MvvmDialogs.Avalonia;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;
using WorldBuilder.Lib;
using WorldBuilder.ViewModels;

namespace WorldBuilder.Services;

/// <summary>
/// Combined view locator that handles both regular views and dialog windows.
/// Implements Avalonia's IDataTemplate for general view resolution and extends ViewLocatorBase for dialog support.
/// </summary>
public class CombinedViewLocator : ViewLocatorBase, IDataTemplate {
    private readonly bool _preferWindows;

    public CombinedViewLocator() : this(false) { }

    public CombinedViewLocator(bool preferWindows) {
        _preferWindows = preferWindows;
    }

    public override Control Build(object? data) {
        if (data is null) {
            return new TextBlock { Text = "data was null" };
        }

        var name = GetViewName(data);
#pragma warning disable IL2057 // Unrecognized value passed to the parameter of method. It's not possible to guarantee the availability of the target type.
        var type = Type.GetType(name);
#pragma warning restore IL2057 // Unrecognized value passed to the parameter of method. It's not possible to guarantee the availability of the target type.

        if (type == null) {
            return new TextBlock { Text = "Not Found: " + name };
        }

        var control = App.Services?.GetService<ProjectManager>()?.GetProjectService<Control>(type);
        if (control != null) {
            return (Control)control!;
        }

        control = App.Services?.GetService(type) as Control;

        if (control != null) {
            return (Control)control!;
        }

        control = Activator.CreateInstance(type) as Control;

        if (control != null) {
            return (Control)control!;
        }

        return new TextBlock { Text = $"Not Found: {type.Name} {name}" };
    }

    public override bool Match(object? data) {
        return data is ViewModelBase;
    }

    /// <inheritdoc />
    [UnconditionalSuppressMessage("Trimming", "IL2026:Reflection is used to locate views", Justification = "View resolution by name is a standard pattern in this app")]
    protected override string GetViewName(object viewModel) {
        var viewModelType = viewModel.GetType();
        var viewModelName = viewModelType.FullName!;

        if (viewModelName.EndsWith("WindowViewModel")) {
            return viewModelName.Replace(".ViewModels.", ".Views.").Replace("ViewModel", "");
        }

        if (_preferWindows) {
            var windowName = viewModelName.Replace(".ViewModels.", ".Views.").Replace("ViewModel", "Window");
            var windowType = viewModelType.Assembly.GetType(windowName);
            if (windowType != null) {
                return windowName;
            }
        }

        // First try the standard Views namespace
        var standardViewName = viewModelName.Replace(".ViewModels.", ".Views.").Replace("ViewModel", "View");
        var standardViewType = viewModelType.Assembly.GetType(standardViewName);
        if (standardViewType != null) {
            return standardViewName;
        }

        // If not found, try the Views.Components namespace
        var componentsViewName = viewModelName.Replace(".ViewModels.", ".Views.Components.").Replace("ViewModel", "View");
        var componentsViewType = viewModelType.Assembly.GetType(componentsViewName);
        if (componentsViewType != null) {
            return componentsViewName;
        }

        // Return the standard name as fallback
        return standardViewName;
    }
}
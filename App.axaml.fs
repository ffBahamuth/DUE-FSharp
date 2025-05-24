namespace PathFinder

open Avalonia
open Avalonia.Controls.ApplicationLifetimes
open Avalonia.Markup.Xaml
open Avalonia.Threading

type App() =
    inherit Application()

    override this.Initialize() =
            AvaloniaXamlLoader.Load(this)
            AvaloniaSynchronizationContext.InstallIfNeeded()

    override this.OnFrameworkInitializationCompleted() =
        match this.ApplicationLifetime with
        | :? IClassicDesktopStyleApplicationLifetime as desktop ->
             desktop.MainWindow <- MainWindow()
        | _ -> ()

        base.OnFrameworkInitializationCompleted()

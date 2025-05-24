module PathFinder.UI.Controls

open Avalonia.Controls
open Avalonia.Markup.Xaml
open Avalonia.Media

type AlgorithmInfoPanel() as this =
    inherit UserControl()
    do
        AvaloniaXamlLoader.Load(this)
    
    member val AlgorithmName = "" with get, set
    member val VisitedCells = 0 with get, set
    member val PathLength = 0 with get, set
    member val ExecutionTime = System.TimeSpan.Zero with get, set
    
    
type GridSizeSelector() as this =
    inherit UserControl()
    do
        AvaloniaXamlLoader.Load(this)
    
    member val SelectedSize = 40 with get, set
    member val AvailableSizes = [20; 30; 40; 50] with get, set

type PathfindingVisualizerControl() as this =
    inherit UserControl()
    do
        AvaloniaXamlLoader.Load(this)
    
    member val Grid = Array2D.zeroCreate<int> 1 1 with get, set
    member val TileSize = 20.0 with get, set
    
    member this.DrawGrid() =
        let canvas = this.FindControl<Canvas>("PART_Canvas")
        canvas.Children.Clear()
        
        this.Grid |> Array2D.iteri (fun x y cellType ->
            let rect = Avalonia.Controls.Shapes.Rectangle()
            rect.Width <- this.TileSize
            rect.Height <- this.TileSize
            rect.Stroke <- Brushes.Black
            rect.Fill <-
                match cellType with
                | 0 -> Brushes.White   // Empty
                | 1 -> Brushes.Gray    // Wall
                | 2 -> Brushes.Green   // Start
                | 3 -> Brushes.Red     // End
                | 4 -> Brushes.LightGreen  // Processed
                | 5 -> Brushes.Orange  // Processing
                | 6 -> Brushes.Yellow  // Path
                | _ -> Brushes.White
            Canvas.SetLeft(rect, float(x) * this.TileSize)
            Canvas.SetTop(rect, float(y) * this.TileSize)
            canvas.Children.Add(rect))
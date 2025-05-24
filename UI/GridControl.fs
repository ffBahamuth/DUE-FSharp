module PathFinder.UI.GridControl

open Avalonia
open Avalonia.Controls
open Avalonia.Media

type GridControl(gridData: int[,], tileSize:float) =
    inherit Control()
    
    let mutable grid = gridData

    
    override this.Render(context) =
        base.Render(context)
        let pen = Pen(Brushes.Black, 1.0)
        let gridSizeX = grid.GetLength(0)
        let gridSizeY = grid.GetLength(1)
        
        for x in 0 .. gridSizeX - 1 do
            for y in 0 .. gridSizeY - 1 do
                let rect = Rect(float x * tileSize, float y * tileSize, tileSize, tileSize)
                let brush =
                    match grid.[x, y] with
                    | 0 -> Brushes.White   // üres
                    | 1 -> Brushes.Gray    // fall
                    | 2 -> Brushes.Green   // Start
                    | 3 -> Brushes.Red     // End
                    | 4 -> Brushes.LightGreen  // feldolgozás alatt
                    | 5 -> Brushes.Orange  // feldolgozás
                    | 6 -> Brushes.Yellow  // útvonal
                    | _ -> Brushes.White
                
                context.DrawRectangle(brush, pen, rect)
    

    /// Updates the grid and triggers a re-render
    member this.UpdateGrid(newGrid: int[,]) =
        grid <- newGrid
        this.InvalidateVisual()

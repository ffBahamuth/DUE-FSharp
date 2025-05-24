namespace PathFinder

open Avalonia
open Avalonia.Animation
open Avalonia.Controls
open Avalonia.Input
open Avalonia.Markup.Xaml
open Avalonia.Media
open System
open Algorithms.DFS
open Algorithms.BFS
open Algorithms.AStar
open Algorithms.Dijkstra
open Algorithms.AlgorithmBase
open Avalonia.Threading
open Models.PathFindingResult
open Models.Grid.Grid
open Avalonia.Controls
open PathFinder.Models.Grid.Grid
open PathFinder.Services.SimulationService

// A főablak kezelése a szimulációhoz
type MainWindow() as this =
    inherit Window()

    // Állandók
    let gridSize = 40
    let tileSize = 20.0
    
    // Állapotváltozók
    let mutable grid = create(gridSize)
    let mutable startPoint = None
    let mutable endPoint = None
    let mutable isDragging = false
    let mutable currentMode = "None"
    let mutable isControlEnabled = true
    let simulationService = SimulationService(this.UpdateInfoPanel,this.OnSimulationStateChange,this.ResetProcessedCells)

    do
        this.InitializeComponent()  // Az XAML betöltése és az események inicializálása
#if DEBUG
        this.AttachDevTools()
#endif
        this.DrawGrid()  // A rács kirajzolása

    // Az XAML betöltéséhez szükséges metódus
    member private this.InitializeComponent() =
        AvaloniaXamlLoader.Load(this)

        // Eseménykezelők hozzáadása
        this.FindControl<Canvas>("GameCanvas").PointerPressed.Add(this.OnCanvasPointerPressed)
        this.FindControl<Canvas>("GameCanvas").PointerMoved.Add(this.OnCanvasPointerMoved)
        this.FindControl<Canvas>("GameCanvas").PointerReleased.Add(fun _ -> isDragging <- false)
        this.FindControl<Button>("StartButton").Click.Add(this.OnStartClicked)
        this.FindControl<Button>("ResetButton").Click.Add(this.OnResetClicked)
        this.FindControl<ComboBox>("ModeSelector").SelectionChanged.Add(this.OnModeChanged)

        // Módváltozás kezelése
        this.OnModeChanged()

    // A rács kirajzolása, vagy frissítése
    member private this.DrawGrid() =
        let canvas = this.FindControl<Canvas>("GameCanvas")
        
        // Első alkalommal inicializálás
        if canvas.Children.Count = 0 then
            for x in 0 .. gridSize - 1 do
                for y in 0 .. gridSize - 1 do
                    let rect = Avalonia.Controls.Shapes.Rectangle()
                    rect.Width <- tileSize
                    rect.Height <- tileSize
                    rect.Stroke <- Brushes.Black
                    Canvas.SetLeft(rect, float x * tileSize)
                    Canvas.SetTop(rect, float y * tileSize)
                    canvas.Children.Add(rect)

        // A meglévő négyzetek frissítése
        for x in 0 .. gridSize - 1 do
            for y in 0 .. gridSize - 1 do
                let index = x * gridSize + y
                let rect = canvas.Children[index] :?> Avalonia.Controls.Shapes.Rectangle
                rect.Fill <-
                    match grid[x, y] with
                    | 0 -> Brushes.White   // Üres cella
                    | 1 -> Brushes.Gray    // Fal
                    | 2 -> Brushes.Green   // Kezdőpont
                    | 3 -> Brushes.Red     // Végpont
                    | 4 -> Brushes.LightGreen  // Feldolgozott cella
                    | 5 -> Brushes.Orange  // Feldolgozás alatt
                    | 6 -> Brushes.Yellow  // Útvonal
                    | _ -> Brushes.White

    // Biztosítja, hogy a rács frissítése a UI szálról történjen
    member this.SafeDrawGrid() =
        Dispatcher.UIThread.InvokeAsync(fun () -> this.DrawGrid()) |> ignore

    // A vezérlőelemek engedélyezési állapotának beállítása
    member this.SetControllerEnablesState(enabled: bool) =
        Dispatcher.UIThread.InvokeAsync(fun () ->
            this.FindControl<Button>("StartButton").IsEnabled <- enabled
            this.FindControl<Button>("ResetButton").IsEnabled <- enabled
            this.FindControl<ComboBox>("AlgorithmSelector").IsEnabled <- enabled
            this.FindControl<ComboBox>("ModeSelector").IsEnabled <- enabled
        ) |> ignore

    // Szimuláció állapotának változásakor végrehajtódó műveletek
    member this.OnSimulationStateChange(state: SimulationState) =
        if (state = Running) then
            this.SetControllerEnablesState(false)
            isControlEnabled <- false
        else
            this.SetControllerEnablesState(true)
            isControlEnabled <- true

    // Üzenet megjelenítése
    member this.ShowToast(message: string) =
        let toast = this.FindControl<Avalonia.Controls.Grid>("ToastContainer")
        let toastText = this.FindControl<TextBlock>("ToastText")
        toastText.Text <- message
        async {
            toast.Opacity <- 1
            do! Async.Sleep(2000) // 2 másodpercig látható lesz
            toast.Opacity <- 0
        } |> Async.StartImmediate

    // Biztosítja, hogy az ui szálról fusson
    member this.SafeShowToast(message: string) =
        Dispatcher.UIThread.InvokeAsync(fun () -> this.ShowToast(message)) |> ignore

    // A feldolgozott cellák alaphelyzetbe állítása
    member private this.ResetProcessedCells() =
        reset(grid)
        this.SafeDrawGrid()

    // Kezdőpont módosítása
    member private this.ChangeStartPoint(x, y) =
        if isControlEnabled then
            startPoint |> Option.iter (fun (oldX, oldY) -> grid[oldX, oldY] <- 0)
            startPoint <- Some (x, y)
            grid[x, y] <- 2
            this.SafeDrawGrid()

    // Végpont módosítása
    member private this.ChangeEndPoint(x, y) =
        if isControlEnabled then
            endPoint |> Option.iter (fun (oldX, oldY) -> grid[oldX, oldY] <- 0)
            endPoint <- Some (x, y)
            grid[x, y] <- 3
            this.SafeDrawGrid()

    // Az egér gomb lenyomásakor történő eseménykezelés
    member private this.OnCanvasPointerPressed(e: PointerPressedEventArgs) =
        if isControlEnabled then
            let pos = e.GetPosition(this.FindControl<Canvas>("GameCanvas"))
            let x = int (pos.X / tileSize)
            let y = int (pos.Y / tileSize)

            if x >= 0 && y >= 0 && x < gridSize && y < gridSize then
                isDragging <- true
                match currentMode with
                | "AddWall" -> grid[x, y] <- 1
                | "ClearWall" -> grid[x, y] <- 0
                | "SelectStart" -> this.ChangeStartPoint(x, y)
                | "SelectEnd" -> this.ChangeEndPoint(x, y)
                | _ -> ()
                this.SafeDrawGrid()

    // Az egér mozgatásakor történő eseménykezelés
    member private this.OnCanvasPointerMoved(e: PointerEventArgs) =
        if isControlEnabled && isDragging then
            let pos = e.GetPosition(this.FindControl<Canvas>("GameCanvas"))
            let x = int (pos.X / tileSize)
            let y = int (pos.Y / tileSize)

            if x >= 0 && y >= 0 && x < gridSize && y < gridSize then
                match currentMode with
                | "AddWall" ->
                    if grid[x,y] <> (int CellType.Start) && grid[x,y] <> (int CellType.End) then
                        grid[x, y] <- (int CellType.Wall)
                | "ClearWall" ->
                     if grid[x,y] = (int CellType.Wall) then
                        grid[x, y] <- (int CellType.Empty)
                | _ -> ()
                this.SafeDrawGrid()

    // A módváltozás kezelésére szolgáló metódus
    member private this.OnModeChanged(_: obj) =
        match this.FindControl<ComboBox>("ModeSelector").SelectedItem with
        | :? ComboBoxItem as item -> currentMode <- item.Tag.ToString()
        | _ -> currentMode <- "None"

    // A Start gomb megnyomásakor végrehajtódó műveletek
    member private this.OnStartClicked(_: obj) =
        match startPoint, endPoint with
        | Some (sx, sy), Some (ex, ey) ->
            let algorithmName =
                match this.FindControl<ComboBox>("AlgorithmSelector").SelectedItem with
                | :? ComboBoxItem as item -> item.Tag.ToString()
                | _ -> "Unknown"

            let algorithm =
                match algorithmName with
                | "BFS" -> BFSAlgorithm(grid, this.SafeDrawGrid) :> PathFindingAlgorithm
                | "DFS" -> DFSAlgorithm(grid, this.SafeDrawGrid) :> PathFindingAlgorithm
                | "AStar" -> AStarAlgorithm(grid, this.SafeDrawGrid) :> PathFindingAlgorithm
                | "Dijkstra" -> DijkstraAlgorithm(grid, this.SafeDrawGrid) :> PathFindingAlgorithm
                | _ -> failwith "Unknown algorithm"

            simulationService.RunAlgorithm(algorithm) (sx, sy) (ex, ey) 10
        | _ ->
            this.SafeShowToast("Válassz kezdő és végpontot a kezdéshez")

    // A Reset gomb megnyomásakor végrehajtódó műveletek
    member private this.OnResetClicked(_: obj) =
        grid <- Array2D.create gridSize gridSize 0
        startPoint <- None
        endPoint <- None
        this.SafeDrawGrid()
        this.UpdateInfoPanel(PathFindingResult.Empty)

    // Az információs panel frissítése a szimulációs eredményekkel
    member this.UpdateInfoPanel(result: PathFindingResult) =
        let formattedTime = result.ExecutionTime.ToString(@"m\:ss\.ff")
        Dispatcher.UIThread.InvokeAsync(fun () ->
            if result.ErrorMessage.IsSome then
                this.ShowToast(result.ErrorMessage.Value)
            this.FindControl<TextBlock>("AlgorithmNameText").Text <- result.AlgorithmName
            this.FindControl<TextBlock>("VisitedCellsText").Text <- $"Bejárt cellák: {result.VisitedCells}"
            this.FindControl<TextBlock>("PathLengthText").Text <- $"Útvonal hossza: {result.Path.Length}"
            this.FindControl<TextBlock>("ExecutionTimeText").Text <- $"Idő: {formattedTime}"
            this.FindControl<TextBlock>("TotalOperations").Text <- $"Műveletek száma: {result.TotalOperations}"
        ) |> ignore

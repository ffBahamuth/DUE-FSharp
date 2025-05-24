module PathFinder.Algorithms.AlgorithmBase

open PathFinder.Models.PathFindingResult
open PathFinder.Models.Grid.Grid

[<AbstractClass>]
type PathFindingAlgorithm(grid: int[,], updateUI: unit -> unit) =
    // Alap irányok (jobb, le, bal, fel), ezek a lehetséges mozgási irányok a cellák között
    member val Directions = [ (0, 1); (1, 0); (0, -1); (-1, 0) ]
    
    // Cellák frissítése UI szálon biztonságosan
    // A SetCellValue metódus a cellát módosítja a grid-ben, és frissíti az UI-t, ha szükséges
    member private this.SetCellValue (x: int) (y: int) (value: int) =
        // Csak akkor frissítjük a cellát, ha az nem Start vagy End típusú
        if grid[x, y] <> (int CellType.Start) && grid[x, y] <> (int CellType.End) then
            grid[x, y] <- value
            updateUI()

    // Public method, ami egy cella típusát frissíti a megadott koordinátákon
    member this.UpdateCell (x: int) (y: int) (cellType: CellType) =
        this.SetCellValue x y (int cellType)  // Az új cella értékét integer formátumban mentjük el
        
    // Public method, ami egy cella értékét frissíti, de nem típusként, hanem számértékként
    member this.UpdateCellValue (x: int) (y: int) (cellTypeValue:int) =
        this.SetCellValue x y cellTypeValue  // A cella értékét közvetlenül a megadott számra állítja be
        
    // Késleltetés async módon, várakozási időt adunk az algoritmushoz
    member this.DelayAsync (ms:int) = async {
        do! Async.Sleep(ms)  // Az Async.Sleep segítségével késleltetjük az algoritmus futását
    }
    
    // Absztrakt metódus, amelyet minden leszármazott algoritmusnak implementálnia kell
    abstract member FindPath : startX:int * startY:int * endX:int * endY:int * delayMs:int -> Async<PathFindingResult>
    // A FindPath metódus az algoritmus fő keresési metódusa, amely a kezdő- és végpont koordinátákat, valamint a késleltetési időt használja
    // Visszatérési értéke egy Async típusú PathFindingResult, amely tartalmazza az útvonalat és az algoritmus eredményeit
    
    // Az algoritmus neve
    abstract AlgorithmName: string with get

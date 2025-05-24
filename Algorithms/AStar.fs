module PathFinder.Algorithms.AStar

open System.Collections.Generic
open PathFinder.Algorithms.AlgorithmBase
open PathFinder.Models.PathFindingResult
open PathFinder.Models.Grid.Grid

type AStarAlgorithm(grid: int[,], updateUI: unit -> unit) =
    inherit PathFindingAlgorithm(grid, updateUI)
    
    // Manhattan distance heuristic, a cél a célpont (endX, endY) elérhetőségének megbecsülése
    // Minél közelebb van egy cella a célponthoz, annál kisebb a fScore értéke
    member private this.Heuristic (x1, y1) (x2, y2) =
        abs(x1 - x2) + abs(y1 - y2)  // Manhattan távolság: |x1 - x2| + |y1 - y2|
    
    override this.FindPath(startX, startY, endX, endY, delayMs) =
        async {
            let stopwatch = System.Diagnostics.Stopwatch.StartNew()  // Időmérés indítása
            let mutable visitedCells = 0  // Látogatott cellák száma
            let mutable totalOps = 0  // Végrehajtott műveletek száma
            let mutable result = None  // Az eredmény tárolása, ha megtaláltuk az utat

            // Az openSet tárolja a még meg nem vizsgált cellákat fScore alapján rendezve
            let openSet = SortedSet<int * int * int>() // (fScore, x, y)
            let cameFrom = Dictionary<int*int, int*int>()  // Nyomon követi, honnan érkeztünk a cellába
            let gScore = Dictionary<int*int, int>()  // A kezdőponttól való távolság (gScore)
            let fScore = Dictionary<int*int, int>()  // Az fScore a teljes költséget tartalmazza (gScore + Heuristic)

            // Inicializáljuk a fScore és gScore értékeket
            for x in 0..grid.GetLength(0)-1 do
                for y in 0..grid.GetLength(1)-1 do
                    gScore.Add((x,y), System.Int32.MaxValue)  // Minden cellát maximális értékre állítunk
                    fScore.Add((x,y), System.Int32.MaxValue)  // Minden cellát maximális értékre állítunk

            // A kezdőpont gScore-ja 0, és a fScore-ja a heuristic alapján van kiszámítva
            gScore[(startX, startY)] <- 0
            fScore[(startX, startY)] <- this.Heuristic (startX, startY) (endX, endY)
            openSet.Add(fScore[(startX, startY)], startX, startY) |> ignore  // Kezdőpont hozzáadása az openSet-hez

            // A kezdőpont kijelölése feldolgozás alattinak
            this.UpdateCell startX startY CellType.Processing
            visitedCells <- visitedCells + 1  // Növeljük a látogatott cellák számát

            // Fő ciklus: addig folytatjuk, amíg van még vizsgálandó cella
            while openSet.Count > 0 && result.IsNone do
                totalOps <- totalOps + 1  // Növeljük a végrehajtott műveletek számát
                let _, x, y = openSet.Min  // Az openSet-ben a legkisebb fScore-val rendelkező cella

                if (x, y) = (endX, endY) then
                    // Ha elértük a célpontot, rekonstruáljuk az utat
                    let mutable path = []
                    let mutable current = (x, y)
                    while cameFrom.ContainsKey(current) do
                        path <- current::path  // Hozzáadjuk a jelenlegi cellát az útvonalhoz
                        current <- cameFrom[current]  // Lépünk vissza az előző cellához
                    path <- (startX, startY)::path  // A kezdőpontot is hozzáadjuk az útvonalhoz
                    
                    stopwatch.Stop()  // Az időmérés leállítása
                    result <- Some {  // Ha megtaláltuk az utat, elmentjük az eredményt
                        PathFindingResult.Empty with
                            Path = path
                            IsSuccess = true
                            VisitedCells = visitedCells
                            TotalOperations = totalOps
                            ExecutionTime = stopwatch.Elapsed
                            AlgorithmName = this.AlgorithmName // Az algoritmus neve
                    }
                else
                    // Ha nem értük el a célpontot, akkor eltávolítjuk az openSet-ből a legjobb cellát
                    openSet.Remove(openSet.Min) |> ignore
                    this.UpdateCell x y CellType.Processed  // A cellát már feldolgozottként jelöljük
                    
                    // Vizsgáljuk meg a szomszédos cellákat
                    for dx, dy in this.Directions do
                        totalOps <- totalOps + 1  // Művelet számláló növelése
                        let newX, newY = x + dx, y + dy  // Az új szomszédos koordináták

                        // Ellenőrizzük, hogy az új cella érvényes és még nem zártuk le
                        if isValidCoordinate grid newX newY &&
                           (grid[newX, newY] = int CellType.Empty ||
                            grid[newX, newY] = int CellType.End) then
                            
                            let tentativeGScore = gScore[(x, y)] + 1  // A gScore becslés a szomszédos cellára
                            
                            // Ha az új gScore jobb, mint a korábbi, frissítjük az adatokat
                            if tentativeGScore < gScore[(newX, newY)] then
                                cameFrom[(newX, newY)] <- (x, y)  // Az új cellához rendeljük a szülőcellát
                                gScore[(newX, newY)] <- tentativeGScore  // Frissítjük a gScore-t
                                fScore[(newX, newY)] <- tentativeGScore + this.Heuristic (newX, newY) (endX, endY)  // Frissítjük az fScore-t

                                // Ha a cellát még nem adtuk hozzá az openSet-hez, akkor hozzáadjuk
                                if not (openSet.Contains(fScore[(newX, newY)], newX, newY)) then
                                    openSet.Add(fScore[(newX, newY)], newX, newY) |> ignore
                                    this.UpdateCell newX newY CellType.Processing  // A cellát feldolgozás alattinak jelöljük
                                    visitedCells <- visitedCells + 1  // Látogatott cellák számának növelése
                    
                    // Aszinkron késleltetés
                    do! this.DelayAsync delayMs
            
            stopwatch.Stop()  // Az időmérés leállítása
            return match result with
                   | Some r -> r  // Ha találtunk utat, visszaadjuk az eredményt
                   | None -> 
                       {  // Ha nem találtunk utat, hibaüzenetet adunk vissza
                           PathFindingResult.Empty with
                               IsSuccess = false
                               VisitedCells = visitedCells
                               TotalOperations = totalOps
                               ExecutionTime = stopwatch.Elapsed
                               ErrorMessage = Some "Nem található útvonal"
                               AlgorithmName = this.AlgorithmName // Az algoritmus neve
                       }
        }
        
    override this.AlgorithmName = "AStar"

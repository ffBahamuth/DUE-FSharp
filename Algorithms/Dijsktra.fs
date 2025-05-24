module PathFinder.Algorithms.Dijkstra

open System.Collections.Generic
open PathFinder.Algorithms.AlgorithmBase
open PathFinder.Models.PathFindingResult
open PathFinder.Models.Grid.Grid
type DijkstraAlgorithm(grid: int[,], updateUI: unit -> unit) =
    inherit PathFindingAlgorithm(grid, updateUI)
    
    override this.FindPath(startX, startY, endX, endY, delayMs) =
        async {
            let stopwatch = System.Diagnostics.Stopwatch.StartNew()  // Az időmérés elindítása
            let mutable visitedCells = 0  // Látogatott cellák száma
            let mutable totalOps = 0  // Végrehajtott műveletek száma
            let mutable result = None  // Eredmény változó, ha sikerült megtalálni az utat

            // Távolságok tárolása, minden cellához beállítjuk a kezdeti távolságot (infinities)
            let distances = Dictionary<int*int, int>()
            let previous = Dictionary<int*int, int*int>()  // A szülő cellák tárolása az úton való visszafejtéshez
            let priorityQueue = SortedSet<int * int * int>() // (distance, x, y), prioritás sor, ami a legkisebb távolságú cellát tartalmazza

            // Inicializáljuk a távolságokat a cellákhoz
            for x in 0..grid.GetLength(0)-1 do
                for y in 0..grid.GetLength(1)-1 do
                    distances.Add((x,y), System.Int32.MaxValue)  // Kezdetben minden cella távolsága végtelen
            
            // A kezdőpont távolsága 0
            distances[(startX, startY)] <- 0
            priorityQueue.Add(0, startX, startY) |> ignore  // A kezdőpontot hozzáadjuk a prioritás sorhoz

            // Kezdőpont feldolgozása, hogy a cella "feldolgozás alatt" legyen
            this.UpdateCell startX startY CellType.Processing
            visitedCells <- visitedCells + 1  // Növeljük a látogatott cellák számát

            // Fő ciklus: amíg van vizsgálandó cella a prioritás sorban, folytatjuk a keresést
            while priorityQueue.Count > 0 && result.IsNone do
                totalOps <- totalOps + 1  // Végrehajtott műveletek számának növelése
                let dist, x, y = priorityQueue.Min  // A legkisebb távolságú cella kiválasztása
                priorityQueue.Remove(priorityQueue.Min) |> ignore  // Eltávolítjuk a prioritás sorból

                if (x, y) = (endX, endY) then
                    // Ha elértük a célpontot, rekonstruáljuk az utat
                    let mutable path = []
                    let mutable current = (x, y)
                    while previous.ContainsKey(current) do
                        path <- current::path  // Az aktuális cellát hozzáadjuk az útvonalhoz
                        current <- previous[current]  // Visszalépünk a szülő cellához
                    path <- (startX, startY)::path  // A kezdőpontot is hozzáadjuk az útvonalhoz

                    stopwatch.Stop()  // Időmérés leállítása
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
                    // Ha nem értük el a célpontot, akkor a cellát feldolgozottként jelöljük
                    this.UpdateCell x y CellType.Processed

                    // Vizsgáljuk meg az összes szomszédos cellát
                    for dx, dy in this.Directions do
                        totalOps <- totalOps + 1  // Művelet számláló növelése
                        let newX, newY = x + dx, y + dy  // Új szomszédos cella koordinátái

                        // Ellenőrizzük, hogy az új cella érvényes-e és nincs-e már lezárva
                        if isValidCoordinate grid newX newY &&
                           (grid[newX, newY] = int CellType.Empty ||
                            grid[newX, newY] = int CellType.End) then
                            
                            let alt = dist + 1  // Mivel minden cella költsége 1, így az alternatív távolságot 1-gyel növeljük
                            if alt < distances[(newX, newY)] then
                                distances[(newX, newY)] <- alt  // Frissítjük az új távolságot
                                previous[(newX, newY)] <- (x, y)  // Az új szülőcellát tároljuk
                                priorityQueue.Add(alt, newX, newY) |> ignore  // Az új cellát hozzáadjuk a prioritás sorhoz
                                this.UpdateCell newX newY CellType.Processing  // A cellát feldolgozás alattinak jelöljük
                                visitedCells <- visitedCells + 1  // Növeljük a látogatott cellák számát

                    // Aszinkron késleltetés a UI frissítéséhez
                    do! this.DelayAsync delayMs

            stopwatch.Stop()  // Időmérés leállítása
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
        
    override this.AlgorithmName = "Dijsktra"

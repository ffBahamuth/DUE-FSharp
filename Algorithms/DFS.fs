module PathFinder.Algorithms.DFS


open System.Collections.Generic
open PathFinder.Algorithms.AlgorithmBase
open PathFinder.Models.PathFindingResult
open PathFinder.Models.Grid.Grid
type DFSAlgorithm(grid: int[,], updateUI: unit -> unit) =
    inherit PathFindingAlgorithm(grid, updateUI)
    
    override this.FindPath(startX, startY, endX, endY, delayMs) =
        async {
            let stopwatch = System.Diagnostics.Stopwatch.StartNew()
            let mutable visitedCells = 0
            let mutable totalOps = 0
            let mutable maxStackDepth = 0
            let visited = Dictionary<int*int, bool>()
            let stack = Stack<int * int * (int * int) list>()

            stack.Push((startX, startY, [(startX, startY)]))  // Kezdőpont hozzáadása a stackhez
            visited.Add((startX, startY), true)  // Kezdőpont megjelölése látogatottnak
            this.UpdateCell startX startY CellType.Processing
            visitedCells <- visitedCells + 1

            // Rekurzív algoritmus implementálása
            let rec loop () =
                async {
                    totalOps <- totalOps + 1
                    maxStackDepth <- max maxStackDepth stack.Count

                    if stack.Count = 0 then
                        stopwatch.Stop()
                        return {
                            PathFindingResult.Empty with
                                IsSuccess = false  // Ha üres a stack, nincs útvonal
                                VisitedCells = visitedCells
                                TotalOperations = totalOps
                                ExecutionTime = stopwatch.Elapsed
                                ErrorMessage = Some "Nem található útvonal"
                                AlgorithmName = this.AlgorithmName // Az algoritmus neve
                        }
                    else
                        let x, y, path = stack.Pop()  // Kiveszünk egy cellát a stackből

                        if (x, y) = (endX, endY) then
                            stopwatch.Stop()
                            return {
                                PathFindingResult.Empty with
                                    Path = List.rev path  // Az útvonal visszafordítása
                                    IsSuccess = true
                                    VisitedCells = visitedCells
                                    TotalOperations = totalOps
                                    ExecutionTime = stopwatch.Elapsed
                                    AlgorithmName = this.AlgorithmName // Az algoritmus neve
                            }
                        else
                            this.UpdateCell x y CellType.Processed
                            
                            // A szomszédos cellák bejárása
                            for dx, dy in this.Directions do
                                totalOps <- totalOps + 1
                                let newX, newY = x + dx, y + dy
                                let coord = (newX, newY)

                                // Ellenőrzés, hogy az új cella érvényes és nem lett-e már látogatva
                                if isValidCoordinate grid newX newY &&
                                   (grid[newX, newY] = int CellType.Empty ||
                                    grid[newX, newY] = int CellType.End) &&
                                   not (visited.ContainsKey(coord)) then
                                    
                                    visited.Add(coord, true)  // Az új cellát látogatottnak jelöljük
                                    stack.Push((newX, newY, (newX, newY) :: path))  // A cellát és az eddigi útvonalat hozzáadjuk a stackhez
                                    this.UpdateCell newX newY CellType.Processing  // Az új cellát feldolgozás alattinak jelöljük
                                    visitedCells <- visitedCells + 1  // Látogatott cellák száma növekszik

                            // Aszinkron késleltetés
                            do! this.DelayAsync delayMs
                            return! loop ()  // Rekurzív hívás a következő lépéshez
                }

            return! loop ()  // Az algoritmus elindítása
        }
        
        override this.AlgorithmName = "DFS"

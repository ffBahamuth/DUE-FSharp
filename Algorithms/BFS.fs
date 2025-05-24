module PathFinder.Algorithms.BFS

open System.Collections.Generic
open PathFinder.Algorithms.AlgorithmBase
open PathFinder.Models.PathFindingResult
open PathFinder.Models.Grid.Grid

type BFSAlgorithm(grid: int[,], updateUI: unit -> unit) =
    inherit PathFindingAlgorithm(grid, updateUI)  // Az osztály öröklődik a PathFindingAlgorithm-ból, tehát a BFS algoritmus az alapértelmezett logikát használja

    // A FindPath metódus, amely végrehajtja a BFS algoritmust
    override this.FindPath(startX, startY, endX, endY, delayMs) =
        async {
            let stopwatch = System.Diagnostics.Stopwatch.StartNew()  // Időmérés kezdése
            let mutable visitedCells = 0  // A látogatott cellák száma
            let mutable totalOps = 0     // Az összes végrehajtott művelet száma
            let mutable maxQueueSize = 0 // Az aktuális legnagyobb várakozó sor mérete (új metrika)

            // Használjunk egy Dictionary-t a már látogatott cellák nyomon követésére
            let visited = Dictionary<int*int, bool>()
            
            // A BFS-hez szükséges sor (queue) inicializálása, a sorban minden elem egy koordinátapár (x, y) és az eddig megtett út
            let queue = Queue<int * int * (int * int) list>()
            queue.Enqueue((startX, startY, [(startX, startY)]))  // A kezdőpontot hozzáadjuk a sorhoz
            visited.Add((startX, startY), true)  // A kezdőpontot már látogatottnak jelöljük

            // Kezdőpont cellájának beállítása, hogy "feldolgozás alatt" legyen
            this.UpdateCell startX startY CellType.Processing
            visitedCells <- visitedCells + 1

            // Rekurzív függvény, amely a BFS algoritmus logikáját valósítja meg
            let rec loop () =
                async {
                    totalOps <- totalOps + 1  // Műveletek száma növelése
                    maxQueueSize <- max maxQueueSize queue.Count  // A legnagyobb sorméret frissítése

                    // Ha a sor üres, akkor nincs több cella, amit feldolgozhatnánk (nincs útvonal)
                    if queue.Count = 0 then 
                        stopwatch.Stop()  // Időmérés leállítása
                        return { 
                            PathFindingResult.Empty with 
                                IsSuccess = false  // Ha nincs útvonal, akkor sikeresség False
                                VisitedCells = visitedCells  // A látogatott cellák száma
                                TotalOperations = totalOps  // Az összes művelet száma
                                ExecutionTime = stopwatch.Elapsed  // Futási idő
                                ErrorMessage = Some "Nem található út"
                                AlgorithmName = this.AlgorithmName // Az algoritmus neve
                        }
                    else
                        // A sorból kiveszünk egy elemet: koordinátapár (x, y) és az eddigi útvonal
                        let x, y, path = queue.Dequeue()

                        // Ha elértük a célpontot, akkor visszatérünk az eredménnyel
                        if (x, y) = (endX, endY) then
                            stopwatch.Stop()  // Időmérés leállítása
                            return { 
                                PathFindingResult.Empty with 
                                    Path = List.rev path  // Az útvonalat megfordítjuk, mert a BFS fordított sorrendben halad
                                    IsSuccess = true  // Sikeres keresés
                                    VisitedCells = visitedCells  // Látogatott cellák száma
                                    TotalOperations = totalOps  // Összes művelet száma
                                    ExecutionTime = stopwatch.Elapsed  // Futási idő
                                    AlgorithmName = this.AlgorithmName // Az algoritmus neve
                            }
                        else
                            // Ha nem értük el a célpontot, akkor jelöljük "feldolgozottnak" a jelenlegi cellát
                            this.UpdateCell x y CellType.Processed
                            
                            // A négy lehetséges irányban (jobb, le, bal, fel) vizsgáljuk a szomszédos cellákat
                            for dx, dy in this.Directions do
                                totalOps <- totalOps + 1  // Műveletek száma növelése
                                let newX, newY = x + dx, y + dy  // Az új koordináták kiszámítása
                                let coord = (newX, newY)

                                // Ellenőrizzük, hogy az új koordináták érvényesek, üresek-e, vagy célpontot tartalmaznak, és nem látogattuk még meg őket
                                if isValidCoordinate grid newX newY && 
                                   (grid[newX, newY] = int CellType.Empty || 
                                    grid[newX, newY] = int CellType.End) &&
                                   not (visited.ContainsKey(coord)) then
                                    
                                    // A cellát látogatottnak jelöljük és hozzáadjuk a sorhoz
                                    visited.Add(coord, true)
                                    queue.Enqueue((newX, newY, (newX, newY) :: path))  // Az új koordinátát hozzáadjuk az útvonalhoz
                                    this.UpdateCell newX newY CellType.Processing  // Az új cellát "feldolgozás alatt"-nak jelöljük
                                    visitedCells <- visitedCells + 1  // A látogatott cellák számát növeljük
                            
                            // Aszinkron késleltetés az algoritmus sebességének szabályozásához
                            do! this.DelayAsync delayMs
                            return! loop ()  // Rekurzív hívás a következő lépéshez
                }
            return! loop ()  // Az algoritmus elindítása
        }

    override this.AlgorithmName = "BFS"

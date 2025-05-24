module PathFinder.Services.SimulationService

open PathFinder.Algorithms.AlgorithmBase
open PathFinder.Models.PathFindingResult
open PathFinder.Models.Grid.Grid

// A szimuláció lehetséges állapotainak meghatározása
type SimulationState =
    | Idle             // A szimuláció nincs futásban
    | Running          // A szimuláció jelenleg fut
    | Completed        // A szimuláció sikeresen befejeződött
    | Failed of string // A szimuláció hibával leállt, és a hibaüzenet tartalmazza a hibát

// A SimulationService felelős a szimuláció állapotának kezeléséért és az algoritmus futtatásáért
type SimulationService(
    updateInfo:PathFindingResult -> unit,  // Frissíti az információkat a szimuláció állapotáról
    onChangeState: SimulationState -> unit,   // Frissíti a szimuláció állapotát
    onReset: unit -> unit                     // Újratölti a grid értékeit törölve a régi keresés értékeit
    ) =
    // Kezdetben az állapot 'Idle', tehát nincs futó szimuláció
    let mutable state = Idle

    // A RunAlgorithm függvény felelős az algoritmus futtatásáért
    // Az algoritmus végrehajtása során frissíti a szimuláció állapotát és az információkat
    member this.RunAlgorithm (algorithm: PathFindingAlgorithm) (start: int*int) (endPos: int*int) (delayMs: int) : unit =
        state <- Idle  // Az állapotot inicializáljuk 'Idle'-ra
        async {
            try
                // Állapot módosítása: a szimuláció elkezdődött
                this.ChangeState(Running)
                // A szimulációt alaphelyzetbe állítjuk
                onReset()

                // Az algoritmus futtatása aszinkron módon
                let! result = algorithm.FindPath (fst start, snd start, fst endPos, snd endPos, delayMs)

                // Ha a keresés sikeres, vizualizáljuk az utat
                if result.IsSuccess then
                    // A térképen a megtalált útvonalat vizualizáljuk
                    // Az első és utolsó elemet kihagyjuk, mert azok a kezdő és végpontok
                    for x, y in result.Path |> Seq.skip 1 |> Seq.take (List.length result.Path - 2) do
                        algorithm.UpdateCellValue x y (int CellType.Path)
                        do! algorithm.DelayAsync delayMs 
                // Az információk frissítése
                updateInfo(result)

                // Állapot módosítása: a szimuláció befejeződött
                this.ChangeState(Completed)
            with ex ->
                // Ha hiba történt, akkor beállítjuk a hibás állapotot
                this.ChangeState(Failed ex.Message)
        } |> Async.Start  // A műveletet aszinkron módon elindítjuk

    // A ChangeState privát függvény felelős a szimuláció állapotának változtatásáért
    // Ez a függvény frissíti a szimuláció állapotát és értesíti az eseménykezelőt
    member private this.ChangeState(newState: SimulationState) =
        state <- newState
        onChangeState(newState)

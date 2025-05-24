module PathFinder.Models.PathFindingResult

open System

type PathFindingResult = 
 {
        Path: (int * int) list  // Az útvonal, amelyet az algoritmus talált (koordinátapárok listája)
        VisitedCells: int       // Az algoritmus által bejárt cellák száma
        TotalOperations: int    // Az algoritmus által végrehajtott összes művelet száma
        ExecutionTime: TimeSpan // Az algoritmus futási ideje
        IsSuccess: bool         // Igaz, ha az algoritmus sikeresen talált útvonalat
        AlgorithmName: string   // A futtatott algoritmus neve
        ErrorMessage: string option // Opcionális hibaüzenet, ha a keresés nem sikerült
    }
    static member Empty =
        {
            Path = []
            VisitedCells = 0
            TotalOperations = 0
            ExecutionTime = TimeSpan.Zero
            IsSuccess = false
            AlgorithmName = ""
            ErrorMessage = None
        }
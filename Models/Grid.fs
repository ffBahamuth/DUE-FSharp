module PathFinder.Models.Grid

module Grid =
    type CellType =
        | Empty = 0        // Üres cella, semmilyen objektum nincs benne (pl. szabad tér)
        | Wall = 1         // Fal cella, amely akadályt jelent a keresés során
        | Start = 2        // Kezdőpont cellája, amely a keresés induló pozícióját jelöli
        | End = 3          // Célpont cellája, amely a keresés végpontját jelöli
        | Processed = 4    // Feldolgozott cella, amelyet már az algoritmus vizsgált
        | Processing = 5   // Jelenleg feldolgozott cella, amelyet az algoritmus éppen vizsgál
        | Path = 6         // Útvonal cellája, amelyet az algoritmus végül választott az útvonal részéül

    type Grid = int[,]
    
    // Létrehoz egy új rácsot
    let create (size:int): int array2d = 
       Array2D.create size size (int CellType.Empty)
    
    // Visszaállítja a rácsot (eltávolítja az utakat és feldolgozott cellákat)
    let reset (grid: Grid) =
        Array2D.iteri (fun x y _ -> 
            if grid[x,y] > int CellType.End then 
                grid[x,y] <- int CellType.Empty) grid
    
    // Ellenőrzi, hogy a koordináták érvényesek-e
    let isValidCoordinate (grid: Grid) x y =
        x >= 0 && y >= 0 && x < grid.GetLength(0) && y < grid.GetLength(1)
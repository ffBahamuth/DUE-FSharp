module PathFinder.Models.Types

type CanvasSelectorMode =
    | None = 0                // Alapértelmezett mód, nincs választott művelet
    | AddWall = 1             // A falak hozzáadásának módja, amikor a felhasználó falat rajzol a vászonra
    | ClearWall = 2           // A falak törlésének módja, amikor a felhasználó eltávolítja a falat a vászonról
    | SelectStartPoint = 3    // A kezdőpont kiválasztásának módja, amikor a felhasználó megadja a kezdőpozíciót
    | SelectEndPoint = 4      // A célpont kiválasztásának módja, amikor a felhasználó megadja a végpontot

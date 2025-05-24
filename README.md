# PathFinder
DUE-FSharp


1. Áttekintés

Ez a program egy útvonalkereső algoritmusokat (mint a BFS, DFS, A* és Dijkstra) implementál, és lehetőséget biztosít egy rácson történő vizualizálásukra. A felhasználó interaktívan módosíthatja a rácsot (például falak hozzáadása, kezdő- és végpontok kijelölése), majd a kiválasztott algoritmus végrehajtásával megkeresheti az optimális utat. Az algoritmusok lépései folyamatosan vizualizálva vannak, így a felhasználó valós időben láthatja a folyamatot.

2. Funkcionalitás

A program főbb funkciói:

    Rács megjelenítése: A program egy négyzetrácsot jelenít meg, ahol a felhasználó falakat, kezdőpontokat, végpontokat és az algoritmus által járt utat láthatja.

    Interaktív módosítás: A felhasználó módosíthatja a rácsot, például falakat adhat hozzá, vagy kijelölheti a kezdő- és végpontokat.

    Algoritmusok futtatása: A program négyféle útvonalkereső algoritmus közül választható: BFS (szélességi keresés), DFS (mélységi keresés), A* és Dijkstra. A kiválasztott algoritmus végrehajtása után a program kiírja az eredményeket, mint például a megtalált útvonal hosszát, a bejárt cellák számát, az algoritmus végrehajtásához szükséges időt és a végrehajtott műveletek számát.

    UI frissítés: Az alkalmazás folyamatosan frissíti a felhasználói felületet a program állapotának megfelelően, például a rácsot, a vizualizált utat, valamint a szimulációval kapcsolatos információkat.

3. Használati útmutató
3.1. Kezdés

    Rács létrehozása: Az alkalmazás indításakor egy 40x40-es rács jelenik meg, amelynek cellái 20x20 pixelesek.

    Algoritmus kiválasztása: A felhasználó választhat az elérhető algoritmusok közül a "AlgorithmSelector" legördülő menüben:

        BFS (Breadth-First Search): Szélességi keresés.

        DFS (Depth-First Search): Mélységi keresés.

        A*: Heurisztikus algoritmus, amely az optimális utat keresi.

        Dijkstra: Az útvonal legkisebb költségét keresi.

    Mód kiválasztása: A "ModeSelector" legördülő menü segítségével választható ki a következő módok egyike:

        AddWall: Fal hozzáadása a rácshoz.

        ClearWall: Fal eltávolítása.

        SelectStart: Kezdőpont kijelölése.

        SelectEnd: Végpont kijelölése.

3.2. Interaktív használat

A felhasználó a rácson való interakcióval módosíthatja a térképet:

    Fal hozzáadása: A "AddWall" módban kattintással falakat lehet elhelyezni a rácson.

    Fal eltávolítása: A "ClearWall" módban eltávolíthatóak a falak.

    Kezdőpont és végpont kijelölése: A "SelectStart" és "SelectEnd" módban kattintással beállítható a kezdő- és végpont a rácson.

A rács módosítása valós időben látható, és az aktuális állapot folyamatosan frissül.
3.3. Algoritmus futtatása

Miután a felhasználó kijelölte a kezdő- és végpontokat, valamint beállította a falakat, elindítható az algoritmus:

    Kattintás a "Start" gombra.

    A program elindítja a kiválasztott algoritmust, és elkezdi keresni az optimális utat.

    A folyamat során a vizualizálás folyamatosan frissül, így a felhasználó látja, ahogy az algoritmus végrehajtja a keresést, a bejárt cellákat és az utat.

3.4. Eredmények megjelenítése

A szimuláció futtatása után az eredmények a következő formában jelennek meg:

    Algoritmus neve: A használt algoritmus neve.

    Bejárt cellák száma: A keresés során a program által felkeresett cellák száma.

    Útvonal hossza: A megtalált út hossza.

    Végrehajtási idő: Az algoritmus futásának ideje.

    Műveletek száma: Az algoritmus végrehajtásához szükséges műveletek száma.

A program emellett egy "Toast" üzenetet is megjeleníthet, ha valamilyen hiba lép fel (például nincs kijelölve kezdő- és végpont), vagy ha más figyelmeztetés szükséges.
3.5. Resetelés

A "Reset" gombbal az alkalmazás visszaállítható alapállapotba. Ekkor a rács törlődik, és a kezdő- és végpontok is eltűnnek.
4. Program működése

A program a következőképpen működik:

    Rács létrehozása: A főablakban egy rács jelenik meg, amelyet a felhasználó interaktívan módosíthat.

    Módválasztás: A felhasználó kiválaszthatja, hogy falat szeretne hozzáadni, eltávolítani, illetve kijelölheti a kezdő- és végpontokat.

    Algoritmus futtatása: Miután a felhasználó a megfelelő módot és algoritmust kiválasztotta, az algoritmus elindul, és a program a háttérben megkezdi a keresést.

    Vizuális frissítés: A rács és a kijelölt pontok folyamatosan frissülnek, és a felhasználó láthatja az algoritmus végrehajtását.

    Eredmények: A futás végén az algoritmus eredményei megjelennek a felhasználói felületen, és a felhasználó további műveleteket hajthat végre.

5. Következtetés

Ez a program egy egyszerű, mégis hatékony módot biztosít az útvonalkereső algoritmusok vizuális bemutatására. A felhasználók könnyen módosíthatják a rácsot, és a kiválasztott algoritmusok futtatásával gyakorolhatják az alapvető algoritmusok működését.
6. Használati javaslatok

    Használja a programot oktatási célokra, hogy megérthesse az alapvető útvonalkeresési algoritmusokat.

    Kísérletezzen a különböző algoritmusokkal, és figyelje meg, hogyan változik az útvonal keresési ideje és hatékonysága.

    Tesztelje a programot különböző rácsokkal és akadályokkal, hogy jobban megismerhesse az algoritmusok viselkedését valós környezetekben.

Ez a program egy remek eszköz az algoritmusok vizualizálásához, és hasznos eszköz lehet mind az oktatásban, mind a szabadidős programozási gyakorlatokban.

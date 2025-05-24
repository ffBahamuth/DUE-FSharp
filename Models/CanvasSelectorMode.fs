module PathFinder.CanvasSelectorMode

/// Enum to track what action the user is performing on the canvas
type CanvasSelectorMode =
    | None = -1
    | AddWall = 0
    | ClearWall = 1
    | SelectStartPoint = 2
    | SelectEndPoint = 3
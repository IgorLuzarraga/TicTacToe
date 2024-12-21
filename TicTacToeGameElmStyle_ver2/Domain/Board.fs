namespace TicTacToeGameElmStyle_ver2.Domain

module Board =
    open Square
    open Player

    type Board = Map<Square, Player option>

    let create size =
        [ for row in 0 .. size - 1 do
              for col in 0 .. size - 1 do
                  { Row = row; Col = col }, None ]
        |> Map.ofList

    let getBoardSize board =
        board |> Map.count |> float |> sqrt |> int

namespace TicTacToeGameElmStyle_ver2.Domain

module Square =

    type Square = { Row: int; Col: int }

    let create row col boardSize board =
        if row >= 0
           && row < boardSize
           && col >= 0
           && col < boardSize then
            let sq = { Row = row; Col = col }

            if Map.containsKey sq board then
                Some sq
            else
                None
        else
            None

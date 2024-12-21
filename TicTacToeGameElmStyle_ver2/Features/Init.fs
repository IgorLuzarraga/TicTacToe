namespace TicTacToeGameElmStyle_ver2.Features

module Init =
    open TicTacToeGameElmStyle_ver2.Domain
    open TicTacToeGameElmStyle_ver2.Domain.Model

    let init size =
        let board = Board.create size

        { Board = board
          CurrentPlayer = Player.X
          GameStatus = InProgress }

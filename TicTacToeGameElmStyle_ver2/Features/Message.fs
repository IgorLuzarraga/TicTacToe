namespace TicTacToeGameElmStyle_ver2.Features

module Message =
    open TicTacToeGameElmStyle_ver2.Domain.Square

    type Message =
        | MakeMove of Square
        | InvalidInput of string
        | ExitGame

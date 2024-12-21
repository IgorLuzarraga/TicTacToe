namespace TicTacToeGameElmStyle_ver2.Shared

module Command =
    open TicTacToeGameElmStyle_ver2.Features.Message

    type Command =
        | ShowError of string
        | Print of string
        | ReadInput of (string -> Message)
        | ExitGame

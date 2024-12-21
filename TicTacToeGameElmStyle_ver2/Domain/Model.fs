namespace TicTacToeGameElmStyle_ver2.Domain

open Player
open Board

module Model =

    type GameStatus =
        | InProgress
        | Draw
        | Winner of Player

    type Model =
        { Board: Board
          CurrentPlayer: Player
          GameStatus: GameStatus }

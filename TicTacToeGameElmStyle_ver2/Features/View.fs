namespace TicTacToeGameElmStyle_ver2.Features

open System

module View =
    open TicTacToeGameElmStyle_ver2.Domain.Model
    open TicTacToeGameElmStyle_ver2.Domain.Square
    open TicTacToeGameElmStyle_ver2.Features.Message
    open TicTacToeGameElmStyle_ver2.Shared.Command
    open TicTacToeGameElmStyle_ver2.Domain.Player
    open TicTacToeGameElmStyle_ver2.Domain

    let view model =
        let formatSquare ({ Row = r; Col = c }, value: Player option) =
            match value with
            | None -> "."
            | Some X -> "X"
            | Some O -> "O"

        let size = model.Board |> Board.getBoardSize

        let currentBoard =
            [ for r in 0 .. size - 1 ->
                  [ for c in 0 .. size - 1 ->
                        formatSquare ({ Row = r; Col = c }, Map.find { Row = r; Col = c } model.Board) ]
                  |> String.concat " " ]
            |> String.concat "\n"

        let statusMessage =
            match model.GameStatus with
            | Winner p -> InfoMessages.playerWins p
            | Draw -> InfoMessages.drawMessage
            | InProgress -> InfoMessages.nextTurn model.CurrentPlayer

        [ Print $"{InfoMessages.currentBoard}\n{currentBoard}"
          Print statusMessage
          if model.GameStatus = InProgress then
              Print InfoMessages.enterMovePrompt

              ReadInput (fun input ->
                  match input.Trim().Split(',')
                        |> Array.map Int32.TryParse
                        |> Array.toList
                      with
                  | [ (true, row); (true, col) ] ->
                      match Square.create row col size model.Board with
                      | Some square -> Message.MakeMove square
                      | None -> InvalidInput InfoMessages.squareOutOfBounds
                  | _ -> InvalidInput InfoMessages.invalidInputFormat) ]

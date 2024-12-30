namespace TicTacToeGameElmStyle_ver2.Features

open System

module View =
    open TicTacToeGameElmStyle_ver2.Domain.Model
    open TicTacToeGameElmStyle_ver2.Domain.Square
    open TicTacToeGameElmStyle_ver2.Features
    open TicTacToeGameElmStyle_ver2.Shared.Command
    open TicTacToeGameElmStyle_ver2.Domain.Player
    open TicTacToeGameElmStyle_ver2.Domain
    open TicTacToeGameElmStyle_ver2.Shared

    let formatSquare ({ Row = r; Col = c }, value: Player option) =
            match value with
            | None -> "."
            | Some X -> "X"
            | Some O -> "O"

    let getCurrentBoard board =
        let size = board |> Board.getBoardSize

        [ for r in 0 .. size - 1 ->
            [ for c in 0 .. size - 1 ->
                    let value = Map.find { Row = r; Col = c } board
                    formatSquare ({ Row = r; Col = c }, value) ]
            |> String.concat " " ]
        |> String.concat "\n"

    let getStatusMessage model=
            match model.GameStatus with
            | Winner p -> InfoMessages.playerWins p
            | Draw -> InfoMessages.drawMessage
            | InProgress -> InfoMessages.nextTurn model.CurrentPlayer

    let handleViewEndGame  =
        [Print InfoMessages.enterStartOverPrompt;
         ReadInput (fun input ->
            match input.Trim().ToLower() with
            | "y" -> Message.StartOver
            | _ -> Message.ExitGame)]

    let handleViewInProgress (model: Model) =
        let size = model.Board |> Board.getBoardSize

        [Print InfoMessages.enterMovePrompt
         ReadInput (fun input ->
            match input.Trim().Split(',')
                    |> Array.map Int32.TryParse
                    |> Array.toList
                with
            | [ (true, row); (true, col) ] ->
                match Square.create row col size model.Board with
                | Some square -> Message.MakeMove square
                | None -> Message.InvalidInput InfoMessages.squareOutOfBounds
            | _ -> Message.InvalidInput InfoMessages.invalidInputFormat)]

    let view model =

        let currentBoard = getCurrentBoard model.Board

        let statusMessage= getStatusMessage model

        let genericCommandList =
            [Print $"{InfoMessages.currentBoard}\n{currentBoard}"
             Print statusMessage]

        let gameStatusCommandList =     
            match model.GameStatus with
                | InProgress -> handleViewInProgress model
                | Draw -> handleViewEndGame
                | Winner _ -> handleViewEndGame

        genericCommandList @ gameStatusCommandList
        
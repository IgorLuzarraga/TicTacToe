// Tic Tac Toe (cmd App) using library Elmish

open Elmish
open System

module TicTacToe =

    // MODEL
    type Player = X | O
    type Square = { Row: int; Col: int }
    type Board = Map<Square, Player option>
    type GameStatus = InProgress | Draw | Winner of Player
    type Model = { Board: Board; CurrentPlayer: Player; GameStatus: GameStatus }

    // SMART CONSTRUCTORS
    module Square =
        let create row col boardSize =
            if row >= 0 && row < boardSize && col >= 0 && col < boardSize then
                Some { Row = row; Col = col }
            else None

    module Board =
        let create size =
            [ for row in 0 .. size - 1 do
                  for col in 0 .. size - 1 do
                      { Row = row; Col = col }, None ]
            |> Map.ofList

    // CONSTANTS
    module Messages =
        let gameOver = "Game is already over!"
        let squareOutOfBounds = "Square is out of bounds."
        let squareOccupied = "Square already occupied!"
        let exitingGame = "Exiting game."
        let invalidInputFormat = "Invalid format. Use row,col."
        let drawMessage = "It's a draw!"
        let playerWins player = sprintf "Player %A wins!" player
        let nextTurn player = sprintf "Next turn: Player %A" player
        let enterMovePrompt = "Enter your move (row,col):"
        let currentBoard = "Current Board:"

    // INITIALIZATION
    let init size =
        let board = Board.create size
        { Board = board; CurrentPlayer = X; GameStatus = InProgress }, Cmd.none

    // MESSAGES
    type Msg = MakeMove of Square | InvalidInput of string | ExitGame

    // HELPER FUNCTIONS
    let checkWinner (board: Board) size =
        let isLineMatching line =
            let values = line |> List.map (fun sq -> Map.tryFind sq board |> Option.flatten)
            match values with
            | [] -> None
            | _ when List.forall ((=) (Some X)) values -> Some X
            | _ when List.forall ((=) (Some O)) values -> Some O
            | _ -> None

        let rows = [ for r in 0 .. size - 1 -> [ for c in 0 .. size - 1 -> { Row = r; Col = c } ] ]
        let cols = [ for c in 0 .. size - 1 -> [ for r in 0 .. size - 1 -> { Row = r; Col = c } ] ]
        let diag1 = [ for i in 0 .. size - 1 -> { Row = i; Col = i } ]
        let diag2 = [ for i in 0 .. size - 1 -> { Row = i; Col = size - i - 1 } ]

        List.concat [ rows; cols; [ diag1; diag2 ] ]
        |> List.choose isLineMatching
        |> List.tryHead

    let boardSize board = board |> Map.count |> float |> sqrt |> int

    let getInput size =
        printfn "%s" Messages.enterMovePrompt
        let input = Console.ReadLine()
        match input.Trim().Split(',') |> Array.map Int32.TryParse |> Array.toList with
        | [ (true, row); (true, col) ] -> Square.create row col size
        | _ -> None

    // UPDATE
    let update msg model =
        match msg, model.GameStatus with
        | MakeMove square, InProgress ->
            match Map.tryFind square model.Board with
            | None -> model, Cmd.ofMsg (InvalidInput Messages.squareOutOfBounds)
            | Some(Some _) -> model, Cmd.ofMsg (InvalidInput Messages.squareOccupied)
            | Some None ->
                let updatedBoard = model.Board |> Map.add square (Some model.CurrentPlayer)
                let updatedGameStatus =
                    match checkWinner updatedBoard (boardSize model.Board) with
                    | Some player -> Winner player
                    | None when updatedBoard |> Map.forall (fun _ v -> v.IsSome) -> Draw
                    | None -> InProgress
                let nextPlayer = if model.CurrentPlayer = X then O else X
                { model with Board = updatedBoard; CurrentPlayer = nextPlayer; GameStatus = updatedGameStatus }, Cmd.none
        | MakeMove _, _ -> model, Cmd.ofMsg (InvalidInput Messages.gameOver)
        | InvalidInput err, _ ->
            printfn "Error: %s" err
            model, Cmd.none
        | ExitGame, _ -> 
            printfn "%s" Messages.exitingGame
            Environment.Exit(0)
            model, Cmd.none

    // VIEW
    let view model dispatch =
        let formatSquare ({ Row = r; Col = c }, value) =
            match value with
            | None -> "."
            | Some X -> "X"
            | Some O -> "O"

        let size = model.Board |> boardSize
        let currentBoard =
            [ for r in 0 .. size - 1 ->
                  [ for c in 0 .. size - 1 ->
                        formatSquare ({ Row = r; Col = c }, Map.find { Row = r; Col = c } model.Board) ]
                  |> String.concat " " ]
            |> String.concat "\n"

        let statusMessage =
            match model.GameStatus with
            | Winner p -> Messages.playerWins p
            | Draw -> Messages.drawMessage
            | InProgress -> Messages.nextTurn model.CurrentPlayer

        printfn "%s\n%s" Messages.currentBoard currentBoard
        printfn "%s" statusMessage
        if model.GameStatus = InProgress then
            match getInput size with
            | Some square -> dispatch (MakeMove square)
            | None -> dispatch (InvalidInput Messages.invalidInputFormat)

    // PROGRAM LOOP
    Program.mkProgram (fun () -> init 3) update view
    |> Program.run

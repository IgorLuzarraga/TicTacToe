open System

module TicTacToe =

    // MODEL
    type Player =
        | X
        | O

    type Square = { Row: int; Col: int }
    type Board = Map<Square, Player option>

    // SMART CONSTRUCTORS
    module Square =
        let create row col boardSize board =
            if row >= 0 && row < boardSize && col >= 0 && col < boardSize then
                let sq = { Row = row; Col = col }
                if Map.containsKey sq board then Some sq else None
            else
                None

    module Board =
        let create size =
            [ for row in 0 .. size - 1 do
                  for col in 0 .. size - 1 do
                      { Row = row; Col = col }, None ]
            |> Map.ofList

    type GameStatus =
        | InProgress
        | Draw
        | Winner of Player

    type Model =
        { Board: Board
          CurrentPlayer: Player
          GameStatus: GameStatus }

    // CONSTANTS
    module Messages =
        let gameOver = "Game is already over!"
        let squareOutOfBounds = "Square is out of bounds."
        let squareOccupied = "Square already occupied!"
        let exitingGame = "Exiting game."
        let invalidInputFormat = "Invalid format. Use row,col."
        let boardSizeError = "Board size must be positive."
        let drawMessage = "It's a draw!"
        let playerWins player = sprintf "Player %A wins!" player
        let nextTurn player = sprintf "Next turn: Player %A" player
        let enterMovePrompt = "Enter your move (row,col):"
        let currentBoard = "Current Board:"

    // INITIALIZATION
    let init size =
        let board = Board.create size

        { Board = board
          CurrentPlayer = X
          GameStatus = InProgress }

    // MESSAGES
    type Message =
        | MakeMove of Square
        | InvalidInput of string
        | ExitGame

    // COMMANDS
    type Command =
        | ShowError of string
        | Print of string
        | ReadInput of (string -> Message)
        | ExitGame

    // HELPER FUNCTIONS
    let checkWinner (board: Board) size =
        let isLineMatching line =
            let values = line |> List.map (fun sq -> Map.tryFind sq board |> Option.flatten)

            match values with
            | [] -> None
            | _ when List.forall (fun v -> v = Some X) values -> Some X
            | _ when List.forall (fun v -> v = Some O) values -> Some O
            | _ -> None

        let rows =
            [ for r in 0 .. size - 1 -> [ for c in 0 .. size - 1 -> { Row = r; Col = c } ] ]

        let cols =
            [ for c in 0 .. size - 1 -> [ for r in 0 .. size - 1 -> { Row = r; Col = c } ] ]

        let diag1 = [ for i in 0 .. size - 1 -> { Row = i; Col = i } ]
        let diag2 = [ for i in 0 .. size - 1 -> { Row = i; Col = size - i - 1 } ]

        List.concat [ rows; cols; [ diag1; diag2 ] ]
        |> List.choose isLineMatching
        |> List.tryHead

    let boardSize board =
        board |> Map.count |> float |> sqrt |> int

    // UPDATE
    let update (msg: Message) (model: Model) =
        match (msg, model.GameStatus) with
        | MakeMove square, InProgress ->
            match Map.tryFind square model.Board with
            | None -> model, [ ShowError Messages.squareOutOfBounds ]
            | Some(Some _) -> model, [ ShowError Messages.squareOccupied ]
            | Some None ->
                let updatedBoard = model.Board |> Map.add square (Some model.CurrentPlayer)

                let updatedGameStatus =
                    match checkWinner updatedBoard (boardSize model.Board) with
                    | Some player -> Winner player
                    | None when updatedBoard |> Map.forall (fun _ v -> v.IsSome) -> Draw
                    | None -> InProgress

                let nextPlayer = if model.CurrentPlayer = X then O else X

                { model with
                    Board = updatedBoard
                    CurrentPlayer = nextPlayer
                    GameStatus = updatedGameStatus },
                []
        | MakeMove _, _ -> model, [ ShowError Messages.gameOver ]
        | InvalidInput err, _ -> model, [ ShowError $"Invalid input: {err}" ]
        | Message.ExitGame, _ -> model, [ Print Messages.exitingGame; ExitGame ]

    // VIEW
    let view model =
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

        [ Print $"{Messages.currentBoard}\n{currentBoard}"
          Print statusMessage
          if model.GameStatus = InProgress then
              Print Messages.enterMovePrompt

              ReadInput(fun input ->
                  match input.Trim().Split(',') |> Array.map Int32.TryParse |> Array.toList with
                  | [ (true, row); (true, col) ] ->
                      match Square.create row col size model.Board with
                      | Some square -> MakeMove square
                      | None -> InvalidInput Messages.squareOutOfBounds
                  | _ -> InvalidInput Messages.invalidInputFormat) ]

    // PROGRAM LOOP
    let rec programLoop model =
        // Generate and handle commands from the view
        let commandsFromView = view model

        // Process all commands generated by the view
        commandsFromView
        |> List.iter (function
            | ShowError err -> printfn "%s" err
            | Print msg -> printfn "%s" msg
            | ReadInput getMsgFromUserInput ->
                let input = Console.ReadLine()
                let msg = getMsgFromUserInput input
                let newModel, commandsFromUpdate = update msg model

                // Process commands from update
                commandsFromUpdate
                |> List.iter (function
                    | ShowError err -> printfn "%s" err
                    | Print msg -> printfn "%s" msg
                    | ExitGame ->
                        printfn "Game exited."
                        exit 0
                    | _ -> ()) // No action needed for other command types

                // Continue the game by calling programLoop with the updated model
                programLoop newModel

            | ExitGame ->
                printfn "Game exited."
                exit 0)

    // RUN THE GAME
    let runGame boardSize =
        if boardSize <= 0 then
            printfn "%s" Messages.boardSizeError
        else
            let initialModel = init boardSize
            programLoop initialModel

    runGame 3

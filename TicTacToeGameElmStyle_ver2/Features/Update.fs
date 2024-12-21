namespace TicTacToeGameElmStyle_ver2.Features

module Update =

    open TicTacToeGameElmStyle_ver2.Shared.Command
    open TicTacToeGameElmStyle_ver2.Features.Message
    open TicTacToeGameElmStyle_ver2.Domain.Board
    open TicTacToeGameElmStyle_ver2.Domain.Model
    open TicTacToeGameElmStyle_ver2.Domain.Square
    open TicTacToeGameElmStyle_ver2.Domain

    let checkWinner (board: Board) size =
        let isLineMatching line =
            let values =
                line
                |> List.map (fun sq -> Map.tryFind sq board |> Option.flatten)

            match values with
            | [] -> None
            | _ when List.forall (fun v -> v = Some Player.X) values -> Some Player.X
            | _ when List.forall (fun v -> v = Some Player.O) values -> Some Player.O
            | _ -> None

        let rows =
            [ for r in 0 .. size - 1 -> [ for c in 0 .. size - 1 -> { Row = r; Col = c } ] ]

        let cols =
            [ for c in 0 .. size - 1 -> [ for r in 0 .. size - 1 -> { Row = r; Col = c } ] ]

        let diag1 = [ for i in 0 .. size - 1 -> { Row = i; Col = i } ]
        let diag2 = [ for i in 0 .. size - 1 -> { Row = i; Col = size - i - 1 } ]

        List.concat [ rows
                      cols
                      [ diag1; diag2 ] ]
        |> List.choose isLineMatching
        |> List.tryHead

    let update (msg: Message) (model: Model) =
        match (msg, model.GameStatus) with
        | MakeMove square, InProgress ->
            match Map.tryFind square model.Board with
            | None -> model, [ ShowError InfoMessages.squareOutOfBounds ]
            | Some (Some _) -> model, [ ShowError InfoMessages.squareOccupied ]
            | Some None ->
                let updatedBoard =
                    model.Board
                    |> Map.add square (Some model.CurrentPlayer)

                let updatedGameStatus =
                    match checkWinner updatedBoard (getBoardSize model.Board) with
                    | Some player -> Winner player
                    | None when updatedBoard |> Map.forall (fun _ v -> v.IsSome) -> Draw
                    | None -> InProgress

                let nextPlayer =
                    if model.CurrentPlayer = Player.X then
                        Player.O
                    else
                        Player.X

                { model with
                    Board = updatedBoard
                    CurrentPlayer = nextPlayer
                    GameStatus = updatedGameStatus },
                []
        | MakeMove _, _ -> model, [ ShowError InfoMessages.gameOver ]
        | InvalidInput err, _ -> model, [ ShowError $"Invalid input: {err}" ]
        | ExitGame, _ ->
            model,
            [ Print InfoMessages.exitingGame
              Command.ExitGame ]

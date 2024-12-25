namespace TicTacToeGameElmStyle_ver2.App

open System

module GameLoop =
    open TicTacToeGameElmStyle_ver2.Features.Update
    open TicTacToeGameElmStyle_ver2.Features.View
    open TicTacToeGameElmStyle_ver2.Shared.Command

    let rec programLoop model =
        let commandsFromView = view model

        commandsFromView
        |> List.iter (function
            | ShowError err -> printfn "%s" err
            | Print msg -> printfn "%s" msg
            | ReadInput getMsgFromUserInput ->
                let input = Console.ReadLine()
                let msg = getMsgFromUserInput input
                let newModel, commandsFromUpdate = update msg model

                commandsFromUpdate
                |> List.iter (function
                    | ShowError err -> printfn "%s" err
                    | Print msg -> printfn "%s" msg
                    | ExitGame ->
                        printfn "Game exited."
                        exit 0
                    | _ -> ())

                programLoop newModel

            | ExitGame ->
                printfn "Game exited."
                exit 0)

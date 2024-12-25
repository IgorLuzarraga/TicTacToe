namespace TicTacToeGameElmStyle_ver2.App

module Program =
    open TicTacToeGameElmStyle_ver2.Features.Init
    open TicTacToeGameElmStyle_ver2.Shared

    let runGame boardSize =
        if boardSize <= 0 then
            printfn "%s" InfoMessages.boardSizeError
        else
            let initialModel = init boardSize
            GameLoop.programLoop initialModel

    [<EntryPoint>]
    let main argv =
        printfn "Tic Tac Toe game started."
        runGame 2
        0

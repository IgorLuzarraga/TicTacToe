namespace TicTacToeGameElmStyle_ver2.Shared

module InfoMessages =
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
    let enterStartOverPrompt = "Do you want to play another game? yes (y), no (n)"
    let currentBoard = "Current Board:"

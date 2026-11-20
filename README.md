# UCI GUI

A Windows desktop application for exploring chess positions with UCI-compatible engines such as Stockfish. Enter a FEN position, edit it on the visual board, and ask the engine to find a move.

## Features

- Enter or edit a position using FEN notation; the board updates to match.
- Drag pieces between board squares. The FEN field stays synchronized with board edits.
- Drag pieces from the piece bank onto the board to set up a position.
- See pieces captured during board editing in the captured tray, and drag them back onto the board.
- Start and stop engine analysis, then view the best move and ponder move.
- Adjust engine options exposed by the selected UCI engine.

Board editing is intended for position setup and does not enforce legal chess moves. The captured tray tracks captures made while editing the board; FEN does not include capture history, so it cannot be reconstructed from a pasted position.

## Requirements

- Windows with the .NET 11 SDK/runtime.
- A UCI-compatible chess engine. The project is configured to use the included `stockfish-windows-x86-64-universal.exe` by default.

## Run

From the repository root:

```powershell
dotnet run --project .\UciGui\UciGui.csproj
```

Enter a FEN in the **Position** field or arrange a position using the board and piece bank, then select **GO**. Select **STOP** to stop the search and display the engine's result.

The configured engine executable is shown in the Engine options panel. To use a different engine, update the `UciEngineExe` application setting in `UciGui\Properties\Settings.settings` and ensure the executable is available to the application.

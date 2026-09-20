# Color Match

A casual mobile arcade game made with Unity. Coloured shapes fall from the top of the screen while the player drags a basket along the bottom with a floating joystick. The basket changes colour every few seconds, and only shapes of its current colour should be caught: a match scores points, a wrong colour costs them. A round lasts a fixed time, after which the score and the best result are shown.

Portrait orientation, one-thumb controls, three difficulty levels.

## Demo

<!-- Replace these links once a build is available -->
- **WebGL:** _coming soon_
- **APK:** _coming soon_

## Screenshots

| Menu | Gameplay |
|---|---|
| <img src="docs/menu.png" width="320" alt="Main menu with difficulty selection" /> | <img src="docs/gameplay.png" width="320" alt="Gameplay" /> |

## How to play

- Drag anywhere on the screen — a joystick appears under your finger and moves the basket horizontally.
- The corner widget shows the current target colour and a ring counting down to the next change. The basket is tinted with that same colour.
- Catching a shape of the target colour scores points; catching any other colour subtracts them. Missed shapes cost nothing.
- The round ends when the timer runs out. The best score is stored separately for each difficulty.

| Difficulty | Spawn interval | Fall speed | Colour change | Points / penalty |
|---|---|---|---|---|
| Easy | 0.9–1.6 s | 2.0–2.8 | 8 s | +10 / −3 |
| Normal | 0.6–1.4 s | 2.5–3.5 | 6 s | +10 / −5 |
| Hard | 0.35–0.8 s | 3.5–4.8 | 4 s | +15 / −10 |

## Tech stack

- **Unity 6** (6000.3.15f1), C#
- **URP 2D** (Renderer2D, Global Light 2D)
- **Input System** — the joystick reads `Pointer.current`, so mouse and touch behave identically

## Project structure

```
Assets/_Project/
├── Art/            sprites (shapes, basket, UI, FX) and particle materials
├── Audio/SFX/      catch and round-end sounds
├── Data/           ShapePalette + Difficulty/{Easy,Normal,Hard}
├── Prefabs/
│   ├── Gameplay/   Basket
│   ├── Shapes/     six shapes
│   └── UI/         UIButton, DifficultyButton, HUD, GameOverPanel, PausePanel, Joystick
├── Scenes/         Menu, Game
└── Scripts/
    ├── Core/       GameManager, SceneFlow, ScoreStorage, DifficultySelection, CatchResult
    ├── Data/       ShapePalette, DifficultySettings
    ├── Gameplay/   TargetColor, RoundEndSound
    │   ├── Basket/ BasketMovement, BasketCatcher, BasketColorView, CatchFeedback, FloatingJoystick
    │   └── Shapes/ ShapeSpawner, FallingShape, ShapeKillzone
    └── UI/         MenuScreen, ScoreView, TimerView, TargetColorView, GameOverPanel, PausePanel
```

A few decisions that shape the architecture:

- **`GameManager` owns the rules and the round state** — score, timer and pause. The basket only reports "I caught this shape" through a single event, and the manager decides whether it was a match. That keeps every rule in one place.
- **Views subscribe to events instead of polling** — `ScoreView`, `TimerView`, `TargetColorView` and the panels refresh only when something actually changes.
- **Shapes know nothing about the pool** — each one raises a `Released` event, and the spawner listening to it returns the object to the pool. A separate trigger zone catches shapes that fall past the bottom edge.
- **Pause and round end freeze the game through `Time.timeScale`** — one line stops the physics, the spawner and the colour timer at once.

## Running the project

1. Open the project in **Unity 6000.3.15f1** or newer.
2. Open `Assets/_Project/Scenes/Menu.unity` and press Play.

Both scenes are already registered in Build Settings in the order `Menu` → `Game`.

# Color Match — план розробки

Unity 6000.3 · URP 2D (Renderer2D) · Input System 1.19 · uGUI + TextMeshPro · портретна орієнтація (1080×1920).

Вимоги ТЗ → де закриваються:

| Вимога | Скрипт |
|---|---|
| Фігури різних кольорів/форм падають зверху (критерій — лише колір) | `ShapeSpawner`, `FallingShape` |
| Кошик рухається drag-ом по горизонталі | `BasketController` |
| Кошик раз на N секунд міняє колір на випадковий | `BasketColorChanger` |
| +очки за правильний колір, −очки за неправильний | `Basket` (тригер) → `ScoreManager` |
| Меню з рекордом / HUD (рахунок + час) / Game Over | `MainMenuScreen`, `HudScreen`, `GameOverScreen` |
| Бонус: анімації + звуки | `ShapeFeedback`/`BasketFeedback` (корутини-твіни), `AudioManager` |
| Бонус: вибір складності | `DifficultySettings` (ScriptableObject) × 3 |

---

## Структура файлів

```
Assets/_Project/
├── Art/
│   └── Sprites/                  ← ГОТОВО (згенеровано процедурно, білі → тонуються через SpriteRenderer.color)
│       ├── Shapes/               Shape_Circle/Square/Triangle/Diamond/Hexagon/Star  (256px, PPU 256 = 1 юніт)
│       ├── Basket/               Basket  (512×320, PPU 256 = 2×1.25 юніта)
│       ├── Background/           Background  (градієнт, розтягується під камеру)
│       ├── UI/                   UI_RoundedRect, UI_RoundedRectOutline, UI_Button (9-slice, border 36)
│       │                         Icon_Clock, Icon_Star, Icon_Play
│       └── FX/                   FX_SoftCircle, FX_Ring, FX_Sparkle  (для Particle System)
├── Audio/                        SFX_Catch_Good, SFX_Catch_Bad, SFX_ColorChange, SFX_Click, Music_Loop
├── Data/                         ScriptableObject-ассети
│   ├── GameConfig.asset          палітра кольорів, список спрайтів форм, тривалість раунду
│   └── Difficulty/               Difficulty_Easy / Normal / Hard.asset
├── Prefabs/
│   ├── FallingShape.prefab
│   ├── Basket.prefab
│   └── FX_Catch.prefab
├── Scenes/
│   └── Game.unity                одна сцена: і меню, і гра (UI-екрани перемикаються)
└── Scripts/
    ├── ColorMatch.asmdef         (опціонально, пришвидшує компіляцію)
    ├── Core/                     GameManager, GameState, ScoreManager, GameTimer, SaveSystem
    ├── Gameplay/                 BasketController, BasketColorChanger, Basket, ShapeSpawner, FallingShape, ShapePool
    ├── Data/                     GameConfig, DifficultySettings
    ├── UI/                       UIManager, MainMenuScreen, HudScreen, GameOverScreen
    ├── Feedback/                 ShapeFeedback, BasketFeedback, Tween (мінімальний хелпер на корутинах)
    └── Audio/                    AudioManager
```

Правила: namespace `ColorMatch.*` за папкою; `[SerializeField] private` замість public полів; одна сцена — менше мороки з перезавантаженням і збереженням стану.

---

## Скрипти (коротко)

### Data
- **`GameConfig`** (SO) — `Color[] palette` (4 кольори, щоб матчі траплялись часто), `Sprite[] shapeSprites`, `float roundDuration = 60`, `int pointsGood = 10`, `int pointsBad = -5`.
- **`DifficultySettings`** (SO) — `fallSpeedMin/Max`, `spawnInterval`, `basketColorInterval`, `matchChance` (ймовірність, що фігура буде кольору кошика — щоб гра не була чистим рандомом), `displayName`.

### Core
- **`GameState`** — `enum { Menu, Playing, GameOver }`.
- **`GameManager`** — синглтон-«диригент». `StartGame(DifficultySettings)`, `EndGame()`, `ReturnToMenu()`. Подія `event Action<GameState> StateChanged`. Вмикає/вимикає спавнер і кошик, скидає рахунок і таймер.
- **`GameTimer`** — відлік `roundDuration`, подія `TimeChanged(float)`, `Finished`.
- **`ScoreManager`** — `Add(int)`, подія `ScoreChanged(int score, int delta)`, рахунок не опускається нижче 0.
- **`SaveSystem`** — статичний клас-обгортка над `PlayerPrefs` (`BestScore`, `LastDifficulty`). Легко замінити на JSON, якщо спитають.

### Gameplay
- **`BasketController`** — drag через Input System (`Pointer.current` — працює і мишка, і тач). Позиція пальця → `Camera.ScreenToWorldPoint`, кошик плавно їде до X (`Mathf.MoveTowards`/`Lerp`), clamp межами екрана з урахуванням половини ширини спрайта.
- **`BasketColorChanger`** — корутина: кожні `basketColorInterval` с обирає **інший** колір з палітри, тонує `SpriteRenderer`, подія `ColorChanged(int colorIndex)`. Перед зміною — попередження (мигання 0.5 с), щоб гравець встиг зреагувати.
- **`Basket`** — `OnTriggerEnter2D`: бере `FallingShape`, порівнює `colorIndex` (індекс, не `Color` — порівняння float-кольорів ненадійне), нараховує очки, запускає фідбек, повертає фігуру в пул.
- **`FallingShape`** — `ColorIndex`, `Setup(sprite, colorIndex, color, speed)`. Рух `transform.Translate` у `Update` (без Rigidbody-фізики — передбачувано). Нижче екрана → назад у пул. Колайдер `CircleCollider2D` (isTrigger), на кошику — `Rigidbody2D Kinematic` + `BoxCollider2D` trigger тільки на «отворі» кошика.
- **`ShapePool`** — обгортка над `UnityEngine.Pool.ObjectPool<FallingShape>` (вбудований, не треба писати свій).
- **`ShapeSpawner`** — корутина з `spawnInterval`, випадковий X у межах екрана, випадкова форма, колір: з імовірністю `matchChance` — поточний колір кошика, інакше випадковий. `StopAndClear()` на кінець раунду.

### UI (uGUI + TMP, Canvas Scaler: Scale With Screen Size 1080×1920, match 0.5)
- **`UIManager`** — підписується на `GameManager.StateChanged`, вмикає потрібний екран.
- **`MainMenuScreen`** — рекорд, 3 кнопки складності (або перемикач + Play).
- **`HudScreen`** — рахунок, таймер (`mm:ss`, червоніє на останніх 10 с), індикатор кольору кошика, спливаючі «+10 / −5».
- **`GameOverScreen`** — фінальний рахунок, рекорд, «NEW BEST!», кнопки Restart / Menu.

### Feedback / Audio
- **`Tween`** — 2-3 статичні корутини (`ScalePunch`, `FadeOut`). DOTween можна поставити, але для такого обсягу зайва залежність.
- **`ShapeFeedback`** — легке обертання/погойдування при падінні, pop при спавні.
- **`BasketFeedback`** — squash & stretch при ловлі, shake при помилці, плавна зміна кольору.
- **`AudioManager`** — синглтон, `PlaySfx(clip)` через один `AudioSource.PlayOneShot` + окремий для музики.

### Потік подій
```
MainMenu ──Play(difficulty)──▶ GameManager.StartGame
   Spawner.Begin · Timer.Begin · Basket.Enable
Basket.OnTrigger ──▶ ScoreManager.Add ──ScoreChanged──▶ HUD / Feedback / Audio
Timer.Finished ──▶ GameManager.EndGame ──▶ SaveSystem.TrySetBest ──▶ GameOverScreen
GameOver ──Restart / Menu──▶ GameManager
```
Зв'язки — через `[SerializeField]` посилання в інспекторі + C#-події. Без DI і шини подій.

---

## Сцена `Game.unity`

```
Main Camera            orthographic size 9.6 (портрет 1080×1920 → ширина ~10.8 юн.)
Global Light 2D        (вже є; спрайти на Sprite-Lit-Default)
Background             SpriteRenderer, order -100, скейл під камеру
Managers               GameManager, ScoreManager, GameTimer, AudioManager
Gameplay
 ├── Basket            (prefab) y ≈ -7
 └── ShapeSpawner      y ≈ +10.5 (над екраном)
Canvas                 UIManager
 ├── MainMenuScreen
 ├── HudScreen
 └── GameOverScreen
EventSystem            InputSystemUIInputModule
```
Sorting: Background -100 · Shapes 0 · Basket 10 · FX 20.

---

## Етапи

### День 1 — Core gameplay
1. Сцена `Game.unity`, камера, фон, папки. Додати в Build Settings.
2. `GameConfig` + `DifficultySettings` + 3 ассети.
3. `FallingShape` + префаб, `ShapePool`, `ShapeSpawner`.
4. `BasketController` (drag + clamp), `BasketColorChanger`, `Basket` (тригер + перевірка кольору).
5. `ScoreManager`, `GameTimer`, мінімальний `GameManager` (старт одразу при Play).
**Ціль:** гра грається, рахунок видно в консолі.

### День 2 — UI + game flow
1. Canvas + 3 екрани, TMP-шрифт.
2. `UIManager`, `GameManager` state machine, `SaveSystem`.
3. Вибір складності в меню.
4. Баланс (стартові значення нижче).
**Ціль:** повний цикл Menu → Play → GameOver → Menu.

### День 3 — полірування
1. Фідбек-анімації кошика/фігур, спливаючі очки.
2. Particle System на ловлю (`FX_SoftCircle`/`FX_Sparkle`, колір = колір фігури).
3. Звуки + музика.
4. Тест на різних співвідношеннях сторін, білд (Android/WebGL), README з поясненням архітектури.

### Стартовий баланс

| | Easy | Normal | Hard |
|---|---|---|---|
| Швидкість падіння | 2.5–3.5 | 3.5–5 | 5–7 |
| Інтервал спавну, с | 0.9 | 0.65 | 0.45 |
| Зміна кольору кошика, с | 7 | 5 | 3.5 |
| matchChance | 0.45 | 0.35 | 0.3 |

Раунд: 60 с · +10 за правильний · −5 за неправильний.

---

## Поради
- Не переускладнювати: читабельність і робочий продукт важливіші за патерни.
- Git: коміт на кожну фічу (`feat: basket drag`, `feat: shape spawner`…).
- Кінець — стабільний баланс і чистий код, а не зайва фіча з багами.

# Spell Core Automata - AI Agent Guide

## Project Overview

**Spell Core Automata** is a Unity-based 3D Action RPG (ARPG) project featuring a data-driven combat system with spell casting, projectile mechanics, and dynamic skill systems.

- **Engine**: Unity 2020.3.45f1 (LTS)
- **Language**: C# with Chinese comments/documentation
- **Render Pipeline**: Built-in Render Pipeline (Deferred rendering)
- **Input System**: Unity Input System (new)

## Project Architecture

### Core Framework (WJS)

The project uses a custom framework under the `WJS` namespace that implements a **data-driven combat system**:

| Module | Purpose |
|--------|---------|
| `WJS/Character/` | Character state management (`ChaState`), properties (`ChaProperty`), resources (`ChaResource`), and animation info |
| `WJS/Skill/` | Skill definitions (`SkillModel`), skill objects (`SkillObj`) - data-driven skill system |
| `WJS/Timeline/` | Timeline-based action sequences (`TimelineModel`, `TimelineNode`, `TimelineManager`) |
| `WJS/Bullet/` | Projectile system (`BulletLauncher`, `BulletState`, `BulletManager`) |
| `WJS/AoE/` | Area of Effect system (`AoeLauncher`, `AoeState`, `AoeManager`) |
| `WJS/Buff/` | Buff/Debuff system (`BuffModel`, `BuffObj`, `AddBuffInfo`) |
| `WJS/Damage/` | Damage calculation (`Damage`, `DamageInfo`, `DamageManager`) |
| `WJS/Item/` | Equipment (`EquipmentModel`, `RuneModel`) and inventory systems |
| `WJS/UI/` | Custom UI framework (`UIManager`, `UIWindow`, etc.) |
| `WJS/Common/` | Event system (`EventManager`), delegates (`GameDelegate`), and utilities |

### Key Design Patterns

1. **MonoSingleton<T>**: Base class for managers (`GameManager`, `BattleManager`, `DamageManager`, etc.)
2. **Data-Driven Design**: Game data defined in static dictionaries (e.g., `SkillData.data`, `BuffData.data`)
3. **Object Pooling**: Uses `MMMultipleObjectPooler` from MMTools for bullets and effects
4. **Event-Driven**: `EventManager<T>` for delegate-based event handling with condition support
5. **SceneVariants**: Static facade providing scene-level APIs for creating game objects

### Data Flow

```
SkillModel (designer-defined)
    ↓
TimelineModel (action sequence)
    ↓
TimelineNode[] (individual actions)
    ↓
BulletLauncher/AoeLauncher/AddBuffInfo
    ↓
SceneVariants.CreateXXX() → GameManager.CreateXXX()
```

## Code Organization

### Folder Structure

```
Assets/
├── Scripts/
│   ├── WJS/              # Core game framework
│   ├── AssetDatabase/    # Asset management (ScriptableObject-based)
│   ├── Cube/             # Throwable cube mechanics
│   ├── UI/UIWindow/      # UI window implementations
│   ├── BattleManager.cs  # Battle state management
│   └── ...
├── Art/
│   ├── Feel/             # MMFeedbacks + MMTools + NiceVibrations
│   └── Epic Toon FX/     # Visual effects library
├── Plugins/
│   ├── FImpossible Creations/  # Bones Stimulator
│   ├── RPG Cameras & Controllers/
│   └── Sirenix/          # Odin Inspector
├── Resources/
│   ├── Prefabs/          # Character, Bullet, Effect prefabs
│   └── ScriptableObject/ # AssetDatabase
└── Scenes/               # Game scenes
```

### Naming Conventions

- **Classes**: PascalCase (e.g., `GameManager`, `ChaState`)
- **Methods**: PascalCase (public), camelCase (private)
- **Properties**: PascalCase
- **Fields**: Private fields use underscore prefix (e.g., `_controlState`)
- **Comments**: Primarily in Chinese

## Third-Party Dependencies

| Package | Version | Purpose |
|---------|---------|---------|
| com.unity.cinemachine | 2.6.17 | Camera control |
| com.unity.inputsystem | 1.5.1 | Input handling |
| com.unity.postprocessing | 3.2.2 | Post-processing effects |
| com.unity.textmeshpro | 3.0.9 | Text rendering |
| Feel (MMFeedbacks) | - | Haptic/visual feedback system |
| MMTools | - | Utility library (singletons, pooling, extensions) |
| NiceVibrations | - | Mobile haptics |
| Odin Inspector | - | Enhanced inspector |
| Epic Toon FX | - | Particle effects |
| RPG Cameras & Controllers | - | Character controller and camera |
| Bones Stimulator | - | Procedural animation |

## Build and Development

### Prerequisites

- Unity 2020.3.45f1 or compatible
- Visual Studio 2019/2022 or JetBrains Rider (configured in packages)

### Build Settings

- **Default Screen**: 1920x1080
- **Color Space**: Linear
- **Rendering Path**: Deferred
- **Target Platforms**: PC (Windows), with mobile support infrastructure

### Key Managers (Singletons)

| Manager | Responsibility |
|---------|----------------|
| `GameManager` | Character spawning, bullet/AoE creation, visual effects, game pause |
| `BattleManager` | Battle state (`IsInBattle`), main character initialization |
| `DamageManager` | Damage calculation and application |
| `TimelineManager` | Skill timeline execution |
| `BulletManager` | Active bullet tracking and management |
| `AoeManager` | Active AoE tracking and management |
| `UIManager` | UI window management |
| `AssetDatabaseManager` | Prefab and asset loading |

## Important Implementation Details

### Character System

- Characters use `ChaState` component for all state management
- Properties calculated from `ChaProperty` (move speed, action speed, etc.)
- Side-based faction system (0 = player, other = enemies)
- Immunity time system for i-frames

### Skill System

- Skills defined in `SkillData.Initialize()` as static data
- Skills use TimelineModel for sequential actions
- Support for charge mechanics and looped timeline nodes
- Dynamic description generation with parameter substitution

### Combat Mechanics

- **Bullets**: Projectiles with hit detection, can trigger timelines on hit
- **AoE**: Area effects with tick/enter/leave callbacks
- **Buffs**: Stackable status effects with various trigger conditions (OnHit, OnBeHurt, OnTick, etc.)
- **Damage**: `DamageInfo` objects with tags for damage type classification

### Event System

The `EventManager<T>` supports these delegate types:
- `BulletOnHit`, `BulletOnCreate`, `BulletOnRemoved`
- `AoeOnCreate`, `AoeOnTick`, `AoeOnRemoved`, `AoeOnCharacterEnter`, `AoeOnCharacterLeave`, `AoeOnBulletEnter`, `AoeOnBulletLeave`
- `BuffOnOccur`, `BuffOnTick`, `BuffOnRemoved`, `BuffOnHit`, `BuffOnBeHurt`, `BuffOnKill`, `BuffOnBeKilled`, `BuffOnCast`

## Testing and Debugging

- Uses Unity Test Framework (com.unity.test-framework)
- `UnitSkillTester` component for skill testing
- MMDebugMenu integration for runtime debugging
- Editor tools in `Assets/Editor/` folder

## Security Considerations

- No sensitive data in repository
- Save/Load system uses binary serialization (potential for encryption via `MMSaveLoadManagerMethodBinaryEncrypted`)

## Common Tasks for AI Agents

### Adding a New Skill

1. Define skill data in `SkillData.Initialize()` with `SkillModel`
2. Create TimelineModel with TimelineNode[] for skill actions
3. Add to SkillData dictionary with unique key
4. Reference key in character initialization or skill learning

### Creating a New Buff

1. Define in `BuffData.Initialize()` with `BuffModel`
2. Implement callback methods (OnTick, OnHit, etc.)
3. Use `AddBuffInfo` to apply to characters

### Adding Projectiles

1. Create prefab with `BulletState` component
2. Configure `BulletLauncher` with model, position, rotation
3. Use `SceneVariants.CreateBullet()` or `GameManager.CreateBullet()`

### UI Development

1. Extend `UIWindow` base class
2. Register in `UIManager`
3. Use window stack system for navigation

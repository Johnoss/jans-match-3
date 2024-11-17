# Fruit Salad Epic Saga 2D

## Overview
Fruit Salad Epic Saga 2D is a match-3 puzzle game developed as an exercise.


https://github.com/user-attachments/assets/a65fd95b-9343-4e33-ad08-803b7e27bd3e


## Key Technologies
- **ECS (LeoECS)**: Utilized for efficient and scalable entity-component-system architecture.
  - Entry points: [`EcsInitializer.cs`](Assets/Scripts/Initialization/ECS/ECSInitializer.cs), [`ProjectContext.prefab`](/Assets/Resources/ProjectContext.prefab), `SceneContext` in Main scene
- **Zenject**: Dependency injection and factory pattern for runtime creation of views
  - Entry points: [`GameSceneInstaller.cs`](Assets/Scripts/Initialization/GameSceneInstaller.cs), [`FactoryInstaller.cs`](Assets/Scripts/Initialization/FactoryInstaller.cs), [`ConfigInstaller.cs`](Assets/Scripts/Initialization/ConfigInstaller.cs)
- **UniRX**: Reactive pattern, mostly for handling presentation layer. Lightly paired with MVC
  - Example: [`GameUIView.cs`](Assets/Scripts/Features/UI/GameUIView.cs), [`MainMenuView.cs`](Assets/Scripts/Features/UI/MainMenuView.cs), [`GameSessionModel.cs`](Assets/Scripts/Features/GameSession/GameSessionModel.cs)
- **DOTween**: Animations and visual feedback
  - Example: [`AnchorPositionTweener.cs`](Assets/Scripts/Features/Tweening/AnchorPositionTweenView.cs) used by [`PieceEntityView.cs`](Assets/Scripts/Features/Grid/Piece/PieceEntityView.cs)

## Project Structure
- **Gameplay Layer**: Handles the core game logic, including entity management, game rules, and state transitions.
  - ECS, Services
- **Presentation Layer**: Manages the user interface, animations, and visual feedback with minimal overlap with business logic (no waiting for animation to finish to continue doing stuff)
  - Views, Tweens
- **Feature-Based Folder Structure**: Scripts are categorized based on features (e.g., [`EmptyTileComponent.cs`](Assets/Scripts/Features/Grid/Components/EmptyTileComponent.cs), [`GridView.cs`](Assets/Scripts/Features/Grid/GridView.cs), [`GridService.cs`](Assets/Scripts/Features/Grid/GridView.cs)) instead of having a generic 'Services', 'Views' folder. This allows for quick navigation based on current context. In some cases feature folders have subfolders with components (to prevent clutter)

## Configurability
The project supports configurability through various configuration files, allowing easy customization of game settings and behaviors. Examples include:
- [`GridConfig.json`](Assets/Scripts/Features/Grid/GridConfig.cs): Specifies grid settings such as grid size.
- [`PieceConfig.json`](Assets/Scripts/Features/Grid/Piece/PieceConfig.cs): Lists all matchable pieces and their properties.
- [`RulesConfig.json`](Assets/Scripts/Features/Grid/Matching/RulesConfig.cs): Defines game rules, such as allowing square matches.

The idea is that these configuration files should be accessible to game designers and artists, enabling them to adjust game settings and behaviors without needing to modify the code.

## Game Features
- No valid matches validator is ECS-based and runs only a certain number of iterations per frame (configurable in `RulesConfig`)
- Board reshuffles when there's no possible move
- Matching 2x2 is allowed (configurable in `RulesConfig`)
- Game is limited to 60 seconds, but pauses during board activity (matches, etc.)
- Score/match scales with current combo (configurable in `RulesConfig`)
- Grid resolution is configurable. Game runs well with insane configurations (though initialization can take a while)

https://github.com/user-attachments/assets/48a23d38-0844-44a8-87eb-1100ed84a61a


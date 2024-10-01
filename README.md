# Nightmare Negotiations

Multiplayer Co-Op Horror Game.

Relay Server: https://github.com/jjszaniszlo/game-relay-server

# Contribution Guidelines

1) Commits should be atomic, meaning that they are short and independent units.
2) All changes will be made through a Pull Request and a feature branch.  This ensures that main stays somewhat stable through development.

# Project Structure

- `addons`: Where Godot plugins/addons go.  Should be largely unchanged
- `Assets`: Raw Asset Resources
  - `models`: All model groups
    - `characters`: All character models
    - `trees`: All tree models
    - `bushes`: All bush models
  - `textures`: All texture groups
      - `characters`: All character textures
      - `trees`: All tree textures
      - `bushes`: All bush textures
- `GameObjects`: Scenes for game objects that *are not* static.  i.e) characters, world-model for items
- `Materials`: Shader material or PBR material specifications.
- `Scenes`: Where each game scene will go.  i.e) Main Menu, Main 3D Scene, etc.
- `Shaders`: GDShader source file location
- `src`: C# source file location
- `WorldObjects`: Scenes for world objects that *are* static. i.e) trees, bushes

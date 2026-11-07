WickedIslandMods UE4SS template

Contents:
- enabled.txt: enables this mod folder.
- scripts/main.lua: starter Lua script with:
  - number-based pose switching scaffold
  - on-load overlay text scaffold
  - universal entity eligibility policy scaffold

Next steps:
1) Hook key events in scripts/main.lua to call switchPoseByNumber(1..9).
2) Replace log/render stubs with game UI and interaction hooks.
3) Add your own game-specific API calls where marked in comments.

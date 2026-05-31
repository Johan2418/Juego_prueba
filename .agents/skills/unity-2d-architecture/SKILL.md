---
name: unity-2d-architecture
description: Use this skill when creating or modifying Unity 2D gameplay architecture, scripts, prefabs, camera logic, player systems, or scene organization.
---

# Unity 2D Architecture Skill

Use this skill for Unity 2D architecture decisions.

## Rules

- Keep systems small.
- Do not place movement, combat, animation, UI, camera and interaction in one script.
- Prefer Inspector references over global searches.
- Use clear C# names.
- Explain required Unity setup after code changes.
- Avoid unnecessary design patterns.
- Prioritize a playable prototype over complex architecture.

## Recommended script separation

- Player movement
- Player input
- Player animation
- Combat or interaction
- Camera follow
- UI
- Game state

## Output expected

When finished, explain:
1. What was changed.
2. Which files were touched.
3. Which GameObjects/components must be configured in Unity.
4. How to test the change.

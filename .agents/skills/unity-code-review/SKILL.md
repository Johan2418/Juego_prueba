---
name: unity-code-review
description: Use this skill when reviewing Unity C# scripts, scene-related changes, prefabs, gameplay bugs, architecture issues, or Codex-generated changes before committing.
---

# Unity Code Review Skill

Review Unity code and project changes.

## Look for

- Null reference risks.
- Missing Inspector references.
- Large MonoBehaviours doing too much.
- Incorrect Update vs FixedUpdate usage.
- Expensive operations during gameplay.
- Scene or prefab setup risks.
- Broken Unity `.meta` handling.
- Mobile performance concerns.
- Unclear naming.
- Unnecessary dependencies.

## Output format

Use this format:

### Main issues

### Suggested fixes

### Risk level

Low / Medium / High

### Files to check manually in Unity

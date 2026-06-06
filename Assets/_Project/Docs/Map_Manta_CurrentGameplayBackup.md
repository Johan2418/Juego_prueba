# Map Manta Current Gameplay Backup

Source: merge stage 2 (`Current/Ours`) from local commit `6414653`.

## Backup files

- Scene: `Assets/_Project/Scenes/Main/Map_Manta_CurrentGameplayBackup.unity`
- Interaction UI script: `Assets/_Project/Docs/InteractionPromptUI_CurrentBackup.txt`

The scene backup contains the complete Current hierarchy and serialized
references. Do not use it as the final map directly; use it as the source when
moving gameplay objects into the Incoming painted map.

## Gameplay objects missing from Incoming

- `NPC_Pescador_Tutorial`
- `NPC_Guia_Pacoche`
- `FoodStand_Ceviche`
- `InteractionArea`
- `SecretEntrance_Rock`
- `Trigger_MarketQuest`
- `Trigger_FishingZone`
- `SuccessZone`
- `BarFrame`
- `Marker_Market`
- `Marker_Port`
- `Marker_Plaza`
- `Marker_Beach`
- `Marker_Forest`
- `Marker_SecretZone`
- `Zone_MERCADO`
- `Zone_PLAZA CENTRAL`
- `Zone_PUERTO PESQUERO`
- `Zone_ZONA SECRETA`

## Resolution choice

- `Assets/Scenes/Map_Manta_Prototype.unity`: accept Incoming/Theirs.
- `Assets/_Project/Scripts/UI/InteractionPromptUI.cs`: accept Current/Ours.

## Commands

```powershell
git checkout --theirs -- "Assets/Scenes/Map_Manta_Prototype.unity"
git checkout --ours -- "Assets/_Project/Scripts/UI/InteractionPromptUI.cs"

git add -- "Assets/Scenes/Map_Manta_Prototype.unity"
git add -- "Assets/_Project/Scripts/UI/InteractionPromptUI.cs"
git add -- "Assets/_Project/Scenes/Main/Map_Manta_CurrentGameplayBackup.unity"
git add -- "Assets/_Project/Scenes/Main/Map_Manta_CurrentGameplayBackup.unity.meta"
git add -- "Assets/_Project/Docs/InteractionPromptUI_CurrentBackup.txt"
git add -- "Assets/_Project/Docs/InteractionPromptUI_CurrentBackup.txt.meta"
git add -- "Assets/_Project/Docs/Map_Manta_CurrentGameplayBackup.md"

git status
```

After resolving, open the Incoming scene in Unity and migrate the listed
objects from the backup before completing the merge commit.

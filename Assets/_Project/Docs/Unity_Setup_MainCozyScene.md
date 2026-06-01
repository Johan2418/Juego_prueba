# Unity Setup: MainCozyScene

Este proyecto no crea escenas `.unity` desde Codex. Sigue estos pasos para preparar:

- `Assets/_Project/Scenes/Main/MainCozyScene.unity`

## 1. Crear escena y objetos base

1. Crea la escena `MainCozyScene` dentro de `Assets/_Project/Scenes/Main`.
2. Crea un `Grid` con `Tilemap` (o un `SpriteRenderer` simple) para piso temporal.
3. Crea obstaculos con `BoxCollider2D` para probar colisiones.
4. Agrega un `Canvas` (Screen Space - Overlay) para UI minima.

## 2. Player

1. Crea `Player`:
   - `Rigidbody2D` (Body Type: Dynamic, Gravity Scale: 0, Freeze Rotation Z).
   - `Collider2D` (Capsule o Box).
   - `PlayerController`.
   - `PlayerInteractor`.
   - `PlayerAnimationController` (opcional si todavia no hay Animator).
2. Tag recomendado: `Player`.
3. En `PlayerInteractor`:
   - `Interaction Key`: `E`.
   - Ajusta `Interaction Radius` y `Forward Offset`.
   - Configura `Interaction Layers` para NPC/Water/Interactables.

## 3. Camera

1. En `Main Camera` agrega `CameraFollow2D`.
2. Asigna `Target` = `Player`.
3. Ajusta `Offset` (ejemplo: `0,0,-10`) y `Smooth Speed`.

## 4. Managers

Crea un objeto `GameManagers` y agrega:

- `DialogueManager`
- `QuestManager`
- `InventoryManager`
- `FishingManager`

En `FishingManager`, si quieres, asigna `InventoryManager` manualmente (si no, usa `InventoryManager.Instance`).

## 5. UI minima (TextMeshPro)

En el `Canvas`, crea:

1. `DialoguePanel`:
   - Fondo + textos TMP para nombre y linea.
   - Texto/objeto de "continuar".
2. `InteractionPrompt`:
   - Texto TMP para "Presiona E para ...".
3. `Notification`:
   - Texto TMP para mensajes temporales.

Agrega scripts:

- `InteractionPromptUI` en objeto de prompt.
- `NotificationUI` en objeto de notificacion.

Conecta en `DialogueManager`:

- `Dialogue Panel`
- `Speaker Name Text`
- `Dialogue Text`
- `Continue Indicator`

Conecta en `PlayerInteractor`:

- `Interaction Prompt UI`
- `Notification UI`

Opcional: conecta `Fallback Notification UI` en `FishingManager`.

## 6. ScriptableObjects de prueba

### Peces (FishData)

Crea en `Assets/_Project/ScriptableObjects/Fish`:

1. `Fish_Sardina`
   - `itemId`: `fish_sardina`
   - `displayName`: `Sardina`
   - `rarity`: `Common`
   - `catchWeight`: `0.6`
2. `Fish_PezAzul`
   - `itemId`: `fish_pez_azul`
   - `displayName`: `Pez Azul`
   - `rarity`: `Uncommon`
   - `catchWeight`: `0.3`
3. `Fish_PezDorado`
   - `itemId`: `fish_pez_dorado`
   - `displayName`: `Pez Dorado`
   - `rarity`: `Rare`
   - `catchWeight`: `0.1`

### QuestData de pesca

Crea en `Assets/_Project/ScriptableObjects/Quests`:

- `Quest_CatchOneFish`
  - `questId`: `quest_catch_one_fish`
  - `questTitle`: `Atrapa 1 pez`
  - `objectiveType`: `CatchFish`
  - `targetId`: (vacio para cualquier pez, o `fish_sardina` para un pez especifico)
  - `targetAmount`: `1`

### QuestData de hablar con NPC (opcional para segunda mision)

- `Quest_TalkVillager`
  - `questId`: `quest_talk_villager`
  - `questTitle`: `Habla con el aldeano`
  - `objectiveType`: `TalkToNPC`
  - `targetId`: `npc_villager`
  - `targetAmount`: `1`

### Dialogues

Crea `DialogueData` en `Assets/_Project/ScriptableObjects/Dialogues` para:

- NPC pescador: antes/durante/completada/entregada.
- NPC aldeano: dialogo simple.

## 7. NPCs

### NPC Pescador

1. Crea objeto `NPC_Fisher` con `Collider2D` (trigger opcional).
2. Agrega:
   - `NPCDialogue`
   - `NPCQuestGiver`
3. En `NPCQuestGiver`:
   - `npcId`: `npc_fisher`
   - `questToGive`: `Quest_CatchOneFish`
   - Asigna dialogos por estado.
4. En `NPCDialogue`:
   - Asigna referencia a `NPCQuestGiver`.

### NPC Aldeano

1. Crea `NPC_Villager` con `Collider2D`.
2. Agrega `NPCDialogue`.
3. Si quieres mision de hablar:
   - Agrega `NPCQuestGiver`
   - `npcId`: `npc_villager`
   - `questToGive`: `Quest_TalkVillager`

## 8. FishingSpot

1. Crea objeto `FishingSpot_WaterEdge` cerca del agua.
2. Agrega `Collider2D`.
3. Agrega script `FishingSpot`.
4. Configura:
   - `interactionPrompt`: `Presiona E para pescar`
   - `fishPool`: `Fish_Sardina`, `Fish_PezAzul`, `Fish_PezDorado`
   - Distancia y duracion.

## 9. Entrada zona secreta

1. Crea objeto `SecretZoneEntrance`.
2. Agrega `Collider2D`.
3. Agrega script `SecretZoneEntrance`.
4. Configura:
   - `interactionPrompt`: `Entrar a zona secreta`
   - `secretZoneSceneName`: nombre real de la escena secreta.
5. Si la escena no existe en Build Settings, el script muestra aviso y no rompe el juego.

## 10. Flujo de prueba (meta jugable inicial)

1. Inicia escena.
2. Muevete con WASD/flechas.
3. Habla con NPC pescador (`E`) y recibe mision.
4. Ve al `FishingSpot` y pesca (`E`).
5. Revisa consola para log de inventario.
6. Regresa al NPC pescador y entrega mision.
7. Interactua con `SecretZoneEntrance` para validar fallback o carga de escena.

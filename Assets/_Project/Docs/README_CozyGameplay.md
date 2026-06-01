# Cozy Gameplay Base (Fishing + NPC + Quests)

Este documento describe la base modular creada para el nuevo enfoque cozy.

## Alcance implementado

- Movimiento top-down del player (`PlayerController`) con bloqueo durante dialogo/pesca.
- Interaccion contextual (`PlayerInteractor`) mediante `IInteractable`.
- Camara simple de seguimiento (`CameraFollow2D`) sin dependencia obligatoria de Cinemachine.
- Dialogo simple con UI (`DialogueManager`, `DialogueData`, `DialogueLine`).
- NPCs interactuables con estado de mision (`NPCDialogue`, `NPCQuestGiver`).
- Misiones basicas (`QuestManager`, `QuestData`, `QuestObjective*`).
- Pesca por zonas (`FishingSpot`, `FishingManager`, `FishData`).
- Inventario simple (`InventoryManager`, `ItemData`).
- UI minima (`InteractionPromptUI`, `NotificationUI`).
- Entrada preparada para zona secreta (`SecretZoneEntrance`).

## Flujo principal esperado

1. Hablar con NPC pescador.
2. Recibir mision `Atrapa 1 pez`.
3. Pescar en `FishingSpot`.
4. Agregar pez al inventario.
5. Completar mision automaticamente.
6. Volver al NPC para entregar.
7. Explorar y encontrar entrada a zona secreta.

## Estructura cozy agregada

- Scripts:
  - `Assets/_Project/Scripts/Player`
  - `Assets/_Project/Scripts/Camera`
  - `Assets/_Project/Scripts/Interaction`
  - `Assets/_Project/Scripts/Dialogue`
  - `Assets/_Project/Scripts/NPC`
  - `Assets/_Project/Scripts/Quests`
  - `Assets/_Project/Scripts/Fishing`
  - `Assets/_Project/Scripts/Inventory`
  - `Assets/_Project/Scripts/World`
  - `Assets/_Project/Scripts/SecretZone` (reservado)
- Datos:
  - `Assets/_Project/ScriptableObjects/Items`
  - `Assets/_Project/ScriptableObjects/Fish`
  - `Assets/_Project/ScriptableObjects/Quests`
  - `Assets/_Project/ScriptableObjects/Dialogues`

## Notas de separacion con gameplay anterior

- No se borraron scripts previos de combate/enemigos/jefe.
- No se movieron assets existentes para evitar cambios destructivos.
- La transicion futura a zona secreta se deja mediante `SecretZoneEntrance`.

## Como probar rapido

1. Crea/configura la escena `Assets/_Project/Scenes/Main/MainCozyScene.unity`.
2. Sigue pasos de `Unity_Setup_MainCozyScene.md`.
3. Ejecuta Play Mode y valida:
   - Prompt de interaccion.
   - Dialogo con `E`.
   - Bloqueo de movimiento durante dialogo/pesca.
   - Pesca y logs de inventario.
   - Cambio de estado de mision.

## Checklist rapido

- [ ] Sin errores en Console.
- [ ] Player se mueve.
- [ ] Camara sigue al player.
- [ ] NPC muestra prompt e inicia dialogo.
- [ ] Mision de pesca inicia y se completa.
- [ ] Pez se agrega a inventario.
- [ ] Entrada de zona secreta responde sin romper escena.

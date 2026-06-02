# Unity Setup: Map_Manta_Prototype Demo Route

Este ajuste conecta las interacciones existentes de `Map_Manta_Prototype` con una ruta jugable de demo.

No requiere cambiar `PlayerController` ni eliminar scripts existentes.

## 1. Crear manager de ruta

1. Opcionalmente, en la escena `Map_Manta_Prototype`, crea un objeto `DemoQuestRouteManager`.
2. Agrega el script `DemoQuestRouteManager`.
3. Asigna:
   - `Current Objective Text`: texto TMP de objetivo actual (si ya existe, reutilizalo).
   - `Notification UI`: `NotificationUI` existente para mensajes temporales.
4. Opcional:
   - `Activate When Secret Unlocked`: objetos/zona secreta que deben activarse al abrir la roca.
   - `Deactivate When Secret Unlocked`: bloqueadores que deben desactivarse.

Si no agregas este objeto, el script crea un manager temporal al iniciar `Map_Manta_Prototype` y muestra objetivos en Console.

## 2. Configurar NPC_Pescador_Tutorial

Si el objeto se llama exactamente `NPC_Pescador_Tutorial`, no necesita configuracion extra.

Configuracion manual equivalente en `NPCDialogue`:

- `Enable Demo Route Hook`: `true`
- `Override Default Interaction With Demo Route`: `true`
- `Demo Route Interaction Id`: `npc_pescador_tutorial`

## 3. Configurar FishingSpot_Muelle

Si el objeto se llama exactamente `FishingSpot_Muelle`, no necesita configuracion extra.

Configuracion manual equivalente en `FishingSpot`:

- `Enable Demo Route Hook`: `true`
- `Override Default Fishing With Demo Route`: `false`
- `Demo Route Interaction Id`: `fishing_spot_muelle`

## 4. Configurar mercado (elige una opcion)

### Opcion A: NPC_Vendedora_Mercado ya usa NPCDialogue

Si el objeto se llama exactamente `NPC_Vendedora_Mercado`, no necesita configuracion extra.

Configuracion manual equivalente en `NPCDialogue`:

- `Enable Demo Route Hook`: `true`
- `Override Default Interaction With Demo Route`: `true`
- `Demo Route Interaction Id`: `npc_vendedora_mercado`

### Opcion B: FoodStand_Ceviche usa SimpleInteraction

Si el objeto se llama exactamente `FoodStand_Ceviche`, `SimpleInteraction` delega automaticamente en `DemoQuestRouteManager`.

Si necesitas otro objeto de puesto, agrega `DemoRouteInteractionPoint` y configura `Demo Route Interaction Id`: `foodstand_ceviche`.

## 5. Configurar NPC_Guia_Pacoche

Si el objeto se llama exactamente `NPC_Guia_Pacoche`, no necesita configuracion extra.

Configuracion manual equivalente en `NPCDialogue`:

- `Enable Demo Route Hook`: `true`
- `Override Default Interaction With Demo Route`: `true`
- `Demo Route Interaction Id`: `npc_guia_pacoche`

## 6. Configurar SecretEntrance_Rock

Si el objeto se llama exactamente `SecretEntrance_Rock`, no necesita configuracion extra.

Configuracion manual equivalente en `SecretZoneEntrance`:

- `Enable Demo Route Hook`: `true`
- `Override Default Scene Load With Demo Route`: `true`
- `Demo Route Interaction Id`: `secret_entrance_rock`

## 7. Configurar Trigger_SecretZone

Si el objeto se llama exactamente `Trigger_SecretZone` y usa `MessageTrigger`, no necesita configuracion extra.

Alternativa: agrega `DemoSecretZoneTrigger` a un objeto trigger (`Collider2D` con `Is Trigger`). Verifica que el player tenga tag `Player`.

## 8. Flujo esperado

1. Inicio: `Habla con el pescador en el malecón.`
2. Pescador tutorial: actualiza a `Ve al muelle y pesca algo.`
3. FishingSpot muelle: `Has conseguido pescado fresco.` y objetivo mercado.
4. Mercado: entrega concha spondylus y objetivo guia.
5. Guia: desbloquea pista de roca y objetivo costa.
6. Roca: revela entrada y objetivo explorar.
7. Trigger secreto: mensaje de hallazgo y `Fin de la demo.`

# Visual Prefab Guide

Estos prefabs visuales deberian crearse luego desde Unity cuando los sprites definitivos esten listos. No se crean desde codigo en esta preparacion.

| Prefab | Carpeta sugerida | Sprite base | Uso |
| --- | --- | --- | --- |
| PF_InteractionIcon | Assets/Prefabs/UI | ui_interaction_icon.png | Indicador visual sobre objetos o NPCs interactuables |
| PF_DialogBox | Assets/Prefabs/UI | ui_dialog_box.png | Caja base para dialogos |
| PF_CevicheStand | Assets/Prefabs/Visual | spr_ceviche_stand.png | Puesto de ceviche en mercado |
| PF_DockCrate | Assets/Prefabs/Visual | spr_dock_crate.png | Caja decorativa o de bloqueo suave en muelle |
| PF_FishingNet | Assets/Prefabs/Visual | spr_fishing_net.png | Red de pesca decorativa o punto visual de pesca |
| PF_PacocheBush | Assets/Prefabs/Visual | spr_pacoche_bush.png | Vegetacion de Pacoche |
| PF_SecretZoneEntrance | Assets/Prefabs/Visual | spr_secret_zone_entrance.png | Entrada visual a zona secreta |

## Reglas para crearlos luego en Unity

- Crear prefabs desde GameObjects con `SpriteRenderer` o componentes UI segun corresponda.
- Mantener nombres exactos con prefijo `PF_`.
- Asignar sprites desde las carpetas `Assets/Art/Sprites/...`.
- No agregar scripts de gameplay a estos prefabs visuales salvo que se coordine antes.
- Revisar orden de sorting layer para que no tapen personajes o UI.
- Para UI, validar escala en Canvas antes de convertir a prefab.

## Pendiente manual

Cuando Unity este abierto, importar los sprites finales, aplicar la configuracion de pixel art y crear los prefabs desde el Inspector.

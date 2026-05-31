# AGENTS.md

## Proyecto

Este es un videojuego 2D en Unity con estilo pixel art.

Meta inicial:
- Construir una demo jugable de 3 a 5 minutos.
- Priorizar aprendizaje, claridad y avance progresivo.
- Mantener una estructura simple antes de agregar sistemas grandes.

## Reglas generales para Codex

- No modificar archivos fuera de `Assets/_Project` salvo que se pida explicitamente.
- No borrar, renombrar ni mover assets sin pedir confirmacion.
- No eliminar archivos `.meta` de Unity.
- No instalar paquetes externos sin explicar el motivo.
- No introducir frameworks grandes sin necesidad.
- No crear sistemas complejos si una solucion simple resuelve la demo.
- Evitar cambios destructivos en escenas, prefabs o configuracion global.
- Explicar siempre que archivos se cambiaron y por que.
- Si una tarea requiere configuracion manual dentro de Unity, indicarlo claramente.

## Estructura del proyecto

- Arte: `Assets/_Project/Art`
- Audio: `Assets/_Project/Audio`
- Prefabs: `Assets/_Project/Prefabs`
- Escenas: `Assets/_Project/Scenes`
- Scripts: `Assets/_Project/Scripts`
- Configuracion propia: `Assets/_Project/Settings`
- Tests: `Assets/_Project/Tests`

## Estilo de codigo C# para Unity

- Usar nombres claros y consistentes.
- No mezclar demasiadas responsabilidades en un solo `MonoBehaviour`.
- Evitar que `PlayerController` controle todo el juego.
- Separar sistemas de:
  - movimiento
  - input
  - camara
  - combate
  - interaccion
  - UI
  - estados globales
- Preferir referencias por Inspector antes que busquedas globales.
- Evitar `FindObjectOfType`, `GameObject.Find` o busquedas costosas durante gameplay.
- Usar `Update` para lectura de input y logica por frame.
- Usar `FixedUpdate` para fisica cuando corresponda.
- Documentar brevemente los scripts importantes.
- Mantener el codigo entendible para alguien que esta aprendiendo Unity.

## Reglas de pixel art

- Mantener sprites nitidos.
- Recomendar `Point Filter` cuando corresponda.
- Evitar compresion agresiva en sprites importantes.
- Mantener consistencia en `Pixels Per Unit`.
- Usar camara Pixel Perfect si el proyecto lo requiere.
- Separar sprites de personaje, tilesets, UI y efectos.

## Escenas recomendadas

- `Boot.unity`: carga inicial y configuracion global.
- `MainMenu.unity`: menu principal.
- `DemoLevel_01.unity`: primer nivel jugable.

No crear escenas si Unity no esta disponible desde el entorno actual. En ese caso, solo documentar los nombres esperados.

## Paquetes recomendados de Unity

Sugeridos para la demo:
- 2D Sprite
- 2D Pixel Perfect
- Cinemachine
- Input System
- Unity Test Framework
- Aseprite Importer, solo si se usara Aseprite
- 2D Tilemap Extras, solo si se trabajara con mapas por tiles

No instalar paquetes automaticamente si el entorno no permite verificar Unity Package Manager.

## Antes de terminar una tarea

Codex debe responder con:

1. Resumen de cambios realizados.
2. Archivos creados o modificados.
3. Configuracion manual pendiente dentro de Unity.
4. Riesgos o puntos a revisar.
5. Siguiente paso recomendado.

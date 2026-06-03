# Sprite Import Guide

Esta guia resume la configuracion recomendada para importar sprites pixel art en Unity sin perder nitidez.

## Configuracion base

| Opcion | Valor recomendado |
| --- | --- |
| Texture Type | Sprite (2D and UI) |
| Sprite Mode | Single o Multiple segun corresponda |
| Pixels Per Unit | 16 o 32 |
| Filter Mode | Point (no filter) |
| Compression | None |
| Generate Mip Maps | Off |
| Mesh Type | Full Rect |

## Cuando usar Single o Multiple

- Usar `Single` para sprites individuales como items, iconos, cajas, puestos o props sueltos.
- Usar `Multiple` para spritesheets de personajes, NPCs, animaciones, tilesets o UI con varios estados.

## Pixels Per Unit

- Usar `32` para personajes y NPCs de 32x32 px cuando una unidad de Unity represente el alto base del personaje.
- Usar `16` para items pequenos o tiles si el mapa fue construido alrededor de tiles de 16 px.
- Mantener el mismo criterio por categoria para evitar escalas inconsistentes.

## Nitidez de pixel art

- `Filter Mode` debe estar en `Point (no filter)`.
- `Compression` debe estar en `None` para sprites importantes.
- `Generate Mip Maps` debe estar apagado.
- Revisar que la camara o Pixel Perfect Camera mantenga los pixeles alineados si se activa en el proyecto.

## Herramienta de Editor

Para aplicar la configuracion base rapidamente:

1. Seleccionar una o varias texturas en el Project Window.
2. Ir a `Tools > Art > Apply Pixel Art Import Settings To Selected Textures`.
3. La herramienta configura solo las texturas seleccionadas como sprites pixel art `Single` con `Pixels Per Unit` en `16`.
4. Revisar manualmente si algun spritesheet necesita `Sprite Mode: Multiple` despues de aplicar la herramienta.

## Slicing de spritesheets

Para spritesheets:

1. Cambiar `Sprite Mode` a `Multiple`.
2. Abrir `Sprite Editor`.
3. Usar slicing por grilla si todos los frames tienen el mismo tamano.
4. Verificar pivots consistentes, normalmente `Center` o `Bottom Center` para personajes top-down.
5. Nombrar frames con una secuencia clara, por ejemplo `spr_fisher_idle_01`, `spr_fisher_idle_02`.

No crear animaciones desde codigo. Las animaciones deben configurarse luego dentro de Unity si hacen falta.

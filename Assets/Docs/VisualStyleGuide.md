# Visual Style Guide

## Direccion visual

El juego usa pixel art 2D top-down con lectura clara, bordes definidos y formas simples. La ambientacion toma inspiracion de Manta, Ecuador: costa, playa, mercado local, muelle pesquero, comida de mar, vegetacion de Pacoche y una zona secreta con un tono mas misterioso.

La prioridad visual de la demo es que cada sprite se entienda rapido en pantalla y mantenga coherencia con el resto del mapa.

## Paleta sugerida

| Zona | Colores sugeridos | Uso |
| --- | --- | --- |
| Arena | #E7C77D, #D7AD5F, #A87B3E | Playas, caminos claros, bases de puestos |
| Mar | #1E88A8, #2FB6C6, #0F5D7A | Agua, borde costero, detalles frescos |
| Mercado | #D94F3D, #F2B84B, #2E8B57 | Toldos, frutas, puestos y acentos vivos |
| Muelle | #7A5230, #4F3828, #B07A42 | Tablas, cajas, redes y estructuras |
| Pacoche | #2F6B3F, #5E9F57, #183A2A | Arbustos, hojas, bosque y sombra natural |
| Zona secreta | #3F2A56, #2A5A68, #77C6B6 | Entrada, brillo raro, elementos especiales |

Evitar que todo el juego dependa de un solo color dominante. Cada zona debe compartir el estilo, pero tener identidad propia.

## Tamanos recomendados

| Elemento | Tamano |
| --- | --- |
| Personajes | 32x32 px |
| NPCs | 32x32 px |
| Items | 16x16 o 32x32 px |
| Objetos medianos | 32x32 o 48x48 px |
| Puestos | 64x64 o 96x64 px |
| UI iconos | 16x16 o 32x32 px |
| UI caja de dialogo | 192x64 px |

Mantener una escala consistente. Si se usa 32 Pixels Per Unit para personajes, revisar que items y tiles no se vean demasiado grandes o pequenos dentro de la escena.

## Contorno y sombra

- Usar contornos de 1 px en personajes, NPCs e items importantes.
- Reservar contornos mas oscuros para separar sprites del fondo.
- Usar sombras simples bajo personajes y objetos, preferiblemente elipses o manchas de 1 a 2 tonos.
- Evitar degradados suaves. Usar bloques de color y dithering solo cuando ayude a la lectura.
- Mantener la fuente de luz coherente, sugerida desde arriba-izquierda.
- Evitar detalles pequenos que desaparezcan al jugar en resoluciones bajas.

## Coherencia visual

- Los personajes y NPCs deben compartir proporcion base: cabeza clara, cuerpo compacto y silueta legible.
- Los items deben tener alto contraste y silueta reconocible.
- Los elementos de mercado pueden tener colores mas vivos, pero no deben competir con UI o objetivos activos.
- Pacoche debe sentirse mas organico: verdes variados, bordes irregulares y sombras naturales.
- La zona secreta puede usar colores menos realistas, pero debe seguir siendo pixel art nitido.

## Convencion de nombres

Usar nombres en minuscula con prefijo por tipo:

- `spr_` para sprites: `spr_fisher_idle.png`
- `ui_` para elementos de interfaz: `ui_dialog_box.png`
- `tile_` para tiles: `tile_sand_01.png`
- `fx_` para efectos visuales: `fx_secret_glow.png`
- `pf_` o `PF_` para prefabs segun convencion de Unity: `PF_CevicheStand`

Para animaciones, usar formato:

- `spr_player_idle_down_01.png`
- `spr_player_walk_down_01.png`
- `spr_fisher_idle_01.png`

Mantener nombres descriptivos y evitar espacios, acentos o caracteres especiales.

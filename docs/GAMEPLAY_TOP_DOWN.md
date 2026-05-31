# GAMEPLAY_TOP_DOWN.md

## Objetivo general

Construir el sistema base de gameplay para un videojuego 2D top-down en Unity, inspirado en la vista clásica de The Legend of Zelda de Game Boy Advance.

El juego debe construirse por módulos, de forma limpia y escalable, empezando por una demo pequeña. No implementar todo de golpe. El objetivo inicial es tener una base jugable con:

- Movimiento del jugador en 4 direcciones.
- Animaciones básicas del jugador.
- Cámara siguiendo al jugador.
- Colisiones con paredes/obstáculos.
- Sistema inicial de enemigos.
- Sistema de vida/daño.
- Ataque básico del jugador.
- Preparación para futuro jefe/boss.
- Arquitectura sencilla, entendible y ampliable.

## Tipo de proyecto

Unity 2D.

Estilo de juego:
- Vista superior/top-down.
- Movimiento arriba, abajo, izquierda y derecha.
- No usar físicas complejas innecesarias.
- Pensado para pixel art.
- Pensado para una demo corta de 3 a 5 minutos.

## Reglas importantes para Codex

1. No crear sistemas gigantes de golpe.
2. No sobreingenierizar.
3. Priorizar código simple y educativo.
4. Separar responsabilidades en scripts distintos.
5. Cada script debe tener comentarios claros.
6. Evitar dependencias externas innecesarias.
7. Usar nombres en español o nombres claros, pero mantener consistencia.
8. Antes de modificar mucho código, explicar brevemente qué archivos se crearán o tocarán.
9. Si hay varias formas de hacerlo, elegir la más simple para una demo inicial.
10. El código debe funcionar en Unity 2D con C#.

## Estructura sugerida de carpetas

Crear o respetar esta estructura dentro de `Assets`:

```txt
Assets/
  Art/
    Characters/
    Enemies/
    Bosses/
    Tilesets/
  Animations/
    Player/
    Enemies/
    Bosses/
  Audio/
    Music/
    SFX/
  Materials/
  Prefabs/
    Player/
    Enemies/
    Bosses/
    Gameplay/
  Scenes/
  Scripts/
    Core/
    Player/
    Enemies/
    Bosses/
    Combat/
    UI/
    Camera/
  ScriptableObjects/
  Tilemaps/
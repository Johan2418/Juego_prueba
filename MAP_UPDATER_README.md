# Map Design Updater - Instrucciones de Uso

## Descripción
El script `MapDesignUpdater.cs` reemplaza automáticamente los sprites del mapa con los nuevos sprites de arena y agua del pack `Cute_Fantasy_Free` y guarda la escena.

## Sprites que se utilizarán:
- **Agua (azul)** → `Water_Tile.png` (Cute_Fantasy_Free/Tiles)
- **Arena (amarillo)** → `Beach_Tile.png` (Cute_Fantasy_Free/Tiles)

## Tilemaps que se actualizarán:
1. `Water_Tilemap` - Se pintará con Water_Tile
2. `Ground_Tilemap` - Se pintará con Beach_Tile
3. `Decoration_Tilemap` - Se pintará con Beach_Tile
4. `Paths_Tilemap` - Se pintará con Beach_Tile

## Cómo usar:

1. **Abre la escena** `Map_Manta_Prototype.unity` en Unity
2. **Ve al menú** → `Map` → `Update Map Design with Sprites`
3. El script ejecutará automáticamente:
   - Cargará los sprites desde `Assets/Sprites/Cute_Fantasy_Free/Tiles/`
   - Creará Tile assets en `Assets/Tiles/`
   - Pintará los tilemaps detectados en la escena
   - Guardará la escena actual
   - Mostrará un diálogo confirmando que se completó

## Resultado esperado:
- Las áreas azules se mostrarán con el sprite `Water_Tile.png`
- Las áreas amarillas se mostrarán con el sprite `Beach_Tile.png`
- El mapa tendrá un aspecto más pulido con los nuevos tiles

## Nota:
- El script vuelve a modificar y guardar la escena, así que úsalo solo cuando quieras aplicar el cambio de tiles al mapa activo

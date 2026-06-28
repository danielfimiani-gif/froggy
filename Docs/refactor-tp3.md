# TP3 — Documento de Refactor

## Contexto

El TP2 implementó un Frogger con arquitectura **MVC** en Unity. Para el TP3 (Unit Testing)
hubo que hacer ajustes para poder testear el **comportamiento** de forma aislada, sin
levantar el motor de Unity.

## ¿Hizo falta un refactor grande?

**Sí**, pero acotado. La buena noticia es que los `Model` del TP2 ya eran **clases C# puras**
(no heredan de `MonoBehaviour`), así que la lógica central ya estaba mayormente desacoplada.
El refactor consistió en **liberar la lógica que había quedado atrapada dentro de un
`MonoBehaviour`** y en separar los assemblies.

## Refactors realizados

### 1. Assembly Definitions (separación vista/comportamiento)

- **Qué:** se agregaron `.asmdef` (`Scripts`, `EditMode`, `PlayMode`).
- **Por qué:** el Unity Test Framework necesita un assembly de tests que referencie al
  código de producción. Sin `.asmdef`, todo compila en `Assembly-CSharp` y no se puede
  referenciar limpio desde los tests.
- **Costo:** al aislar `Scripts`, hubo que referenciar explícitamente `Unity.InputSystem`
  y mover `LaneConfig` dentro del paraguas del assembly.

### 2. Extracción de la lógica de colisión → `CollisionDetector`

- **Qué:** la regla de colisión estaba en `GameController.CheckCollisions()` (un `MonoBehaviour`).
  Se extrajo a una función pura.
- **Por qué:** un `MonoBehaviour` no se puede instanciar con `new` en EditMode. La regla
  central del juego ("si un auto toca al jugador, vuelve al inicio") no era testeable.
- **Beneficio:** ahora es una función pura, testeable con casos (auto a la izquierda /
  encima / a la derecha) sin Unity.

### 3. Extracción de la simulación → `GameWorld`

- **Qué:** la orquestación del frame (mover lanes + detectar colisión + Die/Respawn) estaba
  en `GameController.Update()`. Se movió a una clase pura `GameWorld.Tick(dt)`.
- **Por qué:** mismo motivo que arriba — estaba atrapada en el ciclo de vida de Unity.
- **Beneficio:** se puede simular el juego paso a paso en un test de EditMode.

### 4. Visibilidad `public` en los tipos de dominio

- **Qué:** los tipos de comportamiento eran `internal` (default). Se hicieron `public`.
- **Por qué:** el assembly `EditMode` no ve los `internal` de `Scripts`. Para hacer
  `new PlayerModel(...)` en un test, deben ser `public`.

## Lo que NO cambió (y por qué)

- Los `Model` (`PlayerModel`, `CarModel`, `LaneModel`, `GameStateModel`) ya eran puros:
  se testean tal cual.
- `GameController` quedó como un `MonoBehaviour` **fino**: solo cablea dependencias y delega
  en `GameWorld`. La carga de escena al ganar sigue en Unity (no es comportamiento testeable).
- La capa de **vista** (`PlayerView`, `CarView`, `LaneView`, `PlayerAudioView`, `CameraFollow`)
  no cambió: se cubre con tests de PlayMode.

## Conclusión

El refactor fue **quirúrgico**: no se reescribió el juego, se movieron dos piezas de
comportamiento (`colisión` y `simulación`) desde un `MonoBehaviour` a clases puras, y se
estructuraron los assemblies. El MVC del TP2 hizo que el costo fuera bajo.

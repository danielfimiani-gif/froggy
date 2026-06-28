# TP3 — Documento de Refactor

## Contexto

El TP2 implementó un Frogger con arquitectura **MVC** en Unity. Para el TP3 (Unit Testing)
hubo que hacer ajustes para poder testear el **comportamiento** de forma aislada, sin
levantar el motor de Unity.

## Refators Realizados

- CollisionDetector : Se creó la clase C# pura con el método Overlap bool para podes testear el método.
- PlayerModel se cambió su accesibilidad de internal a public para poder instanciarlo en los test.
- CarModel se cambió su accesibilidad de internal a public para poder instanciarlo en los test.



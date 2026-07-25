# Examen Final — Integración Continua con GitHub Actions

Este documento explica el pipeline de CI del proyecto: qué hace, cómo está armado
y **por qué** cada decisión es la que es.

El archivo del pipeline es [`.github/workflows/main.yml`](../.github/workflows/main.yml).

## Correcciones de los TPs 2 y 3

La consigna pide aplicar las correcciones de los trabajos prácticos anteriores.
**No hubo correcciones que aplicar**: ambos TPs fueron aprobados con nota 10, sin
observaciones pendientes.

Sí se corrigió un bug que apareció recién al montar el CI, explicado más abajo en
[Un bug que encontró el CI](#un-bug-que-encontró-el-ci).

## Qué hace el pipeline

En cada `push` y en cada `pull_request`:

```
                    ┌─ Test editmode ──┐
    push / PR ──────┤                  ├──┬─ Build WebGL ─────────────┐
                    └─ Test playmode ──┘  └─ Build StandaloneWindows64┴─ artifacts
```

| Job | Qué hace | Requisito |
|-----|----------|-----------|
| `Test editmode` | Corre los unit tests de EditMode + reporte de cobertura | Extra (2.5 pts) |
| `Test playmode` | Corre los unit tests de PlayMode + reporte de cobertura | Extra (2.5 pts) |
| `Build for WebGL` | Compila el juego a WebGL | **Obligatorio** |
| `Build for StandaloneWindows64` | Compila el juego a Windows 64 bits | Extra (1.5 pts) |

Los dos jobs de test corren en paralelo entre sí. Los dos builds también. Los builds
esperan a los tests (`needs: test`).

Todo queda publicado como *artifacts* descargables desde la pestaña Actions: los dos
builds, los resultados de test en XML de NUnit y los reportes HTML de cobertura.

## Decisiones técnicas

### El cache de `Library/` va por plataforma

Es el punto más importante del pipeline y el más fácil de hacer mal.

Unity guarda en `Library/` los assets **ya importados y compilados para el target
activo**. No es un cache genérico del proyecto: el mismo sprite importado para WebGL
y para Windows produce artefactos distintos.

Si los dos builds comparten una sola entrada de cache, cada uno le pisa los artefactos
al otro y ambos terminan reimportando todo en cada corrida. Se paga el costo de bajar
y subir ~1 GB para no ahorrar nada. Por eso la clave incluye la plataforma:

```yaml
key: Library-${{ matrix.targetPlatform }}-${{ hashFiles('Assets/**', 'Packages/**', 'ProjectSettings/**') }}
restore-keys: |
  Library-${{ matrix.targetPlatform }}-
```

El `hashFiles(...)` hace que el cache se invalide **solo cuando el proyecto cambia de
verdad**. Sin él, con una clave fija como `Library-WebGL`, el cache nunca se renueva y
se restauran artefactos viejos que ya no corresponden al proyecto.

El `restore-keys` da un *fallback parcial*: si no existe una entrada con ese hash exacto,
se usa la más reciente de la misma plataforma y Unity reimporta solo lo que cambió.

**Ese `restore-keys` no debe incluir el prefijo genérico `Library-`.** Ese prefijo
matchea *cualquier* entrada, incluida la de los tests, y el build termina restaurando
una `Library/` de otro target: reimporta todo igual y encima pagó la descarga. Se
verificó en una corrida real, donde el build de WebGL hizo `Cache hit for restore-key:
Library-test-...` y tardó lo mismo que sin cache.

Los tests tienen su propia entrada (`Library-test-`) porque el Test Runner corre sobre
el target del editor (Linux), no sobre WebGL ni Windows.

#### Los caches tienen scope por rama

Un detalle que confunde: **una rama no ve los caches creados en otra rama.** La
visibilidad va de la rama base hacia abajo — un branch hereda los caches de su base y
los de la rama por defecto, pero nunca los de una rama hermana.

Por eso la primera corrida en `master`, después de mergear, reportó `Cache not found`
aunque la clave y el hash eran idénticos a los que ya existían en el branch de trabajo:
estaban guardados bajo `refs/heads/ci/github-actions`, fuera de su alcance.

No es un problema a resolver, sino algo a tener en cuenta al medir: **la primera corrida
en una rama nueva siempre va sin cache**. Como `master` es la rama por defecto, sus
caches sí quedan disponibles para todo el repositorio de ahí en adelante.

### `actions/cache@v4`, no `v3`

GitHub retiró las versiones anteriores del action de cache. Un workflow con `v3` falla
con *"This request has been automatically failed because it uses a deprecated version"*.

### `versioning: None`

Por defecto, `unity-builder` usa versionado `Semantic`, que arma el número de versión
**leyendo los tags de git**. Este repositorio no tiene ningún tag, así que ese modo falla.
Con `None` el build no intenta versionar.

### `needs: test`

Los builds dependen de los tests. Si un test falla, no se publica ningún build.

Es la decisión correcta para un pipeline: no tiene sentido distribuir un artefacto que
no pasó sus propias pruebas. La contrapartida es que un test roto también corta la build.

### `concurrency`

Si se pushea dos veces seguidas, la corrida anterior se cancela. Un build de Unity tarda
más de veinte minutos; no tiene sentido dejar corriendo trabajo que ya quedó obsoleto.

### `Free disk space`

Los runners de GitHub traen unos 14 GB libres. La imagen de Unity más el build de WebGL
los agotan, y el job muere con *"no space left on device"*. El step borra herramientas
preinstaladas que este pipeline no usa (.NET, Android SDK, GHC, CodeQL).

### El build de WebGL y los servidores estáticos

Un build de WebGL **es una página web**: no se abre haciendo doble click en el
`index.html`, necesita servirse por HTTP. El pipeline lo publica como un `.zip` listo
para subir a itch.io o cualquier hosting estático.

Para que funcione fuera de un servidor configurado a medida hay dos ajustes en Player
Settings, y **la combinación importa**:

- **`webGLDecompressionFallback` activado.** Unity comprime los archivos con Brotli y
  espera que el servidor mande el header `Content-Encoding: br`. itch.io, GitHub Pages y
  la mayoría de los hostings estáticos no lo hacen, y el juego muere al cargar. Con el
  fallback, Unity embebe un descompresor en JavaScript y el build anda en cualquier
  servidor, manteniendo la compresión (y por lo tanto el tamaño chico). Los archivos
  pasan a tener extensión `.unityweb` en lugar de `.br`.

- **`webGLDataCaching` desactivado.** Es un bug de Unity 6: con *Data Caching* y
  *Decompression Fallback* **activados al mismo tiempo**, el loader llama a
  `cacheControl(undefined)` y el juego no arranca:

  ```
  Uncaught (in promise) TypeError: Cannot read properties of undefined (reading 'match')
      at Object.cacheControl (froggy.loader.js:1:952)
  ```

  Está reportado en el issue tracker de Unity y afecta a `6000.0.35f1` en adelante
  (este proyecto usa `6000.0.36f1`). No pasa en `6000.0.34f1`.

  De los dos, el que se sacrifica es el caching: el fallback es un **requisito** para
  que el build cargue en un hosting estático, mientras que el caching es solo una
  optimización de recargas (guarda el build en IndexedDB). Sin él, la primera carga es
  igual de rápida; solo las siguientes dejan de aprovechar el cache del navegador.

### Retención de artifacts

Los artifacts se publican con `retention-days: 90` (el máximo para repos públicos). Con
el valor por defecto de 14 días, los builds desaparecen antes de que alguien llegue a
descargarlos.

### Releases

Los artifacts viven dentro de cada corrida, en la pestaña Actions: para bajarlos hay que
entrar al run y buscarlos. Un **Release**, en cambio, queda en la portada del repositorio
y se descarga con un click.

El job `release` funciona en dos modos según qué haya disparado el workflow:

- **Push a `master`** → actualiza un release llamado `latest` con los últimos builds.
  Siempre hay algo descargable desde la portada, sin tener que acordarse de nada.
- **Push de un tag `v*`** → crea un release versionado que queda fijo:

  ```bash
  git tag v1.0.0
  git push origin v1.0.0
  ```

La convención habitual es que un release sea **solo** un hito versionado, y que los builds
de cada commit vivan como artifacts. Esa convención tiene sentido cuando hay usuarios
reales que necesitan saber en qué versión están y no pueden actualizar sin aviso.

En un proyecto como este —donde quien lo descarga entra una vez y quiere el ejecutable—
ese problema no existe, y obligar a taggear a mano solo agrega un paso que se puede
olvidar. Por eso `latest` se actualiza solo, y el versionado por tag queda disponible
para cuando haga falta.

| Mecanismo | Dónde se ve | Cuándo se genera |
|-----------|-------------|------------------|
| Artifacts | Pestaña Actions, dentro de cada run | En cada push (90 días) |
| Release `latest` | Portada del repositorio | En cada push a `master` |
| Release versionado | Portada del repositorio | Al pushear un tag `v*` |

## Un bug que encontró el CI

La primera corrida con licencia válida dejó los tests en verde y **los dos builds en rojo**:

```
Assets/_Project/Test/PlayMode/CarViewTests.cs(7,6): error CS0246:
The type or namespace name 'UnityTest' could not be found
```

La causa estaba en `PlayMode.asmdef`:

```json
"includePlatforms": [],
"defineConstraints": []
```

En un *assembly definition*, `includePlatforms: []` significa **todas** las plataformas,
no ninguna. Sin `defineConstraints`, ese assembly de tests se compilaba **dentro del build
del juego** — y en un player no existe `UnityEngine.TestTools`, porque el test framework
no se empaqueta. De ahí el `CS0246`.

La solución es el símbolo que Unity define únicamente al compilar incluyendo tests:

```json
"defineConstraints": ["UNITY_INCLUDE_TESTS"]
```

Es lo que Unity genera automáticamente al crear un Test Assembly desde el menú
(*Create > Testing > Tests Assembly Folder*). Al armar los `asmdef` a mano durante el TP3,
se omitió.

Lo interesante es que **el bug estaba latente**: los 36 tests siempre pasaron, porque el
Editor sí tiene el test framework, y hasta ese momento nunca se había hecho un build del
juego. Recién apareció cuando una máquina limpia compiló el proyecto desde cero. Es
exactamente para lo que sirve la integración continua.

## Licencia de Unity en el CI

El Editor de Unity corre dentro de un container Docker efímero y sin cuenta: hay que
activarlo en cada corrida. GameCI necesita tres secrets:

| Secret | Contenido |
|--------|-----------|
| `UNITY_LICENSE` | Contenido completo del archivo `.ulf` |
| `UNITY_EMAIL` | Email de la cuenta Unity |
| `UNITY_PASSWORD` | Contraseña de la cuenta Unity |

### Cómo obtener el `.ulf`

El procedimiento que documenta GameCI (subir un `.alf` a `license.unity3d.com/manual` y
descargar el `.ulf`) **ya no funciona para licencias Personal**: Unity discontinuó esa vía.
El truco de quitar el `style="display: none;"` con las DevTools tampoco sirve — la opción
vuelve a aparecer, pero el backend responde `401 Unauthorized` al enviarla.

Unity 6 tampoco genera un `.ulf` localmente: guarda la licencia como
`UnityEntitlementLicense.xml`, que **no sirve** como reemplazo. GameCI extrae el número de
serie parseando el campo `<DeveloperData Value="..."/>` del `.ulf`, y ese campo no existe
en el formato de entitlements.

La vía que sí funciona es el **cliente de licencias por línea de comandos** que instala
Unity Hub:

```bash
read -rsp "Unity password: " UP && echo && \
/opt/unityhub/UnityLicensingClient_V1/Unity.Licensing.Client \
  --activate-ulf \
  --username "<tu-email>" \
  --password "$UP"; unset UP
```

Sin `--serial` activa una licencia **Personal** (con `--serial` sería Pro). El archivo
queda en `~/.local/share/unity3d/Unity/Unity_lic.ulf` y se carga como secret sin copiarlo
a mano:

```bash
gh secret set UNITY_LICENSE < ~/.local/share/unity3d/Unity/Unity_lic.ulf
```

Para verificar que el archivo sirve, alcanza con confirmar que contiene la línea
`<DeveloperData Value="..."/>`.

### Renovación

Las licencias Personal expiran. Cuando el CI empiece a fallar en la activación, hay que
repetir esos dos comandos. No hace falta tocar el workflow.

## Referencias

- [GameCI — documentación](https://game.ci/docs/)
- [`game-ci/unity-builder`](https://github.com/game-ci/unity-builder)
- [`game-ci/unity-test-runner`](https://github.com/game-ci/unity-test-runner)
- [Unity — Manage your license through the command line](https://docs.unity3d.com/6000.4/Documentation/Manual/ManagingYourUnityLicense.html)

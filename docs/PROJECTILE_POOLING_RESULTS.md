# Projectile pooling experiment (P2-2) — results

Branch: `experiment/projectile-pooling` (not merged into `main`).

## What changed
- `ProjectilePool` (new): one `SimplePool` per projectile prefab. On `Get` it re-parents, resets position / rotation / scale and the Rigidbody2D velocity to the prefab's initial state.
- `ItemController.InstantiateProjectile` takes projectiles from the pool instead of `Instantiate`.
- `ItemBehaviour`: `InitializeValue` now resets every per-use flag (`_reachedPierceLimit`, `_weaponDestroyedAfterPierce`, `_cooldownWaitUntilProjectileDeath`, `_hitCooldown`). A virtual `OnSpawned()` replaces the one-time setup that used to be in `Start()`. `EndProcess` / `DestroyProjectilesNow` release to the pool (or `Destroy` an object that did not come from a pool) and always remove the projectile from its controller's list, so a recycled projectile can't be touched through a stale list entry.
- `Lava`, `Hammer`, `Flail`, `Sword` and `Book` behaviours: `Start()` moved into `OnSpawned()` and the state they mutate (gravity, radius, angle, collider, sprites) is restored.
- Editor/dev-only counters `ItemController.DebugSpawnCount`, `DebugCreateCount` and a `DebugUsePool` switch (A/B against the old `Instantiate` path).

## How it was measured (Unity 6000.5.2f1, editor, Windows)
An editor-only harness (not committed) entered Play mode in `GameScene` per scenario, made the player near-invulnerable, raised enemy spawn counts, and gave one weapon at max level (12 weapons) or all of them. It recorded frame time, GC allocated per frame, projectile counts and scale, kills and errors for 20 s per weapon (40 s with all weapons).

## Results
Real fire rates are low (5–146 shots in 20 s per weapon; about 6 shots/s with every weapon at max level), so per-frame numbers are identical within noise:

| | before | with pooling |
|---|---|---|
| frame time, avg (all 12 weapons together) | 1.73 ms | 1.70 ms |
| GC allocated per frame | 12.2 KB | 12.1 KB (the 12 KB is not from projectiles) |
| script errors across 13 scenarios | 0 | 0 |
| max projectile scale per weapon | matches | identical for every weapon (no `+=` growth) |
| objects created / shots (all weapons, 40 s) | 230 / 230 | 138 / 254 |

A/B micro-benchmark (300 shots in one frame, physics off, steady-state rounds):

| | spawn | whole spawn+release cycle | GC (2 frames) |
|---|---|---|---|
| `Instantiate` / `Destroy` | ~16 ms | ~27.7 ms | ~165 KB |
| pooled | ~11 ms | ~26 ms (noisy: 24.0 / 24.4 / 29.5) | ~101 KB |

So pooling is about 30% cheaper to spawn and produces about 40% less garbage per burst, but the whole spawn+release cost barely changes, because releasing is immediate (`SetActive(false)`) while `Destroy` is deferred. At normal fire rates this is far below what you can see.

## Verdict
Works for all 12 weapons with no regressions found, but it is not a visible performance win at current fire rates. Merge only if you expect hundreds of projectiles per second (large projectile-count upgrades), or want the lower GC churn.

## Found on the way (not changed here)
- `ShotgunHitSO`'s `ShockwaveEffect` root object is never released/destroyed. Observed in Play mode: the child object is deactivated almost immediately (well before the clip's `EndAnimation` event time of 0.32 s), and the root stays active and is never returned to the pool, even 1.5 s later. The original code (`SetActive(false)` in `HitEffectScript.EndAnimation`) did not clean the root up either, so this is not a regression. It leaves one active, empty root per shot (about 1 shot / 6 s). The cause is not investigated; it needs a look at the prefab's two `Animator`/`HitEffectScript` pairs.
- The pooled and original paths behave the same for that prefab (5 shots -> 5 objects created in both).

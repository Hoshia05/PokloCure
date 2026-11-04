# PokloCure — Fix Guide (agent handoff)

This guide lists known defects in this repo and says exactly how to fix each one.
It is written for a coding agent with **no prior context**. Work through the tasks in order.
Every task gives the file, the problem, the fix, and how to check it.

---

## 0. Read this first

### Project facts
- Unity **2022.3.2f1**, 2D, URP, new Input System, Cinemachine, A* Pathfinding Project (third-party).
- Target platform: **WebGL** (the game is played in a browser on itch.io).
- Game code lives only in `Assets/Scripts/`. **Do not edit** the third-party folders
  (`Assets/AstarPathfindingProject`, `Assets/ElvGames`, `Assets/TextMesh Pro`, `Assets/Brackeys`,
  `Assets/2D_*`, `Assets/Backyard - Free`, …).
- Scenes in the build: `TitleScreen`, `SelectScreen`, `GameScene`. `TestGameScene` is not in the build.
- Comments and UI strings are in Korean. Keep them, and don't translate UI strings.

### Key classes
| Class | File | Role |
|---|---|---|
| `GameManager` | `Assets/Scripts/GameManagers/Game Manager.cs` (the filename has a space) | Singleton that persists across scenes (DontDestroyOnLoad). Holds item lists and the selected character, and picks random items. |
| `StageManager` | `Assets/Scripts/GameManagers/StageManager.cs` | Singleton for one game scene: object pools, spawning, timer, pause, level-up, score. |
| `EnemySpawnManager` | `Assets/Scripts/GameManagers/EnemySpawnManager.cs` | Spawn-position helpers. Most of its spawn methods are dead code; only the static helpers and `CheckIfIsInBounds` are used. |
| `SoundFXManager` | `Assets/Scripts/GameManagers/SoundFXManager.cs` | Persistent singleton with a pool of 200 `AudioSource`s for sound effects, plus background music. |
| `PlayerScript` | `Assets/Scripts/PlayerScript.cs` | Player stats, HP, stamina, EXP and level-up, inventory. |
| `PlayerControl` | `Assets/Scripts/PlayerControl.cs` | Input wrapper. |
| `EnemyScript` / `BossScript` | `Assets/Scripts/EnemyScripts/EnemyBehaviourScripts/` | Enemy stats, damage, death, knockback, stun. **Pooled** (they are reused, not destroyed), except bosses. |
| `ItemController` / `ItemBehaviour` | `Assets/Scripts/CharacterScripts/CharacterItemSkillScripts/BaseScripts/` | Weapon or item logic and the projectile it spawns. |

### Hard rules
1. **You probably cannot run Unity.** You can't compile or playtest, so be careful:
   - Change C# only. Don't hand-edit `.unity`, `.prefab`, `.asset` or `.meta` files.
   - Don't rename or move files, classes, or **public/serialized fields**. Unity links scripts and
     inspector values by name and GUID, and renaming silently breaks scenes. Adding new fields is fine.
   - Don't change the signature of any public method that may be wired to a UI button
     (`*Button`, `SelectThisChoice`, `TakeButtonClick`, `DropButtonClick`, `PauseMenu`, `SoundMenu`,
     `Set*Volume`, `Select*Tiger`, `Move*`, `QuitGame`, `PullRandomItem`). Changing a return type from `void` to
     `bool` is fine for methods called **only from code**. Check with grep first.
   - Use only APIs available in Unity 2022.3 and C# 9.
2. `BossScript.cs` is saved in **CP949/EUC-KR**, not UTF-8. If you edit it, convert it to UTF-8 first
   (`iconv -f CP949 -t UTF-8`). Commit the conversion separately, then make the change.
   Every other `.cs` file is UTF-8; some start with a BOM, so keep it.
3. **One task per commit.** Use the message `fix(<area>): <what>` and reference the task ID (for example `[P0-2]`).
4. Don't commit build output, and don't rewrite git history.
5. When done, report each task as **done / skipped (why)**, and note anything that needs a playtest.

---

## P0 — Bugs players can hit (do these first)

### P0-1  A full sound pool breaks gameplay
**File:** `GameManagers/SoundFXManager.cs`

**Problem:** `GetAudioSourceObjectFromPool()` uses `_soundClipPool.First(...)`, which **throws**
when all 200 sources are busy. Callers play the sound *before* doing their real work, so the exception
cancels that work:
- `ItemController.InstantiateProjectile`: the weapon doesn't fire.
- `PlayerScript.TakeDamage`: the player takes no damage.
- `ExpItemScript.OnTriggerStay2D`: `Deactivate()` is skipped, so the EXP orb pays out again next tick.

There's a second bug: `DeactivateAudioSource` waits with `WaitForSeconds`, which uses scaled time. While the game
is paused (`Time.timeScale = 0`), sources are never released, so menu clicks fill the pool.

**Fix:**
```csharp
private AudioSource GetAudioSourceObjectFromPool()
{
    AudioSource audioSource = _soundClipPool.FirstOrDefault(x => x != null && !x.gameObject.activeSelf);
    if (audioSource == null)
        return null;               // pool exhausted: drop this sound
    audioSource.gameObject.SetActive(true);
    return audioSource;
}

IEnumerator DeactivateAudioSource(AudioSource audiosource, float clipLength)
{
    yield return new WaitForSecondsRealtime(clipLength);
    if (audiosource != null)
        audiosource.gameObject.SetActive(false);
}

public void PlaySoundFXClip(AudioClip clip, Transform spawnTransform, float volume)
{
    if (clip == null || spawnTransform == null)
        return;
    AudioSource audiosource = GetAudioSourceObjectFromPool();
    if (audiosource == null)
        return;
    // ...existing body unchanged...
}
```
Also fix the order in `FieldItemScripts/ExpItemScript.cs` `OnTriggerStay2D`: call `Deactivate()` right
after `GainEXP(...)` and **before** playing the sound. `GainEXP` itself stays first.

Optional: `PlayClickSound` passes volume `10f`, but volume is clamped to 0–1. Change it to `1f`.

**Check:** `grep -n "\.First(" GameManagers/SoundFXManager.cs` returns nothing.

---

### P0-2  An enemy can die more than once
**Files:** `EnemyScripts/EnemyBehaviourScripts/EnemyScript.cs`, `BossScript.cs`

**Problem:** `CheckDeath()` starts `KillEnemy()` on *every* hit that leaves HP ≤ 0. Several hits in the same
frame (or before the collider turns off) give double kills, score, EXP and drops. They also lower
`StageManager.CurrentEnemyCount` twice.

**Fix:** In `EnemyScript`, add `protected bool _isDying;` and change:
```csharp
// top of TakeDamage(...)
if (_isDying) return;

public virtual void CheckDeath()
{
    if (_currentHP <= 0 && !_isDying)   // _currentHP comes from P0-4. Until then use _currentMaxHP.
    {
        _isDying = true;
        StartCoroutine(KillEnemy());
    }
}
```
Do the same in `BossScript.CheckDeath()` (see Hard rule 2 about its encoding).
`_isDying` is reset in P0-3.

---

### P0-3  Recycled enemies keep state from their last life
**Files:** `EnemyScript.cs`, `GameManagers/StageManager.cs` (`SetEnemyActive`)

**Problem:** `KillEnemy()` calls `StopAllCoroutines()`. That kills `StunCoroutine` before it sets
`IsStunned = false`, and kills `HitAnimation` before it restores the colour. The enemy then comes back from
the pool still stunned (frozen at `FreezePosition` by `IdleState`) or tinted.

**Fix:** Add this to `EnemyScript`:
```csharp
public void ResetForSpawn()
{
    StopAllCoroutines();
    _isDying = false;
    IsStunned = false;
    IsPatternSpawn = false;
    DamageList.Clear();
    SpriteRenderer.color = Color.white;
    SpriteRenderer.enabled = true;
    _collider.enabled = true;
    _currentHP = _currentMaxHP;          // P0-4
}
```
In `StageManager.SetEnemyActive`, replace `enemyScript.InitializeWithSO(null);` and the manual
collider/sprite enabling with `enemy.SetActive(true); enemyScript.ResetForSpawn();`. Call `SetActive`
first, because coroutines can't run on an inactive object.

---

### P0-4  Enemy buffs heal live enemies and duplicate enemy weapons
**Files:** `EnemyScript.cs`, `BossScript.cs`

**Problem:**
- `_currentMaxHP` is used as **current HP**.
- `BuffEnemy()` (called on **every** pooled enemy, including active ones, every `BuffInterval` seconds)
  calls `InitializeWithSO`, which resets HP to full. That fully heals enemies mid-fight.
- `InitializeWithSO` creates `EnemyData.BasicWeaponController` each time it runs (pool creation, every
  buff, and every respawn before P0-3) and overwrites `_weaponSlot` with the clone, so the weapons pile up.
- `_enemyAnim` is never assigned, so setting `EnemyData.AnimatorController` throws a NullReferenceException.

**Fix:**
1. Add `protected float _currentHP;`. In `TakeDamage`, subtract from `_currentHP`. In `CheckDeath`
   (both classes), test `_currentHP`.
2. Split `InitializeWithSO` into:
   - **one-time setup** (runs only if `!_initialized`): store `_enemyData`, get `_enemyAnim = GetComponent<Animator>()`
     (it may be null, so null-check before use), and create the weapon **once** into a new field
     `_weaponInstance` without overwriting `_weaponSlot`. Set `_initialized = true`.
   - **`ApplyStats()`**: everything that depends on `_buffMultiplier` (speed, `_currentMaxHP`, body damage, EXP,
     ranged stats, sprite, scale, animator controller). It must **not** touch `_currentHP` or create anything.

   Keep the public method `InitializeWithSO(EnemyBase)` with the same signature; it now calls setup, then `ApplyStats()`,
   then sets `_currentHP = _currentMaxHP`.
3. `BuffEnemy()`:
   ```csharp
   public void BuffEnemy()
   {
       float oldMax = _currentMaxHP;
       _buffMultiplier += 0.35f;
       ApplyStats();
       if (gameObject.activeInHierarchy && oldMax > 0)
           _currentHP *= _currentMaxHP / oldMax;  // keep the HP ratio
       else
           _currentHP = _currentMaxHP;
   }
   ```
4. In `StageManager.BuffEnemy`, cache `EnemyScript` instead of calling `GetComponent` per enemy per buff.
   This becomes easy once P2-1 adds a pool of `EnemyScript`; if you skip P2, leave it as is.

---

### P0-5  The debug cheat is reachable in release builds
**File:** `PlayerControl.cs`

**Problem:** The input action `DebugButton` is bound to the `P` key (`PlayerInputScheme.inputactions`)
and calls `PlayerScript.Instance.IncreaseWeaponSlot()`, giving a free weapon slot.

**Fix:** Wrap the subscription in `Awake`:
```csharp
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        _inputActions.actions["DebugButton"].performed += context => DebugButton();
#endif
```
Don't edit the `.inputactions` file or the generated `PlayerInputScheme.cs`.

---

### P0-6  A treasure box soft-locks when there's nothing left to give
**File:** `UIScript/BoxItemUIScript.cs` (`PullRandomItem`)

**Problem:** If every item is maxed, `GetRandomItem` returns null, the loop breaks with 0 items,
and `_pulledItemDict.ElementAt(0)` throws while the game is paused, so the box UI is stuck.

**Fix:** After the loop:
```csharp
if (_pulledItemDict.Count == 0)
{
    StageManager.Instance.GainCoins(50);   // consolation reward; the amount is a placeholder
    StageManager.Instance.EndBoxUI();
    return;
}
```
Also look at `UIScript/LvlUPListScript.cs` `CreateLvlUpList`. With 0 choices, the level-up screen shows no
cards. Make sure the player can still continue: if `_itemList.Count == 0`, call
`StageManager.Instance.GainCoins(...)` and then `PlayerScript.Instance.CheckLevelUp()`, which closes the screen.
Note in your report that this needs a playtest.

---

### P0-7  Spawn loops can hang the browser tab
**Files:** `GameManagers/StageManager.cs` (`MTESpawn`), `GameManagers/EnemySpawnManager.cs`
(`SpawnBossEnemy`, `ClusterSpawn`)

**Problem:** `while (!CheckIfIsInBounds(pos)) pos = random…;` has no attempt limit. If the player is in a
corner and the 60–80 unit ring lies mostly outside the map, this loops forever and freezes the page.

**Fix:** Add a helper to `EnemySpawnManager`:
```csharp
public bool TryGetPositionInPlayerRing(float outer, float inner, out Vector2 pos, int maxAttempts = 20)
{
    for (int i = 0; i < maxAttempts; i++)
    {
        pos = GetRandomPositionInPlayerRadius(outer, inner);
        if (CheckIfIsInBounds(pos)) return true;
    }
    pos = default;
    return false;
}
```
Replace each unbounded `while` with `if (!TryGetPositionInPlayerRing(...)) return;`.

---

### P0-8  Pierce runs out from contact alone
**Files:** `EnemyScript.cs` (`TakeDamage`), `BaseScripts/ItemBehaviour.cs` (`EnemyHitCoroutine`)

**Problem:** `OnTriggerStay2D` runs every physics tick while a weapon touches an enemy. Each tick
calls `CheckPierce()` (and rolls crit and stun), even when `TakeDamage` ignored the hit because of
`DamageList` (the hit cooldown). Weapons with `_hitCooldown > 0` use up pierce in a few frames.

**Fix:**
1. Change `EnemyScript.TakeDamage` to return `bool`: `false` when ignored (dying, or the item is on hit cooldown),
   `true` when it hit. Only code calls it (`ItemBehaviour`, `ShotgunHitController`), so this is safe.
2. In `ItemBehaviour.EnemyHitCoroutine`:
   ```csharp
   EnemyScript script = collision.GetComponent<EnemyScript>();
   if (script == null) yield break;
   bool crit = PlayerScript.Instance.CriticalCheck();
   float dmg = crit ? PlayerScript.Instance.GetCritDamage(_damage) : _damage;
   bool hit = script.TakeDamage(dmg, _knockback, crit, _itemName, _hitCooldown);
   if (!hit) yield break;
   if (_stunTime > 0) script.StunEnemy(_stunTime);
   CheckPierce();
   ```
   Remove the trailing `yield return new WaitForSeconds(_hitCooldown);`, which does nothing.
   Converting this to a plain method is also fine.

---

## P1 — Logic errors

Each of these is small. Fix exactly what's described.

| ID | File · method | Problem | Fix |
|---|---|---|---|
| P1-1 | `StageManager.EnemyBuffEvent` | `(int)((NextDouble()*20) % (Count - 1))` can never pick the last entry, so `ShortenInterval` never runs. | `int randNum = GameManager.Instance.Rand.Next(_enemyEnhanceFuncitons.Count);` |
| P1-2 | `StageManager.ShortenInterval` | `_mteSpawnInvterval == 1` compares floats exactly; the intervals can drop below 1. | `if (_mteSpawnInvterval <= 1f) { _mteSpawnInvterval = 1f; _circleSpawnInterval = Mathf.Max(1f, _circleSpawnInterval); _enemyEnhanceFuncitons.Remove(ShortenInterval); }` |
| P1-3 | `StageManager.UpdateTimer` | `_currentTime % BuffInterval < 0.3`, checked about every 0.3 s, fires 0 or 2 times per interval. | Add the field `private float _nextBuffTime;` and set it to `BuffInterval` in `Start`. In `UpdateTimer`: `if (_currentTime >= _nextBuffTime) { _nextBuffTime += BuffInterval; EnemyBuffEvent(); }` |
| P1-4 | `StageManager.RandomizeEnemyCircleSpawn` | `Rand.Next(1,10)` returns 1–9, so it always returns SWARM; the last line is unreachable. | Keep the current behaviour and simplify it to `return EnemyClass.SWARM;` with a comment. Don't change balance without asking. |
| P1-5 | `EnemySpawnManager.GetRandomPositionInRadius` (used by `MTESpawn`) | The angle `360 / (NextDouble() % 360f)` is meaningless and is ∞ when `NextDouble()` returns 0, giving a NaN position. The local `playerPosition` is unused. | `float a = Random.Range(0f, Mathf.PI * 2f); return targetPosition + new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * radius;` |
| P1-6 | `GameManager.GetFromList` | (a) `Except(exemptList)` throws if it's null (the default parameter). (b) The total weight ignores the ×2 bonus for the character's own items, and `Next(1, Total)` skips the top value, so the odds are biased by list order. | Treat a null `exemptList` as empty. Build `List<(ItemSO item, int w)>` with the bonus applied, `int roll = Rand.Next(0, sum);`, then walk the list with `if (roll < w) return item; roll -= w;`. Return null if `sum <= 0`. |
| P1-7 | `GameManager.GetFullList` | Shuffles `ItemList` but returns `DebugItemList` in debug mode. | Shuffle the list that is returned. |
| P1-8 | `PlayerScript.StaminaReplenish` | While stamina ≤ 0, the `else if` starts a new `StaminaReplenishCoroutine` **every frame** for 1.5 s. | `else if (_currentStamina <= 0 && !_redlineCharging)` |
| P1-9 | `PlayerScript.HealStamina` | No upper clamp. | `_currentStamina = Mathf.Min(_currentStamina + HealValue, _currentMaxStamina);` |
| P1-10 | `PlayerScript.CheckFull` | `_currentCharacterMaxHP >= _characterCurrentHP` is always true, so the HP bar hides even when hurt. | Reverse the comparison: `_characterCurrentHP >= _currentCharacterMaxHP`. Also stop an already running `_HPBarDisappearCoroutine` before starting a new one. |
| P1-11 | `PlayerScript.TakeHit` | `damage * pct - flat` can be negative, which heals the player. | `float ActualDamage = Mathf.Max(0f, damage * _currentDamageReducePercentage - _currentDamageReduceValue);` Optionally use a minimum of 1. |
| P1-12 | `EnemyScript.DamageVariance` | ±2 on small hits can give ≤ 0 damage. | `return Mathf.Max(1f, ItemController.RoundValue(...));` |
| P1-13 | `PlayerScript.CheckLevelUp` | The `else` branch calls `StageManager.LevelUpEventEnd()` on **every EXP pickup**. That unpauses the game (`Time.timeScale = 1`), hides the UI and clears `_pauseEvent` even when no level-up is open. | Add `public bool IsLevelUpOpen => _levelUPUI.activeSelf;` to `StageManager`. In the `else`: `if (StageManager.Instance.IsLevelUpOpen) StageManager.Instance.LevelUpEventEnd();` |
| P1-14 | `PlayerScript` EXP start | `_expRequirement = 79` and `ExperienceBar.InitializeEXP(100)` don't match the formula (level 2→3 needs about 40, less than level 1→2). | In `Awake`, set `_expRequirement = MathRelated.GetNextExpRequirement(_level);` and in `Start` call `ExperienceBar.Instance.InitializeEXP(_expRequirement)`. **This changes early-game pacing, so flag it in the report.** |
| P1-15 | `GameManager.Awake`, `PlayerControl.Awake` | They `Destroy(gameObject)` a duplicate but don't `return`, so setup keeps running. In `PlayerControl`, check which object it's attached to before changing anything. | Add `return;` after `Destroy(gameObject);`, as `SoundFXManager` already does. |
| P1-16 | `SoundMixerManager.Set*Volume` | `Mathf.Log10(0)` = −∞ when a slider reaches 0. | `Mathf.Log10(Mathf.Max(level, 0.0001f)) * 20f`. Optionally save and load the three values with `PlayerPrefs` (load them in `Start`; **`AudioMixer.SetFloat` doesn't work in `Awake`**). |
| P1-17 | `PlayerScript.ApplyMovement` / `FixedUpdate` | Rigidbody velocity is set in `Update`, while the purely visual cursor is updated in `FixedUpdate`. | Swap them: call `CursorUpdate()` from `Update` and `ApplyMovement()` from `FixedUpdate`. Leave `ApplyFlip`, `EatItemRadius` and `StaminaReplenish` in `Update`. Note: `ApplyMovement` drains stamina with `Time.deltaTime`, which is correct in `FixedUpdate` too. |
| P1-18 | Static singletons | `Instance` is never cleared, so stale references remain after a scene change. | In `StageManager`, `PlayerScript`, `EnemySpawnManager` and `PlayerControl`, add `void OnDestroy() { if (Instance == this) Instance = null; }`. `ItemController.OnDestroy` uses `PlayerScript.Instance`, so null-check it there. |

---

## P2 — Performance (WebGL)

Do these after P0 and P1. They're bigger, so make **one commit per pool type**.

### P2-1  Free-list object pools
**File:** `StageManager.cs`

**Problem:** Every pool lookup does `list.Any(x => !x.activeSelf)` then `list.First(...)`, a linear scan
over 3,000–4,000 objects for every spawn, EXP drop, damage popup and hit effect. The sound pool has the same problem.

**Fix:** Add a small generic helper, `Assets/Scripts/Utilities/SimplePool.cs`:
```csharp
using System.Collections.Generic;
using UnityEngine;

public class SimplePool
{
    private readonly GameObject _prefab;
    private readonly Transform _parent;
    private readonly Stack<GameObject> _free = new();

    public SimplePool(GameObject prefab, Transform parent, int prewarm)
    {
        _prefab = prefab; _parent = parent;
        for (int i = 0; i < prewarm; i++) _free.Push(Create());
    }

    private GameObject Create()
    {
        var go = Object.Instantiate(_prefab, _parent);
        go.SetActive(false);
        go.AddComponent<PooledObject>().Pool = this;
        return go;
    }

    public GameObject Get() => _free.Count > 0 ? _free.Pop() : Create();

    public void Release(GameObject go)
    {
        go.SetActive(false);
        _free.Push(go);
    }
}

public class PooledObject : MonoBehaviour
{
    public SimplePool Pool;
    public void Release() => Pool.Release(gameObject);
}
```
- Every place that currently returns an object to a pool with `SetActive(false)` must call `Release` instead. Find them
  with grep: `ExpItemScript.Deactivate`, `StageManager.EnemyDeathEvent`, `StageManager.DeactivateTimer`, the
  diamond pickup script (`DiamondScript.cs`), and the hit effect (`HitEffectScript.cs`).
  Make sure nothing calls both `Release` and `SetActive(false)`; a double release puts the same object
  on the stack twice. Guard against it with a `bool InPool` flag on `PooledObject`.
- Enemy pools are keyed by `EnemyBase`. Keep a `Dictionary<EnemyBase, SimplePool>`, and keep the
  list of all created `EnemyScript`s per type for `BuffEnemy`.
- **Lower the prewarm counts**, since pools now grow on demand: about 300 swarm, 50 medium, 20 elite per type,
  500 EXP, 200 money, 50 popups, 50 hit effects. Keep the `ENEMYLIMIT` cap.
- Enemies created later must get the current buff level. Store a per-class buff multiplier in `StageManager`
  and apply it when a new instance is created.

### P2-2  Pool projectiles
`ItemController.InstantiateProjectile` and `ItemBehaviour.EndProcess` / `DestroyProjectilesNow` use
`Instantiate`/`Destroy` for every shot. Use a `Dictionary<GameObject prefab, SimplePool>` in a new
`ProjectilePool` singleton. On reuse, `ItemBehaviour.InitializeValue` must **reset** `transform.localScale`
to the prefab scale (it currently uses `+=`, which keeps growing), plus `_reachedPierceLimit`, flags and the
Rigidbody velocity. Projectiles created with `independent == false` are parented to the controller, so set the parent
on `Get`. **This is the riskiest task, so playtest every weapon.** If you can't verify it, skip it and report that.

### P2-3  Smaller per-frame costs
- `PlayerScript.EatItemRadius`: use `Physics2D.OverlapCircleNonAlloc` with a cached `Collider2D[64]` buffer
  and a LayerMask for field items (look up the layer used by the `FieldItem` prefabs). Cache `FieldItemBase` lookups.
- `Utilities/MyExtensions.Shuffle`: replace the crypto RNG with `GameManager.Instance.Rand` (Fisher–Yates
  using `Rand.Next(n + 1)`). The current version allocates on every call and **loops forever if the list has more than 255 items**.
- `FieldItemBase.TransformToPlayer`: it calls `StopCoroutine` every frame. Stop once, when `PickedByPlayer` is called.

---

## P3 — Repo hygiene (no gameplay change)

| ID | Task |
|---|---|
| P3-1 | Remove the unused imports `using UnityEditor;` from `PlayerScript.cs`, `ItemSO.cs` and `FieldItemScripts/FoodItemScript.cs` (first confirm with grep that no `UnityEditor` types are used). Also remove `using Unity.VisualScripting;` (from `PlayerScript`, `SoundFXManager` and `BoxItemUIScript`), and `using UnityEngine.InputSystem.XR;` from `ItemBehaviour`. Editor code must be inside `#if UNITY_EDITOR` or under an `Editor/` folder. |
| P3-2 | Add to `.gitignore`: `/WebGLBuild/`, `/*.zip`, `/.plastic/`. Then `git rm -r --cached WebGLBuild "*.zip" .plastic ignore.conf` (the zip files are Git LFS pointers). **Don't** rewrite history. |
| P3-3 | Delete `Assets/Scenes/TestGameScene.unity` and its `.meta` (about 62 MB; not in the build settings). **Ask the owner first.** |
| P3-4 | Delete dead code: `EnemySpawnManager.CircleSpawn`, `ClusterSpawn`, `SpawnBossEnemy`, `PatternSpawn` (first grep that nothing, including prefabs or scenes, references them: `grep -rn "CircleSpawn\|ClusterSpawn\|SpawnBossEnemy\|PatternSpawn" Assets --include=*.unity --include=*.prefab --include=*.cs`), `StageManager.GetEnemyNum`, `PlayerControl.RandomizeMovement` and the empty `GameManager.GameOverEvent`. Keep the static helpers and `CheckIfIsInBounds`. |
| P3-5 | README: replace the Unity version TODO with `2022.3.2f1`. If P2 is not done, change the claim that *all* objects are pooled to say which ones are (enemies, EXP, coins, damage popups, hit effects, sounds). |
| P3-6 | Optional: add an EditMode test assembly `Assets/Tests/EditMode/` with tests for `MathRelated.GetNextExpRequirement` (increasing for levels 1–100) and the weighted pick from P1-6. The pick needs to be moved into a pure static function to be testable. |

**Leave these alone unless the owner asks:**
- `ItemSO.ControllerType` uses `Type.GetType(ControllerScript.name)`. It's fragile, but fixing it means
  re-wiring every item asset in the editor.
- Splitting the 62 MB `GameScene` tilemap into prefabs needs the Unity editor.
- Removing packages from `Packages/manifest.json` (Visual Scripting, Collab) needs a re-import in the editor.

---

## Final report template
```
| Task | Status | Commit | Notes / needs playtest |
|------|--------|--------|------------------------|
| P0-1 | done   | abc123 | — |
| P2-2 | skipped| —      | can't verify without Unity |
...
```
List every change that affects balance or feel (P1-4, P1-14, P0-6 reward amount, P2-1 pool sizes) so
the owner can playtest them.

# PA_IdleBerserker — HANDOFF cho Claude (tiếp tục từ Codex)

Playable ad dựa trên game gốc `/Users/toi/berserkeridle-mobile-client` (nguồn tham khảo/copy asset).
Repo làm việc: `/Users/toi/UnityProject/PA_IdleBerserker`. Unity `6000.0.80f1`, Spine 3.8, ProCamera2D và DOTween đã có.

## Quy ước

- Battle framework chủ yếu trong namespace `IdleBattle`; nhiều preset/companion port vẫn ở global namespace.
- PA không có đầy đủ AntiCheat/Facebook/PurpleCow/Odin/DataFieldBase/server data; bỏ hoặc dùng stub PA-native hiện có.
- Không tạo config trùng: gameplay companion và VFX timing hiện cùng nằm trong `SkillEffectPreset`.

## Đã hoàn thành

### Battle và CompanionSkill

- Battle framework: CharacterObject + Ability + FSM, PlayerObject, quái, boss, combo, dash, hit VFX/reaction, healthbar/damage number và variants.
- Có `CompanionSkill_0..38` (39 skill), base/runtime/spec/buff/stub cần thiết và 39 prefab tại `Assets/03_Prefab/Battle/Companion/`.
- Damage formula từng skill đã port theo gameplay gốc. Spec hỗ trợ value chính/phụ theo level, cooldown, duration, target và tick.
- Bottom skill bar có button, icon, cooldown radial/timer và click để kích hoạt (`CompanionSkillBar.cs`).
- `CompanionSkillManager` trên PlayerObject dùng `GameObject[] SkillPrefabs`, instantiate prefab thật, đọc preset từ `CompanionActiveSkill.SkillEffectPreset`, gọi `CreateCompanionSpec()` và cập nhật cooldown.
- PlayerObject hiện trang bị demo skill `0` và `10`.

### SkillEffectPreset là nguồn config duy nhất

`CompanionSkillPreset` riêng đã bị xóa. `Assets/02_Script/Effect/Preset/SkillEffectPreset.cs` hiện chứa cả:

- Animation/VFX/delay/position/shake/sound.
- `FieldId`, `Icon`.
- `Cooldown`, `Duration`.
- `Value1`, `Value1PerLevel`, `Value2`, `Value2PerLevel`.
- `TargetCount`, `TickInterval`, `TickCount`.
- `CreateCompanionSpec()`.

Đã cấu hình đủ 39 SO tại `Assets/12_Preset/CompanionPreset/CompanionSkill_0..38.asset`:

- 39/39 FieldId, icon và prefab reference đúng.
- Skill 8 trước đây dùng nhầm SO skill 9; nay có SO riêng.
- Skill 0 và 10 giữ thông số riêng.
- Các skill còn lại dùng baseline tạm: CD 8s, duration 3s, Value1 0.5 + 0.02/level, Value2 0.3 + 0.01/level, target 5, tick 1s × 3. Cần thay bằng spec/server table thật để balance chính xác.

Icon `companionSkill_0..38` nằm tại `Assets/04_Sprite/AtlasIcon_DonTouch/Companion/Skill/` và đã gắn đúng từng SO.

### SoundManager và sound companion

`Assets/_Battle/Preset/SoundManager.cs` đã port từ game gốc theo hướng PA/PlayWorks-compatible:

- Singleton persistent, pool 24 AudioSource cho SFX.
- BGM play/stop/resume/fixed BGM/volume.
- SFX delay, volume factor, option BGM/SFX.
- `Register`, `RegisterBackground`, fallback `Resources/BGM|SFX`.
- API UI/death từ bản gốc.
- Đã bỏ asset-pack/network/`SoundQueueManager` vì PA không có.

Đã copy sound gốc `CompanionSkill_0..37` (~11 MB) vào `Assets/10_Sound/SFX/SKILL/Companion/`. Game gốc không có `CompanionSkill_38`, không gán file giả.

Đã thêm root GameObject `SoundManager` vào cả hai scene Build Settings:

- `Assets/_Playable/PlayableAd.unity`
- `Assets/_Battle/BattleDemo.unity`

Mỗi manager có 38 clip companion trong `SfxClips`. Unity hiện được trả lại scene `BattleDemo`.

### Unity/PlayWorks compatibility đã sửa

- Không dùng `Resources.GetBuiltinResource<T>`; font serialize qua Inspector.
- `Image.Origin360.Top` đổi sang fill origin numeric tương thích.
- ProCamera cinematic `SendMessage` dùng overload tương thích.
- Switch expression liên quan đã đổi sang switch statement cũ hơn.
- Timeline dependency không cần thiết đã bỏ.

## Trạng thái kiểm tra gần nhất

- Unity console: 0 error.
- `BattleDemo`: validate clean, 0 missing script, 0 broken prefab; hierarchy thấy root `SoundManager`.
- `PlayableAd`: hierarchy thấy root `SoundManager` và 38 audio refs.
- `PlayableAd` còn 1 missing script cũ trên GameObject `Game`, không liên quan SoundManager; không tự xóa trước khi trace.

## Việc nên làm tiếp

1. Play-test button skill 0/10: damage, cooldown, icon và audio tương ứng trên Unity + PlayWorks.
2. Nếu cần sound skill 38, chọn/tạo clip rõ ràng; không alias ngầm sang skill khác.
3. Thay baseline gameplay skill 1..38 bằng spec thật nếu tìm được bảng source/server.
4. Trace missing script trên `PlayableAd/Game` trước khi sửa/xóa.
5. Nếu cần giảm playable size, chỉ giữ audio của skill thật sự trang bị.

## Bẫy quan trọng

- `Damage` là struct: không dùng null; early-return `default(Damage)`.
- Enum global và `IdleBattle.*` có tên trùng; qualify đúng namespace để tránh CS0576.
- Odin/PCG.String/PurpleCow không có trong PA.
- Prefab copy từ source có thể missing script nếu GUID meta không khớp.
- Ability cache lúc initialize; component phải tồn tại trước `Initialize`.
- Không tạo lại `CompanionSkillPreset`; config đã hợp nhất vào `SkillEffectPreset`.
- Không copy toàn bộ sound source (~418 MB) vào playable; hiện chỉ có companion (~11 MB).
- Có thể có hai Unity Editor cùng chạy; khi dùng Unity MCP phải chọn instance `PA_IdleBerserker`, không chọn source.

---

## Cập nhật: refactor BattleBootstrap → SOLID (tách bộ phận)

`BattleBootstrap` (god-class ~435 dòng, build entity runtime) **đã bị xoá**, thay bằng các thành phần SRP trong `Assets/_Battle/Demo/`:

- **`BattleCameraController`** (trên Main Camera): setup ortho + follow player. Field PosX/PosY/Size/Follow/LeadRatio/Lerp.
- **`BattleEnvironment`** (`[DefaultExecutionOrder(100)]`): dựng nền sky/ground qua `BattleStage.BuildEnvironment(Camera.main)`.
- **`BattleStage`** (static): chỉ còn dựng môi trường (Quad + white sprite).
- **`EnemySpawnManager`**: spawn quái/boss **từ prefab** (`EnemyPrefab`=AbyssMonster.prefab, `BossPrefab`=Boss.prefab) + pool `EnemyVariants` + `BossVariant`; instantiate → set `MonsterAutoInit.Variant` + vị trí → activate (MonsterAutoInit tự init). Cull quái rớt lại; kills→boss; set BattleManager.State=Start. Field SpawnInterval/KillsToBoss/StartDelay/SpawnAheadRange/CullBehindMargin.

Nguyên tắc: **data/VFX/SFX nằm trên chính prefab của entity** — AbyssMonster/Boss prefab đã tự chứa abilities + HitVfxAbility(4 ref) + SpineEventEffect(death/attack/FX_GroundBurst) + **BuffAbility** (mới thêm) + MonsterAutoInit; player prefab đã tự chứa attack SFX. `MonsterData` (mảng skeleton fallback) + toàn bộ field VFX/SFX/preset trên bootstrap cũ **đã bỏ**.

Scene `BattleDemo`: GameObject `BattleSystems` (EnemySpawnManager + BattleEnvironment, đã gán prefab + 3 EnemyVariant Swift/Brute/Elite + Boss RaidA), Main Camera có BattleCameraController. Verify: quái spawn từ prefab đúng variant (Elite hp85/Monster_02, BuffAbility+HitVfx sẵn), camera follow + nền OK, 0 lỗi compile/runtime.

Muốn thêm/đổi biến thể quái: kéo EnemyVariant vào `EnemySpawnManager.EnemyVariants` (không đụng code).

---

## Cập nhật: clone BackgroundSet + prefab nền (parallax)

Clone hệ nền parallax từ game gốc (scope: **World1-4**, theo yêu cầu — không lấy hết 271MB).

- **Script** `Assets/02_Script/Battle/Background/`: `BackgroundSet.cs` (5 layer texture + Init/Refresh/SetLayers) + `Background.cs` (`Background` parallax qua MaterialPropertyBlock `_MainTex_ST`, + `SpriteBackground`). Copy giữ **guid gốc** (prefab resolve đúng), đã **strip `using Sirenix.OdinInspector`** (Odin không có trong PA; không có attribute Odin thực).
- **Prefab** `Assets/03_Prefab/BackgroundSet/`: World1..World4 (giữ guid). Texture `Assets/04_Sprite/AssetBundle/Background/World1..4/` (~27MB), material `Assets/06_Material/Background/` (shader built-in Mobile/Particles Alpha Blended — PA là built-in pipeline nên OK).
- **Driver mới** `BackgroundController.cs` (thay `BackgroundManager` gốc — bỏ addressables/dungeon/berserk-VFX): field `BackgroundPrefab` + `ScrollSpeed` + `Follow`. Awake tạo 5 material từ shader; Start instantiate BackgroundSet + `Init(mats)`; LateUpdate bám player + `Refresh(playerPos, ScrollSpeed)` (cuộn parallax). Scale set (3,1,1) như bản gốc.
- **Scene BattleDemo**: GameObject `Background` có BackgroundController gán World1. Verify play: 5 layer active + material + texture + bám player. 0 lỗi.

Lưu ý: `BattleEnvironment` (ground/sky placeholder) giờ **thừa** khi có nền thật — nên disable/xoá nếu bị chồng. Đổi nền: kéo World2/3/4 vào `BackgroundController.BackgroundPrefab`. Muốn thêm Dungeon/Raid/PVP/EventWorld: copy prefab + folder texture tương ứng từ game gốc (giữ .meta) — script đã sẵn sàng resolve.

---

## Cập nhật: clone nguyên BackgroundManager + FX nền (giống ingame)

Thay `BackgroundController` (bản nhẹ trước đó, ĐÃ XOÁ) bằng **BackgroundManager thật** từ game gốc.

- **Copy nguyên `Assets/03_Prefab/Battle/BackgroundManager.prefab`** + toàn bộ dependency (~4MB, 26 asset: 8 png, 9 mat, 5 prefab FX, 2 custom shader, 1 tga) — giữ .meta/guid. Prefab giữ đúng setup ingame: `_centerOffSet=(0,5,0)`, `_scrollSpeed=0.5`, **37 ParticleSystem** con (FX_Background = Ashes/Fire/Smoke khí quyển; FX_Berserker_Background_01/`_EventWorld`/`` cho berserk).
- **Port `BackgroundManager.cs`** (`02_Script/Battle/Background/`, guid gốc): `SingletonBehaviour<BackgroundManager>`. Bỏ addressables/`CookApps.PAD`/`AssetPackManager` + các mode dungeon/pvp/raid/eventworld/promotion + ground-rotation (DOTween/MeshRendererAbility). Thay nguồn nền bằng serialized **`_stageBackgrounds` (GameObject[] = World1..4)** + `_autoStageIndex`/`_autoStart`. Giữ: follow player + `Refresh` parallax scroll, `_backgroundVFX` bật khi set nền, `SetActiveBerserkBackground(bool)`.
- **Wire BattleDemo**: instance BackgroundManager.prefab (đã gán `_stageBackgrounds`=World1-4, autoStage=0). Gỡ `Background`(BackgroundController) + `BattleEnvironment` placeholder khỏi scene.
- **Hook berserk**: `BerserkerObject` gọi `BackgroundManager.Instance.SetActiveBerserkBackground(true/false)` khi vào/thoát berserk; `Start` tắt berserk bg ban đầu.

Verify play: BackgroundManager pos follow player + **offset Y=5**; World1 (5 layer + texture); **FX_Background particle chạy**; berserk → **FX_Berserker_Background_01 active+play**, thoát → tắt. 0 lỗi.

Đổi nền: `BackgroundManager._autoStageIndex` (0..3 = World1-4) hoặc gọi `SetStageBackground(index)`. Thêm nền: nối prefab World khác vào `_stageBackgrounds`.

---

## Cập nhật: FX_Berserker_Background_01 active nhưng không thấy

Đã xác định nguyên nhân: **thiếu sorting layer `FX1` trong PA**.

- `FX_Berserker_Background_01.prefab` là bản giống hệt game gốc; `BackgroundManager.SetActiveBerserkBackground(true)` cũng đã active và gọi `Play()` đúng.
- Các `ParticleSystemRenderer` con của FX đang serialize `m_SortingLayerID: 824049247`, tương ứng layer **`FX1`** trong `berserkeridle-mobile-client/ProjectSettings/TagManager.asset`.
- PA hiện chỉ có layer `Default`. Background World1 dùng `Default`, order `-50..-10`; FX có order `-999..-994`. Khi `FX1` thiếu, renderer rơi về Default/ID không hợp lệ và bị background che hết.

### Cách sửa khuyến nghị

Thêm sorting layer vào `ProjectSettings/TagManager.asset` theo đúng thứ tự và ID của game gốc:

```yaml
- name: FX1
  uniqueID: 824049247
  locked: 0
```

`FX1` phải đứng **sau `Default`**. Khi đó FX vẫn giữ order âm để ở sau character, nhưng được render phía trên background Default. Không tăng sorting order thủ công từng ParticleSystem: prefab có nhiều renderer và cách đó dễ sai layer/che nhân vật.

Chưa áp dụng thay đổi TagManager ở thời điểm ghi handoff này; cần thêm layer rồi Play-test berserk background.

---

## Cập nhật: UI HUD binder + EnemySpawnManager loop/CallBoss

User kéo nguyên cây UI ingame vào `BattleDemo` (Canvas ScreenSpaceOverlay), các GameObject KHÔNG có script controller (chỉ Image/Button). Đã viết binder nhẹ (poll thay hệ GameEvent gốc) trong `Assets/_Battle/UI/` + wire qua SerializedObject:

- `UIBerserkGaugeBinder` (trên `UI_BerserkGauge`): fill=BerserkGaugeImage (material `LeftHP`/shader `Custom/Fluide`, prop `_FillLevel`), button=BerserkGaugeFrame, animator=FX_UI_BerserkGauge, toggle=ToggleAutoBerserk. Poll `BerserkAbility` (fill GaugeRatio/RemainRatio; click→`SetBerserkState(true)`; toggle→`IsAuto`).
- `UICompanionSkillIconBinder` (4 icon nhóm Companion, slot 0..3) → `CompanionSkillManager.Skills[slot]`.
- `UIPlayerSkillIconBinder` (4 icon nhóm Active, slot 0..3) → `PlayerSkillDriver.Skills[slot]`.
- `UIAutoSkillToggleBinder` (ToggleAutoSkill) → set `AutoUse` cho **CẢ** CompanionSkillManager LẪN PlayerSkillDriver.
- API bind thêm: `BerserkAbility.RemainRatio` (+ `_duration`); `Skills`/`Icons` (IReadOnlyList) + `AutoUse` + **`AutoUseInterval`** (float, mặc định 1s = giãn cách 2 lần auto) trên cả 2 manager; `CompanionSkillManager.BuildRuntimeSkillBar=false`.

**Bắt buộc:** scene phải có **EventSystem** (đã thêm) — thiếu thì mọi nút UI không bấm được. Icon con `ImageSkillIcon` trong prefab ingame tắt sẵn → binder bật (`enabled=true`, alpha=1). Nút cần `raycastTarget=true` trên Image khung + xoá persistent onClick cũ (trỏ script gốc đã mất). Nhóm Active có `OnLock`/`OnGroggyLock` bật sẵn → tắt (playable ad không khoá).

Icon skill: `04_Sprite/AtlasIcon_DonTouch/Icon_Skill_NN` (single sprite). Map **fieldID N → Icon_Skill_(N+1)**; gán vào cả `SkillEffectPreset.Icon` (driver tự lấy) lẫn UI `ImageSkillIcon`.

`EnemySpawnManager` (`_Battle/Demo/`): enemy loop **vô tận** (bỏ spawn boss theo đợt), boss gọi qua `public void CallBoss()` (dedup: 1 boss, chết mới gọi lại) — nút UI kéo-thả OnClick → CallBoss.

---

## Cập nhật: PlayerSkill (38 skill) + DamageTextFactory

- **PlayerSkill**: port `02_Script/Battle/Skill/PlayerSkill/` (base + PlayerActiveSkill/PlayerPassiveSkill + PlayerSkill_0..37 + SpecSkill + PlayerSkillStubs) — global namespace, dùng lại hạ tầng CompanionSkill (Buff/SkillEffectPreset). Driver PA-native `Assets/_Battle/Ability/PlayerSkillDriver.cs` (: CharacterAbility, giống CompanionSkillManager): `GameObject[] SkillPrefabs` + `AutoUse` + `AutoUseInterval`, expose `Skills`/`Icons`. Đã gắn vào PlayerObject.prefab + 4 prefab active copy từ gốc (`03_Prefab/Battle/Skill/BersekerSkill/PlayerSkill_0..3`) + wire 4 icon Active. **KHÔNG** port `Manager/PlayerSkillManager.cs` gốc (coupling nặng). Bổ sung `IdleBattle.Enum_DamageType` += Poison/PoisonDamage/BurnDamage/Evasion + `Enum_EffectTrigger` stub cho PlayerSkill_28. Chi tiết: bản Claude project doc `player-skill-port.md`.
- **DamageTextFactory**: `02_Script/Lib/ObjectPool/ObjectPool.cs` + `02_Script/Battle/DamageText/` (DamageText + DamageTextFactory + `DamageNumberExtension.ToUnitString` nhẹ K/M/B). Prefab `03_Prefab/Battle/DamageText/DamageTextFactory.prefab` (Canvas WorldSpace **scale 0.009**, pool 16, controller FX_DamageText + ~18 anim). Instance trong BattleDemo; `HealthUI.HandleDamaged` gọi `DamageTextFactory.Instance.Show(worldPos, d)` thay DamagePopup. Chi tiết: `damagetext-port.md`.

---

## Cập nhật: fix combat "đứng đánh/cast khi mục tiêu đã chết"

Triệu chứng: enemy chết nhưng player chạy hết animation combo/skill, không tới mục tiêu kế còn xa. Nguyên nhân: state khoá trọn animation (Detect/Death/ReportKill đều đúng ngay lập tức). Fix trong `_Battle/States/PlayerStates.cs` + `_Battle/Ability/PlayerSkillAbility.cs`:

1. `BerserkerAttackState`: gộp windup/hit/recovery thành 1 frame-loop; **huỷ đòn CHỈ trong windup** (`if (!_hit && !_detect.Detect(AttackRange)) break;`) — mục tiêu chết trước khi đòn trúng thì bỏ swing đi tìm mục tiêu kế. Sau khi đã trúng đòn thì để animation chạy hết recovery rồi mới re-check (chủ ý, tránh giật anim).
2. `PlayerSkillAbility.HasTarget()`: siết phạm vi kích từ `DetectRange*2`(16) → **`AttackRange`** (chỉ tung skill khi quái sát; Active vẫn quét AoE `AttackRange+5`).
3. `BerserkerSkillState`: early-out — huỷ cast nếu `!_detect.Detect(AttackRange+5)`; chỉ ResetCooldown+Active khi không huỷ.

Verify: sample ~9 lần, player chỉ Attack/Skill khi nearestDx≤~1, còn lại Run tiến đều; 0 lỗi.

---

## Cập nhật: fix build Unity Playworks (Luna)

- **`Addressable bundle #0 contains multiple assets`**: asset copy từ gốc mang **Asset Bundle label**. Xoá `assetBundleName`/`assetBundleVariant` mọi `.meta` (`sed -i -E 's/^( *assetBundleName:).*/\1/; s/^( *assetBundleVariant:).*/\1/'`) + `AssetDatabase.RemoveAssetBundleName(n,true)`; kiểm `GetAllAssetBundleNames()==0`. **Mỗi lần copy asset gốc phải làm lại.**
- **`Image.Origin360` không tồn tại** khi build: dùng literal `fillOrigin = 2` (Radial360 Top) — đã áp cho UIPlayerSkillIconBinder/UICompanionSkillIconBinder.
- **`CultureInfo` → int overload lỗi** (`DamageNumberExtension.cs`): bỏ `using System.Globalization` + `.ToString(CultureInfo...)` → `.ToString()`/`.ToString(fmt)`.

(Chi tiết combat + build: Claude project doc `combat-and-build-fixes.md`.)

---

## Cập nhật: BGM theo trạng thái (thường / berserk)

Dùng lại `SoundManager` sẵn có (đã có `BackgroundClips` + `PlayBackground(name)`), không viết hệ mới.

- Copy 2 clip từ gốc `10_Sound/BGM/` (guid-preserving): `berserk_bgm_normal` (thường, ~38s loop) + `berserk_bgm_berserkmode` (~7.5s loop). Import nén: **Vorbis, mono, q0.35, CompressedInMemory** (giảm size cho playable/WebGL). Gán cả 2 vào `SoundManager.BackgroundClips` (register theo tên clip).
- Wire 3 hook (SoundManager ở global namespace, gọi từ IdleBattle OK):
  - `EnemySpawnManager` sau `BattleManager.State = Start` → `PlayBackground("berserk_bgm_normal", true)`.
  - `BerserkerObject.BerserkOnRoutine` (sau `SetActiveBerserkBackground(true)`) → `PlayBackground("berserk_bgm_berserkmode", true)`.
  - `BerserkerObject.SetBerserkState(false)` (sau `SetActiveBerserkBackground(false)`) → `PlayBackground("berserk_bgm_normal", true)`.
  - Tất cả guard `if (SoundManager.Instance != null)`.
- `PlayBackground` không restart nếu đang phát đúng clip; key lookup = tên AudioClip. Verify play-test: start=normal, vào berserk=berserkmode, thoát=normal — đều `playing=True`, 0 lỗi.
- Đổi nhạc: thay clip trong `SoundManager.BackgroundClips` (giữ đúng tên) hoặc đổi chuỗi tên trong 3 hook. Lưu ý size: normal gốc 6.5MB WAV → sau nén Vorbis mono nhỏ hơn nhiều; nếu cần playable cực nhẹ, trim clip ngắn lại.

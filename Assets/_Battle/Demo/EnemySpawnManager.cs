using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleBattle
{
    // Spawn quái VÔ TẬN theo nhịp (endless), boss gọi theo YÊU CẦU qua CallBoss()
    // (nút UI kéo-thả gọi tới), không auto-spawn theo đợt nữa.
    public class EnemySpawnManager : MonoBehaviour
    {
        [Header("Prefab entity (tự chứa abilities + VFX + SFX)")]
        public GameObject EnemyPrefab;   // Prefabs/AbyssMonster.prefab
        public GameObject BossPrefab;    // Prefabs/Boss.prefab

        [Header("Pool biến thể")]
        public EnemyVariant[] EnemyVariants;
        public BossVariant BossVariant;

        [Header("Nhịp spawn quái")]
        public float StartDelay = 0.6f;
        public float SpawnInterval = 1.3f;
        [Tooltip("Khoảng cách sinh trước rìa phải màn hình")]
        public Vector2 SpawnAheadRange = new Vector2(1.5f, 3.5f);
        public float CullBehindMargin = 4f;

        [Header("Boss")]
        [Tooltip("Chỉ cho 1 boss sống tại 1 thời điểm (nhấn nút khi boss còn sống sẽ bị bỏ qua)")]
        public bool OneBossAtATime = true;

        private int _spawnOrder;
        private PlayerObject _player;
        private MonsterObject _currentBoss;
        private bool _bossWasAlive;

        // Đang có boss trên sân (chưa chết)? (dùng cho nút UI bật/tắt)
        public bool IsBossActive { get { return _currentBoss != null; } }

        private void Start()
        {
            if (BattleManager.Instance == null)
            {
                new GameObject("BattleManager").AddComponent<BattleManager>();
            }
            BattleManager.Instance.State = Enum_BattleState.Start;
            StartCoroutine(SpawnLoop());
        }

        private void Update()
        {
            if (_currentBoss == null) { return; }
            if (_currentBoss.IsAlive) { _bossWasAlive = true; }
            else if (_bossWasAlive) { _currentBoss = null; _bossWasAlive = false; }  // đã sống rồi mới chết -> cho gọi lại
        }

        private PlayerObject Player()
        {
            if (_player == null) { _player = BattleManager.Instance.PlayerObject; }
            if (_player == null) { _player = FindObjectOfType<PlayerObject>(); }
            return _player;
        }

        private IEnumerator SpawnLoop()
        {
            yield return new WaitForSeconds(StartDelay);
            while (true)
            {
                PlayerObject p = Player();
                if (p != null && !p.IsDeath) { SpawnEnemy(); }   // endless: không bao giờ dừng
                yield return new WaitForSeconds(SpawnInterval);
            }
        }

        private float RightEdgeX()
        {
            Camera cam = Camera.main;
            PlayerObject p = Player();
            float baseX = p != null ? p.Position.x : (cam != null ? cam.transform.position.x : 0f);
            float halfW = cam != null ? cam.orthographicSize * cam.aspect : 8f;
            return baseX + halfW;
        }

        private void SpawnEnemy()
        {
            if (EnemyPrefab == null) { return; }
            CullBehind();
            _spawnOrder++;
            EnemyVariant variant = (EnemyVariants != null && EnemyVariants.Length > 0)
                ? EnemyVariants[Random.Range(0, EnemyVariants.Length)] : null;
            float x = RightEdgeX() + Random.Range(SpawnAheadRange.x, SpawnAheadRange.y);
            SpawnFromPrefab(EnemyPrefab, variant, x, 30 + (_spawnOrder % 15));
        }

        // Gọi từ nút UI (kéo-thả OnClick -> EnemySpawnManager.CallBoss).
        public void CallBoss()
        {
            if (BossPrefab == null) { return; }
            if (OneBossAtATime && _currentBoss != null) { return; }   // đang có boss -> bỏ qua

            float x = RightEdgeX() + 3f;
            GameObject go = SpawnFromPrefab(BossPrefab, BossVariant, x, 46);
            _currentBoss = go != null ? go.GetComponent<MonsterObject>() : null;
            _bossWasAlive = false;
        }

        // Instantiate prefab -> gán Variant + vị trí -> kích hoạt để MonsterAutoInit tự init.
        private GameObject SpawnFromPrefab(GameObject prefab, CharacterVariant variant, float x, int sortingOrder)
        {
            GameObject go = Instantiate(prefab);
            go.SetActive(false);
            go.transform.position = new Vector3(x, 0f, 0f);

            MonsterAutoInit init = go.GetComponent<MonsterAutoInit>();
            if (init != null) { init.Variant = variant; }

            Transform model = go.transform.Find("Model");
            if (model != null)
            {
                MeshRenderer mr = model.GetComponent<MeshRenderer>();
                if (mr != null) { mr.sortingOrder = sortingOrder; }
            }

            go.SetActive(true); // Awake + Start (MonsterAutoInit) chạy với Variant đã set
            return go;
        }

        private void CullBehind()
        {
            PlayerObject p = Player();
            Camera cam = Camera.main;
            if (p == null || cam == null) { return; }
            float halfW = cam.orthographicSize * cam.aspect;
            float limit = p.Position.x - (halfW + CullBehindMargin);
            List<MonsterObject> list = BattleManager.Instance.GetMonsters();
            for (int i = list.Count - 1; i >= 0; i--)
            {
                MonsterObject m = list[i];
                if (m != null && !m.IsDeath && !m.IsBoss && m.Position.x < limit)
                {
                    m.Kill();
                    BattleManager.Instance.RemoveMonster(m);
                }
            }
        }
    }
}

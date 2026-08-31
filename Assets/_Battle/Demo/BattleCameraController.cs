using UnityEngine;

namespace IdleBattle
{
    // Single Responsibility: cấu hình camera orthographic + bám theo player.
    [RequireComponent(typeof(Camera))]
    public class BattleCameraController : MonoBehaviour
    {
        [Header("Camera")]
        public float PosX = 0f;
        public float PosY = 1.7f;
        public float Size = 4.6f;
        public Color Background = new Color(0.20f, 0.24f, 0.34f);

        [Header("Follow")]
        public bool Follow = true;
        [Tooltip("Đẩy camera lên trước player theo % nửa bề rộng màn hình")]
        public float LeadRatio = 0.45f;
        public float Lerp = 5f;

        private Camera _cam;
        private PlayerObject _player;

        private float HalfWidth { get { return _cam.orthographicSize * _cam.aspect; } }

        private void Awake()
        {
            _cam = GetComponent<Camera>();
            _cam.orthographic = true;
            _cam.orthographicSize = Size;
            _cam.transform.position = new Vector3(PosX, PosY, -10f);
            _cam.clearFlags = CameraClearFlags.SolidColor;
            _cam.backgroundColor = Background;
        }

        private void LateUpdate()
        {
            if (!Follow) { return; }
            if (_player == null) { _player = FindObjectOfType<PlayerObject>(); }
            if (_player == null || _player.IsDeath) { return; }

            float targetX = _player.Position.x + HalfWidth * LeadRatio;
            Vector3 p = _cam.transform.position;
            p.x = Mathf.Lerp(p.x, targetX, Time.deltaTime * Lerp);
            _cam.transform.position = p;
        }
    }
}

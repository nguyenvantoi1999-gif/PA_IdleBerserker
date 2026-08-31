using UnityEngine;

namespace IdleBattle
{
    // Single Responsibility: dựng phông nền theo camera (chạy sau khi camera đã setup).
    [DefaultExecutionOrder(100)]
    public class BattleEnvironment : MonoBehaviour
    {
        private void Start()
        {
            BattleStage.BuildEnvironment(Camera.main);
        }
    }
}

using UnityEngine;
using Spine.Unity;

namespace IdleBattle
{
    // Base cho mọi biến thể nhân vật. Mỗi asset = 1 biến thể (hình ảnh + chỉ số).
    // Gắn asset vào prefab (qua AutoInit) hoặc pool spawn của BattleBootstrap
    // để tạo nhiều biến thể từ cùng 1 prefab gốc, không cần tạo prefab mới.
    public abstract class CharacterVariant : ScriptableObject
    {
        [Header("Định danh")]
        public string DisplayName = "Variant";

        [Header("Hình ảnh")]
        [Tooltip("Bỏ trống = giữ nguyên skeleton mặc định của prefab")]
        public SkeletonDataAsset Skeleton;
        [Tooltip("Bỏ trống = giữ skin mặc định")]
        public string Skin = "";
        public float ModelScale = 0.85f;
        [Tooltip("Nhuộm màu skeleton (trắng = giữ nguyên)")]
        public Color Tint = Color.white;

        [Header("Chỉ số cơ bản")]
        public float Damage = 20f;
        public float Health = 100f;
        public float MoveSpeed = 3.2f;
        public float AttackSpeed = 1.2f;
        public float DetectRange = 8f;
        public float AttackRange = 3f;

        // Ghi chỉ số vào bảng Stat của nhân vật.
        public virtual void WriteStats(Stat stat)
        {
            stat[Enum_StatType.Damage] = Damage;
            stat[Enum_StatType.Health] = Health;
            stat[Enum_StatType.MoveSpeed] = MoveSpeed;
            stat[Enum_StatType.AttackSpeed] = AttackSpeed;
            stat[Enum_StatType.DetectRange] = DetectRange;
            stat[Enum_StatType.AttackRange] = AttackRange;
        }

        // Áp hình ảnh (skeleton/skin/scale/màu) lên child Model có SkeletonAnimation.
        public void ApplyVisual(Transform model)
        {
            if (model == null) { return; }
            SkeletonAnimation sa = model.GetComponent<SkeletonAnimation>();
            if (sa != null)
            {
                if (Skeleton != null && sa.skeletonDataAsset != Skeleton)
                {
                    sa.ClearState();
                    sa.skeletonDataAsset = Skeleton;
                    sa.Initialize(true);
                }
                if (sa.Skeleton != null)
                {
                    if (!string.IsNullOrEmpty(Skin))
                    {
                        sa.Skeleton.SetSkin(Skin);
                        sa.Skeleton.SetSlotsToSetupPose();
                    }
                    sa.Skeleton.SetColor(Tint);
                    if (sa.AnimationState != null) { sa.AnimationState.SetAnimation(0, "idle", true); }
                }
            }
            model.localScale = new Vector3(ModelScale, ModelScale, ModelScale);
        }
    }
}

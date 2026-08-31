using UnityEngine;
using Spine.Unity;

namespace IdleBattle
{
    // Wrapper mỏng quanh SkeletonAnimation (Spine 3.8).
    public class AnimationAbility : CharacterAbility
    {
        private SkeletonAnimation _animation;
        private float _animSpeed = 1f;

        public SkeletonAnimation Animation
        {
            get
            {
                if (_animation == null)
                {
                    _animation = GetComponentInChildren<SkeletonAnimation>();
                }
                return _animation;
            }
        }

        public Spine.AnimationState AnimationState { get { return Animation.AnimationState; } }
        public Spine.Skeleton Skeleton { get { return Animation.Skeleton; } }

        public float Height
        {
            get
            {
                SkeletonAnimation sa = Animation;
                if (sa == null) { return 1f; }
                MeshRenderer mr = sa.GetComponent<MeshRenderer>();
                if (mr != null) { return mr.bounds.size.y; }
                if (sa.Skeleton != null && sa.Skeleton.Data != null)
                {
                    return sa.Skeleton.Data.Height * Mathf.Abs(sa.transform.lossyScale.y);
                }
                return 1f;
            }
        }

        // Tâm hình học thực (world) từ bounds của mesh — dùng để đặt VFX/UI.
        public Vector3 CenterWorld
        {
            get
            {
                SkeletonAnimation sa = Animation;
                if (sa != null)
                {
                    MeshRenderer mr = sa.GetComponent<MeshRenderer>();
                    if (mr != null) { return mr.bounds.center; }
                    return sa.transform.position;
                }
                return transform.position;
            }
        }

        public float Width
        {
            get
            {
                SkeletonAnimation sa = Animation;
                if (sa == null || sa.Skeleton == null || sa.Skeleton.Data == null) { return 1f; }
                return sa.Skeleton.Data.Width * Mathf.Abs(sa.transform.lossyScale.x);
            }
        }

        public void SetSkeletonDataAsset(SkeletonDataAsset asset)
        {
            SkeletonAnimation sa = Animation;
            if (sa == null || asset == null) { return; }
            if (sa.skeletonDataAsset == asset) { return; }
            sa.ClearState();
            sa.skeletonDataAsset = asset;
            sa.Initialize(true);
        }

        public void SetSkin(string skinName)
        {
            Skeleton.SetSkin(skinName);
            Skeleton.SetSlotsToSetupPose();
        }

        public void SetScaleX(float x)
        {
            Skeleton.ScaleX = x;
        }

        public Spine.TrackEntry PlayAnimation(string name, bool loop)
        {
            _animSpeed = 1f;
            return AnimationState.SetAnimation(0, name, loop);
        }

        public Spine.TrackEntry PlayAnimation(string name, bool loop, float speed)
        {
            _animSpeed = speed;
            return AnimationState.SetAnimation(0, name, loop);
        }

        public bool IsAnimationExist(string name)
        {
            return Animation != null && Skeleton.Data.FindAnimation(name) != null;
        }

        public bool HasEvent(string name)
        {
            return Animation != null && Skeleton.Data.FindEvent(name) != null;
        }

        public float GetDuration(string name, float speed = 1f)
        {
            Spine.Animation a = Skeleton.Data.FindAnimation(name);
            if (a == null) { return 0f; }
            return a.Duration / Mathf.Max(0.0001f, speed);
        }

        public bool TryGetAnimation(string name, float speed, out float duration)
        {
            Spine.Animation a = Skeleton.Data.FindAnimation(name);
            if (a == null) { duration = 0f; return false; }
            duration = a.Duration / Mathf.Max(0.0001f, speed);
            return true;
        }

        // Chọn animation đầu tiên tồn tại trong danh sách (fallback).
        public string ResolveName(params string[] candidates)
        {
            for (int i = 0; i < candidates.Length; i++)
            {
                if (IsAnimationExist(candidates[i])) { return candidates[i]; }
            }
            return candidates.Length > 0 ? candidates[candidates.Length - 1] : null;
        }

        public override void ProcessAbility(float deltaTime)
        {
            SkeletonAnimation sa = Animation;
            if (sa != null && sa.AnimationState != null)
            {
                sa.AnimationState.TimeScale = _animSpeed;
            }
        }
    }
}

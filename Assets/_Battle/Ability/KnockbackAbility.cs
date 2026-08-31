using System.Collections;
using UnityEngine;

namespace IdleBattle
{
    // Giật lùi khi trúng đòn. Game gốc đang tắt (test cảm giác đánh);
    // bản port làm nhẹ trên Model, mặc định BẬT.
    public class KnockbackAbility : CharacterAbility
    {
        public bool Enabled = true;
        public float Power = 0.15f;
        public float Duration = 0.12f;

        private Coroutine _routine;

        public void Knockback(Enum_DamageType damageType)
        {
            if (!Enabled) { return; }
            Transform model = _ownerObject.Model;
            if (model == null) { return; }
            if (_routine != null) { StopCoroutine(_routine); }
            _routine = StartCoroutine(Do(model));
        }

        private IEnumerator Do(Transform model)
        {
            Vector3 start = Vector3.zero;
            Vector3 back = start + model.right * Power;
            float e = 0f;
            while (e < Duration)
            {
                e += Time.deltaTime;
                model.localPosition = Vector3.Lerp(start, back, e / Duration);
                yield return null;
            }
            e = 0f;
            while (e < Duration)
            {
                e += Time.deltaTime;
                model.localPosition = Vector3.Lerp(back, start, e / Duration);
                yield return null;
            }
            model.localPosition = start;
            _routine = null;
        }
    }
}

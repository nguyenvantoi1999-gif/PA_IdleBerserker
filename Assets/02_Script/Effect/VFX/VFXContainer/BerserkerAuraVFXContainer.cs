using UnityEngine;

public class BerserkerAuraVFXContainer : MonoBehaviour
{
    [SerializeField] private GameObject[] _goAura;
    [SerializeField] private GameObject[] _goUIAura;
    public bool VFXUI = true;
    
    /// <summary>
    /// Set Aura Index 
    /// </summary>
    /// <param name="index">코스튬 인덱스 (fieldID)</param>
    public void Init(int index)
    {
        SetOff();

    }

    private void SetOff()
    {
        for(int i = 0 ; i < _goAura.Length ; i++)
        {
            _goAura[i].SetActive(false);
            _goUIAura[i].SetActive(false);
        }
    }
}
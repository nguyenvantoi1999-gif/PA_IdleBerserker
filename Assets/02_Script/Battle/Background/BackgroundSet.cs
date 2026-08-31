using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class BackgroundSet : MonoBehaviour
{
    public Texture Layer1Texture;
    public Texture Layer2Texture;
    public Texture Layer3Texture;
    public Texture Layer4Texture;
    public Texture Layer5Texture;

    [SerializeField] private List<Texture> _textures;
    [SerializeField] private List<Background> _layers;

    public void Init(List<Material> mats)
    {
        if (_layers == null || _layers.Count <= 0)
        {
            SetLayers();
        }
        
        for (var i = 0; i < _layers.Count; i++)
        {
            _layers[i].SetTexture(_textures[i], mats[i]);
        }
    }
    
    public void Refresh(Vector3 playerPosition, float scrollSpeed)
    {
        foreach (var t in _layers)
        {
            t.Refresh(playerPosition, scrollSpeed);
        }
    }

    public void SetLayers()
    {
        _layers = GetComponentsInChildren<Background>().ToList();

        _textures.Clear();
        
        _textures.Add(Layer1Texture);
        _textures.Add(Layer2Texture);
        _textures.Add(Layer3Texture);
        _textures.Add(Layer4Texture);
        _textures.Add(Layer5Texture);
        
        for (var i = 0; i < _layers.Count; i++)
        {
            _layers[i].transform.localPosition = Vector3.zero + Vector3.back * i;
            _layers[i].transform.localScale = new Vector3(34.56f,19.44f,1);
        }

        _layers[_layers.Count - 1].transform.position += Vector3.up * 0.1f;
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class Background : MonoBehaviour
{
    private static readonly int MainTex = Shader.PropertyToID("_MainTex");
    private static readonly int MainTex_ST = Shader.PropertyToID("_MainTex_ST");
    
    public float Speed = 1f;

    private Texture _currentTexture;
    private MeshRenderer _meshRenderer;

    private Vector2 _localScale;

    // [ValidateInput(condition:"ValidateSortingLayer")]
    [SerializeField] private string _sortingLayer;
    [SerializeField] private int _sortOrder;

    private MaterialPropertyBlock _mpb;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _meshRenderer.sortingLayerName = _sortingLayer;
        _meshRenderer.sortingOrder = _sortOrder;
        _mpb = new MaterialPropertyBlock();
    }

    private bool ValidateSortingLayer(string name)
    {
        foreach (var sortingLayer in SortingLayer.layers)
        {
            if (string.Equals(sortingLayer.name, name))
            {
                return true;
            }
        }

        return false;
    }

    private void Start()
    {
        _localScale = transform.localScale;
    }

    public void SetTexture(Texture texture, Material mat)
    {
        _currentTexture = texture; 

        if (texture == null)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);

            _meshRenderer.material = mat;
        
            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            _meshRenderer.GetPropertyBlock(mpb);
            mpb.SetTexture(MainTex, texture);
            _meshRenderer.SetPropertyBlock(mpb);
            
            Reset();
        }
    }
    
    public void Refresh(Vector3 position , float totalSpeed)
    {
        Vector2 offset = position;
        offset.x *= Speed * totalSpeed;
        offset.y = 0;

        Vector4 materialVector = new Vector4(3f, 1f, offset.x, offset.y);
        _mpb.Clear();
        _meshRenderer.GetPropertyBlock(_mpb);
        _mpb.SetVector(MainTex_ST, materialVector);
        _meshRenderer.SetPropertyBlock(_mpb);
    }
    public void Reset()
    {
        Vector2 offset = Vector2.zero;
        Vector4 materialVector = new Vector4(3f, 1f, offset.x, offset.y);
        
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        _meshRenderer.GetPropertyBlock(mpb);
        mpb.SetVector(MainTex_ST, materialVector);
        _meshRenderer.SetPropertyBlock(mpb);
    }
}

public class SpriteBackground : MonoBehaviour
{
    private static readonly int MainTex = Shader.PropertyToID("_MainTex");
    
    public float Speed = 1f;

    private SpriteRenderer _spriteRenderer;

    private Vector2 _localScale;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        _localScale = transform.localScale;
    }

    public void SetSprite(Sprite sprite)
    {
        _spriteRenderer.sprite = sprite;
    }
    
    public void Refresh(Vector3 position)
    {
        Vector2 offset = position;
        offset.x *= Speed;
        offset.y = 0;
        
        //_meshRenderer.sharedMaterial.SetTextureOffset(MainTex, offset);
    }

    public void Rotate(float angle, float time)
    {
        transform.DORotate(new Vector3(0, 0, angle), time);
    }
}
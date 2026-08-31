using System.Collections.Generic;
using UnityEngine;

public enum PoolInitOption
{
    Awake,
    Start,
    Lazy,
}

public abstract class ObjectPool<ObjPool, ObjPrefab> : SingletonBehaviour<ObjPool> where ObjPool : GameBehaviour where ObjPrefab : GameBehaviour
{
    [Header("풀링할 개수")] [Range(2, 64)] public int m_AmountToPool = 16;

    [Header("풀링할 오브젝트")] public ObjPrefab m_ObjectPrefabToPool;

    [Header("폴링 시점")] public PoolInitOption m_PoolInitOption = PoolInitOption.Awake;

    [Header("풀링 확장 여부")] public bool m_PoolExpansion = true;
    
    private List<ObjPrefab> m_PooledObjects; // 풀 리스트

    protected bool m_Init = false; // 초기화
    
    public int GetActiveObjectCount
    {
        get
        {
            var list =  m_PooledObjects.FindAll(obj => obj.isActiveAndEnabled == true);
            return list.Count;
        }
    }

    protected override void Awake()
    {
        base.Awake();

        m_PooledObjects = new List<ObjPrefab>(m_AmountToPool);
        
        if (!m_Init && m_PoolInitOption == PoolInitOption.Awake)
        {
            InitPool();
        }
    }

    protected virtual void Start()
    {
        if (!m_Init && m_PoolInitOption == PoolInitOption.Start)
        {
            InitPool();
        }
    }

    protected virtual void InitPool()
    {
        if (m_Init) return;
        
        var alreadyExists = GetComponentsInChildren<ObjPrefab>();
        if (alreadyExists.Length > 0)
        {
            foreach (var item in alreadyExists)
            {
                item.SafeSetActive(false);
                
                m_PooledObjects.Add(item);
            }
        }
        
        for (int i = 0; i < m_AmountToPool; ++i)
        {
            ObjPrefab obj = Instantiate(m_ObjectPrefabToPool, transform);
            
            obj.SafeSetActive(false);
            
            m_PooledObjects.Add(obj);
        }
        
        m_Init = true;
    }

    public ObjPrefab GetPooledObject()
    {
        if (!m_Init && m_PoolInitOption == PoolInitOption.Lazy)
        {
            InitPool();
        }

        foreach (var obj in m_PooledObjects)
        {
            if (!obj.isActiveAndEnabled)
            {
                return obj;
            }
        }

        if (m_PoolExpansion)
        {
            return ExpandPool();
        }
        
        return null;
    }

    // 풀 확장
    private ObjPrefab ExpandPool()
    {
        int count = m_AmountToPool / 2;
       
        int prevCapacity = m_PooledObjects.Capacity;
        
        m_PooledObjects.Capacity = prevCapacity + count;
        
        ObjPrefab obj = null;
        
        for (int i = 0; i < count; ++i)
        {
            obj = Instantiate(m_ObjectPrefabToPool, transform);
            
            obj.SafeSetActive(false);
            
            m_PooledObjects.Add(obj);
        }

        return obj;
    }

    public void HideAll()
    {
        foreach (var obj in m_PooledObjects)
        {
            obj.SafeSetActive(false);
        }
    }
}

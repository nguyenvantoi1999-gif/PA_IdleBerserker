using UnityEngine;

public abstract class SingletonBehaviour<T> : GameBehaviour where T : MonoBehaviour
{
    protected static T m_Instance = null;
    protected bool m_Enabled = false;
    
    public static T Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = FindObjectOfType<T>();

                if (m_Instance == null)
                {
                    GameObject obj = new GameObject();
                    m_Instance = obj.AddComponent<T>();
                    obj.name = string.Format(typeof(T).Name);
                }
            }

            return m_Instance;
        }
    }

    protected virtual void Awake()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        if (m_Instance == null)
        {
            m_Instance = this as T;
            m_Enabled = true;
        }
        else
        {
            if (this != m_Instance)
            {
                Debug.LogError("[X] 싱글톤(" + transform.name + ") : " + typeof(T).Name + "의 중복 생성을 시도하고 있습니다.");
                
                Destroy(this.gameObject);
            }
        }
    }
}
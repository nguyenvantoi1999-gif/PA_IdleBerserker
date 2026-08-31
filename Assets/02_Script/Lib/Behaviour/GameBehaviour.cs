using UnityEngine;

public class GameBehaviour : MonoBehaviour
{
    public Vector3 Position
    {
        set => transform.position = value;

        get => transform.position;
    }
    
    public bool ActiveInHierarchy => gameObject.activeInHierarchy;

    public void SafeSetActive(bool value)
    {
        if (gameObject.activeInHierarchy != value)
        {
            gameObject.SetActive(value);
        }
    }
}
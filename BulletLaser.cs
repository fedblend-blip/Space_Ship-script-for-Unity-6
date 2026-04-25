using UnityEngine;

public class BulletLaser : MonoBehaviour
{
    public float speed = 500f;
    
    void Update()
    {
        
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        Destroy(gameObject, 3f); // Удалить через 3 секунды
    }

}

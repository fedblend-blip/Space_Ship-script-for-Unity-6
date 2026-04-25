using UnityEngine;

public class Gun : MonoBehaviour
{
    public GameObject bulletPrefab; // Ссылка на префаб пули
    public Transform firePoint;
    public Transform firePoint1;  // Точка вылета пули
    // Интервал между выстрелами в секундах (по умолчанию 1 секунда)
    public float fireInterval = 0.05f;
    // Время следующего разрешённого выстрела
    private float nextFireTime = 0f;

    void Update()
    {
        if (Input.GetButton("Fire1") && Time.time >= nextFireTime)
        {
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Instantiate(bulletPrefab, firePoint1.position, firePoint1.rotation);
            nextFireTime = Time.time + fireInterval;
        }
    }

}

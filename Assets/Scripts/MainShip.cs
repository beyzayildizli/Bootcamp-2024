using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainShip : MonoBehaviour 
{
    public List<Transform> points; // Noktaların referansları
    public float speed = 5f; // Hareket hızı
    public float rotationSpeed = 2f; // Dönüş hızı
    public float waitTime = 60f; // Bekleme süresi (saniye)
    public GameObject islandUI; // IslandUI referansı
    public Scrollbar islandScrollbar; // Scrollbar referansı

    private int currentPointIndex = 0;

    void Start()
    {
        // İlk noktaya gitmek için Coroutine başlat
        StartCoroutine(MoveToNextPoint());
        // IslandUI başlangıçta görünmez
        islandUI.SetActive(false);
        // Scrollbar başlangıçta sıfırlanır
        if (islandScrollbar != null)
        {
            islandScrollbar.size = 0;
        }
    }

    IEnumerator MoveToNextPoint()
    {
        while (true)
        {
            Transform targetPoint = points[currentPointIndex];

            // Hedef noktaya yönelene kadar dönüş
            while (Quaternion.Angle(transform.rotation, Quaternion.LookRotation(targetPoint.position - transform.position)) > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(targetPoint.position - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                yield return null; // Bir sonraki frame'e kadar bekle
            }

            // Hedef noktaya ulaşana kadar hareket et
            while (Vector3.Distance(transform.position, targetPoint.position) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);
                yield return null; // Bir sonraki frame'e kadar bekle
            }

            // Hedef noktaya ulaştığında yan dön
            Transform parentIsland = targetPoint.parent;
            if (parentIsland != null)
            {
                Vector3 directionToIsland = parentIsland.position - transform.position;
                Quaternion sideRotation = Quaternion.LookRotation(directionToIsland);
                sideRotation *= Quaternion.Euler(0, -90, 0); // Sağ yanını hedefe döndür
                while (Quaternion.Angle(transform.rotation, sideRotation) > 0.1f)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, sideRotation, rotationSpeed * Time.deltaTime);
                    yield return null; // Bir sonraki frame'e kadar bekle
                }

                // IslandUI öğelerini görünür yap
                islandUI.SetActive(true);

                // Scrollbar'ı sıfırla
                if (islandScrollbar != null)
                {
                    islandScrollbar.size = 0;
                }
            }

            // Hedef noktada bekle ve Scrollbar'ı doldur
            float elapsedTime = 0f;
            while (elapsedTime < waitTime)
            {
                elapsedTime += Time.deltaTime;
                if (islandScrollbar != null)
                {
                    islandScrollbar.size = elapsedTime / waitTime;
                }
                yield return null;
            }

            // IslandUI öğelerini tekrar görünmez yap
            islandUI.SetActive(false);

            // Sonraki noktaya geç
            currentPointIndex = (currentPointIndex + 1) % points.Count;
        }
    }
}
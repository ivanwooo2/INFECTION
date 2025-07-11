using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class soldierManager : MonoBehaviour
{
    private GameObject Player;
    [SerializeField] private float LockSpeed;
    [SerializeField] private GameObject BulletPrefab;
    [SerializeField] private int BulletCount;
    [SerializeField] private GameObject Shootingpoint;
    private float gotoframe = 0f;
    void Start()
    {
        Player = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        Vector3 direction = Player.transform.position - transform.position;
        float angle = Mathf.Atan2 (direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90);
    }
    public IEnumerator GoToTarget(Vector2 targetPosition,Vector2 startposition, float Timer)
    {
        gotoframe = 0f;
        while(gotoframe < Timer)
        {
            gotoframe += Time.deltaTime;
            this.transform.position = Vector3.Lerp(startposition, targetPosition,gotoframe/Timer);
            yield return null;
        }
        StartCoroutine(Shoot());
    }

    IEnumerator Shoot()
    {
        yield return new WaitForSeconds(2f);
        for (int i = 0; i < BulletCount; i++)
        {
            yield return new WaitForSeconds(1);
            GameObject Bullet = Instantiate(BulletPrefab, Shootingpoint.transform.position,Quaternion.identity);
        }
    }
}

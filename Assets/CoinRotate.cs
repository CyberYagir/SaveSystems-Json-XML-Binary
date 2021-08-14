using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinRotate : MonoBehaviour
{
    private void Start()
    {
        transform.localScale = Vector3.zero;
        transform.localEulerAngles = new Vector3(90, 0, 0);
    }
    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1, 0.125f, 1), 20 * Time.deltaTime);
        transform.localEulerAngles += new Vector3(0, 180, 0) * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            GameManager.AddMoney(1);
            Destroy(Instantiate(GameManager.GameObjectResources("PoofCoin"), transform.position, Quaternion.Euler(90,0,0)),1);
            Destroy(gameObject);
        }
    }
}

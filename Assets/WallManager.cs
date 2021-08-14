using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallManager : MonoBehaviour
{
    public GameObject left, right, up;
    // Update is called once per frame
    void Update()
    {
        right.transform.position = Vector3.Lerp(right.transform.position, new Vector3(Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, GameManager.distToPlayer)).x, right.transform.position.y, right.transform.position.z), 2 * Time.deltaTime);
        left.transform.position = Vector3.Lerp(left.transform.position, new Vector3(Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, GameManager.distToPlayer)).x, left.transform.position.y, left.transform.position.z), 2 * Time.deltaTime);
        up.transform.position = Vector3.Lerp(up.transform.position, new Vector3(up.transform.position.x, Camera.main.ScreenToWorldPoint(new Vector3(0, 0, GameManager.distToPlayer)).y-2, up.transform.position.z), 2 * Time.deltaTime);
        up.transform.localScale = Vector3.Lerp(up.transform.localScale, new Vector3(Vector3.Distance(right.transform.position, left.transform.position), up.transform.localScale.y, up.transform.localScale.z), 2 * Time.deltaTime);
    }
}

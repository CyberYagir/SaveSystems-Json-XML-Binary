using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] TMP_Text text,cubesCount;
    [SerializeField] Transform value;

    private void Start()
    {
    }
    void Update()
    {
        text.text = "Score: " + GameManager.coins;
        value.localScale = Vector3.Lerp(value.localScale, new Vector3(GameManager.coins / 10f, 1, 1), 5 * Time.deltaTime);
	cubesCount.text = "Count: " + GameManager.instance.players.Count;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadingText : MonoBehaviour
{
    private float fadeSpeed = 0.8f;
    private float moveSpeed = 1f;
    private Text text;
    private Vector3 startPos;
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        text = GetComponent<Text>();
        text.color = new Color(0, 0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        float alpha = text.color.a - fadeSpeed * Time.deltaTime;
        text.color = new Color(0, 0, 0, alpha);
        Vector3 pos = transform.position;
        transform.position = new Vector3(pos.x, pos.y + moveSpeed, pos.z);
    }
    public void Reset()
    {
        text.color = new Color(0, 0, 0, 1);
        transform.position = startPos;
    }
}

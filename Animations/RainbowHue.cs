using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

[ExecuteInEditMode]
public class RainbowHue : MonoBehaviour
{
    public float Speed = 1;
    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        rend.sharedMaterial.SetColor("_Color", Color.HSVToRGB(Mathf.PingPong(Time.time * Speed, 1), 1, 1));
    }
}

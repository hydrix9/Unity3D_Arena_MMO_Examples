using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class ReplacementShaderEffect : MonoBehaviour
{
    public Shader replacementShader;
    public Color overDrawColor;





    void OnValidate()
    {
        Shader.SetGlobalColor("_OverDrawColor", overDrawColor);
        Shader.SetGlobalInt("Stencil ID Reference", 1);
        Shader.SetGlobalFloat("Stencil Comparison", 3);
        Shader.SetGlobalFloat("Stencil Operation", 0);
        Shader.SetGlobalFloat("Stencil Write Mask", 255);
        Shader.SetGlobalFloat ("Stencil Read Mask", 255);



    }

    void OnEnable()
    {
        if (replacementShader == null && GetComponent<Renderer>() != null)
        { //else try get it off of own renderer
            GetComponent<Camera>().SetReplacementShader(GetComponent<Renderer>().sharedMaterial.shader, "RenderType");
           // GetComponent<Camera>().SetReplacementShader(GetComponent<Renderer>().sharedMaterial.shader, "Transparent");
        } else
        {

            GetComponent<Camera>().SetReplacementShader(replacementShader, "RenderType");

        }

        
        
    }

    void OnDisable()
    {
        GetComponent<Camera>().ResetReplacementShader();
    }
}
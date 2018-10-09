using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class RadialDistanceShader : MonoBehaviour
{

    private Material material;
    private Camera cam;

    void OnEnable()
    {
        // Create a material that uses the desired shader
        material = new Material(Shader.Find("Test/RadialDistance"));

        // Get the camera object (this script must be assigned to a camera)
        cam = GetComponent<Camera>();

        // Enable depth buffer generation#
        // (writes to the '_CameraDepthTexture' variable in the shader)
        cam.depthTextureMode = DepthTextureMode.Depth;
    }

    [ImageEffectOpaque] // Draw after opaque, but before transparent geometry
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        // Call custom Blit function
        // (usually Graphics.Blit is used)
        RaycastCornerBlit(source, destination, material);
    }


    void RaycastCornerBlit(RenderTexture source, RenderTexture destination, Material mat)
    {

        // Compute (half) camera frustum size (at distance 1.0)
        float angleFOVHalf = cam.fieldOfView / 2 * Mathf.Deg2Rad;
        float heightHalf = Mathf.Tan(angleFOVHalf);
        float widthHalf = heightHalf * cam.aspect;      // aspect = width/height

        // Compute helper vectors (camera orientation weighted with frustum size)
        Vector3 vRight = cam.transform.right * widthHalf;
        Vector3 vUp = cam.transform.up * heightHalf;
        Vector3 vFwd = cam.transform.forward;


        // Custom Blit
        // ===========

        // Set the given destination texture as the active render texture
        RenderTexture.active = destination;

        // Set the '_MainTex' variable to the texture given by 'source'
        mat.SetTexture("_MainTex", source);

        // Store current transformation matrix
        GL.PushMatrix();

        // Load orthographic transformation matrix
        // (sets viewing frustum from [0,0,-1] to [1,1,100])
        GL.LoadOrtho();

        // Use the first pass of the shader for rendering
        mat.SetPass(0);

        // Activate quad draw mode and draw a quad
        GL.Begin(GL.QUADS);
        {

            // Using MultiTexCoord2 (TEXCOORD0) and Vertex3 (POSITION) to draw on the whole screen
            // Using MultiTexCoord to write the frustum information into TEXCOORD1
            // -> When the shader is called, the TEXCOORD1 value is automatically an interpolated value

            // Bottom Left
            GL.MultiTexCoord2(0, 0, 0);
            GL.MultiTexCoord(1, (vFwd - vRight - vUp) * cam.farClipPlane);
            GL.Vertex3(0, 0, 0);

            // Bottom Right
            GL.MultiTexCoord2(0, 1, 0);
            GL.MultiTexCoord(1, (vFwd + vRight - vUp) * cam.farClipPlane);
            GL.Vertex3(1, 0, 0);

            // Top Right
            GL.MultiTexCoord2(0, 1, 1);
            GL.MultiTexCoord(1, (vFwd + vRight + vUp) * cam.farClipPlane);
            GL.Vertex3(1, 1, 0);

            // Top Left
            GL.MultiTexCoord2(0, 0, 1);
            GL.MultiTexCoord(1, (vFwd - vRight + vUp) * cam.farClipPlane);
            GL.Vertex3(0, 1, 0);

        }
        GL.End();   // Finish quad drawing

        // Restore original transformation matrix
        GL.PopMatrix();
    }
}
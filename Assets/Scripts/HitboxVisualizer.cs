// When player gets hit we use this to flash the hitbox so the player knows where it is

// based on WireFrame Rendering Script on Unity
// https://gist.github.com/naojitaniguchi/862724c55bd322695511

using UnityEngine;
using System.Collections;

public class HitboxVisualizer : MonoBehaviour
{

    public Color lineColor;
    public Color backgroundColor;
    public bool ZWrite = true;
    public bool AWrite = true;
    public bool blend = true;

    private bool showTheHitbox = false;

    private Vector3[] lines;
    private ArrayList linesArray;
    private Material lineMaterial;
    private MeshRenderer meshRenderer;

    private IEnumerator coroutine;


    void turnHitboxDisplayOn()
    {
        Debug.Log("Turning hitbox display on at time " + Time.fixedTime);
        showTheHitbox = true;
    }

    void turnHitboxDisplayOff()
    {
        Debug.Log("Turning hitbox display off at time " + Time.fixedTime);
        showTheHitbox = false;
    }

    // Flash the hitbox so the player knows where it is.
    // This uses a coroutine so it doesn't block the caller.
    public void flashHitbox()
    {
        turnHitboxDisplayOn();
        Debug.Log("Queued coroutine at time " + Time.fixedTime);
        coroutine = WaitAndTurnOffHitboxDisplay(1f);
        StartCoroutine(coroutine);
        Debug.Log("Done queueing coroutine at time " + Time.fixedTime);
    }

    private IEnumerator WaitAndTurnOffHitboxDisplay(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        turnHitboxDisplayOff();
    }

    // Use this for initialization
    void Start()
    {
        GetComponent<Renderer>().enabled = false;
        meshRenderer = GetComponent<MeshRenderer>();
        if (!meshRenderer)
        {
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
        }
        meshRenderer.material = new Material("Shader \"Lines/Background\" { Properties { _Color (\"Main Color\", Color) = (1,1,1,1) } SubShader { Pass {" + (ZWrite ? " ZWrite on " : " ZWrite off ") + (blend ? " Blend SrcAlpha OneMinusSrcAlpha" : " ") + (AWrite ? " Colormask RGBA " : " ") + "Lighting Off Offset 1, 1 Color[_Color] }}}");

        // Old Syntax without Bind :    
        //   lineMaterial = new Material("Shader \"Lines/Colored Blended\" { SubShader { Pass { Blend SrcAlpha OneMinusSrcAlpha ZWrite On Cull Front Fog { Mode Off } } } }"); 

        // New Syntax with Bind : 
        lineMaterial = new Material("Shader \"Lines/Colored Blended\" { SubShader { Pass { Blend SrcAlpha OneMinusSrcAlpha BindChannels { Bind \"Color\",color } ZWrite On Cull Front Fog { Mode Off } } } }");

        lineMaterial.hideFlags = HideFlags.HideAndDontSave;
        lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;

        linesArray = new ArrayList();
        MeshFilter filter = GetComponent<MeshFilter>();
        Mesh mesh = filter.sharedMesh;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        for (int i = 0; i < triangles.Length / 3; i++)
        {
            linesArray.Add(vertices[triangles[i * 3]]);
            linesArray.Add(vertices[triangles[i * 3 + 1]]);
            linesArray.Add(vertices[triangles[i * 3 + 2]]);
        }

        lines = new Vector3[triangles.Length];
        for (int i = 0; i < triangles.Length; i++)
        {
            lines[i] = (Vector3)linesArray[i];
        }
    }

    void OnRenderObject()
    {
        if (!showTheHitbox)
            return;

        meshRenderer.sharedMaterial.color = backgroundColor;
        lineMaterial.SetPass(0);

        GL.PushMatrix();
        GL.MultMatrix(transform.localToWorldMatrix);
        GL.Begin(GL.LINES);
        GL.Color(lineColor);

        for (int i = 0; i < lines.Length / 3; i++)
        {
            GL.Vertex(lines[i * 3]);
            GL.Vertex(lines[i * 3 + 1]);

            GL.Vertex(lines[i * 3 + 1]);
            GL.Vertex(lines[i * 3 + 2]);

            GL.Vertex(lines[i * 3 + 2]);
            GL.Vertex(lines[i * 3]);
        }

        GL.End();
        GL.PopMatrix();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
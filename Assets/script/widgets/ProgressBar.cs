using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Currently all images must be the same size
/// </summary>


[RequireComponent(typeof(SpriteRenderer))]
public class ProgressBar : MonoBehaviour
{
    public Texture2D u_background;
    public Texture2D u_forground;
    public Texture2D u_progressBar;
    
    private int m_id;
    private float m_percent;                    // percentage of image progress bar covers
    private Vector2 m_position;
    private Vector2 m_size;                     // size of finale image in pixels
    private SpriteRenderer m_spriteRenderer;

    private int counter = 0;

    public void Start()
    {
        initialize(Vector3.zero, Vector3.one, 0, 1.0f);
    }


    public void initialize(Vector3 position, Vector3 scale, int id, float percent = 0.0f)
    {
        m_id = id;
        m_position = position;
        m_percent = percent;

        transform.position = m_position;

        Debug.Log("ProgressBar.initialize() >> m_position = " + m_position);
        Debug.Log("ProgressBar.initialize() >> transform.position = " + transform.position);

        m_size.x = u_background.width;
        m_size.y = u_background.height;

        Debug.Log("ProgressBar.initialize() >> m_size = "+  m_size);

        if (m_size.x < u_progressBar.width)
            m_size.x = u_progressBar.width;
        if (m_size.y < u_progressBar.height)
            m_size.y = u_progressBar.height;

        if (m_size.x < u_forground.width)
            m_size.x = u_forground.width;
        if (m_size.y < u_forground.height)
            m_size.y = u_forground.height;

        Debug.Log("ProgressBar.initialize() >> m_size = " + m_size);

        m_spriteRenderer = GetComponent<SpriteRenderer>();

        m_spriteRenderer.sprite = Instantiate(
            m_spriteRenderer.sprite,
            m_position,
            Camera.main.transform.rotation);

        //MaterialPropertyBlock block = new MaterialPropertyBlock();
        //block.SetTexture("bar", combineTextures());
        //m_spriteRenderer.SetPropertyBlock(block);

        Texture2D tex = combineTextures();


        m_spriteRenderer.sprite = Sprite.Create(tex, m_spriteRenderer.sprite.rect, new Vector2(0.5f, 0.5f));

        // update name, main texture and shader, these all seem to be required... even thou you'd think it already has a shader :|
        m_spriteRenderer.sprite.name = m_spriteRenderer.name + "_sprite";
        m_spriteRenderer.material.mainTexture = tex as Texture;
        m_spriteRenderer.material.shader = Shader.Find("Sprites/Default");



    }

    public Texture2D combineTextures()
    {
        Texture2D tex = new Texture2D((int)m_size.x, (int)m_size.y);

        tex = overlayTexture(tex, u_background);
        tex = overlayTexture(tex, u_progressBar);
        tex = overlayTexture(tex, u_forground);

        tex.Apply();

        return tex;
    }

    private Texture2D overlayTexture(Texture2D baseTexture, Texture2D addTexture)
    {
        Texture2D tex = baseTexture;

        Debug.Log("ProgressBar.overlayTexture() >> START Combining Textures");

        int width = (tex.width - addTexture.width) / 2;
        int height = (tex.height - addTexture.height) / 2;

        int i = 0;
        int j = 0;
        while (i < tex.width)
        {
            while (j < tex.height)
            {
                Vector4 p = addTexture.GetPixel(i, j);

                if (j >= width && j < addTexture.width)
                {
                    if (i >= height && i < addTexture.height)
                    {
                        Vector4 pp = addTexture.GetPixel(i, j);
                        p = overlayPixel(p, pp);
                        
                    }
                }

                tex.SetPixel(i, j, p);
                j++;
            }
            i++;
        }

        Debug.Log("ProgressBar.overlayTexture() >> END Combining Textures");

        return tex;
    }



    private Vector4 overlayPixel(Vector4 p, Vector4 o)
    {
        counter++;
        if (counter == 9600)
        {
            Debug.Log("ProgressBar.overlayPixel() >> p = " + p + " o = " + o);
            counter = 0;
        }

        return new Vector4((p.x * p.w + o.x * o.w) / (1.0f * p.w + 1.0f * o.w),
                           (p.x * p.w + o.x * o.w) / (1.0f * p.w + 1.0f * o.w),
                           (p.x * p.w + o.x * o.w) / (1.0f * p.w + 1.0f * o.w),
                            p.w = p.w + o.w * (1.0f - p.w));

       //p.x = (p.x * p.w + pp.x * pp.w) / (1.0f * p.w + 1.0f * pp.w;
       //p.y = (p.y * p.w + pp.y * pp.w) / (1.0f * p.w + 1.0f * pp.w);
       //p.x = (p.z * p.w + pp.z * pp.w) / (1.0f * p.w + 1.0f * pp.w);
       //p.w = p.w + pp.w * (1.0f - p.w);
    }


}

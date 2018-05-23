using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Programmer: Gerald Coggins
/// 
/// Basic Button Funcitonality
/// -displays an image region in a set area
/// -button location/size can be changed and reset
/// -has an image for normal, pressed and disabled state
/// -returns true if a point is inside region
/// </summary>

[RequireComponent(typeof(SpriteRenderer))]
public class GuiButton : MonoBehaviour
{
    public enum ENUMbuttonState : uint
    {
        Hidden = 0,
        Up = 1,
        Down = 2,
        Disabled = 4,

    }
    public Vector2 u_scrPosition;             // default position of the button
    public Vector2 u_scrSize;                 // default size of the button
    public Sprite u_up;                    // texture of button when active and not pressed
    public Sprite u_down;                  // texture of button when active and pressed
    public Sprite u_disabled;              // texture of inactive button
    private SpriteRenderer m_spriteRenderer;  // reference to the spriterenderer
    private uint m_buttonId = 0;               // used by menu to keep track of the button
    private uint m_state = 0;                  // state of the button - uses ENUMbuttonState
    private Rect m_region = Rect.zero;        // current position and size of the button
    
    public void Start()
    {
        // set reference for the spriterenderer
        m_spriteRenderer = GetComponent<SpriteRenderer>();

        // create sprite in world
        m_spriteRenderer.sprite = Instantiate(
            m_spriteRenderer.sprite,
            Vector3.zero,
            Camera.main.transform.rotation);

        // set spriterenderer attributes
        m_spriteRenderer.sprite.texture.filterMode = FilterMode.Point;
        m_spriteRenderer.enabled = false;
    }

    //
    public void init(uint id, uint state, Vector3 parentWindowPositionPixels, Vector3 parentWindowSizePixels)
    {
        // assign button id
        m_buttonId = id;

        // set visible to true if the button is no hidden
        m_spriteRenderer.enabled = (state != (uint)ENUMbuttonState.Hidden);

        // set button state
        setState(state);

        // set button size and position
        setPosition(parentWindowPositionPixels, parentWindowSizePixels);

        // world object scale
        Vector3 scale = Vector3.one;

        float screenPosToWorldPosBottom = Camera.main.ScreenToWorldPoint(Vector2.zero).y;
        float screenPosToWorldPosTop = Camera.main.ScreenToWorldPoint(parentWindowSizePixels).y;

        double objectHeight = m_spriteRenderer.sprite.bounds.size.y;
        double worldHeight = screenPosToWorldPosTop - screenPosToWorldPosBottom;

        scale = scale * (float)(worldHeight / objectHeight) * u_scrSize.y;

        //Debug.Log("GuiButton.init() - screenPosToWorldPosBottom = (" + screenPosToWorldPosBottom + ")");
        //Debug.Log("GuiButton.init() - screenPosToWorldPosTop = (" + screenPosToWorldPosTop + ")");
        //Debug.Log("GuiButton.init() - parentWindowSizePixels = (" + parentWindowSizePixels.x + ", " + parentWindowSizePixels.y + ")");
        //Debug.Log("GuiButton.init() - objectHeight = (" + objectHeight + ")");
        //Debug.Log("GuiButton.init() - worldHeight = (" + worldHeight + ")");
        //Debug.Log("GuiButton.init() - scale = (" + scale.x + ", " + scale.y + ", " + scale.z + ")");

        m_spriteRenderer.transform.localScale = scale;
    }

    public void setState(uint state)
    {
        m_state = state;
        m_spriteRenderer.enabled = (m_state != (uint)ENUMbuttonState.Hidden);
        
        switch (m_state)
        {
            case (uint)ENUMbuttonState.Hidden:
                break;
            case (uint)ENUMbuttonState.Up:
                m_spriteRenderer.sprite = u_up;
                break;
            case (uint)ENUMbuttonState.Down:
                m_spriteRenderer.sprite = u_down;
                break;
            case (uint)ENUMbuttonState.Disabled:
                m_spriteRenderer.sprite = u_disabled;
                break;
            default: Debug.Log("GuiButton.Start() - default case should not happen"); break;
        }
    }

    // changes the position and size of the button
    private void setPosition(Vector3 parentWindowPositionPixels, Vector2 parentWindowSizePixels)
    {
        // get position to create graphic in pixels
        // offset in pixels + parentSize in pixels * buttonSize (percent)
        Vector2 screenPos = new Vector2(
            parentWindowPositionPixels.x + parentWindowSizePixels.x * u_scrPosition.x,
            parentWindowPositionPixels.y + parentWindowSizePixels.y * u_scrPosition.y);
        
        // get size of grphic in pixels
        Vector2 size = new Vector2(
                     parentWindowSizePixels.x * u_scrSize.x,
                     parentWindowSizePixels.y * u_scrSize.y);


        // set touch region rectangle
        //switch (m_state)
        //{
        //case (uint)ENUMbuttonState.Hidden: // hide if button is hidden
        //    m_region.Set(
        //        0,
        //        0,
        //        0,
        //        0);
        //    break;
        //    default: // else set region on screen

        // adjust region position to account for sprite pivot point
        m_region.Set(
                     screenPos.x - size.x / 2,
                     screenPos.y - size.y / 2,
                     size.x,
                     size.y);
       //         break;
       // }

        // resize immage
        resizeImage(size);

        // get world position and size to take the same space on screen
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);

        // set z axis offset
        worldPos.z = 5.0f;

        // set position
        m_spriteRenderer.transform.position = worldPos;
    }

    // resizes all images related to the button
    private void resizeImage(Vector2 size)
    {
        //u_up.Resize((int)size.x, (int)size.y);
        //u_down.Resize((int)size.x, (int)size.y);
        //u_disabled.Resize((int)size.x, (int)size.y);
    }

    // sets button to pressed state
    public uint press(uint id)
    {
        //Debug.Log("GuiBUtton.press() - Button [" + id + "] - " +
        //    "button state is up = " + (m_state == (uint)ENUMbuttonState.Up) + " - " +
        //    "button Id is included in flag = " + ((id & m_buttonId) == m_buttonId));

        // only do if button is in up state
        if (m_state != (uint)ENUMbuttonState.Up || (id & m_buttonId) != m_buttonId)
            return 0;

        //Debug.Log("guibutton.press()");

        //Debug.Log("GuiButton.onPress()- m_region position " + m_region.position);
        //Debug.Log("GuiButton.onPress()- m_region size " + m_region.size);

        //Debug.Log("GuiButton.onPress()- button was pressed at " + touchPos);
        //Debug.Log("button [" + m_buttonId + "] pressed");
        m_state = (uint)ENUMbuttonState.Down;

        //m_spriteRenderer.sprite = Sprite.Create(u_down, new Rect(1, 1, 1, 1), Vector2.zero);
        //u_material.SetTexture((uint)ENUMbuttonState.Down, u_down);
        return m_buttonId;
    }

    public bool contains(Vector2 touchPos)
    {
        //Debug.Log("GuiButton.contains() touchPos = " + touchPos);
        //Debug.Log("GuiButton.contains() buttonPos = " + m_region.x + ", " + m_region.y);
        //Debug.Log("GuiButton.contains() buttonSize = " + m_region.size.x + ", " + m_region.size.y);
        //Debug.Log("GuiButton.contains() bool = " + m_region.Contains(touchPos));
        return m_region.Contains(touchPos);
    }

    // sets button to released state
    public uint release(uint id)
    {
        //Debug.Log("GuiBUtton.release() - Button [" + id + "] - " +
        //    "button state is down = " + (m_state == (uint)ENUMbuttonState.Down) + " - " +
        //    "button Id is included in flag = " + ((id & m_buttonId) == m_buttonId));

        //Debug.Log("guibutton.release() - id = " + id);
        //Debug.Log("guibutton.release() - m_state = " + m_state);
        //Debug.Log("guibutton.release() - m_buttonId = " + m_buttonId);
        // only do if button is in down state and has the correct id
        if (m_state != (uint)ENUMbuttonState.Down || (id & m_buttonId) != m_buttonId)
            return 0;

        //Debug.Log("guibutton.release()");

        //Debug.Log("GuiButton.onRelease()- button was released");

        //Debug.Log("button [" + m_buttonId + "] released");
        m_state = (uint)ENUMbuttonState.Up;
        //m_spriteRenderer.sprite = Sprite.Create(u_up, new Rect(1, 1, 1, 1), Vector2.zero);
        //u_material.SetTexture((uint)ENUMbuttonState.Up, u_up);

        return m_buttonId;
    }

    // returns id if button is in pressed state and has the correct id
    public uint getIsPressed(uint id)
    {
        //Debug.Log("GuiBUtton.getIsPressed() - Button [" + id + "] - " +
        //    "- button state is down = " + (m_state == (uint)ENUMbuttonState.Down)+ " " +
        //    "- button Id is included in flag = " + ((id & m_buttonId) == m_buttonId));

        //Debug.Log("guibutton.getIsPressed() - id = " + id);
        //Debug.Log("guibutton.getIsPressed() - m_state = " + m_state);
        //Debug.Log("guibutton.getIsPressed() - m_buttonId = " + m_buttonId);
        //Debug.Log("guibutton.getIsPressed() - m_state != (uint)ENUMbuttonState.Down = " + (m_state != (uint)ENUMbuttonState.Down));
        //Debug.Log("guibutton.getIsPressed() - (id & m_buttonId) != id = " + ((id & m_buttonId) != id));
        if (m_state != (uint)ENUMbuttonState.Down || (id & m_buttonId) != m_buttonId)
            return 0;

        //Debug.Log("GuiBUtton.getIsPressed() - Button [" + id + "] is pressed - button Id = " + m_buttonId);

        //Debug.Log("guibutton.getispressed - " + m_buttonId + " = " + (id & m_buttonId));

        return m_buttonId;
    }

    // returns the button's id
    public uint getId()
    {
        return m_buttonId;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Programmer: Gerald Coggins
/// 
/// </summary>

[RequireComponent(typeof(SpriteRenderer))]
public class SystemMenu : MonoBehaviour
{
    public enum ENUMsysButtonId
    {
        Continue = 1,
        Save = 2,
        Load = 4,
        Options = 8,
        Quit = 16,


        All = Continue+Save+Load+Options+Quit,
        Count = 5,
        None = 0,
    }
    public Vector2 u_scrPosition;                       // (x, y) position (percent) on the parent window
    public Vector2 u_scrSize;                           // (x, y) size (percent) on the parent window
    public float u_zPos;                                // z position in the world
    public GuiButton u_buttonContinue;                  // button component
    public GuiButton u_buttonSave;                      // button component
    public GuiButton u_buttonLoad;                      // button component
    public GuiButton u_buttonOptions;                   // button component
    public GuiButton u_buttonQuit;                      // button component
    private int[] m_buttonStates = { 0, 0, 0, 0, 0 };   // array of the states of all buttons
    private SpriteRenderer m_spriteRenderer;            // reference to spriterenderer
    private int m_componentId = 0;                      // id for the guihandler to keep track of componenets
    private Rect m_systemMenuMainRegion;                // touchable region on screen

    public void Start()
    {
        // get reference to spriteRenderer
        m_spriteRenderer = GetComponent<SpriteRenderer>();

        // set spriterenderer attributes
        m_spriteRenderer.sprite.texture.filterMode = FilterMode.Point;
        m_spriteRenderer.enabled = false;

        // create sprite in world
        m_spriteRenderer.sprite = Instantiate(
            m_spriteRenderer.sprite,
            new Vector3(
            Camera.main.transform.position.x,
            Camera.main.transform.position.y,
            u_zPos),
            Camera.main.transform.rotation);


        // create buttons
        u_buttonContinue = Instantiate(
            u_buttonContinue,
            new Vector3(
            Camera.main.transform.position.x,
            Camera.main.transform.position.y,
            u_zPos),
            Camera.main.transform.rotation);
        // create buttons
        u_buttonQuit = Instantiate(
            u_buttonQuit,
            new Vector3(
            Camera.main.transform.position.x,
            Camera.main.transform.position.y,
            u_zPos),
            Camera.main.transform.rotation);
    }

    // sets id and sets sprite visible, sets world object and touch area size, scale and position
    public void init(bool isActive, int componentId, Vector2 screenSize, Vector2 hidePos)
    {
        m_componentId = componentId;
        setActive(isActive, screenSize, hidePos);
    }

    // sets sprite visible, sets world object and touch area size, scale and position
    public void setActive(bool isActive, Vector2 screenSize, Vector2 hidePos)
    {
        // set wether sprite is visible or not
        m_spriteRenderer.enabled = isActive;

        Vector3 position = hidePos;
        Vector2 size = Vector2.zero;
        Vector3 scale = new Vector2(1, 1);

        // screenPixels * UIpos(percent)
        position.x = screenSize.x * u_scrPosition.x;
        position.y = screenSize.y * u_scrPosition.y;
        position.z = u_zPos;

        // screenPixels * UIsize(percent)
        size.x = screenSize.x * u_scrSize.x;
        size.y = screenSize.y * u_scrSize.y;

        // world and object height
        double objectHeight = m_spriteRenderer.sprite.bounds.size.y;
        double worldHeight = Camera.main.orthographicSize * 2.0;
        
        // set scale so world object takes the correct number of pixels
        scale = scale * (float)(worldHeight / objectHeight);

        //Debug.Log("SystemMenu.setActive - objectHeight (" + objectHeight + ")");
        //Debug.Log("SystemMenu.setActive - worldHeight (" + worldHeight + ")");
        //Debug.Log("GuiButton.init() - scale = (" + scale.x + ", " + scale.y + ", " + scale.z + ")");

        // set up buttons
        u_buttonContinue.init((int)ENUMsysButtonId.Continue, (int)GuiButton.ENUMbuttonState.Up, position, size);
        u_buttonQuit.init((int)ENUMsysButtonId.Quit, (int)GuiButton.ENUMbuttonState.Up, position, size);
        if (isActive == true)
        {
            u_buttonContinue.setState((int)GuiButton.ENUMbuttonState.Up);
            u_buttonQuit.setState((int)GuiButton.ENUMbuttonState.Up);
        }
        else
        {
            u_buttonContinue.setState((int)GuiButton.ENUMbuttonState.Hidden);
            u_buttonQuit.setState((int)GuiButton.ENUMbuttonState.Hidden);
        }

        //u_buttonSave.init((int)ENUMsysButtonId.Save, (int)GuiButton.ENUMbuttonState.Up, position, size);
        //u_buttonLoad.init((int)ENUMsysButtonId.Load, (int)GuiButton.ENUMbuttonState.Up, position, size);
        //u_buttonOptions.init((int)ENUMsysButtonId.Options, (int)GuiButton.ENUMbuttonState.Up, position, size);
        //u_buttonQuit.init((int)ENUMsysButtonId.Quit, (int)GuiButton.ENUMbuttonState.Up, position, size);

        //Debug.Log("SystemMenu.setActive() - position = " + position);
        //Debug.Log("SystemMenu.setActive() - size = " + size);

        // set touchable region
        // adjust position ot accoutn for sprite pivot point
        m_systemMenuMainRegion.Set(
            position.x,
            position.y,
            size.x,
            size.y);

        //Debug.Log("SystemMenu.setActive() - position = (" + position.x + ", " + position.y + ")");
        //Debug.Log("SystemMenu.setActive() - size = (" + size.x + ", " + size.y + ")");

        // apply position and scale to world object
        m_spriteRenderer.transform.position = position;
        m_spriteRenderer.transform.localScale = scale;
    }

    // sets wether spriteRenderer is visible or not
    public void setVisible(bool isVisible)
    {
        m_spriteRenderer.enabled = isVisible;
    }

    // sets position of world object
    public void setPosition(Vector2 pos)
    {
        m_spriteRenderer.transform.position = new Vector3(pos.x, pos.y, u_zPos);
    }
    
    // returns Id
    public int getId()
    {
        return m_componentId;
    }

    // action for on press
    public void onPress(Vector2 touchPos)
    {
        if (u_buttonContinue.contains(touchPos))
            u_buttonContinue.press(u_buttonContinue.getId());
        else
            u_buttonContinue.release(u_buttonContinue.getId());

        if (u_buttonQuit.contains(touchPos))
            u_buttonQuit.press(u_buttonQuit.getId());
        else
            u_buttonQuit.release(u_buttonQuit.getId());

    }

    // returns flags for press buttons
    public int onRelease()
    {
        int result = (int)ENUMsysButtonId.None;

        // add flags for released buttons
        result += u_buttonContinue.release(u_buttonContinue.getId());
        result += u_buttonQuit.release(u_buttonQuit.getId());

        //Debug.Log("SystemMenu.onRelease()- system menu button was released: " + result);

        return result;
    }

    public bool contains(Vector2 touchPos)
    {
        return m_systemMenuMainRegion.Contains(touchPos);
    }

    // returns bitmask dof all pressed buttons
    public int getPressedButtons(int id)
    {
        int result = (int)ENUMsysButtonId.None;

        result += u_buttonContinue.getIsPressed(id);
        result += u_buttonQuit.getIsPressed(id);

        Debug.Log("SystemMenu.getPressedButtons() - id = " + id);
        Debug.Log("SystemMenu.getPressedButtons() - result = " + result);

        return result;
    }
}

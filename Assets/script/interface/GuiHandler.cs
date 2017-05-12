using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Programmer: Gerald Coggins
/// 
/// Script to Control Player Movement
/// 
/// -Control player position
/// -Move the player with touch screen: movement is relative to current touch point from initial touch point
/// -Create a graphic to show inital touch location
/// </summary>

public class GuiHandler : MonoBehaviour
{

    /*
     * Get number of touches
     * cycle through touches
     *  -check location of touch
     *  -check if in gui region
     *  -assign gui region id to touch id
     * cycle through touches, check id and do stuff
     *  -switch(touchId[guiRegionID])
     *  --do stuff
     *  redo all ID's if touch count changes
     *  cycle through touched regions to check if still touched
     *  -do stuff
     *  cycle though regions updating time change
     *  
     * 
     * 
     * 
     */

    //////////////////////////////////////////
    // GUI ELEMENT VARIABLES
    //////////////////////////////////////////

    // bitmask index for gui elements
    enum ENUMguiIndex
    {
        None = 0,       // default value no element is touched
        Joystick = 1,   // x and y axis input from initial touch point
        Swipe = 2,      // tap/direction input for touch point
        SystemMenu = 4, // continue, quit, save, load, options
        GameMenu = 8,
        MainGame = Joystick+Swipe,
    }

    // GUI components
    public TouchJoystick u_joystick;
    public SimpleSwipe u_swipe;
    public SystemMenu u_systemMenu;

    // player variables
    public GameObject u_player;
    public float u_joySpeed;           // TODO: move these outside joystick // base speed the joysitck moves the player
    public float u_joySpeedMultMax;    // TODO: move these outside joystick // max multiplier for the base speed
    public float u_joySpeedMultMin;    // TODO: move these outside joystick // min multiplier for the base speed

    private int m_screenWidth = 0;    // hold screen width to not call Screen.width
    private int m_screenHeight = 0;   // hold screen height to not call Screen.height

    public Vector3 u_hideObjectPosition; // location where graphics are hidden when not in use
    //public Vector3 u_hiddenUILocation; // location where graphics are hidden when not in use
    
    private int m_guiTouchStateMask = 0;        // bitmask for touched gui state -- uses ENUMuigIndex
    private int m_guiActiveUIStateMask = 0;        // bitmask for visible gui state -- uses ENUMuigIndex
    private int m_numTouches = 0;          // number of touches -- used to check for changes in number of touches
    private int m_maxTouchCount = 10;      // max number of simultanious touches -- touchIndex must have space
    private int[] m_touchIndex = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };  // holds the gui index for each touch

    /////////////////////////////////////////////
    //
    //      JOYSTICK INTERFACE METHODS
    //
    /////////////////////////////////////////////

    private void interfaceJoystickOnPress(Vector2 touchPos)
    {
        u_joystick.onPress(touchPos);
    }
    private void interfaceJoystickOnHold(Vector2 touchPos)
    {
        // get joystick output
        Vector3 axis = u_joystick.onHold(touchPos);

        // player movement method
        float speedMult = Mathf.Clamp(axis.z, u_joySpeedMultMin, u_joySpeedMultMax);

        // adjust for framerate
        axis.x *= Time.deltaTime;
        axis.y *= Time.deltaTime;

        // move player
        u_player.transform.Translate(
            axis.x * u_joySpeed * speedMult,
            axis.y * u_joySpeed * speedMult,
            0);
        // end player movement method
    }
    private void interfaceJoystickOnRelease()
    {
        u_joystick.onRelease(u_hideObjectPosition);
    }
    
    /////////////////////////////////////////////
    //
    //      SWIPE INTERFACE METHODS
    //
    /////////////////////////////////////////////

    private void interfaceSwipeOnPress(Vector2 touchPos)
    {
        u_swipe.onPress(touchPos);
    }

    private void interfaceSwipeOnHold(Vector2 touchPos)
    {
        u_swipe.onHold(touchPos);
    }

    // performs a swipe action function depending on swipe direction
    private void interfaceSwipeOnRelease()
    {
        int output = u_swipe.onRelease();

        switch (output)
        {
            case (int)SimpleSwipe.ENUMswipeOutput.Right: swipeActionRight(); break;
            case (int)SimpleSwipe.ENUMswipeOutput.Left: swipeActionLeft(); break;
            case (int)SimpleSwipe.ENUMswipeOutput.Up: swipeActionUp(); break;
            case (int)SimpleSwipe.ENUMswipeOutput.Down: swipeActionDown(); break;
            case (int)SimpleSwipe.ENUMswipeOutput.Tap: swipeActionTap(); break;
            case (int)SimpleSwipe.ENUMswipeOutput.None: break;
            default: Debug.Log("playerController.swipeOutput() >> default case should not happen"); break;
        }
    }

    private void swipeActionTap()
    {
        /////////////////////////////////
        // do stuff here

        // move player to center
        u_player.transform.position = Vector3.zero;
        /////////////////////////////////
    }
    private void swipeActionRight()
    {
        /////////////////////////////////
        // do stuff here
        /////////////////////////////////
    }
    private void swipeActionLeft()
    {
        /////////////////////////////////
        // do stuff here

        u_systemMenu.setPosition(new Vector2(10, 10));
        /////////////////////////////////
    }
    private void swipeActionUp()
    {
        /////////////////////////////////
        // do stuff here
        /////////////////////////////////
    }
    private void swipeActionDown()
    {
        /////////////////////////////////
        // do stuff here
        Application.Quit();
        /////////////////////////////////
    }

    /////////////////////////////////////////////
    //
    //      SYSTEMMENU INTERFACE METHODS
    //
    /////////////////////////////////////////////

    // toggle system mune on and off
    private void interfaceSystemMenuOpen()
    {
        int flag = u_systemMenu.getId();
        Vector2 screen = new Vector2(m_screenWidth, m_screenHeight);

        Debug.Log("GuiHandler.interfaceSystemMenuOpen() - u_systemMenu.getId() = " + flag);
        
        m_numTouches = 0;

        for (int i = 0; i < m_numTouches; i++)
        {
            m_touchIndex[i] = 0;
        }

        // close menu if open
        if ((flag & m_guiActiveUIStateMask) == flag)
        {
            Debug.Log("Close System Menu");
            u_systemMenu.setActive(false, screen, u_hideObjectPosition);

            m_guiTouchStateMask = (int)ENUMguiIndex.None;
            m_guiActiveUIStateMask = (int)ENUMguiIndex.MainGame;
        }
        // open menu if closed
        else
        {
            Debug.Log("Open System Menu");
            u_systemMenu.setActive(true, screen, u_hideObjectPosition);
            // clear previous touch flags and add system menu flag
            m_guiTouchStateMask = (int)ENUMguiIndex.None;
            m_guiActiveUIStateMask = flag;


        }
    }

    private void interfaceSystemMenuOnPress(Vector2 touchPos)
    {
        u_systemMenu.onPress(touchPos);
    }

    private void interfaceSystemMenuOnHold()
    {
        // make this represent pressed buttons
        //m_guiTouchStateMask = u_systemMenu.getPressedButtons((int)SystemMenu.ENUMsysButtonId.All);
    }

    private void interfaceSystemMenuOnRelease(int buttonFlag)
    {
        /////////////////////////////////
        // do stuff here
        // get flags of buttons that were released
        int button = u_systemMenu.onRelease();

        Debug.Log("GuiHandler.interfaceSysMenuRelease()" + button);

        switch(buttonFlag)
        {
            case (int)SystemMenu.ENUMsysButtonId.Continue:
                {
                    interfaceSystemMenuOpen();
                    break;
                }
            case (int)SystemMenu.ENUMsysButtonId.Quit:
                {
                    Application.Quit();
                    break;
                }
            default: break;
        }
        /////////////////////////////////
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////
    //
    //      AUTOMATIC METHODS
    //
    /////////////////////////////////////////////

    void Start()
    {
        // TODO: account for portrait vs landscape orientation

        // set screen size
        m_screenWidth = Screen.width;
        m_screenHeight = Screen.height;

        m_guiActiveUIStateMask = (int)ENUMguiIndex.MainGame;
        
        // set initial player position
        u_player.transform.position = Vector3.zero;

        Vector2 screenSize = new Vector2(m_screenWidth, m_screenHeight);

        u_joystick.init(true, (int)ENUMguiIndex.Joystick, screenSize, u_hideObjectPosition);
        u_swipe.init(true, (int)ENUMguiIndex.Swipe, screenSize, u_hideObjectPosition);
        u_systemMenu.init(false, (int)ENUMguiIndex.SystemMenu, screenSize, u_hideObjectPosition);
    }

    void Update()
    {
        // create a copy of previous state to compare to current state
        int guiTouchStateMaskPrev = m_guiTouchStateMask;

        // get current touch count
        int touchCount = Input.touchCount;
        if (touchCount > m_maxTouchCount)
            touchCount = m_maxTouchCount;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            updateBackKeyPressed(); // handles gui state - brings up system menu
        }
        else
        {
            // check if touch count has changed
            if (touchCount != m_numTouches)
                updateTouchIndex(touchCount);
            
            updateTouchOnHold();     // performs action on hold for each touch
            updateTouchOnRelease(guiTouchStateMaskPrev);  // performs on release for each touch
        }


    }

    /////////////////////////////////////////////
    //
    //      KEY METHODS
    //
    /////////////////////////////////////////////

    // runs when back is pressed - controls gui state
    private void updateBackKeyPressed()
    {
        interfaceSystemMenuOpen();
    }

    /////////////////////////////////////////////
    //
    //      TOUCH METHODS
    //
    /////////////////////////////////////////////

    // assigns a gui id to each touchIndex[] for each touch and adds gui enum to mask
    // where (int touchCount) is current number of touches
    // also gui regions are initialized
    private void updateTouchIndex(int touchCount)
    {
        Debug.Log("Updating touch Index");
        // update numTouches for comparison next update
        m_numTouches = touchCount;

        // reset gui state
        m_guiTouchStateMask = 0;

        // reset touch indexes
        for(int i = 0; i < m_maxTouchCount; i++)
        {
            m_touchIndex[i] = (int)ENUMguiIndex.None;
        }
        
        // assign a GUI index to each touch
        // cycles through each touch
        int touchId = 0;
        while (touchId < touchCount)
        {
            // get postion of touch
            Vector2 touchPos = Input.GetTouch(touchId).position;
            
            // check game state
            switch(m_guiActiveUIStateMask)
            {
                case (int)ENUMguiIndex.MainGame:
                    {
                        Debug.Log("GuiHandler.updateTouch - Main Game State Active");
                        // check if point is inside rectangle and set touchIndex[touchId] if true
                        // the order of these controls the priority - first is higher priority
                        if (u_joystick.contains(touchPos))
                        {
                            // do if gui element is not active
                            if (((int)ENUMguiIndex.Joystick & m_guiTouchStateMask) != (int)ENUMguiIndex.Joystick)
                            {
                                Debug.Log("Touch Hold Position " + touchId + " is in the Joystick region");

                                // register touch gui element to touchId
                                m_touchIndex[touchId] = (int)ENUMguiIndex.Joystick;

                                // add gui element flag to gui state
                                m_guiTouchStateMask |= (int)ENUMguiIndex.Joystick;

                                /////////////////////////////////
                                // do stuff here
                                /////////////////////////////////
                                interfaceJoystickOnPress(touchPos);
                            }
                        }
                        else if (u_swipe.contains(touchPos))
                        {
                            // do if gui element is not active
                            if (((int)ENUMguiIndex.Swipe & m_guiTouchStateMask) != (int)ENUMguiIndex.Swipe)
                            {
                                Debug.Log("Touch Hold Position " + touchId + " is in the Swipe region");

                                // register touch gui element to touchId
                                m_touchIndex[touchId] = (int)ENUMguiIndex.Swipe;

                                // add gui element flag to gui state
                                m_guiTouchStateMask |= (int)ENUMguiIndex.Swipe;

                                /////////////////////////////////
                                // do stuff here
                                interfaceSwipeOnPress(touchPos);
                                /////////////////////////////////
                            }
                        }
                        else
                            Debug.Log("Touch Hold Position " + touchId + " is not in any region");
                        break;
                    }
                case (int)ENUMguiIndex.SystemMenu:
                    {
                        //Debug.Log("GuiHandler.updateTouch - System Menu State Active");
                        //Debug.Log("GuiHandler.updateTouchRegion() - BEFORE m_guiTouchStateMask  = " + m_guiTouchStateMask);
                        if (u_systemMenu.contains(touchPos))
                        {
                            Debug.Log("1");
                            /////////////////////////////
                            // do stuff here
                            interfaceSystemMenuOnPress(touchPos); // systemmenu.onpress()

                            /////////////////////////////
                            // only works for bitflag enum types
                            m_touchIndex[touchId] = u_systemMenu.getPressedButtons((int)SystemMenu.ENUMsysButtonId.All);
                            m_guiTouchStateMask |= m_touchIndex[touchId];
                            Debug.Log("GuiHandler.updateTouchRegion() - DURING m_guiTouchStateMask  = " + m_guiTouchStateMask);
                            Debug.Log("GuiHandler.updateTouchRegion() - DURING m_touchIndex[touchId]  = " + m_touchIndex[touchId]);

                        }
                        else
                            Debug.Log("GuiHandler.updateTouchIndex() - not touching system menu");

                        
                        //Debug.Log("GuiHandler.updateTouchRegion() - AFTER m_guiTouchStateMask  = " + m_guiTouchStateMask);
                        break;
                    }
                default: Debug.Log("GuiHandler.updateTouchRegion() - Unknown Game State");  break;
            }
            
            touchId++;
        } // end while (i < touchCount)
    }

    // performs action on hold for each touch
    private void updateTouchOnHold()
    {
        int touchId = 0;
        while(touchId < m_numTouches)
        {
            Vector2 touchPos = Input.GetTouch(touchId).position;

            // check game state
            switch(m_guiActiveUIStateMask)
            {
                case (int)ENUMguiIndex.MainGame:
                    {
                        switch (m_touchIndex[touchId])
                        {
                            case (int)ENUMguiIndex.None:
                                {
                                    /////////////////////////////////
                                    // do stuff here
                                    /////////////////////////////////

                                    break;
                                } // end case (int)ENUMguiIndex.Joystick
                            case (int)ENUMguiIndex.Joystick:
                                {
                                    // break if gui element is no longer touched
                                    if (((int)ENUMguiIndex.Joystick & m_guiTouchStateMask) != (int)ENUMguiIndex.Joystick)
                                        break;

                                    // Debug.Log("Touch Position " + touchId + ": Joystick region held");

                                    /////////////////////////////////
                                    // do stuff here
                                    interfaceJoystickOnHold(touchPos);
                                    /////////////////////////////////

                                    break;
                                } // end case (int)ENUMguiIndex.Joystick
                            case (int)ENUMguiIndex.Swipe:
                                {
                                    // break if gui element is no longer touched
                                    if (((int)ENUMguiIndex.Swipe & m_guiTouchStateMask) != (int)ENUMguiIndex.Swipe)
                                        break;

                                    //Debug.Log("Touch Position " + touchId + ": Swipe region held");

                                    /////////////////////////////////
                                    // do stuff here
                                    interfaceSwipeOnHold(touchPos);
                                    /////////////////////////////////

                                    break;
                                } // end case (int)ENUMguiIndex.Swipe
                            default: Debug.Log("playerController.updateTouchHoldAction() >> default case should not happen"); break;
                        }
                        break;
                    }
                case (int)ENUMguiIndex.SystemMenu:
                    {
                        interfaceSystemMenuOnHold();
                        break;
                    }
            }



               
            touchId++;
        } // end while(touchId < m_numTouches)
    }

    // performs action on release for each touch
    private void updateTouchOnRelease(int guiTouchStatePrev)
    {
        if (m_guiTouchStateMask >= guiTouchStatePrev)
            return;

        Debug.Log("updateTouchOnRelease() - m_guiActiveUIStateMask " + m_guiActiveUIStateMask);
        Debug.Log("updateTouchOnRelease() - guiTouchStatePrev " + guiTouchStatePrev);
        //Debug.Log("updateTouchOnRelease() - m_guiTouchStateMask " + m_guiTouchStateMask);

        // check game state
        switch (m_guiActiveUIStateMask)
        {
            case (int)ENUMguiIndex.MainGame:
                {
                    //Debug.Log("GuiHandler.onRelease - Main Game State Active");
                    if (((int)ENUMguiIndex.Joystick & guiTouchStatePrev) == (int)ENUMguiIndex.Joystick &&
                        ((int)ENUMguiIndex.Joystick & m_guiTouchStateMask) != (int)ENUMguiIndex.Joystick)
                    {
                        Debug.Log("Touch Joystick region released");

                        /////////////////////////////////
                        // do stuff here
                        interfaceJoystickOnRelease();
                        /////////////////////////////////
                    }

                    if (((int)ENUMguiIndex.Swipe & guiTouchStatePrev) == (int)ENUMguiIndex.Swipe &&
                        ((int)ENUMguiIndex.Swipe & m_guiTouchStateMask) != (int)ENUMguiIndex.Swipe)
                    {
                        Debug.Log("Touch Swipe region released");

                        /////////////////////////////////
                        // do stuff
                        interfaceSwipeOnRelease();
                        /////////////////////////////////
                    }
                    break;
                }
            case (int)ENUMguiIndex.SystemMenu:
                {
                    //Debug.Log("GuiHandler.onRelease - guiTouchStatePrev    = " + guiTouchStatePrev);
                    //Debug.Log("GuiHandler.onRelease - m_guiTouchStateMask  = " + m_guiTouchStateMask);
                    // run omly if a button was released
                    //if (m_guiTouchStateMask < guiTouchStatePrev)
                    // check each region for if it was held last upadate and is NOT held this update
                    if (((int)SystemMenu.ENUMsysButtonId.Continue & guiTouchStatePrev) == (int)SystemMenu.ENUMsysButtonId.Continue &&
                        ((int)SystemMenu.ENUMsysButtonId.Continue & m_guiTouchStateMask) != (int)SystemMenu.ENUMsysButtonId.Continue)
                    {
                        //Debug.Log("GuiHandler.onRelease - System Menu Continue button released");
                        interfaceSystemMenuOnRelease((int)SystemMenu.ENUMsysButtonId.Continue);
                    }

                    if (((int)SystemMenu.ENUMsysButtonId.Quit & guiTouchStatePrev) == (int)SystemMenu.ENUMsysButtonId.Quit &&
                        ((int)SystemMenu.ENUMsysButtonId.Quit & m_guiTouchStateMask) != (int)SystemMenu.ENUMsysButtonId.Quit)
                    {
                        //Debug.Log("GuiHandler.onRelease - System Menu Quit button released");
                        //interfaceSystemMenuOnRelease((int)SystemMenu.ENUMsysButtonId.Quit);
                    }
                    break;
                }
            default: Debug.Log("GuiHandler.updateTouchOnRelease() - Unknown Game State"); break;
        }
    }
    
    /////////////////////////////////////////////////////////////////////////////////
    //
    //      OLD CODE BIT
    //
    /////////////////////////////////////////////////////////////////////////////////


    /*
    if (graphicScale == true)
    {
        /////////////////////////////////////////////
        // CHANGE TOUCHARROW SCALE
        /////////////////////////////////////////////

        float dist = currentTouchPosDistance_region1;
        dist = Mathf.Clamp(dist, 0, maxTouchDistance_region1);
        //dist = Mathf.Clamp(dist, minTouchArrowSize_region1, maxTouchArrowSize_region1);

        float ps = dist / maxTouchDistance_region1 * touchArrowScale_region1.x;
        float cs = Mathf.Clamp((1.0f - dist / maxTouchDistance_region1) *
            2.0f,
            0.3f,
            10.0f);

        Vector3 parentScale = new Vector3(ps, ps, 1);
        Vector3 childscale = new Vector3(1, cs, 1);

        //Debug.Log("parentScale = " + parentScale);
        //Debug.Log("childScale = " + childscale);

        arrowPrefab_region1.transform.localScale = parentScale;
        arrowPrefab_region1.transform.GetChild(graphicChildIndex_region1).transform.localScale = childscale;
    }
// */
}

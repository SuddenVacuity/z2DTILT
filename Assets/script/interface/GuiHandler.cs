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
    enum ENUMguiIndex : uint
    {
        None = 0,       // default value no element is touched
        Joystick = 1,   // x and y axis input from initial touch point
        Swipe = 2,      // tap/direction input for touch point
        SystemMenu = 4, // continue, quit, save, load, options
        GameMenu = 8,

        MainGame = Joystick+Swipe,
        RunRelease = 2147483648, // included in state if onRelease should run // value of highest bit
    }

    // GUI components
    public TouchJoystick u_joystick;
    public SimpleSwipe u_swipe;
    public GameWindow u_systemMenu;

    // player variables
    public GameObject u_player;
    public float u_joySpeed;           // TODO: move these outside joystick // base speed the joysitck moves the player
    public float u_joySpeedMultMax;    // TODO: move these outside joystick // max multiplier for the base speed
    public float u_joySpeedMultMin;    // TODO: move these outside joystick // min multiplier for the base speed

    private int m_screenWidth = 0;    // hold screen width to not call Screen.width
    private int m_screenHeight = 0;   // hold screen height to not call Screen.height

    public Vector3 u_hideObjectPosition; // location where graphics are hidden when not in use
    //public Vector3 u_hiddenUILocation; // location where graphics are hidden when not in use
    
    private uint m_guiTouchStateMask = 0;        // bitmask for touched gui state -- uses ENUMuigIndex
    private uint m_guiActiveUIStateMask = 0;        // bitmask for visible gui state -- uses ENUMuigIndex
    private int m_numTouches = 0;          // number of touches -- used to check for changes in number of touches
    private int m_maxTouchCount = 10;      // max number of simultanious touches -- touchIndex must have space
    private uint[] m_touchIndex = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };  // holds the gui index for each touch

    private double m_inputWaitSeconds = 0.0f; // add time to this and use as wait time
    private double m_clock = 0.0f;
    private bool m_allowInput = true;

    /////////////////////////////////////////////
    //
    //      JOYSTICK INTERFACE METHODS
    //
    /////////////////////////////////////////////

    private void interfaceJoystickOnPress(Vector2 touchPos)
    {
        u_joystick.onPress(touchPos);
    }
    private uint interfaceJoystickOnHold(Vector2 touchPos, uint id, uint touchState)
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

        return touchState;
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

    private uint interfaceSwipeOnHold(Vector2 touchPos, uint id, uint touchState)
    {
        u_swipe.onHold(touchPos);

        return touchState;
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
        u_player.transform.position += new Vector3(1, 0, 0);
        /////////////////////////////////
    }
    private void swipeActionLeft()
    {
        /////////////////////////////////
        // do stuff here
        u_player.transform.position += new Vector3(-1, 0, 0);
        /////////////////////////////////
    }
    private void swipeActionUp()
    {
        /////////////////////////////////
        // do stuff here
        Debug.Log("Time program has run IN SECONDS = " + m_clock);
        u_player.transform.position += new Vector3(0, 1, 0);
        /////////////////////////////////
    }
    private void swipeActionDown()
    {
        /////////////////////////////////
        // do stuff here
        u_player.transform.position += new Vector3(0, -1, 0);
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
        uint flag = u_systemMenu.getId();
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

            m_guiTouchStateMask = (uint)ENUMguiIndex.None;
            m_guiActiveUIStateMask = (uint)ENUMguiIndex.MainGame;
        }
        // open menu if closed
        else
        {
            Debug.Log("Open System Menu");
            u_systemMenu.setActive(true, screen, u_hideObjectPosition);
            // clear previous touch flags and add system menu flag
            m_guiTouchStateMask = (uint)ENUMguiIndex.None;
            m_guiActiveUIStateMask = flag;
        }

        m_allowInput = false;
    }

    private void interfaceSystemMenuOnPress(Vector2 touchPos)
    {
        u_systemMenu.onPress(touchPos);
    }

    private uint interfaceSystemMenuOnHold(Vector2 touchPos, uint buttonId, uint touchState)
    {
        // make this represent pressed buttons
        //m_guiTouchStateMask = u_systemMenu.getPressedButtons((uint)SystemMenu.ENUMsysButtonId.All);

        // remove released buttons from touchstate
        uint releasedButtons = u_systemMenu.getButtonTouched(buttonId, false, touchPos);
        touchState -= (touchState & releasedButtons);

        uint pressedButtons = u_systemMenu.getButtonTouched(buttonId, true, touchPos);
        
        // number where all bits are set to 1
        uint maxValue = uint.MaxValue;

        //Debug.Assert((maxValue.GetType() == pressedButtons.GetType()), 
        //    "GuiHander.interfaceSystemMenuOnHold() - maxValue must be the same data type as pressedButtons");

        //Debug.Log("============================\n============================");
        //Debug.Log("Handler.onHold() - maxValue =        " + maxValue.ToString("00000000000000000000000000000000"));
        //Debug.Log("Handler.onHold() - _touchState =     " + touchState.ToString("00000000000000000000000000000000"));
        //Debug.Log("Handler.onHold() - _pressedButtons = " + pressedButtons.ToString("00000000000000000000000000000000"));
        
        // only add the flags touchState doesn't have
        uint flags = ((pressedButtons ^ maxValue) ^ (pressedButtons & touchState));
        //Debug.Log("Handler.onHold() - flags =           " + flags.ToString("00000000000000000000000000000000"));
        flags = ~flags;
        //Debug.Log("Handler.onHold() - ~flags =          " + flags.ToString("00000000000000000000000000000000"));

        // if a button was added from moving touchPos don't run onRelease()
        //if (flags == 0)
        //   touchState -= (touchState & (uint)ENUMguiIndex.RunRelease);

        touchState += flags;

        //Debug.Log("Handler.onHold() - touchState =      " + touchState.ToString("00000000000000000000000000000000"));
        //Debug.Log("Handler.onHold() - pressedButtons =  " + pressedButtons.ToString("00000000000000000000000000000000"));

        return touchState;

        /*
        how to get (touchState | pressedButtons) of pressedButtons' 1 digits only

        00011000 < pressedButtons         // a       // bit mask
        10010111 < touchState             // b       // value

        11100111 < a ^ 11111111           // c       // create mask for excluded bits
        00010000 < a & b                  // d       // get the 1 bits mask and value share
        11110111 < c ^ d                  // e       // set all unwanted bits to 1
        00001000 < ~e                     // result  // reverse bits

       Debug.Log("============================\n============================\nSTART TEST BIT OPERATIONS");
                                            // 8 bits                                          // 32 bits
       uint allBitsOneUint = uint.MaxValue; // 255                                             // 4294967295
       uint a = 24;                         // 24     (            8 + 16)                     // 24
       uint b = 151;                        // 151    (1 + 2 + 4 +     16 +    +    + 128)     // 151
       uint c = a ^ allBitsOneUint;         // 231    (1 + 2 + 4 +          32 + 64 + 128)     // 4294967271 (-24)
       uint d = a & b;                      // 16     (                16)                     // 16
       uint e = c ^ d;                      // 247    (1 + 2 + 4 +     16 + 32 + 64 + 128)     // 4294967287 (-8)
       uint result = ~e;                    // 8      (            8)                          // 8
       Debug.Log(" a       = " + a.ToString());                                                  
       Debug.Log(" b       = " + b.ToString());
       Debug.Log(" c(a ^ 1)= " + c.ToString());
       Debug.Log(" d(a & b)= " + d.ToString());
       Debug.Log(" e(c ^ d)= " + e.ToString());
       Debug.Log(" result  = " + result.ToString()); // expecting 8
       Debug.Log("END TEST BIT OPERATIONS\n============================\n============================");
       */
    }

    private void interfaceSystemMenuOnRelease(uint buttonFlag)
    {
        /////////////////////////////////
        // do stuff here
        // get flags of buttons that were released
        uint button = u_systemMenu.onRelease();

        Debug.Log("GuiHandler.interfaceSysMenuRelease()" + button);

        switch(buttonFlag)
        {
            case (uint)GameWindow.ENUMwindowObjectId.Continue:
                {
                    interfaceSystemMenuOpen();
                    break;
                }
            case (uint)GameWindow.ENUMwindowObjectId.Quit:
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

        m_guiActiveUIStateMask = (uint)ENUMguiIndex.MainGame;
        
        // set initial player position
        u_player.transform.position = Vector3.zero;

        Vector2 screenSize = new Vector2(m_screenWidth, m_screenHeight);

        u_joystick.init(true, (int)ENUMguiIndex.Joystick, screenSize, u_hideObjectPosition);
        u_swipe.init(true, (int)ENUMguiIndex.Swipe, screenSize, u_hideObjectPosition);
        u_systemMenu.init(false, (int)ENUMguiIndex.SystemMenu, screenSize, u_hideObjectPosition);
    }

    void Update()
    {
        m_clock += Time.unscaledDeltaTime;
        //Debug.Log("===============================\n" +
        //          "========== NEW FRAME ==========\n" +
        //          "===============================");

        // create a copy of previous state to compare to current state
        uint guiTouchStateMaskPrev = m_guiTouchStateMask;
        
        // get current touch count
        int touchCount = Input.touchCount;
        if (touchCount > m_maxTouchCount)
            touchCount = m_maxTouchCount;

        // for forcing to release all touches after changing menus
        if (touchCount == 0)
            m_allowInput = true;

        // temp way to quit
        if (touchCount > 2)
            Application.Quit();
   
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            updateBackKeyPressed(); // handles gui state - brings up system menu
        }
        else if (m_inputWaitSeconds > 0)
        {
            m_inputWaitSeconds -= Time.fixedUnscaledDeltaTime;
        }
        else if(m_allowInput)
        {
            // check if touch count has changed
            if (touchCount != m_numTouches)
            {
                // TODO: check how to handle +touch and -touch on same frame
                updateTouchIndex(touchCount);
            }

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

        // reset gui state
        m_guiTouchStateMask = 0;

        if (touchCount < m_numTouches)
        {
            //Debug.Log("GuildHandler.updateTouchIndex() - m_guiTouchStateMask = " + m_guiTouchStateMask);
            m_guiTouchStateMask |= (uint)ENUMguiIndex.RunRelease;
            //Debug.Log("GuildHandler.updateTouchIndex() - m_guiTouchStateMask = " + m_guiTouchStateMask);
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
                case (uint)ENUMguiIndex.MainGame:
                    {
                        Debug.Log("GuiHandler.updateTouch - Main Game State Active");
                        // check if point is inside rectangle and set touchIndex[touchId] if true
                        // the order of these controls the priority - first is higher priority
                        if (u_joystick.contains(touchPos) || (m_touchIndex[touchId] == (uint)ENUMguiIndex.Joystick))
                        {
                            // do if gui element is not active
                            if (((uint)ENUMguiIndex.Joystick & m_guiTouchStateMask) != (uint)ENUMguiIndex.Joystick)
                            {
                                Debug.Log("Touch Hold Position " + touchId + " is in the Joystick region");

                                // register touch gui element to touchId
                                m_touchIndex[touchId] = (uint)ENUMguiIndex.Joystick;

                                // add gui element flag to gui state
                                m_guiTouchStateMask |= (uint)ENUMguiIndex.Joystick;

                                /////////////////////////////////
                                // do stuff here
                                interfaceJoystickOnPress(touchPos);
                                /////////////////////////////////
                            }
                            // if this touch is already assigned
                            else if (m_touchIndex[touchId] == (uint)ENUMguiIndex.Joystick)
                            {
                                /////////////////////////////////
                                // do stuff here
                                interfaceJoystickOnPress(touchPos);
                                /////////////////////////////////
                            }
                        }
                        else if (u_swipe.contains(touchPos) || (m_touchIndex[touchId] == (uint)ENUMguiIndex.Swipe))
                        {
                            // do if gui element is not active
                            if (((uint)ENUMguiIndex.Swipe & m_guiTouchStateMask) != (uint)ENUMguiIndex.Swipe)
                            {
                                Debug.Log("Touch Hold Position " + touchId + " is in the Swipe region");

                                // register touch gui element to touchId
                                m_touchIndex[touchId] = (uint)ENUMguiIndex.Swipe;

                                // add gui element flag to gui state
                                m_guiTouchStateMask |= (uint)ENUMguiIndex.Swipe;

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
                case (uint)ENUMguiIndex.SystemMenu:
                    {
                        //Debug.Log("GuiHandler.updateTouch - System Menu State Active");
                        //Debug.Log("GuiHandler.updateTouchRegion() - BEFORE m_guiTouchStateMask  = " + m_guiTouchStateMask);
                        if (u_systemMenu.contains(touchPos))
                        {
                            //Debug.Log("1");
                            /////////////////////////////
                            // do stuff here
                            interfaceSystemMenuOnPress(touchPos); // systemmenu.onpress()

                            /////////////////////////////
                            // only works for bitflag enum types
                            m_touchIndex[touchId] = u_systemMenu.getPressedButtons((uint)GameWindow.ENUMwindowObjectId.All);
                            m_guiTouchStateMask |= m_touchIndex[touchId];
                            //Debug.Log("GuiHandler.updateTouchRegion() - DURING m_guiTouchStateMask  = " + m_guiTouchStateMask);
                            //Debug.Log("GuiHandler.updateTouchRegion() - DURING m_touchIndex[touchId]  = " + m_touchIndex[touchId]);
                        }
                        else
                            Debug.Log("GuiHandler.updateTouchIndex() - not touching system menu");

                        
                        //Debug.Log("GuiHandler.updateTouchRegion() - AFTER m_guiTouchStateMask  = " + m_guiTouchStateMask);
                        break;
                    }
                default: Debug.Log("GuiHandler.updateTouchRegion() - Unknown Game State");  break;
            }

            // reset unused touch indexes
            for (int i = touchCount; i < m_maxTouchCount; i++)
            {
                m_touchIndex[i] = (uint)ENUMguiIndex.None;
            }

            touchId++;
        } // end while (i < touchCount)

        // update numTouches for comparison next update
        m_numTouches = touchCount;
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
                case (uint)ENUMguiIndex.MainGame:
                    {
                        switch (m_touchIndex[touchId])
                        {
                            case (uint)ENUMguiIndex.None:
                                {
                                    /////////////////////////////////
                                    // do stuff here
                                    /////////////////////////////////

                                    break;
                                } // end case (uint)ENUMguiIndex.Joystick
                            case (uint)ENUMguiIndex.Joystick:
                                {
                                    // break if gui element is no longer touched
                                    if (((uint)ENUMguiIndex.Joystick & m_guiTouchStateMask) != (uint)ENUMguiIndex.Joystick)
                                        break;

                                    // Debug.Log("Touch Position " + touchId + ": Joystick region held");

                                    /////////////////////////////////
                                    // do stuff here
                                    m_guiTouchStateMask = interfaceJoystickOnHold(touchPos, m_touchIndex[touchId], m_guiTouchStateMask);
                                    /////////////////////////////////

                                    break;
                                } // end case (uint)ENUMguiIndex.Joystick
                            case (uint)ENUMguiIndex.Swipe:
                                {
                                    // break if gui element is no longer touched
                                    if (((uint)ENUMguiIndex.Swipe & m_guiTouchStateMask) != (uint)ENUMguiIndex.Swipe)
                                        break;

                                    //Debug.Log("Touch Position " + touchId + ": Swipe region held");


                                    /////////////////////////////////
                                    // do stuff here
                                    m_guiTouchStateMask = interfaceSwipeOnHold(touchPos, m_touchIndex[touchId], m_guiTouchStateMask);
                                    /////////////////////////////////

                                    break;
                                } // end case (uint)ENUMguiIndex.Swipe
                            default: Debug.Log("playerController.updateTouchHoldAction() >> default case should not happen"); break;
                        }
                        break;
                    }
                case (uint)ENUMguiIndex.SystemMenu:
                    {
                        //Debug.Log("onHold Start - " + m_guiTouchStateMask);
                        //Debug.Log("AAA onHold touchIndex = " + m_touchIndex[touchId]);
                        m_guiTouchStateMask = interfaceSystemMenuOnHold(touchPos, m_touchIndex[touchId], m_guiTouchStateMask);
                        
                        //Debug.Log("onHold End - " + m_guiTouchStateMask);
                        //Debug.Log("onHold touchIndex = " + m_touchIndex[touchId]);
                        break;
                    }
            }
            touchId++;
        } // end while(touchId < m_numTouches)
    } // end onHold

    // performs action on release for each touch
    private void updateTouchOnRelease(uint guiTouchStateMaskPrev)
    {
        if ((m_guiTouchStateMask & (uint)ENUMguiIndex.RunRelease) != (uint)ENUMguiIndex.RunRelease)
            return;
        
        //Debug.Log("updateTouchOnRelease() - m_guiActiveUIStateMask " + m_guiActiveUIStateMask);
        //Debug.Log("updateTouchOnRelease() - guiTouchStatePrev " + guiTouchStatePrev);
        //Debug.Log("updateTouchOnRelease() - m_guiTouchStateMask " + m_guiTouchStateMask);

        // remove variable RunRelease flag
        //Debug.Log("GuiHandler.onRelease() m_guiTouchStateMask = " + m_guiTouchStateMask);
        m_guiTouchStateMask -= (uint)ENUMguiIndex.RunRelease;
        //Debug.Log("GuiHandler.onRelease() m_guiTouchStateMask = " + m_guiTouchStateMask);
        //Debug.Log("GuiHandler.onRelease() guiTouchStateMaskPrev = " + guiTouchStateMaskPrev);

        // check game state
        switch (m_guiActiveUIStateMask)
        {
            case (uint)ENUMguiIndex.MainGame:
                {
                    //Debug.Log("GuiHandler.onRelease - Main Game State Active");
                    //Debug.Log("if(" + (((uint)ENUMguiIndex.Joystick & guiTouchStateMaskPrev) == (uint)ENUMguiIndex.Joystick) + " && " +
                    //                  (((uint)ENUMguiIndex.Joystick & m_guiTouchStateMask) != (uint)ENUMguiIndex.Joystick) + ")");

                    if (((uint)ENUMguiIndex.Joystick & guiTouchStateMaskPrev) == (uint)ENUMguiIndex.Joystick &&
                        ((uint)ENUMguiIndex.Joystick & m_guiTouchStateMask) != (uint)ENUMguiIndex.Joystick)
                    {
                        Debug.Log("Touch Joystick region released");

                        // remove this from all touches
                        for(uint i = 0; i < m_maxTouchCount; i++)
                        {
                            uint v = (m_touchIndex[i] & (uint)ENUMguiIndex.Joystick);
                            m_touchIndex[i] -= v;
                        }

                        /////////////////////////////////
                        // do stuff here
                        interfaceJoystickOnRelease();
                        /////////////////////////////////
                    }

                    if (((uint)ENUMguiIndex.Swipe & guiTouchStateMaskPrev) == (uint)ENUMguiIndex.Swipe &&
                        ((uint)ENUMguiIndex.Swipe & m_guiTouchStateMask) != (uint)ENUMguiIndex.Swipe)
                    {
                        Debug.Log("Touch Swipe region released");

                        // remove this from all touches
                        for (uint i = 0; i < m_maxTouchCount; i++)
                        {
                            uint v = (m_touchIndex[i] & (uint)ENUMguiIndex.Swipe);
                            m_touchIndex[i] -= v;
                        }

                        /////////////////////////////////
                        // do stuff
                        interfaceSwipeOnRelease();
                        /////////////////////////////////
                    }
                    break;
                }
            case (uint)ENUMguiIndex.SystemMenu:
                {
                    //Debug.Log("GuiHandler.onRelease - guiTouchStatePrev    = " + guiTouchStateMaskPrev);
                    //Debug.Log("GuiHandler.onRelease - m_guiTouchStateMask  = " + m_guiTouchStateMask);
                    // run omly if a button was released
                    //if (m_guiTouchStateMask < guiTouchStatePrev)
                    // check each region for if it was held last upadate and is NOT held this update
                    if (((uint)GameWindow.ENUMwindowObjectId.Continue & guiTouchStateMaskPrev) == (uint)GameWindow.ENUMwindowObjectId.Continue &&
                        ((uint)GameWindow.ENUMwindowObjectId.Continue & m_guiTouchStateMask) != (uint)GameWindow.ENUMwindowObjectId.Continue)
                    {
                        // remove this from all touches
                        for (uint i = 0; i < m_maxTouchCount; i++)
                        {
                            uint v = (m_touchIndex[i] & (uint)GameWindow.ENUMwindowObjectId.Continue);
                            m_touchIndex[i] -= v;
                        }
                        //Debug.Log("GuiHandler.onRelease - System Menu Continue button released");
                        interfaceSystemMenuOnRelease((uint)GameWindow.ENUMwindowObjectId.Continue);
                    }

                    if (((uint)GameWindow.ENUMwindowObjectId.Quit & guiTouchStateMaskPrev) == (uint)GameWindow.ENUMwindowObjectId.Quit &&
                        ((uint)GameWindow.ENUMwindowObjectId.Quit & m_guiTouchStateMask) != (uint)GameWindow.ENUMwindowObjectId.Quit)
                    {
                        // remove this from all touches
                        for (uint i = 0; i < m_maxTouchCount; i++)
                        {
                            uint v = (m_touchIndex[i] & (uint)GameWindow.ENUMwindowObjectId.Continue);
                            m_touchIndex[i] -= v;
                        }
                        //Debug.Log("GuiHandler.onRelease - System Menu Quit button released");
                        interfaceSystemMenuOnRelease((uint)GameWindow.ENUMwindowObjectId.Quit);
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

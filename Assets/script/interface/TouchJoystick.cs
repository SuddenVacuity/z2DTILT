using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Programmer: Gerald Coggins
/// 
/// </summary>

[RequireComponent(typeof(SpriteRenderer))]
public class TouchJoystick : MonoBehaviour
{
    public Vector2 u_position;
    public Vector2 u_size;
    public GameObject u_centerPrefab;  // Grapical Object
    public bool u_graphicVisible;
    public float u_maxDistance;     // max distance in pixels from the initial point
    public float u_deadZone;        // zone around the initial point that doesn't update player/graphics
    public float u_zPos;            // world z pos of UI element
    private bool m_isActive = false;
    private int m_componentFlag = 0;     // used to by handler track touch state and active touch regions
    private Rect m_touchRegion = Rect.zero;    // region on the screen for this gui element - values 0~1 are % of the screen space
    private Vector2 m_touchPosInitial = Vector2.zero;  // coordinates of the initial touch point
    private Vector2 m_touchPosPrev = Vector2.zero;     // used to check it move/graphic update is needed
    private float m_rotationPrev = 0; // last updates Rotation

    // initializes all joystick game objects
    public void init(bool isActive, int componentFlag, Vector2 screenSize, Vector3 hidePos)
    {
        m_componentFlag = componentFlag;

        u_centerPrefab.transform.position = hidePos;

        u_centerPrefab = Instantiate(
        u_centerPrefab,
        hidePos,
        Camera.main.transform.rotation);

        // TODO: add title screen
        setActive(isActive, screenSize, hidePos);
    }

    // controls if a region and related graphics are on screen
    private void setActive(bool isActive, Vector2 screenSize, Vector2 hidePos)
    {
        m_isActive = isActive;

        Vector2 position = hidePos;
        Vector2 size = Vector2.zero;

        if (isActive == true)
        {
            // set gui region locations
            position.x = screenSize.x * u_position.x;
            position.y = screenSize.y * u_position.y;
            size.x = screenSize.x * u_size.x;
            size.y = screenSize.y * u_size.y;
        }

        m_touchRegion.Set(
            position.x,
            position.y,
            size.x,
            size.y);
    }

    public bool contains(Vector2 touchPos)
    {
        return m_touchRegion.Contains(touchPos);
    }

    // runs on press
    public void onPress(Vector2 touchPos)
    {
        // if there is no initial touch point
        if (m_touchPosInitial == Vector2.zero)
        {
            // set initial touch point
            m_touchPosInitial = touchPos;
            m_touchPosPrev = m_touchPosInitial;

            // set graphics
            graphicSetup();
        }
    }

    // sets initial position and location of graphics
    private void graphicSetup()
    {
        if (u_graphicVisible == false)
            return;

        ////////////////////////
        // POSITION 

        // convert initial touch point to world point
        Vector3 touchWorldPoint = Camera.main.ScreenToWorldPoint(m_touchPosInitial);
        touchWorldPoint.z = u_zPos;

        // move hidden touch point sprites to world touch point
        u_centerPrefab.transform.position = touchWorldPoint;
        //JoyCurrentPointPrefab.transform.position = touchWorldPoint;

        ///////////////////////////
        // ROTATION

        Vector2 v2 = (m_touchPosInitial).normalized;
        float angle = Mathf.Atan2(v2.y, v2.x) * Mathf.Rad2Deg;

        // convert angle range to degrees
        // previously 0 >>> 180 > -180 >>> -0 > 0
        // new 0 >>> 360 > 0
        if (angle < 0)
            angle += 360;

        float angleDelta = angle - m_rotationPrev;

        //Debug.Log("angle = " + angle);
        //Debug.Log("anglePrev = " + anglePrev);
        //Debug.Log("angleDelta = " + angleDelta);

        //Debug.Log("touchPosInitial = " + touchPosInitial);
        //Debug.Log("touchPosCurrent = " + touchPosCurrent);

        u_centerPrefab.transform.Rotate(0, 0, angleDelta);

        m_rotationPrev = angle;
    }

    /////////////////////////////////////////////
    //      JOYSTICK HOLD METHODS

    // returns Vector3 axis (x, y, distance)
    public Vector3 onHold(Vector2 touchPos)
    {
        Vector3 result = Vector3.zero;

        Vector2 joyTouchPosCurrent = touchPos;

        float touchDist = Vector2.Distance(m_touchPosInitial, joyTouchPosCurrent);

        // check deadzone
        if (touchDist > u_deadZone)
        {
            Vector2 touchPosDelta = joyTouchPosCurrent - m_touchPosInitial;
            touchPosDelta = touchPosDelta.normalized;

            result.x = touchPosDelta.x;
            result.y = touchPosDelta.y;
            result.z = touchDist / u_maxDistance;
            
            joyMoveGraphics(touchDist, joyTouchPosCurrent);

            // CARRY DATA TO NEXT FRAME
            m_touchPosPrev = joyTouchPosCurrent;
        }

        return result;
    }

    // moves graphics
    private void joyMoveGraphics(float touchDistance, Vector2 touchPos)
    {
        // check if graphic should show
        if (u_graphicVisible == false)
            return;

        // check for a change in touch location
        if ((touchPos - m_touchPosPrev) == Vector2.zero)
            return;

        ///////////////////////////////
        // POSITION

        // make center point follow touch if out of range
        if (touchDistance > u_maxDistance)
        {
            float touchDistanceDeltaPercent = 1 - u_maxDistance / touchDistance;
            float touchPointDeltaPixels = u_maxDistance * touchDistanceDeltaPercent;

            // get the delta between between the 2 points
            Vector2 touchPointDelta = touchPos - m_touchPosInitial;

            touchPointDelta = touchPointDelta.normalized * touchPointDeltaPixels;

            // update centerpoint
            m_touchPosInitial += touchPointDelta;

            // conver new point to work point
            Vector3 touchWorldPointCenter = Camera.main.ScreenToWorldPoint(m_touchPosInitial);
            touchWorldPointCenter.z = u_zPos;

            // move the graphic to the new point
            u_centerPrefab.transform.position = touchWorldPointCenter;
            //JoyCurrentPointPrefab.transform.position = touchWorldPoint;
        }

        ///////////////////////////////
        // ROTATION

        //arrowPrefab_region1.transform.localScale = Vector3.one;
        Vector2 v2 = (touchPos - m_touchPosInitial).normalized;
        float angle = Mathf.Atan2(v2.y, v2.x) * Mathf.Rad2Deg;

        // convert angle range to degrees
        // previously 0 >>> 180 > -180 >>> -0 > 0
        // new 0 >>> 360 > 0
        if (angle < 0)
            angle += 360;

        float angleDelta = angle - m_rotationPrev;

        //Debug.Log("angle = " + angle);
        //Debug.Log("anglePrev = " + anglePrev);
        //Debug.Log("angleDelta = " + angleDelta);

        //Debug.Log("touchPosInitial = " + touchPosInitial);
        //Debug.Log("touchPosCurrent = " + touchPosCurrent);

        u_centerPrefab.transform.Rotate(0, 0, angleDelta);

        m_rotationPrev = angle;

    }

    /////////////////////////////////////////////
    //      JOYSTICK RELEASE METHODS

    // runs on release
    public void onRelease(Vector2 hidePos)
    {
        // hide initial touch point sprite
        u_centerPrefab.transform.position = hidePos;
        //JoyCurrentPointPrefab.transform.position = hiddenGraphicLocation;
        //arrowPrefab_region1.transform.localScale = Vector3.zero;

        // reset touch tracking values
        m_touchPosInitial = Vector2.zero;
        m_touchPosPrev = Vector2.zero;

        // reset accumulator
        //accumulator_region1 = 0;
    }
    
}

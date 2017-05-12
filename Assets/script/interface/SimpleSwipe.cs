using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Programmer: Gerald Coggins
/// </summary>

public class SimpleSwipe : MonoBehaviour
{
    public enum ENUMswipeOutput
    {
        None = 0,
        Up = 1,
        Down = 2,
        Left = 3,
        Right = 4,
        Tap = 5,
    }
    public Vector2 u_position;
    public Vector2 u_size;
    public float u_maxDistance;     // max distnace to track swipe movement
    public float u_tapDistance;     // max distance to be considered a tap
    public float u_zPos;            // world z pos of UI element
    private bool m_isActive = false;
    private int m_componentFlag = 0;        // contains each type of swipe input - uses
    private Rect m_touchRegion = Rect.zero;       // region on the screen for this gui element - values 0~1 are % of the screen space
    private Vector2 m_touchPosInitial = Vector2.zero;  // coordinates of the initial touch point
    private Vector2 m_touchPosEnd = Vector2.zero;  // coordinates of the end touch point

    // initializes all swipe game objects
    public void init(bool isActive, int componentFlag, Vector2 screenSize, Vector2 hidePos)
    {
        m_componentFlag = componentFlag;

        setActive(isActive, screenSize, hidePos);
    }

    // controls if a region and related graphics are on screen
    public void setActive(bool isActive, Vector2 screenSize, Vector2 hidePos)
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

    // runs on press
    public void onPress(Vector2 touchPos)
    {
        Debug.Log("Swipe Started");
        // if there is no initial touch point
        if (m_touchPosInitial == Vector2.zero)
        {
            // get initial touch point
            m_touchPosInitial = touchPos;
            m_touchPosEnd = m_touchPosInitial;

            // convert initial touch point to world point
            //Vector3 touchWorldPoint = Camera.main.ScreenToWorldPoint(m_swipeTouchPosInitial);
            //touchWorldPoint.z = swipeZpos;

            // move hidden touch point sprites to world touch point
            //pointPrefab_region2.transform.position = touchWorldPoint;
        }
    }

    /////////////////////////////////////////////
    //   SWIPE HOLD METHODS

    // runs on update
    public void onHold(Vector2 touchPos)
    {
        //Debug.Log("Swipe Update");
        //touchGraphicVisible_region2 = true;
        //accumulator_region2 += Time.deltaTime;
        //Debug.Log("Region2 is active");

        m_touchPosEnd = touchPos;

        //Vector3 trailWorldPoint = camera.ScreenToWorldPoint(touchPosCurrent_region2);
        //trailWorldPoint.z = zPos;
        //trailPrefab_region2.transform.position = trailWorldPoint;
    }

    /////////////////////////////////////////////
    //   SWIPE RELEASE METHODS

    // runs on release
    public int onRelease()
    {
        int output;
        Debug.Log("Swipe Release");
        output = swipeCalcSwipeDirection();

        // clean up
        m_touchPosInitial = Vector2.zero;
        m_touchPosEnd = Vector2.zero;

        return output;
    }

    // finds the direction of swipe from start and end points
    private int swipeCalcSwipeDirection()
    {
        int output = (int)ENUMswipeOutput.None;

        float swipeTouchPosDistance = Vector2.Distance(m_touchPosInitial, m_touchPosEnd);

        if (swipeTouchPosDistance < u_tapDistance)
            return (int)ENUMswipeOutput.Tap;

        // get angle of input
        // TODO: move to updateSwipe() for graphics and store value for output
        Vector2 swipeTouchPosDelta = m_touchPosEnd - m_touchPosInitial;

        Vector2 v2 = swipeTouchPosDelta.normalized;
        float angle = Mathf.Atan2(v2.y, v2.x) * Mathf.Rad2Deg;

        if (angle < 0)
            angle += 360;

        // check range of angle
        if (angle <= 45 || angle > 315)
            output = (int)ENUMswipeOutput.Right;
        else if (angle <= 135 && angle > 45)
            output = (int)ENUMswipeOutput.Up;
        else if (angle <= 225 && angle > 135)
            output = (int)ENUMswipeOutput.Left;
        else if (angle <= 315 && angle > 225)
            output = (int)ENUMswipeOutput.Down;
        else
            Debug.Log("playerController.swipeCalcSwipeDirection() >> angle not valid (" + angle + ")");

        return output;
    }

    public bool contains(Vector2 pos)
    {
        return m_touchRegion.Contains(pos);
    }

}

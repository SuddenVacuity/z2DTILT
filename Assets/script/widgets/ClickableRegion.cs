using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class ClickableRegion
{
    private Vector2 m_position;
    private Vector2 m_size;
    private int m_id;

    public void initialize(Vector2 position, Vector2 size, int id)
    {
        m_position = position;
        m_size = size;
        m_id = id;
    }

    public void setSize(Vector2 size)
    {
        m_size = size;
    }
    public void setPosition(Vector2 position)
    {
        m_position = position;
    }
    
    private void setId(int id)
    {
        m_id = id;
    }

    public int getId()
    {
        return m_id;
    }

    public Vector2 getSize()
    {
        return m_size;
    }

    public Vector2 getPosition()
    {
        return m_position;
    }

    private bool contains(Vector2 position)
    {
        if (position.x < m_position.x || 
            position.y < m_position.y)
            return false;

        if (position.x > (m_position.x + m_size.x) ||
            position.y > (m_position.y + m_size.y))
            return false;
        
        return true;
    }


}

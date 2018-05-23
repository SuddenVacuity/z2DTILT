using System.Collections;
using System.Collections.Generic;
using UnityEngine;





public class BitMask
{
    ////////////////////////////////////////////////////////
    //
    //          UINT (32)
    //
    ////////////////////////////////////////////////////////
    /// <summary>
    /// returns all 1 bits in mask that are not 1 in value
    /// </summary>
    public static uint getBitsNotSet(uint value)
    {
        return (~(uint.MaxValue & value));
    }

    /// <summary>
    /// returns all 1 bits in mask that are not 1 in value
    /// </summary>
    public static uint getBitsNotSet(uint value, uint mask)
    {
        return ((mask & value) | mask);
        //return (~((~mask) ^ (mask & value)));
    }
    
    /// <summary>
    /// returns a value with all bits from value and mask set 
    /// </summary>
    public static uint setBits(uint value, uint mask)
    {
        return (value |= mask);
    }

    /// <summary>
    /// returns a value with all 1 bits from mask set to 0 
    /// </summary>
    public static uint clearBits(uint value, uint mask)
    {
        return (value -= (value & mask));
    }

    /// <summary>
    /// gets the bit from value at position
    ///  - returns false if position is out of range
    /// </summary>
    public static bool getIsBitSet(uint value, int position)
    {
        if (position < 1 || position > 32)
            return false;

        return ((value & (1 << position)) != 0);
    }
    
    public static bool containsBits(uint value, uint mask)
    {
        return ((value & mask) == mask);
    }
}

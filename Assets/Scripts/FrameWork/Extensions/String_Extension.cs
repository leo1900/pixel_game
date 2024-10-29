using System;
using System.Globalization;
using UnityEngine;

public static class StringExtension
{
	public static bool IsEmptyString(this String _this)
	{
		return _this == null || _this == "" || _this == "none";
	}

    /// <summary>
    /// to bool
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool ToBool(this string input)
    {
        bool.TryParse(input, out bool value);
        return value;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static byte ToByte(this string input)
    {
        byte.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out byte value);
        return value;
    }

    public static decimal ToDecimal(this string input)
    {
        decimal.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal value);
        return value;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static double ToDouble(this string input)
    {
        double.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out double value);
        return value;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static float ToFloat(this string input)
    {
        float.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out float value);
        return value;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static int ToInt(this string input)
    {
        int.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out int value);
        return value;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static long ToLong(this string input)
    {
        long.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out long value);
        return value;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static sbyte ToSbyte(this string input)
    {
        sbyte.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out sbyte value);
        return value;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static short ToShort(this string input)
    {
        short.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out short value);
        return value;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static uint ToUInt(this string input)
    {
        uint.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out uint value);
        return value;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static ulong ToULong(this string input)
    {
        ulong.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out ulong value);
        return value;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static ushort ToUShort(this string input)
    {
        ushort.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out ushort value);
        return value;
    }
    
    /// <summary>
    /// hex转换到color
    /// </summary>
    /// <param name="hex"></param>
    /// <returns></returns>
    public static Color HexToColor(this string hex)
    {
        byte br = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte bg = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte bb = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        float r = br / 255f;
        float g = bg / 255f;
        float b = bb / 255f;
        if (hex.Length >= 8)
        {
            byte cc = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
            float a = cc / 255f;
            return new Color(r, g, b, a);
        }
        
        return new Color(r, g, b, 1);
    }
}


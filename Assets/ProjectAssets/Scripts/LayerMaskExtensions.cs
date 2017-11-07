using System;
using System.Collections.Generic;
using UnityEngine;

public static class LayerMaskExtensions
{
    public static List<string> GetLayers(this LayerMask layerMask)
    {
        List<string> result = new List<string>();
        for (int i = 0; i < 32; i++)
        {
            if ((layerMask & (1 << i)) != 0)
                result.Add(LayerMask.LayerToName(i));
        }
        return result;
    }

    public static bool Has(this LayerMask layerMask, string layerName)
    {
        ValidLayerName(layerName);
        return (layerMask.value & (1 << LayerMask.NameToLayer(layerName))) != 0;
    }

    public static int Count(this LayerMask layerMask)
    {
        int result = 0;
        int value = layerMask.value;
        while (value != 0)
        {
            if (value % 2 == 1)
                result++;
            value = value / 2;
        }
        return result;
    }

    public static LayerMask Add(this LayerMask layerMask, string layerName)
    {
        ValidLayerName(layerName);
        return layerMask | 1 << LayerMask.NameToLayer(layerName);
    }

    public static LayerMask Remove(this LayerMask layerMask, string layerName)
    {
        ValidLayerName(layerName);
        return layerMask & ~(1 << LayerMask.NameToLayer(layerName));
    }

    private static void ValidLayerName(string layerName)
    {
        if (string.IsNullOrEmpty(layerName))
            throw new Exception("Layer name should not be empty");

        if (LayerMask.NameToLayer(layerName) == -1)
            throw new Exception("Invalid layer name: " + layerName);
    }
}
using UnityEngine;

/// <summary>
/// Any geometry class inherit this interface should be closeable
/// </summary>
public interface IPolygonClosable
{
    //finish special polygon
    void ClosePolygon();
}

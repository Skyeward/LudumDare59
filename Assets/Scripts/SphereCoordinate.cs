using UnityEngine;


public class SphereCoordinate
{
    public float Latitude; //in radians
    public float Longitude; //in radians
    
    
    public SphereCoordinate(float latitude, float longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }
    
    
    public static Vector3 GetCartesianPositionFromSphereCoordinate(SphereCoordinate coord, float radius)
    {
        float latDeg = coord.Latitude * Mathf.Deg2Rad;
        float longDeg = coord.Longitude * Mathf.Deg2Rad;
        
        Vector3 cartesianCoord = new Vector3(
            radius * Mathf.Cos(latDeg) * Mathf.Cos(longDeg),
            radius * Mathf.Cos(latDeg) * Mathf.Sin(longDeg),
            radius * Mathf.Sin(latDeg)
        );
        
        return cartesianCoord;
    }
}

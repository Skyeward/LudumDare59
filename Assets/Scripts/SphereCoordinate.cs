using UnityEngine;


public class SphereCoordinate
{
    public float Latitude;
    public float Longitude;
    
    
    public SphereCoordinate(float latitude, float longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }
    
    
    public static Vector3 GetCartesianPositionFromSphereCoordinate(SphereCoordinate coord, float radius)
    {
        Vector3 cartesianCoord = new Vector3(
            radius * Mathf.Cos(coord.Latitude) * Mathf.Cos(coord.Longitude),
            radius * Mathf.Cos(coord.Latitude) * Mathf.Sin(coord.Longitude),
            radius * Mathf.Sin(coord.Latitude)
        );
        
        return cartesianCoord;
    }
}

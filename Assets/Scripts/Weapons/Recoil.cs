using UnityEngine;

public class Recoil : MonoBehaviour
{
    public float snappiness = 6;
    public float returnSpeed = 25;
    
    public float snappinessPos = 6;
    public float returnSpeedPos = 25;

    public Vector3 RecoilRotation = new Vector3(2f, 2f, 2f);
    public float RecoilPositionX;

    private Vector3 currentRotation;
    private Vector3 currentPosition = Vector3.zero;
    private Vector3 Rot;
    private Vector3 Pos = Vector3.zero;

    private void FixedUpdate()
    {
        currentRotation = Vector3.Lerp(currentRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        currentPosition = Vector3.Lerp(currentPosition, Vector3.zero, returnSpeedPos * Time.deltaTime);
        Rot = Vector3.Slerp(Rot, currentRotation, snappiness * Time.fixedDeltaTime);
        Pos = Vector3.Slerp(Pos, currentPosition, snappinessPos * Time.fixedDeltaTime);
        transform.localRotation = Quaternion.Euler(Rot);
        transform.localPosition = Pos;
    }

    public void RecoilFire()
    {
        currentRotation += new Vector3(-RecoilRotation.x, Random.Range(0f, RecoilRotation.y), Random.Range(-RecoilRotation.z, RecoilRotation.z));
        currentPosition += new Vector3(RecoilPositionX, 0f, 0f);
    }
}
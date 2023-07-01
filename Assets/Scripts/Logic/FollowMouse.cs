using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    [Header("Transform")]
    [SerializeField]
    private new Transform transform;
    [Space(10)]

    [Header("Vectors")]
    [SerializeField]
    private Vector3 position;
    [SerializeField]
    private Vector2 velocity;
    [Space(10)]

    [Header("float numbers")]
    [SerializeField]
    private float smoothTime;
    [SerializeField]
    private float maxSpeed;

    // Start is called before the first frame update
    void Start()
    {
        velocity = Vector2.zero;                
        transform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        position = Camera.main.ScreenToWorldPoint(Input.mousePosition);         //Bildschim Koordinaten der Maus werden zu Welt koordinaten des Spiels konvertiert
        transform.position = updatePos();  
    }



    private Vector3 updatePos()
    {
        return Vector2.SmoothDamp(transform.position, position, ref velocity, smoothTime, maxSpeed);        //erzeugt eine fluessige Bewegung der Figur zur Maus
    }

    //wir wollen testen ob das in der neuen branch ist
}

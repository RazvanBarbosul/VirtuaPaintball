using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets._2D;

public class ArmRotation : MonoBehaviour
{

    public float rotationOffset = 0;
    public float speed = 5f;
    [SerializeField]
    PlatformerCharacter2D Character;
    [SerializeField]
    private Player Player;
    [SerializeField]
    private Camera PlayerCamera;
    Vector3 difference;

    private void Start()
    {
        if (Player == null)
        {
            Player = FindObjectOfType<Player>();
        }
    }
    // Update is called once per frame
    void Update ()
    {
      //  Rotate();
	}

    public void Rotate()
    {
        Player = FindObjectOfType<Player>();

        if (Character.Direction == true)
        {
            rotationOffset = 0.0f;
        }
        else
        {
            rotationOffset = -180f;
        }
        //  Debug.Log(Character.gameObject.name);
        if (Character.tag == "Player")
        {
            difference = PlayerCamera.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            difference.Normalize();

            float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, rotZ + rotationOffset);
        }
        else
        {
            difference = Player.EnemyAimLocation.position - transform.position;
            // difference.Normalize();
            float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(rotZ + 10, Vector3.forward);
            // transform.rotation = Quaternion.Slerp(transform.rotation, rotation, speed * Time.deltaTime + rotationOffset);
            transform.rotation = Quaternion.Euler(0f, 0f, rotZ + rotationOffset);
        }
    }
}

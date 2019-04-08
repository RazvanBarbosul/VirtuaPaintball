using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(SpriteRenderer))]

public class Tiling : MonoBehaviour {

    public int offsetX = 2;
    public bool hasRightBuddy = false;
    public bool hasLeftBuddy = false;
    public bool reverseScale = false;
    public float spriteWidth = 0f;
    private Camera cam;
    public Transform myTransform;
    public Transform parents;


    private void Awake()
    {
        cam = Camera.main;
        myTransform = transform;
    }

    // Use this for initialization
    void Start () {
        SpriteRenderer sRenderer = GetComponent<SpriteRenderer>();
        //Debug.Log("Transform name:" + myTransform.name);
        if(myTransform.tag == "Ground" )
        {
            spriteWidth = 40;
           
        }
        else if (myTransform.tag == "Wall")
        {
            spriteWidth = 100;

        }

        else if (myTransform.tag == "Ceiling")
        {
            spriteWidth = 150;

        }
        else
        {
            spriteWidth = sRenderer.sprite.bounds.size.x;
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (!cam)
        {
            cam = Camera.main;
        }
		if (hasLeftBuddy == false || hasRightBuddy == false)
        {
            float camHorizontalExtend = cam.orthographicSize * Screen.width / Screen.height;
            float edgeVisiblePosRight = (myTransform.position.x + spriteWidth / 2) - camHorizontalExtend;
            float edgeVisiblePosLeft = (myTransform.position.x - spriteWidth / 2) + camHorizontalExtend;

            if (cam.transform.position.x >= edgeVisiblePosRight - offsetX && hasRightBuddy == false)
            {
                MakeNewBuddy(1);
                hasRightBuddy = true;
            }
            else if (cam.transform.position.x <= edgeVisiblePosLeft + offsetX && hasLeftBuddy == false)
            {
                MakeNewBuddy(-1);
                hasLeftBuddy = true;
            }
        }
	}

    void MakeNewBuddy(int direction)
    {
        Vector3 newPosition = new Vector3(myTransform.position.x + spriteWidth * direction, myTransform.position.y, myTransform.position.z);
        Transform newBuddy = Instantiate(myTransform, newPosition, myTransform.rotation) as Transform;

        if (reverseScale == true)
        {
            newBuddy.localScale = new Vector3(newBuddy.localScale.x * -1, newBuddy.localScale.y, newBuddy.localScale.z);
        }
        newBuddy.transform.parent = parents.transform;
        //newBuddy.parent = parents.transform;
        //  newBuddy.parent = myTransform.parent;
        if (direction > 0)
        {
            newBuddy.GetComponent<Tiling>().hasLeftBuddy = true;
        }
        else
        {
            newBuddy.GetComponent<Tiling>().hasRightBuddy = true;
        }
    }
}

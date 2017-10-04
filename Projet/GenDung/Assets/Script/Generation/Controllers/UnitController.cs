﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour {

    private int tileX;
    private int tileY;

    private List<Node> currentPath = null;

    int moveSpeed = 2;
    float remainingMovement = 2;

	void Update ()
    {
	    if(currentPath != null)
        {
            Debug.Log("Path not null, Drawing line");
            int currNode = 0;

            while (currNode < currentPath.Count-1)
            {
                Vector3 start = DungeonController.Instance.TileCoordToWorldCoord(currentPath[currNode].x, currentPath[currNode].y) + new Vector3(0, 0, -1f);
                Vector3 end = DungeonController.Instance.TileCoordToWorldCoord(currentPath[currNode + 1].x, currentPath[currNode].y) + new Vector3(0, 0, -1f);

                Debug.DrawLine(start, end, Color.red);
                Debug.Log("Line has been Drawn");
                currNode++;
            }
        }
        
        if (Vector3.Distance(transform.position, DungeonController.Instance.TileCoordToWorldCoord(tileX,tileY)) < 0.1f)
        {
            AdvancePathing();
            transform.position = Vector3.Lerp(transform.position, DungeonController.Instance.TileCoordToWorldCoord(tileX, tileY), 5f * Time.deltaTime);
        }
	}

    public void AdvancePathing()
    {
       if (currentPath == null)
        {
            return;
        }

       if (remainingMovement <= 0)
        {
            return;
        }

        transform.position = DungeonController.Instance.TileCoordToWorldCoord(tileX, tileY);

        remainingMovement -= DungeonController.Instance.CostToEnterTile(currentPath[0].x, currentPath[0].y, currentPath[1].x, currentPath[1].y);

        tileX = currentPath[1].x;
        tileY = currentPath[1].y;

        transform.position = DungeonController.Instance.TileCoordToWorldCoord(tileX, tileY);

        currentPath.RemoveAt(0);

        if (currentPath.Count == 1)
        {
            currentPath = null;
        }
    }

    public void NextTurn()
    {
        while(currentPath!= null && remainingMovement > 0)
        {
            AdvancePathing();
        }

        remainingMovement = moveSpeed;
    }

    /* Accessors Methods*/
    
    public int TileX
    {
        get
        {
            return tileX;
        }
        set
        {
            tileX = value;
        }
    }

    public int TileY
    {
        get
        {
            return tileY;
        }
        set
        {
            tileY = value;
        }
    }

    public List<Node> CurrentPath
    {
        get
        {
            return currentPath;
        }
        set
        {
            currentPath = value;
        }
    }

    /**/
}

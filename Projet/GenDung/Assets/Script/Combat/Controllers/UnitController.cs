﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour {

    private int tileX;
    private int tileY;

    private int tileAttackX;
    private int tileAttackY;

    private List<Node> currentPath = null;
    private bool attacking = false;

    int moveSpeed = 2; // Valeur de base de déplacement de l'unité.
    int actionCount = 3; // Valeur de base d'action de l'unité.

    float remainingMovement = 2; //Points de mouvement restant de l'unité pour ce tour.
    float remainingAction = 3; //Points d'actions restant de l'unité pour ce tour.

    int attackCost = 1; // Coût d'une attaque de l'unité.
    int rangeMax = 2; // Portée maximale de l'unité
    int rangeMin = 1; // Portée minimale de l'unité (dans le cas ou une attaque ne peut se faire à une case du personnage)

	void Update ()
    {
	    if(currentPath != null)
        {
            //Debug.Log("Path not null, Drawing line");
            int currNode = 0;

            while (currNode < currentPath.Count-1)
            {
                Vector3 start = DungeonController.Instance.TileCoordToWorldCoord(currentPath[currNode].x, currentPath[currNode].y) + new Vector3(0, 0, -1f);
                Vector3 end = DungeonController.Instance.TileCoordToWorldCoord(currentPath[currNode + 1].x, currentPath[currNode].y) + new Vector3(0, 0, -1f);

                Debug.DrawLine(start, end, Color.red);
                //Debug.Log("Line has been Drawn");
                currNode++;
            }
        }

        if (attacking)
        {
            Debug.Log("Launching Attack, Please Wait");
            Attack();
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
            Debug.Log("Not enough movement point left, wait for the next turn");
            return;
        }

        transform.position = DungeonController.Instance.TileCoordToWorldCoord(tileX*64, tileY*64);

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

    public void Attack()
    {
        attacking = false;

        if (remainingAction <= 0)
        {
            Debug.Log("No action points left, no action done");
            return;
        }

        if (remainingAction >= attackCost && CheckRange() == true)
        {
            remainingAction -= attackCost;
            Debug.Log("Action points left : " + remainingAction);
        }
    }

    public bool CheckRange()
    {
        Debug.Log("Unit position: "+ tileX + "," + tileY);
        Debug.Log("Tile for Attack position: " + tileAttackX + "," + tileAttackY); // les valeurs sont égales à zéro, pourquoi ? Ok, si int en public mais pas en private (getter et setter fautif)

        // vérifier si la tile pour l'attaque est comprise entre unitPosX + rangeMin et unitPosX + rangeMax mais aussi si celle-ci est comprise entre unitPosY + rangeMin et unitPosY + rangeMax
        if (tileAttackX >= (tileX + rangeMin) && tileAttackX <= (tileX + rangeMax) && tileAttackY >= (tileY + rangeMin) && tileAttackY <= (tileY + rangeMax) || tileAttackX <= (tileX - rangeMin) && tileAttackX >= (tileX - rangeMax) && tileAttackY >= (tileY + rangeMin) && tileAttackY <= (tileY + rangeMax))
        {
            // Attaque coin supérieur droit ou coin supérieur gauche.
            Debug.Log("Range is Ok, we can attack");
            return true;
        }
        else if (tileAttackX <= (tileX - rangeMin) && tileAttackX >= (tileX - rangeMax) && tileAttackY <= (tileY - rangeMin) && tileAttackY >= (tileY - rangeMax) || tileAttackX >= (tileX + rangeMin) && tileAttackX <= (tileX + rangeMax) && tileAttackY <= (tileY - rangeMin) && tileAttackY >= (tileY - rangeMax))
        {
            // Attaque coin inférieur gauche ou coin inférieur droit.
            Debug.Log("Range is Ok, we can attack");
            return true;
        }
        else if (tileAttackX >= (tileX + rangeMin) && tileAttackX <= (tileX + rangeMax) || tileAttackX <= (tileX - rangeMin) && tileAttackX >= (tileX - rangeMax))
        {
            // Attaque Horizontale.
            Debug.Log("Range is Ok, we can attack");
            return true;
        }
        else if(tileAttackY >= (tileY + rangeMin) && tileAttackY <= (tileY + rangeMax) || tileAttackY <= (tileY - rangeMin) && tileAttackY >= (tileY - rangeMax))
        {
            // Attaque Verticale.
            Debug.Log("Range is Ok, we can attack");
            return true;
        }
        else
        {
            Debug.Log("Not in Range, abort Attack");
            return false;
        }
        
    }

    public void NextTurn()
    {
        while(currentPath!= null && remainingMovement > 0)
        {
            AdvancePathing();
        }

        remainingMovement = moveSpeed;
        remainingAction = actionCount;

        Debug.Log("New Turn has begun !!!");
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

    public int getTileAttackX()
    {
        return tileAttackX;
    }

    public void setTileAttackX(int _x)
    {
        tileAttackX = _x;
    }

    public int getTileAttackY()
    {
        return tileAttackY;
    }

    public void setTileAttackY(int _y)
    {
        tileAttackY = _y;
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

    public bool Attacking
    {
        get
        {
            return attacking;
        }
        set
        {
            attacking = value;
        }
    }

    /**/
}

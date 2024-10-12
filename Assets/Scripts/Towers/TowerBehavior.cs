using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TowerBehavior : MonoBehaviour
{
    [NonSerialized] public Enemy target;

    [SerializeField] Transform towerPivot;
    [SerializeField] LayerMask towerInfoMask;
    public LayerMask enemiesLayer;
    public float damage;
    public float fireRate;
    public float range;
    public int cost;

    private float delay;
    private TowerPlacement towerPlacement;
    Camera cam;
    public bool isSelected;

    private IDamageMethod currentDamageMethodClass;

    private void Start()
    {
        towerPlacement = TowerPlacement.Instance;
        cam = towerPlacement.cam;
        isSelected = true;

        currentDamageMethodClass = GetComponent<IDamageMethod>();

        if (currentDamageMethodClass == null )
        {
            Debug.LogError("ERROR: FAILED TO FIND A DAMAGE CLASS ON CURRENT TOWER!");
        }
        else
        {
            currentDamageMethodClass.Init(damage, fireRate);
        }

        delay = 1 / fireRate;
    }

    //Desyncs the towers from regular game loop to prevent errors
    public void Tick()
    {
        currentDamageMethodClass.damageTick(target);

        if (target != null)
        {
            towerPivot.transform.rotation = Quaternion.LookRotation(target.transform.position - transform.position);
        }
        // Create a pointer event for UI detection
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        // List of raycast results for UI
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        // If any UI elements were hit, ignore the raycast for game objects
        if (results.Count > 0)
        {
            return; // Exit if UI is clicked
        }

        //Ray casts from screen to mouse
        Ray camRay = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        //Gets data from raycast
        if (Physics.Raycast(camRay, out hitInfo, 100f, towerInfoMask))
        {
            //If tower was clicked
            if (Input.GetMouseButtonDown(0))
            {
                
                if (Input.GetMouseButtonDown(0) && hitInfo.collider.gameObject == gameObject)
                {
                    isSelected = !isSelected;
                    if (isSelected)
                    {
                        GameManager.Instance.SelectedTower = gameObject;
                        UIManager.Instance.ToggleSell(true);
                    }      
                }  
                else
                {
                    if (isSelected)
                        UIManager.Instance.ToggleSell(false);
                    isSelected = false;
                }
                    
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            if (isSelected)
                UIManager.Instance.ToggleSell(false);
            isSelected = false;
        }
            
        gameObject.transform.Find("Base").transform.Find("Range").gameObject.SetActive(isSelected);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(towerPivot.position, range);
        if (target != null)
            Gizmos.DrawWireCube(target.transform.position, new Vector3(.5f, .5f, .5f));
    }
}
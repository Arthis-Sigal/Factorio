using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class BuildingGridPlacer : BuildingPlacer
{

    public float cellSize;
    public Vector3 gridOffset;
    public Renderer gridRenderer;

    #if UNITY_EDITOR
    private void OnValidate()
    {
        _UpdateGridVisual();
    }
    #endif

    public void Start()
    {
        _EnableGridVisual(false);
        _UpdateGridVisual();
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Destroy(_toBuild);
            _toBuild = null;
            _buildingPrefab = null;
            _EnableGridVisual(false);
            return;
        }

        if (_buildingPrefab != null)
        {
            if(EventSystem.current.IsPointerOverGameObject())
            {
                if (_toBuild.activeSelf) _toBuild.SetActive(false);
            }
            else if (!_toBuild.activeSelf) _toBuild.SetActive(true);

            //rotate
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _toBuild.transform.Rotate(Vector3.up, 90);
            }

            _ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(_ray, out _hit, 1000f, groundLayerMask))
            {
                if(!_toBuild.activeSelf) _toBuild.SetActive(true);
                _toBuild.transform.position = _ClampToNearest(_hit.point, cellSize);

                if (Input.GetMouseButtonDown(0))
                {
                    BuildingManager m = _toBuild.GetComponent<BuildingManager>();
                    if (m.hasValidPlacement)
                    {
                        m.SetPlacementMode(PlacementMode.Fixed);
                        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                        {
                            _toBuild = null;
                            _PrepareBuilding();
                        }

                        if (playerInventory != null)
                        {
                            if (playerInventory.inventory.GetItemAmount(_buildingPrefab.name) > 0) //a retirer à la realise (test)
                            {
                                Debug.Log("✅ Construction de " + _buildingPrefab.name);
                                playerInventory.inventory.RemoveItem(_buildingPrefab.name, 1);
                                UpdateBuildingModeUI();
                            }
                        }
                        
   
                        _buildingPrefab = null;
                        _toBuild = null;
                        _EnableGridVisual(false);
                        
                    }

                }
            }
            else if(_toBuild.activeSelf) _toBuild.SetActive(false); 
        }
    }

    protected override void _PrepareBuilding()
    {
        base._PrepareBuilding();
        _EnableGridVisual(true);
    }

    private Vector3 _ClampToNearest(Vector3 pos, float threshold)
    {
        float t = 1f / threshold;
        Vector3 v = ((Vector3)Vector3Int.FloorToInt(pos * t)) / t;
        float s = threshold * 0.5f;
        v.x += s + gridOffset.x; //recenter in momiddle of cells
        v.z += s + gridOffset.z;
        v.y += s + gridOffset.y;
        return v;
    }

    private void _EnableGridVisual(bool on)
    {
        if (gridRenderer == null) return;
        gridRenderer.gameObject.SetActive(on);
    }
    private void _UpdateGridVisual()
    {
        if (gridRenderer == null) return;
        gridRenderer.sharedMaterial.SetVector("_Cell_Size", new Vector4(cellSize, cellSize, 0, 0));
    }
}

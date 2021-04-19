using System;
using System.Collections.Generic;
using UnityEngine;

namespace DesignCollectyEditor
{
    public class DesignerCollectyAlternativeController : MonoBehaviour
    {
        [SerializeField] public DesignerMovement designerMovement;
        [SerializeField] public SightDrawler sightDrawler;
        [SerializeField] public GridDrawler gridDrawler;
        [SerializeField] public ClickHandler clickHandler;

        public static DesignerCollectyAlternativeController inst;
        private void Start()
        {
            inst = this;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            clickHandler.Init();
        }

        public void Update()
        {
            designerMovement.Move(transform);
            sightDrawler.DrawSight(transform);
            gridDrawler.DrawGrid();
            clickHandler.Click(transform);
        }

        #region Node Click Editor
        [Serializable]
        public class ClickHandler
        {
            [SerializeField] private float HalfOffsetForCollider = 0.3f * 8.0f / 2.0f;  //Collider Y-size * Transform Y-size / half of height
            [SerializeField] List<string> avalableNamesForCollecties = new List<string>();

            #region Fields And Properties - Parametres
            [Serializable] private enum ClickMode { Placing, Connection }
#if UNITY_EDITOR
            [ReadOnlyAttribute.ReadOnly]
#endif
            [SerializeField] ClickMode clickMode = ClickMode.Placing;

            [SerializeField] List<Transform> _prefabs = new List<Transform>();
            int _indexPrefab = 0;

            public int IndexPrefab 
            {
                get
                {
                    if (_indexPrefab < 0) _indexPrefab = _prefabs.Count - 1;
                    if (_indexPrefab > _prefabs.Count - 1) _indexPrefab = 0;
                    return _indexPrefab;
                }
                set => _indexPrefab = value;
            }
            [SerializeField] Transform NodeParentsContainer;

#if UNITY_EDITOR
            [ReadOnlyAttribute.ReadOnly]
#endif
            public Transform CapturedNode = null;

#if UNITY_EDITOR
            [ReadOnlyAttribute.ReadOnly]
#endif
            public Transform fromNode;

#if UNITY_EDITOR
            [ReadOnlyAttribute.ReadOnly]
#endif
            public List<Transform> ParentsForNodes = new List<Transform>();

            RaycastHit hit;
            bool rightDownClick, leftDownClick, leftUpClick, rightUpClick;

            event TransformAction LeftUp, LeftDown, RightUp, RightDown;
            public delegate void TransformAction(RaycastHit hit);
            #endregion

            #region MAJOR METHODS
            public void Init()
            {
                if (!avalableNamesForCollecties.Contains("Collecty")) avalableNamesForCollecties.Add("Collecty");

                RightUp += ReleaseCapturedNode;
                RightDown += CaptureNode;
                //LeftUp += DeleteNode;
                LeftDown += CreateOrDestroyNode;

                for (int i = 0; i < NodeParentsContainer.childCount; i++)
                    ParentsForNodes.Add(NodeParentsContainer.GetChild(i));

            }
            public void Click(Transform transform)
            {
                leftDownClick = Input.GetMouseButtonDown(0);
                rightDownClick = Input.GetMouseButtonDown(1);
                leftUpClick = Input.GetMouseButtonUp(0);
                rightUpClick = Input.GetMouseButtonUp(1);

                if (Input.mouseScrollDelta.y != 0) IndexPrefab = Input.mouseScrollDelta.y > 0 ? IndexPrefab+1 : IndexPrefab-1;

                if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)) clickMode = ClickMode.Placing;

                if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2)) clickMode = ClickMode.Connection;



                if (!Physics.Raycast(transform.position, transform.forward, out hit)) return;

                MoveCapturedNode(transform, hit);

                if (!leftDownClick && !rightDownClick && !leftUpClick && !rightUpClick) return;

                if (leftUpClick) LeftUp?.Invoke(hit);
                if (leftDownClick) LeftDown?.Invoke(hit);
                if (rightUpClick) RightUp?.Invoke(hit);
                if (rightDownClick) RightDown?.Invoke(hit);
            }
            #endregion


            #region PlacingMode Methods
            private void CaptureNode(RaycastHit hit)
            {
                if (clickMode != ClickMode.Placing) return;

                if (avalableNamesForCollecties.Contains(hit.collider.name))
                {
                    CapturedNode = hit.collider.transform;
                    hit.collider.enabled = false;
                }
            }
            private void ReleaseCapturedNode(RaycastHit hit)
            {
                if (clickMode != ClickMode.Placing) return;
                if (CapturedNode == null) return;

                if (!avalableNamesForCollecties.Contains(hit.collider.name))
                    CapturedNode.transform.position = inst.gridDrawler.RoundPosition(hit.point + Vector3.up * HalfOffsetForCollider);

                CapturedNode.GetComponent<Collider>().enabled = true;
                CapturedNode = null;

            }
            private void CreateOrDestroyNode(RaycastHit hit)
            {
                if (clickMode != ClickMode.Placing || _prefabs.Count<1) return;

                if (avalableNamesForCollecties.Contains(hit.collider.name))
                {                    
                    Destroy(hit.collider.gameObject);
                }
                else
                {
                    Transform parent = null;
                    foreach (var p in ParentsForNodes)
                        if (p.childCount < 20) parent = p;

                    if (parent == null)
                    {
                        parent = new GameObject($"Parent {ParentsForNodes.Count + 1}").transform;
                        parent.position = Vector3.zero;
                        parent.parent = NodeParentsContainer;

                        ParentsForNodes.Add(parent);
                    }

                    var instatinated = Instantiate(_prefabs[IndexPrefab], hit.point + Vector3.up * HalfOffsetForCollider, Quaternion.identity, parent);
                    instatinated.position = inst.gridDrawler.RoundPosition(instatinated.position);
                    instatinated.name = instatinated.name.Replace("(Clone)", "");
                }
            }
            private void DeleteNode(RaycastHit hit) { if (clickMode != ClickMode.Placing) return; }
            private void MoveCapturedNode(Transform transform, RaycastHit hit)
            {
                if (hit.transform == null) return;

                if (CapturedNode != null) CapturedNode.transform.position = inst.gridDrawler.RoundPosition(hit.point + Vector3.up * HalfOffsetForCollider);
                if (fromNode != null) Debug.DrawLine(fromNode.transform.position, hit.point, Color.black);
            }
            #endregion
        }
        #endregion

        #region Movement
        [Serializable]
        public class DesignerMovement
        {
            [SerializeField] float movementSpeed = 1.0f;
            [SerializeField] float rotationSpeed = 1.0f;
            [SerializeField] float accelerationSpeed = 50.0f;

            [Header("Move KeyCodes")]
            [SerializeField] KeyCode forwardKey = KeyCode.W;
            [SerializeField] KeyCode leftwardKey = KeyCode.A;
            [SerializeField] KeyCode backwardKey = KeyCode.S;
            [SerializeField] KeyCode rightwardKey = KeyCode.D;
            [SerializeField] KeyCode upwardKey = KeyCode.LeftShift;
            [SerializeField] KeyCode downwardKey = KeyCode.LeftControl;
            [SerializeField] KeyCode accelerationKey = KeyCode.Space;

            //[Header("Rotation keycodes")]
            //public KeyCode clockwiseKey = KeyCode.E;
            //public KeyCode counterClockwiseKey = KeyCode.Q;


            private DesignerMovement movementParametres { get => this; }
            float OX, OY, forward = 0.0f, right = 0.0f, up = 0.0f, rotateOZ = 0.0f;
            Vector3 moveDirection;

            public void Move(Transform transform)
            {
                forward = 0.0f; right = 0.0f; up = 0.0f;


                if (Input.GetKey(movementParametres.forwardKey)) forward += 1.0f;
                if (Input.GetKey(movementParametres.backwardKey)) forward -= 1.0f;

                if (Input.GetKey(movementParametres.leftwardKey)) right -= 1.0f;
                if (Input.GetKey(movementParametres.rightwardKey)) right += 1.0f;

                if (Input.GetKey(movementParametres.upwardKey)) up += 1.0f;
                if (Input.GetKey(movementParametres.downwardKey)) up -= 1.0f;


                moveDirection = new Vector3(right, up, forward);
                if(Input.GetKey(accelerationKey)) moveDirection = transform.rotation * moveDirection.normalized * movementParametres.accelerationSpeed;
                else moveDirection = transform.rotation * moveDirection.normalized * movementParametres.movementSpeed;
                transform.position += moveDirection;


                OX = -Input.GetAxis("Mouse Y");
                OY = Input.GetAxis("Mouse X");

                //rotateOZ = 0.0f;
                //if (Input.GetKey(movementParametres.clockwiseKey)) rotateOZ += 1.0f;
                //if (Input.GetKey(movementParametres.counterClockwiseKey)) rotateOZ -= 1.0f;

                transform.Rotate(OX, OY, -transform.eulerAngles.z);
                //transform.rotation.SetLookRotation(new Vector3(OX, OY, 0));
            }
        }
        #endregion

        #region Draw Sight 
        [Serializable]
        public class SightDrawler
        {
            [SerializeField] float drawDistance = 1.0f;
            Vector3 sightPosition;

            Line line1 = new Line(new Vector3(1, 1, 0), new Vector3(-1, -1, 0));
            Line line2 = new Line(new Vector3(-1, 1, 0), new Vector3(1, -1, 0));

            public void DrawSight(Transform designerLook)
            {
                sightPosition = designerLook.position + designerLook.forward * drawDistance;

                line1.rotateLine(designerLook.rotation).DrawWithOffset = sightPosition;
                line2.rotateLine(designerLook.rotation).DrawWithOffset = sightPosition;

                //if line regenerated every call, like local variable:
                //Line line1 = new Line(new Vector3(1, 1, 0), new Vector3(-1, -1, 0));
                //Line line2 = new Line(new Vector3(-1, 1, 0), new Vector3(1, -1, 0));

                //line1.rotate = designerLook.rotation;
                //line2.rotate = designerLook.rotation;
                //line1.DrawWithOffset = sightPosition;
                //line2.DrawWithOffset = sightPosition;
            }

            private class Line
            {
                public Vector3 pos1, pos2;
                public Line(Vector3 _pos1, Vector3 _pos2, Vector3 _boldOffsetRotated)
                {
                    pos1 = _pos1; pos2 = _pos2; boldOffsetRotated = _boldOffsetRotated;
                }
                public Line(Vector3 _pos1, Vector3 _pos2)
                {
                    pos1 = _pos1; pos2 = _pos2;
                }

                public Line rotateLine(Quaternion rotation) => new Line(rotation * pos1, rotation * pos2, rotation * boldoffset);
                public Quaternion rotate
                {
                    set
                    {
                        pos1 = value * pos1;
                        pos2 = value * pos2;
                        boldOffsetRotated = value * boldoffset;
                    }
                }

                private Vector3 boldoffset = Vector3.up * 0.005f;
                private Vector3 boldOffsetRotated = Vector3.up * 0.005f;
                public Vector3 DrawWithOffset
                {
                    set
                    {
                        Debug.DrawLine(pos1 + value, pos2 + value, Color.green);
                        Debug.DrawLine(pos1 + value + boldOffsetRotated, pos2 + value + boldOffsetRotated, Color.green);
                        Debug.DrawLine(pos1 + value - boldOffsetRotated, pos2 + value - boldOffsetRotated, Color.green);
                    }
                }
            }
        }
        #endregion

        #region Draw Grid

        [Serializable]
        public class GridDrawler
        {
            public float SectorSize = 0.5f;
            [SerializeField] float HeightOfDebugDrawSectors = 0.1f;
            public int maxNumbSectorsByX = 10;
            public int maxNumbSectorsByZ = 10;
            public bool depthTest = true;
            public void DrawGrid()
            {

                Gizmos.color = Color.cyan;

                int minZ = -maxNumbSectorsByZ / 2;
                int maxZ = maxNumbSectorsByZ / 2;

                int minX = -maxNumbSectorsByX / 2;
                int maxX = maxNumbSectorsByX / 2;

                for (int x = -maxNumbSectorsByX / 2; x < maxNumbSectorsByX / 2; x++)
                {
                    //Gizmos.DrawLine(new Vector3(x, HeightOfDebugDrawSectors, minZ) * SectorSize, new Vector3(x, HeightOfDebugDrawSectors, maxZ) * SectorSize);
                    Debug.DrawLine(new Vector3(x, HeightOfDebugDrawSectors, minZ) * SectorSize, new Vector3(x, HeightOfDebugDrawSectors, maxZ) * SectorSize, Color.magenta, 1, depthTest);
                }

                for (int z = minZ; z < maxZ; z++)
                {
                    //Gizmos.DrawLine(new Vector3(minX, HeightOfDebugDrawSectors, z) * SectorSize, new Vector3(maxX, HeightOfDebugDrawSectors, z) * SectorSize);
                    Debug.DrawLine(new Vector3(minX, HeightOfDebugDrawSectors, z) * SectorSize, new Vector3(maxX, HeightOfDebugDrawSectors, z) * SectorSize, Color.magenta, 1, depthTest);
                }
            }

            //public Dictionary<Vector2Int, List<DesignerTrafficNode>> DefineSectorOfAllNodes(Dictionary<DesignerTrafficNode, bool> nodes)
            //{
            //    Dictionary<Vector2Int, List<DesignerTrafficNode>> nodesAtSectors = new Dictionary<Vector2Int, List<DesignerTrafficNode>>();
            //
            //    //foreach (var node in nodes.Keys)
            //    //{
            //    //    nodes[node] = false;
            //    //}
            //
            //    foreach (var node in nodes.Keys)
            //    {
            //        var keySector = DefineSectorByPosition(node.transform.position);
            //        if (!nodesAtSectors.ContainsKey(keySector)) nodesAtSectors.Add(keySector, new List<DesignerTrafficNode>());
            //
            //        nodesAtSectors[keySector].Add(node);
            //
            //    }
            //    return nodesAtSectors;
            //}
            //

            public Vector3 RoundPosition(Vector3 pos) 
            {
                int x = (int)((float)pos.x / (float)(SectorSize));
                int z = (int)((float)pos.z / (float)(SectorSize));

                return new Vector3(
                Mathf.RoundToInt((Mathf.Sign(pos.x) * 0.5f*SectorSize + x) * SectorSize),
                pos.y,
                Mathf.RoundToInt((Mathf.Sign(pos.z) * 0.5f*SectorSize + z) * SectorSize));

            }
            private Vector2Int DefineSectorByPosition(Vector3 pos)
            {
                int x = (int)((float)pos.x / (float)(SectorSize));
                int z = (int)((float)pos.z / (float)(SectorSize));

                //Debug.Log($"x = {x}  :  z = {z}");
                //Debug.Log($"Not Rounded: x = {(x + 0.5f) * SectorSize}  :  z = {(z + 0.5f) * SectorSize}");

                return new Vector2Int(
                    Mathf.RoundToInt((Mathf.Sign(pos.x) * 0.5f + x) * SectorSize),
                    Mathf.RoundToInt((Mathf.Sign(pos.z) * 0.5f + z) * SectorSize));
            }
        }
        #endregion

    }
}

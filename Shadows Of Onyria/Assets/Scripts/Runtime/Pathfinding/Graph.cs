using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DoaT.AI
{
    public class Graph : MonoBehaviour
    {
        private const float NodeCheckRadius = 5f;
        
        [SerializeField] private List<Node> nodeList;

        public List<Node> NodeList => nodeList;

        private Dictionary<string, Node> _nodeByNameMap = new Dictionary<string, Node>();

        private void Awake()
        {
            RecalculateNodeList();
        }
        
        public Node NodeFromWorldPosition(Vector3 worldPosition)
        {
            Node bestNode = default;

            for (int i = 0; i < 1000; i++)
            {
                var nodes = Physics.OverlapSphere(worldPosition, NodeCheckRadius + NodeCheckRadius * i,
                    LayersUtility.NODE_MASK, QueryTriggerInteraction.Collide);
                
                if(nodes.Length == 0) continue;
            
                bestNode = nodes
                    .OrderBy(n => Vector3.Distance(worldPosition, n.transform.position))
                    .First()
                    .GetComponent<Node>();

                break;
            }

            return bestNode;
        }

        public void ResetNodes()
        {
            foreach (var node in nodeList)
            {
                node.Reset();
            }
        }
        
        public void CalculateNodeNeighbours()
        {
            foreach (var node in nodeList)
            {
                node.CalculateNeighbours();
            }
        }
        
        public void CalculateNodeBlockState()
        {
            foreach (var node in nodeList)
            {
                node.SetBlockState(LayersUtility.WALL_MASK);
            }

            var newList = new List<Node>(nodeList);
            
            foreach (var node in nodeList)
            {
                if (node.isBlocked)
                {
                    newList.Remove(node);
                    
                    if(Application.isEditor)
                        DestroyImmediate(node.gameObject);
                    else
                        Destroy(node.gameObject);
                }
            }

            nodeList = new List<Node>(newList);
        }

        public void RecalculateNodeList()
        {
            nodeList = new List<Node>();
            _nodeByNameMap.Clear();
            
            var temp = GetComponentsInChildren<Node>();

            foreach (var node in temp)
            {
                nodeList.Add(node);
                _nodeByNameMap.Add(node.gameObject.name, node);
            }
        }

        public Graph SetNodeList(IEnumerable<Node> list)
        {
            nodeList = new List<Node>(list);
            return this;
        }

        public Vector3 GetValidPosition(Vector3 center, float initialRadius, float entityDeadZone, ref Vector3[] entityPositions)
        {
            var rejectedNodeIndexes = new HashSet<int>();
            var validPosition = new Vector3(-1000, -1000, -1000);

            var radius = initialRadius - 1;
            var watchdog = 1000;
            do
            {
                if (--watchdog < 0)
                {
                    break;
                }

                rejectedNodeIndexes.Clear();
                radius += 1;

                var result = Physics.OverlapSphere(center, radius, LayersUtility.NODE_MASK, QueryTriggerInteraction.Collide);

                var count = result.Length;
                foreach (var col in result)
                {
                    var node = _nodeByNameMap[col.gameObject.name];
                    
                    var rIndex = Random.Range(0, count);
                    
                    if (rejectedNodeIndexes.Contains(rIndex)) continue;
                    
                    var nodePosition = node.Position;
                    
                    if ((nodePosition - center).sqrMagnitude >= radius * radius)
                    {
                        rejectedNodeIndexes.Add(rIndex);
                        continue;
                    }
                    
                    var valid = entityPositions.All(position =>
                        (nodePosition - position).sqrMagnitude > entityDeadZone * entityDeadZone);
                    
                    if (!valid)
                    {
                        rejectedNodeIndexes.Add(rIndex);
                        continue;
                    }
                    
                    validPosition = nodePosition;
                }
            } 
            while (validPosition == new Vector3(-1000, -1000, -1000));

            return validPosition;
        }
        
        public Vector3 GetValidPosition(Vector3 startPosition, float startDeadZone, ref Vector3[] entityPositions, float entityDeadZone)
        { 
            var rejectedNodeIndexes = new HashSet<int>();
            var validPosition = Vector3.zero;

            var count = nodeList.Count;
            for (int i = 0; i < count; i++)
            {
                var rIndex = Random.Range(0, count);
                
                if(rejectedNodeIndexes.Contains(rIndex)) continue;

                var nodePosition = nodeList[rIndex].Position;

                if ((nodePosition - startPosition).sqrMagnitude <= startDeadZone * startDeadZone)
                {
                    rejectedNodeIndexes.Add(rIndex);
                    continue;
                }

                var valid = entityPositions.All(position => (nodePosition - position).sqrMagnitude > entityDeadZone * entityDeadZone);

                if (!valid)
                {
                    rejectedNodeIndexes.Add(rIndex);
                    continue;
                }

                validPosition = nodePosition;
            }

            return validPosition;
        }
    }
}


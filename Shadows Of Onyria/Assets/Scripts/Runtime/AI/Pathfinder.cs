using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace DoaT.AI
{
    public class Pathfinder : MonoBehaviour, IUnloadable
    {
        [SerializeField] private Graph m_nodeGraph;

        [Range(1f, 100f)] public int stepsMaxDefault = 100;
        
        public Graph Graph => m_nodeGraph;

        private readonly Queue<PathRequest> _requests = new Queue<PathRequest>();
        private readonly HashSet<PathRequest> _requestBuckets = new HashSet<PathRequest>();

        private void Awake()
        {
            m_nodeGraph = FindObjectOfType<Graph>();
            StartCoroutine(Pathfinding());
            EventManager.Subscribe(GameEvents.OnSceneUnload, Unload);
        }
        
        public void GetPath(PathRequest request)
        {
            if (_requestBuckets.Contains(request)) return;
            _requestBuckets.Add(request);
            _requests.Enqueue(request);
        }
        
        private IEnumerator Pathfinding()
        {
            while (true)
            {
                while (_requests.Count == 0)
                {
                    yield return null;
                }
                Graph.ResetNodes();
                
                var currentRequest = _requests.Dequeue();
                var startNode = m_nodeGraph.NodeFromWorldPosition(currentRequest.initialPosition.Invoke());
                var targetNode = m_nodeGraph.NodeFromWorldPosition(currentRequest.targetPosition.Invoke());
                
                var dist = Vector3.Distance(startNode.Position, targetNode.Position);
                var dir = (targetNode.Position - startNode.Position).normalized;
                var ray = new Ray(startNode.Position, dir);
                
                if (!Physics.SphereCast(ray, startNode.Radius, dist, LayersUtility.WALL_MASK))
                {
                    targetNode.previousNode = startNode;
                    var firstPassPath = RetracePathSmooth(startNode, targetNode);
                    currentRequest.output.Invoke(new Path(firstPassPath));
                    _requestBuckets.Remove(currentRequest);
                    continue;
                }
            
                var openSet = new PriorityQueue<Node>();
                var closedSet = new HashSet<Node>();
            
                openSet.Enqueue(startNode);
                startNode.gCost = 0;
                startNode.hCost = MathUtility.ManhattanDistance(startNode.Position, targetNode.Position);

                var current = startNode;
                current.pathNumber = 1;

                var stopwatch = new Stopwatch();
                stopwatch.Start();
                while (openSet.Count > 0)
                {
                    if (stopwatch.ElapsedMilliseconds >= 1)
                    {
                        yield return new WaitForEndOfFrame();
                        stopwatch.Restart();
                    }
                    
                    current = openSet.Dequeue();
                    closedSet.Add(current);
        
                    if (current == targetNode || current.pathNumber >= stepsMaxDefault)
                    {
                        break;
                    }

                    foreach (var neighbour in current.neighbours.Where(neighbour => !closedSet.Contains(neighbour.node)))
                    {
                        neighbour.node.pathNumber = current.pathNumber + 1;

                        if (!openSet.Contains(neighbour.node))
                        {
                            openSet.Enqueue(neighbour.node);
                    
                            var positionTar = targetNode.Position;
                            var positionNeigh = neighbour.node.Position;
                            neighbour.node.hCost = Mathf.Abs(positionTar.x - positionNeigh.x)
                                                   + Mathf.Abs(positionTar.y - positionNeigh.y)
                                                   + Mathf.Abs(positionTar.z - positionNeigh.z);
                        }
                    
                    
                        var newG = current.gCost + neighbour.distance;
                    
                        if ((newG >= neighbour.node.gCost)) continue;
                    
                        neighbour.node.gCost = newG;
                        neighbour.node.previousNode = current;
                    }
                }
                
                if (current.pathNumber >= stepsMaxDefault)
                {
                    targetNode = current;
                    DebugManager.LogWarning(current.pathNumber >= stepsMaxDefault);
                }
                var path = RetracePathSmooth(startNode, targetNode);
                
                currentRequest.output.Invoke(new Path(path));
                _requestBuckets.Remove(currentRequest);
            }
        }
        
        public void Unload(params object[] parameters)
        {
            StopAllCoroutines();
        }

        private List<Node> SmoothPath(List<Node> originalPath)
        {
            var currentNode = originalPath[0];
        
            var watchdog = 1000;
        
            while (currentNode)
            {
                if (watchdog-- == 0)
                {
                    return originalPath;
                }
        
                if (currentNode.previousNode != null && currentNode.previousNode.previousNode != null)
                {                
                    var temp = GetLastVisibleNode(currentNode, currentNode.previousNode);

                    currentNode.previousNode = temp;
                    currentNode = currentNode.previousNode;
                }
                else
                {
                    currentNode = currentNode.previousNode;
                }
            }
        
            var pathNode = RetraceNodePath(originalPath[originalPath.Count - 1], originalPath[0]);
        
            return pathNode;
        }
        private Node GetLastVisibleNode(Node initialNode, Node child)
        {
            if (child == null || initialNode == child)
                return initialNode.previousNode;
        
            if (child.previousNode == null)
                return child;

            if (child == initialNode.previousNode)
            {
                var obj = GetLastVisibleNode(initialNode, child.previousNode);
        
                return obj == null ? child : obj;
            }
        
            var dist = Vector3.Distance(initialNode.transform.position, child.transform.position);
            var dir = (child.transform.position - initialNode.transform.position).normalized;
            var ray = new Ray(initialNode.transform.position, dir);
        
            if (Physics.SphereCast(ray, initialNode.Radius, dist, LayersUtility.WALL_MASK) 
            || !MathUtility.FastApproximately(child.Position.y, initialNode.Position.y, 0.0001f))
            {
                return null;
            }
        
            var temp = GetLastVisibleNode(initialNode, child.previousNode);
        
            return temp == null ? child : temp;
        }
        private List<Node> RetraceNodePath(Node start, Node end)
        {
            var pathNode = new List<Node>();
            var currentNode = end;
        
            while (currentNode != start)
            {
                pathNode.Add(currentNode);
                currentNode = currentNode.previousNode;
            }
            pathNode.Add(currentNode);
        
            return pathNode;
        }

        private List<Vector3> RetracePathSmooth(Node start, Node end)
        {
            var pathNode = new List<Node>();
            var pathPositionNode = new List<Vector3>();
            
            var currentNode = end;
        
            while (currentNode != start && currentNode.previousNode)
            {
                pathNode.Add(currentNode);
                currentNode = currentNode.previousNode;
            }
            pathNode.Add(currentNode);
        
        
            var originalPath = new List<Node>(pathNode);
            pathNode = SmoothPath(pathNode);
        
            for (int i = 0; i < pathNode.Count; i++)
            {
                if (!pathNode[i])
                    pathNode.RemoveAt(i);
        
                if(!originalPath[i])
                    originalPath.RemoveAt(i);
            }
        
            while (pathNode.Count > 0)
            {
                pathPositionNode.Add(pathNode[pathNode.Count - 1].transform.position);
                pathNode.RemoveAt(pathNode.Count - 1);
            }
        
            while (originalPath.Count > 0)
            {
                originalPath.RemoveAt(originalPath.Count - 1);
            }
        
        
            return pathPositionNode;
        }

        public Pathfinder SetGraph(Graph graph)
        {
            m_nodeGraph = graph;
            return this;
        }

        private void OnDestroy()
        {
            EventManager.Unsubscribe(GameEvents.OnSceneUnload, Unload);
        }
    }
}
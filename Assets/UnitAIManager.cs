namespace CT6GAMAI
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class UnitAIManager : MonoBehaviour
    {
        [SerializeField] private UnitManager _unitManager;
        private GameManager _gameManager;
        
        private void Start()
        {
            _gameManager = GameManager.Instance;
        }

        public IEnumerator BeginEnemyAI()
        {
            _gameManager.GridManager.GridCursor.MoveCursorTo(_unitManager.StoodNode.Node);

            yield return StartCoroutine(EnemyAITurn());
        }

        private IEnumerator EnemyAITurn()
        {
            yield return new WaitForSeconds(2);

            MoveUnitToRandomNodeWithinRange();

            yield return new WaitForSeconds(2);
        }

        private void MoveUnitToRandomNodeWithinRange()
        {
            List<Node> nodes = _unitManager.MovementRange.ReachableNodes;
            List<Node> steppableNodes = new List<Node>();

            foreach (Node node in nodes) 
            {
                if (node.NodeManager.StoodUnit == null)
                {
                    steppableNodes.Add(node);
                }
            }

            if (steppableNodes.Count > 0)
            {
                int rand = Random.Range(1, steppableNodes.Count);
                Node randNode = steppableNodes[rand];
                _unitManager.MoveUnitToNode(randNode);
            }
        }
    }

}
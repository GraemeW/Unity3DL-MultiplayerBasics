using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.AI;

public class PlayerMover : NetworkBehaviour
{
    // Tunables
    [SerializeField] float speed = 0.1f;
    [SerializeField] float wasdThreshold = 0.01f;
    [SerializeField] NavMeshAgent navMeshAgent = null;

    // State
    Vector3 moveOffset = new Vector3();

    // Cached References
    Camera mainCamera = null;

    #region Server

    [Command]
    private void CmdMove(Vector3 offset)
    {
        navMeshAgent.Move(offset);
    }

    [Command]
    private void CmdMoveToPoint(Vector3 point)
    {
        if (!NavMesh.SamplePosition(point, out NavMeshHit hit, 1f, NavMesh.AllAreas)) { return; }

        navMeshAgent.SetDestination(hit.position);
    }

    #endregion

    #region Client
    public override void OnStartAuthority()
    {
        mainCamera = Camera.main;
    }

    [ClientCallback]
    private void Update()
    {
        if (!hasAuthority) { return; }

        if (HandleKeyboardInput()) { return; }
        if (HandleMouseInput()) { return; }
    }

    [ClientCallback]
    private void FixedUpdate()
    {
        if (!hasAuthority) { return; }

        CmdMove(moveOffset);
    }

    private bool HandleKeyboardInput()
    {
        moveOffset = Vector3.zero;
        float horizontal = speed * Input.GetAxis("Horizontal");
        float vertical = speed * Input.GetAxis("Vertical");
        if (Mathf.Abs(horizontal) > wasdThreshold || Mathf.Abs(vertical) > wasdThreshold)
        {
            if (navMeshAgent.hasPath) { navMeshAgent.SetDestination(gameObject.transform.position); }
            moveOffset = new Vector3(horizontal, 0f, vertical);
            return true;
        }
        return false;
    }

    private bool HandleMouseInput()
    {
        if (!Input.GetMouseButtonDown(1)) { return false; }

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity)) { return false; }

        CmdMoveToPoint(hit.point);
        return true;
    }
    #endregion
}

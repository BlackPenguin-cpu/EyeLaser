using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private LineRenderer eyeRayObj;

    private SpriteRenderer spriteRenderer;
    private EyeRayTarget currentEyeRayTarget;
    private LineRenderer currentEyeRay;
    private readonly Vector3 eyeRayAddPos = new Vector3(0, 0.5f);

    private List<EyeRayTarget> brainControlObjs = new List<EyeRayTarget>();

    private float onStartEyeRayTime;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        UpdateMoveInput();
        UpdateOnClickInput();

        BrainControlMarch();

        var sample = Camera.main.transform.position;
        sample.x = transform.position.x;
        Camera.main.transform.position = sample;
    }

    private void BrainControlMarch()
    {
        var xMargin = 1.5f * (spriteRenderer.flipX ? 1 : -1);
        var count = 0;
        foreach (var brainControlObj in brainControlObjs)
        {
            count++;
            var targetPos = new Vector3(transform.position.x + (xMargin * count)
                , transform.position.y, transform.position.z);

            brainControlObj.transform.position =
                Vector3.Lerp(brainControlObj.transform.position, targetPos, Time.deltaTime * 4f);
        }
    }

    private void UpdateOnClickInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GetEyeRayTarget();
        }

        if (currentEyeRay && Input.GetMouseButton(0))
        {
            OnEyeRay();
        }

        if (currentEyeRay && Input.GetMouseButtonUp(0))
        {
            EndEyeRay();
        }
    }

    private void GetEyeRayTarget()
    {
        var mousePos = Input.mousePosition;
        var layerMask = 1 << LayerMask.NameToLayer("EyeRayTarget");

        var rayTarget = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(mousePos), Vector2.zero, 100, layerMask);

        Debug.unityLogger.Log(rayTarget.collider);
        if (!rayTarget.collider || !rayTarget.collider.TryGetComponent(out EyeRayTarget eyeRayTarget)) return;

        currentEyeRayTarget = eyeRayTarget;

        var lr = Instantiate<LineRenderer>(eyeRayObj, transform.position, Quaternion.identity);
        lr.SetPosition(0, transform.position + eyeRayAddPos);
        lr.SetPosition(1, currentEyeRayTarget.transform.position + eyeRayAddPos);

        currentEyeRay = lr;
        onStartEyeRayTime = Time.time;
    }

    private void OnEyeRay()
    {
        if (Time.time - onStartEyeRayTime > 1.5f)
        {
            brainControlObjs.Add(currentEyeRayTarget);
            EndEyeRay();
        }
    }

    private void EndEyeRay()
    {
        Destroy(currentEyeRay.gameObject);
        currentEyeRay = null;
    }

    private void UpdateMoveInput()
    {
        if (currentEyeRay) return;
        var x = Input.GetAxisRaw("Horizontal") * Time.deltaTime;
        var y = Input.GetAxisRaw("Vertical") * Time.deltaTime;

        if (x != 0)
            spriteRenderer.flipX = x < 0;

        var targetPos = transform.position + new Vector3(x * 3, y * 2, 0);
        targetPos.y = Mathf.Clamp(targetPos.y, -4f, -0.5f);

        transform.position = targetPos;
    }
}
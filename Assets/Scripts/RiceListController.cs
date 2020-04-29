using System.Collections;
using UnityEngine;

public class RiceListController : MonoBehaviour
{
    [SerializeField] private Transform riceList;
    [SerializeField] private Transform sphereRice;
    private Mesh mesh;
    private MeshFilter meshFilter;
    private Vector3[] startMesh;

    private float distanceFromStart;
    private Vector3[] smallStartMesh;
    private float startSizeSphere;

    private bool isAreaClick;
    private bool isAllDraw;

    private void Start()
    {
        startSizeSphere = sphereRice.localScale.x;

        mesh = riceList.GetComponent<MeshFilter>().mesh;
        meshFilter = riceList.GetComponent<MeshFilter>();
        startMesh = mesh.vertices;

        ResizeMeshVerticles(0.01f);
        smallStartMesh = mesh.vertices;

        for (int i = 0; i < mesh.vertexCount; i++)
        {
            distanceFromStart += (startMesh[i] - smallStartMesh[i]).magnitude;
        }
    }

    private float UpdateCompletely()
    {
        float progress = 0;

        Vector3[] verticles = mesh.vertices;
        for (int i = 0; i < mesh.vertexCount; i++)
        {
            progress += (startMesh[i] - verticles[i]).magnitude;
        }

        return 1 - progress / distanceFromStart;
    }

    private void ResizeMeshVerticles(float value)
    {
        Vector3[] verticles = mesh.vertices;
        for (int i = 0; i < mesh.vertexCount; i++)
        {
            verticles[i] = verticles[i].normalized * value;
            verticles[i].y /= 2;
        }

        mesh.vertices = verticles;
        mesh.RecalculateNormals();
    }

    private IEnumerator StartRestoration()
    {
        float procentProgress = 0;
        while (procentProgress <= 0.999f)
        {
            float progress = 0;
            Vector3[] verticles = mesh.vertices;
            for (int i = 0; i < verticles.Length; i++)
            {
                progress += (startMesh[i] - verticles[i]).magnitude;
                verticles[i] = Vector3.MoveTowards(verticles[i], startMesh[i], 0.05f * Time.deltaTime);
            }
            procentProgress = 1 - progress / distanceFromStart;
            sphereRice.localScale = Vector3.one * startSizeSphere * (1 - procentProgress);
            EventManager.OnUpdateProgressBar?.Invoke(procentProgress+0.01f);
            mesh.vertices = verticles;
            mesh.RecalculateNormals();
            yield return null;
        }
        EventManager.OnLevelEnd?.Invoke();
    }


    private void ResizeMeshVerticles()
    {
        Vector3[] verticles = mesh.vertices;
        for (int i = 0; i < mesh.vertexCount; i++)
        {
            verticles[i] = startMesh[i];
        }

        mesh.vertices = verticles;
        mesh.RecalculateNormals();
        sphereRice.localScale = Vector3.zero;
    }

    private void OnMouseDrag()
    {
        if (!isAreaClick || isAllDraw)
        {
            return;
        }

        var mousePoint = GetPositionPointDown() * 0.02f;
        Vector2 mousePointV2 = new Vector2(mousePoint.x, mousePoint.z);
        Vector3[] verticles = mesh.vertices;
        float progress = 0;
        for (int i = 0; i < verticles.Length; i++)
        {
            progress += (startMesh[i] - verticles[i]).magnitude;

            var verticlesV2 = new Vector2(verticles[i].x, -verticles[i].y);
            if ((mousePointV2 - verticlesV2).magnitude < 0.02f)
            {
                verticles[i] = Vector3.Lerp(verticles[i], startMesh[i], 3 * Time.deltaTime);
            }
        }
        float procentProgress = 1 - progress / distanceFromStart;

        EventManager.OnUpdateProgressBar?.Invoke(procentProgress);
        if (procentProgress > 0.9f)
        {
            isAllDraw = true;
            StartCoroutine(StartRestoration());
            return;
        }
        sphereRice.localScale = Vector3.one * startSizeSphere * (1 - procentProgress);
        mesh.vertices = verticles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        meshFilter.sharedMesh = mesh;
    }

    private void OnMouseDown()
    {
        if (isAllDraw) return;
        gameObject.layer = 2;
        isAreaClick = true;
    }

    private void OnMouseUp()
    {
        isAreaClick = false;
        gameObject.layer = 0;
    }

    private Vector3 GetPositionPointDown()
    {
        if (!isAreaClick)
        {
            return Vector3.up * 10;
        }

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            return hit.point;
        }
        else
        {
            return Vector3.up * 10;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(GetPositionPointDown(), 0.1f);
    }
}

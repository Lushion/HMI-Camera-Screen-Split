using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class CameraSplitting : MonoBehaviour
{

    public int nbCameras = 1;

    private List<Camera> cameras = new List<Camera>();
    private Camera[,] cameraGrid;


    // Start is called before the first frame update
    void Start() { 

        int tableSize = Mathf.CeilToInt(Mathf.Sqrt(nbCameras));

        cameraGrid = new Camera[tableSize, tableSize];


        for (int i = 0; i < nbCameras; i++) {
            GameObject cameraGO = new GameObject { name = "Camera" + (i + 1) };
            Camera camera = cameraGO.AddComponent<Camera>();
            camera.GetComponent<Skybox>().enabled = false;

            cameras.Add(camera);


            int row = i % tableSize;
            int col = i / tableSize;

            cameraGrid[row, col] = camera;
        }

        List<List<Camera>> mainAxe = GetLongestLength(GetOccupiedLines(0)) > GetLongestLength(GetOccupiedLines(1)) ? GetOccupiedLines(0) : GetOccupiedLines(1);



        for (int i = 0; i < mainAxe.Count; i++) {
            for (int j = 0; j < mainAxe[i].Count; j++) {

                mainAxe[i][j].rect = new Rect((float)j / mainAxe[i].Count, (float)i / mainAxe.Count, 1f / mainAxe[i].Count, 1f / mainAxe.Count);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        GetCameraFromMouse();
    }

    void GetCameraFromMouse() {

        Vector2 mousePosition = Camera.main.ScreenToViewportPoint(Input.mousePosition);

        Camera selectedCamera = cameras.Find(cam => cam.rect.x < mousePosition.x && cam.rect.x + cam.rect.width > mousePosition.x && cam.rect.y < mousePosition.y && cam.rect.y + cam.rect.height > mousePosition.y);


    }

    void OnHovered(Camera camera) {
        camera.backgroundColor = new Color();
    }

    void OnHoverExit(Camera camera) {

    }

    /// <summary>
    /// If rc = 0, get rows occupied by at least a camera, else if rc = 1, get columns occupied by at least a camera
    /// 
    /// </summary>
    /// <param name="rc">If rc = 0, get rows occupied by at least a camera, else if rc = 1, get columns occupied by at least a camera</param>
    /// <returns></returns>
    List<List<Camera>> GetOccupiedLines(int rc) {

        List<List<Camera>> occupiedRows = new List<List<Camera>>(); 

        if (rc != 0 && rc != 1) {
            Debug.LogError("Value passed as a parameter must be 0 or 1");
        }


        for (int j = 0; j < cameraGrid.GetLength(1-rc); j++) {
            
            int count = 0;
            List<Camera> cameraInRow = new List<Camera>();

            for (int i = 0; i < cameraGrid.GetLength(rc); i++) {

                if (cameraGrid[i, j] != null) {
                    cameraInRow.Add(cameraGrid[i, j]);
                }

                else { 
                    count++;
                }
            }

            if (count == cameraGrid.GetLength(rc)) {
                break;
            }

            occupiedRows.Add(cameraInRow);
        }

        return occupiedRows;
    }



    int GetLongestLength(List<List<Camera>> list) {

        List<int> countList = new List<int>();
            
        list.ForEach(delegate (List<Camera> sublist) {
            countList.Add(sublist.Count);
        });

        return countList.Max();
    }   
}

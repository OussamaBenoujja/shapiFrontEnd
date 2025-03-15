using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

[Serializable]
public class Department
{
    public int id;
    public string name;
    public string description;
    public string slug;
    public string created_at;
    public string updated_at;
}

[Serializable]
public class DepartmentResponse
{
    public List<Department> data;
}

public class DepartmentManager : MonoBehaviour
{
    [Header("API Settings")]
    [SerializeField] private string apiBaseUrl = "http://localhost:8000/api/v1";
    
    [Header("Department Prefab")]
    [SerializeField] private GameObject departmentPrefab;
    
    [Header("Department Position Objects")]
    [SerializeField] private Transform[] positionObjects; // Array to store the objects with React transform
    
    public string GetApiBaseUrl()
    {
        return apiBaseUrl;
    }
    
    private void Start()
    {
        StartCoroutine(GetAllDepartments());
    }
    
    private IEnumerator GetAllDepartments()
    {
        string url = $"{apiBaseUrl}/departments";
        
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // Send the request and wait for response
            yield return webRequest.SendWebRequest();
            
            if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error: {webRequest.error}");
            }
            else
            {
                // Parse the JSON response
                string jsonResponse = webRequest.downloadHandler.text;
                DepartmentResponse response = JsonUtility.FromJson<DepartmentResponse>(jsonResponse);
                
                // Instantiate department objects
                InstantiateDepartments(response.data);
            }
        }
    }
    
    private void InstantiateDepartments(List<Department> departments)
    {
        if (departmentPrefab == null)
        {
            Debug.LogError("Department prefab is not assigned!");
            return;
        }
        
        if (positionObjects == null || positionObjects.Length == 0)
        {
            Debug.LogError("No position objects assigned!");
            return;
        }
        
        // Check if departments exceed available positions
        if (departments.Count > positionObjects.Length)
        {
            Debug.Log($"Over the limit: {departments.Count} departments but only {positionObjects.Length} positions available");
        }
        
        // Use only as many departments as we have positions for
        int count = Mathf.Min(departments.Count, positionObjects.Length);
        
        for (int i = 0; i < count; i++)
        {
            Department dept = departments[i];
            Transform positionTransform = positionObjects[i];
            
            // Instantiate the prefab at the position and rotation of the target object
            GameObject departmentObject = Instantiate(departmentPrefab, positionTransform.position, positionTransform.rotation);
            
            // Rename the game object
            departmentObject.name = $"Department_{dept.name}";
            
            // Set department data
            DepartmentDisplay display = departmentObject.GetComponent<DepartmentDisplay>();
            if (display != null)
            {
                display.SetDepartmentData(dept);
            }
        }
    }
}
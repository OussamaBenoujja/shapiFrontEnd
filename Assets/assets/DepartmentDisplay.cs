using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using System;

public class DepartmentDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshPro nameText;
    [SerializeField] private TextMeshPro descriptionText;
    [SerializeField] private MeshRenderer departmentRenderer;
    
    [SerializeField] private Transform[] boxPoints;
    [SerializeField] private GameObject productPrefab;
    
    private Department departmentData;
    private List<GameObject> instantiatedProducts = new List<GameObject>();
    private bool productsLoaded = false;
    
public void SetDepartmentData(Department department)
{
    departmentData = department;
    
    if (nameText != null)
    {
        nameText.text = department.name;
    }
    
    if (descriptionText != null)
    {
        descriptionText.text = department.description;
    }
    
    // Optionally assign a random color to visually differentiate departments
    if (departmentRenderer != null)
    {
        departmentRenderer.material.color = UnityEngine.Random.ColorHSV(0f, 1f, 0.7f, 1f, 0.7f, 1f);
    }
    
    // Automatically load products
    StartCoroutine(GetProductsForDepartment(department.id));
}
    // You can add more methods to handle interaction with departments
    private void OnMouseDown()
    {
        // Get products for this department when clicked
        if (departmentData != null)
        {
            Debug.Log($"Clicked on department: {departmentData.name}");
            
            if (!productsLoaded)
            {
                StartCoroutine(GetProductsForDepartment(departmentData.id));
            }
            else
            {
                // Toggle visibility of products if they're already loaded
                ToggleProductsVisibility();
            }
        }
    }
    
    private void ToggleProductsVisibility()
    {
        foreach (GameObject product in instantiatedProducts)
        {
            product.SetActive(!product.activeSelf);
        }
    }
    
    private IEnumerator GetProductsForDepartment(int departmentId)
    {
        string apiBaseUrl = FindObjectOfType<DepartmentManager>().GetApiBaseUrl();
        string url = $"{apiBaseUrl}/departments/{departmentId}/products";
        
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
                ProductResponse response = JsonUtility.FromJson<ProductResponse>(jsonResponse);
                
                // Instantiate product objects
                InstantiateProducts(response.data);
                productsLoaded = true;
            }
        }
    }
    
    private void InstantiateProducts(List<Product> products)
    {
        if (productPrefab == null)
        {
            Debug.LogError("Product prefab is not assigned!");
            return;
        }
        
        // Clear any existing products first
        ClearProducts();
        
        // Calculate how many products we can display based on available boxPoints
        int maxProducts = Mathf.Min(products.Count, boxPoints.Length);
        
        for (int i = 0; i < maxProducts; i++)
        {
            // Instantiate the product at the boxpoint position
            GameObject productObject = Instantiate(productPrefab, boxPoints[i].position, boxPoints[i].rotation);
            productObject.transform.parent = transform; // Parent to department
            
            // Rename the game object
            productObject.name = $"Product_{products[i].name}";
            
            // Set product data
            ProductDisplay display = productObject.GetComponent<ProductDisplay>();
            if (display != null)
            {
                display.SetProductData(products[i]);
            }
            
            // Add to our list for tracking
            instantiatedProducts.Add(productObject);
        }
        
        if (products.Count > boxPoints.Length)
        {
            Debug.LogWarning($"Department {departmentData.name} has {products.Count} products but only {boxPoints.Length} display points!");
        }
    }
    
    private void ClearProducts()
    {
        foreach (GameObject product in instantiatedProducts)
        {
            Destroy(product);
        }
        instantiatedProducts.Clear();
    }
}
using UnityEngine;
using TMPro;

public class ProductDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshPro nameText;
    [SerializeField] private TextMeshPro priceText;
    [SerializeField] private TextMeshPro stockText;
    [SerializeField] private MeshRenderer productRenderer;
    
    private Product productData;
    
    public void SetProductData(Product product)
    {
        productData = product;
        
        if (nameText != null)
        {
            nameText.text = product.name;
        }
        
        if (priceText != null)
        {
            priceText.text = $"${product.price:F2}";
        }
        
        if (stockText != null)
        {
            stockText.text = $"Stock: {product.stock_quantity}";
        }
        
        // Set color based on stock (red if low, green if good)
        if (productRenderer != null)
        {
            if (product.stock_quantity <= product.min_stock_threshold)
            {
                productRenderer.material.color = new Color(0.8f, 0.2f, 0.2f); // Red for low stock
            }
            else
            {
                productRenderer.material.color = new Color(0.2f, 0.8f, 0.2f); // Green for good stock
            }
        }
    }
    
    private void OnMouseDown()
    {
        if (productData != null)
        {
            Debug.Log($"Clicked on product: {productData.name} - Price: ${productData.price:F2}");
            // Here you could show a detailed product view or other interaction
        }
    }
}
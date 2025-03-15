using System;
using System.Collections.Generic;

[Serializable]
public class Product
{
    public int id;
    public string name;
    public string description;
    public float price;
    public int stock_quantity;
    public int min_stock_threshold;
    public string slug;
    public string category;
    public bool is_promotional;
    public int department_id;
    public string created_at;
    public string updated_at;
}

[Serializable]
public class ProductResponse
{
    public List<Product> data;
}
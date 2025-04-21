﻿namespace DataLayerRefactoring.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<Product> Products { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

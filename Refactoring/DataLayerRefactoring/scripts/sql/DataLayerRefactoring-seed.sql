DELETE FROM Products;
DELETE FROM Categories;

DELETE FROM sqlite_sequence WHERE name='Products';
DELETE FROM sqlite_sequence WHERE name='Categories';

INSERT INTO Categories (Name, Description, CreatedAt) 
VALUES 
('Electronics', 'Electronic devices and accessories', datetime('now')),
('Clothing', 'Apparel and fashion items', datetime('now')),
('Furniture', 'Home and office furniture', datetime('now')),
('Books', 'Books and publications', datetime('now')),
('Sports', 'Sports equipment and gear', datetime('now'));

-- Seed Products
-- Electronics category (ID 1)
INSERT INTO Products (Name, Description, Price, Stock, CategoryId, CreatedAt) 
VALUES 
('Smartphone', 'Latest model smartphone with advanced features', 699.99, 100, 1, datetime('now')),
('Laptop', 'High-performance laptop for professionals', 1299.99, 50, 1, datetime('now')),
('Wireless Earbuds', 'Premium wireless earbuds with noise cancellation', 149.99, 75, 1, datetime('now')),
('Tablet', '10-inch tablet with high-resolution display', 399.99, 30, 1, datetime('now')),
('Smart Watch', 'Fitness and health tracking smartwatch', 249.99, 45, 1, datetime('now'));

-- Clothing category (ID 2)
INSERT INTO Products (Name, Description, Price, Stock, CategoryId, CreatedAt) 
VALUES 
('T-Shirt', '100% cotton comfortable t-shirt', 19.99, 200, 2, datetime('now')),
('Jeans', 'Classic denim jeans', 49.99, 150, 2, datetime('now')),
('Sweater', 'Warm winter sweater', 39.99, 100, 2, datetime('now')),
('Jacket', 'Waterproof outdoor jacket', 89.99, 75, 2, datetime('now')),
('Sneakers', 'Comfortable athletic shoes', 79.99, 90, 2, datetime('now'));

-- Furniture category (ID 3)
INSERT INTO Products (Name, Description, Price, Stock, CategoryId, CreatedAt) 
VALUES 
('Office Chair', 'Ergonomic office chair with lumbar support', 199.99, 30, 3, datetime('now')),
('Desk', 'Modern computer desk with storage', 249.99, 25, 3, datetime('now')),
('Bookshelf', 'Wooden bookshelf with 5 shelves', 129.99, 20, 3, datetime('now')),
('Sofa', 'Three-seater sofa with premium fabric', 599.99, 15, 3, datetime('now')),
('Coffee Table', 'Glass top coffee table with wooden legs', 149.99, 25, 3, datetime('now'));

-- Books category (ID 4)
INSERT INTO Products (Name, Description, Price, Stock, CategoryId, CreatedAt) 
VALUES 
('Programming Guide', 'Comprehensive programming guide for beginners', 34.99, 50, 4, datetime('now')),
('Science Fiction Novel', 'Bestselling sci-fi novel', 14.99, 100, 4, datetime('now')),
('Cookbook', 'Collection of gourmet recipes', 24.99, 40, 4, datetime('now')),
('History Book', 'Detailed history of ancient civilizations', 29.99, 35, 4, datetime('now')),
('Self-Help Book', 'Guide to personal development', 19.99, 60, 4, datetime('now'));

-- Sports category (ID 5)
INSERT INTO Products (Name, Description, Price, Stock, CategoryId, CreatedAt) 
VALUES 
('Basketball', 'Official size and weight basketball', 29.99, 40, 5, datetime('now')),
('Tennis Racket', 'Professional tennis racket', 89.99, 25, 5, datetime('now')),
('Yoga Mat', 'Non-slip yoga mat', 24.99, 60, 5, datetime('now')),
('Dumbbells', 'Set of adjustable dumbbells', 149.99, 20, 5, datetime('now')),
('Running Shoes', 'Lightweight running shoes', 99.99, 35, 5, datetime('now'));
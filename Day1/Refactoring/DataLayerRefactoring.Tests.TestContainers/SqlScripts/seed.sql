INSERT INTO "Categories" ("Name", "Description")
VALUES 
    ('Electronics', 'Electronic devices and accessories'),
    ('Books', 'Books and publications')
ON CONFLICT DO NOTHING;

INSERT INTO "Products" ("Name", "Description", "Price", "Stock", "CategoryId")
VALUES 
    ('Smartphone', 'Latest model smartphone', 799.99, 50, 1),
    ('Laptop', 'High-performance laptop', 1299.99, 25, 1),
    ('Programming Guide', 'Comprehensive programming guide', 49.99, 100, 2)
ON CONFLICT DO NOTHING;
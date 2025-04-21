-- OrderProcessingAPI Seed Data Script
-- Ensure tables are clean before seeding
DELETE FROM Orders;

-- Seed Orders with random GUIDs and dates
INSERT INTO Orders (Id, CustomerName, ProductId, Quantity, TotalPrice, OrderDate, Status) 
VALUES 
('8f2e9fad-8c2c-4320-82fb-336843bdbe31', 'John Smith', 'PROD-001', 2, 1399.98, datetime('now', '-30 days'), 'Delivered'),
('6d4cb9f1-93e4-4d88-9bbb-23ba83231656', 'Emily Johnson', 'PROD-002', 1, 1299.99, datetime('now', '-25 days'), 'Delivered'),
('d8e4a721-87c2-4daa-9d04-b58c8e6b101f', 'Michael Brown', 'PROD-003', 3, 449.97, datetime('now', '-20 days'), 'Shipped'),
('f4d3c9c6-2c15-4e88-9abc-8ddca6c7d9a2', 'Sarah Williams', 'PROD-001', 1, 699.99, datetime('now', '-15 days'), 'Shipped'),
('a2f9c1a3-3b62-48a7-b7df-59c23498bb12', 'David Miller', 'PROD-004', 2, 799.98, datetime('now', '-10 days'), 'Processing'),
('7ccb8d4e-0a5e-4385-9dd0-4f3c791f5a3b', 'Jessica Davis', 'PROD-005', 1, 249.99, datetime('now', '-5 days'), 'Processing'),
('b0e1f2c5-d6a7-4b89-9c0d-e5a6f7b8c9da', 'Robert Wilson', 'PROD-006', 4, 79.96, datetime('now', '-3 days'), 'Processing'),
('e7d6c5b4-a3f2-41e0-b9d8-c7a6b5d4e3f2', 'Jennifer Taylor', 'PROD-003', 2, 299.98, datetime('now', '-2 days'), 'Pending'),
('1a2b3c4d-5e6f-7a8b-9c0d-1e2f3a4b5c6d', 'Christopher Anderson', 'PROD-007', 3, 149.97, datetime('now', '-1 days'), 'Pending'),
('f1e2d3c4-b5a6-4978-87d6-e5f4c3b2a1d0', 'Amanda Thomas', 'PROD-002', 1, 1299.99, datetime('now'), 'Pending');
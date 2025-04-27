IF OBJECT_ID('dbo.Sessions', 'U') IS NOT NULL DROP TABLE dbo.Sessions;
IF OBJECT_ID('dbo.Events', 'U') IS NOT NULL DROP TABLE dbo.Events;

CREATE TABLE dbo.Events (
                            Id INT IDENTITY(1,1) PRIMARY KEY,
                            Title NVARCHAR(100) NOT NULL,
                            Description NVARCHAR(MAX) NOT NULL,
                            StartDate DATETIME2 NOT NULL,
                            EndDate DATETIME2 NOT NULL,
                            VenueName NVARCHAR(100) NOT NULL,
                            Capacity INT NOT NULL,
                            Type INT NOT NULL, -- 0: Standalone, 1: Series, 2: SubEvent
                            ParentEventId INT NULL,
                            Status INT NOT NULL, -- 0: Draft, 1: Published, 2: Canceled, 3: Completed
                            BannerImageUrl NVARCHAR(255) NULL,
                            CONSTRAINT FK_Events_ParentEvent FOREIGN KEY (ParentEventId) REFERENCES dbo.Events(Id)
);

CREATE TABLE dbo.Sessions (
                              Id INT IDENTITY(1,1) PRIMARY KEY,
                              Title NVARCHAR(100) NOT NULL,
                              Description NVARCHAR(MAX) NULL,
                              StartTime DATETIME2 NOT NULL,
                              EndTime DATETIME2 NOT NULL,
                              Location NVARCHAR(100) NULL,
                              EventId INT NOT NULL,
                              SpeakerName NVARCHAR(100) NULL,
                              SpeakerBio NVARCHAR(MAX) NULL,
                              CONSTRAINT FK_Sessions_Event FOREIGN KEY (EventId) REFERENCES dbo.Events(Id)
);

INSERT INTO dbo.Events (Title, Description, StartDate, EndDate, VenueName, Capacity, Type, ParentEventId, Status, BannerImageUrl)
VALUES
('Summer Music Festival',
 'Join us for a day of amazing music performances by top artists in an outdoor setting.',
 '2025-06-15T16:00:00', '2025-06-15T23:00:00',
 'Central Park', 5000, 0, NULL, 1, -- Published Standalone event
 '/assets/images/summer-festival.jpg'),

('Corporate Team Building',
 'A full day of team building activities for corporate groups, designed to improve collaboration and communication.',
 '2025-05-10T09:00:00', '2025-05-10T17:00:00',
 'Mountain Resort', 200, 0, NULL, 0, -- Draft Standalone event
 '/assets/images/team-building.jpg'),

('Charity Gala Dinner',
 'Annual fundraising dinner to support local community initiatives. Formal attire required.',
 '2025-10-25T19:00:00', '2025-10-25T23:00:00',
 'Grand Ballroom Hotel', 350, 0, NULL, 1, -- Published Standalone event
 '/assets/images/charity-gala.jpg'),

-- Series Events
('Tech Conference 2025',
 'Three-day technology conference featuring industry leaders, workshops, and networking opportunities.',
 '2025-09-15T08:00:00', '2025-09-17T18:00:00',
 'Tech Convention Center', 1200, 1, NULL, 1, -- Published Series event
 '/assets/images/tech-conference.jpg'),

('Film Festival',
 'A week-long celebration of independent cinema from around the world.',
 '2025-07-20T10:00:00', '2025-07-26T22:00:00',
 'Downtown Cinema Complex', 800, 1, NULL, 0, -- Draft Series event
 '/assets/images/film-festival.jpg'),

('Wellness Retreat',
 'A weekend of self-care, mindfulness, and health-focused activities.',
 '2025-08-05T14:00:00', '2025-08-07T12:00:00',
 'Sunset Resort & Spa', 100, 1, NULL, 1, -- Published Series event
 '/assets/images/wellness-retreat.jpg');

INSERT INTO dbo.Events (Title, Description, StartDate, EndDate, VenueName, Capacity, Type, ParentEventId, Status, BannerImageUrl)
VALUES
('Opening Keynote - Future of AI',
 'Join our CEO and special guests as they discuss the future of AI and its impact on society and business.',
 '2025-09-15T09:00:00', '2025-09-15T10:30:00',
 'Main Auditorium', 1200, 2, 4, 1, -- Sub-Event of Tech Conference
 '/assets/images/keynote.jpg'),

('Blockchain Workshop',
 'Hands-on workshop exploring the latest developments in blockchain technology.',
 '2025-09-15T13:00:00', '2025-09-15T16:00:00',
 'Workshop Room A', 100, 2, 4, 1, -- Sub-Event of Tech Conference
 '/assets/images/blockchain.jpg'),

('Networking Reception',
 'Meet fellow attendees, speakers, and sponsors over drinks and hors oeuvres.', 
 '2025-09-15T18:00:00', '2025-09-15T20:00:00', 
 'Grand Foyer', 1000, 2, 4, 1, -- Sub-Event of Tech Conference
 '/assets/images/networking.jpg'),

('Emerging Technologies Panel', 
 'Industry experts discuss the latest trends and emerging technologies in the digital landscape.', 
 '2025-09-16T10:00:00', '2025-09-16T11:30:00', 
 'Main Auditorium', 1200, 2, 4, 1, -- Sub-Event of Tech Conference
 '/assets/images/panel.jpg'),

-- Film Festival Sub-Events
('Opening Night Premiere', 
 'Red carpet premiere of award-winning international feature film with director Q&A.', 
 '2025-07-20T19:00:00', '2025-07-20T22:30:00', 
 'Main Theater', 500, 2, 5, 0, -- Sub-Event of Film Festival
 '/assets/images/film-premiere.jpg'),

('Documentary Showcase', 
 'Selection of thought-provoking documentary films exploring current social issues.', 
 '2025-07-21T13:00:00', '2025-07-21T18:00:00', 
 'Theater 2', 300, 2, 5, 0, -- Sub-Event of Film Festival
 '/assets/images/documentary.jpg'),

-- Wellness Retreat Sub-Events
('Morning Yoga', 
 'Start your day with an invigorating yoga session suitable for all experience levels.', 
 '2025-08-05T07:00:00', '2025-08-05T08:30:00', 
 'Sunset Pavilion', 50, 2, 6, 1, -- Sub-Event of Wellness Retreat
 '/assets/images/yoga.jpg'),

('Nutrition Workshop', 
 'Learn about balanced nutrition and meal planning for optimal health with our expert nutritionist.', 
 '2025-08-05T10:00:00', '2025-08-05T12:00:00', 
 'Workshop Room', 75, 2, 6, 1, -- Sub-Event of Wellness Retreat
 '/assets/images/nutrition.jpg'),

('Meditation Session', 
 'Guided meditation focused on mindfulness and stress reduction techniques.', 
 '2025-08-06T16:00:00', '2025-08-06T17:30:00', 
 'Zen Garden', 50, 2, 6, 1, -- Sub-Event of Wellness Retreat
 '/assets/images/meditation.jpg');

INSERT INTO dbo.Sessions (Title, Description, StartTime, EndTime, Location, EventId, SpeakerName, SpeakerBio)
VALUES
('Welcome Address', 
 'Opening remarks and welcome to the conference', 
 '2025-09-15T09:00:00', '2025-09-15T09:15:00', 
 'Main Stage', 7, 'Jennifer Walters', 'CEO of TechCorp'),

('The Future of Machine Learning', 
 'Exploration of where ML technology is headed in the next decade', 
 '2025-09-15T09:15:00', '2025-09-15T10:00:00', 
 'Main Stage', 7, 'Dr. Samuel Reid', 'AI Research Director at FutureTech Labs'),

('Q&A Session', 
 'Open questions with our keynote speakers', 
 '2025-09-15T10:00:00', '2025-09-15T10:30:00', 
 'Main Stage', 7, NULL, NULL),

-- Sessions for Blockchain Workshop
('Blockchain Fundamentals', 
 'Introduction to blockchain technology and its key concepts', 
 '2025-09-15T13:00:00', '2025-09-15T14:00:00', 
 'Workshop Room A', 8, 'Michael Chen', 'Blockchain Developer & Educator'),

('Smart Contract Development', 
 'Hands-on practice building and deploying smart contracts', 
 '2025-09-15T14:00:00', '2025-09-15T15:30:00', 
 'Workshop Room A', 8, 'Sophia Rodriguez', 'Lead Engineer at BlockTech Solutions'),

-- Sessions for Charity Gala
('Welcome & Cocktail Reception', 
 'Greeting and mingling with guests', 
 '2025-10-25T19:00:00', '2025-10-25T19:45:00', 
 'Foyer', 3, NULL, NULL),

('Dinner & Keynote Address', 
 'Formal dinner with address from charity founder', 
 '2025-10-25T19:45:00', '2025-10-25T21:00:00', 
 'Main Ballroom', 3, 'Dr. Elizabeth Grant', 'Founder of Community First Foundation'),

('Fundraising Auction', 
 'Live auction of donated items to raise funds for the charity', 
 '2025-10-25T21:00:00', '2025-10-25T22:30:00', 
 'Main Ballroom', 3, 'Robert James', 'Professional Auctioneer');

SELECT 'Created ' + CAST(COUNT(*) AS VARCHAR) + ' events' FROM dbo.Events;
SELECT 'Created ' + CAST(COUNT(*) AS VARCHAR) + ' sessions' FROM dbo.Sessions;
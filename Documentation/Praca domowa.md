# Data Layer Refactoring

Należy wprowadzić do tego projektu 2 patterny komunikacji z bazą:

1) [CQRS](https://learn.microsoft.com/en-us/aspnet/mvc/overview/older-versions/getting-started-with-ef-5-using-mvc-4/implementing-the-repository-and-unit-of-work-patterns-in-an-asp-net-mvc-application#create-a-generic-repository)[](https://learn.microsoft.com/en-us/aspnet/mvc/overview/older-versions/getting-started-with-ef-5-using-mvc-4/implementing-the-repository-and-unit-of-work-patterns-in-an-asp-net-mvc-application#create-a-generic-repository)
2) [Unit of Work](https://learn.microsoft.com/en-us/aspnet/mvc/overview/older-versions/getting-started-with-ef-5-using-mvc-4/implementing-the-repository-and-unit-of-work-patterns-in-an-asp-net-mvc-application#create-a-generic-repository)

Przed wprowadzeniem zmiany należy napisać testy, które zapewnia poprawność zmian.

Chcemy bazować na abstrakcji.

Pamiętaj o dobrych praktykach.
# Communication Pattern

Należy wprowadzić do tego projektu nowe sposoby komunikacji:

1) GraphQL
2) Event Based
3) gRPC

Dla ułatwienia testowania możemy równolegle utrzymywać aplikację kliencką, która będzie się komunikować z naszym API.
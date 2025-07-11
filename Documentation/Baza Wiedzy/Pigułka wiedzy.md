# LLM

LLM są to aplikacje, których algorytmy zostały nauczone na ogromnych zbiorach tekstów. Dzięki temu są One w stanie w bardzo dużym stopniu odwzorowywać sposób porozumiewania się mową ludzką.
LLM są częścią grupy aplikacji GenAI, które wykorzystują wyuczone algorytmy do generowania różnej treści. Modele mogą być wytrenowane na:
- teskstach - LLM
- obrazach czy wideo - używane do generowania adekwatnej treści
- multimodalnie - modele wytrenowane zarówno na obrazach jak i tesktach. Przykładowo Gemini.
Ich naturą jest niedeterministyczność, każda interakcja będzie dążyć do chaotycznego celu, dwie te same interakcje nie dadzą tego samego rezultatu.

## Uczenie

Aby osiągnąć swoją funkcjonalność modele są trenowane na podstawie zgromadzonych i zrefinowanych treści. Uczenie modeli można podzielić na 2 duże kroki.
- pre training - faza polegająca na stworzeniu bazy wektorowej z treści przekazanej do modelu. W dużym uproszeniu można sobie wyobrazić, że całości treści tworzona jest skondensowana paczka informacji z których model będzie korzystać. Z tego etapu wynika knowledge cut off date.
- fine tunning - wykonywany po fazie pre trainingu. Polega na manualnym dostrojeniu modelu na podstawie interakcji z modelem. Dostosowane jest długość, precyzja, ton odpowiedzi modelu, poprzez manipulowanie wagami różnych czynników. 
[Wideo](https://www.youtube.com/watch?v=7xTGNNLPyMI&t=3600s) tłumaczące ten proces w detalach.
# Prompty

Podstawowym rodzajem interakcji z LLM jest prowadzenie z nim rozmowy za pomocą czatu.  Tym co wysyłamy na tym czacie są prompty. Dokładność odpowiedzi, którą uzyskamy od czatu jest zależna od dokładności naszego prompty. Im lepiej rozumiem jaki efekt chcemy uzyskać, tym dokładniej jesteśmy w stanie skonstruować nasz prompt celem uzyskania tego rezlutatu.

> [!info] > Złożoność  promptu powinna być proporcjonalna do dokładności odpowiedzi, którą chcemy uzyskać. Im lepiej rozumiemy rezultat, który chcemy uzyskać, tym bardziej złożony prompt chcemy skonstruować

Prompt możemy konstruować za pomocą takich fragmentów:

**Musi być**
Zadanie - określa problem, który chcemy, żeby model dla nas rozwiązał. Sprawdza się lepiej jeśli określimy co będzie sukcesem.
	Dobrze: Napisz maila, który opisze stan projektu.
	Lepiej: Napisz maila o statusie projektu, który pozwoli stake holderom zrozumieć obecny stan zadań, co zostało już zrobione oraz jakich blockerów się spodziewamy. 
	
**Dobrze żeby było**
Kontekst - zawiera wszystkie informacje, które mogą być kluczowe dla rozwiązania zadania. 
Przykład - przykładowy dobrze napisany mail, dobrze napisany fragment kodu etc. Ukierunkuje to model na formę i składnie odpowiedzi, który ma przygotować.

**Dodatkowe elementy**
Persona - określa osobowość z jaką czat ma wygenerować odpowiedź. Pomoże to czatowi dobrać odpowiednią złożoność języka użytego w odpowiedzi. Przykładowo: QA Engineer, Product Owner, Nauczyciel, Junior Developer, Senior Developer etc.
Format - jeśli format odpowiedzi ma być specjalny możemy to podać w wiadomości. Przykładowo: odpowiedz jednym słowem, odpowiedz w formacie JSON o takiej struckturze ...
Ton - przykładowo: formalny, dowcipny. 

Konstruując prompt z użyciem tych elementów możemy uzyskać znacznie dokładniejszą odpowiedź. Nie trzeba tych elementów specjalnie wyróżniać, wystarczy użyć tych elementów podczas konstruowania zwykłej wiadomości.
## Prompty Systemowe

Prompty systemowe to sposób na kontrolowanie zachowania czatu, określa On jakie zadanie model ma rozwiązywać, jak odpowiadać. Najprostszym sposobem na wykorzystanie tej funkcjonalności są [Gemy](https://gemini.google.com/gems/view) w Gemini.

Gemy chcemy stworzyć bardzo dokładnie. 
- wykorzystujemy całą strukturę dobrego promptu
- zawieramy przykład, który składa się z
	-  promtu użytkownika (example prompt: ...)
	- odpowiedź, której oczekujemy (example response: ...)

Systemowe prompty, możemy użyć do:
-  [[Prompty użyte na zajęciach#Tworzenie testów.|tworzenia testów kodu]] - podajemy biblioteki z jakich model ma korzystać, konwencje nazewnictwa testów, instruujemy aby odpowiadał tylko kodem. Podajemy jako przykład klasę, którą chcemy przetestować a jako odpowiedź testy napisane przez nas.
- pisania powtarzalnych maili. Przykładowo o statusie projektu.
- uczenia się - instruujemy czat żeby podawał nam tylko część informacji nie całą poprawną odpowiedź od razu, etc. 
## Rozmowa

Sama rozmowa też posiada swoje cechy, które dobrze zrozumieć dla prowadzenia uzyskania lepszych wyników.
W trakcie prowadzenia pojedynczego czatu model buduje sobie kontekst rozmowy, który będzie używać do udzielania odpowiedzi. Będzie się starał zrozumieć całość przekazanych informacji tak abyśmy my nie musieli powtarzać rzeczy, które już napisaliśmy. 
	Rozmowę o danym problemie czy zagadnieniu prowadzimy wewnątrz 1 konwersacji
	Nowy temat - nowe okno czatu.
	
Po za kontekstem rozmowy ChatGPT buduje sobie kontekst nadrzędny, który zbiera informacje o nas jako rozmówcach. Można zawsze zapytać ChatGPT co o Nas wie.
# Narzędzia do programowania

Do programowania jako asystenta możemy użyć wielu różnych narzędzi. Najpopularniejsze z nich to:
- czat
- asystent CLI - codex, claude code
- zagnieżdżony asystent w IDE -  copilot, cascade
- dedykowane IDE - cursor, zed
# Narzędzia trzecie


## MCP
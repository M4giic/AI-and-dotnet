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
## Czat

Czat jest najprostszym narzędziem, które można wykorzystać podczas programowania. Zasady, którymi się kierujemy są tożsame z tymi wcześniej nakreślonymi. 
Najważniejszą rzeczą podczas wykorzystywania czatu do tworzenia aplikacji jest pierwsze kilka wiadomości oraz prompt inicjujący. Pierwszy prompt powinien zawierać pełnie informacji na temat naszej aplikacji:
- techonologia
- zastosowanie biznesowe
- strukturę - DDD etc.
- używane biblioteki
- charakterystyki techniczne - file scoped namespace
Im więcej takich informacji zawrzemy tym lepszą aplikację czat dla nas wygeneruje. Przykładowy kompletny prompt zawiera się tutaj - [[Prompty użyte na zajęciach#Prompt do tworzenia aplikacji webowej w ASP.NET|tworzenie aplikacji]]. Taki prompt dobrze zakończyć słowami:
	**Jeśli masz jakieś pytania zadaj mi je zanim zaczniesz generować kod**
Taka wiadomość pozwoli nam na poprowadzenie krótkiej rozmowy przed generowaniem kodu o kwestiach, które mogliśmy omyłkowo pominąć podczas pisania naszego pierwszego promptu.

> [!info] > Jeśli znam dokładnie tworzoną aplikacje to w pierwszym promptcie chcemy zawrzeć pełnię informacji (technolgię, zastosowanie biznesowe) oraz zakończyć zdaniem: Jeśli masz jakieś pytania zadaj mi je zanim zaczniesz generować kod

## Asystent CLI

OpenAI oraz Anthropic mają w swojej ofercie narzędzia zintegrowane do CLI. Nąrzędzia te nazywają się 
- Codex od OpenAI
- Claude Code od Anthropic
Takie narzędzia działają tylko na Linuxie i można je zainstalować w taki sposób:
``` bash
sudo apt update
sudo apt install python3-pip
curl -o- https://raw.githubusercontent.com/nvm-sh/nvm/master/install.sh | bash
nvm -v #jak nie działa to może być koniecznie zrestartowanie terminala
nvm install --lts
nvm install node
npm install -g @anthropic-ai/claude-code
claude # mamy uruchomiony program
```
adekwatnie instalowanie codex wygląda tak:
``` bash
npm install -g @openai/codex
codex
```
żeby uruchomić takiego asystenta musimy jeszcze podać klucz api. W ten sposób:
``` bash
export ANTHROPIC_API_KEY="<ANT_KEY>"
export OPENAI_API_KEY="<OAI_KEY>"
```
następnie takie narzędzie zostanie uruchomione i będziemy z nim pracować w formie agentowej. Będziemy prowadzić rozmowę, której celem będzie stowrzenie kodu, przy czym dajemy aplikacji bardzo duży poziom autonomii. Będzie w stanie wykonywać za nas komendy a CLI, tworzyć, usuwać modyfikować pliki etc. 
Jest to bardzo holistyczna opcja i raczej jest skierowana do osób, które głównie programują w CLI.

## Zagnieżdżony asystent

Asystenta możemy zagnieździć w naszym IDE tak aby wspierał nas w codziennych działaniach. Damy mu pewną zdolność działania w zależności od wybranego trybu.
Najpopularniejszym asystentem jest oczywiście CoPilot. Alternatywą jest [Cascade](https://windsurf.com/cascade) od Windsurf (który ja osobiście preferuje).

GitHub Copilot to asystent programowania oparty na sztucznej inteligencji, który pomaga programistom pisać kod szybciej i wydajniej, oferując sugestie kodu, generując testy jednostkowe, wyjaśniając kod i wiele więcej.

#### Jak dodać Copilota (przez plugin)

GitHub Copilot jest dostępny jako rozszerzenie (plugin) dla różnych środowisk IDE. Proces instalacji jest podobny w większości z nich:
- Otwórz widok rozszerzeń 
- Wyszukaj "GitHub Copilot" i zainstaluj rozszerzenie.
- Po instalacji, kliknij ikonę Copilota na pasku statusu i zaloguj się na swoje konto GitHub z dostępem do Copilota.
### Koszty subskrypcji

GitHub Copilot oferuje różne [plany subskrypcji](https://github.com/features/copilot/plans) dla osób indywidualnych oraz organizacji:
- **Dla osób indywidualnych:**
    - **Copilot Pro:**
        - 10 USD miesięcznie lub 100 USD rocznie.
    - **Copilot Pro+:**
        - 39 USD miesięcznie lub 390 USD rocznie.
        - Plan ten oferuje dodatkowe "premium requests" (zapytania premium), które są naliczane po przekroczeniu limitu w cenie 0.04 USD za zapytanie.
        - dostęp do code review, coding agent - automatycznego rozwiązywania zadań
- **Dla organizacji (Copilot Business):**
    - 19 USD za użytkownika miesięcznie.
Dostępna jest również 30-dniowa wersja próbna dla Copilot Pro (Copilot Pro+ nie oferuje wersji próbnej).

### Tryby działania Copilota

GitHub Copilot oferuje różne tryby interakcji, które wspierają programistów w zależności od potrzeb:
1. **Tryb czatu (Chat Mode):**
    - Jest to klasyczny interfejs czatu, który działa w środowisku IDE lub na stronie GitHub.
    - Możesz prosić Copilota o sugestie kodu, wyjaśnianie fragmentów kodu, generowanie testów jednostkowych, sugerowanie poprawek kodu, a także zadawać pytania dotyczące składni, koncepcji programowania i debugowania.
    - Dostępny jest zarówno w osobnym oknie czatu, jak i w trybie "inline chat" (czat w linii), gdzie odpowiedzi pojawiają się bezpośrednio w edytorze kodu.
2. **Tryb edycji (Edit Mode / Copilot Edits):**
    - Łączy konwersacyjny przepływ czatu z możliwością podglądu i akceptowania zmian bezpośrednio w edytorze.
    - Możesz określić, w których plikach Copilot może wprowadzać zmiany i przeglądać proponowane zmiany w widoku różnic (diff view).
    - Kiedy będziemy mieli trochę wprawy to jest to tryb pracy z którego będziemy korzystać najczęściej
3. **Tryb agentowy (Agentic Mode / Agent Mode):**
    - Jest to bardziej autonomiczny tryb, w którym Copilot działa jako "współpracownik AI", wykonując wieloetapowe zadania kodowania na podstawie ogólnego polecenia w języku naturalnym.
    - Agent może analizować bazę kodu, czytać odpowiednie pliki, proponować edycje plików, uruchamiać polecenia terminala i testy.
    - Monitoruje wyniki (np. błędy kompilacji, niepowodzenia testów) i iteruje, dopóki cel nie zostanie osiągnięty lub nie będzie wymagał dodatkowych danych wejściowych.

## Dedykowane IDE

Posiadamy wiele IDE, które jest dedykowane do pracy z AI. Przykładowe takie aplikacje to - Zed, Cursor, Windsurf. Cursor jest z nich najpopularniejszy.
**Cursor** to środowisko programistyczne (IDE) stworzone z myślą o programowaniu wspomaganym sztuczną inteligencją. 

Jest to Visual Studio tylko budowany od zera z myślą o wykorzystaniu AI.
### Cechy charakterystyczne i zasady działania

Cursor opiera się na kilku kluczowych zasadach, które wyróżniają go spośród innych IDE:
- **Wbudowana AI:** Nie musisz instalować dodatkowych wtyczek do AI – jest ona integralną częścią Cursora. To sprawia, że praca z nią jest płynniejsza i bardziej zintegrowana.
- **Promptowanie w edytorze:** Zamiast przełączać się do osobnego czatu, możesz wydawać polecenia AI bezpośrednio w swoim kodzie, zaznaczając fragmenty lub pisząc komentarze.
- **Wielka kontrola:** Chociaż AI jest pomocna, to ty zawsze masz pełną kontrolę nad generowanym kodem. Możesz akceptować, modyfikować lub odrzucać sugestie.
- **Bazowe zasady**: Możemy wprowadzić nadrzędne zasady, który AI będzie się zawsze kierować podczas generowania kodu.
- **Obsługa wielu języków:** Cursor wspiera wiele popularnych języków programowania, co czyni go uniwersalnym narzędziem.

## MCP

[MCP](https://modelcontextprotocol.io/introduction) czyli Model Context Protocol jest sposobem na połączenie naszego asystenta w bardzo prosty sposób z zewnętrznymi zasobami. Daje nam to możliwość, gdzie pytając czat np. o informacje na confulence, będzie On je po prostu w stanie znaleźć.

Wykorzystanie tego protokołu różni się w zależności od czatu do którego się integrujemy oraz aplikacji, którą chcemy dodać. Przykładowe zastowoania:
- [Claude i Notion](https://youtu.be/NMmDnYKD0fE?si=rp-K4wybOmfQLTQa) (teraz można to zrobić 1 kliknięciem)
- [Claude i Obsidian ](https://youtu.be/VeTnndXyJQI?si=AhX0PXMB79NZGndc)


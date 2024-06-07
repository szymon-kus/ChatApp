a# Dokumentacja

## Opis projektu
ChatApp to aplikacja czatu w czasie rzeczywistym wykorzystująca ASP.NET Core oraz Model-View-Controller (MVC). Aplikacja pozwala użytkownikom na komunikację zarówno w czacie grupowym, jak i poprzez prywatne wiadomości. Dzięki wykorzystaniu SignalR aplikacja zapewnia komunikację między użytkownikami.

## Wykorzystywane technologie
ASP.NET Core
MVC (Model-View-Controller
SignalR
## Jak działa aplikacja?
Model (Model): reprezentuje dane aplikacji oraz logikę
Widok (View): odpowiada za renderowanie interfejsu użytkownika.
Kontroler (Controller): obsługuje żądania użytkownika, manipuluje modelem i zwraca odpowiedni widok.
## Struktura projektu
1. Controllers
Plik HomeController obsługuje żądania HTTP, manipuluje modelem i zwraca odpowiednie widoki. To główny kontroler aplikacji, który obsługuje żądania związane z operacjami czatu oraz stroną główną.
2. Hubs
Pliki hubów obsługują komunikację w czasie rzeczywistym przy użyciu SignalR.
ChatHub.cs: hub, który zarządza połączeniami SignalR oraz obsługą wiadomości w czasie rzeczywistym.
3. messages
Folder przechowujący pliki JSON zawierające wiadomości czatu oraz dane użytkowników.
Group.json: przechowuje wiadomości grupowe.
messages.json: przechowuje wszystkie wiadomości.
(username).json: pliki JSON przechowujące wiadomości poszczególnych użytkowników.
4. Models
Pliki modeli reprezentują dane oraz logikę aplikacji.
ErrorViewModel.cs: model błędów widoku.
Login.cs: model do obsługi logowania użytkownika.
Message.cs: model wiadomości czatu.
Register.cs: model do rejestracji użytkownika.
User.cs: model użytkownika.
5. Password Validation
Pliki związane z walidacją haseł.
PasswordValidation.cs: klasa obsługująca walidację haseł.
6. users
Folder przechowujący dane użytkowników w formacie JSON.
7. Views
Pliki odpowiedzialne za renderowanie interfejsów graficznych.
8. appsettings.json
Plik konfiguracji aplikacji, zawierający ustawienia aplikacji oraz konfigurację połączeń.
9. Program.cs
Główny plik startowy aplikacji, odpowiedzialny za konfigurację oraz uruchomienie aplikacji.
## Pliki kluczowe
HomeController.cs: zarządza operacjami związanymi z czatem i stroną główną.
ChatHub.cs: obsługuje połączenia SignalR oraz komunikację w czasie rzeczywistym.
Message.cs: definiuje strukturę wiadomości czatu.
Chat.cshtml: widok renderujący interfejs użytkownika do komunikacji w czacie.
chat.js: skrypt JavaScript obsługujący ładowanie, wysyłanie i odbieranie wiadomości w czasie rzeczywistym.

## Krótka instrukcja
Aplikacja umożliwia stworzenie użytkownika oraz zalogowanie się na konto. Następnie możliwa jest konwersacja z innymi użytkownikami na czacie grupowym lub prywatnym. Dane zapisują się w plikach .json. W celu przeprowadzenia czatu prywatnego, należy zalogować się na dwóch kontach, a następnie na nich wybrać odpowiedni czat (tzn. user1 musi wybrać czat z user2 i analogicznie - user2 wybrać czat z user1). 

## Problemy
Mieliśmy problem z wyświetlaniem wszystkich aktualnie zalogowanych użytkowników oraz korzystaniem z czatu prywatnego, gdy drugi użzytkownik jest offline. Czasem występuje również problem z zapisem wiadomości (w naszym przypadku jest to zależne od systemy, ponieważ na macOs działa wszystko dobrze, natomiast na jednej z naszych maszyn na Windowsie występuje sporadyczny problem z zapisem).

## dodatkowo 

Postawiliśmy również aplikację na serwerze Azure : https://webappchat01.azurewebsites.net/

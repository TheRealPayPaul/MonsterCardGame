@echo off

REM --------------------------------------------------
REM Monster Trading Cards Game
REM --------------------------------------------------
title Monster Trading Cards Game
echo CURL Testing for Monster Trading Cards Game
echo.

REM --------------------------------------------------
echo 1) Create Users (Registration)
REM Create User
curl -X POST http://localhost:8080/auth/register --header "Content-Type: application/json" -d "{\"Username\":\"kienboec\", \"Password\":\"daniel\"}"
echo.
curl -X POST http://localhost:8080/auth/register --header "Content-Type: application/json" -d "{\"Username\":\"altenhof\", \"Password\":\"markus\"}"
echo.

echo should fail:
curl -X POST http://localhost:8080/auth/register --header "Content-Type: application/json" -d "{\"Username\":\"kienboec\", \"Password\":\"daniel\"}"
echo.
curl -X POST http://localhost:8080/auth/register --header "Content-Type: application/json" -d "{\"Username\":\"kienboec\", \"Password\":\"different\"}"
echo. 
echo.

REM --------------------------------------------------
echo 2) Login Users
set KIENBOEC_LOGIN=curl -s -X POST http://localhost:8080/auth/login --header "Content-Type: application/json" -d "{\"Username\":\"kienboec\", \"Password\":\"daniel\"}"
for /f "delims=" %%i in ('%KIENBOEC_LOGIN%') do set KIENBOEC_TOKEN_RAW=%%i
set KIENBOEC_TOKEN=%KIENBOEC_TOKEN_RAW:"=%
echo %KIENBOEC_TOKEN_RAW%
echo.
set ALTENHOF_LOGIN=curl -s -X POST http://localhost:8080/auth/login --header "Content-Type: application/json" -d "{\"Username\":\"altenhof\", \"Password\":\"markus\"}"
for /f "delims=" %%i in ('%ALTENHOF_LOGIN%') do set ALTENHOF_TOKEN_RAW=%%i
set ALTENHOF_TOKEN=%ALTENHOF_TOKEN_RAW:"=%
echo %ALTENHOF_TOKEN_RAW%
echo.

echo should fail:
curl -X POST http://localhost:8080/auth/login --header "Content-Type: application/json" -d "{\"Username\":\"kienboec\", \"Password\":\"different\"}"
echo.
echo.

REM --------------------------------------------------
echo 3) acquire packages kienboec
curl -X POST http://localhost:8080/transactions/packages --header "Content-Type: application/json" --header "Authorization: Bearer %KIENBOEC_TOKEN%" -d ""
echo.
curl -X POST http://localhost:8080/transactions/packages --header "Content-Type: application/json" --header "Authorization: Bearer %KIENBOEC_TOKEN%" -d ""
echo.
curl -X POST http://localhost:8080/transactions/packages --header "Content-Type: application/json" --header "Authorization: Bearer %KIENBOEC_TOKEN%" -d ""
echo.
curl -X POST http://localhost:8080/transactions/packages --header "Content-Type: application/json" --header "Authorization: Bearer %KIENBOEC_TOKEN%" -d ""
echo.
echo.
echo should fail (no money):
curl -X POST http://localhost:8080/transactions/packages --header "Content-Type: application/json" --header "Authorization: Bearer %KIENBOEC_TOKEN%" -d ""
echo.
echo.

REM --------------------------------------------------
echo 4) acquire packages altenhof
curl -X POST http://localhost:8080/transactions/packages --header "Content-Type: application/json" --header "Authorization: Bearer %ALTENHOF_TOKEN%" -d ""
echo.
curl -X POST http://localhost:8080/transactions/packages --header "Content-Type: application/json" --header "Authorization: Bearer %ALTENHOF_TOKEN%" -d ""
echo.
curl -X POST http://localhost:8080/transactions/packages --header "Content-Type: application/json" --header "Authorization: Bearer %ALTENHOF_TOKEN%" -d ""
echo.
curl -X POST http://localhost:8080/transactions/packages --header "Content-Type: application/json" --header "Authorization: Bearer %ALTENHOF_TOKEN%" -d ""
echo.
echo.
echo should fail (no money):
curl -X POST http://localhost:8080/transactions/packages --header "Content-Type: application/json" --header "Authorization: Bearer %ALTENHOF_TOKEN%" -d ""
echo.
echo.

REM --------------------------------------------------
echo 6) show all acquired cards kienboec
curl -X GET http://localhost:8080/cards --header "Authorization: Bearer %KIENBOEC_TOKEN%"
echo.
echo.
echo should fail (no token)
curl -X GET http://localhost:8080/cards 
echo.
echo.

REM --------------------------------------------------
echo 7) show all acquired cards altenhof
curl -X GET http://localhost:8080/cards --header "Authorization: Bearer %ALTENHOF_TOKEN%"
echo.
echo.

REM --------------------------------------------------
echo 8) show unconfigured deck
curl -X GET http://localhost:8080/cards/deck --header "Authorization: Bearer %KIENBOEC_TOKEN%"
echo.
curl -X GET http://localhost:8080/cards/deck --header "Authorization: Bearer %ALTENHOF_TOKEN%"
echo.
echo.

REM --------------------------------------------------
echo 9) configure deck
curl -X PUT http://localhost:8080/cards/deck --header "Content-Type: application/json" --header "Authorization: Bearer %KIENBOEC_TOKEN%" -d "[1, 2, 3, 4]"
echo.
curl -X GET http://localhost:8080/cards/deck --header "Authorization: Bearer %KIENBOEC_TOKEN%"
echo.
curl -X PUT http://localhost:8080/cards/deck --header "Content-Type: application/json" --header "Authorization: Bearer %ALTENHOF_TOKEN%" -d "[21, 22, 23, 24]"
echo.
curl -X GET http://localhost:8080/cards/deck --header "Authorization: Bearer %ALTENHOF_TOKEN%"
echo.
echo.
echo should fail and show original from before:
curl -X PUT http://localhost:8080/cards/deck --header "Content-Type: application/json" --header "Authorization: Bearer %ALTENHOF_TOKEN%" -d "[25, 1, 26, 27]"
echo.
curl -X GET http://localhost:8080/cards/deck --header "Authorization: Bearer %ALTENHOF_TOKEN%"
echo.
echo.
echo should fail ... only 3 cards set
curl -X PUT http://localhost:8080/cards/deck --header "Content-Type: application/json" --header "Authorization: Bearer %ALTENHOF_TOKEN%" -d "[28, 29, 30]"
echo.
echo.

REM --------------------------------------------------
echo 10) show configured deck 
curl -X GET http://localhost:8080/cards/deck --header "Authorization: Bearer %KIENBOEC_TOKEN%"
echo.
echo.
curl -X GET http://localhost:8080/cards/deck --header "Authorization: Bearer %ALTENHOF_TOKEN%"
echo.
echo.


REM --------------------------------------------------
echo 11) edit user data
echo.
curl -X GET http://localhost:8080/users/kienboec --header "Authorization: Bearer %KIENBOEC_TOKEN%"
echo.
curl -X GET http://localhost:8080/users/altenhof --header "Authorization: Bearer %ALTENHOF_TOKEN%"
echo.
curl -X PUT http://localhost:8080/users --header "Content-Type: application/json" --header "Authorization: Bearer %KIENBOEC_TOKEN%" -d "{\"Username\": \"Kienboeck\"}"
echo.
curl -X PUT http://localhost:8080/users --header "Content-Type: application/json" --header "Authorization: Bearer %ALTENHOF_TOKEN%" -d "{\"Username\": \"Altenhofer\"}"
echo.
curl -X GET http://localhost:8080/users/Kienboeck --header "Authorization: Bearer %KIENBOEC_TOKEN%"
echo.
curl -X GET http://localhost:8080/users/Altenhofer --header "Authorization: Bearer %ALTENHOF_TOKEN%"
echo.
echo.
echo should fail:
curl -X PUT http://localhost:8080/users --header "Content-Type: application/json" --header "Authorization: Bearer %ALTENHOF_TOKEN%" -d "{\"Username\": \"Kienboeck\"}"
echo.
curl -X PUT http://localhost:8080/users --header "Content-Type: application/json" --header "Authorization: Bearer %KIENBOEC_TOKEN%" -d "{\"Username\": \"Altenhofer\"}"
echo.
curl -X GET http://localhost:8080/users/Dummy --header "Authorization: Bearer %ALTENHOF_TOKEN%"
echo.
echo.

REM --------------------------------------------------
echo 12) stats
curl -X GET http://localhost:8080/users/stats --header "Authorization: Bearer %KIENBOEC_TOKEN%"
echo.
curl -X GET http://localhost:8080/users/stats --header "Authorization: Bearer %ALTENHOF_TOKEN%"
echo.
echo.

REM --------------------------------------------------
echo 13) scoreboard
curl -X GET http://localhost:8080/game/scoreboard --header "Authorization: Bearer %KIENBOEC_TOKEN%"
echo.
echo.

REM --------------------------------------------------
echo 14) battle
start /b "kienboec battle" curl -X POST http://localhost:8080/game/battles --header "Authorization: Bearer %KIENBOEC_TOKEN%"
start /b "altenhof battle" curl -X POST http://localhost:8080/game/battles --header "Authorization: Bearer %ALTENHOF_TOKEN%"
ping localhost -n 10 >NUL 2>NUL

REM --------------------------------------------------
echo 15) stats
curl -X GET http://localhost:8080/users/stats --header "Authorization: Bearer %KIENBOEC_TOKEN%"
echo.
curl -X GET http://localhost:8080/users/stats --header "Authorization: Bearer %ALTENHOF_TOKEN%"
echo.
echo.

REM --------------------------------------------------
echo 16) scoreboard
curl -X GET http://localhost:8080/game/scoreboard --header "Authorization: Bearer %KIENBOEC_TOKEN%"
echo.
echo.

REM --------------------------------------------------
echo 17) trade
echo check trading deals
curl -X GET http://localhost:8080/tradings --header "Authorization: Bearer %KIENBOEC_TOKEN%"
echo.
echo create trading deal
curl -X POST http://localhost:8080/tradings --header "Content-Type: application/json" --header "Authorization: Bearer %KIENBOEC_TOKEN%" -d "{ \"CardToTradeId\": 20, \"WantedCardType\": \"Monster\", \"WantedMinDamage\": 15}"
echo.
echo check trading deals
curl -X GET http://localhost:8080/tradings --header "Authorization: Bearer %KIENBOEC_TOKEN%"
echo.
curl -X GET http://localhost:8080/tradings --header "Authorization: Bearer %ALTENHOF_TOKEN%"
echo.
echo delete trading deals
curl -X DELETE http://localhost:8080/tradings/1 --header "Authorization: Bearer %KIENBOEC_TOKEN%"
echo.
echo.

REM --------------------------------------------------
echo 18) check trading deals
curl -X GET http://localhost:8080/tradings  --header "Authorization: Bearer %KIENBOEC_TOKEN%"
echo.
curl -X POST http://localhost:8080/tradings --header "Content-Type: application/json" --header "Authorization: Bearer %KIENBOEC_TOKEN%" -d "{ \"CardToTradeId\": 20, \"WantedCardType\": \"Monster\", \"WantedMinDamage\": 15}"
echo check trading deals
curl -X GET http://localhost:8080/tradings  --header "Authorization: Bearer %KIENBOEC_TOKEN%"
echo.
curl -X GET http://localhost:8080/tradings  --header "Authorization: Bearer %ALTENHOF_TOKEN%"
echo.
echo try to trade with yourself (should fail)
curl -X POST http://localhost:8080/tradings/2 --header "Content-Type: application/json" --header "Authorization: Bearer %KIENBOEC_TOKEN%" -d "19"
echo.
echo try to trade 
echo.
curl -X POST http://localhost:8080/tradings/2 --header "Content-Type: application/json" --header "Authorization: Bearer %ALTENHOF_TOKEN%" -d "40"
echo.
curl -X GET http://localhost:8080/tradings --header "Authorization: Bearer %KIENBOEC_TOKEN%"
echo.
curl -X GET http://localhost:8080/tradings --header "Authorization: Bearer %ALTENHOF_TOKEN%"
echo.

REM --------------------------------------------------
echo end...

REM this is approx a sleep 
ping localhost -n 100 >NUL 2>NUL
@echo on
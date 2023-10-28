# Обмен валют
REST API для описания валют и обменных курсов. Позволяет просматривать и редактировать списки валют и обменных курсов, и совершать расчёт конвертации произвольных сумм из одной валюты в другую.

В проекте используется:
- MsSql
- EfCore
- Rest Api
- .Net Core 6

Техническое задание проекта: https://zhukovsd.github.io/java-backend-learning-course/Projects/CurrencyExchange/

# Реализованные запросы:

- GET-запросы
  - /currencies – получение списка валют
  - /currency/EUR – получение конкретной валюты
  - /exchangeRates – получение списка всех обменных курсов
  - /exchangeRate/USDRUB – Получение конкретного обменного курса
  - /exchange?from=USD&to=AUD&amount=10 – перевод определённого количества средств из одной валюты в другую

- POST-запросы
  - добавление новой валюты в базу
    - /currencies?name=US Dollar&code=USD&sign=$
  - добавление нового обменного курса в базу
    - /exchangeRates?baseCurrencyCode=USD&targetCurrencyCode=RUB&rate=77

- PATCH-запрос
  - /exchangeRate/USDRUB?rate=70 – обновление существующего в базе обменного курса

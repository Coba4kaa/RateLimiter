# Описание проекта
Этот проект состоит из серии лабораторных заданий, направленных на создание набора микросервисов для управления пользователями, реализации ограничения по количеству запросов (rate-limiting), обработки событий и кеширования. Основные сервисы реализованы с использованием gRPC для коммуникации, PostgreSQL и MongoDB для хранения данных, Redis для кеширования и Kafka для событийного стриминга.

## Функции

### UserService
- Управление данными пользователей (CRUD операции).
- Хранение информации о пользователях в базе данных PostgreSQL.
- Экспонирование конечных точек для создания, получения, обновления и удаления пользователей.
- Проверка уникальности логина при создании пользователя.

### Writer Service
- Реализация ограничения по количеству запросов в минуту (RPM) для конечных точек UserService.
- Ограничение запросов по маршруту и эндпоинту.
- Хранение правил ограничения в базе данных MongoDB.

### Reader Service
- Загрузка правил ограничения из MongoDB в память для быстрого доступа.
- Использование change streams MongoDB для отслеживания изменений в ограничениях.
- Обработка проверок лимита в памяти с учетом многозадачности и потокобезопасности.

### Интеграция с Kafka
- Отправка запросов пользователей в Kafka для событийного стриминга.
- Динамическая настройка генерации событий от разных пользователей (например, разные RPM).
- Поддержка отправки различных типов запросов от разных пользователей с изменяемыми RPM.

### Кеширование в Redis
- Реализация кеширования данных о пользователях и проверок ограничения в Redis.
- Запрещает пользователям выполнять запросы, если превышен лимит для конкретного эндпоинта.

## Технологии
- **gRPC** для взаимодействия между сервисами.
- **PostgreSQL** для хранения данных пользователей.
- **MongoDB** для хранения правил ограничения по количеству запросов.
- **Redis** для кеширования и отслеживания лимитов.
- **Kafka** для событийного стриминга и отслеживания запросов.
- **Dapper** для взаимодействия с базой данных PostgreSQL.
- **FluentValidation** для валидации входных данных.
- **Confluent.Kafka** для интеграции с Kafka.

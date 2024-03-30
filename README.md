### EuroMotors

EuroMotors - це веб-додаток магазину автозапчастин, розроблений для зручності клієнтів та адміністраторів. Додаток надає можливість перегляду, додавання, редагування та видалення товарів, категорій та замовлень.

### Для клієнтів:

1. **Перегляд товарів**:
   - Клієнти можуть переглядати доступні товари за категоріями, моделями автомобілів тощо.
   - Кожен товар має сторінку з докладним описом, зображеннями та характеристиками.

2. **Додавання товарів в корзину**:
   - Клієнти можуть додавати товари до своєї корзини для подальшого оформлення замовлення.

3. **Оформлення замовлення**:
   - Після додавання товарів в корзину клієнти можуть переглянути її та оформити замовлення.
   - Вони можуть вказати адресу доставки та інші деталі замовлення.

4. **Відстеження статусу замовлення**:
   - Після оформлення замовлення клієнти можуть відстежувати його статус, наприклад, "В обробці", "Відправлено" тощо.

### Для адміністраторів:

1. **Управління товарами**:
   - Адміністратори мають можливість додавати нові товари, редагувати та видаляти існуючі.
   - Вони можуть встановлювати ціни, додавати зображення та інші деталі товару.

2. **Управління категоріями**:
   - Адміністратори можуть створювати нові категорії товарів, редагувати та видаляти існуючі.

3. **Управління замовленнями**:
   - Адміністратори мають доступ до списку замовлень та можуть переглядати їх деталі.
   - Вони можуть оновлювати статуси замовлень, наприклад, позначати їх як "В обробці", "Відправлено" тощо.

4. **Авторизація та автентифікація**:
   - Відокремлені облікові записи адміністраторів дозволяють керувати доступом до адміністративних функцій лише авторизованим користувачам.

### Використані технології

- ASP.NET Core MVC - фреймворк для побудови веб-додатків на мові програмування C#.
- Entity Framework Core - ORM (об'єктно-реляційне відображення) для роботи з базою даних Microsoft SQL Server.
- Identity Framework - для управління аутентифікацією, авторизацією та ролями користувачів.
- Bootstrap - для розробки інтерфейсу та респонсивного дизайну.
- Razor Pages та контролери MVC для організації логіки та представлень.
- HTML, CSS та JavaScript для клієнтського інтерфейсу.
- Nova Poshta API - для отримання інформації про відправлення та доставку замовлень.
- LiqPayApi - для створення та обробки платежу.

### Використані патерни

- MVC (Model-View-Controller): для розділення програми на три основні компоненти, що дозволяє відокремити логіку додатку від представлення та взаємодії з даними.
- Repository Pattern: для розділення логіки доступу до даних від логіки бізнес-логіки та прослойки між ними.
- Unit of Work Pattern: для керування транзакціями та кількома репозиторіями.
- Dependency Injection: для впровадження принципу інверсії керування та полегшення тестування та обслуговування коду.

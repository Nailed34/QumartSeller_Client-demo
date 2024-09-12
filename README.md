### QumartSeller общее<br/>
Qumart Seller - сервис для синхронизации остатков продавца на популярных маректплейсах, таких как Ozon, Wildberries и Yandex Market. Является pet-проектом и выложен с целью демонстрации знаний в среде .Net</br>
На данный момент сервис состоит 2 частей:
- Клиент: написан при помощи WPF C# (.Net 8) с применением паттерна MVVM и библиотекой CommunityToolkit.
- Сервер: ASP.Net C# (.Net 8) с применением SignalR (веб-сокеты) и подключением к базе данных MongoDB.

### Что умеет сервис?<br/>
- Импортировать карточки товаров в базу данных на сервере.
- Объединять карточки с разных маркетплейсов одного товара (с одинаковым артикулом) в отдельное представление, называемое ProductOnion (объединение).
- Синхронизировать остаток у карточек, которые находятся в одном объединении, т.е. при покупке товара, например на Wildberries, сервис автоматически вычтет остаток в других прикрепленных маркеплейсах.
- Настраивать синхронизацию у каждой конкретной карточки.
- Управлять объединенями вручную, в случаях если артикулы карточек различаются, но нужно чтобы они синхронизировались.
- Настраивать кратность покупки у каждой карточки, например если в объединении находится один и тот же товар, но представленный в разном количестве, сервис автоматически вычтет указанное количество в прикрепленных к объединению карточках.
- Отправлять общий остаток на маркетплейсы.

### Что реализовано на данный момент?<br/>
- Авторизация пользователя с применением JWT токена.
- Импорт товаров и остатков с маркетплейса Ozon (применяется официальный Ozon API, работающий через REST).
- Автоматическое создание/прикрепление/удаление объединений при импорте.
- Постраничная навигация по импортированным товарам на клиенте.
- Автоматическое кэширование фотографий на клиенте для их быстрого отображения.

### Что находится в демо версии?<br/>
В демо версии доступно всё, что реализованно на данный момент, за исключением логики импорта товаров/остатков с Ozon с созданием объединений, работающей непосредственно с базой данных.
Для тестирования функционала создан отдельный Демо сервис, генерирующий тестовые данные при запуске сервера, которые он отправляет при запросах клиента.

### Каким образом обрабатываются запросы клиента?<br/>
Общение между клиентом и сервером реализовано преимущественно с применением SignalR, что позволяет поддерживать активное соединение и отображаемые на клиенте данные в актульном состоянии.
Аутентификация пользователя реализована с помощью POST метода с генерацией JWT токена, который используется в авторизации соединения по веб-сокету.

### Какая архитектура сервиса?<br/>
На данный момент сервис имеет монолитную архитектуру, однако основной функционал раздроблен на отдельные несвязные библиотеки, которые легко можно будет развернуть в микросервисы, поэтому, если изолировать логику этих библиотек, текущий сервер
выполняет лишь роль шлюза и шины данных для будущих микросервисов.

### QumartSeller_Client<br/>
Данный репозиторий содержит WPF приложение для использования продавцом. Настройки для подключения к серверу находятся в файле App.config.</br>

### Архитектура приложения<br/>
- Views/ содержит пользовательский интерфейс на xaml, имеет древовидную структуру для отделения страниц, отображаемых в окне.
- ViewModels/ содержит свойства и команды для биндинга во View, а также логику для связи моделей с UI.
- Models/ содержит модели, используемые во ViewModels.
- UIKit/ содержит пользовательские ресурсы, используемые в UI всего приложения.
- Services/ содержит сервисы, используемые ViewModels и Models для выполнения определенных функций, более подробно далее:

### Сервисы приложения и что они делают<br/>
- AppSettingsService - небольшой сервис для удобного чтения настроек приложения, описанных в файле App.config.
- AuthorizationService - сервис для авторизации пользователя на сервере.
- ConnectionService - сервис для управления соединением с сервером с помощью SignalR, содержит HubConnection для запросов в моделях.
- NavigationService - сервис навигации по страницам и связи View с ViewModel. Создает экземпляр страницы и её ViewModel при первой навигации к ней.
- PhotoCacheService - сервис для асинхронного управления кэшем фотографий, имеет авто-сохранение и проверку актуальности ссылок карточек товара.
- UserDataService - сервис для хранения пары логин-пароль в зашифрованном виде в файле UserData.enc, используется для автоматической авторизации при запуске программы.

### Установка клиента<br/>
Удобнее всего скачать последнюю Release версию на странице этого репозитория и запустить ClientWPF.exe файл, либо скомпилировать бинарные файлы с представленных исходников (при разработке использовалась Visual Studio Community 2022).<br/>
Имейте в виду, что для работы с приложением необходим сервер, представленный в следующем репозитории: https://github.com/Nailed34/QumartSeller_Server-demo <br/>
По умолчанию клиент делает запросы по адресу http://localhost:5000, настроить это можно в файле ClientWPF.dll.config в корневом катологе приложения.

### Как авторизоваться в демо версии?<br/>
Для авторизации в демо версии используйте имя пользователя:
```
testlogin
```
и пароль:
```
testpassword
```

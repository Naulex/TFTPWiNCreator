# TFTPWiNCreator

![Banner](https://github.com/Naulex/TFTPWiNCreator/assets/148938265/a257f14d-0987-49e3-b2dc-0459c9b72390)


TFTPWiNCreator - программа, позволяющая автоматизировать процесс создания загрузочных PXE-серверов. Умеет работать с ОС Windows7 - Windows 11 и большинством образов, совместимых с Syslinux. Подготавливает ISO образы Windows для загрузки как через BIOS, так и через UEFI.

![1](https://github.com/Naulex/TFTPWiNCreator/assets/148938265/6208e7c3-e908-417f-b157-b5c2deb348e6)

Пользователю необходимо ввести IP-адрес сервера на котором располагается ISO-образ ОС, указать путь к этому образу и открыть к нему доступ по SMB.

Программа извлечёт из оригинального ISO образа ОС предзагрузочную среду, модифицирует её и добавит изменённый загрузчик, а затем создаст TFTP сервер для подключения. Если всё настроено верно, любое устройство в локальной сети, поддерживающее PXE, сможет подключиться к серверу и начать процесс установки.

![2](https://github.com/Naulex/TFTPWiNCreator/assets/148938265/4b9f8df4-29b9-406a-af54-67f00630ecd1)

Процесс установки на устройстве выглядит следующим образом:

Устройства, поддерживающие загрузчик Windows Boot Manager, загрузят с сервера модифицированные загрузчики:

![3](https://github.com/Naulex/TFTPWiNCreator/assets/148938265/d3f9e5fa-feb0-46a3-8475-7fe45ed0f1d4)

Которые в предзагрузочной среде Windows запустят скрипт, который выполнит подключение к серверу по SMB и начнёт процесс установки:

![4](https://github.com/Naulex/TFTPWiNCreator/assets/148938265/7d40d346-9540-4929-8d08-55556038229a)

Если настройка сервера была выполнена правильно, вмешательство пользователя не потребуется, установка проходит в полностью автоматическом режиме.

![5](https://github.com/Naulex/TFTPWiNCreator/assets/148938265/69d5269a-c2b3-4377-b838-6033c4b679f1)

Для ручного соединения предусмотрен специальный режим работы скрипта.

![6](https://github.com/Naulex/TFTPWiNCreator/assets/148938265/bd3dffa5-1e66-438c-b40e-3fc115268903)

Если речь идёт о загрузчике Syslinux, возможно создание мультизагрузочного PXE-сервера, что может быть удобно при работе с тонкими клиентами или различными ориентированными на сервер решениями. Интерфейс загрузчика Syslinux отличается, кроме того, при загрузке образов, созданных для этого загрузчика, не требуется подключение к серверу по SMB.

![7](https://github.com/Naulex/TFTPWiNCreator/assets/148938265/6aa73d22-82e5-4b84-bb74-66ea759158c4)

Процесс сетевой установки никак не отличается от любого другого процесса установки, не модифицирует никаких системных файлов и не влияет на безопасность установленной системы.

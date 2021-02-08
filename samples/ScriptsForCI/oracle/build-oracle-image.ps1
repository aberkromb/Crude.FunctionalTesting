# Загружаем репозиторий с oracle Dockerfile

git clone https://github.com/oracle/docker-images.git

# Скачайте нужный архив с официальной страницы Oracle
# Потребуется регистрация
# https://www.oracle.com/ru/database/technologies/oracle-database-software-downloads.html
# Этот архив нужно поместить в папку с номером версии в загруженом репозитории
# например в папку ниже, если загрузили oracle 19.3.0
# \docker-images\OracleDatabase\SingleInstance\dockerfiles\19.3.0 


# Запускаем сборку образа

.\docker-images\OracleDatabase\SingleInstance\dockerfiles\buildContainerImage.sh -v 19.3.0 -e

# Запускаем собранный образ
docker run --rm --name my-oracle -p 1521:1521 -p 5500:5500 -e ORACLE_PWD=123 --shm-size="1g" oracle/database:19.3.0-xe

# Образ будет стартовать очень долго
# Чтобы минимизировать время запуска надо сохранить контейнер как образ  

# Сначала берем идентификатор контейнера 
docker ps -f "name=my-oracle"

# Сохраняем новый "быстрый" образ
docker commit --message "container fortests" <container id> my/oracle-19.3.0-my-oracle   


# еще можно улучшить собранный образ воспользовавшись статьей - https://habr.com/ru/post/480106/

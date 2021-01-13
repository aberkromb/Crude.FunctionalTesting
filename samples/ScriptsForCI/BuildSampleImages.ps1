# Пример как можно собрать образы для использовать на CI.
# Пример для windows

$base_image_name = "ft-net5-docker:1.0" 
$test_image_name = "ft-test:1.0" 

cls
echo "Получаем текущий docker host."
$pingResult = Test-Connection -Count 1 host.docker.internal 
$dockerHost = $pingResult.IPV4Address.IpAddressToString
echo "Docker host получен : " $dockerHost

echo "-----------------------------------"
echo "Собираем базовый образ с net 5 alpine и docker."
docker build --add-host=docker:$dockerHost `
             -t $base_image_name `
             -f samples\ScriptsForCI\Dockerfile-net5-docker .
echo "Базовый образ собран."

echo "-----------------------------------"
echo "Собираем образ для тестов."
docker build --build-arg base-image-name=$base_image_name `
             --add-host=docker:$dockerHost `
             -t $test_image_name `
             -f samples\ScriptsForCI\Dockerfile-test .
echo "Образ для тестов собран."

echo "-----------------------------------"
echo "Запускаем образ для тестирования."
docker run -e DOCKER_CUSTOM_HOST_IP=$dockerHost `
           -e ASPNETCORE_TEST_CONTENTROOT_SANDBOX=/app/sandbox/ `
           -v //var/run/docker.sock:/var/run/docker.sock `
           --add-host=docker:$dockerHost `
           -t $test_image_name
echo "Тесты закончены."

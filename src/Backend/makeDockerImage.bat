docker build -t expedientes-web-backend-img --add-host=nexus-desa.minfin.gob.gt:172.30.1.31 . 
docker tag expedientes-web-backend-img srv-osnexus01.minfin.gob.gt:8006/expedientes-web-backend-img:latest
docker push srv-osnexus01.minfin.gob.gt:8006/expedientes-web-backend-img:latest
c:
cd /oc
oc project expedientes-web
oc import-image expedientes-web-backend-img --insecure

call npm run build:qa
copy Dockerfile dist/Dockerfile
copy nginx.conf "dist/nginx.conf"
cd dist
docker build -t expedientes-web-frontend-img .
docker tag expedientes-web-frontend-img srv-osnexus01.minfin.gob.gt:8006/expedientes-web-frontend-img:latest
docker push srv-osnexus01.minfin.gob.gt:8006/expedientes-web-frontend-img:latest
c:
cd /oc
oc project expedientes-web
oc import-image expedientes-web-frontend-img --insecure

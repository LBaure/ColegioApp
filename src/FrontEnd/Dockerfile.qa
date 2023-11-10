FROM nginx
#LABEL io.openshift.expose-services="8080:http"
COPY . /usr/share/nginx/html
COPY nginx.conf /etc/nginx/nginx.conf
RUN chgrp -R 0 /var/cache/nginx /var/run /var/log/nginx && chmod g=u /var/cache/nginx /var/run /var/log/nginx

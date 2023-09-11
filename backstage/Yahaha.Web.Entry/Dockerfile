FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base

WORKDIR /app
EXPOSE 5001

COPY . .
ENV TZ=Asia/Shanghai
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone
RUN sed -i 's/MinProtocol = TLSv1.2/MinProtocol = TLSv1/g' /etc/ssl/openssl.cnf
RUN sed -i 's/MinProtocol = TLSv1.2/MinProtocol = TLSv1/g' /usr/lib/ssl/openssl.cnf
RUN sed -i 's/CipherString = DEFAULT@SECLEVEL=2/CipherString = DEFAULT@SECLEVEL=1/g' /etc/ssl/openssl.cnf
RUN sed -i 's/CipherString = DEFAULT@SECLEVEL=2/CipherString = DEFAULT@SECLEVEL=1/g' /usr/lib/ssl/openssl.cnf


ENTRYPOINT ["dotnet", "Yahaha.Web.Entry.dll"]

# 构建镜像
# docker build -t yahaha .
# 导出镜像
# docker save -o c:\dockerimg\yahaha.tar yahaha
# =>复制到服务器
# 导入镜像
# sudo docker load<yahaha.tar
# 暂停容器
# sudo docker stop yahaha
# 删除容器
# sudo docker rm yahaha
# 启动容器
# sudo docker run --name yahaha -p 5001:5001 --restart=always -d yahaha
# 其他
# 查看日志
# sudo docker logs yahaha
# 查看容器
# sudo docker ps
# 查看镜像
# sudo docker images
# 删除镜像
# sudo docker rmi <IMAGE ID>
# 进入容器
# sduo docker exec -it ae356c5ece1f /bin/bash
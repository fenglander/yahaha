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

# ��������
# docker build -t yahaha .
# ��������
# docker save -o c:\dockerimg\yahaha.tar yahaha
# =>���Ƶ�������
# ���뾵��
# sudo docker load<yahaha.tar
# ��ͣ����
# sudo docker stop yahaha
# ɾ������
# sudo docker rm yahaha
# ��������
# sudo docker run --name yahaha -p 5001:5001 --restart=always -d yahaha
# ����
# �鿴��־
# sudo docker logs yahaha
# �鿴����
# sudo docker ps
# �鿴����
# sudo docker images
# ɾ������
# sudo docker rmi <IMAGE ID>
# ��������
# sduo docker exec -it ae356c5ece1f /bin/bash
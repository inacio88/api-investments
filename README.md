# Setup

Banco:
docker run --name api-investments-postgres -e POSTGRES_PASSWORD=1q2w3e4r@@@ -d -p 5436:5432 postgres

Stringconnection pelo user secrets ( mas também pode ser variável de ambiente normal)
dotnet user-secrets set conexao "User ID=postgres;Password=1q2w3e4r@@@;Host=localhost;Port=5436;Database=ClienteDB;Pooling=true;"
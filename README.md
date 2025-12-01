# Setup

- Necesário: .net 8
- Necessário: docker ou postgres, coloquei aqui numa porta diferente o mapeamento caso já tenha outro rodando na padrão

Banco:
docker run --name api-investments-postgres -e POSTGRES_PASSWORD=1q2w3e4r@@@ -d -p 5436:5432 postgres


- Stringconnection pelo user secrets ( mas também pode ser variável de ambiente normal)
dotnet user-secrets set conexao "User ID=postgres;Password=1q2w3e4r@@@;Host=localhost;Port=5436;Database=ClienteDB;Pooling=true;"

# Libs
- Identity: Para autenticação e autorização, usei pois agiliza bastante a autenticação e autorização
- entityFrameWorkCore.Design: para gerar as migrações, ferramente muito boa de code em code first
- Swashbuckle.AspNetCore.Annotations: para fazer a documentação da api pelo swagger.
- EntityFrameworkCore.PostgreSQL: Oferece orm para lidar com banco postgres, também uma execelente lib com boa performance atualmente. Gostaria de ter inserido também o dapper para operações que exigissem um controle mais perfomátíco, mas pelo escopo acredito que o desempenho do entity é suficiente.
- Moq: Me auxiliou no mock do repositório para testes unitários e focar apenas nos serços e lógicas de negócio.
  
# Run
- Todos os projetos estão .net8, então basta ter .net 8 sdk instalado e rodar "dotnet run" ele vai restaurar pacotes, compilar e rodar.

# Obs
- Deixei as migrações sendo aplicadas no startup da aplicação, mas apenas facilitar o trabalho do avaliador; geralmente coloco isso no pipeline de publicação de uma realease e fica separado num passo de ci/cd.
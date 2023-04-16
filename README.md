# Usuários

API para cadastro e autenticação de usuários.  

Lida com gerenciamento de sessões por meio de *access token* e *refresh token*.  
Faz rotação de *refresh tokens*.

## Sobre a estrutura da solução

O código é organizado em duas pastas principais:
- `src`: Com o código de produção; e
- `test`: Com o código de teste (testes e utilitário de testes)

Os testes são separados em dois projetos conforme o seu tipo:
- Um para testes de unidade; e
- Um para testes de integração

São considerados testes de unidade os testes que:
- Executam rápido; e
- Rodam de forma isolada de outros testes (um teste não interfere em outro); e
- Testam uma única unidade de **comportamento**; e
- Não interagem com recursos externos (Ex.: Banco de dados).

São considerados testes de integração todos os testes que não são testes de unidade:
- No contexto dessa solução essa definição pode ser resumida em: Testes que interagem com processos externos (ex.: banco de dados)

Os projetos que compõe o código de produção são separados em camadas levemente baseadas nas recomendações do *DDD* e do *Clean Architecture*. 
Cada camada é dividida em um conjunto de projetos. A separação dos projetos foi pensada para quebrar referências indiretas entre
projetos que não devem se referenciar por pelo menos um dos motivos:
- Manter a separação arquitetural entre as camadas;
- Possibilitar otimização na criação das imagens docker.

As camadas estão organizadas da seguinte forma:
- `Startup`: A camada "main". É onde todas as configuração são definidas; Onde a pipeline HTTP é configurada; Onde o container de dependências é criado; etc.  
  Os projetos que compõem essa camada são:
  - `Startup`: É, de fato, o ponto de entrada da aplicação;
  - `Startup.Migrations`: Foi separado do `Startup` principal devido ao uso de dependências específicas para migração de banco de dados
     e também para facilitar otimização na geração das imagens docker (a migração do banco possui um imagem específica).
- `Api`: Contém a implementação dos endpoints acessíveis via HTTP.  
   Nessa solução a camada é composta apenas pelo projeto `Api.Rest`, que define os endpoints rest da api.
- `Application`: A camada com os serviços de aplicação. A separação dos projetos dessa camada foi feita principalmente
   para evitar que as camadas clientes tenham acesso a classes de camadas mais baixas por meio de referência indireta.
   Ex.: Os projetos de API não devem ter acesso aos serviços da camada de Domínio mas a implementação dos serviços de aplicação
   precisam desse acesso. Com a separação que foi feta apenas o projeto `App.Handlers` precisa referenciar o domínio
   deixando o `App.Services` seguro para ser referenciado pelos projetos de API.  
   Os projetos que compõem essa camada são:
   - `App.Services`: Define a interface publica da camada para o consumo por outras camadas;
   - `App.Handlers`: Contém as implementações dos serviços da camada.
- `Domain`: A camada de domínio da solução. É a camada "central" ou a "mais baixa", com exceção da camada de infra,
   portanto não referencia nenhum projeto que não seja de infra para a própria camada de domínio.  
   Composta apenas pelo projeto `Domain`.
- `Infra`: É aqui que estão, por exemplo, as implementações dos serviços declarados em outras camadas mas que, por depender
   de processos externos, não podem ser implementados na camada que as interfaces públicas. Também pode implementar serviços
   próprios para consumo por outras camadas.
   Essa camada é composta pelos projetos:
   - `Infra.Domain`: Serviços de infra exclusivos para a camada de domínio;
   - `Infra.Repositories`: Implementação dos serviços que fornecem interação com o banco de dados;
   - `Infra.EntityFramework`: Classes para integração com o EF Core. Separado do projeto de repositórios para que possa ser
     referenciado no projeto de *migrations* sem depender também dos repositórios.

## Sobre o uso do docker

- O sistema foi projetado para ser publicado em containers.
- [docker](https://www.docker.com/) é utilizado para a criação das imagens. A estrutura do projeto e o workflow levam isso em consideração.
- São geradas 3 imagens:
  - `runtime`: Como runtime da aplicação.
  - `testrunner`: Para a execução dos testes.  
     Executar os testes dentro de uma imagem docker gerada com as mesmas bases da imagem de runtime melhora a qualidade
     da validação feita pelas testes pelo fato de eles serem executados em um ambiente muito mais próximo do encontrado em produção.  
     Essa abordagem tem a desvantagem de retardar o feedback fornecido pelos testes já que é preciso esperar, no mínimo, o tempo de
     build da imagem de testes.
  - `migrationrunner`: Para a execução da migração do banco de dados.  
    Ter uma imagem de migração conectada com a de runtime pela número da sua versão facilita uma possível implatanção de uma
    versão específica em um novo ambiente.
- [kubernetes](https://kubernetes.io/) é a forma primária para o gerenciamento dos containers.
- O sistema foi projetado para obter configurações do ambiente.

## Sobre CI/CD

Esse repositório conta com workflows para:
- Validação do código em [pull requests](./.github/workflows/ci-pull-request.yaml).
- Validação do código e deploy automático em ambiente de desenvolvimento quando [o merge é feito para o branch master](./.github/workflows/ci-master.yaml).

## Recursos de destaque

- Cadastro de usuários
- Autenticação

### Sobre o fluxo de autenticação

***Obs.**: O tratamento feito sobre os dois tokens utilizados no processo de autenticação segue o princípio de rotação de refresh tokens*.

- A partir das credenciais do usuário, a API inicia uma sessão.
- Como resposta da criação da sessão, a API retorna dois tokens:
  - *`access token`*: O token utilizado para autenticação com as APIs dos outros serviços do sistema.  
    Esse token é retornado no corpo da resposta para que fique acessível pelo cliente, que deve então armazenar
    o token no local mais seguro viável no seu contexto (provavelmente em memória);
  - *`refresh token`*: Identifica a sessão. Usado para geração de novos tokens de acesso.
    É retornado na forma de um cookie seguro para que o cliente não tenha acesso direto a ele.  
    O único componente do sistema que deve ser capaz de acessar o *refresh token* é essa API, através de cookies.  
    O *refresh token* é "rotacionado" como forma de contramedida ao roubo de *refresh tokens*.
- É esperado que o *access token* tenha vida curta.
- Sempre que um novo *access token* for necessário, o *refresh token* da sessão deve ser apresentado ao endpoint
  adequado para a atualização a sessão.
- A atualização da sessão resulta em:
  - Criação de um novo *access token*;
  - Criação de um novo *refresh token* (o antigo é marcado como "consumido" como parte da rotação do token).

Como funciona a rotação de *refresh tokens*:
- A API mantém registro de todos os *refresh tokens* gerados para uma determinada sessão.
- Apenas o *refresh token* mais recente é aceito para a atualização da respectiva sessão.  
- Caso um *refresh token* já consumido seja utilizado novamente, a sessão inteira é invalidada. O que significa que:
  - A solicitação de atualização da sessão é rejeitada;
  - O *refresh token* atualmente válido é invalidado.
    Qualquer *access tokens* previamente gerado para a sessão continua sendo válido até sua expiração.
    Mas no momento em que o cliente utilizando esse token precisar da renovação da sessão ele vai ser barrado
    já que a sessão foi invalidada durante a tentativa de atualização da sessão com um *refresh token* inválido.

## Desenvolvimento

**Importante**: Para a instalação dos pacotes nuget é necessário um acesso ao repositório de pacotes do projeto.
Esse repositório é, no momento, hospedado de forma privada no github. Por isso não é possível instalar os pacotes e,
por consequência, executar a API localmente, sem as devidas permissões.

### Dependências

- [.NET 7.0](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)
- [PostgreSQL 14.5](https://www.postgresql.org/download/)
- [dotnet-ef](https://learn.microsoft.com/en-us/ef/core/cli/dotnet)

### Instalar os pacotes

Configurar o *source* nuget:
```sh
dotnet nuget add source --username EMAIL --password GITHUB_TOKEN --store-password-in-clear-text --name github "https://nuget.pkg.github.com/sestio/index.json"
```

Instalar os pacotes:
```sh
dotnet restore
```

### Variáveis de ambiente

***Obs.:** Os arquivos de configuração citados existem (ou devem ser criados) no projeto `Sestio.Usuarios.Startup`.*

- As variáveis necessárias podem sem consultadas no arquivo [appsettings.json](./src/Sestio.Usuarios.Startup/appsettings.json).
  Esse arquivo contém valores padrão para produção ou placeholders para as variáveis que dependem do ambiente.
- O arquivo [appsettings.Development.json](./src/Sestio.Usuarios.Startup/appsettings.Development.json) contém valores padrão para desenvolvimento.
- Um arquivo `appsettings.local.json` deve ser criado para configurar a execução em localhost.
- O placeholder `T_SECRET` significa que esse valor vai ser injetado pelo [vault](https://www.vaultproject.io/) quando a api é executada em algum ambiente que não seja o localhost.

Variáveis que precisam ser configuradas no `appsettings.local.json`:
- `Database.ConnectionString`: String de conexão com um banco de dados postgres. Deve ser um banco de dados específico para essa API.
- `PeerJwt.SigningKey`: String que serve como chave para assinatura dos tokens gerados. Essa chave deve ser a mesma utilizada em todos as APIs que consomem os tokens geradas para autenticação.

### Banco de dados

Esse projeto utiliza o recurso de [*migrations do EF Core*](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=dotnet-core-cli) para configuração do *schema* do banco de dados.  
Os arquivos de migração ficam no projeto `Sestio.Usuarios.Startup.Migrations`.  
Esse repositório inclui um script [scripts/ef.bat](./scripts/ef.bat) que deve ser usado no lugar do `dotnet ef`. Esse script:
- Copia as configurações do projeto de `Startup` para o de *migrations*. É dessa configuração que tiramos o banco de dados para a migração.
- Executa o `dotnet ef` apontando para o projeto de *migrations*.

Os comandos mais importantes são:  
- Criar uma *migration*:
    ```sh
    ./scripts/ef migrations add NomeDaMigration
    ```
- Aplicar todas as *migrations*:
    ```sh
    ./scripts/ef database update
    ```
*Obs.: Para mais formas de aplicar *migrations* consulte a [documentação do dotnet-ef](https://learn.microsoft.com/en-us/ef/core/cli/dotnet).*  

### Execução dos testes

Existem dois projetos de testes na solução:
- Um para testes de unidade
- Um para testes de integração

Os testes de integração precisam de acesso a um banco de dados postgres real.  
As configurações necessárias para o acesso ao banco de dados podem ser consultadas no arquivo [dbsettings.json](./test/Sestio.Usuarios.IntegrationTests/dbsettings.json) na raiz do projeto de testes de integração.  
A infra dos testes lida com a criação e remoção dos bancos de dados usados em cada teste:
- Um novo banco de dados é criado e configurado da mesma forma que um banco de produção para cada teste executado.
- Os bancos criados para testes são removidos ao final da execução do respectivo teste. Note que para isso acontecer o teste precisa ser executado até o final (com sucesso ou falha). Se algum teste for interrompido (ex.: no meio de uma sessão de depuração) o banco de dados criado não vai ser removido automaticamente.
- O nome do banco de dados de cada teste é formado pelo nome da classe que contém o teste seguido de um GUID. O nome da classe serve para fins de depuração.

**IMPORTANTE**: Não altere o arquivo `dbsettings.json` diretamente. Configurações personalizadas devem ser informadas em um arquivo `dbsettings.local.json` (no mesmo projeto).  
**OBS.**: O banco de dados informado na variável `DB_NAME` serve apenas para a conexão inicial e não é usado pelos testes.  

Embora seja possível utilizar qualquer servidor postgres compatível é recomendado configurar um servidor específico para os testes.  
Isso pode ser feito facilmente com docker. O comando a seguir cria um container executando um servidor postgres que pode
ser acessado usando a configuração padrão do projeto de testes (assumindo que o docker e os testes sejam executados na mesma máquina):

```sh
docker container run --detach --name sestio_test_pgsql --env POSTGRES_USER=admin --env POSTGRES_PASSWORD=admin --publish 65001:5432 postgres:14.5-alpine
```
**Dica**: Se você executa o docker em um computador que é reiniciado com frequência, considere as flags a seguir:
- `--restart unless-stopped`: Para iniciar o container sempre que o docker for iniciado.
- `--tmpfs /var/lib/postgresql/data`: Isso faz com que o volume de dados do postgres seja montado em RAM.  
  Montar o volume de dados do banco em memória é útil para remover aqueles bancos de teste que, por algum motivo, não
  puderam ser removidos automaticamente após o teste. Basta parar o container (acontece automaticamente quando o processo do docker é encerrado na reinicialização da máquina hospedeira).  
  **Obs.**: O uso dessa flag pode gerar um consumo grande de memória mas isso é pouco provável nesse cenário já que:
  - Os bancos de teste (pelo menos a maioria deles) são removidos automaticamente após cada teste; e
  - A memória utilizada é liberada sempre que o container é reiniciado, o que acontece sempre que o docker é reiniciado.

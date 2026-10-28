# Mitos e Verdades Nucleares: Console .NET de Usuários

Challenge 2026, Sprint 1 (FIAP, Análise e Desenvolvimento de Sistemas).
Disciplina: Advanced Business Development with .NET.

## Integrantes

| Nome | RM |
|---|---|
| Matheus Peres Moraes de Jesus | RM567300 |
| Davi Isac Colin Pereira | RM567265 |
| Pedro Gonçalves | RM567651 |
| Cassio Ribeiro | RM567295 |
| Hugo Souza | RM568542 |

## Solução do grupo

Aplicativo educativo e gamificado que combate a desinformação sobre energia nuclear, voltado a estudantes e ao público geral:

- **Mito ou Verdade:** o jogador diz se uma afirmação é mito ou verdade e recebe a explicação na hora.
- **Cidade:** simulação de abastecimento de energia de uma cidade.
- **Ranking:** pontuação dos jogadores.
- **Totem IoT:** instalado em escolas e eventos, inicia o jogo e mostra o ranking.

## O que foi desenvolvido nesta Sprint

Uma aplicação **console** em .NET com a entidade **Usuario** (o jogador). Ela complementa a API Java, que cuida das perguntas.

- **Entidade `Usuario`:** nome, e-mail, senha e pontuação, com validações:
  - nome obrigatório, com pelo menos 3 caracteres;
  - e-mail obrigatório e em formato válido;
  - senha com pelo menos 6 caracteres, contendo letra e número;
  - pontuação nunca negativa.
- **Regras de negócio:**
  - o e-mail é único (não diferencia maiúsculas de minúsculas);
  - o login bloqueia o usuário após 3 tentativas de senha erradas, e um login correto zera as tentativas;
  - só é possível adicionar pontos positivos à pontuação.
- **Cadastro** de novos usuários.
- **Consulta:** lista todos os usuários e busca por nome ou e-mail (aceita parte do texto).
- **Tratamento de erros:** o menu usa `try/catch` com exceções personalizadas, então nenhuma entrada inválida derruba a aplicação.
- **Persistência em JSON:** os dados, inclusive o estado de bloqueio, ficam no arquivo `usuarios.json` e são mantidos entre execuções.

## Arquitetura

```
MitosNucleares.slnx
MitosNucleares.Project/
├── Entities/
│   └── Usuario.cs                  Entidade com validações e regras (login, bloqueio, pontos)
├── Exceptions/                     Exceções personalizadas, uma por arquivo
│   ├── DadosInvalidosException.cs
│   ├── EmailJaCadastradoException.cs
│   ├── PersistenciaException.cs
│   ├── SenhaIncorretaException.cs
│   ├── UsuarioBloqueadoException.cs
│   └── UsuarioNaoEncontradoException.cs
├── Services/
│   ├── UsuarioService.cs           Regras de negócio e gerenciamento (Dictionary por e-mail)
│   ├── IUsuarioRepositorio.cs      Contrato de leitura e gravação dos dados
│   └── UsuarioRepositorioJson.cs   Leitura e escrita do usuarios.json (System.Text.Json)
└── Program.cs                      Menu do console com try/catch
```

| Camada | Responsabilidade |
|---|---|
| `Entities` | Protege o estado do `Usuario`: as propriedades têm `private set` e só mudam por métodos que validam e lançam exceção. |
| `Exceptions` | Cada violação de regra tem uma exceção com nome próprio, tratada separadamente no menu. |
| `Services` | `UsuarioService` aplica o e-mail único, o login, a consulta e salva após cada alteração. O acesso ao arquivo fica isolado em `UsuarioRepositorioJson`, por trás da interface `IUsuarioRepositorio`. |
| `Program.cs` | Exibe o menu, lê as entradas com `int.TryParse` e trata as exceções da mais específica para a mais genérica. |

**Persistência:** o arquivo `usuarios.json` é criado na pasta de execução (por exemplo, `MitosNucleares.Project/bin/Debug/net10.0/`). Se o arquivo não existir, a aplicação começa com a lista vazia. Se estiver corrompido, a aplicação avisa e continua funcionando. O arquivo não é versionado (`.gitignore`).

## Como executar

Pré-requisito: [.NET SDK 10](https://dotnet.microsoft.com/download).

```bash
git clone <url-do-repositorio>
cd MitosNucleares
dotnet build
dotnet run --project MitosNucleares.Project
```

Menu:

```
1 - Cadastrar usuário
2 - Listar todos os usuários
3 - Buscar usuário por nome ou e-mail
4 - Login
5 - Adicionar pontos a um usuário
0 - Sair
```

Roteiro rápido de teste: cadastre um usuário, liste, busque por parte do nome, erre a senha 3 vezes no login (o usuário fica bloqueado) e reabra a aplicação para ver os dados e o bloqueio mantidos.

> Observação: por ser uma prova de conceito, a senha é gravada em texto puro no JSON. Em produção ela seria armazenada com hash.

## Como o .NET se aplica à solução do grupo?

O .NET cuida dos jogadores da solução. Enquanto a API Java gerencia as perguntas do "Mito ou Verdade", este módulo cadastra os usuários, garante que cada e-mail seja único, controla o login com bloqueio após tentativas erradas e guarda a pontuação que alimenta o ranking exibido no app e no totem das escolas. A orientação a objetos do C#, as exceções personalizadas e as coleções (`Dictionary` e `List`) deixam as regras de negócio protegidas dentro da entidade e do serviço. Como o acesso aos dados fica atrás da interface `IUsuarioRepositorio`, o arquivo JSON desta Sprint pode ser trocado depois por um banco Oracle com Entity Framework Core, sem mudar as regras, e o mesmo serviço pode ser exposto como Web API para o app Flutter e o totem consumirem.

using MitosNucleares.Project.Entities;
using MitosNucleares.Project.Exceptions;
using MitosNucleares.Project.Services;

namespace MitosNucleares.Project
{
    internal class Program
    {
        static void Main(string[] args)
        {
            UsuarioService service = new UsuarioService();
            bool executando = true;

            Console.WriteLine("=== Mitos e Verdades Nucleares: Usuários ===");

            while (executando)
            {
                ExibirMenu();
                string entrada = LerTexto("Escolha uma opção: ");

                if (!int.TryParse(entrada, out int opcao))
                {
                    Console.WriteLine("Opção inválida. Digite um número do menu.");
                    continue;
                }

                try
                {
                    switch (opcao)
                    {
                        case 1:
                            CadastrarUsuario(service);
                            break;
                        case 2:
                            ListarUsuarios(service);
                            break;
                        case 3:
                            BuscarUsuarios(service);
                            break;
                        case 4:
                            FazerLogin(service);
                            break;
                        case 5:
                            AdicionarPontos(service);
                            break;
                        case 0:
                            executando = false;
                            break;
                        default:
                            Console.WriteLine("Opção inexistente. Escolha um número do menu.");
                            break;
                    }
                }
                catch (DadosInvalidosException ex)
                {
                    Console.WriteLine($"[Dados inválidos]: {ex.Message}");
                }
                catch (EmailJaCadastradoException ex)
                {
                    Console.WriteLine($"[Cadastro]: {ex.Message}");
                }
                catch (UsuarioNaoEncontradoException ex)
                {
                    Console.WriteLine($"[Busca]: {ex.Message}");
                }
                catch (SenhaIncorretaException ex)
                {
                    Console.WriteLine($"[Login]: {ex.Message}");
                }
                catch (UsuarioBloqueadoException ex)
                {
                    Console.WriteLine($"[Bloqueio]: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Erro inesperado]: {ex.Message}");
                }
            }

            Console.WriteLine("Aplicação encerrada.");
        }

        static void ExibirMenu()
        {
            Console.WriteLine();
            Console.WriteLine("1 - Cadastrar usuário");
            Console.WriteLine("2 - Listar todos os usuários");
            Console.WriteLine("3 - Buscar usuário por nome ou e-mail");
            Console.WriteLine("4 - Login");
            Console.WriteLine("5 - Adicionar pontos a um usuário");
            Console.WriteLine("0 - Sair");
        }

        static string LerTexto(string mensagem)
        {
            Console.Write(mensagem);
            string? entrada = Console.ReadLine();

            if (entrada == null)
                return "";

            return entrada.Trim();
        }

        static void CadastrarUsuario(UsuarioService service)
        {
            string nome = LerTexto("Nome: ");
            string email = LerTexto("E-mail: ");
            string senha = LerTexto("Senha (mínimo 6 caracteres, com letra e número): ");

            Usuario usuario = new Usuario(nome, email, senha);
            service.Cadastrar(usuario);
            Console.WriteLine($"Usuário {usuario.Nome} cadastrado com sucesso!");
        }

        static void ListarUsuarios(UsuarioService service)
        {
            List<Usuario> usuarios = service.ListarTodos();

            if (usuarios.Count == 0)
            {
                Console.WriteLine("Nenhum usuário cadastrado.");
                return;
            }

            Console.WriteLine($"--- {usuarios.Count} usuário(s) cadastrado(s) ---");
            ExibirUsuarios(usuarios);
        }

        static void BuscarUsuarios(UsuarioService service)
        {
            string termo = LerTexto("Digite o nome ou e-mail (ou parte dele): ");
            List<Usuario> encontrados = service.BuscarPorNomeOuEmail(termo);

            Console.WriteLine($"--- {encontrados.Count} usuário(s) encontrado(s) ---");
            ExibirUsuarios(encontrados);
        }

        static void FazerLogin(UsuarioService service)
        {
            string email = LerTexto("E-mail: ");
            string senha = LerTexto("Senha: ");

            Usuario usuario = service.Login(email, senha);
            Console.WriteLine($"Login realizado! Bem-vindo(a), {usuario.Nome}. Sua pontuação: {usuario.Pontuacao}.");
        }

        static void AdicionarPontos(UsuarioService service)
        {
            string email = LerTexto("E-mail do usuário: ");
            string entradaPontos = LerTexto("Pontos ganhos na partida: ");

            if (!int.TryParse(entradaPontos, out int pontos))
            {
                Console.WriteLine("Valor inválido. Digite um número inteiro.");
                return;
            }

            service.AdicionarPontos(email, pontos);
            Console.WriteLine("Pontuação atualizada com sucesso!");
        }

        static void ExibirUsuarios(List<Usuario> usuarios)
        {
            foreach (Usuario usuario in usuarios)
            {
                string situacao = usuario.Bloqueado ? "Bloqueado" : "Ativo";
                Console.WriteLine($"{usuario.Nome} | {usuario.Email} | {usuario.Pontuacao} pts | {situacao}");
            }
        }
    }
}

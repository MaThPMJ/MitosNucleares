using System.Text.Json.Serialization;
using MitosNucleares.Project.Exceptions;

namespace MitosNucleares.Project.Entities
{
    public class Usuario
    {
        private static readonly int MaximoTentativas = 3;
        private static readonly int TamanhoMinimoSenha = 6;

        public string Nome { get; private set; }
        public string Email { get; private set; }
        public string Senha { get; private set; }
        public int Pontuacao { get; private set; }
        public int TentativasErradas { get; private set; }
        public bool Bloqueado { get; private set; }

        public Usuario(string nome, string email, string senha)
            : this(nome, email, senha, 0, 0, false)
        {
        }

        [JsonConstructor]
        public Usuario(string nome, string email, string senha, int pontuacao, int tentativasErradas, bool bloqueado)
        {
            ValidarNome(nome);
            ValidarEmail(email);
            ValidarSenha(senha);
            ValidarPontuacao(pontuacao);

            if (tentativasErradas < 0 || tentativasErradas > MaximoTentativas)
                throw new DadosInvalidosException($"O número de tentativas erradas deve estar entre 0 e {MaximoTentativas}.");

            Nome = nome.Trim();
            Email = email.Trim().ToLower();
            Senha = senha;
            Pontuacao = pontuacao;
            TentativasErradas = tentativasErradas;
            Bloqueado = bloqueado;
        }

        public void ValidarLogin(string senha)
        {
            if (Bloqueado)
                throw new UsuarioBloqueadoException($"O usuário {Email} está bloqueado após {MaximoTentativas} tentativas de senha erradas.");

            if (Senha != senha)
            {
                TentativasErradas++;

                if (TentativasErradas >= MaximoTentativas)
                {
                    Bloqueado = true;
                    throw new UsuarioBloqueadoException($"Senha incorreta. O usuário {Email} foi bloqueado após {MaximoTentativas} tentativas erradas.");
                }

                int restantes = MaximoTentativas - TentativasErradas;
                throw new SenhaIncorretaException($"Senha incorreta. Restam {restantes} tentativa(s) antes do bloqueio.");
            }

            TentativasErradas = 0;
        }

        public void AdicionarPontos(int pontos)
        {
            if (pontos <= 0)
                throw new DadosInvalidosException("A quantidade de pontos deve ser maior que zero.");

            Pontuacao += pontos;
        }

        private static void ValidarNome(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new DadosInvalidosException("O nome é obrigatório.");

            if (nome.Trim().Length < 3)
                throw new DadosInvalidosException("O nome deve ter pelo menos 3 caracteres.");
        }

        private static void ValidarEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new DadosInvalidosException("O e-mail é obrigatório.");

            string valor = email.Trim();
            int posicaoArroba = valor.IndexOf('@');

            if (valor.Contains(' ')
                || posicaoArroba <= 0
                || posicaoArroba != valor.LastIndexOf('@')
                || valor.LastIndexOf('.') < posicaoArroba + 2
                || valor.EndsWith("."))
            {
                throw new DadosInvalidosException($"O e-mail '{valor}' não está em um formato válido (ex.: nome@dominio.com).");
            }
        }

        private static void ValidarSenha(string senha)
        {
            if (string.IsNullOrWhiteSpace(senha))
                throw new DadosInvalidosException("A senha é obrigatória.");

            if (senha.Length < TamanhoMinimoSenha)
                throw new DadosInvalidosException($"A senha deve ter pelo menos {TamanhoMinimoSenha} caracteres.");

            bool temLetra = false;
            bool temNumero = false;

            foreach (char caractere in senha)
            {
                if (char.IsLetter(caractere))
                    temLetra = true;
                else if (char.IsDigit(caractere))
                    temNumero = true;
            }

            if (!temLetra || !temNumero)
                throw new DadosInvalidosException("A senha deve conter pelo menos uma letra e um número.");
        }

        private static void ValidarPontuacao(int pontuacao)
        {
            if (pontuacao < 0)
                throw new DadosInvalidosException("A pontuação não pode ser negativa.");
        }
    }
}

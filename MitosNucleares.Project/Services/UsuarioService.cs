using MitosNucleares.Project.Entities;
using MitosNucleares.Project.Exceptions;

namespace MitosNucleares.Project.Services
{
    public class UsuarioService
    {
        private readonly Dictionary<string, Usuario> _usuarios = new Dictionary<string, Usuario>();

        public void Cadastrar(Usuario usuario)
        {
            if (_usuarios.ContainsKey(usuario.Email))
                throw new EmailJaCadastradoException($"O e-mail {usuario.Email} já está cadastrado.");

            _usuarios.Add(usuario.Email, usuario);
        }

        public Usuario Login(string email, string senha)
        {
            Usuario usuario = BuscarPorEmail(email);
            usuario.ValidarLogin(senha);
            return usuario;
        }

        public void AdicionarPontos(string email, int pontos)
        {
            Usuario usuario = BuscarPorEmail(email);
            usuario.AdicionarPontos(pontos);
        }

        public Usuario BuscarPorEmail(string email)
        {
            string chave = email.Trim().ToLower();

            if (!_usuarios.TryGetValue(chave, out Usuario? usuario))
                throw new UsuarioNaoEncontradoException($"Nenhum usuário encontrado com o e-mail {chave}.");

            return usuario;
        }
    }
}

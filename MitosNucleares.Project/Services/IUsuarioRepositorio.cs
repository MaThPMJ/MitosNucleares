using MitosNucleares.Project.Entities;

namespace MitosNucleares.Project.Services
{
    public interface IUsuarioRepositorio
    {
        List<Usuario> Carregar();
        void Salvar(List<Usuario> usuarios);
    }
}

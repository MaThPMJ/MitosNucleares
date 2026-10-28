using System.Text.Json;
using MitosNucleares.Project.Entities;
using MitosNucleares.Project.Exceptions;

namespace MitosNucleares.Project.Services
{
    public class UsuarioRepositorioJson : IUsuarioRepositorio
    {
        private static readonly JsonSerializerOptions _opcoes = new JsonSerializerOptions { WriteIndented = true };
        private readonly string _caminhoArquivo;

        public UsuarioRepositorioJson(string caminhoArquivo)
        {
            _caminhoArquivo = caminhoArquivo;
        }

        public string CaminhoArquivo
        {
            get { return _caminhoArquivo; }
        }

        public List<Usuario> Carregar()
        {
            if (!File.Exists(_caminhoArquivo))
                return new List<Usuario>();

            try
            {
                string conteudo = File.ReadAllText(_caminhoArquivo);

                if (string.IsNullOrWhiteSpace(conteudo))
                    return new List<Usuario>();

                List<Usuario>? usuarios = JsonSerializer.Deserialize<List<Usuario>>(conteudo, _opcoes);

                if (usuarios == null)
                    return new List<Usuario>();

                return usuarios;
            }
            catch (JsonException)
            {
                throw new PersistenciaException($"O arquivo {_caminhoArquivo} está corrompido e não pôde ser lido.");
            }
            catch (DadosInvalidosException ex)
            {
                throw new PersistenciaException($"O arquivo {_caminhoArquivo} contém dados inválidos: {ex.Message}");
            }
            catch (IOException ex)
            {
                throw new PersistenciaException($"Não foi possível ler o arquivo {_caminhoArquivo}: {ex.Message}");
            }
        }

        public void Salvar(List<Usuario> usuarios)
        {
            try
            {
                string conteudo = JsonSerializer.Serialize(usuarios, _opcoes);
                File.WriteAllText(_caminhoArquivo, conteudo);
            }
            catch (IOException ex)
            {
                throw new PersistenciaException($"Não foi possível salvar o arquivo {_caminhoArquivo}: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new PersistenciaException($"Sem permissão para salvar o arquivo {_caminhoArquivo}: {ex.Message}");
            }
        }
    }
}

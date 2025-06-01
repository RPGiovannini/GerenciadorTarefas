using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using GerenciadorTarefas.WPF.Enums;
using GerenciadorTarefas.WPF.Models;
using Newtonsoft.Json;

namespace GerenciadorTarefas.WPF.Services
{
    public class TarefaService : ITarefaService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://localhost:5001/api/Tarefas";

        public TarefaService()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        public async Task<List<TarefaModel>> ObterTarefasAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(_baseUrl);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Erro HTTP {response.StatusCode}: {errorContent}");
                }

                var json = await response.Content.ReadAsStringAsync();
                var tarefas = JsonConvert.DeserializeObject<List<TarefaModel>>(json);

                return tarefas ?? new List<TarefaModel>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter tarefas: {ex.Message}");
            }
        }

        public async Task<TarefaModel> ObterTarefaPorIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Erro HTTP {response.StatusCode}: {errorContent}");
                }

                var json = await response.Content.ReadAsStringAsync();
                var tarefa = JsonConvert.DeserializeObject<TarefaModel>(json);

                return tarefa ?? throw new Exception("Tarefa não encontrada");
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter tarefa: {ex.Message}");
            }
        }

        public async Task<TarefaModel> CriarTarefaAsync(TarefaModel tarefa)
        {
            try
            {
                // Garantir que temos um status válido
                var statusValue = (int)tarefa.Status;

                // Criar o objeto base
                var requestData = new
                {
                    titulo = tarefa.Titulo,
                    descricao = tarefa.Descricao,
                    dataInicio = tarefa.DataCriacao,
                    dataFim = tarefa.Status == ETarefaStatus.Concluido ? (DateTime?)DateTime.Now.AddMinutes(1) : null,
                    status = statusValue
                };

                var json = JsonConvert.SerializeObject(requestData, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Include
                });

                System.Diagnostics.Debug.WriteLine($"JSON sendo enviado: {json}");

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(_baseUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Erro HTTP {response.StatusCode}: {errorContent}");
                }

                // A API retorna apenas o ID, então fazemos uma nova requisição para obter a tarefa completa
                var responseJson = await response.Content.ReadAsStringAsync();
                var tarefaId = JsonConvert.DeserializeObject<int>(responseJson);

                // Buscar a tarefa criada
                var tarefaCriada = await ObterTarefaPorIdAsync(tarefaId);

                return tarefaCriada;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao criar tarefa: {ex.Message}");
            }
        }

        public async Task<TarefaModel> AtualizarTarefaAsync(TarefaModel tarefa)
        {
            try
            {
                // Se o status for Concluído e não tiver data de conclusão, definir como agora + 1 minuto
                var dataConclusao = tarefa.DataConclusao;
                if (tarefa.Status == ETarefaStatus.Concluido && dataConclusao == null)
                {
                    dataConclusao = DateTime.Now.AddMinutes(1);
                }

                var json = JsonConvert.SerializeObject(new
                {
                    titulo = tarefa.Titulo,
                    descricao = tarefa.Descricao,
                    dataCriacao = tarefa.DataCriacao,
                    dataConclusao = dataConclusao,
                    status = (int)tarefa.Status
                });

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"{_baseUrl}/{tarefa.Id}", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Erro HTTP {response.StatusCode}: {errorContent}");
                }

                // A API retorna apenas o ID, então fazemos uma nova requisição para obter a tarefa completa
                var responseJson = await response.Content.ReadAsStringAsync();
                var tarefaId = JsonConvert.DeserializeObject<int>(responseJson);

                // Buscar a tarefa atualizada
                var tarefaAtualizada = await ObterTarefaPorIdAsync(tarefaId);

                return tarefaAtualizada;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao atualizar tarefa: {ex.Message}");
            }
        }

        public async Task<bool> DeletarTarefaAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_baseUrl}/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Erro HTTP {response.StatusCode}: {errorContent}");
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao deletar tarefa: {ex.Message}");
            }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
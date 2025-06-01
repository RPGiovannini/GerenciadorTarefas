using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using GerenciadorTarefas.WPF.Enums;
using GerenciadorTarefas.WPF.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

        private async Task<string> ProcessarErroResponseAsync(HttpResponseMessage response)
        {
            var errorContent = await response.Content.ReadAsStringAsync();

            // Se for BadRequest (400), tentar extrair apenas a mensagem principal
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                try
                {
                    var errorJson = JObject.Parse(errorContent);

                    // Tentar extrair o título ou mensagem principal
                    var titulo = errorJson["Titulo"]?.ToString();
                    if (!string.IsNullOrEmpty(titulo))
                    {
                        return titulo;
                    }

                    // Se não houver título, tentar extrair a primeira mensagem de erro
                    var errors = errorJson["errors"];
                    if (errors != null)
                    {
                        var firstError = errors.First;
                        if (firstError is JProperty prop && prop.Value is JArray array && array.Count > 0)
                        {
                            return array[0].ToString();
                        }
                    }

                    // Se houver uma mensagem direta
                    var message = errorJson["message"]?.ToString();
                    if (!string.IsNullOrEmpty(message))
                    {
                        return message;
                    }
                }
                catch
                {
                    // Se não conseguir parsear o JSON, retornar mensagem padrão
                    return "Dados inválidos. Verifique as informações e tente novamente.";
                }

                return "Dados inválidos. Verifique as informações e tente novamente.";
            }

            // Para outros tipos de erro, retornar o conteúdo completo
            return errorContent;
        }

        public async Task<List<TarefaModel>> ObterTarefasAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(_baseUrl);

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = await ProcessarErroResponseAsync(response);
                    throw new Exception($"Erro HTTP {response.StatusCode}: {errorMessage}");
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
                    var errorMessage = await ProcessarErroResponseAsync(response);
                    throw new Exception($"Erro HTTP {response.StatusCode}: {errorMessage}");
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
                    var errorMessage = await ProcessarErroResponseAsync(response);
                    throw new Exception($"Erro ao criar tarefa: {errorMessage}");
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
                    var errorMessage = await ProcessarErroResponseAsync(response);
                    throw new Exception($"Erro ao atualizar tarefa: {errorMessage}");
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
                    var errorMessage = await ProcessarErroResponseAsync(response);
                    throw new Exception($"Erro HTTP {response.StatusCode}: {errorMessage}");
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
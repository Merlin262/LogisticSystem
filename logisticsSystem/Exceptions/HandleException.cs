using System.Text.Json;
using Microsoft.Data.SqlClient;

namespace logisticsSystem.Exceptions
{
    public class HandleException
    {
        public void HandleExceptioGeneric(Exception ex)
        {
            if (ex is SqlException sqlEx)
            {
                Console.WriteLine($"Erro de conexão com o banco de dados: {sqlEx.Message}");
                throw new DatabaseConnectionException("Erro de conexão com o banco de dados.");
            }
            else if (ex is JsonException jsonEx)
            {
                Console.WriteLine($"Erro de serialização JSON: {jsonEx.Message}");
                throw new InvalidDataTypeException("Erro na serialização JSON.");
            }
            else
            {
                Console.WriteLine($"Erro inesperado: {ex.Message}");
                throw new InternalServerException("Erro interno do servidor.");
            }
        }
    }
}

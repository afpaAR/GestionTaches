using System;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace TachesTestDeploie
{
    public class Connexion
    {
        private static string? _connectionString;

        public Connexion()
        {
            // Priorité : variable d'environnement - On va vérifier s'il existe un environnement de test nommé TACHES_DB - Si oui, on prend la valeur stockée de cet environnement comme chaîne de caractères.
            var envCs = Environment.GetEnvironmentVariable("TACHES_DB");
            if (!string.IsNullOrEmpty(envCs))
            {
                _connectionString = envCs;
            }
            else
            {
                // Sinon on charge depuis appsettings.json (prod)
                var config = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                    .Build();

                _connectionString = config.GetConnectionString("Tachesdb"); // production
            }
        }

        public SqlConnection GetConnection()
        {
            if (string.IsNullOrEmpty(_connectionString))
                throw new InvalidOperationException("La chaîne de connexion n'a pas été définie.");

            return new SqlConnection(_connectionString);
        }
    }
}

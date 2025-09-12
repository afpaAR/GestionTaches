using System.Data;
using GestionTaches.Models;
using Microsoft.Data.SqlClient;
using TachesTestDeploie;

namespace GestionTaches.Repositories
{
    internal class TacheRepository
    {

        private SqlConnection activeConnexion;
        public TacheRepository()
        {
            dbConnecter();
        }

        private void dbConnecter()
        {
            Connexion con = new Connexion();
            activeConnexion = con.GetConnection();
            activeConnexion.Open();
        }

        private void chkConnexion()
        {
            if (activeConnexion == null || activeConnexion.State == ConnectionState.Closed)
            {
                dbConnecter();
            }
        }

        public void AddTache(Taches Tache)
        {
            chkConnexion();

            using SqlCommand RequestUpdatetache = activeConnexion.CreateCommand();
            RequestUpdatetache.CommandText = "INSERT INTO TACHES (Id, Nom, Description, DateCreation) VALUES (@Id, @Nom, @Desc, @DateCrea)";

            RequestUpdatetache.Parameters.Add("@Id", SqlDbType.Int).Value = getLastId();
            RequestUpdatetache.Parameters.Add("@Nom", SqlDbType.NChar).Value = Tache.Nom;
            RequestUpdatetache.Parameters.Add("@Desc", SqlDbType.VarChar).Value = Tache.Description;
            RequestUpdatetache.Parameters.Add("@DateCrea", SqlDbType.Date).Value = Tache.DateCreation;

            RequestUpdatetache.ExecuteNonQuery();
        }

        public int getLastId()
        {

            chkConnexion();

            int lastId = 0;
            using SqlCommand RequestGetLastId = activeConnexion.CreateCommand();
            RequestGetLastId.CommandText = "Select max(Id) from TACHES";

            var id = RequestGetLastId.ExecuteScalar().ToString();

            if (id is null || id.Length == 0)
            {
                lastId = 1;
            }
            else
            {
                lastId = Convert.ToInt32(id);
                lastId = lastId + 1;
            }
            return lastId;
        }

  
        public void MarkCompleted(int Id)
        {
            chkConnexion();

            using SqlCommand RequestUpdatetache = activeConnexion.CreateCommand();
            RequestUpdatetache.CommandText = "UPDATE TACHES SET dateFermeture = @dateFermeture WHERE Id = @id";

            RequestUpdatetache.Parameters.Add("@dateFermeture", SqlDbType.Date).Value = DateTime.Now.Date;
            RequestUpdatetache.Parameters.Add("@id", SqlDbType.Int).Value = Id;

            RequestUpdatetache.ExecuteNonQuery();

        }

        public void DeleteTache(int Id)
        {

            chkConnexion();

            //Meilleure pratique d'uliser le using, il n'y a plus à se soucier des open/close de la connection
            using SqlCommand RequestUpdatetache = activeConnexion.CreateCommand();
            RequestUpdatetache.CommandText = "DELETE TACHES WHERE Id = @id";

            RequestUpdatetache.Parameters.Add("@id", SqlDbType.Int).Value = Id;

            RequestUpdatetache.ExecuteNonQuery();
        }

        //Ici la meilleure façon d'écrire GetTaches() avec les ouvertures/fermetures de connection et stream avec using - Plus difficile à lire mais qui ressemble à ce que vous devez arriver in fine
        public List<Taches> GetTaches()
        {
            this.chkConnexion();

            List<Taches> Taches = new List<Taches>();

            using (SqlCommand RequestGetTaches = activeConnexion.CreateCommand())
            {
                RequestGetTaches.CommandText = "SELECT * FROM TACHES";

                using (SqlDataReader taches = RequestGetTaches.ExecuteReader())
                {
                    while (taches.Read())
                    {
                        Taches oneTache = new Taches
                        {
                            Id = Int32.Parse(taches[0].ToString()),
                            Nom = $"{taches[1]}",
                            Description = $"{taches[2]}",
                            DateCreation = DateTime.Parse(taches[3].ToString()),
                            DateFermeture = taches[4].ToString() == "" ? DateTime.MinValue : DateTime.Parse(taches[4].ToString())
                        };
                        Taches.Add(oneTache);
                    }
                }
            }
            return Taches;
        }
    }
}

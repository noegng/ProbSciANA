using System;
using System.Collections.Generic;

namespace ProbSciANA
{
    public class Arete<T>
    {
        private T idPrevious;
        private T idNext;
        private string idLigne;
        private int temps;
        private bool sensUnique;
        private static Dictionary<string, double> vitesseMoyenne = new Dictionary<string, double>();
        private static Dictionary<T, double[]> longitudeLatitude = new Dictionary<T, double[]>();
        public Arete(T idPrevious, T idNext, string idLigne, bool sensUnique = false) {
        this.idPrevious = idPrevious;
        this.idNext = idNext;
        this.idLigne = idLigne;
        this.sensUnique = sensUnique;
        temps = CalculerTempsTrajet();
    }
#region Properties
        public T IdPrevious
        {
            get { return idPrevious; }
            set { idPrevious = value; }
        }

        public T IdNext
        {
            get { return idNext; }
            set { idNext = value; }
        }

        public string IdLigne
        {
            get { return idLigne; }
            set { idLigne = value; }
        }

        public int Temps
        {
            get { return temps; }
            set { temps = value; }
        }
        public static Dictionary<string, double> VitesseMoyenne {
            get {
                return vitesseMoyenne;
            }
            set {
                vitesseMoyenne = value;
            }
        }
        public bool SensUnique {
            get {
                return sensUnique;
            }
            set {
                sensUnique = value;
            }
        }
        public static Dictionary<T, double[]> LongitudeLatitude{
            get {
                return longitudeLatitude;
            }
            set {
                longitudeLatitude = value;
            }
        }
#endregion
        public bool Equals(Arete<T> other)
        {
            if (other == null) return false;
            return EqualityComparer<T>.Default.Equals(IdPrevious, other.IdPrevious) &&
                   EqualityComparer<T>.Default.Equals(IdNext, other.IdNext) &&
                   IdLigne == other.IdLigne;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Arete<T>);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(IdPrevious, IdNext, IdLigne);
        }
        public double CalculerDistance()
        {
            double R = 6371; // Rayon de la Terre en km
            double dLat = Convert.ToDouble((longitudeLatitude[IdNext][1] - longitudeLatitude[IdPrevious][1]) * Math.PI / 180.0);
            double dLon = Convert.ToDouble((longitudeLatitude[IdNext][0] - longitudeLatitude[IdPrevious][0]) * Math.PI / 180.0);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(Convert.ToDouble(longitudeLatitude[IdPrevious][1] * Math.PI / 180.0)) * Math.Cos(Convert.ToDouble(longitudeLatitude[IdNext][1] * Math.PI / 180.0)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c; // Distance en km
        }
        //Calcul et met a jour la variabl temps ( temps de trajet entre deux stations)
        public int CalculerTempsTrajet()
        {
            // calcul de la distance entre idPrevious et idNext avec la formule de Haversine
            // puis conversion en temps de trajet (en minutes) en fonction de la vitesse moyenne du train
            // (ex: 80 km/h = 1.33 km/min)
            // (ex: 60 km/h = 1 km/min)

            double distance = CalculerDistance();
            double t = distance / VitesseMoyenne[idLigne];
            temps =  (int)t + 1; // +1 pour eviter d'avoir un temps de trajet nul et pour arrondir a l'entier superieur
            return temps;
        }
        
    }
}
#region Lecture fichier si on utilise AreteT
/*
static (List<Station>, List<Arete>) LectureFichierExcel(string excelFilePath){
            var stations = new List<Station>();
            var aretes = new List<Arete>(); 
            var VitessesMoyennes = new Dictionary<string, double>();
            Dictionary<Station, double[]> longitudeLatitude = new Dictionary<Station, double[]>();
            using (var package = new ExcelPackage(new FileInfo(excelFilePath)))
            {
                // On considère la première feuille
                var worksheet = package.Workbook.Worksheets[2]; // On prend la deuxième feuille
                // Les données commencent à la ligne 2 (la ligne 1 contient les titres)
                int i=2;
                while(worksheet.Cells[i, 5].Value != null) //On commence par les vitesses moyennes
                {
                    string IdLigne = worksheet.Cells[i, 5].Value.ToString();
                    double VitesseMoyenne = double.Parse(worksheet.Cells[i, 6].Value.ToString());
                    VitessesMoyennes.Add(IdLigne, VitesseMoyenne);
                    i++;
                }
                Arete<Station>.VitesseMoyenne = VitessesMoyennes; // Initialisation de la vitesse moyenne obligatoire pour le calcul du temps de trajet et la création de l'arête
                worksheet = package.Workbook.Worksheets[1]; // On considère la premiere feuille
                i = 2;
                while (worksheet.Cells[i, 1].Value != null)
                {
                    int Id = int.Parse(worksheet.Cells[i, 1].Value.ToString());
                    string Nom = worksheet.Cells[i, 2].Value.ToString();
                    double Longitude = double.Parse(worksheet.Cells[i, 3].Value.ToString());
                    double Latitude = double.Parse(worksheet.Cells[i, 4].Value.ToString());
                    int tempsChamgement=0;
                if (worksheet.Cells[i, 5].Value != null)
                    {
                        tempsChamgement = int.Parse(worksheet.Cells[i, 5].Value.ToString());
                    }
                    Station station = new Station(Id, Nom, Longitude, Latitude, tempsChamgement);
                    longitudeLatitude[station][0] = Longitude;
                    longitudeLatitude[station][1] = Latitude;
                    stations.Add(station);
                    i++;
                }
                stations.Sort((s1, s2) => s1.Id.CompareTo(s2.Id)); // Tri des stations par Id pour que les Id correspondent aux indices de la liste
                Arete<Station>.LongitudeLatitude = longitudeLatitude;  //La longuitude et la latitude d'une station est obligatoire pour créer une arete
                worksheet = package.Workbook.Worksheets[2]; // On considère la deuxième feuille
                i = 2;
                while(worksheet.Cells[i, 1].Value != null)
                {
                    string IdPrevious = worksheet.Cells[i, 2].Value.ToString();
                    string IdNext = worksheet.Cells[i, 3].Value.ToString();
                    string IdLigne = worksheet.Cells[i, 1].Value.ToString();
                    bool sensUnique = false;
                    if (worksheet.Cells[i, 4].Value != null)
                    {
                        sensUnique = true;
                    }
                    int idStationPrevious = 0;
                    int idStationNext = 0;
                    foreach(Station var in stations)
                    {
                        if(var.Nom == IdPrevious)
                        {
                            idStationPrevious = var.Id;
                        }
                        if (var.Nom == IdNext)
                        {
                            idStationNext = var.Id;
                        }

                    }
                    if (idStationPrevious != 0 && idStationNext != 0) // Aucune station a un id = 0 donc on ne peut pas créer l'arête
                    {
                        Arete areteAllé = new Arete(stations[idStationPrevious-1], stations[idStationNext-1], IdLigne, sensUnique); // Création de l'arête avec les stations correspondantes (on faut cela pour consever toutes les informations des stations dans arete et les -1 car les id commencent à 1)
                        if (!sensUnique) // Si l'arête n'est pas sens unique, on crée l'arête retour
                        {
                            Arete areteRetour = new Arete(stations[idStationNext-1], stations[idStationPrevious-1], IdLigne, sensUnique); // Création de l'arête retour
                            aretes.Add(areteRetour); // Ajout de l'arête retour à la liste des arêtes
                        }
                        aretes.Add(areteAllé); // Ajout de l'arête à la liste des arêtes
                    }
                    i++;
                }
            }
            return (stations, aretes);
        }
        */
#endregion
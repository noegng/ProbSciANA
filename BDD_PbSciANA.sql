DROP DATABASE IF EXISTS PbSciANA;
CREATE DATABASE IF NOT EXISTS PbSciANA;
USE PbSciANA;

DROP TABLE IF EXISTS Utilisateur;
CREATE TABLE IF NOT EXISTS Utilisateur(
   id_utilisateur INT PRIMARY KEY AUTO_INCREMENT,
   nom VARCHAR(50),
   prenom VARCHAR(50),
   adresse VARCHAR(100),
   telephone VARCHAR(10),
   email VARCHAR(50) UNIQUE,
   station VARCHAR(50),
   date_inscription DATETIME DEFAULT NOW(),
   mdp VARCHAR(15)
);

DROP TABLE IF EXISTS Client_;
CREATE TABLE IF NOT EXISTS Client_(
   id_utilisateur INT PRIMARY KEY,
   FOREIGN KEY(id_utilisateur) REFERENCES Utilisateur(id_utilisateur) ON DELETE CASCADE
);

DROP TABLE IF EXISTS Cuisinier;
CREATE TABLE IF NOT EXISTS Cuisinier(
   id_utilisateur INT PRIMARY KEY,
   FOREIGN KEY(id_utilisateur) REFERENCES Utilisateur(id_utilisateur) ON DELETE CASCADE
);

DROP TABLE IF EXISTS Particulier;
CREATE TABLE IF NOT EXISTS Particulier(
   id_utilisateur INT PRIMARY KEY,
   FOREIGN KEY(id_utilisateur) REFERENCES Client_(id_utilisateur) ON DELETE CASCADE
);

DROP TABLE IF EXISTS Entreprise;
CREATE TABLE IF NOT EXISTS Entreprise(
   id_utilisateur INT PRIMARY KEY,
   nom_referent VARCHAR(50),
   FOREIGN KEY(id_utilisateur) REFERENCES Client_(id_utilisateur) ON DELETE CASCADE
);

DROP TABLE IF EXISTS Plat;
CREATE TABLE IF NOT EXISTS Plat(
   id_plat INT PRIMARY KEY AUTO_INCREMENT,
   nom VARCHAR(50) UNIQUE,
   prix DECIMAL(6,2) CHECK (prix >=0),
   nb_portions TINYINT CHECK (nb_portions >=0),
   type_ ENUM('entrée','plat','dessert'),
   regime VARCHAR(50),
   nationalite VARCHAR(50),
   date_peremption DATE,
   photo VARCHAR(50)
);

DROP TABLE IF EXISTS Ingredient;
CREATE TABLE IF NOT EXISTS Ingredient(
   id_ingredient INT PRIMARY KEY AUTO_INCREMENT,
   nom VARCHAR(50) UNIQUE
);

DROP TABLE IF EXISTS Avis;
CREATE TABLE IF NOT EXISTS Avis(
   id_avis INT PRIMARY KEY AUTO_INCREMENT,
   note TINYINT NOT NULL CHECK (note >=0 AND note <=5),
   commentaire TEXT,
   date_avis DATETIME DEFAULT NOW(),
   id_Client_ INT NOT NULL,
   id_cuisinier INT NOT NULL,
   FOREIGN KEY(id_Client_) REFERENCES Client_(id_utilisateur) ON DELETE CASCADE,
   FOREIGN KEY(id_cuisinier) REFERENCES Cuisinier(id_utilisateur) ON DELETE CASCADE
);

DROP TABLE IF EXISTS Commande;
CREATE TABLE IF NOT EXISTS Commande(
   id_commande INT PRIMARY KEY AUTO_INCREMENT,
   nom VARCHAR(50),
   prix DECIMAL(6,2) CHECK (prix >=0),
   statut ENUM('en cours','faite','livrée'),
   date_commande DATETIME,
   id_client INT NULL,
   id_cuisinier INT NULL,
   FOREIGN KEY(id_client) REFERENCES Client_(id_utilisateur) ON DELETE SET NULL,
   FOREIGN KEY(id_cuisinier) REFERENCES Cuisinier(id_utilisateur) ON DELETE SET NULL
);

DROP TABLE IF EXISTS Trajet;
CREATE TABLE IF NOT EXISTS Trajet(
   id_trajet INT PRIMARY KEY AUTO_INCREMENT,
   chemin_optimal VARCHAR(50),
   temps_optimal INT CHECK (temps_optimal >=0),
   id_utilisateur INT NOT NULL,
   FOREIGN KEY(id_utilisateur) REFERENCES Cuisinier(id_utilisateur) ON DELETE CASCADE
);

DROP TABLE IF EXISTS Livraison;
CREATE TABLE IF NOT EXISTS Livraison(
   id_livraison INT PRIMARY KEY AUTO_INCREMENT,
   station VARCHAR(50),
   date_livraison DATETIME,
   statut ENUM('à faire', 'en cours', 'finie'),
   id_trajet INT NULL,
   id_commande INT NOT NULL,
   FOREIGN KEY(id_trajet) REFERENCES Trajet(id_trajet) ON DELETE SET NULL,
   FOREIGN KEY(id_commande) REFERENCES Commande(id_commande) ON DELETE CASCADE
);

DROP TABLE IF EXISTS Cuisine;
CREATE TABLE IF NOT EXISTS Cuisine(
   id_cuisinier INT,
   id_plat INT,
   plat_du_jour BOOL,
   date_cuisine DATETIME,
   statut ENUM('à faire','en cours','fait','livré'),
   PRIMARY KEY(id_cuisinier, id_plat, date_cuisine),
   FOREIGN KEY(id_cuisinier) REFERENCES Cuisinier(id_utilisateur) ON DELETE CASCADE,
   FOREIGN KEY(id_plat) REFERENCES Plat(id_plat) ON DELETE CASCADE
);

DROP TABLE IF EXISTS Compose;
CREATE TABLE IF NOT EXISTS Compose(
   id_plat INT,
   id_ingredient INT,
   quantite TINYINT CHECK(quantite > 0) NOT NULL,
   PRIMARY KEY(id_plat, id_ingredient),
   FOREIGN KEY(id_plat) REFERENCES Plat(id_plat),
   FOREIGN KEY(id_ingredient) REFERENCES Ingredient(id_ingredient)
);

DROP TABLE IF EXISTS Requiert;
CREATE TABLE IF NOT EXISTS Requiert(
   id_plat INT,
   id_livraison INT,
   quantite TINYINT CHECK(quantite > 0) NOT NULL,
   PRIMARY KEY(id_plat, id_livraison),
   FOREIGN KEY(id_plat) REFERENCES Plat(id_plat),
   FOREIGN KEY(id_livraison) REFERENCES Livraison(id_livraison) ON DELETE CASCADE
);


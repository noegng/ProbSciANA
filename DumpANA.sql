CREATE DATABASE  IF NOT EXISTS `pbsciana` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `pbsciana`;
-- MySQL dump 10.13  Distrib 8.0.36, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: pbsciana
-- ------------------------------------------------------
-- Server version	8.0.36

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `avis`
--

DROP TABLE IF EXISTS `avis`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `avis` (
  `id_avis` int NOT NULL AUTO_INCREMENT,
  `note` tinyint NOT NULL,
  `commentaire` text,
  `date_avis` date DEFAULT NULL,
  `id_Client_` int NOT NULL,
  `id_cuisinier` int NOT NULL,
  PRIMARY KEY (`id_avis`),
  KEY `id_Client_` (`id_Client_`),
  KEY `id_cuisinier` (`id_cuisinier`),
  CONSTRAINT `avis_ibfk_1` FOREIGN KEY (`id_Client_`) REFERENCES `client_` (`id_utilisateur`) ON DELETE CASCADE,
  CONSTRAINT `avis_ibfk_2` FOREIGN KEY (`id_cuisinier`) REFERENCES `cuisinier` (`id_utilisateur`) ON DELETE CASCADE,
  CONSTRAINT `avis_chk_1` CHECK (((`note` >= 0) and (`note` <= 5)))
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `avis`
--

LOCK TABLES `avis` WRITE;
/*!40000 ALTER TABLE `avis` DISABLE KEYS */;
INSERT INTO `avis` VALUES (3,3,'Moyen, à améliorer','2025-03-03',3,5),(4,5,'Très bon, recommandé','2025-03-04',4,7),(5,2,'Pas satisfait','2025-03-05',5,8),(6,4,'Bonne expérience','2025-03-06',6,10),(8,3,'Correct','2025-03-08',8,3),(9,4,'Bon rapport qualité-prix','2025-03-09',9,5);
/*!40000 ALTER TABLE `avis` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `client_`
--

DROP TABLE IF EXISTS `client_`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `client_` (
  `id_utilisateur` int NOT NULL,
  PRIMARY KEY (`id_utilisateur`),
  CONSTRAINT `client__ibfk_1` FOREIGN KEY (`id_utilisateur`) REFERENCES `utilisateur` (`id_utilisateur`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `client_`
--

LOCK TABLES `client_` WRITE;
/*!40000 ALTER TABLE `client_` DISABLE KEYS */;
INSERT INTO `client_` VALUES (1),(3),(4),(5),(6),(7),(8),(9),(10);
/*!40000 ALTER TABLE `client_` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `commande`
--

DROP TABLE IF EXISTS `commande`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `commande` (
  `id_commande` int NOT NULL AUTO_INCREMENT,
  `nom` varchar(50) DEFAULT NULL,
  `prix` decimal(6,2) DEFAULT NULL,
  `statut` enum('en cours','faite','livrée') DEFAULT NULL,
  `date_commande` date DEFAULT NULL,
  `id_client` int NOT NULL,
  `id_cuisinier` int NOT NULL,
  PRIMARY KEY (`id_commande`),
  KEY `id_client` (`id_client`),
  KEY `id_cuisinier` (`id_cuisinier`),
  CONSTRAINT `commande_ibfk_1` FOREIGN KEY (`id_client`) REFERENCES `client_` (`id_utilisateur`) ON DELETE CASCADE,
  CONSTRAINT `commande_ibfk_2` FOREIGN KEY (`id_cuisinier`) REFERENCES `cuisinier` (`id_utilisateur`) ON DELETE CASCADE,
  CONSTRAINT `commande_chk_1` CHECK ((`prix` >= 0)),
  CONSTRAINT `diff_client_cuisinier` CHECK ((`id_client` <> `id_cuisinier`))
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `commande`
--

LOCK TABLES `commande` WRITE;
/*!40000 ALTER TABLE `commande` DISABLE KEYS */;
INSERT INTO `commande` VALUES (3,'Commande 3',15.75,'livrée','2025-03-03',3,5),(4,'Commande 4',40.00,'en cours','2025-03-04',4,7),(5,'Commande 5',22.50,'faite','2025-03-05',5,8),(6,'Commande 6',30.00,'livrée','2025-03-06',6,10),(8,'Commande 8',27.80,'faite','2025-03-08',8,3),(9,'Commande 9',33.00,'livrée','2025-03-09',9,5),(10,'Commande 10',25.50,'en cours','2025-03-10',10,7);
/*!40000 ALTER TABLE `commande` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `compose`
--

DROP TABLE IF EXISTS `compose`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `compose` (
  `id_plat` int NOT NULL,
  `id_ingredient` int NOT NULL,
  `quantite` tinyint NOT NULL,
  PRIMARY KEY (`id_plat`,`id_ingredient`),
  KEY `id_ingredient` (`id_ingredient`),
  CONSTRAINT `compose_ibfk_1` FOREIGN KEY (`id_plat`) REFERENCES `plat` (`id_plat`),
  CONSTRAINT `compose_ibfk_2` FOREIGN KEY (`id_ingredient`) REFERENCES `ingredient` (`id_ingredient`),
  CONSTRAINT `compose_chk_1` CHECK ((`quantite` > 0))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `compose`
--

LOCK TABLES `compose` WRITE;
/*!40000 ALTER TABLE `compose` DISABLE KEYS */;
INSERT INTO `compose` VALUES (1,1,2),(1,2,1),(2,1,1),(2,7,2),(3,6,1),(3,7,1),(4,3,1),(4,9,1),(5,4,1),(5,6,1);
/*!40000 ALTER TABLE `compose` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cuisine`
--

DROP TABLE IF EXISTS `cuisine`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `cuisine` (
  `id_cuisinier` int NOT NULL,
  `id_plat` int NOT NULL,
  `date_cuisine` date NOT NULL,
  `statut` enum('à faire','en cours','fait','livré') DEFAULT NULL,
  PRIMARY KEY (`id_cuisinier`,`id_plat`,`date_cuisine`),
  KEY `id_plat` (`id_plat`),
  CONSTRAINT `cuisine_ibfk_1` FOREIGN KEY (`id_cuisinier`) REFERENCES `cuisinier` (`id_utilisateur`) ON DELETE CASCADE,
  CONSTRAINT `cuisine_ibfk_2` FOREIGN KEY (`id_plat`) REFERENCES `plat` (`id_plat`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cuisine`
--

LOCK TABLES `cuisine` WRITE;
/*!40000 ALTER TABLE `cuisine` DISABLE KEYS */;
INSERT INTO `cuisine` VALUES (3,2,'2025-03-02','fait'),(3,8,'2025-03-08','fait'),(5,3,'2025-03-03','fait'),(5,9,'2025-03-09','fait'),(7,4,'2025-03-04','fait'),(7,10,'2025-03-10','fait'),(8,5,'2025-03-05','fait'),(10,6,'2025-03-06','fait');
/*!40000 ALTER TABLE `cuisine` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cuisinier`
--

DROP TABLE IF EXISTS `cuisinier`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `cuisinier` (
  `id_utilisateur` int NOT NULL,
  PRIMARY KEY (`id_utilisateur`),
  CONSTRAINT `cuisinier_ibfk_1` FOREIGN KEY (`id_utilisateur`) REFERENCES `utilisateur` (`id_utilisateur`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cuisinier`
--

LOCK TABLES `cuisinier` WRITE;
/*!40000 ALTER TABLE `cuisinier` DISABLE KEYS */;
INSERT INTO `cuisinier` VALUES (3),(5),(7),(8),(10);
/*!40000 ALTER TABLE `cuisinier` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `entreprise`
--

DROP TABLE IF EXISTS `entreprise`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `entreprise` (
  `id_utilisateur` int NOT NULL,
  `nom_referent` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`id_utilisateur`),
  CONSTRAINT `entreprise_ibfk_1` FOREIGN KEY (`id_utilisateur`) REFERENCES `client_` (`id_utilisateur`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `entreprise`
--

LOCK TABLES `entreprise` WRITE;
/*!40000 ALTER TABLE `entreprise` DISABLE KEYS */;
INSERT INTO `entreprise` VALUES (4,'Responsable B'),(6,'Responsable C'),(8,'Responsable D'),(10,'Responsable E');
/*!40000 ALTER TABLE `entreprise` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ingredient`
--

DROP TABLE IF EXISTS `ingredient`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ingredient` (
  `id_ingredient` int NOT NULL AUTO_INCREMENT,
  `nom` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`id_ingredient`),
  UNIQUE KEY `nom` (`nom`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ingredient`
--

LOCK TABLES `ingredient` WRITE;
/*!40000 ALTER TABLE `ingredient` DISABLE KEYS */;
INSERT INTO `ingredient` VALUES (8,'Ail'),(6,'Boeuf'),(10,'Chocolat'),(3,'Fromage'),(2,'Laitue'),(7,'Oignon'),(4,'Pain'),(9,'Pâte'),(5,'Poulet'),(1,'Tomate');
/*!40000 ALTER TABLE `ingredient` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `livraison`
--

DROP TABLE IF EXISTS `livraison`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `livraison` (
  `id_livraison` int NOT NULL AUTO_INCREMENT,
  `station` varchar(50) DEFAULT NULL,
  `date_livraison` date DEFAULT NULL,
  `statut` enum('à faire','en cours','finie') DEFAULT NULL,
  `id_trajet` int NOT NULL,
  `id_commande` int NOT NULL,
  PRIMARY KEY (`id_livraison`),
  KEY `id_trajet` (`id_trajet`),
  KEY `id_commande` (`id_commande`),
  CONSTRAINT `livraison_ibfk_1` FOREIGN KEY (`id_trajet`) REFERENCES `trajet` (`id_trajet`),
  CONSTRAINT `livraison_ibfk_2` FOREIGN KEY (`id_commande`) REFERENCES `commande` (`id_commande`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `livraison`
--

LOCK TABLES `livraison` WRITE;
/*!40000 ALTER TABLE `livraison` DISABLE KEYS */;
INSERT INTO `livraison` VALUES (3,'Station C','2025-03-13','finie',3,3),(4,'Station D','2025-03-14','à faire',4,4),(5,'Station E','2025-03-15','en cours',5,5),(6,'Station F','2025-03-16','finie',6,6),(8,'Station H','2025-03-18','en cours',8,8),(9,'Station I','2025-03-19','finie',9,9),(10,'Station J','2025-03-20','à faire',10,10);
/*!40000 ALTER TABLE `livraison` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `particulier`
--

DROP TABLE IF EXISTS `particulier`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `particulier` (
  `id_utilisateur` int NOT NULL,
  PRIMARY KEY (`id_utilisateur`),
  CONSTRAINT `particulier_ibfk_1` FOREIGN KEY (`id_utilisateur`) REFERENCES `client_` (`id_utilisateur`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `particulier`
--

LOCK TABLES `particulier` WRITE;
/*!40000 ALTER TABLE `particulier` DISABLE KEYS */;
INSERT INTO `particulier` VALUES (1),(3),(5),(7),(9);
/*!40000 ALTER TABLE `particulier` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `plat`
--

DROP TABLE IF EXISTS `plat`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `plat` (
  `id_plat` int NOT NULL AUTO_INCREMENT,
  `nom` varchar(50) DEFAULT NULL,
  `prix` decimal(6,2) DEFAULT NULL,
  `nb_portions` tinyint DEFAULT NULL,
  `type_` enum('entrée','plat','dessert') DEFAULT NULL,
  `regime` varchar(50) DEFAULT NULL,
  `nationalite` varchar(50) DEFAULT NULL,
  `date_peremption` date DEFAULT NULL,
  `photo` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`id_plat`),
  UNIQUE KEY `nom` (`nom`),
  CONSTRAINT `plat_chk_1` CHECK ((`prix` >= 0)),
  CONSTRAINT `plat_chk_2` CHECK ((`nb_portions` >= 0))
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `plat`
--

LOCK TABLES `plat` WRITE;
/*!40000 ALTER TABLE `plat` DISABLE KEYS */;
INSERT INTO `plat` VALUES (1,'Salade de chèvre',6.50,2,'entrée','Végétarien','Française','2025-05-01','salade.jpg'),(2,'Soupe de légumes',4.00,1,'entrée','Végétarien','Française','2025-04-15','soupe.jpg'),(3,'Steak frites',12.50,1,'plat','Standard','Française','2025-06-01','steak.jpg'),(4,'Pizza Margherita',9.50,1,'plat','Standard','Italienne','2025-06-10','pizza.jpg'),(5,'Burger Classic',8.00,1,'plat','Standard','Américaine','2025-07-01','burger.jpg'),(6,'Pâtes Carbonara',10.00,1,'plat','Standard','Italienne','2025-06-15','pates.jpg'),(7,'Tarte aux pommes',5.00,1,'dessert','Standard','Française','2025-05-30','tarte.jpg'),(8,'Crème brûlée',6.00,1,'dessert','Standard','Française','2025-05-25','creme.jpg'),(9,'Mousse au chocolat',5.50,1,'dessert','Standard','Française','2025-06-05','mousse.jpg'),(10,'Quiche Lorraine',7.50,1,'plat','Standard','Française','2025-06-20','quiche.jpg');
/*!40000 ALTER TABLE `plat` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `requiert`
--

DROP TABLE IF EXISTS `requiert`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `requiert` (
  `id_plat` int NOT NULL,
  `id_livraison` int NOT NULL,
  `quantite` tinyint NOT NULL,
  PRIMARY KEY (`id_plat`,`id_livraison`),
  KEY `id_livraison` (`id_livraison`),
  CONSTRAINT `requiert_ibfk_1` FOREIGN KEY (`id_plat`) REFERENCES `plat` (`id_plat`),
  CONSTRAINT `requiert_ibfk_2` FOREIGN KEY (`id_livraison`) REFERENCES `livraison` (`id_livraison`) ON DELETE CASCADE,
  CONSTRAINT `requiert_chk_1` CHECK ((`quantite` > 0))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `requiert`
--

LOCK TABLES `requiert` WRITE;
/*!40000 ALTER TABLE `requiert` DISABLE KEYS */;
INSERT INTO `requiert` VALUES (3,3,1),(4,4,2),(5,5,1),(6,6,1),(8,8,2),(9,9,1),(10,10,1);
/*!40000 ALTER TABLE `requiert` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `trajet`
--

DROP TABLE IF EXISTS `trajet`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `trajet` (
  `id_trajet` int NOT NULL AUTO_INCREMENT,
  `chemin_optimal` varchar(50) DEFAULT NULL,
  `temps_optimal` int DEFAULT NULL,
  `id_utilisateur` int NOT NULL,
  PRIMARY KEY (`id_trajet`),
  KEY `id_utilisateur` (`id_utilisateur`),
  CONSTRAINT `trajet_ibfk_1` FOREIGN KEY (`id_utilisateur`) REFERENCES `cuisinier` (`id_utilisateur`) ON DELETE CASCADE,
  CONSTRAINT `trajet_chk_1` CHECK ((`temps_optimal` >= 0))
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `trajet`
--

LOCK TABLES `trajet` WRITE;
/*!40000 ALTER TABLE `trajet` DISABLE KEYS */;
INSERT INTO `trajet` VALUES (2,'Chemin B',20,3),(3,'Chemin C',10,5),(4,'Chemin D',25,7),(5,'Chemin E',18,8),(6,'Chemin F',22,10),(8,'Chemin H',19,3),(9,'Chemin I',14,5),(10,'Chemin J',16,7);
/*!40000 ALTER TABLE `trajet` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `utilisateur`
--

DROP TABLE IF EXISTS `utilisateur`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `utilisateur` (
  `id_utilisateur` int NOT NULL AUTO_INCREMENT,
  `nom` varchar(50) DEFAULT NULL,
  `prenom` varchar(50) DEFAULT NULL,
  `email` varchar(50) DEFAULT NULL,
  `adresse` varchar(100) DEFAULT NULL,
  `role` varchar(50) DEFAULT NULL,
  `MotDePasse` varchar(50) DEFAULT NULL,
  `station` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`id_utilisateur`),
  UNIQUE KEY `email` (`email`)
  
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `utilisateur`
--

LOCK TABLES `utilisateur` WRITE;
/*!40000 ALTER TABLE `utilisateur` DISABLE KEYS */;
INSERT INTO utilisateur (id_utilisateur, nom, prenom, email, adresse, role, MotDePasse, station)
VALUES
(1, 'Martin', 'Alice', 'alice.martin@example.com', '12 rue de Paris', 'Client', 'alice123', 'République'),
(2, 'Durand', 'Paul', 'paul.durand@example.com', '45 avenue Victor Hugo', 'Cuisinier', 'paulcuisine', 'Nation'),
(3, 'Leroy', 'Camille', 'camille.leroy@example.com', '7 rue Lafayette', 'Client', 'camille2024', 'Gare du Nord'),
(4, 'Bernard', 'Luc', 'luc.bernard@example.com', '88 boulevard Haussmann', 'Cuisinier', 'lucfood', 'Châtelet'),
(5, 'Petit', 'Sophie', 'sophie.petit@example.com', '31 rue Mouffetard', 'Client', 'sophiepass', 'Montparnasse'),
(6, 'Garcia', 'Julien', 'julien.garcia@example.com', '9 rue Oberkampf', 'Cuisinier', 'juliencook', 'Belleville'),
(7, 'Roux', 'Chloé', 'chloe.roux@example.com', '55 rue de Rivoli', 'Client', 'chloe1234', 'Bastille'),
(8, 'Moreau', 'Nicolas', 'nicolas.moreau@example.com', '23 rue Saint-Honoré', 'Cuisinier', 'nicofood', 'Opéra'),
(9, 'Fournier', 'Emma', 'emma.fournier@example.com', '18 rue de Rennes', 'Client', 'emmaclient', 'Denfert-Rochereau'),
(10, 'Lopez', 'Antoine', 'antoine.lopez@example.com', '102 rue Lecourbe', 'Cuisinier', 'antoinechef', 'La Motte-Picquet');
/*!40000 ALTER TABLE `utilisateur` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-04-01 19:50:20

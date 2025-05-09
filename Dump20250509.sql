CREATE DATABASE  IF NOT EXISTS `pbsciana` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `pbsciana`;
-- MySQL dump 10.13  Distrib 8.0.41, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: pbsciana
-- ------------------------------------------------------
-- Server version	8.0.41

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
  `date_avis` datetime DEFAULT CURRENT_TIMESTAMP,
  `id_client` int DEFAULT NULL,
  `id_cuisinier` int NOT NULL,
  PRIMARY KEY (`id_avis`),
  KEY `id_client` (`id_client`),
  KEY `id_cuisinier` (`id_cuisinier`),
  CONSTRAINT `avis_ibfk_1` FOREIGN KEY (`id_client`) REFERENCES `client_` (`id_utilisateur`) ON DELETE SET NULL,
  CONSTRAINT `avis_ibfk_2` FOREIGN KEY (`id_cuisinier`) REFERENCES `cuisinier` (`id_utilisateur`) ON DELETE CASCADE,
  CONSTRAINT `avis_chk_1` CHECK (((`note` >= 0) and (`note` <= 5)))
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `avis`
--

LOCK TABLES `avis` WRITE;
/*!40000 ALTER TABLE `avis` DISABLE KEYS */;
INSERT INTO `avis` VALUES (1,5,'Excellent service','2025-05-09 21:00:09',1,2),(2,4,'Bon plat','2025-05-09 21:00:09',2,3),(3,3,'Moyen, à améliorer','2025-05-09 21:00:09',3,5),(4,5,'Très bon, recommandé','2025-05-09 21:00:09',4,7),(5,2,'Pas satisfait','2025-05-09 21:00:09',5,8),(6,4,'Bonne expérience','2025-05-09 21:00:09',6,10),(7,5,'Meilleur plat jamais vu','2025-05-09 21:00:09',7,2),(8,3,'Correct','2025-05-09 21:00:09',8,3),(9,4,'Bon rapport qualité-prix','2025-05-09 21:00:09',9,5),(10,5,'Service impeccable','2025-05-09 21:00:09',10,7);
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
INSERT INTO `client_` VALUES (1),(2),(3),(4),(5),(6),(7),(8),(9),(10);
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
  `statut` enum('en attente','en cours','faite','livrée') DEFAULT NULL,
  `date_commande` datetime DEFAULT CURRENT_TIMESTAMP,
  `id_client` int DEFAULT NULL,
  `id_cuisinier` int DEFAULT NULL,
  PRIMARY KEY (`id_commande`),
  KEY `id_client` (`id_client`),
  KEY `id_cuisinier` (`id_cuisinier`),
  CONSTRAINT `commande_ibfk_1` FOREIGN KEY (`id_client`) REFERENCES `client_` (`id_utilisateur`) ON DELETE SET NULL,
  CONSTRAINT `commande_ibfk_2` FOREIGN KEY (`id_cuisinier`) REFERENCES `cuisinier` (`id_utilisateur`) ON DELETE SET NULL,
  CONSTRAINT `commande_chk_1` CHECK ((`prix` >= 0))
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `commande`
--

LOCK TABLES `commande` WRITE;
/*!40000 ALTER TABLE `commande` DISABLE KEYS */;
INSERT INTO `commande` VALUES (1,'Commande 1',20.00,'en cours','2025-05-09 21:00:09',1,2),(2,'Commande 2',35.50,'faite','2025-05-09 21:00:09',2,3),(3,'Commande 3',15.75,'livrée','2025-05-09 21:00:09',3,5),(4,'Commande 4',40.00,'en cours','2025-05-09 21:00:09',4,7),(5,'Commande 5',22.50,'faite','2025-05-09 21:00:09',5,8),(6,'Commande 6',30.00,'livrée','2025-05-09 21:00:09',6,10),(7,'Commande 7',18.25,'en cours','2025-05-09 21:00:09',7,2),(8,'Commande 8',27.80,'faite','2025-05-09 21:00:09',8,3),(9,'Commande 9',33.00,'livrée','2025-05-09 21:00:09',9,5),(10,'Commande 10',25.50,'en cours','2025-05-09 21:00:09',10,7);
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
  `quantite` tinyint DEFAULT NULL,
  PRIMARY KEY (`id_plat`,`id_ingredient`),
  KEY `id_ingredient` (`id_ingredient`),
  CONSTRAINT `compose_ibfk_1` FOREIGN KEY (`id_plat`) REFERENCES `plat` (`id_plat`) ON DELETE CASCADE,
  CONSTRAINT `compose_ibfk_2` FOREIGN KEY (`id_ingredient`) REFERENCES `ingredient` (`id_ingredient`) ON DELETE CASCADE,
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
  `quantite` int DEFAULT NULL,
  `plat_du_jour` tinyint(1) DEFAULT NULL,
  `date_cuisine` datetime NOT NULL,
  `statut` enum('à faire','en cours','fait','livré') DEFAULT NULL,
  `photo` varchar(50) DEFAULT NULL,
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
INSERT INTO `cuisine` VALUES (2,1,2,0,'2025-03-01 11:00:00','fait',NULL),(2,7,3,1,'2025-03-07 17:00:00','fait',NULL),(3,1,4,0,'2025-03-02 12:00:00','fait',NULL),(3,8,2,0,'2025-03-08 18:00:00','fait',NULL),(5,3,2,0,'2025-03-03 13:00:00','fait',NULL),(5,9,4,0,'2025-03-09 19:00:00','fait',NULL),(7,4,3,0,'2025-03-04 14:00:00','fait',NULL),(7,10,3,0,'2025-03-10 20:00:00','fait',NULL),(8,5,2,0,'2025-03-05 15:00:00','fait',NULL),(10,6,2,0,'2025-03-06 16:00:00','fait',NULL);
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
INSERT INTO `cuisinier` VALUES (2),(3),(5),(7),(8),(10);
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
INSERT INTO `entreprise` VALUES (2,'Responsable A'),(4,'Responsable B'),(6,'Responsable C'),(8,'Responsable D'),(10,'Responsable E');
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
  `regime` varchar(50) DEFAULT NULL,
  `photo` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`id_ingredient`),
  UNIQUE KEY `nom` (`nom`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ingredient`
--

LOCK TABLES `ingredient` WRITE;
/*!40000 ALTER TABLE `ingredient` DISABLE KEYS */;
INSERT INTO `ingredient` VALUES (1,'Tomate','végétarien','Images/tomate.png'),(2,'Laitue','végétarien','Images/laitue.png'),(3,'Fromage','végétarien','Images/fromage.png'),(4,'Pain','végétarien','Images/pain.png'),(5,'Poulet','carnivore','Images/poulet.png'),(6,'Boeuf','carnivore','Images/boeuf.png'),(7,'Oignon','végétarien','Images/oignon.png'),(8,'Ail','végétarien','Images/ail.png'),(9,'Pâte','végétarien','Images/pate.png'),(10,'Chocolat','végétarien','Images/chocolat.png');
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
  `date_livraison` datetime DEFAULT NULL,
  `statut` enum('en attente','en cours','finie') DEFAULT NULL,
  `adresse` varchar(100) DEFAULT NULL,
  `id_commande` int NOT NULL,
  PRIMARY KEY (`id_livraison`),
  KEY `id_commande` (`id_commande`),
  CONSTRAINT `livraison_ibfk_1` FOREIGN KEY (`id_commande`) REFERENCES `commande` (`id_commande`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `livraison`
--

LOCK TABLES `livraison` WRITE;
/*!40000 ALTER TABLE `livraison` DISABLE KEYS */;
INSERT INTO `livraison` VALUES (1,'2025-03-11 10:00:00','en attente','10 Rue de Paris, 75001 Paris',1),(2,'2025-03-12 11:00:00','en cours','15 Avenue de la République, 75011 Paris',2),(3,'2025-03-13 12:00:00','finie','20 Boulevard Voltaire, 75012 Paris',3),(4,'2025-03-14 13:00:00','en attente','5 Rue Victor Hugo, 75002 Paris',4),(5,'2025-03-15 14:00:00','en cours','8 Rue de la Paix, 75008 Paris',5),(6,'2025-03-16 15:00:00','finie','12 Rue Lafayette, 75009 Paris',6),(7,'2025-03-17 16:00:00','en attente','22 Avenue Mozart, 75016 Paris',7),(8,'2025-03-18 17:00:00','en cours','18 Rue de Rivoli, 75004 Paris',8),(9,'2025-03-19 18:00:00','finie','7 Rue du Bac, 75007 Paris',9),(10,'2025-03-20 19:00:00','en attente','3 Boulevard Saint-Germain, 75006 Paris',10);
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
INSERT INTO `plat` VALUES (1,'Salade de chèvre',6.50,2,'entrée','Végétarien','Française','2025-05-01','Images/salade.png'),(2,'Paella',4.00,1,'plat','Standard','Espagnole','2025-04-15','Images/paella.png'),(3,'Steak frites',12.50,1,'plat','Standard','Française','2025-06-01','Images/steak.png'),(4,'Pizza Margherita',9.50,1,'plat','Standard','Italienne','2025-06-10','Images/pizza.png'),(5,'Burger Classic',8.00,1,'plat','Standard','Américaine','2025-07-01','Images/burger.png'),(6,'Pâtes Carbonara',10.00,1,'plat','Standard','Italienne','2025-06-15','Images/pates.png'),(7,'Tarte aux pommes',5.00,1,'dessert','Standard','Française','2025-05-30','Images/tarte.png'),(8,'Glace',6.00,1,'dessert','Standard','Italienne','2025-05-25','Images/glace.png'),(9,'Sushis',15.00,1,'plat','Standard','Japonaise','2025-06-05','Images/sushis.png'),(10,'Brunch',13.50,1,'plat','Standard','Anglaise','2025-06-20','Images/brunch.png');
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
  `quantite` tinyint DEFAULT NULL,
  PRIMARY KEY (`id_plat`,`id_livraison`),
  KEY `id_livraison` (`id_livraison`),
  CONSTRAINT `requiert_ibfk_1` FOREIGN KEY (`id_plat`) REFERENCES `plat` (`id_plat`) ON DELETE CASCADE,
  CONSTRAINT `requiert_ibfk_2` FOREIGN KEY (`id_livraison`) REFERENCES `livraison` (`id_livraison`) ON DELETE CASCADE,
  CONSTRAINT `requiert_chk_1` CHECK ((`quantite` > 0))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `requiert`
--

LOCK TABLES `requiert` WRITE;
/*!40000 ALTER TABLE `requiert` DISABLE KEYS */;
INSERT INTO `requiert` VALUES (1,1,2),(2,2,1),(3,3,1),(4,4,2),(5,5,1),(6,6,1),(7,7,1),(8,8,2),(9,9,1),(10,10,1);
/*!40000 ALTER TABLE `requiert` ENABLE KEYS */;
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
  `adresse` varchar(100) DEFAULT NULL,
  `telephone` varchar(10) DEFAULT NULL,
  `email` varchar(50) DEFAULT NULL,
  `station` varchar(50) DEFAULT NULL,
  `date_inscription` datetime DEFAULT CURRENT_TIMESTAMP,
  `mdp` varchar(15) DEFAULT NULL,
  `photo` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`id_utilisateur`),
  UNIQUE KEY `email` (`email`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `utilisateur`
--

LOCK TABLES `utilisateur` WRITE;
/*!40000 ALTER TABLE `utilisateur` DISABLE KEYS */;
INSERT INTO `utilisateur` VALUES (1,'Dupont','Jean','10 Rue de Rivoli, 75001 Paris','0102030405','jean.dupont@example.com','Saint-Paul','2025-02-25 10:00:00','mdp1234','Images/jean.png'),(2,'Martin','Alice','15 Avenue de la République, 75011 Paris','0102030406','alice.martin@example.com','Parmentier','2025-02-26 11:00:00','mdp1234','Images/alice.png'),(3,'Durand','Pierre','20 Boulevard Voltaire, 75012 Paris','0102030407','pierre.durand@example.com','Oberkampf','2025-02-27 12:00:00','mdp1234','Images/pierre.png'),(4,'Leroy','Sophie','5 Avenue Victor Hugo, Paris','0102030408','sophie.leroy@example.com','Kléber','2025-02-28 13:00:00','mdp1234','Images/sophie.png'),(5,'Moreau','Julien','8 Rue de la Paix, 75008 Paris','0102030409','julien.moreau@example.com','Opéra','2025-03-01 14:00:00','mdp1234','Images/julien.png'),(6,'Simon','Claire','12 Rue Lafayette, 75009 Paris','0102030410','claire.simon@example.com','Opéra','2025-03-02 15:00:00','mdp1234','Images/claire.png'),(7,'Laurent','Marc','22 Avenue Mozart, 75016 Paris','0102030411','marc.laurent@example.com','Ranelagh','2025-03-03 16:00:00','mdp1234','Images/marc.png'),(8,'Garnier','Emma','18 Rue de Rivoli, 75004 Paris','0102030412','emma.garnier@example.com','Saint-Paul','2025-03-04 17:00:00','mdp1234','Images/emma.png'),(9,'Roux','Lucas','7 Rue du Bac, 75007 Paris','0102030413','lucas.roux@example.com','Rue du Bac','2025-03-05 18:00:00','mdp1234','Images/lucas.png'),(10,'Morel','Léa','3 Boulevard Saint-Germain, 75006 Paris','0102030414','lea.morel@example.com','Jussieu','2025-03-06 19:00:00','mdp1234','Images/lea.png');
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

-- Dump completed on 2025-05-09 21:01:22

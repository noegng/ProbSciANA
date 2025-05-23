--------------------------
-- Table Utilisateur
--------------------------

INSERT INTO Utilisateur (nom, prenom, adresse, telephone, email, station, date_inscription, mdp, photo) VALUES
('Dupont', 'Jean', '10 Rue de Rivoli, 75001 Paris', '0102030405', 'jean.dupont@example.com', 'Station A', '2025-02-25 10:00:00', 'mdp1234','Images/jean.png'),
('Martin', 'Alice', '15 Avenue de la République, 75011 Paris', '0102030406', 'alice.martin@example.com', 'Station B', '2025-02-26 11:00:00', 'mdp1234','Images/alice.png'),
('Durand', 'Pierre', '20 Boulevard Voltaire, 75012 Paris', '0102030407', 'pierre.durand@example.com', 'Station C', '2025-02-27 12:00:00', 'mdp1234','Images/pierre.png'),
('Leroy', 'Sophie', '5 Avenue Victor Hugo, Paris', '0102030408', 'sophie.leroy@example.com', 'Station D', '2025-02-28 13:00:00', 'mdp1234','Images/sophie.png'),
('Moreau', 'Julien', '8 Rue de la Paix, 75008 Paris', '0102030409', 'julien.moreau@example.com', 'Station E', '2025-03-01 14:00:00', 'mdp1234','Images/julien.png'),
('Simon', 'Claire', '12 Rue Lafayette, 75009 Paris', '0102030410', 'claire.simon@example.com', 'Station F', '2025-03-02 15:00:00', 'mdp1234','Images/claire.png'),
('Laurent', 'Marc', '22 Avenue Mozart, 75016 Paris', '0102030411', 'marc.laurent@example.com', 'Station G', '2025-03-03 16:00:00', 'mdp1234','Images/marc.png'),
('Garnier', 'Emma', '18 Rue de Rivoli, 75004 Paris', '0102030412', 'emma.garnier@example.com', 'Station H', '2025-03-04 17:00:00', 'mdp1234','Images/emma.png'),
('Roux', 'Lucas', '7 Rue du Bac, 75007 Paris', '0102030413', 'lucas.roux@example.com', 'Station I', '2025-03-05 18:00:00', 'mdp1234','Images/lucas.png'),
('Morel', 'Léa', '3 Boulevard Saint-Germain, 75006 Paris', '0102030414', 'lea.morel@example.com', 'Station J', '2025-03-06 19:00:00', 'mdp1234','Images/lea.png');

--------------------------
-- Table Client_
--------------------------
INSERT INTO Client_ (id_utilisateur) VALUES
(1),(2),(3),(4),(5),(6),(7),(8),(9),(10);

--------------------------
-- Table Cuisinier
--------------------------
INSERT INTO Cuisinier (id_utilisateur) VALUES
(2),(3),(5),(7),(8),(10);

--------------------------
-- Table Particulier
--------------------------
INSERT INTO Particulier (id_utilisateur) VALUES
(1),(3),(5),(7),(9);

--------------------------
-- Table Entreprise
--------------------------
INSERT INTO Entreprise (id_utilisateur, nom_referent) VALUES
(2, 'Responsable A'),
(4, 'Responsable B'),
(6, 'Responsable C'),
(8, 'Responsable D'),
(10, 'Responsable E');

--------------------------
-- Table Plat
--------------------------
INSERT INTO Plat (nom, prix, nb_portions, type_, regime, nationalite, date_peremption, photo) VALUES
('Salade de chèvre', 6.50, 2, 'entrée', 'Végétarien', 'Française', '2025-05-01', 'Images/salade.png'),
('Paella', 4.00, 1, 'plat', 'Standard', 'Espagnole', '2025-04-15', 'Images/paella.png'),
('Steak frites', 12.50, 1, 'plat', 'Standard', 'Française', '2025-06-01', 'Images/steak.png'),
('Pizza Margherita', 9.50, 1, 'plat', 'Standard', 'Italienne', '2025-06-10', 'Images/pizza.png'),
('Burger Classic', 8.00, 1, 'plat', 'Standard', 'Américaine', '2025-07-01', 'Images/burger.png'),
('Pâtes Carbonara', 10.00, 1, 'plat', 'Standard', 'Italienne', '2025-06-15', 'Images/pates.png'),
('Tarte aux pommes', 5.00, 1, 'dessert', 'Standard', 'Française', '2025-05-30', 'Images/tarte.png'),
('Glace', 6.00, 1, 'dessert', 'Standard', 'Italienne', '2025-05-25', 'Images/glace.png'),
('Sushis', 15, 1, 'plat', 'Standard', 'Japonaise', '2025-06-05', 'Images/sushis.png'),
('Brunch', 13.50, 1, 'plat', 'Standard', 'Anglaise', '2025-06-20', 'Images/brunch.png');

--------------------------
-- Table Ingredient
--------------------------
INSERT INTO Ingredient (nom, regime, photo) VALUES
('Tomate', 'végétarien','Images/tomate.png'),
('Laitue', 'végétarien','Images/laitue.png'),
('Fromage', 'végétarien','Images/fromage.png'),
('Pain', 'végétarien','Images/pain.png'),
('Poulet', 'carnivore','Images/poulet.png'),
('Boeuf', 'carnivore','Images/boeuf.png'),
('Oignon', 'végétarien','Images/oignon.png'),
('Ail', 'végétarien','Images/ail.png'),
('Pâte', 'végétarien','Images/pate.png'),
('Chocolat', 'végétarien','Images/chocolat.png');

--------------------------
-- Table Avis
--------------------------
INSERT INTO Avis (note, commentaire, id_client, id_cuisinier) VALUES
(5, 'Excellent service', 1, 2),
(4, 'Bon plat', 2, 3),
(3, 'Moyen, à améliorer', 3, 5),
(5, 'Très bon, recommandé', 4, 7),
(2, 'Pas satisfait', 5, 8),
(4, 'Bonne expérience', 6, 10),
(5, 'Meilleur plat jamais vu', 7, 2),
(3, 'Correct', 8, 3),
(4, 'Bon rapport qualité-prix', 9, 5),
(5, 'Service impeccable', 10, 7);

--------------------------
-- Table Commande
--------------------------
INSERT INTO Commande (nom, prix, statut, id_client, id_cuisinier) VALUES
('Commande 1', 20.00, 'en cours', 1, 2),
('Commande 2', 35.50, 'faite', 2, 3),
('Commande 3', 15.75, 'livrée', 3, 5),
('Commande 4', 40.00, 'en cours', 4, 7),
('Commande 5', 22.50, 'faite', 5, 8),
('Commande 6', 30.00, 'livrée', 6, 10),
('Commande 7', 18.25, 'en cours', 7, 2),
('Commande 8', 27.80, 'faite', 8, 3),
('Commande 9', 33.00, 'livrée', 9, 5),
('Commande 10', 25.50, 'en cours', 10, 7);

--------------------------
-- Table Livraison
--------------------------
INSERT INTO Livraison (date_livraison, statut, adresse, id_commande) VALUES
('2025-03-11 10:00:00', 'en attente', '10 Rue de Paris, 75001 Paris', 1),
('2025-03-12 11:00:00', 'en cours', '15 Avenue de la République, 75011 Paris', 2),
('2025-03-13 12:00:00', 'finie', '20 Boulevard Voltaire, 75012 Paris', 3),
('2025-03-14 13:00:00', 'en attente', '5 Rue Victor Hugo, 75002 Paris', 4),
('2025-03-15 14:00:00', 'en cours', '8 Rue de la Paix, 75008 Paris', 5),
('2025-03-16 15:00:00', 'finie', '12 Rue Lafayette, 75009 Paris', 6),
('2025-03-17 16:00:00', 'en attente', '22 Avenue Mozart, 75016 Paris', 7),
('2025-03-18 17:00:00', 'en cours', '18 Rue de Rivoli, 75004 Paris', 8),
('2025-03-19 18:00:00', 'finie', '7 Rue du Bac, 75007 Paris', 9),
('2025-03-20 19:00:00', 'en attente', '3 Boulevard Saint-Germain, 75006 Paris', 10);
--------------------------
-- Table Cuisine
--------------------------
INSERT INTO Cuisine (id_cuisinier, id_plat, quantite, plat_du_jour, date_cuisine, statut) VALUES
(2, 1, 2, FALSE, '2025-03-01 11:00:00', 'fait'),
(3, 1, 4, FALSE, '2025-03-02 12:00:00', 'fait'),
(5, 3, 2, FALSE, '2025-03-03 13:00:00', 'fait'),
(7, 4, 3, FALSE, '2025-03-04 14:00:00', 'fait'),
(8, 5, 2, FALSE, '2025-03-05 15:00:00', 'fait'),
(10, 6, 2, FALSE, '2025-03-06 16:00:00', 'fait'),
(2, 7, 3, TRUE, '2025-03-07 17:00:00', 'fait'),
(3, 8, 2, FALSE, '2025-03-08 18:00:00', 'fait'),
(5, 9, 4, FALSE, '2025-03-09 19:00:00', 'fait'),
(7, 10, 3, FALSE, '2025-03-10 20:00:00', 'fait');


--------------------------
-- Table Compose
--------------------------
INSERT INTO Compose (id_plat, id_ingredient, quantite) VALUES
(1, 1, 2),
(1, 2, 1),
(2, 1, 1),
(2, 7, 2),
(3, 6, 1),
(3, 7, 1),
(4, 3, 1),
(4, 9, 1),
(5, 4, 1),
(5, 6, 1);

--------------------------
-- Table Requiert
--------------------------
INSERT INTO Requiert (id_plat, id_livraison, quantite) VALUES
(1, 1, 2),
(2, 2, 1),
(3, 3, 1),
(4, 4, 2),
(5, 5, 1),
(6, 6, 1),
(7, 7, 1),
(8, 8, 2),
(9, 9, 1),
(10, 10, 1);

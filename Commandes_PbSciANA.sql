#****************** MODULE AUTRE *****************#

SELECT * FROM Client_;

SELECT * FROM Utilisateur;

SELECT nom, prenom, email FROM Utilisateur;

SELECT id_commande, nom, Date_Commande, statut FROM Commande;

SELECT id_commande, nom, Date_Commande, statut FROM Commande;

SELECT * FROM Ingredient;

SELECT * FROM Utilisateur ORDER BY nom ASC;

SELECT * FROM Avis;

UPDATE Utilisateur 
SET telephone = '0987654321' 
WHERE id_utilisateur = 1;

DELETE FROM Avis 
WHERE id_avis = 10;

ALTER TABLE Utilisateur 
ADD COLUMN age INT DEFAULT 30;

SELECT u.nom, u.prenom, c.id_utilisateur 
FROM Utilisateur u 
JOIN Client_ c ON u.id_utilisateur = c.id_utilisateur 
ORDER BY u.nom;

SELECT statut, COUNT(*) AS nb_commandes 
FROM Commande 
GROUP BY statut;

SELECT id_cuisinier, AVG(prix) AS avg_price 
FROM Commande 
GROUP BY id_cuisinier
ORDER BY avg_price;

SELECT * FROM Utilisateur 
WHERE id_utilisateur IN (SELECT id_client FROM Commande);

DELETE FROM Utilisateur 
WHERE id_utilisateur = 2;

SELECT * FROM Client_ WHERE id_utilisateur = 2;

SELECT cu.*
FROM Cuisinier cu
JOIN Client_ cl ON cu.id_utilisateur = cl.id_utilisateur;

SELECT u.id_utilisateur
FROM utilisateur u
JOIN Cuisinier c ON u.id_utilisateur = c.id_utilisateur;

SELECT *
FROM Utilisateur;

SELECT *
FROM Commande;

SELECT *
FROM Livraison;

SELECT l.id_livraison, uc.station AS station_client, uu.station AS station_cuisinier
FROM Livraison l
JOIN Commande cmd ON l.id_commande = cmd.id_commande
JOIN Utilisateur uc ON cmd.id_client = uc.id_utilisateur
JOIN Utilisateur uu ON cmd.id_cuisinier = uu.id_utilisateur
ORDER BY l.id_livraison;

SELECT l.id_livraison, uc.station AS station_client, uu.station AS station_cuisinier
FROM Livraison l
JOIN Commande cmd ON l.id_commande = cmd.id_commande
JOIN Utilisateur uc ON cmd.id_client = uc.id_utilisateur
JOIN Trajet t ON l.id_trajet = t.id_trajet
JOIN Utilisateur uu ON t.id_utilisateur = uu.id_utilisateur
ORDER BY l.id_livraison;

SELECT * FROM Utilisateur;

SELECT u.*,
  CASE WHEN cl.id_utilisateur IS NOT NULL THEN TRUE ELSE FALSE END AS estClient,
  CASE WHEN cu.id_utilisateur IS NOT NULL THEN TRUE ELSE FALSE END AS estCuisinier,
  CASE WHEN e.id_utilisateur IS NOT NULL THEN TRUE ELSE FALSE END AS estEntreprise,
  e.nom_referent
FROM utilisateur u
LEFT JOIN client_ cl ON cl.id_utilisateur = u.id_utilisateur
LEFT JOIN cuisinier cu ON cu.id_utilisateur = u.id_utilisateur
LEFT JOIN entreprise e ON e.id_utilisateur = u.id_utilisateur;

Select * from avis;

#****************** MODULE CLIENT *****************#

SELECT u.id_utilisateur, u.nom, u.prenom, SUM(cmd.prix) AS total_achats
FROM client_ c
JOIN utilisateur u ON c.id_utilisateur = u.id_utilisateur
LEFT JOIN commande cmd ON cmd.id_client = c.id_utilisateur
GROUP BY u.id_utilisateur
ORDER BY u.adresse ASC, u.nom ASC;

SELECT u.id_utilisateur, u.nom, u.prenom, SUM(cmd.prix) AS total_achats
FROM client_ c
JOIN utilisateur u ON c.id_utilisateur = u.id_utilisateur
LEFT JOIN commande cmd ON cmd.id_client = c.id_utilisateur
GROUP BY u.id_utilisateur
ORDER BY u.nom DESC, u.adresse ASC;

SELECT u.id_utilisateur, u.nom, u.prenom, SUM(cmd.prix) AS total_achats
FROM client_ c
JOIN utilisateur u ON c.id_utilisateur = u.id_utilisateur
LEFT JOIN commande cmd ON cmd.id_client = c.id_utilisateur
GROUP BY u.id_utilisateur
ORDER BY total_achats DESC;

SELECT u.id_utilisateur, u.nom, u.prenom, SUM(cmd.prix) AS total_achats
                    FROM client_ c
                    JOIN utilisateur u ON c.id_utilisateur = u.id_utilisateur
                    LEFT JOIN commande cmd ON cmd.id_client = c.id_utilisateur
                    GROUP BY u.id_utilisateur
                    ORDER BY c.estEntreprise, u.id_utilisateur ASC;

#****************** MODULE CUISINIER *****************#
SELECT DISTINCT u.id_utilisateur, u.nom, u.prenom, u.email, cmd.date_commande
FROM Utilisateur u
JOIN Client_ c ON c.id_utilisateur = u.id_utilisateur
JOIN Commande cmd ON cmd.id_client = c.id_utilisateur
JOIN Cuisinier cu ON cmd.id_cuisinier = cu.id_utilisateur
JOIN Utilisateur cu_u ON cu_u.id_utilisateur = cu.id_utilisateur
WHERE cmd.id_cuisinier = 2
AND cmd.date_commande >= cu_u.date_inscription;

SELECT p.nom, COUNT(*) AS frequence
FROM Plat p
JOIN Cuisine c ON p.id_plat = c.id_plat
WHERE c.id_cuisinier = 3
GROUP BY p.nom
ORDER BY frequence DESC;

SELECT p.*
FROM Plat p
JOIN Cuisine c ON p.id_plat = c.id_plat
WHERE c.id_cuisinier = 2
AND c.plat_du_jour = TRUE;

#****************** MODULE COMMANDES *****************#


#****************** MODULE STATISTIQUES *****************#
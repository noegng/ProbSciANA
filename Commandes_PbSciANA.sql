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


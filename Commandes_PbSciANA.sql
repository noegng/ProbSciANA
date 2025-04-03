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

SELECT id_utilisateur, nom, prenom, adresse, telephone, email, station, date_inscription, mdp
FROM Utilisateur;

#****************** MODULE CLIENT *****************#

SELECT u.id_utilisateur, u.nom, u.prenom, u.email, u.telephone, u.numero, u.rue, u.code_postal, u.ville, u.station, SUM(cmd.prix) AS total_achats
FROM client_ c
JOIN utilisateur u ON c.id_utilisateur = u.id_utilisateur
LEFT JOIN commande cmd ON cmd.id_client = c.id_utilisateur
GROUP BY u.id_utilisateur
ORDER BY u.nom ASC, u.numero ASC;

SELECT u.id_utilisateur, u.nom, u.prenom, u.email, u.telephone, u.numero, u.rue, u.code_postal, u.ville, u.station, SUM(cmd.prix) AS total_achats
FROM client_ c
JOIN utilisateur u ON c.id_utilisateur = u.id_utilisateur
LEFT JOIN commande cmd ON cmd.id_client = c.id_utilisateur
GROUP BY u.id_utilisateur
ORDER BY u.nom DESC, u.numero ASC;

SELECT u.id_utilisateur, u.nom, u.prenom, u.email, u.telephone, u.numero, u.rue, u.code_postal, u.ville, u.station, SUM(cmd.prix) AS total_achats
FROM client_ c
JOIN utilisateur u ON c.id_utilisateur = u.id_utilisateur
LEFT JOIN commande cmd ON cmd.id_client = c.id_utilisateur
GROUP BY u.id_utilisateur
ORDER BY total_achats DESC;

#****************** MODULE CUISINIER *****************#


#****************** MODULE COMMANDES *****************#


#****************** MODULE STATISTIQUES *****************#
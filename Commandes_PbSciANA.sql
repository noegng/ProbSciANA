#****************** MODULE AUTRE *****************#

SELECT u.*,

    -- Statuts
    CASE WHEN cl.id_utilisateur IS NOT NULL THEN TRUE ELSE FALSE END AS estClient,
    CASE WHEN cu.id_utilisateur IS NOT NULL THEN TRUE ELSE FALSE END AS estCuisinier,
    CASE WHEN e.id_utilisateur IS NOT NULL THEN TRUE ELSE FALSE END AS estEntreprise,
    e.nom_referent,

    -- === Partie CLIENT ===

    -- ✅ Total des achats
    (SELECT COALESCE(SUM(c.prix), 0)
     FROM Commande c
     WHERE c.id_client = u.id_utilisateur) AS total_achats,

    -- ✅ Nombre total de commandes
    (SELECT COUNT(*)
     FROM Commande c
     WHERE c.id_client = u.id_utilisateur) AS nb_commandes_client,

    -- ✅ Montant moyen par commande
    (SELECT COALESCE(AVG(c.prix), 0)
     FROM Commande c
     WHERE c.id_client = u.id_utilisateur) AS moyenne_commande_client,

    -- ✅ Nombre de cuisiniers différents testés
    (SELECT COUNT(DISTINCT c.id_cuisinier)
     FROM Commande c
     WHERE c.id_client = u.id_utilisateur) AS nb_cuisiniers_differents,

    -- ✅ Cuisinier préféré
    (SELECT c.id_cuisinier
     FROM Commande c
     WHERE c.id_client = u.id_utilisateur
     GROUP BY c.id_cuisinier
     ORDER BY COUNT(*) DESC
     LIMIT 1) AS cuisinier_prefere,

    -- ✅ Type de plat préféré
    (SELECT p.type_
     FROM Commande c
     JOIN Livraison l ON l.id_commande = c.id_commande
     JOIN Requiert r ON r.id_livraison = l.id_livraison
     JOIN Plat p ON p.id_plat = r.id_plat
     WHERE c.id_client = u.id_utilisateur
     GROUP BY p.type_
     ORDER BY COUNT(*) DESC
     LIMIT 1) AS type_plat_prefere,

    -- ✅ Nationalité préférée
    (SELECT p.nationalite
     FROM Commande c
     JOIN Livraison l ON l.id_commande = c.id_commande
     JOIN Requiert r ON r.id_livraison = l.id_livraison
     JOIN Plat p ON p.id_plat = r.id_plat
     WHERE c.id_client = u.id_utilisateur
     GROUP BY p.nationalite
     ORDER BY COUNT(*) DESC
     LIMIT 1) AS nationalite_preferee,

    -- ✅ Régime préféré
    (SELECT p.regime
     FROM Commande c
     JOIN Livraison l ON l.id_commande = c.id_commande
     JOIN Requiert r ON r.id_livraison = l.id_livraison
     JOIN Plat p ON p.id_plat = r.id_plat
     WHERE c.id_client = u.id_utilisateur
     GROUP BY p.regime
     ORDER BY COUNT(*) DESC
     LIMIT 1) AS regime_prefere,

    -- ❗ Plat commandé le plus souvent
    (SELECT p.nom
     FROM Commande c
     JOIN Livraison l ON l.id_commande = c.id_commande
     JOIN Requiert r ON r.id_livraison = l.id_livraison
     JOIN Plat p ON p.id_plat = r.id_plat
     WHERE c.id_client = u.id_utilisateur
     GROUP BY p.nom
     ORDER BY COUNT(*) DESC
     LIMIT 1) AS plat_prefere,

    -- ✅ Nombre d’avis laissés
    (SELECT COUNT(*)
     FROM Avis a
     WHERE a.id_Client_ = u.id_utilisateur) AS nb_avis_laisses,

    -- ✅ Note moyenne donnée
    (SELECT COALESCE(AVG(a.note), 0)
     FROM Avis a
     WHERE a.id_Client_ = u.id_utilisateur) AS note_moyenne_donnee,

    -- === Partie CUISINIER ===

    -- ✅ Clients servis
    (SELECT COUNT(DISTINCT c.id_client)
     FROM Commande c
     WHERE c.id_cuisinier = u.id_utilisateur) AS nb_clients_servis,

    -- ✅ Nombre de plats différents cuisinés
    (SELECT COUNT(DISTINCT cu.id_plat)
     FROM Cuisine cu
     WHERE cu.id_cuisinier = u.id_utilisateur) AS nb_plats_differents,

    -- ✅ Plat le plus cuisiné
    (SELECT p.nom
     FROM Cuisine cu
     JOIN Plat p ON cu.id_plat = p.id_plat
     WHERE cu.id_cuisinier = u.id_utilisateur
     GROUP BY p.nom
     ORDER BY COUNT(*) DESC
     LIMIT 1) AS plat_plus_cuisine,

    -- ✅ Plat du jour actuel
    (SELECT p.nom
     FROM Cuisine cu
     JOIN Plat p ON cu.id_plat = p.id_plat
     WHERE cu.id_cuisinier = u.id_utilisateur AND cu.plat_du_jour = TRUE
     ORDER BY cu.date_cuisine DESC
     LIMIT 1) AS plat_du_jour,

    -- ✅ Type de plats majoritairement cuisinés
    (SELECT p.type_
     FROM Cuisine cu
     JOIN Plat p ON cu.id_plat = p.id_plat
     WHERE cu.id_cuisinier = u.id_utilisateur
     GROUP BY p.type_
     ORDER BY COUNT(*) DESC
     LIMIT 1) AS type_plat_majoritaire,

    -- ✅ Note moyenne reçue
    (SELECT COALESCE(AVG(a.note), 0)
     FROM Avis a
     WHERE a.id_cuisinier = u.id_utilisateur) AS note_moyenne_recue,

    -- ✅ Nombre d’avis reçus
    (SELECT COUNT(*)
     FROM Avis a
     WHERE a.id_cuisinier = u.id_utilisateur) AS nb_avis_recus,

    -- ✅ Montant total encaissé
    (SELECT COALESCE(SUM(c.prix), 0)
     FROM Commande c
     WHERE c.id_cuisinier = u.id_utilisateur) AS total_encaisse,

    -- ✅ Montant moyen par commande
    (SELECT COALESCE(AVG(c.prix), 0)
     FROM Commande c
     WHERE c.id_cuisinier = u.id_utilisateur) AS moyenne_encaissee,

    -- ✅ Nationalité la plus souvent cuisinée
    (SELECT p.nationalite
     FROM Cuisine cu
     JOIN Plat p ON cu.id_plat = p.id_plat
     WHERE cu.id_cuisinier = u.id_utilisateur
     GROUP BY p.nationalite
     ORDER BY COUNT(*) DESC
     LIMIT 1) AS nationalite_cuisinee,

    -- ✅ Régime le plus souvent cuisiné
    (SELECT p.regime
     FROM Cuisine cu
     JOIN Plat p ON cu.id_plat = p.id_plat
     WHERE cu.id_cuisinier = u.id_utilisateur
     GROUP BY p.regime
     ORDER BY COUNT(*) DESC
     LIMIT 1) AS regime_cuisine

FROM 
    Utilisateur u
LEFT JOIN Client_ cl ON cl.id_utilisateur = u.id_utilisateur
LEFT JOIN Cuisinier cu ON cu.id_utilisateur = u.id_utilisateur
LEFT JOIN Entreprise e ON e.id_utilisateur = u.id_utilisateur

WHERE 
    u.id_utilisateur = 2;

#****************** MODULE CLIENT *****************#
                    
SELECT u.*,
CASE WHEN cl.id_utilisateur IS NOT NULL THEN TRUE ELSE FALSE END AS estClient,
CASE WHEN cu.id_utilisateur IS NOT NULL THEN TRUE ELSE FALSE END AS estCuisinier,
CASE WHEN e.id_utilisateur IS NOT NULL THEN TRUE ELSE FALSE END AS estEntreprise,
e.nom_referent,
COALESCE(SUM(DISTINCT c.prix), 0) AS total_achats,
COUNT(DISTINCT c.id_commande) AS nb_commandes,
COALESCE(AVG(DISTINCT c.prix), 0) AS moyenne_commande,
COUNT(DISTINCT c.id_cuisinier) AS nb_cuisiniers_differents,
(
	SELECT c2.id_cuisinier
	FROM Commande c2
	WHERE c2.id_client = u.id_utilisateur
	GROUP BY c2.id_cuisinier
	ORDER BY COUNT(*) DESC
	LIMIT 1
) AS cuisinier_prefere,
(
	SELECT p.type_
	FROM Commande c3
	JOIN Livraison l ON l.id_commande = c3.id_commande
	JOIN Requiert r ON r.id_livraison = l.id_livraison
	JOIN Plat p ON p.id_plat = r.id_plat
	WHERE c3.id_client = u.id_utilisateur
	GROUP BY p.type_
	ORDER BY COUNT(*) DESC
	LIMIT 1
) AS type_plat_prefere,
(
	SELECT p.nationalite
	FROM Commande c4
	JOIN Livraison l ON l.id_commande = c4.id_commande
	JOIN Requiert r ON r.id_livraison = l.id_livraison
	JOIN Plat p ON p.id_plat = r.id_plat
	WHERE c4.id_client = u.id_utilisateur
	GROUP BY p.nationalite
	ORDER BY COUNT(*) DESC
	LIMIT 1
) AS nationalite_preferee,
(
	SELECT p.regime
	FROM Commande c5
	JOIN Livraison l ON l.id_commande = c5.id_commande
	JOIN Requiert r ON r.id_livraison = l.id_livraison
	JOIN Plat p ON p.id_plat = r.id_plat
	WHERE c5.id_client = u.id_utilisateur
	GROUP BY p.regime
	ORDER BY COUNT(*) DESC
	LIMIT 1
) AS regime_favori,
(
	SELECT p.nom
	FROM Commande c6
	JOIN Livraison l ON l.id_commande = c6.id_commande
	JOIN Requiert r ON r.id_livraison = l.id_livraison
	JOIN Plat p ON p.id_plat = r.id_plat
	WHERE c6.id_client = u.id_utilisateur
	GROUP BY p.nom
	ORDER BY COUNT(*) DESC
	LIMIT 1
) AS plat_prefere,
COUNT(DISTINCT a.id_avis) AS nb_avis,
COALESCE(AVG(a.note), 0) AS note_moyenne
FROM Utilisateur u
LEFT JOIN Client_ cl ON cl.id_utilisateur = u.id_utilisateur
LEFT JOIN Cuisinier cu ON cu.id_utilisateur = u.id_utilisateur
LEFT JOIN Entreprise e ON e.id_utilisateur = u.id_utilisateur
LEFT JOIN Commande c ON c.id_client = u.id_utilisateur
LEFT JOIN Avis a ON a.id_Client_ = u.id_utilisateur
GROUP BY u.id_utilisateur;

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
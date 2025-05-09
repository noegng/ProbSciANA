#****************** REQUETES *****************#
USE PbSciANA;

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
     WHERE a.id_Client = u.id_utilisateur) AS nb_avis_laisses,

    -- ✅ Note moyenne donnée
    (SELECT COALESCE(AVG(a.note), 0)
     FROM Avis a
     WHERE a.id_Client = u.id_utilisateur) AS note_moyenne_donnee,

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
FROM Utilisateur u
LEFT JOIN Client_ cl ON cl.id_utilisateur = u.id_utilisateur
LEFT JOIN Cuisinier cu ON cu.id_utilisateur = u.id_utilisateur
LEFT JOIN Entreprise e ON e.id_utilisateur = u.id_utilisateur;

Select * from Commande;
Select * from Client_;
Select * from Particulier;
Select * from Cuisinier;
Select * from avis;
Select * from plat;
Select * from Livraison;
Select * from Requiert;
Select * from Cuisine;
Select * from compose;
Select * from ingredient;


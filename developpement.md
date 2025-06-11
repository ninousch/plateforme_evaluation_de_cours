#  Développement – Plateforme d’Évaluation

Ce document sert à suivre l’évolution technique du projet, noter les choix faits, et planifier les étapes suivantes.

---

##  Journal de Développement

###  2025-06-01
- Initialisation du projet ASP.NET Core
- Configuration du projet Razor Pages
- Création du modèle `Utilisateur` avec rôles

###  2025-06-04
- Mise en place de l'API REST avec contrôleur `EvaluationsController`
- Implémentation des modèles `Cours`, `Professeur`, `Evaluation`
- Mise en place d’Entity Framework Core (BDD SQL Server)

###  2025-06-06
- Authentification par JWT
- Razor Page pour se connecter et s’inscrire
- Intégration frontend avec les endpoints API

---

##  Choix Techniques

- **Frontend** : Razor Pages (ASP.NET Core)
- **Backend** : ASP.NET Core Web API (RESTful)
- **BDD** : SQL Server + Entity Framework Core
- **Auth** : JWT Bearer Token
- **IDE** : Visual Studio 2022

---

##  Feuille de route

- [x] Authentification et sécurité
- [x] CRUD pour cours, professeurs, évaluations
- [ ] Interface d’évaluation avec étoiles et commentaires
- [ ] Génération de statistiques (tableaux, graphiques)
- [ ] Tableau de bord administrateur
- [ ] Export PDF ou CSV des résultats

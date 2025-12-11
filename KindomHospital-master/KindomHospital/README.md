# KindomHospital
<img src="https://media.senscritique.com/media/000006507220/300/kingdom_hospital.jpg" width="10%">
Ce dépôt contient l'application `KindomHospital` (.NET 9).

## Objectif

Ce fichier décrit l'organisation des répertoires et des fichiers principaux du projet.

## Architecture (réelle)

Le projet suit une séparation en couches  minimale : `Presentation`, `Application`, `Infrastructure` et `Domain`.

```
┌──────────────────────────────┐
│          Presentation        │  → ASP.NET Core Controllers, Blazor, etc.
└──────────────▲───────────────┘
               │ (calls)
┌──────────────┴───────────────┐
│         Application          │  → Services métiers, Handlers CQRS, DTO, interfaces
└──────────────▲───────────────┘
               │ (depends on abstractions only)
┌──────────────┴───────────────┐
│           Domain             │  → Entités, ValueObjects, règles métier pures
└──────────────▲───────────────┘
               │ (implemented by)
┌──────────────┴───────────────┐
│        Infrastructure        │  → EF Core, Repositories, Files, Email, APIs externes
└──────────────────────────────┘
```


- `Presentation/`
  - Contient l'interface d'exposition de l'application (API controllers, endpoints).
  - Exemple : `Presentation/Controllers/WeatherForecastController.cs`.
  - Rôle : recevoir les requêtes HTTP, valider les entrées, appeler les services de la couche `Application` et retourner les réponses.

- `Application/`
  - `Application/DTOs/` : objets de transfert (DTO) utilisés entre la présentation et les services.
  - `Application/Mappers/` : définitions d'interfaces ou classes de mapping (ex. Mapperly) pour convertir entre entités du `Domain` et DTOs.
  - `Application/Services/` : services d'application (use cases, orchestrations) qui contiennent la logique métier orientée cas d'utilisation et appellent le `Domain` pour les opérations métier.
  - Rôle : centraliser la logique d'application (cas d'utilisation), garder la `Presentation` légère et découpler l'implémentation du `Domain`.

- `Domain/`
  - `Domain/Entities/` : entités et objets de valeur représentant le modèle de domaine (ex. `WeatherForecast` si pertinent).
  - Rôle : contenir les entités et logique du domaine pur.

- `Infrastructure/`
  - `Infrastructure/Migrations/` : migrations de base de données liées au modèle de domaine (si vous utilisez EF Core ici).
  - `Infrastructure/Configurations/` : configurations du modèle (ex. `IEntityTypeConfiguration<T>` pour EF Core) et règles de mapping/domaine.
  - `Infrastructure/Repositories/` : implémentations concrètes des interfaces de dépôt (repositories) pour accéder aux données (ex. via EF Core). 
  - Rôle : contenir les règles métier, invariants...

## Intégration et responsabilités

- `Presentation` dépend de `Application` (appel de services, utilisation de DTOs).
- `Application` dépend de `Domain` (manipulation d'entités, règles métier).
- Les mappers de `Application/Mappers` convertissent entre `Domain` et `Application/DTOs`.
- Les configurations EF (dans `Domain/Configurations`) décrivent la persistance des entités si EF Core est utilisé.

## Où ajouter le code

- En cas d'ajout d'un nouveau cas d'utilisation :
  1. Créer les DTOs dans `Application/DTOs/`.
  2. Ajouter le service d'application correspondant dans `Application/Services/`.
  3. Ajouter l'entité (ou la mettre à jour) dans `Domain/Entities/`.
  4. Ajouter les mappings dans `Application/Mappers/`.
  5. Exposer l'endpoint dans `Presentation/Controllers/`.

## Configuration rapide

- Enregistrer les services d'application et les mappers dans `Program.cs` via DI.
- Si vous utilisez EF Core, vous pouvez ajouter le `DbContext` et les migrations et configurer la chaîne de connexion dans `Program.cs`.

## Commandes utiles

- `dotnet build` — compiler le(s) projet(s)
- `dotnet run --project KindomHospital/KindomHospital.csproj` — lancer l'application

---

## Packages NuGet nécessaires

Pour exploiter pleinement cette architecture, voici les principaux packages NuGet à installer?:

- `Microsoft.AspNetCore.OpenApi` : support OpenAPI/Swagger pour la documentation d'API
- `Microsoft.EntityFrameworkCore` : ORM Entity Framework Core (accès aux données)
- `Microsoft.EntityFrameworkCore.SqlServer` : provider SQL Server pour EF Core
- `Microsoft.EntityFrameworkCore.Design` : outils de design (migrations, scaffolding)
- `Microsoft.EntityFrameworkCore.Tools` : outils CLI/support de migration
- `Riok.Mapperly` : générateur de mappers (pour la couche Application)


---

## N'oubliez pas de :

- Ajouter votre connection string dans le fichier Appsettings
- Pour la commande Add-Migration ajouter le paramètre : -OutputDir Infrastructure/Migrations
- Configurer le pipeline HTTP avec votre contexte, Mapper, Repositories et Services
- Supprimer les fichiers inutiles (WeatherForecast par exemple)
- Adapter le README à votre projet)
- Ajouter un fichier .gitignore si nécessaire

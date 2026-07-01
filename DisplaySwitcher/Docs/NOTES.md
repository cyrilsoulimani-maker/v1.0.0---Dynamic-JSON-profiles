## Problèmes de compilation Visual Studio

Si la compilation devient anormalement lente ou si des erreurs étranges apparaissent
(ex : "System.Object est introuvable", restauration NuGet impossible, build bloqué) :

1. Fermer Visual Studio.
2. Supprimer :
   - .vs
   - bin
   - obj
3. Rouvrir la solution (.sln).
4. Exécuter `dotnet restore`.
5. Régénérer la solution.
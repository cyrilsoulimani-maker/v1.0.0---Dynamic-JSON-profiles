# DisplaySwitcher

Version du projet : Pré-v1.0
Framework : .NET 10 / WPF
IDE : Visual Studio 2026 Community

---

# Vision du projet

DisplaySwitcher est une application Windows permettant de changer rapidement de configuration d'affichage.

L'objectif est de remplacer un script PowerShell par une véritable application moderne, agréable à utiliser et capable de gérer :

- des profils d'affichage
- des résolutions natives
- des résolutions personnalisées
- plusieurs écrans
- une interface moderne
- un fonctionnement dans le Tray Windows

Le projet privilégie la simplicité d'utilisation tout en exploitant les API natives de Windows lorsque cela apporte une réelle valeur.

---

# Etat actuel du projet

## Fonctionnel

### Dashboard

✔ Carte "Etat actuel"

Affiche :

- nom commercial du moniteur
- résolution actuelle
- fréquence actuelle

Les données proviennent maintenant de DisplayConfig.

---

✔ Carte "Profil à appliquer"

- sélection d'un profil
- application immédiate

---

✔ Gestion des profils

- création
- modification
- suppression
- sauvegarde JSON

---

✔ Tray

- ouverture de l'application
- application d'un profil
- accès à la gestion des profils

---

# Architecture

Le projet est organisé en couches.

```
UI
│
├── MainWindow
├── ProfileManagerWindow
│
Services
│
├── DisplayConfigService
├── DisplayService
├── ProfileService
├── TrayIconService
├── MonitorIdentificationService
│
Native
│
├── NativeMethods
├── DisplayConfigStructures
│
Models
```

Règle fondamentale :

La UI ne parle jamais directement à Win32.

Toute communication avec Windows passe par les Services.

---

# DisplayConfig

DisplayConfig devient progressivement le moteur principal de l'application.

Aujourd'hui :

✔ QueryDisplayConfig

✔ FriendlyName

✔ Width

✔ Height

✔ RefreshRate

Le Dashboard n'utilise plus DisplayService pour ces informations.

---

DisplayConfigService expose actuellement :

```
GetCurrentConfiguration()

GetCurrentDisplayState()
```

CurrentDisplayState contient :

```
FriendlyName

Width

Height

RefreshRate
```

---

# DisplayService

DisplayService est encore présent.

Son rôle diminue progressivement.

L'objectif est qu'il ne reste utilisé que pour appliquer une résolution tant que DisplayConfig ne remplace pas complètement cette partie.

---

# MonitorIdentificationService

Responsable de l'identification physique des écrans.

Permet le rapprochement WMI ↔ DisplayConfig.

Ne pas mélanger ses responsabilités avec DisplayConfigService.

---

# Native

Le dossier Native contient uniquement l'interop Win32.

Aucune logique métier.

Aucune UI.

Uniquement :

- structures
- P/Invoke
- enums

---

# Design System

Toutes les fenêtres utilisent un Design System commun.

Aujourd'hui :

✔ DsCard

✔ DsButton

✔ DsPrimaryButton

✔ DsHeaderButton

✔ TextBox

✔ CheckBox

La ComboBox est en cours de refonte.

Objectif :

Créer un véritable DsComboBox réutilisable dans toute l'application.

---

# JSON

Les profils sont enregistrés dans :

profiles.json

Chaque profil contient notamment :

- nom
- écran
- largeur
- hauteur
- fréquence
- création éventuelle d'une résolution personnalisée

---

# Priorités de développement

## PRIORITE 1

Résolutions personnalisées

Objectif :

Pouvoir créer un profil contenant une résolution non native.

Lors de l'enregistrement :

- création automatique de la résolution personnalisée dans Windows
- sauvegarde du profil

C'est actuellement la fonctionnalité principale restante.

---

## PRIORITE 2

Finaliser le Design System

- ComboBox
- petits problèmes visuels

Exemple :

Gestion des profils :

les ComboBox utilisent encore le template WPF.

---

## PRIORITE 3

Compléter DisplayConfig

Ajouter :

- type de connexion (DisplayPort / HDMI)
- fabricant
- HDR
- VRR

Ces informations alimenteront la carte Etat actuel.

---

## PRIORITE 4

Paramètres

Ajouter une fenêtre Paramètres.

Prévoir :

- lancement au démarrage
- démarrage réduit dans le Tray

---

## PRIORITE 5

Raccourcis clavier

Pouvoir associer un raccourci à un profil.

Exemple :

CTRL+ALT+F1

↓

Profil Gaming

---

## PRIORITE 6

Notifications

Après application d'un profil :

Afficher une notification Windows moderne reprenant le Design DisplaySwitcher.

---

# Philosophie du projet

Toujours privilégier :

une fonctionnalité visible

plutôt qu'un long refactoring invisible.

Les refactorings sont acceptés uniquement lorsqu'ils servent une fonctionnalité.

---

# Méthode de travail

Le projet est développé par petites fonctionnalités.

Chaque séance doit produire un résultat visible.

Eviter les longues phases de préparation.

Privilégier :

Objectif

↓

Code

↓

Compilation

↓

Validation

↓

Commit

Le développeur préfère avancer rapidement sur des fonctionnalités concrètes plutôt que passer beaucoup de temps sur la théorie.

Les réponses doivent donc être orientées :

- code
- architecture utile
- résultats visibles

et éviter de "tourner autour du pot".

---

# Dernier état connu

Dernière fonctionnalité terminée :

DisplayConfig alimente maintenant le Dashboard avec :

✔ Nom commercial

✔ Résolution

✔ Fréquence

Prochaine étape :

1. Corriger définitivement le Design System des ComboBox.

2. Implémenter les résolutions personnalisées (fonctionnalité prioritaire).

3. Reprendre l'enrichissement DisplayConfig.

# Règle d'or

Avant chaque nouvelle fonctionnalité, toujours se poser la question :

"Est-ce que cette fonctionnalité rend DisplaySwitcher plus utilisable aujourd'hui ?"

Si la réponse est oui,
elle est prioritaire.

Sinon,
elle peut attendre.

DisplaySwitcher est développé comme un produit, pas comme un exercice technique.

# DisplaySwitcher - Release Notes

---

# v0.1.0 - Core Engine

## Nouveautés

- Création du projet WPF
- Détection des écrans Windows
- Changement de résolution
- Changement de fréquence
- Application des profils d'affichage
- Intégration des API Win32
- Première interface utilisateur

## Architecture

- Mise en place des premiers Services
- Modèles de base

---

# v0.2.0 - Profiles

## Nouveautés

- Gestion complète des profils
- Sauvegarde des profils en JSON
- Chargement automatique des profils
- Sélection automatique du profil actif
- Icône dans la zone de notification (System Tray)
- Gestionnaire de profils

## Architecture

- ObservableCollection
- Persistance JSON
- Refactoring des Services
- Nettoyage du MainWindow

---

# v0.3.0 - UI Foundation

## Nouveautés

- Refonte complète du Dashboard
- Nouvelle hiérarchie visuelle
- Navigation simplifiée
- Bouton "Gérer les profils" depuis l'écran principal
- Card "État actuel"
- Affichage du profil actif

## Design System

- Palette DisplaySwitcher
- Thème sombre
- Cards DisplaySwitcher
- DsButton
- DsPrimaryButton
- DsRadioButton
- Hover et interactions
- Uniformisation de l'interface

## Documentation

- PROJECT.md
- BACKLOG.md
- UI_GUIDELINES.md
- DEVELOPER_GUIDE.md
- SESSION_PROMPT.md
- ARCHITECTURE.md

---

# v0.4.0 - Hardware Awareness

## État

🚧 En cours

## Objectifs

- Identification des moniteurs
- Friendly Name
- Fabricant
- Modèle
- Dashboard enrichi

## Préparation

- MonitorIdentity
- MonitorIdentificationService
- Extension de DisplayDeviceInfo
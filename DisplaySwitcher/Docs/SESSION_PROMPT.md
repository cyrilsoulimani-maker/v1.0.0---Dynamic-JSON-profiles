# DisplaySwitcher - Prompt de reprise

Nous développons DisplaySwitcher en C# / WPF sous Visual Studio 2022.

Le projet suit une philosophie de développement incrémental :

- une fonctionnalité ;
- une compilation ;
- un test ;
- un commit Git.

Ne jamais proposer une grosse réécriture si une évolution incrémentale est possible.

## Architecture

- Models
- Services
- Themes
- Docs

## Documentation

Consulter en priorité :

- PROJECT.md
- UI_GUIDELINES.md
- NOTES.md

## État actuel

Version : v0.2.0-alpha (prévue)

Fonctionnel :

- Gestion des profils
- Détection multi-écrans
- EnumDisplaySettings
- Chargement des modes par écran
- Sélection automatique d'un mode
- Remplissage Width/Height/Frequency
- DisplayProfile utilise INotifyPropertyChanged

En cours :

- Intégration du système de thèmes WPF
- Colors.xaml
- Controls.xaml

Objectif actuel :

Transformer progressivement le Profile Manager vers la maquette V2 validée.

Toujours privilégier :

- petits commits ;
- architecture propre ;
- code lisible ;
- explications pédagogiques.
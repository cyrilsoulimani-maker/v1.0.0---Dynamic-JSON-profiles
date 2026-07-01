# DisplaySwitcher - UI Guidelines

## Philosophie

DisplaySwitcher est un outil de productivité.

L'interface doit être :

- sobre ;
- rapide à comprendre ;
- confortable lors d'une utilisation quotidienne.

Le design privilégie la lisibilité plutôt que les effets visuels.

---

# Palette

## Fond principal

#111111

## Fond des panneaux

#1A1A1A

## Couleur d'accent

#5FBF3A

## Texte principal

#F2F2F2

## Texte secondaire

#BDBDBD

## Bordures

#2A2A2A

---

# Boutons

Le bouton principal est toujours :

- vert
- rempli

Exemple :

Enregistrer

Les actions secondaires sont grises :

- Ajouter
- Supprimer
- Fermer

---

# Contrôles

Les contrôles utilisent une bordure discrète.

La bordure devient verte uniquement lorsque le contrôle possède le focus.

Le vert représente une interaction, pas une décoration.

---

# Icônes

Le logo officiel de DisplaySwitcher est :

DS

Les profils peuvent utiliser une icône représentative :

🎮 Gaming

🎥 Streaming

💼 Bureau

🦀 Rust

...

---

# Gestionnaire de profils

Le parcours utilisateur est :

Nom

↓

Écran

↓

Mode standard

ou

Mode personnalisé

↓

Enregistrer

Le mode standard est privilégié.

Le mode personnalisé est réservé aux cas particuliers.

DisplaySwitcher Design System v1
1. Philosophie
Objectif

DisplaySwitcher est un outil destiné aux utilisateurs exigeants (gaming, hardware, multi-écrans).

L'interface doit être :

sobre ;
moderne ;
efficace ;
lisible ;
orientée productivité.
Principes
Chaque élément de l'interface doit avoir une utilité.
Privilégier la rapidité d'utilisation à l'effet visuel.
Les fonctionnalités avancées doivent rester accessibles sans surcharger les usages simples.
L'utilisateur doit pouvoir comprendre l'état de sa configuration d'un seul coup d'œil.
2. Palette de couleurs
Élément	Couleur
Background	#111111
Panel / Card	#1A1A1A
Accent	#5FBF3A
Texte principal	#F2F2F2
Texte secondaire	#BDBDBD
Bordures	#2A2A2A

Les ressources XAML utilisent la convention :

Ds.Background
Ds.Panel
Ds.Accent
Ds.Text
Ds.TextSecondary
Ds.Border
3. Typographie

Police :

Segoe UI

Hiérarchie :

Titre de fenêtre
Titre de Card
Label
Texte secondaire

Éviter les tailles de police arbitraires.

4. Espacements

Utiliser uniquement les espacements standards :

Nom	Valeur
Small	8 px
Medium	16 px
Large	24 px

Ne pas utiliser des marges "au hasard" (13, 19, 27...).

5. Coins arrondis

Valeurs de référence :

Élément	Rayon
Card	10 px
Bouton	8 px
TextBox	6 px
6. Les composants de l'interface

Le vocabulaire officiel du projet.

Pane

Grande zone fonctionnelle.

Exemple :

Liste des profils
Card

Bloc regroupant une fonctionnalité.

Exemple :

Profil
Affichage
Options
Toolbar

Zone contenant les actions principales.

Dialog

Fenêtres secondaires.

Badge (prévu)

Petits indicateurs visuels.

Exemple :

HDR

360 Hz

G-SYNC

VRR
7. Hiérarchie des boutons
Primaire

Couleur Accent.

Exemple :

Enregistrer
Secondaire

Couleur neutre.

Exemple :

Nouveau profil
Action dangereuse

Rouge.

Exemple :

Supprimer
8. Cartes prévues

Architecture cible de la fenêtre principale :

Profil

Affichage

GPU

Windows

Applications

Options avancées

Toutes les cartes doivent utiliser le même style graphique.

9. Animations

Très discrètes.

Maximum :

Hover
Fade
100 ms

Aucune animation décorative.

10. Icônes

Toutes les icônes devront provenir d'un seul pack.

Ne jamais mélanger plusieurs styles.

À définir ultérieurement :

Fluent
Segoe Fluent
Material
11. Design Tokens (évolution future)

Le système de thème devra progressivement intégrer :

Ds.CardPadding

Ds.CardRadius

Ds.Spacing.Small

Ds.Spacing.Medium

Ds.Spacing.Large

Ds.Icon.Small

Ds.Icon.Medium

L'objectif est de ne plus coder de valeurs "en dur" dans les fenêtres.

12. Vision UX

DisplaySwitcher ne cherche pas à être un logiciel démonstratif.

Il doit donner le sentiment de :

contrôler son environnement ;
comprendre immédiatement l'état de son affichage ;
accéder rapidement aux réglages importants.

L'efficacité prime toujours sur les effets graphiques.

13. Vision long terme

L'interface doit être conçue dès aujourd'hui pour accueillir de nouvelles cartes sans être repensée.

Évolutions envisagées :

GPU
HDR
G-SYNC
VRR
Profils par application
Diagnostic du profil
État global de la configuration

14. Ce que DisplaySwitcher n'est pas

C'est une règle de conception qui aide à éviter les dérives du projet.

Par exemple :

❌ Pas une usine à gaz.
❌ Pas un panneau de configuration Windows alternatif.
❌ Pas un logiciel de benchmarking.
❌ Pas un outil d'overclocking GPU.

En revanche :

✅ Un gestionnaire de profils d'affichage.
✅ Un outil d'optimisation de l'environnement d'affichage.
✅ Une interface simple pour des réglages avancés.
✅ Un logiciel pensé pour les utilisateurs exigeants.
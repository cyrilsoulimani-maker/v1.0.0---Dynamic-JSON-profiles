# 📘 DisplaySwitcher — État du projet (Juin 2026)

## Objectif

DisplaySwitcher est une application WPF destinée à **un usage personnel**.

Ses objectifs sont :

* Changer rapidement de résolution et de fréquence d'affichage.
* Gérer plusieurs profils d'affichage.
* Remplacer les scripts PowerShell initiaux.
* Servir de support d'apprentissage du développement C# / WPF.

Le projet pourra éventuellement devenir open source, mais **ce n'est pas un objectif prioritaire**.

---

# Philosophie du projet

Nous avons adopté deux règles.

## 1. Daily Driver

Chaque fonctionnalité doit améliorer l'utilisation quotidienne.

On ne développe pas des fonctionnalités "parce que c'est possible".

On les développe parce qu'elles apportent un vrai confort.

---

## 2. Simplicité

Nous préférons :

* une architecture propre,
* du code lisible,
* peu de classes,

plutôt qu'une architecture compliquée.

On crée un nouveau service uniquement lorsqu'un besoin réel apparaît.

---

# Technologies

* C#
* .NET 10
* WPF
* JSON
* Git

---

# Architecture actuelle

```text
DisplaySwitcher
│
├── Models
│   └── DisplayProfile.cs
│
├── Services
│   ├── DisplayService.cs
│   ├── ProfileService.cs
│   └── TrayIconService.cs
│
├── Windows
│   ├── MainWindow.xaml
│   └── ProfileManagerWindow.xaml
│
├── Resources
│   └── DisplaySwitcher.ico
│
├── Data
│   └── profiles.json
│
└── App.xaml
```

---

# DisplayProfile

Contient actuellement :

```csharp
Name
Width
Height
Frequency
IsSelected
CreateCustomResolution
```

---

# Fonctionnalités terminées

## MainWindow

* Affichage des profils.
* Profil sélectionné.
* Application d'une résolution.
* Détection du profil actif.
* Interface redimensionnable.
* Bouton Appliquer.

---

## Tray

* Icône.
* Menu dynamique.
* Profil actif coché.
* Changement de profil.
* Ouvrir DisplaySwitcher.
* Gérer les profils.
* Quitter.

---

## Gestionnaire de profils

Fonctionnel.

Il permet :

* afficher les profils ;
* sélectionner un profil ;
* modifier un profil ;
* ajouter un profil ;
* supprimer un profil ;
* sauvegarder dans le JSON.

Le tray est automatiquement rafraîchi.

---

## Validation

Les champs numériques acceptent uniquement des chiffres.

---

# Décisions d'architecture

## ObservableCollection

Toute l'interface utilise :

```csharp
ObservableCollection<DisplayProfile>
```

Le JSON reste basé sur :

```csharp
List<DisplayProfile>
```

afin que `ProfileService` reste indépendant de WPF.

---

## Communication

Les fenêtres communiquent via des événements.

Jamais directement.

Exemple :

```text
TrayIconService

↓

ProfileRequested

↓

MainWindow
```

Même principe pour :

```text
ProfileManagerWindow

↓

ProfilesSaved

↓

MainWindow
```

---

# Ce qui reste à améliorer

Gestionnaire :

* sélection automatique après suppression (fait) ;
* validation du collage ;
* duplication d'un profil ;
* Ctrl+S ;
* annulation des modifications.

Ces améliorations sont secondaires.

---

# Priorité actuelle

Créer des résolutions personnalisées Windows.

Nous avons déjà ajouté dans le modèle :

```csharp
CreateCustomResolution
```

et dans l'interface :

```text
☑ Créer aussi cette résolution personnalisée dans Windows
```

La fonctionnalité n'est pas encore implémentée.

---

# Fonctionnalités futures

Ordre de priorité :

1. Résolutions personnalisées.
2. Démarrage avec Windows.
3. Bouton "Gérer les profils" dans la fenêtre principale.
4. Détection automatique d'applications (Rust, etc.).
5. Multi-écrans.
6. Raccourcis clavier.
7. Paramètres.

---

# Notre façon de travailler

Nous travaillons par petites étapes.

À chaque étape :

* l'application compile ;
* elle reste fonctionnelle ;
* un commit Git peut être réalisé.

Nous privilégions toujours les explications avant le code.

---

## Ce que j'ajouterais à ce document

Je proposerais même de créer un fichier **`PROJECT.md`** à la racine du dépôt Git.

Pas pour faire de la documentation "pour GitHub", mais pour **nous**.

Ainsi, si dans trois mois tu reviens sur le projet, ou si nous reprenons le développement dans une nouvelle conversation, nous aurons immédiatement un point de référence fiable.

---

Je dois te dire une dernière chose.

Depuis le début, je t'ai accompagné comme si nous construisions une application. Mais aujourd'hui, j'ai l'impression que nous avons surtout construit **une manière de travailler ensemble** : des étapes courtes, une application toujours compilable, des choix expliqués, des refactorings uniquement quand ils apportent une vraie valeur.

J'aimerais conserver exactement cette méthode pour la suite, parce que je trouve qu'elle correspond très bien à ton objectif : **apprendre en construisant un outil que tu utilises réellement**. C'est, à mon avis, la meilleure façon de progresser sans perdre le plaisir de développer.


# Principes de développement

- L'application doit toujours compiler après chaque étape.
- Chaque fonctionnalité doit pouvoir être validée avant de passer à la suivante.
- Les explications précèdent toujours le code.
- Les refactorings ne sont réalisés que s'ils apportent une vraie valeur.
- Le projet est destiné en priorité à un usage personnel ("Daily Driver").
- Les fonctionnalités sont développées en fonction de leur utilité réelle.

# Interface utilisateur (V2)

Le Profile Manager suit la maquette V2 validée.

Principes :

- Interface sombre.
- Accent vert.
- Le mode standard est privilégié.
- Le mode personnalisé est réservé aux résolutions non disponibles.
- Les écrans sont sélectionnés avant les modes.

## Vision à long terme

DisplaySwitcher n'est pas uniquement un changeur de résolution.

L'objectif est de devenir un gestionnaire de profils d'affichage et d'environnement permettant de restaurer automatiquement une configuration optimale selon le contexte d'utilisation.

Les profils pourront, à terme, inclure :

- paramètres d'affichage ;
- paramètres GPU (selon les API disponibles) ;
- paramètres Windows ;
- automatisations ;
- lancement d'applications.


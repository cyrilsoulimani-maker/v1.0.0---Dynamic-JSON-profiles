DisplaySwitcher

↓

Architecture générale

↓

Services

↓

Models

↓

UI

↓

Flux des données

↓

Vision Hardware

# Hardware Layer

Responsabilité :
Décrire le matériel d'affichage présent sur la machine.

## DisplayService
Responsable des actions sur l'affichage :
- lecture du mode actuel ;
- changement de résolution ;
- changement de fréquence.

## DisplayEnumerationService
Responsable de l'inventaire des affichages Windows :
- liste des écrans ;
- résolution courante ;
- fréquence ;
- écran principal.

## MonitorIdentificationService
Responsable de l'identification des moniteurs physiques :
- nom convivial ;
- fabricant ;
- modèle ;
- numéro de série (à terme).

## Principes de développement

### 🧱 Une brique à la fois
Une seule responsabilité par étape. Chaque étape doit compiler et être testée avant de passer à la suivante.

### 💡 D'abord l'idée, ensuite le code
Comprendre le problème avant de chercher à l'implémenter.

### 🧪 On expérimente ailleurs, on intègre ici
Les expérimentations sont réalisées dans un projet Sandbox. Le projet principal reste toujours stable.

### 💾 Une brique stable = un commit
Chaque étape fonctionnelle est sécurisée par un commit Git.
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
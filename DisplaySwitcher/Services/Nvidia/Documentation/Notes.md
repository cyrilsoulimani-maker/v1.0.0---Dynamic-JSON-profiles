# Notes NVAPI

## NV_GPU_DISPLAYIDS

- Utiliser la version 3.
- Une version incorrecte retourne NVAPI_INCOMPATIBLE_STRUCT_VERSION.

## Flags

Bit 0 : Dynamic
Bit 1 : MultiStreamRootNode
Bit 2 : Active
Bit 3 : Cluster
Bit 4 : OSVisible
Bit 5 : WFD
Bit 6 : Connected

## Bonnes pratiques

Toujours partir des fichiers du SDK officiel (`nvapi.h`) plutôt que d'exemples trouvés sur Internet.
namespace DisplaySwitcher.Services.Nvidia;

public enum NvApiStatus
{
    Ok = 0,

    // Valeur interne utilisée lorsque la fonction NVAPI
    // n'a pas pu être récupérée.
    NotAvailable = -1
}
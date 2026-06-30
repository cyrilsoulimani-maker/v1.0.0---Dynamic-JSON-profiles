namespace DisplaySwitcher.Services.Nvidia.Interop;

public enum NvApiStatus
{
    Ok = 0,

    Error = -1,

    // Valeur interne utilisée lorsque la fonction NVAPI
    // n'a pas pu être récupérée.
    NotAvailable = -1000,

    InvalidArgument = -5,
    NvidiaDeviceNotFound = -6,
    IncompatibleStructVersion = -9
}
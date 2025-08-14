// Multilingual validation error messages for SUMISAN Wash Management System
// Supports English (default) and Spanish
// Usage: ValidationErrorMessages.SetLanguage(Language.Spanish);
//        throw new ValidationException(ValidationErrorMessages.User.NotFound(userId));

using System.Collections.Generic;

namespace Controlmat.Application.Common.Constants
{
    public enum Language
    {
        English,
        Spanish
    }

    public static class ValidationErrorMessages
    {
        private static Language _currentLanguage = Language.English;

        public static void SetLanguage(Language language)
        {
            _currentLanguage = language;
        }

        public static Language GetCurrentLanguage() => _currentLanguage;

        private static string GetMessage(Dictionary<Language, string> messages)
        {
            return messages.TryGetValue(_currentLanguage, out var message) ? message : messages[Language.English];
        }

        /// <summary>
        /// User-related validation errors
        /// </summary>
        public static class User
        {
            public static string NotFound(int userId) => GetMessage(new Dictionary<Language, string>
            {
                [Language.English] = $"User with ID {userId} does not exist in the system",
                [Language.Spanish] = $"El usuario con ID {userId} no existe en el sistema"
            });

            public static string StartUserNotFound(int userId) => GetMessage(new Dictionary<Language, string>
            {
                [Language.English] = $"Start operator with ID {userId} is not registered",
                [Language.Spanish] = $"El operario de inicio con ID {userId} no está registrado"
            });

            public static string EndUserNotFound(int userId) => GetMessage(new Dictionary<Language, string>
            {
                [Language.English] = $"End operator with ID {userId} is not registered",
                [Language.Spanish] = $"El operario de finalización con ID {userId} no está registrado"
            });

            public static string SameUserStartEnd => GetMessage(new Dictionary<Language, string>
            {
                [Language.English] = "Start and end operators must be different",
                [Language.Spanish] = "El operario de inicio y finalización deben ser diferentes"
            });

            public static string InvalidUserRange(int userId) => GetMessage(new Dictionary<Language, string>
            {
                [Language.English] = $"Invalid user ID: {userId}. Must be a positive integer",
                [Language.Spanish] = $"ID de usuario inválido: {userId}. Debe ser un número entero positivo"
            });
        }

        /// <summary>
        /// Machine-related validation errors
        /// </summary>
        public static class Machine
        {
            public static string NotFound(int machineId) => GetMessage(new Dictionary<Language, string>
            {
                [Language.English] = $"Machine with ID {machineId} does not exist",
                [Language.Spanish] = $"La máquina con ID {machineId} no existe"
            });

            public static string InvalidRange(int machineId) => GetMessage(new Dictionary<Language, string>
            {
                [Language.English] = $"Invalid machine ID: {machineId}. Must be between 1 and 4",
                [Language.Spanish] = $"ID de máquina inválido: {machineId}. Debe estar entre 1 y 4"
            });

            public static string AlreadyInUse(int machineId) => GetMessage(new Dictionary<Language, string>
            {
                [Language.English] = $"Machine {machineId} is already in use by another wash cycle",
                [Language.Spanish] = $"La máquina {machineId} ya está en uso por otro ciclo de lavado"
            });

            public static string NotAvailable(int machineId) => GetMessage(new Dictionary<Language, string>
            {
                [Language.English] = $"Machine {machineId} is not available to start a new wash",
                [Language.Spanish] = $"La máquina {machineId} no está disponible para iniciar un nuevo lavado"
            });

            public static string Required => GetMessage(new Dictionary<Language, string>
            {
                [Language.English] = "Machine ID is required",
                [Language.Spanish] = "El ID de la máquina es obligatorio"
            });
        }

        /// <summary>
        /// Washing cycle validation errors
        /// </summary>
        public static class Washing
        {
            public static string NotFound(long washingId) => GetMessage(new Dictionary<Language, string>
            {
                [Language.English] = $"Wash cycle with ID {washingId} does not exist",
                [Language.Spanish] = $"El ciclo de lavado con ID {washingId} no existe"
            });

            public static string InvalidIdFormat(long washingId) => GetMessage(new Dictionary<Language, string>
            {
                [Language.English] = $"Invalid washing ID format: {washingId}. Must follow YYMMDDXX pattern (e.g., 25080101)",
                [Language.Spanish] = $"Formato de ID de lavado inválido: {washingId}. Debe seguir el patrón YYMMDDXX (ej: 25080101)"
            });

            public static string AlreadyFinished(long washingId) => GetMessage(new Dictionary<Language, string>
            {
                [Language.English] = $"Wash cycle {washingId} is already finished and cannot be modified",
                [Language.Spanish] = $"El ciclo de lavado {washingId} ya está finalizado y no puede modificarse"
            });

            public static string NotInProgress(long washingId) => GetMessage(new Dictionary<Language, string>
            {
                [Language.English] = $"Wash cycle {washingId} is not in progress",
                [Language.Spanish] = $"El ciclo de lavado {washingId} no está en progreso"
            });

            public static string MaxActiveWashesReached => GetMessage(new Dictionary<Language, string>
            {
                [Language.English] = "Cannot start more wash cycles. Maximum 2 active washes allowed simultaneously",
                [Language.Spanish] = "No se pueden iniciar más ciclos de lavado. Máximo 2 lavados activos permitidos simultáneamente"
            });

            public static string CannotModifyFinished(long washingId) => GetMessage(new Dictionary<Language, string>
            {
                [Language.English] = $"Cannot modify finished wash cycle {washingId}",
                [Language.Spanish] = $"No se puede modificar el ciclo de lavado finalizado {washingId}"
            });

            public static string MustHavePhotosToFinish(long washingId) => GetMessage(new Dictionary<Language, string>
            {
                [Language.English] = $"Cannot finish wash cycle {washingId}: at least one photo is required",
                [Language.Spanish] = $"No se puede finalizar el ciclo de lavado {washingId}: se requiere al menos una foto"
            });

            public static string MustHaveProtsToStart => GetMessage(new Dictionary<Language, string>
            {
                [Language.English] = "Must add at least one PROT instrument set to start the wash cycle",
                [Language.Spanish] = "Debe agregar al menos un conjunto de instrumentos PROT para iniciar el ciclo de lavado"
            });

            public static string InvalidStatusTransition(char currentStatus, char targetStatus) => GetMessage(new Dictionary<Language, string>
            {
                [Language.English] = $"Invalid status transition from '{currentStatus}' to '{targetStatus}'. Only 'P' (In Progress) to 'F' (Finished) is allowed",
                [Language.Spanish] = $"Transición de estado inválida de '{currentStatus}' a '{targetStatus}'. Solo se permite de 'P' (En Progreso) a 'F' (Finalizado)"
            });

            public static string Required => GetMessage(new Dictionary<Language, string>
            {
                [Language.English] = "Washing ID is required",
                [Language.Spanish] = "El ID del lavado es obligatorio"
            });
        }

        /// <summary>
        /// PROT (instrument set) validation errors
        /// </summary>
        public static class Prot
        {
            public static string InvalidProtIdFormat(string protId) => GetMessage(new Dictionary<Language, string>
            {
                [Language.English] = $"Invalid ProtId format: '{protId}'. Must follow PROTXXX pattern (e.g., PROT001)",
                [Language.Spanish] = $"Formato de ProtId inválido: '{protId}'. Debe seguir el patrón PROTXXX (ej: PROT001)"
            });

            public static string InvalidBatchNumberFormat(string batchNumber) => GetMessage(new Dictionary<Language, string>
            {
                [Language.English] = $"Invalid BatchNumber format: '{batchNumber}'. Must follow NLXX pattern (e.g., NL01)",
                [Language.Spanish] = $"Formato de BatchNumber inválido: '{batchNumber}'. Debe seguir el patrón NLXX (ej: NL01)"
            });

            public static string InvalidBagNumberFormat(string bagNumber) => GetMessage(new Dictionary<Language, string>
            {
                [Language.English] = $"Invalid BagNumber format: '{bagNumber}'. Must follow XX/XX pattern (e.g., 01/02)",
                [Language.Spanish] = $"Formato de BagNumber inválido: '{bagNumber}'. Debe seguir el patrón XX/XX (ej: 01/02)"
            });

            public static string DuplicateProtInRequest => GetMessage(new Dictionary<Language, string>
            {
                [Language.English] = "Cannot add duplicate PROT entries in the same request",
                [Language.Spanish] = "No se pueden agregar entradas PROT duplicadas en la misma solicitud"
            });

            public static string DuplicateProtInWash(string protId, string batchNumber, string bagNumber) => GetMessage(new Dictionary<Language, string>
            {
                [Language.English] = $"PROT '{protId}' with batch '{batchNumber}' and bag '{bagNumber}' already exists in this wash cycle",
                [Language.Spanish] = $"El PROT '{protId}' con lote '{batchNumber}' y bolsa '{bagNumber}' ya existe en este ciclo de lavado"
            });

            public static string ProtIdRequired => GetMessage(new Dictionary<Language, string>
            {
                [Language.English] = "ProtId is required",
                [Language.Spanish] = "ProtId es obligatorio"
            });

            public static string BatchNumberRequired => GetMessage(new Dictionary<Language, string>
            {
                [Language.English] = "BatchNumber is required",
                [Language.Spanish] = "BatchNumber es obligatorio"
            });

            public static string BagNumberRequired => GetMessage(new Dictionary<Language, string>
            {
                [Language.English] = "BagNumber is required",
                [Language.Spanish] = "BagNumber es obligatorio"
            });

            public static string EmptyProtList => GetMessage(new Dictionary<Language, string>
            {
                [Language.English] = "PROT entries list cannot be empty",
                [Language.Spanish] = "La lista de entradas PROT no puede estar vacía"
            });
        }

        /// <summary>
        /// Photo upload and management validation errors
        /// </summary>
        public static class Photo
        {
            public static string NotFound(int photoId) => GetMessage(new Dictionary<Language, string>
            {
                [Language.English] = $"Photo with ID {photoId} does not exist",
                [Language.Spanish] = $"La foto con ID {photoId} no existe"
            });

            public static string FileNotFound(string fileName) => GetMessage(new Dictionary<Language, string>
            {
                [Language.English] = $"Photo file '{fileName}' not found in the file system",
                [Language.Spanish] = $"El archivo de foto '{fileName}' no se encuentra en el sistema de archivos"
            });

            public static string MaxPhotosReached(long washingId, int maxPhotos = 99) => GetMessage(new Dictionary<Language, string>
            {
                [Language.English] = $"Photo limit reached for wash {washingId}. Maximum {maxPhotos} photos allowed per wash cycle",
                [Language.Spanish] = $"Límite de fotos alcanzado para el lavado {washingId}. Máximo {maxPhotos} fotos permitidas por ciclo de lavado"
            });

            public static string InvalidFileType(string contentType) => GetMessage(new Dictionary<Language, string>
            {
                [Language.English] = $"Invalid file type: '{contentType}'. Only JPEG and PNG files are allowed",
                [Language.Spanish] = $"Tipo de archivo inválido: '{contentType}'. Solo se permiten archivos JPEG y PNG"
            });

            public static string FileSizeExceeded(long fileSizeBytes, int maxSizeMB = 5) => GetMessage(new Dictionary<Language, string>
            {
                [Language.English] = $"File size exceeded: {fileSizeBytes / (1024 * 1024)} MB. Maximum allowed: {maxSizeMB} MB",
                [Language.Spanish] = $"Tamaño de archivo excedido: {fileSizeBytes / (1024 * 1024)} MB. Máximo permitido: {maxSizeMB} MB"
            });

            public static string DescriptionTooLong(int maxLength = 200) => GetMessage(new Dictionary<Language, string>
            {
                [Language.English] = $"Photo description cannot exceed {maxLength} characters",
                [Language.Spanish] = $"La descripción de la foto no puede exceder {maxLength} caracteres"
            });

            public static string FileRequired => GetMessage(new Dictionary<Language, string>
            {
                [Language.English] = "Photo file is required",
                [Language.Spanish] = "El archivo de foto es obligatorio"
            });

            public static string FileEmpty => GetMessage(new Dictionary<Language, string>
            {
                [Language.English] = "Photo file is empty or corrupted",
                [Language.Spanish] = "El archivo de foto está vacío o corrupto"
            });

            public static string InvalidImageContent => GetMessage(new Dictionary<Language, string>
            {
                [Language.English] = "File does not contain valid image content",
                [Language.Spanish] = "El archivo no contiene contenido de imagen válido"
            });

            public static string NoPhotosForWash(long washingId) => GetMessage(new Dictionary<Language, string>
            {
                [Language.English] = $"No photos available for wash cycle {washingId}",
                [Language.Spanish] = $"No hay fotos disponibles para el ciclo de lavado {washingId}"
            });

            public static string FileAccessDenied(string fileName) => GetMessage(new Dictionary<Language, string>
            {
                [Language.English] = $"Access denied to photo file: {fileName}",
                [Language.Spanish] = $"Acceso denegado al archivo de foto: {fileName}"
            });

            public static string InvalidNamingSequence(string expectedName, string actualName) => GetMessage(new Dictionary<Language, string>
            {
                [Language.English] = $"Invalid photo naming sequence. Expected: {expectedName}, Got: {actualName}",
                [Language.Spanish] = $"Secuencia de nomenclatura de foto inválida. Esperado: {expectedName}, Recibido: {actualName}"
            });
        }

        /// <summary>
        /// Observation field validation errors
        /// </summary>
        public static class Observation
        {
            public static string StartObservationTooLong(int maxLength = 100) => GetMessage(new Dictionary<Language, string>
            {
                [Language.English] = $"Start observation cannot exceed {maxLength} characters",
                [Language.Spanish] = $"La observación de inicio no puede exceder {maxLength} caracteres"
            });

            public static string FinishObservationTooLong(int maxLength = 100) => GetMessage(new Dictionary<Language, string>
            {
                [Language.English] = $"Finish observation cannot exceed {maxLength} characters",
                [Language.Spanish] = $"La observación de finalización no puede exceder {maxLength} caracteres"
            });
        }

        /// <summary>
        /// General system validation errors
        /// </summary>
        public static class System
        {
            public static string RequiredField => GetMessage(new Dictionary<Language, string>
            {
                [Language.English] = "This field is required",
                [Language.Spanish] = "Este campo es obligatorio"
            });

            public static string InvalidFormat => GetMessage(new Dictionary<Language, string>
            {
                [Language.English] = "Invalid format",
                [Language.Spanish] = "Formato inválido"
            });

            public static string AccessDenied => GetMessage(new Dictionary<Language, string>
            {
                [Language.English] = "Access denied",
                [Language.Spanish] = "Acceso denegado"
            });

            public static string OperationNotAllowed => GetMessage(new Dictionary<Language, string>
            {
                [Language.English] = "Operation not allowed",
                [Language.Spanish] = "Operación no permitida"
            });

            public static string ConcurrentModification => GetMessage(new Dictionary<Language, string>
            {
                [Language.English] = "The record has been modified by another user",
                [Language.Spanish] = "El registro ha sido modificado por otro usuario"
            });

            public static string DatabaseConstraintViolation(string constraint) => GetMessage(new Dictionary<Language, string>
            {
                [Language.English] = $"Database constraint violation: {constraint}",
                [Language.Spanish] = $"Violación de restricción de base de datos: {constraint}"
            });

            public static string UnexpectedError(string operation) => GetMessage(new Dictionary<Language, string>
            {
                [Language.English] = $"Unexpected error during operation: {operation}",
                [Language.Spanish] = $"Error inesperado durante la operación: {operation}"
            });

            public static string InvalidParameter(string parameterName) => GetMessage(new Dictionary<Language, string>
            {
                [Language.English] = $"Invalid parameter: {parameterName}",
                [Language.Spanish] = $"Parámetro inválido: {parameterName}"
            });
        }

        /// <summary>
        /// Authentication and authorization errors
        /// </summary>
        public static class Auth
        {
            public static string TokenRequired => GetMessage(new Dictionary<Language, string>
            {
                [Language.English] = "Authentication token is required",
                [Language.Spanish] = "Se requiere token de autenticación"
            });

            public static string TokenExpired => GetMessage(new Dictionary<Language, string>
            {
                [Language.English] = "Authentication token has expired",
                [Language.Spanish] = "El token de autenticación ha expirado"
            });

            public static string InvalidToken => GetMessage(new Dictionary<Language, string>
            {
                [Language.English] = "Invalid authentication token",
                [Language.Spanish] = "Token de autenticación inválido"
            });

            public static string InsufficientRole => GetMessage(new Dictionary<Language, string>
            {
                [Language.English] = "Insufficient permissions for this operation",
                [Language.Spanish] = "Permisos insuficientes para esta operación"
            });

            public static string WarehouseUserRequired => GetMessage(new Dictionary<Language, string>
            {
                [Language.English] = "Warehouse user role is required",
                [Language.Spanish] = "Se requiere rol de usuario de almacén"
            });
        }

        /// <summary>
        /// Business rule validation errors
        /// </summary>
        public static class BusinessRules
        {
            public static string MustHaveProtsToStart => GetMessage(new Dictionary<Language, string>
            {
                [Language.English] = "Must add at least one PROT instrument set to start a wash cycle",
                [Language.Spanish] = "Debe agregar al menos un conjunto de instrumentos PROT para iniciar un ciclo de lavado"
            });

            public static string MustHavePhotosToFinish => GetMessage(new Dictionary<Language, string>
            {
                [Language.English] = "Must add at least one photo to finish a wash cycle",
                [Language.Spanish] = "Debe agregar al menos una foto para finalizar un ciclo de lavado"
            });

            public static string MaxTwoActiveWashes => GetMessage(new Dictionary<Language, string>
            {
                [Language.English] = "Cannot have more than 2 active wash cycles simultaneously",
                [Language.Spanish] = "No se pueden tener más de 2 ciclos de lavado activos simultáneamente"
            });

            public static string OnlyOneWashPerMachine => GetMessage(new Dictionary<Language, string>
            {
                [Language.English] = "Each machine can only run one wash cycle at a time",
                [Language.Spanish] = "Cada máquina solo puede ejecutar un ciclo de lavado a la vez"
            });

            public static string CannotModifyFinishedWash => GetMessage(new Dictionary<Language, string>
            {
                [Language.English] = "Cannot modify a finished wash cycle",
                [Language.Spanish] = "No se puede modificar un ciclo de lavado finalizado"
            });

            public static string WashingSequenceViolation(string expectedStatus, string currentStatus) => GetMessage(new Dictionary<Language, string>
            {
                [Language.English] = $"Invalid wash cycle status. Expected: {expectedStatus}, Current: {currentStatus}",
                [Language.Spanish] = $"Estado de ciclo de lavado inválido. Esperado: {expectedStatus}, Actual: {currentStatus}"
            });

            public static string ConcurrentWashLimitEnforcement => GetMessage(new Dictionary<Language, string>
            {
                [Language.English] = "Concurrent wash limit enforcement prevents starting new wash cycles",
                [Language.Spanish] = "La aplicación del límite de lavados concurrentes impide iniciar nuevos ciclos de lavado"
            });
        }
    }
}


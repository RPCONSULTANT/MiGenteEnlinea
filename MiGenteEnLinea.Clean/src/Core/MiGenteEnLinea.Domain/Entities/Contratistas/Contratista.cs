using MiGenteEnLinea.Domain.Common;
using MiGenteEnLinea.Domain.Events.Contratistas;
using MiGenteEnLinea.Domain.ValueObjects;

namespace MiGenteEnLinea.Domain.Entities.Contratistas;

/// <summary>
/// Entidad Contratista - Representa el perfil de un proveedor de servicios
/// Un contratista es una persona física que ofrece servicios laborales (plomería, electricidad, etc.)
/// 
/// MAPEO CON LEGACY:
/// - Tabla: Contratistas (nombre legacy plural - mantener para compatibilidad)
/// - Columnas: contratistaID, fechaIngreso, userID, titulo, tipo, identificacion, Nombre, Apellido, 
///             sector, experiencia, presentacion, telefono1, whatsapp1, telefono2, whatsapp2, email,
///             activo, provincia, nivelNacional, imagenURL
/// 
/// NOTAS DE NEGOCIO:
/// - Un userId (Credencial) puede ser empleador O contratista (relación 1:1)
/// - Los contratistas ofrecen servicios y reciben calificaciones
/// - Pueden tener múltiples fotos de trabajos realizados (relación 1:N)
/// - Pueden ofrecer múltiples servicios (relación N:M)
/// - Tipo: 1=Persona Física, 2=Empresa (mayoría son personas físicas)
/// </summary>
public sealed class Contratista : AggregateRoot
{
    /// <summary>
    /// Identificador único del contratista
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    /// Fecha en que el contratista se registró en la plataforma
    /// </summary>
    public DateTime? FechaIngreso { get; private set; }

    /// <summary>
    /// Identificador del usuario (FK a Credencial.UserId)
    /// Relaciona al contratista con sus credenciales de acceso
    /// </summary>
    public string UserId { get; private set; }

    /// <summary>
    /// Título profesional o descripción corta del contratista
    /// Ejemplo: "Plomero certificado con 10 años de experiencia"
    /// Máximo: 70 caracteres
    /// </summary>
    public string? Titulo { get; private set; }

    /// <summary>
    /// Tipo de contratista
    /// 1 = Persona Física (default)
    /// 2 = Empresa/Compañía
    /// </summary>
    public int Tipo { get; private set; }

    /// <summary>
    /// Cédula o RNC del contratista (identificación oficial)
    /// Máximo: 20 caracteres
    /// </summary>
    public string? Identificacion { get; private set; }

    /// <summary>
    /// Nombre(s) del contratista
    /// Máximo: 20 caracteres (legacy limitation)
    /// </summary>
    public string? Nombre { get; private set; }

    /// <summary>
    /// Apellido(s) del contratista
    /// Máximo: 50 caracteres
    /// </summary>
    public string? Apellido { get; private set; }

    /// <summary>
    /// Sector económico principal del contratista
    /// Ejemplo: "Construcción", "Reparaciones", "Servicios Generales"
    /// Máximo: 40 caracteres
    /// </summary>
    public string? Sector { get; private set; }

    /// <summary>
    /// Años de experiencia del contratista (valor entero)
    /// Ejemplo: 5, 10, 15
    /// </summary>
    public int? Experiencia { get; private set; }

    /// <summary>
    /// Presentación o biografía del contratista
    /// Ejemplo: "Soy un plomero con 10 años de experiencia en..."
    /// Máximo: 250 caracteres
    /// </summary>
    public string? Presentacion { get; private set; }

    /// <summary>
    /// Teléfono principal del contratista
    /// Máximo: 16 caracteres (incluye formato)
    /// </summary>
    public string? Telefono1 { get; private set; }

    /// <summary>
    /// Indica si el teléfono principal es WhatsApp
    /// </summary>
    public bool Whatsapp1 { get; private set; }

    /// <summary>
    /// Teléfono secundario del contratista (opcional)
    /// Máximo: 20 caracteres
    /// </summary>
    public string? Telefono2 { get; private set; }

    /// <summary>
    /// Indica si el teléfono secundario es WhatsApp
    /// </summary>
    public bool Whatsapp2 { get; private set; }

    /// <summary>
    /// Email del contratista (puede ser diferente al de credenciales)
    /// Máximo: 50 caracteres
    /// </summary>
    private string? _email;
    public Email? Email
    {
        get => _email != null ? ValueObjects.Email.CreateUnsafe(_email) : null;
        private set => _email = value?.Value;
    }

    /// <summary>
    /// Indica si el perfil del contratista está activo y visible
    /// </summary>
    public bool Activo { get; private set; }

    /// <summary>
    /// Provincia donde opera el contratista
    /// Ejemplo: "Santo Domingo", "Santiago", "La Vega"
    /// Máximo: 50 caracteres
    /// </summary>
    public string? Provincia { get; private set; }

    /// <summary>
    /// Indica si el contratista ofrece servicios a nivel nacional
    /// true = Trabaja en todo el país
    /// false = Solo trabaja en su provincia
    /// </summary>
    public bool NivelNacional { get; private set; }

    /// <summary>
    /// URL de la imagen de perfil del contratista
    /// Máximo: 150 caracteres
    /// TODO: Migrar a Azure Blob Storage en el futuro
    /// </summary>
    public string? ImagenUrl { get; private set; }

    /// <summary>
    /// Foto de perfil del contratista (byte array)
    /// TODO: Migrar a Azure Blob Storage en el futuro (guardar solo URL)
    /// Tamaño máximo recomendado: 5MB
    /// </summary>
    public byte[]? Foto { get; private set; }

    /// <summary>
    /// Constructor privado para EF Core
    /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor
    private Contratista() { }
#pragma warning restore CS8618

    /// <summary>
    /// Constructor privado para lógica de creación
    /// </summary>
    private Contratista(
        string userId,
        string nombre,
        string apellido,
        int tipo = 1, // Default: Persona Física
        string? titulo = null,
        string? identificacion = null,
        string? sector = null,
        int? experiencia = null,
        string? presentacion = null,
        string? telefono1 = null,
        bool whatsapp1 = false,
        string? provincia = null,
        bool nivelNacional = false)
    {
        UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
        Apellido = apellido ?? throw new ArgumentNullException(nameof(apellido));
        Tipo = tipo;
        Titulo = titulo?.Trim();
        Identificacion = identificacion?.Trim();
        Sector = sector?.Trim();
        Experiencia = experiencia;
        Presentacion = presentacion?.Trim();
        Telefono1 = telefono1?.Trim();
        Whatsapp1 = whatsapp1;
        Provincia = provincia?.Trim();
        NivelNacional = nivelNacional;
        FechaIngreso = DateTime.UtcNow;
        Activo = true; // Por defecto activo al crear
    }

    /// <summary>
    /// Factory Method: Crea un nuevo perfil de contratista
    /// </summary>
    /// <param name="userId">ID del usuario (debe existir en Credenciales)</param>
    /// <param name="nombre">Nombre del contratista (requerido)</param>
    /// <param name="apellido">Apellido del contratista (requerido)</param>
    /// <param name="tipo">Tipo: 1=Persona Física (default), 2=Empresa</param>
    /// <param name="titulo">Título profesional (opcional, max 70 caracteres)</param>
    /// <param name="identificacion">Cédula o RNC (opcional, max 20 caracteres)</param>
    /// <param name="sector">Sector económico (opcional, max 40 caracteres)</param>
    /// <param name="experiencia">Años de experiencia (opcional)</param>
    /// <param name="presentacion">Biografía (opcional, max 250 caracteres)</param>
    /// <param name="telefono1">Teléfono principal (opcional, max 16 caracteres)</param>
    /// <param name="whatsapp1">¿Es WhatsApp? (default: false)</param>
    /// <param name="provincia">Provincia donde opera (opcional, max 50 caracteres)</param>
    /// <param name="nivelNacional">¿Trabaja a nivel nacional? (default: false)</param>
    /// <returns>Nueva instancia de Contratista</returns>
    /// <exception cref="ArgumentException">Si los datos no cumplen validaciones</exception>
    public static Contratista Create(
        string userId,
        string nombre,
        string apellido,
        int tipo = 1,
        string? titulo = null,
        string? identificacion = null,
        string? sector = null,
        int? experiencia = null,
        string? presentacion = null,
        string? telefono1 = null,
        bool whatsapp1 = false,
        string? provincia = null,
        bool nivelNacional = false)
    {
        // Validaciones de negocio
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("UserId es requerido", nameof(userId));

        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("Nombre es requerido", nameof(nombre));

        if (string.IsNullOrWhiteSpace(apellido))
            throw new ArgumentException("Apellido es requerido", nameof(apellido));

        if (tipo < 1 || tipo > 2)
            throw new ArgumentException("Tipo debe ser 1 (Persona Física) o 2 (Empresa)", nameof(tipo));

        if (titulo?.Length > 70)
            throw new ArgumentException("Titulo no puede exceder 70 caracteres", nameof(titulo));

        if (identificacion?.Length > 20)
            throw new ArgumentException("Identificacion no puede exceder 20 caracteres", nameof(identificacion));

        if (nombre.Length > 20)
            throw new ArgumentException("Nombre no puede exceder 20 caracteres", nameof(nombre));

        if (apellido.Length > 50)
            throw new ArgumentException("Apellido no puede exceder 50 caracteres", nameof(apellido));

        if (sector?.Length > 40)
            throw new ArgumentException("Sector no puede exceder 40 caracteres", nameof(sector));

        if (experiencia.HasValue && experiencia.Value < 0)
            throw new ArgumentException("Experiencia no puede ser negativa", nameof(experiencia));

        if (presentacion?.Length > 250)
            throw new ArgumentException("Presentacion no puede exceder 250 caracteres", nameof(presentacion));

        if (telefono1?.Length > 16)
            throw new ArgumentException("Telefono1 no puede exceder 16 caracteres", nameof(telefono1));

        if (provincia?.Length > 50)
            throw new ArgumentException("Provincia no puede exceder 50 caracteres", nameof(provincia));

        var contratista = new Contratista(
            userId, nombre, apellido, tipo, titulo, identificacion,
            sector, experiencia, presentacion, telefono1, whatsapp1,
            provincia, nivelNacional);

        // Levantar evento de dominio
        contratista.RaiseDomainEvent(new ContratistaCreadoEvent(contratista.Id, userId));

        return contratista;
    }

    /// <summary>
    /// DOMAIN METHOD: Actualiza la información básica del perfil
    /// </summary>
    public void ActualizarPerfil(
        string? titulo = null,
        string? sector = null,
        int? experiencia = null,
        string? presentacion = null,
        string? provincia = null,
        bool? nivelNacional = null)
    {
        // Validaciones
        if (titulo?.Length > 70)
            throw new ArgumentException("Titulo no puede exceder 70 caracteres", nameof(titulo));

        if (sector?.Length > 40)
            throw new ArgumentException("Sector no puede exceder 40 caracteres", nameof(sector));

        if (experiencia.HasValue && experiencia.Value < 0)
            throw new ArgumentException("Experiencia no puede ser negativa", nameof(experiencia));

        if (presentacion?.Length > 250)
            throw new ArgumentException("Presentacion no puede exceder 250 caracteres", nameof(presentacion));

        if (provincia?.Length > 50)
            throw new ArgumentException("Provincia no puede exceder 50 caracteres", nameof(provincia));

        // Actualizar solo los campos que no sean null
        if (titulo != null)
            Titulo = titulo.Trim();

        if (sector != null)
            Sector = sector.Trim();

        if (experiencia.HasValue)
            Experiencia = experiencia.Value;

        if (presentacion != null)
            Presentacion = presentacion.Trim();

        if (provincia != null)
            Provincia = provincia.Trim();

        if (nivelNacional.HasValue)
            NivelNacional = nivelNacional.Value;

        // Levantar evento de dominio
        RaiseDomainEvent(new PerfilContratistaActualizadoEvent(Id));
    }

    /// <summary>
    /// DOMAIN METHOD: Actualiza información de contacto
    /// </summary>
    public void ActualizarContacto(
        string? telefono1 = null,
        bool? whatsapp1 = null,
        string? telefono2 = null,
        bool? whatsapp2 = null,
        Email? email = null)
    {
        // Validaciones
        if (telefono1?.Length > 16)
            throw new ArgumentException("Telefono1 no puede exceder 16 caracteres", nameof(telefono1));

        if (telefono2?.Length > 20)
            throw new ArgumentException("Telefono2 no puede exceder 20 caracteres", nameof(telefono2));

        // Actualizar campos
        if (telefono1 != null)
            Telefono1 = telefono1.Trim();

        if (whatsapp1.HasValue)
            Whatsapp1 = whatsapp1.Value;

        if (telefono2 != null)
            Telefono2 = telefono2.Trim();

        if (whatsapp2.HasValue)
            Whatsapp2 = whatsapp2.Value;

        if (email != null)
            Email = email;

        // Levantar evento de dominio
        RaiseDomainEvent(new ContactoActualizadoEvent(Id));
    }

    /// <summary>
    /// DOMAIN METHOD: Actualiza la imagen de perfil
    /// </summary>
    public void ActualizarImagen(string imagenUrl)
    {
        if (string.IsNullOrWhiteSpace(imagenUrl))
            throw new ArgumentException("ImagenUrl no puede estar vacía", nameof(imagenUrl));

        if (imagenUrl.Length > 150)
            throw new ArgumentException("ImagenUrl no puede exceder 150 caracteres", nameof(imagenUrl));

        ImagenUrl = imagenUrl.Trim();

        // Levantar evento de dominio
        RaiseDomainEvent(new ImagenActualizadaEvent(Id));
    }

    /// <summary>
    /// DOMAIN METHOD: Actualiza la foto de perfil (byte array)
    /// </summary>
    /// <param name="foto">Imagen en formato byte array</param>
    /// <exception cref="ArgumentException">Si la foto está vacía o excede el tamaño máximo</exception>
    public void ActualizarFoto(byte[] foto)
    {
        if (foto == null || foto.Length == 0)
            throw new ArgumentException("Foto no puede estar vacía", nameof(foto));

        // Validar tamaño máximo (5MB)
        const int maxSizeBytes = 5 * 1024 * 1024; // 5MB
        if (foto.Length > maxSizeBytes)
        {
            var maxSizeMB = maxSizeBytes / (1024 * 1024);
            throw new ArgumentException($"Foto no puede exceder {maxSizeMB}MB. Tamaño actual: {foto.Length / (1024 * 1024)}MB", nameof(foto));
        }

        Foto = foto;

        // Levantar evento de dominio
        RaiseDomainEvent(new FotoActualizadaEvent(Id));
    }

    /// <summary>
    /// DOMAIN METHOD: Elimina la imagen de perfil
    /// </summary>
    public void EliminarImagen()
    {
        ImagenUrl = null;
    }

    /// <summary>
    /// DOMAIN METHOD: Elimina la foto de perfil
    /// </summary>
    public void EliminarFoto()
    {
        Foto = null;
    }

    /// <summary>
    /// DOMAIN METHOD: Activa el perfil del contratista
    /// </summary>
    public void Activar()
    {
        if (Activo)
            throw new InvalidOperationException("El perfil ya está activo");

        Activo = true;
    }

    /// <summary>
    /// DOMAIN METHOD: Desactiva el perfil del contratista
    /// </summary>
    public void Desactivar()
    {
        if (!Activo)
            throw new InvalidOperationException("El perfil ya está desactivado");

        Activo = false;
    }

    /// <summary>
    /// DOMAIN METHOD: Valida si el contratista puede recibir trabajos
    /// </summary>
    /// <returns>True si cumple con los requisitos mínimos</returns>
    /// <remarks>
    /// Lógica de negocio: Un contratista puede recibir trabajos si:
    /// - Está activo
    /// - Tiene al menos un teléfono de contacto
    /// - Tiene una presentación o título
    /// </remarks>
    public bool PuedeRecibirTrabajos()
    {
        return Activo &&
               !string.IsNullOrWhiteSpace(Telefono1) &&
               (!string.IsNullOrWhiteSpace(Presentacion) || !string.IsNullOrWhiteSpace(Titulo));
    }

    /// <summary>
    /// DOMAIN METHOD: Valida si el perfil está completo
    /// </summary>
    /// <returns>True si tiene toda la información básica</returns>
    public bool PerfilCompleto()
    {
        return !string.IsNullOrWhiteSpace(UserId) &&
               !string.IsNullOrWhiteSpace(Nombre) &&
               !string.IsNullOrWhiteSpace(Apellido) &&
               !string.IsNullOrWhiteSpace(Titulo) &&
               !string.IsNullOrWhiteSpace(Presentacion) &&
               !string.IsNullOrWhiteSpace(Telefono1) &&
               !string.IsNullOrWhiteSpace(Provincia);
    }

    /// <summary>
    /// DOMAIN METHOD: Obtiene el nombre completo del contratista
    /// </summary>
    public string ObtenerNombreCompleto()
    {
        return $"{Nombre} {Apellido}".Trim();
    }

    /// <summary>
    /// DOMAIN METHOD: Obtiene una descripción corta del contratista
    /// </summary>
    /// <returns>Titulo o nombre completo si no hay titulo</returns>
    public string ObtenerDescripcionCorta()
    {
        if (!string.IsNullOrWhiteSpace(Titulo))
            return Titulo;

        return ObtenerNombreCompleto();
    }

    /// <summary>
    /// DOMAIN METHOD: Valida si tiene WhatsApp disponible
    /// </summary>
    public bool TieneWhatsApp()
    {
        return (!string.IsNullOrWhiteSpace(Telefono1) && Whatsapp1) ||
               (!string.IsNullOrWhiteSpace(Telefono2) && Whatsapp2);
    }
}

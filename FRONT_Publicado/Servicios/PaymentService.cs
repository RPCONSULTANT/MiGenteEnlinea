using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ClassLibrary_CSharp.Encryption;
using MiGente_Web.Data;
using Newtonsoft.Json;
using RestSharp;

namespace MiGente_Web.Servicios
{
    public class PaymentService
    {
        private readonly Crypt crypt = new Crypt();

        /// <summary>
        /// Genera/consulta una llave de idempotencia. Algunos gateways devuelven texto tipo "ikey:XXXXX".
        /// Esta función intenta ser tolerante a distintos formatos y valida respuestas no-JSON/HTML.
        /// </summary>
        public async Task<string> ConsultarIdempotency(string baseUrl)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new ArgumentException("Base URL is empty.", nameof(baseUrl));

            var idempotencyUrl = BuildIdempotencyUrl(baseUrl);

            using (var http = new HttpClient())
            {
                // Si el endpoint devuelve texto plano, esto no molesta; si devuelve JSON también.
                http.DefaultRequestHeaders.Accept.Clear();
                http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
                http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var resp = await http.PostAsync(idempotencyUrl, content: null).ConfigureAwait(false);
                var body = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (!resp.IsSuccessStatusCode)
                    throw BuildHttpException("Idempotency", idempotencyUrl, (int)resp.StatusCode, resp.ReasonPhrase, resp.Content.Headers.ContentType?.ToString(), body);

                if (string.IsNullOrWhiteSpace(body))
                    throw new Exception($"Idempotency endpoint returned empty body. Url={idempotencyUrl}");

                // Si por alguna razón llega HTML (login, 404, error), lo detectamos temprano.
                if (LooksLikeHtml(body))
                    throw BuildHttpException("Idempotency", idempotencyUrl, (int)resp.StatusCode, resp.ReasonPhrase, resp.Content.Headers.ContentType?.ToString(), body);

                // Caso esperado: "ikey:XXXX"
                var trimmed = body.Trim();
                if (trimmed.StartsWith("ikey:", StringComparison.OrdinalIgnoreCase))
                    return trimmed.Substring("ikey:".Length).Trim();

                // Caso alterno: devuelve JSON. Intentamos extraer campos comunes.
                // Si tu API devuelve otro shape, ajusta aquí.
                try
                {
                    var dyn = JsonConvert.DeserializeObject<dynamic>(trimmed);
                    // intenta varias claves típicas
                    string k1 = dyn?["idempotency-key"];
                    if (!string.IsNullOrWhiteSpace(k1)) return k1;

                    string k2 = dyn?["idempotencyKey"];
                    if (!string.IsNullOrWhiteSpace(k2)) return k2;

                    string k3 = dyn?["key"];
                    if (!string.IsNullOrWhiteSpace(k3)) return k3;
                }
                catch
                {
                    // ignoramos: puede ser texto plano sin prefijo ikey:
                }

                // Último recurso: devuelve el body tal cual (por si es solo la llave)
                return trimmed;
            }
        }

        public async Task<PaymentResponse> Payment(
            string cardNumber,
            string cvv,
            decimal amount,
            string clientIP,
            string expirationDate,
            string referenceNumber,
            string invoiceNumber)
        {
            // =====================================================================
            // MODO SIMULADO: Se omite la llamada real a Cardnet
            // El sistema simula una respuesta exitosa para permitir uso completo
            // =====================================================================

            // Generar una respuesta simulada exitosa
            var simulatedResponse = new PaymentResponse
            {
                IdempotencyKey = Guid.NewGuid().ToString(),
                ResponseCode = "00",                    // Código 00 = Aprobado
                InternalResponseCode = "00",
                ResponseCodeDesc = "APROBADA - SIMULADO",
                ResponseCodeSource = "SIMULATED",
                ApprovalCode = GenerateSimulatedApprovalCode(),
                PnRef = GenerateSimulatedPnRef()
            };

            // Pequeño delay para simular latencia de red (opcional)
            await Task.Delay(500).ConfigureAwait(false);

            return simulatedResponse;

            /* =====================================================================
               CÓDIGO ORIGINAL COMENTADO - Llamada real a Cardnet
               Descomentar este bloque y comentar el código simulado de arriba
               cuando se quiera volver a usar el gateway real de Cardnet
               =====================================================================
            
            var gatewayParams = GetPaymentParameters();
            if (gatewayParams == null)
                throw new Exception("PaymentGateway parameters not found in DB (PaymentGateway.FirstOrDefault returned null).");

            string baseUrl = gatewayParams.test ? gatewayParams.testURL : gatewayParams.productionURL;
            baseUrl = EnsureTrailingSlash(baseUrl);

            // 1) Obtener idempotency key
            var idempotencyKey = await ConsultarIdempotency(baseUrl).ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(idempotencyKey))
                throw new Exception("Idempotency key is empty.");

            // 2) Construir payload de forma segura (sin string interpolation para JSON)
            var payload = new
            {
                amount = amount,                       // RestSharp/Json.NET lo serializa bien
                ["card-number"] = crypt.Decrypt(cardNumber),
                ["client-ip"] = clientIP,
                currency = "214",
                cvv = cvv,
                environment = "ECommerce",
                ["expiration-date"] = expirationDate,
                ["idempotency-key"] = idempotencyKey,
                ["merchant-id"] = gatewayParams.merchantID,
                ["reference-number"] = referenceNumber,
                ["terminal-id"] = gatewayParams.terminalID,
                token = "454500350001",
                ["invoice-number"] = invoiceNumber
            };

            var jsonBody = JsonConvert.SerializeObject(payload, Formatting.None);

            // 3) Llamar /sales correctamente con RestSharp (base URL + resource)
            var client = new RestClient(baseUrl);
            var request = new RestRequest("sales", Method.POST);

            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/json");
            request.AddStringBody(jsonBody, DataFormat.Json);

            // IMPORTANTE: usa ExecuteAsync para no bloquear (y respetar async/await)
            var response = await client.ExecuteAsync(request).ConfigureAwait(false);

            var content = response.Content ?? string.Empty;

            if (!response.IsSuccessful)
                throw BuildHttpException("Payment", CombineUrl(baseUrl, "sales"), (int)response.StatusCode, response.StatusDescription, response.ContentType, content);

            if (string.IsNullOrWhiteSpace(content))
                throw new Exception("Payment endpoint returned empty body.");

            if (LooksLikeHtml(content))
                throw BuildHttpException("Payment", CombineUrl(baseUrl, "sales"), (int)response.StatusCode, response.StatusDescription, response.ContentType, content);

            if (!string.IsNullOrWhiteSpace(response.ContentType) &&
                response.ContentType.IndexOf("json", StringComparison.OrdinalIgnoreCase) < 0)
            {
                if (LooksLikeHtml(content))
                    throw BuildHttpException("Payment", CombineUrl(baseUrl, "sales"), (int)response.StatusCode, response.StatusDescription, response.ContentType, content);
            }

            try
            {
                var paymentResponse = JsonConvert.DeserializeObject<PaymentResponse>(content);
                return paymentResponse;
            }
            catch (JsonReaderException ex)
            {
                var snippet = content.Substring(0, Math.Min(400, content.Length));
                throw new Exception(
                    $"Failed to parse JSON from Payment response. " +
                    $"HTTP={(int)response.StatusCode} {response.StatusCode}. ContentType={response.ContentType}. BodyStart={snippet}",
                    ex);
            }
            */
        }

        /// <summary>
        /// Genera un código de aprobación simulado (6 dígitos)
        /// </summary>
        private string GenerateSimulatedApprovalCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        /// <summary>
        /// Genera un número de referencia PnRef simulado
        /// </summary>
        private string GenerateSimulatedPnRef()
        {
            return $"SIM{DateTime.Now:yyyyMMddHHmmss}{new Random().Next(1000, 9999)}";
        }

        public class PaymentResponse
        {
            [JsonProperty(PropertyName = "idempotency-key")]
            public string IdempotencyKey { get; set; }

            [JsonProperty(PropertyName = "response-code")]
            public string ResponseCode { get; set; }

            [JsonProperty(PropertyName = "internal-response-code")]
            public string InternalResponseCode { get; set; }

            [JsonProperty(PropertyName = "response-code-desc")]
            public string ResponseCodeDesc { get; set; }

            [JsonProperty(PropertyName = "response-code-source")]
            public string ResponseCodeSource { get; set; }

            [JsonProperty(PropertyName = "approval-code")]
            public string ApprovalCode { get; set; }

            [JsonProperty(PropertyName = "pnRef")]
            public string PnRef { get; set; }
        }

        public PaymentGateway GetPaymentParameters()
        {
            using (var db = new migenteEntities())
            {
                return db.PaymentGateway.FirstOrDefault();
            }
        }

        // -------------------------
        // Helpers
        // -------------------------

        private static bool LooksLikeHtml(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return false;
            var t = s.TrimStart();
            // Cubre HTML y también XML tipo "<Error>"
            return t.StartsWith("<", StringComparison.Ordinal);
        }

        private static string EnsureTrailingSlash(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return url;
            return url.EndsWith("/") ? url : url + "/";
        }

        private static string CombineUrl(string baseUrl, string relative)
        {
            baseUrl = EnsureTrailingSlash(baseUrl);
            relative = (relative ?? string.Empty).TrimStart('/');
            return baseUrl + relative;
        }

        private static string BuildIdempotencyUrl(string baseUrl)
        {
            // Corrige el typo "idenpotency" -> "idempotency"
            // Intentamos reemplazar si existe /transactions/ en la base.
            // Si no existe, simplemente agregamos el recurso.
            var normalized = EnsureTrailingSlash(baseUrl);

            // Si la base trae "transactions" en el path, reemplaza ese segmento por "idempotency-keys"
            // EJ: https://host/api/transactions/  -> https://host/api/idempotency-keys
            var marker = "/transactions/";
            var idx = normalized.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
            if (idx >= 0)
            {
                var before = normalized.Substring(0, idx);
                var after = normalized.Substring(idx + marker.Length); // lo que venga luego
                // Mantén el resto de path si existiera, pero típicamente after estará vacío
                var rebuilt = EnsureTrailingSlash(before + "/idempotency-keys/" + after);
                return rebuilt.TrimEnd('/'); // many APIs accept without trailing slash; ajusta si necesitas
            }

            // Fallback: asume que el recurso está relativo a la base
            return CombineUrl(normalized, "idempotency-keys");
        }

        private static Exception BuildHttpException(string opName, string url, int statusCode, string statusText, string contentType, string body)
        {
            var snippet = (body ?? string.Empty);
            snippet = snippet.Substring(0, Math.Min(500, snippet.Length));

            return new Exception(
                $"{opName} call failed. Url={url}. HTTP={statusCode} {statusText}. ContentType={contentType}. BodyStart={snippet}");
        }
    }
}

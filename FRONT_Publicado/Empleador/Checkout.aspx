<%@ Page Title="" Language="C#" MasterPageFile="~/Empleador/dashboard.Master" AutoEventWireup="true" CodeBehind="Checkout.aspx.cs" Inherits="MiGente_Web.Empleador.Checkout" Async="true" %>

<%@ Register Assembly="DevExpress.Web.Bootstrap.v22.2, Version=22.2.5.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.Bootstrap" TagPrefix="dx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <style>
      body{  
        .card {
    box-shadow: 0 20px 27px 0 rgb(0 0 0 / 5%);
}

.card {
    position: relative;
    display: flex;
    flex-direction: column;
    min-width: 0;
    word-wrap: break-word;
    background-color: #fff;
    background-clip: border-box;
    border: 0 solid rgba(0,0,0,.125);
    border-radius: 1rem;
}

.card-body {
    -webkit-box-flex: 1;
    -ms-flex: 1 1 auto;
    flex: 1 1 auto;
    padding: 1.5rem 1.5rem;
}
    </style>
    <div class="container" style="background-color:white; border-radius:10px;" >
        <br />
    <h1 class="h3 mb-5">Complete su Orden</h1>
    <div class="row">
      <!-- Left -->
      <div class="col-lg-9">
        <div class="accordion" id="accordionPayment">
          <!-- Credit card -->
            <img src="../Images/Cardnet-Web.png" width=200/>
            <div id="collapseCC" class="accordion-collapse collapse show" data-bs-parent="#accordionPayment" style="">
      
                <div class="mb-3">
                  <label class="form-label">Numero de Tarjeta</label>
                    <dx:BootstrapTextBox ID="CardNumber" ValidationSettings-RequiredField-IsRequired="true" ValidationSettings-ValidationGroup="payForm" runat="server"></dx:BootstrapTextBox>
                </div>
                <div class="row">
                  <div class="col-lg-6">
                    <div class="mb-3">
                      <label class="form-label">Nombre en la Tarjeta</label>
                                        <dx:BootstrapTextBox ID="cardName" ValidationSettings-RequiredField-IsRequired="true" ValidationSettings-ValidationGroup="payForm" runat="server"></dx:BootstrapTextBox>

                    </div>
                  </div>
                  <div class="col-lg-3">
                    <div class="mb-3">
                      <label class="form-label">Fecha de Expiracion (MM/AA)</label>
                                      <dx:BootstrapTextBox ID="expiryDate" MaskSettings-Mask="00/00" ValidationSettings-RequiredField-IsRequired="true" ValidationSettings-ValidationGroup="payForm" runat="server"></dx:BootstrapTextBox>

                    </div>
                  </div>
                  <div class="col-lg-3">
                    <div class="mb-3">
                      <label class="form-label">CVV</label>
                                      <dx:BootstrapTextBox ID="cvvNumber" Password="true" ValidationSettings-RequiredField-IsRequired="true" ValidationSettings-ValidationGroup="payForm" runat="server"></dx:BootstrapTextBox>

                    </div>
                  </div>
                </div>
              </div>
          </div>

         
      </div>
      <!-- Right -->
      <div class="col-lg-3">
        <div class="card position-sticky top-0">
          <div class="p-3 bg-light bg-opacity-10">
            <h6 class="card-title mb-3">Resumen de Orden</h6>
             <div class="d-flex justify-content-between mb-1 small">
              <label style="font-size:large" class="col-lg-12">Nombre de Plan</label>
             
            </div>
               <div>
              <label style="font-size:medium" class="col-lg-12" runat="server" id="lbPlanName">Plan</label>
             
            </div>
            <hr>
            <div class="d-flex justify-content-between mb-1 small">
              <span>TOTAL A PAGAR</span> <strong runat="server" id="amount" class="text-dark">224.50</strong>
            </div>
            <div class="form-check small mb-2">
                <dx:BootstrapCheckBox  ID="chkTerminosMiGente"
                    ValidationSettings-ValidationGroup="payForm" Text="Acepto los" ValidationSettings-RequiredField-IsRequired="true"
                    runat="server"></dx:BootstrapCheckBox>
           
              <a href="../Template/TerminosMiGente.html"  style="margin-top:-5px" id="enlaceTerminos1">Términos y Condiciones</a>
           

            </div>
             <div class="form-check small" runat="server" id="divAutorizacionEmpleadores">
  
        <dx:BootstrapCheckBox ID="chkAutorizacion" 
            ValidationSettings-ValidationGroup="payForm" Text="Acepto los"  ValidationSettings-RequiredField-IsRequired="true" runat="server"></dx:BootstrapCheckBox>

        <label class="form-check-label" style="margin-top:-5px" for="chkAutorizacion">
             <a href="../Template/AutorizacionEmpleadores.html" id="enlaceTerminos2">Terminos de inscripcion de Empleadores</a>
        </label>
  
</div>

              <dx:BootstrapButton ID="btnPay" ValidationGroup="payForm" CausesValidation="true" runat="server" OnClick="btnPay_Click" AutoPostBack="false" Text="Adquirir Plan"></dx:BootstrapButton>
          </div>
        </div>
      </div>
    </div>
  </div>
    <script type="text/javascript">
    document.addEventListener("DOMContentLoaded", function () {
        var enlaceTerminos1 = document.getElementById("enlaceTerminos1");
        var enlaceTerminos2 = document.getElementById("enlaceTerminos2");
        var enlaceTerminos3 = document.getElementById("enlaceTerminos3");
        
        enlaceTerminos1.addEventListener("click", function (e) {
            e.preventDefault(); // Evita que el enlace se abra en la ventana actual
            
            // Especifica las dimensiones y opciones de la ventana emergente
            var opcionesVentana = "width=800, height=600, resizable=yes, scrollbars=yes";
            
            // Abre el enlace en una ventana emergente
            window.open(this.href, "Términos y Condiciones", opcionesVentana);
        });


        enlaceTerminos2.addEventListener("click", function (e) {
            e.preventDefault(); // Evita que el enlace se abra en la ventana actual

            // Especifica las dimensiones y opciones de la ventana emergente
            var opcionesVentana = "width=800, height=600, resizable=yes, scrollbars=yes";

            // Abre el enlace en una ventana emergente
            window.open(this.href, "Términos y Condiciones", opcionesVentana);
        });

        enlaceTerminos3.addEventListener("click", function (e) {
            e.preventDefault(); // Evita que el enlace se abra en la ventana actual

            // Especifica las dimensiones y opciones de la ventana emergente
            var opcionesVentana = "width=800, height=600, resizable=yes, scrollbars=yes";

            // Abre el enlace en una ventana emergente
            window.open(this.href, "Términos y Condiciones", opcionesVentana);
        });
    });

        // Función para mostrar alerta de suscripción exitosa
        function mostrarAlertaSuscripcionExitosa() {
            Swal.fire({
                icon: 'success',
                title: '¡Suscripción completada!',
                text: 'Tu suscripción se ha completado correctamente.',
                confirmButtonText: 'OK',  // Cambia el texto del botón de confirmación
            }).then((result) => {
                // Verifica si el usuario hizo clic en "OK"
                if (result.isConfirmed) {
                    // Redirige a otra página
                    window.location= 'miPerfilEmpleador.aspx';
                    window.location.replace = 'miPerfilEmpleador.aspx';

                }
            });
        }

        // Función para mostrar alerta de problema al procesar el pago
        function mostrarAlertaProblemaPago() {
            Swal.fire({
                icon: 'error',
                title: '¡Error al procesar el pago!',
                text: 'Ha ocurrido un problema al procesar tu pago. Por favor, inténtalo de nuevo.',
            });
        }

        function mostrarAlertaTerminos() {
            Swal.fire({
                icon: 'error',
                title: '¡No Puede Continuar con El Registro!',
                text: 'Debe Aceptar todos los Terminos y Condiciones.',
            });
        }
    </script>



</asp:Content>

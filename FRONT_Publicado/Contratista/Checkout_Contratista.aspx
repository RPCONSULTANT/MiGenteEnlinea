<%@ Page Title="" Language="C#" MasterPageFile="~/Contratista/ContratistasM.Master" AutoEventWireup="true" CodeBehind="Checkout_Contratista.aspx.cs" Async="true" Inherits="MiGente_Web.Contratista.Checkout_Contratista" %>

<%@ Register Assembly="DevExpress.Web.Bootstrap.v22.2, Version=22.2.5.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.Bootstrap" TagPrefix="dx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <head>
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>

           <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <link href="../Styles/animated.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0/css/bootstrap.min.css" />
    <link href="https://maxcdn.bootstrapcdn.com/font-awesome/4.3.0/css/font-awesome.min.css" rel="stylesheet" />
    <link href="../Styles/Custom.css" rel="stylesheet" />
    <style>
        @font-face {
            font-family: "Gurajada";
            src: url(../Fonts/Gurajada-Regular.ttf);
        }

        html, body {
            height: 100%;
        }

        body {
            display: flex;
            flex-direction: column;
        }

        .footer {
            margin-top: auto;
        }

        .headerText {
            font-family: "Gurajada", Gurajada;
        }

        /* Estilos adicionales personalizados */
        .navbar-brand {
            padding: 0;
        }

        .navbar {
                 /*   background: url("../Images/bannerADM1.jpg");*/
            background-size: cover;
            background-position: unset;
        }

        .navbar-brand img {
            height: 50px;
            margin-left: 10px;
        }

        .navbar-nav ml-auto {
            margin-right: 10px;
        }

        .banner {
            position: relative;
            height: 500px;
            background: url("../Images/bannerADM1.jpg");
            /*background: url("../Images/banner_Foto1.jpg");*/
            background-size: cover;
            background-position: left;
            background-color: rgba(0, 0, 0, 0.5); /* Añade un overlay semitransparente */
        }

        .banner2 {
            position: center;
            height: 300px;
            background: linear-gradient(123deg, #00176C 0%, #EE09A3 100%);
            background-size: cover;
            background-position: unset;
        }

        .banner-content {
            /*    position: relative;
            top: 50%;
            left: 50%;
            transform: translate(-50%, -50%);
            text-align: center;
            color: white;*/
        }

            .banner-content h1 {
                font-size: 40px;
            }

        .link {
            color: darkgrey;
        }

            .link:hover {
                box-shadow: inset 30px 30px 30px 30px #007BFF;
                color: darkgrey;
            }

        html {
            scroll-behavior: smooth;
        }

        .no-border {
            border: none;
        }
    </style>

    <title>Mi Gente en línea</title>
    </head>
    <div class="container bg-white">
     <h1 class="h3 mb-1 ">Complete su Orden</h1>
        <a class="mb-2 ">Debe completar la adquisicion del Plan Soy Tu Gente</a>
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
             <div class="form-check small" runat="server" id="divAutorizacionProveedores">
  
        <dx:BootstrapCheckBox ID="chkAutorizacion" 
            ValidationSettings-ValidationGroup="payForm" Text="Acepto los"  ValidationSettings-RequiredField-IsRequired="true" runat="server"></dx:BootstrapCheckBox>

        <label class="form-check-label" style="margin-top:-5px" for="chkAutorizacion">
             <a href="../Template/AutorizacionProveedores.html" id="enlaceTerminos2">Terminos de inscripcion de Empleadores</a>
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
                    window.location = 'miPerfilEmpleador.aspx';
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

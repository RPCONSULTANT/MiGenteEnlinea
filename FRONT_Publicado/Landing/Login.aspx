<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="MiGente_Web.Landing.Login" %>

<%@ Register Assembly="DevExpress.Web.Bootstrap.v22.2, Version=22.2.5.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.Bootstrap" TagPrefix="dx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.7.0/css/font-awesome.css" />
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <link href="../Styles/animated.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>

    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css" />
    <title></title>
    <style>
        .logo {
            text-align: center
        }

        body {
            background: linear-gradient(to bottom right, rgba(255, 0, 255, 0.1), rgba(0, 0, 255, 0.91)), url(../Images/back1.jpg);
            background-size: inherit;
        }

        .login-container {
            display: flex;
            justify-content: center;
            align-items: center;
            height: 100vh;
        }

        .login-form {
            background-color: rgba(255, 255, 255, 0.9);
            border-radius: 8px;
            padding: 20px;
            max-width: 400px;
            width: 100%;
            box-shadow: 0 2px 4px rgba(0, 0, 0, 0.2);
        }


        .form-group {
            margin-bottom: 20px;
        }

            .form-group label {
                display: block;
                font-weight: bold;
                margin-bottom: 5px;
            }

        .animated-form {
            display: none;
        }
    </style>

    <script>


        function toggleDivs(hideDivId, showDivId) {
            var hideDiv = document.getElementById(hideDivId);
            var showDiv = document.getElementById(showDivId);

            if (hideDiv && showDiv) {
                hideDiv.style.display = 'none';
                showDiv.style.display = 'block';
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="login-container animate__animated animate__jello">
            <div id="loginForm" class="login-form animate__animated animate__flipInY">
                <h4 class="text-center">Bienvenido a</h4>
                <a href="https://www.migenteenlinea.do" target="_parent">
                    <div style="text-align: center">

                        <img src="../Images/logoMiGene.png" />
                    </div>
                </a>

                <hr />

                <div>
                    <div class="form-group">
                        <label for="txtEmail">Email</label>
                        <input runat="server" type="text" id="txtEmail" class="form-control" placeholder="Correo electrónico" />
                    </div>

                    <div class="form-group">
                        <label for="txtPassword">Contraseña</label>
                        <div class="input-group">
                            <input runat="server" type="password" id="txtPassword" class="form-control" placeholder="Contraseña" />
                            <div class="input-group-append">
                                <button id="btnTogglePassword" class="btn btn-outline-secondary" type="button" onclick="togglePasswordVisibility()">
                                    <i id="eyeIcon" class="fa fa-eye"></i>
                                </button>
                            </div>
                        </div>
                    </div>

                    <div class="form-group">
                        <button runat="server" id="btnLogin" onserverclick="btnLogin_ServerClick" type="submit" class="btn btn-primary btn-block">Iniciar sesión</button>
                    </div>
                    <div class="form-group">
                        <button runat="server" id="btnRegistrar" onserverclick="btnRegistrar_ServerClick" type="submit" class="btn btn-secondary btn-block">Crear Nueva Cuenta</button>
                    </div>
                    <div class="form-group text-center">
                        <a href="#" onclick="toggleDivs('loginForm','forgotPasswordForm')" class="text-primary">¿Olvidaste tu contraseña?</a>
                    </div>
                </div>
            </div>

            <div id="forgotPasswordForm" class="login-form animated-form animate__animated animate__flipInX">
                <h4 class="text-center">Recuperar contraseña</h4>
                <hr />

                <div>
                    <div class="form-group">
                        <label for="txtForgotEmail">Correo electrónico</label>
                        <dx:BootstrapTextBox ID="txtForgotPass" NullText="Correo Electronico" runat="server"></dx:BootstrapTextBox>

                    </div>

                    <div class="form-group">
                        <dx:BootstrapButton ID="btnSendSolicitud" runat="server" Width="100%" AutoPostBack="false" OnClick="btnSendSolicitud_Click" Text="Enviar Solicitud"></dx:BootstrapButton>
                    </div>

                    <div class="form-group text-center">
                        <a href="#" onclick="toggleDivs('forgotPasswordForm','loginForm')" class="text-primary">Volver al inicio de sesión</a>
                    </div>
                </div>
            </div>
        </div>


    </form>
    <script>
        window.addEventListener('load', function () {
            document.getElementById('loginButton').disabled = true;
            document.getElementById('forgotButton').disabled = true;
        });

        function togglePasswordVisibility() {
            var passwordField = document.getElementById("txtPassword");
            var btnShowPassword = document.getElementById("btnShowPassword");

            if (passwordField.type === "password") {
                passwordField.type = "text";
                btnShowPassword.textContent = "Ocultar";
            } else {
                passwordField.type = "password";
                btnShowPassword.textContent = "Mostrar";
            }
        }

    </script>
    <script src="https://code.jquery.com/jquery-3.5.1.slim.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/@popperjs/core@2.5.4/dist/umd/popper.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>


</body>

</html>

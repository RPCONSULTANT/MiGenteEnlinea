<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="activarperfil.aspx.cs" Inherits="MiGente_Web.Landing.activarperfil" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">

   <link rel="stylesheet" type="text/css" href="https://cdn.jsdelivr.net/npm/sweetalert2@11.1.6/dist/sweetalert2.min.css"/>
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11.1.6/dist/sweetalert2.all.min.js"></script>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
     <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <link href="../Styles/animated.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css">
    <title>Activación de Cuenta</title>

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
</head>
<body>
    <form id="form1" runat="server">
  
         <div class="login-container animate__animated animate__jello">
            <div id="activationForm" class="login-form animate__animated animate__flipInY">
                <h4 class="text-center">Activación de Cuenta</h4>
                <hr />

               <div>
                    <div class="form-group">
                        <label for="txtEmail">Correo electrónico</label>
                        <input runat="server" type="text" id="txtEmail" class="form-control" placeholder="Correo electrónico"/>
                    </div>

                    <div class="form-group">
                        <label for="txtPassword">Contraseña</label>
                        <input runat="server" type="password" id="txtPassword" class="form-control" placeholder="Contraseña"/>
                    </div>

                    <div class="form-group">
                        <label for="txtConfirmPassword">Confirmar contraseña</label>
                        <input runat="server" type="password" required="required" id="txtConfirmPassword" class="form-control" placeholder="Confirmar contraseña" />
                    </div>

                

                    <div class="form-group">
                        <button type="submit" class="btn btn-primary btn-block" runat="server" onserverclick="btnActivar_ServerClick">Crear contraseña</button>
                    </div>
                </div>
            </div>
        </div>
    </form>
    <script src="https://code.jquery.com/jquery-3.5.1.slim.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>
    <script>
        window.addEventListener('load', function () {
            document.getElementById('txtEmail').value = getParameterByName('email');; // Asigna aquí el valor del correo electrónico

            document.getElementById('form1').addEventListener('submit', function (e) {
                e.preventDefault(); // Evitar el envío del formulario

                var password = document.getElementById('txtPassword').value;
                var confirmPassword = document.getElementById('txtConfirmPassword').value;

                if (password === confirmPassword) {
                    // Las contraseñas coinciden, enviar el formulario
                   
                    this.submit();
                } else {
                    // Las contraseñas no coinciden, mostrar un mensaje de error
                    alert('Las contraseñas no coinciden. Por favor, verifica.');
                }



            });
            document.getElementById('txtPassword').addEventListener('input', enableDisableButton);
            document.getElementById('txtConfirmPassword').addEventListener('input', enableDisableButton);

        });
            function getParameterByName(name, url = window.location.href) {
                name = name.replace(/[\[\]]/g, '\\$&');
                var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'),
                    results = regex.exec(url);
                if (!results) return null;
                if (!results[2]) return '';
                return decodeURIComponent(results[2].replace(/\+/g, ' '));
            }


            function enableDisableButton() {
                var password = document.getElementById('txtPassword').value;
                var confirmPassword = document.getElementById('txtConfirmPassword').value;
                var submitButton = document.querySelector('button[type="submit"]');

                if (password === confirmPassword) {
                    submitButton.disabled = false;
                } else {
                    submitButton.disabled = true;
                   
                }
        }
        </script>
     <script src="https://code.jquery.com/jquery-3.5.1.slim.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>
    <script>
        window.addEventListener('load', function () {
            document.getElementById('txtEmail').disabled=true;
            // Obtener la URL actual
            var url = new URL(window.location.href);

            // Obtener los parámetros de la URL
            var params = new URLSearchParams(url.search);

            // Obtener el valor del parámetro "parametro"
    
            var email = params.get("email");
            document.getElementById('txtEmail').value = email; // Asigna aquí el valor del correo electrónico
        });
    </script>
</body>
</html>

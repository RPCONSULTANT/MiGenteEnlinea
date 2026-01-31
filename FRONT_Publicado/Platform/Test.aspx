<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Test.aspx.cs" Inherits="MiGente_Web.Platform.Test" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">

  <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
  <title>Directorio de Freelancers</title>
  <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.0/css/bootstrap.min.css"/>
</head>
<body>
    <form id="form1" runat="server">
    <nav class="navbar navbar-expand-lg navbar-light bg-light">
    <a class="navbar-brand" href="#">Mi Gente</a>
    <button class="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarNav" aria-controls="navbarNav" aria-expanded="false" aria-label="Toggle navigation">
      <span class="navbar-toggler-icon"></span>
    </button>
    <div class="collapse navbar-collapse" id="navbarNav">
      <ul class="navbar-nav ml-auto">
        <li class="nav-item">
          <a class="nav-link" href="#">Acceder</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="#">Registro</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="#">Perfil</a>
        </li>
      </ul>
    </div>
  </nav>

  <div class="banner">
    <div class="banner-content">
      <h1 class="headerText animate__animated animate__slideInLeft">Encuentra al colaborador adecuado</h1>
      <div class="input-group mb-3">
        <input type="text" class="form-control" placeholder="Buscar colaborador" aria-label="Buscar colaborador" aria-describedby="button-addon2">
        <div class="input-group-append">
          <button class="btn btn-primary" type="button" id="button-addon2">Buscar</button>
        </div>
      </div>
    </div>
  </div>

  <div class="container col col-lg-12 form-control" style="background-color: #00176B; width: auto; color: white; text-align: center;">
    <h6 class="animate__animated animate__heartBeat animate__infinite animate__slower" style="margin: 10px">Soluciones integrales de autogestión y construcción de referencias</h6>
  </div>

  <div class="container">
    <!-- Aquí va el contenido de los perfiles de freelancer -->
  </div>

  <script src="https://code.jquery.com/jquery-3.5.1.slim.min.js"></script>
  <script src="https://cdn.jsdelivr.net/npm/@popperjs/core@1.14.7/dist/umd/popper.min.js"></script>
  <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>
    </form>
</body>
</html>

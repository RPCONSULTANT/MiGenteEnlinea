<%@ Page Title="" Language="C#" MasterPageFile="~/Empleador/comunity.Master" AutoEventWireup="true" CodeBehind="perfilProfesional.aspx.cs" Inherits="MiGente_Web.Empleador.perfilProfesional" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
     <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Perfil Profesional</title>
    <style>
        body {
            background-color: #f4f4f4;
            font-family: Arial, sans-serif;
        }
        .container {
            background-color: #ffffff;
            border-radius: 5px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
            padding: 20px;
            margin-top: 20px;
        }
        .profile-image {
            border-radius: 50%;
            width: 150px;
            height: 150px;
            object-fit: cover;
            margin: 0 auto;
            display: block;
        }
        .card-title {
            font-size: 24px;
            font-weight: bold;
            text-align: center;
        }
        .form-label {
            font-weight: bold;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:HiddenField ID="HiddenField1" runat="server" />
        <h3 class="card-title">Perfil Profesional</h3>
        <div class="row">
            
            <div class="col-md-3 text-center">
                <img src="https://via.placeholder.com/150" alt="Imagen de perfil" class="profile-image">
                <h5 class="card-title">Imagen de Perfil</h5>
                <p>Calificación de Perfil: 4.5</p>
            </div>
            <div class="col-md-9">
                <ul class="nav nav-tabs">
                    <li class="nav-item">
                        <a class="nav-link active" data-toggle="tab" href="#datosGenerales">Datos Generales</a>
                    </li>
                    <li class="nav-item">
                        <a class="nav-link" data-toggle="tab" href="#servicios">Catálogo de Servicios</a>
                    </li>
                </ul>
                <div class="tab-content">
                    <div class="tab-pane fade show active" id="datosGenerales">
                        <div class="row mt-3">
                        
                            <div class="col-md-6">

                                <div class="form-group">
                                    <label for="titulo" class="form-label">Título de Perfil</label>
                                    <p>Título de ejemplo</p>
                                </div>
                                <div class="form-group">
                                    <label for="sector" class="form-label">Sector Profesional</label>
                                    <p>Sector de ejemplo</p>
                                </div>
                                <div class="form-group">
                                    <label for="presentacion" class="form-label">Presentación</label>
                                    <p>Presentación de ejemplo</p>
                                </div>
                                <div class="form-group">
                                    <label for="email" class="form-label">Email</label>
                                    <p>correo@example.com</p>
                                </div>
                                <div class="form-group">
                                    <label for="experiencia" class="form-label">Años de Experiencia</label>
                                    <p>5 años</p>
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="form-group">
                                    <label for="tipoPerfil" class="form-label">Tipo de Perfil</label>
                                    <p>Persona Física</p>
                                </div>
                                <div class="form-group">
                                    <label for="identificacion" class="form-label">RNC/Cédula</label>
                                    <p>1234567890</p>
                                </div>
                                <div class="form-group">
                                    <label for="nombre" class="form-label">Nombre</label>
                                    <p>Nombre de ejemplo</p>
                                </div>
                                <div class="form-group">
                                    <label for="apellido" class="form-label">Apellido</label>
                                    <p>Apellido de ejemplo</p>
                                </div>
                                <div class="form-group">
                                    <label for="razonSocial" class="form-label">Razón Social</label>
                                    <p>Razón Social de ejemplo</p>
                                </div>
                                <div class="form-group">
                                    <label for="telefono1" class="form-label">Teléfono 1</label>
                                    <p>(123)-456-7890</p>
                                </div>
                                <div class="form-group">
                                    <label for="telefono2" class="form-label">Teléfono 2</label>
                                    <p>(987)-654-3210</p>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="tab-pane fade" id="servicios">
                        <div class="form-group">
                            <label for="servicio" class="form-label">Descripción de Servicio</label>
                            <p>Servicio 1: Descripción de ejemplo</p>
                            <p>Servicio 2: Descripción de ejemplo</p>
                            <p>Servicio 3: Descripción de ejemplo</p>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    <br />
    <hr />
    <br />
</asp:Content>

<%@ Page Title="" Language="C#" MasterPageFile="~/Empleador/comunity.Master" AutoEventWireup="true" CodeBehind="index_empleador.aspx.cs" Inherits="MiGente_Web.Empleador.index_empleador" %>

<%@ Register Assembly="DevExpress.Web.v22.2, Version=22.2.5.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href='https://use.fontawesome.com/releases/v5.7.2/css/all.css' rel='stylesheet'>
    <style>
        @import url('https://fonts.googleapis.com/css2?family=Maven+Pro&display=swap');

        body {
            font-family: 'Maven Pro', sans-serif
        }

        body {
            background-color: #eee
        }

        .add {
            border-radius: 20px
        }

        .card {
            border: none;
            border-radius: 10px;
            transition: all 1s;
            cursor: pointer
        }

            .card:hover {
                -webkit-box-shadow: 3px 5px 17px -4px #777777;
                box-shadow: 3px 5px 17px -4px #777777
            }

        .ratings i {
            color: green
        }

        }

        hr {
            margin: 1rem 0;
            color: inherit;
            background-color: currentColor;
            border: 0;
            opacity: 0.25;
        }

        .star {
            font-size: 20px;
            color: #ccc; /* Color gris para estrella vacía */
            margin-right: 5px; /* Espacio entre estrellas */
        }

            .star.filled {
                color: yellow; /* Color amarillo para estrella llena */
            }

        .avatar {
            border-radius: 50%;
            width: 150px; /* Ajusta el ancho y alto según tus necesidades */
            height: 150px;
            object-fit: cover; /* Para que la imagen se ajuste y no se deforme */
        }

        .profile-image {
            border-radius: 50%;
            width: 150px;
            height: 150px;
            object-fit: cover;
            margin: 0 auto;
            display: block;
        }
    </style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container text-center m-t-5" style="margin-top: 50px;">
    </div>
    <%-- <div class="container text-center m-t-5" style="margin-top:50px;">
    <h4 style="margin-top:10px;">Como Funciona Mi Gente?</h4>
      </div>--%>
    <%--<div class="col col-12" style="text-align:center; margin-top:30px">
    <div class="row justify-content-center">
        <div class="divTarjetas col-lg-3 col-md-3 col-sm-12 mb-3" style="text-align:center">
            <div>
                <img src="../Images/image47.png" />
            </div>
            <label class="m-t-5" style="font-weight:bold">Crea tu Cuenta</label>
            <p>Crea tu perfil para acceder a todos los beneficios que te ofrece mi gente</p>
        </div>
        <div class="divTarjetas col-lg-3 col-md-3 col-sm-12 mb-3" style="text-align:center">
            <div>
                <img src="../Images/image50.png" />
            </div>
            <label class="m-t-5" style="font-weight:bold">Encuentra</label>
            <p>Encuentra el colaborador perfecto que se adapte a tus necesidades</p>
        </div>
        <div class="divTarjetas col-lg-3 col-md-3 col-sm-12 mb-3" style="text-align:center">
            <div>
                <img src="../Images/image51.png" />
            </div>
            <label class="m-t-5" style="font-weight:bold">Cuentanos</label>
            <p>Puedes calificar el perfil y contarle a la comunidad como te fue</p>
        </div>
    </div>
</div>--%>

    <%--   <hr class="bg-danger border-2 border-top border-danger" />
    <div class="row g-2" id="profilesContainer">
        <asp:Repeater ID="repeaterDoctors" runat="server">
            <ItemTemplate>
                <div class="col col-md-3 col-sm-12">
                    <div class="card p-2 py-3 text-center">
                        <div class="img mb-2">
                            <img src='<%# Eval("ImageUrl") %>' width="70" class="rounded-circle" />
                        </div>
                        <h5 class="mb-0"><%# Eval("Name") %></h5>
                        <small><%# Eval("Specialty") %></small>
                        <div class="ratings mt-2">
                            <i class="fa fa-star"></i>
                            <i class="fa fa-star"></i>
                            <i class="fa fa-star"></i>
                            <i class="fa fa-star"></i>
                        </div>
                        <div class="mt-4">
                            <button class="btn btn-success text-uppercase">Book Appointment</button>
                        </div>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
        <asp:LinkButton ID="lnkPrev" runat="server" OnClick="lnkPrev_Click">Anterior</asp:LinkButton>
        <asp:LinkButton ID="lnkNext" runat="server" OnClick="lnkNext_Click">Siguiente</asp:LinkButton>--%>
    <%--</div>--%>

    <h2 runat="server" id="tituloBuscador">Ultimas Publicaciones</h2>
    <a>Encuentra lo que buscas gracias a nuestra comunidad de profesionales calificados</a>
    <br />
    <hr />

    <div class="row col col-md-12 col-sm-12">

        <asp:Repeater ID="repeaterTarjetas" runat="server">
            <ItemTemplate>
                <div class="col-md-3 col-sm-6">
                    <div class="card p-2 py-3 text-center" style="text-align: center; align-items: center; justify-items: center">
                        <div class="img mb-2" style="width: 100%; height: 100px; overflow: hidden; border-radius: 50%;">
                            <img src='<%# Eval("imagenURL") %>' style="width: 100px; height: 100px;" class="card-img-top avatar">
                        </div>

                        <h5 style="font-weight: bold" class="mb-0"><%# Eval("Nombre") %></h5>
                        <medium><%# Eval("titulo") %></medium>
                        <div class="ratings mt-2" title="Perfil calificado: <%# Eval("total_registros") %> veces">
                            <%# GetStarRating((decimal)Eval("calificacion")) %>
                            <small><%# Eval("total_registros") %></small>


                        </div>
                        <div class="mt-4">
                            <asp:Button ID="btnPerfil" runat="server" OnClick="btnPerfil_Click" CssClass="btn btn-success text-uppercase"
                                Text="Ver Perfil" CommandArgument='<%# Eval("userID") %>' />
                        </div>
                    </div>
                </div>
            </ItemTemplate>

        </asp:Repeater>

        <div class="modal fade" id="modalPerfil" tabindex="-1" role="dialog" aria-labelledby="modalPerfilLabel" aria-hidden="true">
            <div class="modal-dialog modal-dialog-centered" role="document">
                <div class="modal-content">
                    <div class="modal-header bg-primary text-white">
                        <h5 class="modal-title" id="modalPerfilLabel">Perfil Profesional</h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Cerrar">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body">
                        <div class="text-center" style="display: flex; flex-direction: column; align-items: center; justify-content: center;">
                             <img src="#" runat="server" id="imgProfile2" style="width: 100px; height: 100px;" class="card-img-top avatar"/>
                            <h5 class="card-title" runat="server" id="NombrePerfil" style="font-weight: bold">Nombre</h5>
                            <div class="rating horizontal">
                                <%= GetStarRating(Convert.ToDecimal(hiddenCalificacion.Value)) %>
                                <!-- Reemplaza 3.5 con la calificación real -->
                                <%=hiddenCalificacion.Value %>
                            </div>
                            <asp:HiddenField ID="hiddenCalificacion" Value="0" runat="server" />

                            <div class="form-group">
                                <p runat="server" id="presentacion">Presentación de ejemplo</p>
                            </div>
                        </div>
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
                                <div class="container">

                                    <div class="row mt-3">

                                        <div class="col-md-6">

                                            <div class="form-group">
                                                <label for="titulo" class="form-label">Título de Perfil</label>
                                                <p runat="server" id="titulo">Título de ejemplo</p>
                                            </div>

                                            <div class="form-group">
                                                <label for="email" class="form-label">Email</label>
                                                <p runat="server" id="email">correo@example.com</p>
                                            </div>
                                            <div class="form-group">
                                                <label for="experiencia" class="form-label">Años de Experiencia</label>
                                                <p runat="server" id="experiencia">5 años</p>
                                            </div>
                                        </div>
                                        <div class="col-md-6">


                                            <div class="form-group ">
                                                <div class="horizontal">
                                                    <a id="enlaceWhatsapp1" runat="server" href="#" style="display: block; color: black">

                                                        <label for="telefono1" class="form-label">Teléfono 1</label>
                                                        <img runat="server" id="whatsapp1" visible="false" style="margin-left: 5px;" src="../Images/whatsapp.png" width="24" height="24" />
                                                    </a>
                                                </div>
                                                    <a id="enlaceTelefono1" runat="server" href="#" style="display: block; color: black">

                                                <p runat="server" id="telefono1">(000)-000-0000</p>
                                                </a>
                                            </div>
                                            <div class="form-group">
                                                <div class="horizontal">
                                                    <a id="enlaceWhatsapp2" runat="server" href="#" style="display: block; color: black">

                                                        <label for="telefono2" class="form-label">Teléfono 2</label>
                                                        <img runat="server" id="whatsapp2" visible="false" style="margin-left: 5px;" src="../Images/whatsapp.png" width="24" height="24" />
                                                    </a>
                                                </div>
                                                    <a id="enlaceTelefono2" runat="server" href="#" style="display: block; color: black">

                                                <p runat="server" id="telefono2">(000)-000-0000</p>
                                                </a>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="tab-pane fade" id="servicios">
                                <div class="container">
                                    <div class="form-group">
                                        <label for="servicio" class="form-label">Catalogo de Servicios</label>
                                   <asp:Repeater ID="repeaterServicios" runat="server">
    <HeaderTemplate>
        <table class="table table-striped">
            <thead>
                <tr>
                    <th>Detalle del Servicio</th>
                </tr>
            </thead>
            <tbody>
    </HeaderTemplate>
    <ItemTemplate>
        <tr>
            <td><%# Eval("detalleServicio") %></td>
        </tr>
    </ItemTemplate>
    <FooterTemplate>
        </tbody>
    </table>
    </FooterTemplate>
</asp:Repeater>

                                    </div>
                                </div>
                            </div>
                        </div>




                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-dismiss="modal">Cerrar</button>
                    </div>
                </div>
            </div>
        </div>

    </div>



    <script type='text/javascript' src='https://cdn.jsdelivr.net/npm/popper.js@1.16.0/dist/umd/popper.min.js'></script>
    <script type='text/javascript' src='https://stackpath.bootstrapcdn.com/bootstrap/5.0.0-alpha1/js/bootstrap.min.js'></script>
    <script src="https://code.jquery.com/jquery-3.1.1.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
</asp:Content>

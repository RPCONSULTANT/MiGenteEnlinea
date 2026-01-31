<%@ Page Title="" Language="C#" MasterPageFile="~/Empleador/dashboard.Master" AutoEventWireup="true" CodeBehind="miPerfilEmpleador.aspx.cs" Inherits="MiGente_Web.Empleador.miPerfilEmpleador" %>

<%@ Register Assembly="DevExpress.Web.Bootstrap.v22.2, Version=22.2.5.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.Bootstrap" TagPrefix="dx" %>
<%@ Register TagPrefix="dx" Namespace="DevExpress.Web" Assembly="DevExpress.Web.v22.2, Version=22.2.5.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container">
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <h3 style="color: white">Información de Perfil</h3>
        <div>
            <div class="form-control">



                <div class="mb-3">
                    <label for="tipoCuenta" class="form-label">Tipo de Cuenta</label>
                    <asp:DropDownList ID="tipoCuenta" Enabled="false" runat="server" CssClass="form-select">
                        <asp:ListItem Text="Empleador" Value="1"></asp:ListItem>
                        <asp:ListItem Text="Contratista" Value="2"></asp:ListItem>
                    </asp:DropDownList>
                </div>
                <asp:UpdatePanel ID="UpdatePanelModal" runat="server" UpdateMode="Always">
                    <ContentTemplate>
                        <div class="mb-3">
                            <label for="tipoIdentificacion" class="form-label">Tipo de Perfil</label>
                            <asp:DropDownList AutoPostBack="true" OnSelectedIndexChanged="ddlTipoIdentificacion_SelectedIndexChanged" ID="ddlTipoIdentificacion" runat="server" CssClass="form-select">
                                <asp:ListItem Text="Persona Fisica" Value="1"></asp:ListItem>
                                <asp:ListItem Text="Empresa" Value="2"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="mb-3">
                            <label for="identificacion" class="form-label">Informacion de Registro</label>

                            <dx:BootstrapTextBox ID="Nombre" ValidationSettings-RequiredField-IsRequired="true" ValidationSettings-ValidationGroup="perfil" NullText="Nombre" NullTextDisplayMode="UnfocusedAndFocused" runat="server"></dx:BootstrapTextBox>

                        </div>
                        <div class="mb-3">
                            <dx:BootstrapTextBox ID="Apellido" ValidationSettings-RequiredField-IsRequired="true" ValidationSettings-ValidationGroup="perfil" NullText="Apellido" NullTextDisplayMode="UnfocusedAndFocused" runat="server"></dx:BootstrapTextBox>

                        </div>
                        <div class="mb-3">
                            <label for="identificacion" class="form-label">RNC/Cedula</label>

                            <dx:BootstrapTextBox ValidationSettings-RequiredField-IsRequired="true" ValidationSettings-ValidationGroup="perfil" ID="txtIdentificacion" runat="server"></dx:BootstrapTextBox>
                        </div>
                        <div runat="server" id="divEmpresa" visible="false" class="mb-3">
                            <div class="mb-3">
                                <label for="nombreComercial" class="form-label">Informacion Comercial</label>

                                <dx:BootstrapTextBox ValidationSettings-RequiredField-IsRequired="true" ID="nombreComercial" ValidationSettings-ValidationGroup="perfil" NullText="Nombre Comercial" Enabled="false" runat="server"></dx:BootstrapTextBox>
                            </div>
                            <div class="mb-3">
                                <h4 class="col-12">Representante Legal</h4>

                            </div>
                            <div class="mb-3">
                                <label for="nombreGerente" class="form-label">Nombre</label>

                                <dx:BootstrapTextBox ValidationSettings-RequiredField-IsRequired="true" ID="txtNombreGerente" ValidationSettings-ValidationGroup="perfil" NullText="Nombre Representante Legal" Enabled="false" runat="server"></dx:BootstrapTextBox>
                            </div>
                            <div class="mb-3">
                                <label for="apellidoGerente" class="form-label">Apellido</label>

                                <dx:BootstrapTextBox ValidationSettings-RequiredField-IsRequired="true" ID="txtApellidoGerente" ValidationSettings-ValidationGroup="perfil" NullText="Apellido Representante Legal" Enabled="false" runat="server"></dx:BootstrapTextBox>
                            </div>
                            <div class="mb-3">
                                <label for="direccionGerente" class="form-label">Domicilio</label>

                                <dx:BootstrapTextBox ValidationSettings-RequiredField-IsRequired="true" ID="txtDireccionGerente" ValidationSettings-ValidationGroup="perfil" NullText="Direccion" MaxLength="250" Enabled="false" runat="server"></dx:BootstrapTextBox>
                            </div>
                        </div>
                        <div class="mb-3">
                            <label for="email" class="form-label">Email</label>

                            <dx:BootstrapTextBox ValidationSettings-RequiredField-IsRequired="true" ValidationSettings-ValidationGroup="perfil" ID="email" runat="server"></dx:BootstrapTextBox>
                        </div>
                        <div class="mb-3">
                            <label for="email" class="form-label">Telefonos</label>

                            <dx:BootstrapTextBox ID="telefono1" MaskSettings-Mask="(999)-000-0000" runat="server"></dx:BootstrapTextBox>
                        </div>
                        <div class="mb-3">
                            <dx:BootstrapTextBox ID="telefono2" MaskSettings-Mask="(999)-000-0000" runat="server"></dx:BootstrapTextBox>

                        </div>
                        <div class="mb-3">
                            <label for="direccion" class="form-label">Dirección</label>
                            <dx:BootstrapTextBox ValidationSettings-RequiredField-IsRequired="true" ValidationSettings-ValidationGroup="perfil" ID="direccion" MaxLength="250" runat="server"></dx:BootstrapTextBox>

                        </div>
                        <dx:BootstrapButton ID="btnGuardar" CssClasses-Control="bg-success" runat="server" ValidationGroup="perfil" CausesValidation="true" OnClick="btnGuardar_Click1" AutoPostBack="true" Text="Actualizar Perfil"></dx:BootstrapButton>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <hr />
            <div class="row">
                <h4 class="col-md-3">Credenciales de Acceso</h4>
                <button type="button" runat="server" id="btnNuevoUsuario" class="btn btn-primary col-md-1" data-bs-toggle="modal" data-bs-target="#modalNuevoUsuario">Nuevo</button>
            </div>
            <asp:Repeater ID="repeaterUsuarios" OnItemCommand="repeaterUsuarios_ItemCommand" runat="server">
                <HeaderTemplate>
                    <table class="table table-bordered table-striped">
                        <thead>
                            <tr>
                                <th style="display: none;">ID</th>
                                <th>Email</th>
                                <th>Password</th>
                                <th>Acciones</th>
                            </tr>
                        </thead>
                        <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td style="display: none;"><%# Eval("id") %></td>
                        <td><%# Eval("email") %></td>
                        <td><%# Eval("password") %></td>
                        <td>
                           <asp:Button runat="server" CssClass="btn btn-primary" Text="Editar" CommandName="Edit" CommandArgument='<%# Eval("id") + "," + Eval("email") + "," + Eval("password") %>' />
                <asp:Button runat="server" CssClass="btn btn-danger" Text="Eliminar" CommandName="Delete" CommandArgument='<%# Eval("id") + "," + Eval("email") + "," + Eval("password") %>' />
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    </tbody>
        </table>
                </FooterTemplate>
            </asp:Repeater>
        </div>
    </div>
    <!-- Modal edit credentials -->
<div class="modal fade" id="editModal" tabindex="-1" role="dialog" aria-labelledby="editModalLabel" aria-hidden="true">
    <div class="modal-dialog modal-sm" role="document">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title" id="editModalLabel">Modificar Contraseña</h5>
             
            </div>
            <div class="modal-body">
                <div class="input-group mt-3">
                    <asp:TextBox ID="txtModalPassword" runat="server" TextMode="Password" CssClass="form-control" placeholder="Password"></asp:TextBox>
                  
                </div>
            </div>
            <div class="modal-footer">
                <asp:Button ID="btnSaveChanges" runat="server" OnClick="btnSaveChanges_Click" CssClass="btn btn-primary" Text="Guardar cambios"  />
                <button type="button" class="btn btn-secondary" data-dismiss="modal">Cerrar</button>
            </div>
        </div>
    </div>
</div>
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
<script type="text/javascript">
    function togglePassword() {
        var passwordField = document.getElementById('<%= txtModalPassword.ClientID %>');
        if (passwordField.type === "password") {
            passwordField.type = "text";
        } else {
            passwordField.type = "password";
        }
    }

    function showModal() {
        $('#editModal').modal('show');
    }
</script>
    <!-- Modal Nuevo Usuario -->
    <div class="modal fade" id="modalNuevoUsuario" tabindex="-1" role="dialog" aria-labelledby="modalNuevoUsuarioLabel" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="modalNuevoUsuarioLabel">Nuevo Usuario</h5>

                </div>
                <div class="modal-body">
                    <!-- Formulario para agregar nuevo usuario -->
                    <div class="form" id="formNuevoUsuario">
                        <div class="form-group">
                            <label for="correo">Correo Electrónico</label>
                            <input type="email" class="form-control" id="correo" placeholder="Correo Electrónico">
                        </div>
                        <div class="form-group">
                            <label for="contrasena">Contraseña</label>
                            <input type="password" class="form-control" id="contrasena" placeholder="Contraseña">
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-dismiss="modal">Cancelar</button>
                    <button type="button" class="btn btn-primary" id="btnGuardarUsuario">Guardar</button>
                </div>
            </div>
        </div>
    </div>


    <script>
        function previewProfilePic(event) {
            var input = event.target;
            var preview = document.getElementById('profile-pic-preview');

            if (input.files && input.files[0]) {
                var reader = new FileReader();

                reader.onload = function (e) {
                    preview.src = e.target.result;
                    preview.style.display = 'block';
                }

                reader.readAsDataURL(input.files[0]);
            }
        }
    </script>
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>

    <asp:HiddenField ID="HiddenField1" runat="server" />
</asp:Content>

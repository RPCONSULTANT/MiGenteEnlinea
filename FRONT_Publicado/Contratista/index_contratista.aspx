<%@ Page Title="" Language="C#" MasterPageFile="~/Contratista/ContratistasM.Master" AutoEventWireup="true" CodeBehind="index_contratista.aspx.cs" Inherits="MiGente_Web.Contratista.index_contratista" %>

<%@ Register Assembly="DevExpress.Web.Bootstrap.v22.2, Version=22.2.5.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.Bootstrap" TagPrefix="dx" %>


<%@ Register Assembly="DevExpress.Web.v22.2, Version=22.2.5.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" UpdateMode="Conditional" runat="server">
        <ContentTemplate>

            <style>
                .avatar {
                    border-radius: 50%;
                    width: 150px; /* Ajusta el ancho y alto según tus necesidades */
                    height: 150px;
                    object-fit: cover; /* Para que la imagen se ajuste y no se deforme */
                }
            </style>

            <div class="container">
                <h3 class="text-white">Administrar Perfil Profesional</h3>

                <div class="row">
                    <div class="col-md-3" style="height: auto">
                        <div class="card" style="text-align: center;">
                            <div class="align-content-center justify-content-center" style="text-align: center; margin-top: 20px;">
                                <img runat="server"  src="https://via.placeholder.com/150" class="card-img-top avatar" id="profileImage" alt="Imagen de perfil">
                            </div>
                            <div class="card-body">
                                <h5 class="card-title">Imagen de Perfil</h5>
                                <asp:FileUpload ID="fileUpload" CssClass="form-control" ClientIDMode="Static" runat="server" />
                                <div class="mb-3" style="display: flex; flex-direction: column; align-items: center; text-align: center;">
                                    <label class="form-label">Calificación de Perfil</label>
                                    <dx:ASPxRatingControl ID="ratingPerfil" runat="server" ReadOnly="true"></dx:ASPxRatingControl>
                                    <a>0</a>
                                </div>
                            </div>
                             <script type="text/javascript">
                                 $(document).ready(function () {
                                     $('#fileUpload').change(function () {
                                         showSelectedImage(this);
                                     });
                                 });

                                 function showSelectedImage(input) {
                                     var image = document.getElementById("profileImage");

                                     if (input.files && input.files[0]) {
                                         var reader = new FileReader();
                                         reader.onload = function (e) {
                                             image.src = e.target.result;
                                         };
                                         reader.readAsDataURL(input.files[0]);
                                     }
                                 }
                             </script>
                        </div>
                    </div>

                    <div class="col-md-9 bg-white" style="height: auto">
                        <ul class="nav nav-tabs">
                            <li class="nav-item">
                                <a class="nav-link active" data-bs-toggle="tab" href="#datosGenerales">Datos Generales</a>
                            </li>
                            <li class="nav-item">
                                <a class="nav-link" data-bs-toggle="tab" href="#servicios">Catalogo de Servicios</a>
                            </li>

                        </ul>
                        <div class="tab-content">
                            <div class="tab-pane fade show active" id="datosGenerales">
                                <div class="col-md-12">
                                    <div class="card">

                                        <div class="card-body">
                                            <div class="form">
                                                <div class="row">
                                                    <div class="col-md-6">
                                                        <div class="mb-3">
                                                            <label for="titulo" class="form-label">Titulo de Perfil</label>
                                                            <dx:BootstrapTextBox ID="txtTitulo" runat="server"></dx:BootstrapTextBox>
                                                        </div>
                                                        <div class="mb-3">
                                                            <label for="nombre" class="form-label">Sector Profesional</label>
                                                            <asp:DropDownList OnDataBound="ddlSector_DataBound" ID="ddlSector" CssClass="form-select" runat="server" DataSourceID="linqSectores" DataTextField="sector" DataValueField="sector"></asp:DropDownList>
                                                        </div>

                                                        <div class="mb-3">
                                                            <label for="comentarios" class="form-label">Presentacion</label>
                                                            <dx:BootstrapMemo ID="presentacion" ValidationSettings-RequiredField-IsRequired="true" NullText="Agregue una breve presentacion para que sus posibles clientes le conozcan mejor." runat="server"></dx:BootstrapMemo>
                                                        </div>
                                                        <div class="mb-3">
                                                            <label for="email" class="form-label">Email</label>
                                                            <dx:BootstrapTextBox ID="txtEmail" runat="server"></dx:BootstrapTextBox>
                                                        </div>
                                                            <div class="mb-3 d-flex flex-row">
                                                            <div class="form-group ">
                                                                <label for="experiencia" class="form-label">Años de Experiencia</label>

                                                                <dx:BootstrapSpinEdit ID="txtExperiencia" NullText="0" SpinButtons-Enabled="false" SpinButtons-ShowIncrementButtons="false" Width="60px" runat="server"></dx:BootstrapSpinEdit>
                                                            </div>

                                                                  <div class="form-group ">
                                                                <label for="prov" class="form-label"></label>

                                                            </div>

                                                                  <div class="form-group ">
                                                                <label for="prov" class="form-label">Provincia</label>
                                                            <asp:DropDownList ID="comboProvincias" CssClass="form-select" runat="server" DataSourceID="linqProvincias" DataTextField="nombre" DataValueField="nombre"></asp:DropDownList>

                                                                <asp:LinqDataSource runat="server" EntityTypeName="" ID="linqProvincias" ContextTypeName="MiGente_Web.Data.migenteEntities" OrderBy="nombre" Select="new (nombre)" TableName="Provincias"></asp:LinqDataSource>
                                                            </div>
                                                        </div>
                                                         
                                                    </div>
                                                    <div class="col-md-6">

                                                        <div class=" row form-group">

                                                            <div class=" col-md-6">
                                                                <label for="telefono1" class="form-label">Tipo de Perfil</label>

                                                                <asp:DropDownList AutoPostBack="true" OnSelectedIndexChanged="ddlTipoPerfil_SelectedIndexChanged" ID="ddlTipoPerfil" CssClass="form-select" runat="server">
                                                                    <asp:ListItem Selected="True" Value="1">Persona Fisica</asp:ListItem>
                                                                    <asp:ListItem Value="2">Empresa</asp:ListItem>
                                                                </asp:DropDownList>

                                                            </div>
                                                            <div class="col-md-6">
                                                                <label for="telefono2" class="form-label">RNC/Cedula</label>
                                                                <dx:BootstrapTextBox ID="txtIdentificacion" runat="server"></dx:BootstrapTextBox>
                                                            </div>
                                                        </div>
                                                        <div runat="server" id="divTipoPersona" visible="false" class=" row form-group">

                                                            <div class=" col-md-6">
                                                                <label for="telefono1" class="form-label">Nombre</label>
                                                                <dx:BootstrapTextBox ID="txtNombre" runat="server"></dx:BootstrapTextBox>

                                                            </div>
                                                            <div class="col-md-6">

                                                                <label for="telefono2" class="form-label">Apellido</label>
                                                                <dx:BootstrapTextBox ID="txtApellido" runat="server"></dx:BootstrapTextBox>
                                                            </div>
                                                        </div>
                                                        <div runat="server" id="divTipoEmpresa" visible="false" class="row form-group">

                                                            <div class=" col-md-12">
                                                                <label for="telefono1" class="form-label">Razon Social</label>
                                                                <dx:BootstrapTextBox ID="txtRazonSocial" runat="server"></dx:BootstrapTextBox>

                                                            </div>

                                                        </div>
                                                                   <div class=" row form-group">
                                                            <div class=" col-md-6">

                                                                <label for="telefono1" class="form-label">Teléfono 1</label>
                                                                <dx:BootstrapTextBox ID="telefono1"  runat="server"></dx:BootstrapTextBox>
                                                            </div>
                                                            <div class=" col-md-6">
                                                                <label for="telefono2" class="form-label">Whatsapp?</label>
                                                                <dx:BootstrapCheckBox ID="chkWhatsapp1" runat="server"></dx:BootstrapCheckBox>
                                                                </div>
                                                                       </div>
                                                                   <div class=" row form-group">

                                                            <div class="col-md-6">
                                                                <label for="telefono2" class="form-label">Teléfono 2</label>
                                                                <dx:BootstrapTextBox ID="telefono2"  runat="server"></dx:BootstrapTextBox>

                                                            </div>
  <div class=" col-md-6">
                                                                <label for="telefono2" class="form-label">Whatsapp?</label>

                                                                <dx:BootstrapCheckBox ID="chkWhatsapp2" runat="server"></dx:BootstrapCheckBox>
                                                                </div>
                                                                       </div>

                                                    



                                                       
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="card-footer bg-secondary">
                                             <dx:BootstrapButton ID="btnGuardar" runat="server" AutoPostBack="false" OnClick="btnGuardar_Click" Text="Guardar Informacion"></dx:BootstrapButton>
                                                        <dx:BootstrapButton ID="btnEstatus" Visible="false" CssClasses-Control="bg-danger" runat="server" OnClick="btnEstatus_Click" AutoPostBack="false" Text="Desactivar Perfil"></dx:BootstrapButton>

                                        </div>
                                    </div>


                                </div>
                            </div>
                            <div class="tab-pane fade" id="servicios">
                                <br />
                                <a>Descripcion de Servicio</a>
                                <div class="input-group">
                                    <div class="col col-md-10 col-sm-12">
                              <dx:BootstrapTextBox ID="txtServicio" NullText="Ingrese la descripcion de servicio que ofrece" ValidationSettings-RequiredField-IsRequired="false"
                                   runat="server"></dx:BootstrapTextBox>
                                        </div>
                                    <div class="col col-md-2 col-sm-12 mx-auto">
                                             <dx:BootstrapButton ID="btnAgregar" runat="server" AutoPostBack="false" OnClick="btnAgregar_Click" Text="Agregar"></dx:BootstrapButton>

</div>
                                    </div>
                                <div class="card-body">
                                
                                    <hr>
                                    <h4 class="card-title">Servicios</h4>
                                    <div class="form">
                                        <div class="mb-3">
                                            <dx:BootstrapGridView ID="gridServicios" runat="server" AutoGenerateColumns="False">
                                                <Columns>
                                                    <dx:BootstrapGridViewTextColumn  FieldName="servicioID" Visible="false"></dx:BootstrapGridViewTextColumn>
                                                    <dx:BootstrapGridViewTextColumn FieldName="detalleServicio" Caption="Descripcion" VisibleIndex="0"></dx:BootstrapGridViewTextColumn>

                                                    <dx:BootstrapGridViewCommandColumn VisibleIndex="1">
                                                        <CustomButtons>
                                                            <dx:BootstrapGridViewCommandColumnCustomButton IconCssClass="fa fa-remove" CssClass="btn bg-danger text-white" ID="btnRemover" Text="Remover"></dx:BootstrapGridViewCommandColumnCustomButton>
                                                            <dx:BootstrapGridViewCommandColumnCustomButton></dx:BootstrapGridViewCommandColumnCustomButton>
                                                        </CustomButtons>
                                                    </dx:BootstrapGridViewCommandColumn>
                                                </Columns>
                                            </dx:BootstrapGridView>
                                        </div>
                                    </div>
                                </div>

                            </div>

                        </div>
                    </div>
                </div>
                <asp:HiddenField ID="HiddenField1" runat="server" />
                <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
            


                <asp:LinqDataSource ID="linqSectores" runat="server" ContextTypeName="MiGente_Web.Data.migenteEntities" EntityTypeName="" OrderBy="sector" TableName="Sectores"></asp:LinqDataSource>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnGuardar" />
            <asp:PostBackTrigger ControlID="btnAgregar" />
        </Triggers>
    </asp:UpdatePanel>
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@10"></script>

<script type="text/javascript">
    function MostrarAlerta() {
        Swal.fire({
            title: 'Perfil Actualizado',
            text: 'Tu perfil ha sido actualizado correctamente.',
            icon: 'success',
            confirmButtonText: 'OK'
        });
    }
</script>

    <script type="text/javascript">
        function MostrarAlerta2() {
            Swal.fire({
                title: 'Perfil Activado',
                text: 'Todos los miembros de la comunidad podran ver tu perfil en el directorio.',
                icon: 'success',
                confirmButtonText: 'OK'
            });
        }
    </script>
    <script type="text/javascript">
        function MostrarAlerta3() {
            Swal.fire({
                title: 'Perfil Desactivado',
                text: 'Nadie podra ver tu perfil publicado en el directorio.',
                icon: 'alert',
                confirmButtonText: 'OK'
            });
        }
    </script>
</asp:Content>

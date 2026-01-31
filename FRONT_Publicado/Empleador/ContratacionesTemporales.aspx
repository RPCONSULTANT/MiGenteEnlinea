<%@ Page Title="" Language="C#" MasterPageFile="~/Empleador/dashboard.Master" AutoEventWireup="true" CodeBehind="ContratacionesTemporales.aspx.cs" Inherits="MiGente_Web.Empleador.ContratacionesTemporales" %>

<%@ Register Assembly="DevExpress.Web.Bootstrap.v22.2, Version=22.2.5.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.Bootstrap" TagPrefix="dx" %>
<%@ Register TagPrefix="dx" Namespace="DevExpress.Web" Assembly="DevExpress.Web.v22.2, Version=22.2.5.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <div class="container bg-white col col-md-12 col-xs-12 pt-2" style="border-radius: 5px;">
        <div class="mb-3">
            <h3>Gestión de Contrataciones Temporales</h3>
        </div>

        <ul class="nav nav-tabs mb-3" id="tabsEmpleados" role="tablist">

            <li class="nav-item" role="presentation">
                <a class="nav-link active" id="contratacionesActivas-tab" data-bs-toggle="tab" href="#contratacionesActivas" role="tab" aria-controls="contratacionesActivas" aria-selected="true">Contratacions Activas</a>
            </li>
            <li class="nav-item" role="presentation">
                <a class="nav-link" id="historial-tab" data-bs-toggle="tab" href="#historial" role="tab" aria-controls="historial" aria-selected="false">Historial</a>
            </li>
        </ul>
        <div class="tab-content" id="tabContent">
            <div class="tab-pane fade show active" id="contratacionesActivas" role="tabpanel" aria-labelledby="contratacionesActivas-tab">

                <div>
                    <p>Registrar y gestiona la contratacion de servicios de una manera centralizada y simple.</p>
                </div>
                <hr />
                <div class="row button-group">
                    <dx:BootstrapButton ID="btnNew" runat="server" AutoPostBack="false" OnClick="btnNew_Click"  CssClasses-Control="bg-primary col-xs-12 col-md-2  mx-1"
                        CssClasses-Icon="fa fa-plus" Text="Registrar Nuevo">
                    </dx:BootstrapButton>

                    <dx:BootstrapButton ID="btn" runat="server" AutoPostBack="false" CssClasses-Control="bg-warning col-xs-12 col-md-2 mx-1"
                        CssClasses-Icon="fa fa-file" Text="Exportar">
                    </dx:BootstrapButton>
                </div>
                <hr />



                <asp:HiddenField ID="HiddenField1" runat="server" />
                <asp:LinqDataSource runat="server" EntityTypeName="" ID="LinqDataSource1" ContextTypeName="MiGente_Web.Data.migenteEntities" TableName="EmpleadosTemporales" Where="userID == @userID">
                    <WhereParameters>
                        <asp:ControlParameter ControlID="HiddenField1" PropertyName="Value" DefaultValue="0" Name="userID" Type="String"></asp:ControlParameter>

                    </WhereParameters>
                </asp:LinqDataSource>
                <a></a>
                <h3>Proveedores Registrados</h3>
                <hr />
                <dx:BootstrapGridView ID="gridActivas" OnCustomButtonCallback="gridActivas_CustomButtonCallback" runat="server" DataSourceID="ObjectDataSource1" EnableRowsCache="false"
                    AutoGenerateColumns="False">
                    <SettingsAdaptivity AdaptivityMode="HideDataCells" AllowOnlyOneAdaptiveDetailExpanded="true"></SettingsAdaptivity>
                    <Settings ShowFooter="true" />
                    <SettingsDataSecurity AllowEdit="true" />
                    <CssClasses Row="grid-nowrap-row" Control="grid-borderless" />
                    <Columns>
                        <dx:BootstrapGridViewTextColumn FieldName="contratacionID" Visible="false" VisibleIndex="0"></dx:BootstrapGridViewTextColumn>
                        <dx:BootstrapGridViewDateColumn FieldName="fechaRegistro" Caption="Fecha Registro" AdaptivePriority="3" VisibleIndex="1"></dx:BootstrapGridViewDateColumn>
                        <dx:BootstrapGridViewTextColumn FieldName="nombre" AdaptivePriority="0" VisibleIndex="2" Caption="Nombre"></dx:BootstrapGridViewTextColumn>
                        <dx:BootstrapGridViewTextColumn FieldName="identificacion" Caption="RNC/Cedula" AdaptivePriority="4" VisibleIndex="3"></dx:BootstrapGridViewTextColumn>
                        <dx:BootstrapGridViewTextColumn FieldName="telefono1" VisibleIndex="4"></dx:BootstrapGridViewTextColumn>
                        <dx:BootstrapGridViewTextColumn FieldName="telefono2" VisibleIndex="5"></dx:BootstrapGridViewTextColumn>

                        <dx:BootstrapGridViewCommandColumn AdaptivePriority="2" VisibleIndex="8">
                            <CustomButtons>
                                <dx:BootstrapGridViewCommandColumnCustomButton IconCssClass="fa fa-profile" CssClass="bg-primary text-white" ID="btnFicha" Text="Ver Ficha"></dx:BootstrapGridViewCommandColumnCustomButton>
                            </CustomButtons>
                        </dx:BootstrapGridViewCommandColumn>
                    </Columns>
                    <SettingsPager PageSize="10" />

                </dx:BootstrapGridView>
                <asp:ObjectDataSource runat="server" ID="ObjectDataSource1" SelectMethod="obtenerTodosLosTemporales" TypeName="MiGente_Web.Servicios.EmpleadosService">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="HiddenField1" PropertyName="Value" DefaultValue="0" Name="userID" Type="String"></asp:ControlParameter>
                    </SelectParameters>
                </asp:ObjectDataSource>
                <hr />
            </div>
            <div class="tab-pane fade" id="historial" role="tabpanel" aria-labelledby="historial-tab">
                <div>
                    <p>Registrar y gestiona tus empleados de una manera centralizada y simple.</p>
                </div>
                <hr />
                <div class="row button-group">
                    <dx:BootstrapButton ID="BootstrapButton1" runat="server" AutoPostBack="false" data-bs-toggle="modal" data-bs-target="#registroContratacionModal" CssClasses-Control="bg-primary col-xs-12 col-md-2  mx-1"
                        CssClasses-Icon="fa fa-plus" Text="Registrar Nuevo">
                    </dx:BootstrapButton>

                    <dx:BootstrapButton ID="BootstrapButton3" runat="server" AutoPostBack="false" CssClasses-Control="bg-warning col-xs-12 col-md-2 mx-1"
                        CssClasses-Icon="fa fa-file" Text="Exportar">
                    </dx:BootstrapButton>
                </div>
                <hr />
                <%--  <dx:BootstrapGridView ID="gridInactivos" runat="server" OnCustomButtonCallback="gridEmpleados_CustomButtonCallback1" OnHtmlDataCellPrepared="gridEmpleados_HtmlDataCellPrepared" DataSourceID="LinqDataSource2" AutoGenerateColumns="False"
            CssClasses-Table="table table-striped table-bordered">
            <Columns>
                <dx:BootstrapGridViewTextColumn FieldName="empleadoID" Visible="false" VisibleIndex="0"></dx:BootstrapGridViewTextColumn>
                <dx:BootstrapGridViewTextColumn FieldName="userID" Visible="false" VisibleIndex="1"></dx:BootstrapGridViewTextColumn>
                <dx:BootstrapGridViewDateColumn FieldName="fechaRegistro" Caption="Registro" VisibleIndex="13">
                </dx:BootstrapGridViewDateColumn>
                <dx:BootstrapGridViewDateColumn FieldName="fechaInicio" Caption="Inicio" VisibleIndex="10" />

                <dx:BootstrapGridViewTextColumn FieldName="identificacion" Caption="Identificacion" VisibleIndex="3"></dx:BootstrapGridViewTextColumn>
                <dx:BootstrapGridViewTextColumn FieldName="Nombre" VisibleIndex="4"></dx:BootstrapGridViewTextColumn>
                <dx:BootstrapGridViewTextColumn FieldName="Apellido" VisibleIndex="5"></dx:BootstrapGridViewTextColumn>
                <dx:BootstrapGridViewTextColumn FieldName="salario" PropertiesTextEdit-DisplayFormatString="C2" VisibleIndex="11"></dx:BootstrapGridViewTextColumn>
                <dx:BootstrapGridViewTextColumn FieldName="periodoPago" VisibleIndex="12"></dx:BootstrapGridViewTextColumn>
                <dx:BootstrapGridViewCommandColumn VisibleIndex="14">
                    <CustomButtons>
                        <dx:BootstrapGridViewCommandColumnCustomButton IconCssClass="fa fa-user" CssClass="bg-primary text-white" ID="btnFicha2" Text="Ver ficha"></dx:BootstrapGridViewCommandColumnCustomButton>
                    </CustomButtons>
                </dx:BootstrapGridViewCommandColumn>
            </Columns>

            <SettingsPager PageSize="10"></SettingsPager>
            <Settings ShowFilterRow="true" ShowHeaderFilterButton="true"></Settings>
            <SettingsBehavior AllowSort="true" AllowDragDrop="true"></SettingsBehavior>
            <SettingsEditing Mode="Inline"></SettingsEditing>

            <SettingsAdaptivity AdaptivityMode="HideDataCells"></SettingsAdaptivity>
        </dx:BootstrapGridView>--%>



                <asp:HiddenField ID="HiddenField2" runat="server" />
                <asp:LinqDataSource runat="server" EntityTypeName="" ID="LinqDataSource2" ContextTypeName="MiGente_Web.Data.migenteEntities" TableName="Empleados" Where="userID == @userID AND Activo=@activo">
                    <WhereParameters>
                        <asp:ControlParameter ControlID="HiddenField1" PropertyName="Value" DefaultValue="0" Name="userID" Type="String"></asp:ControlParameter>
                        <asp:Parameter DefaultValue="False" Name="activo" Type="Boolean" />

                    </WhereParameters>
                </asp:LinqDataSource>

                <hr />
            </div>
        </div>

    </div>

    <!-- Modal -->
    <div class="modal fade" id="registroContratacionModal" tabindex="-1" aria-labelledby="registroContratacionModalLabel" aria-hidden="true">
        <div class=" modal-dialog modal-sm">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="registroContratacionModalLabel">Nueva Contratacion de Servicio</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <!-- Formulario de registro de empleado -->
               
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                            <div class="focus">
                                <div class="mb-3">

                                    <asp:DropDownList ValidationGroup="nuevoEmpleado" AutoPostBack="true" OnSelectedIndexChanged="ddlTipo_SelectedIndexChanged" ID="ddlTipo" runat="server" CssClass="form-select">
                                        <asp:ListItem Text="Persona Fisica" Value="1" Selected="True"></asp:ListItem>
                                        <asp:ListItem Value="2">Empresa</asp:ListItem>
                                    </asp:DropDownList>
                                </div>

                                <div class="mb-3" id="divPersonaFisica" runat="server" visible="true">
                                    <a>Informacion General</a>

                                        <div class="mb-3 content-center" style="text-align:center" runat="server" visible="false" id="divFoto">
                              <asp:Image ID="Image1" runat="server" Width="200px" ImageAlign="Middle"  />
                              </div>

                                    <div class="mb-3">
                                        <label for="identificacion" class="form-label"># Identificación</label>
                                        <dx:BootstrapTextBox ValidationSettings-ValidationGroup="nuevoEmpleado" OnTextChanged="txtIdentificacion_TextChanged" ValidationSettings-RequiredField-IsRequired="true" ID="txtIdentificacion" runat="server"></dx:BootstrapTextBox>
                                    </div>
                                        <asp:UpdateProgress ID="UpdateProgress1" AssociatedUpdatePanelID="UpdatePanel1" 
                         runat="server">
                        <ProgressTemplate>
                            <a>Consultando Padron Electoral</a>
                        </ProgressTemplate>
                    </asp:UpdateProgress>

                          <div class="mb-3">
                            <dx:BootstrapButton ID="btnBuscarCedula" Width="100%" OnClick="btnBuscarCedula_Click" runat="server" AutoPostBack="false" Text="Validar Cedula"></dx:BootstrapButton>

                          </div>
                                    <div class="mb-3">
                                        <label for="nombre" class="form-label">Nombre</label>
                                        <dx:BootstrapTextBox ValidationSettings-ValidationGroup="nuevoEmpleado" ValidationSettings-RequiredField-IsRequired="true" ID="nombre" runat="server"></dx:BootstrapTextBox>
                                    </div>
                                    <div class="mb-3">
                                        <label for="apellido" class="form-label">Apellido</label>
                                        <dx:BootstrapTextBox ID="apellido" ValidationSettings-ValidationGroup="nuevoEmpleado" ValidationSettings-RequiredField-IsRequired="true" runat="server"></dx:BootstrapTextBox>
                                    </div>
                                    <div class="mb-3">
                                        <label for="alias" class="form-label">Alias/Apodo</label>
                                        <dx:BootstrapTextBox ID="alias" runat="server"></dx:BootstrapTextBox>
                                    </div>
                                </div>
                                <div class="mb-3" id="divEmpresa" runat="server" visible="false">
                                    <a>Informacion General</a>
                                    <div class="mb-3">
                                        <label for="rnc" class="form-label">RNC</label>
                                        <dx:BootstrapTextBox ValidationSettings-ValidationGroup="nuevoEmpleado"
                                            ValidationSettings-RequiredField-IsRequired="true" ID="rnc" OnTextChanged="rnc_TextChanged" runat="server">
                                        </dx:BootstrapTextBox>
                                    </div>
                                    <div class="mb-3">
                                        <label for="nombre" class="form-label">Razon Social</label>
                                        <dx:BootstrapTextBox ValidationSettings-ValidationGroup="nuevoEmpleado"
                                            ValidationSettings-RequiredField-IsRequired="true" ID="razonSocial" runat="server">
                                        </dx:BootstrapTextBox>
                                    </div>
                                    <a>Representante Legal</a>
                                    <div class="mb-3">
                                        <label for="apellido" class="form-label">Nombre</label>
                                        <dx:BootstrapTextBox ID="txtNombreRepresentante" ValidationSettings-ValidationGroup="nuevoEmpleado" ValidationSettings-RequiredField-IsRequired="true" runat="server"></dx:BootstrapTextBox>
                                    </div>
                                    <div class="mb-3">
                                        <label for="alias" class="form-label">Cedula</label>
                                        <dx:BootstrapTextBox ID="txtCedulaRepresentante" runat="server"></dx:BootstrapTextBox>
                                    </div>
                                </div>

                                <a>Otros Detalles</a>
                                <div class="mb-3">
                                    <label for="direccion" class="form-label">Dirección</label>
                                    <dx:BootstrapTextBox ID="direccion" MaxLength="250" ValidationSettings-ValidationGroup="nuevoEmpleado" ValidationSettings-RequiredField-IsRequired="true" runat="server"></dx:BootstrapTextBox>
                                </div>
                                <div class="mb-3">
                                    <label for="provincia" class="form-label">Provincia</label>
                                    <dx:BootstrapTextBox ID="provincia" ValidationSettings-ValidationGroup="nuevoEmpleado" ValidationSettings-RequiredField-IsRequired="true" runat="server"></dx:BootstrapTextBox>
                                </div>
                                <div class="mb-3">
                                    <label for="municipio" class="form-label">Municipio</label>

                                    <dx:BootstrapTextBox ID="municipio" ValidationSettings-ValidationGroup="nuevoEmpleado" ValidationSettings-RequiredField-IsRequired="true" runat="server"></dx:BootstrapTextBox>

                                </div>
                                <div class="mb-3">
                                    <label for="telefono1" class="form-label">Teléfono 1</label>

                                    <dx:BootstrapTextBox ID="telefono1" ValidationSettings-ValidationGroup="nuevoEmpleado" ValidationSettings-RequiredField-IsRequired="true" runat="server"></dx:BootstrapTextBox>

                                </div>
                                <div class="mb-3">
                                    <label for="telefono2" class="form-label">Teléfono 2</label>
                                    <dx:BootstrapTextBox ID="telefono2" runat="server"></dx:BootstrapTextBox>


                                </div>


                                <hr />
                                <a>Detalles de Contratacion</a>
                                <div class="mb-3">
                                    <label for="nombreContacto" class="form-label">Descripcion Corta</label>
                                    <dx:BootstrapTextBox MaxLength="60" ID="descripcionCortaTrabajo" ValidationSettings-RequiredField-IsRequired="true"
                                        ValidationSettings-ValidationGroup="nuevoEmpleado" runat="server"></dx:BootstrapTextBox>


                                </div>
                                <div class="mb-3">
                                    <label for="nombreContacto" class="form-label">Descripcion Ampliada</label>
                                    <dx:BootstrapMemo ID="descripcionAmpliada" MaxLength="250" runat="server"></dx:BootstrapMemo>

                                </div>


                                <div class="mb-3">
                                    <label for="fechaInicio" class="form-label">Fecha de Inicio</label>
                                    <dx:ASPxDateEdit ID="fechaInicio" ValidationSettings-RequiredField-IsRequired="true" ValidationSettings-ValidationGroup="nuevoEmpleado" CssClass="form-control" runat="server"></dx:ASPxDateEdit>
                                </div>
                                <div class="mb-3">
                                    <label for="fechaInicio" class="form-label">Fecha de Conclusión Esperada</label>
                                    <dx:ASPxDateEdit ID="fechaConclusion" ValidationSettings-RequiredField-IsRequired="true" ValidationSettings-ValidationGroup="nuevoEmpleado" CssClass="form-control" runat="server"></dx:ASPxDateEdit>
                                </div>

                                <div class="mb-3">
                                    <label for="montoAcordado" class="form-label">Monto Total Acordado</label>
                                    <dx:BootstrapTextBox ID="montoAcordado" NullText="0.00" NullTextDisplayMode="UnfocusedAndFocused" ValidationSettings-ValidationGroup="nuevoEmpleado" ValidationSettings-RequiredField-IsRequired="true" runat="server"></dx:BootstrapTextBox>


                                </div>

                                <div class="mb-3">
                                    <label for="activo" class="form-label">Esquema de Pagos</label>
                                    <asp:DropDownList ID="ddlEsquema" CssClass="form-select" ValidationGroup="nuevoEmpleado" runat="server">
                                        <asp:ListItem Value="1">100 % Contra Entrega</asp:ListItem>
                                        <asp:ListItem Selected="True" Value="2">50% Avance/50% Finalizado</asp:ListItem>
                                        <asp:ListItem Value="3">30% Avance/70% Finalizado</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                                <hr />

                            </div>
                        </ContentTemplate>
                  
                    </asp:UpdatePanel>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cerrar</button>
                    <dx:BootstrapButton ValidationGroup="nuevoEmpleado" OnClick="btnCrear_Click" CssClasses-Control="bg-success" CausesValidation="true" ID="btnCrear" runat="server" AutoPostBack="false" Text="Guardar"></dx:BootstrapButton>
                </div>
            </div>
        </div>
    </div>
        <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11.1.4/dist/sweetalert2.all.min.js"></script>
       <script>
           function registroContratacionModal() {
            var modal = document.getElementById('registroContratacionModal');

            if (modal) {
                var modalBootstrap = new bootstrap.Modal(modal);
                modalBootstrap.show();
            } else {
                console.error('El modal no se encontró en la página.');
            }
        }
       </script>
</asp:Content>

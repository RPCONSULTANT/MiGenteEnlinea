<%@ Page Title="" Language="C#" MasterPageFile="~/Empleador/dashboard.Master" AutoEventWireup="true" CodeBehind="empleados.aspx.cs" Inherits="MiGente_Web.Empleador.empleados" %>

<%@ Register Assembly="DevExpress.Web.Bootstrap.v22.2, Version=22.2.5.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.Bootstrap" TagPrefix="dx" %>


<%@ Register TagPrefix="dx" Namespace="DevExpress.Web" Assembly="DevExpress.Web.v22.2, Version=22.2.5.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <div class="container bg-white col col-md-12 col-xs-12 pt-2" style="border-radius: 5px;">
        <div class="mb-3">
            <h2>Gestión de Colaboradores Fijos</h2>
        </div>

        <ul class="nav nav-tabs mb-3" id="tabsEmpleados" role="tablist">
    <li class="nav-item" role="presentation">
        <a class="nav-link active" id="empleadosActivos-tab" data-bs-toggle="tab" href="#empleadosActivos" role="tab" aria-controls="empleadosActivos" aria-selected="true">Colaboradores Activos</a>
    </li>
    <li class="nav-item" role="presentation">
        <a class="nav-link" id="empleadosInactivos-tab" data-bs-toggle="tab" href="#empleadosInactivos" role="tab" aria-controls="empleadosInactivos" aria-selected="false">Colaboradores Inactivos</a>
    </li>
</ul>

<div class="tab-content" id="tabContent">
    <div class="tab-pane fade show active" id="empleadosActivos" role="tabpanel" aria-labelledby="empleadosActivos-tab">
      
        <div>
            <p>Registrar y gestiona tus colaboradores de una manera centralizada y simple.</p>
        </div>
        <hr />
        <div class="row button-group">
            <dx:BootstrapButton ID="btnNew" runat="server" AutoPostBack="false" OnClick="btnNew_Click" CssClasses-Control="bg-primary col-xs-12 col-md-2  mx-1"
                CssClasses-Icon="fa fa-plus" Text="Registrar Nuevo">
            </dx:BootstrapButton>
            <dx:BootstrapButton ID="btnNomina" runat="server" AutoPostBack="false" CssClasses-Control="bg-secondary col-xs-12 col-md-3  mx-1"
                CssClasses-Icon="fa fa-money" OnClick="btnNomina_Click" Text="Gestión de Nómina">
            </dx:BootstrapButton>
            <dx:BootstrapButton ID="btn" runat="server" AutoPostBack="false" CssClasses-Control="bg-warning col-xs-12 col-md-2 mx-1"
                CssClasses-Icon="fa fa-file" Text="Exportar">
            </dx:BootstrapButton>
        </div>
        <hr />
        <dx:BootstrapGridView ID="gridEmpleados" runat="server" OnCustomButtonCallback="gridEmpleados_CustomButtonCallback" OnHtmlDataCellPrepared="gridEmpleados_HtmlDataCellPrepared" DataSourceID="LinqDataSource1" AutoGenerateColumns="False"
            CssClasses-Table="table table-striped table-bordered">
            <Columns>
                <dx:BootstrapGridViewTextColumn FieldName="empleadoID" Visible="false" VisibleIndex="0"></dx:BootstrapGridViewTextColumn>
                <dx:BootstrapGridViewTextColumn FieldName="userID" Visible="false" VisibleIndex="1"></dx:BootstrapGridViewTextColumn>
                <dx:BootstrapGridViewDateColumn FieldName="fechaRegistro" Caption="Fecha Salida" VisibleIndex="13">
                </dx:BootstrapGridViewDateColumn>
                <dx:BootstrapGridViewDateColumn FieldName="fechaInicio" Caption="Inicio" VisibleIndex="10" />

                <dx:BootstrapGridViewTextColumn FieldName="identificacion" Caption="Identificacion" VisibleIndex="3"></dx:BootstrapGridViewTextColumn>
                <dx:BootstrapGridViewTextColumn FieldName="Nombre" VisibleIndex="4"></dx:BootstrapGridViewTextColumn>
                <dx:BootstrapGridViewTextColumn FieldName="Apellido" VisibleIndex="5"></dx:BootstrapGridViewTextColumn>
                <dx:BootstrapGridViewTextColumn FieldName="salario" PropertiesTextEdit-DisplayFormatString="C2" VisibleIndex="11"></dx:BootstrapGridViewTextColumn>
                <dx:BootstrapGridViewTextColumn FieldName="periodoPago" VisibleIndex="12"></dx:BootstrapGridViewTextColumn>
                <dx:BootstrapGridViewCommandColumn VisibleIndex="14">
                    <CustomButtons>
                        <dx:BootstrapGridViewCommandColumnCustomButton IconCssClass="fa fa-user" CssClass="bg-primary text-white" ID="btnFicha" Text="Ver ficha"></dx:BootstrapGridViewCommandColumnCustomButton>
                    </CustomButtons>
                </dx:BootstrapGridViewCommandColumn>
            </Columns>

            <SettingsPager PageSize="10"></SettingsPager>
            <Settings ShowFilterRow="true" ShowHeaderFilterButton="true"></Settings>
            <SettingsBehavior AllowSort="true" AllowDragDrop="true"></SettingsBehavior>
            <SettingsEditing Mode="Inline"></SettingsEditing>

            <SettingsAdaptivity AdaptivityMode="HideDataCells"></SettingsAdaptivity>
        </dx:BootstrapGridView>



        <asp:HiddenField ID="HiddenField1" runat="server" />
        <asp:LinqDataSource runat="server" EntityTypeName="" ID="LinqDataSource1" ContextTypeName="MiGente_Web.Data.migenteEntities" TableName="Empleados" Where="userID == @userID AND Activo=@activo">
            <WhereParameters>
                <asp:ControlParameter ControlID="HiddenField1" PropertyName="Value" DefaultValue="0" Name="userID" Type="String"></asp:ControlParameter>
                          <asp:Parameter DefaultValue="True" Name="activo" Type="Boolean" />

            </WhereParameters>
        </asp:LinqDataSource>

        <hr />
    </div>
      <div class="tab-pane fade" id="empleadosInactivos" role="tabpanel" aria-labelledby="empleadosActivos-tab">
      
        <div>
            <p>Registrar y gestiona tus empleados de una manera centralizada y simple.</p>
        </div>
        <hr />
        <div class="row button-group">
            <dx:BootstrapButton ID="BootstrapButton1" runat="server" AutoPostBack="false" data-bs-toggle="modal" data-bs-target="#registroEmpleadoModal" CssClasses-Control="bg-primary col-xs-12 col-md-2  mx-1"
                CssClasses-Icon="fa fa-plus" Text="Registrar Nuevo">
            </dx:BootstrapButton>
          
            <dx:BootstrapButton ID="BootstrapButton3" runat="server" AutoPostBack="false" CssClasses-Control="bg-warning col-xs-12 col-md-2 mx-1"
                CssClasses-Icon="fa fa-file" Text="Exportar">
            </dx:BootstrapButton>
        </div>
        <hr />
        <dx:BootstrapGridView ID="gridInactivos" runat="server" OnCustomButtonCallback="gridEmpleados_CustomButtonCallback1" OnHtmlDataCellPrepared="gridEmpleados_HtmlDataCellPrepared" DataSourceID="LinqDataSource2" AutoGenerateColumns="False"
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
        </dx:BootstrapGridView>



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
    <div class="modal fade" id="registroEmpleadoModal" tabindex="-1" aria-labelledby="registroEmpleadoModalLabel" aria-hidden="true">
        <div class=" modal-dialog modal-sm">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="registroEmpleadoModalLabel">Registro de Empleado</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                            
                    <!-- Formulario de registro de empleado -->
                    <div class="focus">


                        <div class="mb-3">
                            <label for="fechaInicio" class="form-label">Fecha de Inicio</label>
                            <dx:ASPxDateEdit ID="fechaDeIngreso" ValidationSettings-RequiredField-IsRequired="true" ValidationSettings-ValidationGroup="nuevoEmpleado" CssClass="form-control" runat="server"></dx:ASPxDateEdit>
                        </div>
                          <div class="mb-3 content-center" style="text-align:center" runat="server" visible="false" id="divFoto">
                              <asp:Image ID="Image1" runat="server" Width="200px" ImageAlign="Middle"  />
                              </div>
                       
                        <div class="mb-3">
                            <label for="identificacion" class="form-label"># Identificación</label>
                            <dx:BootstrapTextBox ValidationSettings-ValidationGroup="nuevoEmpleado" ValidationSettings-RequiredField-IsRequired="true" ID="txtIdentificacion" runat="server"></dx:BootstrapTextBox>
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

                        <div class="mb-3">
                            <label for="ddlEstadoCivil" class="form-label">Estado Civil</label>
                            <asp:DropDownList ID="ddlEstadoCivil" CssClass="form-select" runat="server">
                                <asp:ListItem Value="1">Soltero(a)</asp:ListItem>
                                <asp:ListItem Value="2">Casado(a)</asp:ListItem>
                            </asp:DropDownList>
                        </div>

                        <div class="mb-3">
                            <label for="nacimiento" class="form-label">Fecha de Nacimiento</label>
                            <dx:ASPxDateEdit ID="nacimiento" AllowUserInput="true" CssClass="form-control" ValidationSettings-ValidationGroup="nuevoEmpleado" ValidationSettings-RequiredField-IsRequired="true" runat="server"></dx:ASPxDateEdit>
                        </div>
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
                        <div class="mb-3">
                            <label for="telefono2" class="form-label">Posicion que Ocupa</label>

                            <dx:BootstrapTextBox ID="posicion" ValidationSettings-ValidationGroup="nuevoEmpleado" ValidationSettings-RequiredField-IsRequired="true" runat="server"></dx:BootstrapTextBox>

                        </div>
                        <div class="mb-3">
                            <label for="salario" class="form-label">Salario Bruto</label>
                            <dx:BootstrapTextBox ID="salario" NullText="0.00" NullTextDisplayMode="UnfocusedAndFocused" ValidationSettings-ValidationGroup="nuevoEmpleado" ValidationSettings-RequiredField-IsRequired="true" runat="server"></dx:BootstrapTextBox>


                        </div>
                        <hr />

                        <a>Otras Remuneraciones</a>
                        <div class="mb-3">
                            <label for="nombreRemuneracion1" class="form-label">Descripcion</label>
                            <dx:BootstrapTextBox ID="nombreRemuneracion1"  runat="server"></dx:BootstrapTextBox>


                        </div>
                        <div class="mb-3">
                            <label for="montoRemuneracion1" class="form-label">Monto</label>
                            <dx:BootstrapTextBox ID="montoRemuneracion1" NullText="0.00" runat="server"></dx:BootstrapTextBox>


                        </div>
                        <div class="mb-3">
                            <label for="nombreRemuneracion2" class="form-label">Descripcion</label>
                            <dx:BootstrapTextBox ID="nombreRemuneracion2" runat="server"></dx:BootstrapTextBox>


                        </div>
                        <div class="mb-3">
                            <label for="montoRemuneracion2" class="form-label">Monto</label>
                            <dx:BootstrapTextBox ID="montoRemuneracion2" NullText="0.00"  runat="server"></dx:BootstrapTextBox>


                        </div>
                        <div class="mb-3">
                            <label for="nombreRemuneracion2" class="form-label">Descripcion</label>
                            <dx:BootstrapTextBox ID="nombreRemuneracion3" runat="server"></dx:BootstrapTextBox>


                        </div>
                        <div class="mb-3">
                            <label for="montoRemuneracion3" class="form-label">Monto</label>
                            <dx:BootstrapTextBox ID="montoRemuneracion3" NullText="0.00" NullTextDisplayMode="UnfocusedAndFocused" runat="server"></dx:BootstrapTextBox>


                        </div>
                        <hr />
                        <div class="mb-3">
                            <label for="chkDeducciones" class="form-label">Aplica para Deducciones TSS</label>
                            <dx:BootstrapCheckBox ID="chkDeducciones" runat="server"></dx:BootstrapCheckBox>
                        </div>
                        <div class="mb-3">
                            <label for="activo" class="form-label">Periodo de Pago</label>
                            <asp:DropDownList ID="periodos" CssClass="form-select" runat="server">
                                <asp:ListItem Value="1">Semanal</asp:ListItem>
                                <asp:ListItem Selected="True" Value="2">Quincenal</asp:ListItem>
                                <asp:ListItem Value="3">Mensual</asp:ListItem>
                            </asp:DropDownList>
                        </div>

                        <hr />
                        <a>Contacto de Emergencia</a>
                        <div class="mb-3">
                            <label for="nombreContacto" class="form-label">Nombre</label>
                            <dx:BootstrapTextBox ID="nombreContacto" runat="server"></dx:BootstrapTextBox>


                        </div>
                        <div class="mb-3">
                            <label for="telefonoContacto" class="form-label">Telefono</label>
                            <dx:BootstrapTextBox ID="telefonoContacto" runat="server"></dx:BootstrapTextBox>


                        </div>
                        
                    </div>
                            
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cerrar</button>
                    <dx:BootstrapButton ValidationGroup="nuevoEmpleado" CssClasses-Control="bg-success" CausesValidation="true" ID="btnCrear" OnClick="btnCrear_Click" runat="server" AutoPostBack="false" Text="Guardar"></dx:BootstrapButton>
                </div>
            </div>
        </div>
    </div>
        <script src="https://code.jquery.com/jquery-3.1.1.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
        <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11.1.4/dist/sweetalert2.all.min.js"></script>

    <script>
        function registroEmpleadoModal() {
            var modal = document.getElementById('registroEmpleadoModal');

            if (modal) {
                var modalBootstrap = new bootstrap.Modal(modal);
                modalBootstrap.show();
            } else {
                console.error('El modal no se encontró en la página.');
            }
        }
    </script>
</asp:Content>

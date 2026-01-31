<%@ Page Title="" Language="C#" MasterPageFile="~/Empleador/dashboard.Master" AutoEventWireup="true" CodeBehind="fichaEmpleado.aspx.cs" Inherits="MiGente_Web.Empleador.fichaEmpleado" %>

<%@ Register Assembly="DevExpress.Web.v22.2, Version=22.2.5.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>

<%@ Register Assembly="DevExpress.Web.Bootstrap.v22.2, Version=22.2.5.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.Bootstrap" TagPrefix="dx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

    <style>
        .card-box {
            border: 1px solid #ccc;
            padding: 20px;
            border-radius: 10px;
            box-shadow: 0px 0px 10px rgba(0, 0, 0, 0.1);
        }
    </style>

    <div>
        
        <h2 runat="server" id="NombreEmpleado" class="text-white">Empleado</h2>

        <div class="bg-white pt-5 pb-5 ">
            <div class="container">
                <div id="divMensajeInactivo" runat="server" class="alert alert-light" style="display:none">
    <h4 class="alert-heading">Este empleado se encuentra desactivado</h4>
    <p>El empleado está desactivado y no puede realizar tareas en este momento. ¿Deseas imprimir el descargo?</p>
    <button id="btnImprimirDescargo" runat="server" onserverclick="btnImprimirDescargo_ServerClick" class="btn btn-primary">Imprimir Descargo</button>
</div>


                <div class="row">
                    <div class="col-md-12 mt-1">
                        <div class="card-box">
                            <h4 class="card-title">Información del Empleado</h4>
                            <hr>
                            <div class="row">
                                <div class="col-md-3">
                                    <p><strong>Fecha de Registro:</strong> <span runat="server" id="fechaRegistro"></span></p>
                                    <p><strong>Fecha de Inicio:</strong> <span runat="server" id="fechaInicio"></span></p>
                                    <p><strong>Identificación:</strong> <span runat="server" id="identificacion"></span></p>
                                    <p><strong>Nombre:</strong> <span runat="server" id="nombre"></span></p>
                                    <p><strong>Salario:</strong> <span runat="server" id="salario"></span></p>

                                </div>

                                <div class="col-md-3">
                                    <p><strong>Direccion:</strong> <span runat="server" id="htmlDireccion"></span></p>
                                    <p><strong>Provincia:</strong> <span runat="server" id="htmlProvincia"></span></p>
                                    <p><strong>Municipio:</strong> <span runat="server" id="htmlMunucipio"></span></p>
                                    <p><strong>Contacto Emergencia:</strong> <span runat="server" id="htmlEmergencia"></span></p>
                                    <dx:BootstrapCheckBox ID="chkTss" ClientReadOnly="true" Text="Aplica para deducciones TSS" runat="server"></dx:BootstrapCheckBox>
                                </div>
                                <div class="col-md-3">
                                    <p><strong>Teléfono 1:</strong> <span runat="server" id="telefono1"></span></p>
                                    <p><strong>Teléfono 2:</strong> <span runat="server" id="telefono2"></span></p>
                                    <p><strong>Período de Pago:</strong> <span runat="server" id="periodoPago"></span></p>
                                    <p><strong style="color:red">Contrato:</strong> <a runat="server" class="btn btn-primary" style="color:white;font-weight:bold" onserverclick="Unnamed_ServerClick"><span runat="server" id="contrato"></span></a></p>
                                    <dx:BootstrapCheckBox ID="chkActivo" ClientReadOnly="true" Text="Activo" runat="server"></dx:BootstrapCheckBox>
                                </div>
                                <div class="col-md-3">
                                    <asp:Image ID="Image2" runat="server" />
                                    </div>
                            </div>
                            <div class="row mt-3">
                                <div class="col-md-12">
                                    <button runat="server" class="btn btn-primary me-2" id="btnEditarPerfil" type="button" data-bs-toggle="modal" data-bs-target="#registroEmpleadoModal">Editar</button>
                                    <button type="button" class="btn btn-danger me-2" id="btnBaja" runat="server"  onserverclick="btnBaja_ServerClick1">Dar de Baja</button>
                                    <button runat="server" id="btnPago" class="btn btn-success me-2" type="button" data-bs-toggle="modal" data-bs-target="#modalPago">Realizar Pago</button>
                                    <button runat="server" class="btn btn-info" type="button" data-bs-toggle="modal" id="btnNota" data-bs-target="#agregarNotaModal">Agregar Nota</button>

                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-md-12 mt-1">
                        <div class="card-box">
                            <h4 class="card-title">Historial de Pagos</h4>
                            <dx:BootstrapGridView ID="gridPagos" OnHtmlRowCreated="gridPagos_HtmlRowCreated" runat="server" OnCustomButtonCallback="gridPagos_CustomButtonCallback" AutoGenerateColumns="False" DataSourceID="LinqPagos"
                                CssClasses-Table="table table-striped table-bordered">
                                <SettingsAdaptivity AdaptivityMode="HideDataCells" AllowOnlyOneAdaptiveDetailExpanded="true"></SettingsAdaptivity>
                                <Settings ShowFooter="true" />
                                <SettingsDataSecurity AllowEdit="true" />
                                <SettingsEditing Mode="EditForm"></SettingsEditing>
                                <CssClasses Row="grid-nowrap-row" Control="grid-borderless" />
                                <Columns>
                                    <dx:BootstrapGridViewTextColumn VisibleIndex="0" Visible="false" FieldName="pagoID" ReadOnly="True"></dx:BootstrapGridViewTextColumn>
                                    <dx:BootstrapGridViewTextColumn VisibleIndex="1" Visible="false" FieldName="userID" ReadOnly="True"></dx:BootstrapGridViewTextColumn>
                                    <dx:BootstrapGridViewDateColumn FieldName="fechaRegistro" ReadOnly="True" VisibleIndex="2"></dx:BootstrapGridViewDateColumn>
                                    <dx:BootstrapGridViewDateColumn FieldName="fechaPago" ReadOnly="True" VisibleIndex="3"></dx:BootstrapGridViewDateColumn>
                                    <dx:BootstrapGridViewTextColumn FieldName="Monto" ReadOnly="True" VisibleIndex="5"></dx:BootstrapGridViewTextColumn>
                                    <dx:BootstrapGridViewTextColumn FieldName="Expr1" Caption="Tipo" ReadOnly="True" VisibleIndex="4"></dx:BootstrapGridViewTextColumn>
                                    <dx:BootstrapGridViewCommandColumn VisibleIndex="6" AdaptivePriority="6">
                                        <CustomButtons>
                                            <dx:BootstrapGridViewCommandColumnCustomButton CssClass="btn bg-danger text-white" ID="btnAnular" Text="Anular"></dx:BootstrapGridViewCommandColumnCustomButton>
                                            <dx:BootstrapGridViewCommandColumnCustomButton CssClass="btn bg-primary text-white" ID="btnImprimirRecibo" Text="Imprimir"></dx:BootstrapGridViewCommandColumnCustomButton>
                                        </CustomButtons>
                                    </dx:BootstrapGridViewCommandColumn>
                                </Columns>
                            </dx:BootstrapGridView>

                            <asp:LinqDataSource runat="server" EntityTypeName="" ID="LinqPagos" ContextTypeName="MiGente_Web.Data.migenteEntities" Select="new (pagoID, userID, fechaRegistro, fechaPago, Monto, Expr1)" TableName="VPagos" Where="empleadoID == @empleadoID && userID == @userID">
                                <WhereParameters>
                                    <asp:QueryStringParameter QueryStringField="id" DefaultValue="0" Name="empleadoID" Type="Int32"></asp:QueryStringParameter>
                                    <asp:ControlParameter ControlID="HiddenField1" PropertyName="Value" Name="userID" Type="String"></asp:ControlParameter>
                                </WhereParameters>
                            </asp:LinqDataSource>
                        </div>
                    </div>

                    <div class="col-md-12 mt-1">
                        <div class="card-box" style="height: 300px">
                            <h4 class="card-title">Historial de Notas</h4>
                            <dx:BootstrapGridView CssClasses-Control="mt-1" ID="BootstrapGridView2" runat="server" AutoGenerateColumns="False" DataSourceID="linqNotas"
                                CssClasses-Table="table table-striped table-bordered">
                                <SettingsAdaptivity AdaptivityMode="HideDataCells" AllowOnlyOneAdaptiveDetailExpanded="true"></SettingsAdaptivity>
                                <Settings ShowFooter="true" />
                                <SettingsDataSecurity AllowEdit="true" />
                                <SettingsEditing Mode="EditForm"></SettingsEditing>
                                <CssClasses Row="grid-nowrap-row" Control="grid-borderless" />
                                <Columns>
                                    <dx:BootstrapGridViewTextColumn VisibleIndex="0" Visible="false" FieldName="notaID"></dx:BootstrapGridViewTextColumn>
                                    <dx:BootstrapGridViewTextColumn VisibleIndex="1" Visible="false" FieldName="userID"></dx:BootstrapGridViewTextColumn>
                                    <dx:BootstrapGridViewTextColumn FieldName="empleadoID" Visible="false" VisibleIndex="2"></dx:BootstrapGridViewTextColumn>
                                    <dx:BootstrapGridViewDateColumn FieldName="fecha" Width="15%" VisibleIndex="3"></dx:BootstrapGridViewDateColumn>
                                    <dx:BootstrapGridViewTextColumn FieldName="nota" VisibleIndex="4"></dx:BootstrapGridViewTextColumn>
                                </Columns>
                                <SettingsPager PageSize="12" Visible="false"></SettingsPager>
                            </dx:BootstrapGridView>
                            <asp:LinqDataSource runat="server" EntityTypeName="" ID="linqNotas" ContextTypeName="MiGente_Web.Data.migenteEntities" TableName="Empleados_Notas" Where="empleadoID == @empleadoID && userID == @userID">
                                <WhereParameters>
                                    <asp:QueryStringParameter QueryStringField="id" DefaultValue="0" Name="empleadoID" Type="Int32"></asp:QueryStringParameter>
                                    <asp:ControlParameter ControlID="HiddenField1" PropertyName="Value" DefaultValue="0" Name="userID" Type="String"></asp:ControlParameter>
                                </WhereParameters>
                            </asp:LinqDataSource>
                        </div>
                    </div>

                </div>
            </div>
        </div>

    </div>
    <!-- Modal Nota -->
    <div class="modal fade" id="agregarNotaModal" tabindex="-1" aria-labelledby="agregarNotaModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="agregarNotaModalLabel">Agregar Nota</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <asp:TextBox ID="txtNota" MaxLength="200" runat="server" CssClass="form-control" placeholder="Ingrese la nota"></asp:TextBox>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cerrar</button>
                    <asp:Button ID="btnGuardar" runat="server" Text="Guardar Nota" OnClick="btnGuardar_Click" CssClass="btn btn-primary" />
                </div>
            </div>
        </div>
    </div>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
    <asp:HiddenField ID="HiddenField1" runat="server" />

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
                            <dx:ASPxDateEdit ID="fechaDeIngreso" Enabled="false" ValidationSettings-RequiredField-IsRequired="true" ValidationSettings-ValidationGroup="nuevoEmpleado" CssClass="form-control" runat="server"></dx:ASPxDateEdit>
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
                            <dx:BootstrapTextBox ValidationSettings-ValidationGroup="nuevoEmpleado" ValidationSettings-RequiredField-IsRequired="true" ID="modalNombre" runat="server"></dx:BootstrapTextBox>
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
                                <asp:ListItem Selected="True" Value="1">Soltero(a)</asp:ListItem>
                                <asp:ListItem Value="2">Casado(a)</asp:ListItem>
                            </asp:DropDownList>
                        </div>

                        <div class="mb-3">
                            <label for="nacimiento" class="form-label">Fecha de Nacimiento</label>
                            <dx:ASPxDateEdit ID="nacimiento" CssClass="form-control" ValidationSettings-ValidationGroup="nuevoEmpleado" ValidationSettings-RequiredField-IsRequired="true" runat="server"></dx:ASPxDateEdit>
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

                            <dx:BootstrapTextBox ID="modalTelefono1" ValidationSettings-ValidationGroup="nuevoEmpleado" ValidationSettings-RequiredField-IsRequired="true" runat="server"></dx:BootstrapTextBox>

                        </div>
                        <div class="mb-3">
                            <label for="telefono2" class="form-label">Teléfono 2</label>
                            <dx:BootstrapTextBox ID="modalTelefono2" runat="server"></dx:BootstrapTextBox>


                        </div>
                        <div class="mb-3">
                            <label for="telefono2" class="form-label">Posicion que Ocupa</label>

                            <dx:BootstrapTextBox ID="posicion" ValidationSettings-ValidationGroup="nuevoEmpleado" ValidationSettings-RequiredField-IsRequired="true" runat="server"></dx:BootstrapTextBox>

                        </div>
                        <div class="mb-3">
                            <label for="salario" class="form-label">Salario Bruto</label>
                            <dx:BootstrapTextBox ID="modalSalario" NullText="0.00" NullTextDisplayMode="UnfocusedAndFocused" ValidationSettings-ValidationGroup="nuevoEmpleado" ValidationSettings-RequiredField-IsRequired="true" runat="server"></dx:BootstrapTextBox>


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
                        <hr />

                        <a>Otras Remuneraciones</a>
                        <div class="mb-3">
                            <label for="nombreRemuneracion1" class="form-label">Descripcion</label>
                            <dx:BootstrapTextBox ID="nombreRemuneracion1" runat="server"></dx:BootstrapTextBox>


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
                            <dx:BootstrapTextBox ID="montoRemuneracion2" NullText="0.00" runat="server"></dx:BootstrapTextBox>


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

    <!-- Modal -->

    <div class="modal fade" id="modalPago" tabindex="-1" aria-labelledby="modalPagoModalLabel" data-backdrop="static" aria-hidden="true">
        <div class=" modal-dialog modal-md">

            <div class="modal-content">
                <div class="modal-header">
                    <h4>Realizar Pago a empleado</h4>
                </div>
                <div class="modal-body">
                    <asp:UpdatePanel ID="UpdatePanelModal" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="row">
                                   <div class="form-group col-lg-12 col-sm-12">
                                    <a>Cocepto de Pago</a>
                                       <asp:DropDownList ID="ddlConceptoPago" ValidationGroup="nuevoPago" CssClass="form-select" runat="server">
                                           <asp:ListItem Selected="True">Pago de Salario</asp:ListItem>
                                           <asp:ListItem Selected="False">Regalía Pascual</asp:ListItem>
                                     
                                    
                                           </asp:DropDownList>
                                   </div>
                                <div class="form-group col-lg-4 col-sm-12">
                                    <a>Fecha de Pago</a>
                                    <dx:ASPxDateEdit ID="fechaPago" ValidationSettings-RequiredField-IsRequired="true"
                                        ValidationSettings-ValidationGroup="nuevoPago" CssClass="form-control" Width="130px" runat="server"></dx:ASPxDateEdit>
                                </div>
                                <div class="form-group col-lg-5 col-sm-12">
                                    <a>Tipo de Pago</a>
                                    <asp:DropDownList ID="ddlTipoPago" CssClass="form-select" runat="server">
                                        <asp:ListItem Selected="True" Value="1">Periodo Completo</asp:ListItem>
                                        <asp:ListItem Value="2">Fracción de Periodo</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                                <div class="form-group col-lg-2 col-sm-12">
                                    <a style="color: white">..</a>
                                    <dx:BootstrapButton ID="btnContinuar" ValidationGroup="nuevoPago" CausesValidation="true" OnClick="btnContinuar_Click" CssClasses-Control="bg-primary" runat="server" AutoPostBack="false" Text="Continuar"></dx:BootstrapButton>
                                </div>
                                    <div class="form-group col-lg-7 col-sm-12" title="Descripcion de Descuento">
                                    <a>Descuento</a>
                                        <dx:BootstrapTextBox  ValidationSettings-RequiredField-IsRequired="true" ValidationSettings-ValidationGroup="nuevoDescuento" ID="descripcionDescuento" runat="server"></dx:BootstrapTextBox>
                                </div>
                                <div class="form-group col-lg-3 col-sm-12" title="Monto del Descuento">
                                    <a>Monto</a>
                                    <dx:BootstrapTextBox ID="montoDescuento" NullText="0.00" DisplayFormatString="{0:n2}"  ValidationSettings-RequiredField-IsRequired="true" ValidationSettings-ValidationGroup="nuevoDescuento"  runat="server">
                                    </dx:BootstrapTextBox>
                                </div>
                                <div class="form-group col-lg-2 col-sm-12">
                                    <a style="color: white">..</a>
                                    <dx:BootstrapButton ID="btnNuevoDescuento" OnClick="btnNuevoDescuento_Click" ValidationGroup="nuevoDescuento" CausesValidation="true" CssClasses-Control="bg-danger" runat="server" AutoPostBack="false"  CssClasses-Icon="fa fa-plus"></dx:BootstrapButton>
                                </div>


                                <a>Detalle</a>
                                <br />
                                <dx:BootstrapGridView ID="gridDetallePago" runat="server" AutoGenerateColumns="False">
                                    <Settings ShowFooter="True"></Settings>
                                    <Columns>
                                        <dx:BootstrapGridViewTextColumn Caption="Concepto" Width="70%" FieldName="Concepto"></dx:BootstrapGridViewTextColumn>
                                        <dx:BootstrapGridViewTextColumn Caption="Monto" PropertiesTextEdit-DisplayFormatString="N2" FieldName="Monto"></dx:BootstrapGridViewTextColumn>

                                    </Columns>
                                    <TotalSummary>
                                        <dx:ASPxSummaryItem ShowInColumn="Monto" SummaryType="Sum" FieldName="Monto" ValueDisplayFormat="{0:c2}" Tag="Monto a Pagar"></dx:ASPxSummaryItem>
                                    </TotalSummary>
                                </dx:BootstrapGridView>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <div class="modal-footer">
                    <div class="avatar-group">
                        <dx:BootstrapButton ID="btnGenerar" ValidationGroup="nuevoPago" CausesValidation="true" OnClick="btnGenerar_Click" CssClasses-Control="bg-success" runat="server" AutoPostBack="false" Text="Procesar Pago"></dx:BootstrapButton>
                    </div>
                </div>
            </div>
        </div>
        <a href=""></a>
    </div>
    <asp:HiddenField ID="hiddenPrestaciones" runat="server" />
    <script src="https://code.jquery.com/jquery-3.1.1.min.js">

    </script>
<script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>

  <script type="text/javascript">
    function AbrirPopupImprimirRecibo(pagoID,userID,documento) {
     
        var url = 'Impresion/PrintViewer.aspx?documento='+encodeURIComponent(documento) +'&id='+encodeURIComponent(pagoID)+'&userID='+encodeURIComponent(userID); // Reemplaza con la URL de tu página popup
console.log(url);
              var ventanaPopup = window.open(url, 'PopupWindow', 'width=800,height=600,location=no');
      }

      function DarDeBajaEmpleado() {
          console.log('Dar de baja');
          var valorHidden = document.getElementById('<%= HiddenField1.ClientID %>').value;

          // Obtiene la fecha actual en formato YYYY-MM-DD
          var fechaActual = new Date().toISOString().split('T')[0];

          Swal.fire({
              title: '¿Estás seguro?',
              html:
              '<hr>'+
                  'Esta acción dará de baja al empleado. ¿Estás seguro que deseas continuar?<br>' +
                  '<label for="motivoBaja" class="form-label">Motivo de Baja:</label>' +
                  '<select class="form-select" id="motivoBaja" class="swal2-input" style="text-align: center;">' +
                  '<option value="Desahucio">Desahucio</option>' +
                  '<option value="Renuncia">Renuncia</option>' +
                  '<option value="Despido Justificado">Despido Justificado</option>' +
                  '<option value="Despido Injustificado">Despido Injustificado</option>' +
                  '<option value="Dimisión">Dimisión</option>' +
                  '</select>' +
                  '<label for="prestaciones" class="form-label">Prestaciones Laborales:</label>' +
                  '<input type="number" id="prestaciones" class="form-control" step="any" required style="text-align: center;" value="0.00" oninput="validarPrestaciones(this)" pattern="^\d+(\.\d{1,2})?$">' +
                  '<a href="https://calculo.mt.gob.do/" target="_blank" style="color:red;">Calcular Prestaciones</a>' +
                  '</br>' + 
                  '</br>' + 
                  '<label for="fechaBaja" class="form-label">Fecha de Salida:</label>' +

                  '<input type="date" id="fechaBaja" class="form-control" style="text-align: center;" value="' + fechaActual + '">',
              icon: 'warning',
              showCancelButton: true,
              confirmButtonText: 'Sí, dar de baja',
              cancelButtonText: 'Cancelar',
              preConfirm: () => {
                  var motivoSeleccionado = document.getElementById('motivoBaja').value;
                  var prestacionesLaborales = parseFloat(document.getElementById('prestaciones').value);
                  var fechaSeleccionada = document.getElementById('fechaBaja').value;

                  // Verifica que los campos no estén vacíos y que prestaciones sea un número válido
                  if (!motivoSeleccionado || isNaN(prestacionesLaborales) || prestacionesLaborales < 0) {
                      Swal.showValidationMessage('Por favor, completa todos los campos correctamente.');
                  } else {
                      // Llamada AJAX para activar el evento en el servidor.
                      $.ajax({
                          type: 'POST',
                          url: 'fichaEmpleado.aspx/DarDeBaja', // Ajusta la URL según tu estructura.
                          data: JSON.stringify({
                              userID: valorHidden,
                              motivoBaja: motivoSeleccionado,
                              prestaciones: prestacionesLaborales,
                              fechaBaja: fechaSeleccionada
                          }), // Envía los parámetros en formato JSON
                          contentType: 'application/json; charset=utf-8',
                          dataType: 'json',
                          success: function (response) {
                              // Maneja la respuesta del servidor si es necesario.
                              window.location.href = window.location.href;
                              console.log('Evento de dar de baja activado.');
                          },
                          error: function (xhr, status, error) {
                              // Maneja los errores si es necesario.
                              console.error('Error al activar el evento de dar de baja: ' + error);
                          }
                      });
                  }
              }
          });
      }

      function validarPrestaciones(input) {
          // Reemplaza cualquier valor no numérico o negativo por 0.00
          input.value = input.value.replace(/[^0-9.]/g, '');
          if (input.value < 0) {
              input.value = '0.00';
          }
      }


      function mostrarAlertaDesactivado() {
          Swal.fire({
              title: 'Este empleado se encuentra desactivado',
              icon: 'warning',
              html: 'El empleado está desactivado y no puede realizar tareas en este momento. ¿Deseas imprimir el descargo?',
              showCancelButton: true,
              cancelButtonText: 'Cancelar',
              confirmButtonText: 'Imprimir Descargo',
              showLoaderOnConfirm: true,
              preConfirm: () => {
                  $.ajax({
                      type: 'POST',
                      url: 'fichaEmpleado.aspx/imprimirDescargo', // Ajusta la URL según tu estructura.
                      contentType: 'application/json; charset=utf-8',
                      dataType: 'json',
                      success: function (response) {
                          // Realiza acciones adicionales después de la impresión, si es necesario.
                          console.log('Descargo impreso con éxito.');
                      },
                      error: function (xhr, status, error) {
                          // Maneja los errores si es necesario.
                          console.error('Error al imprimir el descargo: ' + error);
                      }
                  });

              }
          });
      }
     



   

  </script>

</asp:Content>

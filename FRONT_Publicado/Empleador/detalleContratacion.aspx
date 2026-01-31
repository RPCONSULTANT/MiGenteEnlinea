<%@ Page Title="" Language="C#" MasterPageFile="~/Empleador/dashboard.Master" AutoEventWireup="true" CodeBehind="detalleContratacion.aspx.cs" Inherits="MiGente_Web.Empleador.detalleContratacion" %>

<%@ Register Assembly="DevExpress.Web.v22.2, Version=22.2.5.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>

<%@ Register Assembly="DevExpress.Web.Bootstrap.v22.2, Version=22.2.5.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.Bootstrap" TagPrefix="dx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" EnablePageMethods="true" runat="server"></asp:ScriptManager>

    <a runat="server" id="linkVolver">
        <h2 class="section-title text-white" runat="server" id="tituloContratacion">Titulo Contratacion</h2>
    </a> 
    <asp:HiddenField ID="HiddenField1" runat="server" />
    <div class="row">
        <!-- Tarjeta 1 (6 columnas) -->
        <div class="col-md-6 mt-1">
            <div class="col-md-12 mt-1">
                <div class="card">
                    <div class="card-header bg-success text-white">
                        Detalles de Contratación
                    </div>
                    <div class="card-body">
                        <!-- Coloca aquí tus campos de contratación -->
                        <div class="mb-3">
                            <label for="nombreContacto" class="form-label">Descripcion Corta</label>
                            <dx:BootstrapTextBox MaxLength="60" ID="descripcionCortaTrabajo" ValidationSettings-RequiredField-IsRequired="true"
                                ValidationSettings-ValidationGroup="nuevaContratacion" runat="server">
                            </dx:BootstrapTextBox>


                        </div>
                        <div class="mb-3">
                            <label for="nombreContacto" class="form-label">Descripcion Ampliada</label>
                            <dx:BootstrapMemo ID="descripcionAmpliada" MaxLength="250" runat="server"></dx:BootstrapMemo>

                        </div>
                        <div class="mb-3">
                            <label for="fechaInicio" class="form-label">Fecha de Inicio</label>
                            <dx:ASPxDateEdit ID="fechaInicio" ValidationSettings-RequiredField-IsRequired="true" ValidationSettings-ValidationGroup="nuevaContratacion" CssClass="form-control" runat="server"></dx:ASPxDateEdit>
                        </div>
                        <div class="mb-3">
                            <label for="fechaInicio" class="form-label">Fecha de Conclusión Esperada</label>
                            <dx:ASPxDateEdit ID="fechaConclusion" ValidationSettings-RequiredField-IsRequired="true" ValidationSettings-ValidationGroup="nuevaContratacion" CssClass="form-control" runat="server"></dx:ASPxDateEdit>
                        </div>

                        <div class="mb-3">
                            <label for="montoAcordado" class="form-label">Monto Total Acordado</label>
                            <dx:BootstrapTextBox ID="montoAcordado" NullText="0.00" NullTextDisplayMode="UnfocusedAndFocused" ValidationSettings-ValidationGroup="nuevaContratacion" ValidationSettings-RequiredField-IsRequired="true" runat="server"></dx:BootstrapTextBox>


                        </div>

                        <div class="mb-3">
                            <label for="activo" class="form-label">Esquema de Pagos</label>
                            <asp:DropDownList ID="ddlEsquema" CssClass="form-select" ValidationGroup="nuevaContratacion" runat="server">
                                <asp:ListItem Value="1">100 % Contra Entrega</asp:ListItem>
                                <asp:ListItem Selected="True" Value="2">50% Avance/50% Finalizado</asp:ListItem>
                                <asp:ListItem Value="3">30% Avance/70% Finalizado</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <!-- Otros campos aquí -->

                        <!-- Botones -->
                        <dx:BootstrapButton ID="btnActualizar" OnClick="btnActualizar_Click" CssClasses-Control="bg-primary" runat="server" AutoPostBack="false" Text="Actualizar Informacion"></dx:BootstrapButton>
                        <dx:BootstrapButton ID="btnCancelar" OnClick="btnCancelar_Click" CssClasses-Control="bg-danger" runat="server" AutoPostBack="false" Text="Cancelar Trabajo"></dx:BootstrapButton>

                        <dx:BootstrapButton ID="btnFinalizar" CssClasses-Control="bg-success" runat="server" AutoPostBack="false" Text="Trabajo Finalizado"></dx:BootstrapButton>

                    </div>
                </div>
            </div>
        </div>
        <!-- Tarjeta 2 (6 columnas) -->
        <div class="col-md-6  mt-1">
            <div class="col-md-12  mt-1">
                <div class="card">
                    <div class="card-header  bg-secondary text-white">
                        Detalles de Pagos
                    </div>
                    <div class="card-body">
                        <div class="row horizontal">
                            <div class="form-group d-inline col col-md-4 col-sm-12">
                                <h6>Monto Acordado:</h6>
                                <h6 runat="server" id="lbMontoAcordado">0.00</h6>
                            </div>
                            <div class="form-group d-inline col col-md-4 col-sm-12">
                                <h6>Pagos Realizados:</h6>
                                <h6 runat="server" id="pagosRealizados">0.00</h6>
                            </div>
                            <div class="form-group d-inline col col-md-4 col-sm-12">
                                <h6 style="color: red">Monto Pendiente:</h6>
                                <h6 runat="server" id="montoPendiente" style="color: red">0.00</h6>
                            </div>
                        </div>
                        <hr />
                        <div class="table-responsive-sm">

                            <asp:Repeater ID="repeaterPagos" runat="server">
                                <HeaderTemplate>
                                    <table class="table table-striped">
                                        <thead>
                                            <tr>
                                                <th>Fecha de Pago</th>
                                                <th>Monto</th>
                                                <th>Acciones</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td><%# Eval("fechaPago", "{0:dd/MM/yyyy}") %></td>
                                        <td><%# Eval("Monto", "{0:C}") %></td>
                                        <td>
                                            <asp:Button ID="btnImprimir" runat="server" Text="Imprimir" OnClick="btnImprimir_Click"
                                                CommandArgument='<%# Eval("pagoID") %>' CssClass="btn btn-primary" />
                                            <asp:Button ID="btnEliminar" runat="server" Text="Anular Recibo" OnClick="btnEliminar_Click"
                                                CommandArgument='<%# Eval("pagoID") %>' CssClass="btn btn-danger" />
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

                    <div class="modal-footer">
                        <div class="container">
                            <button type="button" class="btn btn-success" runat="server" id="btnPago" data-bs-target="#modalPago" data-bs-toggle="modal">Realizar Pago</button>
                        </div>
                    </div>
                </div>
            </div>

            <div class="col col-md-12  mt-2"  id="tarjetaCalificacion">
                <div class="card">
                    <div class="card-header  bg-gradient-primary text-white">
                        Calificacion de Trabajo
                    </div>
                    <div class="card-body">
                        <h4 runat="server" id="lbCalificar">Para calificar el trabajo primero debe finalizar el mismo.</h4>
                        <a runat="server" id="editarCalificacion" onserverclick="editarCalificacion_ServerClick" title="Editar Calificacion">
                     <div class="rating horizontal" >
    <%= GetStarRating(Convert.ToDecimal(hiddenCalificacion.Value)) %> <!-- Reemplaza 3.5 con la calificación real -->
<%=hiddenCalificacion.Value %>
</div>
                             </a>
                        <asp:HiddenField ID="hiddenCalificacion" runat="server" Value="0" />

                    </div>
                   
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="modalPago" tabindex="-1" aria-labelledby="modalPagoModalLabel" data-backdrop="static" aria-hidden="true">
        <div class=" modal-dialog modal-md">

            <div class="modal-content">
                <div class="modal-header">
                    <h4>Realizar Pago a Colaborador</h4>
                </div>
                <div class="modal-body">
                    <asp:UpdatePanel ID="UpdatePanelModal" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="row">
                                <div class="form-group col-lg-12 col-sm-12">
                                    <a>Cocepto de Pago</a>
                                    <asp:DropDownList ID="ddlConceptoPago" ValidationGroup="nuevoPago" CssClass="form-select" runat="server">
                                        <asp:ListItem Selected="True">Pago de Avance</asp:ListItem>
                                        <asp:ListItem Selected="False">Pago Final</asp:ListItem>


                                    </asp:DropDownList>
                                </div>
                                <div class="form-group col-lg-4 col-sm-12">
                                    <a>Fecha de Pago</a>
                                    <dx:ASPxDateEdit ID="fechaPago" ValidationSettings-RequiredField-IsRequired="true"
                                        ValidationSettings-ValidationGroup="nuevoPago" CssClass="form-control" Width="130px" runat="server">
                                    </dx:ASPxDateEdit>
                                </div>


                                <div class="form-group col-lg-3 col-sm-12" title="Monto de Pago">
                                    <a>Monto</a>
                                    <dx:BootstrapTextBox ID="montoPago" NullText="0.00" DisplayFormatString="{0:n2}" ValidationSettings-RequiredField-IsRequired="true" ValidationSettings-ValidationGroup="nuevoPago" runat="server">
                                    </dx:BootstrapTextBox>
                                </div>
                                <div class="form-group col-lg-2 col-sm-6">
                                    <a style="color: white">..</a>
                                    <dx:BootstrapButton ID="btnContinuar" OnClick="btnContinuar_Click" ValidationGroup="nuevoPago" CausesValidation="true" CssClasses-Control="bg-primary" runat="server" AutoPostBack="false" Text="Continuar"></dx:BootstrapButton>
                                </div>



                                <a>Detalle</a>
                                <br />
                                <dx:BootstrapGridView ID="gridDetallePago" OnCustomButtonCallback="gridDetallePago_CustomButtonCallback" runat="server" AutoGenerateColumns="False">
                                    <Settings ShowFooter="True"></Settings>
                                    <Columns>
                                        <dx:BootstrapGridViewTextColumn Caption="Concepto" Width="70%" FieldName="Concepto"></dx:BootstrapGridViewTextColumn>
                                        <dx:BootstrapGridViewTextColumn Caption="Monto" PropertiesTextEdit-DisplayFormatString="N2" FieldName="Monto"></dx:BootstrapGridViewTextColumn>

                                        <dx:BootstrapGridViewCommandColumn VisibleIndex="2">
                                            <CustomButtons>
                                                <dx:BootstrapGridViewCommandColumnCustomButton CssClass="bg-danger text-white" ID="btnQuitar" Text="Quitar"></dx:BootstrapGridViewCommandColumnCustomButton>
                                            </CustomButtons>
                                        </dx:BootstrapGridViewCommandColumn>
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
                        <dx:BootstrapButton ID="btnGenerar" ValidationGroup="procesarPago" OnClick="btnGenerar_Click" CausesValidation="true" CssClasses-Control="bg-success" runat="server" AutoPostBack="false" Text="Procesar Pago"></dx:BootstrapButton>
                    </div>
                </div>
            </div>
        </div>

    </div>

    <div class="modal fade" id="modalCalificar" tabindex="-1" aria-labelledby="modalCalificarLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered modal-md">
            <div class="modal-content">
                <div class="modal-header bg-primary text-white">
                    <h5 class="modal-title text-white" id="modalCalificarLabel">Calificar Perfil</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>

                            <div class="mb-3">

                                <label class="form-label">¿Cuantas estrellas le das a su puntualidad?</label>
                                <dx:ASPxRatingControl ID="ratingPuntualidad" Value="0" ItemCount="5" ClientIDMode="Static" FillPrecision="Exact" runat="server">
                                    <ClientSideEvents ItemClick="onRatingValueChanged1" />
                                </dx:ASPxRatingControl>
                            </div>
                            <div class="mb-3">
                                <label class="form-label">¿Que te pareció el Servicio Recibido?</label>
                                <dx:ASPxRatingControl ID="ratingCumplimiento" ClientIDMode="Static" Value="0" ItemCount="5" FillPrecision="Exact" runat="server">
                                    <ClientSideEvents ItemClick="onRatingValueChanged2" />
                                </dx:ASPxRatingControl>

                            </div>
                            <div class="mb-3">
                                <label class="form-label">¿Cómo calificas sus conocimientos profesionales?</label>
                                <dx:ASPxRatingControl ID="ratingConocimientos" Value="0" ClientIDMode="Static" ItemCount="5" FillPrecision="Exact" runat="server">
                                    <ClientSideEvents ItemClick="onRatingValueChanged3" />
                                </dx:ASPxRatingControl>

                            </div>
                            <div class="mb-3">
                                <label class="form-label">¿Que tanto lo recomendarias?</label>
                                <dx:ASPxRatingControl ID="ratingRecomendacion" Value="0" ClientIDMode="Static" ItemCount="5" FillPrecision="Exact" runat="server">
                                    <ClientSideEvents ItemClick="onRatingValueChanged4" />
                                </dx:ASPxRatingControl>

                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancelar</button>
                    <dx:BootstrapButton ID="btnCalificar" ValidationGroup="calificar" runat="server"
                        AutoPostBack="false" Text="Realizar Calificacion">
                        <ClientSideEvents Click="function(s, e) { confirmarCalificacion(); }" />
                    </dx:BootstrapButton>

                </div>
            </div>
        </div>
    </div>
    <script src="https://code.jquery.com/jquery-3.1.1.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <script>
        function ConfirmarEliminarRecibo(pagoID) {
            Swal.fire({
                title: '¿Estás seguro?',
                text: '¿Deseas anular este recibo?',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Sí, anularlo',
                cancelButtonText: 'Cancelar'
            }).then((result) => {
                if (result.isConfirmed) {
                    // Llama al método C# para anular el recibo
                    PageMethods.AnularRecibo(pagoID, OnAnularReciboSuccess);
                }
            });
        }

        // Función que se llama después de anular el recibo
        function OnAnularReciboSuccess(response) {
            // Muestra un mensaje de éxito o error
            MostrarMensaje(response, 'success');

            // Recarga la página después de un breve retraso (por ejemplo, 1 segundo)
            setTimeout(function () {
                window.location.href = window.location.href;
            }, 500); // 1000 milisegundos = 1 segundo
        }

        // Función para mostrar mensajes de SweetAlert
        function MostrarMensaje(mensaje, tipo) {
            Swal.fire(mensaje, '', tipo);
        }
    </script>
    <script>
        function ConfirmarCancelacionTrabajo(contratacionID,detalleID) {
            Swal.fire({
                title: '¿Estás seguro?',
                text: '¿Deseas Cancelar este trabajo sin concluir?',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Sí, Cancelarlo',
                cancelButtonText: 'Cancelar'
            }).then((result) => {
                if (result.isConfirmed) {
                    // Llama al método C# para anular el recibo
                    PageMethods.CancelarTrabajo(contratacionID, detalleID, OnCancelarTrabajoSuccess);
                }
            });
        }

        // Función que se llama después de anular el recibo
        function OnCancelarTrabajoSuccess(response) {
            // Muestra un mensaje de éxito o error
            MostrarMensaje(response, 'success');

            // Recarga la página después de un breve retraso (por ejemplo, 1 segundo)
            setTimeout(function () {
                window.location.href = window.location.href;
            }, 500); // 1000 milisegundos = 1 segundo
        }

        // Función para mostrar mensajes de SweetAlert
        function MostrarMensaje(mensaje, tipo) {
            Swal.fire(mensaje, '', tipo);
        }
    </script>

    <%--    // Calificacion--%>
    <script>
        function mostrarAlerta() {
            Swal.fire({
                icon: 'success',
                title: 'Calificación Exitosa',
                text: 'El perfil se ha calificado correctamente.',
                confirmButtonColor: '#3085d6',
                confirmButtonText: 'OK'
            }).then((result) => {
                if (result.isConfirmed) {
                    // Recargar la página para limpiar los parámetros
                    location.reload();
                }
            });
        }
        function confirmarCalificacion() {
            Swal.fire({
                icon: 'warning',
                title: 'Confirmación',
                text: '¿Estás seguro de que deseas calificar este perfil?',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Sí, calificar',
                cancelButtonText: 'Cancelar'
            }).then((result) => {
                if (result.isConfirmed) {
                    // Procesar el método de calificación
                    calificarPerfil();
                }
            });
        }
        var puntualidad = 0;
        var cumplimiento = 0;
        var conocimientos = 0;
        var recomendacion = 0;


        function onRatingValueChanged1(s, e) {
            puntualidad = s.GetValue();
        }
        function onRatingValueChanged2(s, e) {
            cumplimiento = s.GetValue();
        }
        function onRatingValueChanged3(s, e) {
            conocimientos = s.GetValue();
        }
        function onRatingValueChanged4(s, e) {
            recomendacion = s.GetValue();
        }


        function calificarPerfil() {
            var tipo = 1;

            console.log(tipo);


            console.log(puntualidad);


            $.ajax({
                type: "POST",
                url: "detalleContratacion.aspx/Calificar",
                data: JSON.stringify({ puntualidad: puntualidad, cumplimiento: cumplimiento, conocimientos: conocimientos, recomendacion: recomendacion }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    if (data.d) {
                        onSuccess();
                        limpiarCampos();
                    } else {
                        // No se cumplieron las reglas, maneja el error
                        onError();
                    }
                },
                error: function (error) {
                    console.log(error);
                    onError();
                }
            });
        }


        function onSuccess() {
            Swal.fire({
                icon: 'success',
                title: 'Calificación Exitosa',
                text: 'El perfil se ha calificado correctamente.',
                confirmButtonColor: '#3085d6',
                confirmButtonText: 'OK'
            }).then((result) => {
                if (result.isConfirmed) {
                    window.location.href = window.location.href;
                }
            });
        }

        function onError() {
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'No puede calificar 2 veces el mismo perfil en un intervalo de tiempo tan corto.',
                confirmButtonColor: '#3085d6',
                confirmButtonText: 'OK'
            });
        }
    </script>

</asp:Content>

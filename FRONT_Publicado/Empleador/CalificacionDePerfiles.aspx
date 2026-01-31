<%@ Page Title="" Language="C#" MasterPageFile="~/Empleador/dashboard.Master" AutoEventWireup="true" CodeBehind="CalificacionDePerfiles.aspx.cs" Inherits="MiGente_Web.Empleador.CalificacionDePerfiles" %>
<%@ Register Assembly="DevExpress.Web.Bootstrap.v22.2, Version=22.2.5.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.Bootstrap" TagPrefix="dx" %>

<%@ Register Assembly="DevExpress.Web.v22.2, Version=22.2.5.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11.1.4/dist/sweetalert2.all.min.js"></script>
    <script src="https://kit.fontawesome.com/a076d05399.js"></script>
    <div class="container mt-1">
        <div class="section-title justify-content-center" style="text-align: center;">
            <h2 class="text-white">Calificación de Perfiles</h2>
        </div>
        <div class="row">
            <div class="col-md-6 mt-3">
                <div class="card text-center">
                    <div class="card-body">
                        <h5 class="card-title">Calificar Perfil</h5>
                        <i class="fas fa-star fa-5x text-warning mb-3"></i>
                        <p class="card-text">Califica perfiles y se colabora al crecimiento de esta gran comunidad.</p>
                        <button type="button" data-bs-toggle="modal" data-bs-target="#modalCalificar" class="btn btn-primary">Calificar</button>
                    </div>
                </div>
            </div>
            <div class="col-md-6 mt-3">
                <div class="card text-center">
                    <div class="card-body">
                        <h5 class="card-title">Consultar Perfiles</h5>
                        <i class="fas fa-search fa-5x text-primary mb-3"></i>
                        <p class="card-text">Utiliza el directorio para buscar y consultar perfiles.</p>
                        <a href="#" class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalConsultarPerfil">Consultar</a>
                    </div>
                </div>
            </div>

        </div>

    </div>
    <div class="container mt-6 align-content-center" style="text-align: center">
        <h3>Mis calificaciones</h3>

  <div class="table-responsive">
    <dx:BootstrapGridView ID="gridCalificaciones" OnHtmlDataCellPrepared="gridCalificaciones_HtmlDataCellPrepared" runat="server" AutoGenerateColumns="False" DataSourceID="linqMisCalificaciones">
        <Columns>
            <dx:BootstrapGridViewTextColumn FieldName="calificacionID" Visible="false" VisibleIndex="0"></dx:BootstrapGridViewTextColumn>
            <dx:BootstrapGridViewDateColumn FieldName="fecha" SortOrder="Descending" Caption="Fecha" VisibleIndex="1"></dx:BootstrapGridViewDateColumn>
            <dx:BootstrapGridViewTextColumn FieldName="userID" Visible="false" VisibleIndex="2"></dx:BootstrapGridViewTextColumn>
            <dx:BootstrapGridViewTextColumn FieldName="tipo" Visible="false" VisibleIndex="3"></dx:BootstrapGridViewTextColumn>
            <dx:BootstrapGridViewTextColumn FieldName="identificacion" VisibleIndex="4"></dx:BootstrapGridViewTextColumn>
            <dx:BootstrapGridViewTextColumn FieldName="nombre" Caption="Nombre" VisibleIndex="5"></dx:BootstrapGridViewTextColumn>
            <dx:BootstrapGridViewTextColumn HorizontalAlign="Center" FieldName="puntualidad" Caption="Puntualidad" VisibleIndex="6">
                <PropertiesTextEdit EncodeHtml="false" />
            </dx:BootstrapGridViewTextColumn>
            <dx:BootstrapGridViewTextColumn HorizontalAlign="Center" FieldName="conocimientos" Caption="Conocimientos" VisibleIndex="7">
                <PropertiesTextEdit EncodeHtml="false" />
            </dx:BootstrapGridViewTextColumn>
            <dx:BootstrapGridViewTextColumn HorizontalAlign="Center" FieldName="cumplimiento" Caption="Cumplimiento" VisibleIndex="8">
                <PropertiesTextEdit EncodeHtml="false" />
            </dx:BootstrapGridViewTextColumn>
           <dx:BootstrapGridViewTextColumn HorizontalAlign="Center" FieldName="recomendacion" Caption="Recomendacion" VisibleIndex="9">
                <PropertiesTextEdit EncodeHtml="false" />
            </dx:BootstrapGridViewTextColumn>
        </Columns>
    </dx:BootstrapGridView>
</div>

        <asp:LinqDataSource runat="server" EntityTypeName="" ID="linqMisCalificaciones" ContextTypeName="MiGente_Web.Data.migenteEntities" TableName="Calificaciones" Where="userID == @userID">
            <WhereParameters>
                <asp:ControlParameter ControlID="HiddenField1" PropertyName="Value" DefaultValue="0" Name="userID" Type="String"></asp:ControlParameter>
            </WhereParameters>
        </asp:LinqDataSource>
    </div>
    <div class="modal fade" id="modalCalificar" tabindex="-1" aria-labelledby="modalCalificarLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered modal-md">
            <div class="modal-content">
                <div class="modal-header bg-primary text-white">
                    <h5 class="modal-title text-white" id="modalCalificarLabel">Calificar Perfil</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                      <asp:UpdatePanel ID="UpdatePanelModal" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                    <div class="mb-3">
                        <label for="tipo" class="form-label">Seleccione Perfil para Calificar</label>
                        <asp:DropDownList ValidationGroup="calificar" OnDataBound="ddlPerfil_DataBound" AutoPostBack="true" OnSelectedIndexChanged="ddlPerfil_SelectedIndexChanged" ID="ddlPerfil" runat="server" CssClass="form-select" DataSourceID="linqEmpleados" DataTextField="Nombre" DataValueField="Identificacion">
                     <asp:ListItem Text="" Value=""></asp:ListItem> 
                            </asp:DropDownList>
                        <asp:LinqDataSource runat="server" EntityTypeName="" ID="linqEmpleados" ContextTypeName="MiGente_Web.Data.migenteEntities" Select="new (Nombre, identificacion)" TableName="VEmpleados" Where="userID == @userID AND Activo == @activo">
                            <WhereParameters>
                                <asp:ControlParameter ControlID="HiddenField1" PropertyName="Value" DefaultValue="0" Name="userID" Type="String"></asp:ControlParameter>
                          <asp:Parameter DefaultValue="False" Name="activo" Type="Boolean" />
                                </WhereParameters>
                        </asp:LinqDataSource>
                    </div>
                    <div class="mb-3">
                        <label for="identificacion"  class="form-label">RNC/Cedula</label>
                        <dx:BootstrapTextBox Enabled="false" ID="calif_identificacion" ClientIDMode="Static" ClientInstanceName="calif_identificacion" ValidationSettings-RequiredField-IsRequired="true" ValidationSettings-ValidationGroup="calificar" runat="server">
                            <ClientSideEvents ValueChanged="onIdentificacionChanged" />
                        </dx:BootstrapTextBox>
                    </div>
                    <div class="mb-3">
                        <label for="nombre" class="form-label">Nombre</label>
                        <dx:BootstrapTextBox Enabled="false" ID="calif_nombre" ValidationSettings-RequiredField-IsRequired="true" ValidationSettings-ValidationGroup="calificar" runat="server">
                                    <ClientSideEvents ValueChanged="onNombreChanged" />
                        </dx:BootstrapTextBox>
                    </div>
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
                    <dx:BootstrapButton Enabled="false" ID="btnCalificar" ValidationGroup="calificar" runat="server"
                        AutoPostBack="false" Text="Realizar Calificacion">
                        <ClientSideEvents Click="function(s, e) { confirmarCalificacion(); }" />
                    </dx:BootstrapButton>

                </div>
            </div>
        </div>
    </div>

    <!-- Modal para consulta de perfil -->
    <div class="modal fade" id="modalConsultarPerfil" tabindex="-1" role="dialog" aria-labelledby="modalConsultarPerfilLabel">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="modalConsultarPerfilLabel">Consultar Perfil</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">
                    
                    <div class="form-group">
                        <label for="txtIdentificacion">Identificación</label>
                        <input type="text" id="txtIdentificacion" class="form-control">
                    </div>
                </div>
                <div class="modal-footer">
                 <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cerrar</button>
                    <button type="button" class="btn btn-primary" onclick="consultarPerfil()">Buscar</button>
                </div>
            </div>
        </div>
    </div>

    <!-- Modal para mostrar perfil encontrado -->
    <div class="modal fade" id="modalPerfilEncontrado" tabindex="-1" role="dialog" aria-labelledby="modalPerfilEncontradoLabel">
        <div class="modal-dialog modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header bg-primary text-white">
                    <h5 class="modal-title text-white" id="modalPerfilEncontradoLabel">Perfil Encontrado</h5>

                </div>
                <div class="modal-body">
                    <div class="row">
                        <div class="col-md-12 text-center">
                            <i class="fas fa-user-circle fa-5x text-primary"></i>
                        </div>
                    </div>
                    <div class="row mt-3">
                        <div class="col-md-12">
                            <p class="mb-0"><strong>Tipo:</strong> <span id="spanTipo"></span></p>
                            <p class="mb-0"><strong>Identificación:</strong> <span id="spanIdentificacion"></span></p>
                            <p class="mb-0"><strong>Nombre:</strong> <span id="spanNombre"></span></p>
                        </div>
                    </div>
                    <div class="row mt-3">
                        <div class="col-md-12">
                            <p class="mb-0"><strong>Calificacion de Puntualidad</strong></p>
                            <div class="rating" id="spanPuntualidad"></div>
                        </div>
                    </div>
                    <div class="row mt-2">
                        <div class="col-md-12">
                            <p class="mb-0"><strong>Calificacion de Cumplimiento</strong></p>
                            <div class="rating" id="spanCumplimiento"></div>
                        </div>
                    </div>
                    <div class="row mt-2">
                        <div class="col-md-12">
                            <p class="mb-0"><strong>Calificacion de Conocimientos Profesionales</strong></p>
                            <div class="rating" id="spanConocimientos"></div>
                        </div>
                    </div>
                    <div class="row mt-2">
                        <div class="col-md-12">
                            <p class="mb-0"><strong>Calificacion de Recomendacion</strong></p>
                            <div class="rating" id="spanRecomendacion"></div>
                        </div>
                    </div>
                 
                    <asp:LinqDataSource runat="server" EntityTypeName="" ID="linqHistoricoCalificaciones" ContextTypeName="MiGente_Web.Data.migenteEntities" Select="new (fecha, Expr3, puntualidad, cumplimiento, conocimientos, comentario)" TableName="VCalificaciones" Where="identificacion == @identificacion">
                      
                    </asp:LinqDataSource>
                </div>
                <div class="modal-footer">
                   <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cerrar</button>
                </div>
            </div>
        </div>
    </div>

    <!-- Script para llamar al WebMethod y mostrar el modal -->
    <script>

        function consultarPerfil() {
            console.log("click");

            var identificacion = document.getElementById("txtIdentificacion").value;

            $.ajax({
                type: "POST",
                url: "CalificacionDePerfiles.aspx/ConsultarPerfilPorTipoIdentificacion",
                data: JSON.stringify({ identificacion: identificacion }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    // Verifica si se recibió algún dato válido en la respuesta
                    if (data && data.d !== null) {
                        console.log(data.d);
                        // Manipula los datos devueltos (data.d) y muestra el modal
                        mostrarModalPerfil(data.d);
                    } else {
                        // Muestra un mensaje de error si no se encontraron coincidencias
                        Swal.fire({
                            icon: 'warning',
                            title: 'No. de Identificacion no Encontrado',
                            text: 'Este perfil no cuenta con calificaciones previas.'
                        });
                    }
                },
                error: function (error) {
                    console.log(error);
                }
            });
        }

        function mostrarModalPerfil(datosPerfil) {
            $('#spanTipo').text(datosPerfil.tipo);
            $('#spanIdentificacion').text(datosPerfil.identificacion);
            $('#spanNombre').text(datosPerfil.nombre + " " + datosPerfil.Apellido);
            // Convertir calificaciones en estrellas y asignar al elemento
            $('#spanPuntualidad').html(convertirANumeroEstrellas(datosPerfil.puntualidad));
            $('#spanCumplimiento').html(convertirANumeroEstrellas(datosPerfil.cumplimiento));
            $('#spanConocimientos').html(convertirANumeroEstrellas(datosPerfil.conocimientos));
            $('#spanComentario').html(convertirANumeroEstrellas(datosPerfil.recomendacion));
            $('#modalConsultarPerfil').modal('hide');
            $('#modalPerfilEncontrado').modal('show');

          
        }
        function convertirANumeroEstrellas(numero) {
            var estrellasHtml = '';
            for (var i = 1; i <= 5; i++) {
                if (i <= numero) {
                    estrellasHtml += '<i class="fas fa-star text-warning"></i>';
                } else {
                    estrellasHtml += '<i class="far fa-star"></i>';
                }
            }
            return estrellasHtml;
        }
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

        var identificacion = "";
        var comentario = "";
        var nombre = "";
        function onRatingValueChanged1(s, e) {
            puntualidad = s.GetValue();
            // Aquí puedes hacer lo que quieras con el valor, como mostrar una alerta
        }
        function onRatingValueChanged2(s, e) {
            cumplimiento = s.GetValue();
            // Aquí puedes hacer lo que quieras con el valor, como mostrar una alerta
        }
        function onRatingValueChanged3(s, e) {
            conocimientos = s.GetValue();
            // Aquí puedes hacer lo que quieras con el valor, como mostrar una alerta
        }
        function onRatingValueChanged4(s, e) {
            recomendacion = s.GetValue();
            // Aquí puedes hacer lo que quieras con el valor, como mostrar una alerta
        }
        function onIdentificacionChanged(s, e) {
            identificacion = s.GetValue();
            console.log(identificacion);
        }
     
        function onNombreChanged(s, e) {
            nombre = s.GetValue();
            console.log(nombre);
        }

        function calificarPerfil() {
            var tipo = 1;

            console.log(tipo);

            console.log(identificacion);

            console.log(puntualidad);

            
            $.ajax({
                type: "POST",
                url: "CalificacionDePerfiles.aspx/Calificar",
                data: JSON.stringify({ tipo: tipo, identificacion: identificacion, nombre: nombre, puntualidad: puntualidad, cumplimiento: cumplimiento, conocimientos: conocimientos, recomendacion: recomendacion}),
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
                    // Recargar la página para limpiar los parámetros
                    location.reload();
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
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <asp:HiddenField ID="HiddenField1" runat="server" />
</asp:Content>

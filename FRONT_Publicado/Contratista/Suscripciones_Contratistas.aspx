<%@ Page Title="" Language="C#" MasterPageFile="~/Contratista/ContratistasM.Master" AutoEventWireup="true" CodeBehind="Suscripciones_Contratistas.aspx.cs" Inherits="MiGente_Web.Contratista.Suscripciones_Contratistas" %>

<%@ Register Assembly="DevExpress.Web.Bootstrap.v22.2, Version=22.2.5.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.Bootstrap" TagPrefix="dx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
     <head>

     <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">
    </head>

    <body>

<div class="container mt-5">
    <div class="row justify-content-center">
        <div class="col-md-6">
            <div class="card">
                <div class="card-header bg-primary text-white">
                    <h5 class="mb-0"  style="color:white">Gestión de Suscripción</h5>
                </div>
                <div class="card-body">
                        <div class="mb-3">
                            <dx:BootstrapTextBox Caption="Plan Actual" ReadOnly="true" ID="txtPlanActual" runat="server"></dx:BootstrapTextBox>
                        </div>
                        <div class="mb-3">
                          <dx:BootstrapTextBox Caption="Fecha de Inicio" ReadOnly="true" ID="txtFechaInicio" runat="server"></dx:BootstrapTextBox>

                        </div>
                        <div class="mb-3" id="proxPago">
                         <dx:BootstrapTextBox Caption="Proximo Pago" ReadOnly="true" ID="txtProximoPago" runat="server"></dx:BootstrapTextBox>

                        </div>
                     
                    <div class="mb-3 " style="margin-block:10px">

                    <dx:BootstrapButton  CssClasses-Control="col-md-4 col-12" ID="btnDetalles" runat="server" Visible="false" AutoPostBack="false" Text="Ver Detalles"></dx:BootstrapButton>
                    <dx:BootstrapButton  CssClasses-Control="col-md-5 col-12 bg-danger" ID="btnCancelar" runat="server" AutoPostBack="false" Text="Cancelar Suscripcion"></dx:BootstrapButton>
                      
                        </div>
                </div>
            </div>
        </div>
    </div>
</div>
        <div class="container mt-2">
            <h5>Historico de Facturacion</h5>
        </div>
        <div class="container mt-2">
       <dx:BootstrapGridView ID="gridPagos" runat="server" AutoGenerateColumns="False">
           <SettingsAdaptivity HideDataCellsAtWindowInnerWidth="Small"></SettingsAdaptivity>
    <Columns>
        <dx:BootstrapGridViewTextColumn FieldName="ventaID" Visible="false" ReadOnly="True" VisibleIndex="0"></dx:BootstrapGridViewTextColumn>
        <dx:BootstrapGridViewDateColumn FieldName="fecha" ReadOnly="True" VisibleIndex="1"></dx:BootstrapGridViewDateColumn>
        <dx:BootstrapGridViewTextColumn FieldName="planID" ReadOnly="True" Caption="Id Plan" VisibleIndex="2"></dx:BootstrapGridViewTextColumn>
        <dx:BootstrapGridViewTextColumn FieldName="precio" ReadOnly="True" VisibleIndex="3"></dx:BootstrapGridViewTextColumn>
        <dx:BootstrapGridViewTextColumn FieldName="card" Caption="Tarjeta" ReadOnly="True" VisibleIndex="4"></dx:BootstrapGridViewTextColumn>
    </Columns>

    <SettingsPager>
        <PageSizeItemSettings Visible="True" />
    </SettingsPager>
</dx:BootstrapGridView>

            
        </div>
<!-- Agrega los scripts de Bootstrap 5 y otros scripts necesarios -->
<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
<script src="https://code.jquery.com/jquery-3.6.4.min.js"></script>

<!-- Agrega tu script personalizado aquí -->
<script>
 
</script>

</body>
</asp:Content>

<%@ Page Title="" Language="C#" MasterPageFile="~/Empleador/dashboard.Master" AutoEventWireup="true" CodeBehind="Nomina.aspx.cs" Inherits="MiGente_Web.Empleador.Nomina" %>
<%@ Register Assembly="DevExpress.Web.Bootstrap.v22.2, Version=22.2.5.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.Bootstrap" TagPrefix="dx" %>


<%@ Register TagPrefix="dx" Namespace="DevExpress.Web" Assembly="DevExpress.Web.v22.2, Version=22.2.5.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2 style="color:white">Administra la Nomina de tu Gente</h2>
    <hr />
    <div style="background-color:white; padding-block:5px; padding-left:10px; ">

        <div class="container mt-5">
    <h2>Gestión de Nómina</h2>
    
    <!-- Tabs -->
    <ul class="nav nav-tabs mt-3" id="myTabs" role="tablist">
        <li class="nav-item" role="presentation">
            <a class="nav-link active" id="novedad-tab" data-bs-toggle="tab" href="#novedad" role="tab" aria-controls="novedad" aria-selected="true">Novedad</a>
        </li>
        <li class="nav-item" role="presentation">
            <a class="nav-link" id="historico-tab" data-bs-toggle="tab" href="#historico" role="tab" aria-controls="historico" aria-selected="false">Histórico</a>
        </li>
    </ul>
    
    <!-- Contenido de los tabs -->
    <div class="tab-content mt-3" id="myTabContent">
        <!-- Tab Novedad -->
        <div class="tab-pane fade show active" id="novedad" role="tabpanel" aria-labelledby="novedad-tab">
            <div class="row">
             
          <div class="container">
            <div class="row mt-3">
                  <div class="col-md-2">
                    <label for="txtFechaPago" class="form-label">Fecha del Pago:</label>
                    <dx:ASPxDateEdit ID="fechaPago" runat="server" CssClass="form-control"></dx:ASPxDateEdit>

                </div>
                <div class="col-md-3">
                    <label for="ddlEmpleado" class="form-label">Seleccionar Empleado:</label>
                    <asp:DropDownList ID="ddlEmpleado" runat="server" CssClass="form-select" DataSourceID="linqEmpleados" DataTextField="Nombre" DataValueField="empleadoID">

                    </asp:DropDownList>
                    <asp:LinqDataSource runat="server" EntityTypeName="" ID="linqEmpleados"  ContextTypeName="MiGente_Web.Data.migenteEntities" Select="new (empleadoID, identificacion, Nombre)" TableName="VEmpleados" Where="userID == @userID">
                        <WhereParameters>
                            <asp:ControlParameter ControlID="HiddenField1" PropertyName="Value" DefaultValue="0" Name="userID" Type="String"></asp:ControlParameter>
                        </WhereParameters>
                    </asp:LinqDataSource>
                </div>
              
                   <div class="col-md-2">
                    <label for="txtSalario" class="form-label">Salario:</label>
                   <dx:BootstrapTextBox ID="txtSalario" runat="server" NullText="0.00" onkeypress="return soloNumeros(event)" ></dx:BootstrapTextBox>
                </div>
                <div class="col-md-2">
                    <label for="txtDescuento" class="form-label">Descuento</label>
                    <asp:TextBox ID="txtDescuento" runat="server" CssClass="form-control"></asp:TextBox>
                </div>
            </div>
             
            <div class="row mt-3">
              
                <div class="col-md-4">
                    <label for="txtDescripcion" class="form-label">Descripcion del Descuento:</label>
                    <asp:TextBox ID="txtDescripcion" runat="server" CssClass="form-control"></asp:TextBox>
                </div>
                <div class="col-md-4">
                      <a style="color:white">.</a>
                   <br />
                  
                    <asp:Button ID="Button1" runat="server" Text="Agregar" CssClass="btn btn-primary"  />
                </div>
            </div>
           
            <div class="row mt-4">
                <div class="col-md-12">
                    <dx:BootstrapGridView ID="bootstrapGridView" runat="server" AutoGenerateColumns="False">
                        <SettingsAdaptivity>
                        </SettingsAdaptivity>
                        <Columns>
                            <dx:BootstrapGridViewTextColumn FieldName="Empleado" Caption="Empleado" />
                            <dx:BootstrapGridViewTextColumn FieldName="FechaPago" Caption="Fecha del Pago" />
                            <dx:BootstrapGridViewTextColumn FieldName="Monto" Caption="Monto" />
                            <dx:BootstrapGridViewCommandColumn VisibleIndex="3">
                                <CustomButtons>
                                    <dx:BootstrapGridViewCommandColumnCustomButton Text="Ver" CssClass="bg-success text-white"></dx:BootstrapGridViewCommandColumnCustomButton>
                                    <dx:BootstrapGridViewCommandColumnCustomButton Text="Anular" CssClass="bg-danger text-white"></dx:BootstrapGridViewCommandColumnCustomButton>
                                </CustomButtons>
                            </dx:BootstrapGridViewCommandColumn>
                        </Columns>
                    </dx:BootstrapGridView>
                </div>
            </div>
        </div>
            </div>
        </div>
        
        <!-- Tab Histórico -->
        <div class="tab-pane fade" id="historico" role="tabpanel" aria-labelledby="historico-tab">
            <div class="row">
                <!-- Código del contenido Histórico -->
            </div>
        </div>
    </div>
</div>



    </div>
    <asp:HiddenField ID="HiddenField1" runat="server" />
    <script>
        function soloNumeros(event) {
            var charCode = (event.which) ? event.which : event.keyCode;
            if (charCode > 31 && (charCode < 48 || charCode > 57)) {
                return false;
            }
            return true;
        }
    </script>
</asp:Content>

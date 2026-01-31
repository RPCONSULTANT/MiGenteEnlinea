<%@ Page Title="" Language="C#" MasterPageFile="~/Empleador/dashboard.Master" AutoEventWireup="true" CodeBehind="AdquirirPlanEmpleador.aspx.cs" Inherits="MiGente_Web.Empleador.AdquirirPlanEmpleador" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
 <section id="pricing" class="pricing-content section-padding">
     <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <div class="container">
            <div class="section-title text-center">
                <h2 style="color:white">Nuestros Planes</h2>
                <p style="color:white">Para acceder y disfrutar de los beneficios que te ofrece Mi Gente en Linea, debes adquirir uno de nuestros planes</p>
            </div>
            <div class="row text-center">
                <div class="col-lg-4 col-sm-6 col-xs-12 wow fadeInUp" data-wow-duration="1s" data-wow-delay="0.1s" data-wow-offset="0" style="visibility: visible; animation-duration: 1s; animation-delay: 0.1s; animation-name: fadeInUp;">
                    <div class="pricing_design">
                        <div class="single-pricing">
                            <div class="price-head">
                                <h4>Eres mi Gente</h4>
                                <h2>RD$495.00</h2>
                                <span>/Anual</span>
                            </div>
                            <ul>
                                <li><b>1</b> Administrador</li>
                                <li><b>1</b> Regisro de Empleado</li>
                                <li>Consultas Ilimitadas</li>
                                <li><b>12</b> Meses de Data Historica</li>

                            </ul>

                                                     <button runat="server"  id="Button1" onserverclick="btnPlan1_ServerClick" class="price_btn">Adquirir</button>

                        </div>
                    </div>
                </div>
                <!--- END COL -->
                <div class="col-lg-4 col-sm-6 col-xs-12 wow fadeInUp" data-wow-duration="1s" data-wow-delay="0.2s" data-wow-offset="0" style="visibility: visible; animation-duration: 1s; animation-delay: 0.2s; animation-name: fadeInUp;">
                    <div class="pricing_design">
                        <div class="single-pricing">
                            <div class="price-head">
                                <h4>Somos Mi Gente</h4>
                                <h2 runat="server" id="precio2" class="price">RD$1,695.00</h2>
                                <span>/Anual</span>
                            </div>
                            <ul>
                                <li><b>1</b> Administrador</li>
                                <li><b>1</b> Usuario de Consulta</li>
                                <li><b>5</b> Registros de Empleados</li>
                                <li>Consultas Ilimitadas</li>
                                <li><b>12</b> Meses de Data Historica</li>
                            </ul>
                            <div class="pricing-price">
                            </div>
                            <button runat="server"  id="btnPlan2" onserverclick="btnPlan2_ServerClick" class="price_btn">Adquirir</button>
                        </div>
                    </div>
                </div>
                <!--- END COL -->
                <div class="col-lg-4 col-sm-6 col-xs-12 wow fadeInUp" data-wow-duration="1s" data-wow-delay="0.3s" data-wow-offset="0" style="visibility: visible; animation-duration: 1s; animation-delay: 0.3s; animation-name: fadeInUp;">
                    <div class="pricing_design">
                        <div class="single-pricing">
                            <div class="price-head">
                                <h4>Mi Gente Somos Todos</h4>
                                <h2 class="price">RD$3,750.00</h2>
                                <span>/Anual</span>
                            </div>
                            <ul>
                                <li><b>1</b> Administrador</li>
                                <li><b>2</b> Usuarios de Consulta</li>
                                <li><b>15</b> Registros de Empleados</li>
                                <li>Consultas Ilimitadas</li>
                                <li><b>12</b> Meses de Data Historica</li>
                                <li><b>Nomina</b> Pre-diseñada para registro de TSS</li>
                            </ul>
                            <div class="pricing-price">
                            </div>
                                                        <button runat="server"  id="btnPlan3" onserverclick="btnPlan3_ServerClick" class="price_btn">Adquirir</button>

                        </div>
                    </div>
                </div>
                <!--- END COL -->
            </div>
            <!--- END ROW -->
        </div>
        <!--- END CONTAINER -->
    </section>
</asp:Content>

<%@ Page Title="" Language="C#" MasterPageFile="~/Landing/landing.Master" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="MiGente_Web.Landing.Index" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
          .bannerSoyTuGente {
            background-color: #00176B;
            color: white;
            padding: 20px;
            display: flex;
            align-items: center;
        }
        
        .bannerSoyTuGente-description {
            flex: 1;
            margin-right: 20px;
        }
        
        .bannerSoyTuGente-title {
            font-size: 24px;
            font-weight: bold;
            margin-bottom: 10px;
        }
        
        .bannerSoyTuGente-button {
            background-color: #ffffff;
            color: #00176B;
            border: none;
            padding: 10px 20px;
            font-size: 18px;
            font-weight: bold;
            cursor: pointer;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="banner">
        <div class="banner-content">
            <h1 class="headerText animate__animated animate__slideInLeft">Soluciones integrales  de auto gestion y construccion de referencias</h1>
            <button class="btn btn-primary btn-lg" style="padding: 15px;">Conoce mas sobre Mi Gente</button>
        </div>
    </div>
    <div class="container col col-lg-12 form-control" style="background-color: #00176B; width: auto; color: white; text-align: center;">
        <h6 class="animate__animated animate__heartBeat animate__infinite animate__slower" style="margin: 10px">La solucion para el manejo de tus procesos laborales</h6>
    </div>
    <div class="container">
        <div class="form-inline">
            <h1 class="headerText">Por que usar </h1>
            <h1 class="headerText" style="margin-left: 8px; font-weight: bold; color: #2460FC">Mi Gente?</h1>
        </div>
        <div class="form-inline align-content-center">
            <a class="link" href="../Landing/LandingPage.aspx">
                <div class="card text-center" style="width: 15rem; text-align: center; margin: 15px; transition: background-color 2s ease-out 100ms">
                    <div class="inline ">
                        <img class="center-block" src="../Images/workers.png" style="max-width: 70px;" alt="Card image cap" />
                    </div>
                    <div class="card-body">
                        <p class=" font-weight-bold" style="color: black; font-size: medium">Gestión de Trabajadores</p>
                        <p class="card-text">Lleva un control de tus colaboradores en todo momento.</p>
                    </div>
                </div>
            </a>
            <a class="link" href="#">
                <div class="card text-center" style="width: 15rem; text-align: center; margin: 15px;">
                    <div class="inline">
                        <img class="center-block" src="../Images/moneyIcon.png" style="max-width: 70px;" alt="Card image cap" />
                    </div>
                    <div class="card-body">
                        <p class=" font-weight-bold" style="color: black; font-size: medium">Pago de Nomina</p>
                        <p class="card-text">Gestiona tu nomina de una manera eficiente y rapida en la palma de tu mano</p>
                    </div>
                </div>
            </a>
            <a class="link" target="_blank" href="https://calculo.mt.gob.do/">
                <div class="card text-center" style="width: 15rem; text-align: center; margin: 15px;">
                    <div class="inline">
                        <img class="center-block" src="../Images/calculatorIcon.png" style="max-width: 70px;" alt="Card image cap" />
                    </div>
                    <div class="card-body">
                        <p class=" font-weight-bold" style="color: black; font-size: medium">Calculo de Prestaciones</p>
                        <p class="card-text">Realiza el calculo de las prestaciones de tu colaborador para generear recibo de descargo</p>
                    </div>
                </div>
            </a>
            <a class="link" href="#">
                <div class="card text-center" style="width: 15rem; text-align: center; margin: 15px;">
                    <div class="inline">
                        <img class="center-block" src="../Images/legalDocIcon.png" style="max-width: 70px;" alt="Card image cap" />
                    </div>
                    <div class="card-body">
                        <p class=" font-weight-bold" style="color: black; font-size: medium">Documentacion Legal</p>
                        <p class="card-text">Genera facilmente los documentos legales que necesitas en tu proceso de contratacion</p>
                    </div>
                </div>

            </a>
        </div>
    </div>
    <div class="container">
        <h1 class="headerText">Cuentas con un directorio de profesionales con su historial de comportamiento </h1>
        <hr />
        <h1 class="headerText" style="color: #2460FC; margin-top: -15px;">Calificacion de Perfiles</h1>

    </div>

    <div class="container profile-page">
        <div class="row">
            <div class="col-xl-6 col-lg-7 col-md-12">
                <div class="card profile-header">
                    <div class="body">
                        <div class="row">
                            <div class="col-lg-4 col-md-4 col-12">
                                <div class="profile-image float-md-right">
                                    <img src="https://bootdey.com/img/Content/avatar/avatar7.png" alt="">
                                </div>
                            </div>
                            <div class="col-lg-8 col-md-8 col-12">
                                <h4 class="m-t-0 m-b-0"><strong>Michael</strong> Deo</h4>
                                <span class="job_post">Ui UX Designer</span>
                                <p>795 Folsom Ave, Suite 600 San Francisco, CADGE 94107</p>
                                <div>
                                    <button class="btn btn-primary btn-round">Ver Perfil</button>
                                    <button class="btn btn-primary btn-round btn-simple">Enviar Mensaje</button>
                                </div>
                                <div class="divCalificacionCard">
                                    <a>Calificación</a>
                                    <span class="fa fa-star checked"></span>
                                    <span class="fa fa-star checked"></span>
                                    <span class="fa fa-star checked"></span>
                                    <span class="fa fa-star"></span>
                                    <span class="fa fa-star"></span>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>


            <div class="col-xl-6 col-lg-7 col-md-12">
                <div class="card profile-header">
                    <div class="body">
                        <div class="row">
                            <div class="col-lg-4 col-md-4 col-12">
                                <div class="profile-image float-md-right">
                                    <img src="https://bootdey.com/img/Content/avatar/avatar2.png" alt="">
                                </div>
                            </div>
                            <div class="col-lg-8 col-md-8 col-12">
                                <h4 class="m-t-0 m-b-0"><strong>Michael</strong> Deo</h4>
                                <span class="job_post">Ui UX Designer</span>
                                <p>795 Folsom Ave, Suite 600 San Francisco, CADGE 94107</p>
                                <div>
                                    <button class="btn btn-primary btn-round">Ver Perfil</button>
                                    <button class="btn btn-primary btn-round btn-simple">Enviar Mensaje</button>
                                </div>
                                <div class="divCalificacionCard">
                                    <a>Calificación</a>
                                    <span class="fa fa-star checked"></span>
                                    <span class="fa fa-star checked"></span>
                                    <span class="fa fa-star"></span>
                                    <span class="fa fa-star"></span>
                                    <span class="fa fa-star"></span>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="row">
            <div class="col-xl-6 col-lg-7 col-md-12">
                <div class="card profile-header">
                    <div class="body">
                        <div class="row">
                            <div class="col-lg-4 col-md-4 col-12">
                                <div class="profile-image float-md-right">
                                    <img src="https://bootdey.com/img/Content/avatar/avatar3.png" alt="">
                                </div>
                            </div>
                            <div class="col-lg-8 col-md-8 col-12">
                                <h4 class="m-t-0 m-b-0"><strong>Michael</strong> Deo</h4>
                                <span class="job_post">Ui UX Designer</span>
                                <p>795 Folsom Ave, Suite 600 San Francisco, CADGE 94107</p>
                                <div>
                                    <button class="btn btn-primary btn-round">Ver Perfil</button>
                                    <button class="btn btn-primary btn-round btn-simple">Enviar Mensaje</button>
                                </div>
                                <div class="divCalificacionCard">
                                    <a>Calificación</a>
                                    <span class="fa fa-star checked"></span>
                                    <span class="fa fa-star checked"></span>
                                    <span class="fa fa-star checked"></span>
                                    <span class="fa fa-star checked"></span>
                                    <span class="fa fa-star"></span>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>


            <div class="col-xl-6 col-lg-7 col-md-12">
                <div class="card profile-header">
                    <div class="body">
                        <div class="row">
                            <div class="col-lg-4 col-md-4 col-12">
                                <div class="profile-image float-md-right">
                                    <img src="https://bootdey.com/img/Content/avatar/avatar6.png" alt="" />
                                </div>
                            </div>
                            <div class="col-lg-8 col-md-8 col-12">
                                <h4 class="m-t-0 m-b-0"><strong>Michael</strong> Deo</h4>
                                <span class="job_post">Ui UX Designer</span>
                                <p>795 Folsom Ave, Suite 600 San Francisco, CADGE 94107</p>
                                <div>
                                    <button class="btn btn-primary btn-round">Ver Perfil</button>
                                    <button class="btn btn-primary btn-round btn-simple">Enviar Mensaje</button>
                                </div>
                                <div class="divCalificacionCard">
                                    <a>Calificación</a>
                                    <span class="fa fa-star checked"></span>
                                    <span class="fa fa-star"></span>
                                    <span class="fa fa-star"></span>
                                    <span class="fa fa-star"></span>
                                    <span class="fa fa-star"></span>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <hr />
    <section id="pricing" class="pricing-content section-padding">
        <div class="container">
            <div class="section-title text-center">
                <h2>Nuestros Planes</h2>
                <p>Escoge el paquete de suscripcion que mejor se acomode a tus necesidades y forma parte de Mi Gente En Linea</p>
            </div>

            <div class=" row bannerSoyTuGente mb-5">
        <div class="bannerSoyTuGente-description">
            <div class="bannerSoyTuGente-title">Mi Gente, Soy Yo</div>
            <p class="bannerSoyTuGente-text">Adquiere una membresía que te permitirá publicar tu perfil profesional para captar más clientes.</p>
            <p class="bannerSoyTuGente-text">Precio: RD$ 495.00 Anual</p>
        </div>
        <button class="bannerSoyTuGente-button">Adquirir</button>
    </div>


            <div class="row text-center">
                <div class="col-lg-4 col-sm-6 col-xs-12 wow fadeInUp" data-wow-duration="1s" data-wow-delay="0.1s" data-wow-offset="0" style="visibility: visible; animation-duration: 1s; animation-delay: 0.1s; animation-name: fadeInUp;">
                    <div class="pricing_design">
                        <div class="single-pricing">
                            <div class="price-head">
                                <h2>Mi Gente, Soy Yo</h2>
                                <h1>RD$495.00</h1>
                                <span>/Anual</span>
                            </div>
                            <ul>
                                <li><b>1</b> Administrador</li>
                                <li><b>1</b> Regisro de Empleado</li>
                                <li>Consultas Ilimitadas</li>
                                <li><b>12</b> Meses de Data Historica</li>

                            </ul>

                            <a href="#" class="price_btn">Adquirir</a>
                        </div>
                    </div>
                </div>
                <!--- END COL -->
                <div class="col-lg-4 col-sm-6 col-xs-12 wow fadeInUp" data-wow-duration="1s" data-wow-delay="0.2s" data-wow-offset="0" style="visibility: visible; animation-duration: 1s; animation-delay: 0.2s; animation-name: fadeInUp;">
                    <div class="pricing_design">
                        <div class="single-pricing">
                            <div class="price-head">
                                <h2>MI Gente en Familia</h2>
                                <h1 class="price">RD$1,695.00</h1>
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
                            <a href="#" class="price_btn">Adquirir</a>
                        </div>
                    </div>
                </div>
                <!--- END COL -->
                <div class="col-lg-4 col-sm-6 col-xs-12 wow fadeInUp" data-wow-duration="1s" data-wow-delay="0.3s" data-wow-offset="0" style="visibility: visible; animation-duration: 1s; animation-delay: 0.3s; animation-name: fadeInUp;">
                    <div class="pricing_design">
                        <div class="single-pricing">
                            <div class="price-head">
                                <h2>Mi Gente Somos Todos</h2>
                                <h1 class="price">RD$3,750.00</h1>
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
                            <a href="#" class="price_btn">Adquirir</a>
                        </div>
                    </div>
                </div>
                <!--- END COL -->
            </div>
            <!--- END ROW -->
        </div>
        <!--- END CONTAINER -->
    </section>
    <br />
    <hr />
</asp:Content>

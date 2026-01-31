<%@ Page Title="" Language="C#" MasterPageFile="~/Empleador/FAQ_Master.Master" AutoEventWireup="true" CodeBehind="FAQ.aspx.cs" Inherits="MiGente_Web.Empleador.FAQ" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <style>
        body {
            margin-top: 0px;
        }

        .section_padding_130 {
            padding-top: 130px;
            padding-bottom: 130px;
        }

        .faq_area {
            position: relative;
            z-index: 1;
            background-color: #f5f5ff;
        }

        .faq-accordian {
            position: relative;
            z-index: 1;
        }

            .faq-accordian .card {
                position: relative;
                z-index: 1;
                margin-bottom: 1.5rem;
            }

                .faq-accordian .card:last-child {
                    margin-bottom: 0;
                }

                .faq-accordian .card .card-header {
                    background-color: #ffffff;
                    padding: 0;
                    border-bottom-color: #ebebeb;
                }

                    .faq-accordian .card .card-header h6 {
                        cursor: pointer;
                        padding: 1.75rem 2rem;
                        color: #3f43fd;
                        display: -webkit-box;
                        display: -ms-flexbox;
                        display: flex;
                        -webkit-box-align: center;
                        -ms-flex-align: center;
                        -ms-grid-row-align: center;
                        align-items: center;
                        -webkit-box-pack: justify;
                        -ms-flex-pack: justify;
                        justify-content: space-between;
                    }

                        .faq-accordian .card .card-header h6 span {
                            font-size: 1.5rem;
                        }

                        .faq-accordian .card .card-header h6.collapsed {
                            color: #070a57;
                        }

                            .faq-accordian .card .card-header h6.collapsed span {
                                -webkit-transform: rotate(-180deg);
                                transform: rotate(-180deg);
                            }

                .faq-accordian .card .card-body {
                    padding: 1.75rem 2rem;
                }

                    .faq-accordian .card .card-body p:last-child {
                        margin-bottom: 0;
                    }

        @media only screen and (max-width: 575px) {
            .support-button p {
                font-size: 14px;
            }
        }

        .support-button i {
            color: #3f43fd;
            font-size: 1.25rem;
        }

        @media only screen and (max-width: 575px) {
            .support-button i {
                font-size: 1rem;
            }
        }

        .support-button a {
            text-transform: capitalize;
            color: #2ecc71;
        }

        @media only screen and (max-width: 575px) {
            .support-button a {
                font-size: 13px;
            }
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
      <div class="faq_area section_padding_130" id="faq">
        <div class="container">
            <div class="row justify-content-center">
                <div class="col-12 col-sm-8 col-lg-6">
                    <!-- Section Heading-->
                    <div class="section_heading text-center wow fadeInUp" data-wow-delay="0.2s" style="visibility: visible; animation-delay: 0.2s; animation-name: fadeInUp;">
                        <h3><span>Preguntas </span>Frecuentes de la Comunidad</h3>
                        <p>Aqui encontraras todo lo que necesitas para aclarar tus dudas sobre Mi Gente en linea</p>
                        <div class="line"></div>
                    </div>
                </div>
            </div>
            <div class="row justify-content-center">
                <!-- FAQ Area-->
                <div class="col-12 col-sm-10 col-lg-8">
                    <div class="accordion faq-accordian" id="faqAccordion">
                        <div class="card border-0 wow fadeInUp" data-wow-delay="0.2s" style="visibility: visible; animation-delay: 0.2s; animation-name: fadeInUp;">
                            <div class="card-header" id="headingOne">
                                <h6 class="mb-0 collapsed" data-toggle="collapse" data-target="#collapseOne" aria-expanded="true" aria-controls="collapseOne">1. ¿Cómo inscribo a mis empleados en el sistema?<span class="lni-chevron-up"></span></h6>
                            </div>
                            <div class="collapse" id="collapseOne" aria-labelledby="headingOne" data-parent="#faqAccordion">
                                <div class="card-body">
                                    <p>Para inscribir a tus empleados en nuestro sistema, simplemente sigue estos pasos:</p>
                                    <ul>
                                        <li>Accede a tu cuenta en nuestra plataforma.</li>
                                        <li>Ingresa los datos de tus empleados, y ¡listo! No tendrás que volver a completar su información nunca más.</li>
                                    </ul>
                                </div>
                            </div>
                        </div>
                        <div class="card border-0 wow fadeInUp" data-wow-delay="0.3s" style="visibility: visible; animation-delay: 0.3s; animation-name: fadeInUp;">
                            <div class="card-header" id="headingTwo">
                                <h6 class="mb-0 collapsed" data-toggle="collapse" data-target="#collapseTwo" aria-expanded="true" aria-controls="collapseTwo">2. ¿Qué ventajas ofrece la inscripción de empleados en la nube?<span class="lni-chevron-up"></span></h6>
                            </div>
                            <div class="collapse" id="collapseTwo" aria-labelledby="headingTwo" data-parent="#faqAccordion">
                                <div class="card-body">
                                    <p>Al inscribir a tus empleados en la nube, podrás:</p>
                                    <ul>
                                        <li>Calcular fácilmente los días laborados.</li>
                                        <li>Gestionar las horas extras.</li>
                                        <li>Generar nóminas regulares.</li>
                                        <li>Controlar derechos adquiridos.</li>
                                        <li>Manejar prestaciones laborales.</li>
                                        <li>Emitir recibos de pago.</li>
                                    </ul>
                                </div>
                            </div>
                        </div>
                        <div class="card border-0 wow fadeInUp" data-wow-delay="0.4s" style="visibility: visible; animation-delay: 0.4s; animation-name: fadeInUp;">
                            <div class="card-header" id="headingThree">
                                <h6 class="mb-0 collapsed" data-toggle="collapse" data-target="#collapseThree" aria-expanded="true" aria-controls="collapseThree">3. ¿Cómo se realiza un contrato de trabajo?<span class="lni-chevron-up"></span></h6>
                            </div>
                            <div class="collapse" id="collapseThree" aria-labelledby="headingThree" data-parent="#faqAccordion">
                                <div class="card-body">
                                    <p>El contrato de trabajo se realiza de acuerdo con las leyes laborales vigentes. En la República Dominicana, se define como un acuerdo mediante el cual una persona se compromete a prestar un servicio personal a otra a cambio de una remuneración. Puede ser por tiempo indefinido, por cierto tiempo o para una obra o servicio determinado.</p>
                                </div>
                            </div>
                        </div>
                        <div class="card border-0 wow fadeInUp" data-wow-delay="0.5s" style="visibility: visible; animation-delay: 0.5s; animation-name: fadeInUp;">
                            <div class="card-header" id="headingFour">
                                <h6 class="mb-0 collapsed" data-toggle="collapse" data-target="#collapseFour" aria-expanded="true" aria-controls="collapseFour">4. ¿Cómo calculo los días laborados y las horas extras de mis empleados?<span class="lni-chevron-up"></span></h6>
                            </div>
                            <div class="collapse" id="collapseFour" aria-labelledby="headingFour" data-parent="#faqAccordion">
                                <div class="card-body">
                                    <p>El cálculo de los días laborados y las horas extras se basa en la jornada de trabajo de cada empleado. Esto implica contabilizar las horas que el trabajador dedica a sus tareas y puede ser diario, semanal, mensual o anual. Este cálculo es fundamental para determinar las condiciones de trabajo y las horas extraordinarias.</p>
                                </div>
                            </div>
                        </div>
                        <div class="card border-0 wow fadeInUp" data-wow-delay="0.6s" style="visibility: visible; animation-delay: 0.6s; animation-name: fadeInUp;">
                            <div class="card-header" id="headingFive">
                                <h6 class="mb-0 collapsed" data-toggle="collapse" data-target="#collapseFive" aria-expanded="true" aria-controls="collapseFive">5. ¿Cómo puedo realizar la nómina regular de mis empleados?<span class="lni-chevron-up"></span></h6>
                            </div>
                            <div class="collapse" id="collapseFive" aria-labelledby="headingFive" data-parent="#faqAccordion">
                                <div class="card-body">
                                    <p>La nómina regular es un documento legal que refleja la cantidad de dinero que un empleado recibe por su trabajo. Incluye el salario bruto y los descuentos obligatorios por ley, así como los descuentos circunstanciales. El salario neto es la cantidad que el empleado recibe después de aplicar estos descuentos.</p>
                                </div>
                            </div>
                        </div>
                        <div class="card border-0 wow fadeInUp" data-wow-delay="0.7s" style="visibility: visible; animation-delay: 0.7s; animation-name: fadeInUp;">
                            <div class="card-header" id="headingSix">
                                <h6 class="mb-0 collapsed" data-toggle="collapse" data-target="#collapseSix" aria-expanded="true" aria-controls="collapseSix">6. ¿Cómo se calcula la Regalía Pascual (Salario de Navidad)?<span class="lni-chevron-up"></span></h6>
                            </div>
                            <div class="collapse" id="collapseSix" aria-labelledby="headingSix" data-parent="#faqAccordion">
                                <div class="card-body">
                                    <p>La Regalía Pascual, o salario de navidad, es la duodécima parte del salario ordinario devengado por el trabajador en el año calendario. Se calcula dividiendo entre doce el total de salarios ordinarios devengados, excluyendo las horas extras y la participación en los beneficios de la empresa. En ningún caso puede ser mayor que cinco salarios mínimos legalmente establecidos.</p>
                                </div>
                            </div>
                        </div>
                        <div class="card border-0 wow fadeInUp" data-wow-delay="0.8s" style="visibility: visible; animation-delay: 0.8s; animation-name: fadeInUp;">
                            <div class="card-header" id="headingSeven">
                                <h6 class="mb-0 collapsed" data-toggle="collapse" data-target="#collapseSeven" aria-expanded="true" aria-controls="collapseSeven">7. ¿Cómo genero un recibo de pago para mis empleados?<span class="lni-chevron-up"></span></h6>
                            </div>
                            <div class="collapse" id="collapseSeven" aria-labelledby="headingSeven" data-parent="#faqAccordion">
                                <div class="card-body">
                                    <p>Generar un recibo de pago es fundamental para registrar las transacciones entre empleador y empleado. Este documento sirve como comprobante de pago por los servicios prestados y evita futuros reclamos. Debes incluir detalles como la fecha, el monto y la firma de ambas partes.</p>
                                </div>
                            </div>
                        </div>
                        <div class="card border-0 wow fadeInUp" data-wow-delay="0.9s" style="visibility: visible; animation-delay: 0.9s; animation-name: fadeInUp;">
                            <div class="card-header" id="headingEight">
                                <h6 class="mb-0 collapsed" data-toggle="collapse" data-target="#collapseEight" aria-expanded="true" aria-controls="collapseEight">8. ¿Cómo se calculan los Derechos Adquiridos de los empleados?<span class="lni-chevron-up"></span></h6>
                            </div>
                            <div class="collapse" id="collapseEight" aria-labelledby="headingEight" data-parent="#faqAccordion">
                                <div class="card-body">
                                    <p>Los Derechos Adquiridos son derechos que se generan desde el inicio de la relación laboral y deben ser reconocidos por el empleador, independientemente de cómo termine la relación laboral. Estos derechos incluyen el salario, las vacaciones y el salario de navidad.</p>
                                </div>
                            </div>
                        </div>
                        <div class="card border-0 wow fadeInUp" data-wow-delay="1.0s" style="visibility: visible; animation-delay: 1.0s; animation-name: fadeInUp;">
                            <div class="card-header" id="headingNine">
                                <h6 class="mb-0 collapsed" data-toggle="collapse" data-target="#collapseNine" aria-expanded="true" aria-controls="collapseNine">9. ¿Qué son las Prestaciones Laborales y cómo se calculan?<span class="lni-chevron-up"></span></h6>
                            </div>
                            <div class="collapse" id="collapseNine" aria-labelledby="headingNine" data-parent="#faqAccordion">
                                <div class="card-body">
                                    <p>Las prestaciones laborales son derechos que corresponden a los empleados según la forma en que termine su contrato laboral (desahucio, despido, dimisión o incapacidad/muerte). Los cálculos varían según la situación, y pueden incluir el pago de cesantía en caso de desahucio o prestaciones acumuladas hasta el momento de la terminación del contrato.</p>
                                </div>
                            </div>
                        </div>
                        <div class="card border-0 wow fadeInUp" data-wow-delay="1.1s" style="visibility: visible; animation-delay: 1.1s; animation-name: fadeInUp;">
                            <div class="card-header" id="headingTen">
                                <h6 class="mb-0 collapsed" data-toggle="collapse" data-target="#collapseTen" aria-expanded="true" aria-controls="collapseTen">10. ¿Por qué necesito un Recibo de Descargo al finalizar una relación laboral?<span class="lni-chevron-up"></span></h6>
                            </div>
                            <div class="collapse" id="collapseTen" aria-labelledby="headingTen" data-parent="#faqAccordion">
                                <div class="card-body">
                                    <p>Un Recibo de Descargo es esencial al concluir una relación laboral. En este documento, se detallan la fecha de ingreso y salida del empleado, su cargo y los montos a liquidar. Debe ser emitido por el empleador y firmado por ambas partes para registrar oficialmente la extinción del contrato laboral.</p>
                                </div>
                            </div>
                        </div>
                    </div>
                    <!-- Support Button-->
                    <div class="support-button text-center d-flex align-items-center justify-content-center mt-4 wow fadeInUp" data-wow-delay="1.2s" style="visibility: visible; animation-delay: 1.2s; animation-name: fadeInUp;">
                        <i class="lni-emoji-sad"></i>
                        <p class="mb-0 px-2">¿No encuentras respuestas a tus preguntas?</p>
                        <a href="#">Contáctanos</a>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

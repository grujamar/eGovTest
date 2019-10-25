<%@ Page Language="C#" AutoEventWireup="true" CodeFile="GreskaBaza.aspx.cs" Inherits="GreskaBaza" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <!--#include virtual="~/elements/head.inc"-->
    <title>Greška-konekcija sa bazom</title>
</head>
<body>
    <form id="form1" runat="server">
        <!--header start-->
        <header class="py-3" style="background-image: linear-gradient(to right, rgba(220, 220, 220,0.3), rgba(220, 220, 220,0.9))">
            <div class="container">
                <nav class="navbar navbar-expand-md navbar-light px-0">
                    <!--logo start-->
                    <div class="navbar-container" id="navbar-container">
      
                    </div><!--logo end-->
                    <!--header navigation start-->
			        <div class="collapse navbar-collapse" id="main-menu">
				        <article class="navbar-nav ml-auto mt-2 px-lg-5">
				        </article>                        
			        </div><!--header navigation end-->
                </nav>
            </div>
        </header><!--header end-->
        <!--main start-->
        <main>
            <!--lead-section start-->
            <section class="lead-section my-4">
                <div class="container">
                    <asp:Label id="lblstranicanaziv" runat="server" CssClass="page-name">
                        Greška prilikom konekcije sa bazom podataka. Proverite konekciju ili pokušajte kasnije!
                    </asp:Label>
                </div>
            </section><!--lead-section end-->
            <!--button-section start-->
            <section>
                <div class="container">
                    <div class="row" runat="server">
                        <!--div back start-->
                        <div class="col-12 col-lg-2 mb-1 mb-md-4">
                            <asp:Button ID="btnBack" runat="server" Text="Nazad" CssClass="btn btn-secondary" OnClick="btnBack_Click"/>
                        </div>
                        <div class="col-12 col-lg-5 mb-3 mb-md-4">     
                        </div>
                        <div class="col-12 col-lg-5 mb-3 mb-lg-0">
                        </div><!--div back end-->
                    </div>
                </div>
            </section><!--button-section end-->
        </main>
    </form>
</body>
</html>
